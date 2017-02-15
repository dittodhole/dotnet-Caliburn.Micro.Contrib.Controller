using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface IControllerManager
  {
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    [NotNull]
    Task<TController> ShowWindowAsync<TController>([CanBeNull] object options = null,
                                                   [CanBeNull] object context = null,
                                                   [CanBeNull] IDictionary<string, object> settings = null) where TController : IController<IScreen>;

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    [NotNull]
    Task<TController> ShowDialogAsync<TController>([CanBeNull] object options = null,
                                                   [CanBeNull] object context = null,
                                                   [CanBeNull] IDictionary<string, object> settings = null) where TController : IController<IScreen>;

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    TScreen CreateScreen<TScreen>([NotNull] IController<TScreen> controller,
                                  [CanBeNull] object options = null) where TScreen : IScreen;
  }

  public class ControllerManager : IControllerManager
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controllerLocator" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="windowManagerLocator" /> is <see langword="null" /></exception>
    public ControllerManager([NotNull] IScreenFactory screenFactory,
                             [NotNull] ILocator<IController> controllerLocator,
                             [NotNull] ILocator<IWindowManager> windowManagerLocator)
    {
      if (screenFactory == null)
      {
        throw new ArgumentNullException(nameof(screenFactory));
      }
      if (controllerLocator == null)
      {
        throw new ArgumentNullException(nameof(controllerLocator));
      }
      if (windowManagerLocator == null)
      {
        throw new ArgumentNullException(nameof(windowManagerLocator));
      }
      this.ScreenFactory = screenFactory;
      this.ControllerLocator = controllerLocator;
      this.WindowManagerLocator = windowManagerLocator;
    }

    [NotNull]
    private IScreenFactory ScreenFactory { get; }

    [NotNull]
    private ILocator<IController> ControllerLocator { get; }

    [NotNull]
    private ILocator<IWindowManager> WindowManagerLocator { get; }

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    public virtual async Task<TController> ShowWindowAsync<TController>(object options = null,
                                                                        object context = null,
                                                                        IDictionary<string, object> settings = null) where TController : IController<IScreen>
    {
      var controller = this.ControllerLocator.Locate<TController>();
      var screen = this.CreateScreen(controller,
                                     options);

      var windowManager = this.WindowManagerLocator.Locate();

      await Execute.OnUIThreadAsync(() => windowManager.ShowWindow(screen,
                                                                   context,
                                                                   settings))
                   .ConfigureAwait(false);

      return controller;
    }

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    public virtual async Task<TController> ShowDialogAsync<TController>(object options = null,
                                                                        object context = null,
                                                                        IDictionary<string, object> settings = null) where TController : IController<IScreen>
    {
      var controller = this.ControllerLocator.Locate<TController>();
      var screen = this.CreateScreen(controller,
                                     options);

      var windowManager = this.WindowManagerLocator.Locate();

      await Execute.OnUIThreadAsync(() => windowManager.ShowDialog(screen,
                                                                   context,
                                                                   settings))
                   .ConfigureAwait(false);

      return controller;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    public virtual TScreen CreateScreen<TScreen>(IController<TScreen> controller,
                                                 object options = null) where TScreen : IScreen
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var screenType = controller.GetScreenType(options);
      var screen = (TScreen) this.ScreenFactory.Create(screenType,
                                                       controller);
      screen = controller.BuildUp(screen,
                                  options);

      return screen;
    }
  }
}
