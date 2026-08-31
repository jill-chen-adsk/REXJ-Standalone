
using System;
using System.Collections.Generic;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using RvtExtApp = ADSK.JExtRAC.AveSiteLevelHeightCalc;
using ADSK.JExtRAC.AveSiteLevelHeightCalc.Entities;


namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Components
{
    /// ================================================================================
    /// <summary>要素</summary>
    /// ================================================================================
    public partial class Elements
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>Circle symbol name</summary>
        private string _SymbolCircle;

        /// <summary>Tag symbol name</summary>
        private string _SymbolTag;

        /// <summary>データフォルダ</summary>
        private string _DataFolder;

        /// <summary>シンボル名-平均地盤面算定ポイント符号</summary>
        private string _SymNameAveGlLevelCalcPosSign;

        /// <summary>シンボル名-既存BGL</summary>
        private string _SymNameCurrentBGL;

        /// <summary>シンボル名-設計GL</summary>
        private string _SymNameDGL;

        /// <summary>シンボル名-縮尺</summary>
        private string _SymNameScale;

        /// <summary>トランザクション</summary>
        public Revit.DB.Transaction trans;

        /// <summary>True: select element then run command, False: Run command</summary>
        public bool _IsSelectElement = false;

        private readonly UIDocument _rvtUIDoc;

        public Document RvtDBDoc { get; private set; }

        #endregion Memeber Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        /// <param name="rvtUIDoc">Revit UIドキュメント</p></param>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Elements(RvtExtApp.Components.Attribute cmpAttribute,
                        Revit.UI.UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            RvtDBDoc = rvtUIDoc.Document;

            _CmpAttribute = cmpAttribute;

            string asmDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string rfaSubDir = System.IO.Path.Combine(asmDir, "rfa2027");
            _DataFolder = System.IO.Directory.Exists(rfaSubDir) ? rfaSubDir : asmDir;

            _SymbolCircle = _CmpAttribute.ResourceText("IDS_SYMCIRCLE");
            _SymbolTag = _CmpAttribute.ResourceText("IDS_SYMTAG");

            _SymNameAveGlLevelCalcPosSign = _CmpAttribute.ResourceText("IDS_SYMNAME_AVEGLLEVELPOINTSIGN");
            _SymNameCurrentBGL = _CmpAttribute.ResourceText("IDS_SYMNAME_CURRENTBGL");
            _SymNameDGL = _CmpAttribute.ResourceText("IDS_SYMNAME_DGL");
            _SymNameScale = _CmpAttribute.ResourceText("IDS_SYMNAME_SCALE");
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>Load symbol and tag to current document</summary>
        ///
        /// <param name="doc"               >Current document</param>
        /// <param name="nScale"            >Scale of current view</param>
        /// <param name="symbolCircle"      >Out family symbol of circle</param>
        /// <param name="symbolTag"         >Out family symbol of tag</param>
        ///
        /// <returns>True or false</returns>
        ///
        /// <history>2021/12/20 Modified Applied Technology</history>
        /// ================================================================================
        public bool LoadSymbolAndTag(Revit.DB.Document doc, int nScale, ref Revit.DB.FamilySymbol symbolCircle, ref Revit.DB.FamilySymbol symbolTag)
        {
            if (nScale <= 0)
                return false;

            string fileNameTag = _DataFolder + "\\" + _SymbolTag + ".rfa";
            string fileNameSymbol = _DataFolder + "\\" + _SymbolCircle + ".rfa";

            string typeName = string.Empty;

            if (nScale < 50)
                typeName = _CmpAttribute.ResourceText("IDS_SYMNAME5.0");
            else if (nScale < 100 && nScale >= 50)
                typeName = _CmpAttribute.ResourceText("IDS_SYMNAME4.0");
            else if (nScale < 500 && nScale >= 100)
                typeName = _CmpAttribute.ResourceText("IDS_SYMNAME3.0");
            else if (nScale < 1000 && nScale >= 500)
                typeName = _CmpAttribute.ResourceText("IDS_SYMNAME2.0");
            else if (nScale >= 1000)
                typeName = _CmpAttribute.ResourceText("IDS_SYMNAME1.0");

            // Load family circle
            symbolCircle = LoadFamilyToCurrentdoc(doc, fileNameSymbol, System.IO.Path.GetFileNameWithoutExtension(fileNameSymbol));

            // Load symbol tag
            symbolTag = LoadFamilyToCurrentdoc(doc, fileNameTag, typeName);

            if (symbolCircle == null || symbolTag == null)
                return false;

            return true;
        }

        /// ================================================================================
        /// <summary>Load family and symbol of tag to current document</summary>
        ///
        /// <param name="doc"           >Current document</param>
        /// <param name="pathFile"      >Path of file .rfa</param>
        /// <param name="symbolName"    >Family type name</param>
        ///
        /// <returns>Symbol</returns>
        ///
        /// <history>2021/12/20 Modified Applied Technology</history>
        /// ================================================================================
        private Revit.DB.FamilySymbol LoadFamilyToCurrentdoc(Revit.DB.Document doc, string pathFile, string symbolName)
        {
            try
            {
                // Search family by name
                string familyName = System.IO.Path.GetFileNameWithoutExtension(pathFile);
                var lstFamilyInProject = new Revit.DB.FilteredElementCollector(doc).OfClass(typeof(Revit.DB.Family))
                                                                                .Cast<Revit.DB.Family>().Where(x => x.Name == familyName).ToList();

                Revit.DB.Family familyLoaded = null;
                if (lstFamilyInProject.Count == 0)
                {
                    if (System.IO.File.Exists(pathFile) == false)
                        return null;

                    if (doc.LoadFamily(pathFile, out familyLoaded) == false)
                        return null;

                    if (familyLoaded == null)
                        return null;
                }
                else
                    familyLoaded = lstFamilyInProject.FirstOrDefault();

                // Find symbol by name; fall back to the first type when names differ from the RFA.
                Revit.DB.FamilySymbol fallbackSymbol = null;
                foreach (Revit.DB.ElementId symbolId in familyLoaded.GetFamilySymbolIds())
                {
                    Revit.DB.FamilySymbol fmSymbol = doc.GetElement(symbolId) as Revit.DB.FamilySymbol;
                    if (fmSymbol == null)
                        continue;

                    if (fallbackSymbol == null)
                        fallbackSymbol = fmSymbol;

                    if (fmSymbol.Name != symbolName)
                        continue;

                    return fmSymbol;
                }

                return fallbackSymbol;
            }
            catch
            {
                return null;
            }
        }

        /// ================================================================================
        /// <summary>シンボルロード - 平均地盤面算定ポイント符号</summary>
        ///
        /// <returns>平均地盤面算定ポイント符号シンボル</returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.AnnotationSymbolType LoadSymbolAveGlLevelCalcPosSign()
        {
            Revit.DB.AnnotationSymbolType ret = null;
            string fileName = _DataFolder + "\\" + _SymNameAveGlLevelCalcPosSign + ".rfa";
            Revit.DB.FamilySymbol famSym = LoadFamilyByFamilyName(fileName, _SymNameAveGlLevelCalcPosSign);
            if (famSym != null)
            {
                ret = famSym as Revit.DB.AnnotationSymbolType;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>シンボルロード - 既存BGL</summary>
        ///
        /// <returns>既存BGLシンボル</returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.AnnotationSymbolType LoadSymbolCurrentBGL()
        {
            Revit.DB.AnnotationSymbolType ret = null;
            string fileName = _DataFolder + "\\" + _SymNameCurrentBGL + ".rfa";
            Revit.DB.FamilySymbol famSym = LoadFamilyByFamilyName(fileName, _SymNameCurrentBGL);
            if (famSym != null)
            {
                ret = famSym as Revit.DB.AnnotationSymbolType;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>シンボルロード - 設計GL</summary>
        ///
        /// <returns>設計GLシンボル</returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.AnnotationSymbolType LoadSymbolDGL()
        {
            Revit.DB.AnnotationSymbolType ret = null;
            string fileName = _DataFolder + "\\" + _SymNameDGL + ".rfa";
            Revit.DB.FamilySymbol famSym = LoadFamilyByFamilyName(fileName, _SymNameDGL);
            if (famSym != null)
            {
                ret = famSym as Revit.DB.AnnotationSymbolType;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>シンボルロード - 縮尺</summary>
        ///
        /// <returns>縮尺シンボル</returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.AnnotationSymbolType LoadSymbolScale()
        {
            Revit.DB.AnnotationSymbolType ret = null;
            string fileName = _DataFolder + "\\" + _SymNameScale + ".rfa";
            Revit.DB.FamilySymbol famSym = LoadFamilyByFamilyName(fileName, _SymNameScale);
            if (famSym != null)
            {
                ret = famSym as Revit.DB.AnnotationSymbolType;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>平均地盤面算定ポイント削除</summary>
        ///
        /// <param name="aveGlLevelCalcPoss">平均地盤面算定ポイント</param>
        ///
        /// <history><p>2011/07/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/12/20 Modified Applied Technology</p></history>
        /// ================================================================================
        public void DelAveGlLevelCalcPos(Collections.Generic.IList<ObjectTag> aveGlLevelCalcPoss)
        {
            Collections.Generic.IList<Revit.DB.ElementId> elemIdAry = new Collections.Generic.List<Revit.DB.ElementId>();
            if (aveGlLevelCalcPoss != null)
            {
                foreach (ObjectTag objTag in aveGlLevelCalcPoss)
                {
                    if (objTag.CircleTag != null)
                        elemIdAry.Add(objTag.CircleTag.Id);

                    if (objTag.Tag != null)
                        elemIdAry.Add(objTag.Tag.Id);
                }
            }
            if (elemIdAry.Count > 0)
            {
                RvtDBDoc.Delete(elemIdAry);
            }
        }

        /// ================================================================================
        /// <summary>平均地盤面算定ポイント要素取得</summary>
        ///
        /// <param name="id">平均地盤面算定ポイント要素ID</param>
        ///
        /// <returns>平均地盤面算定ポイント要素</returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.Element GetAveGlLvlCalcPos(int id)
        {
            return GetElementDocByLegacyId(id);
        }

        /// ================================================================================
        /// <summary>ビューの要素削除</summary>
        ///
        /// <param name="view">ビュー</param>
        ///
        /// <history><p>2011/07/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public void DelElemsView(Revit.DB.View view)
        {
            Collections.Generic.IList<Revit.DB.ElementId> elemIdAry = new Collections.Generic.List<Revit.DB.ElementId>();
            Collections.Generic.IList<Revit.DB.Element> elems = null;

            if (view != null)
            {
                // 注釈
                elems = GetViewElements(view, typeof(Revit.DB.AnnotationSymbol));
                foreach (Revit.DB.Element elem in elems)
                {
                    elemIdAry.Add(elem.Id);
                }

                // 寸法
                elems = GetViewElements(view, typeof(Revit.DB.Dimension));
                foreach (Revit.DB.Element elem in elems)
                {
                    elemIdAry.Add(elem.Id);
                }

                // 詳細線分
                elems = GetViewElements(view, typeof(Revit.DB.DetailLine));
                foreach (Revit.DB.Element elem in elems)
                {
                    elemIdAry.Add(elem.Id);
                }

                // 文字
                elems = GetViewElements(view, typeof(Revit.DB.TextNote));
                foreach (Revit.DB.Element elem in elems)
                {
                    elemIdAry.Add(elem.Id);
                }
            }

            if (elemIdAry.Count > 0)
            {
                RvtDBDoc.Delete(elemIdAry);
            }
        }

        /// ================================================================================
        /// <summary>選択セットのエリア境界線</summary>
        ///
        /// <returns>エリア境界線</returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Collections.Generic.IList<Revit.DB.CurveElement> SelAreaCurve()
        {
            // 戻り値
            Collections.Generic.IList<Revit.DB.CurveElement> ret = new Collections.Generic.List<Revit.DB.CurveElement>();

            // 要素取得
            Collections.Generic.IList<Revit.DB.Category> categories = GetCategoriesList(Revit.DB.BuiltInCategory.OST_AreaSchemeLines);
            Collections.Generic.IList<Revit.DB.Element> elems = GetElementsSelection(null, categories, null, true);

            // Pre-selection is cleared when the ribbon command is clicked; fall back to all
            // area-boundary lines visible in the active area plan.
            if (elems.Count == 0)
            {
                Revit.DB.ViewPlan areaPlan = ActiveViewAreaPlan;
                if (areaPlan != null)
                {
                    var categoryIds = new System.Collections.Generic.HashSet<Revit.DB.ElementId>();
                    foreach (Revit.DB.Category category in categories)
                    {
                        if (category?.Id != null)
                            categoryIds.Add(category.Id);
                    }

                    foreach (Revit.DB.Element elem in GetViewElements(areaPlan, typeof(Revit.DB.CurveElement), null))
                    {
                        if (elem?.Category?.Id == null || !categoryIds.Contains(elem.Category.Id))
                            continue;

                        elems.Add(elem);
                    }
                }
            }

            foreach (Revit.DB.Element elem in elems)
            {
                Revit.DB.CurveElement curveElement = elem as Revit.DB.CurveElement;
                if (curveElement != null)
                {
                    ret.Add(curveElement);
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>平均地盤面算定ポイント</summary>
        ///
        /// <param name="view">ビュー</param>
        ///
        /// <returns>平均地盤面算定ポイント要素</returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/12/20 Modified Applied Technology</p></history>
        /// ================================================================================
        public Collections.Generic.IList<ObjectTag> AveGlLevelCalcPos(Revit.DB.View view)
        {
            Collections.Generic.IList<ObjectTag> ret = new Collections.Generic.List<ObjectTag>();

            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();

            // Find circle in current document
            names.Add(_SymbolCircle);
            Collections.Generic.IList<Revit.DB.Element> elemsCircle = GetViewElements(view,
                                                                                     typeof(Revit.DB.FamilyInstance),
                                                                                     names);

            // Find tag in current document
            names.Clear();
            names.Add(_CmpAttribute.ResourceText("IDS_SYMNAME1.0"));
            names.Add(_CmpAttribute.ResourceText("IDS_SYMNAME2.0"));
            names.Add(_CmpAttribute.ResourceText("IDS_SYMNAME3.0"));
            names.Add(_CmpAttribute.ResourceText("IDS_SYMNAME4.0"));
            names.Add(_CmpAttribute.ResourceText("IDS_SYMNAME5.0"));
            Collections.Generic.IList<Revit.DB.Element> elemstag = GetViewElements(view,
                                                                                     typeof(Revit.DB.IndependentTag),
                                                                                     names);

            // Tag
            foreach (Revit.DB.Element elem in elemstag)
            {
                Revit.DB.IndependentTag tag = elem as Revit.DB.IndependentTag;
                if (tag != null)
                {
                    ObjectTag objTag = new ObjectTag();
                    objTag.Tag = tag;

                    var findElement = elemsCircle.Where(x => x.Id == tag.GetTaggedLocalElementIds().FirstOrDefault()).FirstOrDefault();
                    if (findElement != null)
                        objTag.CircleTag = findElement as Revit.DB.FamilyInstance;

                    ret.Add(objTag);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>製図ビュー</summary>
        ///
        /// <param name="viewName">ビュー名</param>
        ///
        /// <returns>製図ビュー</returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.ViewDrafting GetViewDrafting(string viewName)
        {
            Revit.DB.ViewDrafting ret = null;

            Collections.Generic.IList<System.Type> sysTypes = new Collections.Generic.List<System.Type>();
            sysTypes.Add(typeof(Revit.DB.ViewDrafting));
            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
            names.Add(viewName);

            Collections.Generic.IList<Revit.DB.Element> elems = GetElementsDoc(null,
                                                                                    sysTypes,
                                                                                    null,
                                                                                    names,
                                                                                    null);

            foreach (Revit.DB.Element elem in elems)
            {
                Revit.DB.ViewDrafting view = elem as Revit.DB.ViewDrafting;
                if (view != null)
                {
                    ret = view;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>文字作成 - 位置合わせ(オーバーロード)</summary>
        ///
        /// <param name="view"  >ビュー</param>
        /// <param name="pos"   >作成位置</param>
        /// <param name="strVal">文字値</param>
        ///
        /// <history>2015/09/11 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public Revit.DB.TextNote CreateTextNoteSetPosRotate(Revit.DB.View view,
                                                     Revit.DB.XYZ pos,
                                                     string strVal)
        {
            return CreateTextNoteSetPosRotate(view,
                                              pos,
                                              0,
                                              Revit.DB.HorizontalTextAlignment.Center,
                                              Revit.DB.VerticalTextAlignment.Middle,
                                              null,
                                              strVal);
        }

        /// ================================================================================
        /// <summary>文字作成 - 位置合わせ(オーバーロード)</summary>
        ///
        /// <param name="view"  >ビュー</param>
        /// <param name="pos"   >作成位置</param>
        /// <param name="angle" >角度</param>
        /// <param name="strVal">文字値</param>
        ///
        /// <history>2015/09/11 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public Revit.DB.TextNote CreateTextNoteSetPosRotate(Revit.DB.View view,
                                                     Revit.DB.XYZ pos,
                                                     double angle,
                                                     string strVal)
        {
            return CreateTextNoteSetPosRotate(view,
                                              pos,
                                              angle,
                                              Revit.DB.HorizontalTextAlignment.Center,
                                              Revit.DB.VerticalTextAlignment.Middle,
                                              null,
                                              strVal);
        }

        /// ================================================================================
        /// <summary>文字作成 - 位置合わせ(オーバーロード)</summary>
        ///
        /// <param name="view"                >ビュー</param>
        /// <param name="pos"                 >作成位置</param>
        /// <param name="horizontalTextAlign" >水平位置</param>
        /// <param name="strVal"              >文字値</param>
        ///
        /// <history>2015/09/11 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public Revit.DB.TextNote CreateTextNoteSetPosRotate(Revit.DB.View view,
                                                     Revit.DB.XYZ pos,
                                                     Revit.DB.HorizontalTextAlignment horizontalTextAlign,
                                                     string strVal)
        {
            return CreateTextNoteSetPosRotate(view,
                                              pos,
                                              0,
                                              horizontalTextAlign,
                                              Revit.DB.VerticalTextAlignment.Middle,
                                              null,
                                              strVal);
        }

        /// ================================================================================
        /// <summary>文字作成 - 位置合わせ(オーバーロード)</summary>
        ///
        /// <param name="view"              >ビュー</param>
        /// <param name="pos"               >作成位置</param>
        /// <param name="verticalTextAlign" >垂直位置</param>
        /// <param name="strVal"            >文字値</param>
        ///
        /// <history>2015/09/11 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public Revit.DB.TextNote CreateTextNoteSetPosRotate(Revit.DB.View view,
                                                     Revit.DB.XYZ pos,
                                                     Revit.DB.VerticalTextAlignment verticalTextAlign,
                                                     string strVal)
        {
            return CreateTextNoteSetPosRotate(view,
                                              pos,
                                              0,
                                              Revit.DB.HorizontalTextAlignment.Center,
                                              verticalTextAlign,
                                              null,
                                              strVal);
        }

        /// ================================================================================
        /// <summary>文字作成 - 位置合わせ(オーバーロード)</summary>
        ///
        /// <param name="view"  >ビュー</param>
        /// <param name="pos"   >作成位置</param>
        /// <param name="typeId">文字タイプID</param>
        /// <param name="strVal">文字値</param>
        ///
        /// <history>2015/09/11 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public Revit.DB.TextNote CreateTextNoteSetPosRotate(Revit.DB.View view,
                                                     Revit.DB.XYZ pos,
                                                     Revit.DB.ElementId typeId,
                                                     string strVal)
        {
            return CreateTextNoteSetPosRotate(view,
                                              pos,
                                              0,
                                              Revit.DB.HorizontalTextAlignment.Center,
                                              Revit.DB.VerticalTextAlignment.Middle,
                                              typeId,
                                              strVal);
        }

        /// ================================================================================
        /// <summary>文字作成 - 位置合わせ(オーバーロード)</summary>
        ///
        /// <param name="view"                >ビュー</param>
        /// <param name="pos"                 >作成位置</param>
        /// <param name="angle"               >角度</param>
        /// <param name="horizontalTextAlign" >水平位置</param>
        /// <param name="verticalTextAlign"   >垂直位置</param>
        /// <param name="textNoteTypeId"      >文字タイプID</param>
        /// <param name="strVal"              >文字値</param>
        ///
        /// <history>2015/09/11 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public Revit.DB.TextNote CreateTextNoteSetPosRotate(Revit.DB.View view,
                                                     Revit.DB.XYZ pos,
                                                     double angle,
                                                     Revit.DB.HorizontalTextAlignment horizontalTextAlign,
                                                     Revit.DB.VerticalTextAlignment verticalTextAlign,
                                                     Revit.DB.ElementId textNoteTypeId,
                                                     string strVal)
        {
            // 戻り値
            Revit.DB.TextNote ret = null;

            trans.Start("CreateTextNote");
            // ライブラリで作成
            ret = CreateTextNoteInternal(view,
                                      pos,
                                      horizontalTextAlign,
                                      strVal);
            trans.Commit();
            // Revit2016から新しい文字の縦方向は上基点に固定される

            trans.Start("CreateTextNote");
            // 垂直位置調整
            // 上基点はそのまま
            if (verticalTextAlign != Revit.DB.VerticalTextAlignment.Top)
            {
                Revit.DB.BoundingBoxXYZ bndBox = ret.get_BoundingBox(view);
                //Revit.DB.XYZ min = bndBox.Min;
                Revit.DB.XYZ min = new Revit.DB.XYZ(0, 0, 0);
                if (bndBox != null)
                {
                    min = bndBox.Min;
                }

                // 移動量
                double disY = 0;

                // 中央
                if (verticalTextAlign == Revit.DB.VerticalTextAlignment.Middle)
                {
                    // 半分移動
                    disY = (pos.Y - min.Y) / 2;
                }
                // 下
                else if (verticalTextAlign == Revit.DB.VerticalTextAlignment.Bottom)
                {
                    // 全体移動
                    disY = pos.Y - min.Y;
                }

                Revit.DB.XYZ translate = new Revit.DB.XYZ(0, disY, 0);

                // 移動
                ret.Location.Move(translate);
            }

            // 文字タイプ変更
            if (textNoteTypeId != null)
            {
                try
                {
                    ret.ChangeTypeId(textNoteTypeId);
                }
                catch
                {
                }
            }

            // 回転
            if (angle != 0d)
            {
                Revit.DB.XYZ z = pos + Revit.DB.XYZ.BasisZ;

                // 回転軸
                Revit.DB.Line axis = Revit.DB.Line.CreateBound(pos, z);

                // 回転
                ret.Location.Rotate(axis, angle);
            }

            trans.Commit();

            return ret;
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>アクティブなエリア平面図ビュー</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.ViewPlan ActiveViewAreaPlan
        {
            get
            {
                Revit.DB.ViewPlan viewPlan = null;
                Revit.DB.View activeView = GetActiveView(Revit.DB.ViewType.AreaPlan);
                if (activeView != null)
                {
                    viewPlan = activeView as Revit.DB.ViewPlan;
                }
                return viewPlan;
            }
        }

        /// ================================================================================
        /// <summary>シンボル-平均地盤面算定ポイント符号</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.AnnotationSymbolType SymbolAveGlLevelCalcPosSign
        {
            get
            {
                Revit.DB.AnnotationSymbolType ret = null;

                Collections.Generic.IList<System.Type> sysTypes = new Collections.Generic.List<System.Type>();
                sysTypes.Add(typeof(Revit.DB.AnnotationSymbolType));

                Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                names.Add(_SymNameAveGlLevelCalcPosSign);

                Collections.Generic.IList<Revit.DB.Element> elems = GetElementsDoc(new Revit.DB.ElementIsElementTypeFilter(),
                                                                                        sysTypes,
                                                                                        null,
                                                                                        names,
                                                                                        null);
                if (elems.Count > 0)
                {
                    Revit.DB.AnnotationSymbolType antatSymType = elems[0] as Revit.DB.AnnotationSymbolType;
                    if (antatSymType != null)
                    {
                        ret = antatSymType;
                    }
                }
                return ret;
            }
        }

        /// ================================================================================
        /// <summary>シンボル-既存BGL</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.AnnotationSymbolType SymbolCurrentBGL
        {
            get
            {
                Revit.DB.AnnotationSymbolType ret = null;

                Collections.Generic.IList<System.Type> sysTypes = new Collections.Generic.List<System.Type>();
                sysTypes.Add(typeof(Revit.DB.AnnotationSymbolType));

                Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                names.Add(_SymNameCurrentBGL);

                Collections.Generic.IList<Revit.DB.Element> elems = GetElementsDoc(new Revit.DB.ElementIsElementTypeFilter(),
                                                                                        sysTypes,
                                                                                        null,
                                                                                        names,
                                                                                        null);
                if (elems.Count > 0)
                {
                    Revit.DB.AnnotationSymbolType antatSymType = elems[0] as Revit.DB.AnnotationSymbolType;
                    if (antatSymType != null)
                    {
                        ret = antatSymType;
                    }
                }
                return ret;
            }
        }

        /// ================================================================================
        /// <summary>シンボル-設計GL</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.AnnotationSymbolType SymbolDGL
        {
            get
            {
                Revit.DB.AnnotationSymbolType ret = null;

                Collections.Generic.IList<System.Type> sysTypes = new Collections.Generic.List<System.Type>();
                sysTypes.Add(typeof(Revit.DB.AnnotationSymbolType));

                Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                names.Add(_SymNameDGL);

                Collections.Generic.IList<Revit.DB.Element> elems = GetElementsDoc(new Revit.DB.ElementIsElementTypeFilter(),
                                                                                        sysTypes,
                                                                                        null,
                                                                                        names,
                                                                                        null);
                if (elems.Count > 0)
                {
                    Revit.DB.AnnotationSymbolType antatSymType = elems[0] as Revit.DB.AnnotationSymbolType;
                    if (antatSymType != null)
                    {
                        ret = antatSymType;
                    }
                }
                return ret;
            }
        }

        /// ================================================================================
        /// <summary>シンボル-縮尺</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Revit.DB.AnnotationSymbolType SymbolScale
        {
            get
            {
                Revit.DB.AnnotationSymbolType ret = null;

                Collections.Generic.IList<System.Type> sysTypes = new Collections.Generic.List<System.Type>();
                sysTypes.Add(typeof(Revit.DB.AnnotationSymbolType));

                Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                names.Add(_SymNameScale);

                Collections.Generic.IList<Revit.DB.Element> elems = GetElementsDoc(new Revit.DB.ElementIsElementTypeFilter(),
                                                                                        sysTypes,
                                                                                        null,
                                                                                        names,
                                                                                        null);
                if (elems.Count > 0)
                {
                    Revit.DB.AnnotationSymbolType antatSymType = elems[0] as Revit.DB.AnnotationSymbolType;
                    if (antatSymType != null)
                    {
                        ret = antatSymType;
                    }
                }
                return ret;
            }
        }

        /// ================================================================================
        /// <summary>地形面</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Collections.Generic.IList<Revit.DB.Architecture.TopographySurface> TopoSurface
        {
            get
            {
                Collections.Generic.IList<Revit.DB.Architecture.TopographySurface> ret =
                    new Collections.Generic.List<Revit.DB.Architecture.TopographySurface>();

                Collections.Generic.IList<Revit.DB.Element> elems = GetElementsDoc(typeof(Revit.DB.Architecture.TopographySurface));

                foreach (Revit.DB.Element elem in elems)
                {
                    Revit.DB.Architecture.TopographySurface topoSurface = elem as Revit.DB.Architecture.TopographySurface;
                    if (topoSurface != null)
                    {
                        ret.Add(topoSurface);
                    }
                }
                return ret;
            }
        }

        /// <summary>ドキュメント内のToposolidをリストで取得</summary>
        public IList<Toposolid> TopoSolids => new FilteredElementCollector(RvtDBDoc).OfClass(typeof(Toposolid)).OfType<Toposolid>().ToList();
        /// ================================================================================
        /// <summary>Create a new text note type </summary>
        /// <history>2021/11/08 Created by AT</history>
        /// <returns>A new element type ID</returns>
        /// ================================================================================
        public Revit.DB.ElementId CreateTextNoteType()
        {
            try
            {
                string textNoteTypeName = "ＭＳ ゴシック";

                Revit.DB.TextNoteType textNoteType = new Revit.DB.FilteredElementCollector(RvtDBDoc).OfClass(typeof(Revit.DB.TextNoteType))
                                                        .Cast<Revit.DB.TextNoteType>().Where(q => q.Name == textNoteTypeName).FirstOrDefault();

                if (textNoteType != null)
                {
                    return textNoteType.Id;
                }
                else //Create new
                {
                    Revit.DB.TextNoteType toDuplicate = null;

                    //Check exist [2mm Arial]
                    string arial2mm = "2mm Arial";
                    Revit.DB.TextNoteType textNoteTypeArial = new Revit.DB.FilteredElementCollector(RvtDBDoc).
                        OfClass(typeof(Revit.DB.TextNoteType)).
                        Cast<Revit.DB.TextNoteType>().
                        Where(q => q.Name == arial2mm).
                        FirstOrDefault();

                    if (textNoteTypeArial != null)
                    {
                        toDuplicate = textNoteTypeArial;
                    }
                    else
                    {
                        //Get first element
                        var coll = new Revit.DB.FilteredElementCollector(RvtDBDoc).OfClass(typeof(Revit.DB.TextNoteType));
                        if (coll.GetElementCount() != 0)
                        {
                            toDuplicate = coll.FirstElement() as Revit.DB.TextNoteType;
                        }
                    }

                    if (toDuplicate != null)
                    {
                        trans.Start("Create new text note type");

                        Revit.DB.TextNoteType noteType = toDuplicate.Duplicate(textNoteTypeName) as Revit.DB.TextNoteType;

                        if (null != noteType)
                        {
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.LINE_COLOR).Set(0);
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.LINE_PEN).Set(1);

                            noteType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_BACKGROUND).Set(0);
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_BOX_VISIBILITY).Set(0);
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.LEADER_OFFSET_SHEET).Set(0.00666666666666667);//2.0320 mm
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.LEADER_ARROWHEAD).Set(1);

                            noteType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_FONT).Set(textNoteTypeName);
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).Set(0.00656167979002625); //2mm
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_TAB_SIZE).Set(0.0328083989501312);//10mm

                            noteType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_STYLE_BOLD).Set(0);
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_STYLE_ITALIC).Set(0);
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_STYLE_UNDERLINE).Set(0);
                            noteType.get_Parameter(Revit.DB.BuiltInParameter.TEXT_WIDTH_SCALE).Set(1);
                        }

                        trans.Commit();

                        return noteType.Id;
                    }
                }
            }
            catch (System.Exception ex)
            {
                string mess = ex.Message;
            }
            finally
            {
                if (trans.HasStarted())
                    trans.RollBack();
            }
            return Revit.DB.ElementId.InvalidElementId;
        }

        public Revit.DB.ProjectInfo ProjectInfo => RvtDBDoc.ProjectInformation;

        public void ReleaseElementsSelection()
        {
            _rvtUIDoc.Selection.SetElementIds(new Collections.Generic.List<ElementId>());
        }

        #endregion Properties
    }
}