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
  public abstract class AutofacBootstrapper<TRootController> : Micro.Autofac.AutofacBootstrapper<TRootController>
    where TRootController : IController
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

      builder.RegisterType<ScreenManager>()
             .As<IScreenManager>()
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
    ///   Locates the controller, locates view model, locates the associate view, binds them and shows it as the root view.
    /// </summary>
    /// <param name="options">The optional view model options.</param>
    /// <param name="context">The optional view model context.</param>
    /// <param name="settings">The optional window settings.</param>
    /// <exception cref="Exception" />
    [PublicAPI]
    public virtual async Task<TScreenFactoryAdapter> DisplayViewForAsync<TScreenFactoryAdapter>([CanBeNull] object options = null,
                                                                                                [CanBeNull] object context = null,
                                                                                                [CanBeNull] IDictionary<string, object> settings = null)
      where TScreenFactoryAdapter : IScreenFactoryAdapter
    {
      var screenManager = IoC.Get<IScreenManager>();
      var screenFactoryAdapter = await screenManager.ShowWindowAsync<TScreenFactoryAdapter>(options,
                                                                                            context,
                                                                                            settings)
                                                    .ConfigureAwait(false);

      return screenFactoryAdapter;
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
