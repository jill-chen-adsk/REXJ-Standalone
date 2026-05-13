using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AveSiteLevelHeightCalc;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - 寸法タイプ</summary>
    /// ================================================================================
    public class SpDimType : RvtExtApp.Entities.SpBase
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
        /// <history>2011/07/31 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public SpDimType(RvtExtApp.Components.Attribute cmpAttribute,
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
        /// <summary>補助線タイプ</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        int AuxLineType
        {
            set
            {
                if (base.CurrentElem != null)
                {
                    base.CmpParameters.SetValue(base.CurrentElem,
                                                Revit.DB.BuiltInParameter.DIM_WITNS_LINE_CNTRL,
                                                value);
                }
            }
        }

        /// ================================================================================
        /// <summary>補助線長さ</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        double AuxLineLength
        {
            set
            {
                if (base.CurrentElem != null)
                {
                    base.CmpParameters.SetValue(base.CurrentElem,
                                                Revit.DB.BuiltInParameter.DIM_WITNS_LINE_EXTENSION_BELOW,
                                                value);
                }
            }
        }

        /// ================================================================================
        /// <summary>補助線延長長さ</summary>
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public
        double AuxLineExtensionLength
        {
            set
            {
                if (base.CurrentElem != null)
                {
                    base.CmpParameters.SetValue(base.CurrentElem,
                                                Revit.DB.BuiltInParameter.WITNS_LINE_EXTENSION,
                                                value);
                }
            }
        }

        #endregion Properties
    }
}