using System.Collections.Generic;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IConductorController : IController,
                                          IInterceptConductorEvents
  {
    [NotNull]
    [ItemNotNull]
    IEnumerable<IConductorControllerRoutine> ConductorControllerRoutines { get; }
  }
}
