using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenManager
  {
    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    Task<TScreenFactoryAdapter> ShowWindowAsync<TScreenFactoryAdapter>(object? options = null,
                                                                       object? context = null,
                                                                       IDictionary<string, object>? settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter;

    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    Task<TScreenFactoryAdapter> ShowDialogAsync<TScreenFactoryAdapter>(object? options = null,
                                                                       object? context = null,
                                                                       IDictionary<string, object>? settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter;
  }

  public class ScreenManager : IScreenManager
  {
    private ConcurrentDictionary<Type, IScreenFactoryAdapter> SingletonScreenFactoryAdapters { get; } = new ConcurrentDictionary<Type, IScreenFactoryAdapter>();

    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    public virtual Task<TScreenFactoryAdapter> ShowWindowAsync<TScreenFactoryAdapter>(object? options = null,
                                                                                      object? context = null,
                                                                                      IDictionary<string, object>? settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      if (this.TryCreateScreen(options,
                               out TScreenFactoryAdapter screenFactoryAdapter,
                               out var screen))
      {
        var windowManager = IoC.Get<IWindowManager>();

        Execute.BeginOnUIThread(() => windowManager.ShowWindow(screen,
                                                               context,
                                                               settings));
      }

#if NET40
      return TaskEx.FromResult(screenFactoryAdapter);
#else
      return Task.FromResult(screenFactoryAdapter);
#endif
    }

    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    public virtual Task<TScreenFactoryAdapter> ShowDialogAsync<TScreenFactoryAdapter>(object? options = null,
                                                                                      object? context = null,
                                                                                      IDictionary<string, object>? settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      if (this.TryCreateScreen(options,
                               out TScreenFactoryAdapter screenFactoryAdapter,
                               out var screen))
      {
        var windowManager = IoC.Get<IWindowManager>();

        Execute.BeginOnUIThread(() => windowManager.ShowDialog(screen,
                                                               context,
                                                               settings));
      }

#if NET40
      return TaskEx.FromResult(screenFactoryAdapter);
#else
      return Task.FromResult(screenFactoryAdapter);
#endif
    }

    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    public virtual bool TryCreateScreen<TScreenFactoryAdapter>(object? options,
                                                               out TScreenFactoryAdapter screenFactoryAdapter,
                                                               out IScreen? screen)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      bool result;

      if (this.AllowMultipleScreenCreation<TScreenFactoryAdapter>())
      {
        screenFactoryAdapter = IoC.Get<TScreenFactoryAdapter>();
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
                                                                  arg =>
                                                                  {
                                                                    created = true;

                                                                    return IoC.Get<TScreenFactoryAdapter>();
                                                                  });

      screenFactoryAdapter = (TScreenFactoryAdapter) instance;

      return created;
    }

    public virtual bool Release<TScreenFactoryAdapter>()
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      var result = this.SingletonScreenFactoryAdapters.TryRemove(typeof(TScreenFactoryAdapter),
                                                                 out var instance);

      return result;
    }
  }
}
