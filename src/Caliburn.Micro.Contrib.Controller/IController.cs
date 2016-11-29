using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IController : IInterceptScreenEvents
  {
    [CanBeNull]
    IScreen CreateScreen([CanBeNull] object options = null);

    [NotNull]
    Type GetScreenType([CanBeNull] object options = null);
  }
}
