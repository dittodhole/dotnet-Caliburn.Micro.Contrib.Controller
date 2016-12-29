using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class ControllerBase : IController
  {
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] [ItemNotNull] params IRoutine[] routines)
    {
      if (routines == null)
      {
        throw new ArgumentNullException(nameof(routines));
      }

      foreach (var controllerRoutine in routines)
      {
        this.RegisterRoutine(controllerRoutine);
      }
    }

    public virtual ICollection<IRoutine> Routines { get; } = new List<IRoutine>();

    public abstract IScreen CreateScreen(object options = null);

    /// <exception cref="ArgumentNullException"><paramref name="routine" /> is <see langword="null" /></exception>
    [NotNull]
    public virtual T RegisterRoutine<T>([NotNull] T routine) where T : IRoutine
    {
      if (routine == null)
      {
        throw new ArgumentNullException(nameof(routine));
      }

      this.Routines.Add(routine);

      return routine;
    }

    /// <exception cref="ArgumentNullException"><paramref name="routine" /> is <see langword="null" /></exception>
    public virtual bool UnregisterRoutine<T>([NotNull] T routine) where T : IRoutine
    {
      if (routine == null)
      {
        throw new ArgumentNullException(nameof(routine));
      }

      return this.Routines.Remove(routine);
    }

    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    public virtual void RegisterRoutines([NotNull] [ItemNotNull] IEnumerable<IRoutine> routines)
    {
      if (routines == null)
      {
        throw new ArgumentNullException(nameof(routines));
      }

      foreach (var routine in routines)
      {
        this.RegisterRoutine(routine);
      }
    }
  }

  [PublicAPI]
  public abstract class ControllerBase<TScreen> : ControllerBase
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] IScreenFactory screenFactory,
                             [NotNull] [ItemNotNull] params IRoutine[] routines)
      : base(routines)
    {
      if (screenFactory == null)
      {
        throw new ArgumentNullException(nameof(screenFactory));
      }
      this.ScreenFactory = screenFactory;
    }

    [NotNull]
    private IScreenFactory ScreenFactory { get; }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [UsedImplicitly]
    [ScreenMethodLink(MethodName = "OnViewReady", CallBase = true)]
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

      this.OnViewReady((TScreen) screen,
                       view);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [UsedImplicitly]
    [ScreenMethodLink(MethodName = "OnActivate", CallBase = true)]
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
    [ScreenMethodLink(MethodName = "OnDeactivate", CallBase = true)]
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
    [ScreenMethodLink(MethodName = "OnInitialize", CallBase = true)]
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
    [ScreenMethodLink(MethodName = nameof(IClose.TryClose), CallBase = true)]
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
    protected virtual Type GetScreenType([CanBeNull] object options = null) => typeof(TScreen);

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
    public virtual void OnViewReady([NotNull] TScreen screen,
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
        routine.OnViewReady(screen,
                            view);
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

    public override IScreen CreateScreen(object options = null)
    {
      var screen = this.CreateScreenImpl(options);

      return screen;
    }

    [NotNull]
    public virtual TScreen CreateScreenImpl([CanBeNull] object options = null)
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
