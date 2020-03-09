using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenFactoryAdapter
  {
    /// <exception cref="ArgumentException"/>
    /// <exception cref="Exception"/>
    IScreen CreateScreen(object? options = null);

    /// <exception cref="ArgumentException"/>
    /// <exception cref="Exception"/>
    Type GetScreenType(object? options = null);
  }

  public interface IScreenFactoryAdapter<TScreen> : IScreenFactoryAdapter
    where TScreen : IScreen
  {
    /// <exception cref="ArgumentException"/>
    /// <exception cref="Exception"/>
    new TScreen CreateScreen(object? options = null);
  }
}
