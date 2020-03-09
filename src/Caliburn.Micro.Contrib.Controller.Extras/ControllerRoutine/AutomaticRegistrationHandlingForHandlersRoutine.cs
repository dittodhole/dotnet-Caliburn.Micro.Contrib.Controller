using System;

namespace Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine
{
  public sealed class AutomaticRegistrationHandlingForHandlersRoutine : IControllerRoutine
  {
    /// <exception cref="ArgumentNullException"/>
    public AutomaticRegistrationHandlingForHandlersRoutine(IEventAggregator eventAggregator)
    {
      this.EventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
    }

    private IEventAggregator EventAggregator { get; }

    /// <inheritdoc/>
    public void OnInitialize(IScreen screen) { }

    /// <inheritdoc/>
    public void OnActivate(IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      this.EventAggregator.Subscribe(screen);
    }

    /// <inheritdoc/>
    public void OnViewReady(IScreen screen,
                            object view) { }

    /// <inheritdoc/>
    public void OnDeactivate(IScreen screen,
                             bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      this.EventAggregator.Unsubscribe(screen);
    }

    /// <inheritdoc/>
    public void OnClose(IScreen screen,
                        bool? dialogResult = null) { }
  }
}
