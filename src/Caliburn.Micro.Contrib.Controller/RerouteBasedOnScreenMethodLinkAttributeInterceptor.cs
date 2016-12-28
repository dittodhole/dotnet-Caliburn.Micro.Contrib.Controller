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
  public sealed class RerouteBasedOnScreenMethodLinkAttributeInterceptor : IInterceptor
  {
    static RerouteBasedOnScreenMethodLinkAttributeInterceptor()
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

    /// <exception cref="ArgumentNullException"><paramref name="interceptionTarget" /> is <see langword="null" /></exception>
    public RerouteBasedOnScreenMethodLinkAttributeInterceptor([NotNull] object interceptionTarget)
    {
      if (interceptionTarget == null)
      {
        throw new ArgumentNullException(nameof(interceptionTarget));
      }
      this.InterceptionTarget = interceptionTarget;

      this.ScreenMethodMapping = this.CreateScreenMethodMapping();
    }

    [NotNull]
    private object InterceptionTarget { get; }

    [NotNull]
    private IDictionary<string, ICollection<ControllerMethodInvocation>> ScreenMethodMapping { get; }

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
                                                                        var controllerMethodParameterInfos = controllerMethodInfo.GetParameters();
                                                                        var screenParameter = controllerMethodParameterInfos.FirstOrDefault();
                                                                        if (screenParameter == null)
                                                                        {
                                                                          return false;
                                                                        }
                                                                        var screenParameterType = screenParameter.ParameterType;
                                                                        if (!invocation.Proxy.GetType()
                                                                                       .IsDescendant(screenParameterType))
                                                                        {
                                                                          return false;
                                                                        }

                                                                        var screenMethodParameterTypes = controllerMethodParameterInfos.Skip(1)
                                                                                                                                       .Select(parameterInfo => parameterInfo.ParameterType)
                                                                                                                                       .ToArray();
                                                                        var returnType = controllerMethodInfo.ReturnType;

                                                                        var result = screenMethod.DoesSignatureMatch(returnType,
                                                                                                                     screenMethodParameterTypes);

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
        var returnValue = controllerMethodInvocation.ControllerMethodInfo.Invoke(this.InterceptionTarget,
                                                                                 controllerMethodParameters);

        if (!callBase)
        {
          invocation.ReturnValue = returnValue;
        }
      }
    }

    [Pure]
    [NotNull]
    private IDictionary<string, ICollection<ControllerMethodInvocation>> CreateScreenMethodMapping()
    {
      var result = new Dictionary<string, ICollection<ControllerMethodInvocation>>();

      var interceptionTargetType = this.InterceptionTarget.GetType();
      var interceptionTargetMethodInfos = interceptionTargetType.GetMethods(TypeExtensions.DefaultBindingFlags);
      foreach (var interceptionTargetMethodInfo in interceptionTargetMethodInfos)
      {
        var screenMethodLinkAttributes = interceptionTargetMethodInfo.GetAttributes<ScreenMethodLinkAttribute>(true)
                                                                     .ToArray();
        if (!screenMethodLinkAttributes.Any())
        {
          continue;
        }

        foreach (var screenMethodLinkAttribute in screenMethodLinkAttributes)
        {
          var screenMethodName = screenMethodLinkAttribute.MethodName ?? interceptionTargetMethodInfo.Name;

          ICollection<ControllerMethodInvocation> controllerMethodInvocations;
          if (!result.TryGetValue(screenMethodName,
                                  out controllerMethodInvocations))
          {
            result[screenMethodName] = controllerMethodInvocations = new List<ControllerMethodInvocation>();
          }

          var controllerMethodInvocation = new ControllerMethodInvocation(interceptionTargetMethodInfo,
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
