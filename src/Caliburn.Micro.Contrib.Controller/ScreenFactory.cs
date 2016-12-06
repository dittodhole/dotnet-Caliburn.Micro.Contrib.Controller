using System;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenFactory : IDisposable
  {
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [Pure]
    [CanBeNull]
    IScreen Create([NotNull] ControllerBase controller,
                   [CanBeNull] object options = null);
  }

  public class ScreenFactory : IScreenFactory
  {
    [NotNull]
    private IWeakCollection<IScreenInterceptor> ScreenInterceptors { get; } = new WeakCollection<IScreenInterceptor>();

    [NotNull]
    private IWeakCollection<IScreen> Screens { get; } = new WeakCollection<IScreen>();

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException" />
    /// <exception cref="Exception" />
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
      this.ScreenInterceptors.Add(screenInterceptor);

      var screen = this.CreateScreen(screenInterceptor);
      this.Screens.Add(screen);

      return screen;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenInterceptor" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [NotNull]
    public virtual IScreen CreateScreen([NotNull] IScreenInterceptor screenInterceptor)
    {
      if (screenInterceptor == null)
      {
        throw new ArgumentNullException(nameof(screenInterceptor));
      }

      var screen = screenInterceptor.CreateProxiedScreen();

      return screen;
    }

    public void Dispose()
    {
      this.ScreenInterceptors.Dispose();
      this.Screens.Dispose();
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

      return screenInterceptor;
    }
  }
}
