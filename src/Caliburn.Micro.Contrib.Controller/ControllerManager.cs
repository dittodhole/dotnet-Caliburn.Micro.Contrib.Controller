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
    /// <exception cref="ArgumentNullException"><paramref name="controllerLocator" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="windowManagerLocator" /> is <see langword="null" /></exception>
    public ControllerManager([NotNull] ILocator<IController> controllerLocator,
                             [NotNull] ILocator<IWindowManager> windowManagerLocator)
    {
      if (controllerLocator == null)
      {
        throw new ArgumentNullException(nameof(controllerLocator));
      }
      if (windowManagerLocator == null)
      {
        throw new ArgumentNullException(nameof(windowManagerLocator));
      }
      this.ControllerLocator = controllerLocator;
      this.WindowManagerLocator = windowManagerLocator;
    }

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
      var controller = this.ControllerLocator.Locate<TController>();
      var screen = controller.CreateScreen(options);

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
      var controller = this.ControllerLocator.Locate<TController>();
      var screen = controller.CreateScreen(options);

      var windowManager = this.WindowManagerLocator.Locate();

      await Execute.OnUIThreadAsync(() => windowManager.ShowDialog(screen,
                                                                   context,
                                                                   settings))
                   .ConfigureAwait(false);

      return controller;
    }
  }
}
