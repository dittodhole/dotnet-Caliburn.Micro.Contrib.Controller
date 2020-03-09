using System;
using System.Threading;
using System.Threading.Tasks;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IProvideResultAsync<T>
  {
    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="Exception"/>
    Task<T> GetResultAsync(CancellationToken cancellationToken);
  }
}
