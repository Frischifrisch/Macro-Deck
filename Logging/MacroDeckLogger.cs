﻿using SuchByte.MacroDeck.GUI.Dialogs;
using SuchByte.MacroDeck.Plugins;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuchByte.MacroDeck.Logging
{
    public static class MacroDeckLogger
    {
        public static event EventHandler OnWarningOrError;

        private static LogLevel _logLevel = LogLevel.Info;

        private static DebugConsole _debugConsole;

        private static int _errors = 0;

        private static int _warnings = 0;

        public static int Errors
        {
            get
            {
                return _errors;
            }
        }

        public static int Warnings
        {
            get
            {
                return _warnings;
            }
        }


        public static void StartDebugConsole()
        {
            if (_debugConsole != null && !_debugConsole.IsDisposed)
            {
                _debugConsole.Dispose();
                _debugConsole.Close();
            }

            _debugConsole = new DebugConsole();
            _debugConsole.Show();
        }

        /// <summary>
        /// Level of what should be logged.
        /// </summary>
        internal static LogLevel LogLevel
        {
            get
            {
                return _logLevel;
            }
            set
            {
                _logLevel = value;
                Info("Set log level to " + _logLevel.ToString());
            }
        }

        internal static bool FileLogging = true;

        /// <summary>
        /// Debug trace messages for internal debugging
        /// </summary>
        /// <param name="macroDeckPlugin"></param>
        /// <param name="classType"></param>
        /// <param name="message"></param>
        public static void Trace(MacroDeckPlugin macroDeckPlugin, Type classType, string message)
        {
            Trace(macroDeckPlugin, string.Format("{0}: {1}", classType.Name.ToString(), message));
        }

        /// <summary>
        /// Log informations that could be useful
        /// </summary>
        /// <param name="macroDeckPlugin"></param>
        /// <param name="classType"></param>
        /// <param name="message"></param>
        public static void Info(MacroDeckPlugin macroDeckPlugin, Type classType, string message)
        {
            Info(macroDeckPlugin, string.Format("{0}: {1}", classType.Name.ToString(), message));
        }

        /// <summary>
        /// Log warnings if something gone wrong
        /// </summary>
        /// <param name="macroDeckPlugin"></param>
        /// <param name="classType"></param>
        /// <param name="message"></param>
        public static void Warning(MacroDeckPlugin macroDeckPlugin, Type classType, string message)
        {
            Warning(macroDeckPlugin, string.Format("{0}: {1}", classType.Name.ToString(), message));
        }

        /// <summary>
        /// Log errors if something gone really wrong
        /// </summary>
        /// <param name="macroDeckPlugin"></param>
        /// <param name="classType"></param>
        /// <param name="message"></param>
        public static void Error(MacroDeckPlugin macroDeckPlugin, Type classType, string message)
        {
            Error(macroDeckPlugin, string.Format("{0}: {1}", classType.Name.ToString(), message));
        }


        /// <summary>
        /// Debug trace messages for internal debugging
        /// </summary>
        /// <param name="macroDeckPlugin"></param>
        /// <param name="message"></param>
        public static void Trace(MacroDeckPlugin macroDeckPlugin, string message)
        {
            if (macroDeckPlugin == null)
            {
                Log("Macro Deck", LogLevel.Error, "Plugin logging failed: plugin instance was null");
                return;
            }
            Log(macroDeckPlugin.Name, LogLevel.Trace, message);
        }

        /// <summary>
        /// Log informations that could be useful
        /// </summary>
        /// <param name="macroDeckPlugin"></param>
        /// <param name="message"></param>
        public static void Info(MacroDeckPlugin macroDeckPlugin, string message)
        {
            if (macroDeckPlugin == null)
            {
                Log("Macro Deck", LogLevel.Error, "Plugin logging failed: plugin instance was null");
                return;
            }
            Log(macroDeckPlugin.Name, LogLevel.Info, message);
        }

        /// <summary>
        /// Log warnings if something gone wrong
        /// </summary>
        /// <param name="macroDeckPlugin"></param>
        /// <param name="message"></param>
        public static void Warning(MacroDeckPlugin macroDeckPlugin, string message)
        {
            if (macroDeckPlugin == null)
            {
                Log("Macro Deck", LogLevel.Error, "Plugin logging failed: plugin instance was null");
                return;
            }
            Log(macroDeckPlugin.Name, LogLevel.Warning, message);
        }

        /// <summary>
        /// Log errors if something gone really wrong
        /// </summary>
        /// <param name="macroDeckPlugin"></param>
        /// <param name="message"></param>
        public static void Error(MacroDeckPlugin macroDeckPlugin, string message)
        {
            if (macroDeckPlugin == null)
            {
                Log("Macro Deck", LogLevel.Error, "Plugin logging failed: plugin instance was null");
                return;
            }
            Log(macroDeckPlugin.Name, LogLevel.Error, message);
        }

        internal static void Trace(Type classType, string message)
        {
            Trace(string.Format("{0}: {1}", classType.Name.ToString(), message));
        }

        internal static void Info(Type classType, string message)
        {
            Info(string.Format("{0}: {1}", classType.Name.ToString(), message));
        }

        internal static void Warning(Type classType, string message)
        {
            Warning(string.Format("{0}: {1}", classType.Name.ToString(), message));
        }

        internal static void Error(Type classType, string message)
        {
            Error(string.Format("{0}: {1}", classType.Name.ToString(), message));
        }


        internal static void Trace(string message)
        {
            Log("Macro Deck", LogLevel.Trace, message);
        }
        internal static void Info(string message)
        {
            Log("Macro Deck", LogLevel.Info, message);
        }
        internal static void Warning(string message)
        {
            Log("Macro Deck", LogLevel.Warning, message);
        }

        internal static void Error(string message)
        {
            Log("Macro Deck", LogLevel.Error, message);
        }


        private static void Log(string sender, LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Warning:
                    _warnings++;
                    if (OnWarningOrError != null)
                    {
                        OnWarningOrError(message, EventArgs.Empty);
                    }
                    break;
                case LogLevel.Error:
                    _errors++;
                    if (OnWarningOrError != null)
                    {
                        OnWarningOrError(message, EventArgs.Empty);
                    }
                    break;
            }
            if ((!Debugger.IsAttached && !FileLogging) || logLevel < LogLevel) return;

            var formattedLog = string.Format("{0} [{1}] [{2}] >> {3}", DateTime.Now.ToString("T"), sender, logLevel.ToString(), message);
            if (Debugger.IsAttached)
            {
                Debug.WriteLine(formattedLog);
            }
            if (_debugConsole != null && !_debugConsole.IsDisposed && _debugConsole.Visible)
            {
                Color logColor = Color.White;
                switch (logLevel)
                {
                    case LogLevel.Info:
                        logColor = Color.Aqua;
                        break;
                    case LogLevel.Warning:
                        logColor = Color.Orange;
                        break;
                    case LogLevel.Error:
                        logColor = Color.Red;
                        break;
                }
                _debugConsole.AppendText(formattedLog + Environment.NewLine, sender, logColor);
            }
            if (FileLogging && Directory.Exists(MacroDeck.LogsDirectoryPath) && CurrentFilename != null)
            {
                try
                {
                    Utils.Retry.Do(new Action(() =>
                    {
                        File.AppendAllText(CurrentFilename, Environment.NewLine + formattedLog, Encoding.UTF8);
                    }), TimeSpan.FromMilliseconds(10), 3);
                } catch (Exception ex)
                {
                    FileLogging = false;
                    Log(sender, LogLevel.Error, "File logging failed: " + ex.Message);
                    FileLogging = true;
                }
            }
        }

        internal static string CurrentFilename
        {
            get
            {
                return Path.Combine(MacroDeck.LogsDirectoryPath, DateTime.Now.ToString("yyyy-MM-dd") + ".log");
            }
        }
    }

    public enum LogLevel
    {
        /// <summary>
        /// Log Trace, Info, Warnings and Errors
        /// </summary>
        Trace = 1,
        /// <summary>
        /// Log Info, Warnings and Errors
        /// </summary>
        Info = 2,
        /// <summary>
        /// Log Warnings and Errors
        /// </summary>
        Warning = 3,
        /// <summary>
        /// Log only Errors
        /// </summary>
        Error = 4,
        /// <summary>
        /// Log nothing
        /// </summary>
        Nothing = 100,
    }
}
