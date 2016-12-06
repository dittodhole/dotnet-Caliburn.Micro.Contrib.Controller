using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class ControllerBase : IDisposable,
                                         IInterceptScreenEvents
  {
    /// <exception cref="ArgumentNullException"><paramref name="mixinLocator" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] ILocator<object> mixinLocator,
                             [NotNull] IScreenFactory screenFactory,
                             [NotNull] [ItemNotNull] params ControllerRoutineBase[] controllerRoutines)
    {
      if (mixinLocator == null)
      {
        throw new ArgumentNullException(nameof(mixinLocator));
      }
      if (screenFactory == null)
      {
        throw new ArgumentNullException(nameof(screenFactory));
      }
      if (controllerRoutines == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutines));
      }
      this.MixinLocator = mixinLocator;
      this.ScreenFactory = screenFactory;

      foreach (var controllerRoutine in controllerRoutines)
      {
        this.RegisterRoutine(controllerRoutine);
      }
    }

    [NotNull]
    private ILocator<object> MixinLocator { get; }

    [NotNull]
    private IScreenFactory ScreenFactory { get; }

    [NotNull]
    [ItemNotNull]
    public virtual IEnumerable<ControllerRoutineBase> Routines => this.ControllerRoutines;

    [NotNull]
    [ItemNotNull]
    private ICollection<ControllerRoutineBase> ControllerRoutines { get; } = new List<ControllerRoutineBase>();

    public virtual void Dispose()
    {
      this.ControllerRoutines.Clear();
    }

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

    /// <exception cref="InvalidOperationException" />
    [NotNull]
    public virtual IScreen CreateScreen([CanBeNull] object options = null)
    {
      var mixinSources = new object[]
                         {
                           this
                         }.Concat(this.Routines);
      var screenMixinTypes = mixinSources.Select(arg => arg.GetType())
                                         .SelectMany(arg => arg.GetInterfaces())
                                         .Distinct()
                                         .Where(arg => arg.IsDescendant<IScreenMixin>())
                                         .Where(arg => arg.IsGenericType)
                                         .Select(arg => new
                                                        {
                                                          GenericTypeDefinition = arg.GetGenericTypeDefinition(),
                                                          GenericArguments = arg.GetGenericArguments()
                                                        })
                                         .Where(arg => arg.GenericTypeDefinition == typeof(IScreenMixin<>))
                                         .SelectMany(arg => arg.GenericArguments)
                                         .ToArray();
      var additionalInterfaces = screenMixinTypes.Where(arg => arg.IsInterface)
                                                 .ToArray();
      var mixinInstances = screenMixinTypes.Select(this.MixinLocator.LocateOptional)
                                           .Where(arg => arg != null)
                                           .ToArray();
      var customAttributeBuilders = mixinSources.OfType<IScreenAttributesMixin>()
                                                .SelectMany(arg => arg.GetCustomAttributeBuilders())
                                                .ToArray();

      var screenType = this.GetScreenType(options);
      var screen = this.ScreenFactory.Create(screenType,
                                             additionalInterfaces,
                                             mixinInstances,
                                             customAttributeBuilders,
                                             this);

      return screen;
    }

    [NotNull]
    public abstract Type GetScreenType([CanBeNull] object options = null);

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutine" /> is <see langword="null" /></exception>
    [NotNull]
    public virtual T RegisterRoutine<T>([NotNull] T controllerRoutine) where T : ControllerRoutineBase
    {
      if (controllerRoutine == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutine));
      }

      this.ControllerRoutines.Add(controllerRoutine);

      return controllerRoutine;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutine" /> is <see langword="null" /></exception>
    public virtual bool UnregisterRoutine([NotNull] ControllerRoutineBase controllerRoutine)
    {
      if (controllerRoutine == null)
      {
        throw new ArgumentNullException(nameof(controllerRoutine));
      }

      return this.ControllerRoutines.Remove(controllerRoutine);
    }

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    public virtual void RegisterRoutines([NotNull] [ItemNotNull] IEnumerable<ControllerRoutineBase> controllerRoutines)
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
  }

  [PublicAPI]
  public abstract class ControllerBase<TScreen> : ControllerBase
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="mixinLocator" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutines" /> is <see langword="null" /></exception>
    protected ControllerBase([NotNull] ILocator<object> mixinLocator,
                             [NotNull] IScreenFactory screenFactory,
                             [NotNull] [ItemNotNull] params ControllerRoutineBase[] controllerRoutines)
      : base(mixinLocator,
             screenFactory,
             controllerRoutines) {}

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

      foreach (var controllerRoutine in this.Routines)
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

      foreach (var controllerRoutine in this.Routines)
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

      foreach (var controllerRoutine in this.Routines)
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

      foreach (var controllerRoutine in this.Routines)
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

      foreach (var controllerRoutine in this.Routines)
      {
        controllerRoutine.OnDeactivate(screen,
                                       close);
      }
    }
  }
}
