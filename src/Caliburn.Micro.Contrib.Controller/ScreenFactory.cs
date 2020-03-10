using System;

namespace Caliburn.Micro.Contrib.Controller
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IScreenFactory
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    IScreenFactory With(IController controller);

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    IScreen CreateScreen(Type type,
                         object?[] args);
  }

  public sealed class ScreenFactory : IScreenFactory
  {
    /// <inheritdoc/>
    public IScreenFactory With(IController controller)
    {
      return this;
    }

    /// <inheritdoc/>
    public IScreen CreateScreen(Type type,
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

      var screen = Activator.CreateInstance(type,
                                            args);

      return (IScreen) screen;
    }
  }
}
