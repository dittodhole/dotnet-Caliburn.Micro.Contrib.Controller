using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;

namespace Caliburn.Micro.Contrib.Controller
{
  public sealed class ControllerTypeMethodsMap
  {
    /// <exception cref="ArgumentNullException">
    private ControllerTypeMethodsMap(Type controllerType,
                                     IDictionary<string, TargetMethod[]> targetMethods)
    {
      this.ControllerType = controllerType ?? throw new ArgumentNullException(nameof(controllerType));
      this.TargetMethods = targetMethods ?? throw new ArgumentNullException(nameof(targetMethods));
    }

    private Type ControllerType { get; }
    private IDictionary<string, TargetMethod[]> TargetMethods { get; }

    /// <exception cref="ArgumentNullException"/>
    public TargetMethod[] GetTargetMethods(Type screenType,
                                           MethodInfo screenMethodInfo)
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

      if (this.TargetMethods.TryGetValue(screenMethodName,
                                         out var targetMethods))
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

                                              var targetMethodParameterTypes = targetMethodParameterInfos.Skip(1)
                                                                                                         .Select(parameterInfo => parameterInfo.ParameterType)
                                                                                                         .ToArray();

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

    /// <exception cref="ArgumentNullException"/>
    public static ControllerTypeMethodsMap Create(Type controllerType)
    {
      if (controllerType == null)
      {
        throw new ArgumentNullException(nameof(controllerType));
      }

      var targetMethods = controllerType.GetMethods(TypeExtensions.DefaultBindingFlags)
                                        .Select(methodInfo => new
                                                              {
                                                                MethodInfo = methodInfo,
                                                                HandlesViewModelMethodAttributes = methodInfo.GetAttributes<HandlesViewModelMethodAttribute>(true)
                                                              })
                                        .Where(arg => arg.HandlesViewModelMethodAttributes.Any())
                                        .SelectMany(arg => arg.HandlesViewModelMethodAttributes.Select(handlesEventAttribute => new
                                                                                                                                {
                                                                                                                                  HandlesViewModelMethodAttribute = handlesEventAttribute,
                                                                                                                                  arg.MethodInfo,
                                                                                                                                  ScreenMethodName = handlesEventAttribute.MethodName ?? arg.MethodInfo.Name
                                                                                                                                }))
                                        .GroupBy(arg => arg.ScreenMethodName)
                                        .ToDictionary(group => group.Key,
                                                      group => group.Select(arg => new TargetMethod(arg.MethodInfo,
                                                                                                    arg.HandlesViewModelMethodAttribute))
                                                                    .ToArray());

      var controllerTypeMethodsMap = new ControllerTypeMethodsMap(controllerType,
                                                                  targetMethods);

      return controllerTypeMethodsMap;
    }

    public sealed class TargetMethod
    {
      /// <exception cref="ArgumentNullException"><paramref name="methodInfo" /> is <see langword="null" /></exception>
      /// <exception cref="ArgumentNullException"><paramref name="handlesViewModelMethodAttribute" /> is <see langword="null" /></exception>
      public TargetMethod(MethodInfo methodInfo,
                          HandlesViewModelMethodAttribute handlesViewModelMethodAttribute)
      {
        this.MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
        this.HandlesViewModelMethodAttribute = handlesViewModelMethodAttribute ?? throw new ArgumentNullException(nameof(handlesViewModelMethodAttribute));
      }

      public MethodInfo MethodInfo { get; }
      public HandlesViewModelMethodAttribute HandlesViewModelMethodAttribute { get; }
    }
  }
}
