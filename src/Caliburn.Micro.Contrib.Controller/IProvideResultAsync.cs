using System.Threading;
using System.Threading.Tasks;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IProvideResultAsync<T>
  {
    Task<T> GetResultAsync(CancellationToken cancellationToken);
  }
}
