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
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="type" /> is neither of type <typeparamref name="T" /> nor implements it.</exception>
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    T Locate([NotNull] Type type);

    /// <exception cref="Exception" />
    [Pure]
    [CanBeNull]
    T LocateOptional();

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="type" /> is neither of type <typeparamref name="T" /> nor implements it.</exception>
    /// <exception cref="Exception" />
    [Pure]
    [CanBeNull]
    T LocateOptional([NotNull] Type type);
  }

  public class Locator<T> : ILocator<T>
    where T : class
  {
    public virtual T Locate()
    {
      var type = typeof(T);
      var obj = this.Locate(type);
      var instance = obj;

      return instance;
    }

    public virtual T Locate(Type type)
    {
      var obj = IoC.GetInstance(type,
                                null);
      var instance = (T) obj;

      return instance;
    }

    public virtual T LocateOptional()
    {
      return this.Locate();
    }

    public virtual T LocateOptional(Type type)
    {
      return this.Locate(type);
    }
  }
}
