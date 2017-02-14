using System;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Caliburn.Micro.Contrib.Controller.DynamicProxy.ExtensionMethods;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Autofac
{
  public class LocatorAdapter<T> : Locator<T>
    where T : class
  {
    /// <exception cref="ArgumentNullException"><paramref name="lifetimeScope" /> is <see langword="null" /></exception>
    public LocatorAdapter([NotNull] ILifetimeScope lifetimeScope)
    {
      if (lifetimeScope == null)
      {
        throw new ArgumentNullException(nameof(lifetimeScope));
      }
      this.LifetimeScope = lifetimeScope;
    }

    [NotNull]
    private ILifetimeScope LifetimeScope { get; }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="type" /> is neither of type <typeparamref name="T" /> nor implements it.</exception>
    /// <exception cref="ComponentNotRegisteredException" />
    /// <exception cref="DependencyResolutionException" />
    public override T Locate(Type type)
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

      var obj = this.LifetimeScope.Resolve(type);
      var instance = (T) obj;

      return instance;
    }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="type" /> is neither of type <typeparamref name="T" /> nor implements it.</exception>
    /// <exception cref="DependencyResolutionException" />
    public override T LocateOptional(Type type)
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

      var obj = this.LifetimeScope.ResolveOptional(type);
      var instance = (T) obj;

      return instance;
    }
  }
}
