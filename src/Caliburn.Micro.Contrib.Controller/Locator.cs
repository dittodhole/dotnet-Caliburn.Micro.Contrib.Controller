using System;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public interface ILocator<T>
    where T : class
  {
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    T Locate();

    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    TInstance Locate<TInstance>() where TInstance : T;

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

    public virtual TInstance Locate<TInstance>() where TInstance : T
    {
      var type = typeof(TInstance);
      var obj = this.Locate(type);
      var instance = (TInstance) obj;

      return instance;
    }

    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="Exception" />
    public virtual T Locate(Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }
      if (!type.IsDescendantOrMatches<T>())
      {
        throw new ArgumentOutOfRangeException(nameof(type),
                                              $"{nameof(type)} is neither of type {typeof(T)} nor implements it.");
      }

      var obj = IoC.GetInstance(type,
                                null);
      var instance = (T) obj;

      return instance;
    }

    public virtual T LocateOptional()
    {
      return this.Locate();
    }

    /// <exception cref="ArgumentNullException">type is <see langword="null"/></exception>
    /// <exception cref="ArgumentOutOfRangeException" />
    /// <exception cref="Exception" />
    public virtual T LocateOptional(Type type)
    {
      return this.Locate(type);
    }
  }
}
