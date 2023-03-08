using System;
using System.Runtime.CompilerServices;

namespace StartEx.Core.Interfaces;

public interface ILoggerService<out T>
{
    void Log(LogLevel level, object message, string? tag);
    void Debug(object message, [CallerMemberName] string? tag = null);
    void Info(object message, [CallerMemberName] string? tag = null);
    void Warn(object message, [CallerMemberName] string? tag = null);
    void Error(object message, [CallerMemberName] string? tag = null);

    /// <summary>
    /// 致命错误或者未处理的异常，指示应用程序应当立即退出
    /// </summary>
    /// <param name="message"></param>
    /// <param name="tag"></param>
    void Fatal(object message, [CallerMemberName] string? tag = null);

    void Exception(Exception e, [CallerMemberName] string? tag = null);
}

public enum LogLevel {
    Debug,
    Info,
    Warn,
    Error,
    Fatal
}
