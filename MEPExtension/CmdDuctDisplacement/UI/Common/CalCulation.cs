using CmdDuctDisplacement.UI.ViewModel;
using CmdDuctDisplacement.Constant;
using RevitMEPAddin.Common;
using CmdDuctDisplacement.Resource;

namespace CmdDuctDisplacement.UI.Common
{
    /// <summary>
    /// 画面部の計算関連クラス
    /// </summary>
    class CalCulation
    {
        private Logger log;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CalCulation()
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
        /// <summary>
        /// インクリメント or デクリメントボタン押下で値を加算する
        /// </summary>
        /// <param name="connectionsettingviewmodel">ConnectionSettingViewModelインスタンス</param>
        /// <param name="cal">演算方法</param>
        /// <returns>FMテキストボックスに演算した結果</returns>
        public double AddValueToOriginarValue(ConnectionSettingViewModel connectionsettingviewmodel, DuctDisplacementDefine.MethodOfCalculation cal)
        {
            double rc;
            RoundNum round = new RoundNum();
            DuctDisplacementDefine.Rounder roundnum;

            if (connectionsettingviewmodel.FiftyButton == true &&
                connectionsettingviewmodel.OneHandredButton == false &&
                connectionsettingviewmodel.NothingButton == false)
            {
                roundnum = DuctDisplacementDefine.Rounder.Multiple_50;
            }

            else if (connectionsettingviewmodel.FiftyButton == false &&
                    connectionsettingviewmodel.OneHandredButton == true &&
                    connectionsettingviewmodel.NothingButton == false)
            {
                roundnum = DuctDisplacementDefine.Rounder.Multiple_100;
            }

            else if (connectionsettingviewmodel.FiftyButton == false &&
                    connectionsettingviewmodel.OneHandredButton == false &&
                    connectionsettingviewmodel.NothingButton == true)
            {
                roundnum = DuctDisplacementDefine.Rounder.Multiple_None;
            }

            else
            {
                //errow
                log.Error("RoundButton");
                roundnum = DuctDisplacementDefine.Rounder.Multiple_50;
            }

            if (roundnum == DuctDisplacementDefine.Rounder.Multiple_50)
            {
                if (cal == DuctDisplacementDefine.MethodOfCalculation.Add)
                {
                    rc = connectionsettingviewmodel.InternalDuctOffsetLevel + DuctDisplacementDefine.num_50;
                }

                else if (cal == DuctDisplacementDefine.MethodOfCalculation.Sub)
                {
                    rc = connectionsettingviewmodel.InternalDuctOffsetLevel - (DuctDisplacementDefine.num_50);
                }

                else
                {
                    //errow
                    log.Error("RoundButton");
                    rc = connectionsettingviewmodel.InternalDuctOffsetLevel + DuctDisplacementDefine.num_50;
                }

            }

            else if (roundnum == DuctDisplacementDefine.Rounder.Multiple_100)
            {
                if (cal == DuctDisplacementDefine.MethodOfCalculation.Add)
                {
                    rc = connectionsettingviewmodel.InternalDuctOffsetLevel + DuctDisplacementDefine.num_100;
                }

                else if (cal == DuctDisplacementDefine.MethodOfCalculation.Sub)
                {
                    rc = connectionsettingviewmodel.InternalDuctOffsetLevel - (DuctDisplacementDefine.num_100);
                }

                else
                {
                    //errow
                    log.Error("RoundButton");
                    rc = connectionsettingviewmodel.InternalDuctOffsetLevel + DuctDisplacementDefine.num_100;
                }
            }

            else if (roundnum == DuctDisplacementDefine.Rounder.Multiple_None)
            {
                if (cal == DuctDisplacementDefine.MethodOfCalculation.Add)
                {
                    rc = (double)((decimal)connectionsettingviewmodel.InternalDuctOffsetLevel + (decimal)DuctDisplacementDefine.num_50);
                    //rc = connectionsettingviewmodel.InternalDuctOffsetLevel + DuctDisplacementDefine.num_50;
                }

                else if (cal == DuctDisplacementDefine.MethodOfCalculation.Sub)
                {
                    rc = (double)((decimal)connectionsettingviewmodel.InternalDuctOffsetLevel - (decimal)DuctDisplacementDefine.num_50);
                }

                else
                {
                    //errow
                    log.Error("RoundButton");
                    rc = (double)((decimal)connectionsettingviewmodel.InternalDuctOffsetLevel + (decimal)DuctDisplacementDefine.num_50);
                }
            }

            else
            {
                //errow
                log.Error("RoundButton");
                rc = connectionsettingviewmodel.InternalDuctOffsetLevel + DuctDisplacementDefine.num_50;
            }
            return rc;
        }
    }
}