using System;
using System.Collections.Generic;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IController
  {
    [NotNull]
    [ItemNotNull]
    IEnumerable<IRoutine> Routines { get; }

    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    IScreen CreateScreen([CanBeNull] object options = null);
  }
}
