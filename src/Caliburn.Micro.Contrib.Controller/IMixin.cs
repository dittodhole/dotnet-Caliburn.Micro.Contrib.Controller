using System.Reflection.Emit;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface IMixin
  {
    [NotNull]
    [ItemNotNull]
    CustomAttributeBuilder[] GetCustomAttributeBuilders();
  }
}
