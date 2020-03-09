using System;
using System.Reflection;

namespace Caliburn.Micro.Contrib.Controller.ExtensionMethods
{
  public static class TypeExtensions
  {
    public const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    public static bool IsDescendantOrMatches<T>(this Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      return type.IsDescendantOrMatches(typeof(T));
    }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    public static bool IsDescendant<T>(this Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      return type.IsDescendant(typeof(T));
    }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="parentType" /> is <see langword="null" /></exception>
    public static bool IsDescendant(this Type type,
                                    Type parentType)
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
    public static bool IsDescendantOrMatches(this Type type,
                                             Type parentType)
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
  }
}
