using System.Reflection.Emit;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IScreenAttributesMixin
  {
    [Pure]
    [ItemNotNull]
    [NotNull]
    CustomAttributeBuilder[] GetCustomAttributeBuilders();
  }
}
