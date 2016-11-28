using System.Collections.Generic;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface IInterceptHandleWithCoroutine<TMessage> : IInterceptHandle
  {
    [NotNull]
    [ItemNotNull]
    [ScreenMethodLink(SkipInvocation = true)]
    IEnumerable<IResult> Handle([NotNull] object screen,
                                [NotNull] TMessage message);
  }
}
