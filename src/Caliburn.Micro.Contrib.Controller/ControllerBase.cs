using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.Proxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class ControllerBase<TScreen> : IController
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] IScreenFactory screenFactory,
                             [NotNull] [ItemNotNull] ICollection<IRoutine> routines)
    {
      if (screenFactory == null)
      {
        throw new ArgumentNullException(nameof(screenFactory));
      }
      if (routines == null)
      {
        throw new ArgumentNullException(nameof(routines));
      }
      this.ScreenFactory = screenFactory;
      this.Routines = routines;
    }

    [NotNull]
    private IScreenFactory ScreenFactory { get; }

    [NotNull]
    [ItemNotNull]
    public virtual ICollection<IRoutine> Routines { get; }

    public virtual Task<object> GetResultAsync(CancellationToken cancellationToken)
    {
      return TaskEx.FromResult<object>(null);
    }

    IScreen IController.CreateScreen(object options = null)
    {
      return this.CreateScreen(options);
    }

    /// <exception cref="ArgumentNullException">If <see cref="Type" /> returned by <see cref="GetScreenType" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    public virtual TScreen CreateScreen([CanBeNull] object options = null)
    {
      var screenType = this.GetScreenType(options);
      var mixinProviders = new object[]
                           {
                             this
                           }.Concat(this.Routines)
                            .OfType<IMixinProvider>()
                            .ToArray();
      var screen = (TScreen) this.ScreenFactory.Create(screenType,
                                                       mixinProviders,
                                                       this);

      screen = this.BuildUp(screen,
                            options);

      return screen;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [UsedImplicitly]
    [InterceptProxyMethod(MethodName = "OnViewReady", CallBase = true)]
    internal void OnViewReady([NotNull] IScreen screen,
                              [NotNull] object view)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (view == null)
      {
        throw new ArgumentNullException(nameof(view));
      }

      TaskEx.Run(() => this.OnViewReadyAsync((TScreen) screen,
                                             view));
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [UsedImplicitly]
    [InterceptProxyMethod(MethodName = "OnActivate", CallBase = true)]
    internal void OnActivate([NotNull] IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      this.OnActivate((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [UsedImplicitly]
    [InterceptProxyMethod(MethodName = "OnDeactivate", CallBase = true)]
    internal void OnDeactivate([NotNull] IScreen screen,
                               bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      this.OnDeactivate((TScreen) screen,
                        close);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [UsedImplicitly]
    [InterceptProxyMethod(MethodName = "OnInitialize", CallBase = true)]
    internal void OnInitialize([NotNull] IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      this.OnInitialize((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [UsedImplicitly]
    [InterceptProxyMethod(MethodName = nameof(IClose.TryClose), CallBase = true)]
    internal void OnClose([NotNull] IScreen screen,
                          bool? dialogResult = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      this.OnClose((TScreen) screen,
                   dialogResult);
    }

    [Pure]
    [NotNull]
    public virtual Type GetScreenType([CanBeNull] object options = null) => typeof(TScreen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnClose([NotNull] TScreen screen,
                                bool? dialogResult = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines)
      {
        routine.OnClose(screen,
                        dialogResult);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    public virtual void OnInitialize([NotNull] TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines)
      {
        routine.OnInitialize(screen);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <remarks>Should be used for funky UI stuff (like initial validation, initial focus, ... stuff ... :beers:)</remarks>
    public virtual async Task OnViewReadyAsync([NotNull] TScreen screen,
                                               [NotNull] object view)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (view == null)
      {
        throw new ArgumentNullException(nameof(view));
      }

      foreach (var routine in this.Routines)
      {
        await routine.OnViewReadyAsync(screen,
                                       view)
                     .ConfigureAwait(false);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to attach events</remarks>
    public virtual void OnActivate([NotNull] TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines)
      {
        routine.OnActivate(screen);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to detach events</remarks>
    public virtual void OnDeactivate([NotNull] TScreen screen,
                                     bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines)
      {
        routine.OnDeactivate(screen,
                             close);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [NotNull]
    public virtual TScreen BuildUp([NotNull] TScreen screen,
                                   [CanBeNull] object options = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      return screen;
    }
  }
}
