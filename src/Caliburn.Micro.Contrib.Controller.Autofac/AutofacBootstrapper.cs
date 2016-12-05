using System;
using System.Collections.Generic;
using Autofac;
using Caliburn.Micro.Contrib.Controller.Autofac.ViewModel;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.Extras;
using Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Autofac
{
  [PublicAPI]
  public abstract class AutofacBootstrapper<TRootController> : Micro.Autofac.AutofacBootstrapper<TRootController>
    where TRootController : IController
  {
    public new bool AutoSubscribeEventAggegatorHandlers { get; set; }
    public bool EnableLifetimeScopesForViewModels { get; set; }

    protected override void Configure()
    {
      base.Configure();

      // TODO implement cache

      Controller.CreateScreenInterceptorFn = (controller,
                                              type) =>
                                             {
                                               var autofacScreenInterceptor = new AutofacScreenInterceptor(this.Container,
                                                                                                           this.EnableLifetimeScopesForViewModels,
                                                                                                           controller,
                                                                                                           type);

                                               return autofacScreenInterceptor;
                                             };
    }

    protected override void ConfigureBootstrapper()
    {
      base.ConfigureBootstrapper();

      this.AutoSubscribeEventAggegatorHandlers = false;
      this.EnableLifetimeScopesForViewModels = true;
    }

    protected override void ConfigureContainer(ContainerBuilder builder)
    {
      base.ConfigureContainer(builder);

      builder.RegisterGeneric(typeof(LocatorAdapter<>))
             .As(typeof(ILocator<>));
      builder.RegisterType<BlockingRoutine>()
             .InstancePerDependency();

      if (this.AutoSubscribeEventAggegatorHandlers)
      {
        builder.RegisterType<AutomaticRegistrationHandlingForHandlersRoutine>()
               .As<IControllerRoutine>();
      }
    }

    /// <summary>
    ///   Locates the controller for <typeparamref name="TRootController"/>, locates view model, locates the associate view, binds them and shows it as the root view.
    /// </summary>
    /// <param name="options">The optional view model settings.</param>
    /// <param name="settings">The optional window settings.</param>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TRootController" /> does not implement <see cref="IController" />.</exception>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TRootController" /> could not create a <see cref="IScreen" /> for <paramref name="options" />.</exception>
    protected void DisplayRootViewFor([CanBeNull] object options = null,
                                      [CanBeNull] IDictionary<string, object> settings = null)
    {
      this.DisplayRootViewFor<TRootController>();
    }

    /// <summary>
    ///   Locates the controller, locates view model, locates the associate view, binds them and shows it as the root view.
    /// </summary>
    /// <typeparam name="TController">The controller model type.</typeparam>
    /// <param name="options">The optional view model settings.</param>
    /// <param name="settings">The optional window settings.</param>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TRootController" /> does not implement <see cref="IController" />.</exception>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TRootController" /> could not create a <see cref="IScreen" /> for <paramref name="options" />.</exception>
    protected void DisplayRootViewFor<TController>([CanBeNull] object options = null,
                                                   [CanBeNull] IDictionary<string, object> settings = null) where TController : IController
    {
      Controller.ShowWindowAsync<TController>(options,
                                              settings);
    }

    protected override void OnExit(object sender,
                                   EventArgs e)
    {
      base.OnExit(sender,
                  e);

      this.Container.Dispose();
    }
  }
}
