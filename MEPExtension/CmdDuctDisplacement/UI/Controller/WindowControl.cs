using System.Windows;
using System.Windows.Media;
using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.UI.ViewModel;
using CmdDuctDisplacement.UI.Common;
using CmdDuctDisplacement.Logic;
using RevitMEPAddin.Common;
using Autodesk.Revit.DB;
using CmdDuctDisplacement.Resource;
using System.Collections.Generic;
using System.Text;

namespace CmdDuctDisplacement.UI.Controller
{
    /// <summary>
    /// 画面部制御関連メソッド
    /// </summary>
    internal class WindowControl
    {
        //メンバ変数

        #region Memeber Variables

        private Logger log;

        #endregion Memeber Variables

        //コンストラクタ

        #region Constructor

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowControl()
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

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// <summary>
        /// 基準位置から位置情報を求める
        /// </summary>
        /// <param name="mep">MEPOperationインスタンス</param>
        /// <param name="referenceline">基準ライン</param>
        /// <returns>基準位置変更分の幅</returns>
        public double ReferenceLineChange(MEPOperation mep, string referenceline)
        {
            double rv = 0;

            var halfHeight = (UnitUtils.Convert(mep.GetHeight(DuctDisplacementDefine.InstructionObj.MoveObj_1),
                              UnitTypeId.Feet, UnitTypeId.Millimeters)) / 2;

            var route = GetReferenceLineChangeRoute(referenceline);
            switch (route)
            {
                case (DuctDisplacementDefine.ReferenceLineChangeRoute.ToptoCenter):
                    rv = halfHeight;
                    break;

                case (DuctDisplacementDefine.ReferenceLineChangeRoute.ToptoBottom):
                    rv = halfHeight * 2;
                    break;

                case (DuctDisplacementDefine.ReferenceLineChangeRoute.CentertoTop):
                    rv = -(halfHeight);
                    break;

                case (DuctDisplacementDefine.ReferenceLineChangeRoute.CentertoButtom):
                    rv = halfHeight;
                    break;

                case (DuctDisplacementDefine.ReferenceLineChangeRoute.BottumtoTop):
                    rv = -(halfHeight) * 2;
                    break;

                case (DuctDisplacementDefine.ReferenceLineChangeRoute.BottomtoCenter):
                    rv = -(halfHeight);
                    break;

                case (DuctDisplacementDefine.ReferenceLineChangeRoute.Keep):
                default:
                    rv = 0;
                    break;
            }
            log.Trace("ReferenceLineChangeRoute:" + route);
            return rv;
        }

        /// <summary>
        /// 基準ラインを考慮した移動量を算出する
        /// </summary>
        /// <param name="mep">MEPOperationインスタンス</param>
        /// <param name="referenceline">基準ライン</param>
        /// <param name="flvalue">FLオフセット値</param>
        /// <param name="intitalfl">移動図形の初期FLオフセット値</param>
        /// <returns>基準ラインを考慮した移動量</returns>
        public double AmountOfMovementABSCal(MEPOperation mep, string referenceline, double flvalue, double intitalfl)
        {
            int refline;
            double rv = 0;
            RoundNum roundnum = new RoundNum();

            //幅の半分を取得
            var halfHeight = (UnitUtils.Convert(mep.GetHeight(DuctDisplacementDefine.InstructionObj.MoveObj_1),
                              UnitTypeId.Feet, UnitTypeId.Millimeters)) / 2;

            refline = RefalenceLineConvert(referenceline);

            switch (refline)
            {
                case (DuctDisplacementDefine.OFFSET_POS_BOTTOM):
                    rv = (double)((decimal)flvalue - ((decimal)intitalfl - (decimal)halfHeight));
                    break;

                case (DuctDisplacementDefine.OFFSET_POS_TOP):
                    rv = (double)((decimal)flvalue - ((decimal)intitalfl + (decimal)halfHeight));
                    break;

                case (DuctDisplacementDefine.OFFSET_POS_MIDDLE):
                default:
                    rv = (double)((decimal)flvalue - (decimal)intitalfl);
                    break;
            }
            return roundnum.RoundUnnecessaryNum(rv);
        }

        /// <summary>
        /// 幅を加算する
        /// </summary>
        /// <param name="referenceline">基準ライン</param>
        /// <param name="mep">MEPOperationインスタンス</param>
        /// <returns>基準ライン中央 - 基準ラインリストにセットされている基準ライン</returns>
        public double AddHeight(string referenceline, MEPOperation mep)
        {
            double rc;
            if (referenceline == ExResources.ResxString(DuctDisplacementDefine.LVL_Center))
            {
                rc = 0;
            }
            else if (referenceline == ExResources.ResxString(DuctDisplacementDefine.LVL_TopSide))
            {
                rc = (UnitUtils.Convert(mep.GetHeight(DuctDisplacementDefine.InstructionObj.MoveObj_1),
                              UnitTypeId.Feet, UnitTypeId.Millimeters)) / 2;
            }
            else if (referenceline == ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide))
            {
                rc = -(UnitUtils.Convert(mep.GetHeight(DuctDisplacementDefine.InstructionObj.MoveObj_1),
                              UnitTypeId.Feet, UnitTypeId.Millimeters)) / 2;
            }
            else
            {
                //erroe
                log.Error("check SwitchCalMethodCommand");
                rc = 0;
            }
            return rc;
        }

        /// <summary>
        /// ↑↓ボタンの色を入れ替える
        /// </summary>
        /// <param name="ConnectionSettingViewModel">ConnectionSettingViewModelインスタンス</param>
        /// <param name="buttonname">押下されたボタン(↑ or ↓)</param>
        public void UpDownButtonSwapColor(ConnectionSettingViewModel ConnectionSettingViewModel, DuctDisplacementDefine.PressButton buttonname)
        {
            var controlstatus = ControlStatus.Instance;

            if (buttonname == DuctDisplacementDefine.PressButton.Top)
            {
                ConnectionSettingViewModel.BottomArrangementColor = Brushes.LightGray.ToString();
                ConnectionSettingViewModel.TopArrangementColor = Brushes.LightSteelBlue.ToString();
            }
            else if (buttonname == DuctDisplacementDefine.PressButton.Bottom)
            {
                ConnectionSettingViewModel.BottomArrangementColor = Brushes.LightSteelBlue.ToString();
                ConnectionSettingViewModel.TopArrangementColor = Brushes.LightGray.ToString();
            }
            else
            {
                //error
                log.Error("Check buttonname Out");
                ConnectionSettingViewModel.BottomArrangementColor = Brushes.LightGray.ToString();
                ConnectionSettingViewModel.TopArrangementColor = Brushes.LightSteelBlue.ToString();
            }
        }

        /// <summary>
        /// 移動図形と回避対象図形のラベル色を入れ替える
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        /// <param name="buttonname">押下されたボタン(↑ or ↓)</param>
        public void UpDownObjLabelSwapColor(ConnectionSettingViewModel ConnectionSettingViewModel, DuctDisplacementDefine.PressButton buttonname)
        {
            if (buttonname == DuctDisplacementDefine.PressButton.Top)
            {
                ConnectionSettingViewModel.Location_BottomObjColor = "Blue";
                ConnectionSettingViewModel.Location_TopObjColor = "Red";
            }
            else if (buttonname == DuctDisplacementDefine.PressButton.Bottom)
            {
                ConnectionSettingViewModel.Location_BottomObjColor = "Red";
                ConnectionSettingViewModel.Location_TopObjColor = "Blue";
            }
            else
            {
                //error
                log.Error("Check buttonname Out");
                ConnectionSettingViewModel.Location_BottomObjColor = "Blue";
                ConnectionSettingViewModel.Location_TopObjColor = "Red";
            }
        }

        /// <summary>
        /// 丸め精度の設定値を取得する
        /// </summary>
        /// <param name="connectionsettingviewmodel">ConnectionSettingViewModelインスタンス</param>
        /// <returns>丸め精度</returns>
        public DuctDisplacementDefine.Rounder GetRoundAccuracyButton(ConnectionSettingViewModel connectionsettingviewmodel)
        {
            if (connectionsettingviewmodel.FiftyButton == true &&
                connectionsettingviewmodel.OneHandredButton == false &&
                connectionsettingviewmodel.NothingButton == false)
            {
                return DuctDisplacementDefine.Rounder.Multiple_50;
            }
            else if (connectionsettingviewmodel.FiftyButton == false &&
                    connectionsettingviewmodel.OneHandredButton == true &&
                    connectionsettingviewmodel.NothingButton == false)
            {
                return DuctDisplacementDefine.Rounder.Multiple_100;
            }
            else if (connectionsettingviewmodel.FiftyButton == false &&
                    connectionsettingviewmodel.OneHandredButton == false &&
                    connectionsettingviewmodel.NothingButton == true)
            {
                return DuctDisplacementDefine.Rounder.Multiple_None;
            }
            else
            {
                //error
                log.Error("Check Rounder Out");
                return DuctDisplacementDefine.Rounder.Multiple_50;
            }
        }

        /// <summary>
        /// 基準位置の切り替わりを取得する
        /// </summary>
        /// <param name="referenceline"></param>
        /// <returns>基準位置</returns>
        public DuctDisplacementDefine.ReferenceLineChangeRoute GetReferenceLineChangeRoute(string referenceline)
        {
            int refline;
            ControlStatus controlstatus = ControlStatus.Instance;
            Window window = new Window();
            DuctDisplacementDefine.ReferenceLineChangeRoute rc;

            refline = RefalenceLineConvert(referenceline);

            switch (refline)
            {
                case (DuctDisplacementDefine.OFFSET_POS_TOP):
                    if (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_TopSide))
                    {
                        log.Warn("Check referenceline Out");
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    }
                    else if (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_Center))
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.ToptoCenter;
                    }
                    else if (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide))
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.ToptoBottom;
                    }
                    else if (controlstatus.ReferenceLine_Befor == string.Empty)
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    }
                    else
                    {
                        //error
                        log.Error("Check referenceline Out");
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    }
                    break;

                case (DuctDisplacementDefine.OFFSET_POS_MIDDLE):
                    if (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_TopSide))
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.CentertoTop;
                    }
                    else if (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_Center))
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    }
                    else if (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide))
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.CentertoButtom;
                    }
                    else if (controlstatus.ReferenceLine_Befor == string.Empty)
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    }
                    else
                    {
                        //error
                        log.Error("Check referenceline Out");
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    }
                    break;

                case (DuctDisplacementDefine.OFFSET_POS_BOTTOM):
                    if (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_TopSide))
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.BottumtoTop;
                    }
                    else if (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_Center))
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.BottomtoCenter;
                    }
                    else if (controlstatus.ReferenceLine_Befor == ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide))
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    }
                    else if (controlstatus.ReferenceLine_Befor == string.Empty)
                    {
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    }
                    else
                    {
                        //error
                        log.Error("Check referenceline Out");
                        rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    }
                    break;

                default:
                    //error
                    log.Error("Check referenceline Out");
                    rc = DuctDisplacementDefine.ReferenceLineChangeRoute.Keep;
                    break;
            }
            return rc;
        }

        /// <summary>
        /// 移動図形から初期基準位置を判定する
        /// </summary>
        /// <param name="mep"></param>
        /// <param name="instructionobj"></param>
        /// <returns>初期基準位置</returns>
        public string GetInitRefalenceLine(MEPOperation mep, DuctDisplacementDefine.InstructionObj instructionobj)
        {
            try
            {
                var actions = new Dictionary<string, string>();
                actions.Add(ExResources.ResxString(DuctDisplacementDefine.LVL_EllipticalDuct), ExResources.ResxString(DuctDisplacementDefine.LVL_Center));
                actions.Add(ExResources.ResxString(DuctDisplacementDefine.LVL_RoundDuct), ExResources.ResxString(DuctDisplacementDefine.LVL_Center));
                actions.Add(ExResources.ResxString(DuctDisplacementDefine.LVL_PipeType), ExResources.ResxString(DuctDisplacementDefine.LVL_Center));
                actions.Add(ExResources.ResxString(DuctDisplacementDefine.LVL_SquareDuct), ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide));

                return actions[mep.GetFamilyName(instructionobj)];
            }
            catch
            {
                log.Error("Check mep.GetFamilyName(instructionobj)");
                return ExResources.ResxString(DuctDisplacementDefine.LVL_Center);
            }
        }

        /// <summary>
        /// 基準位置を内部ロジック用に切り替える
        /// </summary>
        /// <param name="refline"></param>
        /// <returns>基準位置</returns>
        public int RefalenceLineConvert(string refline)
        {
            try
            {
                var actions = new Dictionary<string, int>();
                actions.Add(ExResources.ResxString(DuctDisplacementDefine.LVL_TopSide), DuctDisplacementDefine.OFFSET_POS_TOP);
                actions.Add(ExResources.ResxString(DuctDisplacementDefine.LVL_Center), DuctDisplacementDefine.OFFSET_POS_MIDDLE);
                actions.Add(ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide), DuctDisplacementDefine.OFFSET_POS_BOTTOM);

                return actions[refline];
            }
            catch
            {
                log.Error("Check mep.GetFamilyName(instructionobj)");
                return DuctDisplacementDefine.OFFSET_POS_MIDDLE;
            }
        }

        /// <summary>
        /// 画面部の上下ボタンを内部ロジック用に切り替える
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>定数 上下ボタン</returns>
        public int GetMovePlaceConvert(ConnectionSettingViewModel instance)
        {
            int MovePlace;
            if (instance.BottomArrangementColor == "#FFB0C4DE")
            {
                MovePlace = DuctDisplacementDefine.DIR_DOWN;
            }
            else if (instance.BottomArrangementColor == "#FFD3D3D3")
            {
                MovePlace = DuctDisplacementDefine.DIR_UPPER;
            }
            else
            {
                //error
                log.Error("Check BottomArrangementColor:" + instance.BottomArrangementColor);
                MovePlace = DuctDisplacementDefine.DIR_UPPER;
            }
            return MovePlace;
        }

        /// <summary>
        /// 移動図形のFLオフセット値の初期値を基準ライン中央で取得する
        /// </summary>
        /// <param name="instance"></param>
        /// <returns>移動図形基準ライン中央</returns>
        public double GetInitSelectOBJRefCenterValue(MEPOperation mep)
        {
            RoundNum roundnum = new RoundNum();

            return
            (double)((decimal)roundnum.RoundUnnecessaryNum
                (UnitUtils.Convert((double)((decimal)(mep.GetLocationCurveGLLevel_mm(DuctDisplacementDefine.InstructionObj.MoveObj_1)) -
                (decimal)(mep.GetActiveViewFlLevel())),
                UnitTypeId.Feet, UnitTypeId.Millimeters)));
        }

        /// <summary>
        /// 丸め精度を内部ロジック用に切り替える
        /// </summary>
        /// <param name="ductp"></param>
        /// <returns>定数 丸め精度</returns>
        public int RoundType(ConnectionSettingViewModel ductp)
        {
            int rc;

            if ((ductp.FiftyButton == true) &&
                (ductp.OneHandredButton == false) &&
                (ductp.NothingButton == false))
            {
                rc = DuctDisplacementDefine.num_50;
            }
            else if ((ductp.FiftyButton == false) &&
                    (ductp.OneHandredButton == true) &&
                    (ductp.NothingButton == false))
            {
                rc = DuctDisplacementDefine.num_100;
            }
            else if ((ductp.FiftyButton == false) &&
                (ductp.OneHandredButton == false) &&
                (ductp.NothingButton == true))
            {
                rc = DuctDisplacementDefine.num_0;
            }
            else
            {
                //error
                log.Error("Check RoundType Out");
                rc = DuctDisplacementDefine.num_50;
            }

            return rc;
        }

        /// <summary>
        /// 耐火被覆厚が有効/無効 判定
        ///
        /// </summary>
        /// <returns>戻り値ture  :有効
        ///                false :無効</returns>
        public bool FireProofingType(ConnectionSettingViewModel ductp)
        {
            bool rc;

            if ((ductp.FireProofingValidButton == true) &&
                (ductp.FireProofingInValidButton == false))
            {
                rc = true;
            }
            else if ((ductp.FireProofingValidButton == false) &&
                    (ductp.FireProofingInValidButton == true))
            {
                rc = false;
            }
            else
            {
                //error
                log.Error("Check ReferenceLineType Out");
                rc = true;
            }

            return rc;
        }

        /// <summary>
        /// テキストボックスに値をセットできるか判定
        /// </summary>
        /// <param name="text"></param>
        /// <param name="befortext"></param>
        /// <returns> true:値をセットできる
        /// 　　　　 false:値をセットできない</returns>
        public bool TextBoxSetterControl(string text, string befortext)
        {
            bool acceptflag = true;

            //全角文字判定
            // Todo:確認必要
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            byte[] byte_data = System.Text.Encoding.GetEncoding(932).GetBytes(text);
            if (byte_data.Length != text.Length)
            {
                acceptflag &= false;
            }

            //.判定
            if (text.Length - text.Replace(".", "").Length > 1)
            {
                acceptflag &= false;
            }

            //文字変更判定
            if (befortext == text)
            {
                acceptflag &= false;
            }

            //文字列の長さ判定
            //if (text.Length >= 11)
            //{
            //    acceptflag &= false;
            //}

            return acceptflag;
        }

        #endregion Member Functions
    }
}