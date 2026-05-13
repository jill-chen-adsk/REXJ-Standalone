using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitMEPAddin.Common
{
    public class WrpViews
    {
        // メンバ変数
        #region Memeber Variables
        private UIDocument uidoc;
        private Document doc;
        private Logger log;
        #endregion

        // コンストラクタ
        #region Constructor
        public WrpViews(UIDocument uidoc, Logger log)
        {
            this.uidoc = uidoc;
            this.doc = uidoc.Document;
            this.log = log;
        }
        #endregion

        // メンバ関数
        #region Member Functions

        /// <summary>
        /// ビューが平面であるかどうか
        /// ※ただし、ビューの指定がない場合は、
        /// 　アクティブビューに関する結果を返す。
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public bool IsViewPlan(View view = null)
        {
            if (view == null)
            {
                // ビューの指定がない場合
                // アクティブビュー
                view = doc.ActiveView;
            }

            return view is ViewPlan;
        }

        /// <summary>
        /// SketchPlane設定を
        /// 指定の点を通り、指定されたビューの視点方向を法線ベクトルとする面に
        /// 変更する。
        /// ※ただし、ビューの指定がなければ、アクティブビュー
        /// 　　　　　点の指定がなければ、原点とする。
        /// </summary>
        /// <param name="view"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public SketchPlane SetSketchPlane(View view = null, XYZ pt = null)
        { 
            if(view == null)
            {
                view = doc.ActiveView;
            }
            if (pt == null)
            {
                pt = new XYZ();
            }
            XYZ vec = view.ViewDirection;
            return SetSketchPlane(vec, pt);
        }
    
        public SketchPlane SetSketchPlane(XYZ vec, XYZ pt)
        {
            try
            {
                // 原点を通り、ビューの始点方向を法線ベクトルとする面
                Plane plane = Plane.CreateByNormalAndOrigin(vec, pt);
                SketchPlane sketch = SketchPlane.Create(doc, plane);
                uidoc.ActiveView.SketchPlane = sketch;
                log.Info("point" + pt + " direction" + vec + "で作成された平面をSketchPlaneにセットしました。");
                return sketch;
            }
            catch (Exception ex)
            {
                //TODO エラーログ, エラー処理検討
                log.Error("SketchPlaneがセットできませんでした。");
                log.Error("" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// ビューの参照レベルの名前取得
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public string GetViewLevelName(View view)
        {
            return view.GenLevel.Name;
        }

        /// <summary>
        /// ビューの参照レベルのプロジェクト基準点からの高さ取得
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        public double GetViewLevelElevation(View view)
        {
            return view.GenLevel.ProjectElevation;
        }
        #endregion

        // プロパティ
        #region Properties
        #endregion
    }
}
