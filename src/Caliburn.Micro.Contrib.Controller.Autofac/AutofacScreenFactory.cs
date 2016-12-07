using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Registration;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Autofac
{
  [PublicAPI]
  public class AutofacScreenFactory : ScreenFactory
  {
    protected const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    protected const string GetConstructorBindingsMethodName = "GetConstructorBindings";

    /// <exception cref="ArgumentNullException"><paramref name="lifetimeScope" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixinLocator" /> is <see langword="null" /></exception>
    public AutofacScreenFactory([NotNull] ILifetimeScope lifetimeScope,
                                [NotNull] ILocator<object> mixinLocator)
      : base(mixinLocator)
    {
      if (lifetimeScope == null)
      {
        throw new ArgumentNullException(nameof(lifetimeScope));
      }
      this.LifetimeScope = lifetimeScope;
    }

    [NotNull]
    private ILifetimeScope LifetimeScope { get; }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="additionalInterfaces" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixinInstances" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="customAttributeBuilders" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="interceptor" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException">If the screen is registered with a non <see cref="ReflectionActivator" /> in Autofac.</exception>
    /// <exception cref="MethodAccessException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="TargetParameterCountException" />
    /// <exception cref="TargetInvocationException" />
    /// <exception cref="TargetException" />
    /// <exception cref="ArgumentException" />
    /// <exception cref="AmbiguousMatchException" />
    /// <exception cref="DependencyResolutionException" />
    /// <exception cref="ComponentNotRegisteredException" />
    protected override IScreen CreateInternal(Type screenType,
                                              Type[] additionalInterfaces,
                                              object[] mixinInstances,
                                              CustomAttributeBuilder[] customAttributeBuilders,
                                              IInterceptor interceptor)
    {
      object[] constructorArguments;

      IComponentRegistration registration;
      var service = new TypedService(screenType);
      if (this.LifetimeScope.ComponentRegistry.TryGetRegistration(service,
                                                                  out registration))
      {
        constructorArguments = this.GetResolvedConstructorArgumentsFromRegistration(screenType,
                                                                                    registration);
      }
      else
      {
        constructorArguments = null;
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
                                                  constructorArguments,
                                                  interceptor);

      var screen = (IScreen) proxy;

      if (registration != null)
      {
        this.LifetimeScope.InjectProperties(screen);
      }

      return screen;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="registration" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException">If the screen is registered with a non <see cref="ReflectionActivator" /> in Autofac.</exception>
    /// <exception cref="MethodAccessException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="TargetParameterCountException" />
    /// <exception cref="TargetInvocationException" />
    /// <exception cref="TargetException" />
    /// <exception cref="ArgumentException" />
    /// <exception cref="AmbiguousMatchException" />
    /// <exception cref="DependencyResolutionException" />
    /// <exception cref="ComponentNotRegisteredException" />
    [Pure]
    [NotNull]
    protected virtual object[] GetResolvedConstructorArgumentsFromRegistration([NotNull] Type screenType,
                                                                               [NotNull] IComponentRegistration registration)
    {
      if (screenType == null)
      {
        throw new ArgumentNullException(nameof(screenType));
      }
      if (registration == null)
      {
        throw new ArgumentNullException(nameof(registration));
      }

      var activatorType = registration.Activator.GetType();
      var reflectionActivator = registration.Activator as ReflectionActivator;
      if (reflectionActivator == null)
      {
        throw new InvalidOperationException($"The activator {activatorType} for {screenType} is not implementing {nameof(ReflectionActivator)}.");
      }

      var constructors = reflectionActivator.ConstructorFinder.FindConstructors(screenType);

      var constructorBindings = (IEnumerable<ConstructorParameterBinding>) activatorType.GetMethod(AutofacScreenFactory.GetConstructorBindingsMethodName,
                                                                                                   AutofacScreenFactory.DefaultBindingFlags)
                                                                                        .Invoke(reflectionActivator,
                                                                                                new object[]
                                                                                                {
                                                                                                  this.LifetimeScope,
                                                                                                  Enumerable.Empty<global::Autofac.Core.Parameter>(),
                                                                                                  constructors
                                                                                                });
      var array = constructorBindings.Where(arg => arg.CanInstantiate)
                                     .ToArray();

      object[] constructorArguments;
      var constructorSelector = reflectionActivator.ConstructorSelector;
      if (constructorSelector is MostParametersConstructorSelector
          && !array.Any())
      {
        constructorArguments = new object[0];
      }
      else
      {
        var constructorBinding = constructorSelector.SelectConstructorBinding(array);
        constructorArguments = constructorBinding.TargetConstructor.GetParameters()
                                                 .Select(parameterInfo => parameterInfo.ParameterType)
                                                 .Select(parameterType => new TypedService(parameterType))
                                                 .Select(service => this.LifetimeScope.ResolveService(service))
                                                 .ToArray();
      }

      return constructorArguments;
    }
  }
}
