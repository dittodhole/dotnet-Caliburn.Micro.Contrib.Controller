using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Anotar.LibLog;
using Caliburn.Micro.Contrib.Controller.Proxy;
using Caliburn.Micro.Contrib.Controller.Proxy.ExtensionMethods;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenFactory
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixinProviders" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="interceptionTarget" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    IScreen Create([NotNull] Type screenType,
                   [NotNull] [ItemNotNull] IEnumerable<IMixinProvider> mixinProviders,
                   [NotNull] object interceptionTarget);
  }

  [PublicAPI]
  public class ScreenFactory : IScreenFactory,
                               IDisposable
  {
    static ScreenFactory()
    {
      var locateTypeForModelType = ViewLocator.LocateTypeForModelType;
      ViewLocator.LocateTypeForModelType = (modelType,
                                            displayLocation,
                                            context) =>
                                           {
                                             if (ProxyUtil.IsProxyType(modelType))
                                             {
                                               modelType = modelType.BaseType;
                                             }

                                             var viewType = locateTypeForModelType.Invoke(modelType,
                                                                                          displayLocation,
                                                                                          context);

                                             return viewType;
                                           };
    }

    [NotNull]
    private IWeakCollection<IScreen> Screens { get; } = new WeakCollection<IScreen>();

    [NotNull]
    private ConcurrentDictionary<Type, InterceptionTargetTypeMethodMapping> InterceptionTargetTypeMethodMappings { get; } = new ConcurrentDictionary<Type, InterceptionTargetTypeMethodMapping>();

    public virtual void Dispose()
    {
      this.Screens.Dispose();
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixinProviders" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="interceptionTarget" /> is <see langword="null" /></exception>
    /// <exception cref="TargetInvocationException">Thrown when constructor of type <paramref name="screenType" /> throws an exception.</exception>
    /// <exception cref="ArgumentException">Thrown when no constructor exists on type <paramref name="screenType" /> with matching parameters.</exception>
    /// <exception cref="Exception" />
    public virtual IScreen Create(Type screenType,
                                  IEnumerable<IMixinProvider> mixinProviders,
                                  object interceptionTarget)
    {
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }
      if (mixinProviders == null)
      {
        throw new ArgumentNullException(nameof(mixinProviders));
      }
      if (interceptionTarget == null)
      {
        throw new ArgumentNullException(nameof(interceptionTarget));
      }

      var interceptionTargetTypeMethodMapping = this.InterceptionTargetTypeMethodMappings.GetOrAdd(interceptionTarget.GetType(),
                                                                                                   InterceptionTargetTypeMethodMapping.Create);
      var interceptor = new InterceptProxyMethodAttributeBasedInterceptor(interceptionTarget,
                                                                          interceptionTargetTypeMethodMapping);

      var additionalInterfaces = this.GetAdditionalInterfaces(mixinProviders);
      var mixinInstances = this.GetMixinInstances(mixinProviders);
      var customAttributeBuilders = this.GetCustomAttributeBuilders(mixinProviders);

      var screen = this.CreateInternal(screenType,
                                       additionalInterfaces,
                                       mixinInstances,
                                       customAttributeBuilders,
                                       null,
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

    /// <exception cref="ArgumentNullException"><paramref name="mixinProviders" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual Type[] GetAdditionalInterfaces([NotNull] [ItemNotNull] IEnumerable<IMixinProvider> mixinProviders)
    {
      if (mixinProviders == null)
      {
        throw new ArgumentNullException(nameof(mixinProviders));
      }

      var additionalInterfaces = mixinProviders.SelectMany(arg =>
                                                           {
                                                             var type = arg.GetType();

                                                             Type[] interfaces;
                                                             try
                                                             {
                                                               interfaces = type.GetInterfaces();
                                                             }
                                                             catch (TargetInvocationException targetInvocationException)
                                                             {
                                                               LogTo.FatalException($"Could not get interfaces for {type}.",
                                                                                    targetInvocationException);
                                                               return Enumerable.Empty<GenericDefinition>();
                                                             }

                                                             var result = interfaces.Where(@interface => @interface.IsGenericType)
                                                                                    .Where(@interface => @interface.IsDescendant<IMixinProvider>())
                                                                                    .Select(@interface =>
                                                                                            {
                                                                                              var genericTypeDefinition = @interface.GetGenericTypeDefinition();
                                                                                              var genericArguments = @interface.GetGenericArguments();

                                                                                              var genericDefinition = new GenericDefinition(genericTypeDefinition,
                                                                                                                                            genericArguments);

                                                                                              return genericDefinition;
                                                                                            })
                                                                                    .Where(@interface => @interface.GenericTypeDefinition == typeof(IMixinInterface<>));

                                                             return result;
                                                           })
                                               .Select(arg => arg.GenericArguments.Single())
                                               .ToArray();

      return additionalInterfaces;
    }

    /// <exception cref="ArgumentNullException"><paramref name="mixinProviders" /> is <see langword="null" /></exception>
    /// <exception cref="Exception" />
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual object[] GetMixinInstances([NotNull] [ItemNotNull] IEnumerable<IMixinProvider> mixinProviders)
    {
      if (mixinProviders == null)
      {
        throw new ArgumentNullException(nameof(mixinProviders));
      }

      var mixinInstances = mixinProviders.SelectMany(arg =>
                                                     {
                                                       var type = arg.GetType();

                                                       Type[] interfaces;
                                                       var methodInfos = type.GetMethods(TypeExtensions.DefaultBindingFlags);
                                                       try
                                                       {
                                                         interfaces = type.GetInterfaces();
                                                       }
                                                       catch (TargetInvocationException targetInvocationException)
                                                       {
                                                         LogTo.FatalException($"Could not get interfaces for {type}.",
                                                                              targetInvocationException);
                                                         return Enumerable.Empty<MixinDefinition>();
                                                       }

                                                       var result = interfaces.Where(@interface => @interface.IsGenericType)
                                                                              .Where(@interface => @interface.IsDescendant<IMixinProvider>())
                                                                              .Select(@interface =>
                                                                                      {
                                                                                        var genericTypeDefinition = @interface.GetGenericTypeDefinition();
                                                                                        var genericArguments = @interface.GetGenericArguments();

                                                                                        var mixinDefinition = new MixinDefinition(type,
                                                                                                                                  methodInfos,
                                                                                                                                  @interface,
                                                                                                                                  genericTypeDefinition,
                                                                                                                                  genericArguments,
                                                                                                                                  arg);

                                                                                        return mixinDefinition;
                                                                                      })
                                                                              .Where(@interface => @interface.GenericTypeDefinition == typeof(IMixinInstance<>));

                                                       return result;
                                                     })
                                         .Select(arg =>
                                                 {
                                                   var mixinType = arg.GenericArguments.Single();

                                                   object mixinInstance;
                                                   try
                                                   {
                                                     mixinInstance = arg.MethodInfos.SingleOrDefault(methodInfo => methodInfo.DoesSignatureMatch(mixinType,
                                                                                                                                                 new Type[0],
                                                                                                                                                 nameof(IMixinInstance<object>.CreateMixinInstance)))
                                                                        ?.Invoke(arg.MixinProvider,
                                                                                 null);
                                                   }
                                                   catch (TargetException targetException)
                                                   {
                                                     LogTo.FatalException($"Could not call {nameof(IMixinInstance<object>.CreateMixinInstance)}",
                                                                          targetException);
                                                     mixinInstance = null;
                                                   }

                                                   return mixinInstance;
                                                 })
                                         .Where(arg => arg != null)
                                         .ToArray();

      return mixinInstances;
    }

    /// <exception cref="ArgumentNullException"><paramref name="mixinProviders" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    protected virtual CustomAttributeBuilder[] GetCustomAttributeBuilders([NotNull] [ItemNotNull] IEnumerable<IMixinProvider> mixinProviders)
    {
      if (mixinProviders == null)
      {
        throw new ArgumentNullException(nameof(mixinProviders));
      }

      var customAttributeBuilders = mixinProviders.OfType<IMixinAttributes>()
                                                  .SelectMany(arg => arg.GetCustomAttributeBuilders())
                                                  .ToArray();

      return customAttributeBuilders;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="additionalInterfaces" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixinInstances" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="customAttributeBuilders" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="interceptor" /> is <see langword="null" /></exception>
    /// <exception cref="TargetInvocationException">Thrown when constructor of type <paramref name="screenType" /> throws an exception.</exception>
    /// <exception cref="T:System.ArgumentException">Thrown when no constructor exists on type <paramref name="screenType" /> with matching parameters.</exception>
    [Pure]
    [NotNull]
    protected virtual IScreen CreateInternal([NotNull] Type screenType,
                                             [NotNull] Type[] additionalInterfaces,
                                             [NotNull] object[] mixinInstances,
                                             [NotNull] CustomAttributeBuilder[] customAttributeBuilders,
                                             [CanBeNull] object[] constructorParameters,
                                             [NotNull] object interceptor)
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

      object proxy;
      if (constructorParameters == null)
      {
        proxy = proxyGenerator.CreateClassProxy(screenType,
                                                additionalInterfaces,
                                                proxyGenerationOptions,
                                                (IInterceptor) interceptor);
      }
      else
      {
        proxy = proxyGenerator.CreateClassProxy(screenType,
                                                additionalInterfaces,
                                                proxyGenerationOptions,
                                                constructorParameters,
                                                (IInterceptor) interceptor);
      }

      var screen = (IScreen) proxy;

      return screen;
    }

    private sealed class MixinDefinition
    {
      public MixinDefinition([NotNull] Type type,
                             [NotNull] MethodInfo[] methodInfos,
                             [NotNull] Type @interface,
                             [NotNull] Type genericTypeDefinition,
                             [NotNull] Type[] genericArguments,
                             [NotNull] IMixinProvider mixinProvider)
      {
        this.Type = type;
        this.MethodInfos = methodInfos;
        this.Interface = @interface;
        this.GenericTypeDefinition = genericTypeDefinition;
        this.GenericArguments = genericArguments;
        this.MixinProvider = mixinProvider;
      }

      [NotNull]
      public Type Type { get; }

      [NotNull]
      public MethodInfo[] MethodInfos { get; }

      [NotNull]
      public Type Interface { get; }

      [NotNull]
      public Type GenericTypeDefinition { get; }

      [NotNull]
      public Type[] GenericArguments { get; }

      [NotNull]
      public IMixinProvider MixinProvider { get; }
    }

    private sealed class GenericDefinition
    {
      public GenericDefinition([NotNull] Type genericTypeDefinition,
                               [NotNull] Type[] genericArguments)
      {
        this.GenericTypeDefinition = genericTypeDefinition;
        this.GenericArguments = genericArguments;
      }

      [NotNull]
      public Type GenericTypeDefinition { get; }

      [NotNull]
      public Type[] GenericArguments { get; }
    }
  }
}
