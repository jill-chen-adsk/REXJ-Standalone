using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitMEPAddin.Common
{
    public class WrpDocument
    {
        #region member

        private UIDocument uidoc;
        private Document doc;
        private Logger log;

        #endregion member

        #region constractor

        public WrpDocument(UIDocument uidoc, Logger log)
        {
            this.uidoc = uidoc;
            doc = this.uidoc.Document;
            this.log = log;
        }

        #endregion constractor

        #region methods

        /// <summary>
        /// ドキュメント内の長さ設定値にセットされている浮動小数点の精度を取得する
        /// </summary>
        /// <returns></returns>
        public double GetAccuracyDecimalLength()
        {
            double rc = 1;
            Units unit = uidoc.Document.GetUnits();
            FormatOptions foumat = unit.GetFormatOptions(SpecTypeId.Length);
            var caldate = foumat.Accuracy;
            try
            {
                //打ち切り誤差判定
                if (foumat.Accuracy == (foumat.Accuracy / 10) * 10)
                {
                    //整数であるか判定
                    if (foumat.Accuracy == (int)foumat.Accuracy)
                    {
                        rc = 0.1;
                    }
                    else
                    {
                        rc = foumat.Accuracy;
                    }
                }
                else
                {
                    //無限ループを防ぐため意味のない数字100に設定
                    for (int i = 0; i < 100; i++)
                    {
                        //誤差修正
                        caldate *= 10;
                        rc /= 10;
                        if (caldate > 0)
                        {
                            break;
                        }
                    }
                }
            }
            catch
            {
                //error
                log.Error("Check Revit Accuracy");
                rc = 0.1;
            }

            return rc;
        }

        #endregion methods
    }
}