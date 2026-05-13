using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FittingSchedule;

namespace ADSK.JExtRAC.FittingSchedule.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 建具タイプ</summary>
    /// ================================================================================
    public class DtWinDoorType : RvtExtApp.Entities.DtBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>共有パラメータ</summary>
        private RvtExtApp.Entities.SpWinDoorType _EntSpWinDoorType;

        /// <summary>ドアタグデータ</summary>
        private System.Data.DataTable _DataDoorTags;

        /// <summary>窓タグデータ</summary>
        private System.Data.DataTable _DataWindowTags;

        /// <summary>ドアタグシンボルID</summary>
        private int _IdDoorTag;

        /// <summary>窓タグシンボルID</summary>
        private int _IdWindowTag;

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
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtWinDoorType(RvtExtApp.Components.Attribute cmpAttribute,
                             RvtExtApp.Components.Elements cmpElements,
                             RvtExtApp.Components.Geometry cmpGeometry,
                             RvtExtApp.Components.Parameters cmpParameters,
                             RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ
            _EntSpWinDoorType = new RvtExtApp.Entities.SpWinDoorType(cmpAttribute, cmpParameters, cmpSettings);
            if (_EntSpWinDoorType.DefSuccess == false)
            {
                string strCategory = cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = cmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpWinDoorType.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpWinDoorType.ErrDefName + "]";
            }

            // 初期化
            _IdDoorTag = -1;
            _IdWindowTag = -1;
            _DataDoorTags = base.CmpElements.ElementsTableDoorTag;
            _DataWindowTags = base.CmpElements.ElementsTableWindowTag;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>データ取得 - 建具姿図作成・更新</summary>
        ///
        /// <param name="idDoorTag"   >ドアタグシンボルID</param>
        /// <param name="idWindowTag" >窓タグシンボルID</param>
        ///
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetDataCreateAndEdit(string idDoorTag, string idWindowTag)
        {
            if ((idDoorTag != null) && (idDoorTag != ""))
            {
                _IdDoorTag = int.Parse(idDoorTag);
            }

            if ((idWindowTag != null) && (idWindowTag != ""))
            {
                _IdWindowTag = int.Parse(idWindowTag);
            }
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>共有パラメータ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        RvtExtApp.Entities.SpWinDoorType EntSpWinDoorType
        {
            get
            {
                return _EntSpWinDoorType;
            }
        }

        /// ================================================================================
        /// <summary>ドアタグシンボルID</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int IdDoorTag
        {
            get
            {
                return _IdDoorTag;
            }
            set
            {
                _IdDoorTag = value;
            }
        }

        /// ================================================================================
        /// <summary>窓タグシンボルID</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int IdWindowTag
        {
            get
            {
                return _IdWindowTag;
            }
            set
            {
                _IdWindowTag = value;
            }
        }

        /// ================================================================================
        /// <summary>ドアタグデータ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable DataDoorTags
        {
            get
            {
                return _DataDoorTags;
            }
        }

        /// ================================================================================
        /// <summary>窓タグデータ</summary>
        /// <history>2011/08/02 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        System.Data.DataTable DataWindowTags
        {
            get
            {
                return _DataWindowTags;
            }
        }

        #endregion Properties
    }
}
