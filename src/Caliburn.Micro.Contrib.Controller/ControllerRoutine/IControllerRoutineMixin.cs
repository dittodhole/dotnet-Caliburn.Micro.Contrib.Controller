using System.Reflection.Emit;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ControllerRoutine
{
  public interface IControllerRoutineMixin
  {
    [NotNull]
    [ItemNotNull]
    CustomAttributeBuilder[] GetCustomAttributeBuilders();
  }

  public interface IControllerRoutineMixin<[UsedImplicitly] TMixin> : IControllerRoutineMixin
    where TMixin : class, new() {}
}
