using System;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;

namespace Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine
{
  public class AutomaticRegistrationHandlingForHandlersRoutine : ControllerRoutineBase
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public override void OnInitialize(IScreen screen)
    {
      base.OnInitialize(screen);

      var eventAggregator = IoC.Get<IEventAggregator>();

      eventAggregator.Subscribe(screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
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
