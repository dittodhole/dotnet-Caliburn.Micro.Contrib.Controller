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
                                                   [CanBeNull] IDictionary<string, object> settings = null) where TController : ControllerBase;

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    [NotNull]
    Task<TController> ShowDialogAsync<TController>([CanBeNull] object options = null,
                                                   [CanBeNull] object context = null,
                                                   [CanBeNull] IDictionary<string, object> settings = null) where TController : ControllerBase;
  }

  public class ControllerManager : IControllerManager,
                                   IDisposable
  {
    /// <exception cref="ArgumentNullException"><paramref name="controllerLocator" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="windowManagerLocator" /> is <see langword="null" /></exception>
    public ControllerManager([NotNull] ILocator<ControllerBase> controllerLocator,
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
    private ILocator<ControllerBase> ControllerLocator { get; }

    [NotNull]
    private ILocator<IWindowManager> WindowManagerLocator { get; }

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    public virtual async Task<TController> ShowWindowAsync<TController>(object options = null,
                                                                        object context = null,
                                                                        IDictionary<string, object> settings = null) where TController : ControllerBase
    {
      var controller = this.CreateController<TController>();
      var screen = controller.CreateScreen(options);
      if (screen == null)
      {
        throw new InvalidOperationException($"{typeof(TController)} did not return a {nameof(screen)}.");
      }

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
                                                                        IDictionary<string, object> settings = null) where TController : ControllerBase
    {
      var controller = this.CreateController<TController>();
      var screen = controller.CreateScreen(options);
      if (screen == null)
      {
        throw new InvalidOperationException($"{typeof(TController)} did not return a {nameof(screen)}.");
      }

      var windowManager = this.WindowManagerLocator.Locate();

      await Execute.OnUIThreadAsync(() => windowManager.ShowDialog(screen,
                                                                   context,
                                                                   settings))
                   .ConfigureAwait(false);

      return controller;
    }

    public virtual void Dispose() {}

    /// <exception cref="Exception" />
    [NotNull]
    public virtual TController CreateController<TController>() where TController : ControllerBase
    {
      var type = typeof(TController);
      var obj = this.ControllerLocator.Locate(type);
      var controller = (TController) obj;

      return controller;
    }
  }
}
