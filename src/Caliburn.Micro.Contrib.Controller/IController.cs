using System;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface IController : IInterceptScreenEvents
  {
    /// <exception cref="InvalidOperationException" />
    [CanBeNull]
    IScreen CreateScreen([CanBeNull] object options = null);

    /// <exception cref="InvalidOperationException" />
    [NotNull]
    Type GetScreenType([CanBeNull] object options = null);

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutine" /> is <see langword="null" /></exception>
    [NotNull]
    T RegisterRoutine<T>([NotNull] T controllerRoutine) where T : IControllerRoutine;

    /// <exception cref="ArgumentNullException"><paramref name="controllerRoutine" /> is <see langword="null" /></exception>
    bool UnregisterRoutine([NotNull] IControllerRoutine controllerRoutine);
  }
}
