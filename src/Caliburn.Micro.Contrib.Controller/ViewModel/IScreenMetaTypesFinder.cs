using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ViewModel
{
  [PublicAPI]
  public interface IScreenMetaTypesFinder
  {
    /// <exception cref="ArgumentNullException"><paramref name="screenBaseType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="controllerMethodInvocations" /> is <see langword="null" /></exception>
    [NotNull]
    [ItemNotNull]
    Type[] GetAdditionalInterfacesToProxy([NotNull] Type screenBaseType,
                                          [NotNull] IEnumerable<ScreenInterceptor.ControllerMethodInvocation> controllerMethodInvocations);

    /// <exception cref="ArgumentNullException"><paramref name="screenBaseType" /> is <see langword="null" /></exception>
    [NotNull]
    [ItemNotNull]
    object[] GetMixinInstances([NotNull] Type screenBaseType);

    /// <exception cref="ArgumentNullException"><paramref name="screenBaseType" /> is <see langword="null" /></exception>
    /// <exception cref="ArgumentNullException"><paramref name="mixins" /> is <see langword="null" /></exception>
    void RegisterMixinsForType([NotNull] Type screenBaseType,
                               [ItemNotNull] [NotNull] Type[] mixins);
  }
}
