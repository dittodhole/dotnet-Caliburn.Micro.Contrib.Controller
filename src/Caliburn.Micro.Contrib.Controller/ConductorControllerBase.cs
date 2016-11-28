﻿using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public abstract class ConductorControllerBase<TScreen, TItem> : ControllerBase<TScreen>,
                                                                  IConductorController
    where TScreen : IScreen
    where TItem : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ConductorControllerBase([NotNull] [ItemNotNull] params IControllerRoutine[] controllerRoutines)
      : base(controllerRoutines) {}

    public virtual IEnumerable<IConductorControllerRoutine> ConductorControllerRoutines => this.ControllerRoutines.OfType<IConductorControllerRoutine>();

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [ScreenMethodLink(MethodName = nameof(IConductor.ActivateItem), SkipInvocation = false)]
    public void OnActivateItem(object screen,
                               object item)
    {
      this.OnActivateItem((TScreen) screen,
                          (TItem) item);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    [ScreenMethodLink(MethodName = nameof(IConductor.DeactivateItem), SkipInvocation = false)]
    public void OnDeactivateItem(object screen,
                                 object item,
                                 bool close)
    {
      this.OnDeactivateItem((TScreen) screen,
                            (TItem) item,
                            close);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    protected virtual void OnActivateItem([NotNull] TScreen screen,
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
    protected virtual void OnDeactivateItem([NotNull] TScreen screen,
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