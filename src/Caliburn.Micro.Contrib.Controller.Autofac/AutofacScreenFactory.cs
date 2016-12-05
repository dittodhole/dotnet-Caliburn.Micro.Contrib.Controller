using System;
using Autofac;
using Caliburn.Micro.Contrib.Controller.Autofac.ViewModel;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Autofac
{
  public class AutofacScreenFactory : IScreenFactory
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
    public IScreen Create(ControllerBase controller,
                          object options = null)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var screenType = controller.GetScreenType(options);
      var autofacScreenInterceptor = new AutofacScreenInterceptor(this.LifetimeScope,
                                                                  controller,
                                                                  screenType);

      var screen = autofacScreenInterceptor.CreateProxiedScreen();

      return screen;
    }
  }
}
