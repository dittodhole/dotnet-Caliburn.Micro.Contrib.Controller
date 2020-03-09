using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public class ReflectionBasedScreenFactory : IScreenFactory
  {
    /// <inheritdoc/>
    protected override IScreen CreateImpl(Type screenType,
                                          object[] constructorArguments,
                                          IController controller)
    {
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }
      if (constructorArguments == null)
      {
        throw new ArgumentNullException(nameof(constructorArguments));
      }
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var screen = (IScreen) Activator.CreateInstance(screenType,
                                                      constructorArguments);

      return screen;
    }
  }
}
