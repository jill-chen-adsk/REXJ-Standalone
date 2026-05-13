using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JExtComCompat;

namespace RSTExtension.Components
{
    /// ================================================================================
    /// <summary>要素</summary>
    /// ================================================================================
    public class ESM_Elements : RvtElements
    {
        // メンバ変数

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="rvtUIDoc">Revit UIドキュメント</param>
        ///
        /// <history>2011/12/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public ESM_Elements(UIDocument rvtUIDoc) : base(rvtUIDoc)
        {
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>要素タイプ取得</summary>
        ///
        /// <param name="elem">要素</param>
        ///
        /// <returns>要素タイプ</returns>
        ///
        /// <history><p>2011/12/05 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        Element GetElemType(Element elem)
        {
            // 戻り値
            Element ret = null;

            // 要素タイプ
            ret = GetElementType(elem);

            if (ret == null)
            {
                ElementId elemTypeId = elem.GetTypeId();
                if (elemTypeId != null)
                {
                    ret = RvtDBDoc.GetElement(elemTypeId);
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>タグ要素取得</summary>
        ///
        /// <param name="doc">Document</param>
        /// <param name="builtInCategory">BuiltInCategory</param>
        /// <returns>タグ要素</returns>
        ///
        /// <history><p>2011/12/07 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/10/12 Created  Applied Technology</p><history>
        /// ================================================================================
        public
        IList<Element> GetElemTag(Document doc, BuiltInCategory builtInCategory)
        {
            IList<Element> retVal = new List<Element>();
            var col = new FilteredElementCollector(doc);
            var elems = col.OfCategory(builtInCategory)
                           .WhereElementIsNotElementType().OfClass(typeof(IndependentTag)).ToList();

            foreach (var ele in elems)
            {
                if (ele == null)
                    continue;

                //Independent tag
                IndependentTag tag = ele as IndependentTag;
                if (tag == null)
                    continue;

                retVal.Add(tag);
            }

            return retVal;
        }

        /// ================================================================================
        /// <summary>製図ビュー</summary>
        ///
        /// <param name="viewName">ビュー名</param>
        ///
        /// <returns>製図ビュー</returns>
        ///
        /// <history>2011/12/10 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
      ViewDrafting GetViewDrafting(string viewName)
        {
            ViewDrafting ret = null;

            IList<System.Type> sysTypes = new List<System.Type>();
            sysTypes.Add(typeof(ViewDrafting));
            IList<string> names = new List<string>();
            names.Add(viewName);

            IList<Element> elems = GetElementsDoc(null,
                                                                                    sysTypes,
                                                                                    null,
                                                                                    names,
                                                                                    null);

            foreach (Element elem in elems)
            {
                ViewDrafting view = elem as ViewDrafting;
                if (view != null)
                {
                    ret = view;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>ビューの要素削除</summary>
        ///
        /// <param name="view">ビュー</param>
        ///
        /// <history><p>2011/12/10 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        void DelElemsView(View view)
        {
            IList<ElementId> delElemIds = new List<ElementId>();
            IList<Element> elems = null;

            if (view != null)
            {
                // 注釈
                elems = GetViewElements(view, typeof(AnnotationSymbol));
                foreach (Element elem in elems)
                {
                    delElemIds.Add(elem.Id);
                }

                // 寸法
                elems = GetViewElements(view, typeof(Dimension));
                foreach (Element elem in elems)
                {
                    delElemIds.Add(elem.Id);
                }

                // 詳細線分
                elems = GetViewElements(view, typeof(DetailLine));
                foreach (Element elem in elems)
                {
                    delElemIds.Add(elem.Id);
                }

                // 文字
                elems = GetViewElements(view, typeof(TextNote));
                foreach (Element elem in elems)
                {
                    delElemIds.Add(elem.Id);
                }
            }

            if (delElemIds.Count > 0)
            {
                RvtDBDoc.Delete(delElemIds);
            }
        }

        /// ================================================================================
        /// <summary>文字作成 + 位置・傾き調整（作成時に回転しているものには非対応）</summary>
        ///
        /// <param name="view"            >ビュー</param>
        /// <param name="pos"             >基点</param>
        /// <param name="horizontalAlign" >横方向位置</param>
        /// <param name="text"            >文字</param>
        /// <param name="angle"           >回転角度</param>
        /// <param name="vertical"        ><p>縦方向位置</p>
        ///                                 <p>0 = 上</p>
        ///                                 <p>1 = 中央</p>
        ///                                 <p>2 = 下</p></param>
        ///
        /// <history>2015/05/14 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        TextNote CreateTextNotePosRotate(View view,
                                                  XYZ pos,
                                                  HorizontalTextAlignment horizontalAlign,
                                                  string text,
                                                  double angle,
                                                  int vertical)
        {
            // Revit2016は文字の整列位置が横方向しか指定できなくなった
            // 縦方向は上辺で固定なのでY軸上で移動させる
            // 回転を作成時に行うと、上辺基準で回ってしまい面倒なので、作成移動後に行う

            // 作成
            TextNote ret = CreateTextNote(view, pos, horizontalAlign, text);

            if (ret == null)
            {
                return ret;
            }

            // 外形
            BoundingBoxXYZ bndBoxXYZ = ret.get_BoundingBox(view);
            XYZ min = bndBoxXYZ.Min;

            double dis = 0;

            if (vertical == 1)
            {
                // 中間まで = (上辺 - 下辺) / 2
                dis = (pos.Y - min.Y) / 2;
            }
            else if (vertical == 2)
            {
                // 下辺まで
                dis = pos.Y - min.Y;
            }

            // 移動量
            XYZ translation = new XYZ(0, dis, 0);

            if (vertical != 0)
            {
                // 移動
                ret.Location.Move(translation);
            }

            XYZ p0 = pos;
            XYZ p1 = p0 + new XYZ(0, 0, 1);

            // 回転軸
            Line axis = Line.CreateBound(p0, p1);

            if (angle != 0d)
            {
                // 回転
                ret.Location.Rotate(axis, angle);
            }

            return ret;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>壁タグ</summary>
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<Element> WallTag
        {
            get
            {
                // 戻り値
                IList<Element> ret = new List<Element>();

                // 要素
                IList<System.Type> sysTypes = new List<System.Type>();
                sysTypes.Add(typeof(IndependentTag));

                IList<Category> categories = new List<Category>();
                categories.Add(GetCategory(BuiltInCategory.OST_WallTags));

                IList<Element> elems = GetElementsDoc(null,
                                                                                        sysTypes,
                                                                                        categories,
                                                                                        null,
                                                                                        null);
                foreach (Element elem in elems)
                {
                    ret.Add(elem);
                }
                return ret;
            }
        }

        /// ================================================================================
        /// <summary>柱タグ</summary>
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<Element> ColumnTag
        {
            get
            {
                // 戻り値
                IList<Element> ret = new List<Element>();

                // 要素
                IList<System.Type> sysTypes = new List<System.Type>();
                sysTypes.Add(typeof(IndependentTag));

                IList<Category> categories = new List<Category>();
                categories.Add(GetCategory(BuiltInCategory.OST_StructuralColumnTags));

                IList<Element> elems = GetElementsDoc(null,
                                                                                        sysTypes,
                                                                                        categories,
                                                                                        null,
                                                                                        null);
                foreach (Element elem in elems)
                {
                    ret.Add(elem);
                }
                return ret;
            }
        }

        /// ================================================================================
        /// <summary>梁タグ</summary>
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<Element> BeamTag
        {
            get
            {
                // 戻り値
                IList<Element> ret = new List<Element>();

                // 要素
                IList<System.Type> sysTypes = new List<System.Type>();
                sysTypes.Add(typeof(IndependentTag));

                IList<Category> categories = new List<Category>();
                categories.Add(GetCategory(BuiltInCategory.OST_StructuralFramingTags));

                IList<Element> elems = GetElementsDoc(null,
                                                                                        sysTypes,
                                                                                        categories,
                                                                                        null,
                                                                                        null);
                foreach (Element elem in elems)
                {
                    ret.Add(elem);
                }
                return ret;
            }
        }

        /// ================================================================================
        /// <summary>床タグ</summary>
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<Element> FloorTag
        {
            get
            {
                // 戻り値
                IList<Element> ret = new List<Element>();

                // 要素
                IList<System.Type> sysTypes = new List<System.Type>();
                sysTypes.Add(typeof(IndependentTag));

                IList<Category> categories = new List<Category>();
                categories.Add(GetCategory(BuiltInCategory.OST_FloorTags));

                IList<Element> elems = GetElementsDoc(null,
                                                                                        sysTypes,
                                                                                        categories,
                                                                                        null,
                                                                                        null);
                foreach (Element elem in elems)
                {
                    ret.Add(elem);
                }
                return ret;
            }
        }

        /// ================================================================================
        /// <summary>基礎タグ</summary>
        /// <history>2011/12/05 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        IList<Element> FoundationTag
        {
            get
            {
                // 戻り値
                IList<Element> ret = new List<Element>();

                // 要素
                IList<System.Type> sysTypes = new List<System.Type>();
                sysTypes.Add(typeof(IndependentTag));

                IList<Category> categories = new List<Category>();
                categories.Add(GetCategory(BuiltInCategory.OST_StructuralFoundationTags));

                IList<Element> elems = GetElementsDoc(null,
                                                                                        sysTypes,
                                                                                        categories,
                                                                                        null,
                                                                                        null);
                foreach (Element elem in elems)
                {
                    ret.Add(elem);
                }
                return ret;
            }
        }

        #endregion Properties
    }
}
