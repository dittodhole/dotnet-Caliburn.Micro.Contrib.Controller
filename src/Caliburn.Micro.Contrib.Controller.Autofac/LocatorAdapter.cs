using System;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public class LocatorAdapter<T> : ILocator<T>
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

    /// <exception cref="ComponentNotRegisteredException" />
    /// <exception cref="DependencyResolutionException" />
    public T Locate()
    {
      var instance = this.LifetimeScope.Resolve<T>();

      return instance;
    }
  }
}
