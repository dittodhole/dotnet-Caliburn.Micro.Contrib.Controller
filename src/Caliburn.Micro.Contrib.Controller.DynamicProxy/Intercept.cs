using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IMixinProvider { }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IMixinInterface<TInterface> : IMixinProvider { }

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
