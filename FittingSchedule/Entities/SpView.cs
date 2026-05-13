using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FittingSchedule;

namespace ADSK.JExtRAC.FittingSchedule.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - ビュー</summary>
    /// ================================================================================
    public class SpView : RvtExtApp.Entities.SpBase
    {
        // メンバ変数

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"  >属性</param>
        /// <param name="cmpParameters" >パラメータ</param>
        /// <param name="cmpSettings"   >設定</param>
        ///
        /// <history>2011/07/28 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public SpView(RvtExtApp.Components.Attribute cmpAttribute,
                      RvtExtApp.Components.Parameters cmpParameters,
                      RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpParameters, cmpSettings)
        {
        }

        #endregion Constructor

        // メンバ関数

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>記号非表示設定</summary>
        /// <history><p>2011/07/28 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        int SignNotDisp
        {
            get
            {
                int ret = 0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.SECTION_COARSER_SCALE_PULLDOWN_METRIC,
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
                                                    Revit.DB.BuiltInParameter.SECTION_COARSER_SCALE_PULLDOWN_METRIC,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>トリミング設定</summary>
        /// <history><p>2011/07/28 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        int FarClipping
        {
            get
            {
                int ret = 0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.VIEWER_BOUND_FAR_CLIPPING,
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
                                                    Revit.DB.BuiltInParameter.VIEWER_BOUND_FAR_CLIPPING,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>シートタイトル名</summary>
        /// <history><p>2011/07/28 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string TitleOnSheet
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.VIEW_DESCRIPTION,
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
                                                    Revit.DB.BuiltInParameter.VIEW_DESCRIPTION,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        #endregion Properties
    }
}
