using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public static class Intercept<TScreen>
    where TScreen : IScreen
  {
    public interface IHandle {}

    public interface IHandle<TMessage> : IHandle
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" /></exception>
      void Handle([NotNull] TScreen screen,
                  [NotNull] TMessage message);
    }

    public interface IHandleWithCoroutine<TMessage> : IHandle
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" /></exception>
      [NotNull]
      [ItemNotNull]
      IEnumerable<IResult> Handle([NotNull] TScreen screen,
                                  [NotNull] TMessage message);
    }

    public interface IHandleWithTask<TMessage> : IHandle
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" /></exception>
      [NotNull]
      Task Handle([NotNull] TScreen screen,
                  [NotNull] TMessage message);
    }
  }
}
