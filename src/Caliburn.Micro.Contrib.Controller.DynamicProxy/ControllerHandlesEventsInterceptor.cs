using System;
using System.Linq;
using System.Reflection;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using Castle.DynamicProxy;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  public sealed class ControllerHandlesEventsInterceptor : IInterceptor
  {
    private static ILog Logger { get; } = LogManager.GetLog.Invoke(typeof(ControllerHandlesEventsInterceptor));

    /// <exception cref="ArgumentNullException"/>
    public ControllerHandlesEventsInterceptor(IController controller)
    {
      this.Controller = controller ?? throw new ArgumentNullException(nameof(controller));
    }

    private IController Controller { get; }

    /// <inheritdoc/>
    public void Intercept(IInvocation invocation)
    { // TODO work with tasks!
      if (invocation == null)
      {
        throw new ArgumentNullException(nameof(invocation));
      }

      // TODO check if task - skip invocation

      invocation.Proceed();

      var screenMethodInfo = invocation.GetConcreteMethodInvocationTarget();
      var interceptinMethodInfo = Caliburn.Micro.Contrib.Controller.Controller.GetInterceptingMethodInfo(this.Controller,
                                                                                                         BindingFlags.Default,
                                                                                                         screenMethodInfo.Name);
      if (interceptinMethodInfo != null)
      {
        var parameters = invocation.Arguments;

        var controllerMethodParameters = new object[parameters.Length + 1];
        controllerMethodParameters[0] = invocation.InvocationTarget;
        Array.Copy(parameters,
                   0,
                   controllerMethodParameters,
                   1,
                   parameters.Length);

        var returnValue = interceptinMethodInfo.Invoke(this.Controller,
                                                      controllerMethodParameters);

        invocation.ReturnValue = returnValue;
      }
    }
  }
}
