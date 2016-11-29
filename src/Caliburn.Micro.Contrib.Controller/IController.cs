using System;
using System.Collections.Generic;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IController : IInterceptScreenEvents
  {
    [NotNull]
    [ItemNotNull]
    IEnumerable<IControllerRoutine> ControllerRoutines { get; }

    [CanBeNull]
    IScreen CreateScreen([CanBeNull] object options = null);

    [NotNull]
    Type GetScreenType([CanBeNull] object options = null);
  }
}
