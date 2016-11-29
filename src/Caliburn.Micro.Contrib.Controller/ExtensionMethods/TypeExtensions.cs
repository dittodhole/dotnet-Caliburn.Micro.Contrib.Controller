using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ExtensionMethods
{
  [PublicAPI]
  public static class TypeExtensions
  {
    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException">If <paramref name="type" /> is an interface.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="type" /> is <see langword="sealed" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="type" /> does not implement <see cref="IScreen" />.</exception>
    public static void CheckTypeForRealScreenType([NotNull] this Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }
      if (type.IsInterface)
      {
        throw new InvalidOperationException($"Cannot create proxy for interface {type}.");
      }
      if (type.IsSealed)
      {
        throw new InvalidOperationException($"Cannot create proxy for {type}, as this type is not defined as abstract.");
      }
      if (!typeof(IScreen).IsAssignableFrom(type))
      {
        throw new InvalidOperationException($"Cannot create proxy for {type}, as this type does implement {nameof(IScreen)}.");
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="name" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="returnType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="parameterTypes" /> is <see langword="null" /></exception>
    [CanBeNull]
    public static MethodInfo GetMethod([NotNull] this Type type,
                                       [NotNull] string name,
                                       BindingFlags bindingFlags,
                                       [NotNull] Type returnType,
                                       [NotNull] [ItemNotNull] Type[] parameterTypes)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
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

      var methodInfo = type.GetMethods(bindingFlags)
                           .Where(arg => arg.DoesMatch(name,
                                                       returnType,
                                                       parameterTypes))
                           .FirstOrDefault();

      return methodInfo;
    }
  }
}
