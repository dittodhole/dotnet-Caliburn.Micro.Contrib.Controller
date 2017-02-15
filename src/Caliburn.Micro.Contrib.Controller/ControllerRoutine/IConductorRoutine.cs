namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public interface IConductorRoutine : IRoutine,
                                       IProvideConductorEventHandlers<IScreen, IScreen> {}
}
