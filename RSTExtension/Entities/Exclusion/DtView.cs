using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities.Exclusion
{
    /// ================================================================================
    /// <summary>データテーブル - ビュー</summary>
    /// ================================================================================
    public class DtView : RvtExtApp.Entities.Exclusion.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>共有パラメータ</summary>
        private RvtExtApp.Entities.Exclusion.SpView _EntSpView;

        /// <summary>壁 - データ</summary>
        private IList<string> _WallData;

        /// <summary>壁 - 項目数</summary>
        private int _WallItemNum;

        /// <summary>柱 - データ</summary>
        private IList<string> _ColumnData;

        /// <summary>柱 - 項目数</summary>
        private int _ColumnItemNum;

        /// <summary>梁 - データ</summary>
        private IList<string> _BeamData;

        /// <summary>梁 - 項目数</summary>
        private int _BeamItemNum;

        /// <summary>スラブ - データ</summary>
        private IList<string> _SlabData;

        /// <summary>スラブ - 項目数</summary>
        private int _SlabItemNum;

        /// <summary>基礎 - データ</summary>
        private IList<string> _FoundationData;

        /// <summary>基礎 - 項目数</summary>
        private int _FoundationItemNum;

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
        /// <history><p>2011/12/07 Created  GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/10/12 Modified Applied Technology<p></history>
        /// ================================================================================
        public DtView(RvtExtApp.Components.Attribute cmpAttribute,
                      RvtExtApp.Components.ESM_Elements cmpElements,
                      RvtExtApp.Components.ESM_Geometry cmpGeometry,
                      RvtExtApp.Components.ESM_Parameters cmpParameters,
                      RvtExtApp.Components.ESM_Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            // 共有パラメータ
            _EntSpView = new RvtExtApp.Entities.Exclusion.SpView(cmpAttribute, cmpParameters, cmpSettings);
            if (_EntSpView.DefSuccess == false)
            {
                string strCategory = cmpAttribute.ResourceText("IDS_TXT_CATEGORY");
                string strParam = cmpAttribute.ResourceText("IDS_TXT_PARAMETER");
                base.ErrMsg = cmpAttribute.ResourceText("IDS_ERR_PARAMDEF") + "\n" +
                                          strCategory + " = " + _EntSpView.DefCatName + "\n" +
                                          "    " + strParam + "[" + _EntSpView.ErrDefName + "]";
            }

            // 壁
            _WallItemNum = 4;
            _WallData = new List<string>();
            for (int i = 0; i < _WallItemNum; ++i)
            {
                _WallData.Add("");
            }

            // 柱
            _ColumnItemNum = 2;
            _ColumnData = new List<string>();
            for (int i = 0; i < _ColumnItemNum; ++i)
            {
                _ColumnData.Add("");
            }

            // 梁
            _BeamItemNum = 10;
            _BeamData = new List<string>();
            for (int i = 0; i < _BeamItemNum; ++i)
            {
                _BeamData.Add("");
            }

            // スラブ
            _SlabItemNum = 2;
            _SlabData = new List<string>();
            for (int i = 0; i < _SlabItemNum; ++i)
            {
                _SlabData.Add("");
            }

            // 基礎
            _FoundationItemNum = 2;
            _FoundationData = new List<string>();
            for (int i = 0; i < _FoundationItemNum; ++i)
            {
                _FoundationData.Add("");
            }
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="elemView">ビュー要素</param>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetData(View elemView)
        {
            // 要素
            _EntSpView.CurrentElem = elemView;

            // 壁
            base.GetDataString(_EntSpView.Wall, _WallItemNum, ref _WallData);

            // 柱
            base.GetDataString(_EntSpView.Column, _ColumnItemNum, ref _ColumnData);

            // 梁
            base.GetDataString(_EntSpView.Beam, _BeamItemNum, ref _BeamData);

            // スラブ
            base.GetDataString(_EntSpView.Slab, _SlabItemNum, ref _SlabData);

            // 基礎
            base.GetDataString(_EntSpView.Foundation, _FoundationItemNum, ref _FoundationData);
        }

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <param name="elemView">ビュー要素</param>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void SetData(View elemView)
        {
            string sValue = "";

            // 要素
            _EntSpView.CurrentElem = elemView;

            // 壁
            sValue = null;
            base.GetDataString(_WallData, ref sValue);
            if (sValue != null)
            {
                _EntSpView.Wall = sValue;
            }

            // 柱
            sValue = null;
            base.GetDataString(_ColumnData, ref sValue);
            if (sValue != null)
            {
                _EntSpView.Column = sValue;
            }

            // 梁
            sValue = null;
            base.GetDataString(_BeamData, ref sValue);
            if (sValue != null)
            {
                _EntSpView.Beam = sValue;
            }

            // スラブ
            sValue = null;
            base.GetDataString(_SlabData, ref sValue);
            if (sValue != null)
            {
                _EntSpView.Slab = sValue;
            }

            // 基礎
            sValue = null;
            base.GetDataString(_FoundationData, ref sValue);
            if (sValue != null)
            {
                _EntSpView.Foundation = sValue;
            }
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>壁 - データ</summary>
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<string> WallData
        {
            get
            {
                return _WallData;
            }
            set
            {
                _WallData = value;
            }
        }

        /// ================================================================================
        /// <summary>柱 - データ</summary>
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<string> ColumnData
        {
            get
            {
                return _ColumnData;
            }
            set
            {
                _ColumnData = value;
            }
        }

        /// ================================================================================
        /// <summary>梁 - データ</summary>
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<string> BeamData
        {
            get
            {
                return _BeamData;
            }
            set
            {
                _BeamData = value;
            }
        }

        /// ================================================================================
        /// <summary>スラブ - データ</summary>
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<string> SlabData
        {
            get
            {
                return _SlabData;
            }
            set
            {
                _SlabData = value;
            }
        }

        /// ================================================================================
        /// <summary>基礎 - データ</summary>
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<string> FoundationData
        {
            get
            {
                return _FoundationData;
            }
            set
            {
                _FoundationData = value;
            }
        }

        #endregion Properties
    }
}

