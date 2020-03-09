using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caliburn.Micro.Contrib.Controller
{
  public static class Intercept<TScreen>
    where TScreen : IScreen
  {
    public interface IHandle : IMixinInterface<Micro.IHandle> { }

    public interface IHandle<TMessage> : IHandle,
                                         IMixinInterface<Micro.IHandle<TMessage>>
    {
      /// <exception cref="ArgumentNullException"/>
      /// <exception cref="Exception"/>
      void Handle(TScreen screen,
                  TMessage message);
    }

    public interface IHandleWithCoroutine<TMessage> : IHandle,
                                                      IMixinInterface<Micro.IHandleWithCoroutine<TMessage>>
    {
      /// <exception cref="ArgumentNullException"/>
      /// <exception cref="Exception"/>
      IEnumerable<IResult> Handle(TScreen screen,
                                  TMessage message);
    }

    public interface IHandleWithTask<TMessage> : IHandle,
                                                 IMixinInterface<Micro.IHandleWithTask<TMessage>>
    {
      /// <exception cref="ArgumentNullException"/>
      /// <exception cref="OperationCanceledException"/>
      /// <exception cref="Exception"/>
      Task Handle(TScreen screen,
                  TMessage message);
    }
  }
}
