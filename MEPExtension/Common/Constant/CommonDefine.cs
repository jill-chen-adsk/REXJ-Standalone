using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Constant
{
    public class CommonDefine
    {
        /************************/
        /* リソースファイル関連 */
        /************************/
        #region ログファイル関連
        public static string RESKEY_LOG_FILE_PATH = "LOG_FILE_PATH";
        public static string RESKEY_LOG_LEVEL = "LOG_LEVEL";
        #endregion
        /************************/
        /* TaskDialog           */
        /************************/
        public static string DIALOG_TITLE_WARN = "Warning";
        public static string DIALOG_TITLE_CONFIRM = "Confirmation";
        public static string DIALOG_MSG_CONFIRM1 = "The family is already loaded. Do you want to overwrite it?";

        /************************/
        /* その他               */
        /************************/
        #region ログ関連
        public static string STR_LOG_ERROR = "ERROR";
        public static string STR_LOG_WARN = "WARN";
        public static string STR_LOG_INFO = "INFO";
        public static string STR_LOG_TRACE = "TRACE";
        #endregion
        #region その他
        // 許容誤差
        public static double TOLERANCE = 0.0000001;



        #endregion
    }
}
