using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caliburn.Micro.Contrib.Controller
{
  public static class Intercept<TScreen>
    where TScreen : IScreen
  {
    public interface IHandle : IMixinInterface<Micro.IHandle> {}

    public interface IHandle<TMessage> : IHandle,
                                         IMixinInterface<Micro.IHandle<TMessage>>
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" /></exception>
      void Handle(TScreen screen,
                  TMessage message);
    }

    public interface IHandleWithCoroutine<TMessage> : IHandle,
                                                      IMixinInterface<Micro.IHandleWithCoroutine<TMessage>>
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" /></exception>
      IEnumerable<IResult> Handle(TScreen screen,
                                  TMessage message);
    }

    public interface IHandleWithTask<TMessage> : IHandle,
                                                 IMixinInterface<Micro.IHandleWithTask<TMessage>>
    {
      /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="message" /> is <see langword="null" /></exception>
      Task Handle(TScreen screen,
                  TMessage message);
    }
  }
}
