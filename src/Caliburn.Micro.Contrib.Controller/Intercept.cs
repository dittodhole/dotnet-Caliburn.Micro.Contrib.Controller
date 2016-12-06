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
    public interface IHandle : IScreenMixin<Micro.IHandle> {}

    public interface IHandle<TMessage> : IHandle,
                                         IScreenMixin<Micro.IHandle<TMessage>>
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" /></exception>
      [UsedImplicitly]
      void Handle([NotNull] TScreen screen,
                  [NotNull] TMessage message);
    }

    public interface IHandleWithCoroutine<TMessage> : IHandle,
                                                      IScreenMixin<Micro.IHandleWithCoroutine<TMessage>>
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" /></exception>
      [UsedImplicitly]
      [NotNull]
      [ItemNotNull]
      IEnumerable<IResult> Handle([NotNull] TScreen screen,
                                  [NotNull] TMessage message);
    }

    public interface IHandleWithTask<TMessage> : IHandle,
                                                 IScreenMixin<Micro.IHandleWithTask<TMessage>>
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" /></exception>
      [UsedImplicitly]
      [NotNull]
      Task Handle([NotNull] TScreen screen,
                  [NotNull] TMessage message);
    }
  }
}
