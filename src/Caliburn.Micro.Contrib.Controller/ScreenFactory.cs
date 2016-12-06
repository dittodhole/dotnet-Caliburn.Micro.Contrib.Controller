using System;
using System.Reflection.Emit;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenFactory
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="additionalInterfaces" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixinInstances" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="customAttributeBuilders" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [NotNull]
    IScreen Create([NotNull] Type screenType,
                   [NotNull] [ItemNotNull] Type[] additionalInterfaces,
                   [NotNull] [ItemNotNull] object[] mixinInstances,
                   [NotNull] [ItemNotNull] CustomAttributeBuilder[] customAttributeBuilders,
                   [NotNull] ControllerBase controller);
  }

  [PublicAPI]
  public class ScreenFactory : IScreenFactory,
                               IDisposable
  {
    [NotNull]
    private IWeakCollection<IScreen> Screens { get; } = new WeakCollection<IScreen>();

    void IDisposable.Dispose()
    {
      this.Screens.Dispose();
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="additionalInterfaces" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixinInstances" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="customAttributeBuilders" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [NotNull]
    public virtual IScreen Create([NotNull] Type screenType,
                                  [NotNull] Type[] additionalInterfaces,
                                  [NotNull] object[] mixinInstances,
                                  [NotNull] CustomAttributeBuilder[] customAttributeBuilders,
                                  [NotNull] ControllerBase controller)
    {
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }
      if (additionalInterfaces == null)
      {
        throw new ArgumentNullException(nameof(additionalInterfaces));
      }
      if (mixinInstances == null)
      {
        throw new ArgumentNullException(nameof(mixinInstances));
      }
      if (customAttributeBuilders == null)
      {
        throw new ArgumentNullException(nameof(customAttributeBuilders));
      }
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var interceptor = new RerouteToControllerInterceptor(controller,
                                                           screenType);

      var screen = this.CreateInternal(screenType,
                                       additionalInterfaces,
                                       mixinInstances,
                                       customAttributeBuilders,
                                       interceptor);

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
    /// <exception cref="ArgumentNullException"><paramref name="additionalInterfaces" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixinInstances" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="customAttributeBuilders" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="interceptor" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    protected virtual IScreen CreateInternal([NotNull] Type screenType,
                                             [NotNull] Type[] additionalInterfaces,
                                             [NotNull] object[] mixinInstances,
                                             [NotNull] CustomAttributeBuilder[] customAttributeBuilders,
                                             [NotNull] IInterceptor interceptor)
    {
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }
      if (additionalInterfaces == null)
      {
        throw new ArgumentNullException(nameof(additionalInterfaces));
      }
      if (mixinInstances == null)
      {
        throw new ArgumentNullException(nameof(mixinInstances));
      }
      if (customAttributeBuilders == null)
      {
        throw new ArgumentNullException(nameof(customAttributeBuilders));
      }
      if (interceptor == null)
      {
        throw new ArgumentNullException(nameof(interceptor));
      }

      var proxyGenerationOptions = new ProxyGenerationOptions();
      foreach (var mixinInstance in mixinInstances)
      {
        proxyGenerationOptions.AddMixinInstance(mixinInstance);
      }
      foreach (var customAttributeBuilder in customAttributeBuilders)
      {
        proxyGenerationOptions.AdditionalAttributes.Add(customAttributeBuilder);
      }

      var proxyGenerator = new ProxyGenerator();

      var proxy = proxyGenerator.CreateClassProxy(screenType,
                                                  additionalInterfaces,
                                                  proxyGenerationOptions,
                                                  interceptor);
      var screen = (IScreen) proxy;

      return screen;
    }
  }
}
