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

  public abstract class ScreenFactoryBase : IScreenFactory,
                                            IDisposable
  {
    private IWeakCollection<IScreen> Screens { get; } = new WeakCollection<IScreen>();

    public virtual void Dispose()
    {
      this.Screens.Dispose();
    }

    /// <inheritdoc/>
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

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    protected abstract IScreen CreateImpl(Type screenType,
                                          object?[] constructorArguments,
                                          IController controller);
  }
}
