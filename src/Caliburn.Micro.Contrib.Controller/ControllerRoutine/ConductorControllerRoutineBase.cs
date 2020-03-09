using System;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public interface IConductorRoutine : IRoutine,
                                       IProvideConductorEventHandlers<IScreen, IScreen> { }

  public abstract class ConductorControllerRoutineBase : ControllerRoutineBase,
                                                         IConductorRoutine
  {
    /// <inheritdoc/>
    public virtual void OnActivateItem(IScreen screen,
                                       IScreen item)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }
    }

    /// <inheritdoc/>
    public virtual void OnDeactivateItem(IScreen screen,
                                         IScreen item,
                                         bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (item == null)
      {
        throw new ArgumentNullException(nameof(item));
      }
    }
  }
}
