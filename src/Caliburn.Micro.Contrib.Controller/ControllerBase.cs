﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Anotar.LibLog;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class ControllerBase : IController
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

    public virtual Task<object> GetResultAsync(CancellationToken cancellationToken)
    {
      return TaskEx.FromResult<object>(null);
    }

    public virtual IEnumerable<IRoutine> Routines { get; }

    IScreen IController.CreateScreen(object options = null)
    {
      throw new NotImplementedException();
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    public virtual void OnViewReady([NotNull] IScreen screen,
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
        try
        {
          routine.OnViewReadyAsync(screen,
                                   view)
                 .RunSynchronously();
        }
        catch (InvalidCastException invalidCastException)
        {
          LogTo.ErrorException($"Tried to call {nameof(IRoutine.OnViewReadyAsync)} on instance of {routine.GetType()}.",
                               invalidCastException);
        }
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [UsedImplicitly]
    public virtual void OnActivate([NotNull] IScreen screen)
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
    [UsedImplicitly]
    public virtual void OnDeactivate([NotNull] IScreen screen,
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
    [UsedImplicitly]
    public virtual void OnInitialize([NotNull] IScreen screen)
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
    [UsedImplicitly]
    public virtual void OnClose([NotNull] IScreen screen,
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
  }

  public abstract class ControllerBase<TScreen> : ControllerBase,
                                                  IController
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] IScreenFactory screenFactory,
                             [NotNull] [ItemNotNull] ICollection<IRoutine> routines)
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

    IScreen IController.CreateScreen(object options)
    {
      return this.CreateScreen(options);
    }

    /// <exception cref="ArgumentNullException">If <see cref="Type" /> returned by <see cref="GetScreenType" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    public virtual TScreen CreateScreen([CanBeNull] object options = null)
    {
      var screenType = this.GetScreenType(options);
      var screen = (TScreen) this.ScreenFactory.Create(screenType,
                                                       this);

      screen = this.BuildUp(screen,
                            options);

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
    [UsedImplicitly]
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
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    public virtual void OnInitialize([NotNull] TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="sceen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    public virtual void OnViewReady([NotNull] TScreen sceen,
                                    [NotNull] object view)
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
    public virtual void OnActivate([NotNull] TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
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
