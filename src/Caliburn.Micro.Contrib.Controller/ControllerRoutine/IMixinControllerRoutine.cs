using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public interface IMixinControllerRoutine {}

  public interface IMixinControllerRoutine<[UsedImplicitly] TMixin> : IMixinControllerRoutine
    where TMixin : class, new() {}
}
