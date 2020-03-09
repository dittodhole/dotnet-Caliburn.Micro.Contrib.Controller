using System;

namespace Caliburn.Micro.Contrib.Controller
{
  public abstract class ScreenFactoryBase : IScreenFactory,
                                            IDisposable
  {
    private IWeakCollection<IScreen> Screens { get; } = new WeakCollection<IScreen>();

    public virtual void Dispose()
    {
      this.Screens.Dispose();
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="constructorArguments" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    public virtual IScreen Create(Type screenType,
                                  object[] constructorArguments,
                                  IController controller)
    {
      var screen = this.CreateImpl(screenType,
                                   constructorArguments,
                                   controller);

      this.Screens.Add(screen);

      EventHandler<DeactivationEventArgs> onDeactived = null;
      onDeactived = (sender,
                     args) =>
                    {
                      if (args.WasClosed)
                      {
                        screen.Deactivated -= onDeactived;
                        this.Screens.Remove(screen);
                      }
                    };
      screen.Deactivated += onDeactived;

      return screen;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="constructorArguments" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    protected abstract IScreen CreateImpl(Type screenType,
                                          object?[] constructorArguments,
                                          IController controller);
  }
}
