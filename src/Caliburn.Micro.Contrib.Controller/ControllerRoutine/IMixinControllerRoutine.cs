namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public interface IMixinControllerRoutine {}

  public interface IMixinControllerRoutine<TMixin> : IMixinControllerRoutine
    where TMixin : class, new() {}
}
