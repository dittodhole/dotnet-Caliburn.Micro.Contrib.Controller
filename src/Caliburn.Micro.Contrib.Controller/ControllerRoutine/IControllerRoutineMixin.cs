using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public interface IControllerRoutineMixin {}

  public interface IControllerRoutineMixin<[UsedImplicitly] TMixin> : IControllerRoutineMixin
    where TMixin : class, new() {}
}
