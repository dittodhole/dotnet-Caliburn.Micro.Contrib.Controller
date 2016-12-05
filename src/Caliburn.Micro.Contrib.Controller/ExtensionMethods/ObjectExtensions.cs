using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ExtensionMethods
{
  public static class ObjectExtensions
  {
    /// <exception cref="ArgumentNullException"><paramref name="obj" /> is <see langword="null" /></exception>
    [Pure]
    [CanBeNull]
    public static object WaitForResultIfTaskWithResult([NotNull] this object obj)
    {
      if (obj == null)
      {
        throw new ArgumentNullException(nameof(obj));
      }

      object result;
      var type = obj.GetType();
      if (!type.IsDescendantOrMatches<Task>())
      {
        result = null;
      }
      else if (type == typeof(Task))
      {
        result = null;
      }
      else
      {
        result = type.GetProperty(nameof(Task<object>.Result),
                                  TypeExtensions.TaskResultBindingFlags)
                     .GetValue(obj,
                               null);
      }

      return result;
    }
  }
}
