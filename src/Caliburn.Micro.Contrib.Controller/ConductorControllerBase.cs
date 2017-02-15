using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public abstract class ConductorControllerBase : ControllerBase,
                                                  IProvideConductorEventHandlers
  {
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ConductorControllerBase([NotNull] ICollection<IRoutine> routines)
      : base(routines) {}

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    public virtual void OnActivateItem(IScreen screen,
                                       IScreen item)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      foreach (var routine in this.Routines.OfType<IConductorRoutine>())
      {
        routine.OnActivateItem(screen,
                               item);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    public virtual void OnDeactivateItem(IScreen screen,
                                         IScreen item,
                                         bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      foreach (var routine in this.Routines.OfType<IConductorRoutine>())
      {
        routine.OnDeactivateItem(screen,
                                 item,
                                 close);
      }
    }
  }

  public abstract class ConductorControllerBase<TScreen, TItem> : ConductorControllerBase,
                                                                  IController<TScreen>,
                                                                  IProvideScreenEventHandlers<TScreen>,
                                                                  IProvideConductorEventHandlers<TScreen, TItem>
    where TScreen : IScreen
    where TItem : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ConductorControllerBase([NotNull] [ItemNotNull] ICollection<IRoutine> routines)
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
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    [HandlesEvent(MethodName = nameof(IConductor.ActivateItem), CallBase = true)]
    public virtual void OnActivateItem(TScreen screen,
                                       TItem item)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      base.OnActivateItem(screen,
                          item);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    [HandlesEvent(MethodName = nameof(IConductor.DeactivateItem), CallBase = true)]
    public virtual void OnDeactivateItem(TScreen screen,
                                         TItem item,
                                         bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      base.OnDeactivateItem(screen,
                            item,
                            close);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [HandlesEvent(MethodName = nameof(IClose.TryClose), CallBase = true)]
    public virtual void OnClose(TScreen screen,
                                bool? dialogResult = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      base.OnClose(screen,
                   dialogResult);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    [HandlesEvent(MethodName = "OnInitialize", CallBase = true)]
    public virtual void OnInitialize(TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      base.OnInitialize(screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    [HandlesEvent(MethodName = "OnViewReady", CallBase = true)]
    public virtual void OnViewReady(TScreen screen,
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

      base.OnViewReady(screen,
                       view);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to attach events</remarks>
    [HandlesEvent(MethodName = "OnActivate", CallBase = true)]
    public virtual void OnActivate(TScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      base.OnActivate(screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to detach events</remarks>
    [HandlesEvent(MethodName = "OnDeactivate", CallBase = true)]
    public virtual void OnDeactivate(TScreen screen,
                                     bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      base.OnDeactivate(screen,
                        close);
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
