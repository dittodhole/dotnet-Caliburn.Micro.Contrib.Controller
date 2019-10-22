using System;
using System.Collections.Generic;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public abstract class ControllerBase<TScreen> : IController,
                                                  IScreenFactoryAdapter<TScreen>,
                                                  IProvideScreenEventHandlers<TScreen>
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] IScreenFactory screenFactory,
                             [NotNull] [ItemNotNull] ICollection<IRoutine> routines)
    {
      this.ScreenFactory = screenFactory ?? throw new ArgumentNullException(nameof(screenFactory));
      this.Routines = routines ?? throw new ArgumentNullException(nameof(routines));
    }

    [NotNull]
    private IScreenFactory ScreenFactory { get; }

    public virtual IEnumerable<IRoutine> Routines { get; }

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

    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    public virtual TScreen CreateScreen(object options = null)
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

    IScreen IScreenFactoryAdapter.CreateScreen(object options)
    {
      return this.CreateScreen(options);
    }

    public virtual Type GetScreenType(object options = null)
    {
      return typeof(TScreen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    [PublicAPI]
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

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    [PublicAPI]
    [NotNull]
    public virtual object[] GetConstructorArguments([NotNull] Type screenType,
                                                    [CanBeNull] object options = null)
    {
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }

      return new object[0];
    }
  }
}
