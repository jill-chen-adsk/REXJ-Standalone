using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FloorHeightDimension;

namespace ADSK.JExtRAC.FloorHeightDimension.Entities
{
    /// ================================================================================
    /// <summary>データテーブル - コマンド</summary>
    /// ================================================================================
    public class DtCmd : RvtExtApp.Entities.DtBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>共有パラメータ</summary>
        private RvtExtApp.Entities.SpCmd _EntSpCmd;

        /// <summary>データ</summary>
        private Collections.Generic.IList<string> _Data;

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
        /// <param name="elemProjInfo"  >プロジェクト情報</param>
        /// <param name="defName"       >定義名</param>
        /// <param name="itemNum"       >項目数</param>
        ///
        /// <history>2011/11/30 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtCmd(RvtExtApp.Components.Attribute cmpAttribute,
                     RvtExtApp.Components.Elements cmpElements,
                     RvtExtApp.Components.Geometry cmpGeometry,
                     RvtExtApp.Components.Parameters cmpParameters,
                     RvtExtApp.Components.Settings cmpSettings,
                     Revit.DB.ProjectInfo elemProjInfo,
                     string defName,
                     int itemNum) :
          base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ
            _EntSpCmd = new RvtExtApp.Entities.SpCmd(cmpAttribute,
                                                     cmpParameters,
                                                     cmpSettings,
                                                     elemProjInfo,
                                                     defName,
                                                     itemNum);
            if (_EntSpCmd.DefSuccess == false)
            {
                base.ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF");
            }
            else
            {
                _Data = _EntSpCmd.GetData();
            }
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <param name="elems">要素</param>
        ///
        /// <history>2011/11/30 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetData()
        {
            _EntSpCmd.SetData(_Data);
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>データ</summary>
        /// <history>2011/11/30 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> Data
        {
            get
            {
                return _Data;
            }
        }

        #endregion Properties
    }
}