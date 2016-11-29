﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public static class Controller
  {
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException" />
    [CanBeNull]
    public delegate IScreen CreateScreen([NotNull] IController controller,
                                         [CanBeNull] object options = null);

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException" />
    [NotNull]
    public delegate IScreenInterceptor CreateScreenInterceptor([NotNull] IController controller,
                                                               [NotNull] Type screenType);

    [CanBeNull]
    public static CreateScreen CreateScreenFn = (controller,
                                                 options) =>
                                                {
                                                  var screenType = controller.GetScreenType(options);
                                                  var screenInterceptor = Controller.CreateScreenInterceptorFn?.Invoke(controller,
                                                                                                                       screenType);

                                                  var screen = screenInterceptor?.CreateProxiedScreen();

                                                  return screen;
                                                };

    [CanBeNull]
    public static CreateScreenInterceptor CreateScreenInterceptorFn = (controller,
                                                                       screenType) =>
                                                                      {
                                                                        var screenInterceptor = new ScreenInterceptor(controller,
                                                                                                                      screenType);

                                                                        return screenInterceptor;
                                                                      };

    /// <exception cref="InvalidOperationException" />
    public static async Task<TController> ShowWindowAsync<TController>([CanBeNull] object options = null,
                                                                       [CanBeNull] object context = null,
                                                                       [CanBeNull] IDictionary<string, object> settings = null) where TController : IController
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
                                                                   settings))
                   .ConfigureAwait(false);

      return controller;
    }

    /// <exception cref="InvalidOperationException" />
    public static async Task<TController> ShowDialogAsync<TController>([CanBeNull] object options = null,
                                                                       [CanBeNull] object context = null,
                                                                       [CanBeNull] IDictionary<string, object> settings = null) where TController : IController
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
                                                                   settings))
                   .ConfigureAwait(false);

      return controller;
    }
  }
}
