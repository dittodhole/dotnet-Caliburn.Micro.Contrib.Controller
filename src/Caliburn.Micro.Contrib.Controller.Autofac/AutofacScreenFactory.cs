using System;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Autofac
{
  public class AutofacScreenFactory : ScreenFactory
  {
    /// <exception cref="ArgumentNullException"><paramref name="lifetimeScope" /> is <see langword="null" /></exception>
    public AutofacScreenFactory([NotNull] ILifetimeScope lifetimeScope)
    {
      if (lifetimeScope == null)
      {
        throw new ArgumentNullException(nameof(lifetimeScope));
      }
      this.LifetimeScope = lifetimeScope;
    }

    [NotNull]
    private ILifetimeScope LifetimeScope { get; }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="ComponentNotRegisteredException" />
    /// <exception cref="DependencyResolutionException" />
    /// <exception cref="Exception" />
    public override IScreenInterceptor CreateScreenInterceptor(ControllerBase controller,
                                                               Type screenType)
    {
      IScreenInterceptor screenInterceptor;

      object instance;
      if (this.LifetimeScope.TryResolveService(new TypedService(typeof(IScreenInterceptor)),
                                               new[]
                                               {
                                                 TypedParameter.From(controller),
                                                 TypedParameter.From(screenType)
                                               },
                                               out instance))
      {
        screenInterceptor = (IScreenInterceptor) instance;
      }
      else
      {
        screenInterceptor = base.CreateScreenInterceptor(controller,
                                                         screenType);
      }

      return screenInterceptor;
    }
  }
}
