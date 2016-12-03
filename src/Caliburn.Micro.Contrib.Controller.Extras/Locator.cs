using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Extras
{
  public class Locator<T> : ILocator<T>
  {
    /// <exception cref="ArgumentNullException"><paramref name="locateFn" /> is <see langword="null" /></exception>
    public Locator([NotNull] Func<T> locateFn)
    {
      if (locateFn == null)
      {
        throw new ArgumentNullException(nameof(locateFn));
      }
      this.LocateFn = locateFn;
    }

    [NotNull]
    private Func<T> LocateFn { get; }

    public T Locate()
    {
      var instance = this.LocateFn.Invoke();

      return instance;
    }
  }
}
