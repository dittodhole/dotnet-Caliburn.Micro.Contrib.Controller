using System;

namespace Caliburn.Micro.Contrib.Controller
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
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

      var result = Activator.CreateInstance(type,
                                            args);

      return (IScreen) result;
    }
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public static class ScreenFactoryExtensions
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public static TScreen Create<TScreen>(this IScreenFactory screenFactory,
                                          object?[] args) where TScreen : IScreen
    {
      if (screenFactory == null)
      {
        throw new ArgumentNullException(nameof(screenFactory));
      }
      if (args == null)
      {
        throw new ArgumentNullException(nameof(args));
      }

      var type = typeof(TScreen);

      var result = screenFactory.Create(type,
                                        args);

      return (TScreen) result;
    }
  }
}
