using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Caliburn.Micro.Contrib.Controller
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IController
  {
    /// <exception cref="Exception"/>
    Type GetScreenType(object? options = null);

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    object?[] GetScreenConstructorArguments(Type type,
                                            object? options = null);
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IController<TScreen> : IController where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    TScreen InitializeScreen(TScreen screen,
                             object? options = null);
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IHandleScreenEvents<TScreen>
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    /// <remarks>Should be used to prepare <paramref name="screen" /></remarks>
    void OnInitialize(TScreen screen);

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    void OnViewReady(TScreen screen,
                     object view);

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    /// <remarks>Should be used to attach events</remarks>
    void OnActivate(TScreen screen);

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    void OnClose(TScreen screen,
                 bool? dialogResult = null);

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    /// <remarks>Should be used to detach events</remarks>
    void OnDeactivate(TScreen screen,
                      bool close);
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IControllerRoutine : IHandleScreenEvents<IScreen> { }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IHandleConductorEvents<TScreen, TItem>
    where TScreen : IScreen
    where TItem : IScreen
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    void OnActivateItem(TScreen screen,
                        TItem item);

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    void OnDeactivateItem(TScreen screen,
                          TItem item,
                          bool close);
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IConductorControllerRoutine : IControllerRoutine,
                                                 IHandleConductorEvents<IScreen, IScreen> { }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0049:Type name should not match namespace", Justification = "<Pending>")]
  public static class Controller
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public static MethodInfo? GetInterceptingMethodInfo(IController controller,
                                                        BindingFlags bindingFlags,
                                                        string methodName,
                                                        Type returnType,
                                                        ParameterInfo[] parameterInfos)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }
      if (methodName == null)
      {
        throw new ArgumentNullException(nameof(methodName));
      }
      if (returnType == null)
      {
        throw new ArgumentNullException(nameof(returnType));
      }
      if (parameterInfos == null)
      {
        throw new ArgumentNullException(nameof(parameterInfos));
      }

      var result = controller.GetType()
                             .FindMembers(MemberTypes.Method,
                                          bindingFlags,
                                          (memberInfo,
                                           _) =>
                                          {
                                            if (memberInfo is MethodInfo methodInfo)
                                            {
                                              var attributes = methodInfo.GetAttributes<HandlesViewModelMethodAttribute>(true);
                                              if (attributes.Any(attribute => StringComparer.Ordinal.Equals(attribute.MethodName ?? methodInfo.Name,
                                                                                                            methodName)))
                                              if (methodInfo.ReturnType == returnType)
                                              if (methodInfo.GetParameters()
                                                            .Skip(1)
                                                            .SequenceEqual(parameterInfos))
                                              {
                                                return true;
                                              }
                                            }
                                            return false;
                                          },
                                          null)
                             .Cast<MethodInfo>()
                             .SingleOrDefault();

      return result;
    }
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public abstract class ControllerBase<TScreen> : IController<TScreen>,
                                                  IHandleScreenEvents<TScreen>
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"/>
    protected ControllerBase(ICollection<IControllerRoutine> controllerRoutines)
    {
      this.ControllerRoutines = controllerRoutines ?? throw new ArgumentNullException(nameof(controllerRoutines));
    }

    private ICollection<IControllerRoutine> ControllerRoutines { get; }

    /// <inheritdoc/>
    public virtual Type GetScreenType(object? options = null)
    {
      return typeof(TScreen);
    }

    /// <inheritdoc/>
    public virtual object?[] GetScreenConstructorArguments(Type type,
                                                           object? options = null)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      return new object[0];
    }

    /// <inheritdoc/>
    public virtual TScreen InitializeScreen(TScreen screen,
                                            object? options = null)
    {
      return screen ?? throw new ArgumentNullException(nameof(screen));
    }

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
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public abstract class ControllerWithResultBase<TScreen, TResult> : ControllerBase<TScreen>
    where TScreen : IScreen
  {
    /// <inheritdoc/>
    protected ControllerWithResultBase(ICollection<IControllerRoutine> routines)
      : base(routines) { }

    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="Exception"/>
    public abstract Task<TResult> GetResultAsync(CancellationToken cancellationToken);
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public abstract class ConductorControllerBase<TScreen, TItem> : IController<TScreen>,
                                                                  IHandleScreenEvents<TScreen>,
                                                                  IHandleConductorEvents<TScreen, TItem>
    where TScreen : IScreen
    where TItem : IScreen
  {
    /// <exception cref="ArgumentNullException"/>
    protected ConductorControllerBase(ICollection<IControllerRoutine> controllerRoutines)
    {
      this.ControllerRoutines = controllerRoutines ?? throw new ArgumentNullException(nameof(controllerRoutines));
    }

    private ICollection<IControllerRoutine> ControllerRoutines { get; }

    /// <inheritdoc/>
    public virtual Type GetScreenType(object? options = null)
    {
      return typeof(TScreen);
    }

    /// <inheritdoc/>
    public virtual object?[] GetScreenConstructorArguments(Type type,
                                                           object? options = null)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      return new object[0];
    }

    /// <inheritdoc/>
    public virtual TScreen InitializeScreen(TScreen screen,
                                            object? options = null)
    {
      return screen ?? throw new ArgumentNullException(nameof(screen));
    }

    /// <inheritdoc/>
    [HandlesViewModelMethod(MethodName = nameof(IConductor.ActivateItem))]
    public virtual void OnActivateItem(TScreen screen,
                                       TItem item)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      foreach (var conductorControllerRoutine in this.ControllerRoutines.OfType<IConductorControllerRoutine>())
      {
        conductorControllerRoutine.OnActivateItem(screen,
                                                  item);
      }
    }

    /// <inheritdoc/>
    [HandlesViewModelMethod(MethodName = nameof(IConductor.DeactivateItem))]
    public virtual void OnDeactivateItem(TScreen screen,
                                         TItem item,
                                         bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }

      foreach (var conductorControllerRoutine in this.ControllerRoutines.OfType<IConductorControllerRoutine>())
      {
        conductorControllerRoutine.OnDeactivateItem(screen,
                                                    item,
                                                    close);
      }
    }

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
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public static class ControllerExtension
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public static TScreen CreateScreen<TScreen>(this IController<TScreen> controller,
                                                object? options = null) where TScreen : IScreen
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var type = controller.GetScreenType(options);
      var args = controller.GetScreenConstructorArguments(type,
                                                          options);

      var screenFactory = IoC.Get<IScreenFactory>();

      var screen = screenFactory.With(controller)
                                .CreateScreen(type,
                                              args);

      var result = controller.InitializeScreen((TScreen) screen,
                                               options);

      return result;
    }

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public static TScreen ShowWindow<TScreen>(this IController<TScreen> controller,
                                              object? options = null,
                                              object? context = null,
                                              IDictionary<string, object>? settings = null) where TScreen : IScreen
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var result = controller.CreateScreen(options);

      var windowManager = IoC.Get<IWindowManager>();

      Execute.BeginOnUIThread(() => windowManager.ShowWindow(result,
                                                             context,
                                                             settings));

      return result;
    }

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public static TScreen ShowDialog<TScreen>(this IController<TScreen> controller,
                                              object? options = null,
                                              object? context = null,
                                              IDictionary<string, object>? settings = null) where TScreen : IScreen
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var result = controller.CreateScreen(options);

      var windowManager = IoC.Get<IWindowManager>();

      Execute.BeginOnUIThread(() => windowManager.ShowDialog(result,
                                                             context,
                                                             settings));

      return result;
    }
  }
}
