using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
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

    public virtual IEnumerable<IRoutine> Routines { get; }

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

    public Type GetScreenType(object options = null) => typeof(TScreen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [HandlesViewModelMethod(MethodName = nameof(IClose.TryClose), CallBase = true)]
    public virtual void OnClose(TScreen screen,
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
    [HandlesViewModelMethod(MethodName = "OnInitialize", CallBase = true)]
    public virtual void OnInitialize(TScreen screen)
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
    [HandlesViewModelMethod(MethodName = "OnViewReady", CallBase = true)]
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

      foreach (var routine in this.Routines)
      {
        routine.OnViewReady(screen,
                            view);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <remarks>Should be used to attach events</remarks>
    [HandlesViewModelMethod(MethodName = "OnActivate", CallBase = true)]
    public virtual void OnActivate(TScreen screen)
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
    [HandlesViewModelMethod(MethodName = "OnDeactivate", CallBase = true)]
    public virtual void OnDeactivate(TScreen screen,
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
  }
}
