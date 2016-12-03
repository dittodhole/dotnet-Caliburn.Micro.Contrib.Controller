using System;
using Castle.DynamicProxy;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.ViewModel
{
  public interface IScreenInterceptor : IInterceptor
  {
    /// <exception cref="Exception" />
    [NotNull]
    IScreen CreateProxiedScreen();
  }
}
