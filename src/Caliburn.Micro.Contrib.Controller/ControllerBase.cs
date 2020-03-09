﻿using System;
using System.Collections.Generic;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IController { }

  public interface IControllerRoutine : IProvideScreenEventHandlers<IScreen> { }

  public abstract class ControllerBase<TScreen> : IController,
                                                  IScreenFactoryAdapter<TScreen>,
                                                  IProvideScreenEventHandlers<TScreen>
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"/>
    protected ControllerBase(IScreenFactory screenFactory,
                             ICollection<IControllerRoutine> controllerRoutines)
    {
      this.ScreenFactory = screenFactory ?? throw new ArgumentNullException(nameof(screenFactory));
      this.ControllerRoutines = controllerRoutines ?? throw new ArgumentNullException(nameof(controllerRoutines));
    }

    private IScreenFactory ScreenFactory { get; }
    private ICollection<IControllerRoutine> ControllerRoutines { get; }

    /// <inheritdoc/>
    [HandlesViewModelMethod(MethodName = nameof(IClose.TryClose), CallBase = true)]
    public virtual void OnClose(TScreen screen,
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

    /// <inheritdoc/>
    [HandlesViewModelMethod(MethodName = "OnInitialize", CallBase = true)]
    public virtual void OnInitialize(TScreen screen)
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

      foreach (var controllerRoutine in this.ControllerRoutines)
      {
        controllerRoutine.OnViewReady(screen,
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

      foreach (var controllerRoutine in this.ControllerRoutines)
      {
        controllerRoutine.OnActivate(screen);
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

      foreach (var controllerRoutine in this.ControllerRoutines)
      {
        controllerRoutine.OnDeactivate(screen,
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
