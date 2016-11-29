using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ViewModel
{
  public interface IScreenInterceptor : IInterceptor
  {
    [NotNull]
    IScreen CreateProxiedScreen();
  }
}
