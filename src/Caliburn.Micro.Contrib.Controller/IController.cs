using System;
using System.Threading;
using System.Threading.Tasks;
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
    Task<object> GetResultAsync(CancellationToken cancellationToken);
  }
}
