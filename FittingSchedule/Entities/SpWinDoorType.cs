using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.FittingSchedule;

namespace ADSK.JExtRAC.FittingSchedule.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - 建具</summary>
    /// ================================================================================
    public class SpWinDoorType : RvtExtApp.Entities.SpBase
    {
        // メンバ変数
        #region Memeber Variables

        /// <summary>定義値 記号</summary>
        private ParamDefStrc _DefValMark;

        /// <summary>定義値 番号</summary>
        private ParamDefStrc _DefValNo;

        #endregion Memeber Variables

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
        public SpWinDoorType(RvtExtApp.Components.Attribute cmpAttribute,
                             RvtExtApp.Components.Parameters cmpParameters,
                             RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpParameters, cmpSettings)
        {
            // 定義設定
            SetDef();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>定義設定</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/27 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void SetDef()
        {
            // 初期化
            bool success = true;

            // 記号
            if (success == true)
            {
                success = DefMark();
            }

            // 番号
            if (success == true)
            {
                success = DefNo();
            }

            // 実行状態
            base.DefSuccess = success;
        }

        /// ================================================================================
        /// <summary>定義 記号</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefMark()
        {
            _DefValMark = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_MARK"),
                                                  Revit.DB.SpecTypeId.String.Text,
                                                  new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValMark.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 番号</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/28 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefNo()
        {
            _DefValNo = new ParamDefStrc(base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_NUMBER"),
                                                Revit.DB.SpecTypeId.String.Text,
                                                new Revit.DB.ForgeTypeId(string.Empty));

            bool ret = true;
            if (ret == false)
            {
                base.ErrDefName = _DefValNo.DefName;
            }
            return ret;
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>記号</summary>
        /// <history><p>2011/07/28 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string Mark
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValMark.DefName,
                                                    _DefValMark.ParamType,
                                                    _DefValMark.BltParamGroup,
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
                                                    _DefValMark.DefName,
                                                    _DefValMark.ParamType,
                                                    _DefValMark.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>番号</summary>
        /// <history><p>2011/07/28 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/05/01 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        string No
        {
            get
            {
                string ret = "";
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValNo.DefName,
                                                    _DefValNo.ParamType,
                                                    _DefValNo.BltParamGroup,
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
                                                    _DefValNo.DefName,
                                                    _DefValNo.ParamType,
                                                    _DefValNo.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        #endregion Properties
    }
}
