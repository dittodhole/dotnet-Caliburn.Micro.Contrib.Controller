namespace Caliburn.Micro.Contrib.Controller
{
  public interface IMixinProvider { }

  public interface IMixinInstance<T> : IMixinProvider
  {
    T CreateMixinInstance();
  }

  public interface IMixinInterface<TInterface> : IMixinProvider { }
}
