
using System;
using Autodesk.Revit.DB ;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Entities
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

        /// <summary>定義値 種類</summary>
        private ParamDefStrc _DefValKind;

        /// <summary>定義値 グループ</summary>
        private ParamDefStrc _DefValGroup;

        /// <summary>定義値 計算グループ</summary>
        private ParamDefStrc _DefValCalcGroup;

        /// <summary>定義値 屋外</summary>
        private ParamDefStrc _DefValOutDoors;

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
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
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

            // 種類
            if (success == true)
            {
                success = DefKind();
            }

            // グループ
            if (success == true)
            {
                success = DefGroup();
            }

            // 計算グループ
            if (success == true)
            {
                success = DefCalcGroup();
            }

            // 屋外
            if (success == true)
            {
                success = DefOutDoors();
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
                                                       new Revit.DB.ForgeTypeId(string.Empty),
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

        /// ================================================================================
        /// <summary>定義 種類</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefKind()
        {
            _DefValKind = new ParamDefStrc(_ParamCategory,
                                                  base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_ROOM_KIND"),
                                                  Revit.DB.SpecTypeId.String.Text,
                                                   GroupTypeId.IdentityData,
                                                  true,
                                                  0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValKind.Category,
                                                        _DefValKind.DefName,
                                                        _DefValKind.ParamType,
                                                        _DefValKind.BltParamGroup,
                                                        _DefValKind.Visible,
                                                        _DefValKind.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValKind.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 グループ</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefGroup()
        {
            _DefValGroup = new ParamDefStrc(_ParamCategory,
                                                   base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_ROOM_GROUP"),
                                                   Revit.DB.SpecTypeId.String.Text,
                                                   GroupTypeId.IdentityData,
                                                   true,
                                                   0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValGroup.Category,
                                                        _DefValGroup.DefName,
                                                        _DefValGroup.ParamType,
                                                        _DefValGroup.BltParamGroup,
                                                        _DefValGroup.Visible,
                                                        _DefValGroup.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValGroup.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 計算グループ</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefCalcGroup()
        {
            _DefValCalcGroup = new ParamDefStrc(_ParamCategory,
                                                       base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_ROOM_CALCGROUP"),
                                                       Revit.DB.SpecTypeId.String.Text,
                                                        GroupTypeId.IdentityData,
                                                       true,
                                                       0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValCalcGroup.Category,
                                                        _DefValCalcGroup.DefName,
                                                        _DefValCalcGroup.ParamType,
                                                        _DefValCalcGroup.BltParamGroup,
                                                        _DefValCalcGroup.Visible,
                                                        _DefValCalcGroup.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValCalcGroup.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 屋外</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefOutDoors()
        {
            _DefValOutDoors = new ParamDefStrc(_ParamCategory,
                                                      base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_ROOM_OUTDOORS"),
                                                      Revit.DB.SpecTypeId.Boolean.YesNo,
                                                       GroupTypeId.IdentityData,
                                                      true,
                                                      0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValOutDoors.Category,
                                                        _DefValOutDoors.DefName,
                                                        _DefValOutDoors.ParamType,
                                                        _DefValOutDoors.BltParamGroup,
                                                        _DefValOutDoors.Visible,
                                                        _DefValOutDoors.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValOutDoors.DefName;
            }
            return ret;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>法定面積</summary>
        /// <history><p>2011/07/27 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double LegalArea
        {
            get
            {
                return TryGetLegalAreaValue();
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

        double TryGetLegalAreaValue()
        {
            if (base.CurrentElem == null)
                return 0.0;

            double value = 0.0;
            if (TryGetLegalAreaFromParameter(_DefValLegalArea.DefName, ref value))
                return value;

            // AreaSchedule writes this English parameter name via JPExtension.txt.
            if (TryGetLegalAreaFromParameter("Legal Area (Room shared)", ref value))
                return value;

            // Legacy CheckingALVS English binding and Japanese projects.
            if (TryGetLegalAreaFromParameter("Legal Area", ref value))
                return value;

            if (TryGetLegalAreaFromParameter("法定面積", ref value))
                return value;

            return 0.0;
        }

        bool TryGetLegalAreaFromParameter(string paramName, ref double value)
        {
            double localValue = 0.0;
            if (base.CmpParameters.GetValue(base.CurrentElem,
                                            paramName,
                                            _DefValLegalArea.ParamType,
                                            _DefValLegalArea.BltParamGroup,
                                            ref localValue) < -1)
            {
                return false;
            }

            value = localValue;
            return true;
        }

        bool TryGetStringFromParameter(ParamDefStrc def, string paramName, ref string value)
        {
            string localValue = "";
            if (base.CmpParameters.GetValue(base.CurrentElem,
                                            paramName,
                                            def.ParamType,
                                            def.BltParamGroup,
                                            ref localValue) != 0)
            {
                return false;
            }

            value = localValue ?? "";
            return true;
        }

        string TryGetSharedStringValue(ParamDefStrc def, string alternateJapaneseName, string alternateEnglishName)
        {
            string value = "";
            if (TryGetStringFromParameter(def, def.DefName, ref value))
            {
                return value;
            }

            if (TryGetStringFromParameter(def, alternateJapaneseName, ref value))
            {
                return value;
            }

            if (TryGetStringFromParameter(def, alternateEnglishName, ref value))
            {
                return value;
            }

            return "";
        }

        bool TrySetSharedStringValue(ParamDefStrc def, string alternateJapaneseName, string alternateEnglishName, string value)
        {
            if (base.CmpParameters.SetValue(base.CurrentElem,
                                            def.DefName,
                                            def.ParamType,
                                            def.BltParamGroup,
                                            value) == 0)
            {
                return true;
            }

            if (base.CmpParameters.SetValue(base.CurrentElem,
                                            alternateJapaneseName,
                                            def.ParamType,
                                            def.BltParamGroup,
                                            value) == 0)
            {
                return true;
            }

            if (base.CmpParameters.SetValue(base.CurrentElem,
                                            alternateEnglishName,
                                            def.ParamType,
                                            def.BltParamGroup,
                                            value) == 0)
            {
                return true;
            }

            return false;
        }

        /// ================================================================================
        /// <summary>種類</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string Kind
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValKind.DefName,
                                                    _DefValKind.ParamType,
                                                    _DefValKind.BltParamGroup,
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
                                                    _DefValKind.DefName,
                                                    _DefValKind.ParamType,
                                                    _DefValKind.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>グループ</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string Group
        {
            get
            {
                if (base.CurrentElem == null)
                {
                    return "";
                }

                return TryGetSharedStringValue(_DefValGroup, "部屋グループ", "Room Group");
            }
            set
            {
                if (base.CurrentElem != null)
                {
                    TrySetSharedStringValue(_DefValGroup, "部屋グループ", "Room Group", value);
                }
            }
        }

        /// ================================================================================
        /// <summary>計算グループ</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string CalcGroup
        {
            get
            {
                if (base.CurrentElem == null)
                {
                    return "";
                }

                return TryGetSharedStringValue(_DefValCalcGroup, "部屋計算グループ", "Room Calc Group");
            }
            set
            {
                if (base.CurrentElem != null)
                {
                    TrySetSharedStringValue(_DefValCalcGroup, "部屋計算グループ", "Room Calc Group", value);
                }
            }
        }

        /// ================================================================================
        /// <summary>屋外</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        bool OutDoors
        {
            get
            {
                bool ret = false;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValOutDoors.DefName,
                                                    _DefValOutDoors.ParamType,
                                                    _DefValOutDoors.BltParamGroup,
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
                                                    _DefValOutDoors.DefName,
                                                    _DefValOutDoors.ParamType,
                                                    _DefValOutDoors.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>部屋名</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
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
        /// <history><p>2011/08/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string RoomNo
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
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
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