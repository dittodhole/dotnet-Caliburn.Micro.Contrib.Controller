namespace Caliburn.Micro.Contrib.Controller
{
  public interface IMixinInstance<T> : IMixinProvider
  {
    T CreateMixinInstance();
  }
}
