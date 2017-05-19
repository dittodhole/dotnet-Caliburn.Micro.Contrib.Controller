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
    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    [NotNull]
    Task<TScreenFactoryAdapter> ShowWindowAsync<TScreenFactoryAdapter>([CanBeNull] object options = null,
                                                                       [CanBeNull] object context = null,
                                                                       [CanBeNull] IDictionary<string, object> settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter;

    /// <exception cref="ArgumentException" />
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
    private ConcurrentDictionary<Type, IScreenFactoryAdapter> SingletonScreenFactoryAdapters { get; } = new ConcurrentDictionary<Type, IScreenFactoryAdapter>();

    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    public virtual async Task<TScreenFactoryAdapter> ShowWindowAsync<TScreenFactoryAdapter>(object options = null,
                                                                                            object context = null,
                                                                                            IDictionary<string, object> settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      TScreenFactoryAdapter screenFactoryAdapter;
      IScreen screen;
      if (!this.TryCreateScreen(options,
                                out screenFactoryAdapter,
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

    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    public virtual async Task<TScreenFactoryAdapter> ShowDialogAsync<TScreenFactoryAdapter>(object options = null,
                                                                                            object context = null,
                                                                                            IDictionary<string, object> settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      TScreenFactoryAdapter screenFactoryAdapter;
      IScreen screen;
      if (!this.TryCreateScreen(options,
                                out screenFactoryAdapter,
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

    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    public virtual bool TryCreateScreen<TScreenFactoryAdapter>([CanBeNull] object options,
                                                               [NotNull] out TScreenFactoryAdapter screenFactoryAdapter,
                                                               [CanBeNull] out IScreen screen)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      bool result;

      if (this.AllowMultipleScreenCreation<TScreenFactoryAdapter>())
      {
        screenFactoryAdapter = this.ScreenFactoryAdapterLocator.Locate<TScreenFactoryAdapter>();
        screen = screenFactoryAdapter.CreateScreen(options);
        result = true;
      }
      else
      {
        if (this.CreateOrGet(out screenFactoryAdapter))
        {
          var screenInstance = screenFactoryAdapter.CreateScreen(options);

          EventHandler<DeactivationEventArgs> onDeactived = null;
          onDeactived = (sender,
                         args) =>
                        {
                          if (args.WasClosed)
                          {
                            screenInstance.Deactivated -= onDeactived;

                            this.Release<TScreenFactoryAdapter>();
                          }
                        };
          screenInstance.Deactivated += onDeactived;

          screen = screenInstance;
          result = true;
        }
        else
        {
          screen = null;
          result = false;
        }
      }

      return result;
    }

    public virtual bool AllowMultipleScreenCreation<TScreenFactoryAdapter>()
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      var disallowConcurrentScreenCreation = typeof(TScreenFactoryAdapter).GetAttributes<DisallowConcurrentScreenCreation>(true)
                                                                          .FirstOrDefault();

      var result = disallowConcurrentScreenCreation == null;

      return result;
    }

    public virtual bool CreateOrGet<TScreenFactoryAdapter>(out TScreenFactoryAdapter screenFactoryAdapter)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      var created = false;
      var instance = this.SingletonScreenFactoryAdapters.GetOrAdd(typeof(TScreenFactoryAdapter),
                                                                  screenFactoryAdapterType =>
                                                                  {
                                                                    created = true;

                                                                    return this.ScreenFactoryAdapterLocator.Locate(screenFactoryAdapterType);
                                                                  });

      screenFactoryAdapter = (TScreenFactoryAdapter) instance;

      return created;
    }

    public virtual bool Release<TScreenFactoryAdapter>()
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      IScreenFactoryAdapter instance;
      var result = this.SingletonScreenFactoryAdapters.TryRemove(typeof(TScreenFactoryAdapter),
                                                                 out instance);

      return result;
    }
  }
}
