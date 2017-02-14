using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IProvideConductorEventHandlers
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    void OnActivateItem([NotNull] IScreen screen,
                        [NotNull] IScreen item);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    void OnDeactivateItem([NotNull] IScreen screen,
                          [NotNull] IScreen item,
                          bool close);
  }

  public interface IProvideConductorEventHandlers<TScreen, TItem> : IProvideConductorEventHandlers
    where TScreen : IScreen
    where TItem : IScreen
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    void OnActivateItem([NotNull] TScreen screen,
                        [NotNull] TItem item);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    void OnDeactivateItem([NotNull] TScreen screen,
                          [NotNull] TItem item,
                          bool close);
  }
}
