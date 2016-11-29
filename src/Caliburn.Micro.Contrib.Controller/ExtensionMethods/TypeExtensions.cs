using System;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ExtensionMethods
{
  [PublicAPI]
  public static class TypeExtensions
  {
    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException">If <paramref name="type" /> is an interface.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="type" /> does not implement <see cref="IScreen" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="type" /> is <see langword="sealed"/>.</exception>
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
      if (!typeof(IScreen).IsAssignableFrom(type))
      {
        throw new InvalidOperationException($"Cannot create proxy for {type}, as this type does implement {nameof(IScreen)}.");
      }
      if (type.IsSealed)
      {
        throw new InvalidOperationException($"Cannot create proxy for {type}, as this type is sealed.");
      }
    }
  }
}
