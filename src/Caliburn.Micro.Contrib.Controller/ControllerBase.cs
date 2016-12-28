using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class ControllerBase : IInterceptScreenEvents,
                                         IDisposable
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] IScreenFactory screenFactory,
                             [NotNull] [ItemNotNull] params ControllerRoutineBase[] controllerRoutines)
    {
      if (screenFactory == null)
      {
        throw new ArgumentNullException(nameof(screenFactory));
      }
      if (controllerRoutines == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutines));
      }
      this.ScreenFactory = screenFactory;

      foreach (var controllerRoutine in controllerRoutines)
      {
        this.RegisterRoutine(controllerRoutine);
      }
    }

    [NotNull]
    private IScreenFactory ScreenFactory { get; }

    [NotNull]
    [ItemNotNull]
    public virtual IEnumerable<IInterceptScreenEvents> ScreenEventInterceptors => this.Routines.OfType<IInterceptScreenEvents>();

    [NotNull]
    [ItemNotNull]
    public virtual ICollection<ControllerRoutineBase> Routines { get; } = new List<ControllerRoutineBase>();

    public virtual void Dispose()
    {
      this.Routines.Clear();
    }

    [UsedImplicitly]
    [ScreenMethodLink]
    public abstract void OnInitialize(IScreen screen);

    [UsedImplicitly]
    [ScreenMethodLink]
    public abstract void OnActivate(IScreen screen);

    [UsedImplicitly]
    [ScreenMethodLink]
    public abstract void OnDeactivate(IScreen screen,
                                      bool close);

    [UsedImplicitly]
    [ScreenMethodLink]
    public abstract void OnViewReady(IScreen screen,
                                     object view);

    [UsedImplicitly]
    [ScreenMethodLink(MethodName = nameof(IClose.TryClose))]
    public abstract void OnClose(IScreen screen,
                                 bool? dialogResult = null);

    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    public virtual IScreen CreateScreen([CanBeNull] object options = null)
    {
      var screen = this.CreateScreenImpl(options);

      return screen;
    }

    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    public virtual IScreen CreateScreenImpl([CanBeNull] object options = null)
    {
      var screenType = this.GetScreenType(options);
      var screen = this.ScreenFactory.Create(screenType,
                                             this,
                                             options);

      return screen;
    }

    [Pure]
    [NotNull]
    public abstract Type GetScreenType([CanBeNull] object options = null);

    /// <exception cref="ArgumentNullException"><paramref name="routine" /> is <see langword="null" /></exception>
    [NotNull]
    public virtual T RegisterRoutine<T>([NotNull] T routine) where T : ControllerRoutineBase
    {
      if (routine == null)
      {
        throw new ArgumentNullException(nameof(routine));
      }

      this.Routines.Add(routine);

      return routine;
    }

    /// <exception cref="ArgumentNullException"><paramref name="routine" /> is <see langword="null" /></exception>
    public virtual bool UnregisterRoutine<T>([NotNull] T routine) where T : ControllerRoutineBase
    {
      if (routine == null)
      {
        throw new ArgumentNullException(nameof(routine));
      }

      return this.Routines.Remove(routine);
    }

    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    public virtual void RegisterRoutines<T>([NotNull] [ItemNotNull] IEnumerable<T> routines) where T : ControllerRoutineBase
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
  }

  [PublicAPI]
  public abstract class ControllerBase<TScreen> : ControllerBase
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] IScreenFactory screenFactory,
                             [NotNull] [ItemNotNull] params ControllerRoutineBase[] controllerRoutines)
      : base(screenFactory,
             controllerRoutines) {}

    public override Type GetScreenType(object options = null) => typeof(TScreen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnViewReady(IScreen screen,
                                     object view)
    {
      this.OnViewReady((TScreen) screen,
                       view);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnActivate(IScreen screen)
    {
      this.OnActivate((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnDeactivate(IScreen screen,
                                      bool close)
    {
      this.OnDeactivate((TScreen) screen,
                        close);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnInitialize(IScreen screen)
    {
      this.OnInitialize((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnClose(IScreen screen,
                                 bool? dialogResult = null)
    {
      this.OnClose((TScreen) screen,
                   dialogResult);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnClose([NotNull] TScreen screen,
                                bool? dialogResult = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var screenEventInterceptor in this.ScreenEventInterceptors)
      {
        screenEventInterceptor.OnClose(screen,
                                       dialogResult);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnInitialize([NotNull] TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var screenEventInterceptor in this.ScreenEventInterceptors)
      {
        screenEventInterceptor.OnInitialize(screen);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
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

      foreach (var screenEventInterceptor in this.ScreenEventInterceptors)
      {
        screenEventInterceptor.OnViewReady(screen,
                                           view);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnActivate([NotNull] TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var screenEventInterceptor in this.ScreenEventInterceptors)
      {
        screenEventInterceptor.OnActivate(screen);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnDeactivate([NotNull] TScreen screen,
                                     bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var screenEventInterceptor in this.ScreenEventInterceptors)
      {
        screenEventInterceptor.OnDeactivate(screen,
                                            close);
      }
    }

    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    public new virtual TScreen CreateScreen([CanBeNull] object options = null)
    {
      var screen = this.CreateConcreteScreenImpl(options);

      return screen;
    }

    public override IScreen CreateScreenImpl(object options = null)
    {
      var screen = this.CreateConcreteScreenImpl(options);

      return screen;
    }

    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    public virtual TScreen CreateConcreteScreenImpl([CanBeNull] object options = null)
    {
      var screen = (TScreen) base.CreateScreenImpl(options);

      return screen;
    }
  }
}
