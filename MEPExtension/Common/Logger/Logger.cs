using Autodesk.Revit.UI;
using Common.Constant;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitMEPAddin.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class Logger
    {
        private LOGLEVEL loglevelMin;
        private LOGLEVEL loglevelMax;
        private string logDirectoryPath;
        private string logFilePath;
        // ログレベル
        enum LOGLEVEL : int
        {
            NONE = 5,
            ERROR = 1,
            WARN = 2,
            INFO = 3,
            TRACE = 4
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Logger(int logLevelMax, int logLevelMin, string logFolderPath)
        {
            loglevelMax = (LOGLEVEL)Enum.ToObject(typeof(LOGLEVEL), logLevelMax);
            loglevelMin = (LOGLEVEL)Enum.ToObject(typeof(LOGLEVEL), logLevelMin);
            logDirectoryPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + logFolderPath;
            logFilePath = logDirectoryPath + DateTime.Today.ToString("yyyyMMdd") + ".log";
            // フォルダがなければ作成する。
            if (!(Directory.Exists(logDirectoryPath)))
            {
                Directory.CreateDirectory(logDirectoryPath);
            }
        }

        /// <summary>
        /// <Traceレベル>ログ出力        /// </summary>
        /// <param name="message"></param>
        public void Trace(string message)
        {
            if ((int)loglevelMin < (int)LOGLEVEL.TRACE || (int)loglevelMax > (int)LOGLEVEL.TRACE) return;
            Write(CommonDefine.STR_LOG_TRACE, message);
        }

        /// <summary>
        /// <Infoレベル>ログ出力        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            if ((int)loglevelMin < (int)LOGLEVEL.INFO || (int)loglevelMax > (int)LOGLEVEL.INFO) return;
            Write(CommonDefine.STR_LOG_INFO, message);
        }

        /// <summary>
        /// <Warnレベル>ログ出力
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message)
        {
            if ((int)loglevelMin < (int)LOGLEVEL.WARN || (int)loglevelMax > (int)LOGLEVEL.WARN) return;
            Write(CommonDefine.STR_LOG_WARN, message);
        }

        /// <summary>
        /// <Errorレベル>ログ出力
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            if ((int)loglevelMin < (int)LOGLEVEL.ERROR || (int)loglevelMax > (int)LOGLEVEL.ERROR) return;
            Write(CommonDefine.STR_LOG_ERROR, message);
        }

        /// <summary>
        /// <共通>ログ出力
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool Write(string level, string message)
        {
            try
            {
                const int callerFrameIndex = 2; // 2つ前の呼び出し元メソッド名.
                System.Diagnostics.StackFrame callerFrame = new System.Diagnostics.StackFrame(callerFrameIndex);
                System.Reflection.MethodBase callerMethod = callerFrame.GetMethod();

                File.AppendAllText(logFilePath, DateTime.Now + @"  " + level + @"  ");
                File.AppendAllText(logFilePath, @"[" + callerMethod.Name + @"]  ");
                File.AppendAllText(logFilePath, message);
                File.AppendAllText(logFilePath, Environment.NewLine);
                return true;
            }
            catch (Exception)
            {
                // ログが出ないのは致命的なエラーではないので、
                // やり過ごさせる。
                return false;
            }
        }

    }
}
