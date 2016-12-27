using System.Reflection.Emit;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IMixinAttributes : IMixinProvider
  {
    [Pure]
    [ItemNotNull]
    [NotNull]
    CustomAttributeBuilder[] GetCustomAttributeBuilders();
  }
}
