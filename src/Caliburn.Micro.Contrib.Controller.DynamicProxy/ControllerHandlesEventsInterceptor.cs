using System;
using System.Linq;
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
      this.ControllerTypeMethodsMap = ControllerTypeMethodsMap.Create(controller.GetType());
    }

    private ControllerTypeMethodsMap ControllerTypeMethodsMap { get; }
    private IController Controller { get; }

    /// <inheritdoc/>
    public void Intercept(IInvocation invocation)
    { // TODO work with tasks!
      if (invocation == null)
      {
        return;
      }

      invocation.Proceed();

      var proxyMethodInfo = invocation.Method;
      if (proxyMethodInfo != null)
      {
        var proxy = invocation.Proxy;
        var proxyType = proxy.GetType();
        var targetMethods = this.ControllerTypeMethodsMap.GetTargetMethods(proxyType,
                                                                           proxyMethodInfo);
        if (targetMethods.Any())
        {
          var proxyMethodParameters = invocation.Arguments;
          var targetMethodParameters = new object[proxyMethodParameters.Length + 1];
          targetMethodParameters[0] = proxy;
          Array.Copy(proxyMethodParameters,
                     0,
                     targetMethodParameters,
                     1,
                     proxyMethodParameters.Length);

          foreach (var targetMethod in targetMethods)
          {
            var returnValue = targetMethod.MethodInfo.Invoke(this.Controller,
                                                             targetMethodParameters);

            invocation.ReturnValue = returnValue;
          }
        }
      }
    }
  }
}
