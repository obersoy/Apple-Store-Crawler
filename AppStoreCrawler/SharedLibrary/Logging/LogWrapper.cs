using BDC.BDCCommons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Logging
{
    public class LogWrapper
    {   
        private static readonly object _threadLock = new object ();

        public LogWrapper ()
        {
            LogWriter.ConfigureLogWriter (TLogEventLevel.Information);
        }

        public void LogMessage (string message, string tag = "", TLogEventLevel eventLevel = TLogEventLevel.Information)
        {
            lock (_threadLock)
            {
                // Logging Message + Tag into Console
                Console.WriteLine(String.Join("\t", tag, message));

                switch (eventLevel)
                {
                    case TLogEventLevel.Information:
                        LogWriter.Info(tag, message);
                        break;

                    case TLogEventLevel.Error:
                        LogWriter.Error(tag, message);
                        break;

                    case TLogEventLevel.Warning:
                        LogWriter.Warn(tag, message);
                        break;

                    case TLogEventLevel.Critical:
                        LogWriter.Fatal(tag, message);
                        break;

                    case TLogEventLevel.Verbose:
                        LogWriter.Trace(tag, message);
                        break;
                }
            }
        }

        public void LogMessage (Exception ex)
        {
            LogMessage (ex.StackTrace, ex.Message, TLogEventLevel.Error);
        }
    }
}
