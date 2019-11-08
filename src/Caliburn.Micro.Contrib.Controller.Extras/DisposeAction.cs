using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Extras
{
  public class DisposeAction : IDisposable
  {
    [NotNull]
    private static ILog Logger { get; } = LogManager.GetLog.Invoke(typeof(DisposeAction));

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
        DisposeAction.Logger.Error(exception);
      }
    }
  }
}
