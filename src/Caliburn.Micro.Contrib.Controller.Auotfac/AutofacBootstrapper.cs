using System;
using System.Collections.Generic;
using Autofac;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class AutofacBootstrapper<TRootController> : Autofac.AutofacBootstrapper<TRootController>
  {
    public new bool AutoSubscribeEventAggegatorHandlers { get; set; }

    protected override void Configure()
    {
      base.Configure();

      Controller.CreateScreenInterceptorFn = (controller,
                                              type) =>
                                             {
                                               var autofacScreenInterceptor = new AutofacScreenInterceptor(this.Container,
                                                                                                           controller,
                                                                                                           type);

                                               return autofacScreenInterceptor;
                                             };
    }

    protected override void ConfigureBootstrapper()
    {
      base.ConfigureBootstrapper();

      this.AutoSubscribeEventAggegatorHandlers = false;
    }

    protected override void ConfigureContainer(ContainerBuilder builder)
    {
      base.ConfigureContainer(builder);

      builder.RegisterGeneric(typeof(LocatorAdapter<>))
             .As(typeof(ILocator<>));

      if (this.AutoSubscribeEventAggegatorHandlers)
      {
        builder.RegisterType<AutomaticRegistrationHandlingForHandlersRoutine>()
               .As<IControllerRoutine>();
      }
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
  }
}
