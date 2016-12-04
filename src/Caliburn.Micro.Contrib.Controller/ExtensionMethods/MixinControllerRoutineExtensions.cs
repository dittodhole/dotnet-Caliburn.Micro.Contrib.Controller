using System;
using System.Linq;
using Caliburn.Micro.Contrib.Controller.ControllerRoutine;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ExtensionMethods
{
  public static class MixinControllerRoutineExtensions
  {
    /// <exception cref="ArgumentNullException"><paramref name="mixinControllerRoutine" /> is <see langword="null" /></exception>
    public static Type[] GetMixins([NotNull] this IMixinControllerRoutine mixinControllerRoutine)
    {
      if (mixinControllerRoutine == null)
      {
        throw new ArgumentNullException(nameof(mixinControllerRoutine));
      }

      var concreteMixinTypes = mixinControllerRoutine.GetType()
                                                     .GetInterfaces()
                                                     .Where(arg => arg.IsDescendant<IMixinControllerRoutine>())
                                                     .Where(arg => arg != typeof(IMixinControllerRoutine))
                                                     .Where(arg => arg.IsGenericType)
                                                     .Select(arg => new
                                                                    {
                                                                      GenericTypeDefinition = arg.GetGenericTypeDefinition(),
                                                                      GenericArguments = arg.GetGenericArguments()
                                                                    })
                                                     .Where(arg => arg.GenericTypeDefinition == typeof(IMixinControllerRoutine<>))
                                                     .SelectMany(arg => arg.GenericArguments)
                                                     .Where(arg => arg.IsClass)
                                                     .ToArray();

      return concreteMixinTypes;
    }
  }
}
