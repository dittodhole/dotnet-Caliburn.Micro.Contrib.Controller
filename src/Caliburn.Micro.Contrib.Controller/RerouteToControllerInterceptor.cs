using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.LibLog;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public sealed class RerouteToControllerInterceptor : IInterceptor,
                                                       IDisposable
  {
    static RerouteToControllerInterceptor()
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
    public RerouteToControllerInterceptor([NotNull] ControllerBase controller,
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
    private ControllerBase Controller { get; }

    [NotNull]
    private Type ScreenType { get; }

    [NotNull]
    private IDictionary<string, ICollection<ControllerMethodInvocation>> ScreenMethodMapping { get; }

    public void Dispose()
    {
      this.ScreenMethodMapping.Clear();
    }

    /// <exception cref="ArgumentNullException"><paramref name="invocation" /> is <see langword="null" /></exception>
    public void Intercept(IInvocation invocation)
    {
      if (invocation == null)
      {
        throw new ArgumentNullException(nameof(invocation));
      }

      var screenMethod = invocation.Method;
      var screenMethodName = screenMethod.Name;

      ICollection<ControllerMethodInvocation> controllerMethodInvocations;
      if (!this.ScreenMethodMapping.TryGetValue(screenMethodName,
                                                out controllerMethodInvocations))
      {
        LogTo.Debug($"No reroute for {screenMethodName} was found, proceeding invocation.");
        invocation.Proceed();
        return;
      }

      controllerMethodInvocations = controllerMethodInvocations.Where(arg =>
                                                                      {
                                                                        var controllerMethodInfo = arg.ControllerMethodInfo;
                                                                        var controllerMethodParameterTypes = controllerMethodInfo.GetParameters()
                                                                                                                                 .Skip(1)
                                                                                                                                 .Select(parameterInfo => parameterInfo.ParameterType)
                                                                                                                                 .ToArray();
                                                                        var returnType = controllerMethodInfo.ReturnType;

                                                                        var result = controllerMethodInfo.DoesSignatureMatch(returnType,
                                                                                                                             controllerMethodParameterTypes);

                                                                        return result;
                                                                      })
                                                               .ToArray();
      if (!controllerMethodInvocations.Any())
      {
        LogTo.Debug($"No reroute for {screenMethodName} with matching signature was found, proceeding invocation.");
        invocation.Proceed();
        return;
      }

      var callBase = controllerMethodInvocations.Any(arg => arg.CallBase);
      if (callBase)
      {
        LogTo.Debug($"Calling {invocation.TargetType}.{screenMethodName}");
        invocation.Proceed();
      }
      else
      {
        LogTo.Debug($"Skipping {invocation.TargetType}.{screenMethodName}.");
      }

      var screenMethodParameters = invocation.Arguments;
      var controllerMethodParameters = new object[screenMethodParameters.Count() + 1];
      controllerMethodParameters[0] = invocation.Proxy;
      Array.Copy(screenMethodParameters,
                 0,
                 controllerMethodParameters,
                 1,
                 screenMethodParameters.Count());

      foreach (var controllerMethodInvocation in controllerMethodInvocations)
      {
        var returnValue = controllerMethodInvocation.ControllerMethodInfo.Invoke(this.Controller,
                                                                                 controllerMethodParameters);

        if (!callBase)
        {
          invocation.ReturnValue = returnValue;
        }
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which has no parameters.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which cannot be found on <see cref="ScreenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which is not declared as <see langword="virtual" /> or <see langword="abstract" /> on <see cref="ScreenType" />.</exception>
    [Pure]
    [NotNull]
    private IDictionary<string, ICollection<ControllerMethodInvocation>> CreateScreenMethodMapping([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var result = new Dictionary<string, ICollection<ControllerMethodInvocation>>();

      var controllerType = controller.GetType();
      var controllerMethodInfos = controllerType.GetMethods(TypeExtensions.DefaultBindingFlags);
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
        if (!this.ScreenType.IsDescendantOrMatches(screenParameter.ParameterType))
        {
          continue;
        }

        foreach (var screenMethodLinkAttribute in screenMethodLinkAttributes)
        {
          var screenMethodName = screenMethodLinkAttribute.MethodName ?? controllerMethodInfo.Name;

          ICollection<ControllerMethodInvocation> controllerMethodInvocations;
          if (!result.TryGetValue(screenMethodName,
                                  out controllerMethodInvocations))
          {
            result[screenMethodName] = controllerMethodInvocations = new List<ControllerMethodInvocation>();
          }

          var controllerMethodInvocation = new ControllerMethodInvocation(controllerMethodInfo,
                                                                          screenMethodLinkAttribute.CallBase);
          controllerMethodInvocations.Add(controllerMethodInvocation);
        }
      }

      return result;
    }

    private sealed class ControllerMethodInvocation
    {
      /// <exception cref="ArgumentNullException"><paramref name="controllerMethodInfo" /> is <see langword="null" /></exception>
      public ControllerMethodInvocation([NotNull] MethodInfo controllerMethodInfo,
                                        bool callBase)
      {
        if (controllerMethodInfo == null)
        {
          throw new ArgumentNullException(nameof(controllerMethodInfo));
        }
        this.ControllerMethodInfo = controllerMethodInfo;
        this.CallBase = callBase;
      }

      [NotNull]
      public MethodInfo ControllerMethodInfo { get; }

      public bool CallBase { get; }
    }
  }
}
