using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IHandleConductorEvents<TScreen, TItem>
    where TScreen : IScreen
    where TItem : IScreen
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    void OnActivateItem(TScreen screen,
                        TItem item);

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    void OnDeactivateItem(TScreen screen,
                          TItem item,
                          bool close);
  }
}
