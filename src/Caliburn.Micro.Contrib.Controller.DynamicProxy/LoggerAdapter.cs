using System;
using Castle.Core.Logging;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  public class LoggerAdapter : ILogger
  {
    [NotNull]
    private static ILog Logger { get; } = LogManager.GetLog.Invoke(typeof(LoggerAdapter));

    /// <exception cref="ArgumentNullException"><paramref name="loggerName" /> is <see langword="null" /></exception>
    public virtual ILogger CreateChildLogger([NotNull] string loggerName)
    {
      return this;
    }

    public virtual void Debug(string message)
    {
      LoggerAdapter.Logger.Info(message);
    }

    public virtual void Debug(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      this.Debug(message);
    }

    public virtual void Debug(string message,
                              Exception exception)
    {
      LoggerAdapter.Logger.Info(message,
                                exception);
    }

    public virtual void DebugFormat(string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    public virtual void DebugFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    public virtual void DebugFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    public virtual void DebugFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    public virtual void Error(string message)
    {
      LoggerAdapter.Logger.Warn(message);
    }

    public virtual void Error(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      this.Error(message);
    }

    public virtual void Error(string message,
                              Exception exception)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    public virtual void ErrorFormat(string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    public virtual void ErrorFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    public virtual void ErrorFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    public virtual void ErrorFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    public virtual void Fatal(string message)
    {
      LoggerAdapter.Logger.Warn(message);
    }

    public virtual void Fatal(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      this.Fatal(message);
    }

    public virtual void Fatal(string message,
                              Exception exception)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    public virtual void FatalFormat(string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    public virtual void FatalFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    public virtual void FatalFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    public virtual void FatalFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      LoggerAdapter.Logger.Error(exception);
    }

    public virtual void Info(string message)
    {
      LoggerAdapter.Logger.Info(message);
    }

    public virtual void Info(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      this.Info(message);
    }

    public virtual void Info(string message,
                             Exception exception)
    {
      LoggerAdapter.Logger.Info(message);
    }

    public virtual void InfoFormat(string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    public virtual void InfoFormat(Exception exception,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    public virtual void InfoFormat(IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    public virtual void InfoFormat(Exception exception,
                                   IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Info(format,
                                args);
    }

    public virtual void Warn(string message)
    {
      LoggerAdapter.Logger.Warn(message);
    }

    public virtual void Warn(Func<string> messageFactory)
    {
      var message = messageFactory.Invoke();

      this.Warn(message);
    }

    public virtual void Warn(string message,
                             Exception exception)
    {
      LoggerAdapter.Logger.Warn(message);
    }

    public virtual void WarnFormat(string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    public virtual void WarnFormat(Exception exception,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    public virtual void WarnFormat(IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    public virtual void WarnFormat(Exception exception,
                                   IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      LoggerAdapter.Logger.Warn(format,
                                args);
    }

    public virtual bool IsDebugEnabled => true;
    public virtual bool IsErrorEnabled => true;
    public virtual bool IsFatalEnabled => true;
    public virtual bool IsInfoEnabled => true;
    public virtual bool IsWarnEnabled => true;
  }
}
