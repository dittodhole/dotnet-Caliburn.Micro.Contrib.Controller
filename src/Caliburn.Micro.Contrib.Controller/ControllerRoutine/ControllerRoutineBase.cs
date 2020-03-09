using System;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public interface IRoutine : IProvideScreenEventHandlers<IScreen> { }

  public abstract class ControllerRoutineBase : IRoutine
  {
    /// <inheritdoc/>
    public virtual void OnInitialize(IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <inheritdoc/>
    public virtual void OnActivate(IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <inheritdoc/>
    public virtual void OnDeactivate(IScreen screen,
                                     bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <inheritdoc/>
    public virtual void OnViewReady(IScreen screen,
                                    object view)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
      if (view == null)
      {
        throw new ArgumentNullException(nameof(view));
      }
    }

    /// <inheritdoc/>
    public virtual void OnClose(IScreen screen,
                                bool? dialogResult = null)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }
  }
}
