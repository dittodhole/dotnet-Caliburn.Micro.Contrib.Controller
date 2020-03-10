using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public sealed class DisposableAction : IDisposable
  {
    /// <exception cref="ArgumentNullException"/>
    public DisposableAction(System.Action action)
    {
      this.Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    private System.Action Action { get; }

    /// <inheritdoc/>
    public void Dispose()
    {
      this.Action.Invoke();
    }
  }
}
