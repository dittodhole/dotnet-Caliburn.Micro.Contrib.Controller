using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ExtensionMethods
{
  public static class MethodInfoExtensions
  {
    /// <exception cref="ArgumentNullException"><paramref name="methodInfo" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="returnType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="parameterTypes" /> is <see langword="null" /></exception>
    public static bool DoesMatch([NotNull] this MethodInfo methodInfo,
                                 [NotNull] string name,
                                 [NotNull] Type returnType,
                                 [NotNull] [ItemNotNull] Type[] parameterTypes)
    [Pure]
    {
      if (methodInfo == null)
      {
        throw new ArgumentNullException(nameof(methodInfo));
      }
      if (name == null)
      {
        throw new ArgumentNullException(nameof(name));
      }
      if (returnType == null)
      {
        throw new ArgumentNullException(nameof(returnType));
      }
      if (parameterTypes == null)
      {
        throw new ArgumentNullException(nameof(parameterTypes));
      }

      if (methodInfo.Name != name)
      {
        return false;
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
