using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface IScreenManager
  {
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    [NotNull]
    Task<TScreenFactoryAdapter> ShowWindowAsync<TScreenFactoryAdapter>([CanBeNull] object options = null,
                                                                       [CanBeNull] object context = null,
                                                                       [CanBeNull] IDictionary<string, object> settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter;

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    [NotNull]
    Task<TScreenFactoryAdapter> ShowDialogAsync<TScreenFactoryAdapter>([CanBeNull] object options = null,
                                                                       [CanBeNull] object context = null,
                                                                       [CanBeNull] IDictionary<string, object> settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter;
  }

  public class ScreenManager : IScreenManager
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenFactoryAdapterLocator" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="windowManagerLocator" /> is <see langword="null" /></exception>
    public ScreenManager([NotNull] ILocator<IScreenFactoryAdapter> screenFactoryAdapterLocator,
                         [NotNull] ILocator<IWindowManager> windowManagerLocator)
    {
      if (screenFactoryAdapterLocator == null)
      {
        throw new ArgumentNullException(nameof(screenFactoryAdapterLocator));
      }
      if (windowManagerLocator == null)
      {
        throw new ArgumentNullException(nameof(windowManagerLocator));
      }
      this.ScreenFactoryAdapterLocator = screenFactoryAdapterLocator;
      this.WindowManagerLocator = windowManagerLocator;
    }

    [NotNull]
    private ILocator<IScreenFactoryAdapter> ScreenFactoryAdapterLocator { get; }

    [NotNull]
    private ILocator<IWindowManager> WindowManagerLocator { get; }

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    public virtual async Task<TScreenFactoryAdapter> ShowWindowAsync<TScreenFactoryAdapter>(object options = null,
                                                                                            object context = null,
                                                                                            IDictionary<string, object> settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      var screenFactoryAdapter = this.ScreenFactoryAdapterLocator.Locate<TScreenFactoryAdapter>();
      var screen = screenFactoryAdapter.CreateScreen(options);

      var windowManager = this.WindowManagerLocator.Locate();

      await Execute.OnUIThreadAsync(() => windowManager.ShowWindow(screen,
                                                                   context,
                                                                   settings))
                   .ConfigureAwait(false);

      return screenFactoryAdapter;
    }

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    public virtual async Task<TScreenFactoryAdapter> ShowDialogAsync<TScreenFactoryAdapter>(object options = null,
                                                                                            object context = null,
                                                                                            IDictionary<string, object> settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      var screenFactoryAdapter = this.ScreenFactoryAdapterLocator.Locate<TScreenFactoryAdapter>();
      var screen = screenFactoryAdapter.CreateScreen(options);

      var windowManager = this.WindowManagerLocator.Locate();

      await Execute.OnUIThreadAsync(() => windowManager.ShowDialog(screen,
                                                                   context,
                                                                   settings))
                   .ConfigureAwait(false);

      return screenFactoryAdapter;
    }
  }
}
