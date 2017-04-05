using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IProvideResultAsync<T>
  {
    [NotNull]
    [ItemCanBeNull]
    Task<T> GetResultAsync(CancellationToken cancellationToken);
  }
}
