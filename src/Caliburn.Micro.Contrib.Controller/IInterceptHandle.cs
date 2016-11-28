using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface IInterceptHandle {}

  [PublicAPI]
  public interface IInterceptHandle<TMessage> : IInterceptHandle
  {
    void Handle([NotNull] object screen,
                [NotNull] TMessage message);
  }
}
