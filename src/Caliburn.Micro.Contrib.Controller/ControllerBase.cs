using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anotar.LibLog;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public abstract class ControllerBase : IController,
                                         IProvideScreenEventHandlers
  {
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] [ItemNotNull] ICollection<IRoutine> routines)
    {
      if (routines == null)
      {
        throw new ArgumentNullException(nameof(routines));
      }
      this.Routines = routines;
    }

    public virtual IEnumerable<IRoutine> Routines { get; }

    public abstract Type GetScreenType(object options = null);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual IScreen BuildUp(IScreen screen,
                                   object options = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      return screen;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    [HandlesEvent(MethodName = "OnViewReady", CallBase = true)]
    public virtual void OnViewReady(IScreen screen,
                                    object view)
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
        try
        {
          routine.OnViewReady(screen,
                              view);
        }
        catch (InvalidCastException invalidCastException)
        {
          LogTo.ErrorException($"Tried to call {nameof(IRoutine.OnViewReady)} on instance of {routine.GetType()}.",
                               invalidCastException);
        }
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [HandlesEvent(MethodName = "OnActivate", CallBase = true)]
    public virtual void OnActivate(IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines)
      {
        try
        {
          routine.OnActivate(screen);
        }
        catch (InvalidCastException invalidCastException)
        {
          LogTo.ErrorException($"Tried to call {nameof(IRoutine.OnActivate)} on instance of {routine.GetType()}.",
                               invalidCastException);
        }
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [HandlesEvent(MethodName = "OnDeactivate", CallBase = true)]
    public virtual void OnDeactivate(IScreen screen,
                                     bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines)
      {
        try
        {
          routine.OnDeactivate(screen,
                               close);
        }
        catch (InvalidCastException invalidCastException)
        {
          LogTo.ErrorException($"Tried to call {nameof(IRoutine.OnDeactivate)} on instance of {routine.GetType()}.",
                               invalidCastException);
        }
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [HandlesEvent(MethodName = "OnInitialize", CallBase = true)]
    public virtual void OnInitialize(IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines)
      {
        try
        {
          routine.OnInitialize(screen);
        }
        catch (InvalidCastException invalidCastException)
        {
          LogTo.ErrorException($"Tried to call {nameof(IRoutine.OnInitialize)} on instance of {routine.GetType()}.",
                               invalidCastException);
        }
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [HandlesEvent(MethodName = nameof(IClose.TryClose), CallBase = true)]
    public virtual void OnClose(IScreen screen,
                                bool? dialogResult = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines)
      {
        try
        {
          routine.OnClose(screen,
                          dialogResult);
        }
        catch (InvalidCastException invalidCastException)
        {
          LogTo.ErrorException($"Tried to call {nameof(IRoutine.OnClose)} on instance of {routine.GetType()}.",
                               invalidCastException);
        }
      }
    }

    // ReSharper disable UnusedMember.Global
    [NotNull]
    [ItemCanBeNull]
    public virtual Task<object> GetResultAsync(CancellationToken cancellationToken)
    {
      return TaskEx.FromResult<object>(null);
    }

    // ReSharper restore UnusedMember.Global
  }

  public abstract class ControllerBase<TScreen> : ControllerBase,
                                                  IController<TScreen>,
                                                  IProvideScreenEventHandlers<TScreen>
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] [ItemNotNull] ICollection<IRoutine> routines)
      : base(routines) {}

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual TScreen BuildUp(TScreen screen,
                                   object options = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      return screen;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnViewReady(IScreen screen,
                                     object view)
    {
      base.OnViewReady(screen,
                       view);

      this.OnViewReady((TScreen) screen,
                       view);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnActivate(IScreen screen)
    {
      base.OnActivate(screen);

      this.OnActivate((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnDeactivate(IScreen screen,
                                      bool close)
    {
      base.OnDeactivate(screen,
                        close);

      this.OnDeactivate((TScreen) screen,
                        close);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnInitialize(IScreen screen)
    {
      base.OnInitialize(screen);

      this.OnInitialize((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnClose(IScreen screen,
                                 bool? dialogResult = null)
    {
      base.OnClose(screen,
                   dialogResult);

      this.OnClose((TScreen) screen,
                   dialogResult);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnClose(TScreen screen,
                                bool? dialogResult = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    public virtual void OnInitialize(TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="sceen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    public virtual void OnViewReady(TScreen sceen,
                                    object view)
    {
      if (sceen == null)
      {
        throw new ArgumentNullException(nameof(sceen));
      }
      if (view == null)
      {
        throw new ArgumentNullException(nameof(view));
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to attach events</remarks>
    public virtual void OnActivate(TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to detach events</remarks>
    public virtual void OnDeactivate(TScreen screen,
                                     bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    public override Type GetScreenType(object options = null) => typeof(TScreen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override IScreen BuildUp(IScreen screen,
                                    object options = null)
    {
      screen = base.BuildUp(screen,
                            options);

      screen = this.BuildUp((TScreen) screen,
                            options);

      return screen;
    }
  }
}
