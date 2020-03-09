using System;
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

    private MethodInfo[] GetControllerMethodInfos(MethodInfo screenMethodInfo)
    {
      var result = this.Controller.GetType()
                                  .FindMembers(MemberTypes.Method,
                                               BindingFlags.Default,
                                               (memberInfo,
                                                _) =>
                                               {
                                                 if (memberInfo is MethodInfo methodInfo)
                                                 {
                                                   var attributes = methodInfo.GetAttributes<HandlesViewModelMethodAttribute>(true);
                                                   // TODO
                                                 }
                                                 return false;
                                               },
                                               null);

      //.Where(arg =>
      //{
      //  var attributes = arg.GetAttributes<HandlesViewModelMethodAttribute>(true);
      //})

      return result;
    }
  }
}
