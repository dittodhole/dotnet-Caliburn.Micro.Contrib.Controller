using System;
using System.Collections.Generic;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class ControllerBase : IController
  {
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] [ItemNotNull] params IControllerRoutine[] controllerRoutines)
    {
      if (controllerRoutines == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutines));
      }

      foreach (var controllerRoutine in controllerRoutines)
      {
        this.RegisterRoutine(controllerRoutine);
      }
    }

    [NotNull]
    [ItemNotNull]
    private ICollection<IControllerRoutine> Routines { get; } = new List<IControllerRoutine>();

    public virtual IEnumerable<IControllerRoutine> ControllerRoutines => this.Routines;

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

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutine" /> is <see langword="null" /></exception>
    protected virtual void RegisterRoutine([NotNull] IControllerRoutine controllerRoutine)
    {
      if (controllerRoutine == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutine));
      }

      this.Routines.Add(controllerRoutine);
    }

    public abstract Type GetScreenType(object options = null);

    public virtual IScreen CreateScreen(object options = null)
    {
      var screen = Controller.CreateScreenFn?.Invoke(this,
                                                     options);

      return screen;
    }
  }

  [PublicAPI]
  public abstract class ControllerBase<TScreen> : ControllerBase
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] [ItemNotNull] params IControllerRoutine[] controllerRoutines)
      : base(controllerRoutines) {}

    public override Type GetScreenType(object options = null) => typeof(TScreen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public sealed override void OnViewReady(IScreen screen,
                                            object view)
    {
      this.OnViewReady((TScreen) screen,
                       view);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public sealed override void OnActivate(IScreen screen)
    {
      this.OnActivate((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public sealed override void OnDeactivate(IScreen screen,
                                             bool close)
    {
      this.OnDeactivate((TScreen) screen,
                        close);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public sealed override void OnInitialize(IScreen screen)
    {
      this.OnInitialize((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public sealed override void OnClose(IScreen screen,
                                        bool? dialogResult = null)
    {
      this.OnClose((TScreen) screen,
                   dialogResult);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    protected virtual void OnClose([NotNull] TScreen screen,
                                   bool? dialogResult = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var controllerRoutine in this.ControllerRoutines)
      {
        controllerRoutine.OnClose(screen,
                                  dialogResult);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    protected virtual void OnInitialize([NotNull] TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var controllerRoutine in this.ControllerRoutines)
      {
        controllerRoutine.OnInitialize(screen);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    protected virtual void OnViewReady([NotNull] TScreen screen,
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

      foreach (var controllerRoutine in this.ControllerRoutines)
      {
        controllerRoutine.OnViewReady(screen,
                                      view);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    protected virtual void OnActivate([NotNull] TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var controllerRoutine in this.ControllerRoutines)
      {
        controllerRoutine.OnActivate(screen);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    protected virtual void OnDeactivate([NotNull] TScreen screen,
                                        bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var controllerRoutine in this.ControllerRoutines)
      {
        controllerRoutine.OnDeactivate(screen,
                                       close);
      }
    }
  }
}
