using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AveSiteLevelHeightCalc;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - 注釈</summary>
    /// ================================================================================
    public class SpAnnotation : RvtExtApp.Entities.SpBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>定義値 平均地盤面算定ポイント-レベル</summary>
        private ParamDefStrc _DefValAveGlLvlCalcPosLevel;

        /// <summary>定義値 平均地盤面算定ポイント-表示レベル</summary>
        private ParamDefStrc _DefValAveGlLvlCalcPosDispLevel;

        /// <summary>定義値 平均地盤面算定ポイント-番号</summary>
        private ParamDefStrc _DefValAveGlLvlCalcPosNo;

        /// <summary>定義値 BGL</summary>
        private ParamDefStrc _DefValBGL;

        /// <summary>定義値 縦縮尺</summary>
        private ParamDefStrc _DefValScaleVertical;

        /// <summary>定義値 横縮尺</summary>
        private ParamDefStrc _DefValScaleHorizontal;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        ///
        /// <history>2011/07/31 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public SpAnnotation(RvtExtApp.Components.Attribute cmpAttribute,
                            RvtExtApp.Components.Parameters cmpParameters,
                            RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpParameters, cmpSettings)
        {
            // 定義設定
            SetDef();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>定義設定</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private void SetDef()
        {
            // 初期化
            bool success = true;

            // 平均地盤面算定ポイント-レベル
            if (success == true)
            {
                success = DefAveGlLvlCalcPosLevel();
            }

            // 平均地盤面算定ポイント-表示レベル
            if (success == true)
            {
                success = DefAveGlLvlCalcPosDispLevel();
            }

            // 平均地盤面算定ポイント-番号
            if (success == true)
            {
                success = DefAveGlLvlCalcPosNo();
            }

            // BGL
            if (success == true)
            {
                success = DefBGL();
            }

            // 縦縮尺
            if (success == true)
            {
                success = DefScaleVertical();
            }

            // 横縮尺
            if (success == true)
            {
                success = DefScaleHorizontal();
            }

            // 実行状態
            base.DefSuccess = success;
        }

        /// ================================================================================
        /// <summary>定義 平均地盤面算定ポイント-レベル</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private bool DefAveGlLvlCalcPosLevel()
        {
            _DefValAveGlLvlCalcPosLevel = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_LEVEL"),
                                                                  Revit.DB.SpecTypeId.Number,
                                                                  new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValAveGlLvlCalcPosLevel.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 平均地盤面算定ポイント-表示レベル</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        ///          <p>2021/12/20 Modified Applied Technology</p></history>
        /// ================================================================================
        private bool DefAveGlLvlCalcPosDispLevel()
        {
            _DefValAveGlLvlCalcPosDispLevel = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_DISPLEVEL"),
                                                                      Revit.DB.SpecTypeId.Number,
                                                                      new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValAveGlLvlCalcPosDispLevel.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 平均地盤面算定ポイント-番号</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private bool DefAveGlLvlCalcPosNo()
        {
            _DefValAveGlLvlCalcPosNo = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_NUMBER"),
                                                               Revit.DB.SpecTypeId.Int.Integer,
                                                               new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValAveGlLvlCalcPosNo.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 BGL</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private bool DefBGL()
        {
            _DefValBGL = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_BGL"),
                                                 Revit.DB.SpecTypeId.String.Text,
                                                 new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValBGL.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 縦縮尺</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private bool DefScaleVertical()
        {
            _DefValScaleVertical = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_SCALEVERTICAL"),
                                                           Revit.DB.SpecTypeId.String.Text,
                                                           new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValScaleVertical.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 横縮尺</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private bool DefScaleHorizontal()
        {
            _DefValScaleHorizontal = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_SCALEHORIZONTAL"),
                                                             Revit.DB.SpecTypeId.String.Text,
                                                             new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValScaleHorizontal.DefName;
            }
            return ret;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>平均地盤面算定ポイント-レベル</summary>
        /// <history><p>2011/07/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public double AveGlLvlCalcPosLevel
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValAveGlLvlCalcPosLevel.DefName,
                                                    _DefValAveGlLvlCalcPosLevel.ParamType,
                                                    _DefValAveGlLvlCalcPosLevel.BltParamGroup,
                                                    ref ret) < -1)
                    {
                    }
                }

                if (base.CurrentCircle != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentCircle,
                                                    _DefValAveGlLvlCalcPosLevel.DefName,
                                                    _DefValAveGlLvlCalcPosLevel.ParamType,
                                                    _DefValAveGlLvlCalcPosLevel.BltParamGroup,
                                                    ref ret) < -1)
                    {
                    }
                }
                return ret;
            }
            set
            {
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.SetValue(base.CurrentElem,
                                                    _DefValAveGlLvlCalcPosLevel.DefName,
                                                    _DefValAveGlLvlCalcPosLevel.ParamType,
                                                    _DefValAveGlLvlCalcPosLevel.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }

                if (base.CurrentCircle != null)
                {
                    if (base.CmpParameters.SetValue(base.CurrentCircle,
                                                    _DefValAveGlLvlCalcPosLevel.DefName,
                                                    _DefValAveGlLvlCalcPosLevel.ParamType,
                                                    _DefValAveGlLvlCalcPosLevel.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>平均地盤面算定ポイント-表示レベル</summary>
        /// <history><p>2011/07/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        ///          <p>2021/12/20 Modified Applied Technology</p></history>
        /// ================================================================================
        public double AveGlLvlCalcPosDispLevel
        {
            get
            {
                double ret = 0;

                if (base.CurrentCircle != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentCircle,
                                                    _DefValAveGlLvlCalcPosDispLevel.DefName,
                                                    _DefValAveGlLvlCalcPosDispLevel.ParamType,
                                                    _DefValAveGlLvlCalcPosDispLevel.BltParamGroup,
                                                    ref ret) < -1)
                    {
                    }
                }

                return ret;
            }
            set
            {
                if (base.CurrentCircle != null)
                {
                    if (base.CmpParameters.SetValue(base.CurrentCircle,
                                                    _DefValAveGlLvlCalcPosDispLevel.DefName,
                                                    _DefValAveGlLvlCalcPosDispLevel.ParamType,
                                                    _DefValAveGlLvlCalcPosDispLevel.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>平均地盤面算定ポイント-番号</summary>
        /// <history><p>2011/07/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public int AveGlLvlCalcPosNo
        {
            get
            {
                int ret = 0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValAveGlLvlCalcPosNo.DefName,
                                                    _DefValAveGlLvlCalcPosNo.ParamType,
                                                    _DefValAveGlLvlCalcPosNo.BltParamGroup,
                                                    ref ret) < -1)
                    {
                    }
                }

                return ret;
            }
            set
            {
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.SetValue(base.CurrentElem,
                                                    _DefValAveGlLvlCalcPosNo.DefName,
                                                    _DefValAveGlLvlCalcPosNo.ParamType,
                                                    _DefValAveGlLvlCalcPosNo.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>Change no of circle</summary>
        /// <history><p>2021/12/20 Created  Applied Technology</history>
        /// ================================================================================
        public int AveGlLvlCalcPosCircleNo
        {
            get
            {
                int ret = 0;
                if (base.CurrentCircle != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentCircle,
                                                    _DefValAveGlLvlCalcPosNo.DefName,
                                                    _DefValAveGlLvlCalcPosNo.ParamType,
                                                    _DefValAveGlLvlCalcPosNo.BltParamGroup,
                                                    ref ret) < -1)
                    {
                    }
                }
                return ret;
            }
            set
            {
                if (base.CurrentCircle != null)
                {
                    if (base.CmpParameters.SetValue(base.CurrentCircle,
                                                    _DefValAveGlLvlCalcPosNo.DefName,
                                                    _DefValAveGlLvlCalcPosNo.ParamType,
                                                    _DefValAveGlLvlCalcPosNo.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>BGL</summary>
        /// <history><p>2011/07/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public string BGL
        {
            set
            {
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.SetValue(base.CurrentElem,
                                                    _DefValBGL.DefName,
                                                    _DefValBGL.ParamType,
                                                    _DefValBGL.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>縦縮尺</summary>
        /// <history><p>2011/07/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public string ScaleVertical
        {
            set
            {
                if (base.CurrentElem != null)
                {
                    if (CmpParameters.SetValue(base.CurrentElem,
                                               _DefValScaleVertical.DefName,
                                               _DefValScaleVertical.ParamType,
                                               _DefValScaleVertical.BltParamGroup,
                                               value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>横縮尺</summary>
        /// <history><p>2011/07/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public string ScaleHorizontal
        {
            set
            {
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.SetValue(base.CurrentElem,
                                                    _DefValScaleHorizontal.DefName,
                                                    _DefValScaleHorizontal.ParamType,
                                                    _DefValScaleHorizontal.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        #endregion Properties
    }
}