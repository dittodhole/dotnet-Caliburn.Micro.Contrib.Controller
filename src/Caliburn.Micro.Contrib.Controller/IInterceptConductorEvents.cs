using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IInterceptConductorEvents : IInterceptScreenEvents
  {
    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    void OnActivateItem([NotNull] object screen,
                        [NotNull] object item);

    /// <exception cref="ArgumentNullException"><paramref name="screen" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="item" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidCastException" />
    void OnDeactivateItem([NotNull] object screen,
                          [NotNull] object item,
                          bool close);
  }
}
