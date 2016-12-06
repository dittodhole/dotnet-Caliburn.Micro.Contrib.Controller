﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ExtensionMethods
{
  [PublicAPI]
  public static class TypeExtensions
  {
    public static BindingFlags TaskResultBindingFlags = BindingFlags.Instance | BindingFlags.Public;

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
    [Pure]
    public static bool IsDescendantOrMatches<T>([NotNull] this Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      return type.IsDescendantOrMatches(typeof(T));
    }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    [Pure]
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
    [Pure]
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
    [Pure]
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
    [Pure]
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
        if (type.IsDescendantOrMatches<INotifyPropertyChanged>())
        {
          continue;
        }
        if (type.IsDescendantOrMatches<INotifyPropertyChangedEx>())
        {
          continue;
        }
        if (type.IsDescendantOrMatches<INotifyPropertyChanging>())
        {
          continue;
        }
        yield return type;
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="types"/> is <see langword="null"/></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    public static IEnumerable<Type> FilterInternalMixinInterface([NotNull] [ItemNotNull] this IEnumerable<Type> types)
    {
      if (types == null)
      {
        throw new ArgumentNullException(nameof(types));
      }

      foreach (var type in types)
      {
        if (type.IsDescendantOrMatches<IMixin>())
        {
          continue;
        }
        yield return type;
      }
    }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    [Pure]
    [CanBeNull]
    public static Type GetTaskReturnType([NotNull] this Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      Type taskReturnType;
      if (!type.IsDescendantOrMatches<Task>())
      {
        taskReturnType = null;
      }
      else if (type == typeof(Task))
      {
        taskReturnType = typeof(void);
      }
      else
      {
        taskReturnType = type.GetProperty(nameof(Task<object>.Result),
                                          TypeExtensions.TaskResultBindingFlags)
                             .PropertyType;
      }

      return taskReturnType;
    }
  }
}
