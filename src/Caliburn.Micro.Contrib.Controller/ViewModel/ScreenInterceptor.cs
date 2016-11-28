using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Anotar.LibLog;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Internal;
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

      this.ScreenMethodMapping = this.CreateScreenMethodMapping(this.Controller);
      this.ScreenType.CheckTypeForRealScreenType();
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

      ControllerMethodInvocation controllerMethodInvocation;
      {
        ControllerMethodInvocation[] controllerMethodInvocations;
        if (this.ScreenMethodMapping.TryGetValue(screenMethodName,
                                                 out controllerMethodInvocations))
        {
          var screenMethodInfo = invocation.Method;
          var screenParameterInfos = screenMethodInfo.GetParameters()
                                                     .Select(parameterInfo => parameterInfo.ParameterType)
                                                     .ToArray();

          controllerMethodInvocation = controllerMethodInvocations.FirstOrDefault(arg =>
                                                                                  {
                                                                                    var controllerMethodInfo = arg.ControllerMethodInfo;
                                                                                    if (controllerMethodInfo.ReturnType != screenMethodInfo.ReturnType)
                                                                                    {
                                                                                      return false;
                                                                                    }

                                                                                    var controllerParameterInfos = controllerMethodInfo.GetParameters();
                                                                                    if (controllerParameterInfos.ElementAtOrDefault(0)
                                                                                                                ?.ParameterType != typeof(object))
                                                                                    {
                                                                                      return false;
                                                                                    }
                                                                                    if (controllerParameterInfos.Skip(1)
                                                                                                                .Select(parameterInfo => parameterInfo.ParameterType)
                                                                                                                .SequenceEqual(screenParameterInfos))
                                                                                    {
                                                                                      return true;
                                                                                    }

                                                                                    return false;
                                                                                  });
        }
        else
        {
          controllerMethodInvocation = default(ControllerMethodInvocation);
        }
      }

      if (controllerMethodInvocation.SkipInvocationOfScreenMethod)
      {
        LogTo.Debug($"Skipping {this.ScreenType}.{screenMethodName}.");
      }
      else
      {
        LogTo.Debug($"Calling {this.ScreenType}.{screenMethodName}");
        invocation.Proceed();
      }

      var methodInfo = controllerMethodInvocation.ControllerMethodInfo;
      if (methodInfo != null)
      {
        var parameters = new object[invocation.Arguments.Count() + 1];
        parameters[0] = invocation.Proxy;

        Array.Copy(invocation.Arguments,
                   0,
                   parameters,
                   1,
                   invocation.Arguments.Count());
        methodInfo.Invoke(this.Controller,
                          parameters);
      }
    }

    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has multiple methods intercepting the same method of <see cref="ScreenType" /> defined.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" />.</exception>
    [NotNull]
    protected virtual IDictionary<string, ControllerMethodInvocation[]> CreateScreenMethodMapping([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var controllerType = controller.GetType();
      var lookup = controllerType.GetMethods(ScreenInterceptor.DefaultBindingFlags)
                                 .Select(methodInfo => new
                                                       {
                                                         ControllerMethodInfo = methodInfo,
                                                         ScreenMethodLinkAttribute = methodInfo.GetAttributes<ScreenMethodLinkAttribute>(true)
                                                                                               .FirstOrDefault()
                                                       })
                                 .Where(arg => arg.ScreenMethodLinkAttribute != null)
                                 .ToLookup(arg => arg.ScreenMethodLinkAttribute.MethodName ?? arg.ControllerMethodInfo.Name,
                                           arg => new ControllerMethodInvocation(arg.ControllerMethodInfo)
                                                  {
                                                    SkipInvocationOfScreenMethod = arg.ScreenMethodLinkAttribute.SkipInvocation
                                                  });

      var result = lookup.ToDictionary(arg => arg.Key,
                                       arg => arg.ToArray());

      return result;
    }

    [CanBeNull]
    public virtual IScreen CreateProxiedScreen()
    {
      var additionalInterfacesToProxy = this.Controller.GetType()
                                            .GetAllInterfaces()
                                            .Select(type => this.GetMappedHandleType(type))
                                            .Where(type => type != null)
                                            .Distinct()
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

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    [CanBeNull]
    protected virtual Type GetMappedHandleType([NotNull] Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      var genericArgument = default(Type);
      if (type.IsGenericType)
      {
        genericArgument = type.GetGenericArguments()
                              .FirstOrDefault();
        type = type.GetGenericTypeDefinition();
      }

      Type result;
      if (type == typeof(IInterceptHandle<>))
      {
        result = typeof(IHandle<>).MakeGenericType(genericArgument);
      }
      else if (type == typeof(IInterceptHandleWithCoroutine<>))
      {
        result = typeof(IHandleWithCoroutine<>).MakeGenericType(genericArgument);
      }
      else if (type == typeof(IInterceptHandleWithTask<>))
      {
        result = typeof(IHandleWithTask<>).MakeGenericType(genericArgument);
      }
      else
      {
        result = null;
      }

      return result;
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
    }
  }
}
