using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface IScreenFactory
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="constructorArguments" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    IScreen Create([NotNull] Type screenType,
                   [NotNull] [ItemCanBeNull] object[] constructorArguments,
                   [NotNull] IController controller);
  }
}
