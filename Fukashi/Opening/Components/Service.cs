using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using RvtExtApp   = ADSK.Ext.Fukashi.Opening;

namespace ADSK.Ext.Fukashi.Opening.Components
{
  /// ================================================================================
  /// <summary>サービス</summary>
  /// ================================================================================
  class Service
  {
    // メンバ変数
    #region Member Variables

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

    /// <summary>データテーブル コマンド</summary>
    private RvtExtApp.Entities.DtCmd _EntDtCmd;

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
    Service(RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Components.Elements cmpElements,
            RvtExtApp.Components.Geometry cmpGeometry,
            RvtExtApp.Components.Parameters cmpParameters,
            RvtExtApp.Components.Settings cmpSettings)
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
    /// <history>2016/08/18 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    string WorkFlow()
    {
      string ret = null;

      // プロジェクト情報
      Revit.DB.ProjectInfo projInfo = _CmpElements.ProjectInfo;

      _EntDtCmd = new RvtExtApp.Entities.DtCmd(_CmpAttribute,
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
    /// <history>2016/12/05 Created  CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void Set()
    {
      _EntDtCmd.SetData();
    }

    /// ================================================================================
    /// <summary>島フカシ作成 - 三角形</summary>
    /// 
    /// <param name="curves"    >カーブ</param>
    /// <param name="height"    >高さ</param>
    /// <param name="materialId">マテリアルID</param>
    /// 
    /// <history><p>2016/12/15 Created  CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/02/14 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Triangle(Collections.Generic.IList<Revit.DB.Curve> curves,
                                                   double height,
                                                   Revit.DB.ElementId materialId)
    {
      // 戻り値
      Revit.DB.FamilyInstance ret = null;

      Revit.DB.FamilySymbol famSym = _CmpElements.FamSymTriangle;
      if (famSym != null)
      {
        Revit.DB.Transaction trans = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);
        trans.Start("作成");

        Revit.DB.XYZ p0 = curves[0].GetEndPoint(0);
        Revit.DB.XYZ p1 = curves[1].GetEndPoint(0);
        Revit.DB.XYZ p2 = curves[2].GetEndPoint(0);

        Revit.DB.View actView = _CmpElements.RvtDBDoc.ActiveView;
        Revit.DB.Level level = actView.GenLevel;
        Revit.DB.Reference plnReference = level.GetPlaneReference();

        Revit.DB.XYZ direction = _CmpGeometry.UnitVector(p0, p1);

        ret = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnReference,
                                                             p0,
                                                             direction,
                                                             famSym);

        Revit.DB.Parameter parThickness = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
        parThickness.Set(height);

        double width = _CmpGeometry.Distance(p0, p1);
        double rad1 = _CmpGeometry.Angle3D(p0, p1, p2);
        double rad2 = _CmpGeometry.Angle3D(p1, p2, p0);

        try
        {
          Revit.DB.Parameter parWidth = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
          parWidth.Set(width);

          Revit.DB.Parameter parAngle1 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_ANGLE1"));
          parAngle1.Set(rad1);

          Revit.DB.Parameter parAngle2 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_ANGLE2"));
          parAngle2.Set(rad2);

          Revit.DB.Parameter parMaterial = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
          parMaterial.Set(materialId);

          Revit.DB.Parameter parPosX = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_X"));
          parPosX.Set(0);

          Revit.DB.Parameter parPosY = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_Y"));
          parPosY.Set(0);
        }
        catch
        {
        }

        trans.Commit();
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>島フカシ作成 - 台形</summary>
    /// 
    /// <param name="curves"    >カーブ</param>
    /// <param name="height"    >高さ</param>
    /// <param name="materialId">マテリアルID</param>
    /// 
    /// <history><p>2016/12/15 Created  CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/02/14 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Torapezoid(Collections.Generic.IList<Revit.DB.Curve> curves,
                                                     double height,
                                                     Revit.DB.ElementId materialId)
    {
      // 戻り値
      Revit.DB.FamilyInstance ret = null;

      Revit.DB.FamilySymbol famSym = _CmpElements.FamSymTorapezoid;
      if (famSym != null)
      {
        Revit.DB.Transaction trans = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);
        trans.Start("作成");

        Revit.DB.Curve curve0 = curves[0];
        Revit.DB.Curve curve1 = curves[1];
        Revit.DB.Curve curve2 = curves[2];
        Revit.DB.Curve curve3 = curves[3];

        Revit.DB.Line line0 = curve0 as Revit.DB.Line;
        Revit.DB.Line line1 = curve1 as Revit.DB.Line;
        Revit.DB.Line line2 = curve2 as Revit.DB.Line;
        Revit.DB.Line line3 = curve3 as Revit.DB.Line;

        Revit.DB.XYZ p0 = curves[0].GetEndPoint(0);
        Revit.DB.XYZ p1 = curves[1].GetEndPoint(0);
        Revit.DB.XYZ p2 = curves[2].GetEndPoint(0);
        Revit.DB.XYZ p3 = curves[3].GetEndPoint(0);

        if (_CmpGeometry.IsParallelLine(line0, line2) == false)
        {
          p0 = curves[0].GetEndPoint(1);
          p1 = curves[1].GetEndPoint(1);
          p2 = curves[2].GetEndPoint(1);
          p3 = curves[3].GetEndPoint(1);
        }

        Revit.DB.View actView = _CmpElements.RvtDBDoc.ActiveView;
        Revit.DB.Level level = actView.GenLevel;
        Revit.DB.Reference plnReference = level.GetPlaneReference();

        Revit.DB.XYZ direction = _CmpGeometry.UnitVector(p0, p1);

        ret = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnReference,
                                                             p0,
                                                             direction,
                                                             famSym);

        Revit.DB.Line line = Revit.DB.Line.CreateBound(p0, p1);
        Revit.DB.XYZ verticalPos = _CmpGeometry.GetVertical(line, p3);

        double depth = _CmpGeometry.Distance(p3, verticalPos);
        double width = _CmpGeometry.Distance(p0, p1);
        double rad1 = _CmpGeometry.Angle3D(p0, p1, p3);
        double rad2 = _CmpGeometry.Angle3D(p1, p2, p0);

        try
        {
          Revit.DB.Parameter parThickness = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
          parThickness.Set(height);

          Revit.DB.Parameter parWidth = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
          parWidth.Set(width);

          Revit.DB.Parameter parDepth = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH"));
          parDepth.Set(depth);

          Revit.DB.Parameter parAngle1 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_ANGLE1"));
          parAngle1.Set(rad1);

          Revit.DB.Parameter parAngle2 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_ANGLE2"));
          parAngle2.Set(rad2);

          Revit.DB.Parameter parMaterial = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
          parMaterial.Set(materialId);

          Revit.DB.Parameter parPosX = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_X"));
          parPosX.Set(0);

          Revit.DB.Parameter parPosY = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_Y"));
          parPosY.Set(0);
        }
        catch
        {
        }

        trans.Commit();
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>島フカシ作成 - 長方形</summary>
    /// 
    /// <param name="curves"    >カーブ</param>
    /// <param name="height"    >高さ</param>
    /// <param name="materialId">マテリアルID</param>
    /// 
    /// <history><p>2016/12/15 Created  CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/02/14 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Rectangle(Collections.Generic.IList<Revit.DB.Curve> curves,
                                                    double height,
                                                    Revit.DB.ElementId materialId)
    {
      // 戻り値
      Revit.DB.FamilyInstance ret = null;

      Revit.DB.FamilySymbol famSym = _CmpElements.FamSymRectRect;
      if (famSym != null)
      {
        Revit.DB.Transaction trans = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);
        trans.Start("作成");

        Revit.DB.XYZ p0 = curves[0].GetEndPoint(0);
        Revit.DB.XYZ p1 = curves[1].GetEndPoint(0);
        Revit.DB.XYZ p2 = curves[2].GetEndPoint(0);
        Revit.DB.XYZ p3 = curves[3].GetEndPoint(0);
        Revit.DB.XYZ loc = p0;

        Revit.DB.View actView = _CmpElements.RvtDBDoc.ActiveView;
        Revit.DB.Level level = actView.GenLevel;
        Revit.DB.Reference plnReference = level.GetPlaneReference();

        Revit.DB.XYZ direction = _CmpGeometry.UnitVector(p0, p1);

        ret = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnReference,
                                                             loc,
                                                             direction,
                                                             famSym);

        double width = _CmpGeometry.Distance(p0, p1);
        double depth = _CmpGeometry.Distance(p0, p3);

        try
        {
          Revit.DB.Parameter parThickness = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
          parThickness.Set(height);

          Revit.DB.Parameter parWidth = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH"));
          parWidth.Set(width);

          Revit.DB.Parameter parDepth = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH"));
          parDepth.Set(depth);

          Revit.DB.Parameter parMaterial = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
          parMaterial.Set(materialId);

          Revit.DB.Parameter parPosX = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_X"));
          parPosX.Set(0);

          Revit.DB.Parameter parPosY = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_Y"));
          parPosY.Set(0);
        }
        catch
        {
        }

        trans.Commit();
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>島フカシ作成 - 平行四辺形</summary>
    /// 
    /// <param name="curves"    >カーブ</param>
    /// <param name="height"    >高さ</param>
    /// <param name="materialId">マテリアルID</param>
    /// 
    /// <history><p>2016/12/15 Created  CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/02/14 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Parallelogram(Collections.Generic.IList<Revit.DB.Curve> curves,
                                                        double height,
                                                        Revit.DB.ElementId materialId)
    {
      // 戻り値
      Revit.DB.FamilyInstance ret = null;

      Revit.DB.FamilySymbol famSym = _CmpElements.FamSymParallelogram;
      if (famSym != null)
      {
        Revit.DB.Transaction trans = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);
        trans.Start("作成");

        Revit.DB.XYZ p0 = curves[0].GetEndPoint(0);
        Revit.DB.XYZ p1 = curves[1].GetEndPoint(0);
        Revit.DB.XYZ p2 = curves[2].GetEndPoint(0);
        Revit.DB.XYZ p3 = curves[3].GetEndPoint(0);

        Revit.DB.View actView = _CmpElements.RvtDBDoc.ActiveView;
        Revit.DB.Level level = actView.GenLevel;
        Revit.DB.Reference plnReference = level.GetPlaneReference();

        Revit.DB.XYZ direction = _CmpGeometry.UnitVector(p0, p1);

        ret = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnReference,
                                                             p0,
                                                             direction,
                                                             famSym);

        double length1 = _CmpGeometry.Distance(p0, p1);
        double length2 = _CmpGeometry.Distance(p0, p3);

        double rad = _CmpGeometry.Angle3D(p0, p1, p3);

        try
        {
          Revit.DB.Parameter parThickness = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
          parThickness.Set(height);

          Revit.DB.Parameter parLength1 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_LENGTH1"));
          parLength1.Set(length1);

          Revit.DB.Parameter parLength2 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_LENGTH2"));
          parLength2.Set(length2);

          Revit.DB.Parameter parAngle = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_ANGLE"));
          parAngle.Set(rad);

          Revit.DB.Parameter parMaterial = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
          parMaterial.Set(materialId);

          Revit.DB.Parameter parPosX = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_X"));
          parPosX.Set(0);

          Revit.DB.Parameter parPosY = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_Y"));
          parPosY.Set(0);
        }
        catch
        {
        }
          
        trans.Commit();
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>島フカシ作成 - L字形</summary>
    /// 
    /// <param name="curves"    >カーブ</param>
    /// <param name="height"    >高さ</param>
    /// <param name="materialId">マテリアルID</param>
    /// 
    /// <history><p>2016/12/26 Created  CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/02/14 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Lshape(Collections.Generic.IList<Revit.DB.Curve> curves,
                                                 double height,
                                                 Revit.DB.ElementId materialId)
    {
      // 戻り値
      Revit.DB.FamilyInstance ret = null;

      Revit.DB.FamilySymbol famSym = _CmpElements.FamSymLshape;
      if (famSym != null)
      {
        Revit.DB.Transaction trans = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);
        trans.Start("作成");

        // ファミリ始点に合わせてソート
        Collections.Generic.IList<Revit.DB.Curve> optimizeCrueves = _CmpGeometry.OptimizeLineVertexConvLine(curves);
        Collections.Generic.IList<Revit.DB.Curve> famCurves = _CmpGeometry.SortLshapeCurves(optimizeCrueves);

        if (famCurves.Count != 6)
        {
          return ret;
        }

        Revit.DB.XYZ p0 = famCurves[0].GetEndPoint(0);
        Revit.DB.XYZ p1 = famCurves[1].GetEndPoint(0);

        Revit.DB.View actView = _CmpElements.RvtDBDoc.ActiveView;
        Revit.DB.Level level = actView.GenLevel;
        Revit.DB.Reference plnReference = level.GetPlaneReference();

        Revit.DB.XYZ direction = _CmpGeometry.UnitVector(p0, p1);

        ret = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnReference,
                                                             p0,
                                                             direction,
                                                             famSym);

        double width1 = famCurves[0].Length;
        double width2 = famCurves[4].Length;

        double depth1 = famCurves[5].Length;
        double depth2 = famCurves[1].Length;

        try
        {
          Revit.DB.Parameter parThickness = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
          parThickness.Set(height);

          Revit.DB.Parameter parWidth1 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH1"));
          parWidth1.Set(width1);

          Revit.DB.Parameter parWidth2 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH2"));
          parWidth2.Set(width2);

          Revit.DB.Parameter parDepth1 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH1"));
          parDepth1.Set(depth1);

          Revit.DB.Parameter parDepth2 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH2"));
          parDepth2.Set(depth2);

          Revit.DB.Parameter parMaterial = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
          parMaterial.Set(materialId);

          Revit.DB.Parameter parPosX = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_X"));
          parPosX.Set(0);

          Revit.DB.Parameter parPosY = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_Y"));
          parPosY.Set(0);
        }
        catch
        {
        }

        trans.Commit();
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>島フカシ作成 - 凸形</summary>
    /// 
    /// <param name="curves"    >カーブ</param>
    /// <param name="height"    >高さ</param>
    /// <param name="materialId">マテリアルID</param>
    /// 
    /// <history><p>2016/12/26 Created  CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/02/14 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Convex(Collections.Generic.IList<Revit.DB.Curve> curves,
                                                 double height,
                                                 Revit.DB.ElementId materialId)
    {
      // 戻り値
      Revit.DB.FamilyInstance ret = null;

      Revit.DB.FamilySymbol famSym = _CmpElements.FamSymUneven;
      if (famSym != null)
      {
        Revit.DB.Transaction trans = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);
        trans.Start("作成");

        // ファミリ始点に合わせてソート
        Collections.Generic.IList<Revit.DB.Curve> optimizeCrueves = _CmpGeometry.OptimizeLineVertexConvLine(curves);
        Collections.Generic.IList<Revit.DB.Curve> famCurves = _CmpGeometry.SortConvexCurves(optimizeCrueves);

        if (famCurves.Count != 8)
        {
          return ret;
        }

        Revit.DB.XYZ p0 = famCurves[0].GetEndPoint(0);
        Revit.DB.XYZ p1 = famCurves[1].GetEndPoint(0);

        Revit.DB.View actView = _CmpElements.RvtDBDoc.ActiveView;
        Revit.DB.Level level = actView.GenLevel;
        Revit.DB.Reference plnReference = level.GetPlaneReference();

        Revit.DB.XYZ direction = _CmpGeometry.UnitVector(p0, p1);

        ret = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnReference,
                                                             p0,
                                                             direction,
                                                             famSym);

        double width1 = famCurves[0].Length;
        double width2 = famCurves[6].Length;
        double width3 = famCurves[2].Length;

        Revit.DB.Line line1 = famCurves[0] as Revit.DB.Line;
        Revit.DB.Line line2 = famCurves[4] as Revit.DB.Line;

        double depth1 = _CmpGeometry.ParallelLineDistance(line1, line2);
        double depth2 = famCurves[7].Length;
        double depth3 = famCurves[1].Length;

        try
        {
          Revit.DB.Parameter parThickness = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
          parThickness.Set(height);

          Revit.DB.Parameter parWidth1 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH1"));
          parWidth1.Set(width1);

          Revit.DB.Parameter parWidth2 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH2"));
          parWidth2.Set(width2);

          Revit.DB.Parameter parWidth3 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH3"));
          parWidth3.Set(width3);

          Revit.DB.Parameter parDepth1 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH1"));
          parDepth1.Set(depth1);

          Revit.DB.Parameter parDepth2 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH2"));
          parDepth2.Set(depth2);

          Revit.DB.Parameter parDepth3 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH3"));
          parDepth3.Set(depth3);

          Revit.DB.Parameter parMaterial = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
          parMaterial.Set(materialId);

          Revit.DB.Parameter parPosX = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_X"));
          parPosX.Set(0);

          Revit.DB.Parameter parPosY = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_Y"));
          parPosY.Set(0);
        }
        catch
        {
        }

        trans.Commit();
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>島フカシ作成 - 凹形</summary>
    /// 
    /// <param name="curves"    >カーブ</param>
    /// <param name="height"    >高さ</param>
    /// <param name="materialId">マテリアルID</param>
    /// 
    /// <history><p>2017/01/11 Created  CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/02/14 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Revit.DB.FamilyInstance CreateFukashi_Concave(Collections.Generic.IList<Revit.DB.Curve> curves,
                                                  double height,
                                                  Revit.DB.ElementId materialId)
    {
      // 戻り値
      Revit.DB.FamilyInstance ret = null;

      Revit.DB.FamilySymbol famSym = _CmpElements.FamSymUneven;
      if (famSym != null)
      {
        Revit.DB.Transaction trans = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);
        trans.Start("作成");

        // ファミリ始点に合わせてソート
        Collections.Generic.IList<Revit.DB.Curve> optimizeCrueves = _CmpGeometry.OptimizeLineVertexConvLine(curves);
        Collections.Generic.IList<Revit.DB.Curve> famCurves = _CmpGeometry.SortConcaveCurves(optimizeCrueves);

        if (famCurves.Count != 8)
        {
          return ret;
        }

        Revit.DB.XYZ p0 = famCurves[0].GetEndPoint(0);
        Revit.DB.XYZ p1 = famCurves[1].GetEndPoint(0);

        Revit.DB.View actView = _CmpElements.RvtDBDoc.ActiveView;
        Revit.DB.Level level = actView.GenLevel;
        Revit.DB.Reference plnReference = level.GetPlaneReference();

        Revit.DB.XYZ direction = _CmpGeometry.UnitVector(p0, p1);

        ret = _CmpElements.RvtDBDoc.Create.NewFamilyInstance(plnReference,
                                                             p0,
                                                             direction,
                                                             famSym);

        double width1 = famCurves[0].Length;
        double width2 = famCurves[6].Length;
        double width3 = famCurves[2].Length;

        Revit.DB.Line line1 = famCurves[0] as Revit.DB.Line;
        Revit.DB.Line line2 = famCurves[4] as Revit.DB.Line;

        double depth1 = _CmpGeometry.ParallelLineDistance(line1, line2);
        double depth2 = famCurves[7].Length;
        double depth3 = famCurves[1].Length;

        try
        {
          Revit.DB.Parameter parThickness = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_THICKNESS"));
          parThickness.Set(height);

          Revit.DB.Parameter parWidth1 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH1"));
          parWidth1.Set(width1);

          Revit.DB.Parameter parWidth2 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH2"));
          parWidth2.Set(width2);

          Revit.DB.Parameter parWidth3 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_WIDTH3"));
          parWidth3.Set(width3);

          Revit.DB.Parameter parDepth1 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH1"));
          parDepth1.Set(depth1);

          Revit.DB.Parameter parDepth2 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH2"));
          parDepth2.Set(depth2);

          Revit.DB.Parameter parDepth3 = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHI_DEPTH3"));
          parDepth3.Set(depth3);

          Revit.DB.Parameter parMaterial = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_FUKASHIMATERIAL"));
          parMaterial.Set(materialId);

          Revit.DB.Parameter parPosX = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_X"));
          parPosX.Set(0);

          Revit.DB.Parameter parPosY = ret.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_POS_Y"));
          parPosY.Set(0);
        }
        catch
        {
        }

        trans.Commit();
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>島フカシ作成 - DirectShape</summary>
    /// 
    /// <param name="category"        >カテゴリ</param>
    /// <param name="crvLoops"        >カーブループ</param>
    /// <param name="direction"       >向き</param>
    /// <param name="height"          >高さ</param>
    /// <param name="materialId"      >マテリアルID</param>
    /// <param name="graphicsStyleId" >グラフィックススタイルID</param>
    /// <param name="appId"           >アプリケーションID</param>
    /// <param name="appDataId"       >アプリケーションデータID</param>
    /// 
    /// <history>2016/12/15 Created  CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.DirectShape CreateFukashi_DirectShape(Revit.DB.Category category,
                                                   Collections.Generic.IList<Revit.DB.Curve> curveAry,
                                                   Revit.DB.XYZ direction,
                                                   double height,
                                                   Revit.DB.ElementId materialId,
                                                   Revit.DB.ElementId graphicsStyleId)
    {
      Revit.DB.Transaction trans = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);
      trans.Start("作成");

      string appId = System.Reflection.Assembly.GetExecutingAssembly().GetType().GUID.ToString();
      string appDataId = System.Guid.NewGuid().ToString();

      Revit.DB.CurveLoop crvLoop = new Revit.DB.CurveLoop();

      foreach (Revit.DB.Curve crv in curveAry)
      {
        crvLoop.Append(crv);
      }

      Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = new Collections.Generic.List<Revit.DB.CurveLoop>();
      crvLoops.Add(crvLoop);
      
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

      Revit.DB.Solid solid = null;

      try
      {
        // 押し出し作成
        solid = Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(crvLoops,
                                                                           direction,
                                                                           height,
                                                                           solidOpt);
      }
      catch
      {
        return null;
      }

      if (solid.Volume == 0)
      {
        return null;
      }

      // 既存要素内判定
      Revit.DB.FilteredElementCollector fecElems = new Autodesk.Revit.DB.FilteredElementCollector(_CmpElements.RvtDBDoc, _CmpElements.RvtDBDoc.ActiveView.Id);
      fecElems.WhereElementIsNotElementType();

      Revit.DB.ElementIntersectsSolidFilter solidFilter = new Autodesk.Revit.DB.ElementIntersectsSolidFilter(solid);
      fecElems.WherePasses(solidFilter);

      if (fecElems.GetElementCount() > 0)
      {
       return null;
      }

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

      trans.Commit();

      return ds;
    }

    /// ================================================================================
    /// <summary>ビュー内要素内判定</summary>
    /// 
    /// <param name="famIns">ファミリインスタンス</param>
    /// 
    /// <history>2016/12/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool InElementInView(Revit.DB.FamilyInstance famIns)
    {
      // 戻り値
      bool ret = false;

      try
      {
        if (famIns != null)
        {
          Revit.DB.Transform transform = famIns.GetTransform();

          Collections.Generic.IList<Revit.DB.Solid> solids = _CmpGeometry.GetFamInsSolid(famIns);

          // ビュー内要素
          Revit.DB.FilteredElementCollector fecElems = new Autodesk.Revit.DB.FilteredElementCollector(_CmpElements.RvtDBDoc, _CmpElements.RvtDBDoc.ActiveView.Id);
          fecElems.WhereElementIsNotElementType();

          foreach (Revit.DB.Solid solid in solids)
          {
            // 交差ソリッドフィルター
            Revit.DB.ElementIntersectsSolidFilter solidFilter = new Autodesk.Revit.DB.ElementIntersectsSolidFilter(solid);
            fecElems.WherePasses(solidFilter);

            foreach (Revit.DB.Element elem in fecElems)
            {
              if (elem.Id.ToString() != famIns.Id.ToString())
              {
                ret = true;
                return ret;
              }
            }
          }
        }
      }
      catch (Revit.Exceptions.InvalidObjectException)
      {
        return ret;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ビュー内要素カーブ取得</summary>
    /// 
    /// <history><p>2017/01/10 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2017/01/25 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> GetInViewEdgeCurves()
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new System.Collections.Generic.List<Autodesk.Revit.DB.Curve>();

      // 現在ビュー
      Revit.DB.View view = _CmpElements.RvtDBDoc.ActiveView;
      Revit.DB.ViewPlan viewPlan = view as Revit.DB.ViewPlan;
      if (viewPlan == null)
      {
        return ret;
      }

      // ビュー範囲
      Revit.DB.PlanViewRange viewRange = viewPlan.GetViewRange();

      // ビューズーム領域
      Collections.Generic.IList<Revit.DB.XYZ> zoomCorners = _CmpElements.ViewZoomCorners;

      // 下部レベル
      Revit.DB.Level btmLevel = null;
      // 切断面レベル
      Revit.DB.Level cutLevel = null;

      Revit.DB.ElementId btmLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.BottomClipPlane);
      Revit.DB.ElementId cutLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.CutPlane);

      if (btmLvlId != null)
      {
        btmLevel = _CmpElements.RvtDBDoc.GetElement(btmLvlId) as Revit.DB.Level;
      }
      if (cutLvlId != null)
      {
        cutLevel = _CmpElements.RvtDBDoc.GetElement(cutLvlId) as Revit.DB.Level;
      }

      // オフセット
      double btmOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.BottomClipPlane);
      double topOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.CutPlane);

      double btm = 0;
      double top = 0;

      if (btmLevel != null)
      {
        btm = btmLevel.Elevation + btmOffset - 10;
      }
      else
      {
        btm = -100000;
      }

      if (cutLevel != null)
      {
        top = cutLevel.Elevation + topOffset;
      }
      else
      {
        top = 100000;
      }

      double height = top - btm;
      Revit.DB.XYZ direction = Revit.DB.XYZ.BasisZ;

      Revit.DB.XYZ corner1 = zoomCorners[0];
      Revit.DB.XYZ corner2 = zoomCorners[1];

      Revit.DB.XYZ p0 = new Revit.DB.XYZ(corner1.X, corner1.Y, btm);
      Revit.DB.XYZ p1 = new Revit.DB.XYZ(corner2.X, corner1.Y, btm);
      Revit.DB.XYZ p2 = new Revit.DB.XYZ(corner2.X, corner2.Y, btm);
      Revit.DB.XYZ p3 = new Revit.DB.XYZ(corner1.X, corner2.Y, btm);

      Revit.DB.Line l0 = Revit.DB.Line.CreateBound(p0, p1);
      Revit.DB.Line l1 = Revit.DB.Line.CreateBound(p1, p2);
      Revit.DB.Line l2 = Revit.DB.Line.CreateBound(p2, p3);
      Revit.DB.Line l3 = Revit.DB.Line.CreateBound(p3, p0);

      Revit.DB.CurveLoop crvLoop = new Revit.DB.CurveLoop();

      crvLoop.Append(l0);
      crvLoop.Append(l1);
      crvLoop.Append(l2);
      crvLoop.Append(l3);

      Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = new Collections.Generic.List<Revit.DB.CurveLoop>();
      crvLoops.Add(crvLoop);

      Revit.DB.Solid viewSolid = null;

      try
      {
        // 押し出し作成
        viewSolid = Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(crvLoops,
                                                                           direction,
                                                                           height);
      }
      catch
      {
        return ret;
      }

      if (viewSolid.Volume == 0)
      {
        return ret;
      }

      // ビュー内要素
      Revit.DB.FilteredElementCollector fecElems = new Autodesk.Revit.DB.FilteredElementCollector(_CmpElements.RvtDBDoc, _CmpElements.RvtDBDoc.ActiveView.Id);
      fecElems.WhereElementIsNotElementType();

      // 交差ソリッドフィルター
      Revit.DB.ElementIntersectsSolidFilter solidFilter = new Autodesk.Revit.DB.ElementIntersectsSolidFilter(viewSolid);
      fecElems.WherePasses(solidFilter);

      Collections.Generic.IList<Revit.DB.Element> allElemeAry = new Collections.Generic.List<Revit.DB.Element>();

      foreach (Revit.DB.Element elem in fecElems)
      {
        if (elem.Category != null)
        {
          if (elem.IsHidden(view) == false &&
              view.GetCategoryHidden(elem.Category.Id) == false)
          {
            allElemeAry.Add(elem);
          }
        }
      }

      // 面取得
      #region 面取得
      Collections.Generic.IList<Revit.DB.Face> allFaces = new Collections.Generic.List<Revit.DB.Face>();

      Revit.DB.Options opt = _CmpElements.RvtDBDoc.Application.Create.NewGeometryOptions();
      
      foreach (Revit.DB.Element elem in allElemeAry)
      {
        Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

        if (geomElem == null)
        {
          continue;
        }

        Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
        geoObjEnum.Reset();

        while (geoObjEnum.MoveNext())
        {
          Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
          Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

          if (geoIns != null)
          {
            Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
            if (famIns != null)
            {
              Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
              Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
              goEnum.Reset();

              while (goEnum.MoveNext())
              {
                Revit.DB.GeometryObject go = goEnum.Current;

                Revit.DB.Solid solid = go as Revit.DB.Solid;

                if (solid != null)
                {
                  foreach (Revit.DB.Face face in solid.Faces)
                  {
                    // 表示判定
                    if (_CmpGeometry.IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                    {
                      Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                      if (pf == null)
                      {
                        continue;
                      }

                      allFaces.Add(face);
                    }
                  }
                }
              }
            }
          }
          else
          {
            Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

            if (solid != null)
            {
              foreach (Revit.DB.Face face in solid.Faces)
              {
                // 表示判定
                if (_CmpGeometry.IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                {
                  Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                  if (pf == null)
                  {
                    continue;
                  }

                  allFaces.Add(face);
                }
              }
            }
          }
        }
      }
      #endregion

      // 作業面
      Revit.DB.SketchPlane sktPln = view.SketchPlane;

      if (sktPln != null)
      {
        _CmpGeometry.BasePlane = sktPln.GetPlane();
        Revit.DB.XYZ plnOrigin = _CmpGeometry.BasePlane.Origin;
        Revit.DB.XYZ xVec = _CmpGeometry.BasePlane.XVec;
        Revit.DB.XYZ yVec = _CmpGeometry.BasePlane.YVec;

        Revit.DB.XYZ posPln1 = plnOrigin + xVec * 1000000;
        Revit.DB.XYZ posPln2 = plnOrigin + yVec * 1000000;
        Revit.DB.XYZ posPln3 = plnOrigin - xVec * 1000000;
        Revit.DB.XYZ posPln4 = plnOrigin - yVec * 1000000;

        Revit.DB.Line linePln1 = Revit.DB.Line.CreateBound(posPln1, posPln2);
        Revit.DB.Line linePln2 = Revit.DB.Line.CreateBound(posPln2, posPln3);
        Revit.DB.Line linePln3 = Revit.DB.Line.CreateBound(posPln3, posPln4);
        Revit.DB.Line linePln4 = Revit.DB.Line.CreateBound(posPln4, posPln1);

        crvLoop = new Revit.DB.CurveLoop();
        crvLoop.Append(linePln1);
        crvLoop.Append(linePln2);
        crvLoop.Append(linePln3);
        crvLoop.Append(linePln4);

        crvLoops = new Collections.Generic.List<Revit.DB.CurveLoop>();
        crvLoops.Add(crvLoop);

        // 面ソリッド
        Revit.DB.Solid plnSolid = null;

        try
        {
          // ビューと逆向きに押し出し作成
          plnSolid = Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(crvLoops, -view.ViewDirection, _CmpGeometry.Approx0Len);
        }
        catch
        {
          return ret;
        }

        Revit.DB.PlanarFace orgFace = null;
        foreach (Revit.DB.Face plnFace in plnSolid.Faces)
        {
          Revit.DB.PlanarFace pf = plnFace as Revit.DB.PlanarFace;

          if (pf != null)
          {
            // ビューと同じ向きの面
            if (_CmpGeometry.ToHalfAdjust(pf.FaceNormal.Z, -9) == _CmpGeometry.ToHalfAdjust(view.ViewDirection.Z, -9))
            {
              orgFace = pf;
            }
          }
        }


        foreach (Revit.DB.Face face in allFaces)
        {
          try
          {
            // 基準面との関連
            // 面と交差または接する場合、関連あり
            // 面のどちらか側にある場合、関連なし
            bool relationFace = _CmpGeometry.CheckRelationFace(orgFace,
                                                               face as Revit.DB.PlanarFace);

            if (relationFace)
            {
              Revit.DB.Curve intersectCrv = null;

              // 面と面の交差カーブ
              if (orgFace.Intersect(face, out intersectCrv) == Revit.DB.FaceIntersectionFaceResult.Intersecting)
              {
                if (intersectCrv.IsCyclic == false)
                {
                  ret.Add(intersectCrv);
                }
                else
                {
                  Collections.Generic.IList<Revit.DB.Line> convertLines = _CmpGeometry.CurveConvertToLine(intersectCrv);

                  foreach (Revit.DB.Line line in convertLines)
                  {
                    ret.Add(line);
                  }
                }
              }
            }
            
            if(relationFace == false)
            {
              Collections.Generic.IList<Revit.DB.CurveLoop> loops = face.GetEdgesAsCurveLoops();

              foreach (Revit.DB.CurveLoop loop in loops)
              {
                Collections.Generic.IList<Revit.DB.Curve> crvs = new Collections.Generic.List<Revit.DB.Curve>();
                foreach (Revit.DB.Curve curve in loop)
                {
                  crvs.Add(curve);
                }

                crvs = _CmpGeometry.OptimizeLineVertexConvLine(crvs);

                foreach (Revit.DB.Curve curve in crvs)
                {
                  // ビュー範囲内線分
                  Revit.DB.Line line = _CmpGeometry.GetLineInHeight(curve, null, cutLevel, btmOffset, topOffset);

                  if (line != null)
                  {
                    // 面への投影線分
                    Revit.DB.Line shadowLine = _CmpGeometry.GetShadowLine(orgFace, line);

                    if (shadowLine != null)
                    {
                      bool b = false;

                      foreach (Revit.DB.Line l in ret)
                      {
                        if (_CmpGeometry.Distance2D(l.GetEndPoint(0), shadowLine.GetEndPoint(0)) < _CmpGeometry.Approx0Len &&
                            _CmpGeometry.Distance2D(l.GetEndPoint(1), shadowLine.GetEndPoint(1)) < _CmpGeometry.Approx0Len)
                        {
                          b = true;
                          break;
                        }
                        if (_CmpGeometry.Distance2D(l.GetEndPoint(0), shadowLine.GetEndPoint(1)) < _CmpGeometry.Approx0Len &&
                            _CmpGeometry.Distance2D(l.GetEndPoint(1), shadowLine.GetEndPoint(0)) < _CmpGeometry.Approx0Len)
                        {
                          b = true;
                          break;
                        }
                      }

                      if (b == false)
                      {
                        ret.Add(shadowLine);
                      }
                    }
                  }
                }
              }
            }
          }
          catch
          {
            continue;
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ビュー内要素カーブ取得 2</summary>
    /// 
    /// <history>2017/01/13 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> GetInViewEdgeCurves2()
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new System.Collections.Generic.List<Autodesk.Revit.DB.Curve>();

      // 現在ビュー
      Revit.DB.View view = _CmpElements.RvtDBDoc.ActiveView;
      Revit.DB.ViewPlan viewPlan = view as Revit.DB.ViewPlan;
      if (viewPlan == null)
      {
        return ret;
      }

      // ビュー範囲
      Revit.DB.PlanViewRange viewRange = viewPlan.GetViewRange();

      // ビューズーム領域
      Collections.Generic.IList<Revit.DB.XYZ> zoomCorners = _CmpElements.ViewZoomCorners;

      // 下部レベル
      Revit.DB.Level btmLevel = null;
      // 切断面レベル
      Revit.DB.Level cutLevel = null;

      Revit.DB.ElementId btmLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.BottomClipPlane);
      Revit.DB.ElementId cutLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.CutPlane);

      if (btmLvlId != null)
      {
        btmLevel = _CmpElements.RvtDBDoc.GetElement(btmLvlId) as Revit.DB.Level;
      }
      if (cutLvlId != null)
      {
        cutLevel = _CmpElements.RvtDBDoc.GetElement(cutLvlId) as Revit.DB.Level;
      }

      // オフセット
      double btmOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.BottomClipPlane);
      double topOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.CutPlane);

      double btm = 0;
      double top = 0;

      if (btmLevel != null)
      {
        btm = btmLevel.Elevation + btmOffset;
      }
      else
      {
        btm = -100000;
      }

      if (cutLevel != null)
      {
        top = cutLevel.Elevation + topOffset;
      }
      else
      {
        top = 100000;
      }

      double height = top - btm;
      Revit.DB.XYZ direction = Revit.DB.XYZ.BasisZ;

      Revit.DB.XYZ corner1 = zoomCorners[0];
      Revit.DB.XYZ corner2 = zoomCorners[1];

      Revit.DB.XYZ p0 = new Revit.DB.XYZ(corner1.X, corner1.Y, btm);
      Revit.DB.XYZ p1 = new Revit.DB.XYZ(corner2.X, corner1.Y, btm);
      Revit.DB.XYZ p2 = new Revit.DB.XYZ(corner2.X, corner2.Y, btm);
      Revit.DB.XYZ p3 = new Revit.DB.XYZ(corner1.X, corner2.Y, btm);

      Revit.DB.Line l0 = Revit.DB.Line.CreateBound(p0, p1);
      Revit.DB.Line l1 = Revit.DB.Line.CreateBound(p1, p2);
      Revit.DB.Line l2 = Revit.DB.Line.CreateBound(p2, p3);
      Revit.DB.Line l3 = Revit.DB.Line.CreateBound(p3, p0);

      Revit.DB.CurveLoop crvLoop = new Revit.DB.CurveLoop();

      crvLoop.Append(l0);
      crvLoop.Append(l1);
      crvLoop.Append(l2);
      crvLoop.Append(l3);

      Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = new Collections.Generic.List<Revit.DB.CurveLoop>();
      crvLoops.Add(crvLoop);

      Revit.DB.Solid ViewSolid = null;

      try
      {
        // 押し出し作成
        ViewSolid = Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(crvLoops,
                                                                           direction,
                                                                           height);
      }
      catch
      {
        return ret;
      }

      if (ViewSolid.Volume == 0)
      {
        return ret;
      }

      // ビュー内要素
      Revit.DB.FilteredElementCollector fecElems = new Autodesk.Revit.DB.FilteredElementCollector(_CmpElements.RvtDBDoc, _CmpElements.RvtDBDoc.ActiveView.Id);
      fecElems.WhereElementIsNotElementType();

      // 交差ソリッドフィルター
      Revit.DB.ElementIntersectsSolidFilter solidFilter = new Autodesk.Revit.DB.ElementIntersectsSolidFilter(ViewSolid);
      fecElems.WherePasses(solidFilter);

      Collections.Generic.IList<Revit.DB.Element> allElemeAry = new Collections.Generic.List<Revit.DB.Element>();

      foreach (Revit.DB.Element elem in fecElems)
      {
        if (elem.Category != null)
        {
          if (elem.IsHidden(view) == false &&
              view.GetCategoryHidden(elem.Category.Id) == false)
          {
            allElemeAry.Add(elem);
          }
        }
      }

      // 面取得
      #region 面取得
      Collections.Generic.IList<Revit.DB.Face> allFaces = new Collections.Generic.List<Revit.DB.Face>();

      Revit.DB.Options opt = _CmpElements.RvtDBDoc.Application.Create.NewGeometryOptions();

      foreach (Revit.DB.Element elem in allElemeAry)
      {
        Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

        if (geomElem == null)
        {
          continue;
        }

        Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
        geoObjEnum.Reset();

        while (geoObjEnum.MoveNext())
        {
          Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
          Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

          if (geoIns != null)
          {
            Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
            if (famIns != null)
            {
              Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
              Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
              goEnum.Reset();

              while (goEnum.MoveNext())
              {
                Revit.DB.GeometryObject go = goEnum.Current;

                Revit.DB.Solid solid = go as Revit.DB.Solid;

                if (solid != null)
                {
                  foreach (Revit.DB.Face face in solid.Faces)
                  {
                    // 表示判定
                    if (_CmpGeometry.IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                    {
                      Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                      if (pf == null)
                      {
                        continue;
                      }

                      allFaces.Add(face);
                    }
                  }
                }
              }
            }
          }
          else
          {
            Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

            if (solid != null)
            {
              foreach (Revit.DB.Face face in solid.Faces)
              {
                // 表示判定
                if (_CmpGeometry.IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                {
                  Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                  if (pf == null)
                  {
                    continue;
                  }

                  allFaces.Add(face);
                }
              }
            }
          }
        }
      }
      #endregion

      // 作業面
      Revit.DB.SketchPlane sktPln = view.SketchPlane;

      if (sktPln != null)
      {
        double btmHeight = -1000000;
        double topHeight = 1000000;

        if (btmLevel != null)
        {
          btmHeight = btmLevel.Elevation + btmOffset;
        }

        if (cutLevel != null)
        {
          topHeight = cutLevel.Elevation + topOffset;
        }

        double extDist = topHeight - btmHeight;

        Revit.DB.XYZ posPln1 = new Revit.DB.XYZ(-1000000, -1000000, btmHeight);
        Revit.DB.XYZ posPln2 = new Revit.DB.XYZ(1000000, -1000000, btmHeight);
        Revit.DB.XYZ posPln3 = new Revit.DB.XYZ(1000000, 1000000, btmHeight);
        Revit.DB.XYZ posPln4 = new Revit.DB.XYZ(-1000000, 1000000, btmHeight);

        Revit.DB.Line linePln1 = Revit.DB.Line.CreateBound(posPln1, posPln2);
        Revit.DB.Line linePln2 = Revit.DB.Line.CreateBound(posPln2, posPln3);
        Revit.DB.Line linePln3 = Revit.DB.Line.CreateBound(posPln3, posPln4);
        Revit.DB.Line linePln4 = Revit.DB.Line.CreateBound(posPln4, posPln1);

        crvLoop = new Revit.DB.CurveLoop();
        crvLoop.Append(linePln1);
        crvLoop.Append(linePln2);
        crvLoop.Append(linePln3);
        crvLoop.Append(linePln4);

        crvLoops = new Collections.Generic.List<Revit.DB.CurveLoop>();
        crvLoops.Add(crvLoop);

        // 面ソリッド
        Revit.DB.Solid plnSolid = null;

        try
        {
          // ビュー範囲の押し出し作成
          plnSolid = Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(crvLoops, Revit.DB.XYZ.BasisZ, extDist);
        }
        catch
        {
          return ret;
        }

        Revit.DB.PlanarFace topFace = null;
        Revit.DB.PlanarFace btmFace = null;

        foreach (Revit.DB.Face plnFace in plnSolid.Faces)
        {
          Revit.DB.PlanarFace pf = plnFace as Revit.DB.PlanarFace;

          if (pf != null)
          {
            if (_CmpGeometry.ToHalfAdjust(pf.FaceNormal.Z, -9) == 1)
            {
              topFace = pf;
            }
            if (_CmpGeometry.ToHalfAdjust(pf.FaceNormal.Z, -9) == -1)
            {
              btmFace = pf;
            }
          }
        }


        foreach (Revit.DB.Face face in allFaces)
        {
          try
          {
            // 基準面との関連
            // 面と交差または接する場合、関連あり
            // 面のどちらか側にある場合、関連なし
            bool relationTopFace = _CmpGeometry.CheckRelationFace(topFace,
                                                                  face as Revit.DB.PlanarFace);

            bool relationBtmFace = _CmpGeometry.CheckRelationFace(btmFace,
                                                                  face as Revit.DB.PlanarFace);

            if (relationTopFace)
            {
              Revit.DB.Curve intersectCrv = null;

              // 面と面の交差カーブ
              if (topFace.Intersect(face, out intersectCrv) == Revit.DB.FaceIntersectionFaceResult.Intersecting)
              {
                if (intersectCrv.IsCyclic == false)
                {
                  ret.Add(intersectCrv);
                }
                else
                {
                  Collections.Generic.IList<Revit.DB.Line> convertLines = _CmpGeometry.CurveConvertToLine(intersectCrv);

                  foreach (Revit.DB.Line line in convertLines)
                  {
                    ret.Add(line);
                  }
                }
              }
            }

            if (relationBtmFace)
            {
              Revit.DB.Curve intersectCrv = null;

              // 面と面の交差カーブ
              if (btmFace.Intersect(face, out intersectCrv) == Revit.DB.FaceIntersectionFaceResult.Intersecting)
              {
                if (intersectCrv.IsCyclic == false)
                {
                  ret.Add(intersectCrv);
                }
                else
                {
                  Collections.Generic.IList<Revit.DB.Line> convertLines = _CmpGeometry.CurveConvertToLine(intersectCrv);

                  foreach (Revit.DB.Line line in convertLines)
                  {
                    ret.Add(line);
                  }
                }
              }
            }

            //if (relationTopFace == false && relationBtmFace == false)
            {
              Collections.Generic.IList<Revit.DB.CurveLoop> loops = face.GetEdgesAsCurveLoops();

              foreach (Revit.DB.CurveLoop loop in loops)
              {
                foreach (Revit.DB.Curve curve in loop)
                {
                  // ビュー範囲内線分
                  Revit.DB.Line line = _CmpGeometry.GetLineInHeight(curve, btmLevel, cutLevel, btmOffset, topOffset);

                  if (line != null)
                  {
                    // 面への投影線分
                    Revit.DB.Line shadowLine = _CmpGeometry.GetShadowLine(topFace, line);

                    if (shadowLine != null)
                    {
                      ret.Add(shadowLine);
                    }
                  }
                }
              }
            }
          }
          catch
          {
            continue;
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ビュー内要素カーブ取得 3</summary>
    /// 
    /// <history>2017/01/23 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> GetInViewEdgeCurves3(Collections.Generic.ICollection<Revit.DB.Reference> pickObjs)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new System.Collections.Generic.List<Autodesk.Revit.DB.Curve>();

      // 現在ビュー
      Revit.DB.View view = _CmpElements.RvtDBDoc.ActiveView;
      Revit.DB.ViewPlan viewPlan = view as Revit.DB.ViewPlan;
      if (viewPlan == null)
      {
        return ret;
      }

      // ビュー範囲
      Revit.DB.PlanViewRange viewRange = viewPlan.GetViewRange();

      // ビューズーム領域
      Collections.Generic.IList<Revit.DB.XYZ> zoomCorners = _CmpElements.ViewZoomCorners;

      // 下部レベル
      Revit.DB.Level btmLevel = null;
      // 切断面レベル
      Revit.DB.Level cutLevel = null;

      Revit.DB.ElementId btmLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.BottomClipPlane);
      Revit.DB.ElementId cutLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.CutPlane);

      if (btmLvlId != null)
      {
        btmLevel = _CmpElements.RvtDBDoc.GetElement(btmLvlId) as Revit.DB.Level;
      }
      if (cutLvlId != null)
      {
        cutLevel = _CmpElements.RvtDBDoc.GetElement(cutLvlId) as Revit.DB.Level;
      }

      // オフセット
      double btmOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.BottomClipPlane);
      double topOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.CutPlane);

      double btm = 0;
      double top = 0;

      if (btmLevel != null)
      {
        btm = btmLevel.Elevation + btmOffset - 10;
      }
      else
      {
        btm = -100000;
      }

      if (cutLevel != null)
      {
        top = cutLevel.Elevation + topOffset;
      }
      else
      {
        top = 100000;
      }

      double height = top - btm;
      Revit.DB.XYZ direction = Revit.DB.XYZ.BasisZ;
      
      Collections.Generic.IList<Revit.DB.Element> allElemeAry = new Collections.Generic.List<Revit.DB.Element>();

      foreach (Revit.DB.Reference refe in pickObjs)
      {
        Revit.DB.Element elem = _CmpElements.RvtDBDoc.GetElement(refe);

        if (elem.Category != null)
        {
          allElemeAry.Add(elem);
        }
      }


      // 面取得
      #region 面取得
      Collections.Generic.IList<Revit.DB.Face> allFaces = new Collections.Generic.List<Revit.DB.Face>();

      Revit.DB.Options opt = _CmpElements.RvtDBDoc.Application.Create.NewGeometryOptions();
      opt.View = _CmpElements.RvtDBDoc.ActiveView;

      foreach (Revit.DB.Element elem in allElemeAry)
      {
        Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

        if (geomElem == null)
        {
          continue;
        }

        Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
        geoObjEnum.Reset();

        while (geoObjEnum.MoveNext())
        {
          Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
          Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

          if (geoIns != null)
          {
            Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
            if (famIns != null)
            {
              Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
              Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
              goEnum.Reset();

              while (goEnum.MoveNext())
              {
                Revit.DB.GeometryObject go = goEnum.Current;

                Revit.DB.Solid solid = go as Revit.DB.Solid;

                if (solid != null)
                {
                  foreach (Revit.DB.Face face in solid.Faces)
                  {
                    // 表示判定
                    if (_CmpGeometry.IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                    {
                      Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                      if (pf == null)
                      {
                        continue;
                      }

                      allFaces.Add(face);
                    }
                  }
                }
              }
            }
          }
          else
          {
            Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

            if (solid != null)
            {
              foreach (Revit.DB.Face face in solid.Faces)
              {
                // 表示判定
                if (_CmpGeometry.IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                {
                  Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                  if (pf == null)
                  {
                    continue;
                  }

                  allFaces.Add(face);
                }
              }
            }
          }
        }
      }
      #endregion

      // 作業面
      Revit.DB.SketchPlane sktPln = view.SketchPlane;

      if (sktPln != null)
      {
        _CmpGeometry.BasePlane = sktPln.GetPlane();
        Revit.DB.XYZ plnOrigin = _CmpGeometry.BasePlane.Origin;
        Revit.DB.XYZ xVec = _CmpGeometry.BasePlane.XVec;
        Revit.DB.XYZ yVec = _CmpGeometry.BasePlane.YVec;

        Revit.DB.XYZ posPln1 = plnOrigin + xVec * 1000000;
        Revit.DB.XYZ posPln2 = plnOrigin + yVec * 1000000;
        Revit.DB.XYZ posPln3 = plnOrigin - xVec * 1000000;
        Revit.DB.XYZ posPln4 = plnOrigin - yVec * 1000000;

        Revit.DB.Line linePln1 = Revit.DB.Line.CreateBound(posPln1, posPln2);
        Revit.DB.Line linePln2 = Revit.DB.Line.CreateBound(posPln2, posPln3);
        Revit.DB.Line linePln3 = Revit.DB.Line.CreateBound(posPln3, posPln4);
        Revit.DB.Line linePln4 = Revit.DB.Line.CreateBound(posPln4, posPln1);

        Revit.DB.CurveLoop crvLoop = new Revit.DB.CurveLoop();
        crvLoop.Append(linePln1);
        crvLoop.Append(linePln2);
        crvLoop.Append(linePln3);
        crvLoop.Append(linePln4);

        Collections.Generic.IList<Revit.DB.CurveLoop>  crvLoops = new Collections.Generic.List<Revit.DB.CurveLoop>();
        crvLoops.Add(crvLoop);

        // 面ソリッド
        Revit.DB.Solid plnSolid = null;

        try
        {
          // ビューと逆向きに押し出し作成
          plnSolid = Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(crvLoops, -view.ViewDirection, _CmpGeometry.Approx0Len);
        }
        catch
        {
          return ret;
        }

        Revit.DB.PlanarFace orgFace = null;
        foreach (Revit.DB.Face plnFace in plnSolid.Faces)
        {
          Revit.DB.PlanarFace pf = plnFace as Revit.DB.PlanarFace;

          if (pf != null)
          {
            // ビューと同じ向きの面
            if (_CmpGeometry.ToHalfAdjust(pf.FaceNormal.Z, -9) == _CmpGeometry.ToHalfAdjust(view.ViewDirection.Z, -9))
            {
              orgFace = pf;
            }
          }
        }


        foreach (Revit.DB.Face face in allFaces)
        {
          try
          {
            // 基準面との関連
            // 面と交差または接する場合、関連あり
            // 面のどちらか側にある場合、関連なし
            bool relationFace = _CmpGeometry.CheckRelationFace(orgFace,
                                                               face as Revit.DB.PlanarFace);

            if (relationFace)
            {
              Revit.DB.Curve intersectCrv = null;

              // 面と面の交差カーブ
              if (orgFace.Intersect(face, out intersectCrv) == Revit.DB.FaceIntersectionFaceResult.Intersecting)
              {
                if (intersectCrv.Length < 1000000)
                {
                  if (intersectCrv.IsCyclic == false)
                  {
                    bool b = false;

                    foreach (Revit.DB.Line l in ret)
                    {
                      if (_CmpGeometry.Distance2D(l.GetEndPoint(0), intersectCrv.GetEndPoint(0)) < _CmpGeometry.Approx0Len &&
                          _CmpGeometry.Distance2D(l.GetEndPoint(1), intersectCrv.GetEndPoint(1)) < _CmpGeometry.Approx0Len)
                      {
                        b = true;
                        break;
                      }
                      if (_CmpGeometry.Distance2D(l.GetEndPoint(0), intersectCrv.GetEndPoint(1)) < _CmpGeometry.Approx0Len &&
                          _CmpGeometry.Distance2D(l.GetEndPoint(1), intersectCrv.GetEndPoint(0)) < _CmpGeometry.Approx0Len)
                      {
                        b = true;
                        break;
                      }
                    }

                    if (b == false)
                    {
                      ret.Add(intersectCrv);
                    }
                  }
                  else
                  {
                    Collections.Generic.IList<Revit.DB.Line> convertLines = _CmpGeometry.CurveConvertToLine(intersectCrv);

                    foreach (Revit.DB.Line line in convertLines)
                    {
                      ret.Add(line);
                    }
                  }
                }
                else
                {
                  relationFace = false;
                }
              }
            }

            if (relationFace == false)
            {
              Collections.Generic.IList<Revit.DB.CurveLoop> loops = face.GetEdgesAsCurveLoops();

              foreach (Revit.DB.CurveLoop loop in loops)
              {
                Collections.Generic.IList<Revit.DB.Curve> crvs = new Collections.Generic.List<Revit.DB.Curve>();
                foreach (Revit.DB.Curve curve in loop)
                {
                  crvs.Add(curve);
                }

                crvs = _CmpGeometry.OptimizeLineVertexConvLine(crvs);

                foreach (Revit.DB.Curve curve in crvs)
                {
                  // ビュー範囲内線分
                  Revit.DB.Line line = _CmpGeometry.GetLineInHeight(curve, null, cutLevel, btmOffset, topOffset);

                  if (line != null)
                  {
                    // 面への投影線分
                    Revit.DB.Line shadowLine = _CmpGeometry.GetShadowLine(orgFace, line);

                    if (shadowLine != null)
                    {
                      bool b = false;

                      foreach (Revit.DB.Line l in ret)
                      {
                        if (_CmpGeometry.Distance2D(l.GetEndPoint(0), shadowLine.GetEndPoint(0)) < _CmpGeometry.Approx0Len &&
                            _CmpGeometry.Distance2D(l.GetEndPoint(1), shadowLine.GetEndPoint(1)) < _CmpGeometry.Approx0Len)
                        {
                          b = true;
                          break;
                        }
                        if (_CmpGeometry.Distance2D(l.GetEndPoint(0), shadowLine.GetEndPoint(1)) < _CmpGeometry.Approx0Len &&
                            _CmpGeometry.Distance2D(l.GetEndPoint(1), shadowLine.GetEndPoint(0)) < _CmpGeometry.Approx0Len)
                        {
                          b = true;
                          break;
                        }
                      }

                      if (b == false)
                      {
                        ret.Add(shadowLine);
                      }
                    }
                  }
                }
              }
            }
          }
          catch
          {
            continue;
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ビュー内要素カーブ取得 4</summary>
    /// 
    /// <history>2017/01/23 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Curve> GetInViewEdgeCurves4(Collections.Generic.ICollection<Revit.DB.Reference> pickObjs)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Curve> ret = new System.Collections.Generic.List<Autodesk.Revit.DB.Curve>();

      // 現在ビュー
      Revit.DB.View view = _CmpElements.RvtDBDoc.ActiveView;
      Revit.DB.ViewPlan viewPlan = view as Revit.DB.ViewPlan;
      if (viewPlan == null)
      {
        return ret;
      }

      // ビュー範囲
      Revit.DB.PlanViewRange viewRange = viewPlan.GetViewRange();

      // ビューズーム領域
      Collections.Generic.IList<Revit.DB.XYZ> zoomCorners = _CmpElements.ViewZoomCorners;

      // 下部レベル
      Revit.DB.Level btmLevel = null;
      // 切断面レベル
      Revit.DB.Level cutLevel = null;

      Revit.DB.ElementId btmLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.BottomClipPlane);
      Revit.DB.ElementId cutLvlId = viewRange.GetLevelId(Revit.DB.PlanViewPlane.CutPlane);

      if (btmLvlId != null)
      {
        btmLevel = _CmpElements.RvtDBDoc.GetElement(btmLvlId) as Revit.DB.Level;
      }
      if (cutLvlId != null)
      {
        cutLevel = _CmpElements.RvtDBDoc.GetElement(cutLvlId) as Revit.DB.Level;
      }

      // オフセット
      double btmOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.BottomClipPlane);
      double topOffset = viewRange.GetOffset(Revit.DB.PlanViewPlane.CutPlane);

      double btm = 0;
      double top = 0;

      if (btmLevel != null)
      {
        btm = btmLevel.Elevation + btmOffset - 10;
      }
      else
      {
        btm = -100000;
      }

      if (cutLevel != null)
      {
        top = cutLevel.Elevation + topOffset;
      }
      else
      {
        top = 100000;
      }

      double height = top - btm;
      Revit.DB.XYZ direction = Revit.DB.XYZ.BasisZ;

      Collections.Generic.IList<Revit.DB.Element> allElemeAry = new Collections.Generic.List<Revit.DB.Element>();

      foreach (Revit.DB.Reference refe in pickObjs)
      {
        Revit.DB.Element elem = _CmpElements.RvtDBDoc.GetElement(refe);

        if (elem.Category != null)
        {
          allElemeAry.Add(elem);
        }
      }


      // 面取得
      #region 面取得
      Collections.Generic.IList<Revit.DB.Face> allFaces = new Collections.Generic.List<Revit.DB.Face>();

      Revit.DB.Options opt = _CmpElements.RvtDBDoc.Application.Create.NewGeometryOptions();
      opt.View = _CmpElements.RvtDBDoc.ActiveView;

      foreach (Revit.DB.Element elem in allElemeAry)
      {
        Revit.DB.GeometryElement geomElem = elem.get_Geometry(opt);

        if (geomElem == null)
        {
          continue;
        }

        Collections.Generic.IEnumerator<Revit.DB.GeometryObject> geoObjEnum = geomElem.GetEnumerator();
        geoObjEnum.Reset();

        while (geoObjEnum.MoveNext())
        {
          Revit.DB.GeometryObject geoObj = geoObjEnum.Current;
          Revit.DB.GeometryInstance geoIns = geoObj as Revit.DB.GeometryInstance;

          if (geoIns != null)
          {
            Revit.DB.FamilyInstance famIns = elem as Revit.DB.FamilyInstance;
            if (famIns != null)
            {
              Revit.DB.GeometryElement ge = geoIns.GetSymbolGeometry(famIns.GetTransform());
              Collections.Generic.IEnumerator<Revit.DB.GeometryObject> goEnum = ge.GetEnumerator();
              goEnum.Reset();

              while (goEnum.MoveNext())
              {
                Revit.DB.GeometryObject go = goEnum.Current;

                Revit.DB.Solid solid = go as Revit.DB.Solid;

                if (solid != null)
                {
                  foreach (Revit.DB.Face face in solid.Faces)
                  {
                    // 表示判定
                    if (_CmpGeometry.IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                    {
                      Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                      if (pf == null)
                      {
                        continue;
                      }

                      allFaces.Add(face);
                    }
                  }
                }
              }
            }
          }
          else
          {
            Revit.DB.Solid solid = geoObj as Revit.DB.Solid;

            if (solid != null)
            {
              foreach (Revit.DB.Face face in solid.Faces)
              {
                // 表示判定
                if (_CmpGeometry.IsVisibleFaceInHeight(face, btmLevel, cutLevel, btmOffset, topOffset))
                {
                  Revit.DB.PlanarFace pf = face as Revit.DB.PlanarFace;
                  if (pf == null)
                  {
                    continue;
                  }

                  allFaces.Add(face);
                }
              }
            }
          }
        }
      }
      #endregion

      // 作業面
      Revit.DB.SketchPlane sktPln = view.SketchPlane;

      if (sktPln != null)
      {
        _CmpGeometry.BasePlane = sktPln.GetPlane();
        Revit.DB.XYZ plnOrigin = _CmpGeometry.BasePlane.Origin;
        Revit.DB.XYZ xVec = _CmpGeometry.BasePlane.XVec;
        Revit.DB.XYZ yVec = _CmpGeometry.BasePlane.YVec;

        Revit.DB.XYZ posPln1 = plnOrigin + xVec * 1000000;
        Revit.DB.XYZ posPln2 = plnOrigin + yVec * 1000000;
        Revit.DB.XYZ posPln3 = plnOrigin - xVec * 1000000;
        Revit.DB.XYZ posPln4 = plnOrigin - yVec * 1000000;

        Revit.DB.Line linePln1 = Revit.DB.Line.CreateBound(posPln1, posPln2);
        Revit.DB.Line linePln2 = Revit.DB.Line.CreateBound(posPln2, posPln3);
        Revit.DB.Line linePln3 = Revit.DB.Line.CreateBound(posPln3, posPln4);
        Revit.DB.Line linePln4 = Revit.DB.Line.CreateBound(posPln4, posPln1);

        Revit.DB.CurveLoop crvLoop = new Revit.DB.CurveLoop();
        crvLoop.Append(linePln1);
        crvLoop.Append(linePln2);
        crvLoop.Append(linePln3);
        crvLoop.Append(linePln4);

        Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = new Collections.Generic.List<Revit.DB.CurveLoop>();
        crvLoops.Add(crvLoop);

        // ビューソリッド
        Revit.DB.Solid viewSolid = null;

        try
        {
          // ビューの向きに押し出し作成
          viewSolid = Revit.DB.GeometryCreationUtilities.CreateExtrusionGeometry(crvLoops, view.ViewDirection, top - plnOrigin.Z);
        }
        catch
        {
          return ret;
        }

        Revit.DB.PlanarFace orgFace = null;
        Revit.DB.PlanarFace topFace = null;
        foreach (Revit.DB.Face plnFace in viewSolid.Faces)
        {
          Revit.DB.PlanarFace pf = plnFace as Revit.DB.PlanarFace;

          if (pf != null)
          {
            // ビューと逆向きの面
            if (_CmpGeometry.ToHalfAdjust(pf.FaceNormal.Z, -9) == -_CmpGeometry.ToHalfAdjust(view.ViewDirection.Z, -9))
            {
              orgFace = pf;
            }

            // ビューと同じ向きの面
            if (_CmpGeometry.ToHalfAdjust(pf.FaceNormal.Z, -9) == _CmpGeometry.ToHalfAdjust(view.ViewDirection.Z, -9))
            {
              topFace = pf;
            }
          }
        }


        foreach (Revit.DB.Face face in allFaces)
        {
          try
          {
            // 面と交差または接する場合、関連あり
            // 面のどちらか側にある場合、関連なし
            bool relationFace = _CmpGeometry.CheckRelationFace(topFace,
                                                               face as Revit.DB.PlanarFace);

            if (relationFace)
            {
              Revit.DB.Curve intersectCrv = null;

              // 面と面の交差カーブ
              if (orgFace.Intersect(face, out intersectCrv) == Revit.DB.FaceIntersectionFaceResult.Intersecting)
              {
                if (intersectCrv.Length < 1000000)
                {
                  if (intersectCrv.IsCyclic == false)
                  {
                    bool b = false;

                    foreach (Revit.DB.Line l in ret)
                    {
                      if (_CmpGeometry.Distance2D(l.GetEndPoint(0), intersectCrv.GetEndPoint(0)) < _CmpGeometry.Approx0Len &&
                          _CmpGeometry.Distance2D(l.GetEndPoint(1), intersectCrv.GetEndPoint(1)) < _CmpGeometry.Approx0Len)
                      {
                        b = true;
                        break;
                      }
                      if (_CmpGeometry.Distance2D(l.GetEndPoint(0), intersectCrv.GetEndPoint(1)) < _CmpGeometry.Approx0Len &&
                          _CmpGeometry.Distance2D(l.GetEndPoint(1), intersectCrv.GetEndPoint(0)) < _CmpGeometry.Approx0Len)
                      {
                        b = true;
                        break;
                      }
                    }

                    if (b == false)
                    {
                      ret.Add(intersectCrv);
                    }
                  }
                  else
                  {
                    Collections.Generic.IList<Revit.DB.Line> convertLines = _CmpGeometry.CurveConvertToLine(intersectCrv);

                    foreach (Revit.DB.Line line in convertLines)
                    {
                      ret.Add(line);
                    }
                  }
                }
              }
            }

            //if (relationFace == false)
            {
              Collections.Generic.IList<Revit.DB.CurveLoop> loops = face.GetEdgesAsCurveLoops();

              foreach (Revit.DB.CurveLoop loop in loops)
              {
                Collections.Generic.IList<Revit.DB.Curve> crvs = new Collections.Generic.List<Revit.DB.Curve>();
                foreach (Revit.DB.Curve curve in loop)
                {
                  crvs.Add(curve);
                }

                crvs = _CmpGeometry.OptimizeLineVertexConvLine(crvs);

                foreach (Revit.DB.Curve curve in crvs)
                {
                  // ビュー範囲内線分
                  Revit.DB.Line line = _CmpGeometry.GetLineInHeight(curve, null, cutLevel, btmOffset, topOffset);

                  if (line != null)
                  {
                    // 面への投影線分
                    Revit.DB.Line shadowLine = _CmpGeometry.GetShadowLine(orgFace, line);

                    if (shadowLine != null)
                    {
                      bool b = false;

                      foreach (Revit.DB.Line l in ret)
                      {
                        if (_CmpGeometry.Distance2D(l.GetEndPoint(0), shadowLine.GetEndPoint(0)) < _CmpGeometry.Approx0Len &&
                            _CmpGeometry.Distance2D(l.GetEndPoint(1), shadowLine.GetEndPoint(1)) < _CmpGeometry.Approx0Len)
                        {
                          b = true;
                          break;
                        }
                        if (_CmpGeometry.Distance2D(l.GetEndPoint(0), shadowLine.GetEndPoint(1)) < _CmpGeometry.Approx0Len &&
                            _CmpGeometry.Distance2D(l.GetEndPoint(1), shadowLine.GetEndPoint(0)) < _CmpGeometry.Approx0Len)
                        {
                          b = true;
                          break;
                        }
                      }

                      if (b == false)
                      {
                        ret.Add(shadowLine);
                      }
                    }
                  }
                }
              }
            }
          }
          catch
          {
            continue;
          }
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
