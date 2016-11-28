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
    protected IDictionary<string, ControllerMethodInvocation> ScreenMethodMapping { get; }

    /// <exception cref="ArgumentNullException"><paramref name="invocation" /> is <see langword="null" /></exception>
    public virtual void Intercept([NotNull] IInvocation invocation)
    {
      if (invocation == null)
      {
        throw new ArgumentNullException(nameof(invocation));
      }

      var screenMethodName = invocation.Method.Name;

      ControllerMethodInvocation controllerMethodInvocation;
      this.ScreenMethodMapping.TryGetValue(screenMethodName,
                                           out controllerMethodInvocation);

      if (controllerMethodInvocation.SkipInvocationOfScreenMethod)
      {
        LogTo.Debug($"Skipping {this.ScreenType}.{screenMethodName}.");
      }
      else
      {
        LogTo.Debug($"Calling {this.ScreenType}.{screenMethodName}");
        invocation.Proceed();
      }

      var methodInfo = controllerMethodInvocation.MethodInfo;
      if (methodInfo != null)
      {
        var parameters = new object[invocation.Arguments.Count() + 1];
        parameters[0] = invocation.InvocationTarget;

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
    protected virtual IDictionary<string, ControllerMethodInvocation> CreateScreenMethodMapping([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var controllerType = controller.GetType();
      var lookup = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
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
      if (lookup.Any(group => group.Count() > 1))
      {
        throw new InvalidOperationException($"Controller {controllerType} has multiple methods intercepting the same method of {this.ScreenType}.");
      }

      var result = lookup.ToDictionary(arg => arg.Key,
                                       arg => arg.Single());

      return result;
    }

    protected struct ControllerMethodInvocation
    {
      /// <exception cref="ArgumentNullException"><paramref name="methodInfo" /> is <see langword="null" /></exception>
      public ControllerMethodInvocation([NotNull] MethodInfo methodInfo)
        : this()
      {
        if (methodInfo == null)
        {
          throw new ArgumentNullException(nameof(methodInfo));
        }
        this.MethodInfo = methodInfo;
      }

      [CanBeNull]
      public MethodInfo MethodInfo { get; }

      public bool SkipInvocationOfScreenMethod { get; set; }
    }

    [CanBeNull]
    public virtual IScreen CreateProxiedScreen()
    {
      var proxyGenerationOptions = new ProxyGenerationOptions();
      var proxyGenerator = new ProxyGenerator();
      var proxy = proxyGenerator.CreateClassProxy(this.ScreenType,
                                                  proxyGenerationOptions,
                                                  this);
      var screen = (IScreen) proxy;

      return screen;
    }
  }
}
