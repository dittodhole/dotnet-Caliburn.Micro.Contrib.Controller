using Autofac;
using Caliburn.Micro.Autofac;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  [PublicAPI]
  public abstract class AutofacBootstrapper<TRootController> : Autofac.AutofacBootstrapper<TRootController>
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

      builder.RegisterGeneric(typeof(LocatorAdapter<>))
             .As(typeof(ILocator<>));

      if (this.AutoSubscribeEventAggegatorHandlers)
      {
        builder.RegisterType<AutomaticRegistrationHandlingForHandlersRoutine>()
               .As<IControllerRoutine>();
      }
    }
  }
}
