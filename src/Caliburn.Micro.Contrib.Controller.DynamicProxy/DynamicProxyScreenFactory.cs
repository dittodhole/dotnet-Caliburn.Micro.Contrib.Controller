using System;
using System.Reflection;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Castle.DynamicProxy;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  public sealed class DynamicProxyScreenFactory : IScreenFactory
  {
    static DynamicProxyScreenFactory()
    {
      var locateTypeForModelType = ViewLocator.LocateTypeForModelType;
      ViewLocator.LocateTypeForModelType = (modelType,
                                            displayLocation,
                                            context) =>
                                           {
                                             if (ProxyUtil.IsProxyType(modelType))
                                             {
                                               modelType = modelType.BaseType;
                                             }

                                             var result = locateTypeForModelType.Invoke(modelType,
                                                                                        displayLocation,
                                                                                        context);

                                             return result;
                                           };
    }

    /// <exception cref="ArgumentNullException"/>
    public DynamicProxyScreenFactory(IController controller)
    {
      this.Controller = controller ?? throw new ArgumentNullException(nameof(controller));
    }

    private IController Controller { get; }
    private ProxyGenerator ProxyGenerator { get; } = new ProxyGenerator
                                                     {
                                                       Logger = new Logger(typeof(ProxyGenerator))
                                                     };

    /// <inheritdoc/>
    public IScreen Create(Type type,
                          object?[] args)
    {
      if (type == null)
      {
        throw new ArgumentNullException(nameof(type));
      }
      if (args == null)
      {
        throw new ArgumentNullException(nameof(args));
      }

      var interceptor = new Interceptor(this.Controller);

      Type[] additionalInterfacesToProxy;
      if (this.Controller is IMixinProvider mixinProvider)
      {
        additionalInterfacesToProxy = null; // TODO
      }
      else
      {
        additionalInterfacesToProxy = new Type[0];
      }

      var proxy = this.ProxyGenerator.CreateClassProxy(type,
                                                       additionalInterfacesToProxy,
                                                       ProxyGenerationOptions.Default,
                                                       args,
                                                       interceptor);

      var result = (IScreen) proxy;

      return result;
    }

    /*
    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public virtual Type[] GetAdditionalInterfaces(IEnumerable<IMixinProvider> mixinProviders)
    {
      if (mixinProviders == null)
      {
        throw new ArgumentNullException(nameof(mixinProviders));
      }

      var additionalInterfaces = mixinProviders.SelectMany(arg =>
                                                           {
                                                             var type = arg.GetType();

                                                             Type[] interfaces;
                                                             try
                                                             {
                                                               interfaces = type.GetInterfaces();
                                                             }
                                                             catch (TargetInvocationException targetInvocationException)
                                                             {
                                                               ProxyScreenFactory.Logger.Error(targetInvocationException);
                                                               return Enumerable.Empty<GenericDefinition>();
                                                             }

                                                             var result = interfaces.Where(@interface => @interface.IsGenericType)
                                                                                    .Where(@interface => @interface.IsDescendant<IMixinProvider>())
                                                                                    .Select(@interface =>
                                                                                            {
                                                                                              var genericTypeDefinition = @interface.GetGenericTypeDefinition();
                                                                                              var genericArguments = @interface.GetGenericArguments();

                                                                                              var genericDefinition = new GenericDefinition(genericTypeDefinition,
                                                                                                                                            genericArguments);

                                                                                              return genericDefinition;
                                                                                            })
                                                                                    .Where(@interface => @interface.GenericTypeDefinition == typeof(IMixinInterface<>));

                                                             return result;
                                                           })
                                               .Select(arg => arg.GenericArguments.Single())
                                               .ToArray();

      return additionalInterfaces;
    }

    /// <exception cref="ArgumentNullException"/>
    /// <exception cref="Exception"/>
    public virtual CustomAttributeInfo[] GetCustomAttributeInfos(IEnumerable<IMixinProvider> mixinProviders)
    {
      if (mixinProviders == null)
      {
        throw new ArgumentNullException(nameof(mixinProviders));
      }

      var customAttributeInfos = mixinProviders.OfType<IMixinAttributes>()
                                               .SelectMany(arg => arg.GetCustomAttributeInfos())
                                               .ToArray();

      return customAttributeInfos;
    }

    private sealed class MixinDefinition
    {
      public MixinDefinition(Type type,
                             MethodInfo[] methodInfos,
                             Type @interface,
                             Type genericTypeDefinition,
                             Type[] genericArguments,
                             IMixinProvider mixinProvider)
      {
        this.Type = type;
        this.MethodInfos = methodInfos;
        this.Interface = @interface;
        this.GenericTypeDefinition = genericTypeDefinition;
        this.GenericArguments = genericArguments;
        this.MixinProvider = mixinProvider;
      }

      public Type Type { get; }
      public MethodInfo[] MethodInfos { get; }
      public Type Interface { get; }
      public Type GenericTypeDefinition { get; }
      public Type[] GenericArguments { get; }
      public IMixinProvider MixinProvider { get; }
    }

    private sealed class GenericDefinition
    {
      public GenericDefinition(Type genericTypeDefinition,
                               Type[] genericArguments)
      {
        this.GenericTypeDefinition = genericTypeDefinition;
        this.GenericArguments = genericArguments;
      }

      public Type GenericTypeDefinition { get; }
      public Type[] GenericArguments { get; }
    }

    */

    private sealed class Logger : ILogger
    {
      /// <exception cref="ArgumentNullException"/>
      /// <exception cref="Exception"/>
      public Logger(Type type)
      {
        if (type == null)
        {
          throw new ArgumentNullException(nameof(type));
        }
        this.Log = LogManager.GetLog.Invoke(type);
      }

      private ILog Log { get; }

      /// <inheritdoc/>
      public ILogger CreateChildLogger(string loggerName)
      {
        return this;
      }

      /// <inheritdoc/>
      public void Debug(string message)
      {
        this.Log.Info(message);
      }

      /// <inheritdoc/>
      public void Debug(Func<string> messageFactory)
      {
        var message = messageFactory.Invoke();

        this.Log.Info(message);
      }

      /// <inheritdoc/>
      public void Debug(string message,
                        Exception exception)
      {
        this.Log.Info(message,
                      exception);
      }

      /// <inheritdoc/>
      public void DebugFormat(string format,
                              params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void DebugFormat(Exception exception,
                              string format,
                                      params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void DebugFormat(IFormatProvider formatProvider,
                              string format,
                              params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void DebugFormat(Exception exception,
                              IFormatProvider formatProvider,
                              string format,
                              params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void Error(string message)
      {
        this.Log.Warn(message);
      }

      /// <inheritdoc/>
      public void Error(Func<string> messageFactory)
      {
        var message = messageFactory.Invoke();

        this.Log.Warn(message);
      }

      /// <inheritdoc/>
      public void Error(string message,
                        Exception exception)
      {
        this.Log.Error(exception);
      }

      /// <inheritdoc/>
      public void ErrorFormat(string format,
                              params object[] args)
      {
        this.Log.Warn(format,
                      args);
      }

      /// <inheritdoc/>
      public void ErrorFormat(Exception exception,
                              string format,
                              params object[] args)
      {
        this.Log.Error(exception);
      }

      /// <inheritdoc/>
      public void ErrorFormat(IFormatProvider formatProvider,
                              string format,
                              params object[] args)
      {
        this.Log.Warn(format,
                      args);
      }

      /// <inheritdoc/>
      public void ErrorFormat(Exception exception,
                              IFormatProvider formatProvider,
                              string format,
                              params object[] args)
      {
        this.Log.Error(exception);
      }

      /// <inheritdoc/>
      public void Fatal(string message)
      {
        this.Log.Warn(message);
      }

      /// <inheritdoc/>
      public void Fatal(Func<string> messageFactory)
      {
        var message = messageFactory.Invoke();

        this.Log.Warn(message);
      }

      /// <inheritdoc/>
      public void Fatal(string message,
                        Exception exception)
      {
        this.Log.Error(exception);
      }

      /// <inheritdoc/>
      public void FatalFormat(string format,
                              params object[] args)
      {
        this.Log.Warn(format,
                      args);
      }

      /// <inheritdoc/>
      public void FatalFormat(Exception exception,
                              string format,
                              params object[] args)
      {
        this.Log.Error(exception);
      }

      /// <inheritdoc/>
      public void FatalFormat(IFormatProvider formatProvider,
                              string format,
                              params object[] args)
      {
        this.Log.Warn(format,
                      args);
      }

      /// <inheritdoc/>
      public void FatalFormat(Exception exception,
                              IFormatProvider formatProvider,
                              string format,
                              params object[] args)
      {
        this.Log.Error(exception);
      }

      /// <inheritdoc/>
      public void Info(string message)
      {
        this.Log.Info(message);
      }

      /// <inheritdoc/>
      public void Info(Func<string> messageFactory)
      {
        var message = messageFactory.Invoke();

        this.Log.Info(message);
      }

      /// <inheritdoc/>
      public void Info(string message,
                       Exception exception)
      {
        this.Log.Info(message);
      }

      /// <inheritdoc/>
      public void InfoFormat(string format,
                             params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void InfoFormat(Exception exception,
                             string format,
                             params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void InfoFormat(IFormatProvider formatProvider,
                             string format,
                             params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void InfoFormat(Exception exception,
                             IFormatProvider formatProvider,
                             string format,
                             params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void Warn(string message)
      {
        this.Log.Warn(message);
      }

      /// <inheritdoc/>
      public void Warn(Func<string> messageFactory)
      {
        var message = messageFactory.Invoke();

        this.Log.Warn(message);
      }

      /// <inheritdoc/>
      public void Warn(string message,
                       Exception exception)
      {
        this.Log.Warn(message);
      }

      /// <inheritdoc/>
      public void WarnFormat(string format,
                             params object[] args)
      {
        this.Log.Warn(format,
                      args);
      }

      /// <inheritdoc/>
      public void WarnFormat(Exception exception,
                             string format,
                             params object[] args)
      {
        this.Log.Warn(format,
                      args);
      }

      /// <inheritdoc/>
      public void WarnFormat(IFormatProvider formatProvider,
                             string format,
                             params object[] args)
      {
        this.Log.Warn(format,
                      args);
      }

      /// <inheritdoc/>
      public void WarnFormat(Exception exception,
                             IFormatProvider formatProvider,
                             string format,
                             params object[] args)
      {
        this.Log.Warn(format,
                      args);
      }

      /// <inheritdoc/>
      public void Trace(string message)
      {
        this.Log.Info(message);
      }

      /// <inheritdoc/>
      public void Trace(Func<string> messageFactory)
      {
        var message = messageFactory.Invoke();

        this.Log.Info(message);
      }

      /// <inheritdoc/>
      public void Trace(string message,
                        Exception exception)
      {
        this.Log.Info(message);
      }

      /// <inheritdoc/>
      public void TraceFormat(string format,
                              params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void TraceFormat(Exception exception,
                              string format,
                              params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void TraceFormat(IFormatProvider formatProvider,
                              string format,
                              params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public void TraceFormat(Exception exception,
                              IFormatProvider formatProvider,
                              string format,
                              params object[] args)
      {
        this.Log.Info(format,
                      args);
      }

      /// <inheritdoc/>
      public bool IsDebugEnabled => true;

      /// <inheritdoc/>
      public bool IsErrorEnabled => true;

      /// <inheritdoc/>
      public bool IsFatalEnabled => true;

      /// <inheritdoc/>
      public bool IsInfoEnabled => true;

      /// <inheritdoc/>
      public bool IsWarnEnabled => true;

      /// <inheritdoc/>
      public bool IsTraceEnabled => true;
    }

    private sealed class Interceptor : IInterceptor
    {
      /// <exception cref="ArgumentNullException"/>
      public Interceptor(IController controller)
      {
        this.Controller = controller ?? throw new ArgumentNullException(nameof(controller));
      }

      private IController Controller { get; }

      /// <inheritdoc/>
      public void Intercept(IInvocation invocation)
      {
        if (invocation == null)
        {
          throw new ArgumentNullException(nameof(invocation));
        }

        var screenMethodInfo = invocation.GetConcreteMethodInvocationTarget();
        if (screenMethodInfo != null)
        {
          if (screenMethodInfo.ReturnType != typeof(Task))
          { // tasks are not executed here, no intention on awaiting here!
            screenMethodInfo.Invoke(invocation.InvocationTarget,
                                    invocation.Arguments);
          }
        }

        var proxyMethodInfo = invocation.GetConcreteMethod();

        var interceptingMethodInfo = Contrib.Controller.Controller.GetInterceptingMethodInfo(this.Controller,
                                                                                             BindingFlags.Default,
                                                                                             proxyMethodInfo.Name,
                                                                                             proxyMethodInfo.ReturnType,
                                                                                             proxyMethodInfo.GetParameters());
        if (interceptingMethodInfo != null)
        {
          var parameters = new object[invocation.Arguments.Length + 1];
          parameters[0] = invocation.InvocationTarget;

          Array.Copy(invocation.Arguments,
                     0,
                     parameters,
                     1,
                     invocation.Arguments.Length);

          var returnValue = interceptingMethodInfo.Invoke(this.Controller,
                                                          parameters);

          invocation.ReturnValue = returnValue;
        }
      }
    }
  }
}
