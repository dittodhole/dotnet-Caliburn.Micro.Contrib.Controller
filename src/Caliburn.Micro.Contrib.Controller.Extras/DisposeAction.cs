using System;

namespace Caliburn.Micro.Contrib.Controller.Extras
{
  public class DisposeAction : IDisposable
  {
    private static ILog Logger { get; } = LogManager.GetLog.Invoke(typeof(DisposeAction));

    /// <exception cref="ArgumentNullException"/>
    public DisposeAction(Action<DisposeAction> action)
    {
      this.Action = action ?? throw new ArgumentNullException(nameof(action));
    }

    private Action<DisposeAction>? Action { get; set; }

    /// <inheritdoc/>
    public void Dispose()
    {
      try
      {
        this.Action?.Invoke(this);
        this.Action = null;
      }
      catch (Exception exception)
      {
        DisposeAction.Logger.Error(exception);
      }
    }
  }
}
