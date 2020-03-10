using System;

namespace Caliburn.Micro.Contrib.Controller
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IScreenFactory
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    IScreenFactory<TScreen> With<TScreen>(IController<TScreen> controller) where TScreen : IScreen;
  }

  [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0048:File name must match type name", Justification = "<Pending>")]
  public interface IScreenFactory<TScreen> where TScreen : IScreen
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    TScreen CreateScreen(Type type,
                         object?[] args);
  }

  public sealed class ScreenFactory : IScreenFactory
  {
    /// <inheritdoc/>
    public IScreenFactory<TScreen> With<TScreen>(IController<TScreen> controller) where TScreen : IScreen
    {
      return ScreenFactoryImpl<TScreen>.Instance;
    }

    private sealed class ScreenFactoryImpl<TScreen> : IScreenFactory<TScreen> where TScreen : IScreen
    {
      public static ScreenFactoryImpl<TScreen> Instance { get; } = new ScreenFactoryImpl<TScreen>();

      private ScreenFactoryImpl() { }

      /// <inheritdoc/>
      public TScreen CreateScreen(Type type,
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

        return (TScreen) screen;
      }
    }
  }
}
