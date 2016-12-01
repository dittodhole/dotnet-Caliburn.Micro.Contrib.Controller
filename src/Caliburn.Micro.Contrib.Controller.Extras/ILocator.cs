using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller
{
  public interface ILocator<T>
  {
    [NotNull]
    T Locate();
  }
}
