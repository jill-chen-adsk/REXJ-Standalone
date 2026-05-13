using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 基底</summary>
    /// ================================================================================
    public abstract class DtBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private RvtExtApp.Components.CFP_Elements _CmpElements;

        /// <summary>図形</summary>
        private RvtExtApp.Components.CFP_Geometry _CmpGeometry;

        /// <summary>パラメータ</summary>
        private RvtExtApp.Components.CFP_Parameters _CmpParameters;

        /// <summary>設定</summary>
        private RvtExtApp.Components.CFP_Settings _CmpSettings;

        /// <summary>エラーメッセージ</summary>
        private string _ErrMsg;

        /// <summary>列名 ID</summary>
        private string _ColNameID;

        /// <summary>列名 名称</summary>
        private string _ColNameName;

        /// <summary>列名 レベル</summary>
        private string _ColNameLevel;

        /// <summary>列名 高さ</summary>
        private string _ColNameHeight;

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
        /// <history>2011/11/27 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected DtBase(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.CFP_Elements cmpElements,
                         RvtExtApp.Components.CFP_Geometry cmpGeometry,
                         RvtExtApp.Components.CFP_Parameters cmpParameters,
                         RvtExtApp.Components.CFP_Settings cmpSettings)
        {
            // 初期化
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _ErrMsg = "";
        }

        #endregion Constructor

        // メンバ関数

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
        /// <summary>要素</summary>
        /// <history>2011/11/27 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        RvtExtApp.Components.CFP_Elements CmpElements
        {
            get
            {
                return _CmpElements;
            }
        }

        /// ================================================================================
        /// <summary>図形</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        protected
        RvtExtApp.Components.CFP_Geometry CmpGeometry
        {
            get
            {
                return _CmpGeometry;
            }
        }

        /// ================================================================================
        /// <summary>パラメーター</summary>
        /// <history>2011/11/27 Created  GSA,Inc. Shinichi Ishii</history>
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
        /// <history>2011/11/27 Created  GSA,Inc. Shinichi Ishii</history>
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
        /// <summary>エラーメッセージ</summary>
        /// <history>2011/11/27 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ErrMsg
        {
            get
            {
                return _ErrMsg;
            }
            set
            {
                _ErrMsg = value;
            }
        }

        /// ================================================================================
        /// <summary>列名 ID</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColNameID
        {
            get
            {
                if (_ColNameID == null)
                {
                    _ColNameID = _CmpAttribute.ResourceText("IDS_COLNAME_ID");
                }
                return _ColNameID;
            }
        }

        /// ================================================================================
        /// <summary>列名 名称</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColNameName
        {
            get
            {
                if (_ColNameName == null)
                {
                    _ColNameName = _CmpAttribute.ResourceText("IDS_COLNAME_NAME");
                }
                return _ColNameName;
            }
        }

        /// ================================================================================
        /// <summary>列名 レベル</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColNameLevel
        {
            get
            {
                if (_ColNameLevel == null)
                {
                    _ColNameLevel = _CmpAttribute.ResourceText("IDS_COLNAME_LEVEL");
                }
                return _ColNameLevel;
            }
        }

        /// ================================================================================
        /// <summary>列名 高さ</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string ColNameHeight
        {
            get
            {
                if (_ColNameHeight == null)
                {
                    _ColNameHeight = _CmpAttribute.ResourceText("IDS_COLNAME_HEIGHT");
                }
                return _ColNameHeight;
            }
        }

        #endregion Properties
    }
}
