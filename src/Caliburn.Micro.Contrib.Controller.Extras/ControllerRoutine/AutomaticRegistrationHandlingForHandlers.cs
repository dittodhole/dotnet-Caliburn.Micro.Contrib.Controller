using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public class AutomaticRegistrationHandlingForHandlers : ControllerRoutineBase
  {
    /// <exception cref="ArgumentNullException"><paramref name="eventAggregatorLocator" /> is <see langword="null" /></exception>
    public AutomaticRegistrationHandlingForHandlers([NotNull] ILocator<IEventAggregator> eventAggregatorLocator)
    {
      if (eventAggregatorLocator == null)
      {
        throw new ArgumentNullException(nameof(eventAggregatorLocator));
      }
      this.EventAggregatorLocator = eventAggregatorLocator;
    }

    [NotNull]
    private ILocator<IEventAggregator> EventAggregatorLocator { get; }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public override void OnInitialize(IScreen screen)
    {
      base.OnInitialize(screen);

      var eventAggregator = this.EventAggregatorLocator.Locate();

      eventAggregator.Subscribe(screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public override void OnDeactivate(IScreen screen,
                                      bool close)
    {
      base.OnDeactivate(screen,
                        close);

      var eventAggregator = this.EventAggregatorLocator.Locate();

      eventAggregator.Unsubscribe(screen);
    }
  }
}
