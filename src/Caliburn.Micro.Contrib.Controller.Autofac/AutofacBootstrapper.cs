using Autofac;
using Caliburn.Micro.Contrib.Controller.DynamicProxy;
using Caliburn.Micro.Contrib.Controller.ControllerRoutines;

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

      builder.RegisterType<DynamicProxyScreenFactory>()
             .As<IScreenFactory>()
             .SingleInstance();

      builder.RegisterType<BlockingRoutine>()
             .As<BlockingRoutine>()
             .InstancePerDependency();

      if (this.AutoSubscribeEventAggegatorHandlers)
      {
        builder.RegisterType<AutomaticRegistrationHandlingForHandlersRoutine>()
               .As<IControllerRoutine>()
               .InstancePerDependency();
      }
    }
  }
}
