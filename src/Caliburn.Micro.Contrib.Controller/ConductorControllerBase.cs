using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class ConductorControllerBase<TScreen, TItem> : ControllerBase<TScreen>,
                                                                  IInterceptConductorEvents
    where TScreen : IScreen
    where TItem : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="mixinLocator" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ConductorControllerBase([NotNull] ILocator<object> mixinLocator,
                                      [NotNull] IScreenFactory screenFactory,
                                      [NotNull] [ItemNotNull] params ControllerRoutineBase[] controllerRoutines)
      : base(mixinLocator,
             screenFactory,
             controllerRoutines) {}

    [NotNull]
    public virtual IEnumerable<ConductorControllerRoutineBase> ConductorControllerRoutines => this.Routines.OfType<ConductorControllerRoutineBase>();

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [UsedImplicitly]
    [ScreenMethodLink(MethodName = nameof(IConductor.ActivateItem))]
    public virtual void OnActivateItem(IScreen screen,
                                       IScreen item)
    {
      this.OnActivateItem((TScreen) screen,
                          (TItem) item);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [UsedImplicitly]
    [ScreenMethodLink(MethodName = nameof(IConductor.DeactivateItem))]
    public virtual void OnDeactivateItem(IScreen screen,
                                         IScreen item,
                                         bool close)
    {
      this.OnDeactivateItem((TScreen) screen,
                            (TItem) item,
                            close);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    public virtual void OnActivateItem([NotNull] TScreen screen,
                                       [NotNull] TItem item)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      foreach (var conductorControllerRoutine in this.ConductorControllerRoutines)
      {
        conductorControllerRoutine.OnActivateItem(screen,
                                                  item);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    public virtual void OnDeactivateItem([NotNull] TScreen screen,
                                         [NotNull] TItem item,
                                         bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var conductorControllerRoutine in this.ConductorControllerRoutines)
      {
        conductorControllerRoutine.OnDeactivateItem(screen,
                                                    item,
                                                    close);
      }
    }
  }
}
