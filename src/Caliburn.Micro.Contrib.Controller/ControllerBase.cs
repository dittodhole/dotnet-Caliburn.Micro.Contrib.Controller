﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Caliburn.Micro.Contrib.Controller
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IController { }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IControllerRoutine : IProvideScreenEventHandlers<IScreen> { }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0049:Type name should not match namespace", Justification = "<Pending>")]
  public static class Controller
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="InvalidOperationException"/>
    /// <exception cref="Exception"/>
    public static MethodInfo? GetInterceptingMethodInfo(IController controller,
                                                        BindingFlags bindingFlags,
                                                        string methodName)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }
      if (methodName == null)
      {
        throw new ArgumentNullException(nameof(methodName));
      }

      var result = controller.GetType()
                             .FindMembers(MemberTypes.Method,
                                          bindingFlags,
                                          (memberInfo,
                                           _) =>
                                          {
                                            if (memberInfo is MethodInfo methodInfo)
                                            {
                                              return methodInfo.GetAttributes<HandlesViewModelMethodAttribute>(true)
                                                               .Any(arg => StringComparer.Ordinal.Equals(arg.MethodName ?? methodInfo.Name,
                                                                                                         methodName));
                                            }
                                            return false;
                                          },
                                          null)
                             .Cast<MethodInfo>()
                             .SingleOrDefault();

      return result;
    }
  }

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
    [HandlesViewModelMethod(MethodName = nameof(IClose.TryClose))]
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
    [HandlesViewModelMethod(MethodName = "OnInitialize")]
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
    [HandlesViewModelMethod(MethodName = "OnViewReady")]
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
    [HandlesViewModelMethod(MethodName = "OnActivate")]
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
    [HandlesViewModelMethod(MethodName = "OnDeactivate")]
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
