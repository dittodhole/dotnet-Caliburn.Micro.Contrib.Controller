using System;
using Anotar.LibLog;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public class AutomaticRegistrationHandlingForHandlersRoutine : ControllerRoutineBase
  {
    /// <exception cref="ArgumentNullException"><paramref name="eventAggregatorLocator" /> is <see langword="null" /></exception>
    public AutomaticRegistrationHandlingForHandlersRoutine([NotNull] ILocator<IEventAggregator> eventAggregatorLocator)
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

      IEventAggregator eventAggregator;
      try
      {
        eventAggregator = this.EventAggregatorLocator.Locate();
      }
      catch (Exception exception)
      {
        LogTo.WarnException($"Could not locate {nameof(IEventAggregator)}, skipping subscribing {screen.GetType()}.",
                            exception);
        return;
      }

      eventAggregator.Subscribe(screen);
    }

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    public override void OnDeactivate(IScreen screen,
                                      bool close)
    {
      base.OnDeactivate(screen,
                        close);

      IEventAggregator eventAggregator;
      try
      {
        eventAggregator = this.EventAggregatorLocator.Locate();
      }
      catch (Exception exception)
      {
        LogTo.WarnException($"Could not locate {nameof(IEventAggregator)}, skipping unsubscribing {screen.GetType()}.",
                            exception);
        return;
      }

      eventAggregator.Unsubscribe(screen);
    }
  }
}
