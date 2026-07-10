
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 基底</summary>
    /// ================================================================================
    public abstract class DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private RvtExtApp.Components.Elements _CmpElements;

        /// <summary>図形</summary>
        private RvtExtApp.Components.Geometry _CmpGeometry;

        /// <summary>パラメーター</summary>
        private RvtExtApp.Components.Parameters _CmpParameters;

        /// <summary>設定</summary>
        private RvtExtApp.Components.Settings _CmpSettings;

        /// <summary>データテーブル - 項目</summary>
        private RvtExtApp.Entities.DtItems _EntDtItems;

        /// <summary>エラーメッセージ</summary>
        private string _ErrMsg;

        /// <summary>コマンド種類</summary>
        private int _CommandKind;

        /// <summary>コマンド値 - 用途地域オプション</summary>
        private int _CvUseDistrictOpt;

        /// <summary>コマンド値 - 見出し作成チェック</summary>
        private bool _CvChkCreateHeader;

        /// <summary>コマンド値 - 縁側</summary>
        private bool _CvVeranda;

        /// <summary>コマンド値 - 道路</summary>
        private bool _CvRoadSide;

        /// <summary>コマンド値 - 水平測定距離</summary>
        private string _CvHorizontalMeas;

        /// <summary>コマンド値 - 水平補正距離</summary>
        private string _CvHorizontalCorr;

        /// <summary>コマンド値 - 垂直測定距離</summary>
        private string _CvVerticalMeas;

        /// <summary>コマンド値 - 垂直補正距離</summary>
        private string _CvVerticalCorr;

        /// <summary>コマンド値 - 天端高さ</summary>
        private string _CvHeadHeight;

        /// <summary>コマンド値 - 天井高さ</summary>
        private string _CvCeilingHeight;

        /// <summary>コマンド値 - 防煙壁長さ</summary>
        private string _CvSmokeWallLength;

        /// <summary>コマンド値 - 排煙有効高さ</summary>
        private string _CvUsableHeightSmoke;

        /// <summary>列名 ID</summary>
        private string _ColNameID;

        /// <summary>列名 レベル名</summary>
        private string _ColNameLevelName;

        /// <summary>列名 グループ名</summary>
        private string _ColNameGroupName;

        /// <summary>列名 部屋名</summary>
        private string _ColNameRoomName;

        /// <summary>列名 部屋番号</summary>
        private string _ColNameRoomNo;

        /// <summary>列名 面積</summary>
        private string _ColNameArea;

        /// <summary>列名 必要係数</summary>
        private string _ColNameNecessaryCoefficient;

        /// <summary>列名 平均天井高</summary>
        private string _ColNameAverageCeilingHeight;

        /// <summary>列名 必要面積</summary>
        private string _ColNameNecessaryArea;

        /// <summary>列名 合計有効面積</summary>
        private string _ColNameTotalUsableArea;

        /// <summary>列名 判定</summary>
        private string _ColNameJudgment;

        /// <summary>列名 所属部屋</summary>
        private string _ColNameAffiliationRoom;

        /// <summary>列名 符号</summary>
        private string _ColNameSign;

        /// <summary>列名 水平距離</summary>
        private string _ColNameHorizontalDist;

        /// <summary>列名 垂直距離</summary>
        private string _ColNameVerticalDist;

        /// <summary>列名 d/h</summary>
        private string _ColNameDsH;

        /// <summary>列名 α</summary>
        private string _ColNameA;

        /// <summary>列名 β</summary>
        private string _ColNameB;

        /// <summary>列名 D</summary>
        private string _ColNameD;

        /// <summary>列名 A(仮)</summary>
        private string _ColNameATemp;

        /// <summary>列名 A(補正値)</summary>
        private string _ColNameACorr;

        /// <summary>列名 開口係数</summary>
        private string _ColNameOpenCoefficient;

        /// <summary>列名 有効幅</summary>
        private string _ColNameUsableWidth;

        /// <summary>列名 有効高さ</summary>
        private string _ColNameUsableHeight;

        /// <summary>列名 排煙有効高さ</summary>
        private string _ColNameUsableHeightSmoke;

        /// <summary>列名 有効開口面積</summary>
        private string _ColNameUsableOpenArea;

        /// <summary>列名 有効面積</summary>
        private string _ColNameUsableArea;

        /// <summary>列名 カテゴリ</summary>
        private string _ColNameCategory;

        /// <summary>列名 排煙窓幅</summary>
        private string _ColNameSmokeWinWidth;

        /// <summary>列名 排煙窓高さ</summary>
        private string _ColNameSmokeWinHeight;

        /// <summary>小数点桁数の最小値</summary>
        private int _DecimalMin;

        /// <summary>小数点桁数の最大値</summary>
        private int _DecimalMax;

        /// <summary>Legal area rounding type</summary>
        private int _LegalAreaRoundingOpt;

        /// <summary>Area To Get Light rounding type</summary>
        private int _AreaToGetLightRoundingOpt;

        /// <summary>d/h・A(仮)・A(補正値) rounding type</summary>
        private int _DHRoundingOpt;

        /// <summary>Effective Lighting Area rounding type</summary>
        private int _EffectiveLightingAreaRoundingOpt;

        /// <summary>Effective Opening Area rounding type</summary>
        private int _EffectiveOpeningAreaRoundingOpt;

        /// <summary>Area To Be Smoked rounding type</summary>
        private int _AreaToBeSmokedRoundingOtp;

        /// <summary>Effective Smoke Extraction Area rounding type</summary>
        private int _EffectiveSmokeExtractionAreaRoundingOtp;

        /// <summary>Area To Be Ventilated rounding type</summary>
        private int _AreaToBeVentilatedRoundingOtp;

        /// <summary>Effective Ventilation Area rounding type</summary>
        private int _EffectiveVentilationAreaRoundingOtp;

        /// <summary>Decimal point position of legal area</summary>
        private int _LegalAreaRoundingDecimal;

        /// <summary>Decimal point position of Area To Get Light</summary>
        private int _AreaToGetLightRoundingDecimal;

        /// <summary>Decimal point position of d/h・A(仮)・A(補正値)</summary>
        private int _DHRoundingDecimal;

        /// <summary>Decimal point position of Effective Lighting Area</summary>
        private int _EffectiveLightingAreaRoundingDecimal;

        /// <summary>Decimal point position of Effective Opening Area</summary>
        private int _EffectiveOpeningAreaRoundingDecimal;

        /// <summary>Decimal point position of Area To Be Smoked</summary>
        private int _AreaToBeSmokedRoundingDecimal;

        /// <summary>Decimal point position of Effective Smoke Extraction Area</summary>
        private int _EffectiveSmokeExtractionAreaRoundingDecimal;

        /// <summary>Decimal point position of Area To Be Ventilated</summary>
        private int _AreaToBeVentilatedRoundingDecimal;

        /// <summary>Decimal point position of Effective Ventilation Area</summary>
        private int _EffectiveVentilationAreaRoundingDecimal;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpElements"   >要素</param>
        /// <param name="cmpGeometry"   >図形</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        ///
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii<p>
        ///         <p>2021/11/24 Modified Applied Technology<p></history>
        /// ================================================================================
        protected DtBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.Elements cmpElements,
                         RvtExtApp.Components.Geometry cmpGeometry,
                         RvtExtApp.Components.Parameters cmpParameters,
                         RvtExtApp.Components.Settings cmpSettings)
        {
            // 初期化
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _ErrMsg = "";
            _DecimalMax = 4;
            _DecimalMin = 1;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>コマンド値取得</summary>
        ///
        /// <param name="dataAry">コマンドデータ</param>
        ///
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected void GetCmdValue(Collections.Generic.IList<string> dataAry)
        {
            // 初期化
            string sValue = "";
            int iValue = 0;

            // 用途地域オプション
            _CvUseDistrictOpt = 0;
            if (dataAry.Count > 0)
            {
                sValue = dataAry[0];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 0;
                        }
                        _CvUseDistrictOpt = iValue;
                    }
                }
            }

            // 見出し作成チェック
            _CvChkCreateHeader = true;
            if (dataAry.Count > 1)
            {
                sValue = dataAry[1];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsBool(sValue) == true)
                    {
                        _CvChkCreateHeader = bool.Parse(sValue);
                    }
                }
            }

            // 縁側
            _CvVeranda = false;
            if (dataAry.Count > 2)
            {
                sValue = dataAry[2];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsBool(sValue) == true)
                    {
                        _CvVeranda = bool.Parse(sValue);
                    }
                }
            }

            // 道路
            _CvRoadSide = false;
            if (dataAry.Count > 3)
            {
                sValue = dataAry[3];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsBool(sValue) == true)
                    {
                        _CvRoadSide = bool.Parse(sValue);
                    }
                }
            }

            // 水平測定距離
            _CvHorizontalMeas = "0";
            if (dataAry.Count > 4)
            {
                sValue = dataAry[4];
                if ((sValue != null) && (sValue != ""))
                {
                    _CvHorizontalMeas = sValue;
                }
            }

            // 水平補正距離
            _CvHorizontalCorr = "0";
            if (dataAry.Count > 5)
            {
                sValue = dataAry[5];
                if ((sValue != null) && (sValue != ""))
                {
                    _CvHorizontalCorr = sValue;
                }
            }

            // 垂直測定距離
            _CvVerticalMeas = "0";
            if (dataAry.Count > 6)
            {
                sValue = dataAry[6];
                if ((sValue != null) && (sValue != ""))
                {
                    _CvVerticalMeas = sValue;
                }
            }

            // 垂直補正距離
            _CvVerticalCorr = "0";
            if (dataAry.Count > 7)
            {
                sValue = dataAry[7];
                if ((sValue != null) && (sValue != ""))
                {
                    _CvVerticalCorr = sValue;
                }
            }

            // 天端高さ
            _CvHeadHeight = "0";
            if (dataAry.Count > 8)
            {
                sValue = dataAry[8];
                if ((sValue != null) && (sValue != ""))
                {
                    _CvHeadHeight = sValue;
                }
            }

            // 天井高さ
            _CvCeilingHeight = "0";
            if (dataAry.Count > 9)
            {
                sValue = dataAry[9];
                if ((sValue != null) && (sValue != ""))
                {
                    _CvCeilingHeight = sValue;
                }
            }

            // 防煙壁長さ
            _CvSmokeWallLength = "0";
            if (dataAry.Count > 10)
            {
                sValue = dataAry[10];
                if ((sValue != null) && (sValue != ""))
                {
                    _CvSmokeWallLength = sValue;
                }
            }

            // 排煙有効高さ
            _CvUsableHeightSmoke = "0";
            if (dataAry.Count > 11)
            {
                sValue = dataAry[11];
                if ((sValue != null) && (sValue != ""))
                {
                    _CvUsableHeightSmoke = sValue;
                }
            }
            //Decimal point position of legal area
            _LegalAreaRoundingDecimal = 3;
            if (dataAry.Count > 12)
            {
                sValue = dataAry[12];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < _DecimalMin || iValue > _DecimalMax)
                        {
                            iValue = 3;
                        }
                        _LegalAreaRoundingDecimal = iValue;
                    }
                }
            }
            // Legal area fraction type
            _LegalAreaRoundingOpt = 2;
            if (dataAry.Count > 13)
            {
                sValue = dataAry[13];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 2;
                        }
                        _LegalAreaRoundingOpt = iValue;
                    }
                }
            }
            // Decimal point posittion of are to get light
            _AreaToGetLightRoundingDecimal = 3;
            if (dataAry.Count > 14)
            {
                sValue = dataAry[14];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < _DecimalMin || iValue > _DecimalMax)
                        {
                            iValue = 3;
                        }
                        _AreaToGetLightRoundingDecimal = iValue;
                    }
                }
            }
            // Are to get light fraction type
            _AreaToGetLightRoundingOpt = 2;
            if (dataAry.Count > 15)
            {
                sValue = dataAry[15];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 2;
                        }
                        _AreaToGetLightRoundingOpt = iValue;
                    }
                }
            }
            //Decimal point position of d/h
            _DHRoundingDecimal = 3;
            if (dataAry.Count > 16)
            {
                sValue = dataAry[16];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < _DecimalMin || iValue > _DecimalMax)
                        {
                            iValue = 3;
                        }
                        _DHRoundingDecimal = iValue;
                    }
                }
            }
            // D /H fraction type
            _DHRoundingOpt = 2;
            if (dataAry.Count > 17)
            {
                sValue = dataAry[17];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 2;
                        }
                        _DHRoundingOpt = iValue;
                    }
                }
            }
            //Decimal point position of Effective Opening Area
            _EffectiveOpeningAreaRoundingDecimal = 3;
            if (dataAry.Count > 18)
            {
                sValue = dataAry[18];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < _DecimalMin || iValue > _DecimalMax)
                        {
                            iValue = 3;
                        }
                        _EffectiveOpeningAreaRoundingDecimal = iValue;
                    }
                }
            }
            // Effective Opening Area fraction type
            _EffectiveOpeningAreaRoundingOpt = 2;
            if (dataAry.Count > 19)
            {
                sValue = dataAry[19];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 2;
                        }
                        _EffectiveOpeningAreaRoundingOpt = iValue;
                    }
                }
            }
            //Decimal point position of Effective Lighting Area
            _EffectiveLightingAreaRoundingDecimal = 3;
            if (dataAry.Count > 20)
            {
                sValue = dataAry[20];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < _DecimalMin || iValue > _DecimalMax)
                        {
                            iValue = 3;
                        }
                        _EffectiveLightingAreaRoundingDecimal = iValue;
                    }
                }
            }
            // Effective Lighting Area fraction type
            _EffectiveLightingAreaRoundingOpt = 2;
            if (dataAry.Count > 21)
            {
                sValue = dataAry[21];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 2;
                        }
                        _EffectiveLightingAreaRoundingOpt = iValue;
                    }
                }
            }
            //Decimal point position of Area To Be Smoked
            _AreaToBeSmokedRoundingDecimal = 3;
            if (dataAry.Count > 22)
            {
                sValue = dataAry[22];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < _DecimalMin || iValue > _DecimalMax)
                        {
                            iValue = 3;
                        }
                        _AreaToBeSmokedRoundingDecimal = iValue;
                    }
                }
            }
            // Area To Be Smoked fraction type
            _AreaToBeSmokedRoundingOtp = 2;
            if (dataAry.Count > 23)
            {
                sValue = dataAry[23];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 2;
                        }
                        _AreaToBeSmokedRoundingOtp = iValue;
                    }
                }
            }
            //Decimal point position of Effective Smoke Extraction Area
            _EffectiveSmokeExtractionAreaRoundingDecimal = 3;
            if (dataAry.Count > 24)
            {
                sValue = dataAry[24];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < _DecimalMin || iValue > _DecimalMax)
                        {
                            iValue = 3;
                        }
                        _EffectiveSmokeExtractionAreaRoundingDecimal = iValue;
                    }
                }
            }
            // Effective Smoke Extraction Area fraction type
            _EffectiveSmokeExtractionAreaRoundingOtp = 2;
            if (dataAry.Count > 25)
            {
                sValue = dataAry[25];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 2;
                        }
                        _EffectiveSmokeExtractionAreaRoundingOtp = iValue;
                    }
                }
            }
            //Decimal point position of Area To Be Ventilated
            _AreaToBeVentilatedRoundingDecimal = 3;
            if (dataAry.Count > 26)
            {
                sValue = dataAry[26];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < _DecimalMin || iValue > _DecimalMax)
                        {
                            iValue = 3;
                        }
                        _AreaToBeVentilatedRoundingDecimal = iValue;
                    }
                }
            }
            //Area To Be Ventilated fraction type
            _AreaToBeVentilatedRoundingOtp = 2;
            if (dataAry.Count > 27)
            {
                sValue = dataAry[27];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 2;
                        }
                        _AreaToBeVentilatedRoundingOtp = iValue;
                    }
                }
            }
            //Decimal point position of Effective Ventilation Area
            _EffectiveVentilationAreaRoundingDecimal = 3;
            if (dataAry.Count > 28)
            {
                sValue = dataAry[28];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < _DecimalMin || iValue > _DecimalMax)
                        {
                            iValue = 3;
                        }
                        _EffectiveVentilationAreaRoundingDecimal = iValue;
                    }
                }
            }
            //Effective Ventilation Area fraction type
            _EffectiveVentilationAreaRoundingOtp = 2;
            if (dataAry.Count > 29)
            {
                sValue = dataAry[29];
                if ((sValue != null) && (sValue != ""))
                {
                    if (UtilValue.IsInteger(sValue) == true)
                    {
                        iValue = int.Parse(sValue);
                        if (iValue < 0)
                        {
                            iValue = 2;
                        }
                        _EffectiveVentilationAreaRoundingOtp = iValue;
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>コマンド値設定</summary>
        ///
        /// <param name="dataAry">コマンドデータ</param>
        /// <param name="isFNeed">bool form need get data</param>
        ///
        /// <history><p>2011/08/31 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/11/24 Created Modified Applied Technology</p><history>
        /// ================================================================================
        protected void SetCmdValue(ref Collections.Generic.IList<string> dataAry)
        {
            // 用途地域オプション
            if (dataAry.Count > 0)
            {
                dataAry[0] = _CvUseDistrictOpt.ToString();
            }

            // 見出し作成チェック
            if (dataAry.Count > 1)
            {
                dataAry[1] = _CvChkCreateHeader.ToString();
            }

            // 縁側
            if (dataAry.Count > 2)
            {
                dataAry[2] = _CvVeranda.ToString();
            }

            // 道路
            if (dataAry.Count > 3)
            {
                dataAry[3] = _CvRoadSide.ToString();
            }

            // 水平測定距離
            if (dataAry.Count > 4)
            {
                dataAry[4] = _CvHorizontalMeas.ToString();
            }

            // 水平補正距離
            if (dataAry.Count > 5)
            {
                dataAry[5] = _CvHorizontalCorr.ToString();
            }

            // 垂直測定距離
            if (dataAry.Count > 6)
            {
                dataAry[6] = _CvVerticalMeas.ToString();
            }

            // 垂直補正距離
            if (dataAry.Count > 7)
            {
                dataAry[7] = _CvVerticalCorr.ToString();
            }

            // 天端高さ
            if (dataAry.Count > 8)
            {
                dataAry[8] = _CvHeadHeight.ToString();
            }

            // 天井高さ
            if (dataAry.Count > 9)
            {
                dataAry[9] = _CvCeilingHeight.ToString();
            }

            // 防煙壁長さ
            if (dataAry.Count > 10)
            {
                dataAry[10] = _CvSmokeWallLength.ToString();
            }

            // 排煙有効高さ
            if (dataAry.Count > 11)
            {
                dataAry[11] = _CvUsableHeightSmoke.ToString();
            }

            //Decimal point position of legal area
            if (dataAry.Count > 12)
            {
                dataAry[12] = _LegalAreaRoundingDecimal.ToString();
            }
            //Legal area rounding type
            if (dataAry.Count > 13)
            {
                dataAry[13] = _LegalAreaRoundingOpt.ToString();
            }
            //Decimal point position of Area To Get Light
            if (dataAry.Count > 14)
            {
                dataAry[14] = _AreaToGetLightRoundingDecimal.ToString();
            }
            //Area To Get Light rounding type
            if (dataAry.Count > 15)
            {
                dataAry[15] = _AreaToGetLightRoundingOpt.ToString();
            }
            //Decimal point position of d/h
            if (dataAry.Count > 16)
            {
                dataAry[16] = _DHRoundingDecimal.ToString();
            }
            //d/h rounding type
            if (dataAry.Count > 17)
            {
                dataAry[17] = _DHRoundingOpt.ToString();
            }
            //Decimal point position of Effective Opening Area
            if (dataAry.Count > 18)
            {
                dataAry[18] = _EffectiveOpeningAreaRoundingDecimal.ToString();
            }
            //Effective Opening Area rounding type
            if (dataAry.Count > 19)
            {
                dataAry[19] = _EffectiveOpeningAreaRoundingOpt.ToString();
            }
            //Decimal point position of Effective Lighting Area
            if (dataAry.Count > 20)
            {
                dataAry[20] = _EffectiveLightingAreaRoundingDecimal.ToString();
            }
            //Effective Lighting Area rounding type
            if (dataAry.Count > 21)
            {
                dataAry[21] = _EffectiveLightingAreaRoundingOpt.ToString();
            }
            //Decimal point position of Area To Be Smoked
            if (dataAry.Count > 22)
            {
                dataAry[22] = _AreaToBeSmokedRoundingDecimal.ToString();
            }
            //Area To Be Smoked rounding type
            if (dataAry.Count > 23)
            {
                dataAry[23] = _AreaToBeSmokedRoundingOtp.ToString();
            }
            //Decimal point position of Effective Smoke Extraction Area
            if (dataAry.Count > 24)
            {
                dataAry[24] = _EffectiveSmokeExtractionAreaRoundingDecimal.ToString();
            }
            //Effective Smoke Extraction Area rounding type
            if (dataAry.Count > 25)
            {
                dataAry[25] = _EffectiveSmokeExtractionAreaRoundingOtp.ToString();
            }
            //Decimal point position of Area To Be Ventilated
            if (dataAry.Count > 26)
            {
                dataAry[26] = _AreaToBeVentilatedRoundingDecimal.ToString();
            }
            //Area To Be Ventilated rounding type
            if (dataAry.Count > 27)
            {
                dataAry[27] = _AreaToBeVentilatedRoundingOtp.ToString();
            }
            //Decimal point position of Effective Ventilation Area
            if (dataAry.Count > 28)
            {
                dataAry[28] = _EffectiveVentilationAreaRoundingDecimal.ToString();
            }
            //Effective Ventilation Area rounding type
            if (dataAry.Count > 29)
            {
                dataAry[29] = _EffectiveVentilationAreaRoundingOtp.ToString();
            }
        }

        /// ================================================================================
        /// <summary>テーブルデータ表示設定 - 部屋</summary>
        ///
        /// <param name="data"      >データ</param>
        /// <param name="levelName" >レベル名</param>
        /// <param name="groupName" >グループ名</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetVisbleRooms(System.Data.DataTable data, string levelName, string groupName)
        {
            string colName1 = ColNameLevelName;
            string colName2 = ColNameGroupName;
            string filterStr = colName1 + " = " + "'" + levelName + "'" + " AND " +
                                colName2 + " = " + "'" + groupName + "'";
            data.DefaultView.RowFilter = filterStr;
            SortDataRoom(data);
        }

        /// ================================================================================
        /// <summary>テーブルデータ表示設定 - 部屋</summary>
        ///
        /// <param name="data">データ</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetVisbleRooms(System.Data.DataTable data)
        {
            data.DefaultView.RowFilter = null;
            SortDataRoom(data);
        }

        /// ================================================================================
        /// <summary>テーブルデータ表示設定 - 部屋</summary>
        ///
        /// <param name="dgv"           >DataGridView</param>
        /// <param name="roomGroupName" >部屋グループ名</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetVisbleRooms(ref System.Windows.Forms.DataGridView dgv, string roomGroupName)
        {
            System.Data.DataTable dt = (System.Data.DataTable)dgv.DataSource;
            string colName = ColNameGroupName;
            string filterStr = "";
            filterStr = colName + " = " + "'" + roomGroupName + "'";
            dt.DefaultView.RowFilter = filterStr;
            SortDataRoom(dt);
        }

        /// ================================================================================
        /// <summary>テーブルデータ表示設定 - 建具</summary>
        ///
        /// <param name="data"  >データ</param>
        /// <param name="roomID">部屋ID</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetVisbleWinDoor(System.Data.DataTable data, int roomID)
        {
            string filterStr = "";
            string sRoomID = roomID.ToString();

            filterStr = ColNameAffiliationRoom + " = " + "'" + sRoomID + "'";
            data.DefaultView.RowFilter = filterStr;
            SortDataWinDoor(data);
        }

        /// ================================================================================
        /// <summary>テーブルデータをソート - 部屋</summary>
        ///
        /// <param name="dataView">データビュー</param>
        ///
        /// <history>2011/08/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SortDataRoom(System.Data.DataView dataView)
        {
            dataView.Sort = ColNameLevelName + " " + "ASC" + "," +
                            ColNameGroupName + " " + "ASC" + "," +
                            ColNameRoomName + " " + "ASC" + "," +
                            ColNameRoomNo + " " + "ASC";
        }

        /// ================================================================================
        /// <summary>テーブルデータをソート - 部屋(オーバーロード)</summary>
        ///
        /// <param name="dt">データテーブル</param>
        ///
        /// <history>2011/08/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SortDataRoom(System.Data.DataTable dt)
        {
            System.Data.DataView dataView = dt.DefaultView;
            SortDataRoom(dataView);
        }

        /// ================================================================================
        /// <summary>テーブルデータをソート - 建具</summary>
        ///
        /// <param name="dataView">データビュー</param>
        ///
        /// <history>2011/08/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SortDataWinDoor(System.Data.DataView dataView)
        {
            dataView.Sort = ColNameCategory + " " + "ASC" + "," +
                            ColNameSign + " " + "ASC";
        }

        /// ================================================================================
        /// <summary>テーブルデータをソート - 建具(オーバーロード)</summary>
        ///
        /// <param name="dt">データテーブル</param>
        ///
        /// <history>2011/08/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SortDataWinDoor(System.Data.DataTable dt)
        {
            System.Data.DataView dataView = dt.DefaultView;
            SortDataWinDoor(dataView);
        }

        /// ================================================================================
        /// <summary>数値エラー設定</summary>
        ///
        /// <param name="value">値</param>
        /// <param name="checkValue">bool check value </param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</p></returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string SetErrPvdNumeric(string value, bool checkValue)
        {
            string errMsg = "";

            // Chack blank
            // 空白チェック
            if (UtilValue.IsNull(value) == true)
            {
                errMsg = _CmpAttribute.ResourceText("IDS_ERR_VALNULL");
            }

            // Check numeric
            // 数値チェック
            if (errMsg == "")
            {
                if (UtilValue.IsNumber(value) == false)
                {
                    errMsg = _CmpAttribute.ResourceText("IDS_ERR_VALNUMERIC");
                }
            }
            if (checkValue)
            {
                //　値の範囲チェック
                if (errMsg == "")
                {
                    if (UtilValue.IsInteger(value) == false)
                    {
                        errMsg = _CmpAttribute.ResourceText("IDS_ERR_VALNUMERIC");
                    }

                    int iValue = int.Parse(value);

                    if ((iValue < DecimalMin) || (iValue > DecimalMax))
                    {
                        errMsg = _CmpAttribute.ResourceText("IDS_ERR_VALRANGE");
                    }
                }
            }
            return errMsg;
        }

        /// ================================================================================
        /// <summary>防煙壁長さのエラー設定</summary>
        ///
        /// <param name="value">値</param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</p></returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string SetErrPvdSmokeWallLength(string value)
        {
            string errMsg = "";

            errMsg = SetErrPvdNumeric(value, false);

            // Check value range
            // 値の範囲チェック
            if (errMsg == "")
            {
                double dValue = double.Parse(value);
                if (dValue < 0.5)
                {
                    errMsg = _CmpAttribute.ResourceText("IDS_ERR_VALSMOKEWALLLENGTH");
                }
            }

            return errMsg;
        }

        /// ================================================================================
        /// <summary>必要係数取得</summary>
        ///
        /// <param name="value">値</param>
        ///
        /// <returns>必要係数</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetNesCoeff(string value)
        {
            string ret = null;

            switch (_CommandKind)
            {
                case 0:
                    ret = UtilData.GetValueTableData(EntDtItems.RoomKind, "Name", value, "Value");
                    break;

                case 1:
                    ret = UtilData.GetValueTableData(EntDtItems.SmokeNesCoeff, "Name", "0", "Value");
                    break;

                case 2:
                    ret = UtilData.GetValueTableData(EntDtItems.VentilationNesCoeff, "Name", "0", "Value");
                    break;
            }
            if (ret == null)
            {
                ret = "-";
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>防煙壁長さのデフォルト値取得</summary>
        ///
        /// <returns>必要係数</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetDefaultSmokeWallLengthDefault()
        {
            string ret = null;

            ret = UtilData.GetValueTableData(EntDtItems.DefaultSmokeWallLength, "Name", "0", "Value");

            if (ret == null)
            {
                ret = "-";
            }
            else if (UtilValue.IsNumber(ret) == true)
            {
                double millimeters = double.Parse(ret);
                ret = CmpGeometry.FromMillimeters(millimeters).ToString(
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>判定設定</summary>
        ///
        /// <param name="dataRoom"    >データ - 部屋</param>
        /// <param name="dataWinDoor" >データ - 建具</param>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetJudgment(System.Data.DataTable dataRoom, System.Data.DataTable dataWinDoor)
        {
            if ((dataRoom == null) || (dataWinDoor == null))
            {
                return;
            }

            int rowCountRoom = dataRoom.Rows.Count;
            if (rowCountRoom == 0)
            {
                return;
            }

            int rowCountWinDoor = dataWinDoor.Rows.Count;
            if (rowCountWinDoor == 0)
            {
                return;
            }

            for (int i = 0; i < rowCountRoom; ++i)
            {
                string sNesArea = dataRoom.Rows[i][ColNameNecessaryArea].ToString();
                string sTotalUsableArea = dataRoom.Rows[i][ColNameTotalUsableArea].ToString();

                dataRoom.Rows[i][ColNameJudgment] = GetJudgment(sNesArea, sTotalUsableArea);
            }
        }

        /// ================================================================================
        /// <summary>判定取得</summary>
        ///
        /// <param name="nesArea"         >必要面積</param>
        /// <param name="totalUsableArea" >合計有効面積</param>
        ///
        /// <returns>判定</returns>
        ///
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string GetJudgment(string nesArea, string totalUsableArea)
        {
            string ret = "NG";

            int flag = 0;
            double dNesArea = 0.0;
            double dTotalUsableArea = 0.0;

            if (UtilValue.IsNumber(nesArea) == true)
            {
                dNesArea = double.Parse(nesArea);
                flag++;
            }

            if (UtilValue.IsNumber(totalUsableArea) == true)
            {
                dTotalUsableArea = double.Parse(totalUsableArea);
                flag++;
            }

            if (flag == 2)
            {
                if (dTotalUsableArea > dNesArea)
                {
                    ret = "OK";
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>値を初期化</summary>
        ///
        /// <param name="flag">Processing options</param>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public void Initvalue(int flag)
        {
            //legal area
            if (flag == 0 || flag == 1)
            {
                _LegalAreaRoundingDecimal = 3;
                _LegalAreaRoundingOpt = 2;
            }
            //Area to get light
            if (flag == 0 || flag == 2)
            {
                _AreaToGetLightRoundingDecimal = 3;
                _AreaToGetLightRoundingOpt = 2;
            }
            // d/h
            if (flag == 0 || flag == 3)
            {
                _DHRoundingDecimal = 3;
                _DHRoundingOpt = 2;
            }
            //Effective opening area
            if (flag == 0 || flag == 4)
            {
                _EffectiveOpeningAreaRoundingDecimal = 3;
                _EffectiveOpeningAreaRoundingOpt = 2;
            }
            //Effective light area
            if (flag == 0 || flag == 5)
            {
                _EffectiveLightingAreaRoundingDecimal = 3;
                _EffectiveLightingAreaRoundingOpt = 2;
            }
            //Area to be smoked
            if (flag == 0 || flag == 6)
            {
                _AreaToBeSmokedRoundingDecimal = 3;
                _AreaToBeSmokedRoundingOtp = 2;
            }
            //Effective smoke extraction area
            if (flag == 0 || flag == 7)
            {
                _EffectiveSmokeExtractionAreaRoundingDecimal = 3;
                _EffectiveSmokeExtractionAreaRoundingOtp = 2;
            }
            //Area to be ventilated
            if (flag == 0 || flag == 8)
            {
                _AreaToBeVentilatedRoundingDecimal = 3;
                _AreaToBeVentilatedRoundingOtp = 2;
            }
            //Effective ventilation area
            if (flag == 0 || flag == 9)
            {
                _EffectiveVentilationAreaRoundingDecimal = 3;
                _EffectiveVentilationAreaRoundingOtp = 2;
            }
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>属性</summary>
        /// <history>2015/12/14 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected RvtExtApp.Components.Attribute CmpAttribute
        {
            get
            {
                return _CmpAttribute;
            }
        }

        /// ================================================================================
        /// <summary>要素</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected RvtExtApp.Components.Elements CmpElements
        {
            get
            {
                return _CmpElements;
            }
        }

        /// ================================================================================
        /// <summary>図形</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected RvtExtApp.Components.Geometry CmpGeometry
        {
            get
            {
                return _CmpGeometry;
            }
        }

        /// ================================================================================
        /// <summary>パラメータ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected RvtExtApp.Components.Parameters CmpParameters
        {
            get
            {
                return _CmpParameters;
            }
        }

        /// ================================================================================
        /// <summary>設定</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected RvtExtApp.Components.Settings CmpSettings
        {
            get
            {
                return _CmpSettings;
            }
        }

        /// ================================================================================
        /// <summary>データテーブル - 項目</summary>
        /// <history>2011/01/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public RvtExtApp.Entities.DtItems EntDtItems
        {
            get
            {
                if (_EntDtItems == null)
                {
                    _EntDtItems = new RvtExtApp.Entities.DtItems(_CmpAttribute);
                }
                return _EntDtItems;
            }
        }

        /// ================================================================================
        /// <summary>エラーメッセージ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ErrMsg
        {
            get
            {
                return _ErrMsg;
            }
            set
            {
                _ErrMsg = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド種類</summary>
        /// <history>2011/08/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int CommandKind
        {
            get
            {
                return _CommandKind;
            }
            set
            {
                _CommandKind = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 用途地域オプション</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int CvUseDistrictOpt
        {
            get
            {
                return _CvUseDistrictOpt;
            }
            set
            {
                _CvUseDistrictOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 見出し作成チェック</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool CvChkCreateHeader
        {
            get
            {
                return _CvChkCreateHeader;
            }
            set
            {
                _CvChkCreateHeader = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 縁側</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool CvVeranda
        {
            get
            {
                return _CvVeranda;
            }
            set
            {
                _CvVeranda = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 道路</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool CvRoadSide
        {
            get
            {
                return _CvRoadSide;
            }
            set
            {
                _CvRoadSide = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 水平測定距離</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string CvHorizontalMeas
        {
            get
            {
                return _CvHorizontalMeas;
            }
            set
            {
                _CvHorizontalMeas = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 水平補正距離</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string CvHorizontalCorr
        {
            get
            {
                return _CvHorizontalCorr;
            }
            set
            {
                _CvHorizontalCorr = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 垂直測定距離</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string CvVerticalMeas
        {
            get
            {
                return _CvVerticalMeas;
            }
            set
            {
                _CvVerticalMeas = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 垂直補正距離</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string CvVerticalCorr
        {
            get
            {
                return _CvVerticalCorr;
            }
            set
            {
                _CvVerticalCorr = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 天端高さ</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string CvHeadHeight
        {
            get
            {
                return _CvHeadHeight;
            }
            set
            {
                _CvHeadHeight = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 天井高さ</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string CvCeilingHeight
        {
            get
            {
                return _CvCeilingHeight;
            }
            set
            {
                _CvCeilingHeight = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 防煙壁長さ</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string CvSmokeWallLength
        {
            get
            {
                return _CvSmokeWallLength;
            }
            set
            {
                _CvSmokeWallLength = value;
            }
        }

        /// ================================================================================
        /// <summary>コマンド値 - 排煙有効高さ</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string CvUsableHeightSmoke
        {
            get
            {
                return _CvUsableHeightSmoke;
            }
            set
            {
                _CvUsableHeightSmoke = value;
            }
        }

        /// ================================================================================
        /// <summary>列名 ID</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameID
        {
            get
            {
                if (_ColNameID == null)
                {
                    _ColNameID = _CmpAttribute.ResourceText("IDS_COLNAME_ID");
                }
                return _ColNameID;
            }
        }

        /// ================================================================================
        /// <summary>列名 レベル名</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameLevelName
        {
            get
            {
                if (_ColNameLevelName == null)
                {
                    _ColNameLevelName = _CmpAttribute.ResourceText("IDS_COLNAME_LEVELNAME");
                }
                return _ColNameLevelName;
            }
        }

        /// ================================================================================
        /// <summary>列名 グループ名</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameGroupName
        {
            get
            {
                if (_ColNameGroupName == null)
                {
                    _ColNameGroupName = _CmpAttribute.ResourceText("IDS_COLNAME_GROUPNAME");
                }
                return _ColNameGroupName;
            }
        }

        /// ================================================================================
        /// <summary>列名 部屋名</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameRoomName
        {
            get
            {
                if (_ColNameRoomName == null)
                {
                    _ColNameRoomName = _CmpAttribute.ResourceText("IDS_COLNAME_ROOMNAME");
                }
                return _ColNameRoomName;
            }
        }

        /// ================================================================================
        /// <summary>列名 部屋番号</summary>
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameRoomNo
        {
            get
            {
                if (_ColNameRoomNo == null)
                {
                    _ColNameRoomNo = _CmpAttribute.ResourceText("IDS_COLNAME_ROOMNO");
                }
                return _ColNameRoomNo;
            }
        }

        /// ================================================================================
        /// <summary>列名 面積</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameArea
        {
            get
            {
                if (_ColNameArea == null)
                {
                    _ColNameArea = _CmpAttribute.ResourceText("IDS_COLNAME_AREA");
                }
                return _ColNameArea;
            }
        }

        /// ================================================================================
        /// <summary>列名 必要係数</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameNecessaryCoefficient
        {
            get
            {
                if (_ColNameNecessaryCoefficient == null)
                {
                    _ColNameNecessaryCoefficient = _CmpAttribute.ResourceText("IDS_COLNAME_NECESSARYCOEFFICIENT");
                }
                return _ColNameNecessaryCoefficient;
            }
        }

        /// ================================================================================
        /// <summary>列名 平均天井高</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameAverageCeilingHeight
        {
            get
            {
                if (_ColNameAverageCeilingHeight == null)
                {
                    _ColNameAverageCeilingHeight = _CmpAttribute.ResourceText("IDS_COLNAME_AVERAGECEILINGHEIGHT");
                }
                return _ColNameAverageCeilingHeight;
            }
        }

        /// ================================================================================
        /// <summary>列名 必要面積</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameNecessaryArea
        {
            get
            {
                if (_ColNameNecessaryArea == null)
                {
                    _ColNameNecessaryArea = _CmpAttribute.ResourceText("IDS_COLNAME_NECESSARYAREA");
                }
                return _ColNameNecessaryArea;
            }
        }

        /// ================================================================================
        /// <summary>列名 合計有効面積</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameTotalUsableArea
        {
            get
            {
                if (_ColNameTotalUsableArea == null)
                {
                    _ColNameTotalUsableArea = _CmpAttribute.ResourceText("IDS_COLNAME_TOTALUSABLEAREA");
                }
                return _ColNameTotalUsableArea;
            }
        }

        /// ================================================================================
        /// <summary>列名 判定</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameJudgment
        {
            get
            {
                if (_ColNameJudgment == null)
                {
                    _ColNameJudgment = _CmpAttribute.ResourceText("IDS_COLNAME_JUDGMENT");
                }
                return _ColNameJudgment;
            }
        }

        /// ================================================================================
        /// <summary>列名 所属部屋</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameAffiliationRoom
        {
            get
            {
                if (_ColNameAffiliationRoom == null)
                {
                    _ColNameAffiliationRoom = _CmpAttribute.ResourceText("IDS_COLNAME_AFFILIATIONROOM");
                }
                return _ColNameAffiliationRoom;
            }
        }

        /// ================================================================================
        /// <summary>列名 符号</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameSign
        {
            get
            {
                if (_ColNameSign == null)
                {
                    _ColNameSign = _CmpAttribute.ResourceText("IDS_COLNAME_SIGN");
                }
                return _ColNameSign;
            }
        }

        /// ================================================================================
        /// <summary>列名 水平距離</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameHorizontalDist
        {
            get
            {
                if (_ColNameHorizontalDist == null)
                {
                    _ColNameHorizontalDist = _CmpAttribute.ResourceText("IDS_COLNAME_HORIZONTALDIST");
                }
                return _ColNameHorizontalDist;
            }
        }

        /// ================================================================================
        /// <summary>列名 垂直距離</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameVerticalDist
        {
            get
            {
                if (_ColNameVerticalDist == null)
                {
                    _ColNameVerticalDist = _CmpAttribute.ResourceText("IDS_COLNAME_VERTICALDIST");
                }
                return _ColNameVerticalDist;
            }
        }

        /// ================================================================================
        /// <summary>列名 d/h</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameDsH
        {
            get
            {
                if (_ColNameDsH == null)
                {
                    _ColNameDsH = _CmpAttribute.ResourceText("IDS_COLNAME_DSH");
                }
                return _ColNameDsH;
            }
        }

        /// ================================================================================
        /// <summary>列名 α</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameA
        {
            get
            {
                if (_ColNameA == null)
                {
                    _ColNameA = _CmpAttribute.ResourceText("IDS_COLNAME_A");
                }
                return _ColNameA;
            }
        }

        /// ================================================================================
        /// <summary>列名 β</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameB
        {
            get
            {
                if (_ColNameB == null)
                {
                    _ColNameB = _CmpAttribute.ResourceText("IDS_COLNAME_B");
                }
                return _ColNameB;
            }
        }

        /// ================================================================================
        /// <summary>列名 D</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameD
        {
            get
            {
                if (_ColNameD == null)
                {
                    _ColNameD = _CmpAttribute.ResourceText("IDS_COLNAME_D");
                }
                return _ColNameD;
            }
        }

        /// ================================================================================
        /// <summary>列名 A(仮)</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameATemp
        {
            get
            {
                if (_ColNameATemp == null)
                {
                    _ColNameATemp = _CmpAttribute.ResourceText("IDS_COLNAME_ATEMP");
                }
                return _ColNameATemp;
            }
        }

        /// ================================================================================
        /// <summary>列名 A(補正値)</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameACorr
        {
            get
            {
                if (_ColNameACorr == null)
                {
                    _ColNameACorr = _CmpAttribute.ResourceText("IDS_COLNAME_ACORR");
                }
                return _ColNameACorr;
            }
        }

        /// ================================================================================
        /// <summary>列名 開口係数</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameOpenCoefficient
        {
            get
            {
                if (_ColNameOpenCoefficient == null)
                {
                    _ColNameOpenCoefficient = _CmpAttribute.ResourceText("IDS_COLNAME_OPENCOEFFICIENT");
                }
                return _ColNameOpenCoefficient;
            }
        }

        /// ================================================================================
        /// <summary>列名 有効幅</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameUsableWidth
        {
            get
            {
                if (_ColNameUsableWidth == null)
                {
                    _ColNameUsableWidth = _CmpAttribute.ResourceText("IDS_COLNAME_USABLEWIDTH");
                }
                return _ColNameUsableWidth;
            }
        }

        /// ================================================================================
        /// <summary>列名 有効高さ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameUsableHeight
        {
            get
            {
                if (_ColNameUsableHeight == null)
                {
                    _ColNameUsableHeight = _CmpAttribute.ResourceText("IDS_COLNAME_USABLEHEIGHT");
                }
                return _ColNameUsableHeight;
            }
        }

        /// ================================================================================
        /// <summary>列名 排煙有効高さ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameUsableHeightSmoke
        {
            get
            {
                if (_ColNameUsableHeightSmoke == null)
                {
                    _ColNameUsableHeightSmoke = _CmpAttribute.ResourceText("IDS_COLNAME_USABLEHEIGHTSMOKE");
                }
                return _ColNameUsableHeightSmoke;
            }
        }

        /// ================================================================================
        /// <summary>列名 有効開口面積</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameUsableOpenArea
        {
            get
            {
                if (_ColNameUsableOpenArea == null)
                {
                    _ColNameUsableOpenArea = _CmpAttribute.ResourceText("IDS_COLNAME_USABLEOPENAREA");
                }
                return _ColNameUsableOpenArea;
            }
        }

        /// ================================================================================
        /// <summary>列名 有効面積</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameUsableArea
        {
            get
            {
                if (_ColNameUsableArea == null)
                {
                    _ColNameUsableArea = _CmpAttribute.ResourceText("IDS_COLNAME_USABLEAREA");
                }
                return _ColNameUsableArea;
            }
        }

        /// ================================================================================
        /// <summary>列名 カテゴリ</summary>
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameCategory
        {
            get
            {
                if (_ColNameCategory == null)
                {
                    _ColNameCategory = _CmpAttribute.ResourceText("IDS_COLNAME_CATEGORY");
                }
                return _ColNameCategory;
            }
        }

        /// ================================================================================
        /// <summary>列名 排煙窓幅</summary>
        /// <history>2011/09/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameSmokeWinWidth
        {
            get
            {
                if (_ColNameSmokeWinWidth == null)
                {
                    _ColNameSmokeWinWidth = _CmpAttribute.ResourceText("IDS_COLNAME_SMOKEWINWIDTH");
                }
                return _ColNameSmokeWinWidth;
            }
        }

        /// ================================================================================
        /// <summary>列名 排煙窓高さ</summary>
        /// <history>2011/09/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ColNameSmokeWinHeight
        {
            get
            {
                if (_ColNameSmokeWinHeight == null)
                {
                    _ColNameSmokeWinHeight = _CmpAttribute.ResourceText("IDS_COLNAME_SMOKEWINHEIGHT");
                }
                return _ColNameSmokeWinHeight;
            }
        }

        /// ================================================================================
        /// <summary>Legal area rounding type</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int LegalAreaRoundingOpt
        {
            get
            {
                return _LegalAreaRoundingOpt;
            }
            set
            {
                _LegalAreaRoundingOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>Decimal point position of legal area</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int LegalAreaRoundingDecimal
        {
            get
            {
                return _LegalAreaRoundingDecimal;
            }
            set
            {
                _LegalAreaRoundingDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>Area To Get Light rounding type</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int AreaToGetLightRoundingOpt
        {
            get
            {
                return _AreaToGetLightRoundingOpt;
            }
            set
            {
                _AreaToGetLightRoundingOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>Decimal point position of Area To Get Light</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int AreaToGetLightRoundingDecimal
        {
            get
            {
                return _AreaToGetLightRoundingDecimal;
            }
            set
            {
                _AreaToGetLightRoundingDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>d/h・A(仮)・A(補正値) rounding type</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int DHRoundingOpt
        {
            get
            {
                return _DHRoundingOpt;
            }
            set
            {
                _DHRoundingOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>Decimal point position of d/h・A(仮)・A(補正値)</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int DHRoundingDecimal
        {
            get
            {
                return _DHRoundingDecimal;
            }
            set
            {
                _DHRoundingDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>Effective Opening Area rounding type</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int EffectiveOpeningAreaRoundingOpt
        {
            get
            {
                return _EffectiveOpeningAreaRoundingOpt;
            }
            set
            {
                _EffectiveOpeningAreaRoundingOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>Decimal point position of Effective Opening Area</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int EffectiveOpeningAreaRoundingDecimal
        {
            get
            {
                return _EffectiveOpeningAreaRoundingDecimal;
            }
            set
            {
                _EffectiveOpeningAreaRoundingDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>Effective Lighting Area rounding type</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int EffectiveLightingAreaRoundingOpt
        {
            get
            {
                return _EffectiveLightingAreaRoundingOpt;
            }
            set
            {
                _EffectiveLightingAreaRoundingOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>Decimal point position of Effective Lighting Area</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int EffectiveLightingAreaRoundingDecimal
        {
            get
            {
                return _EffectiveLightingAreaRoundingDecimal;
            }
            set
            {
                _EffectiveLightingAreaRoundingDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>Area To Be Smoked rounding type</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int AreaToBeSmokedRoundingOtp
        {
            get
            {
                return _AreaToBeSmokedRoundingOtp;
            }
            set
            {
                _AreaToBeSmokedRoundingOtp = value;
            }
        }

        /// ================================================================================
        /// <summary>Decimal point position of Area To Be Smoked</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int AreaToBeSmokedRoundingDecimal
        {
            get
            {
                return _AreaToBeSmokedRoundingDecimal;
            }
            set
            {
                _AreaToBeSmokedRoundingDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>Effective Smoke Extraction Area rounding type</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int EffectiveSmokeExtractionAreaRoundingOtp
        {
            get
            {
                return _EffectiveSmokeExtractionAreaRoundingOtp;
            }
            set
            {
                _EffectiveSmokeExtractionAreaRoundingOtp = value;
            }
        }

        /// ================================================================================
        /// <summary>Decimal point position of Effective Smoke Extraction Area</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int EffectiveSmokeExtractionAreaRoundingDecimal
        {
            get
            {
                return _EffectiveSmokeExtractionAreaRoundingDecimal;
            }
            set
            {
                _EffectiveSmokeExtractionAreaRoundingDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>Area To Be Ventilated rounding type</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int AreaToBeVentilatedRoundingOtp
        {
            get
            {
                return _AreaToBeVentilatedRoundingOtp;
            }
            set
            {
                _AreaToBeVentilatedRoundingOtp = value;
            }
        }

        /// ================================================================================
        /// <summary>Decimal point position of Area To Be Ventilated</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int AreaToBeVentilatedRoundingDecimal
        {
            get
            {
                return _AreaToBeVentilatedRoundingDecimal;
            }
            set
            {
                _AreaToBeVentilatedRoundingDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>Effective Ventilation Area rounding type</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int EffectiveVentilationAreaRoundingOtp
        {
            get
            {
                return _EffectiveVentilationAreaRoundingOtp;
            }
            set
            {
                _EffectiveVentilationAreaRoundingOtp = value;
            }
        }

        /// ================================================================================
        /// <summary>Decimal point position of Effective Ventilation Area</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int EffectiveVentilationAreaRoundingDecimal
        {
            get
            {
                return _EffectiveVentilationAreaRoundingDecimal;
            }
            set
            {
                _EffectiveVentilationAreaRoundingDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>小数点桁数の最小値</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int DecimalMin
        {
            get
            {
                return _DecimalMin;
            }
        }

        /// ================================================================================
        /// <summary>小数点桁数の最大値</summary>
        /// <history>2021/11/24 Created Applied Technology</history>
        /// ================================================================================
        public int DecimalMax
        {
            get
            {
                return _DecimalMax;
            }
        }

        #endregion Properties
    }
}