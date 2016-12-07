using System;
using System.Linq;
using System.Reflection.Emit;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenFactory
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [NotNull]
    IScreen Create([NotNull] Type screenType,
                   [NotNull] ControllerBase controller);
  }

  [PublicAPI]
  public class ScreenFactory : IScreenFactory,
                               IDisposable
  {
    /// <exception cref="ArgumentNullException"><paramref name="mixinLocator" /> is <see langword="null" /></exception>
    public ScreenFactory([NotNull] ILocator<object> mixinLocator)
    {
      if (mixinLocator == null)
      {
        throw new ArgumentNullException(nameof(mixinLocator));
      }
      this.MixinLocator = mixinLocator;
    }

    [NotNull]
    private ILocator<object> MixinLocator { get; }

    [NotNull]
    private IWeakCollection<IScreen> Screens { get; } = new WeakCollection<IScreen>();

    public virtual void Dispose()
    {
      this.Screens.Dispose();
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [NotNull]
    public virtual IScreen Create([NotNull] Type screenType,
                                  [NotNull] ControllerBase controller)
    {
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var interceptor = new RerouteToControllerInterceptor(controller,
                                                           screenType);

      var additionalInterfaces = this.GetAdditionalInterfaces(controller);
      var mixinInstances = this.GetMixinInstances(controller);
      var customAttributeBuilders = this.GetCustomAttributeBuilders(controller);

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

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual Type[] GetAdditionalInterfaces([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var typesToMixin = this.GetTypesToMixin(controller);
      var additionalInterfaces = typesToMixin.Where(arg => arg.IsInterface)
                                             .ToArray();

      return additionalInterfaces;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual object[] GetMixinInstances([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var typesToMixin = this.GetTypesToMixin(controller);
      var mixinInstances = typesToMixin.Select(this.MixinLocator.LocateOptional)
                                       .Where(arg => arg != null)
                                       .ToArray();

      return mixinInstances;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual Type[] GetTypesToMixin([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var screenMixins = this.GetScreenMixins(controller);
      var typesToMixin = screenMixins.Select(arg => arg.GetType())
                                     .SelectMany(arg => arg.GetInterfaces())
                                     .Distinct()
                                     .Where(arg => arg.IsDescendant<IScreenMixin>())
                                     .Where(arg => arg.IsGenericType)
                                     .Select(arg => new
                                                    {
                                                      GenericTypeDefinition = arg.GetGenericTypeDefinition(),
                                                      GenericArguments = arg.GetGenericArguments()
                                                    })
                                     .Where(arg => arg.GenericTypeDefinition == typeof(IScreenMixin<>))
                                     .SelectMany(arg => arg.GenericArguments)
                                     .ToArray();

      return typesToMixin;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual CustomAttributeBuilder[] GetCustomAttributeBuilders([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var screenMixins = this.GetScreenMixins(controller);
      var customAttributeBuilders = screenMixins.OfType<IScreenAttributesMixin>()
                                                .SelectMany(arg => arg.GetCustomAttributeBuilders())
                                                .ToArray();

      return customAttributeBuilders;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual IScreenMixin[] GetScreenMixins([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var screenMixins = new object[]
                         {
                           controller
                         }.Concat(controller.Routines)
                          .OfType<IScreenMixin>()
                          .ToArray();

      return screenMixins;
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
