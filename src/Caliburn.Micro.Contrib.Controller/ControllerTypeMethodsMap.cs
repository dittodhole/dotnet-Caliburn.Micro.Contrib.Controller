using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public sealed class ControllerTypeMethodsMap
  {
    /// <exception cref="ArgumentNullException"><paramref name="controllerType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="targetMethods" /> is <see langword="null" /></exception>
    private ControllerTypeMethodsMap([NotNull] Type controllerType,
                                     [NotNull] IDictionary<string, TargetMethod[]> targetMethods)
    {
      if (controllerType == null)
      {
        throw new ArgumentNullException(nameof(controllerType));
      }
      if (targetMethods == null)
      {
        throw new ArgumentNullException(nameof(targetMethods));
      }
      this.ControllerType = controllerType;
      this.TargetMethods = targetMethods;
    }

    [NotNull]
    private Type ControllerType { get; }

    [NotNull]
    private IDictionary<string, TargetMethod[]> TargetMethods { get; }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenMethodInfo" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    public TargetMethod[] GetTargetMethods([NotNull] Type screenType,
                                           [NotNull] MethodInfo screenMethodInfo)
    {
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }
      if (screenMethodInfo == null)
      {
        throw new ArgumentNullException(nameof(screenMethodInfo));
      }

      var screenMethodName = screenMethodInfo.Name;

      TargetMethod[] targetMethods;
      if (this.TargetMethods.TryGetValue(screenMethodName,
                                         out targetMethods))
      {
        targetMethods = targetMethods.Where(targetMethod =>
                                            {
                                              var targetMethodInfo = targetMethod.MethodInfo;
                                              var targetMethodParameterInfos = targetMethodInfo.GetParameters();

                                              var screenTypeParameterInfo = targetMethodParameterInfos.FirstOrDefault();
                                              if (screenTypeParameterInfo == null)
                                              {
                                                return false;
                                              }

                                              if (!screenType.IsDescendantOrMatches(screenTypeParameterInfo.ParameterType))
                                              {
                                                return false;
                                              }

                                              var targetMethodParameterTypes = targetMethod.HandlesEventAttribute.MethodParamterTypes;
                                              if (targetMethodParameterTypes == null)
                                              {
                                                targetMethodParameterTypes = targetMethodParameterInfos.Skip(1)
                                                                                                       .Select(parameterInfo => parameterInfo.ParameterType)
                                                                                                       .ToArray();
                                              }

                                              var targetMethodReturnType = targetMethodInfo.ReturnType;

                                              var doesSignatureMatch = screenMethodInfo.DoesSignatureMatch(targetMethodReturnType,
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

    /// <exception cref="ArgumentNullException"><paramref name="controllerType" /> is <see langword="null" /></exception>
    [NotNull]
    public static ControllerTypeMethodsMap Create([NotNull] Type controllerType)
    {
      if (controllerType == null)
      {
        throw new ArgumentNullException(nameof(controllerType));
      }

      var targetMethods = controllerType.GetMethods(TypeExtensions.DefaultBindingFlags)
                                        .Select(methodInfo => new
                                                              {
                                                                MethodInfo = methodInfo,
                                                                HandlesEventAttributes = methodInfo.GetAttributes<HandlesEvent>(true)
                                                              })
                                        .Where(arg => arg.HandlesEventAttributes.Any())
                                        .SelectMany(arg => arg.HandlesEventAttributes.Select(handlesEventAttribute => new
                                                                                                                      {
                                                                                                                        HandlesEventAttribute = handlesEventAttribute,
                                                                                                                        arg.MethodInfo,
                                                                                                                        ScreenMethodName = handlesEventAttribute.MethodName ?? arg.MethodInfo.Name
                                                                                                                      }))
                                        .GroupBy(arg => arg.ScreenMethodName)
                                        .ToDictionary(group => group.Key,
                                                      group => group.Select(arg => new TargetMethod(arg.MethodInfo,
                                                                                                    arg.HandlesEventAttribute))
                                                                    .ToArray());

      var controllerTypeMethodsMap = new ControllerTypeMethodsMap(controllerType,
                                                                  targetMethods);

      return controllerTypeMethodsMap;
    }

    public sealed class TargetMethod
    {
      /// <exception cref="ArgumentNullException"><paramref name="methodInfo" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="handlesEventAttribute" /> is <see langword="null" /></exception>
      public TargetMethod([NotNull] MethodInfo methodInfo,
                          [NotNull] HandlesEvent handlesEventAttribute)
      {
        if (methodInfo == null)
        {
          throw new ArgumentNullException(nameof(methodInfo));
        }
        if (handlesEventAttribute == null)
        {
          throw new ArgumentNullException(nameof(handlesEventAttribute));
        }
        this.MethodInfo = methodInfo;
        this.HandlesEventAttribute = handlesEventAttribute;
      }

      [NotNull]
      public MethodInfo MethodInfo { get; }

      [NotNull]
      public HandlesEvent HandlesEventAttribute { get; }
    }
  }
}