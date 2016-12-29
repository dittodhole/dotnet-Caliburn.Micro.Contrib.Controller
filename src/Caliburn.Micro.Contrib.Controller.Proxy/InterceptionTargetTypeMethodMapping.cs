using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro.Contrib.Controller.Proxy.ExtensionMethods;
using Castle.Core.Internal;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Proxy
{
  internal sealed class InterceptionTargetTypeMethodMapping
  {
    /// <exception cref="ArgumentNullException"><paramref name="interceptionTargetType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="targetMethods" /> is <see langword="null" /></exception>
    private InterceptionTargetTypeMethodMapping([NotNull] Type interceptionTargetType,
                                                [NotNull] IDictionary<string, TargetMethod[]> targetMethods)
    {
      if (interceptionTargetType == null)
      {
        throw new ArgumentNullException(nameof(interceptionTargetType));
      }
      if (targetMethods == null)
      {
        throw new ArgumentNullException(nameof(targetMethods));
      }
      this.InterceptionTargetType = interceptionTargetType;
      this.TargetMethods = targetMethods;
    }

    [NotNull]
    private Type InterceptionTargetType { get; }

    [NotNull]
    private IDictionary<string, TargetMethod[]> TargetMethods { get; }

    [Pure]
    [NotNull]
    [ItemNotNull]
    public TargetMethod[] GetTargetMethods([NotNull] Type proxyType,
                                           [NotNull] MethodInfo proxyMethodInfo)
    {
      if (proxyType == null)
      {
        throw new ArgumentNullException(nameof(proxyType));
      }
      if (proxyMethodInfo == null)
      {
        throw new ArgumentNullException(nameof(proxyMethodInfo));
      }

      var proxyMethodName = proxyMethodInfo.Name;

      TargetMethod[] targetMethods;
      if (this.TargetMethods.TryGetValue(proxyMethodName,
                                         out targetMethods))
      {
        targetMethods = targetMethods.Where(targetMethod =>
                                            {
                                              var targetMethodInfo = targetMethod.MethodInfo;
                                              var targetMethodParameterInfos = targetMethodInfo.GetParameters();

                                              var proxiedTypeParameterInfo = targetMethodParameterInfos.FirstOrDefault();
                                              if (proxiedTypeParameterInfo == null)
                                              {
                                                return false;
                                              }

                                              var proxiedType = proxiedTypeParameterInfo.ParameterType;
                                              if (!proxyType.IsDescendantOrMatches(proxiedType))
                                              {
                                                return false;
                                              }

                                              var targetMethodParameterTypes = targetMethod.InterceptProxyMethodAttribute.MethodParamterTypes;
                                              if (targetMethodParameterTypes == null)
                                              {
                                                targetMethodParameterTypes = targetMethodParameterInfos.Skip(1)
                                                                                                       .Select(parameterInfo => parameterInfo.ParameterType)
                                                                                                       .ToArray();
                                              }

                                              var targetMethodReturnType = targetMethodInfo.ReturnType;

                                              var doesSignatureMatch = proxyMethodInfo.DoesSignatureMatch(targetMethodReturnType,
                                                                                                          targetMethodParameterTypes);

                                              return doesSignatureMatch;
                                            })
                                     .ToArray();
      }
      else
      {
        targetMethods = new TargetMethod[0];
      }

      return targetMethods;
    }

    /// <exception cref="ArgumentNullException"><paramref name="interceptionTargetType" /> is <see langword="null" /></exception>
    [NotNull]
    public static InterceptionTargetTypeMethodMapping Create([NotNull] Type interceptionTargetType)
    {
      if (interceptionTargetType == null)
      {
        throw new ArgumentNullException(nameof(interceptionTargetType));
      }

      var targetMethods = interceptionTargetType.GetMethods(TypeExtensions.DefaultBindingFlags)
                                                .Select(methodInfo => new
                                                                      {
                                                                        MethodInfo = methodInfo,
                                                                        Attributes = methodInfo.GetAttributes<InterceptProxyMethodAttribute>()
                                                                      })
                                                .Where(arg => arg.Attributes.Any())
                                                .SelectMany(arg => arg.Attributes.Select(attribute => new
                                                                                                      {
                                                                                                        Attribute = attribute,
                                                                                                        arg.MethodInfo,
                                                                                                        ProxyMethodName = attribute.MethodName ?? arg.MethodInfo.Name
                                                                                                      }))
                                                .GroupBy(arg => arg.ProxyMethodName)
                                                .ToDictionary(group => group.Key,
                                                              group => group.Select(arg => new TargetMethod(arg.MethodInfo,
                                                                                                            arg.Attribute))
                                                                            .ToArray());

      var interceptionTargetTypeMethodMapping = new InterceptionTargetTypeMethodMapping(interceptionTargetType,
                                                                                        targetMethods);

      return interceptionTargetTypeMethodMapping;
    }

    internal sealed class TargetMethod
    {
      /// <exception cref="ArgumentNullException"><paramref name="methodInfo" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="interceptProxyMethodAttribute" /> is <see langword="null" /></exception>
      public TargetMethod([NotNull] MethodInfo methodInfo,
                          [NotNull] InterceptProxyMethodAttribute interceptProxyMethodAttribute)
      {
        if (methodInfo == null)
        {
          throw new ArgumentNullException(nameof(methodInfo));
        }
        if (interceptProxyMethodAttribute == null)
        {
          throw new ArgumentNullException(nameof(interceptProxyMethodAttribute));
        }
        this.MethodInfo = methodInfo;
        this.InterceptProxyMethodAttribute = interceptProxyMethodAttribute;
      }

      [NotNull]
      public MethodInfo MethodInfo { get; }

      [NotNull]
      public InterceptProxyMethodAttribute InterceptProxyMethodAttribute { get; }
    }
  }
}
