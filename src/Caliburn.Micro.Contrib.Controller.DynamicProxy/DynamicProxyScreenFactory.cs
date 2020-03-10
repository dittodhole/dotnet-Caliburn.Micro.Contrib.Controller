﻿using System;
using System.Linq;
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

    private ProxyGenerator ProxyGenerator { get; } = new ProxyGenerator
                                                     {
                                                       Logger = new Logger(typeof(ProxyGenerator))
                                                     };

    /// <inheritdoc/>
    public IScreenFactory With(IController controller)
    {
      if (controller == null)
      {
        throw new ArgumentNullException(nameof(controller));
      }

      var result = new ScreenFactory(this.ProxyGenerator,
                                     controller);

      return result;
    }

    /// <inheritdoc/>
    IScreen IScreenFactory.CreateScreen(Type type,
                                        object?[] args)
    {
      throw new InvalidOperationException($"Call {nameof(this.With)} first");
    }

    private sealed class ScreenFactory : IScreenFactory
    {
      /// <exception cref="ArgumentNullException"/>
      public ScreenFactory(ProxyGenerator proxyGenerator,
                           IController controller)
      {
        this.ProxyGenerator = proxyGenerator ?? throw new ArgumentNullException(nameof(proxyGenerator));
        this.Controller = controller ?? throw new ArgumentNullException(nameof(controller));
      }

      private ProxyGenerator ProxyGenerator { get; }
      private IController Controller { get; }

      /// <inheritdoc/>
      IScreenFactory IScreenFactory.With(IController controller)
      {
        throw new InvalidOperationException($"{this.GetType()} is already bound to {this.Controller.GetType()}");
      }

      /// <inheritdoc/>
      public IScreen CreateScreen(Type type,
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

        var additionalInterfacesToProxy = this.Controller.GetType()
                                                         .GetInterfaces()
                                                         .Where(arg => arg.IsGenericType)
                                                         .Where(arg => arg.GetGenericTypeDefinition() == typeof(IMixinInterface<>))
                                                         .Select(arg => arg.GetGenericArguments().Single())
                                                         .ToArray();

        var proxy = this.ProxyGenerator.CreateClassProxy(type,
                                                         additionalInterfacesToProxy,
                                                         ProxyGenerationOptions.Default,
                                                         args,
                                                         interceptor);

        var result = (IScreen) proxy;

        return result;
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
  }
}
