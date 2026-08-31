using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AveSiteLevelHeightCalc;
using ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 注釈</summary>
    /// ================================================================================
    public class DtAnnotation : RvtExtApp.Entities.DtBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>共有パラメータ</summary>
        private RvtExtApp.Entities.SpAnnotation _EntSpAnnotation;

        /// <summary>平均地盤面算定ポイント</summary>
        private Collections.Generic.IList<ObjectTag> _AveGlLvlCalcPoss;

        /// <summary>平均地盤面算定ポイントテーブルデータ</summary>
        private System.Data.DataTable _TableAveGlLvlCalcPos;

        /// <summary>削除平均地盤面算定ポイントテーブルデータ</summary>
        private System.Data.DataTable _TableDelAveGlLvlCalcPos;

        /// <summary>BM高さ</summary>
        private double _BMHeight;

        /// <summary>縮尺</summary>
        private int _Scale;

        /// <summary>横の比</summary>
        private int _RaiteHorizontal;

        /// <summary>縦の比</summary>
        private int _RaiteVertical;

        /// <summary>面積の小数点位置</summary>
        private int _AreaDecimal;

        /// <summary>面積の端数処理タイプ</summary>
        private int _AreaRoundingOpt;

        /// <summary>長さの単位</summary>
        private int _LengthUnit;

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
        /// <param name="cmpAttribute"    >属性</param>
        /// <param name="cmpElements"     >要素</param>
        /// <param name="cmpGeometry"     >図形</param>
        /// <param name="cmpParameters"   >パラメータ</param>
        /// <param name="cmpSettings"     >設定</param>
        /// <param name="aveGlLvlCalcPoss">平均地盤面算定ポイント</param>
        /// <param name="flagNewCalcPoss" ><p>平均地盤面算定ポイント作成フラグ</p>
        ///                                   <p>True  = 新規作成</p>
        ///                                   <p>False = 既存</p></param>
        ///
        /// <history>2011/08/07 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtAnnotation(RvtExtApp.Components.Attribute cmpAttribute,
                            RvtExtApp.Components.Elements cmpElements,
                            RvtExtApp.Components.Geometry cmpGeometry,
                            RvtExtApp.Components.Parameters cmpParameters,
                            RvtExtApp.Components.Settings cmpSettings,
                            Collections.Generic.IList<ObjectTag> aveGlLvlCalcPoss,
                            bool flagNewCalcPoss) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ
            _EntSpAnnotation = new RvtExtApp.Entities.SpAnnotation(cmpAttribute, cmpParameters, cmpSettings);
            if (_EntSpAnnotation.DefSuccess == false)
            {
                string strCategory = base.CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = base.CmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = base.CmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpAnnotation.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpAnnotation.ErrDefName + "]";
            }

            // 初期化
            _AveGlLvlCalcPoss = aveGlLvlCalcPoss;

            _BMHeight = 0.0;
            _Scale = 200;
            _RaiteHorizontal = 1;
            _RaiteVertical = 1;

            _DecimalMin = 1;
            _DecimalMax = 4;

            _AreaDecimal = 3;
            _AreaRoundingOpt = 0;
            _LengthUnit = 0;

            _TableDelAveGlLvlCalcPos = new System.Data.DataTable();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>値を取得</summary>
        ///
        /// <param name="flagNewCalcPoss" ><p>平均地盤面算定ポイント作成フラグ</p>
        ///                                   <p>True  = 新規作成</p>
        ///                                   <p>False = 既存</p></param>
        /// <param name="bmHeight"        >BM高さ</param>
        /// <param name="scale"           >縮尺</param>
        /// <param name="raiteHorizontal" >横の比</param>
        /// <param name="raiteVertical"   >縦の比</param>
        /// <param name="areaDecimal"     >面積の小数点位置</param>
        /// <param name="areaRoundingOpt" >面積の端数処理タイプ</param>
        /// <param name="lengthUnit"      >長さの単位</param>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetValue(bool flagNewCalcPoss,
                      string bmHeight,
                      string scale,
                      string raiteHorizontal,
                      string raiteVertical,
                      string areaDecimal,
                      string areaRoundingOpt,
                      string lengthUnit)
        {
            if (flagNewCalcPoss == true)
            {
                _BMHeight = 0;
            }
            else
            {
                if ((bmHeight != null) && (bmHeight != ""))
                {
                    _BMHeight = double.Parse(bmHeight);
                }
            }

            if ((scale != null) && (scale != ""))
            {
                _Scale = int.Parse(scale);
            }

            if ((raiteHorizontal != null) && (raiteHorizontal != ""))
            {
                _RaiteHorizontal = int.Parse(raiteHorizontal);
            }

            if ((raiteVertical != null) && (raiteVertical != ""))
            {
                _RaiteVertical = int.Parse(raiteVertical);
            }

            if ((areaDecimal != null) && (areaDecimal != ""))
            {
                _AreaDecimal = int.Parse(areaDecimal);
            }

            if ((areaRoundingOpt != null) && (areaRoundingOpt != ""))
            {
                _AreaRoundingOpt = int.Parse(areaRoundingOpt);
            }

            if ((lengthUnit != null) && (lengthUnit != ""))
            {
                _LengthUnit = int.Parse(lengthUnit);
            }
        }

        /// ================================================================================
        /// <summary>平均地盤面算定ポイントのパラメータの値を設定</p></summary>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SetParamValueAveGlLvlCalcPos()
        {
            string sValue = "";
            int iValue = 0;
            double dValue = 0.0;
            double unitCoe = base.CmpGeometry.UnitCoe;

            if ((_TableAveGlLvlCalcPos != null) && (_TableAveGlLvlCalcPos.Rows.Count > 0))
            {
                for (int i = 0; i < _TableAveGlLvlCalcPos.Rows.Count; ++i)
                {
                    int idCircle = 0, idTag = 0;

                    // Id circle
                    sValue = _TableAveGlLvlCalcPos.Rows[i]["IDCircle"].ToString();
                    if (UtilValue.IsInteger(sValue) == true)
                        idCircle = int.Parse(sValue);

                    // Id tag
                    sValue = _TableAveGlLvlCalcPos.Rows[i]["IDTag"].ToString();
                    if (UtilValue.IsInteger(sValue) == true)
                        idTag = int.Parse(sValue);

                    Revit.DB.FamilyInstance fmCircle = base.CmpElements.GetAveGlLvlCalcPos(idCircle) as Revit.DB.FamilyInstance;
                    Revit.DB.IndependentTag tag = base.CmpElements.GetAveGlLvlCalcPos(idTag) as Revit.DB.IndependentTag;
                    if (fmCircle == null || tag == null)
                        continue;

                    _EntSpAnnotation.CurrentCircle = fmCircle;
                    _EntSpAnnotation.CurrentTag = tag;

                    // 番号
                    iValue = int.Parse(_TableAveGlLvlCalcPos.Rows[i]["Number"].ToString());
                    _EntSpAnnotation.AveGlLvlCalcPosCircleNo = iValue;

                    // レベル
                    dValue = double.Parse(_TableAveGlLvlCalcPos.Rows[i]["Level"].ToString());
                    _EntSpAnnotation.AveGlLvlCalcPosLevel = dValue / unitCoe;
                }
            }
        }

        /// ================================================================================
        /// <summary>値のエラー設定</summary>
        ///
        /// <param name="value"     >値</param>
        /// <param name="valueType" ><p>値型</p>
        ///                             <p>0=整数</p>
        ///                             <p>1=実数</p></param>
        /// <param name="mode"      ><p>モード</p>
        ///                             <p>0=制限なし</p>
        ///                             <p>1=正(0含まない)</p>
        ///                             <p>2=負(0含む)</p></param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</p></returns>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string SetErrPvdValue(string value, int valueType, int mode)
        {
            string errMsg = "";

            // 空白チェック
            if (UtilValue.IsNull(value) == true)
            {
                errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALNULL");
            }

            // 整数チェック
            if (valueType == 0)
            {
                if (errMsg == "")
                {
                    if (UtilValue.IsInteger(value) == false)
                    {
                        errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALNUMBER");
                    }
                }
            }

            // 実数チェック
            if (valueType == 1)
            {
                if (errMsg == "")
                {
                    if (UtilValue.IsNumber(value) == false)
                    {
                        errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALNUMERIC");
                    }
                }
            }

            //　値の範囲チェック - 整数
            if (valueType == 0)
            {
                if (errMsg == "")
                {
                    int iValue = int.Parse(value);
                    if (mode == 1)
                    {
                        if (iValue < 1)
                        {
                            errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALLARGE0");
                        }
                    }
                    else if (mode == 2)
                    {
                        if (iValue > 0)
                        {
                            errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALLES0");
                        }
                    }
                }
            }

            //　値の範囲チェック - 実数
            if (valueType == 1)
            {
                if (errMsg == "")
                {
                    double dValue = double.Parse(value);
                    if (mode == 1)
                    {
                        if (dValue <= 0.0)
                        {
                            errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALLARGE0");
                        }
                    }
                    else if (mode == 2)
                    {
                        if (dValue > 0.0)
                        {
                            errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALLES0");
                        }
                    }
                }
            }
            return errMsg;
        }

        /// ================================================================================
        /// <summary>小数点桁数のエラー設定</summary>
        ///
        /// <param name="value">値</param>
        ///
        /// <returns><p>エラー文字列</p>
        ///             <p>空白時はエラーなし</p></returns>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string SetErrPvdDecimalText(string value)
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

            //　値の範囲チェック
            if (errMsg == "")
            {
                int iValue = int.Parse(value);
                if ((iValue < _DecimalMin) || (iValue > _DecimalMax))
                {
                    errMsg = base.CmpAttribute.ResourceText("IDS_ERR_VALRANGE");
                }
            }

            return errMsg;
        }

        /// ================================================================================
        /// <summary>テーブルデータ行の上下移動</summary>
        ///
        /// <param name="selectIndex" >選択されたインデックス</param>
        /// <param name="upFlag"      >上方向</param>
        ///
        /// <returns>移動行データ</returns>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public System.Object UpDnViewTable(int selectIndex, bool upFlag)
        {
            return DataTableRowUtil.UpDnDataTableRow(ref _TableAveGlLvlCalcPos, selectIndex, upFlag);
        }

        /// ================================================================================
        /// <summary>テーブルデータの行削除</summary>
        ///
        /// <param name="selectIndex">選択されたインデックス</param>
        ///
        /// <returns>削除行データ</returns>
        ///
        /// <history>2011/08/07 Modified GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public System.Object DelViewTable(int selectIndex)
        {
            if (_TableDelAveGlLvlCalcPos.Rows.Count == 0)
            {
                _TableDelAveGlLvlCalcPos = _TableAveGlLvlCalcPos.Clone();
            }
            System.Object retObj = DataTableRowUtil.MoveListDataTableRow(ref _TableAveGlLvlCalcPos, selectIndex, ref _TableDelAveGlLvlCalcPos);
            return retObj;
        }

        /// ================================================================================
        /// <summary>テーブルデータの番号更新</summary>
        ///
        /// <history>2011/08/07 Modified GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void UpdateNumberViewTable()
        {
            if (_TableAveGlLvlCalcPos.Rows.Count > 0)
            {
                for (int i = 0; i < _TableAveGlLvlCalcPos.Rows.Count; ++i)
                {
                    // 番号
                    _TableAveGlLvlCalcPos.Rows[i]["Number"] = i + 1;
                }
            }
        }

        /// ================================================================================
        /// <summary>テーブルデータのレベル更新</summary>
        ///
        /// <param name="bmLevelNew">新しいBMレベル</param>
        /// <param name="bmLevelOld">古いBMレベル</param>
        ///
        /// <history><p>2011/08/07 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2015/09/11 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public void UpdateLevelViewTable(string bmLevelNew, string bmLevelOld)
        {
            if (_TableAveGlLvlCalcPos.Rows.Count > 0)
            {
                for (int i = 0; i < _TableAveGlLvlCalcPos.Rows.Count; ++i)
                {
                    // レベル
                    double level = double.Parse(_TableAveGlLvlCalcPos.Rows[i]["Level"].ToString());

                    // 値
                    double iBmLevelNew = double.Parse(bmLevelNew);
                    double iBmLevelOld = double.Parse(bmLevelOld);
                    double diff = iBmLevelOld - iBmLevelNew;
                    level += diff;
                    //_TableAveGlLvlCalcPos.Rows[i]["Level"] = level.ToString();
                    _TableAveGlLvlCalcPos.Rows[i]["Level"] = CmpParameters.StrZeroPadding(UtilValue.Rounding(level, 5, 2), 4);
                }
            }
        }

        /// ================================================================================
        /// <summary>平均地盤面算定ポイントテーブルデータをソート</summary>
        ///
        /// <param name="dataTable">ソート前のデータテーブル</param>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void SortTableAveGlLvlCalcPos(System.Data.DataTable dataTable)
        {
            _TableAveGlLvlCalcPos = new System.Data.DataTable();

            // Numberでソート
            string sortOrder = "Number";
            System.Data.DataRow[] dataTableRows = dataTable.Select(null, sortOrder);
            _TableAveGlLvlCalcPos = dataTable.Clone();

            foreach (System.Data.DataRow row in dataTableRows)
            {
                _TableAveGlLvlCalcPos.ImportRow(row);
            }
        }

        /// ================================================================================
        /// <summary>平均地盤面算定ポイント削除</summary>
        ///
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void DelAveGlLevelCalcPos()
        {
            Collections.Generic.IList<ObjectTag> delAveGlLvlCalcPoss = new Collections.Generic.List<ObjectTag>();
            if ((_TableDelAveGlLvlCalcPos != null) && (_TableDelAveGlLvlCalcPos.Rows.Count > 0))
            {
                for (int i = 0; i < _TableDelAveGlLvlCalcPos.Rows.Count; ++i)
                {
                    int idCircle = 0, idTag = 0;

                    string sValue = _TableDelAveGlLvlCalcPos.Rows[i]["IDCircle"].ToString();
                    if (UtilValue.IsInteger(sValue) == true)
                        idCircle = int.Parse(sValue);

                    sValue = _TableDelAveGlLvlCalcPos.Rows[i]["IDTag"].ToString();
                    if (UtilValue.IsInteger(sValue) == true)
                        idTag = int.Parse(sValue);

                    Revit.DB.FamilyInstance fmCircle = base.CmpElements.GetAveGlLvlCalcPos(idCircle) as Revit.DB.FamilyInstance;
                    Revit.DB.IndependentTag tag = base.CmpElements.GetAveGlLvlCalcPos(idTag) as Revit.DB.IndependentTag;
                    if (fmCircle != null && tag != null)
                    {
                        ObjectTag objTag = new ObjectTag();
                        objTag.CircleTag = fmCircle;
                        objTag.Tag = tag;
                        delAveGlLvlCalcPoss.Add(objTag);
                    }
                }
            }
            base.CmpElements.DelAveGlLevelCalcPos(delAveGlLvlCalcPoss);
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>平均地盤面算定ポイントテーブルデータ</summary>
        /// <history><p>2011/08/07 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2015/09/11 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public System.Data.DataTable TableAveGlLvlCalcPos
        {
            get
            {
                if (_TableAveGlLvlCalcPos == null)
                {
                    double unitCoe = base.CmpGeometry.UnitCoe;
                    double dValue = 0.0;

                    // 列定義
                    System.Data.DataTable dt = new System.Data.DataTable();

                    // ID Circle
                    dt.Columns.Add("IDCircle", typeof(int));

                    // ID Tag
                    dt.Columns.Add("IDTag", typeof(int));

                    // 01 番号
                    dt.Columns.Add("Number", typeof(int));

                    // 02 レベル
                    dt.Columns.Add("Level", typeof(string));

                    // 行定義
                    int countRow = -1;
                    foreach (ObjectTag aveGlLvlCalcPos in _AveGlLvlCalcPoss)
                    {
                        _EntSpAnnotation.CurrentCircle = aveGlLvlCalcPos.CircleTag;
                        _EntSpAnnotation.CurrentTag = aveGlLvlCalcPos.Tag;

                        // 行データ
                        countRow++;
                        System.Data.DataRow row = dt.NewRow();

                        // ID circle
                        row["IDCircle"] = aveGlLvlCalcPos.CircleTag.Id.ToString();

                        // ID
                        row["IDTag"] = aveGlLvlCalcPos.Tag.Id.ToString();

                        // 番号
                        if (aveGlLvlCalcPos.HasStoredValues)
                            row["Number"] = aveGlLvlCalcPos.Number;
                        else
                            row["Number"] = _EntSpAnnotation.AveGlLvlCalcPosCircleNo;

                        // レベル
                        if (aveGlLvlCalcPos.HasStoredValues)
                            dValue = aveGlLvlCalcPos.Level;
                        else
                            dValue = _EntSpAnnotation.AveGlLvlCalcPosLevel;
                        if (CmpElements._IsSelectElement)
                            dValue = dValue * unitCoe;

                        //row["Level"] = UtilValue.Rounding(dValue, 1, 2);
                        dValue = dValue < Int64.MinValue ? 0 : dValue ;
                        row["Level"] = CmpParameters.StrZeroPadding(UtilValue.Rounding(dValue, 5, 2), 4);

                        dt.Rows.Add(row);
                    }
                    SortTableAveGlLvlCalcPos(dt);
                }
                return _TableAveGlLvlCalcPos;
            }
        }

        /// ================================================================================
        /// <summary>削除平均地盤面算定ポイントテーブルデータ</summary>
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public System.Data.DataTable TableDelAveGlLvlCalcPos
        {
            get
            {
                return _TableDelAveGlLvlCalcPos;
            }
        }

        /// ================================================================================
        /// <summary>BM高さ</summary>
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public double BMHeight
        {
            get
            {
                return _BMHeight;
            }
            set
            {
                _BMHeight = value;
            }
        }

        /// ================================================================================
        /// <summary>縮尺</summary>
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int Scale
        {
            get
            {
                return _Scale;
            }
            set
            {
                _Scale = value;
            }
        }

        /// ================================================================================
        /// <summary>横の比</summary>
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int RaiteHorizontal
        {
            get
            {
                return _RaiteHorizontal;
            }
            set
            {
                _RaiteHorizontal = value;
            }
        }

        /// ================================================================================
        /// <summary>縦の比</summary>
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int RaiteVertical
        {
            get
            {
                return _RaiteVertical;
            }
            set
            {
                _RaiteVertical = value;
            }
        }

        /// ================================================================================
        /// <summary>面積の小数点位置</summary>
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
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
        /// <summary>面積の端数処理タイプ</summary>
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
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
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
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
        /// <summary>小数点桁数の最小値</summary>
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
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
        /// <history>2011/08/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int DecimalMax
        {
            get
            {
                return _DecimalMax;
            }
        }

        /// ================================================================================
        /// <summary>共有パラメータ</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public RvtExtApp.Entities.SpAnnotation EntSpAnnotation
        {
            get
            {
                return _EntSpAnnotation;
            }
        }

        #endregion Properties
    }
}