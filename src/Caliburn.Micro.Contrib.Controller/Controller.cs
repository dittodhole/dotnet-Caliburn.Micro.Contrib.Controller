using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public static class Controller
  {
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException">The <see cref="Type" /> returned by <see cref="ControllerBase.GetScreenType" /> of <paramref name="controller" /> is an interface.</exception>
    /// <exception cref="InvalidOperationException">The <see cref="Type" /> returned by <see cref="ControllerBase.GetScreenType" /> of <paramref name="controller" /> does not implement <see cref="IScreen" />.</exception>
    public delegate IScreen CreateScreen([NotNull] ControllerBase controller,
                                         [CanBeNull] object options = null);

    /// <exception cref="InvalidOperationException">The <paramref name="type" /> is an interface.</exception>
    /// <exception cref="InvalidOperationException">The <paramref name="type" /> does not implement <see cref="IScreen" />.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    public static void CheckTypeForRealScreenType([NotNull] this Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }
      if (type.IsInterface)
      {
        throw new InvalidOperationException($"Cannot create proxy for interface {type}.");
      }
      if (!typeof(IScreen).IsAssignableFrom(type))
      {
        throw new InvalidOperationException($"Cannot create proxy for {type}, as this type does implement {nameof(IScreen)}.");
      }
    }

    [CanBeNull]
    public static CreateScreen CreateScreenFn = (controller,
                                                 options) =>
                                                {
                                                  var screenType = controller.GetScreenType(options);

                                                  var screenInterceptor = new ScreenInterceptor(controller,
                                                                                                screenType);
                                                  var proxyGenerationOptions = new ProxyGenerationOptions();
                                                  var proxyGenerator = new ProxyGenerator();
                                                  var proxy = proxyGenerator.CreateClassProxy(screenInterceptor.ScreenType,
                                                                                              proxyGenerationOptions,
                                                                                              screenInterceptor);

                                                  var screen = (IScreen) proxy;

                                                  return screen;
                                                };

    /// <exception cref="InvalidOperationException">If <typeparamref name="TController" />-instance did not create a screen.</exception>
    public static async Task<TController> ShowWindowAsync<TController>([CanBeNull] object options = null,
                                                                       [CanBeNull] object context = null,
                                                                       [CanBeNull] IDictionary<string, object> settings = null) where TController : ControllerBase
    {
      var controller = IoC.Get<TController>();
      var screen = controller.CreateScreen(options);
      if (screen == null)
      {
        throw new InvalidOperationException($"{typeof(TController)} did not return a {nameof(screen)}.");
      }

      var windowManager = IoC.Get<IWindowManager>();

      await Execute.OnUIThreadAsync(() => windowManager.ShowWindow(screen,
                                                                   context,
                                                                   settings));

      return controller;
    }

    /// <exception cref="InvalidOperationException">If <typeparamref name="TController" />-instance did not create a screen.</exception>
    public static async Task<TController> ShowDialogAsync<TController>([CanBeNull] object options = null,
                                                                       [CanBeNull] object context = null,
                                                                       [CanBeNull] IDictionary<string, object> settings = null) where TController : ControllerBase
    {
      var controller = IoC.Get<TController>();
      var screen = controller.CreateScreen(options);
      if (screen == null)
      {
        throw new InvalidOperationException($"{typeof(TController)} did not return a {nameof(screen)}.");
      }

      var windowManager = IoC.Get<IWindowManager>();

      await Execute.OnUIThreadAsync(() => windowManager.ShowDialog(screen,
                                                                   context,
                                                                   settings));

      return controller;
    }
  }
}
