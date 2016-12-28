using System;
using System.Collections.Generic;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IController
  {
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    IScreen CreateScreen([CanBeNull] object options = null);

    [NotNull]
    [ItemNotNull]
    ICollection<IRoutine> Routines { get; }
  }
}
