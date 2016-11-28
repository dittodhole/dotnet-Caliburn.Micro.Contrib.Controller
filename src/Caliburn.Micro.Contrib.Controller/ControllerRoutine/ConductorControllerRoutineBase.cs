using System;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public abstract class ConductorControllerRoutineBase : ControllerRoutineBase,
                                                         IConductorControllerRoutine
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    public virtual void OnActivateItem(object screen,
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

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    public virtual void OnDeactivateItem(object screen,
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
