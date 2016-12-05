using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Autofac.Core.Registration;
using Caliburn.Micro.Contrib.Controller.ViewModel;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.Autofac.ViewModel
{
  public class AutofacScreenInterceptor : ScreenInterceptor
  {
    protected const string GetConstructorBindingsMethodName = "GetConstructorBindings";

    /// <exception cref="ArgumentNullException"><paramref name="lifetimeScope" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controller" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="screenType" /> is <see langword="null" /></exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which has no parameters.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which cannot be found on <see cref="ScreenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="controller" /> has a method defined via <see cref="ScreenMethodLinkAttribute" />, which is not declared as <see langword="virtual" /> or <see langword="abstract" /> on <see cref="ScreenType" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="screenType" /> is an interface.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="screenType" /> is <see langword="sealed" />.</exception>
    /// <exception cref="InvalidOperationException">If <paramref name="screenType" /> does not implement <see cref="IScreen" />.</exception>
    public AutofacScreenInterceptor([NotNull] ILifetimeScope lifetimeScope,
                                    [NotNull] IController controller,
                                    [NotNull] Type screenType)
      : base(controller,
             screenType)
    {
      if (lifetimeScope == null)
      {
        throw new ArgumentNullException(nameof(lifetimeScope));
      }
      this.LifetimeScope = lifetimeScope;
    }

    [NotNull]
    private ILifetimeScope LifetimeScope { get; }

    /// <exception cref="InvalidOperationException">If the activator for <see cref="ScreenInterceptor.ScreenType" /> has no method 'InjectProperties' defined.</exception>
    /// <exception cref="TargetException" />
    /// <exception cref="TargetInvocationException" />
    /// <exception cref="TargetParameterCountException" />
    /// <exception cref="MethodAccessException" />
    /// <exception cref="ArgumentException" />
    /// <exception cref="NotSupportedException" />
    /// <exception cref="AmbiguousMatchException" />
    /// <exception cref="DependencyResolutionException" />
    /// <exception cref="ComponentNotRegisteredException" />
    public override IScreen CreateProxiedScreen()
    {
      object[] constructorArguments;

      IComponentRegistration registration;
      var service = new TypedService(this.ScreenType);
      if (this.LifetimeScope.ComponentRegistry.TryGetRegistration(service,
                                                                  out registration))
      {
        constructorArguments = this.GetResolvedConstructorArgumentsFromRegistration(registration);
      }
      else
      {
        constructorArguments = null;
      }

      var proxyGenerationOptions = new ProxyGenerationOptions();

      foreach (var mixinInstance in this.MixinTypes.Select(this.CreateMixinInstance))
      {
        proxyGenerationOptions.AddMixinInstance(mixinInstance);
      }

      var proxyGenerator = new ProxyGenerator();

      var additionalInterfacesToProxy = this.GetAdditionalInterfacesToProxy();
      var proxy = proxyGenerator.CreateClassProxy(this.ScreenType,
                                                  additionalInterfacesToProxy,
                                                  proxyGenerationOptions,
                                                  constructorArguments,
                                                  this);

      var screen = (IScreen) proxy;

      if (registration != null)
      {
        this.LifetimeScope.InjectProperties(screen);
      }

      return screen;
    }

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
    protected virtual object[] GetResolvedConstructorArgumentsFromRegistration([NotNull] IComponentRegistration registration)
    {
      if (registration == null)
      {
        throw new ArgumentNullException(nameof(registration));
      }

      var activatorType = registration.Activator.GetType();
      var reflectionActivator = registration.Activator as ReflectionActivator;
      if (reflectionActivator == null)
      {
        throw new InvalidOperationException($"The activator {activatorType} for {this.ScreenType} is not implementing {nameof(ReflectionActivator)}.");
      }

      var constructors = reflectionActivator.ConstructorFinder.FindConstructors(this.ScreenType);

      var constructorBindings = (IEnumerable<ConstructorParameterBinding>) activatorType.GetMethod(AutofacScreenInterceptor.GetConstructorBindingsMethodName,
                                                                                                   ScreenInterceptor.DefaultBindingFlags)
                                                                                        .Invoke(reflectionActivator,
                                                                                                new object[]
                                                                                                {
                                                                                                  this.LifetimeScope,
                                                                                                  Enumerable.Empty<global::Autofac.Core.Parameter>(),
                                                                                                  constructors
                                                                                                });
      var array = constructorBindings.Where(arg => arg.CanInstantiate)
                                     .ToArray();
      var constructorBinding = reflectionActivator.ConstructorSelector.SelectConstructorBinding(array);
      var constructorArguments = constructorBinding.TargetConstructor.GetParameters()
                                                   .Select(parameterInfo => parameterInfo.ParameterType)
                                                   .Select(parameterType => new TypedService(parameterType))
                                                   .Select(service => this.LifetimeScope.ResolveService(service))
                                                   .ToArray();

      return constructorArguments;
    }

    /// <exception cref="ArgumentNullException"><paramref name="type" /> is <see langword="null" /></exception>
    public override object CreateMixinInstance(Type type)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }

      object instance;
      if (this.LifetimeScope.IsRegistered(type))
      {
        instance = this.LifetimeScope.Resolve(type);
      }
      else
      {
        instance = base.CreateMixinInstance(type);
      }

      return instance;
    }
  }
}
