using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities.Exclusion
{
    /// ================================================================================
    /// <summary>共有パラメータ - ビュー</summary>
    /// ================================================================================
    public class SpView : SpBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>定義値 壁</summary>
        private ParamDefStrc _DefValWall;

        /// <summary>定義値 柱</summary>
        private ParamDefStrc _DefValColumn;

        /// <summary>定義値 梁</summary>
        private ParamDefStrc _DefValBeam;

        /// <summary>定義値 スラブ</summary>
        private ParamDefStrc _DefValSlab;

        /// <summary>定義値 基礎</summary>
        private ParamDefStrc _DefValFoundation;

        /// <summary>パラメータカテゴリ</summary>
        private IList<Category> _ParamCategories;

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
        /// <history>2011/12/07 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public SpView(RvtExtApp.Components.Attribute cmpAttribute,
                      RvtExtApp.Components.ESM_Parameters cmpParameters,
                      RvtExtApp.Components.ESM_Settings cmpSettings) :
                 base(cmpAttribute, cmpParameters, cmpSettings)
        {
            // パラメータカテゴリ
            _ParamCategories = new List<Category>();
            _ParamCategories.Add(base.CmpSettings.CategoryView);
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
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetDef()
        {
            // 初期化
            bool success = true;

            // 壁
            if (success == true)
            {
                success = DefWall();
            }

            // 柱
            if (success == true)
            {
                success = DefColumn();
            }

            // 梁
            if (success == true)
            {
                success = DefBeam();
            }

            // スラブ
            if (success == true)
            {
                success = DefSlab();
            }

            // 基礎
            if (success == true)
            {
                success = DefFoundation();
            }

            // 実行状態
            base.DefSuccess = success;
        }

        /// ================================================================================
        /// <summary>定義 壁</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefWall()
        {
            _DefValWall = new ParamDefStrc(_ParamCategories,
                                                  base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_EXCLSPMENTION_WALL"),
                                                  SpecTypeId.String.Text,
                                                  new ForgeTypeId(string.Empty),
                                                  false,
                                                  0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValWall.Categories,
                                                        _DefValWall.DefName,
                                                        _DefValWall.ParamType,
                                                        _DefValWall.BltParamGroup,
                                                        _DefValWall.Visible,
                                                        _DefValWall.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValWall.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 柱</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefColumn()
        {
            _DefValColumn = new ParamDefStrc(_ParamCategories,
                                                    base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_EXCLSPMENTION_COLUMN"),
                                                    SpecTypeId.String.Text,
                                                    new ForgeTypeId(string.Empty),
                                                    false,
                                                    0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValColumn.Categories,
                                                        _DefValColumn.DefName,
                                                        _DefValColumn.ParamType,
                                                        _DefValColumn.BltParamGroup,
                                                        _DefValColumn.Visible,
                                                        _DefValColumn.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValColumn.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 梁</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefBeam()
        {
            _DefValBeam = new ParamDefStrc(_ParamCategories,
                                                  base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_EXCLSPMENTION_BEAM"),
                                                  SpecTypeId.String.Text,
                                                  new ForgeTypeId(string.Empty),
                                                  false,
                                                  0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValBeam.Categories,
                                                        _DefValBeam.DefName,
                                                        _DefValBeam.ParamType,
                                                        _DefValBeam.BltParamGroup,
                                                        _DefValBeam.Visible,
                                                        _DefValBeam.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValBeam.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 スラブ</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefSlab()
        {
            _DefValSlab = new ParamDefStrc(_ParamCategories,
                                                  base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_EXCLSPMENTION_SLAB"),
                                                  SpecTypeId.String.Text,
                                                  new ForgeTypeId(string.Empty),
                                                  false,
                                                  0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValSlab.Categories,
                                                        _DefValSlab.DefName,
                                                        _DefValSlab.ParamType,
                                                        _DefValSlab.BltParamGroup,
                                                        _DefValSlab.Visible,
                                                        _DefValSlab.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValSlab.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 基礎</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefFoundation()
        {
            _DefValFoundation = new ParamDefStrc(_ParamCategories,
                                                        base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_EXCLSPMENTION_FOUNDATION"),
                                                        SpecTypeId.String.Text,
                                                        new ForgeTypeId(string.Empty),
                                                        false,
                                                        0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValFoundation.Categories,
                                                        _DefValFoundation.DefName,
                                                        _DefValFoundation.ParamType,
                                                        _DefValFoundation.BltParamGroup,
                                                        _DefValFoundation.Visible,
                                                        _DefValFoundation.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValFoundation.DefName;
            }
            return ret;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>壁</summary>
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string Wall
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValWall.DefName,
                                                    _DefValWall.ParamType,
                                                    _DefValWall.BltParamGroup,
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
                                                    _DefValWall.DefName,
                                                    _DefValWall.ParamType,
                                                    _DefValWall.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>柱</summary>
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string Column
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValColumn.DefName,
                                                    _DefValColumn.ParamType,
                                                    _DefValColumn.BltParamGroup,
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
                                                    _DefValColumn.DefName,
                                                    _DefValColumn.ParamType,
                                                    _DefValColumn.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>梁</summary>
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string Beam
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValBeam.DefName,
                                                    _DefValBeam.ParamType,
                                                    _DefValBeam.BltParamGroup,
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
                                                    _DefValBeam.DefName,
                                                    _DefValBeam.ParamType,
                                                    _DefValBeam.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>スラブ</summary>
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string Slab
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValSlab.DefName,
                                                    _DefValSlab.ParamType,
                                                    _DefValSlab.BltParamGroup,
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
                                                    _DefValSlab.DefName,
                                                    _DefValSlab.ParamType,
                                                    _DefValSlab.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>基礎</summary>
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string Foundation
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValFoundation.DefName,
                                                    _DefValFoundation.ParamType,
                                                    _DefValFoundation.BltParamGroup,
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
                                                    _DefValFoundation.DefName,
                                                    _DefValFoundation.ParamType,
                                                    _DefValFoundation.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        #endregion Properties
    }
}

