using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenManager
  {
    /// <exception cref="ArgumentException"/>
    /// <exception cref="Exception"/>
    Task<TScreenFactoryAdapter> ShowWindowAsync<TScreenFactoryAdapter>(object? options = null,
                                                                       object? context = null,
                                                                       IDictionary<string, object>? settings = null) where TScreenFactoryAdapter : IScreenFactoryAdapter;

    /// <exception cref="ArgumentException"/>
    /// <exception cref="Exception"/>
    Task<TScreenFactoryAdapter> ShowDialogAsync<TScreenFactoryAdapter>(object? options = null,
                                                                       object? context = null,
                                                                       IDictionary<string, object>? settings = null) where TScreenFactoryAdapter : IScreenFactoryAdapter;
  }

  public class ScreenManager : IScreenManager
  {
    /// <inheritdoc/>
    public virtual Task<TScreenFactoryAdapter> ShowWindowAsync<TScreenFactoryAdapter>(object? options = null,
                                                                                      object? context = null,
                                                                                      IDictionary<string, object>? settings = null) where TScreenFactoryAdapter : IScreenFactoryAdapter
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

    /// <inheritdoc/>
    public virtual Task<TScreenFactoryAdapter> ShowDialogAsync<TScreenFactoryAdapter>(object? options = null,
                                                                                      object? context = null,
                                                                                      IDictionary<string, object>? settings = null) where TScreenFactoryAdapter : IScreenFactoryAdapter
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

    /// <exception cref="ArgumentException"/>
    /// <exception cref="Exception"/>
    public virtual bool TryCreateScreen<TScreenFactoryAdapter>(object? options,
                                                               out TScreenFactoryAdapter screenFactoryAdapter,
                                                               out IScreen? screen) where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      bool result;

      screenFactoryAdapter = IoC.Get<TScreenFactoryAdapter>();
      screen = screenFactoryAdapter.CreateScreen(options);
      result = true;

      return result;
    }
  }
}
