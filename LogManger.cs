using ChaseLabs.CLLogger.Events;
using ChaseLabs.CLLogger.Interfaces;
using System;
using System.Diagnostics;
using System.IO;
using static ChaseLabs.CLLogger.Lists;

namespace ChaseLabs.CLLogger
{
    /// <summary>
    /// <para>
    /// Author: Drew Chase
    /// </para>
    /// <para>
    /// Company: Chase Labs
    /// </para>
    /// </summary>
    public class LogManger : ILog
    {
        private readonly string _pattern_prefix;
        private static string _path = "";
        private static LogTypes minLogType = LogTypes.All;
        private readonly bool fatal = false, warn = false, info = false, debug = false, error = false, logDefaultConsole = false;

        /// <summary>
        /// Initializes an Empty LogManager
        /// <para>See <see cref="Empty"/></para>
        /// </summary>
        /// <returns></returns>
        public static LogManger Init()
        {
            return Empty();
        }

        /// <summary>
        /// Sets the Minimum Log Type
        /// </summary>
        /// <param name="minLogType">See <see cref="LogTypes"/> Documentation for Sorting Order</param>
        /// <returns></returns>
        public LogManger SetMinLogType(LogTypes minLogType)
        {
            return new LogManger(Path, minLogType, Pattern, logDefaultConsole);
        }
        /// <summary>
        /// Sets the Log File Path
        /// </summary>
        /// <param name="path">Log Path<para>Example</para><code>path="c:\temp\latest.log"</code></param>
        /// <returns></returns>
        public LogManger SetLogDirectory(string path)
        {
            return new LogManger(path, minLogType, Pattern, logDefaultConsole);
        }
        /// <summary>
        /// Sets the Log File Path
        /// </summary>
        /// <param name="path">Log Path<para>Example</para><code>path="c:\temp\latest.log"</code></param>
        /// <returns></returns>
        public LogManger SetLogDirectory(FileInfo path)
        {
            return new LogManger(path.FullName, minLogType, Pattern, logDefaultConsole);
        }
        /// <summary>
        /// Enables Logging for Default Console.
        /// </summary>
        /// <returns></returns>
        public LogManger EnableDefaultConsoleLogging()
        {
            return new LogManger(Path, minLogType, Pattern, true);
        }
        /// <summary>
        /// Disables Logging for Default Console.
        /// </summary>
        /// <returns></returns>
        public LogManger DisableDefaultConsoleLogging()
        {
            return new LogManger(Path, minLogType, Pattern, false);
        }
        /// <summary>
        /// <para>%DATE% - Current Date and Time</para>
        /// <para>%TYPE% - Log Type</para>
        /// <para>%MESSAGE% - The Message</para>
        /// Example
        /// <code>
        /// [ %TYPE%: %DATE% ]: %MESSAGE%
        /// </code>
        /// <para>[ ERROR: 01/01/1999/8:30:25 ] Example Message is Surprising</para>
        /// </summary>
        /// <returns></returns>
        public LogManger SetPattern(string Pattern)
        {
            return new LogManger(Path, minLogType, Pattern, logDefaultConsole);
        }

        /// <summary>
        /// <para>
        /// Default Path: <code>C:\Default-Log-File-Location(Please_Change)\latest.log</code>
        /// </para>
        /// <para>
        /// Default Minimum Log Type: All Logs
        /// </para>
        /// </summary>
        /// <returns>a Empty LogManager Object</returns>
        public static LogManger Empty()
        {
            return new LogManger();
        }

        private LogManger(string path = "", LogTypes _minLogType = LogTypes.All, string _pattern_prefix = "[ %TYPE%: %DATE% ]: %MESSAGE%", bool _logDefaultConsole = true)
        {
            _path = path;
            minLogType = _minLogType;
            this._pattern_prefix = _pattern_prefix;
            logDefaultConsole = _logDefaultConsole;

            switch (minLogType)
            {
                case LogTypes.All:
                    fatal = true; warn = true; info = true; debug = true; error = true;
                    break;
                case LogTypes.Debug:
                    fatal = true; warn = true; info = true; debug = true; error = true;
                    break;
                case LogTypes.Info:
                    fatal = true; warn = true; info = true; debug = false; error = true;
                    break;
                case LogTypes.Warn:
                    fatal = true; warn = true; info = false; debug = false; error = true;
                    break;
                case LogTypes.Error:
                    fatal = true; warn = false; info = false; debug = false; error = true;
                    break;
                case LogTypes.Fatal:
                    fatal = true; warn = false; info = false; debug = false; error = false;
                    break;
                default:
                    fatal = true; warn = true; info = true; debug = true; error = true;
                    minLogType = LogTypes.All;
                    break;
            }

            AppDomain.CurrentDomain.ProcessExit += Close;
        }

        private void Close(object sender, EventArgs e)
        {
            if (File.Exists(Path))
            {
                File.Move(Path, System.IO.Path.Combine(Directory.GetParent(Path).FullName, DateTime.Now.ToString().Replace(":", "-").Replace("/", "-") + ".log"));
            }
        }

        /// <summary>
        /// Returns if Fatal Messages will be Logged
        /// </summary>
        public bool IsFatalEnabled => fatal;
        /// <summary>
        /// Returns if Warning Messages will be Logged
        /// </summary>
        public bool IsWarnEnabled => warn;
        /// <summary>
        /// Returns if Informational Messages will be Logged
        /// </summary>
        public bool IsInfoEnabled => info;
        /// <summary>
        /// Returns if Debug Messages will be Logged
        /// </summary>
        public bool IsDebugEnabled => debug;
        /// <summary>
        /// Returns if Error Messages will be Logged
        /// </summary>
        public bool IsErrorEnabled => error;

        /// <summary>
        /// <para>%DATE% - Current Date and Time</para>
        /// <para>%TYPE% - Log Type</para>
        /// <para>%MESSAGE% - The Message</para>
        /// Example
        /// <code>
        /// [ %TYPE%: %DATE% ]: %MESSAGE%
        /// </code>
        /// <para>[ ERROR: 01/01/1999/8:30:25 ] Example Message is Surprising</para>
        /// </summary>
        public string Pattern => _pattern_prefix;

        /// <summary>
        /// Gets the Current Log File Path
        /// </summary>
        public string Path => _path;

        public delegate void LoggedMessageEventHandler(object sender, LogEventArgs args);
        public event LoggedMessageEventHandler LoggedMessage;

        /// <summary>
        /// Runs Every Time a Message is Logged
        /// </summary>
        public virtual void OnMessageLogged(string message)
        {
            LoggedMessage?.Invoke(this, new LogEventArgs() { Log = message });
        }

        private void SendMessage(object message)
        {
            if (!Directory.GetParent(_path).Exists)
            {
                Directory.CreateDirectory(Directory.GetParent(_path).FullName);
            }

            StreamWriter writer = new StreamWriter(_path, true);
            writer.WriteLine(message);
            writer.Flush();
            writer.Dispose();
            writer.Close();
            OnMessageLogged(message as string);
            Console.WriteLine(message);
        }

        public void Debug(object message)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "DEBUG").Replace("%MESSAGE%", message as string));
        }

        public void Debug(params object[] messages)
        {
            foreach (object message in messages)
            {
                SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "DEBUG").Replace("%MESSAGE%", message as string));
            }
        }

        public void Debug(object message, Exception exception)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "DEBUG").Replace("%MESSAGE%", message as string) + $" [Exception {exception.GetType().Name} at line {new StackTrace(exception, true).GetFrame(0).GetFileLineNumber()}]:{Environment.NewLine}{exception.StackTrace}");
        }

        public void Info(object message, Exception exception)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "INFO").Replace("%MESSAGE%", message as string) + $" [Exception {exception.GetType().Name} at line {new StackTrace(exception, true).GetFrame(0).GetFileLineNumber()}]:{Environment.NewLine}{exception.StackTrace}");

        }

        public void Info(object message)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "INFO").Replace("%MESSAGE%", message as string));
        }

        public void Info(params object[] messages)
        {
            foreach (object message in messages)
            {
                SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "INFO").Replace("%MESSAGE%", message as string));
            }
        }

        public void Warn(object message)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "WARN").Replace("%MESSAGE%", message as string));
        }

        public void Warn(params object[] messages)
        {
            foreach (object message in messages)
            {
                SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "WARN").Replace("%MESSAGE%", message as string));
            }
        }

        public void Warn(object message, Exception exception)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "WARN").Replace("%MESSAGE%", message as string) + $" [Exception {exception.GetType().Name} at line {new StackTrace(exception, true).GetFrame(0).GetFileLineNumber()}]:{Environment.NewLine}{exception.StackTrace}");

        }

        public void Error(object message)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "ERROR").Replace("%MESSAGE%", message as string));
        }

        public void Error(params object[] messages)
        {
            foreach (object message in messages)
            {
                SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "ERROR").Replace("%MESSAGE%", message as string));
            }
        }

        public void Error(object message, Exception exception)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "ERROR").Replace("%MESSAGE%", message as string) + $" [Exception {exception.GetType().Name} at line {new StackTrace(exception, true).GetFrame(0).GetFileLineNumber()}]:{Environment.NewLine}{exception.StackTrace}");
        }

        public void Fatal(object message)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "FATAL").Replace("%MESSAGE%", message as string));
        }

        public void Fatal(params object[] messages)
        {
            foreach (object message in messages)
            {
                SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "FATAL").Replace("%MESSAGE%", message as string));
            }
        }

        public void Fatal(object message, Exception exception)
        {
            SendMessage(Pattern.Replace("%DATE%", DateTime.Now.ToString()).Replace("%TYPE%", "DEBUG").Replace("%MESSAGE%", message as string) + $" [Exception {exception.GetType().Name} at line {new StackTrace(exception, true).GetFrame(0).GetFileLineNumber()}]:{Environment.NewLine}{exception.StackTrace}");
        }
    }
}
