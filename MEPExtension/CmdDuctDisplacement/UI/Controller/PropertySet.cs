using Autodesk.Revit.DB;
using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.Logic;
using CmdDuctDisplacement.Resource;
using CmdDuctDisplacement.UI.Common;
using CmdDuctDisplacement.UI.Model;
using CmdDuctDisplacement.UI.Model.Entity;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System.Runtime.CompilerServices;

namespace CmdDuctDisplacement.UI.Controller
{
    /// <summary>
    /// Viewで変更されたプロパティのSetterで呼び出されるクラス
    /// 内部でViewModelの関連プロパティを書き換える
    /// </summary>
    //インスタンス生成
    class PropertySet
    {
        //メンバ変数
        #region Memeber Variables
        private Logger log;
        MEPOperation mep;
        ConnectionSettingViewModel instance;
        #endregion

        //コンストラクタ
        #region Constructor
        public PropertySet(ConnectionSettingViewModel _connectionsettingviewmodel, MEPOperation _mep)
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

            mep = _mep;
            instance = _connectionsettingviewmodel;
        }
        #endregion

        // メンバ関数
        #region Member Functions

        /// <summary>
        /// 呼び出し元のメンバネームを解析し、各メソッドに振り分ける
        /// </summary>
        /// <param name="memberName">呼び出し元のメンバネーム</param>
        public void ChangesReflect([CallerMemberName] string memberName = "")
        {
            string str = memberName;

            switch (memberName)
            {
                //ラジオボタン
                //基準ラインコンボボックスの表示/非表示フラグ
                case "OffsetRadioButton":
                case "UnityRadioButton":
                case "FortyFiveElbowRadioButton":
                case "NinetyElbowRadioButton":
                case "ScarveElbowRadioButton":
                case "FiftyButton":
                case "OneHandredButton":
                case "BaseLineControlFlag":
                    break;

                //移動図形名称
                case "SelectObjName":
                    SelectObjNameChangeCheck(instance);
                    break;
                case "MoveOptionExpandedFlag":
                    MoveOptionExpandedFlagChangeCheck(instance);
                    break;
                //ユーザー入力テキストボックス
                case "DuctOffsetLevel":
                    DuctOffsetLevelChangeCheck(instance);
                    break;
                //レベルボタン名称
                case "LevelButtonName":
                    LevelButtonNameChangeCheck(instance);
                    break;
                //選択図形の基準ライン
                case "DuctReferenceLine":
                    DuctReferenceLineChangeCheck(instance);
                    break;
                //離隔※値はセットしないが、エラーチェックのみ担当
                case "IsolationValue":
                    IsolationValueChangeCheck(instance);
                    break;
                //上部配置
                case "TopArrangementColor":
                    TopArrangementColorChangeCheck(instance);
                    break;
                //下部配置
                case "BottomArrangementColor":
                    BottomArrangementColorChangeCheck(instance);
                    break;
                //耐火被覆 有
                case "FireProofingValidButton":
                    FireProofingValidButtonChangeCheck(instance);
                    break;
                //耐火被覆 無
                case "FireProofingInValidButton":
                    FireProofingInValidButtonChangeCheck(instance);
                    break;
                //部材間距離
                case "BetweenObjValue":
                    BetweenObjValueChangeCheck(instance);
                    break;
                //耐火被覆厚
                case "FireProofingValue":
                    FireProofingValueChangeCheck(instance);
                    break;

                //未実装もしくは、変更されてはいけないプロパティ
                case "WindowTitle":
                default:
                    //error
                    break;

            }


        }

        /// <summary>
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        private void DuctOffsetLevelChangeCheck(ConnectionSettingViewModel instance)
        {

        }

        /// <summary>
        /// FMボタン切り替わりに付随する処理関連
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        private void LevelButtonNameChangeCheck(ConnectionSettingViewModel instance)
        {
            //基準ラインプルダウン切り替え制御
            if (instance.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel))
            {
                instance.BaseLineControlFlag = true;
            }

            //移動量の場合は、基準ラインの切り替えができないように制御する
            else if (instance.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovement))
            {
                instance.BaseLineControlFlag = false;
            }

            else
            {
                //error
                log.Error("LevelButtonName: " + instance.LevelButtonName);
                instance.BaseLineControlFlag = false;
            }
        }

        /// <summary>
        /// 基準ライン変更に付随する処理関連
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        private void DuctReferenceLineChangeCheck(ConnectionSettingViewModel instance)
        {
            ControlStatus controlstatus = ControlStatus.Instance;
            WindowControl ce = new WindowControl();

            //移動図形の初期値を基準ラインに合わせた値にセットしなおす
            instance.SelctObj_InitalFL += ce.ReferenceLineChange(mep, instance.DuctReferenceLine);

            //基準ラインを考慮したFL値をセットする
            if (controlstatus.ReferenceLine_Befor != string.Empty)
            {
                //FMテキストボックスの表示形式がFLの場合
                if (instance.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel))
                {
                    instance.DuctOffsetLevel = ((double)((decimal)instance.InternalDuctOffsetLevel + (decimal)ce.ReferenceLineChange(mep, instance.DuctReferenceLine))).ToString();
                }

                //FMテキストボックスの表示形式が移動量の場合
                else if (instance.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovement))
                {
                    //移動量では基準ラインを変更できないのでerror
                    log.Error("Ckeck LevelButtonName Out");
                    instance.DuctOffsetLevel = ((double)((decimal)instance.InternalDuctOffsetLevel + (decimal)ce.ReferenceLineChange(mep, instance.DuctReferenceLine))).ToString();
                }

                else
                {
                    //error
                    log.Error("Ckeck LevelButtonName Out");
                }
            }
        }

        /// <summary>
        /// 隙間間隔変更に付随する処理関連
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        private void IsolationValueChangeCheck(ConnectionSettingViewModel instance)
        {
            var optproperty = WindowReceiveProperty.Instance;
            double xx;

            //警告メッセージをクリアしておく
            instance.IsolationWorningMessage = "";

            //隙間間隔赤文字対応 値を見て判断する
            double.TryParse(instance.BetweenObjValue, out xx);
            if (instance.IsolationValue < xx)
            {
                instance.IsolationValueColor = "Red";

                //隙間間隔警告メッセージ対応 値を見て判断する
                double.TryParse(instance.BetweenObjValue, out xx);
                if ((instance.IsolationValue < xx) &&
                    (instance.IsolationValue > 0))
                {
                    instance.IsolationWorningMessage = ExResources.ResxString(DuctDisplacementDefine.LVL_IsolationWorningMessage_more);
                }

                else if (instance.IsolationValue <= 0)
                {
                    instance.IsolationWorningMessage = instance.TargetObjName + ExResources.ResxString(DuctDisplacementDefine.LVL_IsolationWorningMessage_less);
                }

                else
                {
                    log.Error("Check IsolationValue");
                    instance.IsolationWorningMessage = "";
                }

            }
            else
            {
                instance.IsolationValueColor = "DimGray";
            }

        }

        /// <summary>
        /// 耐火被覆有効ラジオボタン押下に付随する処理関連
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        private void FireProofingValidButtonChangeCheck(ConnectionSettingViewModel instance)
        {
            if (instance.FireProofingValidButton == true)
            {
                instance.SetDisplayFireProofing = "Visible";
            }
        }

        /// <summary>
        /// 耐火被覆無効ラジオボタン押下に付随する処理関連
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        private void FireProofingInValidButtonChangeCheck(ConnectionSettingViewModel instance)
        {
            if (instance.FireProofingInValidButton == true)
            {
                instance.SetDisplayFireProofing = "Hidden";
            }

        }

        /// <summary>
        /// オプション画面表示/非表示切り替えに付随する処理関連
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        private void MoveOptionExpandedFlagChangeCheck(ConnectionSettingViewModel instance)
        {
            //Expandederの文字色を変更する
            if (instance.MoveOptionExpandedFlag)
            {
                instance.MoveOptionShow = ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovementOPT_Hidden);
                instance.OptionFontWeight = "Normal";
            }
            else
            {
                instance.MoveOptionShow = ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovementOPT_Disp);
                instance.OptionFontWeight = "Black";
            }
        }

        /// <summary>
        /// 移動図形と回避対象物の位置関係が反転しラベルの色変化に付随する処理関連、
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        private void TopArrangementColorChangeCheck(ConnectionSettingViewModel instance)
        {
            if (instance.TopArrangementColor == "#FFB0C4DE")
            {
                instance.Location_TopObj = instance.SelectObjName;
                instance.TopObjAddColorName = ExResources.ResxString(DuctDisplacementDefine.LVL_Moving) + instance.SelectObjName;
            }

            else if (instance.TopArrangementColor == "#FFD3D3D3")
            {
                instance.Location_TopObj = instance.TargetObjName;
                instance.TopObjAddColorName = ExResources.ResxString(DuctDisplacementDefine.LVL_OfTarget) + instance.TargetObjName;
            }

            else
            {
                //error
                log.Error("Check TopArrangementColor:" + instance.TopArrangementColor);
            }
        }

        /// <summary>
        /// 移動図形と回避対象物の位置関係が反転しラベルの色変化に付随する処理関連
        /// </summary>
        /// <param name="instance">ConnectionSettingViewModelインスタンス</param>
        /// <param name="instance"></param>
        private void BottomArrangementColorChangeCheck(ConnectionSettingViewModel instance)
        {
            if (instance.BottomArrangementColor == "#FFB0C4DE")
            {
                instance.Location_BottomObj = instance.SelectObjName;
                instance.BottomObjAddColorName = ExResources.ResxString(DuctDisplacementDefine.LVL_Moving) + instance.SelectObjName;
            }

            else if (instance.BottomArrangementColor == "#FFD3D3D3")
            {
                instance.Location_BottomObj = instance.TargetObjName;
                instance.BottomObjAddColorName = ExResources.ResxString(DuctDisplacementDefine.LVL_OfTarget) + instance.TargetObjName;
            }

            else
            {
                //error
                log.Error("Check BottomArrangementColor:" + instance.BottomArrangementColor);
            }
        }

        /// <summary>
        /// 回避対象物選択時に付随する処理関連
        /// </summary>
        /// <param name="instance"></param>
        private void SelectObjNameChangeCheck(ConnectionSettingViewModel instance)
        {
            instance.SelectObjName_MoveMethod = instance.SelectObjName + ExResources.ResxString(DuctDisplacementDefine.LVL_MovingMethod);
            instance.SelectObjName_Move = instance.SelectObjName + ExResources.ResxString(DuctDisplacementDefine.LVL_Move);
        }
        private void BetweenObjValueChangeCheck(ConnectionSettingViewModel instance)
        {

        }
        private void FireProofingValueChangeCheck(ConnectionSettingViewModel instance)
        {

        }
        #endregion
    }

    class PropertyGet
    {
        //メンバ変数
        #region Memeber Variables
        private Logger log;
        #endregion

        //コンストラクタ
        #region Constructor
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PropertyGet()
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


        // メンバ関数
        #region Member Functions

        #region DuctOffsetLevel getter 呼び出しメソッド
        /// <summary>
        /// Viewで変更されたプロパティのGetterで呼び出されるクラス
        /// 内部でViewModelの関連プロパティを書き換える
        /// </summary>
        /// <param name="connectionsettingviewmodel">ConnectionSettingViewModelインスタンス</param>
        /// <param name="mep">MEPOperationインスタンス</param>
        /// <returns>変更経路によって再計算されたFMテキストボックスの数値</returns>
        public double DuctOffsetLevelChangeCheck(ConnectionSettingViewModel connectionsettingviewmodel, MEPOperation mep)
        {
            var controlstatus = ControlStatus.Instance;
            WindowControl ductcon = new WindowControl();
            DuctDisplacementDefine.Rounder roundnum;
            RoundNum round = new RoundNum();

            //FMボタン"FL"
            if (connectionsettingviewmodel.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel))
            {
                log.Trace("DuctOffsetLevelChangeCheck CallRoute:" + controlstatus.CallRoute);
                switch (controlstatus.CallRoute)
                {
                    //経路なし
                    case (DuctDisplacementDefine.TextChangeRoute.NoSelect):
                        //受け取った文字をそのまま書く
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //テキストボックス直接入力経路
                    case (DuctDisplacementDefine.TextChangeRoute.Text):
                        //受け取った文字をそのまま書く
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //▲▼ボタン経路
                    case (DuctDisplacementDefine.TextChangeRoute.IncreaseButton):
                    case (DuctDisplacementDefine.TextChangeRoute.DecreaseButton):
                        //丸めて表示
                        roundnum = ductcon.GetRoundAccuracyButton(connectionsettingviewmodel);

                        //丸め有/無判定
                        if (((0 != connectionsettingviewmodel.InternalDuctOffsetLevel % DuctDisplacementDefine.num_50) &&
                            (roundnum == DuctDisplacementDefine.Rounder.Multiple_50)) ||
                            ((0 != connectionsettingviewmodel.InternalDuctOffsetLevel % DuctDisplacementDefine.num_100) &&
                            (roundnum == DuctDisplacementDefine.Rounder.Multiple_100)))
                        {
                            //丸め処理
                            if (controlstatus.CallRoute == DuctDisplacementDefine.TextChangeRoute.IncreaseButton)
                            {
                                return round.GetRoundUpValue(connectionsettingviewmodel.InternalDuctOffsetLevel, roundnum);
                            }

                            else if (controlstatus.CallRoute == DuctDisplacementDefine.TextChangeRoute.DecreaseButton)
                            {
                                return round.GetRoundDownValue(connectionsettingviewmodel.InternalDuctOffsetLevel, roundnum);
                            }

                            else
                            {
                                //Error
                                log.Error("Check TextChangeRoute:" + controlstatus.CallRoute + " LevelButtonName:" + connectionsettingviewmodel.LevelButtonName);
                                return round.GetRoundUpValue(connectionsettingviewmodel.InternalDuctOffsetLevel, roundnum);
                            }
                        }
                        //丸めず表示
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //基準位置変更経路
                    case (DuctDisplacementDefine.TextChangeRoute.RefalenceLine):
                        //丸めず表示
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //回避対象物選択経路
                    case (DuctDisplacementDefine.TextChangeRoute.MoveDistanceCalButton):
                        //丸めず表示
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //FMボタン押下経路
                    case (DuctDisplacementDefine.TextChangeRoute.SwitchCalMethodButton):
                        //丸めず表示
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //1,2点選択経路
                    case (DuctDisplacementDefine.TextChangeRoute.PointClick_1_2):
                        return round.RoundUnnecessaryNum((double)((decimal)connectionsettingviewmodel.InternalDuctOffsetLevel + (decimal)ductcon.AddHeight(connectionsettingviewmodel.DuctReferenceLine, mep)));

                    //↓↑ボタン経路
                    case (DuctDisplacementDefine.TextChangeRoute.BottomArrangmentButton):
                    case (DuctDisplacementDefine.TextChangeRoute.TopArrangementButton):
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;
                    //3点入力時
                    case (DuctDisplacementDefine.TextChangeRoute.PointClick_3):
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;
                    default:
                        //error
                        log.Error("Check TextChangeRoute:" + controlstatus.CallRoute);
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;
                }
            }

            //FMボタン"移動量"
            else if (connectionsettingviewmodel.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovement))
            {
                switch (controlstatus.CallRoute)
                {
                    //経路なし
                    case (DuctDisplacementDefine.TextChangeRoute.NoSelect):
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //テキストボックス直接入力経路
                    case (DuctDisplacementDefine.TextChangeRoute.Text):
                        //受け取った文字をそのまま書く
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //▲▼ボタン経路
                    case (DuctDisplacementDefine.TextChangeRoute.IncreaseButton):
                    case (DuctDisplacementDefine.TextChangeRoute.DecreaseButton):
                        //丸めて表示
                        roundnum = ductcon.GetRoundAccuracyButton(connectionsettingviewmodel);

                        //丸め有/無判定
                        if (((0 != connectionsettingviewmodel.InternalDuctOffsetLevel % DuctDisplacementDefine.num_50) &&
                            (roundnum == DuctDisplacementDefine.Rounder.Multiple_50)) ||
                            ((0 != connectionsettingviewmodel.InternalDuctOffsetLevel % DuctDisplacementDefine.num_100) &&
                            (roundnum == DuctDisplacementDefine.Rounder.Multiple_100)))
                        {
                            //丸め処理
                            if (controlstatus.CallRoute == DuctDisplacementDefine.TextChangeRoute.IncreaseButton)
                            {
                                return round.GetRoundUpValue(connectionsettingviewmodel.InternalDuctOffsetLevel, roundnum);
                            }

                            else if (controlstatus.CallRoute == DuctDisplacementDefine.TextChangeRoute.DecreaseButton)
                            {
                                return round.GetRoundDownValue(connectionsettingviewmodel.InternalDuctOffsetLevel, roundnum);
                            }

                            else
                            {
                                //Error
                                log.Error("Check TextChangeRoute:" + controlstatus.CallRoute + " LevelButtonName:" + connectionsettingviewmodel.LevelButtonName);
                                return round.GetRoundUpValue(connectionsettingviewmodel.InternalDuctOffsetLevel, roundnum);
                            }
                        }
                        //丸めず表示
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //基準位置変更経路
                    case (DuctDisplacementDefine.TextChangeRoute.RefalenceLine):
                        //移動量で基準ラインを変更することはできない仕様のためError
                        log.Error("Check TextChangeRoute:" + controlstatus.CallRoute + " LevelButtonName:" + connectionsettingviewmodel.LevelButtonName);
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //FMボタン押下経路
                    case (DuctDisplacementDefine.TextChangeRoute.SwitchCalMethodButton):
                        //丸めず表示
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //回避対象物選択経路
                    case (DuctDisplacementDefine.TextChangeRoute.MoveDistanceCalButton):
                        //対象物選択時は、必ずFL表示の為Error
                        log.Error("Check TextChangeRoute:" + controlstatus.CallRoute + " LevelButtonName:" + connectionsettingviewmodel.LevelButtonName);
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //↓ボタン経路
                    case (DuctDisplacementDefine.TextChangeRoute.BottomArrangmentButton):
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;

                    //↑ボタン経路
                    case (DuctDisplacementDefine.TextChangeRoute.TopArrangementButton):
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;
                    //3点入力時
                    case (DuctDisplacementDefine.TextChangeRoute.PointClick_3):
                        log.Error("Check TextChangeRoute:" + controlstatus.CallRoute);
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;
                    default:
                        //Error
                        log.Error("Check TextChangeRoute:" + controlstatus.CallRoute + " LevelButtonName:" + connectionsettingviewmodel.LevelButtonName);
                        return connectionsettingviewmodel.InternalDuctOffsetLevel;
                }
            }
            else
            {
                //Error
                log.Error("Check TextChangeRoute:" + controlstatus.CallRoute + " LevelButtonName:" + connectionsettingviewmodel.LevelButtonName);
                return connectionsettingviewmodel.InternalDuctOffsetLevel;
            }

        }
        #endregion

        #region 隙間間隔算出メソッド


        /// <summary>
        /// 隙間間隔算出
        /// </summary>
        /// <param name="connectionsettingviewmodel">ConnectionSettingViewModelインスタンス</param>
        /// <param name="mep">MEPOperationインスタンス</param>
        /// <returns>隙間間隔</returns>
        public double SetIsolationValue(ConnectionSettingViewModel connectionsettingviewmodel, MEPOperation mep)
        {
            RoundNum roundnum = new RoundNum();
            WindowControl windowcontrol = new WindowControl();

            //移動図形の基準面を考慮したFLオフセット値
            double fl;
            //回避対象図形の基準面を考慮したFLオフセット値
            double ObjReferenceLevel;
            //戻り値
            double rc;

            //移動図形の幅
            double Height = (UnitUtils.Convert(mep.GetHeight(DuctDisplacementDefine.InstructionObj.MoveObj_1),
                              UnitTypeId.Feet, UnitTypeId.Millimeters));

            double halfHeight = Height / 2;
            double addheight;

            //移動図形のFL初期値の幅を考慮した値を算出するための位置合わせ
            double initheight;

            //下ボタンがONの時
            if ((connectionsettingviewmodel.BottomArrangementColor == "#FFB0C4DE") &&
                (connectionsettingviewmodel.TopArrangementColor == "#FFD3D3D3"))
            {
                if (connectionsettingviewmodel.DuctReferenceLine == ExResources.ResxString(DuctDisplacementDefine.LVL_TopSide))
                {
                    addheight = 0;
                    initheight = halfHeight;
                }
                else if (connectionsettingviewmodel.DuctReferenceLine == ExResources.ResxString(DuctDisplacementDefine.LVL_Center))
                {
                    addheight = halfHeight;
                    initheight = 0;
                }
                else if (connectionsettingviewmodel.DuctReferenceLine == ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide))
                {
                    addheight = Height;
                    initheight = -halfHeight;
                }

                else
                {
                    //error
                    log.Error("Check SetIsolationValue");
                    addheight = 0;
                    initheight = 0;
                }

                if (connectionsettingviewmodel.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel))
                {


                    fl = (double)((decimal)connectionsettingviewmodel.InternalDuctOffsetLevel + (decimal)addheight);
                }
                else if (connectionsettingviewmodel.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovement))
                {
                    fl = (double)(((decimal)connectionsettingviewmodel.InternalDuctOffsetLevel + (decimal)addheight) + ((decimal)windowcontrol.GetInitSelectOBJRefCenterValue(mep) + (decimal)initheight));
                }
                else
                {
                    fl = 0;
                }
            }

            //上ボタンがONの時
            else if ((connectionsettingviewmodel.TopArrangementColor == "#FFB0C4DE") &&
                    (connectionsettingviewmodel.BottomArrangementColor == "#FFD3D3D3"))
            {
                if (connectionsettingviewmodel.DuctReferenceLine == ExResources.ResxString(DuctDisplacementDefine.LVL_TopSide))
                {
                    addheight = -Height;
                    initheight = halfHeight;
                }
                else if (connectionsettingviewmodel.DuctReferenceLine == ExResources.ResxString(DuctDisplacementDefine.LVL_Center))
                {
                    addheight = -halfHeight;
                    initheight = 0;
                }
                else if (connectionsettingviewmodel.DuctReferenceLine == ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide))
                {
                    addheight = 0;
                    initheight = -halfHeight;
                }

                else
                {
                    //error
                    log.Error("Check SetIsolationValue");
                    addheight = 0;
                    initheight = 0;
                }

                if (connectionsettingviewmodel.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_FloorLevel))
                {
                    fl = (double)((decimal)connectionsettingviewmodel.InternalDuctOffsetLevel + (decimal)addheight);
                }
                else if (connectionsettingviewmodel.LevelButtonName == ExResources.ResxString(DuctDisplacementDefine.LVL_AmountMovement))
                {
                    fl = (double)(((decimal)connectionsettingviewmodel.InternalDuctOffsetLevel + (decimal)addheight) + ((decimal)windowcontrol.GetInitSelectOBJRefCenterValue(mep) + (decimal)initheight));
                }
                else
                {
                    fl = 0;
                }
            }

            else
            {
                //error
                log.Error("LevelButtonName: " + connectionsettingviewmodel.LevelButtonName);
                connectionsettingviewmodel.BaseLineControlFlag = false;
                fl = 0;
            }

            //下ボタンがONの時
            if ((connectionsettingviewmodel.BottomArrangementColor == "#FFB0C4DE") &&
                (connectionsettingviewmodel.TopArrangementColor == "#FFD3D3D3"))
            {
                ObjReferenceLevel = roundnum.RoundUnnecessaryNum(UnitUtils.Convert((double)(decimal)mep.GetObjReferenceLevel
                     (DuctDisplacementDefine.InstructionObj.TargetObj, DuctDisplacementDefine.Line.Bottom),
                      UnitTypeId.Feet, UnitTypeId.Millimeters));
                rc = (double)((decimal)ObjReferenceLevel - (decimal)fl);
            }

            //上ボタンがONの時
            else if ((connectionsettingviewmodel.TopArrangementColor == "#FFB0C4DE") &&
                    (connectionsettingviewmodel.BottomArrangementColor == "#FFD3D3D3"))
            {
                ObjReferenceLevel = roundnum.RoundUnnecessaryNum(UnitUtils.Convert((double)(decimal)mep.GetObjReferenceLevel
                     (DuctDisplacementDefine.InstructionObj.TargetObj, DuctDisplacementDefine.Line.Top),
                      UnitTypeId.Feet, UnitTypeId.Millimeters));
                rc = (double)((decimal)fl - (decimal)ObjReferenceLevel);
            }

            else
            {
                rc = 0;
            }

            //耐火被覆と断熱材の厚み
            double pick1_thickness = mep.GetInsulationMaterialThickness(DuctDisplacementDefine.InstructionObj.MoveObj_1);
            double pick3_thickness = mep.GetInsulationMaterialThickness(DuctDisplacementDefine.InstructionObj.TargetObj);
            double pickthickness = roundnum.RoundUnnecessaryNum(UnitUtils.Convert
                (pick1_thickness + pick3_thickness, UnitTypeId.Feet, UnitTypeId.Millimeters));

            double FireProofingValue = 0;

            if ((mep.GetFamilyName(DuctDisplacementDefine.InstructionObj.TargetObj) == ExResources.ResxString(DuctDisplacementDefine.LVL_Beam)) &&
                (windowcontrol.FireProofingType(connectionsettingviewmodel)))
            {
                double xx;
                double.TryParse(connectionsettingviewmodel.FireProofingValue, out xx);
                FireProofingValue = xx;
            }

            else
            {
                FireProofingValue = 0;
            }

            double thickness = pickthickness + FireProofingValue;

            return rc - thickness;
        }
        #endregion
        #endregion
    }
}