using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - レベル</summary>
    /// ================================================================================
    public class SpLevel : RvtExtApp.Entities.SpBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>定義値 スラブレベル</summary>
        private ParamDefStrc _DefValSlabLevel;

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
        /// <history>2011/11/26 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public SpLevel(RvtExtApp.Components.Attribute cmpAttribute,
                       RvtExtApp.Components.CFP_Parameters cmpParameters,
                       RvtExtApp.Components.CFP_Settings cmpSettings) :
               base(cmpAttribute, cmpParameters, cmpSettings)
        {
            // パラメータカテゴリ
            _ParamCategories = new List<Category>();
            _ParamCategories.Add(base.CmpSettings.CategoryLevel);
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
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetDef()
        {
            // 初期化
            bool success = true;

            // スラブレベル
            if (success == true)
            {
                success = DefSlabLevel();
            }

            // 実行状態
            base.DefSuccess = success;
        }

        /// ================================================================================
        /// <summary>定義 スラブレベル</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefSlabLevel()
        {
            _DefValSlabLevel = new ParamDefStrc(_ParamCategories,
                                                       base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_SLABLEVEL"),
                                                       SpecTypeId.Boolean.YesNo,
                                                       new ForgeTypeId(string.Empty),
                                                       true,
                                                       0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValSlabLevel.Categories,
                                                        _DefValSlabLevel.DefName,
                                                        _DefValSlabLevel.ParamType,
                                                        _DefValSlabLevel.BltParamGroup,
                                                        _DefValSlabLevel.Visible,
                                                        _DefValSlabLevel.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValSlabLevel.DefName;
            }
            return ret;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>スラブレベル</summary>
        /// <history><p>2011/11/27 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        bool SlabLevel
        {
            get
            {
                bool ret = false;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValSlabLevel.DefName,
                                                    _DefValSlabLevel.ParamType,
                                                    _DefValSlabLevel.BltParamGroup,
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
                                                    _DefValSlabLevel.DefName,
                                                    _DefValSlabLevel.ParamType,
                                                    _DefValSlabLevel.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        #endregion Properties
    }
}
