using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenFactory
  {
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    IScreen Create(Type screenType,
                   object?[] constructorArguments,
                   IController controller);
  }
}
