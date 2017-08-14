using System;
using Caliburn.Micro.Contrib.Controller.DynamicProxy.Logging;
using Castle.Core.Logging;
using JetBrains.Annotations;

namespace Caliburn.Micro.Contrib.Controller.DynamicProxy
{
  public class LoggerAdapter : ILogger
  {
    /// <exception cref="ArgumentNullException"><paramref name="loggerName" /> is <see langword="null" /></exception>
    public LoggerAdapter([NotNull] string loggerName)
    {
      this.LoggerName = loggerName ?? throw new ArgumentNullException(nameof(loggerName));
      this.Logger = LogProvider.GetLogger(this.LoggerName);
    }

    [NotNull]
    private string LoggerName { get; }

    [NotNull]
    private Logging.ILog Logger { get; }

    /// <exception cref="ArgumentNullException"><paramref name="loggerName" /> is <see langword="null" /></exception>
    public virtual ILogger CreateChildLogger([NotNull] string loggerName)
    {
      // TODO
      if (loggerName == null)
      {
        throw new ArgumentNullException(nameof(loggerName));
      }

      loggerName = $"{this.LoggerName}.{loggerName}";

      var logger = new LoggerAdapter(loggerName);

      return logger;
    }

    public virtual void Debug(string message)
    {
      this.Logger.Debug(message);
    }

    public virtual void Debug(Func<string> messageFactory)
    {
      this.Logger.Debug(messageFactory);
    }

    public virtual void Debug(string message,
                              Exception exception)
    {
      this.Logger.DebugException(message,
                                 exception);
    }

    public virtual void DebugFormat(string format,
                                    params object[] args)
    {
      this.Logger.DebugFormat(format,
                              args);
    }

    public virtual void DebugFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      this.Logger.DebugException(format,
                                 exception,
                                 args);
    }

    public virtual void DebugFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      this.Logger.DebugFormat(format,
                              args);
    }

    public virtual void DebugFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      this.Logger.DebugException(format,
                                 exception,
                                 args);
    }

    public virtual void Error(string message)
    {
      this.Logger.Error(message);
    }

    public virtual void Error(Func<string> messageFactory)
    {
      this.Logger.Error(messageFactory);
    }

    public virtual void Error(string message,
                              Exception exception)
    {
      this.Logger.ErrorException(message,
                                 exception);
    }

    public virtual void ErrorFormat(string format,
                                    params object[] args)
    {
      this.Logger.ErrorFormat(format,
                              args);
    }

    public virtual void ErrorFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      this.Logger.ErrorException(format,
                                 exception,
                                 args);
    }

    public virtual void ErrorFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      this.Logger.ErrorFormat(format,
                              args);
    }

    public virtual void ErrorFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      this.Logger.ErrorException(format,
                                 exception,
                                 args);
    }

    public virtual void Fatal(string message)
    {
      this.Logger.Fatal(message);
    }

    public virtual void Fatal(Func<string> messageFactory)
    {
      this.Logger.Fatal(messageFactory);
    }

    public virtual void Fatal(string message,
                              Exception exception)
    {
      this.Logger.FatalException(message,
                                 exception);
    }

    public virtual void FatalFormat(string format,
                                    params object[] args)
    {
      this.Logger.DebugFormat(format,
                              args);
    }

    public virtual void FatalFormat(Exception exception,
                                    string format,
                                    params object[] args)
    {
      this.Logger.FatalFormat(format,
                              args);
    }

    public virtual void FatalFormat(IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      this.Logger.FatalFormat(format,
                              args);
    }

    public virtual void FatalFormat(Exception exception,
                                    IFormatProvider formatProvider,
                                    string format,
                                    params object[] args)
    {
      this.Logger.FatalException(format,
                                 exception,
                                 args);
    }

    public virtual void Info(string message)
    {
      this.Logger.Info(message);
    }

    public virtual void Info(Func<string> messageFactory)
    {
      this.Logger.Info(messageFactory);
    }

    public virtual void Info(string message,
                             Exception exception)
    {
      this.Logger.InfoException(message,
                                exception);
    }

    public virtual void InfoFormat(string format,
                                   params object[] args)
    {
      this.Logger.InfoFormat(format,
                             args);
    }

    public virtual void InfoFormat(Exception exception,
                                   string format,
                                   params object[] args)
    {
      this.Logger.InfoException(format,
                                exception,
                                args);
    }

    public virtual void InfoFormat(IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      this.Logger.InfoFormat(format,
                             args);
    }

    public virtual void InfoFormat(Exception exception,
                                   IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      this.Logger.InfoException(format,
                                exception,
                                args);
    }

    public virtual void Warn(string message)
    {
      this.Logger.Warn(message);
    }

    public virtual void Warn(Func<string> messageFactory)
    {
      this.Logger.Warn(messageFactory);
    }

    public virtual void Warn(string message,
                             Exception exception)
    {
      this.Logger.WarnException(message,
                                exception);
    }

    public virtual void WarnFormat(string format,
                                   params object[] args)
    {
      this.Logger.WarnFormat(format,
                             args);
    }

    public virtual void WarnFormat(Exception exception,
                                   string format,
                                   params object[] args)
    {
      this.Logger.WarnException(format,
                                exception,
                                args);
    }

    public virtual void WarnFormat(IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      this.Logger.WarnFormat(format,
                             args);
    }

    public virtual void WarnFormat(Exception exception,
                                   IFormatProvider formatProvider,
                                   string format,
                                   params object[] args)
    {
      this.Logger.WarnException(format,
                                exception,
                                args);
    }

    public virtual bool IsDebugEnabled => this.Logger.IsDebugEnabled();
    public virtual bool IsErrorEnabled => this.Logger.IsErrorEnabled();
    public virtual bool IsFatalEnabled => this.Logger.IsFatalEnabled();
    public virtual bool IsInfoEnabled => this.Logger.IsInfoEnabled();
    public virtual bool IsWarnEnabled => this.Logger.IsWarnEnabled();
  }
}
