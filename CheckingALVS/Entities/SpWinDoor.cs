
using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Entities
{
    /// ================================================================================
    /// <summary>共有パラメータ - 建具</summary>
    /// ================================================================================
    public class SpWinDoor : RvtExtApp.Entities.SpBase
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>定義値 縁側</summary>
        private ParamDefStrc _DefValVeranda;

        /// <summary>定義値 道路面</summary>
        private ParamDefStrc _DefValRoadSide;

        /// <summary>定義値 水平距離</summary>
        private ParamDefStrc _DefValHorizontalDist;

        /// <summary>定義値 水平測定距離</summary>
        private ParamDefStrc _DefValHorizontalMeasDist;

        /// <summary>定義値 水平補正距離</summary>
        private ParamDefStrc _DefValHorizontalCorrDist;

        /// <summary>定義値 垂直距離</summary>
        private ParamDefStrc _DefValVerticalDist;

        /// <summary>定義値 垂直測定距離</summary>
        private ParamDefStrc _DefValVerticalMeasDist;

        /// <summary>定義値 垂直補正距離</summary>
        private ParamDefStrc _DefValVerticalCorrDist;

        /// <summary>定義値 排煙有効高さ</summary>
        private ParamDefStrc _DefValUsableHeightSmoke;

        /// <summary>定義値 天端高さ</summary>
        private ParamDefStrc _DefValHeadHeight;

        /// <summary>定義値 天井高さ</summary>
        private ParamDefStrc _DefValCeilingHeight;

        /// <summary>定義値 防煙壁長さ</summary>
        private ParamDefStrc _DefValSmokeWallLength;

        /// <summary>パラメータカテゴリ</summary>
        private Collections.Generic.IList<Revit.DB.Category> _ParamCategories;

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
        /// <history>2011/07/29 Created  GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public SpWinDoor(RvtExtApp.Components.Attribute cmpAttribute,
                         RvtExtApp.Components.Parameters cmpParameters,
                         RvtExtApp.Components.Settings cmpSettings) :
               base(cmpAttribute, cmpParameters, cmpSettings)
        {
            // パラメータカテゴリ
            _ParamCategories = base.CmpSettings.CategoryWinDoor;
            base.SetDefCatName(_ParamCategories);

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

            // 縁側
            if (success == true)
            {
                success = DefVeranda();
            }

            // 道路面
            if (success == true)
            {
                success = DefRoadSide();
            }

            // 水平距離
            if (success == true)
            {
                success = DefHorizontalDist();
            }

            // 水平測定距離
            if (success == true)
            {
                success = DefHorizontalMeasDist();
            }

            // 水平補正距離
            if (success == true)
            {
                success = DefHorizontalCorrDist();
            }

            // 垂直距離
            if (success == true)
            {
                success = DefVerticalDist();
            }

            // 垂直測定距離
            if (success == true)
            {
                success = DefVerticalMeasDist();
            }

            // 垂直補正距離
            if (success == true)
            {
                success = DefVerticalCorrDist();
            }

            // 排煙有効高さ
            if (success == true)
            {
                success = DefUsableHeightSmoke();
            }

            // 天端高さ
            if (success == true)
            {
                success = DefHeadHeight();
            }

            // 天井高さ
            if (success == true)
            {
                success = DefCeilingHeight();
            }

            // 防煙壁長さ
            if (success == true)
            {
                success = DefSmokeWallLength();
            }

            // 実行状態
            base.DefSuccess = success;
        }

        /// ================================================================================
        /// <summary>定義 縁側</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefVeranda()
        {
            _DefValVeranda = new ParamDefStrc(_ParamCategories,
                                                     base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_VERANDA"),
                                                     Revit.DB.SpecTypeId.Boolean.YesNo,
                                                     new Revit.DB.ForgeTypeId(string.Empty),
                                                     true,
                                                     0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValVeranda.Categories,
                                                        _DefValVeranda.DefName,
                                                        _DefValVeranda.ParamType,
                                                        _DefValVeranda.BltParamGroup,
                                                        _DefValVeranda.Visible,
                                                        _DefValVeranda.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValVeranda.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 道路面</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefRoadSide()
        {
            _DefValRoadSide = new ParamDefStrc(_ParamCategories,
                                                      base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_ROADSIDE"),
                                                      Revit.DB.SpecTypeId.Boolean.YesNo,
                                                      new Revit.DB.ForgeTypeId(string.Empty),
                                                      true,
                                                      0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValRoadSide.Categories,
                                                        _DefValRoadSide.DefName,
                                                        _DefValRoadSide.ParamType,
                                                        _DefValRoadSide.BltParamGroup,
                                                        _DefValRoadSide.Visible,
                                                        _DefValRoadSide.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValRoadSide.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 水平距離</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefHorizontalDist()
        {
            _DefValHorizontalDist = new ParamDefStrc(_ParamCategories,
                                                            base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_HORIZONTALDIST"),
                                                            Revit.DB.SpecTypeId.Length,
                                                            new Revit.DB.ForgeTypeId(string.Empty),
                                                            true,
                                                            0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValHorizontalDist.Categories,
                                                        _DefValHorizontalDist.DefName,
                                                        _DefValHorizontalDist.ParamType,
                                                        _DefValHorizontalDist.BltParamGroup,
                                                        _DefValHorizontalDist.Visible,
                                                        _DefValHorizontalDist.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValHorizontalDist.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 水平測定距離</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefHorizontalMeasDist()
        {
            _DefValHorizontalMeasDist = new ParamDefStrc(_ParamCategories,
                                                                base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_HORIZONTALMEASDIST"),
                                                                Revit.DB.SpecTypeId.Length,
                                                                new Revit.DB.ForgeTypeId(string.Empty),
                                                                true,
                                                                0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValHorizontalMeasDist.Categories,
                                                        _DefValHorizontalMeasDist.DefName,
                                                        _DefValHorizontalMeasDist.ParamType,
                                                        _DefValHorizontalMeasDist.BltParamGroup,
                                                        _DefValHorizontalMeasDist.Visible,
                                                        _DefValHorizontalMeasDist.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValHorizontalMeasDist.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 水平補正距離</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefHorizontalCorrDist()
        {
            _DefValHorizontalCorrDist = new ParamDefStrc(_ParamCategories,
                                                                base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_HORIZONTALCORRDIST"),
                                                                Revit.DB.SpecTypeId.Length,
                                                                new Revit.DB.ForgeTypeId(string.Empty),
                                                                true,
                                                                0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValHorizontalCorrDist.Categories,
                                                        _DefValHorizontalCorrDist.DefName,
                                                        _DefValHorizontalCorrDist.ParamType,
                                                        _DefValHorizontalCorrDist.BltParamGroup,
                                                        _DefValHorizontalCorrDist.Visible,
                                                        _DefValHorizontalCorrDist.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValHorizontalCorrDist.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 垂直距離</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefVerticalDist()
        {
            _DefValVerticalDist = new ParamDefStrc(_ParamCategories,
                                                          base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_VERTICALDIST"),
                                                          Revit.DB.SpecTypeId.Length,
                                                          new Revit.DB.ForgeTypeId(string.Empty),
                                                          true,
                                                          0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValVerticalDist.Categories,
                                                        _DefValVerticalDist.DefName,
                                                        _DefValVerticalDist.ParamType,
                                                        _DefValVerticalDist.BltParamGroup,
                                                        _DefValVerticalDist.Visible,
                                                        _DefValVerticalDist.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValVerticalDist.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 垂直測定距離</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefVerticalMeasDist()
        {
            _DefValVerticalMeasDist = new ParamDefStrc(_ParamCategories,
                                                              base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_VERTICALMEASDIST"),
                                                              Revit.DB.SpecTypeId.Length,
                                                              new Revit.DB.ForgeTypeId(string.Empty),
                                                              true,
                                                              0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValVerticalMeasDist.Categories,
                                                        _DefValVerticalMeasDist.DefName,
                                                        _DefValVerticalMeasDist.ParamType,
                                                        _DefValVerticalMeasDist.BltParamGroup,
                                                        _DefValVerticalMeasDist.Visible,
                                                        _DefValVerticalMeasDist.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValVerticalMeasDist.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 垂直補正距離</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/08/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefVerticalCorrDist()
        {
            _DefValVerticalCorrDist = new ParamDefStrc(_ParamCategories,
                                                              base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_VERTICALCORRDIST"),
                                                              Revit.DB.SpecTypeId.Length,
                                                              new Revit.DB.ForgeTypeId(string.Empty),
                                                              true,
                                                              0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValVerticalCorrDist.Categories,
                                                        _DefValVerticalCorrDist.DefName,
                                                        _DefValVerticalCorrDist.ParamType,
                                                        _DefValVerticalCorrDist.BltParamGroup,
                                                        _DefValVerticalCorrDist.Visible,
                                                        _DefValVerticalCorrDist.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValVerticalCorrDist.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 排煙有効高さ</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefUsableHeightSmoke()
        {
            _DefValUsableHeightSmoke = new ParamDefStrc(_ParamCategories,
                                                               base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_USABLEHEIGHTSMOKE"),
                                                               Revit.DB.SpecTypeId.Length,
                                                               new Revit.DB.ForgeTypeId(string.Empty),
                                                               true,
                                                               0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValUsableHeightSmoke.Categories,
                                                        _DefValUsableHeightSmoke.DefName,
                                                        _DefValUsableHeightSmoke.ParamType,
                                                        _DefValUsableHeightSmoke.BltParamGroup,
                                                        _DefValUsableHeightSmoke.Visible,
                                                        _DefValUsableHeightSmoke.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValUsableHeightSmoke.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 天端高さ</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefHeadHeight()
        {
            _DefValHeadHeight = new ParamDefStrc(_ParamCategories,
                                                        base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_HEADHEIGHT"),
                                                        Revit.DB.SpecTypeId.Length,
                                                        new Revit.DB.ForgeTypeId(string.Empty),
                                                        true,
                                                        0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValHeadHeight.Categories,
                                                        _DefValHeadHeight.DefName,
                                                        _DefValHeadHeight.ParamType,
                                                        _DefValHeadHeight.BltParamGroup,
                                                        _DefValHeadHeight.Visible,
                                                        _DefValHeadHeight.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValHeadHeight.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 天井高さ</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefCeilingHeight()
        {
            _DefValCeilingHeight = new ParamDefStrc(_ParamCategories,
                                                           base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_CEILINGHEIGHT"),
                                                           Revit.DB.SpecTypeId.Length,
                                                           new Revit.DB.ForgeTypeId(string.Empty),
                                                           true,
                                                           0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValCeilingHeight.Categories,
                                                        _DefValCeilingHeight.DefName,
                                                        _DefValCeilingHeight.ParamType,
                                                        _DefValCeilingHeight.BltParamGroup,
                                                        _DefValCeilingHeight.Visible,
                                                        _DefValCeilingHeight.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValCeilingHeight.DefName;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>定義 防煙壁長さ</summary>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/29 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        bool DefSmokeWallLength()
        {
            _DefValSmokeWallLength = new ParamDefStrc(_ParamCategories,
                                                             base.CmpAttribute.ResourceText("IDS_SHPARAM_DEF_PARTS_SMOKEWALLLENGTH"),
                                                             Revit.DB.SpecTypeId.Length,
                                                             new Revit.DB.ForgeTypeId(string.Empty),
                                                             true,
                                                             0);

            bool ret = base.CmpParameters.SetDefinition(null,
                                                        _DefValSmokeWallLength.Categories,
                                                        _DefValSmokeWallLength.DefName,
                                                        _DefValSmokeWallLength.ParamType,
                                                        _DefValSmokeWallLength.BltParamGroup,
                                                        _DefValSmokeWallLength.Visible,
                                                        _DefValSmokeWallLength.BindingMode);
            if (ret == false)
            {
                base.ErrDefName = _DefValSmokeWallLength.DefName;
            }
            return ret;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        /// ================================================================================
        /// <summary>縁側</summary>
        /// <history><p>2011/07/29 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        bool Veranda
        {
            get
            {
                bool ret = false;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValVeranda.DefName,
                                                    _DefValVeranda.ParamType,
                                                    _DefValVeranda.BltParamGroup,
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
                    if (CmpParameters.SetValue(base.CurrentElem,
                                              _DefValVeranda.DefName,
                                              _DefValVeranda.ParamType,
                                              _DefValVeranda.BltParamGroup,
                                              value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>道路</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        bool RoadSide
        {
            get
            {
                bool ret = false;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValRoadSide.DefName,
                                                    _DefValRoadSide.ParamType,
                                                    _DefValRoadSide.BltParamGroup,
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
                                                    _DefValRoadSide.DefName,
                                                    _DefValRoadSide.ParamType,
                                                    _DefValRoadSide.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>水平距離</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double HorizontalDist
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValHorizontalDist.DefName,
                                                    _DefValHorizontalDist.ParamType,
                                                    _DefValHorizontalDist.BltParamGroup,
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
                                                    _DefValHorizontalDist.DefName,
                                                    _DefValHorizontalDist.ParamType,
                                                    _DefValHorizontalDist.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>水平測定距離</summary>
        /// <history><p>2011/08/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double HorizontalMeasDist
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValHorizontalMeasDist.DefName,
                                                    _DefValHorizontalMeasDist.ParamType,
                                                    _DefValHorizontalMeasDist.BltParamGroup,
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
                                                    _DefValHorizontalMeasDist.DefName,
                                                    _DefValHorizontalMeasDist.ParamType,
                                                    _DefValHorizontalMeasDist.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>水平補正距離</summary>
        /// <history><p>2011/08/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double HorizontalCorrDist
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValHorizontalCorrDist.DefName,
                                                    _DefValHorizontalCorrDist.ParamType,
                                                    _DefValHorizontalCorrDist.BltParamGroup,
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
                                                    _DefValHorizontalCorrDist.DefName,
                                                    _DefValHorizontalCorrDist.ParamType,
                                                    _DefValHorizontalCorrDist.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>垂直距離</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double VerticalDist
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValVerticalDist.DefName,
                                                    _DefValVerticalDist.ParamType,
                                                    _DefValVerticalDist.BltParamGroup,
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
                                                    _DefValVerticalDist.DefName,
                                                    _DefValVerticalDist.ParamType,
                                                    _DefValVerticalDist.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>垂直測定距離</summary>
        /// <history><p>2011/08/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double VerticalMeasDist
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValVerticalMeasDist.DefName,
                                                    _DefValVerticalMeasDist.ParamType,
                                                    _DefValVerticalMeasDist.BltParamGroup,
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
                                                    _DefValVerticalMeasDist.DefName,
                                                    _DefValVerticalMeasDist.ParamType,
                                                    _DefValVerticalMeasDist.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>垂直補正距離</summary>
        /// <history><p>2011/08/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double VerticalCorrDist
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValVerticalCorrDist.DefName,
                                                    _DefValVerticalCorrDist.ParamType,
                                                    _DefValVerticalCorrDist.BltParamGroup,
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
                                                    _DefValVerticalCorrDist.DefName,
                                                    _DefValVerticalCorrDist.ParamType,
                                                    _DefValVerticalCorrDist.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>排煙有効高さ</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double UsableHeightSmoke
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValUsableHeightSmoke.DefName,
                                                    _DefValUsableHeightSmoke.ParamType,
                                                    _DefValUsableHeightSmoke.BltParamGroup,
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
                                                    _DefValUsableHeightSmoke.DefName,
                                                    _DefValUsableHeightSmoke.ParamType,
                                                    _DefValUsableHeightSmoke.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>天端高さ</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double HeadHeight
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValHeadHeight.DefName,
                                                    _DefValHeadHeight.ParamType,
                                                    _DefValHeadHeight.BltParamGroup,
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
                                                    _DefValHeadHeight.DefName,
                                                    _DefValHeadHeight.ParamType,
                                                    _DefValHeadHeight.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>天井高さ</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double CeilingHeight
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValCeilingHeight.DefName,
                                                    _DefValCeilingHeight.ParamType,
                                                    _DefValCeilingHeight.BltParamGroup,
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
                                                    _DefValCeilingHeight.DefName,
                                                    _DefValCeilingHeight.ParamType,
                                                    _DefValCeilingHeight.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>防煙壁長さ</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double SmokeWallLength
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    _DefValSmokeWallLength.DefName,
                                                    _DefValSmokeWallLength.ParamType,
                                                    _DefValSmokeWallLength.BltParamGroup,
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
                                                    _DefValSmokeWallLength.DefName,
                                                    _DefValSmokeWallLength.ParamType,
                                                    _DefValSmokeWallLength.BltParamGroup,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>建具敷居高さ</summary>
        /// <history><p>2011/07/29 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double SillHeight
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM,
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
                                                    Revit.DB.BuiltInParameter.INSTANCE_SILL_HEIGHT_PARAM,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>建具-天端高さ(ビルトインパラメータ）</summary>
        /// <history><p>2011/07/29 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/30 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public
        double HeadHeightBltIn
        {
            get
            {
                double ret = 0.0;
                if (base.CurrentElem != null)
                {
                    if (base.CmpParameters.GetValue(base.CurrentElem,
                                                    Revit.DB.BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM,
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
                                                    Revit.DB.BuiltInParameter.INSTANCE_HEAD_HEIGHT_PARAM,
                                                    value) < -1)
                    {
                    }
                }
            }
        }

        #endregion Properties
    }
}