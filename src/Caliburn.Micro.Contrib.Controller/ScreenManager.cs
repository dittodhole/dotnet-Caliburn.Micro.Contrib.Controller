using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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

    [NotNull]
    private ConcurrentDictionary<Type, IScreenFactoryAdapter> SingletonScreenTypes { get; } = new ConcurrentDictionary<Type, IScreenFactoryAdapter>();

    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    public virtual async Task<TScreenFactoryAdapter> ShowWindowAsync<TScreenFactoryAdapter>(object options = null,
                                                                                            object context = null,
                                                                                            IDictionary<string, object> settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      var screenFactoryAdapter = this.ScreenFactoryAdapterLocator.Locate<TScreenFactoryAdapter>();

      IScreen screen;
      if (!this.TryCreateScreen(ref screenFactoryAdapter,
                                options,
                                out screen))
      {
        return screenFactoryAdapter;
      }

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

      IScreen screen;
      if (!this.TryCreateScreen(ref screenFactoryAdapter,
                                options,
                                out screen))
      {
        return screenFactoryAdapter;
      }

      var windowManager = this.WindowManagerLocator.Locate();

      await Execute.OnUIThreadAsync(() => windowManager.ShowDialog(screen,
                                                                   context,
                                                                   settings))
                   .ConfigureAwait(false);

      return screenFactoryAdapter;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenFactoryAdapter" /> is <see langword="null" /></exception>
    public virtual bool TryCreateScreen<TScreenFactoryAdapter>([NotNull] ref TScreenFactoryAdapter screenFactoryAdapter,
                                                               [CanBeNull] object options,
                                                               [CanBeNull] out IScreen screen)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      if (screenFactoryAdapter == null)
      {
        throw new ArgumentNullException(nameof(screenFactoryAdapter));
      }

      // TODO maybe refactor the DisallowMultipleDisplayAttribute-part
      // to be done in ControllerBase.CreateScreen (returning null),
      // but atm this seems like a big change, and might be better
      // off within the scope of Screen*Manager* though ...

      var screenType = screenFactoryAdapter.GetScreenType(options);

      var disallowMultipleDisplayAttribute = screenType.GetAttributes<DisallowMultipleDisplayAttribute>(true)
                                                       .FirstOrDefault();
      if (disallowMultipleDisplayAttribute == null)
      {
        screen = screenFactoryAdapter.CreateScreen(options);
        return true;
      }

      var bondedScreenFactoryAdapter = this.SingletonScreenTypes.GetOrAdd(screenType,
                                                                          screenFactoryAdapter);
      if (object.ReferenceEquals(bondedScreenFactoryAdapter,
                                 screenFactoryAdapter))
      {
        var screenInstance = screenFactoryAdapter.CreateScreen(options);

        EventHandler<DeactivationEventArgs> onDeactived = null;
        onDeactived = (sender,
                       args) =>
                      {
                        if (args.WasClosed)
                        {
                          screenInstance.Deactivated -= onDeactived;
                          this.SingletonScreenTypes.TryRemove(screenType,
                                                              out bondedScreenFactoryAdapter);
                        }
                      };
        screenInstance.Deactivated += onDeactived;

        screen = screenInstance;
        return true;
      }

      screenFactoryAdapter = (TScreenFactoryAdapter) bondedScreenFactoryAdapter;
      screen = null;
      return false;
    }
  }
}
