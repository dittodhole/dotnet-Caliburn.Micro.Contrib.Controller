using System;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenFactory
  {
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException" />
    [Pure]
    [CanBeNull]
    IScreen Create([NotNull] ControllerBase controller,
                   [CanBeNull] object options = null);
  }

  public class ScreenFactory : IScreenFactory
  {
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException" />
    public IScreen Create(ControllerBase controller,
                          object options = null)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var screenType = controller.GetScreenType(options);
      var screenInterceptor = new ScreenInterceptor(controller,
                                                    screenType);

      var screen = screenInterceptor.CreateProxiedScreen();

      return screen;
    }
  }
}
