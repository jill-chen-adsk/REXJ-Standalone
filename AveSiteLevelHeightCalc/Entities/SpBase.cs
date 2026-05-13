using System;
using System.Linq;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AveSiteLevelHeightCalc;
using ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - 基底</summary>
    /// ================================================================================
    public abstract class SpBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>パラメータ</summary>
        private RvtExtApp.Components.Parameters _CmpParameters;

        /// <summary>設定</summary>
        private RvtExtApp.Components.Settings _CmpSettings;

        /// <summary>定義成功</summary>
        private bool _DefSuccess;

        /// <summary>エラー定義名</summary>
        private string _ErrDefName;

        /// <summary>定義カテゴリ名</summary>
        private string _DefCatName;

        /// <summary>現在要素</summary>
        private Revit.DB.Element _CurrentElem;

        /// <summary>Circle of tag</summary>
        private Revit.DB.Element _CurrentCircle;

        /// <summary>Tag</summary>
        private Revit.DB.Element _CurrentTag;

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
        protected SpBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.Parameters cmpParameters,
                         RvtExtApp.Components.Settings cmpSettings)
        {
            // 初期化
            _CmpAttribute = cmpAttribute;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _DefSuccess = true;
            _ErrDefName = "";
            _DefCatName = "";
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>定義カテゴリ名設定</summary>
        ///
        /// <param name="category">カテゴリ</param>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected void SetDefCatName(Revit.DB.Category category)
        {
            Collections.Generic.IList<string> defCatNames = UtilValue.SplitString(_DefCatName, ",");
            if (defCatNames.Contains(category.Name) == false)
            {
                if (_DefCatName != "")
                {
                    _DefCatName += ",";
                }
                _DefCatName += category.Name;
            }
        }

        /// ================================================================================
        /// <summary>定義カテゴリ名設定(オーバーロード)</summary>
        ///
        /// <param name="categories">カテゴリ</param>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected void SetDefCatName(Collections.Generic.IList<Revit.DB.Category> categories)
        {
            foreach (Revit.DB.Category category in categories)
            {
                SetDefCatName(category);
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
        /// <summary>パラメータ</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
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
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected RvtExtApp.Components.Settings CmpSettings
        {
            get
            {
                return _CmpSettings;
            }
        }

        /// ================================================================================
        /// <summary>定義成功</summary>
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool DefSuccess
        {
            get
            {
                return _DefSuccess;
            }
            set
            {
                _DefSuccess = value;
            }
        }

        /// ================================================================================
        /// <summary>エラー定義名</summary>
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ErrDefName
        {
            get
            {
                return _ErrDefName;
            }
            set
            {
                _ErrDefName = value;
            }
        }

        /// ================================================================================
        /// <summary>定義カテゴリ名</summary>
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string DefCatName
        {
            get
            {
                return _DefCatName;
            }
        }

        /// ================================================================================
        /// <summary>現在要素</summary>
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.Element CurrentElem
        {
            get
            {
                return _CurrentElem;
            }
            set
            {
                _CurrentElem = value;
            }
        }

        /// ================================================================================
        /// <summary>Circle of tag</summary>
        /// <history>2021/12/20 Created Applied Technology</history>
        /// ================================================================================
        public Revit.DB.Element CurrentCircle
        {
            get
            {
                return _CurrentCircle;
            }
            set
            {
                _CurrentCircle = value;
            }
        }

        /// ================================================================================
        /// <summary>Tag</summary>
        /// <history>2021/12/20 Created Applied Technology</history>
        /// ================================================================================
        public Revit.DB.Element CurrentTag
        {
            get
            {
                return _CurrentTag;
            }
            set
            {
                _CurrentTag = value;
            }
        }

        #endregion Properties
    }
}