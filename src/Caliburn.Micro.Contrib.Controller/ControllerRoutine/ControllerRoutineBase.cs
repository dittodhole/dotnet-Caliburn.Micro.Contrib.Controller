using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  [PublicAPI]
  public abstract class ControllerRoutineBase : IInterceptScreenEvents,
                                                IDisposable
  {
    public virtual void Dispose() {}

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnInitialize(IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnActivate(IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public virtual void OnDeactivate(IScreen screen,
                                     bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="view" /> is <see langword="null" /></exception>
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

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
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
