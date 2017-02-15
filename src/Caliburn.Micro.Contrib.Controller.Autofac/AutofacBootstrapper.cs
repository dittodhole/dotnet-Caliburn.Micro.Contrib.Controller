using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.DynamicProxy;
using Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Autofac
{
  [PublicAPI]
  public abstract class AutofacBootstrapper<TRootController> : Micro.Autofac.AutofacBootstrapper<TRootController>
    where TRootController : IController<IScreen>
  {
    public new bool AutoSubscribeEventAggegatorHandlers { get; set; }

    protected override void ConfigureBootstrapper()
    {
      base.ConfigureBootstrapper();

      this.AutoSubscribeEventAggegatorHandlers = false;
    }

    protected override void ConfigureContainer(ContainerBuilder builder)
    {
      base.ConfigureContainer(builder);

      builder.RegisterType<ControllerManager>()
             .As<IControllerManager>()
             .SingleInstance();

      builder.RegisterType<ProxyScreenFactory>()
             .As<IScreenFactory>()
             .SingleInstance();

      builder.RegisterGeneric(typeof(LocatorAdapter<>))
             .As(typeof(ILocator<>))
             .InstancePerDependency();

      builder.RegisterType<BlockingRoutine>()
             .InstancePerDependency();
      builder.RegisterType<BlockingRoutine.CanBeBlocked>()
             .As<BlockingRoutine.ICanBeBlocked>()
             .InstancePerDependency();

      if (this.AutoSubscribeEventAggegatorHandlers)
      {
        builder.RegisterType<AutomaticRegistrationHandlingForHandlersRoutine>()
               .As<IRoutine>()
               .InstancePerDependency();
      }
    }

    /// <summary>
    ///   Locates the controller for <typeparamref name="TRootController" />, locates view model, locates the associate view, binds them and shows it as the root view.
    /// </summary>
    /// <param name="options">The optional view model options.</param>
    /// <param name="context">The optional view model context.</param>
    /// <param name="settings">The optional window settings.</param>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TRootController" /> does not implement <see cref="ControllerBase" />.</exception>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TRootController" /> could not create a <see cref="IScreen" /> for <paramref name="options" />.</exception>
    protected async Task<TRootController> DisplayRootViewAsync([CanBeNull] object options = null,
                                                               [CanBeNull] object context = null,
                                                               [CanBeNull] IDictionary<string, object> settings = null)
    {
      var rootController = await this.DisplayViewFor<TRootController>(options,
                                                                      context,
                                                                      settings)
                                     .ConfigureAwait(false);
      return rootController;
    }

    /// <summary>
    ///   Locates the controller, locates view model, locates the associate view, binds them and shows it as the root view.
    /// </summary>
    /// <typeparam name="TController">The controller model type.</typeparam>
    /// <param name="options">The optional view model options.</param>
    /// <param name="context">The optional view model context.</param>
    /// <param name="settings">The optional window settings.</param>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TRootController" /> does not implement <see cref="ControllerBase" />.</exception>
    /// <exception cref="InvalidOperationException">If <typeparamref name="TRootController" /> could not create a <see cref="IScreen" /> for <paramref name="options" />.</exception>
    /// <exception cref="Exception" />
    protected async Task<TController> DisplayViewFor<TController>([CanBeNull] object options = null,
                                                                  [CanBeNull] object context = null,
                                                                  [CanBeNull] IDictionary<string, object> settings = null) where TController : IController<IScreen>
    {
      var controllerManager = IoC.Get<IControllerManager>();
      var controller = await controllerManager.ShowWindowAsync<TController>(options,
                                                                            context,
                                                                            settings)
                                              .ConfigureAwait(false);

      return controller;
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
