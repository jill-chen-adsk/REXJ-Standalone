using System.Collections.Generic;
using Autodesk.Revit.DB;


using RvtExtApp = RSTExtension;

namespace RSTExtension.Components
{
    /// ================================================================================
    /// <summary>サービス</summary>
    /// ================================================================================
    public class CFP_Service
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
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public CFP_Service(RvtExtApp.Components.Attribute cmpAttribute,
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
        #region Member Functions

        /// ================================================================================
        /// <summary>壁取得</summary>
        ///
        /// <param name="level"   >レベル</param>
        /// <param name="elemsAg" >要素 - 一致</param>
        /// <param name="elemsDf" >要素 - 非一致</param>
        ///
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetWalls(Level level,
                      ref IList<Element> elemsAg,
                      ref IList<Element> elemsDf)
        {
            // レベルID
            var idLevel = level.Id.Value;

            // 要素
            IList<System.Type> sysTypes = new List<System.Type>();
            sysTypes.Add(typeof(Wall));

            IList<Element> elems = _CmpElements.GetElementsDoc(null,
                                                                                            sysTypes,
                                                                                            null,
                                                                                            null,
                                                                                            null);
            foreach (Element elem in elems)
            {
                // 要素レベル
                bool isLevel = false;
                Level elemLevel = _CmpElements.GetElementLevel(elem);
                if (elemLevel != null)
                {
                    var idElemLevel = elemLevel.Id.Value;

                    // レベル比較
                    if (idLevel == idElemLevel)
                    {
                        isLevel = true;
                    }
                }

                // 要素追加
                if (isLevel == true)
                {
                    elemsAg.Add(elem);
                }
                else
                {
                    elemsDf.Add(elem);
                }
            }
        }

        /// ================================================================================
        /// <summary>柱取得</summary>
        ///
        /// <param name="level"   >レベル</param>
        /// <param name="elemsAg" >要素 - 一致</param>
        /// <param name="elemsDf" >要素 - 非一致</param>
        ///
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetColumns(Level level,
                        ref IList<Element> elemsAg,
                        ref IList<Element> elemsDf)
        {
            // レベルID
            var idLevel = level.Id.Value;

            // 要素
            IList<System.Type> sysTypes = new List<System.Type>();
            sysTypes.Add(typeof(FamilyInstance));

            IList<Category> categories = new List<Category>();
            categories.Add(_CmpElements.GetCategory(BuiltInCategory.OST_Columns));
            categories.Add(_CmpElements.GetCategory(BuiltInCategory.OST_StructuralColumns));

            IList<Element> elems = _CmpElements.GetElementsDoc(null,
                                                                                            sysTypes,
                                                                                            categories,
                                                                                            null,
                                                                                            null);

            foreach (Element elem in elems)
            {
                // 要素レベル
                bool isLevel = false;
                Level elemLevel = _CmpElements.GetElementLevel(elem);
                if (elemLevel != null)
                {
                    var idElemLevel = elemLevel.Id.Value;

                    // レベル比較
                    if (idLevel == idElemLevel)
                    {
                        isLevel = true;
                    }
                }

                // 要素追加
                if (isLevel == true)
                {
                    elemsAg.Add(elem);
                }
                else
                {
                    elemsDf.Add(elem);
                }
            }
        }

        /// ================================================================================
        /// <summary>梁取得</summary>
        ///
        /// <param name="levelH"      >レベル - 水平</param>
        /// <param name="levelV"      >レベル - 垂直</param>
        /// <param name="girderAryAg" >大梁 - 一致</param>
        /// <param name="girderAryDf" >大梁 - 非一致</param>
        /// <param name="beamAryAg"   >小梁 - 一致</param>
        /// <param name="beamAryDf"   >小梁 - 非一致</param>
        /// <param name="vbraceAryAg" >鉛直ブレース - 一致</param>
        /// <param name="vbraceAryDf" >鉛直ブレース - 非一致</param>
        /// <param name="hbraceAryAg" >水平ブレース - 一致</param>
        /// <param name="hbraceAryDf" >水平ブレース - 非一致</param>
        ///
        /// <history><p>2011/11/27 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public void GetBeams(Level levelH,
                      Level levelV,
                      ref IList<Element> girderAryAg,
                      ref IList<Element> girderAryDf,
                      ref IList<Element> beamAryAg,
                      ref IList<Element> beamAryDf,
                      ref IList<Element> vbraceAryAg,
                      ref IList<Element> vbraceAryDf,
                      ref IList<Element> hbraceAryAg,
                      ref IList<Element> hbraceAryDf)
        {
            // レベルID
            var idLevelH = levelH.Id.Value;
            var idLevelV = levelV.Id.Value;

            // 要素
            IList<System.Type> sysTypes = new List<System.Type>();
            sysTypes.Add(typeof(FamilyInstance));

            IList<Category> categories = new List<Category>();
            categories.Add(_CmpElements.GetCategory(BuiltInCategory.OST_StructuralFraming));

            IList<Element> elems = _CmpElements.GetElementsDoc(null,
                                                                                            sysTypes,
                                                                                            categories,
                                                                                            null,
                                                                                            null);

            foreach (Element elem in elems)
            {
                // 要素レベル
                bool isLevelH = false;
                bool isLevelV = false;

                // 参照レベル
                ElementId levelElemId = null;
                if (_CmpParameters.GetValue(elem,
                                            BuiltInParameter.INSTANCE_REFERENCE_LEVEL_PARAM,
                                            ref levelElemId) < -1)
                {
                }
                if (levelElemId != null)
                {
                    var idElemLevel = levelElemId.Value;

                    // レベル比較
                    if (idLevelH == idElemLevel)
                    {
                        isLevelH = true;
                    }
                    if (idLevelV == idElemLevel)
                    {
                        isLevelV = true;
                    }
                }

                // 構造用途
                int iStruct = 0;
                if (_CmpParameters.GetValue(elem,
                                            BuiltInParameter.INSTANCE_STRUCT_USAGE_PARAM,
                                            ref iStruct) < -1)
                {
                }

                switch (iStruct)
                {
                    // 大梁
                    case 3:
                        if (isLevelH == true)
                        {
                            girderAryAg.Add(elem);
                        }
                        else
                        {
                            girderAryDf.Add(elem);
                        }
                        break;

                    // 小梁
                    case 4:
                        if (isLevelH == true)
                        {
                            beamAryAg.Add(elem);
                        }
                        else
                        {
                            beamAryDf.Add(elem);
                        }
                        break;

                    // 鉛直ブレース
                    case 7:
                        if (isLevelV == true)
                        {
                            vbraceAryAg.Add(elem);
                        }
                        else
                        {
                            vbraceAryDf.Add(elem);
                        }
                        break;

                    // 水平ブレース
                    case 8:
                        if (isLevelH == true)
                        {
                            hbraceAryAg.Add(elem);
                        }
                        else
                        {
                            hbraceAryDf.Add(elem);
                        }
                        break;
                }
            }
        }

        /// ================================================================================
        /// <summary>スラブ取得</summary>
        ///
        /// <param name="level"   >レベル</param>
        /// <param name="elemsAg" >要素 - 一致</param>
        /// <param name="elemsDf" >要素 - 非一致</param>
        ///
        /// <history><p>2011/11/27 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public void GetSlabs(Level level,
                      ref IList<Element> elemsAg,
                      ref IList<Element> elemsDf)
        {
            // レベルID
            var idLevel = level.Id.Value;

            // 要素
            IList<System.Type> sysTypes = new List<System.Type>();
            sysTypes.Add(typeof(Floor));

            IList<Element> elems = _CmpElements.GetElementsDoc(null,
                                                                                            sysTypes,
                                                                                            null,
                                                                                            null,
                                                                                            null);

            foreach (Element elem in elems)
            {
                // 構造用途
                bool isStruct = false;
                if (_CmpParameters.GetValue(elem,
                                            BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL,
                                            ref isStruct) < -1)
                {
                }
                if (isStruct == true)
                {
                    // 要素レベル
                    bool isLevel = false;
                    Level elemLevel = _CmpElements.GetElementLevel(elem);
                    if (elemLevel != null)
                    {
                        var idElemLevel = elemLevel.Id.Value;

                        // レベル比較
                        if (idLevel == idElemLevel)
                        {
                            isLevel = true;
                        }
                    }

                    // 要素追加
                    if (isLevel == true)
                    {
                        elemsAg.Add(elem);
                    }
                    else
                    {
                        elemsDf.Add(elem);
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>ビュー処理</summary>
        ///
        /// <param name="activeView">アクティブビュー</param>
        /// <param name="entDtLevel">データテーブル - レベル</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/11/27 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/10/28 Modified Applied Technology</p></history>
        /// ================================================================================
        public bool WorkView(ViewPlan activeView,
                      RvtExtApp.Entities.DtLevel entDtLevel)
        {
            // 戻り値
            bool ret = false;
            _ErrMsg = "";

            // レベル
            Level levelBase = entDtLevel.BaseLevel;
            Level levelWork = entDtLevel.WorkLevel;
            if ((levelBase == null) || (levelWork == null))
            {
                _ErrMsg = _CmpAttribute.ResourceText("IDS_ERR_WORKVIEW");
                return ret;
            }

            // 柱
            IList<Element> columnAryAg = new List<Element>();
            IList<Element> columnAryDf = new List<Element>();
            GetColumns(levelBase, ref columnAryAg, ref columnAryDf);

            // 壁
            IList<Element> wallAryAg = new List<Element>();
            IList<Element> wallAryDf = new List<Element>();
            GetWalls(levelBase, ref wallAryAg, ref wallAryDf);

            // 梁
            IList<Element> girderAryAg = new List<Element>();
            IList<Element> girderAryDf = new List<Element>();
            IList<Element> beamAryAg = new List<Element>();
            IList<Element> beamAryDf = new List<Element>();
            IList<Element> vbraceAryAg = new List<Element>();
            IList<Element> vbraceAryDf = new List<Element>();
            IList<Element> hbraceAryAg = new List<Element>();
            IList<Element> hbraceAryDf = new List<Element>();
            GetBeams(levelWork,
                     levelBase,
                     ref girderAryAg,
                     ref girderAryDf,
                     ref beamAryAg,
                     ref beamAryDf,
                     ref vbraceAryAg,
                     ref vbraceAryDf,
                     ref hbraceAryAg,
                     ref hbraceAryDf);

            // スラブ
            IList<Element> slabAryAg = new List<Element>();
            IList<Element> slabAryDf = new List<Element>();
            GetSlabs(levelWork, ref slabAryAg, ref slabAryDf);

            // ビュー設定
            IList<ElementId> elemIdsHide = new List<ElementId>();
            IList<ElementId> elemIdsUnHide = new List<ElementId>();
            // 柱
            for (int i = 0; i < columnAryAg.Count; ++i)
            {
                elemIdsUnHide.Add(columnAryAg[i].Id);
            }

            for (int i = 0; i < columnAryDf.Count; ++i)
            {
                elemIdsHide.Add(columnAryDf[i].Id);
            }

            // 壁
            for (int i = 0; i < wallAryAg.Count; ++i)
            {
                elemIdsUnHide.Add(wallAryAg[i].Id);
            }

            for (int i = 0; i < wallAryDf.Count; ++i)
            {
                elemIdsHide.Add(wallAryDf[i].Id);
            }

            // 大梁
            for (int i = 0; i < girderAryAg.Count; ++i)
            {
                elemIdsUnHide.Add(girderAryAg[i].Id);
            }

            for (int i = 0; i < girderAryDf.Count; ++i)
            {
                elemIdsHide.Add(girderAryDf[i].Id);
            }

            // 小梁
            for (int i = 0; i < beamAryAg.Count; ++i)
            {
                elemIdsUnHide.Add(beamAryAg[i].Id);
            }

            for (int i = 0; i < beamAryDf.Count; ++i)
            {
                elemIdsHide.Add(beamAryDf[i].Id);
            }

            // 水平ブレース
            for (int i = 0; i < hbraceAryAg.Count; ++i)
            {
                elemIdsUnHide.Add(hbraceAryAg[i].Id);
            }

            for (int i = 0; i < hbraceAryDf.Count; ++i)
            {
                elemIdsHide.Add(hbraceAryDf[i].Id);
            }

            // 垂直ブレース
            for (int i = 0; i < vbraceAryAg.Count; ++i)
            {
                elemIdsUnHide.Add(vbraceAryAg[i].Id);
            }

            for (int i = 0; i < vbraceAryDf.Count; ++i)
            {
                elemIdsHide.Add(vbraceAryDf[i].Id);
            }

            // スラブ
            for (int i = 0; i < slabAryAg.Count; ++i)
            {
                elemIdsUnHide.Add(slabAryAg[i].Id);
            }

            for (int i = 0; i < slabAryDf.Count; ++i)
            {
                elemIdsHide.Add(slabAryDf[i].Id);
            }

            // 表示設定
            if (elemIdsUnHide.Count > 0)
            {
                activeView.UnhideElements(elemIdsUnHide);
            }

            // 非表示設定
            if (elemIdsHide.Count > 0)
            {
                activeView.HideElements(elemIdsHide);
            }

            ret = true;
            return ret;
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>エラーメッセージ</summary>
        /// <history>2011/11/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string ErrMsg
        {
            get
            {
                return _ErrMsg;
            }
        }

        #endregion Properties
    }
}
