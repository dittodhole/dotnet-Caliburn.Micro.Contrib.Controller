using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IProvideConductorEventHandlers<TScreen, TItem>
    where TScreen : IScreen
    where TItem : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    void OnActivateItem(TScreen screen,
                        TItem item);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    void OnDeactivateItem(TScreen screen,
                          TItem item,
                          bool close);
  }
}
