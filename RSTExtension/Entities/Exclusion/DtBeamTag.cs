using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using JExtComCompat;

using RvtExtApp = RSTExtension;

namespace RSTExtension.Entities.Exclusion
{
    /// ================================================================================
    /// <summary>データテーブル - 梁タグ</summary>
    /// ================================================================================
    public class DtBeamTag : RvtExtApp.Entities.Exclusion.DtBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>データ - 大梁</summary>
        private System.Data.DataTable _DataGirder;

        /// <summary>データ - 小梁</summary>
        private System.Data.DataTable _DataBeam;

        /// <summary>データ - 鉛直ブレース</summary>
        private System.Data.DataTable _DataVbrace;

        /// <summary>データ - 水平ブレース</summary>
        private System.Data.DataTable _DataHbrace;

        /// <summary>データ - その他</summary>
        private System.Data.DataTable _DataOther;

        /// <summary>大梁 - 符号 - 値</summary>
        private string _GirderMarkVal;

        /// <summary>大梁 - レベル - 値</summary>
        private string _GirderLevelVal;

        /// <summary>小梁 - 符号 - 値</summary>
        private string _BeamMarkVal;

        /// <summary>小梁 - レベル - 値</summary>
        private string _BeamLevelVal;

        /// <summary>鉛直ブレース - 符号 - 値</summary>
        private string _VbraceMarkVal;

        /// <summary>鉛直ブレース - レベル - 値</summary>
        private string _VbraceLevelVal;

        /// <summary>水平ブレース - 符号 - 値</summary>
        private string _HbraceMarkVal;

        /// <summary>水平ブレース - レベル - 値</summary>
        private string _HbraceLevelVal;

        /// <summary>その他 - 符号 - 値</summary>
        private string _OtherMarkVal;

        /// <summary>その他 - レベル - 値</summary>
        private string _OtherLevelVal;

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
        /// <history>2011/12/05 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public DtBeamTag(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.ESM_Elements cmpElements,
                         RvtExtApp.Components.ESM_Geometry cmpGeometry,
                         RvtExtApp.Components.ESM_Parameters cmpParameters,
                         RvtExtApp.Components.ESM_Settings cmpSettings) :
               base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
        {
            _DataGirder = new System.Data.DataTable();
            DefDataFormat(ref _DataGirder);

            _DataBeam = new System.Data.DataTable();
            DefDataFormat(ref _DataBeam);

            _DataVbrace = new System.Data.DataTable();
            DefDataFormat(ref _DataVbrace);

            _DataHbrace = new System.Data.DataTable();
            DefDataFormat(ref _DataHbrace);

            _DataOther = new System.Data.DataTable();
            DefDataFormat(ref _DataOther);

            _GirderMarkVal = "";

            _GirderLevelVal = "";

            _BeamMarkVal = "";

            _BeamLevelVal = "";

            _VbraceMarkVal = "";

            _VbraceLevelVal = "";

            _HbraceMarkVal = "";

            _HbraceLevelVal = "";

            _OtherMarkVal = "";

            _OtherLevelVal = "";
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>データ書式定義</summary>
        ///
        /// <param name="data">データテーブル</param>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void DefDataFormat(ref System.Data.DataTable data)
        {
            // ID
            data.Columns.Add(base.ColNameID, typeof(long));

            // 名称
            data.Columns.Add(base.ColNameName, typeof(string));
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="elemTagType" >タグタイプ要素</param>
        /// <param name="mode"        ><p>モード</p>
        ///                               <p>1 = 大梁</p>
        ///                               <p>2 = 小梁</p>
        ///                               <p>3 = 鉛直ブレース</p>
        ///                               <p>4 = 水平ブレース</p>
        ///                               <p>5 = その他</p></param>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetData(Element elemTagType, int mode)
        {
            // 初期化
            System.Data.DataRow row = null;

            // データ
            System.Data.DataTable dt = null;
            switch (mode)
            {
                case 1:
                    dt = _DataGirder;
                    break;

                case 2:
                    dt = _DataBeam;
                    break;

                case 3:
                    dt = _DataVbrace;
                    break;

                case 4:
                    dt = _DataHbrace;
                    break;

                case 5:
                    dt = _DataOther;
                    break;
            }

            if (dt == null)
            {
                return;
            }

            // 行設定
            if (elemTagType != null)
            {
                // 行データ
                row = dt.NewRow();

                // ID
                row[base.ColNameID] = (long)elemTagType.Id.Value;

                // 名称
                row[base.ColNameName] = base.GetFamilyTypeName(elemTagType);

                dt.Rows.Add(row);
            }
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        void GetData()
        {
            // 初期化
            if (_DataGirder == null)
            {
                _DataGirder = new System.Data.DataTable();
                DefDataFormat(ref _DataGirder);
            }
            if (_DataBeam == null)
            {
                _DataBeam = new System.Data.DataTable();
                DefDataFormat(ref _DataBeam);
            }
            if (_DataVbrace == null)
            {
                _DataVbrace = new System.Data.DataTable();
                DefDataFormat(ref _DataVbrace);
            }
            if (_DataHbrace == null)
            {
                _DataHbrace = new System.Data.DataTable();
                DefDataFormat(ref _DataHbrace);
            }
            if (_DataOther == null)
            {
                _DataOther = new System.Data.DataTable();
                DefDataFormat(ref _DataOther);
            }

            // なし設定
            base.SetItemNothing(_DataGirder);
            base.SetItemNothing(_DataBeam);
            base.SetItemNothing(_DataVbrace);
            base.SetItemNothing(_DataHbrace);
            base.SetItemNothing(_DataOther);

            // 梁要素
            IList<Element> girderElemAry = new List<Element>();
            IList<Element> beamElemAry = new List<Element>();
            IList<Element> vbraceElemAry = new List<Element>();
            IList<Element> hbraceElemAry = new List<Element>();
            IList<Element> otherElemAry = new List<Element>();
            IList<string> girderIdAry = new List<string>();
            IList<string> beamIdAry = new List<string>();
            IList<string> vbraceIdAry = new List<string>();
            IList<string> hbraceIdAry = new List<string>();
            IList<string> otherIdAry = new List<string>();
            base.GetBeams(ref girderElemAry, ref beamElemAry, ref vbraceElemAry, ref hbraceElemAry, ref otherElemAry,
                          ref girderIdAry, ref beamIdAry, ref vbraceIdAry, ref hbraceIdAry, ref otherIdAry);

            // 梁タグ要素
            IList<Element> beamTags = base.CmpElements.BeamTag;

            // タグタイプ
            IList<Element> girderTagTypeElemAry = new List<Element>();
            IList<Element> beamTagTypeElemAry = new List<Element>();
            IList<Element> vbraceTagTypeElemAry = new List<Element>();
            IList<Element> hbraceTagTypeElemAry = new List<Element>();
            IList<Element> otherTagTypeElemAry = new List<Element>();
            IList<string> girderTagTypeIdAry = new List<string>();
            IList<string> beamTagTypeIdAry = new List<string>();
            IList<string> vbraceTagTypeIdAry = new List<string>();
            IList<string> hbraceTagTypeIdAry = new List<string>();
            IList<string> otherTagTypeIdAry = new List<string>();
            for (int i = 0; i < beamTags.Count; ++i)
            {
                // タグ要素
                IndependentTag indpdtTag = beamTags[i] as IndependentTag;
                if (indpdtTag == null)
                {
                    continue;
                }

                // 配置要素
                foreach (Element locElem in indpdtTag.GetTaggedLocalElements())
                {
                    if (locElem == null)
                    {
                        continue;
                    }
                    var idLocElem = locElem.Id.ToString();

                    // タグタイプ
                    Element indpdtTagType = CmpElements.GetElemType(indpdtTag);
                    if (indpdtTagType == null)
                    {
                        continue;
                    }
                    var idIndpdtTagType = indpdtTagType.Id.ToString();

                    // 大梁
                    if (girderIdAry.Contains(idLocElem) == true)
                    {
                        if (girderTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            girderTagTypeElemAry.Add(indpdtTagType);
                            girderTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                    // 小梁
                    else if (beamIdAry.Contains(idLocElem) == true)
                    {
                        if (beamTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            beamTagTypeElemAry.Add(indpdtTagType);
                            beamTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                    // 鉛直ブレース
                    else if (vbraceIdAry.Contains(idLocElem) == true)
                    {
                        if (vbraceTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            vbraceTagTypeElemAry.Add(indpdtTagType);
                            vbraceTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                    // 水平ブレース
                    else if (hbraceIdAry.Contains(idLocElem) == true)
                    {
                        if (hbraceTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            hbraceTagTypeElemAry.Add(indpdtTagType);
                            hbraceTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                    // その他
                    else if (otherIdAry.Contains(idLocElem) == true)
                    {
                        if (otherTagTypeIdAry.Contains(idIndpdtTagType) == false)
                        {
                            otherTagTypeElemAry.Add(indpdtTagType);
                            otherTagTypeIdAry.Add(idIndpdtTagType);
                        }
                    }
                }
            }

            // データ登録 - 大梁
            for (int i = 0; i < girderTagTypeElemAry.Count; ++i)
            {
                GetData(girderTagTypeElemAry[i], 1);
            }

            // データ登録 - 小梁
            for (int i = 0; i < beamTagTypeElemAry.Count; ++i)
            {
                GetData(beamTagTypeElemAry[i], 2);
            }

            // データ登録 - 鉛直ブレース
            for (int i = 0; i < vbraceTagTypeElemAry.Count; ++i)
            {
                GetData(vbraceTagTypeElemAry[i], 3);
            }

            // データ登録 - 水平ブレース
            for (int i = 0; i < hbraceTagTypeElemAry.Count; ++i)
            {
                GetData(hbraceTagTypeElemAry[i], 4);
            }

            // データ登録 - その他
            for (int i = 0; i < otherTagTypeElemAry.Count; ++i)
            {
                GetData(otherTagTypeElemAry[i], 5);
            }

            _DataGirder.DefaultView.Sort = base.ColNameName + " " + "ASC";
            _DataBeam.DefaultView.Sort = base.ColNameName + " " + "ASC";
            _DataVbrace.DefaultView.Sort = base.ColNameName + " " + "ASC";
            _DataHbrace.DefaultView.Sort = base.ColNameName + " " + "ASC";
            _DataOther.DefaultView.Sort = base.ColNameName + " " + "ASC";
        }

        /// ================================================================================
        /// <summary>データ取得</summary>
        ///
        /// <param name="data">データ</param>
        ///
        /// <history><p>2011/12/06 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/10/12 Modified Applied Technology<p></history>
        /// ================================================================================
        public
        void GetData(IList<string> data)
        {
            // ========== 大梁 ==========

            // 大梁 - 符号 - 値
            _GirderMarkVal = data[0];

            // 大梁 - レベル - 値
            _GirderLevelVal = data[1];

            // ========== 小梁 ==========

            // 小梁 - 符号 - 値
            _BeamMarkVal = data[2];

            // 小梁 - レベル - 値
            _BeamLevelVal = data[3];

            // ========== 鉛直ブレース ==========

            // 鉛直ブレース - 符号 - 値
            _VbraceMarkVal = data[4];

            // 鉛直ブレース - レベル - 値
            _VbraceLevelVal = data[5];

            // ========== 水平ブレース ==========

            // 水平ブレース - 符号 - 値
            _HbraceMarkVal = data[6];

            // 水平ブレース - レベル - 値
            _HbraceLevelVal = data[7];

            // ========== その他 ==========

            // その他 - 符号 - 値
            _OtherMarkVal = data[8];

            // その他 - レベル - 値
            _OtherLevelVal = data[9];
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>大梁 - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string GirderMarkVal
        {
            get
            {
                return _GirderMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>大梁 - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string GirderLevelVal
        {
            get
            {
                return _GirderLevelVal;
            }
        }

        /// ================================================================================
        /// <summary>小梁 - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string BeamMarkVal
        {
            get
            {
                return _BeamMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>小梁 - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string BeamLevelVal
        {
            get
            {
                return _BeamLevelVal;
            }
        }

        /// ================================================================================
        /// <summary>鉛直ブレース - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string VbraceMarkVal
        {
            get
            {
                return _VbraceMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>鉛直ブレース - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string VbraceLevelVal
        {
            get
            {
                return _VbraceLevelVal;
            }
        }

        /// ================================================================================
        /// <summary>水平ブレース - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string HbraceMarkVal
        {
            get
            {
                return _HbraceMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>水平ブレース - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string HbraceLevelVal
        {
            get
            {
                return _HbraceLevelVal;
            }
        }

        /// ================================================================================
        /// <summary>その他 - 符号 - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string OtherMarkVal
        {
            get
            {
                return _OtherMarkVal;
            }
        }

        /// ================================================================================
        /// <summary>その他 - レベル - 値</summary>
        /// <history>2011/12/06 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        string OtherLevelVal
        {
            get
            {
                return _OtherLevelVal;
            }
        }

        #endregion Properties
    }
}



