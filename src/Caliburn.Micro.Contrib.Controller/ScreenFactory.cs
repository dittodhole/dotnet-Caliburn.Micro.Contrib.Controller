﻿using System;
using System.Collections.Generic;
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
    [Pure]
    [NotNull]
    IScreen Create([NotNull] Type screenType,
                   [NotNull] ControllerBase controller,
                   [CanBeNull] object options = null);
  }

  [PublicAPI]
  public class ScreenFactory : IScreenFactory,
                               IDisposable
  {
    [NotNull]
    private IWeakCollection<IScreen> Screens { get; } = new WeakCollection<IScreen>();

    public virtual void Dispose()
    {
      this.Screens.Dispose();
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    public virtual IScreen Create(Type screenType,
                                  ControllerBase controller,
                                  object options = null)
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

      var mixinProviders = this.GetMixinProviders(controller);
      var additionalInterfaces = this.GetAdditionalInterfaces(controller);
      var mixinInstances = this.GetMixinInstances(mixinProviders,
                                                  options);
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

    /// <exception cref="ArgumentNullException"><paramref name="mixinProviders" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual object[] GetMixinInstances([NotNull] [ItemNotNull] IEnumerable<IMixinProvider> mixinProviders,
                                                 [CanBeNull] object options = null)
    {
      if (mixinProviders == null)
      {
        throw new ArgumentNullException(nameof(mixinProviders));
      }

      var mixinInstances = mixinProviders.SelectMany(arg =>
                                                     {
                                                       var type = arg.GetType();
                                                       var methodInfos = type.GetMethods(TypeExtensions.DefaultBindingFlags);
                                                       var interfaces = type.GetInterfaces();
                                                       var result = interfaces.Where(@interface => @interface.IsDescendant<IMixinProvider>())
                                                                              .Where(@interface => @interface.IsGenericType)
                                                                              .Select(@interface => new
                                                                                                    {
                                                                                                      Type = type,
                                                                                                      MethodInfos = methodInfos,
                                                                                                      Interface = @interface,
                                                                                                      GenericTypeDefinition = @interface.GetGenericTypeDefinition(),
                                                                                                      GenericArguments = @interface.GetGenericArguments(),
                                                                                                      MixinProvider = arg
                                                                                                    })
                                                                              .Where(@interface => @interface.GenericTypeDefinition == typeof(IMixinInstance<>));

                                                       return result;
                                                     })
                                         .Select(arg =>
                                                 {
                                                   var mixinType = arg.GenericArguments.Single();
                                                   var mixinInstance = arg.MethodInfos.Single(methodInfo => methodInfo.DoesSignatureMatch(nameof(IMixinInstance<object>.CreateMixinInstance),
                                                                                                                                          mixinType,
                                                                                                                                          new[]
                                                                                                                                          {
                                                                                                                                            typeof(object)
                                                                                                                                          }))
                                                                          .Invoke(arg.MixinProvider,
                                                                                  new[]
                                                                                  {
                                                                                    options
                                                                                  });

                                                   return mixinInstance;
                                                 })
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

      var screenMixins = this.GetMixinProviders(controller);
      var typesToMixin = screenMixins.Select(arg => arg.GetType())
                                     .SelectMany(arg => arg.GetInterfaces())
                                     .Distinct()
                                     .Where(arg => arg.IsDescendant<IMixinProvider>())
                                     .Where(arg => arg.IsGenericType)
                                     .Select(arg => new
                                                    {
                                                      GenericTypeDefinition = arg.GetGenericTypeDefinition(),
                                                      GenericArguments = arg.GetGenericArguments()
                                                    })
                                     .Where(arg => arg.GenericTypeDefinition == typeof(IMixinProvider))
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

      var screenMixins = this.GetMixinProviders(controller);
      var customAttributeBuilders = screenMixins.OfType<IMixinAttributes>()
                                                .SelectMany(arg => arg.GetCustomAttributeBuilders())
                                                .ToArray();

      return customAttributeBuilders;
    }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual IMixinProvider[] GetMixinProviders([NotNull] ControllerBase controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var screenMixins = new object[]
                         {
                           controller
                         }.Concat(controller.Routines)
                          .OfType<IMixinProvider>()
                          .ToArray();

      return screenMixins;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="additionalInterfaces" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixinInstances" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="customAttributeBuilders" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="interceptor" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
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
