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
  }
}
