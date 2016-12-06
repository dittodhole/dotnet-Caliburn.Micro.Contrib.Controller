using System;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenFactory : IDisposable
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
    [NotNull]
    private IWeakCollection<IScreenInterceptor> ScreenInterceptors { get; } = new WeakCollection<IScreenInterceptor>();

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException" />
    public virtual IScreen Create(ControllerBase controller,
                                  object options = null)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var screenType = controller.GetScreenType(options);
      var screenInterceptor = this.CreateScreenInterceptor(controller,
                                                           screenType);

      var screen = screenInterceptor.CreateProxiedScreen();

      return screen;
    }

    public void Dispose()
    {
      this.ScreenInterceptors.Dispose();
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
    [NotNull]
    public virtual IScreenInterceptor CreateScreenInterceptor([NotNull] ControllerBase controller,
                                                              [NotNull] Type screenType)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }

      var screenInterceptor = new ScreenInterceptor(controller,
                                                    screenType);

      this.ScreenInterceptors.Add(screenInterceptor);

      return screenInterceptor;
    }
  }
}
