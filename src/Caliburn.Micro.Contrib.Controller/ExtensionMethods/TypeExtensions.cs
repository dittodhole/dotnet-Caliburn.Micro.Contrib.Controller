using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ExtensionMethods
{
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
      if (!type.IsDescendant<IScreen>())
      {
        throw new InvalidOperationException($"Cannot create proxy for {type}, as this type does not implement {nameof(IScreen)}.");
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

    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/></exception>
    public static bool IsDescendantOrMatches<T>([NotNull] this Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      return type.IsDescendantOrMatches(typeof(T));
    }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    public static bool IsDescendant<T>([NotNull] this Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      return type.IsDescendant(typeof(T));
    }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="parentType" /> is <see langword="null" /></exception>
    public static bool IsDescendant([NotNull] this Type type,
                                    [NotNull] Type parentType)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }
      if (parentType == null)
      {
        throw new ArgumentNullException(nameof(parentType));
      }

      if (type == parentType)
      {
        return false;
      }

      return type.IsDescendantOrMatches(parentType);
    }

    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/></exception>
    /// <exception cref="ArgumentNullException"><paramref name="parentType"/> is <see langword="null"/></exception>
    public static bool IsDescendantOrMatches([NotNull] this Type type,
                                             [NotNull] Type parentType)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }
      if (parentType == null)
      {
        throw new ArgumentNullException(nameof(parentType));
      }

      if (parentType.IsAssignableFrom(type))
      {
        return true;
      }

      return false;
    }

    /// <exception cref="ArgumentNullException"><paramref name="types"/> is <see langword="null"/></exception>
    [NotNull]
    [ItemNotNull]
    public static IEnumerable<Type> FilterNotifyInterfaces([NotNull] [ItemNotNull] this IEnumerable<Type> types)
    {
      if (types == null)
      {
        throw new ArgumentNullException(nameof(types));
      }

      foreach (var type in types)
      {
        if (type == typeof(INotifyPropertyChanged))
        {
          continue;
        }
        if (type.IsDescendant<INotifyPropertyChanged>())
        {
          continue;
        }
        if (type == typeof(INotifyPropertyChangedEx))
        {
          continue;
        }
        if (type.IsDescendant<INotifyPropertyChangedEx>())
        {
          continue;
        }
        if (type == typeof(INotifyPropertyChanging))
        {
          continue;
        }
        if (type.IsDescendant<INotifyPropertyChanging>())
        {
          continue;
        }
        yield return type;
      }
    }
  }
}
