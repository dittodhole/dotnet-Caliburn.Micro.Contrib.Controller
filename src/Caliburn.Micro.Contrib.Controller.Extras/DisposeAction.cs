using System;
using Caliburn.Micro.Contrib.Controller.Extras.Logging;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Extras
{
  public class DisposeAction : IDisposable
  {
    [NotNull]
    private static Caliburn.Micro.Contrib.Controller.Extras.Logging.ILog Logger { get; } = LogProvider.GetCurrentClassLogger();

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
        DisposeAction.Logger.FatalException($"Calling {nameof(this.Action)} raised an exception.",
                                            exception);
      }
    }
  }
}
