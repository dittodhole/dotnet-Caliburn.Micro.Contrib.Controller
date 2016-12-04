using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro.Contrib.Controller.ExtensionMethods;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ViewModel
{
  public class ScreenMetaTypesFinder : IScreenMetaTypesFinder
  {
    [NotNull]
    private IDictionary<Type, Type[]> MixinsForScreenBaseTypes { get; } = new Dictionary<Type, Type[]>();

    /// <exception cref="ArgumentNullException"><paramref name="controllerMethodInvocations" /> is <see langword="null" /></exception>
    public virtual Type[] GetAdditionalInterfacesToProxy(Type screenBaseType,
                                                         IEnumerable<ScreenInterceptor.ControllerMethodInvocation> controllerMethodInvocations)
    {
      if (controllerMethodInvocations == null)
      {
        throw new ArgumentNullException(nameof(controllerMethodInvocations));
      }

      var additionalInterfacesToProxy = controllerMethodInvocations.Select(arg => arg.InjectInterfaceDefinition)
                                                                   .Concat(this.GetMixinTypes(screenBaseType)
                                                                               .SelectMany(arg => arg.GetInterfaces()))
                                                                   .Where(arg => arg != null)
                                                                   .Where(arg => arg.IsInterface)
                                                                   .FilterNotifyInterfaces()
                                                                   .ToArray();

      return additionalInterfacesToProxy;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenBaseType" /> is <see langword="null" /></exception>
    public object[] GetMixinInstances(Type screenBaseType)
    {
      if (screenBaseType == null)
      {
        throw new ArgumentNullException(nameof(screenBaseType));
      }

      var mixinTypes = this.GetMixinTypes(screenBaseType);

      var mixinInstances = mixinTypes.Select(Activator.CreateInstance)
                                     .ToArray();

      return mixinInstances;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenBaseType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixins" /> is <see langword="null" /></exception>
    public virtual void RegisterMixinsForType(Type screenBaseType,
                                              Type[] mixins)
    {
      if (screenBaseType == null)
      {
        throw new ArgumentNullException(nameof(screenBaseType));
      }
      if (mixins == null)
      {
        throw new ArgumentNullException(nameof(mixins));
      }

      this.MixinsForScreenBaseTypes[screenBaseType] = mixins;
    }

    /// <exception cref="ArgumentNullException"><paramref name="screenBaseType" /> is <see langword="null" /></exception>
    [NotNull]
    [ItemNotNull]
    public virtual Type[] GetMixinTypes([NotNull] Type screenBaseType)
    {
      if (screenBaseType == null)
      {
        throw new ArgumentNullException(nameof(screenBaseType));
      }

      Type[] mixinTypes;
      if (!this.MixinsForScreenBaseTypes.TryGetValue(screenBaseType,
                                                     out mixinTypes))
      {
        mixinTypes = new Type[0];
      }

      return mixinTypes;
    }
  }
}
