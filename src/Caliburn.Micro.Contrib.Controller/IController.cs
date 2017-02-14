using System;
using System.Collections.Generic;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IController
  {
    [NotNull]
    [ItemNotNull]
    IEnumerable<IRoutine> Routines { get; }

    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    Type GetScreenType([CanBeNull] object options = null);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [NotNull]
    IScreen BuildUp([NotNull] IScreen screen,
                    [CanBeNull] object options = null);
  }

  public interface IController<TScreen>
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    [NotNull]
    TScreen BuildUp([NotNull] TScreen screen,
                    [CanBeNull] object options = null);
  }
}
