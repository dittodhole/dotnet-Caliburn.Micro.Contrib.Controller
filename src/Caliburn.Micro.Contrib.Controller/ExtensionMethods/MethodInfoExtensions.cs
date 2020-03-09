using System;
using System.Linq;
using System.Reflection;

namespace Caliburn.Micro.Contrib.Controller.ExtensionMethods
{
  public static class MethodInfoExtensions
  {
    /// <exception cref="ArgumentNullException"/>
    public static bool DoesSignatureMatch(this MethodInfo methodInfo,
                                          Type returnType,
                                          Type[] parameterTypes,
                                          string? name = null)
    {
      if (methodInfo == null)
      {
        throw new ArgumentNullException(nameof(methodInfo));
      }
      if (returnType == null)
      {
        throw new ArgumentNullException(nameof(returnType));
      }
      if (parameterTypes == null)
      {
        throw new ArgumentNullException(nameof(parameterTypes));
      }

      if (name != null)
      {
        if (methodInfo.Name != name)
        {
          return false;
        }
      }

      if (methodInfo.ReturnType != returnType)
      {
        return false;
      }
      if (!methodInfo.GetParameters()
                     .Select(arg => arg.ParameterType)
                     .SequenceEqual(parameterTypes))
      {
        return false;
      }

      return true;
    }
  }
}
