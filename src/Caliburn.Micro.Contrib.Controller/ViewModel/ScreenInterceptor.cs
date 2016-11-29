using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.LibLog;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ViewModel
{
  public class ScreenInterceptor : IInterceptor
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
    /// <exception cref="InvalidOperationException">The <paramref name="screenType" /> is an interface.</exception>
    /// <exception cref="InvalidOperationException">The <paramref name="screenType" /> does not implement <see cref="IScreen" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined, which has a <see langword="null" /> destination on <paramref name="screenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined, which has not declared as <see langword="virtual" /> on <paramref name="screenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined, which has an invalid injection defined in <see cref="ScreenMethodLinkAttribute.InjectInterfaceDefinition" />.</exception>
    public ScreenInterceptor([NotNull] ControllerBase controller,
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

      this.ScreenType.CheckTypeForRealScreenType();
      this.ScreenMethodMapping = this.CreateScreenMethodMapping(this.Controller);
    }

    [NotNull]
    private ControllerBase Controller { get; }

    [NotNull]
    private Type ScreenType { get; }

    [NotNull]
    protected IDictionary<string, ControllerMethodInvocation[]> ScreenMethodMapping { get; }

    /// <exception cref="ArgumentNullException"><paramref name="invocation" /> is <see langword="null" /></exception>
    public virtual void Intercept([NotNull] IInvocation invocation)
    {
      if (invocation == null)
      {
        throw new ArgumentNullException(nameof(invocation));
      }

      var screenMethodName = invocation.Method.Name;

      ControllerMethodInvocation[] controllerMethodInvocations;
      if (this.ScreenMethodMapping.TryGetValue(screenMethodName,
                                               out controllerMethodInvocations))
      {
        var screenMethodInfo = invocation.Method;
        var screenMethodParameterInfos = screenMethodInfo.GetParameters()
                                                         .Select(parameterInfo => parameterInfo.ParameterType)
                                                         .ToArray();

        controllerMethodInvocations = controllerMethodInvocations.Where(arg => this.AreMethodsIntertwined(arg.ControllerMethodInfo,
                                                                                                          screenMethodInfo,
                                                                                                          screenMethodParameterInfos))
                                                                 .ToArray();
      }
      else
      {
        controllerMethodInvocations = new ControllerMethodInvocation[0];
      }

      if (controllerMethodInvocations.Any(arg => arg.SkipInvocationOfScreenMethod))
      {
        LogTo.Debug($"Skipping {this.ScreenType}.{screenMethodName}.");
      }
      else
      {
        LogTo.Debug($"Calling {this.ScreenType}.{screenMethodName}");
        invocation.Proceed();
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

          controllerMethodInfo.Invoke(this.Controller,
                                      controllerMethodParameters);
        }
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="controllerMethodInfo" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenMethodInfo" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenMethodParameterTypes" /> is <see langword="null" /></exception>
    protected virtual bool AreMethodsIntertwined([NotNull] MethodInfo controllerMethodInfo,
                                                 [NotNull] MethodInfo screenMethodInfo,
                                                 [NotNull] [ItemNotNull] Type[] screenMethodParameterTypes)
    {
      if (controllerMethodInfo == null)
      {
        throw new ArgumentNullException(nameof(controllerMethodInfo));
      }
      if (screenMethodInfo == null)
      {
        throw new ArgumentNullException(nameof(screenMethodInfo));
      }
      if (screenMethodParameterTypes == null)
      {
        throw new ArgumentNullException(nameof(screenMethodParameterTypes));
      }

      if (controllerMethodInfo.ReturnType != screenMethodInfo.ReturnType)
      {
        return false;
      }

      var controllerParameterInfos = controllerMethodInfo.GetParameters();
      if (controllerParameterInfos.ElementAtOrDefault(0)
                                  ?.ParameterType != typeof(IScreen))
      {
        return false;
      }
      if (controllerParameterInfos.Skip(1)
                                  .Select(parameterInfo => parameterInfo.ParameterType)
                                  .SequenceEqual(screenMethodParameterTypes))
      {
        return true;
      }

      return false;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined, which has a <see langword="null" /> destination on <see cref="ScreenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined, which has not declared as <see langword="virtual" /> on <see cref="ScreenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined, which has an invalid injection defined in <see cref="ScreenMethodLinkAttribute.InjectInterfaceDefinition" />.</exception>
    [NotNull]
    protected virtual IDictionary<string, ControllerMethodInvocation[]> CreateScreenMethodMapping([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var controllerType = controller.GetType();
      var lookup = controllerType.GetMethods(ScreenInterceptor.DefaultBindingFlags)
                                 .SelectMany(methodInfo => methodInfo.GetAttributes<ScreenMethodLinkAttribute>(true)
                                                                     .Select(screenMethodLinkAttribute => new
                                                                                                          {
                                                                                                            ControllerMethodInfo = methodInfo,
                                                                                                            ScreenMethodLinkAttribute = screenMethodLinkAttribute
                                                                                                          }))
                                 .ToLookup(arg => arg.ScreenMethodLinkAttribute.MethodName ?? arg.ControllerMethodInfo.Name,
                                           arg => new ControllerMethodInvocation(arg.ControllerMethodInfo)
                                                  {
                                                    SkipInvocationOfScreenMethod = arg.ScreenMethodLinkAttribute.SkipInvocation,
                                                    InjectInterfaceDefinition = arg.ScreenMethodLinkAttribute.InjectInterfaceDefinition
                                                  });

      var result = new Dictionary<string, ControllerMethodInvocation[]>();

      foreach (var element in lookup)
      {
        var screenMethodName = element.Key;

        var controllerMethodInvocations = element.ToArray();

        foreach (var controllerMethodInvocation in controllerMethodInvocations)
        {
          var controllerMethodInfo = controllerMethodInvocation.ControllerMethodInfo;

          var screenMethodParameterInfos = controllerMethodInfo.GetParameters()
                                                               .Skip(1)
                                                               .ToArray();
          var screenMethodParameterTypes = screenMethodParameterInfos.Select(arg => arg.ParameterType)
                                                                     .ToArray();
          var screenMethodInfo = this.ScreenType.GetMethods(ScreenInterceptor.DefaultBindingFlags)
                                     .FirstOrDefault(arg => arg.Name == screenMethodName);
          if (screenMethodInfo == null)
          {
            var injectInterfaceDefinition = controllerMethodInvocation.InjectInterfaceDefinition;
            if (injectInterfaceDefinition == null)
            {
              throw new InvalidOperationException($"{controllerType} has a {nameof(ScreenMethodLinkAttribute)} defined on {controllerMethodInfo}, but the corresponding method does not exist on {this.ScreenType}. (hint: try setting {nameof(ScreenMethodLinkAttribute.InjectInterfaceDefinition)})");
            }
            var interfaceMethodInfo = injectInterfaceDefinition.GetMethods(ScreenInterceptor.DefaultBindingFlags)
                                                               .FirstOrDefault(arg => this.AreMethodsIntertwined(controllerMethodInfo,
                                                                                                                 arg,
                                                                                                                 screenMethodParameterTypes));
            if (interfaceMethodInfo == null)
            {
              throw new InvalidOperationException($"{controllerType} has a {nameof(ScreenMethodLinkAttribute)} defined on {controllerMethodInfo} with {nameof(ScreenMethodLinkAttribute.InjectInterfaceDefinition)} set. {injectInterfaceDefinition} does not contain an applicable method for later injection.");
            }
          }
          else if (this.AreMethodsIntertwined(controllerMethodInfo,
                                              screenMethodInfo,
                                              screenMethodParameterTypes))
          {
            if (!screenMethodInfo.IsVirtual)
            {
              throw new InvalidOperationException($"{controllerType} has a {nameof(ScreenMethodLinkAttribute)} defined on {controllerMethodInfo}, but the corresponding method is not declared as virtual on {this.ScreenType}.");
            }
          }
        }

        result[screenMethodName] = controllerMethodInvocations;
      }

      return result;
    }

    [CanBeNull]
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

    protected struct ControllerMethodInvocation
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
      }

      [CanBeNull]
      public MethodInfo ControllerMethodInfo { get; }

      public bool SkipInvocationOfScreenMethod { get; set; }

      [CanBeNull]
      public Type InjectInterfaceDefinition { get; set; }
    }
  }
}
