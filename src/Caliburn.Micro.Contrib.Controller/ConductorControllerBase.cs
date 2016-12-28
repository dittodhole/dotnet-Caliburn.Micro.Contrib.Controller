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
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ConductorControllerBase([NotNull] IScreenFactory screenFactory,
                                      [NotNull] [ItemNotNull] params IRoutine[] routines)
      : base(screenFactory,
             routines) {}

    [NotNull]
    public virtual IEnumerable<IInterceptConductorEvents> ConductorEventInterceptors => this.Routines.OfType<IInterceptConductorEvents>();

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

      foreach (var conductorEventInterceptor in this.ConductorEventInterceptors)
      {
        conductorEventInterceptor.OnActivateItem(screen,
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

      foreach (var conductorEventInterceptor in this.ConductorEventInterceptors)
      {
        conductorEventInterceptor.OnDeactivateItem(screen,
                                                   item,
                                                   close);
      }
    }
  }
}
