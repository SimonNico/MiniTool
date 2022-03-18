using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;

namespace MiniTool.Log
{
    /// <summary>
    /// 日志组件
    /// </summary>
    public class LogHelper
    {
        static readonly ConcurrentQueue<Tuple<string, string>> LogQueue = new ConcurrentQueue<Tuple<string, string>>();
        static DateTime Now=DateTime.Now;
        /// <summary>
        /// 自定义事件
        /// </summary>
        public static event Action<LogInfo> Event;

        private static AutoResetEvent Pause=new AutoResetEvent(false);

        static LogHelper()
        {
            Tuple<string, string> tempTuple;
            var writeTask = new Task((obj) =>
            {
                while (true)
                {
                    Pause.WaitOne(1000, true);
                    List<string[]> temp = new List<string[]>();
                    foreach (var logItem in LogQueue)
                    {
                        string logPath = logItem.Item1;
                        string logMergeContent = string.Concat(logItem.Item2, Environment.NewLine, "----------------------------------------------------------------------------------------------------------------------", Environment.NewLine);
                        string[] logArr = temp.FirstOrDefault(d => d[0].Equals(logPath));
                        if (logArr != null)
                        {
                            logArr[1] = string.Concat(logArr[1], logMergeContent);
                        }
                        else
                        {
                            logArr = new[]
                            {
                                logPath,
                                logMergeContent
                            };
                            temp.Add(logArr);
                        }

                        LogQueue.TryDequeue(out  tempTuple);
                    }

                    foreach (var item in temp)
                    {
                        WriteText(item[0], item[1]);
                    }
                }
            }, null, TaskCreationOptions.LongRunning);
            writeTask.Start();
        }
       private static void WriteText(string logPath, string logContent)
        {
            try
            {
                if (!File.Exists(logPath))
                {
                    File.CreateText(logPath).Close();
                }

                using( var sw = File.AppendText(logPath)){
                     sw.Write(logContent);
                }
               
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static string LogDirectory
        {
            get{
                return Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory).Any(s => s.Contains("Web.config")) ? AppDomain.CurrentDomain.BaseDirectory + @"Logs\" : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            } 
            set
            {
            }
        }

        /// <summary>
        /// 写入Info级别的日志
        /// </summary>
        /// <param name="info"></param>
        public static void Info(string info)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}", Now, Thread.CurrentThread.ManagedThreadId, "Info", info);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            var log = new LogInfo()
            {
                LogLevel = LogLevel.Info,
                Message = info,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入Info级别的日志
        /// </summary>
        /// <param name="source"></param>
        /// <param name="info"></param>
        public static void Info(string source, string info)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4}", Now, Thread.CurrentThread.ManagedThreadId, "Info", source,info);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(),msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Info,
                Message = info,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入Info级别的日志
        /// </summary>
        /// <param name="source"></param>
        /// <param name="info"></param>
        public static void Info(Type source, string info)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4}", Now, Thread.CurrentThread.ManagedThreadId, "Info", source.FullName, info);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Info,
                Message = info,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source.FullName
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入debug级别日志
        /// </summary>
        /// <param name="debug">异常对象</param>
        public static void Debug(string debug)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}", Now, Thread.CurrentThread.ManagedThreadId, "Debug", debug);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Debug,
                Message = debug,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入debug级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="debug">异常对象</param>
        public static void Debug(string source, string debug)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4}", Now, Thread.CurrentThread.ManagedThreadId, "Debug", source, debug);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Debug,
                Message = debug,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入debug级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="debug">异常对象</param>
        public static void Debug(Type source, string debug)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4}", Now, Thread.CurrentThread.ManagedThreadId, "Debug", source.FullName, debug);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Debug,
                Message = debug,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source.FullName
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="error">异常对象</param>
        public static void Error(Exception error)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4} {5} {6} ", Now, Thread.CurrentThread.ManagedThreadId, "Error", error.Source, error.Message,Environment.NewLine,error.StackTrace);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Error,
                Message = error.Message,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = error.Source,
                Exception = error,
                ExceptionType = error.GetType().Name
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常对象</param>
        public static void Error(Type source, Exception error)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4} {5} {6} ", Now, Thread.CurrentThread.ManagedThreadId, "Error", source.FullName, error.Message, Environment.NewLine, error.StackTrace);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Error,
                Message = error.Message,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source.FullName,
                Exception = error,
                ExceptionType = error.GetType().Name
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常信息</param>
        public static void Error(Type source, string error)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4}  ", Now, Thread.CurrentThread.ManagedThreadId, "Error", source.FullName, error);
           
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Error,
                Message = error,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source.FullName,
                //Exception = error,
                ExceptionType = error.GetType().Name
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常对象</param>
        public static void Error(string source, Exception error)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4} {5} {6} ", Now, Thread.CurrentThread.ManagedThreadId, "Error", source, error.Message, Environment.NewLine, error.StackTrace);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Error,
                Message = error.Message,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source,
                Exception = error,
                ExceptionType = error.GetType().Name
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入error级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="error">异常信息</param>
        public static void Error(string source, string error)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4}  ", Now, Thread.CurrentThread.ManagedThreadId, "Error", source, error);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Error,
                Message = error,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source,
                //Exception = error,
                ExceptionType = error.GetType().Name
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(Exception fatal)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4} {5} {6} ", Now, Thread.CurrentThread.ManagedThreadId, "Fatal", fatal.Source, fatal.Message, Environment.NewLine, fatal.StackTrace);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Fatal,
                Message = fatal.Message,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = fatal.Source,
                Exception = fatal,
                ExceptionType = fatal.GetType().Name
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(Type source, Exception fatal)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4} {5} {6} ", Now, Thread.CurrentThread.ManagedThreadId, "Fatal", source.FullName, fatal.Message, Environment.NewLine, fatal.StackTrace);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Fatal,
                Message = fatal.Message,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source.FullName,
                Exception = fatal,
                ExceptionType = fatal.GetType().Name
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(Type source, string fatal)
        {

            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4}", Now, Thread.CurrentThread.ManagedThreadId, "Fatal", source.FullName,fatal);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(),msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Fatal,
                Message = fatal,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source.FullName,
                ExceptionType = fatal.GetType().Name
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(string source, Exception fatal)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4} {5} {6} ", Now, Thread.CurrentThread.ManagedThreadId, "Fatal", source, fatal.Message, Environment.NewLine, fatal.StackTrace);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Fatal,
                Message = fatal.Message,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source,
                Exception = fatal,
                ExceptionType = fatal.GetType().Name
            };
            Event.Invoke(log);
        }

        /// <summary>
        /// 写入fatal级别日志
        /// </summary>
        /// <param name="source">异常源的类型</param>
        /// <param name="fatal">异常对象</param>
        public static void Fatal(string source, string fatal)
        {
            string msg = string.Format("{0}   [{1}]   {2}  {3}  {4} ", Now, Thread.CurrentThread.ManagedThreadId, "Fatal", source, fatal);
            LogQueue.Enqueue(new Tuple<string, string>(GetLogPath(), msg));
            LogInfo log = new LogInfo()
            {
                LogLevel = LogLevel.Fatal,
                Message = fatal,
                Time = Now,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Source = source,
                ExceptionType = fatal.GetType().Name
            };
            Event.Invoke(log);
        }

        private static string GetLogPath()
        {
            string newFilePath;
            var logDir = string.IsNullOrEmpty(LogDirectory) ? Path.Combine(Environment.CurrentDirectory, "logs") : LogDirectory;
            Directory.CreateDirectory(logDir);
            string extension = ".log";
            string fileNameNotExt = Now.ToString("yyyyMMdd");
            string fileNamePattern = string.Concat(fileNameNotExt, "(*)", extension);
            List<string> filePaths = Directory.GetFiles(logDir, fileNamePattern, SearchOption.TopDirectoryOnly).ToList();
            int tempno=0;

            if (filePaths.Count > 0)
            {
                int fileMaxLen = filePaths.Max(d => d.Length);
                string lastFilePath = filePaths.Where(d => d.Length == fileMaxLen).OrderByDescending(d => d).FirstOrDefault();
                if (new FileInfo(lastFilePath).Length > 1 * 1024 * 1024)
                {
                    var no = new Regex(@"(?is)(?<=\()(.*)(?=\))").Match(Path.GetFileName(lastFilePath)).Value;
                    var parse = int.TryParse(no, out tempno);
                    var formatno = parse ? (tempno + 1) : tempno;
                    var newFileName = String.Concat(fileNameNotExt, formatno, extension);
                    newFilePath = Path.Combine(logDir, newFileName);
                }
                else
                {
                    newFilePath = lastFilePath;
                }
            }
            else
            {
                var newFileName = string.Concat(fileNameNotExt, "(0)", extension);
                newFilePath = Path.Combine(logDir, newFileName);
            }

            return newFilePath;
        }

    }
}
