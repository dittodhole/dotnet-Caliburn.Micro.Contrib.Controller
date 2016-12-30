using System.Reflection.Emit;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IMixinAttributes : IMixinProvider
  {
    [Pure]
    [NotNull]
    [ItemNotNull]
    CustomAttributeBuilder[] GetCustomAttributeBuilders();
  }
}
