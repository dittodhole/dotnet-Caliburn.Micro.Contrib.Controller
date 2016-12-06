using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro.Contrib.Controller.Autofac.ViewModel;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Autofac
{
  [PublicAPI]
  public abstract class AutofacBootstrapper<TRootController> : Micro.Autofac.AutofacBootstrapper<TRootController>
    where TRootController : ControllerBase
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
             .SingleInstance();

      builder.RegisterType<AutofacScreenInterceptor>()
             .As<IScreenInterceptor>()
             .InstancePerDependency();

      builder.RegisterType<AutofacScreenFactory>()
             .As<IScreenFactory>()
             .InstancePerDependency();

      builder.RegisterGeneric(typeof(LocatorAdapter<>))
             .As(typeof(ILocator<>))
             .InstancePerDependency();
      builder.RegisterType<BlockingRoutine>()
             .InstancePerDependency();

      if (this.AutoSubscribeEventAggegatorHandlers)
      {
        builder.RegisterType<AutomaticRegistrationHandlingForHandlersRoutine>()
               .As<ControllerRoutineBase>()
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
    protected async Task DisplayRootView([CanBeNull] object options = null,
                                         [CanBeNull] object context = null,
                                         [CanBeNull] IDictionary<string, object> settings = null)
    {
      await this.DisplayViewFor<TRootController>(options,
                                                 context,
                                                 settings)
                .ConfigureAwait(false);
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
    protected async Task DisplayViewFor<TController>([CanBeNull] object options = null,
                                                     [CanBeNull] object context = null,
                                                     [CanBeNull] IDictionary<string, object> settings = null) where TController : ControllerBase
    {
      var controllerManager = IoC.Get<ControllerManager>();
      await controllerManager.ShowWindowAsync<TController>(options,
                                                           context,
                                                           settings)
                             .ConfigureAwait(false);
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
