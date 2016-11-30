using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IController : IInterceptScreenEvents
  {
    /// <exception cref="InvalidOperationException" />
    [CanBeNull]
    IScreen CreateScreen([CanBeNull] object options = null);

    /// <exception cref="InvalidOperationException" />
    [NotNull]
    Type GetScreenType([CanBeNull] object options = null);
  }
}
