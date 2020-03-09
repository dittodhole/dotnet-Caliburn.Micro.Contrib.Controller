﻿using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;

namespace Caliburn.Micro.Contrib.Controller
{
  public abstract class ConductorControllerBase<TScreen, TItem> : IController,
                                                                  IScreenFactoryAdapter<TScreen>,
                                                                  IProvideScreenEventHandlers<TScreen>,
                                                                  IProvideConductorEventHandlers<TScreen, TItem>
    where TScreen : IScreen
    where TItem : IScreen
  {
    /// <exception cref="ArgumentNullException"/>
    protected ConductorControllerBase(IScreenFactory screenFactory,
                                      ICollection<IRoutine> routines)
    {
      this.ScreenFactory = screenFactory ?? throw new ArgumentNullException(nameof(screenFactory));
      this.Routines = routines ?? throw new ArgumentNullException(nameof(routines));
    }

    private IScreenFactory ScreenFactory { get; }
    private ICollection<IRoutine> Routines { get; }

    /// <inheritdoc/>
    [HandlesViewModelMethod(MethodName = nameof(IConductor.ActivateItem), CallBase = true)]
    public virtual void OnActivateItem(TScreen screen,
                                       TItem item)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines.OfType<IConductorRoutine>())
      {
        routine.OnActivateItem(screen,
                               item);
      }
    }

    /// <inheritdoc/>
    [HandlesViewModelMethod(MethodName = nameof(IConductor.DeactivateItem), CallBase = true)]
    public virtual void OnDeactivateItem(TScreen screen,
                                         TItem item,
                                         bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      foreach (var routine in this.Routines.OfType<IConductorRoutine>())
      {
        routine.OnDeactivateItem(screen,
                                 item,
                                 close);
      }
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public virtual TScreen CreateScreen(object? options = null)
    {
      var screenType = this.GetScreenType(options);
      var constructorArguments = this.GetConstructorArguments(screenType,
                                                              options);
      var screen = (TScreen) this.ScreenFactory.Create(screenType,
                                                       constructorArguments,
                                                       this);
      screen = this.BuildUp(screen,
                            options);

      return screen;
    }

    /// <inheritdoc/>
    IScreen IScreenFactoryAdapter.CreateScreen(object? options = null)
    {
      return this.CreateScreen(options);
    }

    /// <inheritdoc/>
    public virtual Type GetScreenType(object? options = null)
    {
      return typeof(TScreen);
    }

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public virtual object[] GetConstructorArguments(Type screenType,
                                                    object? options = null)
    {
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }

      return new object[0];
    }

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public virtual TScreen BuildUp(TScreen screen,
                                   object? options = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      return screen;
    }
  }
}
