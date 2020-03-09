using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Caliburn.Micro.Contrib.Controller
{
  public abstract class ControllerWithResultBase<TScreen, TResult> : ControllerBase<TScreen>,
                                                                     IProvideResultAsync<TResult>
    where TScreen : IScreen
  {
    /// <inheritdoc/>
    protected ControllerWithResultBase(IScreenFactory screenFactory,
                                       ICollection<IControllerRoutine> routines)
      : base(screenFactory,
             routines) { }

    /// <exception cref="OperationCanceledException"/>
    /// <exception cref="Exception"/>
    public abstract Task<TResult> GetResultAsync(CancellationToken cancellationToken);
  }
}
