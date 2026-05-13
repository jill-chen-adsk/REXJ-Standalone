
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - 建具タイプ</summary>
    /// ================================================================================
    public class SpWinDoorType : RvtExtApp.Entities.SpBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>定義値 開口係数</summary>
        private ParamDefStrc _DefValOpenCoeff;

        /// <summary>定義値 排煙窓幅</summary>
        private ParamDefStrc _DefValSmokeWinWidth;

        /// <summary>定義値 排煙窓高さ</summary>
        private ParamDefStrc _DefValSmokeWinHeight;

        /// <summary>定義値 記号</summary>
        private ParamDefStrc _DefValMark;

        /// <summary>定義値 番号</summary>
        private ParamDefStrc _DefValNo;

        /// <summary>パラメータカテゴリ</summary>
        private Collections.Generic.IList<Revit.DB.Category> _ParamCategories;

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
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public SpWinDoorType(RvtExtApp.Components.Attribute cmpAttribute,
                             RvtExtApp.Components.Parameters cmpParameters,
                             RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpParameters, cmpSettings)
        {
            // パラメータカテゴリ
            _ParamCategories = base.CmpSettings.CategoryWinDoor;
            base.SetDefCatName(_ParamCategories);

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
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetDef()
        {
            // 初期化
            bool success = true;

            // 開口係数
            if (success == true)
            {
                success = DefOpenCoeff();
            }

            // 排煙窓幅
            if (success == true)
            {
                success = DefSmokeWinWidth();
            }

            // 排煙窓高さ
            if (success == true)
            {
                success = DefSmokeWinHeight();
            }

            // 記号
            if (success == true)
            {
                success = DefMark();
            }

            // 番号
            if (success == true)
            {
                success = DefNo();
            }

            // 実行状態
            base.DefSuccess = success;
        }

        /// ================================================================================
        /// <summary>定義 開口係数</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefOpenCoeff()
        {
            _DefValOpenCoeff = new ParamDefStrc(_ParamCategories,
                                                       base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_OPENCOEFF"),
                                                       Revit.DB.SpecTypeId.Number,
                                                       new Revit.DB.ForgeTypeId(string.Empty),
                                                       true,
                                                       1);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValOpenCoeff.Categories,
                                                        _DefValOpenCoeff.DefName,
                                                        _DefValOpenCoeff.ParamType,
                                                        _DefValOpenCoeff.BltParamGroup,
                                                        _DefValOpenCoeff.Visible,
                                                        _DefValOpenCoeff.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValOpenCoeff.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 排煙窓幅</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/09/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefSmokeWinWidth()
        {
            _DefValSmokeWinWidth = new ParamDefStrc(_ParamCategories,
                                                           base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_SMOKEWINWIDTH"),
                                                           Revit.DB.SpecTypeId.Length,
                                                           new Revit.DB.ForgeTypeId(string.Empty),
                                                           true,
                                                           1);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValSmokeWinWidth.Categories,
                                                        _DefValSmokeWinWidth.DefName,
                                                        _DefValSmokeWinWidth.ParamType,
                                                        _DefValSmokeWinWidth.BltParamGroup,
                                                        _DefValSmokeWinWidth.Visible,
                                                        _DefValSmokeWinWidth.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValSmokeWinWidth.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 排煙窓高さ</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/09/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefSmokeWinHeight()
        {
            _DefValSmokeWinHeight = new ParamDefStrc(_ParamCategories,
                                                            base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_SMOKEWINHEIGHT"),
                                                            Revit.DB.SpecTypeId.Length,
                                                            new Revit.DB.ForgeTypeId(string.Empty),
                                                            true,
                                                            1);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValSmokeWinHeight.Categories,
                                                        _DefValSmokeWinHeight.DefName,
                                                        _DefValSmokeWinHeight.ParamType,
                                                        _DefValSmokeWinHeight.BltParamGroup,
                                                        _DefValSmokeWinHeight.Visible,
                                                        _DefValSmokeWinHeight.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValSmokeWinHeight.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 記号</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefMark()
        {
            _DefValMark = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_MARK"),
                                                  Revit.DB.SpecTypeId.String.Text,
                                                  new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValMark.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 番号</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefNo()
        {
            _DefValNo = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_NUMBER"),
                                                Revit.DB.SpecTypeId.String.Text,
                                                new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValNo.DefName;
            }
            return ret;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>開口係数</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/29 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double OpenCoeff
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValOpenCoeff.DefName,
                                                    _DefValOpenCoeff.ParamType,
                                                    _DefValOpenCoeff.BltParamGroup,
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
                                                    _DefValOpenCoeff.DefName,
                                                    _DefValOpenCoeff.ParamType,
                                                    _DefValOpenCoeff.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>排煙窓幅</summary>
        /// <history><p>2011/09/01 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double SmokeWinWidth
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValSmokeWinWidth.DefName,
                                                    _DefValSmokeWinWidth.ParamType,
                                                    _DefValSmokeWinWidth.BltParamGroup,
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
                                                    _DefValSmokeWinWidth.DefName,
                                                    _DefValSmokeWinWidth.ParamType,
                                                    _DefValSmokeWinWidth.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>排煙窓高さ</summary>
        /// <history><p>2011/09/01 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double SmokeWinHeight
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValSmokeWinHeight.DefName,
                                                    _DefValSmokeWinHeight.ParamType,
                                                    _DefValSmokeWinHeight.BltParamGroup,
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
                                                    _DefValSmokeWinHeight.DefName,
                                                    _DefValSmokeWinHeight.ParamType,
                                                    _DefValSmokeWinHeight.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>記号</summary>
        /// <history><p>2011/07/28 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string Mark
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValMark.DefName,
                                                    _DefValMark.ParamType,
                                                    _DefValMark.BltParamGroup,
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
                                                    _DefValMark.DefName,
                                                    _DefValMark.ParamType,
                                                    _DefValMark.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>番号</summary>
        /// <history><p>2011/07/28 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string No
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValNo.DefName,
                                                    _DefValNo.ParamType,
                                                    _DefValNo.BltParamGroup,
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
                                                    _DefValNo.DefName,
                                                    _DefValNo.ParamType,
                                                    _DefValNo.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>有効幅</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double UsableWidth
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.CASEWORK_WIDTH,
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
                                                    Revit.DB.BuiltInParameter.CASEWORK_WIDTH,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>有効高さ</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double UsableHeight
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.FAMILY_HEIGHT_PARAM,
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
                                                    Revit.DB.BuiltInParameter.FAMILY_HEIGHT_PARAM,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>幅</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double Width
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.GENERIC_WIDTH,
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
                                                    Revit.DB.BuiltInParameter.GENERIC_WIDTH,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>高さ</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double Height
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.DOOR_HEIGHT,
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
                                                    Revit.DB.BuiltInParameter.DOOR_HEIGHT,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        #endregion Properties
    }
}