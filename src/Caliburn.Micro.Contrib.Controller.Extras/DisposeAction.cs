using System;

namespace Caliburn.Micro.Contrib.Controller.Extras
{
  public class DisposeAction : IDisposable
  {
    /// <exception cref="ArgumentNullException"/>
    public DisposeAction(Action action)
    {
      this.Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    private Action? Action { get; set; }

    /// <inheritdoc/>
    public void Dispose()
    {
      this.Action?.Invoke();
      this.Action = null;
    }
  }
}
