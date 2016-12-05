using System;
using System.Collections.Generic;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class ControllerBase : IController,
                                         IDisposable
  {
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] [ItemNotNull] params IControllerRoutine[] controllerRoutines)
      : this(new ScreenMetaTypesFinder(),
             controllerRoutines) {}

    /// <exception cref="ArgumentNullException"><paramref name="screenMetaTypesFinder" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] IScreenMetaTypesFinder screenMetaTypesFinder,
                             [NotNull] [ItemNotNull] params IControllerRoutine[] controllerRoutines)
    {
      if (screenMetaTypesFinder == null)
      {
        throw new ArgumentNullException(nameof(screenMetaTypesFinder));
      }
      if (controllerRoutines == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutines));
      }

      this.ScreenMetaTypesFinder = screenMetaTypesFinder;

      foreach (var controllerRoutine in controllerRoutines)
      {
        this.RegisterRoutine(controllerRoutine);
      }
    }

    [NotNull]
    [ItemNotNull]
    private ICollection<IControllerRoutine> Routines { get; } = new List<IControllerRoutine>();

    [NotNull]
    [ItemNotNull]
    public virtual IEnumerable<IControllerRoutine> ControllerRoutines => this.Routines;

    public abstract Type ScreenBaseType { get; }

    [NotNull]
    public virtual IScreenMetaTypesFinder ScreenMetaTypesFinder { get; }

    [UsedImplicitly]
    [ScreenMethodLink]
    public abstract void OnInitialize(IScreen screen);

    [UsedImplicitly]
    [ScreenMethodLink]
    public abstract void OnActivate(IScreen screen);

    [UsedImplicitly]
    [ScreenMethodLink]
    public abstract void OnDeactivate(IScreen screen,
                                      bool close);

    [UsedImplicitly]
    [ScreenMethodLink]
    public abstract void OnViewReady(IScreen screen,
                                     object view);

    [UsedImplicitly]
    [ScreenMethodLink(MethodName = nameof(IClose.TryClose))]
    public abstract void OnClose(IScreen screen,
                                 bool? dialogResult = null);

    public abstract Type GetScreenType(object options = null);

    /// <exception cref="InvalidOperationException" />
    public virtual IScreen CreateScreen(object options = null)
    {
      var screen = Controller.CreateScreenFn?.Invoke(this,
                                                     options);

      return screen;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutine" /> is <see langword="null" /></exception>
    public virtual T RegisterRoutine<T>(T controllerRoutine) where T : IControllerRoutine
    {
      if (controllerRoutine == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutine));
      }

      var mixinControllerRoutine = controllerRoutine as IMixinControllerRoutine;
      if (mixinControllerRoutine != null)
      {
        var screenBaseType = this.ScreenBaseType;
        var mixins = mixinControllerRoutine.GetMixins();
        this.ScreenMetaTypesFinder.RegisterMixinsForType(screenBaseType,
                                                         mixins);
      }

      this.Routines.Add(controllerRoutine);

      return controllerRoutine;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutine" /> is <see langword="null" /></exception>
    public virtual bool UnregisterRoutine(IControllerRoutine controllerRoutine)
    {
      if (controllerRoutine == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutine));
      }

      return this.Routines.Remove(controllerRoutine);
    }

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    public virtual void RegisterRoutines([NotNull] [ItemNotNull] IEnumerable<IControllerRoutine> controllerRoutines)
    {
      if (controllerRoutines == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutines));
      }

      foreach (var controllerRoutine in controllerRoutines)
      {
        this.RegisterRoutine(controllerRoutine);
      }
    }

    public virtual void Dispose()
    {
      foreach (var routine in this.Routines)
      {
        var disposable = routine as IDisposable;
        if (disposable != null)
        {
          disposable.Dispose();
        }
      }
    }
  }

  [PublicAPI]
  public abstract class ControllerBase<TScreen> : ControllerBase
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenMetaTypesFinder" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] IScreenMetaTypesFinder screenMetaTypesFinder,
                             [NotNull] [ItemNotNull] params IControllerRoutine[] controllerRoutines)
      : base(screenMetaTypesFinder,
             controllerRoutines) {}

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] [ItemNotNull] params IControllerRoutine[] controllerRoutines)
      : base(controllerRoutines) {}

    public override Type ScreenBaseType => typeof(TScreen);

    public override Type GetScreenType(object options = null) => typeof(TScreen);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnViewReady(IScreen screen,
                                     object view)
    {
      this.OnViewReady((TScreen) screen,
                       view);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnActivate(IScreen screen)
    {
      this.OnActivate((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnDeactivate(IScreen screen,
                                      bool close)
    {
      this.OnDeactivate((TScreen) screen,
                        close);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnInitialize(IScreen screen)
    {
      this.OnInitialize((TScreen) screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    public override void OnClose(IScreen screen,
                                 bool? dialogResult = null)
    {
      this.OnClose((TScreen) screen,
                   dialogResult);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnClose([NotNull] TScreen screen,
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

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnInitialize([NotNull] TScreen screen)
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

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
    public virtual void OnViewReady([NotNull] TScreen screen,
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

      foreach (var controllerRoutine in this.ControllerRoutines)
      {
        controllerRoutine.OnViewReady(screen,
                                      view);
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnActivate([NotNull] TScreen screen)
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

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnDeactivate([NotNull] TScreen screen,
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
  }
}
