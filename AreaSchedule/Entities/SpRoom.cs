using Autodesk.Revit.DB ;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;

namespace ADSK.JExtRAC.AreaSchedule.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - 部屋</summary>
    /// ================================================================================
    public class SpRoom : RvtExtApp.Entities.SpBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>定義値 法定面積</summary>
        private ParamDefStrc _DefValLegalArea;

        /// <summary>パラメータカテゴリ</summary>
        private Revit.DB.Category _ParamCategory;

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
        /// <history>2011/07/27 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public SpRoom(RvtExtApp.Components.Attribute cmpAttribute,
                      RvtExtApp.Components.Parameters cmpParameters,
                      RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpParameters, cmpSettings)
        {
            // パラメータカテゴリ
            _ParamCategory = base.CmpSettings.CategoryRoom;
            base.SetDefCatName(_ParamCategory);

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

            // 法定面積
            if (success == true)
            {
                success = DefLegalArea();
            }

            // 実行状態
            base.DefSuccess = success;
        }

        /// ================================================================================
        /// <summary>定義 法定面積</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefLegalArea()
        {

            _DefValLegalArea = new ParamDefStrc(_ParamCategory,
                                                       base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_ROOM_LEGALAREA"),
                                                       Revit.DB.SpecTypeId.Area,
                                                       new ForgeTypeId(string.Empty),
                                                       true,
                                                       0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValLegalArea.Category,
                                                        _DefValLegalArea.DefName,
                                                        _DefValLegalArea.ParamType,
                                                        _DefValLegalArea.BltParamGroup,
                                                        _DefValLegalArea.Visible,
                                                        _DefValLegalArea.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValLegalArea.DefName;
            }
            return ret;
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>法定面積</summary>
        /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double LegalArea
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValLegalArea.DefName,
                                                    _DefValLegalArea.ParamType,
                                                    _DefValLegalArea.BltParamGroup,
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
                                                    _DefValLegalArea.DefName,
                                                    _DefValLegalArea.ParamType,
                                                    _DefValLegalArea.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>部屋名</summary>
        /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string RoomName
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.ROOM_NAME,
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
                                                    Revit.DB.BuiltInParameter.ROOM_NAME,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>部屋番号</summary>
        /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string RoomNumber
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.ROOM_NUMBER,
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
                                                    Revit.DB.BuiltInParameter.ROOM_NUMBER,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>部屋面積</summary>
        /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double RoomArea
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.ROOM_AREA,
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
                                                    Revit.DB.BuiltInParameter.ROOM_AREA,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        #endregion Properties
    }
}