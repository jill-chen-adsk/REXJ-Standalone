using System;
using System.Collections.Generic;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.AveSiteLevelHeightCalc;
using ADSK.JExtRAC.AveSiteLevelHeightCalc.Entities;
using ADSK.JExtRAC.AveSiteLevelHeightCalc.Utils;
using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Components
{
    /// ================================================================================
    /// <summary>サービス</summary>
    /// ================================================================================
    public class Service
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private RvtExtApp.Components.Elements _CmpElements;

        /// <summary>図形</summary>
        private RvtExtApp.Components.Geometry _CmpGeometry;

        /// <summary>パラメータ</summary>
        private RvtExtApp.Components.Parameters _CmpParameters;

        /// <summary>設定</summary>
        private RvtExtApp.Components.Settings _CmpSettings;

        /// <summary>共有パラメータ - 注釈</summary>
        private RvtExtApp.Entities.SpAnnotation _EntSpAnnotation;

        /// <summary>共有パラメータ - 寸法タイプ</summary>
        private RvtExtApp.Entities.SpDimType _EntSpDimType;

        /// <summary>トランザクション</summary>
        public Revit.DB.Transaction trans;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"    >属性</param>
        /// <param name="cmpElements"     >要素</param>
        /// <param name="cmpGeometry"     >図形</param>
        /// <param name="cmpParameters"   >パラメータ</param>
        /// <param name="cmpSettings"     >設定</param>
        /// <param name="entSpAnnotation" >共有パラメータ - 注釈</param>
        /// <param name="entSpDimType"    >共有パラメータ - 寸法タイプ</param>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public Service(RvtExtApp.Components.Attribute cmpAttribute,
                     RvtExtApp.Components.Elements cmpElements,
                     RvtExtApp.Components.Geometry cmpGeometry,
                     RvtExtApp.Components.Parameters cmpParameters,
                     RvtExtApp.Components.Settings cmpSettings,
                     RvtExtApp.Entities.SpAnnotation entSpAnnotation,
                     RvtExtApp.Entities.SpDimType entSpDimType)
        {
            // 初期化
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _EntSpAnnotation = entSpAnnotation;
            _EntSpDimType = entSpDimType;
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>平均地盤面算定ポイント作成</summary>
        ///
        /// <param name="areaCurves"            >エリア境界線</param>
        /// <param name="aveGlLevelCalcPosList" >平均地盤面算定ポイント</param>
        /// <param name="flagAreaCurvesConnect" ><p>エリア境界線接続フラグ</p>
        ///                                         <p>True  = 閉じている</p>
        ///                                         <p>False = 開いている</p></param>
        /// <param name="flagEndPosConnect"     ><p>最終点接続フラグ</p>
        ///                                         <p>True  = 最後の点が最初の点に接続している</p>
        ///                                         <p>False = 最後の点が最初の点に接続していない</p></param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/07/31 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2021/12/20 Modified Applied Technology</p></history>
        /// ================================================================================
        public bool CreateAveGlLevelCalcPos(Collections.Generic.IList<Revit.DB.CurveElement> areaCurves,
                                            ref Collections.Generic.IList<ObjectTag> aveGlLevelCalcPosList,
                                            ref bool flagAreaCurvesConnect,
                                            ref bool flagEndPosConnect)
        {
            bool ret = false;
            flagAreaCurvesConnect = false;
            flagEndPosConnect = false;

            // 平均地盤面算定ポイント
            Revit.DB.FamilySymbol symbolCircle = null, symbolTag = null;
            var currentDoc = this._CmpSettings.RvtUIDoc.Document;
            int scale = this._CmpSettings.RvtUIDoc.Document.ActiveView.Scale;
            if (_CmpElements.LoadSymbolAndTag(currentDoc, scale, ref symbolCircle, ref symbolTag) == false)
                return ret;

            // 地形面取得
            var flagTopo = true;
            var topoSurfaces = _CmpElements.TopoSurface ;
            if (topoSurfaces.Count == 0)
            {
                flagTopo = false;
            }

            // 地形面のメッシュ取得
            Collections.Generic.IList<Revit.DB.MeshTriangle>  topoTriMeshes = new Collections.Generic.List<Revit.DB.MeshTriangle>();

            
            // 地形ソリッド取得
            #if (REVIT2021 || REVIT2022 || REVIT2023 )
            #else
            var toposolids = _CmpElements.TopoSolids ;
            if ( toposolids.Count > 0 ) {
                foreach ( var toposolid in toposolids ) _CmpGeometry.GetTriMeshTopoSurface( toposolid, ref topoTriMeshes ) ;
            }
            #endif


            if (topoSurfaces.Count >0) {
                foreach (var topoSurface in topoSurfaces) _CmpGeometry.GetTriMeshTopoSurface(topoSurface, ref topoTriMeshes);
            }

            // エリア境界取得
            if (areaCurves.Count == 0)
            {
                return ret;
            }

            // エリア境界線並び替え
            Collections.Generic.IList<Revit.DB.Curve> areaGeoCurves = new Collections.Generic.List<Revit.DB.Curve>();
            Collections.Generic.IList<Collections.Generic.IList<int>> areaGeoCurvesIndex = new Collections.Generic.List<Collections.Generic.IList<int>>();
            Collections.Generic.IList<bool> areaGeoCurvesConnect = new Collections.Generic.List<bool>();
            SortClockwise(areaCurves, ref areaGeoCurves, ref areaGeoCurvesIndex, ref areaGeoCurvesConnect);

            // 平均地盤面算定ポイントの座標
            Collections.Generic.IList<Revit.DB.XYZ> aveGlLevelCalcPoss = new Collections.Generic.List<Revit.DB.XYZ>();
            Revit.DB.Curve curve;
            int index0 = 0;
            int index1 = 0;
            bool flagAdd = false;
            for (int i = 0; i < areaGeoCurves.Count; ++i)
            {
                // エリア境界線１点目
                curve = areaGeoCurves[i];
                index0 = areaGeoCurvesIndex[i][0];
                index1 = areaGeoCurvesIndex[i][1];
                Revit.DB.XYZ pos1 = curve.GetEndPoint(index0);
                Revit.DB.XYZ pos2 = curve.GetEndPoint(index1);
                double zValue = 0.0;
                flagAdd = true;
                if (topoTriMeshes.Count > 0)
                {
                    flagAdd = false;
                    if (CheckTrianglePoint(pos1, topoTriMeshes, ref zValue) == true)
                    {
                        flagAdd = true;
                    }
                }
                if (flagAdd == true)
                {
                    aveGlLevelCalcPoss.Add(new Revit.DB.XYZ(pos1.X, pos1.Y, zValue));
                }

                // エリア境界線２点目
                if (areaGeoCurvesConnect[i] == false)
                {
                    pos1 = pos2;
                    zValue = 0;
                    flagAdd = true;
                    if (flagTopo == true)
                    {
                        flagAdd = false;
                        if (CheckTrianglePoint(pos1, topoTriMeshes, ref zValue) == true)
                        {
                            flagAdd = true;
                        }
                    }
                    if (flagAdd == true)
                    {
                        aveGlLevelCalcPoss.Add(new Revit.DB.XYZ(pos1.X, pos1.Y, zValue));
                    }
                }
            }

            // 平均地盤面算定ポイント配置
            if (aveGlLevelCalcPoss.Count > 0)
            {
                int i = 0;
                foreach (Revit.DB.XYZ pos in aveGlLevelCalcPoss)
                {
                    double zValue = pos.Z;
                    Revit.DB.XYZ locPos = new Revit.DB.XYZ(pos.X, pos.Y, 0.0);

                    // Create tag
                    if (PlaceSymbolTag(_CmpElements.RvtDBDoc, pos, symbolCircle, symbolTag, out Revit.DB.FamilyInstance fmCircle, out Revit.DB.IndependentTag tag))
                    {
                        // Setting data
                        _EntSpAnnotation.CurrentCircle = fmCircle;
                        _EntSpAnnotation.CurrentTag = tag;

                        _EntSpAnnotation.AveGlLvlCalcPosLevel = zValue;
                        _EntSpAnnotation.AveGlLvlCalcPosCircleNo = ++i;

                        // New object tag
                        ObjectTag objTag = new ObjectTag();
                        objTag.CircleTag = fmCircle;
                        objTag.Tag = tag;

                        // Add to list tag
                        aveGlLevelCalcPosList.Add(objTag);
                    }
                }
            }

            // エリア境界線接続フラグ
            if (areaGeoCurvesConnect.Count > 0)
            {
                flagAreaCurvesConnect = true;
                for (int i = 0; i < areaGeoCurvesConnect.Count; ++i)
                {
                    if (areaGeoCurvesConnect[i] == false)
                    {
                        flagAreaCurvesConnect = false;
                        break;
                    }
                }
            }

            // 最終点接続フラグ
            if (areaGeoCurvesConnect[areaGeoCurvesConnect.Count - 1] == true)
            {
                flagEndPosConnect = true;
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>平均地盤面算定ポイント表示レベル設定</summary>
        ///
        /// <param name="entDtAnnotation">データテーブル - 注釈</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/07/31 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool SetAveGlLevelCalcPosDispLevel(RvtExtApp.Entities.DtAnnotation entDtAnnotation, bool isSelectElement)
        {
            // 戻り値
            bool ret = false;

            // 初期化
            double unitM = 0.001;
            double dValue = 0.0;
            string sValue = "";

            int decimalPoint = entDtAnnotation.AreaDecimal;
            int fractionType = entDtAnnotation.AreaRoundingOpt;
            int unitLen = entDtAnnotation.LengthUnit;
            System.Data.DataTable dt = entDtAnnotation.TableAveGlLvlCalcPos;

            // 平均地盤面算定ポイントテーブルデータ
            if (dt.Rows.Count == 0)
            {
                return ret;
            }

            int rows = dt.Rows.Count;
            for (int i = 0; i < rows; ++i)
            {
                int rowNo = i;

                // ID Circle
                int idCircle = int.Parse(dt.Rows[rowNo]["IDCircle"].ToString());
                // ID Tag
                int idTag = int.Parse(dt.Rows[rowNo]["IDTag"].ToString());

                Revit.DB.FamilyInstance fmCircle = _CmpElements.GetAveGlLvlCalcPos(idCircle) as Revit.DB.FamilyInstance;
                Revit.DB.IndependentTag tag = _CmpElements.GetAveGlLvlCalcPos(idTag) as Revit.DB.IndependentTag;

                if (fmCircle == null || tag == null)
                    continue;

                // レベル
                dValue = double.Parse(dt.Rows[rowNo]["Level"].ToString());
                if (unitLen == 1)
                    dValue *= unitM;

                // 何回も実行したら、0になった不具合の修正
                //if (isSelectElement == false)
                //    dValue = dValue / _CmpGeometry.UnitCoe;

                // 桁処理(負数対応)
                sValue = RoundingNegNum(dValue,
                                        decimalPoint,
                                        fractionType);

                // 表示レベル
                _EntSpAnnotation.CurrentCircle = fmCircle;
                _EntSpAnnotation.CurrentTag = tag;

                _EntSpAnnotation.AveGlLvlCalcPosDispLevel = double.Parse(sValue);
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>算定図作成</summary>
        ///
        /// <param name="areaViewName"        >エリア平面図名</param>
        /// <param name="bmHeight"            >BM高さ</param>
        /// <param name="scale"               >縮尺</param>
        /// <param name="raiteHorizontal"     >横の比</param>
        /// <param name="raiteVertical"       >縦の比</param>
        /// <param name="tableAveGlLvlCalcPos">平均地盤面算定ポイントテーブルデータ</param>
        /// <param name="flagEndPosConnect"   ><p>最終点接続フラグ</p>
        ///                                       <p>True  = 最後の点が最初の点に接続している</p>
        ///                                       <p>False = 最後の点が最初の点に接続していない</p></param>
        /// <param name="elemDim1"            >寸法1要素</param>
        /// <param name="elemDim2"            >寸法2要素</param>
        /// <param name="numbes"              >番号</param>
        /// <param name="levels"              >レベル</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool CreateCalcDrawing(string areaViewName,
                               double bmHeight,
                               int scale,
                               int raiteHorizontal,
                               int raiteVertical,
                               System.Data.DataTable tableAveGlLvlCalcPos,
                               bool flagEndPosConnect,
                               ref Revit.DB.Dimension elemDim1,
                               ref Revit.DB.Dimension elemDim2,
                               ref Collections.Generic.IList<int> numbers,
                               ref Collections.Generic.IList<double> levels)
        {
            bool ret = false;

            // 引数チェック
            if (scale < 1)
            {
                return ret;
            }
            if (raiteHorizontal < 1)
            {
                return ret;
            }
            if (raiteVertical < 1)
            {
                return ret;
            }
            if (tableAveGlLvlCalcPos.Rows.Count == 0)
            {
                return ret;
            }

            // 算定図の注釈ロード
            Revit.DB.AnnotationSymbolType symbolCurrentBGL = null;
            Revit.DB.AnnotationSymbolType symbolDGL = null;
            Revit.DB.AnnotationSymbolType symbolScale = null;
            Revit.DB.AnnotationSymbolType symbolAveGlLvlCalcPosSign = null;

            ret = LoadAnnotationCalcDraw(ref symbolCurrentBGL,
                                         ref symbolDGL,
                                         ref symbolScale,
                                         ref symbolAveGlLvlCalcPosSign);

            if (ret == false)
            {
                return ret;
            }

            // 算定図ビュー
            string viewName = areaViewName + "-" + _CmpAttribute.ResourceText("IDS_VIEW_CALCDRAW");
            Revit.DB.ViewDrafting viewCalcDraw = _CmpElements.GetViewDrafting(viewName);
            if (viewCalcDraw == null)
            {
                // 算定図ビュー作成
                viewCalcDraw = _CmpElements.CreateViewDrafting(viewName, null, scale);
            }
            else
            {
                // 算定図ビューの要素削除
                viewCalcDraw.Scale = scale;
                _CmpElements.DelElemsView(viewCalcDraw);
            }

            // 平均地盤面算定ポイント作成
            Revit.DB.XYZ cPos = new Revit.DB.XYZ(0, 0, 0);
            Revit.DB.XYZ nPos = new Revit.DB.XYZ(0, 0, 0);
            double dist = 0.0;
            double cX = 0.0;
            double nX = 0.0;
            double cY = 0.0;
            double nY = 0.0;
            double cL = 0.0;
            double nL = 0.0;
            int cN = 0;
            int nN = 0;
            double maxX = 0.0;
            double minY = 0.0;
            double unitCoe = _CmpGeometry.UnitCoe;
            double approx0Len = _CmpGeometry.Approx0Len;
            Revit.DB.ReferenceArray refAry1 = new Revit.DB.ReferenceArray();
            Revit.DB.ReferenceArray refAry2 = new Revit.DB.ReferenceArray();

            Revit.DB.XYZ pos1;
            Revit.DB.XYZ pos2;
            Revit.DB.Curve crv;
            Revit.DB.CurveElement crvElem1;
            Revit.DB.CurveElement crvElem2;
            Revit.DB.CurveElement crvElem3;
            Collections.Generic.IList<double> nXs = new Collections.Generic.List<double>();
            int rows = tableAveGlLvlCalcPos.Rows.Count;
            for (int i = 0; i <= rows; ++i)
            {
                int rowNo = i;
                if (i == rows)
                {
                    if (flagEndPosConnect == true)
                    {
                        rowNo = 0;
                    }
                    else
                    {
                        continue;
                    }
                }

                // ID
                int id = int.Parse(tableAveGlLvlCalcPos.Rows[rowNo]["IDCircle"].ToString());
                Revit.DB.FamilyInstance aveGlLvlCalcPos = _CmpElements.GetAveGlLvlCalcPos(id) as Revit.DB.FamilyInstance;
                if (aveGlLvlCalcPos == null)
                    continue;

                cPos = nPos;
                nPos = _CmpGeometry.GetElementLocPos(aveGlLvlCalcPos);
                if (i > 0)
                {
                    cX = nX;
                    dist = _CmpGeometry.Distance2D(cPos, nPos) * raiteHorizontal;
                    nX += dist;
                }

                // Y軸位置
                cL = nL;
                cY = nY;
                nL = double.Parse(tableAveGlLvlCalcPos.Rows[rowNo]["Level"].ToString());
                nY = nL / unitCoe * raiteVertical;

                // Number
                // 番号
                cN = nN;
                nN = int.Parse(tableAveGlLvlCalcPos.Rows[rowNo]["Number"].ToString());

                // 詳細線分作成
                bool flagSegment = false;
                try
                {
                    crvElem1 = null;
                    crvElem2 = null;
                    crvElem3 = null;

                    // Y軸
                    pos1 = new Revit.DB.XYZ(nX, 0.0, 0.0);
                    pos2 = new Revit.DB.XYZ(nX, nY, 0.0);
                    nXs.Add(nX);
                    if (_CmpGeometry.Distance2D(pos1, pos2) > approx0Len)
                    {
                        crv = Revit.DB.Line.CreateBound(pos1, pos2);
                        crvElem1 = _CmpElements.CreateDetailCurve(viewCalcDraw, crv);
                    }

                    if (i > 0)
                    {
                        // X軸上
                        pos1 = new Revit.DB.XYZ(cX, 0.0, 0.0);
                        pos2 = new Revit.DB.XYZ(nX, 0.0, 0.0);
                        if (_CmpGeometry.Distance2D(pos1, pos2) > approx0Len)
                        {
                            crv = Revit.DB.Line.CreateBound(pos1, pos2);
                            crvElem2 = _CmpElements.CreateDetailCurve(viewCalcDraw, crv);
                        }

                        // X軸下
                        pos1 = new Revit.DB.XYZ(cX, cY, 0.0);
                        pos2 = new Revit.DB.XYZ(nX, nY, 0.0);
                        if (_CmpGeometry.Distance2D(pos1, pos2) > approx0Len)
                        {
                            crv = Revit.DB.Line.CreateBound(pos1, pos2);
                            crvElem3 = _CmpElements.CreateDetailCurve(viewCalcDraw, crv);
                        }
                    }

                    // 参照線
                    if (i == 1)
                    {
                        if (crvElem3 != null)
                        {
                            refAry2.Append(crvElem3.GeometryCurve.GetEndPointReference(0));
                            flagSegment = true;
                        }
                        else if (crvElem2 != null)
                        {
                            refAry2.Append(crvElem2.GeometryCurve.GetEndPointReference(0));
                            flagSegment = true;
                        }
                        if (flagSegment == true)
                        {
                            // 番号
                            numbers.Add(cN);

                            // レベル
                            levels.Add(cL);
                        }
                        flagSegment = false;
                    }

                    if (i > 0)
                    {
                        if (crvElem3 != null)
                        {
                            refAry2.Append(crvElem3.GeometryCurve.GetEndPointReference(1));
                            flagSegment = true;
                        }
                        else if (crvElem2 != null)
                        {
                            refAry2.Append(crvElem2.GeometryCurve.GetEndPointReference(1));
                            flagSegment = true;
                        }

                        if (flagSegment == true)
                        {
                            // 番号
                            numbers.Add(nN);

                            // レベル
                            levels.Add(nL);
                        }
                    }

                    // Y軸最小値
                    if (minY > nY)
                    {
                        minY = nY;
                    }

                    // X軸最大値
                    maxX = nX;
                }
                catch (System.Exception ex)
                {
                    string errMsg = ex.Message;
                }
            }
            if (refAry2.Size > 0)
            {
                refAry1.Append(refAry2.get_Item(0));
                refAry1.Append(refAry2.get_Item(refAry2.Size - 1));
            }

            // 注釈作成
            CreateAnnotationCalcDraw(viewCalcDraw,
                                     bmHeight,
                                     scale,
                                     raiteHorizontal,
                                     raiteVertical,
                                     maxX,
                                     minY,
                                     symbolCurrentBGL,
                                     symbolDGL,
                                     symbolScale);

            // 寸法
            CreateDimensionCalcDraw(viewCalcDraw,
                                    scale,
                                    maxX,
                                    minY,
                                    refAry1,
                                    refAry2,
                                    nXs,
                                    numbers,
                                    symbolAveGlLvlCalcPosSign,
                                    ref elemDim1,
                                    ref elemDim2);

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>根拠式作成</summary>
        ///
        /// <param name="areaViewName"    >エリア平面図名</param>
        /// <param name="elemDim1"        >寸法1要素</param>
        /// <param name="elemDim2"        >寸法2要素</param>
        /// <param name="numbes"          >番号</param>
        /// <param name="levels"          >レベル</param>
        /// <param name="unitLen"         ><p>長さの単位</p>
        ///                                   <p>0=mm</p>
        ///                                   <p>1=m</p></param>
        /// <param name="decimalPointArea">面積の小数点位置</param>
        /// <param name="fractionTypeArea"><p>面積の端数タイプ</p>
        ///                                   <p>0=切り捨て</p>
        ///                                   <p>1=切り上げ</p>
        ///                                   <p>2=四捨五入</p></param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history><p>2011/08/01 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2015/09/11 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public bool CreateGroundsExp(string areaViewName,
                              Revit.DB.Dimension elemDim1,
                              Revit.DB.Dimension elemDim2,
                              Collections.Generic.IList<int> numbers,
                              Collections.Generic.IList<double> levels,
                              int unitLen,
                              int decimalPointArea,
                              int fractionTypeArea)
        {
            // 戻り値
            bool ret = false;

            double approx0Len = _CmpGeometry.Approx0Len;

            // 引数チェック
            if (elemDim1 == null)
            {
                return ret;
            }
            if (elemDim2 == null)
            {
                return ret;
            }
            if (numbers.Count < 2)
            {
                return ret;
            }
            if (numbers.Count != levels.Count)
            {
                return ret;
            }
            if (numbers.Count - 1 != elemDim2.Segments.Size)
            {
                return ret;
            }

            // 平均地盤面算定ポイント符号
            Revit.DB.AnnotationSymbolType symbolAveGlLevelCalcPosSign = _CmpElements.SymbolAveGlLevelCalcPosSign;
            if (symbolAveGlLevelCalcPosSign == null)
            {
                symbolAveGlLevelCalcPosSign = _CmpElements.LoadSymbolAveGlLevelCalcPosSign();
                if (symbolAveGlLevelCalcPosSign == null)
                {
                    return ret;
                }
            }

            // 根拠式ビュー
            int scale = 1;
            string viewName = areaViewName + "-" + _CmpAttribute.ResourceText("IDS_VIEW_GROUNDSEXP");
            Revit.DB.ViewDrafting viewGroundsExp = _CmpElements.GetViewDrafting(viewName);

            trans.Start("CreateViewDrafting");
            if (viewGroundsExp == null)
            {
                // 根拠式ビュー作成
                viewGroundsExp = _CmpElements.CreateViewDrafting(viewName, null, scale);
            }
            else
            {
                // 根拠式ビューの要素削除
                _CmpElements.DelElemsView(viewGroundsExp);
            }
            trans.Commit();

            // 値
            Collections.Generic.IList<string> listLv = new Collections.Generic.List<string>();
            Collections.Generic.IList<string> listSeg = new Collections.Generic.List<string>();
            Collections.Generic.IList<int> digitsLv = new Collections.Generic.List<int>();
            Collections.Generic.IList<int> digitsSeg = new Collections.Generic.List<int>();
            int digitMaxLv = 0;
            int digitMaxSeg = 0;
            double unitM = 0.001;
            double dVal = 0.0;
            int decimalType = decimalPointArea;
            int fractionType = fractionTypeArea;

            double segs = 0.0;
            for (int i = 0; i < numbers.Count; ++i)
            {
                // レベル
                dVal = double.Parse(levels[i].ToString());
                if (unitLen == 1)
                {
                    dVal *= unitM;
                }
                AddListValExp(dVal, decimalType, fractionType, ref listLv, ref digitsLv, ref digitMaxLv);
                if (dVal < 0.0)
                {
                }

                // セグメント
                if (i > 0)
                {
                    dVal = double.Parse(elemDim2.Segments.get_Item(i - 1).ValueString);
                    if (unitLen == 1)
                    {
                        dVal *= unitM;
                    }

                    AddListValExp(dVal, decimalType, fractionType, ref listSeg, ref digitsSeg, ref digitMaxSeg);
                    segs += double.Parse(listSeg[listSeg.Count - 1]);
                }
            }

            // 根拠式作成
            Collections.Generic.IList<string> listArea = new Collections.Generic.List<string>();
            Collections.Generic.IList<int> digitsArea = new Collections.Generic.List<int>();
            int digitMaxArea = 0;
            Collections.Generic.IList<string> listExp = new Collections.Generic.List<string>();
            Collections.Generic.IList<int> listNum1 = new Collections.Generic.List<int>();
            Collections.Generic.IList<int> listNum2 = new Collections.Generic.List<int>();

            double areas = 0.0;
            for (int i = 1; i < numbers.Count; ++i)
            {
                // レベル1
                string lv1Str = AddBlankValExp(listLv[i - 1], digitsLv[i - 1], digitMaxLv);
                double lv1Val = double.Parse(listLv[i - 1]);

                // レベル2
                string lv2Sign = "";
                string lv2Str = "";
                AddBlankValExp(listLv[i], digitsLv[i], digitMaxLv, ref lv2Sign, ref lv2Str);
                double lv2Val = double.Parse(listLv[i]);

                // セグメント
                string segStr = AddBlankValExp(listSeg[i - 1], digitsSeg[i - 1], digitMaxSeg);
                double segVal = double.Parse(listSeg[i - 1]);

                // 根拠式
                string expStr = "(" + lv1Str + lv2Sign + lv2Str + ")" + " x " + segStr + " / " + "2";
                double area = (lv1Val + lv2Val) * segVal * 0.5;
                int lenC = UtilValue.GetByteCountString(expStr);
                int lenN = lenC;
                double lv1ValAbs = System.Math.Abs(lv1Val);
                double lv2ValAbs = System.Math.Abs(lv2Val);
                double lv12ValAbs = System.Math.Abs(lv1Val + lv2Val);

                double areaExp = 0.0;
                if (expStr != null)
                {
                    // 符号1
                    listNum1.Add(numbers[i - 1]);

                    // 符号2
                    listNum2.Add(numbers[i]);

                    // 根拠式
                    listExp.Add(expStr);

                    // 面積
                    AddListValExp(area, decimalPointArea, fractionTypeArea, ref listArea, ref digitsArea, ref digitMaxArea);
                    areaExp = double.Parse(listArea[listArea.Count - 1]);
                }

                // 累積
                areas += areaExp;
            }

            //Calculate lenght of sum areas
            string strAreas = _CmpParameters.StrZeroPadding(UtilValue.Rounding(areas, decimalPointArea, fractionTypeArea), decimalPointArea - 1);
            int digit = UtilValue.GetByteCountString(strAreas);
            if (digitMaxArea < digit)
                digitMaxArea = digit;

            // 根拠式配置
            double nX = 0.0;
            double nY = 0.0;
            int strLen = 0;
            int strLenMax = 0;

            Revit.DB.XYZ pos1;
            Revit.DB.XYZ pos2;

            Revit.DB.FamilyInstance elemFamInst;
            Revit.DB.AnnotationSymbol elemAnnotation;
            Revit.DB.TextNote elemText;

            double unitCoe = _CmpGeometry.UnitCoe;
            double interX = double.Parse(_CmpAttribute.ResourceText("IDS_VAL_EXPINTERX")) / unitCoe;
            double interY = double.Parse(_CmpAttribute.ResourceText("IDS_VAL_EXPINTERY")) / unitCoe;
            double offset_txt = Math.Abs(interY) / 4.0;
            string signText = _CmpAttribute.ResourceText("IDS_VAL_EXPSIGNTEXT");

            //Create new text note type
            Revit.DB.ElementId noteTypeId = _CmpElements.CreateTextNoteType();

            for (int i = 0; i < listExp.Count; ++i)
            {
                // 符号1
                nX = 0.0;
                pos1 = new Revit.DB.XYZ(nX, nY + offset_txt, 0.0);

                trans.Start("NewFamilyInstance");
                elemFamInst = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(pos1,
                                                                             symbolAveGlLevelCalcPosSign,
                                                                             viewGroundsExp);
                if (elemFamInst != null)
                {
                    elemAnnotation = elemFamInst as Revit.DB.AnnotationSymbol;
                    if (elemAnnotation != null)
                    {
                        _EntSpAnnotation.CurrentElem = elemAnnotation;
                        _EntSpAnnotation.AveGlLvlCalcPosNo = listNum1[i];
                    }
                }
                trans.Commit();

                // 符号文字
                nX += interX;
                pos1 = new Revit.DB.XYZ(nX, nY, 0.0);
                elemText = _CmpElements.CreateTextNoteSetPosRotate(viewGroundsExp, pos1, signText);

                // 符号2
                nX += interX;
                pos1 = new Revit.DB.XYZ(nX, nY + offset_txt, 0.0);

                trans.Start("NewFamilyInstance");
                elemFamInst = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(pos1,
                                                                             symbolAveGlLevelCalcPosSign,
                                                                             viewGroundsExp);
                if (elemFamInst != null)
                {
                    elemAnnotation = elemFamInst as Revit.DB.AnnotationSymbol;
                    if (elemAnnotation != null)
                    {
                        _EntSpAnnotation.CurrentElem = elemAnnotation;
                        _EntSpAnnotation.AveGlLvlCalcPosNo = listNum2[i];
                    }
                }
                trans.Commit();

                // 面積
                string areaStr = AddBlankValExp(listArea[i], digitsArea[i], digitMaxArea);

                // 文字列数
                strLen = UtilValue.GetByteCountString(listExp[i]);
                if (strLenMax < strLen)
                {
                    strLenMax = strLen;
                }

                // 根拠式
                string expStr = listExp[i] + " = " + areaStr;
                nX += interX;
                pos1 = new Revit.DB.XYZ(nX, nY - Math.Abs(offset_txt) / 1.4, 0.0);
                elemText = _CmpElements.CreateTextNoteSetPosRotate(viewGroundsExp, pos1, 0,
                    Revit.DB.HorizontalTextAlignment.Left, Revit.DB.VerticalTextAlignment.Middle, noteTypeId, expStr);

                nY += interY;
            }

            string aveLvStr;
            double aveLvVal;
            string blankStr;
            string valStr;
            int strLenMax2 = strLenMax;

            // 面積合計罫線
            double downOffset = Math.Abs(interY) * 0.5;
            double kY1 = nY - (interY * 0.5);
            nY = kY1 + (interY * 0.5) - offset_txt - downOffset;
            nX = (interX * 3);

            // 面積合計値
            valStr = "ΣS";
            blankStr = UtilValue.CreateBlankString(strLenMax - UtilValue.GetByteCountString(valStr));
            valStr = blankStr + valStr + " = " + _CmpParameters.StrZeroPadding(UtilValue.Rounding(areas, decimalPointArea, fractionTypeArea), decimalPointArea - 1);
            pos1 = new Revit.DB.XYZ(nX, nY, 0.0);
            elemText = _CmpElements.CreateTextNoteSetPosRotate(viewGroundsExp, pos1, 0,
                Revit.DB.HorizontalTextAlignment.Left, Revit.DB.VerticalTextAlignment.Middle, noteTypeId, valStr);
            strLen = UtilValue.GetByteCountString(valStr);
            if (strLenMax2 < strLen)
            {
                strLenMax2 = strLen;
            }

            // 平均レベル根拠式
            valStr = "ΣS" + " / " + "ΣL";
            blankStr = UtilValue.CreateBlankString(strLenMax - UtilValue.GetByteCountString(valStr));
            dVal = segs;

            string areasRoundStr = UtilValue.Rounding(areas, decimalPointArea, fractionTypeArea);
            string dValRoundStr = UtilValue.Rounding(dVal, decimalPointArea, fractionTypeArea);
            areasRoundStr = _CmpParameters.StrZeroPadding(areasRoundStr, decimalPointArea - 1);
            dValRoundStr = _CmpParameters.StrZeroPadding(dValRoundStr, decimalPointArea - 1);
            aveLvVal = double.Parse(areasRoundStr) / double.Parse(dValRoundStr);
            aveLvStr = aveLvVal.ToString();
            valStr = blankStr + valStr + " = " + areasRoundStr + " / " + dValRoundStr + " = " + aveLvStr;
            nY += interY;
            pos1 = new Revit.DB.XYZ(nX, nY, 0.0);
            elemText = _CmpElements.CreateTextNoteSetPosRotate(viewGroundsExp, pos1, 0,
                Revit.DB.HorizontalTextAlignment.Left, Revit.DB.VerticalTextAlignment.Middle, noteTypeId, valStr);
            strLen = UtilValue.GetByteCountString(valStr);
            if (strLenMax2 < strLen)
            {
                strLenMax2 = strLen;
            }

            // 平均レベル値
            blankStr = UtilValue.CreateBlankString(strLenMax);
            aveLvStr = RoundingNegNum(aveLvVal, decimalType, fractionType);
            valStr = blankStr + " ≈ " + aveLvStr;
            nY += interY;
            pos1 = new Revit.DB.XYZ(nX, nY, 0.0);
            elemText = _CmpElements.CreateTextNoteSetPosRotate(viewGroundsExp, pos1, 0,
                Revit.DB.HorizontalTextAlignment.Left, Revit.DB.VerticalTextAlignment.Middle, noteTypeId, valStr);
            strLen = UtilValue.GetByteCountString(valStr);
            if (strLenMax2 < strLen)
            {
                strLenMax2 = strLen;
            }

            // 平均レベル
            valStr = " * " + _CmpAttribute.ResourceText("IDS_TXT_AVEGLLEVEL");
            blankStr = UtilValue.CreateBlankString(strLenMax - UtilValue.GetByteCountString(valStr));
            string sign = " + ";
            if (aveLvVal < 0.0)
            {
                sign = " - ";
                aveLvStr = aveLvStr.Substring(1);
            }
            valStr = blankStr + valStr + " = " + _CmpAttribute.ResourceText("IDS_TXT_CURRENTBGL") + sign + aveLvStr;
            nY += interY;
            pos1 = new Revit.DB.XYZ(nX, nY, 0.0);
            elemText = _CmpElements.CreateTextNoteSetPosRotate(viewGroundsExp, pos1, 0,
                Revit.DB.HorizontalTextAlignment.Left, Revit.DB.VerticalTextAlignment.Middle, noteTypeId, valStr);
            strLen = UtilValue.GetByteCountString(valStr);
            if (strLenMax2 < strLen)
            {
                strLenMax2 = strLen;
            }

            // 罫線
            double dArial2 = 0.229593;
            double coe = 1.2 * (elemText.Width / dArial2);
            trans.Start("CreateDetailCurve");
            Revit.DB.Curve crv;
            Revit.DB.CurveElement crvElem;
            double kX1 = -interX;
            double kX2 = (interX * 3) + ((strLenMax2 * coe) / unitCoe);
            pos1 = new Revit.DB.XYZ(kX1, kY1, 0.0);
            pos2 = new Revit.DB.XYZ(kX2, kY1, 0.0);
            crv = Revit.DB.Line.CreateBound(pos1, pos2);
            crvElem = _CmpElements.CreateDetailCurve(viewGroundsExp, crv);

            double kY2 = nY + (interY * 0.5);
            pos1 = new Revit.DB.XYZ(kX1, kY2, 0.0);
            pos2 = new Revit.DB.XYZ(kX2, kY2, 0.0);
            crv = Revit.DB.Line.CreateBound(pos1, pos2);
            crvElem = _CmpElements.CreateDetailCurve(viewGroundsExp, crv);

            kY2 += (interY * 0.25);
            pos1 = new Revit.DB.XYZ(kX1, kY2, 0.0);
            pos2 = new Revit.DB.XYZ(kX2, kY2, 0.0);
            crv = Revit.DB.Line.CreateBound(pos1, pos2);
            crvElem = _CmpElements.CreateDetailCurve(viewGroundsExp, crv);
            trans.Commit();

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>算定図の注釈ロード</summary>
        ///
        /// <param name="symbolCurrentBGL"            >既存BGLシンボル</param>
        /// <param name="symbolDGL"                   >設計GLシンボル</param>
        /// <param name="symbolScale"                 >縮尺シンボル</param>
        /// <param name="symbolAveGlLevelCalcPosSign" >平均地盤面算定ポイント符号シンボル</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 成功</p>
        ///             <p>False = 失敗</p></returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool LoadAnnotationCalcDraw(ref Revit.DB.AnnotationSymbolType symbolCurrentBGL,
                                    ref Revit.DB.AnnotationSymbolType symbolDGL,
                                    ref Revit.DB.AnnotationSymbolType symbolScale,
                                    ref Revit.DB.AnnotationSymbolType symbolAveGlLevelCalcPosSign)
        {
            bool ret = false;

            // 既存BGL
            symbolCurrentBGL = _CmpElements.SymbolCurrentBGL;
            if (symbolCurrentBGL == null)
            {
                symbolCurrentBGL = _CmpElements.LoadSymbolCurrentBGL();
                if (symbolCurrentBGL == null)
                {
                    return ret;
                }
            }

            // 設計GL
            symbolDGL = _CmpElements.SymbolDGL;
            if (symbolDGL == null)
            {
                symbolDGL = _CmpElements.LoadSymbolDGL();
                if (symbolDGL == null)
                {
                    return ret;
                }
            }

            // 縮尺
            symbolScale = _CmpElements.SymbolScale;
            if (symbolScale == null)
            {
                symbolScale = _CmpElements.LoadSymbolScale();
                if (symbolScale == null)
                {
                    return ret;
                }
            }

            // 平均地盤面算定ポイント符号
            symbolAveGlLevelCalcPosSign = _CmpElements.SymbolAveGlLevelCalcPosSign;
            if (symbolAveGlLevelCalcPosSign == null)
            {
                symbolAveGlLevelCalcPosSign = _CmpElements.LoadSymbolAveGlLevelCalcPosSign();
                if (symbolAveGlLevelCalcPosSign == null)
                {
                    return ret;
                }
            }

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>算定図の注釈作成</summary>
        ///
        /// <param name="view"            >ビュー</param>
        /// <param name="bmHeight"        >BM高さ</param>
        /// <param name="scale"           >縮尺</param>
        /// <param name="raiteHorizontal" >横の比</param>
        /// <param name="raiteVertical"   >縦の比</param>
        /// <param name="maxX"            >Xの最大値</param>
        /// <param name="maxY"            >Yの最大値</param>
        /// <param name="symbolCurrentBGL">既存BGLシンボル</param>
        /// <param name="symbolDGL"       >設計GLシンボル</param>
        /// <param name="symbolScale"     >縮尺シンボル</param>
        ///
        /// <history><p>2011/08/01 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p>
        ///          <p>2015/09/11 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public void CreateAnnotationCalcDraw(Revit.DB.View view,
                                      double bmHeight,
                                      int scale,
                                      int raiteHorizontal,
                                      int raiteVertical,
                                      double maxX,
                                      double maxY,
                                      Revit.DB.AnnotationSymbolType symbolCurrentBGL,
                                      Revit.DB.AnnotationSymbolType symbolDGL,
                                      Revit.DB.AnnotationSymbolType symbolScale)
        {
            // 初期化
            double px = 0.0;
            double py = 0.0;
            Revit.DB.XYZ locPos;
            Revit.DB.XYZ pos1;
            Revit.DB.XYZ pos2;
            double unitCoe = _CmpGeometry.UnitCoe;
            Revit.DB.FamilyInstance elemFamInst;
            Revit.DB.AnnotationSymbol elemAnnotation;
            Revit.DB.Curve crv;
            Revit.DB.CurveElement crvElem;

            // 既存BGL
            px = 0.0;
            py = 0.0;
            if (bmHeight != 0)
            {
                locPos = new Revit.DB.XYZ(px, py, 0.0);
                elemFamInst = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(locPos, symbolCurrentBGL, view);
                if (elemFamInst != null)
                {
                    elemAnnotation = elemFamInst as Revit.DB.AnnotationSymbol;
                }
            }

            // 設計GL
            py = -bmHeight / unitCoe * raiteVertical;
            locPos = new Revit.DB.XYZ(px, py, 0.0);
            elemFamInst = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(locPos, symbolDGL, view);
            if (elemFamInst != null)
            {
                elemAnnotation = elemFamInst as Revit.DB.AnnotationSymbol;

                if (elemAnnotation != null)
                {
                    string bmHeightStr = "";
                    if (bmHeight <= 0.0)
                    {
                        bmHeightStr = "+";
                    }
                    else
                    {
                        bmHeightStr = "-";
                    }
                    bmHeightStr += System.Math.Abs(bmHeight).ToString();
                    _EntSpAnnotation.CurrentElem = elemAnnotation;
                    _EntSpAnnotation.BGL = bmHeightStr;
                }
            }

            // 設計GL線分
            double valExt = double.Parse(_CmpAttribute.ResourceText("IDS_VAL_GLEXT")) / unitCoe * scale;
            pos1 = locPos;
            pos2 = new Revit.DB.XYZ(maxX + valExt, py, 0);
            crv = Revit.DB.Line.CreateBound(pos1, pos2);
            crvElem = _CmpElements.CreateDetailCurve(view, crv);

            // 縮尺
            px = double.Parse(_CmpAttribute.ResourceText("IDS_VAL_SCALEPOSX")) / unitCoe * scale;
            py = maxY + (double.Parse(_CmpAttribute.ResourceText("IDS_VAL_DIMPOSY")) / unitCoe * scale);
            locPos = new Revit.DB.XYZ(px, py, 0.0);
            elemFamInst = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(locPos, symbolScale, view);
            if (elemFamInst != null)
            {
                elemAnnotation = elemFamInst as Revit.DB.AnnotationSymbol;

                if (elemAnnotation != null)
                {
                    double dScale = (double)(scale);
                    double dRaiteVertical = (double)(raiteVertical);
                    double dRaiteHorizontal = (double)(raiteHorizontal);
                    double mod = 0.0;
                    double scRate = 0.0;

                    // 垂直縮尺文字
                    string scaleVeticalStr = "";
                    scRate = dScale / dRaiteVertical;
                    mod = dScale % dRaiteVertical;
                    if (mod == 0.0)
                    {
                        scaleVeticalStr = ((int)(scRate)).ToString();
                    }
                    else
                    {
                        scaleVeticalStr = UtilValue.Rounding(scRate, 2, 2);
                        scaleVeticalStr = _CmpParameters.StrZeroPadding(scaleVeticalStr, 1);
                    }

                    // 水平縮尺文字
                    string scaleHorizontalStr = "";
                    scRate = dScale / dRaiteHorizontal;
                    mod = dScale % dRaiteHorizontal;
                    if (mod == 0.0)
                    {
                        scaleHorizontalStr = ((int)(scRate)).ToString();
                    }
                    else
                    {
                        scaleHorizontalStr = UtilValue.Rounding(scRate, 2, 2);
                        scaleHorizontalStr = _CmpParameters.StrZeroPadding(scaleHorizontalStr, 1);
                    }

                    _EntSpAnnotation.CurrentElem = elemAnnotation;
                    _EntSpAnnotation.ScaleVertical = scaleVeticalStr;
                    _EntSpAnnotation.ScaleHorizontal = scaleHorizontalStr;
                }
            }
        }

        /// ================================================================================
        /// <summary>算定図の寸法作成</summary>
        ///
        /// <param name="view"                        >ビュー</param>
        /// <param name="scale"                       >縮尺</param>
        /// <param name="maxX"                        >Xの最大値</param>
        /// <param name="maxY"                        >Yの最大値</param>
        /// <param name="refAry1"                     >参照配列 1</param>
        /// <param name="refAry2"                     >参照配列 2</param>
        /// <param name="valuesX"                     >Xの値</param>
        /// <param name="numbers"                     >番号</param>
        /// <param name="symbolAveGlLevelCalcPosSign" >平均地盤面算定ポイント符号シンボル</param>
        /// <param name="elemDim1"                    >寸法1要素</param>
        /// <param name="elemDim2"                    >寸法2要素</param>
        ///
        /// <history><p>2011/08/01 Created  GSA,Inc. Shinichi Ishii</p>
        ///          <p>2012/04/19 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        public void CreateDimensionCalcDraw(Revit.DB.View view,
                                     int scale,
                                     double maxX,
                                     double maxY,
                                     Revit.DB.ReferenceArray refAry1,
                                     Revit.DB.ReferenceArray refAry2,
                                     Collections.Generic.IList<double> valuesX,
                                     Collections.Generic.IList<int> numbers,
                                     Revit.DB.AnnotationSymbolType symbolAveGlLevelCalcPosSign,
                                     ref Revit.DB.Dimension elemDim1,
                                     ref Revit.DB.Dimension elemDim2)
        {
            double p1x = 0.0;
            double p1y = 0.0;
            double p2x = 0.0;
            double p2y = 0.0;
            Revit.DB.XYZ pos1;
            Revit.DB.XYZ pos2;
            double unitCoe = _CmpGeometry.UnitCoe;

            double dimPosY = double.Parse(_CmpAttribute.ResourceText("IDS_VAL_DIMPOSY")) / unitCoe * scale;
            double dimDistY = double.Parse(_CmpAttribute.ResourceText("IDS_VAL_DIMDISTY")) / unitCoe * scale;
            double dimSupDist = double.Parse(_CmpAttribute.ResourceText("IDS_VAL_DIMSUPDIST")) / unitCoe;
            double dimSignDist = double.Parse(_CmpAttribute.ResourceText("IDS_VAL_SIGNPOSY")) / unitCoe * scale;

            // 寸法 1
            p1x = 0.0;
            p1y = maxY + dimPosY;
            p2x = maxX;
            p2y = p1y;
            pos1 = new Revit.DB.XYZ(p1x, p1y, 0.0);
            pos2 = new Revit.DB.XYZ(p2x, p2y, 0.0);
            elemDim1 = _CmpElements.CreateDimension(view, pos1, pos2, refAry1);

            // 寸法 2
            p1x = 0.0;
            p1y = p1y + dimDistY;
            p2x = maxX;
            p2y = p1y;
            pos1 = new Revit.DB.XYZ(p1x, p1y, 0.0);
            pos2 = new Revit.DB.XYZ(p2x, p2y, 0.0);
            elemDim2 = _CmpElements.CreateDimension(view, pos1, pos2, refAry2);

            // 寸法タイプ
            _EntSpDimType.CurrentElem = elemDim1.DimensionType;
            _EntSpDimType.AuxLineType = 1;
            _EntSpDimType.AuxLineLength = dimSupDist;
            _EntSpDimType.AuxLineExtensionLength = 0.0;

            // 寸法符号
            p1y = p1y + dimSignDist;
            if (valuesX.Count > 0)
            {
                Revit.DB.FamilyInstance elemFamInst;
                Revit.DB.AnnotationSymbol elemAnnotation;
                for (int i = 0; i < valuesX.Count; ++i)
                {
                    pos1 = new Revit.DB.XYZ(valuesX[i], p1y, 0.0);
                    elemFamInst = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(pos1, symbolAveGlLevelCalcPosSign, view);
                    if (elemFamInst != null)
                    {
                        elemAnnotation = elemFamInst as Revit.DB.AnnotationSymbol;
                        if (elemAnnotation != null)
                        {
                            if (i < numbers.Count)
                            {
                                _EntSpAnnotation.CurrentElem = elemAnnotation;
                                _EntSpAnnotation.AveGlLvlCalcPosNo = numbers[i];
                            }
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>式の値をリストに追加</summary>
        ///
        /// <param name="value"       >値</param>
        /// <param name="decimalType" >小数点位置</param>
        /// <param name="fractionType">端数タイプ</param>
        /// <param name="listVal"     >値リスト</param>
        /// <param name="listDigit"   >桁リスト</param>
        /// <param name="maxDigit"    >最大桁</param>
        ///
        /// <history><p>2011/08/01 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2015/09/11 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public void AddListValExp(double value,
                           int decimalType,
                           int fractionType,
                           ref Collections.Generic.IList<string> listVal,
                           ref Collections.Generic.IList<int> listDigit,
                           ref int maxDigit)
        {
            // 負数の処理
            double valueAbs = value;
            bool flagAbs = false;
            if (value < 0.0)
            {
                valueAbs = System.Math.Abs(value);
                flagAbs = true;
            }

            // 桁処理
            string sValue = UtilValue.Rounding(valueAbs, decimalType, fractionType);
            sValue = _CmpParameters.StrZeroPadding(sValue, decimalType - 1);
            if (flagAbs == true)
            {
                double dValue = double.Parse(sValue);
                if (dValue != 0.0)
                {
                    sValue = "-" + sValue;
                }
            }
            listVal.Add(sValue);

            // 文字数
            int digit = UtilValue.GetByteCountString(sValue);
            listDigit.Add(digit);

            // 最大文字数
            if (maxDigit < digit)
            {
                maxDigit = digit;
            }
        }

        /// ================================================================================
        /// <summary>桁処理(負数対応)</summary>
        ///
        /// <param name="value"       >値</param>
        /// <param name="decimalType" >小数点位置</param>
        /// <param name="fractionType">端数タイプ</param>
        ///
        /// <returns>値</returns>
        ///
        /// <history><p>2011/08/01 Created GSA,Inc. Shinichi Ishii</p>
        ///          <p>2015/09/11 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public string RoundingNegNum(double value,
                              int decimalType,
                              int fractionType)
        {
            // 戻り値
            string ret = "0";

            // 負数の処理
            double valueAbs = value;
            bool flagAbs = false;
            if (value < 0.0)
            {
                valueAbs = System.Math.Abs(value);
                flagAbs = true;
            }

            // 桁処理
            ret = UtilValue.Rounding(valueAbs, decimalType, fractionType);
            ret = _CmpParameters.StrZeroPadding(ret, decimalType - 1);
            if (flagAbs == true)
            {
                double dValue = double.Parse(ret);
                if (dValue != 0.0)
                {
                    ret = "-" + ret;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>式の値に空白追加</summary>
        ///
        /// <param name="value"   >値</param>
        /// <param name="digit"   >桁</param>
        /// <param name="maxDigit">最大桁</param>
        ///
        /// <returns>値</returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public string AddBlankValExp(string value,
                              int digit,
                              int maxDigit)
        {
            int blankNum = maxDigit - digit;
            string blkStr = UtilValue.CreateBlankString(blankNum);
            return blkStr + value;
        }

        /// ================================================================================
        /// <summary>式の値に空白追加</summary>
        ///
        /// <param name="value"   >値</param>
        /// <param name="digit"   >桁</param>
        /// <param name="maxDigit">最大桁</param>
        /// <param name="sign"    >符号</param>
        /// <param name="retValue">戻り値</param>
        ///
        /// <history>2011/09/04 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void AddBlankValExp(string value,
                            int digit,
                            int maxDigit,
                            ref string sign,
                            ref string retValue)
        {
            int blankNum = maxDigit - digit;
            sign = " + ";
            string valStr = value;
            if (double.Parse(value) < 0.0)
            {
                blankNum++;
                sign = " - ";
                valStr = valStr.Substring(1);
            }

            string blkStr = UtilValue.CreateBlankString(blankNum);
            retValue = blkStr + valStr;
        }

        /// ================================================================================
        /// <summary>半時計廻りに並び替え</summary>
        ///
        /// <param name="curveElems"  >カーブ要素</param>
        /// <param name="curves"      >カーブ</param>
        /// <param name="vertexIndex" >頂点インデックス</param>
        /// <param name="flagConnect" >接続フラグ</param>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void SortClockwise(Collections.Generic.IList<Revit.DB.CurveElement> curveElems,
                           ref Collections.Generic.IList<Revit.DB.Curve> curves,
                           ref Collections.Generic.IList<Collections.Generic.IList<int>> vertexIndex,
                           ref Collections.Generic.IList<bool> connect)
        {
            // ジオメトリ
            Collections.Generic.IList<Revit.DB.Curve> geoCurves = new Collections.Generic.List<Revit.DB.Curve>();
            foreach (Revit.DB.CurveElement crvElem in curveElems)
            {
                geoCurves.Add(crvElem.GeometryCurve);
            }

            // 左下頂点
            int indexCrv0 = -1;
            int indexVer0 = -1;
            GetLeftBottomIndex(geoCurves, ref indexCrv0, ref indexVer0);

            // 左下頂点の同線１点
            int indexCrv1 = indexCrv0;
            int indexVer1 = System.Math.Abs(indexVer0 - 1);

            // 左下頂点につながるもう１点
            int indexCrv2 = -1;
            int indexVer2 = -1;
            Revit.DB.XYZ pos1 = geoCurves[indexCrv0].GetEndPoint(indexVer0);
            Revit.DB.XYZ pos2;
            for (int i = 0; i < geoCurves.Count; ++i)
            {
                if (i != indexCrv0)
                {
                    for (int j = 0; j < 2; ++j)
                    {
                        pos2 = geoCurves[i].GetEndPoint(j);
                        if (_CmpGeometry.Distance2D(pos1, pos2) < _CmpGeometry.Approx0Len)
                        {
                            indexCrv2 = i;
                            indexVer2 = System.Math.Abs(j - 1);
                            break;
                        }
                    }
                    if (indexCrv2 != -1)
                    {
                        break;
                    }
                }
            }

            // 次のポイント(最初のカーブ）
            if (indexCrv2 != -1)
            {
                double cross = _CmpGeometry.CrossProduct2D(geoCurves[indexCrv0].GetEndPoint(indexVer0),
                                                           geoCurves[indexCrv1].GetEndPoint(indexVer1),
                                                           geoCurves[indexCrv2].GetEndPoint(indexVer2));
                // 半時計回り(cross < 0)の場合
                if (cross < 0)
                {
                    indexCrv0 = indexCrv2;
                    indexVer0 = System.Math.Abs(indexVer2 - 1);
                }
            }

            // 最初の線
            curves.Add(geoCurves[indexCrv0].Clone());
            geoCurves.RemoveAt(indexCrv0);

            indexVer1 = System.Math.Abs(indexVer0 - 1);
            Collections.Generic.IList<int> curveIndex = new Collections.Generic.List<int>();
            curveIndex.Add(indexVer0);
            curveIndex.Add(indexVer1);
            vertexIndex.Add(curveIndex);

            pos1 = curves[0].GetEndPoint(indexVer1);

            // 並び替え
            int geoCurvesLen = geoCurves.Count;
            if (geoCurvesLen == 0)
            {
                connect.Add(false);
                return;
            }

            for (int i = 0; i < geoCurvesLen; ++i)
            {
                // 同点検索
                indexCrv0 = -1;
                for (int j = 0; j < geoCurves.Count; ++j)
                {
                    for (int k = 0; k < 2; ++k)
                    {
                        pos2 = geoCurves[j].GetEndPoint(k);
                        if (_CmpGeometry.Distance2D(pos1, pos2) < _CmpGeometry.Approx0Len)
                        {
                            indexCrv0 = j;
                            indexVer0 = k;
                            connect.Add(true);
                            break;
                        }
                    }
                    if (indexCrv0 != -1)
                    {
                        break;
                    }
                }
                // 近似点検索
                if (indexCrv0 == -1)
                {
                    connect.Add(false);
                    indexCrv0 = 0;
                    indexVer0 = 0;
                    pos2 = geoCurves[indexCrv0].GetEndPoint(indexVer0);
                    double distMin = _CmpGeometry.Distance2D(pos1, pos2);
                    for (int j = 0; j < geoCurves.Count; ++j)
                    {
                        for (int k = 0; k < 2; ++k)
                        {
                            pos2 = geoCurves[j].GetEndPoint(k);
                            double dist = _CmpGeometry.Distance2D(pos1, pos2);
                            if (distMin > dist)
                            {
                                distMin = dist;
                                indexCrv0 = j;
                                indexVer0 = k;
                            }
                        }
                    }
                }

                // リストに追加
                curves.Add(geoCurves[indexCrv0].Clone());
                geoCurves.RemoveAt(indexCrv0);

                indexVer1 = System.Math.Abs(indexVer0 - 1);
                curveIndex = new Collections.Generic.List<int>();
                curveIndex.Add(indexVer0);
                curveIndex.Add(indexVer1);
                vertexIndex.Add(curveIndex);

                pos1 = curves[curves.Count - 1].GetEndPoint(indexVer1);
            }

            // 最後のコネクト
            indexVer0 = vertexIndex[vertexIndex.Count - 1][1];
            indexVer1 = vertexIndex[0][0];
            pos1 = curves[curves.Count - 1].GetEndPoint(indexVer0);
            pos2 = curves[0].GetEndPoint(indexVer1);
            if (_CmpGeometry.Distance2D(pos1, pos2) < _CmpGeometry.Approx0Len)
            {
                connect.Add(true);
            }
            else
            {
                connect.Add(false);
            }
        }

        /// ================================================================================
        /// <summary>三角形と点の内外判定</summary>
        ///
        /// <param name="pos"       >ポイント</param>
        /// <param name="triMeshes" >三角メッシュ</param>
        /// <param name="zValue"    >内側にあった場合のZ値</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 内側</p>
        ///             <p>False = 外側</p></returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool CheckTrianglePoint(Revit.DB.XYZ pos,
                                Collections.Generic.IList<Revit.DB.MeshTriangle> triMeshes,
                                ref double zValue)
        {
            bool ret = false;
            try
            {
                // 三角メッシュの内外判定で取得
                foreach (Revit.DB.MeshTriangle triMesh in triMeshes)
                {
                    // 2Dでポイントが含まれる三角形
                    double z = 0;
                    int retFunc = ChackTrianglePoint2D(pos, triMesh);
                    
                    if (retFunc == 3)
                    {
                        // ３角メッシュの内点のZ値取得
                        if (GetZValuePointInTriMesh(pos, triMesh, ref z) == true)
                        {
                            ret = true;
                            if( z > zValue)
                                zValue = z;
                            continue;
                        }
                    }
                    else if ((retFunc >= 0) && (retFunc <= 2))
                    {
                        ret = true;
                        zValue = MeshTriangleAccess.GetVertex(triMesh, retFunc).Z;
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                ret = false;
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>線分リストの左下頂点のインデックス取得</summary>
        ///
        /// <param name="curves"      >線分リスト</param>
        /// <param name="indexCurve"  >線分インデックス</param>
        /// <param name="indexVertex" >頂点インデックス</param>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public void GetLeftBottomIndex(Collections.Generic.IList<Revit.DB.Curve> curves,
                                ref int indexCurve,
                                ref int indexVertex)
        {
            double approx0Len = _CmpGeometry.Approx0Len;
            indexCurve = -1;
            indexVertex = -1;
            if (curves.Count == 0)
            {
                return;
            }

            // 一番左下点を確認
            Revit.DB.XYZ pos = null;
            Revit.DB.XYZ firstPos = curves[0].GetEndPoint(0);
            for (int i = 0; i < curves.Count; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    pos = curves[i].GetEndPoint(j);
                    if (firstPos.Y > pos.Y)
                    {
                        if (System.Math.Abs(System.Math.Abs(firstPos.Y) - System.Math.Abs(pos.Y)) < approx0Len)
                        {
                            if (firstPos.X > pos.X)
                            {
                                firstPos = pos;
                            }
                        }
                        else
                        {
                            firstPos = pos;
                        }
                    }
                    else
                    {
                        if (System.Math.Abs(System.Math.Abs(firstPos.Y) - System.Math.Abs(pos.Y)) < approx0Len)
                        {
                            if (firstPos.X > pos.X)
                            {
                                firstPos = pos;
                            }
                        }
                    }
                }
            }

            // 左下点のインデックス
            for (int i = 0; i < curves.Count; ++i)
            {
                for (int j = 0; j < 2; ++j)
                {
                    if (_CmpGeometry.Distance(curves[i].GetEndPoint(j), firstPos) < approx0Len)
                    {
                        indexCurve = i;
                        indexVertex = j;
                        break;
                    }
                }
                if (indexCurve > -1)
                {
                    break;
                }
            }
        }

        /// ================================================================================
        /// <summary>直線か判断</summary>
        ///
        /// <param name="elemCurves">線分リスト</param>
        ///
        /// <returns>結果</returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool IsLine(Collections.Generic.IList<Revit.DB.CurveElement> elemCurves)
        {
            bool ret = false;

            if (elemCurves == null)
            {
                return ret;
            }

            if (elemCurves.Count == 0)
            {
                return ret;
            }

            ret = true;
            foreach (Revit.DB.CurveElement elemCurve in elemCurves)
            {
                Revit.DB.Curve curve = elemCurve.GeometryCurve;
                Revit.DB.Line line = curve as Revit.DB.Line;
                if (line == null)
                {
                    ret = false;
                    break;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>三角形と点の内外判定</summary>
        ///
        /// <param name="pos" >ポイント</param>
        /// <param name="pos1">１つ目のポイント</param>
        /// <param name="pos2">２つ目のポイント</param>
        /// <param name="pos3">３つ目のポイント</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = 内側</p>
        ///             <p>False = 外側</p></returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool ChackTrianglePoint2D(Revit.DB.XYZ pos,
                                  Revit.DB.XYZ pos1,
                                  Revit.DB.XYZ pos2,
                                  Revit.DB.XYZ pos3)
        {
            bool ret = false;

            Revit.DB.XYZ posB = new Revit.DB.XYZ(pos.X, pos.Y, 0.0);
            Collections.Generic.IList<Revit.DB.XYZ> poss = new Collections.Generic.List<Revit.DB.XYZ>();
            poss.Add(new Revit.DB.XYZ(pos1.X, pos1.Y, 0.0));
            poss.Add(new Revit.DB.XYZ(pos2.X, pos2.Y, 0.0));
            poss.Add(new Revit.DB.XYZ(pos3.X, pos3.Y, 0.0));

            try
            {
                // 三角形の重心
                Revit.DB.XYZ gravity = _CmpGeometry.TriangleGravity2D(poss[0], poss[1], poss[2]);
                if (gravity == null)
                {
                    return ret;
                }

                // 三角形重心とポイントの線分
                Revit.DB.Line line1 = null;
                Revit.DB.Line line2 = null;
                Revit.DB.IntersectionResultArray interRetAry = null;
                line1 = Revit.DB.Line.CreateBound(posB, gravity);

                // 線分と三角形の交点
                bool flag = false;
                for (int i = 0; i < 3; ++i)
                {
                    int j = i + 1;
                    if (j > 2)
                    {
                        j = 0;
                    }
                    interRetAry = new Revit.DB.IntersectionResultArray();
                    line2 = Revit.DB.Line.CreateBound(poss[i], poss[j]);
                    if (line2 != null)
                    {
                        if (_CmpGeometry.CompareInterCurve(line1, line2, ref interRetAry) == Revit.DB.SetComparisonResult.Overlap)
                        {
                            flag = true;
                            break;
                        }
                    }
                    else
                    {
                        flag = true;
                        break;
                    }
                }

                // 交点判定
                if (flag == false)
                {
                    ret = true;
                }
            }
            catch (System.Exception ex)
            {
                string errMsg = ex.Message;
                ret = false;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>三角形と点の内外判定</summary>
        ///
        /// <param name="pos"     >ポイント</param>
        /// <param name="triMesh" >三角メッシュ</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>-1  = 外側</p>
        ///             <p>0-2 = 頂点上(頂点番号)</p>
        ///             <p>3   = 内側</p></returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public int ChackTrianglePoint2D(Revit.DB.XYZ pos, Revit.DB.MeshTriangle triMesh)
        {
            int ret = -1;
            Collections.Generic.IList<Revit.DB.XYZ> poss = new Collections.Generic.List<Revit.DB.XYZ>();
            poss.Add(MeshTriangleAccess.GetVertex(triMesh, 0));
            poss.Add(MeshTriangleAccess.GetVertex(triMesh, 1));
            poss.Add(MeshTriangleAccess.GetVertex(triMesh, 2));

            for (int i = 0; i < 3; ++i)
            {
                if (_CmpGeometry.Distance2D(pos, poss[i]) < _CmpGeometry.Approx0Len)
                {
                    ret = i;
                    break;
                }
            }

            if (ret == -1)
            {
                bool retFunc = ChackTrianglePoint2D(pos, poss[0], poss[1], poss[2]);
                if (retFunc == true)
                {
                    ret = 3;
                }
            }
            return ret;
        }

        /// ================================================================================
        /// <summary>３角メッシュの内点のZ値取得</summary>
        ///
        /// <param name="pos"     >ポイント</param>
        /// <param name="triMesh" >三角メッシュ</param>
        /// <param name="zValue"  >交点</param>
        ///
        /// <returns><p>結果</p>
        ///             <p>True  = ポイントが三角メッシュ内</p>
        ///             <p>False = ポイントが三角メッシュ外</p></returns>
        ///
        /// <history>2011/08/01 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public bool GetZValuePointInTriMesh(Revit.DB.XYZ pos,
                                     Revit.DB.MeshTriangle triMesh,
                                     ref double zValue)
        {
            bool ret = false;

            if ((pos == null) || (triMesh == null))
            {
                return ret;
            }

            // 三角メッシュの法線
            Revit.DB.XYZ normal = _CmpGeometry.CrossProduct(MeshTriangleAccess.GetVertex(triMesh, 0), MeshTriangleAccess.GetVertex(triMesh, 1), MeshTriangleAccess.GetVertex(triMesh, 2));

            // ベクトル
            double vecX = pos.X - MeshTriangleAccess.GetVertex(triMesh, 0).X;
            double vecY = pos.Y - MeshTriangleAccess.GetVertex(triMesh, 0).Y;
            double vecZ = (-(vecX * normal.X) - (vecY * normal.Y)) / normal.Z;

            // Z値
            zValue = vecZ + MeshTriangleAccess.GetVertex(triMesh, 0).Z;

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>Place tag to current document</summary>
        ///
        /// <param name="doc"           >Current document</param>
        /// <param name="pos"           >Position of tag</param>
        /// <param name="smbolCircle"   >Family type of circle</param>
        /// <param name="symbolTag"     >Family type of tag</param>
        /// <param name="fmCircle"      >Out circle</param>
        /// <param name="tag"           >Out tag</param>
        ///
        /// <returns>True or false</returns>
        ///
        /// <history>2021/12/20 Created Applied Technology</history>
        /// ================================================================================
        private bool PlaceSymbolTag(Revit.DB.Document doc, Revit.DB.XYZ pos, Revit.DB.FamilySymbol smbolCircle,
                                    Revit.DB.FamilySymbol symbolTag, out Revit.DB.FamilyInstance fmCircle, out Revit.DB.IndependentTag tag)
        {
            // Set default
            fmCircle = null;
            tag = null;

            if (smbolCircle == null || symbolTag == null)
                return false;

            // View scale
            int nViewScale = doc.ActiveView.Scale;

            // Get Z of tag
            pos = new Revit.DB.XYZ(pos.X, pos.Y, doc.ActiveView.GenLevel.Elevation);

            try
            {
                // Active symbol
                if (smbolCircle.IsActive == false)
                    smbolCircle.Activate();
                if (symbolTag.IsActive == false)
                    symbolTag.Activate();

                // Create circle
                fmCircle = doc.Create.NewFamilyInstance(pos, smbolCircle, doc.ActiveView);
                if (fmCircle == null)
                    return false;

                // Pin circle
                fmCircle.Pinned = true;

                // Get ref circle
                Revit.DB.Reference refCircle = new Revit.DB.Reference(fmCircle);
                if (refCircle == null)
                    return false;

                // Place tag
                tag = Revit.DB.IndependentTag.Create(doc, symbolTag.Id, doc.ActiveView.Id, refCircle, true, Revit.DB.TagOrientation.Horizontal, pos);
                if (tag == null)
                    return false;

                // Get setting file default value
                GetSettingDistanceValue(out double distX, out double distY);

                // Convert unit
                distX = Revit.DB.UnitUtils.ConvertToInternalUnits(distX, Revit.DB.UnitTypeId.Millimeters);
                distY = Revit.DB.UnitUtils.ConvertToInternalUnits(distY, Revit.DB.UnitTypeId.Millimeters);

                // Set leader position = distance * scale
                var headerPos = pos + new Revit.DB.XYZ(distX * nViewScale, distY * nViewScale, pos.Z);

                tag.TagHeadPosition = headerPos;
            }
            catch (Exception ex)
            {
                var mess = ex.Message;
                return false;
            }

            return true;
        }

        /// ================================================================================
        /// <summary>Get default setting of setting file</summary>
        ///
        /// <param name="distX"           >Out distance of X value</param>
        /// <param name="distY"           >Out distance of Y value</param>
        ///
        /// <history>2021/12/29 Created Applied Technology</history>
        /// ================================================================================
        public void GetSettingDistanceValue(out double distX, out double distY)
        {
            // default a and b
            distX = 100;
            distY = -100;

            // Get file path
            var filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                    + "\\" + _CmpAttribute.ResourceText("IDS_FILESETTINGNAME");

            if (System.IO.File.Exists(filePath) == false)
                return;

            //Register providers for legacy encodings like shift_jis
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            // read value from file settings
            string[] arrValue = System.IO.File.ReadAllLines(filePath, System.Text.Encoding.GetEncoding("shift_jis"));

            if (arrValue.Length < 4)
                return;

            // Get distance a
            if (UtilValue.IsNumber(arrValue[2]))
                distX = double.Parse(arrValue[2]);

            // Get distance b
            if (UtilValue.IsNumber(arrValue[3]))
                distY = double.Parse(arrValue[3]);
        }

        #endregion Member Functions

        // プロパティ
    }
}