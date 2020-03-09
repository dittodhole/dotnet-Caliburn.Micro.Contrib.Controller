using System;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;

namespace Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine
{
  public class AutomaticRegistrationHandlingForHandlersRoutine : ControllerRoutineBase
  {
    /// <inheritdoc/>
    public override void OnInitialize(IScreen screen)
    {
      base.OnInitialize(screen);

      var eventAggregator = IoC.Get<IEventAggregator>();

      eventAggregator.Subscribe(screen);
    }

    /// <inheritdoc/>
    public override void OnDeactivate(IScreen screen,
                                      bool close)
    {
      base.OnDeactivate(screen,
                        close);

      var eventAggregator = IoC.Get<IEventAggregator>();

      eventAggregator.Unsubscribe(screen);
    }
  }
}
