using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenFactory
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    IScreen Create(Type type,
                   object?[] args);
  }

  public sealed class ScreenFactory : IScreenFactory
  {
    /// <inheritdoc/>
    public IScreen Create(Type type,
                          object?[] args)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }
      if (args == null)
      {
        throw new ArgumentNullException(nameof(args));
      }

      var result = (IScreen) Activator.CreateInstance(type,
                                                      args);

      return result;
    }
  }
}
