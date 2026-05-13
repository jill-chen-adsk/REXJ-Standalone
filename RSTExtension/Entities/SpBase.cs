using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities
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
        private RvtExtApp.Components.CFP_Parameters _CmpParameters;

        /// <summary>設定</summary>
        private RvtExtApp.Components.CFP_Settings _CmpSettings;

        /// <summary>定義成功</summary>
        private bool _DefSuccess;

        /// <summary>エラー定義名</summary>
        private string _ErrDefName;

        /// <summary>定義カテゴリ名</summary>
        private string _DefCatName;

        /// <summary>現在要素</summary>
        private Element _CurrentElem;

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
        protected SpBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.CFP_Parameters cmpParameters,
                         RvtExtApp.Components.CFP_Settings cmpSettings)
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
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        void SetDefCatName(Category category)
        {
            IList<string> defCatNames = JExtComCompat.UtilValue.SplitString(_DefCatName, ",");
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
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        void SetDefCatName(IList<Category> categories)
        {
            foreach (Category category in categories)
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
        protected
        RvtExtApp.Components.Attribute CmpAttribute
        {
            get
            {
                return _CmpAttribute;
            }
        }

        /// ================================================================================
        /// <summary>パラメータ</summary>
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        RvtExtApp.Components.CFP_Parameters CmpParameters
        {
            get
            {
                return _CmpParameters;
            }
        }

        /// ================================================================================
        /// <summary>設定</summary>
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        RvtExtApp.Components.CFP_Settings CmpSettings
        {
            get
            {
                return _CmpSettings;
            }
        }

        /// ================================================================================
        /// <summary>定義成功</summary>
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool DefSuccess
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
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ErrDefName
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
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string DefCatName
        {
            get
            {
                return _DefCatName;
            }
        }

        /// ================================================================================
        /// <summary>現在要素</summary>
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Element CurrentElem
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

        #endregion Properties
    }
}
