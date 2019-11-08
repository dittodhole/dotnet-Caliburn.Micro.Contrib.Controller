using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  public class ProxyScreenFactory : ScreenFactoryBase
  {
    [NotNull]
    private static ILog Logger { get; } = LogManager.GetLog.Invoke(typeof(ProxyScreenFactory));

    static ProxyScreenFactory()
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

    /// <exception cref="ArgumentNullException"><paramref name="proxyGenerator" /> is <see langword="null" /></exception>
    public ProxyScreenFactory([NotNull] ProxyGenerator proxyGenerator)
    {
      this.ProxyGenerator = proxyGenerator ?? throw new ArgumentNullException(nameof(proxyGenerator));
    }

    /// <exception cref="ArgumentNullException"><paramref name="logger" /> is <see langword="null" /></exception>
    public ProxyScreenFactory([NotNull] ILogger logger)
    {
      if (logger == null)
      {
        throw new ArgumentNullException(nameof(logger));
      }
      this.ProxyGenerator = new ProxyGenerator
                            {
                              Logger = logger
                            };
    }

    public ProxyScreenFactory()
    {
      this.ProxyGenerator = new ProxyGenerator
                            {
                              Logger = new LoggerAdapter()
                            };
    }

    [NotNull]
    private ProxyGenerator ProxyGenerator { get; }

    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    [Pure]
    [ItemNotNull]
    [NotNull]
    public virtual IMixinProvider[] GetMixinProviders([NotNull] IController controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var mixinProviders = new object[]
                           {
                             controller
                           }.Concat(controller.Routines)
                            .OfType<IMixinProvider>()
                            .ToArray();

      return mixinProviders;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="constructorArguments" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="TargetInvocationException">Thrown when constructor of type <paramref name="screenType" /> throws an exception.</exception>
    /// <exception cref="ArgumentException">Thrown when no constructor exists on type <paramref name="screenType" /> with matching parameters.</exception>
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

      var interceptor = new ControllerHandlesEventsInterceptor(controller);

      var mixinProviders = this.GetMixinProviders(controller);

      var additionalInterfaces = this.GetAdditionalInterfaces(mixinProviders);
      var mixinInstances = this.GetMixinInstances(mixinProviders);
      var customAttributeInfos = this.GetCustomAttributeInfos(mixinProviders);

      var proxyGenerationOptions = new ProxyGenerationOptions();
      foreach (var mixinInstance in mixinInstances)
      {
        proxyGenerationOptions.AddMixinInstance(mixinInstance);
      }
      foreach (var customAttributeInfo in customAttributeInfos)
      {
        proxyGenerationOptions.AdditionalAttributes.Add(customAttributeInfo);
      }

      var proxy = this.ProxyGenerator.CreateClassProxy(screenType,
                                                       additionalInterfaces,
                                                       proxyGenerationOptions,
                                                       constructorArguments,
                                                       interceptor);

      var screen = (IScreen) proxy;

      return screen;
    }

    /// <exception cref="ArgumentNullException"><paramref name="mixinProviders" /> is <see langword="null" /></exception>
    [Pure]
    [NotNull]
    [ItemNotNull]
    public virtual Type[] GetAdditionalInterfaces([NotNull] [ItemNotNull] IEnumerable<IMixinProvider> mixinProviders)
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
                                                               ProxyScreenFactory.Logger.Error(targetInvocationException);
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
    public virtual object[] GetMixinInstances([NotNull] [ItemNotNull] IEnumerable<IMixinProvider> mixinProviders)
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
                                                         ProxyScreenFactory.Logger.Error(targetInvocationException);
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
                                                     ProxyScreenFactory.Logger.Error(targetException);
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
    public virtual CustomAttributeInfo[] GetCustomAttributeInfos([NotNull] [ItemNotNull] IEnumerable<IMixinProvider> mixinProviders)
    {
      if (mixinProviders == null)
      {
        throw new ArgumentNullException(nameof(mixinProviders));
      }

      var customAttributeInfos = mixinProviders.OfType<IMixinAttributes>()
                                               .SelectMany(arg => arg.GetCustomAttributeInfos())
                                               .ToArray();

      return customAttributeInfos;
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
