using System;
using Anotar.LibLog;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Extras
{
  public class DisposeAction : IDisposable
  {
    /// <exception cref="ArgumentNullException"><paramref name="action" /> is <see langword="null" /></exception>
    public DisposeAction([NotNull] Action<DisposeAction> action)
    {
      this.Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    [CanBeNull]
    private Action<DisposeAction> Action { get; set; }

    public void Dispose()
    {
      try
      {
        this.Action?.Invoke(this);
        this.Action = null;
      }
      catch (Exception exception)
      {
        LogTo.ErrorException($"Calling {nameof(this.Action)} raised an exception.",
                             exception);
      }
    }
  }
}