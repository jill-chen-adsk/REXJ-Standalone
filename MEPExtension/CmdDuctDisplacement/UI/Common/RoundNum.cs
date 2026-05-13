using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.Resource;
using CmdDuctDisplacement.UI.Model;
using RevitMEPAddin.Common;
using System;

namespace CmdDuctDisplacement.UI.Common
{
    /// <summary>
    /// 画面部の丸めに関するクラス
    /// </summary>
    class RoundNum
    {
        // メンバ変数
        #region Memeber Variables
        private Logger log;
        #endregion

        // コンストラクタ
        #region Constructor
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RoundNum()
        {
            int min, max;
            if (!int.TryParse(ExResources.ResxString(DuctDisplacementDefine.LOG_LEVEL_MAX), out max))
            {
                max = DuctDisplacementDefine.LOG_LEVEL_MAX_DEF;
            }
            if (!int.TryParse(ExResources.ResxString(DuctDisplacementDefine.LOG_LEVEL_MIN), out min))
            {
                min = DuctDisplacementDefine.LOG_LEVEL_MIN_DEF;
            }
            this.log = new Logger(max, min, DuctDisplacementDefine.LOG_FOLDER_PATH_DEF);
        }
        #endregion

        #region Member Functions
        /// <summary>
        /// numを丸めて絶対値で50 or 100単位で切り上げるもしくは、丸めずそのまま出力する
        /// </summary>
        /// <param name="num">丸める値</param>
        /// <param name="en">丸め精度</param>
        /// <returns>enを参照しnumを丸めた値</returns>
        public double GetRoundUpAbsValue(double num, DuctDisplacementDefine.Rounder en)
        {
            double dividenum;
            double roundnum;

            if (en == DuctDisplacementDefine.Rounder.Multiple_50)
            {
                dividenum = num / DuctDisplacementDefine.num_50;
                if (num < 0)
                {
                    roundnum = Math.Floor(dividenum);
                }
                else
                {
                    roundnum = Math.Ceiling(dividenum);
                }

                var rc = (int)roundnum * DuctDisplacementDefine.num_50;
                return rc;

            }
            else if (en == DuctDisplacementDefine.Rounder.Multiple_100)
            {
                dividenum = num / DuctDisplacementDefine.num_100;
                if (num < 0)
                {
                    roundnum = Math.Floor(dividenum);
                }
                else
                {
                    roundnum = Math.Ceiling(dividenum);
                }

                var rc = (int)roundnum * DuctDisplacementDefine.num_100;
                return rc;
            }

            else if (en == DuctDisplacementDefine.Rounder.Multiple_None)
            {
                if (num < 0)
                {
                    return Math.Floor(num);
                }
                else
                {
                    return Math.Ceiling(num);
                }

            }

            else
            {
                //errow
                log.Error("DuctDisplacementDefine.Rounder:" + en);
                dividenum = num / DuctDisplacementDefine.num_50;
                if (num < 0)
                {
                    roundnum = Math.Floor(dividenum);
                }
                else
                {
                    roundnum = Math.Ceiling(dividenum);
                }

                var rc = (int)roundnum * DuctDisplacementDefine.num_50;
                return rc;
            }
        }

        /// <summary>
        /// numを丸めて50 or 100単位で切り上げるもしくは、丸めずそのまま出力する
        /// </summary>
        /// <param name="num">丸める値</param>
        /// <param name="en">丸め精度</param>
        /// <returns>enを参照しnumを丸めた値</returns>
        public double GetRoundUpValue(double num, DuctDisplacementDefine.Rounder en)
        {
            double dividenum;
            double roundnum;

            if (en == DuctDisplacementDefine.Rounder.Multiple_50)
            {
                dividenum = num / DuctDisplacementDefine.num_50;
                roundnum = Math.Floor(dividenum);

                var rc = (int)roundnum * DuctDisplacementDefine.num_50;
                return rc;

            }
            else if (en == DuctDisplacementDefine.Rounder.Multiple_100)
            {
                dividenum = num / DuctDisplacementDefine.num_100;
                roundnum = Math.Floor(dividenum);

                var rc = (int)roundnum * DuctDisplacementDefine.num_100;
                return rc;
            }

            else if (en == DuctDisplacementDefine.Rounder.Multiple_None)
            {
                return num;
            }

            else
            {
                //errow
                log.Error("DuctDisplacementDefine.Rounder:" + en);
                dividenum = num / DuctDisplacementDefine.num_50;
                roundnum = Math.Floor(dividenum);

                var rc = (int)roundnum * DuctDisplacementDefine.num_50;
                return rc;
            }
        }


        /// <summary>
        /// numを丸めて50 or 100単位で切り捨てるもしくは、丸めずそのまま出力する
        /// </summary>
        /// <param name="num">丸める値</param>
        /// <param name="en">丸め精度</param>
        /// <returns>enを参照しnumを丸めた値</returns>
        public double GetRoundDownValue(double num, DuctDisplacementDefine.Rounder en)
        {
            double dividenum;
            double roundnum;

            if (en == DuctDisplacementDefine.Rounder.Multiple_50)
            {
                dividenum = num / DuctDisplacementDefine.num_50;
                roundnum = Math.Ceiling(dividenum);

                var rc = (int)roundnum * DuctDisplacementDefine.num_50;
                return rc;

            }
            else if (en == DuctDisplacementDefine.Rounder.Multiple_100)
            {
                dividenum = num / DuctDisplacementDefine.num_100;
                roundnum = Math.Ceiling(dividenum);

                var rc = (int)roundnum * DuctDisplacementDefine.num_100;
                return rc;
            }

            else if (en == DuctDisplacementDefine.Rounder.Multiple_None)
            {
                return num;
            }

            else
            {
                //errow
                log.Error("DuctDisplacementDefine.Rounder:" + en);
                dividenum = num / DuctDisplacementDefine.num_50;
                roundnum = Math.Ceiling(dividenum);

                var rc = (int)roundnum * DuctDisplacementDefine.num_50;
                return rc;
            }
        }

        /// <summary>
        /// 指定された小数点未満を四捨五入する
        /// </summary>
        /// <param name="ApointDecimal">有効少数点桁(0.1や0.001など)</param>
        /// <param name="num">実数値</param>
        /// <returns>numを四捨五入した値</returns>
        public double ApointDecimalRound(double ApointDecimal, double num)
        {

            if (double.IsNaN(num))
            {
                return 0;
            }

            double i;
            decimal dnum = (decimal)num;

            for (i = ApointDecimal; i < 1; i *= 10)
            {
                dnum *= 10;
            }

            decimal roundnum = Math.Round(dnum);

            for (; i > ApointDecimal; i /= 10)
            {
                roundnum /= 10;
            }

            return (double)roundnum;

        }

        /// <summary>
        /// 少数点精度に合わせた丸め(フェールセーフ)
        ///  注意:オーバーロードあり
        /// </summary>
        /// <param name="num">丸める値</param>
        /// <returns>Revitの小数点精度に合わせたnumの値</returns>
        public double RoundUnnecessaryNum(double num)
        {
            var controlstatus = ControlStatus.Instance;
            int j = 1;
            decimal decrc;

            //整数部と小数部を切り分ける
            int integer = (int)num;
            decimal dec = (decimal)num - (decimal)integer;

            //小数部の計算                
            decimal cal = dec;

            //浮動小数点精度を取得
            for (var i = controlstatus.RevitProjectDecimalAccuracy; i < 1; i *= 10)
            {
                cal *= 10;
                j *= 10;
            }

            //有効桁数以下のみ取得
            cal = (cal % 1M) / j;

            if ((cal != 0) && (controlstatus.RevitProjectDecimalAccuracy > (double)cal))
            {

                //設定された小数点以下を四捨五入
                decrc = (decimal)ApointDecimalRound(controlstatus.RevitProjectDecimalAccuracy, (double)dec);

            }
            else
            {
                decrc = dec;
            }

            //FLテキストボックスに値を整列した値を反映させる
            return integer + (double)decrc;

        }
        #endregion
    }
}

