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
                                                   [CanBeNull] IDictionary<string, object> settings = null) where TController : IController;

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    [NotNull]
    Task<TController> ShowDialogAsync<TController>([CanBeNull] object options = null,
                                                   [CanBeNull] object context = null,
                                                   [CanBeNull] IDictionary<string, object> settings = null) where TController : IController;
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
                                                                        IDictionary<string, object> settings = null) where TController : IController
    {
      var controllerAndScreen = this.CreateScreen<TController>(options);
      var controller = controllerAndScreen.Controller;
      var screen = controllerAndScreen.Screen;

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
                                                                        IDictionary<string, object> settings = null) where TController : IController
    {
      var controllerAndScreen = this.CreateScreen<TController>(options);
      var controller = controllerAndScreen.Controller;
      var screen = controllerAndScreen.Screen;

      var windowManager = this.WindowManagerLocator.Locate();

      await Execute.OnUIThreadAsync(() => windowManager.ShowDialog(screen,
                                                                   context,
                                                                   settings))
                   .ConfigureAwait(false);

      return controller;
    }

    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    public virtual ControllerAndScreen<TController> CreateScreen<TController>([CanBeNull] object options = null) where TController : IController
    {
      var controller = this.ControllerLocator.Locate<TController>();
      var screenType = controller.GetScreenType(options);
      var screen = this.ScreenFactory.Create(screenType,
                                             controller);
      screen = controller.BuildUp(screen,
                                  options);

      var controllerAndScreen = new ControllerAndScreen<TController>(controller,
                                                                     screen);

      return controllerAndScreen;
    }

    public sealed class ControllerAndScreen<TController>
      where TController : IController
    {
      /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      public ControllerAndScreen([NotNull] TController controller,
                                 [NotNull] IScreen screen)
      {
        if (controller == null)
        {
          throw new ArgumentNullException(nameof(controller));
        }
        if (screen == null)
        {
          throw new ArgumentNullException(nameof(screen));
        }
        this.Controller = controller;
        this.Screen = screen;
      }

      [NotNull]
      public TController Controller { get; }

      [NotNull]
      public IScreen Screen { get; }
    }
  }
}
