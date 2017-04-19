using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface IScreenFactoryAdapter
  {
    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    IScreen CreateScreen([CanBeNull] object options = null);
  }

  public interface IScreenFactoryAdapter<TScreen> : IScreenFactoryAdapter
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentException" />
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    new TScreen CreateScreen([CanBeNull] object options = null);
  }
}
