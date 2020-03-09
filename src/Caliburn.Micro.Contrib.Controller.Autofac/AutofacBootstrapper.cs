using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Caliburn.Micro.Contrib.Controller.DynamicProxy;
using Caliburn.Micro.Contrib.Controller.Extras.ControllerRoutine;

namespace Caliburn.Micro.Contrib.Controller.Autofac
{
  public abstract class AutofacBootstrapper<TRootController> : Micro.Autofac.AutofacBootstrapper<TRootController>
    where TRootController : IController
  {
    public new bool AutoSubscribeEventAggegatorHandlers { get; set; }

    /// <inheritdoc/>
    protected override void ConfigureBootstrapper()
    {
      base.ConfigureBootstrapper();

      this.AutoSubscribeEventAggegatorHandlers = false;
    }

    /// <inheritdoc/>
    protected override void ConfigureContainer(ContainerBuilder builder)
    {
      base.ConfigureContainer(builder);

      builder.RegisterType<ScreenManager>()
             .As<IScreenManager>()
             .SingleInstance();

      builder.RegisterType<ProxyScreenFactory>()
             .As<IScreenFactory>()
             .SingleInstance();

      builder.RegisterType<BlockingRoutine>()
             .InstancePerDependency();
      builder.RegisterType<BlockingRoutine.CanBeBlocked>()
             .As<BlockingRoutine.ICanBeBlocked>()
             .InstancePerDependency();

      if (this.AutoSubscribeEventAggegatorHandlers)
      {
        builder.RegisterType<AutomaticRegistrationHandlingForHandlersRoutine>()
               .As<IControllerRoutine>()
               .InstancePerDependency();
      }
    }

    /// <summary>
    ///   Locates the controller, locates view model, locates the associate view, binds them and shows it as the root view.
    /// </summary>
    /// <param name="options">The optional view model options.</param>
    /// <param name="context">The optional view model context.</param>
    /// <param name="settings">The optional window settings.</param>
    /// <exception cref="Exception" />
    public virtual async Task<TScreenFactoryAdapter> DisplayViewForAsync<TScreenFactoryAdapter>(object? options = null,
                                                                                                object? context = null,
                                                                                                IDictionary<string, object>? settings = null) where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      var screenManager = IoC.Get<IScreenManager>();
      var screenFactoryAdapter = await screenManager.ShowWindowAsync<TScreenFactoryAdapter>(options,
                                                                                            context,
                                                                                            settings)
                                                    .ConfigureAwait(false);

      return screenFactoryAdapter;
    }
  }
}
