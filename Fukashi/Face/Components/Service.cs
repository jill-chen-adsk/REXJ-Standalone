using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi;

namespace ADSK.Ext.Fukashi.Face.Components
{
  /// ================================================================================
  /// <summary>サービス</summary>
  /// ================================================================================
  class Service
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private RvtExtApp.Face.Components.Attribute _CmpAttribute;
    /// <summary>要素</summary>
    private RvtExtApp.Face.Components.Elements _CmpElements;
    /// <summary>図形</summary>
    private RvtExtApp.Face.Components.Geometry _CmpGeometry;
    /// <summary>パラメータ</summary>
    private RvtExtApp.Face.Components.Parameters _CmpParameters;
    /// <summary>設定</summary>
    private RvtExtApp.Face.Components.Settings _CmpSettings;

    /// <summary>データテーブル コマンド</summary>
    private RvtExtApp.Face.Entities.DtCmd _EntDtCmd;

    const double PIW = 6.283185307179586;
    const double Ztol99 = 0.999999985;
    public Revit.DB.XYZ pcpos;
    #endregion

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
        /// <history>2016/11/17 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Service(RvtExtApp.Face.Components.Attribute cmpAttribute,
            RvtExtApp.Face.Components.Elements cmpElements,
            RvtExtApp.Face.Components.Geometry cmpGeometry,
            RvtExtApp.Face.Components.Parameters cmpParameters,
            RvtExtApp.Face.Components.Settings cmpSettings)
    {
      _CmpAttribute   = cmpAttribute;
      _CmpElements    = cmpElements;
      _CmpGeometry    = cmpGeometry;
      _CmpParameters  = cmpParameters;
      _CmpSettings    = cmpSettings;
    }
    #endregion

    // メンバ関数
    #region Member Functions
    /// ================================================================================
    /// <summary>ワークフロー</summary>
    /// 
    /// <history>2016/08/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    string WorkFlow()
    {
      string ret = null;

      // プロジェクト情報
      Revit.DB.ProjectInfo projInfo = _CmpElements.ProjectInfo;

      _EntDtCmd = new RvtExtApp.Face.Entities.DtCmd(_CmpAttribute,
                                               _CmpElements,
                                               _CmpGeometry,
                                               _CmpParameters,
                                               _CmpSettings,
                                               projInfo,
                                               _CmpAttribute.ResourceText("IDS_SHPARAM_DEF"),
                                               2);

      return ret;
    }

    /// ================================================================================
    /// <summary>設定</summary>
    /// 
    /// <history>2016/12/05 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void Set()
    {
      _EntDtCmd.SetData();
    }

    /// ================================================================================
    /// <summary>面フカシ作成 - 三角形</summary>
    /// 
    /// <param name="plnFace"     >基準面</param>
    /// <param name="heightFace"  >高さ用面</param>
    /// 
    /// <history>2016/12/12 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Triangle(Revit.DB.PlanarFace plnFace,
                                                   Revit.DB.PlanarFace plnFaceBase,
                                                Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                                Revit.DB.PlanarFace heightFace,
                                                Revit.DB.Edge heightedge)
    {
        Revit.DB.FamilyInstance ret = null;
      // 高さ用面
      Revit.DB.XYZ p0 = heightFace.Origin;
      Revit.DB.XYZ p1 = p0 + heightFace.XVector;
      Revit.DB.XYZ p2 = p0 + heightFace.YVector;

      Revit.DB.XYZ normal = plnFace.FaceNormal;
      Collections.Generic.IList<Revit.DB.Curve> curves = _CmpGeometry.GetFaceCurves(plnFace);

      //Faceのカーブを置き換え
      if(curvesA.Count != 0)
      {
          curves = _CmpGeometry.GetCurves(curvesA); ;
      }

      Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();
      Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> posBtmTops = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      //面から生成
      if(heightedge == null)
      { 
          foreach (Revit.DB.Curve curve in curves)
          {
            Revit.DB.XYZ p = curve.GetEndPoint(0);
            Revit.DB.Line line = Revit.DB.Line.CreateBound(p, p + normal);

            // 面への投影
            Revit.DB.XYZ crossPos = _CmpGeometry.GetCrossPoint(line, p0, p1, p2, 1);

            if (crossPos == null)
            {
              break;
            }

            Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
            posAry.Add(p);
            posAry.Add(crossPos);

            posBtmTops.Add(posAry);
          }

          // 投影交点なしなど
          if (posBtmTops.Count != curves.Count)
          {
            return ret;
          }

          // 各頂点からの投影距離
          foreach (Collections.Generic.IList<Revit.DB.XYZ> btmTop in posBtmTops)
          {
            heights.Add(_CmpGeometry.Distance(btmTop[0], btmTop[1]));
          }

            //高さチェック 0はNG
            double dTotalLen = 0.0;
            foreach (double len in heights)
            {
                dTotalLen += len;
            }
            if (dTotalLen < _CmpGeometry.Approx0Len)
            {
                return ret;
            }

          // 距離別数
          //Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();
          foreach (double h in heights)
          {
            bool contain = false;

            foreach (double key in dicHeightCount.Keys)
            {
              if (System.Math.Abs(h - key) < _CmpGeometry.Approx0Len)
              {
                dicHeightCount[key] += 1;

                contain = true;
                break;
              }
            }

            if (contain == false)
            {
              dicHeightCount.Add(h, 1);
            }
          }
      }
      else//エッジから生成
      {
            Revit.DB.XYZ pb0 = plnFaceBase.Origin;
            Revit.DB.XYZ pb1 = pb0 + plnFaceBase.XVector;
            Revit.DB.XYZ pb2 = pb0 + plnFaceBase.YVector;

            Revit.DB.XYZ pa1 = heightedge.AsCurve().GetEndPoint(0) - pb0;
            double h1 = _CmpGeometry.Naiseki(normal, pa1);
            if(h1 < 0)
            {
                //埋まる
                return ret;
            }

            Revit.DB.XYZ pa2 = heightedge.AsCurve().GetEndPoint(1) - pb0;
            double h2 = _CmpGeometry.Naiseki(normal, pa2);
            if(System.Math.Abs(h1- h2) > _CmpGeometry.Approx0Len)
            {
                //傾いている
                return ret;
            }

            heights.Add(h1);
            dicHeightCount.Add(h1, 1);
      }

        // すべて同じ距離
        if (dicHeightCount.Keys.Count == 1)
        {
            if (heightedge != null)
            {
                if (!_CmpGeometry.PlaneFaceRel(plnFace, heightFace))
                    return ret; //ふかすと埋まってしまう
            }

        Revit.DB.FamilySymbol famSym = _CmpElements.FamSymTriangle;
        if (famSym != null)
        {
          Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
          Revit.DB.UV uv = bbUV.Min;
          //Revit.DB.XYZ loc = plnFace.Evaluate(uv);
          Revit.DB.XYZ loc = curves[0].GetEndPoint(0);    //基準点

          Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
          Revit.DB.XYZ refDir = norm.CrossProduct(norm);

          // 外積による面の向き
          Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
          Revit.DB.XYZ direction = plnFace.XVector;
          // 逆向き
          if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
          {
            direction = plnFace.YVector;
          }

            ///////////////////////
            //図形基準ベクトル
            Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(curves[0].GetEndPoint(0), curves[0].GetEndPoint(1));

            //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
            double dot = _CmpGeometry.Naiseki(dirLine, direction);
            if (dot > 1.0) dot = 1.0;
            if (dot < -1.0) dot = -1.0;
            double rad = System.Math.Acos(dot);
            double crs = (direction.X * dirLine.Y - direction.Y * dirLine.X);

            if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;

            double c = System.Math.Cos(rad);
            double s = System.Math.Sin(rad);
            direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);
            if (System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
            {
                direction = dirLine;
            }

            ///////////////////////
            Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
            Revit.DB.FamilyInstance famIns = null;
            double valX = 0;
            double valY = 0;
            if (interRet == null)   //基準点オフセット
            {
                Revit.DB.XYZ org = pcpos;
                Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
            }
            else
            {
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
            }
            Revit.DB.Parameter param;
            param = famIns.LookupParameter("座標X");
            if (param != null)
            {
                param.Set(valX);
            }

            param = famIns.LookupParameter("座標Y");
            if (param != null)
            {
                param.Set(valY);
            }
            ///////////////////////

          ret = famIns;

          Revit.DB.Parameter parThickness = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
          parThickness.Set(heights[0]);

          Revit.DB.Parameter parWidth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
          parWidth.Set(curves[0].Length);

          Revit.DB.XYZ ep1 = curves[0].GetEndPoint(0);
          Revit.DB.XYZ ep2 = curves[1].GetEndPoint(0);
          Revit.DB.XYZ ep3 = curves[2].GetEndPoint(0);

          double rad1 = _CmpGeometry.Angle3DA(ep1, ep2, ep3);
          double rad2 = _CmpGeometry.Angle3DA(ep2, ep3, ep1);
          rad1 = Math.Abs(rad1);
          rad2 = Math.Abs(rad2);

          Revit.DB.Parameter parAngle1 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_ANGLE1"));
          parAngle1.Set(rad1);

          Revit.DB.Parameter parAngle2 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_ANGLE2"));
          parAngle2.Set(rad2);
        }
      }
            return ret;
    }

    /// ================================================================================
    /// <summary>面フカシ作成 - 三角形</summary>
    /// 
    /// <param name="plnFace"     >基準面</param>
    /// <param name="heightFace"  >高さ用面</param>
    /// <param name="heightEdges" >エッジ</param>
    /// <param name="edgesFaces"  >エッジを含む面</param>
    /// 
    /// <history>2016/12/12 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void CreateFukashi_Triangle(Revit.DB.PlanarFace plnFace,
                                Revit.DB.PlanarFace plnFaceBase,
                                Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                Revit.DB.PlanarFace heightFace,
                                Collections.Generic.IList<Revit.DB.Edge> heightEdges,
                                Collections.Generic.IList<Collections.Generic.IList<Revit.DB.PlanarFace>> edgesFaces)
    {
      // 面指定
        if (heightFace != null)
        {
            Revit.DB.FamilyInstance famIns = CreateFukashi_Triangle(plnFace, plnFaceBase, curvesA, heightFace, null);
            if (famIns == null)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANTFACE"),
                                                        _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                return;
            }

            Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
            if (parMaterial != null)
            {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
            }
            else
            {
              parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
              if (parMaterial != null)
              {
                  Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                  parMaterial.Set(materialId);
              }
            }

        }
      // エッジ指定
      else if (heightEdges.Count > 0)
      {
        Revit.DB.Edge edge = heightEdges[0];

        //// エッジと平面の関連性
        //bool relevant = _CmpGeometry.RelevantEdgeOnCurves(edge, curvesA, plnFace);
        //if (relevant == false)
        //{
        //  System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
        //                                       _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
        //  return;
        //}

        Collections.Generic.IList<Revit.DB.PlanarFace> edgeFaces = edgesFaces[0];

        Revit.DB.PlanarFace similarPlnFace = _CmpGeometry.GetSimilarPlnFace(plnFace, edgeFaces);

        if (similarPlnFace != null)
        {
            Revit.DB.FamilyInstance famIns = CreateFukashi_Triangle(plnFace, plnFaceBase, curvesA, similarPlnFace, edge);
            if (famIns == null)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
                                                    _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                return;
            }

          Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
          if (parMaterial != null)
          {
            Revit.DB.ElementId materialId = _CmpElements.MaterialId;
            parMaterial.Set(materialId);
          }
          else
          {
            parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
            if (parMaterial != null)
            {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
            }
          }
        }
        }
    }
    /// ================================================================================
    /// <summary>面フカシ作成 - 四角形</summary>
    /// 
    /// <param name="plnFace"     >基準面</param>
    /// <param name="heightFace"  >高さ用面</param>
    /// 
    /// <history>2016/12/07 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Rectangle(Revit.DB.PlanarFace plnFace,
                                                    Revit.DB.PlanarFace plnFaceBase,
                                                    Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                                    Revit.DB.PlanarFace heightFace,
                                                    Revit.DB.Edge heightedge)
    {
      Revit.DB.FamilyInstance ret = null;

      // 高さ用面
      Revit.DB.XYZ p0 = heightFace.Origin;
      Revit.DB.XYZ p1 = p0 + heightFace.XVector;
      Revit.DB.XYZ p2 = p0 + heightFace.YVector;

      Revit.DB.XYZ normal = plnFace.FaceNormal;
      Collections.Generic.IList<Revit.DB.Curve> curves = _CmpGeometry.GetFaceCurves(plnFace);

      //Faceのカーブを置き換え
      if(curvesA.Count != 0)
      {
          curves = _CmpGeometry.GetCurves(curvesA); ;
      }

      Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
      Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();

      //四角形でエッジ指定（一つ）の場合、エッジが基準面と平行なら、その高さとする(側面長方形となる)
      //斜めである場合は、今まで通り面から高さを取得
      //基準面と並行かチェック
      if (heightedge != null)
      {
        Revit.DB.XYZ pb0 = plnFaceBase.Origin;
        Revit.DB.XYZ pb1 = pb0 + plnFaceBase.XVector;
        Revit.DB.XYZ pb2 = pb0 + plnFaceBase.YVector;

        Revit.DB.XYZ pa1 = heightedge.AsCurve().GetEndPoint(0) - pb0;
        double h1 = _CmpGeometry.Naiseki(normal, pa1);
        if (h1 < _CmpGeometry.Approx0Len)
        {
            //埋まる
            return ret;
        }

        Revit.DB.XYZ pa2 = heightedge.AsCurve().GetEndPoint(1) - pb0;
        double h2 = _CmpGeometry.Naiseki(normal, pa2);
        if (System.Math.Abs(h1 - h2) < _CmpGeometry.Approx0Len)
        {
            //平行
            heights.Add(h1);
            dicHeightCount.Add(h1, 1);
        }
      }

      int indx1 = 0;
      int indx2 = 0;
      double upper = 0;
      double lower = 0;
      double dWidth = 0;
      double dDepth = 0;

      if(heights.Count != 1)
      { 

          Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> posBtmTops = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

          foreach (Revit.DB.Curve curve in curves)
          {
            Revit.DB.XYZ p = curve.GetEndPoint(0);
            Revit.DB.Line line = Revit.DB.Line.CreateBound(p, p + normal);

            // 面への投影
            Revit.DB.XYZ crossPos = _CmpGeometry.GetCrossPoint(line, p0, p1, p2, 1);

            if (crossPos == null)
            {
              break;
            }

            Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
            posAry.Add(p);
            posAry.Add(crossPos);

            posBtmTops.Add(posAry);
          }

          // 投影交点なしなど
          if (posBtmTops.Count != curves.Count)
          {
            return ret;
          }

          // 各頂点からの投影距離
          Revit.DB.XYZ uv0 = null;
    //      Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
          foreach (Collections.Generic.IList<Revit.DB.XYZ> btmTop in posBtmTops)
          {
            Revit.DB.XYZ uv = _CmpGeometry.UnitVector(btmTop[0], btmTop[1]);

            //作成する面が基準面に埋まらないかチェック
            if(uv0 == null)
            {
                uv0 = uv;
            }
            else
            {
                if (_CmpGeometry.Distance(btmTop[0], btmTop[1]) > _CmpGeometry.Approx0Len)
                {
                    if (_CmpGeometry.Naiseki(uv0, uv) < _CmpGeometry.Approx0Len)
                    {
                        return ret;
                    }
                }
            }
            heights.Add(_CmpGeometry.Distance(btmTop[0], btmTop[1]));
          }


          //高さチェック 0はNG
          double dTotalLen = 0.0;
          foreach(double len in heights)
          {
            dTotalLen += len;
          }
          if (dTotalLen < _CmpGeometry.Approx0Len)
          {
              return null;
          }


          //側面台形・三角形のための基準点インデックス
            double maxH = -double.MaxValue;
            foreach(double h in heights)
            {
                if(maxH < h)
                {
                    maxH = h;
                }
            }
            for(int ii = 0; ii < heights.Count; ii++)
            {
                int jj = (ii + 1) % heights.Count;
                double up = heights[ii];
                double lw = heights[jj];

                if (up - lw > _CmpGeometry.Approx0Len)
                {
                    indx1 = jj;
                    indx2 = ii;
                    upper = up;
                    lower = lw;
                    break;
                }
            }
            dWidth = curves[indx1].Length;
            dDepth = curves[indx2].Length;

        }
      // 距離別数
//      Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();
      foreach (double h in heights)
      {
        bool contain = false;

        foreach (double key in dicHeightCount.Keys)
        {
          if (System.Math.Abs(h - key) < _CmpGeometry.Approx0Len)
          {
            dicHeightCount[key] += 1;

            contain = true;
            break;
          }
        }

        if (contain == false)
        {
          dicHeightCount.Add(h, 1);
        }
      }

      // すべて同じ距離
      if (dicHeightCount.Keys.Count == 1)
      {

        if (!_CmpGeometry.PlaneFaceRel(plnFace, heightFace))
            return ret; //ふかすと埋まってしまう

        Revit.DB.FamilySymbol famSym = _CmpElements.FamSymRectRect;

        if (famSym != null)
        {
          Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
          Revit.DB.UV uv = (bbUV.Max + bbUV.Min) / 2;
          //Revit.DB.XYZ loc = plnFace.Evaluate(uv);

          Revit.DB.XYZ loc = curves[0].GetEndPoint(0);    //基準点
          double dW = curves[0].Length;
          double dD = curves[1].Length;
          Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
          Revit.DB.XYZ refDir = norm.CrossProduct(norm);

          // 外積による面の向き
          Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
          Revit.DB.XYZ direction = plnFace.XVector;
          // 逆向き
          if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
          {
            direction = plnFace.YVector;
          }

            ///////////////////////
            //図形基準ベクトル
            Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(curves[0].GetEndPoint(0), curves[0].GetEndPoint(1));

            //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
            double dot = _CmpGeometry.Naiseki(dirLine, direction);
            if (dot > 1.0) dot = 1.0;
            if (dot < -1.0) dot = -1.0;
            double rad = System.Math.Acos(dot);
            double crs = (direction.X * dirLine.Y - direction.Y * dirLine.X);

            if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;

            double c = System.Math.Cos(rad);
            double s = System.Math.Sin(rad);
            direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);

            if (System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
            {
                direction = dirLine;
            }
            ///////////////////////

            ///////////////////////
            Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
            Revit.DB.FamilyInstance famIns = null;
            double valX = 0;
            double valY = 0;
            if (interRet == null)   //基準点オフセット
            {
                Revit.DB.XYZ org = pcpos;
                Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
            }
            else
            {
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
            }
            Revit.DB.Parameter param;
            param = famIns.LookupParameter("座標X");
            if (param != null)
            {
                param.Set(valX);
            }

            param = famIns.LookupParameter("座標Y");
            if (param != null)
            {
                param.Set(valY);
            }
            ///////////////////////

          Revit.DB.Parameter parThickness = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
          parThickness.Set(heights[0]);

          Revit.DB.Parameter parWidth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
          parWidth.Set(dW);

          Revit.DB.Parameter parBreadth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH"));
          parBreadth.Set(dD);


            ret = famIns;
        }
      }
      // 同じ距離が2つずつ
      else if (dicHeightCount.Keys.Count == 2)
      {
        bool zeroDis = false;

        double minDis = 0;
        double maxDis = 0;

        foreach (double key in dicHeightCount.Keys)
        {
          if (minDis == 0 && maxDis == 0)
          {
            minDis = key;
            maxDis = key;
          }
          else
          {
            if (minDis > key)
            {
              minDis = key;
            }
            if (maxDis < key)
            {
              maxDis = key;
            }
          }

          if (key < _CmpGeometry.Approx0Len)
          {
            zeroDis = true;
          }
        }

        double d1 = heights[0];
        double d2 = heights[1];
        double d3 = heights[2];
        double d4 = heights[3];

        // どちらかの距離がゼロ
        // 側面三角形
        if (zeroDis)
        {
          if (!_CmpGeometry.PlaneFaceRel(plnFace, heightFace))
            return ret; //ふかすと埋まってしまう

          Revit.DB.FamilySymbol famSym = _CmpElements.FamSymRectTriang;
          if (famSym != null)
          {
            Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
            Revit.DB.UV uv = (bbUV.Max + bbUV.Min) / 2;
            //Revit.DB.XYZ loc = plnFace.Evaluate(uv);
            Revit.DB.XYZ loc = curves[indx1].GetEndPoint(0);    //基準点

            Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
            Revit.DB.XYZ refDir = norm.CrossProduct(norm);

            // 外積による面の向き
            Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
            Revit.DB.XYZ direction = plnFace.XVector;
            if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
            {
              direction = plnFace.YVector;
            }

            ///////////////////////
            //図形基準ベクトル
            //Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(curves[0].GetEndPoint(0), curves[0].GetEndPoint(1));
            Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(curves[indx1].GetEndPoint(0), curves[indx1].GetEndPoint(1));

            //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
            double dot = _CmpGeometry.Naiseki(dirLine, direction);
            if (dot > 1.0) dot = 1.0;
            if (dot < -1.0) dot = -1.0;
            double rad = System.Math.Acos(dot);
            double crs = (direction.X * dirLine.Y - direction.Y * dirLine.X);

            if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;

            double c = System.Math.Cos(rad);
            double s = System.Math.Sin(rad);
            direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);
            if (System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
            {
                direction = dirLine;
            }
            ///////////////////////

            ///////////////////////
            Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
            Revit.DB.FamilyInstance famIns = null;
            double valX = 0;
            double valY = 0;
            if (interRet == null)   //基準点オフセット
            {
                Revit.DB.XYZ org = pcpos;
                Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
            }
            else
            {
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
            }
            Revit.DB.Parameter param;
            param = famIns.LookupParameter("座標X");
            if (param != null)
            {
                param.Set(valX);
            }

            param = famIns.LookupParameter("座標Y");
            if (param != null)
            {
                param.Set(valY);
            }
            ///////////////////////

            Revit.DB.Parameter parThickness = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
            parThickness.Set(upper);

            Revit.DB.Parameter parWidth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
            Revit.DB.Parameter parBreadth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH"));

            parWidth.Set(dWidth);
            parBreadth.Set(dDepth);

            Revit.DB.Line axis = Revit.DB.Line.CreateBound(loc, loc + normal);

            ret = famIns;
          }
        }
        // 側面台形
        else
        {
          if (!_CmpGeometry.PlaneFaceRel(plnFace, heightFace))
            return ret; //ふかすと埋まってしまう

          Revit.DB.FamilySymbol famSym = _CmpElements.FamSymRectTorapezoid;
          if (famSym != null)
          {
            Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
            Revit.DB.UV uv = (bbUV.Max + bbUV.Min) / 2;
            //Revit.DB.XYZ loc = plnFace.Evaluate(uv);
            Revit.DB.XYZ loc = curves[indx1].GetEndPoint(0);    //基準点

            Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
            Revit.DB.XYZ refDir = norm.CrossProduct(norm);

            // 外積による面の向き
            Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
            Revit.DB.XYZ direction = plnFace.XVector;
            if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
            {
              direction = plnFace.YVector;
            }

            ///////////////////////
            //図形基準ベクトル
            Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(curves[indx1].GetEndPoint(0), curves[indx1].GetEndPoint(1));

            //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
            double dot = _CmpGeometry.Naiseki(dirLine, direction);
            if (dot > 1.0) dot = 1.0;
            if (dot < -1.0) dot = -1.0;
            double rad = System.Math.Acos(dot);
            double crs = (direction.X * dirLine.Y - direction.Y * dirLine.X);

            if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;

            double c = System.Math.Cos(rad);
            double s = System.Math.Sin(rad);
            direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);
            if (System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
            {
                direction = dirLine;
            }
            ///////////////////////

            ///////////////////////
            Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
            Revit.DB.FamilyInstance famIns = null;
            double valX = 0;
            double valY = 0;
            if (interRet == null)   //基準点オフセット
            {
                Revit.DB.XYZ org = pcpos;
                Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
            }
            else
            {
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
            }
            Revit.DB.Parameter param;
            param = famIns.LookupParameter("座標X");
            if (param != null)
            {
                param.Set(valX);
            }

            param = famIns.LookupParameter("座標Y");
            if (param != null)
            {
                param.Set(valY);
            }
            ///////////////////////

            Revit.DB.Parameter parThickness1 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS1"));
            parThickness1.Set(lower);

            Revit.DB.Parameter parThickness2 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS2"));
            parThickness2.Set(upper);

            Revit.DB.Parameter parWidth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
            Revit.DB.Parameter parBreadth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH"));

            parWidth.Set(dWidth);
            parBreadth.Set(dDepth);

            Revit.DB.Line axis = Revit.DB.Line.CreateBound(loc, loc + normal);

            ret = famIns;
          }
        }
      }
      else
      {
        // 条件外
        return ret;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>面フカシ作成 - 四角形</summary>
    /// 
    /// <param name="plnFace"   >基準面</param>
    /// <param name="heightFace">高さ用面</param>
    /// <param name="divideArea">分割された範囲</param>
    /// 
    /// <history>2016/12/08 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Rectangle(Revit.DB.PlanarFace plnFace,
                                                    Revit.DB.PlanarFace plnFaceBase,
                                                    Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                                    Revit.DB.PlanarFace heightFace,
                                                    Collections.Generic.IList<Revit.DB.Curve> divideArea,
                                                    Revit.DB.Edge heightedge)
    {
      Revit.DB.FamilyInstance ret = null;

      // 高さ用面
      Revit.DB.XYZ p0 = heightFace.Origin;
      Revit.DB.XYZ p1 = p0 + heightFace.XVector;
      Revit.DB.XYZ p2 = p0 + heightFace.YVector;

      Revit.DB.XYZ normal = plnFace.FaceNormal;
      
      // 平均点
//      Revit.DB.XYZ loc = _CmpGeometry.AveragePos(divideArea);


      divideArea = _CmpGeometry.GetCurves(divideArea); ;

      Revit.DB.XYZ loc = divideArea[0].GetEndPoint(0);    //基準点


      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> posBtmTops = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      foreach (Revit.DB.Curve curve in divideArea)
      {
        Revit.DB.XYZ p = curve.GetEndPoint(0);
        Revit.DB.Line line = Revit.DB.Line.CreateBound(p, p + normal);

        // 面への投影
        Revit.DB.XYZ crossPos = _CmpGeometry.GetCrossPoint(line, p0, p1, p2, 1);

        if (crossPos == null)
        {
          break;
        }

        Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
        posAry.Add(p);
        posAry.Add(crossPos);

        posBtmTops.Add(posAry);
      }

      // 投影交点なしなど
      if (posBtmTops.Count != divideArea.Count)
      {
        return ret;
      }

      // 各頂点からの投影距離
      Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
      foreach (Collections.Generic.IList<Revit.DB.XYZ> btmTop in posBtmTops)
      {
        heights.Add(_CmpGeometry.Distance(btmTop[0], btmTop[1]));
      }

        //高さチェック 0はNG
        double dTotalLen = 0.0;
        foreach (double len in heights)
        {
            dTotalLen += len;
        }
        if (dTotalLen < _CmpGeometry.Approx0Len)
        {
            return null;
        }

        //側面台形・三角形のための基準点インデックス
        int indx1 = 0;
        int indx2 = 0;
        double upper = 0;
        double lower = 0;
        double maxH = -double.MaxValue;
        foreach (double h in heights)
        {
            if (maxH < h)
            {
                maxH = h;
            }
        }
        for (int ii = 0; ii < heights.Count; ii++)
        {
            int jj = (ii + 1) % heights.Count;
            double up = heights[ii];
            double lw = heights[jj];

            if (up - lw > 0.01)
            {
                indx1 = jj;
                indx2 = ii;
                upper = up;
                lower = lw;
                break;
            }
        }
        double dWidth = divideArea[indx1].Length;
        double dDepth = divideArea[indx2].Length;



      // 距離別数
      Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();
      foreach (double h in heights)
      {
        bool contain = false;

        foreach (double key in dicHeightCount.Keys)
        {
          if (System.Math.Abs(h - key) < _CmpGeometry.Approx0Len)
          {
            dicHeightCount[key] += 1;

            contain = true;
            break;
          }
        }

        if (contain == false)
        {
          dicHeightCount.Add(h, 1);
        }
      }

      // すべて同じ距離
      if (dicHeightCount.Keys.Count == 1)
      {
        if (!_CmpGeometry.PlaneFaceRel(plnFace, heightFace))
            return ret; //ふかすと埋まってしまう

        Revit.DB.FamilySymbol famSym = _CmpElements.FamSymRectRect;
        if (famSym != null)
        {
          Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
          Revit.DB.UV uv = (bbUV.Max + bbUV.Min) / 2;
          Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
          Revit.DB.XYZ refDir = norm.CrossProduct(norm);

          // 外積による面の向き
          Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
          Revit.DB.XYZ direction = plnFace.XVector;
          // 逆向き
          if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
          {
            direction = plnFace.YVector;
          }


            ///////////////////////
            //図形基準ベクトル
            Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(divideArea[0].GetEndPoint(0), divideArea[0].GetEndPoint(1));

            //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
            double dot = _CmpGeometry.Naiseki(dirLine, direction);
            if (dot > 1.0) dot = 1.0;
            if (dot < -1.0) dot = -1.0;
            double rad = System.Math.Acos(dot);
            double crs = (direction.X * dirLine.Y - direction.Y * dirLine.X);

            if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;

            double c = System.Math.Cos(rad);
            double s = System.Math.Sin(rad);
            direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);
            if (System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
            {
                direction = dirLine;
            }

            ///////////////////////
            Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
            Revit.DB.FamilyInstance famIns = null;
            double valX = 0;
            double valY = 0;
            if (interRet == null)   //基準点オフセット
            {
                Revit.DB.XYZ org = pcpos;
                Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
            }
            else
            {
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
            }
            Revit.DB.Parameter param;
            param = famIns.LookupParameter("座標X");
            if (param != null)
            {
                param.Set(valX);
            }

            param = famIns.LookupParameter("座標Y");
            if (param != null)
            {
                param.Set(valY);
            }
            ///////////////////////

          Revit.DB.Parameter parThickness = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
          parThickness.Set(heights[0]);

          Revit.DB.Parameter parWidth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
          parWidth.Set(divideArea[0].Length);

          Revit.DB.Parameter parBreadth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH"));
          parBreadth.Set(divideArea[1].Length);

          ret = famIns;
        }
      }
      // 同じ距離が2つずつ
      else if (dicHeightCount.Keys.Count == 2)
      {
        bool zeroDis = false;

        double minDis = 0;
        double maxDis = 0;

        foreach (double key in dicHeightCount.Keys)
        {
          if (minDis == 0 && maxDis == 0)
          {
            minDis = key;
            maxDis = key;
          }
          else
          {
            if (minDis > key)
            {
              minDis = key;
            }
            if (maxDis < key)
            {
              maxDis = key;
            }
          }

          if (key < _CmpGeometry.Approx0Len)
          {
            zeroDis = true;
          }
        }

        double d1 = heights[0];
        double d2 = heights[1];
        double d3 = heights[2];
        double d4 = heights[3];

        // どちらかの距離がゼロ
        // 側面三角形
        if (zeroDis)
        {
          if (!_CmpGeometry.PlaneFaceRel(plnFace, heightFace))
            return ret; //ふかすと埋まってしまう

          Revit.DB.FamilySymbol famSym = _CmpElements.FamSymRectTriang;
          if (famSym != null)
          {
            Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
            Revit.DB.UV uv = (bbUV.Max + bbUV.Min) / 2;
            Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
            Revit.DB.XYZ refDir = norm.CrossProduct(norm);

            loc = divideArea[indx1].GetEndPoint(0);    //基準点

            // 外積による面の向き
            Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
            Revit.DB.XYZ direction = plnFace.XVector;
            if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
            {
              direction = plnFace.YVector;
            }

            ///////////////////////
            //図形基準ベクトル
            Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(divideArea[indx1].GetEndPoint(0), divideArea[indx1].GetEndPoint(1));

            //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
            double dot = _CmpGeometry.Naiseki(dirLine, direction);
            if (dot > 1.0) dot = 1.0;
            if (dot < -1.0) dot = -1.0;
            double rad = System.Math.Acos(dot);
            double crs = (direction.X * dirLine.Y - direction.Y * dirLine.X);

            if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;

            double c = System.Math.Cos(rad);
            double s = System.Math.Sin(rad);
            direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);
            if (System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
            {
                direction = dirLine;
            }
            ///////////////////////

            ///////////////////////
            Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
            Revit.DB.FamilyInstance famIns = null;
            double valX = 0;
            double valY = 0;
            if (interRet == null)   //基準点オフセット
            {
                Revit.DB.XYZ org = pcpos;
                Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
            }
            else
            {
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
            }
            Revit.DB.Parameter param;
            param = famIns.LookupParameter("座標X");
            if (param != null)
            {
                param.Set(valX);
            }

            param = famIns.LookupParameter("座標Y");
            if (param != null)
            {
                param.Set(valY);
            }
            ///////////////////////

            Revit.DB.Parameter parThickness = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
            parThickness.Set(upper);

            Revit.DB.Parameter parWidth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
            Revit.DB.Parameter parBreadth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH"));

            parWidth.Set(dWidth);
            parBreadth.Set(dDepth);

            Revit.DB.Line axis = Revit.DB.Line.CreateBound(loc, loc + normal);

            ret = famIns;
          }
        }
        // 側面台形
        else
        {
          Revit.DB.FamilySymbol famSym = _CmpElements.FamSymRectTorapezoid;
          if (famSym != null)
          {
            Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
            Revit.DB.UV uv = (bbUV.Max + bbUV.Min) / 2;
            Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
            Revit.DB.XYZ refDir = norm.CrossProduct(norm);

            loc = divideArea[indx1].GetEndPoint(0);    //基準点

            // 外積による面の向き
            Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
            Revit.DB.XYZ direction = plnFace.XVector;
            if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
            {
              direction = plnFace.YVector;
            }


            ///////////////////////
            //図形基準ベクトル
            Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(divideArea[indx1].GetEndPoint(0), divideArea[indx1].GetEndPoint(1));

            //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
            double dot = _CmpGeometry.Naiseki(dirLine, direction);
            if (dot > 1.0) dot = 1.0;
            if (dot < -1.0) dot = -1.0;
            double rad = System.Math.Acos(dot);
            double crs = (direction.X * dirLine.Y - direction.Y * dirLine.X);

            if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;

            double c = System.Math.Cos(rad);
            double s = System.Math.Sin(rad);
            direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);
            if (System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
            {
                direction = dirLine;
            }
            ///////////////////////

            ///////////////////////
            Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
            Revit.DB.FamilyInstance famIns = null;
            double valX = 0;
            double valY = 0;
            if (interRet == null)   //基準点オフセット
            {
                Revit.DB.XYZ org = pcpos;
                Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
            }
            else
            {
                famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
            }
            Revit.DB.Parameter param;
            param = famIns.LookupParameter("座標X");
            if (param != null)
            {
                param.Set(valX);
            }

            param = famIns.LookupParameter("座標Y");
            if (param != null)
            {
                param.Set(valY);
            }
            ///////////////////////

            Revit.DB.Parameter parThickness1 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS1"));
            parThickness1.Set(lower);

            Revit.DB.Parameter parThickness2 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS2"));
            parThickness2.Set(upper);

            Revit.DB.Parameter parWidth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
            Revit.DB.Parameter parBreadth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH"));

            parWidth.Set(dWidth);
            parBreadth.Set(dDepth);

            Revit.DB.Line axis = Revit.DB.Line.CreateBound(loc, loc + normal);

            ret = famIns;
          }
        }
      }
      else
      {
        // 条件外
        return ret;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>面フカシ作成 - 四角形</summary>
    /// 
    /// <param name="plnFace"     >基準面</param>
    /// <param name="heightFace"  >高さ用面</param>
    /// <param name="heightEdges" >高さ用エッジ</param>
    /// <param name="edgesFaces"  >エッジを含む面</param>
    /// 
    /// <history><p>2016/12/07 Created  CST,Co.Ltd. Ryo Kuroda
    ///           <p>2016/12/13 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    void CreateFukashi_Rectangle(Revit.DB.PlanarFace plnFace,
                                 Revit.DB.PlanarFace plnFaceBase,
                                 Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                 Revit.DB.PlanarFace heightFace,
                                 Collections.Generic.IList<Revit.DB.Edge> heightEdges,
                                 Collections.Generic.IList<Collections.Generic.IList<Revit.DB.PlanarFace>> edgesFaces,
                                 Revit.DB.Transaction trans)
    {
      // 面指定
      if (heightFace != null)
      {
        Revit.DB.FamilyInstance famIns = CreateFukashi_Rectangle(plnFace, plnFaceBase, curvesA, heightFace, null);
        if(famIns == null)
        {
            System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANTFACE"),
                                                 _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
            return;                                            
        }

        Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
        if (parMaterial != null)
        {
            Revit.DB.ElementId materialId = _CmpElements.MaterialId;
            parMaterial.Set(materialId);
        }
        else
        {
            parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
            if (parMaterial != null)
            {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
            }
        }
      }
      // エッジ指定
      else if (heightEdges.Count > 0)
      {
        #region
      
        // エッジが1つ
        if (heightEdges.Count == 1)
        {
          Revit.DB.Edge edge = heightEdges[0];

          //// エッジと平面の関連性
          //bool relevant = _CmpGeometry.RelevantEdgeOnCurves(edge, curvesA, plnFace);
          //if (relevant == false)
          //{
          //  System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
          //                                       _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
          //  return;
          //}

          Collections.Generic.IList<Revit.DB.PlanarFace> edgeFaces = edgesFaces[0];

//          Revit.DB.PlanarFace similarPlnFace = _CmpGeometry.GetSimilarPlnFace(plnFace, edgeFaces);
          Revit.DB.PlanarFace similarPlnFace = _CmpGeometry.GetSimilarPlnFace(plnFaceBase, edgeFaces);

          if (similarPlnFace != null)
          {
            Revit.DB.FamilyInstance famIns = CreateFukashi_Rectangle(plnFace, plnFaceBase, curvesA, similarPlnFace, edge);
            if (famIns == null)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
                                                        _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                return;
            }

            Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
            if (parMaterial != null)
            {
              Revit.DB.ElementId materialId = _CmpElements.MaterialId;
              parMaterial.Set(materialId);
            }
            else
            {
              parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
                if (parMaterial != null)
                {
                    Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                    parMaterial.Set(materialId);
                }
            }
          }
        }
        // エッジが2つ
        else if(heightEdges.Count == 2)
        {
          // エッジごとに範囲を分割してファミリを配置
          Revit.DB.Edge edge0 = heightEdges[0];
          Revit.DB.Edge edge1 = heightEdges[1];

          // エッジと平面の関連性
          bool relevant0 = _CmpGeometry.RelevantEdgeOnCurves(edge0, curvesA, plnFace);
          bool relevant1 = _CmpGeometry.RelevantEdgeOnCurves(edge1, curvesA, plnFace);

          if (relevant0 == false || relevant1 == false)
          {
            System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
                                                 _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
            return;
          }

          // エッジ同士の関連性
          bool relevantEdges = _CmpGeometry.RelevantEdges(edge0, edge1, plnFace);

          if (relevantEdges == false)
          {
            System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANTEDGES"),
                                                 _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
            return;
          }

          Collections.Generic.IList<Revit.DB.PlanarFace> edgeFaces0 = edgesFaces[0];
//          Revit.DB.PlanarFace similarPlnFace0 = _CmpGeometry.GetSimilarPlnFace(plnFace, edgeFaces0);
          Revit.DB.PlanarFace similarPlnFace0 = _CmpGeometry.GetSimilarPlnFace(plnFaceBase, edgeFaces0);
          Collections.Generic.IList<Revit.DB.PlanarFace> edgeFaces1 = edgesFaces[1];
//          Revit.DB.PlanarFace similarPlnFace1 = _CmpGeometry.GetSimilarPlnFace(plnFace, edgeFaces1);
          Revit.DB.PlanarFace similarPlnFace1 = _CmpGeometry.GetSimilarPlnFace(plnFaceBase, edgeFaces1);
          
          Revit.DB.Line divide = null;
          bool fulFill = _CmpGeometry.FulFillEdges(edge0, edge1, plnFace, ref divide);

          if (fulFill)
          {
            Collections.Generic.IList<Revit.DB.Curve> curves = _CmpGeometry.GetFaceCurves(plnFace);
            if(curvesA.Count != 0)
            {
                curves = curvesA;
            }

            // 分割
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.Curve>> divideCurves = _CmpGeometry.DividePolCurves(curves, divide);

            Collections.Generic.IList<Revit.DB.Curve> curves0 = divideCurves[0];
            Collections.Generic.IList<Revit.DB.Curve> curves1 = divideCurves[1];

            //分割線が不正
            if(curves0.Count == 0 || curves1.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANTEDGES"),
                            _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                return;
            }

            // エッジ端点を面に投影し、端点と頂点を結んだとき分割線と交差しない
            bool sameSide0 = _CmpGeometry.SameSideCurves(edge0, plnFace, curves0, divide);
            bool sameSide1 = _CmpGeometry.SameSideCurves(edge1, plnFace, curves0, divide);

            if (sameSide0)
            {
              Revit.DB.FamilyInstance famIns1 = CreateFukashi_Rectangle(plnFace, plnFaceBase, curvesA, similarPlnFace0, curves0, null);
              Revit.DB.FamilyInstance famIns2 = CreateFukashi_Rectangle(plnFace, plnFaceBase, curvesA, similarPlnFace1, curves1, null);
              if (famIns1 == null || famIns2 == null)
              {
                  System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
                                                          _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                  return;
              }

              if (famIns1 != null)
              {
                Revit.DB.Parameter parMaterial = famIns1.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
                if (parMaterial != null)
                {
                  Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                  parMaterial.Set(materialId);
                }
                else
                {
                  parMaterial = famIns1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
                    if (parMaterial != null)
                    {
                        Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                        parMaterial.Set(materialId);
                    }
                }
              }
              if (famIns2 != null)
              {
                Revit.DB.Parameter parMaterial = famIns2.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
                if (parMaterial != null)
                {
                  Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                  parMaterial.Set(materialId);
                }
                else
                {
                  parMaterial = famIns2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
                    if (parMaterial != null)
                    {
                        Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                        parMaterial.Set(materialId);
                    }
                }
              }

              trans.Commit();

//              trans.Start("Join");
//              Revit.DB.JoinGeometryUtils.JoinGeometry(_CmpElements.RvtDBDoc, famIns1, famIns2);
//              trans.Commit();
            }
            else if (sameSide1)
            {
              Revit.DB.FamilyInstance famIns1 = CreateFukashi_Rectangle(plnFace, plnFaceBase, curvesA, similarPlnFace0, curves1, null);
              Revit.DB.FamilyInstance famIns2 = CreateFukashi_Rectangle(plnFace, plnFaceBase, curvesA, similarPlnFace1, curves0, null);

              if (famIns1 == null || famIns2 == null)
              {
                  System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
                                                          _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                  return;
              }
              Revit.DB.Parameter parMaterial = famIns1.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
              if (parMaterial != null)
              {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
              }
              else
              {
                parMaterial = famIns1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
                if (parMaterial != null)
                {
                    Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                    parMaterial.Set(materialId);
                }
              }

              parMaterial = famIns2.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
              if (parMaterial != null)
              {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
              }
              else
              {
                parMaterial = famIns2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
                if (parMaterial != null)
                {
                    Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                    parMaterial.Set(materialId);
                }
              }

              trans.Commit();

//              trans.Start("Join");
//              Revit.DB.JoinGeometryUtils.JoinGeometry(_CmpElements.RvtDBDoc, famIns1, famIns2);
//              trans.Commit();
            }
          }
        }

        #endregion
      }
    }

    /// ================================================================================
    /// <summary>面フカシ作成 - DirectShape</summary>
    /// 
    /// <param name="category"        >カテゴリ</param>
    /// <param name="plnFace"         >面</param>
    /// <param name="thickness"       >厚さ</param>
    /// <param name="appId"           >アプリケーションID</param>
    /// <param name="appDataId"       >アプリケーションデータID</param>
    /// 
    /// <history><p>2016/11/18 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <P>2016/11/22 Modified CST,Co.Ltd. Ryo Kuroda</P></history>
    /// ================================================================================
    public
    Revit.DB.DirectShape CreateFukashi_DirectShape(Revit.DB.Category category,
                                                   Revit.DB.PlanarFace plnFace,
                                                   Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                                   double thickness,
                                                   string appId,
                                                   string appDataId)
    {
      // 法線
      Revit.DB.XYZ normal = plnFace.FaceNormal;

      // ループ線分
      Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = plnFace.GetEdgesAsCurveLoops();

      // マテリアルID
      Revit.DB.ElementId materialId       = _CmpElements.MaterialId;
      // グラフィックススタイルID
      Revit.DB.ElementId graphicsStyleId  = _CmpElements.GraphicsStyleId;

      if (materialId == null)
      {
        materialId = Revit.DB.ElementId.InvalidElementId;
      }
      if (graphicsStyleId == null)
      {
        graphicsStyleId = Revit.DB.ElementId.InvalidElementId;
      }

      // オプション
      Revit.DB.SolidOptions solidOpt = new Autodesk.Revit.DB.SolidOptions(materialId,
                                                                          graphicsStyleId);
      
      // 押し出し作成
      Revit.DB.Solid solid = Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(crvLoops,
                                                                                        normal,
                                                                                        thickness,
                                                                                        solidOpt);

      // 作成
      Revit.DB.DirectShape ds = Revit.DB.DirectShape.CreateElement(_CmpElements.RvtDBDoc, category.Id);

      ds.SetName("フカシ");
      ds.ApplicationId = appId;
      ds.ApplicationDataId = appDataId;

      Collections.Generic.IList<Revit.DB.GeometryObject> goAry = new Collections.Generic.List<Revit.DB.GeometryObject>();
      goAry.Add(solid);
      ds.SetShape(goAry);

      Revit.DB.Parameter parMaterial = ds.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
      if (parMaterial != null)
      {
        parMaterial.Set(materialId);
      }

      return ds;
    }
    /// ================================================================================
    /// <summary>面フカシ作成 - 台形</summary>
    /// 
    /// <param name="plnFace"     >基準面</param>
    /// <param name="heightFace"  >高さ用面</param>
    /// <param name="heightEdges" >エッジ</param>
    /// <param name="edgesFaces"  >エッジを含む面</param>
    /// 
    /// <history>2016/12/12 Created  GSA,Inc. Hideki Sudo</history>
    /// ================================================================================
    public
    void CreateFukashi_Trapezoid(Revit.DB.PlanarFace plnFace,
                                 Revit.DB.PlanarFace plnFaceBase,
                                Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                Revit.DB.PlanarFace heightFace,
                                Collections.Generic.IList<Revit.DB.Edge> heightEdges,
                                Collections.Generic.IList<Collections.Generic.IList<Revit.DB.PlanarFace>> edgesFaces)
    {
        // 面指定
        if (heightFace != null)
        {
            Revit.DB.FamilyInstance famIns = CreateFukashi_Trapezoid(plnFace, plnFaceBase, curvesA, heightFace, null);
            if (famIns == null)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANTFACE"),
                                                        _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                return;
            }

        Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
        if (parMaterial != null)
        {
          Revit.DB.ElementId materialId = _CmpElements.MaterialId;
          parMaterial.Set(materialId);
        }
        else
        {
          parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
            if (parMaterial != null)
            {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
            }
        }

      }
        // エッジ指定
        else if (heightEdges.Count > 0)
        {
            Revit.DB.Edge edge = heightEdges[0];

            //// エッジと平面の関連性
            //bool relevant = _CmpGeometry.RelevantEdgeOnCurves(edge, curvesA, plnFace);
            //if (relevant == false)
            //{
            //    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
            //                                            _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
            //    return;
            //}

            Collections.Generic.IList<Revit.DB.PlanarFace> edgeFaces = edgesFaces[0];

            Revit.DB.PlanarFace similarPlnFace = _CmpGeometry.GetSimilarPlnFace(plnFace, edgeFaces);

            if (similarPlnFace != null)
            {
                Revit.DB.FamilyInstance famIns = CreateFukashi_Trapezoid(plnFace, plnFaceBase, curvesA, similarPlnFace, edge);
                if (famIns == null)
                {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
                                                            _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
                    return;
                }

          Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
          if (parMaterial != null)
          {
            Revit.DB.ElementId materialId = _CmpElements.MaterialId;
            parMaterial.Set(materialId);
          }
          else
          {
            parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
            if (parMaterial != null)
            {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
            }
          }
        }
        }
    }
    /// ================================================================================
    /// <summary>面フカシ作成 - 台形</summary>
    /// 
    /// <param name="plnFace"     >基準面</param>
    /// <param name="heightFace"  >高さ用面</param>
    /// 
    /// <history>2016/12/12 Created  GSA,Inc. Hideki Sudo</history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Trapezoid(Revit.DB.PlanarFace plnFace,
                                                    Revit.DB.PlanarFace plnFaceBase,
                                                    Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                                    Revit.DB.PlanarFace heightFace,
                                                    Revit.DB.Edge heightedge)
    {
        Revit.DB.FamilyInstance ret = null;

        // 高さ用面
        Revit.DB.XYZ p0 = heightFace.Origin;
        Revit.DB.XYZ p1 = p0 + heightFace.XVector;
        Revit.DB.XYZ p2 = p0 + heightFace.YVector;

        Revit.DB.XYZ normal = plnFace.FaceNormal;
        Collections.Generic.IList<Revit.DB.Curve> curves = _CmpGeometry.GetFaceCurvesBase(plnFace);

        //Faceのカーブを置き換え
        if(curvesA.Count != 0)
        {
            curves = _CmpGeometry.GetCurvesBase(curvesA);
        }

        Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> posBtmTops = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
        Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
        Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();

        //面から生成
        if (heightedge == null)
        {

            foreach (Revit.DB.Curve curve in curves)
            {
                Revit.DB.XYZ p = curve.GetEndPoint(0);
                Revit.DB.Line line = Revit.DB.Line.CreateBound(p, p + normal);

                // 面への投影
                Revit.DB.XYZ crossPos = _CmpGeometry.GetCrossPoint(line, p0, p1, p2, 1);

                if (crossPos == null)
                {
                    break;
                }

                Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
                posAry.Add(p);
                posAry.Add(crossPos);

                posBtmTops.Add(posAry);
            }

            // 投影交点なしなど
            if (posBtmTops.Count != curves.Count)
            {
                return ret;
            }

            // 各頂点からの投影距離
//                Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
            foreach (Collections.Generic.IList<Revit.DB.XYZ> btmTop in posBtmTops)
            {
                heights.Add(_CmpGeometry.Distance(btmTop[0], btmTop[1]));
            }

            //高さチェック 0はNG
            double dTotalLen = 0.0;
            foreach (double len in heights)
            {
                dTotalLen += len;
            }
            if (dTotalLen < _CmpGeometry.Approx0Len)
            {
                return ret;
            }


            // 距離別数
//                Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();
            foreach (double h in heights)
            {
                bool contain = false;

                foreach (double key in dicHeightCount.Keys)
                {
                    if (System.Math.Abs(h - key) < _CmpGeometry.Approx0Len)
                    {
                        dicHeightCount[key] += 1;

                        contain = true;
                        break;
                    }
                }

                if (contain == false)
                {
                    dicHeightCount.Add(h, 1);
                }
            }
        }
        else//エッジから生成
        {
            Revit.DB.XYZ pb0 = plnFaceBase.Origin;
            Revit.DB.XYZ pb1 = pb0 + plnFaceBase.XVector;
            Revit.DB.XYZ pb2 = pb0 + plnFaceBase.YVector;

            Revit.DB.XYZ pa1 = heightedge.AsCurve().GetEndPoint(0) - pb0;
            double h1 = _CmpGeometry.Naiseki(normal, pa1);
            if (h1 < 0)
            {
                //埋まる
                return ret;
            }

            Revit.DB.XYZ pa2 = heightedge.AsCurve().GetEndPoint(1) - pb0;
            double h2 = _CmpGeometry.Naiseki(normal, pa2);
            if (System.Math.Abs(h1 - h2) > _CmpGeometry.Approx0Len)
            {
                //傾いている
                return ret;
            }

            heights.Add(h1);
            dicHeightCount.Add(h1, 1);
        }

        // すべて同じ距離
        if (dicHeightCount.Keys.Count == 1)
        {
            //面から生成
            if (heightedge == null)
            {
                if (!_CmpGeometry.PlaneFaceRel(plnFace, heightFace))
                    return ret; //ふかすと埋まってしまう
            }

            Revit.DB.FamilySymbol famSym = _CmpElements.FamSymTorapezoid;
            if (famSym != null)
            {
                Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
                Revit.DB.UV uv = bbUV.Min;

                Revit.DB.XYZ loc = curves[0].GetEndPoint(0);    //基準点

                Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
                Revit.DB.XYZ refDir = norm.CrossProduct(norm);

                // 外積による面の向き
                Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
                Revit.DB.XYZ direction = plnFace.XVector;
                // 逆向き
                if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
                {
                    direction = plnFace.YVector;
                }


                ///////////////////////
                //図形基準ベクトル
                Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(curves[0].GetEndPoint(0), curves[0].GetEndPoint(1));

                //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
                double dot = _CmpGeometry.Naiseki(dirLine, direction);
                if (dot > 1.0)  dot = 1.0;
                if (dot < -1.0) dot = -1.0;
                double rad = System.Math.Acos(dot);
                double crs = (direction.X * dirLine.Y - direction.Y * dirLine.X);

                if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;


                double c = System.Math.Cos(rad);
                double s = System.Math.Sin(rad);
                direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);
                if(System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
                {
                    direction = dirLine;
                }
                ///////////////////////

                ///////////////////////
                Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
                Revit.DB.FamilyInstance famIns = null;
                double valX = 0;
                double valY = 0;
                if (interRet == null)   //基準点オフセット
                {
                    Revit.DB.XYZ org = pcpos;
                    Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                    valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                    Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                    valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                    famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
                }
                else
                {
                    famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
                }
                Revit.DB.Parameter param;
                param = famIns.LookupParameter("座標X");
                if (param != null)
                {
                    param.Set(valX);
                }

                param = famIns.LookupParameter("座標Y");
                if (param != null)
                {
                    param.Set(valY);
                }
                ///////////////////////

                Revit.DB.Parameter parThickness = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
                parThickness.Set(heights[0]);

                Revit.DB.Parameter parWidth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
                parWidth.Set(curves[0].Length);

                Revit.DB.XYZ ep1 = curves[0].GetEndPoint(0);
                Revit.DB.XYZ ep2 = curves[1].GetEndPoint(0);
                Revit.DB.XYZ ep3 = curves[2].GetEndPoint(0);
                Revit.DB.XYZ ep4 = curves[3].GetEndPoint(0);

                Revit.DB.XYZ edgeUv = Geometry.UnitVector(ep2 - ep1);

                double dDepth2 = edgeUv.X * (ep4.Y - ep1.Y) - edgeUv.Y * (ep4.X - ep1.X);
                dDepth2 = System.Math.Abs(dDepth2);

                Revit.DB.XYZ va = new Revit.DB.XYZ(ep4.X - ep1.X, ep4.Y - ep1.Y, ep4.Z - ep1.Z);
                Revit.DB.XYZ vn = Geometry.UnitVector(ep2 - ep1);
                Revit.DB.XYZ cr2 = _CmpGeometry.Gaiseki(vn, va);
                double dDepth = Math.Sqrt(cr2.X * cr2.X + cr2.Y * cr2.Y + cr2.Z * cr2.Z);

                Revit.DB.Parameter parDepth = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH"));
                parDepth.Set(dDepth);

                double rad1 = _CmpGeometry.Angle3DA(ep1, ep2, ep4);
                double rad2 = _CmpGeometry.Angle3DA(ep2, ep3, ep1);
                rad1 = System.Math.Abs(rad1);
                rad2 = System.Math.Abs(rad2);

                Revit.DB.Parameter parAngle1 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_ANGLE1"));
                parAngle1.Set(rad1);

                Revit.DB.Parameter parAngle2 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_ANGLE2"));
                parAngle2.Set(rad2);

                ret = famIns;
            }
        }
        return ret;
    }


    /// ================================================================================
    /// <summary>面フカシ作成 - L型</summary>
    /// 
    /// <param name="plnFace"     >基準面</param>
    /// <param name="heightFace"  >高さ用面</param>
    /// <param name="heightEdges" >エッジ</param>
    /// <param name="edgesFaces"  >エッジを含む面</param>
    /// 
    /// <history>2016/12/26 Created  GSA,Inc. Hideki Sudo</history>
    /// ================================================================================
    public
    void CreateFukashi_LType(Revit.DB.PlanarFace plnFace,
                             Revit.DB.PlanarFace plnFaceBase,
                             Collections.Generic.IList<Revit.DB.Curve> curvesA,
                             Revit.DB.PlanarFace heightFace,
                             Collections.Generic.IList<Revit.DB.Edge> heightEdges,
                             Collections.Generic.IList<Collections.Generic.IList<Revit.DB.PlanarFace>> edgesFaces)
    {
      // 面指定
      if (heightFace != null)
      {
        Revit.DB.FamilyInstance famIns = CreateFukashi_LType(plnFace, plnFaceBase, curvesA, heightFace, null);
        if (famIns == null)
        {
          System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANTFACE"),
                                                  _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
          return;
        }

        Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
        if (parMaterial != null)
        {
          Revit.DB.ElementId materialId = _CmpElements.MaterialId;
          parMaterial.Set(materialId);
        }
        else
        {
          parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
            if (parMaterial != null)
            {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
            }
        }
      }
      // エッジ指定
      else if (heightEdges.Count > 0)
      {
        Revit.DB.Edge edge = heightEdges[0];

        //// エッジと平面の関連性
        //bool relevant = _CmpGeometry.RelevantEdgeOnCurves(edge, curvesA, plnFace);
        //if (relevant == false)
        //{
        //  System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
        //                                          _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
        //  return;
        //}

        Collections.Generic.IList<Revit.DB.PlanarFace> edgeFaces = edgesFaces[0];

        Revit.DB.PlanarFace similarPlnFace = _CmpGeometry.GetSimilarPlnFace(plnFace, edgeFaces);

        if (similarPlnFace != null)
        {
          Revit.DB.FamilyInstance famIns = CreateFukashi_LType(plnFace, plnFaceBase, curvesA, similarPlnFace, edge);
          if (famIns == null)
          {
            System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
                                                    _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
            return;
          }

          Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
          if (parMaterial != null)
          {
            Revit.DB.ElementId materialId = _CmpElements.MaterialId;
            parMaterial.Set(materialId);
          }
          else
          {
            parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
            if (parMaterial != null)
            {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
            }
          }
        }
      }
    }
        /// ================================================================================
        /// <summary>面フカシ作成 - L型</summary>
        /// 
        /// <param name="plnFace"     >基準面</param>
        /// <param name="heightFace"  >高さ用面</param>
        /// 
        /// <history>2016/12/07 Created  GSA,Inc. Hideki Sudo</history>
        /// ================================================================================
        public
        Revit.DB.FamilyInstance CreateFukashi_LType(Revit.DB.PlanarFace plnFace,
                                                    Revit.DB.PlanarFace plnFaceBase,
                                                    Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                                    Revit.DB.PlanarFace heightFace,
                                                    Revit.DB.Edge heightedge)
        {
            Revit.DB.FamilyInstance ret = null;

            // 高さ用面
            Revit.DB.XYZ p0 = heightFace.Origin;
            Revit.DB.XYZ p1 = p0 + heightFace.XVector;
            Revit.DB.XYZ p2 = p0 + heightFace.YVector;

            Revit.DB.XYZ normal = plnFace.FaceNormal;
            Collections.Generic.IList<Revit.DB.Curve> curves = _CmpGeometry.GetFaceCurvesLType(plnFace);

            //Faceのカーブを置き換え
            if (curvesA.Count != 0)
            {
                curves = _CmpGeometry.GetCurvesLType(curvesA);
            }

            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> posBtmTops = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
            Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
            Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();

            //面から生成
            if (heightedge == null)
            {
                foreach (Revit.DB.Curve curve in curves)
                {
                    Revit.DB.XYZ p = curve.GetEndPoint(0);
                    Revit.DB.Line line = Revit.DB.Line.CreateBound(p, p + normal);

                    // 面への投影
                    Revit.DB.XYZ crossPos = _CmpGeometry.GetCrossPoint(line, p0, p1, p2, 1);

                    if (crossPos == null)
                    {
                        break;
                    }

                    Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
                    posAry.Add(p);
                    posAry.Add(crossPos);

                    posBtmTops.Add(posAry);
                }

                // 投影交点なしなど
                if (posBtmTops.Count != curves.Count)
                {
                    return ret;
                }

                // 各頂点からの投影距離
                //            Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
                foreach (Collections.Generic.IList<Revit.DB.XYZ> btmTop in posBtmTops)
                {
                    heights.Add(_CmpGeometry.Distance(btmTop[0], btmTop[1]));
                }

                //高さチェック 0はNG
                double dTotalLen = 0.0;
                foreach (double len in heights)
                {
                    dTotalLen += len;
                }
                if (dTotalLen < _CmpGeometry.Approx0Len)
                {
                    return null;
                }

                // 距離別数
                //            Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();
                foreach (double h in heights)
                {
                    bool contain = false;

                    foreach (double key in dicHeightCount.Keys)
                    {
                        if (System.Math.Abs(h - key) < _CmpGeometry.Approx0Len)
                        {
                            dicHeightCount[key] += 1;

                            contain = true;
                            break;
                        }
                    }

                    if (contain == false)
                    {
                        dicHeightCount.Add(h, 1);
                    }
                }
            }
            else//エッジから生成
            {
                Revit.DB.XYZ pb0 = plnFaceBase.Origin;
                Revit.DB.XYZ pb1 = pb0 + plnFaceBase.XVector;
                Revit.DB.XYZ pb2 = pb0 + plnFaceBase.YVector;

                Revit.DB.XYZ pa1 = heightedge.AsCurve().GetEndPoint(0) - pb0;
                double h1 = _CmpGeometry.Naiseki(normal, pa1);
                if (h1 < 0)
                {
                    //埋まる
                    return ret;
                }

                Revit.DB.XYZ pa2 = heightedge.AsCurve().GetEndPoint(1) - pb0;
                double h2 = _CmpGeometry.Naiseki(normal, pa2);
                if (System.Math.Abs(h1 - h2) > _CmpGeometry.Approx0Len)
                {
                    //傾いている
                    return ret;
                }

                heights.Add(h1);
                dicHeightCount.Add(h1, 1);
            }


            // すべて同じ距離
            if (dicHeightCount.Keys.Count == 1)
            {
                //面から生成
                if (heightedge == null)
                {
                    if (!_CmpGeometry.PlaneFaceRel(plnFace, heightFace))
                        return ret; //ふかすと埋まってしまう
                }

                Revit.DB.FamilySymbol famSym = _CmpElements.FamSymLshape;
                if (famSym != null)
                {
                    Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();

                    Revit.DB.UV uv = bbUV.Min;
                    Revit.DB.XYZ loc = curves[0].GetEndPoint(0);    //基準点

                    Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
                    Revit.DB.XYZ refDir = norm.CrossProduct(norm);

                    // 外積による面の向き
                    Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
                    Revit.DB.XYZ direction = plnFace.XVector;
                    // 逆向き
                    if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
                    {
//                        direction = plnFace.YVector;
                    }

                    ///////////////////////
                    //図形基準ベクトル
                    Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(curves[0].GetEndPoint(0), curves[0].GetEndPoint(1));

                    //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
                    double dot = _CmpGeometry.Naiseki(dirLine, direction);
                    if (dot > 1.0) dot = 1.0;
                    if (dot < -1.0) dot = -1.0;
                    double rad = System.Math.Acos(dot);
                    double crs =  (direction.X * dirLine.Y - direction.Y * dirLine.X);

                    if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;


                    double c = System.Math.Cos(rad);
                    double s = System.Math.Sin(rad);
                    direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);
                    if(System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
                    {
                        direction = dirLine;
                    }
                    ///////////////////////

                    ///////////////////////
                    Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
                    Revit.DB.FamilyInstance famIns = null;
                    double valX = 0;
                    double valY = 0;
                    if (interRet == null)   //基準点オフセット
                    {
                        Revit.DB.XYZ org = pcpos;
                        Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                        valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                        Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                        valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                        famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
                    }
                    else
                    {
                        famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
                    }
                    Revit.DB.Parameter param;
                    param = famIns.LookupParameter("座標X");
                    if (param != null)
                    {
                        param.Set(valX);
                    }

                    param = famIns.LookupParameter("座標Y");
                    if (param != null)
                    {
                        param.Set(valY);
                    }
                    ///////////////////////

                    Revit.DB.Parameter parThickness = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
                    parThickness.Set(heights[0]);

                    Revit.DB.Parameter parWidth1 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH1"));
                    parWidth1.Set(curves[0].Length);

                    Revit.DB.Parameter parBreadth2 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH2"));
                    parBreadth2.Set(curves[1].Length);

                    Revit.DB.Parameter parWidth2 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH2"));
                    parWidth2.Set(curves[4].Length);

                    Revit.DB.Parameter parBreadth1 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH1"));
                    parBreadth1.Set(curves[5].Length);

                    ret = famIns;
                }
            }
            return ret;
        }

    /// ================================================================================
    /// <summary>面フカシ作成 - T型</summary>
    /// 
    /// <param name="plnFace"     >基準面</param>
    /// <param name="heightFace"  >高さ用面</param>
    /// <param name="heightEdges" >エッジ</param>
    /// <param name="edgesFaces"  >エッジを含む面</param>
    /// 
    /// <history>2017/01/06 Created  GSA,Inc. Hideki Sudo</history>
    /// ================================================================================
    public
    void CreateFukashi_TType(Revit.DB.PlanarFace plnFace,
                             Revit.DB.PlanarFace plnFaceBase,
                             Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                Revit.DB.PlanarFace heightFace,
                                Collections.Generic.IList<Revit.DB.Edge> heightEdges,
                                Collections.Generic.IList<Collections.Generic.IList<Revit.DB.PlanarFace>> edgesFaces)
    {
      // 面指定
      if (heightFace != null)
      {
        Revit.DB.FamilyInstance famIns = CreateFukashi_TType(plnFace, plnFaceBase, curvesA, heightFace, null);
        if (famIns == null)
        {
          System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANTFACE"),
                                                  _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
          return;
        }

        Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
        if (parMaterial != null)
        {
          Revit.DB.ElementId materialId = _CmpElements.MaterialId;
          parMaterial.Set(materialId);
        }
        else
        {
          parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
            if (parMaterial != null)
            {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
            }
        }
      }
      // エッジ指定
      else if (heightEdges.Count > 0)
      {
        Revit.DB.Edge edge = heightEdges[0];

        //// エッジと平面の関連性
        //bool relevant = _CmpGeometry.RelevantEdgeOnCurves(edge, curvesA, plnFace);
        //if (relevant == false)
        //{
        //  System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
        //                                          _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
        //  return;
        //}

        Collections.Generic.IList<Revit.DB.PlanarFace> edgeFaces = edgesFaces[0];

        Revit.DB.PlanarFace similarPlnFace = _CmpGeometry.GetSimilarPlnFace(plnFace, edgeFaces);

        if (similarPlnFace != null)
        {
          Revit.DB.FamilyInstance famIns = CreateFukashi_TType(plnFace, plnFaceBase, curvesA, similarPlnFace, edge);
          if (famIns == null)
          {
            System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_RELEVANT"),
                                                    _CmpAttribute.ResourceText("IDS_TXT_FUKASHIFACE"));
            return;
          }

          Revit.DB.Parameter parMaterial = famIns.get_Parameter(Revit.DB.BuiltInParameter.STRUCTURAL_MATERIAL_PARAM);
          if (parMaterial != null)
          {
            Revit.DB.ElementId materialId = _CmpElements.MaterialId;
            parMaterial.Set(materialId);
          }
          else
          {
            parMaterial = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
            if (parMaterial != null)
            {
                Revit.DB.ElementId materialId = _CmpElements.MaterialId;
                parMaterial.Set(materialId);
            }
          }
        }
      }
    }
        /// ================================================================================
        /// <summary>面フカシ作成 - T型</summary>
        /// 
        /// <param name="plnFace"     >基準面</param>
        /// <param name="heightFace"  >高さ用面</param>
        /// 
        /// <history>2017/01/06 Created  GSA,Inc. Hideki Sudo</history>
        /// ================================================================================
        public
        Revit.DB.FamilyInstance CreateFukashi_TType(Revit.DB.PlanarFace plnFace,
                                                    Revit.DB.PlanarFace plnFaceBase,
                                                    Collections.Generic.IList<Revit.DB.Curve> curvesA,
                                                    Revit.DB.PlanarFace heightFace,
                                                    Revit.DB.Edge heightedge)
        {
            Revit.DB.FamilyInstance ret = null;

            // 高さ用面
            Revit.DB.XYZ p0 = heightFace.Origin;
            Revit.DB.XYZ p1 = p0 + heightFace.XVector;
            Revit.DB.XYZ p2 = p0 + heightFace.YVector;

            Revit.DB.XYZ normal = plnFace.FaceNormal;
            Collections.Generic.IList<Revit.DB.Curve> curves = _CmpGeometry.GetFaceCurvesTType(plnFace);

            //Faceのカーブを置き換え
            if (curvesA.Count != 0)
            {
                curves = _CmpGeometry.GetCurvesTType(curvesA);
            }

            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> posBtmTops = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
            Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
            Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();

            //面から生成
            if (heightedge == null)
            {
                foreach (Revit.DB.Curve curve in curves)
                {
                    Revit.DB.XYZ p = curve.GetEndPoint(0);
                    Revit.DB.Line line = Revit.DB.Line.CreateBound(p, p + normal);

                    // 面への投影
                    Revit.DB.XYZ crossPos = _CmpGeometry.GetCrossPoint(line, p0, p1, p2, 1);

                    if (crossPos == null)
                    {
                        break;
                    }

                    Collections.Generic.IList<Revit.DB.XYZ> posAry = new Collections.Generic.List<Revit.DB.XYZ>();
                    posAry.Add(p);
                    posAry.Add(crossPos);

                    posBtmTops.Add(posAry);
                }

                // 投影交点なしなど
                if (posBtmTops.Count != curves.Count)
                {
                    return ret;
                }

                // 各頂点からの投影距離
                //            Collections.Generic.IList<double> heights = new Collections.Generic.List<double>();
                foreach (Collections.Generic.IList<Revit.DB.XYZ> btmTop in posBtmTops)
                {
                    heights.Add(_CmpGeometry.Distance(btmTop[0], btmTop[1]));
                }

                //高さチェック 0はNG
                double dTotalLen = 0.0;
                foreach (double len in heights)
                {
                    dTotalLen += len;
                }
                if (dTotalLen < _CmpGeometry.Approx0Len)
                {
                    return null;
                }

                // 距離別数
                //            Collections.Generic.IDictionary<double, int> dicHeightCount = new Collections.Generic.Dictionary<double, int>();
                foreach (double h in heights)
                {
                    bool contain = false;

                    foreach (double key in dicHeightCount.Keys)
                    {
                        if (System.Math.Abs(h - key) < _CmpGeometry.Approx0Len)
                        {
                            dicHeightCount[key] += 1;

                            contain = true;
                            break;
                        }
                    }

                    if (contain == false)
                    {
                        dicHeightCount.Add(h, 1);
                    }
                }
            }
            else//エッジから生成
            {
                Revit.DB.XYZ pb0 = plnFaceBase.Origin;
                Revit.DB.XYZ pb1 = pb0 + plnFaceBase.XVector;
                Revit.DB.XYZ pb2 = pb0 + plnFaceBase.YVector;

                Revit.DB.XYZ pa1 = heightedge.AsCurve().GetEndPoint(0) - pb0;
                double h1 = _CmpGeometry.Naiseki(normal, pa1);
                if (h1 < 0)
                {
                    //埋まる
                    return ret;
                }

                Revit.DB.XYZ pa2 = heightedge.AsCurve().GetEndPoint(1) - pb0;
                double h2 = _CmpGeometry.Naiseki(normal, pa2);
                if (System.Math.Abs(h1 - h2) > _CmpGeometry.Approx0Len)
                {
                    //傾いている
                    return ret;
                }

                heights.Add(h1);
                dicHeightCount.Add(h1, 1);
            }

            // すべて同じ距離
            if (dicHeightCount.Keys.Count == 1)
            {
                if (!_CmpGeometry.PlaneFaceRel(plnFace, heightFace))
                    return ret; //ふかすと埋まってしまう

                Revit.DB.FamilySymbol famSym = _CmpElements.FamSymTshape;
                if (famSym != null)
                {
                    Revit.DB.BoundingBoxUV bbUV = plnFace.GetBoundingBox();
                    //Revit.DB.UV uv = (bbUV.Max + bbUV.Min) / 2;
                    //Revit.DB.XYZ loc = plnFace.Evaluate(uv);

                    Revit.DB.UV uv = bbUV.Min;
                    Revit.DB.XYZ loc = curves[0].GetEndPoint(0);    //基準点

                    Revit.DB.XYZ norm = plnFace.ComputeNormal(uv);
                    Revit.DB.XYZ refDir = norm.CrossProduct(norm);

                    // 外積による面の向き
                    Revit.DB.XYZ cross = _CmpGeometry.Gaiseki(plnFace.XVector, plnFace.YVector);
                    Revit.DB.XYZ direction = plnFace.XVector;
                    // 逆向き
                    if (_CmpGeometry.Distance(norm, cross) > _CmpGeometry.Approx0Len)
                    {
                        direction = plnFace.YVector;
                    }

                    ///////////////////////
                    //図形基準ベクトル
                    Revit.DB.XYZ dirLine = _CmpGeometry.UnitVector(curves[0].GetEndPoint(0), curves[0].GetEndPoint(1));

                    //対象面の基準ベクトル(direction)を図形基準ベクトルに合わせるための角度計算
                    double dot = _CmpGeometry.Naiseki(dirLine, direction);
                    if (dot > 1.0) dot = 1.0;
                    if (dot < -1.0) dot = -1.0;
                    double rad = System.Math.Acos(dot);
                    double crs = (direction.X * dirLine.Y - direction.Y * dirLine.X);

                    if (System.Math.Abs(dot) < Ztol99 && crs < 0.0) rad = PIW - rad;

                    double c = System.Math.Cos(rad);
                    double s = System.Math.Sin(rad);
                    direction = new Revit.DB.XYZ(c * direction.X - s * direction.Y, s * direction.X + c * direction.Y, 0.0);
                    if (System.Math.Abs(plnFace.FaceNormal.Z) < 0.1)
                    {
                        direction = dirLine;
                    }
                    ///////////////////////
                    ///////////////////////
                    Revit.DB.IntersectionResult interRet = plnFaceBase.Project(loc);    //基準点の面上確認
                    Revit.DB.FamilyInstance famIns = null;
                    double valX = 0;
                    double valY = 0;
                    if (interRet == null)   //基準点オフセット
                    {
                        Revit.DB.XYZ org = pcpos;
                        Revit.DB.XYZ distOrg = new Revit.DB.XYZ(loc.X - org.X, loc.Y - org.Y, loc.Z - org.Z);

                        valX = _CmpGeometry.Naiseki(dirLine, distOrg);
                        Revit.DB.XYZ cr = _CmpGeometry.Gaiseki(dirLine, distOrg);
                        valY = -Math.Sqrt(cr.X * cr.X + cr.Y * cr.Y + cr.Z * cr.Z);
                        famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, org, direction, famSym);
                    }
                    else
                    {
                        famIns = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnFaceBase, loc, direction, famSym);
                    }
                    Revit.DB.Parameter param;
                    param = famIns.LookupParameter("座標X");
                    if (param != null)
                    {
                        param.Set(valX);
                    }

                    param = famIns.LookupParameter("座標Y");
                    if (param != null)
                    {
                        param.Set(valY);
                    }
                    ///////////////////////


                    Revit.DB.XYZ vn = _CmpGeometry.UnitVector(curves[0].GetEndPoint(0), curves[0].GetEndPoint(1));
                    Revit.DB.XYZ va = new Revit.DB.XYZ(curves[4].GetEndPoint(0).X - curves[0].GetEndPoint(0).X, curves[4].GetEndPoint(0).Y - curves[0].GetEndPoint(0).Y, curves[4].GetEndPoint(0).Z - curves[0].GetEndPoint(0).Z);
                    Revit.DB.XYZ cr2 = _CmpGeometry.Gaiseki(vn, va);
                    double dDepth = Math.Sqrt(cr2.X * cr2.X + cr2.Y * cr2.Y + cr2.Z * cr2.Z);

                    Revit.DB.Parameter parThickness = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
                    parThickness.Set(heights[0]);

                    Revit.DB.Parameter parWidth1 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH1"));
                    parWidth1.Set(curves[0].Length);

                    Revit.DB.Parameter parDepth3 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH3"));
                    parDepth3.Set(curves[1].Length);

                    Revit.DB.Parameter parWidth3 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH3"));
                    parWidth3.Set(curves[2].Length);

                    Revit.DB.Parameter parWidth2 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH2"));
                    parWidth2.Set(curves[6].Length);

                    Revit.DB.Parameter parDepth2 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH2"));
                    parDepth2.Set(curves[7].Length);

                    Revit.DB.Parameter parDepth1 = famIns.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH1"));
                    parDepth1.Set(dDepth);

                    ret = famIns;
                }
            }
            return ret;
        }

        #endregion

        // プロパティ
        #region Properties

        #endregion
    }
}
