using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface IInterceptHandleWithTask<TMessage> : IInterceptHandle
  {
    [NotNull]
    Task Handle([NotNull] IScreen screen,
                [NotNull] TMessage message);
  }
}
