using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IController
  {
    [NotNull]
    [ItemNotNull]
    ICollection<IRoutine> Routines { get; } // TODO

    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    IScreen CreateScreen([CanBeNull] object options = null);

    [NotNull]
    Task<object> GetResultAsync(CancellationToken cancellationToken);
  }
}
