using System;
using System.Collections.Concurrent;
using System.Linq;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Proxy
{
  public sealed class InterceptProxyMethodAttributeBasedInterceptor : IInterceptor
  {
    /// <exception cref="ArgumentNullException"><paramref name="interceptionTarget" /> is <see langword="null" /></exception>
    public InterceptProxyMethodAttributeBasedInterceptor([NotNull] object interceptionTarget)
    {
      if (interceptionTarget == null)
      {
        throw new ArgumentNullException(nameof(interceptionTarget));
      }
      this.InterceptionTarget = interceptionTarget;
      this.InterceptionTargetTypeMethodMapping = InterceptProxyMethodAttributeBasedInterceptor.InterceptionTargetTypeMethodMappings.GetOrAdd(interceptionTarget.GetType(),
                                                                                                                                             InterceptionTargetTypeMethodMapping.Create);
    }

    [NotNull]
    private InterceptionTargetTypeMethodMapping InterceptionTargetTypeMethodMapping { get; }

    [NotNull]
    private static ConcurrentDictionary<Type, InterceptionTargetTypeMethodMapping> InterceptionTargetTypeMethodMappings { get; } = new ConcurrentDictionary<Type, InterceptionTargetTypeMethodMapping>();

    [NotNull]
    private object InterceptionTarget { get; }

    /// <exception cref="ArgumentNullException"><paramref name="invocation" /> is <see langword="null" /></exception>
    public void Intercept(IInvocation invocation)
    {
      if (invocation == null)
      {
        throw new ArgumentNullException(nameof(invocation));
      }

      var proxyMethodInfo = invocation.Method;
      var targetMethods = this.InterceptionTargetTypeMethodMapping.GetTargetMethods(proxyMethodInfo);
      if (!targetMethods.Any())
      {
        invocation.Proceed();
      }
      else
      {
        var callBase = targetMethods.Any(targetMethod => targetMethod.InterceptProxyMethodAttribute.CallBase);
        if (callBase)
        {
          invocation.Proceed();
        }

        var proxyMethodParameters = invocation.Arguments;
        var targetMethodParameters = new object[proxyMethodParameters.Length + 1];
        targetMethodParameters[0] = invocation.Proxy;
        Array.Copy(proxyMethodParameters,
                   0,
                   targetMethodParameters,
                   1,
                   proxyMethodParameters.Length);

        foreach (var targetMethod in targetMethods)
        {
          var returnValue = targetMethod.MethodInfo.Invoke(this.InterceptionTarget,
                                                           targetMethodParameters);

          if (!callBase)
          {
            invocation.ReturnValue = returnValue;
          }
        }
      }
    }
  }
}
