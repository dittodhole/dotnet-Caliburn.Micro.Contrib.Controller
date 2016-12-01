﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.LibLog;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ViewModel
{
  [PublicAPI]
  public class ScreenInterceptor : IScreenInterceptor
  {
    public static BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    static ScreenInterceptor()
    {
      var locateTypeForModelType = ViewLocator.LocateTypeForModelType;
      ViewLocator.LocateTypeForModelType = (modelType,
                                            displayLocation,
                                            context) =>
                                           {
                                             if (ProxyUtil.IsProxyType(modelType))
                                             {
                                               modelType = modelType.BaseType;
                                             }

                                             var viewType = locateTypeForModelType.Invoke(modelType,
                                                                                          displayLocation,
                                                                                          context);

                                             return viewType;
                                           };
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which has no parameters.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which cannot be found on <see cref="ScreenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which is not declared as <see langword="virtual" /> or <see langword="abstract" /> on <see cref="ScreenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="screenType" /> is an interface.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="screenType" /> is <see langword="sealed" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="screenType" /> does not implement <see cref="IScreen" />.</exception>
    public ScreenInterceptor([NotNull] IController controller,
                             [NotNull] Type screenType)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }
      this.Controller = controller;
      this.ScreenType = screenType;

      this.ScreenMethodMapping = this.CreateScreenMethodMapping(this.Controller);
      this.ScreenType.CheckTypeForRealScreenType();
    }

    [NotNull]
    private IController Controller { get; }

    [NotNull]
    private Type ScreenType { get; }

    [NotNull]
    private IDictionary<string, ICollection<ControllerMethodInvocation>> ScreenMethodMapping { get; }

    /// <exception cref="ArgumentNullException"><paramref name="invocation" /> is <see langword="null" /></exception>
    public virtual void Intercept(IInvocation invocation)
    {
      if (invocation == null)
      {
        throw new ArgumentNullException(nameof(invocation));
      }

      var screenMethod = invocation.Method;
      var screenMethodName = screenMethod.Name;

      ICollection<ControllerMethodInvocation> controllerMethodInvocations;
      if (this.ScreenMethodMapping.TryGetValue(screenMethodName,
                                               out controllerMethodInvocations))
      {
        controllerMethodInvocations = controllerMethodInvocations.Where(arg =>
                                                                        {
                                                                          bool result;

                                                                          var controllerMethodInfo = arg.ControllerMethodInfo;
                                                                          if (controllerMethodInfo == null)
                                                                          {
                                                                            result = false;
                                                                          }
                                                                          else
                                                                          {
                                                                            var controllerMethodParameterTypes = controllerMethodInfo.GetParameters()
                                                                                                                                     .Skip(1)
                                                                                                                                     .Select(parameterInfo => parameterInfo.ParameterType)
                                                                                                                                     .ToArray();
                                                                            result = screenMethod.DoesMatch(screenMethodName,
                                                                                                            controllerMethodInfo.ReturnType,
                                                                                                            controllerMethodParameterTypes);
                                                                          }

                                                                          return result;
                                                                        })
                                                                 .ToArray();
      }
      else
      {
        controllerMethodInvocations = new ControllerMethodInvocation[0];
      }

      var callBase = controllerMethodInvocations.Any(arg => arg.CallBase);
      if (callBase)
      {
        LogTo.Debug($"Calling {this.ScreenType}.{screenMethodName}");
        invocation.Proceed();
      }
      else
      {
        LogTo.Debug($"Skipping {this.ScreenType}.{screenMethodName}.");
      }

      foreach (var controllerMethodInvocation in controllerMethodInvocations)
      {
        var controllerMethodInfo = controllerMethodInvocation.ControllerMethodInfo;
        if (controllerMethodInfo != null)
        {
          var screenMethodParameters = invocation.Arguments;

          var controllerMethodParameters = new object[screenMethodParameters.Count() + 1];
          controllerMethodParameters[0] = invocation.Proxy;
          Array.Copy(screenMethodParameters,
                     0,
                     controllerMethodParameters,
                     1,
                     screenMethodParameters.Count());

          var returnValue = controllerMethodInfo.Invoke(this.Controller,
                                                        controllerMethodParameters);
          if (!callBase)
          {
            invocation.ReturnValue = returnValue;
          }
        }
      }
    }

    public virtual IScreen CreateProxiedScreen()
    {
      var additionalInterfacesToProxy = this.ScreenMethodMapping.Values.SelectMany(value => value)
                                            .Select(arg => arg.InjectInterfaceDefinition)
                                            .Where(arg => arg != null)
                                            .Where(arg => arg.IsInterface)
                                            .ToArray();
      var proxyGenerationOptions = new ProxyGenerationOptions();
      var proxyGenerator = new ProxyGenerator();
      var proxy = proxyGenerator.CreateClassProxy(this.ScreenType,
                                                  additionalInterfacesToProxy,
                                                  proxyGenerationOptions,
                                                  this);
      var screen = (IScreen) proxy;

      if (screen is IHandle)
      {
        var eventAggregator = IoC.Get<IEventAggregator>();
        eventAggregator.Subscribe(screen);
      }

      return screen;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which has no parameters.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which cannot be found on <see cref="ScreenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which is not declared as <see langword="virtual" /> or <see langword="abstract" /> on <see cref="ScreenType" />.</exception>
    [NotNull]
    public virtual IDictionary<string, ICollection<ControllerMethodInvocation>> CreateScreenMethodMapping([NotNull] IController controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var result = new Dictionary<string, ICollection<ControllerMethodInvocation>>();

      var controllerType = controller.GetType();
      var controllerMethodInfos = controllerType.GetMethods(ScreenInterceptor.DefaultBindingFlags);
      foreach (var controllerMethodInfo in controllerMethodInfos)
      {
        var screenMethodLinkAttributes = controllerMethodInfo.GetAttributes<ScreenMethodLinkAttribute>(true)
                                                             .ToArray();
        if (!screenMethodLinkAttributes.Any())
        {
          continue;
        }

        var controllerMethodParameterInfos = controllerMethodInfo.GetParameters();
        var screenParameter = controllerMethodParameterInfos.FirstOrDefault();
        if (screenParameter == null)
        {
          throw new InvalidOperationException($"{controllerType} has a {nameof(ScreenMethodLinkAttribute)} defined on {controllerMethodInfo}, which has no parameters.");
        }
        if (!screenParameter.ParameterType.IsAssignableFrom(this.ScreenType))
        {
          continue;
        }

        var screenMethodParameterInfos = controllerMethodParameterInfos.Skip(1)
                                                                       .ToArray();
        var screenMethodParameterTypes = screenMethodParameterInfos.Select(arg => arg.ParameterType)
                                                                   .ToArray();

        foreach (var screenMethodLinkAttribute in screenMethodLinkAttributes)
        {
          var screenMethodName = screenMethodLinkAttribute.MethodName ?? controllerMethodInfo.Name;
          var injectInterfaceDefinition = screenMethodLinkAttribute.InjectInterfaceDefinition;

          var screenMethodInfo = this.ScreenType.GetMethod(screenMethodName,
                                                           ScreenInterceptor.DefaultBindingFlags,
                                                           controllerMethodInfo.ReturnType,
                                                           screenMethodParameterTypes);
          if (screenMethodInfo == null)
          {
            if (injectInterfaceDefinition != null)
            {
              screenMethodInfo = injectInterfaceDefinition.GetMethod(screenMethodName,
                                                                     ScreenInterceptor.DefaultBindingFlags,
                                                                     controllerMethodInfo.ReturnType,
                                                                     screenMethodParameterTypes);
            }
          }
          else
          {
            injectInterfaceDefinition = null;
          }

          if (screenMethodInfo == null)
          {
            throw new InvalidOperationException($"{controllerType} has a {nameof(ScreenMethodLinkAttribute)} defined on {controllerMethodInfo}, but the corresponding method does not exist on {this.ScreenType}. (hint: try setting {nameof(ScreenMethodLinkAttribute.InjectInterfaceDefinition)})");
          }
          if (!screenMethodInfo.IsVirtual)
          {
            throw new InvalidOperationException($"{controllerType} has a {nameof(ScreenMethodLinkAttribute)} defined on {controllerMethodInfo}, but the corresponding method is not declared as virtual on {this.ScreenType}.");
          }

          ICollection<ControllerMethodInvocation> controllerMethodInvocations;
          if (!result.TryGetValue(screenMethodName,
                                  out controllerMethodInvocations))
          {
            result[screenMethodName] = controllerMethodInvocations = new List<ControllerMethodInvocation>();
          }

          var controllerMethodInvocation = new ControllerMethodInvocation(controllerMethodInfo)
                                           {
                                             InjectInterfaceDefinition = injectInterfaceDefinition,
                                             CallBase = screenMethodLinkAttribute.CallBase
                                           };
          controllerMethodInvocations.Add(controllerMethodInvocation);
        }
      }

      return result;
    }

    public struct ControllerMethodInvocation
    {
      /// <exception cref="ArgumentNullException"><paramref name="controllerMethodInfo" /> is <see langword="null" /></exception>
      public ControllerMethodInvocation([NotNull] MethodInfo controllerMethodInfo)
        : this()
      {
        if (controllerMethodInfo == null)
        {
          throw new ArgumentNullException(nameof(controllerMethodInfo));
        }
        this.ControllerMethodInfo = controllerMethodInfo;
        this.CallBase = true;
      }

      [CanBeNull]
      public MethodInfo ControllerMethodInfo { get; }

      public bool CallBase { get; set; }

      [CanBeNull]
      public Type InjectInterfaceDefinition { get; set; }
    }
  }
}
