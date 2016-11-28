using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface IInterceptHandleWithTask<TMessage> : IInterceptHandle
  {
    [NotNull]
    Task Handle([NotNull] object screen,
                [NotNull] TMessage message);
  }
}
