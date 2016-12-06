using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface ILocator<T>
    where T : class
  {
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    T Locate();

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="type"/> is neither of type <typeparamref name="T"/> nor implements it.</exception>
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    object Locate([NotNull] Type type);
  }
}
