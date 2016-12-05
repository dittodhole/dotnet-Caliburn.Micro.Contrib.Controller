using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Extras
{
  public class DisposeAction : IDisposable
  {
    /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" /></exception>
    public DisposeAction([NotNull] Action action)
    {
      if (action == null)
      {
        throw new ArgumentNullException(nameof(action));
      }
      this.Action = action;
    }

    [CanBeNull]
    private Action Action { get; set; }

    public void Dispose()
    {
      this.Action?.Invoke();
      this.Action = null;
    }
  }
}