using Castle.DynamicProxy;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  public interface IMixinAttributes : IMixinProvider
  {
    CustomAttributeInfo[] GetCustomAttributeInfos();
  }
}
