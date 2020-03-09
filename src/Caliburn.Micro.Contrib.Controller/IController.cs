using System.Collections.Generic;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IController
  {
    IEnumerable<IRoutine> Routines { get; }
  }
}
