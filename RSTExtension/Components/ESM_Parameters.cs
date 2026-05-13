using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Components
{
    /// ================================================================================
    /// <summary>パラメータ</summary>
    /// ================================================================================
    public class ESM_Parameters : RvtParameters
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>標準共有パラメータファイル名</summary>
        private string _ShParamDefaultFileName;

        /// <summary>共有パラメータフォルダ名</summary>
        private string _ShParamFolderName;

        /// <summary>共有パラメータファイル名</summary>
        private string _ShParamFileName;

        /// <summary>共有パラメータグループ名</summary>
        private string _ShParamGroupName;

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="rvtUIDoc">Revit UIドキュメント</p></param>
        ///
        /// <history>2011/12/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public ESM_Parameters(RvtExtApp.Components.Attribute cmpAttribute,
                          Autodesk.Revit.UI.UIDocument rvtUIDoc) : base(rvtUIDoc)
        {
            _CmpAttribute = cmpAttribute;

            // デフォルト共有パラメータ
            _ShParamDefaultFileName = null;
            DefinitionFile defFile = base.GetSharedParameterFile();
            if (defFile != null)
            {
                _ShParamDefaultFileName = defFile.Filename;
            }

            // アプリケーション用共有パラメータ
            _ShParamFolderName = _CmpAttribute.DataFolder;
            if (System.IO.Directory.Exists(_ShParamFolderName) == false)
            {
                _ShParamFolderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            _ShParamFileName = _CmpAttribute.ResourceText("IDS_SHPARAM_FILE");
            _ShParamGroupName = _CmpAttribute.ResourceText("IDS_SHPARAM_GROUP");

            if (_ShParamDefaultFileName == null)
            {
                _ShParamDefaultFileName = _ShParamFolderName + "\\" + _ShParamFileName;
            }
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>標準共有パラメータファイル設定</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/12/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool SetSharedParamDefault()
        {
            bool ret = false;

            // 共有パラメータファイル設定
            DefinitionFile defFile = base.SetSharedParameterFile(null, _ShParamDefaultFileName);
            if (defFile != null)
            {
                ret = true;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義設定</summary>
        ///
        /// <param name="elem"          >要素</param>
        /// <param name="categories"    >カテゴリ</param>
        /// <param name="defName"       >定義名</param>
        /// <param name="paramType"     >パラメータタイプ</param>
        /// <param name="bltParamGroup" >組込パラメータグループ</param>
        /// <param name="visible"       >可視</param>
        /// <param name="bindingMode"   ><p>結合モード</p>
        ///                                 <p>0 = インスタンス</p>
        ///                                 <p>1 = タイプ</p></param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool SetDefinition(Autodesk.Revit.DB.Element? elem,
                           IList<Category> categories,
                           string defName,
                           ForgeTypeId paramType,
                           ForgeTypeId bltParamGroup,
                           bool visible,
                           int bindingMode)
        {
            return base.SetDefinition(elem,
                                      _ShParamFolderName,
                                      _ShParamFileName,
                                      _ShParamGroupName,
                                      categories,
                                      defName,
                                      paramType,
                                      bltParamGroup,
                                      visible,
                                      bindingMode);
        }

        /// ================================================================================
        /// <summary>定義設定(オーバーロード)</summary>
        ///
        /// <param name="elem"          >要素</param>
        /// <param name="category"      >カテゴリ</param>
        /// <param name="defName"       >定義名</param>
        /// <param name="paramType"     >パラメータタイプ</param>
        /// <param name="bltParamGroup" >組込パラメータグループ</param>
        /// <param name="visible"       >可視</param>
        /// <param name="bindingMode"   ><p>結合モード</p>
        ///                                 <p>0 = インスタンス</p>
        ///                                 <p>1 = タイプ</p></param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/12/01 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool SetDefinition(Autodesk.Revit.DB.Element? elem,
                           Category category,
                           string defName,
                           ForgeTypeId paramType,
                           ForgeTypeId bltParamGroup,
                           bool visible,
                           int bindingMode)
        {
            IList<Category> categories = new List<Category>();
            categories.Add(category);
            return SetDefinition(elem,
                                 categories,
                                 defName,
                                 paramType,
                                 bltParamGroup,
                                 visible,
                                 bindingMode);
        }

        #endregion Member Functions

        // プロパティ
    }
}
