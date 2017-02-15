using System;
using System.Linq;
using Anotar.LibLog;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  public sealed class ControllerHandlesEventsInterceptor : IInterceptor
  {
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    public ControllerHandlesEventsInterceptor([NotNull] IController controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }
      this.Controller = controller;
      this.ControllerTypeMethodsMap = ControllerTypeMethodsMap.Create(controller.GetType());
    }

    [NotNull]
    private ControllerTypeMethodsMap ControllerTypeMethodsMap { get; }

    [NotNull]
    private IController Controller { get; }

    public void Intercept(IInvocation invocation)
    {
      if (invocation == null)
      {
        return;
      }

      var proxyMethodInfo = invocation.Method;
      if (proxyMethodInfo == null)
      {
        invocation.Proceed();
      }
      else
      {
        var proxy = invocation.Proxy;
        var proxyType = proxy.GetType();
        var targetMethods = this.ControllerTypeMethodsMap.GetTargetMethods(proxyType,
                                                                           proxyMethodInfo);
        if (targetMethods.Any())
        {
          var callBase = targetMethods.Any(targetMethod => targetMethod.HandlesViewModelMethodAttribute.CallBase);
          if (callBase)
          {
            invocation.Proceed();
          }

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
            object returnValue;
            try
            {
              returnValue = targetMethod.MethodInfo.Invoke(this.Controller,
                                                           targetMethodParameters);
            }
            catch (Exception exception)
            {
              LogTo.FatalException($"Could not invoke {targetMethod.MethodInfo.Name}.",
                                   exception);
              continue;
            }

            if (!callBase)
            {
              invocation.ReturnValue = returnValue;
            }
          }
        }
        else
        {
          invocation.Proceed();
        }
      }
    }
  }
}
