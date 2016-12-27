using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IMixinInstance<T> : IMixinProvider
  {
    [UsedImplicitly]
    [Pure]
    [NotNull]
    T CreateMixinInstance([CanBeNull] object options = null);
  }
}
