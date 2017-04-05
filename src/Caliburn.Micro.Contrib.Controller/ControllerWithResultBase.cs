using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public abstract class ControllerWithResultBase<TScreen, TResult> : ControllerBase<TScreen>,
                                                                     IProvideResultAsync<TResult>
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenFactory" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="routines" /> is <see langword="null" /></exception>
    protected ControllerWithResultBase([NotNull] IScreenFactory screenFactory,
                                       [NotNull] ICollection<IRoutine> routines)
      : base(screenFactory,
             routines) {}

    public abstract Task<TResult> GetResultAsync(CancellationToken cancellationToken);
  }
}
