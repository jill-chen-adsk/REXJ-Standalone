using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;
using ADSK.JExtRAC.AreaSchedule.Utils;

namespace ADSK.JExtRAC.AreaSchedule.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - エリア</summary>
    /// ================================================================================
    public class DtArea : RvtExtApp.Entities.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>共有パラメータ</summary>
        private RvtExtApp.Entities.SpArea _EntSpArea;

        /// <summary>エリアタグ追加チェック</summary>
        private bool _ChkAddAreaTag;

        /// <summary>エリアタグデータ</summary>
        private System.Data.DataTable _DataAreaTags;

        /// <summary>タグ要素ID</summary>
        private int _TagID;

        /// <summary>タグ名オプション</summary>
        private int _TagNameOpt;

        /// <summary>長さの小数点位置</summary>
        private int _LengthDecimal;

        /// <summary>面積の小数点位置</summary>
        private int _AreaDecimal;

        /// <summary>長さの端数処理タイプ</summary>
        private int _LengthRoundingOpt;

        /// <summary>面積の端数処理タイプ</summary>
        private int _AreaRoundingOpt;

        /// <summary>長さの単位</summary>
        private int _LengthUnit;

        /// <summary>PIタイプ</summary>
        private int _PiOpt;

        /// <summary>Piデータ</summary>
        private System.Data.DataTable _DataPI;

        /// <summary>小数点桁数の最小値</summary>
        private int _DecimalMin;

        /// <summary>小数点桁数の最大値</summary>
        private int _DecimalMax;

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
        /// <history>2011/08/01 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtArea(RvtExtApp.Components.Attribute cmpAttribute,
                      RvtExtApp.Components.Elements cmpElements,
                      RvtExtApp.Components.Geometry cmpGeometry,
                      RvtExtApp.Components.Parameters cmpParameters,
                      RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ
            _EntSpArea = new RvtExtApp.Entities.SpArea(cmpAttribute, cmpParameters, cmpSettings);
            if (_EntSpArea.DefSuccess == false)
            {
                string strCategory = base.CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = base.CmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = base.CmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpArea.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpArea.ErrDefName + "]";
            }

            // 初期化
            _ChkAddAreaTag = false;
            _DataAreaTags = base.CmpElements.TableAreaTag;
            _TagID = -1;
            _TagNameOpt = 0;

            Initvalue(0, true);
            _PiOpt = 0;

            _DataPI = new System.Data.DataTable();
            _DataPI.Columns.Add("ID", typeof(int));
            _DataPI.Columns.Add("NAME", typeof(string));
            base.CmpSettings.GetPIData(4, 6, ref _DataPI);

            _DecimalMin = 1;
            _DecimalMax = 9;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ取得 - 部屋をエリアに変換</summary>
        ///
        /// <param name="chkAddAreaTag" >エリアタグ追加チェック</param>
        /// <param name="tagID"         >タグ要素ID</param>
        /// <param name="tagNameOpt"    >タグ名オプション</param>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetDataRoomConvertedToArea(string chkAddAreaTag, string tagID, string tagNameOpt)
        {
            if ((chkAddAreaTag != null) && (chkAddAreaTag != ""))
            {
                _ChkAddAreaTag = Convert.ToBoolean(Byte.Parse(chkAddAreaTag));
            }

            if ((tagID != null) && (tagID != ""))
            {
                _TagID = int.Parse(tagID);
            }

            if ((tagNameOpt != null) && (tagNameOpt != ""))
            {
                _TagNameOpt = int.Parse(tagNameOpt);
            }
        }

        /// ================================================================================
        /// <summary>データ取得 - 根拠式</summary>
        ///
        /// <param name="lengthDecimal"     >長さの小数点位置</param>
        /// <param name="areaDecimal"       >面積の小数点位置</param>
        /// <param name="lengthRoundingOpt" >長さの端数処理タイプ</param>
        /// <param name="areaRoundingOpt"   >面積の端数処理タイプ</param>
        /// <param name="piOpt"             >PIタイプ</param>
        /// <param name="lengthUnit"        >長さの単位</param>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetDataGroundsExpression(string lengthDecimal,
                                      string areaDecimal,
                                      string lengthRoundingOpt,
                                      string areaRoundingOpt,
                                      string piOpt,
                                      string lengthUnit)
        {
            if (!string.IsNullOrEmpty(lengthDecimal))
            {
                _LengthDecimal = int.Parse(lengthDecimal);
            }

            if (!string.IsNullOrEmpty(areaDecimal))
            {
                _AreaDecimal = int.Parse(areaDecimal);
            }

            if (!string.IsNullOrEmpty(lengthRoundingOpt))
            {
                _LengthRoundingOpt = int.Parse(lengthRoundingOpt);
            }

            if (!string.IsNullOrEmpty(areaRoundingOpt))
            {
                _AreaRoundingOpt = int.Parse(areaRoundingOpt);
            }

            if (!string.IsNullOrEmpty(piOpt))
            {
                _PiOpt = int.Parse(piOpt);
            }

            if (!string.IsNullOrEmpty(lengthUnit))
            {
                _LengthUnit = int.Parse(lengthUnit);
            }
        }

        /// ================================================================================
        /// <summary>値を初期化</summary>
        ///
        /// <param name="flag"        ><p>処理オプション</p>
        ///                               <p>0=全て初期化</p>
        ///                               <p>1=長さのみ初期化</p>
        ///                               <p>2=面積のみ初期化</p></param>
        /// <param name="isLenUnitMM" >長さ単位がmm</param>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void Initvalue(int flag, bool isLenUnitMM)
        {
            if ((flag == 0) || (flag == 1))
            {
                _LengthUnit = 0;
                _LengthDecimal = 3;

                // 長さ単位がmm
                if (isLenUnitMM == true)
                {
                    _LengthDecimal = 1;
                }

                _LengthRoundingOpt = 0;
            }
           
            if ((flag == 0) || (flag == 2))
            {
                _AreaDecimal = 3;
                _AreaRoundingOpt = 0;
            }
        }

        /// ================================================================================
        /// <summary>小数点桁数のエラー設定</summary>
        ///
        /// <param name="value">値</param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</p></returns>
        ///
        /// <history><p>2011/08/02 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2011/11/24 Modifed Applied Techbology</p><history>
        /// ================================================================================
        public string SetErrPvdDecimalText(string value, bool check1, bool check2, bool isLenUnitMM)
        {
            string errMsg = "";

            // 空白チェック
            if (UtilValue.IsNull(value) == true)
            {
                errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALNULL");
            }

            // 整数チェック
            if (errMsg == "")
            {
                if (UtilValue.IsInteger(value) == false)
                {
                    errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALNUMBER");
                }
            }

            if (check1)
            {
                //　値の範囲チェック
                if (errMsg == "")
                {
                    if (check2)
                    {
                        int iValue = int.Parse(value);
                        if (!isLenUnitMM)
                        {
                            if ((iValue < DecimalMin) || (iValue > DecimalMax - 7))
                            {
                                errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALRANGE");
                            }
                        }
                        else
                        {
                            if ((iValue < DecimalMin) || (iValue > DecimalMax - 4))
                            {
                                errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALRANGE");
                            }
                        }
                    }
                }
            }
            else
            {
                //　値の範囲チェック
                if (errMsg == "")
                {
                    int iValue = int.Parse(value);
                    if ((iValue < DecimalMin) || (iValue > DecimalMax))
                    {
                        errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALRANGE");
                    }
                }
            }

            return errMsg;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>エリアタグ追加チェック</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool ChkAddAreaTag
        {
            get
            {
                return _ChkAddAreaTag;
            }
            set
            {
                _ChkAddAreaTag = value;
            }
        }

        /// ================================================================================
        /// <summary>エリアタグデータ</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public System.Data.DataTable DataAreaTags
        {
            get
            {
                return _DataAreaTags;
            }
        }

        /// ================================================================================
        /// <summary>タグ名オプション</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int TagNameOpt
        {
            get
            {
                return _TagNameOpt;
            }
            set
            {
                _TagNameOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>タグ要素ID</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int TagID
        {
            get
            {
                return _TagID;
            }
            set
            {
                _TagID = value;
            }
        }

        /// ================================================================================
        /// <summary>共有パラメータ</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public RvtExtApp.Entities.SpArea EntSpArea
        {
            get
            {
                return _EntSpArea;
            }
        }

        /// ================================================================================
        /// <summary>長さの小数点位置</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int LengthDecimal
        {
            get
            {
                return _LengthDecimal;
            }
            set
            {
                _LengthDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>面積の小数点位置</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int AreaDecimal
        {
            get
            {
                return _AreaDecimal;
            }
            set
            {
                _AreaDecimal = value;
            }
        }

        /// ================================================================================
        /// <summary>長さの端数処理タイプ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int LengthRoundingOpt
        {
            get
            {
                return _LengthRoundingOpt;
            }
            set
            {
                _LengthRoundingOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>面積の端数処理タイプ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int AreaRoundingOpt
        {
            get
            {
                return _AreaRoundingOpt;
            }
            set
            {
                _AreaRoundingOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>長さの単位</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int LengthUnit
        {
            get
            {
                return _LengthUnit;
            }
            set
            {
                _LengthUnit = value;
            }
        }

        /// ================================================================================
        /// <summary>PIタイプ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int PiOpt
        {
            get
            {
                return _PiOpt;
            }
            set
            {
                _PiOpt = value;
            }
        }

        /// ================================================================================
        /// <summary>Piデータ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public System.Data.DataTable DataPI
        {
            get
            {
                return _DataPI;
            }
        }

        /// ================================================================================
        /// <summary>小数点桁数の最小値</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
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
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
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