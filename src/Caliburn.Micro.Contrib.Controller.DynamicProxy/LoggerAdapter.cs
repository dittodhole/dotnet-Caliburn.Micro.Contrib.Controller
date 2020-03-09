using System;
using Castle.Core.Logging;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  public class LoggerAdapter : ILogger
  {
    private static ILog Logger { get; } = LogManager.GetLog.Invoke(typeof(LoggerAdapter));

    /// <inheritdoc/>
    public virtual ILogger CreateChildLogger(string loggerName)
    {
      return this;
    }

    /// <inheritdoc/>
    public virtual void Debug(string message)
    {
      LoggerAdapter.Logger.Info(message);
    }

    /// <inheritdoc/>
    public virtual void Debug(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      LoggerAdapter.Logger.Info(message);
    }

    /// <inheritdoc/>
    public virtual void Debug(string message,
                              Exception exception)
    {
      LoggerAdapter.Logger.Info(message,
                                exception);
    }

    /// <inheritdoc/>
    public virtual void DebugFormat(string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void DebugFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void DebugFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void DebugFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void Error(string message)
    {
      LoggerAdapter.Logger.Warn(message);
    }

    /// <inheritdoc/>
    public virtual void Error(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      LoggerAdapter.Logger.Warn(message);
    }

    /// <inheritdoc/>
    public virtual void Error(string message,
                              Exception exception)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    /// <inheritdoc/>
    public virtual void ErrorFormat(string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void ErrorFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    /// <inheritdoc/>
    public virtual void ErrorFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void ErrorFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    /// <inheritdoc/>
    public virtual void Fatal(string message)
    {
      LoggerAdapter.Logger.Warn(message);
    }

    /// <inheritdoc/>
    public virtual void Fatal(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      LoggerAdapter.Logger.Warn(message);
    }

    /// <inheritdoc/>
    public virtual void Fatal(string message,
                              Exception exception)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    /// <inheritdoc/>
    public virtual void FatalFormat(string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void FatalFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    /// <inheritdoc/>
    public virtual void FatalFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void FatalFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    /// <inheritdoc/>
    public virtual void Info(string message)
    {
      LoggerAdapter.Logger.Info(message);
    }

    /// <inheritdoc/>
    public virtual void Info(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      LoggerAdapter.Logger.Info(message);
    }

    /// <inheritdoc/>
    public virtual void Info(string message,
                             Exception exception)
    {
      LoggerAdapter.Logger.Info(message);
    }

    /// <inheritdoc/>
    public virtual void InfoFormat(string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void InfoFormat(Exception exception,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void InfoFormat(IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void InfoFormat(Exception exception,
                                   IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void Warn(string message)
    {
      LoggerAdapter.Logger.Warn(message);
    }

    /// <inheritdoc/>
    public virtual void Warn(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      LoggerAdapter.Logger.Warn(message);
    }

    /// <inheritdoc/>
    public virtual void Warn(string message,
                             Exception exception)
    {
      LoggerAdapter.Logger.Warn(message);
    }

    /// <inheritdoc/>
    public virtual void WarnFormat(string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void WarnFormat(Exception exception,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void WarnFormat(IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void WarnFormat(Exception exception,
                                   IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void Trace(string message)
    {
      LoggerAdapter.Logger.Info(message);
    }

    /// <inheritdoc/>
    public virtual void Trace(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      LoggerAdapter.Logger.Info(message);
    }

    /// <inheritdoc/>
    public virtual void Trace(string message,
                              Exception exception)
    {
      LoggerAdapter.Logger.Info(message);
    }

    /// <inheritdoc/>
    public virtual void TraceFormat(string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void TraceFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void TraceFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual void TraceFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    /// <inheritdoc/>
    public virtual bool IsDebugEnabled => true;

    /// <inheritdoc/>
    public virtual bool IsErrorEnabled => true;

    /// <inheritdoc/>
    public virtual bool IsFatalEnabled => true;

    /// <inheritdoc/>
    public virtual bool IsInfoEnabled => true;

    /// <inheritdoc/>
    public virtual bool IsWarnEnabled => true;

    /// <inheritdoc/>
    public virtual bool IsTraceEnabled => true;
  }
}
