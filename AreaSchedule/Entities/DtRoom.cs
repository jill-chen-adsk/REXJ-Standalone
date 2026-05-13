using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AreaSchedule;

namespace ADSK.JExtRAC.AreaSchedule.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - 部屋</summary>
    /// ================================================================================
    public class DtRoom : RvtExtApp.Entities.DtBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>共有パラメータ</summary>
        private RvtExtApp.Entities.SpRoom _EntSpRoom;

        /// <summary>部屋をエリアに変換チェック</summary>
        private bool _ChkConvertArea;

        /// <summary>部屋の境界線配置タイプ</summary>
        private Revit.DB.SpatialElementBoundaryLocation _RoomBndLocType;

        /// <summary>Revitの部屋の境界線配置タイプ</summary>
        private Revit.DB.SpatialElementBoundaryLocation _RvtRoomBndLocType;

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
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtRoom(RvtExtApp.Components.Attribute cmpAttribute,
                      RvtExtApp.Components.Elements cmpElements,
                      RvtExtApp.Components.Geometry cmpGeometry,
                      RvtExtApp.Components.Parameters cmpParameters,
                      RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ
            _EntSpRoom = new RvtExtApp.Entities.SpRoom(cmpAttribute, cmpParameters, cmpSettings);
            if (_EntSpRoom.DefSuccess == false)
            {
                string strCategory = base.CmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = base.CmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = base.CmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpRoom.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpRoom.ErrDefName + "]";
            }

            // 初期化
            _ChkConvertArea = true;
            _RoomBndLocType = base.CmpSettings.GetRoomAreaComputation();
            _RvtRoomBndLocType = _RoomBndLocType;
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>部屋の境界線配置タイプ設定</summary>
        ///
        /// <param name="value">値</param>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetRoomBndLocType(int value)
        {
            switch (value)
            {
                case 0:
                    _RoomBndLocType = Revit.DB.SpatialElementBoundaryLocation.Finish;
                    break;

                case 1:
                    _RoomBndLocType = Revit.DB.SpatialElementBoundaryLocation.Center;
                    break;

                case 2:
                    _RoomBndLocType = Revit.DB.SpatialElementBoundaryLocation.CoreBoundary;
                    break;

                case 3:
                    _RoomBndLocType = Revit.DB.SpatialElementBoundaryLocation.CoreCenter;
                    break;
            }
        }

        /// ================================================================================
        /// <summary>部屋の境界線配置タイプ取得</summary>
        ///
        /// <returns>境界線配置タイプ</returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Revit.DB.SpatialElementBoundaryLocation GetRoomBndLocType()
        {
            return _RoomBndLocType;
        }

        /// ================================================================================
        /// <summary>部屋の境界線配置タイプの番号を取得</summary>
        ///
        /// <returns>境界線配置タイプの番号</returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int GetRoomBndLocTypeNo()
        {
            int ret = 0;

            switch (_RoomBndLocType)
            {
                case Revit.DB.SpatialElementBoundaryLocation.Finish:
                    ret = 0;
                    break;

                case Revit.DB.SpatialElementBoundaryLocation.Center:
                    ret = 1;
                    break;

                case Revit.DB.SpatialElementBoundaryLocation.CoreBoundary:
                    ret = 2;
                    break;

                case Revit.DB.SpatialElementBoundaryLocation.CoreCenter:
                    ret = 3;
                    break;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>データ取得 -  - 部屋をエリアに変換</summary>
        ///
        /// <param name="chkConvertArea">部屋をエリアに変換チェック</param>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetDataRoomConvertedToArea(string chkConvertArea)
        {
            if ((chkConvertArea != null) && (chkConvertArea != ""))
            {
                _ChkConvertArea = Convert.ToBoolean(Byte.Parse(chkConvertArea));
            }
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>部屋をエリアに変換チェック</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        bool ChkConvertArea
        {
            get
            {
                return _ChkConvertArea;
            }
            set
            {
                _ChkConvertArea = value;
            }
        }

        /// ================================================================================
        /// <summary>部屋の境界線配置タイプ</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Revit.DB.SpatialElementBoundaryLocation RoomBndLocType
        {
            get
            {
                return _RoomBndLocType;
            }
        }

        /// ================================================================================
        /// <summary>Revitの部屋の境界線配置タイプ</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Revit.DB.SpatialElementBoundaryLocation RvtRoomBndLocType
        {
            get
            {
                return _RvtRoomBndLocType;
            }
        }

        /// ================================================================================
        /// <summary>共有パラメータ</summary>
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        RvtExtApp.Entities.SpRoom EntSpRoom
        {
            get
            {
                return _EntSpRoom;
            }
        }

        #endregion Properties
    }
}