using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public class ReflectionBasedScreenFactory : ScreenFactoryBase
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="constructorArguments" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
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
