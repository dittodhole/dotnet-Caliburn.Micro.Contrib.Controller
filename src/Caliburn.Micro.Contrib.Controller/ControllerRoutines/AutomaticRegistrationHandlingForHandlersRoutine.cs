using System;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutines
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
    void IHandleScreenEvents<IScreen>.OnInitialize(IScreen screen) { }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnActivate(IScreen screen)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      this.EventAggregator.Subscribe(screen);
    }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnViewReady(IScreen screen,
                                                  object view) { }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnClose(IScreen screen,
                                              bool? dialogResult) { }

    /// <inheritdoc/>
    void IHandleScreenEvents<IScreen>.OnDeactivate(IScreen screen,
                                                   bool close)
    {
      if (screen == null)
      {
        throw new ArgumentNullException(nameof(screen));
      }

      this.EventAggregator.Unsubscribe(screen);
    }
  }
}
