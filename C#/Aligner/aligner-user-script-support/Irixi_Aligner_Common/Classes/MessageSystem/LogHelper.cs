using System;
using System.IO;

namespace Irixi_Aligner_Common.Message
{
    public static class LogHelper
    {
        public enum LogType
        {
            NORMAL,
            ERROR
        }

        /// <summary>
        /// lock the log file while writing since this is multi-thread application
        /// </summary>
        static readonly object locker = new object();

        /// <summary>
        /// whether write the log or not
        /// </summary>
        public static bool LogEnabled = false;

        public static void WriteLine(string Message, LogType Type = LogType.NORMAL)
        {
            lock (locker)
            {
                if (LogEnabled)
                {
                    StreamWriter w = File.AppendText(GetLogFileName());
                    w.WriteLine(FormatMessage(Message, Type));
                    w.Close();
                }
            }
        }

        public static void WriteLine(string Message, object arg0, LogType Type = LogType.NORMAL)
        {
            WriteLine(string.Format(Message, arg0), Type);
        }

        public static void WriteLine(string Message, object arg0, object arg1, LogType Type = LogType.NORMAL)
        {
            WriteLine(string.Format(Message, arg0, arg1), Type);
        }

        public static void WriteLine(string Message, object arg0, object arg1, object arg2, LogType Type = LogType.NORMAL)
        {
            WriteLine(string.Format(Message, arg0, arg1, arg2), Type);
        }

        public static void WriteLine(string Message, object[] arg, LogType Type = LogType.NORMAL)
        {
            WriteLine(string.Format(Message, arg), Type);
        }

        static string FormatMessage(string Message, LogType Type = LogType.NORMAL)
        {
            string retval = "";

            switch(Type)
            {
                case LogType.NORMAL:
                    retval = string.Format("{0:HH:mm:ss.ff}\t{1}", DateTime.Now, Message);
                    break;

                case LogType.ERROR:
                    retval = string.Format("{0:HH:mm:ss.ff}\t**ERROR** {1}", DateTime.Now, Message);
                    break;
            }

            return retval;
        }

        static string GetLogFileName()
        {
            Directory.CreateDirectory("log");
            return string.Format("log\\irixi_aligner_log_{0:yyyy_MM_dd}.txt", DateTime.Now);
        }
    }
}
