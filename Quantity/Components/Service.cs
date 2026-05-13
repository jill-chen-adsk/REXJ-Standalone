using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using AdWindows   = Autodesk.Windows;
using System.Text;

namespace Quantity.Components
{
  /// ================================================================================
  /// <summary>サービス</summary>
  /// ================================================================================
  class Service
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private Quantity.Components.Attribute _CmpAttribute;

    /// <summary>要素</summary>
    private Quantity.Components.Elements _CmpElements;

    /// <summary>図形</summary>
    private Quantity.Components.Geometry _CmpGeometry;

    /// <summary>パラメータ</summary>
    private Quantity.Components.Parameters _CmpParameters;

    /// <summary>設定</summary>
    private Quantity.Components.Settings _CmpSettings;

    /// <summary>データテーブル コマンド</summary>
    private Quantity.Entities.DtCmd _EntDtCmd;

    /// <summary>書き出しフォルダパス</summary>
    private string _ExportFolderPath;

    #endregion

    // コンストラクタ
    #region Constructor
    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    /// 
    /// <param name="cmpElements"   >要素</param>
    /// <param name="cmpGeometry"   >図形</param>
    /// <param name="cmpParameters" >パラメータ</param>
    /// <param name="cmpSettings"   >設定</param>
    /// <param name="cmpAttribute"  >属性</param>
    /// 
    /// <history><p>2015/11/26 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2017/07/18 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public Service(Quantity.Components.Elements cmpElements,
                   Quantity.Components.Geometry cmpGeometry,
                   Quantity.Components.Parameters cmpParameters,
                   Quantity.Components.Settings cmpSettings,
                   Quantity.Components.Attribute cmpAttribute)
    {
      _CmpElements = cmpElements;
      _CmpGeometry = cmpGeometry;
      _CmpParameters = cmpParameters;
      _CmpSettings = cmpSettings;
      _CmpAttribute = cmpAttribute;

      //// プロジェクト名
      //_ProjName = _CmpElements.ProjectName;

      //// Revitファイルフォルダ
      //string pathName = _CmpElements.RvtDBDoc.PathName;
      //if (string.IsNullOrWhiteSpace(pathName) == false)
      //{
      //  _RvtFileFolder = System.IO.Path.GetDirectoryName(pathName);
      //}

      //// XMLファイルフォルダ
      //_XMLFileFolder = _RvtFileFolder + "\\" + _CmpAttribute.ResourceText("IDS_TXT_XMLFILEFOLDER");

      //if (System.IO.Directory.Exists(_XMLFileFolder))
      //{
      //  // ファイル
      //  _Files = System.IO.Directory.GetFiles(_XMLFileFolder);
      //}

      //_LostChildDuctIds = new Collections.Generic.List<int>();
      //_LostChildPipeIds = new Collections.Generic.List<int>();
    }
    #endregion

    // メンバ関数
    #region

    /// ================================================================================
    /// <summary>ワークフロー</summary>
    /// 
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    string WorkFlow()
    {
      string ret = null;

      // プロジェクト情報
      Revit.DB.ProjectInfo projInfo = _CmpElements.ProjectInfo;

      _EntDtCmd = new Quantity.Entities.DtCmd(_CmpAttribute,
                                               _CmpElements,
                                               _CmpGeometry,
                                               _CmpParameters,
                                               _CmpSettings,
                                               projInfo,
                                               _CmpAttribute.ResourceText("IDS_SHPARAM_DEF"),
                                               4);

      return ret;
    }

    /// ================================================================================
    /// <summary>タイプ取得</summary>
    /// 
    /// <param name="dimType"></param>
    /// <param name="textType"></param>
    /// 
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void GetTypes(ref Revit.DB.DimensionType dimType,
                  ref Revit.DB.TextNoteType textType)
    {
      string dimTypeName = "";
      string dimTypeId = "";
      string textTypeName = "";
      string textTypeId = "";

      string tabName = _CmpAttribute.ResourceText("IDS_BTN_TABNAME");
      string pnlName = _CmpAttribute.ResourceText("IDS_BTN_PANELNAME");

      // Revit 内部ID
      string internalID_DimTypeCmbBox = _CmpAttribute.ResourceText("IDS_RVT_INTERNALID_HEAD") + "%" +
                                        tabName + "%" +
                                        _CmpAttribute.ResourceText("IDS_BTN_PANELNAME") + "%" +
                                        _CmpAttribute.ResourceText("IDS_BTN_DIMTYPE_NAME");

      string internalID_TextTypeCmbBox = _CmpAttribute.ResourceText("IDS_RVT_INTERNALID_HEAD") + "%" +
                                         tabName + "%" +
                                         _CmpAttribute.ResourceText("IDS_BTN_PANELNAME") + "%" +
                                         _CmpAttribute.ResourceText("IDS_BTN_TEXTTYPE_NAME");

      // コンボボックス - 寸法タイプ
      AdWindows.RibbonCombo cmbDimTypes = null;
      // コンボボックス - 文字タイプ
      AdWindows.RibbonCombo cmbTextTypes = null;

      // リボンアイテム取得
      #region リボンアイテム取得

      // リボン
      AdWindows.RibbonControl rbnCtrl = UIFramework.RevitRibbonControl.RibbonControl;

      // タブ
      AdWindows.RibbonTabCollection rbnTabCollection = rbnCtrl.Tabs;

      foreach (AdWindows.RibbonTab rbnTab in rbnTabCollection)
      {
        if (rbnTab.AutomationName == tabName)
        {
          // パネル
          AdWindows.RibbonPanelCollection rbnPnlCollection = rbnTab.Panels;

          foreach (AdWindows.RibbonPanel rbnPnl in rbnPnlCollection)
          {
            if (rbnPnl.Source.AutomationName == pnlName)
            {
              // 寸法タイプ
              AdWindows.RibbonItem item = rbnPnl.Source.FindItem(internalID_DimTypeCmbBox, true);

              if (item != null)
              {
                cmbDimTypes = item as AdWindows.RibbonCombo;
              }

              // 文字タイプ
              item = rbnPnl.Source.FindItem(internalID_TextTypeCmbBox, true);

              if (item != null)
              {
                cmbTextTypes = item as AdWindows.RibbonCombo;
              }
            }
          }
        }
      }

      #endregion

      if (cmbDimTypes != null)
      {
        Revit.UI.ComboBoxMemberData memberData = cmbDimTypes.Current as Revit.UI.ComboBoxMemberData;

        dimTypeName = memberData.Text;
        dimTypeId = memberData.Name;
      }

      if (cmbTextTypes != null)
      {
        Revit.UI.ComboBoxMemberData memberData = cmbTextTypes.Current as Revit.UI.ComboBoxMemberData;

        textTypeName = memberData.Text;
        textTypeId = memberData.Name;
      }

      Revit.DB.FilteredElementCollector fecDimType = new Revit.DB.FilteredElementCollector(_CmpElements.RvtDBDoc);
      fecDimType.OfClass(typeof(Revit.DB.DimensionType));
      fecDimType.WhereElementIsElementType();

      foreach (Revit.DB.DimensionType dt in fecDimType)
      {
        if (dt.Name == dimTypeName &&
            dt.Id.ToString() == dimTypeId)
        {
          dimType = dt;
          break;
        }
      }

      Revit.DB.FilteredElementCollector fecTextType = new Revit.DB.FilteredElementCollector(_CmpElements.RvtDBDoc);
      fecTextType.OfClass(typeof(Revit.DB.TextNoteType));
      fecTextType.WhereElementIsElementType();

      foreach (Revit.DB.TextNoteType tt in fecTextType)
      {
        if (tt.Name == textTypeName &&
            tt.Id.ToString() == textTypeId)
        {
          textType = tt;
          break;
        }
      }
    }

    /// ================================================================================
    /// <summary>詳細線分グラフィックススタイル取得</summary>
    /// 
    /// <param name="graphicsStyle" >グラフィックススタイル</param>
    /// <param name="viewPlan"      >平面ビュー</param>
    /// <param name="transaction"   >トランザクション</param>
    /// 
    /// <history>2017/09/29 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void GetDetailCurveGraphicsStyle(ref Revit.DB.GraphicsStyle graphicsStyle,
                                     Revit.DB.ViewPlan viewPlan,
                                     Revit.DB.Transaction transaction)
    {
      double z = viewPlan.GenLevel.Elevation;

      // 仮線分
      Revit.DB.Line dummyLine = Revit.DB.Line.CreateBound(new Revit.DB.XYZ(0, 0, z), new Revit.DB.XYZ(1, 0, z));

      transaction.Start("Create");

      Revit.DB.DetailCurve dc = _CmpElements.RvtDBDoc.Create.NewDetailCurve(viewPlan, dummyLine);

      transaction.Commit();

      // すべてのスタイル
      Collections.Generic.ICollection<Revit.DB.ElementId> graphicsStyleIds = dc.GetLineStyleIds();

      foreach (Revit.DB.ElementId graphicsId in graphicsStyleIds)
      {
        Revit.DB.GraphicsStyle gs = _CmpElements.RvtDBDoc.GetElement(graphicsId) as Revit.DB.GraphicsStyle;

        Revit.DB.Category category = gs.GraphicsStyleCategory;

        if (category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_CurvesThinLines).ToString()))
        {
          graphicsStyle = gs;
          break;
        }
      }

      transaction.Start("Delete");

      _CmpElements.RvtDBDoc.Delete(dc.Id);

      transaction.Commit();
    }

    /// ================================================================================
    /// <summary>設定値保存</summary>
    /// 
    /// <history>2017/07/19 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void Set(Revit.DB.DimensionType dimType,
             Revit.DB.TextNoteType textType)
    {
      if (dimType != null)
      {
        _EntDtCmd.DimTypeName = dimType.Name;
        _EntDtCmd.DimTypeId = dimType.Id.ToString();
      }
      else
      {
        _EntDtCmd.DimTypeName = "";
        _EntDtCmd.DimTypeId = "";
      }

      if (textType != null)
      {
        _EntDtCmd.TextTypeName = textType.Name;
        _EntDtCmd.TextTypeId = textType.Id.ToString();
      }
      else
      {
        _EntDtCmd.TextTypeName = "";
        _EntDtCmd.TextTypeId = "";
      }

      _EntDtCmd.SetData();
    }

    /// ================================================================================
    /// <summary>Excel書き出しフォルダ選択</summary>
    /// 
    /// <history>2015/11/30 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool GetExportFolderPath()
    {
      bool ret = false;

      // フォルダ選択
      System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();
      folderDlg.ShowNewFolderButton = true;

      if (folderDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        _ExportFolderPath = folderDlg.SelectedPath;

        // 確認
        // OK
        if (System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_STARTEXPORT"),
                                                 _CmpAttribute.ResourceText("IDS_TXT_QUANTITYEXPORT"),
                                                 System.Windows.Forms.MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
        {
          ret = true;
        }
        // キャンセル
        else
        {
          System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_CANCEL"));

          ret = false;
        }
      }
      // キャンセル
      else
      {
        System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_CANCEL"));

        ret = false;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>平面ビュー内スペース</summary>
    /// 
    /// <param name="viewPlan">平面ビュー</param>
    /// 
    /// <history>2014/10/31 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Space> InViewPlanSpaceAry(Revit.DB.ViewPlan viewPlan)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Mechanical.Space> ret = new Collections.Generic.List<Revit.DB.Mechanical.Space>();

      Collections.Generic.IList<Revit.DB.Mechanical.Space> allSpace = _CmpElements.AllSpaceAry;

      _CmpParameters.GetSpaceElev(allSpace);

      // ビュー範囲
      _CmpParameters.GetViewPlanRange(viewPlan);

      // 上下高さ
      double viewTop = _CmpParameters.ViewRangeTopElevation;
      double viewBtm = _CmpParameters.ViewRangeBottomElevation;


      // クロップボックス
      Revit.DB.ViewCropRegionShapeManager viewCropMgr = viewPlan.GetCropRegionShapeManager();

      // トリミング
      if (viewCropMgr.ShapeSet)
      {
        Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = viewCropMgr.GetCropShape();

        Collections.Generic.IList<Revit.DB.Curve> crvCrop = new Collections.Generic.List<Revit.DB.Curve>();

        foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
        {
          foreach (Revit.DB.Curve crv in crvLoop)
          {
            crvCrop.Add(crv);
          }
        }

        // 多角形の重心
        Revit.DB.XYZ gra2d = _CmpGeometry.PolygonGravity2D(crvCrop);

        foreach (Revit.DB.Mechanical.Space space in allSpace)
        {
          // 高さ範囲確認
          #region  高さ範囲確認

          double spaceTop = _CmpParameters.GetSpaceTopElev(space);
          double spaceBtm = _CmpParameters.GetSpaceBtmElev(space);

          bool isHeight = false;

          // ビュー高さ範囲内か
          if ((viewBtm <= spaceBtm && spaceBtm < viewTop) || (viewBtm < spaceTop && spaceTop <= viewTop))
          {
            isHeight = true;
          }
          // スペース高さ範囲内か
          else if (spaceBtm <= viewBtm && viewTop <= spaceTop)
          {
            isHeight = true;
          }

          if (isHeight == false)
          {
            continue;
          }

          #endregion

          Revit.DB.Location loc = space.Location;
          Revit.DB.LocationPoint locPnt = loc as Revit.DB.LocationPoint;
          Revit.DB.XYZ pnt = locPnt.Point;

          // スペースと重心を結ぶ線
          Revit.DB.Line lineSpaceGra = Revit.DB.Line.CreateBound(gra2d, pnt);

          // スペース境界線
          Collections.Generic.IList<Revit.DB.Curve> bndryCrvs = _CmpGeometry.GetSpaceBndryCrv(space, 1);

          // 
          bool isCross = false;

          foreach (Revit.DB.Curve crv1 in crvCrop)
          {
            Revit.DB.Line line = crv1 as Revit.DB.Line;

            foreach (Revit.DB.Curve crv2 in bndryCrvs)
            {
              if (crv2.IsCyclic)
              {
                Revit.DB.Arc bndryArc = crv2 as Revit.DB.Arc;

                Collections.Generic.IList<Revit.DB.XYZ> crossPnts = _CmpGeometry.GetXYCrossPoint(line, bndryArc);

                if (crossPnts.Count > 0)
                {
                  isCross = true;

                  break;
                }
              }
              else
              {
                Revit.DB.Line bndryLine = crv2 as Revit.DB.Line;

                Revit.DB.XYZ crossPnt = _CmpGeometry.CrossPointXY(line, bndryLine, 0);

                if (crossPnt != null)
                {
                  isCross = true;

                  break;
                }
              }
            }

            if (isCross)
            {
              break;
            }
          }

          if (isCross)
          {
            ret.Add(space);
          }
          else
          {
            // 完全内部

            foreach (Revit.DB.Curve crv in crvCrop)
            {
              Revit.DB.Line line = crv as Revit.DB.Line;

              Revit.DB.XYZ crossPnt = _CmpGeometry.CrossPointXY(line, lineSpaceGra, 0);

              if (crossPnt != null)
              {
                isCross = true;

                break;
              }
            }

            if (isCross == false)
            {
              ret.Add(space);
            }
          }

        }
      }
      else
      {
        Revit.DB.BoundingBoxXYZ bbXYZ = viewPlan.CropBox;

        Revit.DB.XYZ pntTopRight = bbXYZ.Max;
        Revit.DB.XYZ pntBtmLeft = bbXYZ.Min;
        Revit.DB.XYZ pntTopLeft = new Revit.DB.XYZ(pntBtmLeft.X, pntTopRight.Y, pntTopRight.Z);
        Revit.DB.XYZ pntBtmRight = new Revit.DB.XYZ(pntTopRight.X, pntBtmLeft.Y, pntTopRight.Z);

        Revit.DB.Line l1 = Revit.DB.Line.CreateBound(pntTopLeft, pntBtmLeft);
        Revit.DB.Line l2 = Revit.DB.Line.CreateBound(pntBtmLeft, pntBtmRight);
        Revit.DB.Line l3 = Revit.DB.Line.CreateBound(pntBtmRight, pntTopRight);
        Revit.DB.Line l4 = Revit.DB.Line.CreateBound(pntTopRight, pntTopLeft);

        foreach (Revit.DB.Mechanical.Space space in allSpace)
        {
          double spaceTop = _CmpParameters.GetSpaceTopElev(space);
          double spaceBtm = _CmpParameters.GetSpaceBtmElev(space);

          bool isHeight = false;

          // ビュー高さ範囲内か
          if ((viewBtm <= spaceBtm && spaceBtm < viewTop) || (viewBtm < spaceTop && spaceTop <= viewTop))
          {
            isHeight = true;
          }
          // スペース高さ範囲内か
          else if (spaceBtm <= viewBtm && viewTop <= spaceTop)
          {
            isHeight = true;
          }

          if (isHeight == false)
          {
            continue;
          }


          // 境界線
          Collections.Generic.IList<Revit.DB.Curve> bndryCrvs = _CmpGeometry.GetSpaceBndryCrv(space, 1);

          // 完全範囲内か

          // 中心点
          Revit.DB.XYZ center = _CmpGeometry.Center2Point(pntBtmLeft, pntTopRight);

          Revit.DB.Location loc = space.Location;
          Revit.DB.LocationPoint locPnt = loc as Revit.DB.LocationPoint;
          Revit.DB.XYZ pnt = locPnt.Point;

          Revit.DB.Line _l = Revit.DB.Line.CreateBound(center, pnt);

          // 交点なし
          if (_CmpGeometry.CrossPointXY(_l, l1, 0) == null &&
              _CmpGeometry.CrossPointXY(_l, l2, 0) == null &&
              _CmpGeometry.CrossPointXY(_l, l3, 0) == null &&
              _CmpGeometry.CrossPointXY(_l, l4, 0) == null)
          {
            ret.Add(space);
          }
          else
          {
            // 交差判定
            bool isCross = false;

            foreach (Revit.DB.Curve crv in bndryCrvs)
            {
              // 直線
              if (crv.IsCyclic == false)
              {
                Revit.DB.Line l = crv as Revit.DB.Line;

                if (_CmpGeometry.CrossPointXY(l, l1, 0) != null)
                {
                  isCross = true;
                  break;
                }
                if (_CmpGeometry.CrossPointXY(l, l2, 0) != null)
                {
                  isCross = true;
                  break;
                }
                if (_CmpGeometry.CrossPointXY(l, l3, 0) != null)
                {
                  isCross = true;
                  break;
                }
                if (_CmpGeometry.CrossPointXY(l, l4, 0) != null)
                {
                  isCross = true;
                  break;
                }
              }
              else
              {
                Revit.DB.Arc arc = crv as Revit.DB.Arc;

                if (_CmpGeometry.GetXYCrossPoint(l1, arc) != null)
                {
                  isCross = true;
                  break;
                }
                if (_CmpGeometry.GetXYCrossPoint(l2, arc) != null)
                {
                  isCross = true;
                  break;
                }
                if (_CmpGeometry.GetXYCrossPoint(l3, arc) != null)
                {
                  isCross = true;
                  break;
                }
                if (_CmpGeometry.GetXYCrossPoint(l4, arc) != null)
                {
                  isCross = true;
                  break;
                }
              }

            }

            if (isCross)
            {
              ret.Add(space);
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>指定ビューの配管</summary>
    /// 
    /// <param name="view"    >ビュー</param>
    /// <param name="rvtDbDoc">ドキュメント</param>
    /// 
    /// <history>2014/11/06 Created GSA, Inc, Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Plumbing.Pipe> InViewPipe(Revit.DB.View view,
                                                                 Revit.DB.Document rvtDbDoc)
    {
      Collections.Generic.IList<Revit.DB.Plumbing.Pipe> ret = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();

      Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(rvtDbDoc, view.Id);
      fec.OfCategory(Revit.DB.BuiltInCategory.OST_PipeCurves);
      fec.WhereElementIsNotElementType();

      // クロップボックス
      Revit.DB.ViewCropRegionShapeManager viewCropMgr = view.GetCropRegionShapeManager();
      if (viewCropMgr.ShapeSet)
      {
        Revit.DB.CurveLoop crvLoop = new Revit.DB.CurveLoop();
        Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = viewCropMgr.GetCropShape();

        foreach (Revit.DB.CurveLoop cl in crvLoops)
        {
          foreach (Revit.DB.Curve crv in cl)
          {
            crvLoop.Append(crv);
          }
        }

        foreach (Revit.DB.Plumbing.Pipe pipe in fec)
        {
          bool isCross = false;

          foreach (Revit.DB.Curve crv in crvLoop)
          {
            Revit.DB.Line line = crv as Revit.DB.Line;

            Revit.DB.Line l = _CmpGeometry.GetPipeLine(pipe);

            // XY交点
            Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(line, l, 1);

            if (cross != null)
            {
              isCross = true;
              break;
            }
          }

          // 交差あり
          if (isCross)
          {
            ret.Add(pipe);
          }
          else
          {
            // 完全内部判定

            Revit.DB.XYZ center = null;
            int count = 0;

            foreach (Revit.DB.Curve crv in crvLoop)
            {
              Revit.DB.XYZ p0 = crv.GetEndPoint(0);
              Revit.DB.XYZ p1 = crv.GetEndPoint(1);

              if (center == null)
              {
                center = p0;
              }
              else
              {
                center += p0;
              }

              center += p1;

              count += 2;
            }

            center = center / count;

            Revit.DB.Line line = _CmpGeometry.GetPipeLine(pipe);
            Revit.DB.XYZ ep0 = line.GetEndPoint(0);

            Revit.DB.Line l0 = Revit.DB.Line.CreateBound(center, ep0);

            foreach (Revit.DB.Curve crv in crvLoop)
            {
              Revit.DB.Line crvLine = crv as Revit.DB.Line;

              // XY交点
              Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(crvLine, l0, 0);

              if (cross != null)
              {
                isCross = true;
                break;
              }
            }

            // 交差なし
            if (isCross == false)
            {
              ret.Add(pipe);
            }
          }
        }
      }
      else
      {
        foreach (Revit.DB.Plumbing.Pipe pipe in fec)
        {
          ret.Add(pipe);
        }

        return ret;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>指定ビューのフレキシブルダクト</summary>
    /// 
    /// <param name="view"    >ビュー</param>
    /// <param name="rvtDbDoc">ドキュメント</param>
    /// 
    /// <history>2015/12/18 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.FlexDuct> InViewFlexDuct(Revit.DB.View view, Revit.DB.Document rvtDbDoc)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Mechanical.FlexDuct> ret = new Collections.Generic.List<Revit.DB.Mechanical.FlexDuct>();

      Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(rvtDbDoc, view.Id);
      fec.OfCategory(Revit.DB.BuiltInCategory.OST_FlexDuctCurves);
      fec.WhereElementIsNotElementType();

      // クロップボックス
      Revit.DB.ViewCropRegionShapeManager viewCropMgr = view.GetCropRegionShapeManager();

      if (viewCropMgr.ShapeSet)
      {
        Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = viewCropMgr.GetCropShape();

        foreach (Revit.DB.Mechanical.FlexDuct flexDuct in fec)
        {
          bool isCross = false;

          Revit.DB.LocationCurve locCrv = flexDuct.Location as Revit.DB.LocationCurve;

          Revit.DB.HermiteSpline herSpline = locCrv.Curve as Revit.DB.HermiteSpline;

          Collections.Generic.IList<Revit.DB.XYZ> pnts = herSpline.Tessellate();

          foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
          {
            foreach (Revit.DB.Curve crv in crvLoop)
            {
              Revit.DB.Line line = crv as Revit.DB.Line;

              for (int i = 0; i < pnts.Count - 1; ++i)
              {
                Revit.DB.XYZ p0 = pnts[0];
                Revit.DB.XYZ p1 = pnts[i + 1];

                Revit.DB.Line l = Revit.DB.Line.CreateBound(p0, p1);

                Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(line, l, 1);

                if (cross != null)
                {
                  isCross = true;
                  break;
                }
              }

              if (isCross)
              {
                break;
              }
            }

            if (isCross)
            {
              break;
            }
          }

          // 交差あり
          if (isCross)
          {
            ret.Add(flexDuct);
          }
          else
          {
            // 完全内部判定

            // 内部点
            Revit.DB.XYZ center = null;
            int count = 0;

            foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
            {
              foreach (Revit.DB.Curve crv in crvLoop)
              {
                Revit.DB.XYZ p0 = crv.GetEndPoint(0);
                Revit.DB.XYZ p1 = crv.GetEndPoint(1);

                if (center == null)
                {
                  center = p0;
                }
                else
                {
                  center += p0;
                }

                center += p1;

                count += 2;
              }
            }

            center = center / count;

            for (int i = 0; i < pnts.Count - 1; ++i)
            {
              Revit.DB.XYZ ep0 = pnts[i];

              Revit.DB.Line l = Revit.DB.Line.CreateBound(center, ep0);

              foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
              {
                foreach (Revit.DB.Curve crv in crvLoop)
                {
                  Revit.DB.Line crvLine = crv as Revit.DB.Line;

                  Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(crvLine, l, 0);

                  if (cross != null)
                  {
                    isCross = true;
                    break;
                  }
                }

                if (isCross)
                {
                  break;
                }
              }

              if (isCross)
              {
                break;
              }
            }

            // 交差なし
            if (isCross == false)
            {
              ret.Add(flexDuct);
            }
          }
        }
      }
      else
      {
        foreach (Revit.DB.Mechanical.FlexDuct flexDuct in fec)
        {
          ret.Add(flexDuct);
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>指定ビューのダクト</summary>
    /// 
    /// <param name="view"    >ビュー</param>
    /// <param name="rvtDbDoc">ドキュメント</param>
    /// 
    /// <history>2014/10/03 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Duct> InViewDuct(Revit.DB.View view,
                                                                   Revit.DB.Document rvtDbDoc)
    {
      Collections.Generic.IList<Revit.DB.Mechanical.Duct> ret = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();

      Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(rvtDbDoc, view.Id);
      fec.OfCategory(Revit.DB.BuiltInCategory.OST_DuctCurves);
      fec.WhereElementIsNotElementType();

      // クロップボックス
      Revit.DB.ViewCropRegionShapeManager viewCropMgr = view.GetCropRegionShapeManager();
      if (viewCropMgr.ShapeSet)
      {
        Collections.Generic.IList<Revit.DB.CurveLoop> crvLoops = viewCropMgr.GetCropShape();

        foreach (Revit.DB.Mechanical.Duct duct in fec)
        {
          bool isCross = false;

          foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
          {
            foreach (Revit.DB.Curve crv in crvLoop)
            {
              Revit.DB.Line line = crv as Revit.DB.Line;

              Revit.DB.Line l = _CmpGeometry.GetDuctLine(duct);

              // XY交点
              Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(line, l, 1);

              if (cross != null)
              {
                isCross = true;
                break;
              }
            }

            if (isCross)
            {
              break;
            }
          }

          // 交差あり
          if (isCross)
          {
            ret.Add(duct);
          }
          else
          {
            // 完全内部判定

            Revit.DB.XYZ center = null;
            int count = 0;

            foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
            {
              foreach (Revit.DB.Curve crv in crvLoop)
              {
                Revit.DB.XYZ p0 = crv.GetEndPoint(0);
                Revit.DB.XYZ p1 = crv.GetEndPoint(1);

                if (center == null)
                {
                  center = p0;
                }
                else
                {
                  center += p0;
                }

                center += p1;

                count += 2;
              }
            }

            center = center / count;

            Revit.DB.Line line = _CmpGeometry.GetDuctLine(duct);
            Revit.DB.XYZ ep0 = line.GetEndPoint(0);

            Revit.DB.Line l0 = Revit.DB.Line.CreateBound(center, ep0);

            foreach (Revit.DB.CurveLoop crvLoop in crvLoops)
            {
              foreach (Revit.DB.Curve crv in crvLoop)
              {
                Revit.DB.Line crvLine = crv as Revit.DB.Line;

                // XY交点
                Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(crvLine, l0, 0);

                if (cross != null)
                {
                  isCross = true;
                  break;
                }
              }

              if (isCross)
              {
                break;
              }
            }

            // 交差なし
            if (isCross == false)
            {
              ret.Add(duct);
            }
          }
        }
      }
      else
      {
        foreach (Revit.DB.Mechanical.Duct duct in fec)
        {
          ret.Add(duct);
        }

        return ret;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>配管または継手</summary>
    /// 
    /// <param name="elem">要素</param>
    /// 
    /// <history>2014/09/09 Created GAS, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsPipeOrFitting(Revit.DB.Element elem)
    {
      bool ret = true;

      if (elem.Category == null)
      {
        ret = false;
      }
      // 機械設備、衛生器具、配管システム
      else if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipingSystem).ToString()) ||
               elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_MechanicalEquipment).ToString()) ||
               elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PlumbingFixtures).ToString()))
      {
        ret = false;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ダクトまたは継手</summary>
    /// 
    /// <param name="elem">要素</param>
    /// 
    /// <history>2014/09/09 Created GAS, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsDuctOrFitting(Revit.DB.Element elem)
    {
      bool ret = true;

      if (elem.Category == null)
      {
        ret = false;
      }
      // 機械設備、衛生器具、配管システム
      else if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipingSystem).ToString()) ||
               elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_MechanicalEquipment).ToString()) ||
               elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PlumbingFixtures).ToString()))
      {
        ret = false;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>コネクタ先取得(継手や付属品)</summary>
    /// 
    /// <param name="pipe"      >配管</param>
    /// <param name="connector1">コネクタ1</param>
    /// <param name="connector2">コネクタ2</param>
    /// 
    /// <history>2014/10/28 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    void GetConnectorOwner(Revit.DB.Plumbing.Pipe pipe,
                           ref Revit.DB.Element connector1,
                           ref Revit.DB.Element connector2)
    {
      // 線分
      Revit.DB.Line line = _CmpGeometry.GetPipeLine(pipe);

      // 基準配管との交点
      Collections.Generic.IDictionary<Revit.DB.XYZ, Revit.DB.Element> dicXYZElem = new Collections.Generic.Dictionary<Revit.DB.XYZ, Revit.DB.Element>();

      Revit.DB.ConnectorManager mgr = pipe.ConnectorManager;

      // 「配管」に接続する要素
      Revit.DB.ConnectorSet set = mgr.Connectors;

      foreach (Revit.DB.Connector cnct in set)
      {
        // 「継手」に接続する要素
        Revit.DB.ConnectorSet cs = cnct.AllRefs;

        //// 継手の分岐数
        //if (cs.Size > 2)
        //{
        //  continue;
        //}

        foreach (Revit.DB.Connector c in cs)
        {
          // 継手か付属品
          if ((c.Owner.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeFitting).ToString()) ||
               c.Owner.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeAccessory).ToString())))
          {
            // 繋がっているダクト
            Collections.Generic.IList<Revit.DB.Plumbing.Pipe> cnctPipes = _CmpElements.GetSameConnectorPipe(c.Owner);

            // 基準配管のみ
            if (cnctPipes.Count == 1)
            {
              Revit.DB.LocationPoint lp = c.Owner.Location as Revit.DB.LocationPoint;

              dicXYZElem.Add(lp.Point, c.Owner);
            }
            // 両側に配管
            else if (cnctPipes.Count == 2)
            {
              foreach (Revit.DB.Plumbing.Pipe p in cnctPipes)
              {
                if (pipe.Id.ToString() == p.Id.ToString())
                {
                  continue;
                }

                Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

                Revit.DB.XYZ cross = _CmpGeometry.TwoLineCrossPnt(line, l);

                if (_CmpGeometry.IsVerticalSinglePipe(p))
                {
                  cross = new Revit.DB.XYZ(l.GetEndPoint(0).X, l.GetEndPoint(0).Y, cross.Z);
                }

                if (cross != null)
                {
                  dicXYZElem.Add(cross, c.Owner);
                }
              }
            }
            // 3つ以上
            else if (cnctPipes.Count > 2)
            {
              Revit.DB.XYZ cross = null;

              foreach (Revit.DB.Plumbing.Pipe p in cnctPipes)
              {
                if (pipe.Id.ToString() == p.Id.ToString())
                {
                  continue;
                }

                Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

                // 角度あり
                if (_CmpGeometry.IsTwoLineParallelZeroDIstance_NoSlope(line, l) == false)
                {
                  cross = _CmpGeometry.TwoLineCrossPnt(line, l);
                }
              }

              if (cross == null)
              {
                // 縦管
                foreach (Revit.DB.Plumbing.Pipe p in cnctPipes)
                {
                  if (pipe.Id.ToString() == p.Id.ToString())
                  {
                    continue;
                  }

                  Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

                  cross = _CmpGeometry.TwoLineCrossPnt(line, l);

                  if (cross == null)
                  {
                    continue;
                  }

                  // 基準が縦管
                  if (_CmpGeometry.IsVerticalSinglePipe(pipe))
                  {
                    cross = new Revit.DB.XYZ(l.GetEndPoint(0).X, l.GetEndPoint(0).Y, cross.Z);
                  }
                  else if (_CmpGeometry.IsVerticalSinglePipe(p))
                  {
                    cross = new Revit.DB.XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, cross.Z);
                  }
                }
              }


              if (cross != null)
              {
                dicXYZElem.Add(cross, c.Owner);
              }
            }
            else
            {
              continue;
            }
          }
        }
      }


      // 戻り
      if (dicXYZElem.Count <= 2)
      {
        foreach (Revit.DB.XYZ key in dicXYZElem.Keys)
        {
          if (connector1 == null)
          {
            connector1 = dicXYZElem[key];
          }
          else if (connector2 == null)
          {
            connector2 = dicXYZElem[key];
          }
        }
      }
      else
      {
        // 2014/10/28
        // ダクトの「横」に継手が付いて分岐している場合、基準となるダクトの端部以外にダクト同士の交点が3つ以上できる
        // 一番離れた2点？

        Collections.Generic.ICollection<Revit.DB.XYZ> pnts = dicXYZElem.Keys;

        double distance = 0;
        Revit.DB.XYZ p1 = null;
        Revit.DB.XYZ p2 = null;

        foreach (Revit.DB.XYZ pnt1 in pnts)
        {
          foreach (Revit.DB.XYZ pnt2 in pnts)
          {
            double dis = _CmpGeometry.Distance(pnt1, pnt2);

            if (distance < dis)
            {
              distance = dis;

              p1 = pnt1;
              p2 = pnt2;
            }
          }
        }

        if (p1 != null)
        {
          connector1 = dicXYZElem[p1];
        }
        if (p2 != null)
        {
          connector2 = dicXYZElem[p2];
        }
      }
    }

    /// ================================================================================
    /// <summary>コネクタ先取得(継手や付属品)</summary>
    /// 
    /// <param name="duct"      >ダクト</param>
    /// <param name="connector1">コネクタ1</param>
    /// <param name="connector2">コネクタ2</param>
    /// 
    /// <history><p>2014/10/28 Created GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    void GetConnectorOwner(Revit.DB.Mechanical.Duct duct,
                           ref Revit.DB.Element connector1,
                           ref Revit.DB.Element connector2)
    {
      // 線分
      Revit.DB.Line line = _CmpGeometry.GetDuctLine(duct);

      Revit.DB.XYZ lp0 = line.GetEndPoint(0);
      Revit.DB.XYZ lp1 = line.GetEndPoint(1);

      // 基準ダクトとの交点
      Collections.Generic.IDictionary<Revit.DB.XYZ, Revit.DB.Element> dicXYZElem = new Collections.Generic.Dictionary<Revit.DB.XYZ, Revit.DB.Element>();

      Revit.DB.ConnectorManager mgr = duct.ConnectorManager;

      Revit.DB.ConnectorSet set = mgr.Connectors;

      foreach (Revit.DB.Connector cnect in set)
      {
        // 継手に接続する要素
        Revit.DB.ConnectorSet cs = cnect.AllRefs;

        //// 継手の分岐数
        //int size = cs.Size;
        //if (size > 2)
        //{
        //  continue;
        //}

        foreach (Revit.DB.Connector c in cs)
        {
          // 継手か付属品
          if (c.Owner.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctFitting).ToString()) ||
               c.Owner.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctAccessory).ToString()))
          {
            // Connectorの先があるか
            // 先がない場合は?

            // 繋がっているダクト
            Collections.Generic.IList<Revit.DB.Mechanical.Duct> cnctDucts = _CmpElements.GetSameConnectorDuct(c.Owner);

            // 基準ダクトのみ
            if (cnctDucts.Count == 1)
            {
              Revit.DB.LocationPoint lp = c.Owner.Location as Revit.DB.LocationPoint;

              dicXYZElem.Add(lp.Point, c.Owner);
            }
            // 両側にダクト
            else if (cnctDucts.Count == 2)
            {
              foreach (Revit.DB.Mechanical.Duct d in cnctDucts)
              {
                if (duct.Id.ToString() == d.Id.ToString())
                {
                  continue;
                }

                Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

                Revit.DB.XYZ cross = _CmpGeometry.TwoLineCrossPnt(line, l);

                if (_CmpGeometry.IsVerticalSingleDuct(duct))
                {
                  cross = new Revit.DB.XYZ(l.GetEndPoint(0).X, l.GetEndPoint(0).Y, cross.Z);
                }

                if (cross != null)
                {
                  dicXYZElem.Add(cross, c.Owner);
                }
              }
            }
            // 3つ以上
            else if (cnctDucts.Count > 2)
            {
              Revit.DB.XYZ cross = null;

              foreach (Revit.DB.Mechanical.Duct d in cnctDucts)
              {
                if (duct.Id.ToString() == d.Id.ToString())
                {
                  continue;
                }

                Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

                // 角度あり
                if (_CmpGeometry.IsTwoLineParallelZeroDIstance_NoSlope(line, l) == false)
                {
                  cross = _CmpGeometry.TwoLineCrossPnt(line, l);
                }
              }

              if (cross == null)
              {
                // 縦管
                foreach (Revit.DB.Mechanical.Duct d in cnctDucts)
                {
                  if (duct.Id.ToString() == d.Id.ToString())
                  {
                    continue;
                  }

                  Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

                  cross = _CmpGeometry.TwoLineCrossPnt(line, l);

                  if (cross == null)
                  {
                    continue;
                  }

                  // 基準が縦管
                  if (_CmpGeometry.IsVerticalSingleDuct(duct))
                  {
                    cross = new Revit.DB.XYZ(l.GetEndPoint(0).X, l.GetEndPoint(0).Y, cross.Z);
                  }
                  else if (_CmpGeometry.IsVerticalSingleDuct(d))
                  {
                    cross = new Revit.DB.XYZ(line.GetEndPoint(0).X, line.GetEndPoint(0).Y, cross.Z);
                  }
                }
              }


              if (cross != null)
              {
                dicXYZElem.Add(cross, c.Owner);
              }
            }
            else
            {
              continue;
            }
          }
        }
      }


      // 戻り
      if (dicXYZElem.Count == 1)
      {
        foreach (Revit.DB.XYZ key in dicXYZElem.Keys)
        {
          // 近い方
          if (_CmpGeometry.Distance(lp0, key) <= _CmpGeometry.Distance(lp1, key))
          {
            connector1 = dicXYZElem[key];
          }
          else
          {
            connector2 = dicXYZElem[key];
          }
        }
      }
      else if (dicXYZElem.Count == 2)
      {
        Collections.Generic.IList<Revit.DB.XYZ> keyList = new Collections.Generic.List<Revit.DB.XYZ>();
        foreach (Revit.DB.XYZ key in dicXYZElem.Keys)
        {
          keyList.Add(key);
        }

        Revit.DB.XYZ key0 = keyList[0];
        Revit.DB.XYZ key1 = keyList[1];

        // 近い方
        if (_CmpGeometry.Distance(lp0, key0) <= _CmpGeometry.Distance(lp1, key0) &&
            _CmpGeometry.Distance(lp0, key1) <= _CmpGeometry.Distance(lp1, key1))
        {
          if (_CmpGeometry.Distance(lp1, key0) <= _CmpGeometry.Distance(lp1, key1))
          {
            connector1 = dicXYZElem[key1];
            connector2 = dicXYZElem[key0];
          }
          else
          {
            connector1 = dicXYZElem[key0];
            connector2 = dicXYZElem[key1];
          }
        }
        else if (_CmpGeometry.Distance(lp0, key0) <= _CmpGeometry.Distance(lp1, key0) &&
                 _CmpGeometry.Distance(lp0, key1) > _CmpGeometry.Distance(lp1, key1))
        {
          connector1 = dicXYZElem[key0];
          connector2 = dicXYZElem[key1];
        }
        else if (_CmpGeometry.Distance(lp0, key0) > _CmpGeometry.Distance(lp1, key0) &&
                 _CmpGeometry.Distance(lp0, key1) <= _CmpGeometry.Distance(lp1, key1))
        {
          connector1 = dicXYZElem[key1];
          connector2 = dicXYZElem[key0];
        }
        else
        {
          if (_CmpGeometry.Distance(lp0, key0) <= _CmpGeometry.Distance(lp0, key1))
          {
            connector1 = dicXYZElem[key0];
            connector2 = dicXYZElem[key1];
          }
          else
          {
            connector1 = dicXYZElem[key1];
            connector2 = dicXYZElem[key0];
          }
        }


        //foreach (Revit.DB.XYZ key in dicXYZElem.Keys)
        //{
        //  // 近い方
        //  if (_CmpGeometry.Distance(lp0, key) <= _CmpGeometry.Distance(lp1, key))
        //  {
        //    if (connector1 != null)
        //    {
        //      // 近い方
        //      if (_CmpGeometry.Distance(lp0, key0) <= _CmpGeometry.Distance(lp0, key1))
        //      {
        //        connector1 = dicXYZElem[key0];
        //        connector2 = dicXYZElem[key1];
        //      }
        //      else
        //      {
        //        connector1 = dicXYZElem[key1];
        //        connector2 = dicXYZElem[key0];
        //      }
        //    }
        //    else
        //    {
        //      connector1 = dicXYZElem[key];
        //    }
        //  }
        //  else
        //  {
        //    if (connector2 != null)
        //    {
        //      // 近い方
        //      if (_CmpGeometry.Distance(lp1, key0) <= _CmpGeometry.Distance(lp1, key1))
        //      {
        //        connector1 = dicXYZElem[key1];
        //        connector2 = dicXYZElem[key0];
        //      }
        //      else
        //      {
        //        connector1 = dicXYZElem[key0];
        //        connector2 = dicXYZElem[key1];
        //      }
        //    }
        //    else
        //    {
        //      connector2 = dicXYZElem[key];
        //    }
        //  }
        //}
      }
      else
      {
        // 2014/10/28
        // ダクトの「横」に継手が付いて分岐している場合、基準となるダクトの端部以外にダクト同士の交点が3つ以上できる
        // 一番離れた2点？

        Collections.Generic.ICollection<Revit.DB.XYZ> pnts = dicXYZElem.Keys;

        double distance = 0;
        Revit.DB.XYZ p1 = null;
        Revit.DB.XYZ p2 = null;

        foreach (Revit.DB.XYZ pnt1 in pnts)
        {
          foreach (Revit.DB.XYZ pnt2 in pnts)
          {
            double dis = _CmpGeometry.Distance(pnt1, pnt2);

            if (distance < dis)
            {
              distance = dis;

              p1 = pnt1;
              p2 = pnt2;
            }
          }
        }

        if (p1 != null)
        {
          connector1 = dicXYZElem[p1];
        }
        if (p2 != null)
        {
          connector2 = dicXYZElem[p2];
        }
      }
    }

    /// ================================================================================
    /// <summary>指定コネクタを持つ配管</summary>
    /// 
    /// <param name="connector" >コネクタ</param>
    /// <param name="pipeAry"   >配管</param>
    /// 
    /// <history><p>2015/01/21 Created GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Plumbing.Pipe> GetSameConnectorPipe(Revit.DB.Element connector)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Plumbing.Pipe> ret = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();

      Collections.Generic.IList<string> ids = new Collections.Generic.List<string>();

      // 継手につながっている要素(1回目)
      Collections.Generic.IList<Revit.DB.Element> cnctElemes = _CmpElements.GetConnectorConnectElems(connector);

      foreach (Revit.DB.Element elem in cnctElemes)
      {
        // 配管
        if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeCurves).ToString()))
        {
          Revit.DB.Plumbing.Pipe pipe = elem as Revit.DB.Plumbing.Pipe;

          if (ids.Contains(pipe.Id.ToString()) == false)
          {
            ret.Add(pipe);
            ids.Add(pipe.Id.ToString());
          }
        }
        // 継手につながっている要素(2回目)
        //else
        else if ((elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeFitting).ToString()) ||
                  elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeAccessory).ToString())))
        {
          Collections.Generic.IList<Revit.DB.Element> cncts = _CmpElements.GetConnectorConnectElems(elem);

          foreach (Revit.DB.Element el in cncts)
          {
            if (el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeCurves).ToString()))
            {
              Revit.DB.Plumbing.Pipe pipe = el as Revit.DB.Plumbing.Pipe;

              if (ids.Contains(pipe.Id.ToString()) == false)
              {
                ret.Add(pipe);
                ids.Add(pipe.Id.ToString());
              }
            }
            // 継手につながっている要素(3回目)
            //else
            else if ((el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeFitting).ToString()) ||
                      el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeAccessory).ToString())))
            {
              Collections.Generic.IList<Revit.DB.Element> cs = _CmpElements.GetConnectorConnectElems(el);

              foreach (Revit.DB.Element e in cs)
              {
                if (e.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeCurves).ToString()))
                {
                  Revit.DB.Plumbing.Pipe pipe = e as Revit.DB.Plumbing.Pipe;

                  if (ids.Contains(pipe.Id.ToString()) == false)
                  {
                    ret.Add(pipe);
                    ids.Add(pipe.Id.ToString());
                  }
                }
              }
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>指定コネクタを持つダクト</summary>
    /// 
    /// <param name="connector" >コネクタ</param>
    /// <param name="ductAry"   >配管</param>
    /// 
    /// <history><p>2015/01/21 Created GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Duct> GetSameConnectorDuct(Revit.DB.Element connector)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Mechanical.Duct> ret = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();

      // 継手につながっている要素(1回目)
      Collections.Generic.IList<Revit.DB.Element> cnctElemes = _CmpElements.GetConnectorConnectElems(connector);

      foreach (Revit.DB.Element elem in cnctElemes)
      {
        // 配管
        if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctCurves).ToString()))
        {
          Revit.DB.Mechanical.Duct duct = elem as Revit.DB.Mechanical.Duct;

          ret.Add(duct);
        }
        // 継手につながっている要素(2回目)
        //else
        else if ((elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctFitting).ToString()) ||
                  elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctAccessory).ToString())))
        {
          Collections.Generic.IList<Revit.DB.Element> cncts = _CmpElements.GetConnectorConnectElems(elem);

          foreach (Revit.DB.Element el in cncts)
          {
            if (el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctCurves).ToString()))
            {
              Revit.DB.Mechanical.Duct duct = el as Revit.DB.Mechanical.Duct;

              ret.Add(duct);
            }
            // 継手につながっている要素(3回目)
            //else
            else if ((el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctFitting).ToString()) ||
                      el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctAccessory).ToString())))
            {
              Collections.Generic.IList<Revit.DB.Element> cs = _CmpElements.GetConnectorConnectElems(el);

              foreach (Revit.DB.Element e in cs)
              {
                if (e.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctCurves).ToString()))
                {
                  Revit.DB.Mechanical.Duct duct = e as Revit.DB.Mechanical.Duct;

                  ret.Add(duct);
                }
              }
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>XY面上で直線的に連続する配管</summary>
    /// 
    /// <param name="pipe"        >配管</param>
    /// <param name="farPnt1"     >端点1</param>
    /// <param name="farPnt2"     >端点2</param>
    /// <param name="pipe1"       >交点計算用配管1</param>
    /// <param name="pipe2"       >交点計算用配管2</param>
    /// <param name="connectPipes">連続する配管</param>
    /// <param name="inViewPipeId">ビュー内配管ID</param>
    /// 
    /// <returns>配管</returns>
    /// 
    /// <history><p>2014/07/14 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/12/03 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    void StraightConnectPipesXY(Revit.DB.Plumbing.Pipe pipe,
                                ref Revit.DB.XYZ farPnt1,
                                ref Revit.DB.XYZ farPnt2,
                                ref Revit.DB.Plumbing.Pipe pipe1,
                                ref Revit.DB.Plumbing.Pipe pipe2,
                                ref Collections.Generic.IList<Revit.DB.Plumbing.Pipe> connectPipes,
                                Collections.Generic.IList<string> inViewPipeId)
    {
      // ビュー外配管
      if (inViewPipeId.Contains(pipe.Id.ToString()) == false)
      {
        return;
      }

      connectPipes.Add(pipe);

      // 配管サイズ
      Revit.DB.Parameter parDiameter = pipe.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
      double diameter = parDiameter != null ? parDiameter.AsDouble() : 0;
      diameter = _CmpGeometry.ToHalfAdjust(diameter * 304.8, 0);

      // 冷媒管径符号
      Revit.DB.Parameter parReibaikanHugo = pipe.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_REIBAIKANKEIHUGO"));
      string strReibaikanHugo = parReibaikanHugo != null ? parReibaikanHugo.AsString() : "";

      // 配管システム名
      Revit.DB.Parameter parSystemName = pipe.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
      string strSystemName = parSystemName != null ? parSystemName.AsString() : "";

      // 配管の直線分
      Revit.DB.Line line = _CmpGeometry.GetPipeLine(pipe);

      farPnt1 = line.GetEndPoint(0);
      farPnt2 = line.GetEndPoint(1);

      Collections.Generic.IList<string> ids = new Collections.Generic.List<string>();
      ids.Add(pipe.Id.ToString());

      // 継手
      Revit.DB.Element connector1 = null;
      Revit.DB.Element connector2 = null;

      // 継手取得
      GetConnectorOwner(pipe, ref connector1, ref connector2);

      // 継手なし
      if (connector1 == null && connector2 == null)
      {
        return;
      }

      // 連続する配管があるか
      bool isHaveNext = true;

      #region 継手1側

      if (connector1 != null &&
          IsPipeOrFitting(connector1))
      {
        while (isHaveNext)
        {
          // 指定の継手を持つ配管
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> sameCnctPipes = GetSameConnectorPipe(connector1);

          // 3つ以上の配管
          if (sameCnctPipes.Count > 2)
          {
            isHaveNext = false;

            // 交点計算用配管の取得
            pipe1 = _CmpGeometry.GetMostAnglePipe(pipe, sameCnctPipes);

            // 20140812
            // 継手の種類によっては接続する配管の数には関係なしに、
            // 交点ではなく配管端部で計算するので交点計算用配管は不要

            //foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
            //{
            //  // 角度のある方
            //  Revit.DB.Line line2 = _CmpGeometry.GetPipeLine(p);

            //  if (_CmpGeometry.IsTwoLineParallelZeroDIstance_NoSlope(line, line2) == false)
            //  {
            //    pipe1 = p;
            //  }
            //}

            // どちらもXY平面での角度がない場合
            if (pipe1 == null)
            {
              // 縦管
              foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
              {
                if (pipe.Id.ToString() != p.Id.ToString())
                {
                  if (_CmpGeometry.IsVerticalSinglePipe(p))
                  {
                    pipe1 = p;

                    break;
                  }
                }
              }

              if (pipe1 == null)
              {
                foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
                {
                  if (pipe.Id.ToString() != p.Id.ToString())
                  {
                    pipe1 = p;
                    break;
                  }
                }
              }
            }

            break;
          }
          // 2つ以上の配管
          else if (sameCnctPipes.Count > 1)
          {
            // 連続する配管の継手
            Revit.DB.Element cnct1 = null;
            Revit.DB.Element cnct2 = null;

            bool isHaveStraight = false;

            foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
            {
              if (inViewPipeId.Contains(p.Id.ToString()) == false)
              {
                continue;
              }

              if (ids.Contains(p.Id.ToString()))
              {
                continue;
              }

              // 配管サイズ
              Revit.DB.Parameter parDia = p.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
              double dia = parDia != null ? parDia.AsDouble() : 0;
              dia = _CmpGeometry.ToHalfAdjust(dia * 304.8, 0);

              // サイズ違い
              if (diameter != dia)
              {
                continue;
              }

              // 配管システム名
              Revit.DB.Parameter parSN = p.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
              string strSN = parSN != null ? parSN.AsString() : "";

              if (strSystemName != strSN)
              {
                continue;
              }

              // 仕様パラメータ比較
              if (_CmpParameters.ComparePipeShiyo(pipe, p) == false)
              {
                continue;
              }

              Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

              // 連続する配管
              if (_CmpGeometry.IsTwoLineParallelZeroDIstance(line, l))
              {
                connectPipes.Add(p);
                ids.Add(p.Id.ToString());

                isHaveStraight = true;

                // 遠い端点
                if (_CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(0)) > _CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(1)))
                {
                  farPnt1 = l.GetEndPoint(0);
                }
                else
                {
                  farPnt1 = l.GetEndPoint(1);
                }

                // 両端の継手
                GetConnectorOwner(p, ref cnct1, ref cnct2);
                // _CmpElements.GetConnectorOwner(p, ref cnct1, ref cnct2);

                // 両端があり、除外項目でない
                if (cnct1 != null &&
                    cnct2 != null &&
                    IsPipeOrFitting(cnct1) &&
                    IsPipeOrFitting(cnct2))
                {
                  // 継手に繋がる配管
                  Collections.Generic.IList<Revit.DB.Plumbing.Pipe> cps1 = _CmpElements.GetSameConnectorPipe(cnct1);
                  Collections.Generic.IList<Revit.DB.Plumbing.Pipe> cps2 = _CmpElements.GetSameConnectorPipe(cnct2);

                  // 同じ配管を持っているか
                  // 持っていない方を与える
                  bool b = true;
                  foreach (Revit.DB.Plumbing.Pipe cp in cps1)
                  {
                    bool b2 = false;

                    foreach (Revit.DB.Plumbing.Pipe _p in sameCnctPipes)
                    {
                      if (cp.Id.ToString() == _p.Id.ToString())
                      {
                        b2 = true;
                      }
                    }

                    if (b2 == false)
                    {
                      b = b2;
                    }
                  }

                  if (b)
                  {
                    b = true;
                    foreach (Revit.DB.Plumbing.Pipe cp in cps2)
                    {
                      bool b2 = false;

                      foreach (Revit.DB.Plumbing.Pipe _p in sameCnctPipes)
                      {
                        if (cp.Id.ToString() == _p.Id.ToString())
                        {
                          b2 = true;
                        }
                      }

                      if (b2 == false)
                      {
                        b = b2;
                      }
                    }

                    if (!b)
                    {
                      connector1 = cnct2;
                    }
                  }
                  else
                  {
                    connector1 = cnct1;
                  }


                  //if (connector1.Id.IntegerValue == cnct1.Id.IntegerValue)
                  //{
                  //  connector1 = cnct2;
                  //}
                  //else if (connector1.Id.IntegerValue == cnct2.Id.IntegerValue)
                  //{
                  //  connector1 = cnct1;
                  //}
                }
                else
                {
                  isHaveNext = false;
                }
              }
              else
              {

              }
            }

            // 同じ継手を持つがまっすぐではない場合
            if (isHaveStraight == false)
            {
              // 同じ継手を持つ配管
              foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
              {
                if (ids.Contains(p.Id.ToString()))
                {
                  continue;
                }

                if (!connector1.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeFitting).ToString()))
                {
                  continue;
                }

                //Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

                //// 遠い端点
                //if (_CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(0)) > _CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(1)))
                //{
                //  farPnt1 = l.GetEndPoint(0);
                //}
                //else
                //{
                //  farPnt1 = l.GetEndPoint(1);
                //}

                pipe1 = p;

                isHaveNext = false;

                break;
              }
            }


            if (cnct1 == null && cnct2 == null)
            {
              isHaveNext = false;
            }
          }
          // 1つなら基準の配管
          else
          {
            isHaveNext = false;
            break;
          }


        }
      }

      #endregion

      isHaveNext = true;

      #region 継手2側

      if (connector2 != null &&
          IsPipeOrFitting(connector2))
      {
        while (isHaveNext)
        {
          // 指定の継手を持つ配管
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> sameCnctPipes = GetSameConnectorPipe(connector2);

          // 3つ以上の配管
          if (sameCnctPipes.Count > 2)
          {
            isHaveNext = false;

            // 交点計算用配管の取得
            pipe2 = _CmpGeometry.GetMostAnglePipe(pipe, sameCnctPipes);

            // 20140812
            // 継手の種類によっては接続する配管の数にはよらず
            // 交点ではなく配管端部で計算するので交点計算用配管は不要

            //foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
            //{
            //  // 角度のある方
            //  Revit.DB.Line line2 = _CmpGeometry.GetPipeLine(p);

            //  if (_CmpGeometry.IsTwoLineParallelZeroDIstance_NoSlope(line, line2) == false)
            //  {
            //    pipe2 = p;
            //  }
            //}

            // どちらもXY平面での角度がない場合
            if (pipe2 == null)
            {
              // 縦管
              foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
              {
                if (pipe.Id.ToString() != p.Id.ToString())
                {
                  if (_CmpGeometry.IsVerticalSinglePipe(p))
                  {
                    pipe2 = p;
                    break;
                  }
                }
              }

              if (pipe2 == null)
              {
                foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
                {
                  if (pipe.Id.ToString() != p.Id.ToString())
                  {
                    pipe2 = p;
                    break;
                  }
                }
              }
            }

            break;
          }
          // 2つ以上の配管
          else if (sameCnctPipes.Count > 1)
          {
            // 連続する配管継手
            Revit.DB.Element cnct1 = null;
            Revit.DB.Element cnct2 = null;

            bool isHaveStraight = false;

            foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
            {
              if (inViewPipeId.Contains(p.Id.ToString()) == false)
              {
                continue;
              }
              if (ids.Contains(p.Id.ToString()))
              {
                continue;
              }

              // 配管サイズ
              Revit.DB.Parameter parDia = p.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
              double dia = parDia != null ? parDia.AsDouble() : 0;
              dia = _CmpGeometry.ToHalfAdjust(dia * 304.8, 0);

              // サイズ違い
              if (diameter != dia)
              {
                continue;
              }

              // 配管システム名
              Revit.DB.Parameter parSN = p.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
              string strSN = parSN != null ? parSN.AsString() : "";

              if (strSystemName != strSN)
              {
                continue;
              }

              // 仕様パラメータ比較
              if (_CmpParameters.ComparePipeShiyo(pipe, p) == false)
              {
                continue;
              }

              Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

              // 連続する配管
              if (_CmpGeometry.IsTwoLineParallelZeroDIstance(line, l))
              {
                connectPipes.Add(p);
                ids.Add(p.Id.ToString());

                isHaveStraight = true;

                // 遠い端点
                if (_CmpGeometry.Distance2D(farPnt2, l.GetEndPoint(0)) > _CmpGeometry.Distance2D(farPnt2, l.GetEndPoint(1)))
                {
                  farPnt2 = l.GetEndPoint(0);
                }
                else
                {
                  farPnt2 = l.GetEndPoint(1);
                }

                // 両端の継手
                GetConnectorOwner(p, ref cnct1, ref cnct2);
                //_CmpElements.GetConnectorOwner(p, ref cnct1, ref cnct2);

                if (cnct1 != null &&
                    cnct2 != null &&
                    IsPipeOrFitting(cnct1) &&
                    IsPipeOrFitting(cnct2))
                {
                  // 継手に繋がる配管
                  Collections.Generic.IList<Revit.DB.Plumbing.Pipe> cps1 = _CmpElements.GetSameConnectorPipe(cnct1);
                  Collections.Generic.IList<Revit.DB.Plumbing.Pipe> cps2 = _CmpElements.GetSameConnectorPipe(cnct2);

                  // 同じ配管を持っているか
                  // 持っていない方を与える
                  bool b = true;
                  foreach (Revit.DB.Plumbing.Pipe cp in cps1)
                  {
                    bool b2 = false;

                    foreach (Revit.DB.Plumbing.Pipe _p in sameCnctPipes)
                    {
                      if (cp.Id.ToString() == _p.Id.ToString())
                      {
                        b2 = true;
                      }
                    }

                    if (b2 == false)
                    {
                      b = b2;
                    }
                  }

                  if (b)
                  {
                    b = true;
                    foreach (Revit.DB.Plumbing.Pipe cp in cps2)
                    {
                      bool b2 = false;

                      foreach (Revit.DB.Plumbing.Pipe _p in sameCnctPipes)
                      {
                        if (cp.Id.ToString() == _p.Id.ToString())
                        {
                          b2 = true;
                        }
                      }

                      if (b2 == false)
                      {
                        b = b2;
                      }
                    }

                    if (!b)
                    {
                      connector2 = cnct2;
                    }
                  }
                  else
                  {
                    connector2 = cnct1;
                  }


                  //if (connector2.Id.IntegerValue == cnct1.Id.IntegerValue)
                  //{
                  //  connector2 = cnct2;
                  //}
                  //else if (connector2.Id.IntegerValue == cnct2.Id.IntegerValue)
                  //{
                  //  connector2 = cnct1;
                  //}
                }
                else
                {
                  isHaveNext = false;
                }
              }
            }


            // 同じ継手を持つがまっすぐではない場合
            if (isHaveStraight == false)
            {
              // 同じ継手を持つ配管
              foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
              {
                if (ids.Contains(p.Id.ToString()))
                {
                  continue;
                }

                if (!connector2.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeFitting).ToString()))
                {
                  continue;
                }

                //Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

                //// 遠い端点
                //if (_CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(0)) > _CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(1)))
                //{
                //  farPnt1 = l.GetEndPoint(0);
                //}
                //else
                //{
                //  farPnt1 = l.GetEndPoint(1);
                //}

                pipe2 = p;

                isHaveNext = false;

                break;
              }
            }


            if (cnct1 == null && cnct2 == null)
            {
              isHaveNext = false;
            }
          }
          else
          {
            isHaveNext = false;
            break;
          }
        }
      }

      #endregion
    }

    /// ================================================================================
    /// <summary>連続する縦管(斜め管はXY成分の差があるので平面で)</summary>
    /// 
    /// <param name="verticPipe"  >基準縦管</param>
    /// <param name="pipeAry"     >配管</param>
    /// <param name="inViewPipeId">ビュー内配管ID</param>
    /// 
    /// <history><p>2014/09/24 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2015/12/03 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Plumbing.Pipe> ConnectVerticalPipe(Revit.DB.Plumbing.Pipe verticPipe,
                                                                          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> pipeAry,
                                                                          ref Revit.DB.XYZ farPoint1,
                                                                          ref Revit.DB.XYZ farPoint2,
                                                                          ref Revit.DB.Plumbing.Pipe pipe1,
                                                                          ref Revit.DB.Plumbing.Pipe pipe2,
                                                                          Collections.Generic.IList<string> inViewPipeId)
    {
      //戻り値
      Collections.Generic.IList<Revit.DB.Plumbing.Pipe> ret = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();

      // ビュー外配管
      if (inViewPipeId.Contains(verticPipe.Id.ToString()) == false)
      {
        return ret;
      }

      ret.Add(verticPipe);

      // 配管サイズ
      Revit.DB.Parameter parDiameter = verticPipe.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
      double diameter = parDiameter != null ? parDiameter.AsDouble() : 0;
      diameter = _CmpGeometry.ToHalfAdjust(diameter * 304.8, 0);

      // 配管システム名
      Revit.DB.Parameter parSystemName = verticPipe.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
      string strSystemName = parSystemName != null ? parSystemName.AsString() : "";

      // 縦管の直線分
      Revit.DB.Line line = _CmpGeometry.GetPipeLine(verticPipe);

      farPoint1 = line.GetEndPoint(0);
      farPoint2 = line.GetEndPoint(1);

      Collections.Generic.IList<string> ids = new Collections.Generic.List<string>();
      ids.Add(verticPipe.Id.ToString());

      // 継手
      Revit.DB.Element connector1 = null;
      Revit.DB.Element connector2 = null;

      // 継手取得
      GetConnectorOwner(verticPipe, ref connector1, ref connector2);
      //_CmpElements.GetConnectorOwner(verticPipe, ref connector1, ref connector2);

      // 継手なし
      if (connector1 == null && connector2 == null)
      {
        return ret;
      }

      // 連続する配管があるか
      bool isHaveNext = true;

      #region 継手1側

      if (connector1 != null &&
          IsPipeOrFitting(connector1))
      {
        while (isHaveNext)
        {
          // 連続する要素
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> sameCnctPipes = GetSameConnectorPipe(connector1);

          // 2つ以上(縦は上下1つずつしかないので全体がいくつでも関係ない)
          if (sameCnctPipes.Count > 1)
          {
            isHaveNext = false;

            foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
            {
              if (inViewPipeId.Contains(p.Id.ToString()) == false)
              {
                continue;
              }

              if (ids.Contains(p.Id.ToString()) == false)
              {
                // 縦管
                if (_CmpGeometry.IsVerticalSinglePipe(p))
                {
                  // 配管サイズ
                  Revit.DB.Parameter parDia = p.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
                  double dia = parDia != null ? parDia.AsDouble() : 0;
                  dia = _CmpGeometry.ToHalfAdjust(dia * 304.8, 0);

                  // サイズ違い
                  if (diameter != dia)
                  {
                    // 交点を求める farPoint1
                    Revit.DB.XYZ _p0 = null;
                    Revit.DB.XYZ _p1 = null;

                    _CmpGeometry.GetNearLinesPoints(_CmpGeometry.GetPipeLine(verticPipe), _CmpGeometry.GetPipeLine(p), ref _p0, ref _p1);

                    farPoint1 = (farPoint1 + _p1) / 2;

                    continue;
                  }

                  // 配管システム名
                  Revit.DB.Parameter parSN = p.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
                  string strSN = parSN != null ? parSN.AsString() : "";

                  if (strSystemName != strSN)
                  {
                    continue;
                  }

                  // 仕様パラメータ比較
                  if (_CmpParameters.ComparePipeShiyo(verticPipe, p) == false)
                  {
                    continue;
                  }

                  isHaveNext = true;

                  ret.Add(p);
                  ids.Add(p.Id.ToString());

                  // 連続する配管の継手
                  Revit.DB.Element cnct1 = null;
                  Revit.DB.Element cnct2 = null;

                  Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

                  // 遠い端点
                  if (_CmpGeometry.Distance(farPoint1, l.GetEndPoint(0)) > _CmpGeometry.Distance(farPoint1, l.GetEndPoint(1)))
                  {
                    farPoint1 = l.GetEndPoint(0);
                  }
                  else
                  {
                    farPoint1 = l.GetEndPoint(1);
                  }

                  GetConnectorOwner(p, ref cnct1, ref cnct2);
                  //_CmpElements.GetConnectorOwner(p, ref cnct1, ref cnct2);

                  // 逆側に続くか
                  if (cnct1 != null &&
                      cnct2 != null &&
                      IsPipeOrFitting(cnct1) &&
                      IsPipeOrFitting(cnct2))
                  {
                    if (connector1.Id.ToString() == cnct1.Id.ToString())
                    {
                      connector1 = cnct2;
                    }
                    else if (connector1.Id.ToString() == cnct2.Id.ToString())
                    {
                      connector1 = cnct1;
                    }
                  }
                  else
                  {
                    isHaveNext = false;
                  }

                  break;
                }
                // 横、斜め管
                else
                {
                  // 縦管が続いていた場合は計算不要
                  if (isHaveNext == false)
                  {
                    // 縦管のXY座標に横管を延長したときのZ座標
                    Revit.DB.XYZ p0 = line.GetEndPoint(0);

                    double crossPntZ = _CmpGeometry.GetExtLineZPoint(_CmpGeometry.GetPipeLine(p), p0.X, p0.Y);

                    Revit.DB.XYZ crossPnt = new Revit.DB.XYZ(p0.X, p0.Y, crossPntZ);

                    // 近い方
                    if (_CmpGeometry.Distance(farPoint1, crossPnt) < _CmpGeometry.Distance(farPoint2, crossPnt))
                    {
                      farPoint1 = crossPnt;
                    }
                    else
                    {
                      farPoint2 = crossPnt;
                    }
                  }
                }
              }
            }

          }
          // 1つ
          else
          {
            isHaveNext = false;
            break;
          }
        }
      }

      #endregion

      #region 継手2側

      isHaveNext = true;

      if (connector2 != null &&
          IsPipeOrFitting(connector1))
      {
        while (isHaveNext)
        {
          // 連続する要素
          Collections.Generic.IList<Revit.DB.Plumbing.Pipe> sameCnctPipes = GetSameConnectorPipe(connector2);

          // 2つ以上(縦は上下1つずつしかないので全体がいくつでも関係ない)
          if (sameCnctPipes.Count > 1)
          {
            isHaveNext = false;

            foreach (Revit.DB.Plumbing.Pipe p in sameCnctPipes)
            {
              if (inViewPipeId.Contains(p.Id.ToString()) == false)
              {
                continue;
              }

              if (ids.Contains(p.Id.ToString()) == false)
              {
                // 縦管
                if (_CmpGeometry.IsVerticalSinglePipe(p))
                {
                  // 配管サイズ
                  Revit.DB.Parameter parDia = p.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);
                  double dia = parDia != null ? parDia.AsDouble() : 0;
                  dia = _CmpGeometry.ToHalfAdjust(dia * 304.8, 0);

                  // サイズ違い
                  if (diameter != dia)
                  {
                    // 交点を求める farPoint2
                    Revit.DB.XYZ _p0 = null;
                    Revit.DB.XYZ _p1 = null;

                    _CmpGeometry.GetNearLinesPoints(_CmpGeometry.GetPipeLine(verticPipe), _CmpGeometry.GetPipeLine(p), ref _p0, ref _p1);

                    farPoint2 = (farPoint2 + _p1) / 2;

                    continue;
                  }

                  // 配管システム名
                  Revit.DB.Parameter parSN = p.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
                  string strSN = parSN != null ? parSN.AsString() : "";

                  if (strSystemName != strSN)
                  {
                    continue;
                  }

                  // 仕様パラメータ比較
                  if (_CmpParameters.ComparePipeShiyo(verticPipe, p) == false)
                  {
                    continue;
                  }

                  isHaveNext = true;

                  ret.Add(p);
                  ids.Add(p.Id.ToString());

                  // 連続する配管の継手
                  Revit.DB.Element cnct1 = null;
                  Revit.DB.Element cnct2 = null;

                  Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

                  // 遠い端点
                  if (_CmpGeometry.Distance(farPoint2, l.GetEndPoint(0)) > _CmpGeometry.Distance(farPoint2, l.GetEndPoint(1)))
                  {
                    farPoint2 = l.GetEndPoint(0);
                  }
                  else
                  {
                    farPoint2 = l.GetEndPoint(1);
                  }

                  GetConnectorOwner(p, ref cnct1, ref cnct2);
                  //_CmpElements.GetConnectorOwner(p, ref cnct1, ref cnct2);

                  // 逆側に続くか
                  if (cnct1 != null &&
                      cnct2 != null &&
                      IsPipeOrFitting(cnct1) &&
                      IsPipeOrFitting(cnct2))
                  {
                    if (connector2.Id.ToString() == cnct1.Id.ToString())
                    {
                      connector2 = cnct2;
                    }
                    else if (connector2.Id.ToString() == cnct2.Id.ToString())
                    {
                      connector2 = cnct1;
                    }
                  }
                  else
                  {
                    isHaveNext = false;
                  }

                  break;
                }
                // 横、斜め管
                else
                {
                  // 縦管が続いていた場合は計算不要
                  if (isHaveNext == false)
                  {
                    // 縦管のXY座標に横管を延長したときのZ座標
                    Revit.DB.XYZ p0 = line.GetEndPoint(0);

                    double crossPntZ = _CmpGeometry.GetExtLineZPoint(_CmpGeometry.GetPipeLine(p), p0.X, p0.Y);

                    Revit.DB.XYZ crossPnt = new Revit.DB.XYZ(p0.X, p0.Y, crossPntZ);

                    // 近い方
                    if (_CmpGeometry.Distance(farPoint1, crossPnt) < _CmpGeometry.Distance(farPoint2, crossPnt))
                    {
                      farPoint1 = crossPnt;
                    }
                    else
                    {
                      farPoint2 = crossPnt;
                    }
                  }
                }
              }
            }

          }
          // 1つ
          else
          {
            isHaveNext = false;
            break;
          }
        }
      }

      #endregion

      return ret;
    }

    /// ================================================================================
    /// <summary>XY面上で直線的に連続するダクト</summary>
    /// 
    /// <param name="duct"        >ダクト</param>
    /// <param name="farPnt1"     >端点1</param>
    /// <param name="farPnt2"     >端点2</param>
    /// <param name="duct1"       >交点計算用配管1</param>
    /// <param name="duct2"       >交点計算用配管2</param>
    /// <param name="inViewDuctId">ビュー内ダクトID</param>
    /// <param name="isQuantity"  >拾い書</param>
    /// 
    /// <returns>ダクト</returns>
    /// 
    /// <history><p>2014/07/29 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2016/06/23 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Duct> StraightConnectDuctsXY(Revit.DB.Mechanical.Duct duct,
                                                                               ref Revit.DB.XYZ farPnt1,
                                                                               ref Revit.DB.XYZ farPnt2,
                                                                               ref Revit.DB.Mechanical.Duct duct1,
                                                                               ref Revit.DB.Mechanical.Duct duct2,
                                                                               Collections.Generic.IList<string> inViewDuctId,
                                                                               bool isQuantity)
    {


      // 戻り値
      Collections.Generic.IList<Revit.DB.Mechanical.Duct> ret = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();

      // ビュー外ダクト
      if (inViewDuctId.Contains(duct.Id.ToString()) == false)
      {
        return ret;
      }

      ret.Add(duct);

      // ダクトサイズ
      Revit.DB.Parameter parCalSize = duct.get_Parameter(Revit.DB.BuiltInParameter.RBS_CALCULATED_SIZE);
      string calSize = parCalSize != null ? parCalSize.AsString() : "";

      // ダクトシステム名
      Revit.DB.Parameter parSystemName = duct.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
      string strSystemName = parSystemName != null ? parSystemName.AsString() : "";

      // ダクト付加条件
      Revit.DB.Parameter parHukajouken = duct.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_HUKAJOKEN"));
      string strHukajouken = parHukajouken != null ? parHukajouken.AsString() : "";

      // 配管の直線分
      Revit.DB.Line line = _CmpGeometry.GetDuctLine(duct);

      farPnt1 = line.GetEndPoint(0);
      farPnt2 = line.GetEndPoint(1);

      Collections.Generic.IList<string> ids = new Collections.Generic.List<string>();
      ids.Add(duct.Id.ToString());

      // 継手
      Revit.DB.Element connector1 = null;
      Revit.DB.Element connector2 = null;


      GetConnectorOwner(duct, ref connector1, ref connector2);

      if (connector1 == null && connector2 == null)
      {
        return ret;
      }

      // 連続するダクトがあるか
      bool isHaveNext = true;

      #region 継手1側

      if (connector1 != null &&
          IsDuctOrFitting(connector1))
      {
        while (isHaveNext)
        {
          // 指定の継手を持つダクト
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> sameCnctDucts = GetSameConnectorDuct(connector1);

          if (isQuantity)
          {
            //// 単管処理
            //#region 単管処理

            //Revit.DB.FamilyInstance famInsDumper = connector1 as Revit.DB.FamilyInstance;
            //Revit.DB.FamilySymbol famSymDumper = famInsDumper.Symbol;

            //// 呼称
            //string strKosyo = "";

            //if (famSymDumper != null)
            //{
            //    Revit.DB.Parameter parKosyo = famSymDumper.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KOSYO"));

            //    if (parKosyo != null && parKosyo.HasValue)
            //    {
            //        strKosyo = parKosyo.AsString();
            //    }
            //}

            //// 単管有無
            //string strTankan = GetTankanExist(strKosyo);
            //bool tankanExist = strTankan == "有" ? true : false;

            //Revit.DB.FamilyInstance retDumper = null;
            //Revit.DB.Mechanical.Duct retDuct1 = null;
            //Revit.DB.Mechanical.Duct retDuct2 = null;
            //Revit.DB.Mechanical.Duct retDuct3 = null;

            //// 単管有り
            //if (tankanExist)
            //{
            //    retDumper = famInsDumper;
            //    if (sameCnctDucts.Count > 0)
            //    {
            //        retDuct1 = sameCnctDucts[0];
            //    }
            //    if (sameCnctDucts.Count > 1)
            //    {
            //        retDuct2 = sameCnctDucts[1];
            //    }
            //    if (sameCnctDucts.Count > 2)
            //    {
            //        retDuct1 = sameCnctDucts[2];
            //    }

            //    //              SetTankanExportTable(retDumper, retDuct1, retDuct2, retDuct3);
            //}

            //#endregion
          }

          // 3つ以上のダクト
          if (sameCnctDucts.Count > 2)
          {
            isHaveNext = false;

            duct1 = _CmpGeometry.GetMostAngleDuct(duct, sameCnctDucts);

            if (duct1 == null)
            {
              // 縦ダクト
              foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
              {
                if (duct.Id.ToString() != d.Id.ToString())
                {
                  if (_CmpGeometry.IsVerticalSingleDuct(d))
                  {
                    duct1 = d;

                    break;
                  }
                }
              }

              if (duct1 == null)
              {
                // 縦ダクト
                foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
                {
                  if (duct.Id.ToString() != d.Id.ToString())
                  {
                    duct1 = d;
                    break;
                  }
                }
              }
            }

            break;
          }
          // 2つ以上のダクト
          else if (sameCnctDucts.Count > 1)
          {
            // 連続する配管の継手
            Revit.DB.Element cnct1 = null;
            Revit.DB.Element cnct2 = null;

            bool isHaveStraight = false;

            foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
            {
              if (inViewDuctId.Contains(d.Id.ToString()) == false)
              {
                continue;
              }

              if (ids.Contains(d.Id.ToString()))
              {
                continue;
              }

              // ダクトサイズ
              Revit.DB.Parameter parCS = d.get_Parameter(Revit.DB.BuiltInParameter.RBS_CALCULATED_SIZE);
              string cS = parCS != null ? parCS.AsString() : "";

              // サイズ違い
              if (calSize != cS)
              {
                continue;
              }

              // ダクトシステム名
              Revit.DB.Parameter parSN = d.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
              string strSN = parSN != null ? parSN.AsString() : "";

              if (strSystemName != strSN)
              {
                continue;
              }

              // 仕様パラメータ比較
              if (_CmpParameters.CompareDuctShiyo(duct, d) == false)
              {
                continue;
              }

              Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

              // 連続する配管
              if (_CmpGeometry.IsTwoLineParallelZeroDIstance(line, l))
              {
                ret.Add(d);
                ids.Add(d.Id.ToString());

                isHaveStraight = true;

                // 遠い端点
                if (_CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(0)) > _CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(1)))
                {
                  farPnt1 = l.GetEndPoint(0);
                }
                else
                {
                  farPnt1 = l.GetEndPoint(1);
                }

                // 逆側の継手があるか
                GetConnectorOwner(d, ref cnct1, ref cnct2);
                // _CmpElements.GetConnectorOwner(d, ref cnct1, ref cnct2);

                if (cnct1 != null &&
                    cnct2 != null &&
                    IsDuctOrFitting(cnct1) &&
                    IsDuctOrFitting(cnct2))
                {
                  // 継手に繋がるダクト
                  Collections.Generic.IList<Revit.DB.Mechanical.Duct> cds1 = _CmpElements.GetSameConnectorDuct(cnct1);
                  Collections.Generic.IList<Revit.DB.Mechanical.Duct> cds2 = _CmpElements.GetSameConnectorDuct(cnct2);

                  // 同じダクトを持っているか
                  // 持っていない方を与える
                  bool b = true;
                  foreach (Revit.DB.Mechanical.Duct cd in cds1)
                  {
                    bool b2 = false;

                    foreach (Revit.DB.Mechanical.Duct _d in sameCnctDucts)
                    {
                      if (cd.Id.ToString() == _d.Id.ToString())
                      {
                        b2 = true;
                      }
                    }

                    if (b2 == false)
                    {
                      b = b2;
                    }
                  }

                  if (b)
                  {
                    b = true;
                    foreach (Revit.DB.Mechanical.Duct cd in cds2)
                    {
                      bool b2 = false;

                      foreach (Revit.DB.Mechanical.Duct _d in sameCnctDucts)
                      {
                        if (cd.Id.ToString() == _d.Id.ToString())
                        {
                          b2 = true;
                        }
                      }

                      if (b2 == false)
                      {
                        b = b2;
                      }
                    }

                    if (!b)
                    {
                      connector1 = cnct2;
                    }
                  }
                  else
                  {
                    connector1 = cnct1;
                  }
                }
                else
                {
                  isHaveNext = false;
                }
              }
              else
              {


              }
            }

            // 同じ継手を持つがまっすぐではない場合
            if (isHaveStraight == false)
            {
              foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
              {
                if (ids.Contains(d.Id.ToString()))
                {
                  continue;
                }

                if (!connector1.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctFitting).ToString()))
                {
                  continue;
                }

                duct1 = d;

                isHaveNext = false;

                break;
              }
            }


            if (cnct1 == null && cnct2 == null)
            {
              isHaveNext = false;
            }
          }
          else
          {
            isHaveNext = false;
            break;
          }


        }
      }

      #endregion

      isHaveNext = true;

      #region 継手2側

      if (connector2 != null &&
          IsDuctOrFitting(connector2))
      {
        while (isHaveNext)
        {
          // 指定の継手を持つダクト
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> sameCnctDucts = GetSameConnectorDuct(connector2);

          if (isQuantity)
          {
            //// 単管処理
            //#region 単管処理

            //Revit.DB.FamilyInstance famInsDumper = connector2 as Revit.DB.FamilyInstance;
            //Revit.DB.FamilySymbol famSymDumper = famInsDumper.Symbol;

            //// 呼称
            //string strKosyo = "";

            //if (famSymDumper != null)
            //{
            //    Revit.DB.Parameter parKosyo = famSymDumper.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KOSYO"));

            //    if (parKosyo != null && parKosyo.HasValue)
            //    {
            //        strKosyo = parKosyo.AsString();
            //    }
            //}

            //// 単管有無
            //string strTankan = GetTankanExist(strKosyo);
            //bool tankanExist = strTankan == "有" ? true : false;

            //Revit.DB.FamilyInstance retDumper = null;
            //Revit.DB.Mechanical.Duct retDuct1 = null;
            //Revit.DB.Mechanical.Duct retDuct2 = null;
            //Revit.DB.Mechanical.Duct retDuct3 = null;

            //// 単管有り
            //if (tankanExist)
            //{
            //    retDumper = famInsDumper;
            //    if (sameCnctDucts.Count > 0)
            //    {
            //        retDuct1 = sameCnctDucts[0];
            //    }
            //    if (sameCnctDucts.Count > 1)
            //    {
            //        retDuct2 = sameCnctDucts[1];
            //    }
            //    if (sameCnctDucts.Count > 2)
            //    {
            //        retDuct1 = sameCnctDucts[2];
            //    }

            //    //              SetTankanExportTable(retDumper, retDuct1, retDuct2, retDuct3);
            //}

            //#endregion
          }

          // 3つ以上のダクト
          if (sameCnctDucts.Count > 2)
          {
            isHaveNext = false;

            duct2 = _CmpGeometry.GetMostAngleDuct(duct, sameCnctDucts);

            //// 交点計算用配管の取得
            //foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
            //{
            //  // 角度のある方
            //  Revit.DB.Line line2 = _CmpGeometry.GetDuctLine(d);

            //  if (_CmpGeometry.IsTwoLineParallelZeroDIstance_NoSlope(line, line2) == false)
            //  {
            //    duct2 = d;
            //  }
            //}

            if (duct2 == null)
            {
              // 縦ダクト
              foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
              {
                if (duct.Id.ToString() != d.Id.ToString())
                {
                  if (_CmpGeometry.IsVerticalSingleDuct(d))
                  {
                    duct2 = d;

                    break;
                  }
                }
              }

              if (duct2 == null)
              {
                // 違うダクト
                foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
                {
                  if (duct.Id.ToString() != d.Id.ToString())
                  {
                    duct2 = d;
                    break;
                  }
                }
              }
            }

            break;
          }
          // 2つ以上のダクト
          else if (sameCnctDucts.Count > 1)
          {
            // 連続する配管継手
            Revit.DB.Element cnct1 = null;
            Revit.DB.Element cnct2 = null;

            bool isHaveStraight = false;

            foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
            {
              if (inViewDuctId.Contains(d.Id.ToString()) == false)
              {
                continue;
              }

              if (ids.Contains(d.Id.ToString()))
              {
                continue;
              }

              // ダクトサイズ
              Revit.DB.Parameter parCS = d.get_Parameter(Revit.DB.BuiltInParameter.RBS_CALCULATED_SIZE);
              string cS = parCS != null ? parCS.AsString() : "";

              // サイズ違い
              if (calSize != cS)
              {
                continue;
              }

              // ダクトシステム名
              Revit.DB.Parameter parSN = d.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
              string strSN = parSN != null ? parSN.AsString() : "";

              if (strSystemName != strSN)
              {
                continue;
              }

              // 仕様パラメータ比較
              if (_CmpParameters.CompareDuctShiyo(duct, d) == false)
              {
                continue;
              }

              Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

              // 連続する配管
              if (_CmpGeometry.IsTwoLineParallelZeroDIstance(line, l))
              {
                ret.Add(d);
                ids.Add(d.Id.ToString());

                isHaveStraight = true;

                // 遠い端点
                if (_CmpGeometry.Distance2D(farPnt2, l.GetEndPoint(0)) > _CmpGeometry.Distance2D(farPnt2, l.GetEndPoint(1)))
                {
                  farPnt2 = l.GetEndPoint(0);
                }
                else
                {
                  farPnt2 = l.GetEndPoint(1);
                }

                // 逆側の継手があるか
                GetConnectorOwner(d, ref cnct1, ref cnct2);
                // _CmpElements.GetConnectorOwner(d, ref cnct1, ref cnct2);

                if (cnct1 != null &&
                    cnct2 != null &&
                    IsDuctOrFitting(cnct1) &&
                    IsDuctOrFitting(cnct2))
                {
                  // 継手に繋がるダクト
                  Collections.Generic.IList<Revit.DB.Mechanical.Duct> cds1 = _CmpElements.GetSameConnectorDuct(cnct1);
                  Collections.Generic.IList<Revit.DB.Mechanical.Duct> cds2 = _CmpElements.GetSameConnectorDuct(cnct2);

                  // 同じダクトを持っているか
                  // 持っていない方を与える
                  bool b = true;
                  foreach (Revit.DB.Mechanical.Duct cd in cds1)
                  {
                    bool b2 = false;

                    foreach (Revit.DB.Mechanical.Duct _d in sameCnctDucts)
                    {
                      if (cd.Id.ToString() == _d.Id.ToString())
                      {
                        b2 = true;
                      }
                    }

                    if (b2 == false)
                    {
                      b = b2;
                    }
                  }

                  if (b)
                  {
                    b = true;
                    foreach (Revit.DB.Mechanical.Duct cd in cds2)
                    {
                      bool b2 = false;

                      foreach (Revit.DB.Mechanical.Duct _d in sameCnctDucts)
                      {
                        if (cd.Id.ToString() == _d.Id.ToString())
                        {
                          b2 = true;
                        }
                      }

                      if (b2 == false)
                      {
                        b = b2;
                      }
                    }

                    if (!b)
                    {
                      connector2 = cnct2;
                    }
                  }
                  else
                  {
                    connector2 = cnct1;
                  }


                  //if (connector2.Id.IntegerValue == cnct1.Id.IntegerValue)
                  //{
                  //  connector2 = cnct2;
                  //}
                  //else if (connector2.Id.IntegerValue == cnct2.Id.IntegerValue)
                  //{
                  //  connector2 = cnct1;
                  //}
                }
                else
                {
                  isHaveNext = false;
                }
              }
            }


            // 同じ継手を持つがまっすぐではない場合
            if (isHaveStraight == false)
            {
              foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
              {
                if (ids.Contains(d.Id.ToString()))
                {
                  continue;
                }

                if (!connector2.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctFitting).ToString()))
                {
                  continue;
                }

                //Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

                //// 遠い端点
                //if (_CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(0)) > _CmpGeometry.Distance2D(farPnt1, l.GetEndPoint(1)))
                //{
                //  farPnt1 = l.GetEndPoint(0);
                //}
                //else
                //{
                //  farPnt1 = l.GetEndPoint(1);
                //}

                duct2 = d;

                isHaveNext = false;

                break;
              }
            }


            if (cnct1 == null && cnct2 == null)
            {
              isHaveNext = false;
            }
          }
          else
          {
            isHaveNext = false;
            break;
          }
        }
      }

      #endregion

      return ret;
    }

    /// ================================================================================
    /// <summary>連続する縦ダクト</summary>
    /// 
    /// <param name="duct"        >ダクト</param>
    /// <param name="ductAry"     >ダクト</param>
    /// <param name="farPoint1"   >端点1</param>
    /// <param name="farPoint2"   >端点2</param>
    /// <param name="duct1"       >交点計算用ダクト1</param>
    /// <param name="duct2"       >交点計算用ダクト2</param>
    /// <param name="inViewDuctId">ビュー内ダクトID</param>
    /// <param name="isQuantity"  >拾い書</param>
    /// 
    /// <history><p>2014/09/24 Created GSA, Inc. Ryo Kuroda</p>
    ///           <p>2016/06/23 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Duct> ConnectVerticalDuct(Revit.DB.Mechanical.Duct duct,
                                                                            Collections.Generic.IList<Revit.DB.Mechanical.Duct> ductAry,
                                                                            ref Revit.DB.XYZ farPoint1,
                                                                            ref Revit.DB.XYZ farPoint2,
                                                                            ref Revit.DB.Mechanical.Duct duct1,
                                                                            ref Revit.DB.Mechanical.Duct duct2,
                                                                            Collections.Generic.IList<string> inViewDuctId,
                                                                            bool isQuantity)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Mechanical.Duct> ret = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();

      // ビュー外ダクト
      if (inViewDuctId.Contains(duct.Id.ToString()) == false)
      {
        return ret;
      }

      ret.Add(duct);

      // ダクトサイズ
      Revit.DB.Parameter parCalSize = duct.get_Parameter(Revit.DB.BuiltInParameter.RBS_CALCULATED_SIZE);
      string calSize = parCalSize != null ? parCalSize.AsString() : "";

      // ダクトシステム名
      Revit.DB.Parameter parSystemName = duct.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
      string strSystemName = parSystemName != null ? parSystemName.AsString() : "";

      // ダクト付加条件
      Revit.DB.Parameter parHukajouken = duct.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_HUKAJOKEN"));
      string strHukajouken = parHukajouken != null ? parHukajouken.AsString() : "";

      // 縦ダクトの直線分
      Revit.DB.Line line = _CmpGeometry.GetDuctLine(duct);

      farPoint1 = line.GetEndPoint(0);
      farPoint2 = line.GetEndPoint(1);

      Collections.Generic.IList<string> ids = new Collections.Generic.List<string>();
      ids.Add(duct.Id.ToString());

      // 継手
      Revit.DB.Element connector1 = null;
      Revit.DB.Element connector2 = null;

      GetConnectorOwner(duct, ref connector1, ref connector2);
      //_CmpElements.GetConnectorOwner(duct, ref connector1, ref connector2);

      // 継手なし
      if (connector1 == null && connector2 == null)
      {
        return ret;
      }

      // 連続するダクトがあるか
      bool isHaveNext = true;

      #region 継手1側

      if (connector1 != null &&
          IsDuctOrFitting(connector1))
      {
        while (isHaveNext)
        {
          // 連続する要素
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> sameCnctDucts = GetSameConnectorDuct(connector1);

          if (isQuantity)
          {
            //// 単管処理
            //#region 単管処理

            //Revit.DB.FamilyInstance famInsDumper = connector1 as Revit.DB.FamilyInstance;
            //Revit.DB.FamilySymbol famSymDumper = famInsDumper.Symbol;

            //// 呼称
            //string strKosyo = "";

            //if (famSymDumper != null)
            //{
            //    Revit.DB.Parameter parKosyo = famSymDumper.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KOSYO"));

            //    if (parKosyo != null && parKosyo.HasValue)
            //    {
            //        strKosyo = parKosyo.AsString();
            //    }
            //}

            //// 単管有無
            //string strTankan = GetTankanExist(strKosyo);
            //bool tankanExist = strTankan == "有" ? true : false;

            //Revit.DB.FamilyInstance retDumper = null;
            //Revit.DB.Mechanical.Duct retDuct1 = null;
            //Revit.DB.Mechanical.Duct retDuct2 = null;
            //Revit.DB.Mechanical.Duct retDuct3 = null;

            //// 単管有り
            //if (tankanExist)
            //{
            //    retDumper = famInsDumper;
            //    if (sameCnctDucts.Count > 0)
            //    {
            //        retDuct1 = sameCnctDucts[0];
            //    }
            //    if (sameCnctDucts.Count > 1)
            //    {
            //        retDuct2 = sameCnctDucts[1];
            //    }
            //    if (sameCnctDucts.Count > 2)
            //    {
            //        retDuct1 = sameCnctDucts[2];
            //    }

            //    //              SetTankanExportTable(retDumper, retDuct1, retDuct2, retDuct3);
            //}

            //#endregion
          }

          // 2つ以上
          if (sameCnctDucts.Count > 1)
          {
            isHaveNext = false;

            foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
            {
              if (inViewDuctId.Contains(d.Id.ToString()) == false)
              {
                continue;
              }

              if (ids.Contains(d.Id.ToString()) == false)
              {
                if (_CmpGeometry.IsVerticalSingleDuct(d))
                {
                  // ダクトサイズ
                  Revit.DB.Parameter parCS = d.get_Parameter(Revit.DB.BuiltInParameter.RBS_CALCULATED_SIZE);
                  string cS = parCS != null ? parCS.AsString() : "";

                  if (calSize != cS)
                  {
                    // 交点を求める farPoint1
                    Revit.DB.XYZ _p0 = null;
                    Revit.DB.XYZ _p1 = null;

                    _CmpGeometry.GetNearLinesPoints(_CmpGeometry.GetDuctLine(duct), _CmpGeometry.GetDuctLine(d), ref _p0, ref _p1);

                    farPoint1 = (farPoint1 + _p1) / 2;

                    continue;
                  }

                  // ダクトシステム名
                  Revit.DB.Parameter parSN = d.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
                  string strSN = parSN != null ? parSN.AsString() : "";

                  if (strSystemName != strSN)
                  {
                    continue;
                  }

                  // 仕様パラメータ比較
                  if (_CmpParameters.CompareDuctShiyo(duct, d) == false)
                  {
                    continue;
                  }

                  isHaveNext = true;

                  ret.Add(d);
                  ids.Add(d.Id.ToString());

                  // 連続する配管の継手
                  Revit.DB.Element cnct1 = null;
                  Revit.DB.Element cnct2 = null;

                  Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

                  // 遠い端点
                  if (_CmpGeometry.Distance(farPoint1, l.GetEndPoint(0)) > _CmpGeometry.Distance(farPoint1, l.GetEndPoint(1)))
                  {
                    farPoint1 = l.GetEndPoint(0);
                  }
                  else
                  {
                    farPoint1 = l.GetEndPoint(1);
                  }

                  GetConnectorOwner(d, ref cnct1, ref cnct2);
                  //_CmpElements.GetConnectorOwner(d, ref cnct1, ref cnct2);

                  // 逆側に続くか
                  if (cnct1 != null &&
                      cnct2 != null &&
                      IsDuctOrFitting(cnct1) &&
                      IsDuctOrFitting(cnct2))
                  {
                    if (connector1.Id.ToString() == cnct1.Id.ToString())
                    {
                      connector1 = cnct2;
                    }
                    else if (connector1.Id.ToString() == cnct2.Id.ToString())
                    {
                      connector1 = cnct1;
                    }
                  }
                  else
                  {
                    isHaveNext = false;
                  }

                  break;
                }
                else
                {
                  if (isHaveNext == false)
                  {
                    Revit.DB.XYZ p0 = line.GetEndPoint(0);

                    Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

                    double z = _CmpGeometry.GetExtLineZPoint(l, p0.X, p0.Y);

                    Revit.DB.XYZ extPnt = new Revit.DB.XYZ(p0.X, p0.Y, z);

                    farPoint1 = extPnt;
                  }
                }
              }
            }

          }
          // 1つ
          else
          {
            isHaveNext = false;
            break;
          }
        }
      }

      #endregion

      #region 継手2側

      isHaveNext = true;

      if (connector2 != null &&
          IsDuctOrFitting(connector2))
      {
        while (isHaveNext)
        {
          // 連続する要素
          Collections.Generic.IList<Revit.DB.Mechanical.Duct> sameCnctDucts = GetSameConnectorDuct(connector2);

          if (isQuantity)
          {
            //// 単管処理
            //#region 単管処理

            //Revit.DB.FamilyInstance famInsDumper = connector2 as Revit.DB.FamilyInstance;
            //Revit.DB.FamilySymbol famSymDumper = famInsDumper.Symbol;

            //// 呼称
            //string strKosyo = "";

            //if (famSymDumper != null)
            //{
            //    Revit.DB.Parameter parKosyo = famSymDumper.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KOSYO"));

            //    if (parKosyo != null && parKosyo.HasValue)
            //    {
            //        strKosyo = parKosyo.AsString();
            //    }
            //}

            //// 単管有無
            //string strTankan = GetTankanExist(strKosyo);
            //bool tankanExist = strTankan == "有" ? true : false;

            //Revit.DB.FamilyInstance retDumper = null;
            //Revit.DB.Mechanical.Duct retDuct1 = null;
            //Revit.DB.Mechanical.Duct retDuct2 = null;
            //Revit.DB.Mechanical.Duct retDuct3 = null;

            //// 単管有り
            //if (tankanExist)
            //{
            //    retDumper = famInsDumper;
            //    if (sameCnctDucts.Count > 0)
            //    {
            //        retDuct1 = sameCnctDucts[0];
            //    }
            //    if (sameCnctDucts.Count > 1)
            //    {
            //        retDuct2 = sameCnctDucts[1];
            //    }
            //    if (sameCnctDucts.Count > 2)
            //    {
            //        retDuct1 = sameCnctDucts[2];
            //    }

            //    //              SetTankanExportTable(retDumper, retDuct1, retDuct2, retDuct3);
            //}

            //#endregion
          }

          // 2つ以上(縦は上下1つずつ)
          if (sameCnctDucts.Count > 1)
          {
            isHaveNext = false;

            foreach (Revit.DB.Mechanical.Duct d in sameCnctDucts)
            {
              if (inViewDuctId.Contains(d.Id.ToString()) == false)
              {
                continue;
              }

              if (ids.Contains(d.Id.ToString()) == false)
              {
                if (_CmpGeometry.IsVerticalSingleDuct(d))
                {
                  // ダクトサイズ
                  Revit.DB.Parameter parCS = d.get_Parameter(Revit.DB.BuiltInParameter.RBS_CALCULATED_SIZE);
                  string cS = parCS != null ? parCS.AsString() : "";

                  if (calSize != cS)
                  {
                    // 交点を求める farPoint1
                    Revit.DB.XYZ _p0 = null;
                    Revit.DB.XYZ _p1 = null;

                    _CmpGeometry.GetNearLinesPoints(_CmpGeometry.GetDuctLine(duct), _CmpGeometry.GetDuctLine(d), ref _p0, ref _p1);

                    farPoint2 = (farPoint2 + _p1) / 2;

                    continue;
                  }

                  // ダクトシステム名
                  Revit.DB.Parameter parSN = d.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
                  string strSN = parSN != null ? parSN.AsString() : "";

                  if (strSystemName != strSN)
                  {
                    continue;
                  }

                  // 仕様パラメータ比較
                  if (_CmpParameters.CompareDuctShiyo(duct, d) == false)
                  {
                    continue;
                  }

                  isHaveNext = true;

                  ret.Add(d);
                  ids.Add(d.Id.ToString());

                  // 連続する配管の継手
                  Revit.DB.Element cnct1 = null;
                  Revit.DB.Element cnct2 = null;

                  Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

                  // 遠い端点
                  if (_CmpGeometry.Distance(farPoint2, l.GetEndPoint(0)) > _CmpGeometry.Distance(farPoint2, l.GetEndPoint(1)))
                  {
                    farPoint2 = l.GetEndPoint(0);
                  }
                  else
                  {
                    farPoint2 = l.GetEndPoint(1);
                  }

                  GetConnectorOwner(d, ref cnct1, ref cnct2);
                  // _CmpElements.GetConnectorOwner(d, ref cnct1, ref cnct2);

                  // 逆側に続くか

                  if (cnct1 != null &&
                      cnct2 != null &&
                      IsDuctOrFitting(cnct1) &&
                      IsDuctOrFitting(cnct2))
                  {
                    if (connector2.Id.ToString() == cnct1.Id.ToString())
                    {
                      connector2 = cnct2;
                    }
                    else if (connector2.Id.ToString() == cnct2.Id.ToString())
                    {
                      connector2 = cnct1;
                    }
                  }
                  else
                  {
                    isHaveNext = false;
                  }

                  break;
                }
                else
                {
                  if (isHaveNext == false)
                  {
                    Revit.DB.XYZ p0 = line.GetEndPoint(0);

                    Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

                    double z = _CmpGeometry.GetExtLineZPoint(l, p0.X, p0.Y);

                    Revit.DB.XYZ extPnt = new Revit.DB.XYZ(p0.X, p0.Y, z);

                    farPoint2 = extPnt;
                  }
                }
              }
            }

          }
          // 1つ
          else
          {
            isHaveNext = false;
            break;
          }
        }
      }

      #endregion

      return ret;
    }

    /// ================================================================================
    /// <summary>配管のソート</summary>
    /// 
    /// <param name="pipes"   >配管</param>
    /// <param name="farPnt1" >端点1</param>
    /// <param name="farPnt2" >端点2</param>
    /// 
    /// <history>2014/07/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Plumbing.Pipe> SortPipes(Collections.Generic.IList<Revit.DB.Plumbing.Pipe> pipes,
                                                                Revit.DB.XYZ farPnt1)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Plumbing.Pipe> ret = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();

      // 最端の配管
      Revit.DB.Plumbing.Pipe pipe = null;
      Revit.DB.XYZ endPnt = null;

      foreach (Revit.DB.Plumbing.Pipe p in pipes)
      {
        Revit.DB.Line line = _CmpGeometry.GetPipeLine(p);
        Revit.DB.XYZ p0 = line.GetEndPoint(0);
        Revit.DB.XYZ p1 = line.GetEndPoint(1);

        if (_CmpGeometry.ToHalfAdjust(_CmpGeometry.Distance2D(p0, farPnt1), -9) == 0)
        {
          pipe = p;
          endPnt = p0;

          break;
        }
        if (_CmpGeometry.ToHalfAdjust(_CmpGeometry.Distance2D(p1, farPnt1), -9) == 0)
        {
          pipe = p;
          endPnt = p1;

          break;
        }
      }

      // 端点が一致しない場合
      if (pipe == null)
      {
        endPnt = farPnt1;
        pipe = _CmpGeometry.GetNearEndPntPipe(pipe, ref endPnt, pipes);
      }

      ret.Add(pipe);


      Collections.Generic.IList<Revit.DB.Plumbing.Pipe> ary = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();
      foreach (Revit.DB.Plumbing.Pipe p in pipes)
      {
        ary.Add(p);
      }

      ary.Remove(pipe);


      while (ret.Count < pipes.Count)
      {
        // 基準配管の端点に一番近い端点を持つ配管
        Revit.DB.Plumbing.Pipe p = _CmpGeometry.GetNearEndPntPipe(pipe, ref endPnt, ary);

        ary.Remove(p);

        ret.Add(p);

        pipe = p;
      }


      return ret;
    }

    /// ================================================================================
    /// <summary>ダクトのソート</summary>
    /// 
    /// <param name="ducts"   >ダクト</param>
    /// <param name="farPnt1" >端点1</param>
    /// <param name="farPnt2" >端点2</param>
    /// 
    /// <history>2014/07/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Duct> SortDucts(Collections.Generic.IList<Revit.DB.Mechanical.Duct> ducts,
                                                                  Revit.DB.XYZ farPnt1)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Mechanical.Duct> ret = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();

      // 最端の配管
      Revit.DB.Mechanical.Duct duct = null;
      Revit.DB.XYZ endPnt = null;

      foreach (Revit.DB.Mechanical.Duct d in ducts)
      {
        Revit.DB.Line line = _CmpGeometry.GetDuctLine(d);
        Revit.DB.XYZ p0 = line.GetEndPoint(0);
        Revit.DB.XYZ p1 = line.GetEndPoint(1);

        if (_CmpGeometry.ToHalfAdjust(_CmpGeometry.Distance2D(p0, farPnt1), -9) == 0)
        {
          duct = d;
          endPnt = line.GetEndPoint(0);

          break;
        }
        if (_CmpGeometry.ToHalfAdjust(_CmpGeometry.Distance2D(p1, farPnt1), -9) == 0)
        {
          duct = d;
          endPnt = line.GetEndPoint(1);

          break;
        }
      }


      ret.Add(duct);


      Collections.Generic.IList<Revit.DB.Mechanical.Duct> ary = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();
      foreach (Revit.DB.Mechanical.Duct d in ducts)
      {
        ary.Add(d);
      }

      ary.Remove(duct);


      while (ret.Count < ducts.Count)
      {
        // 基準配管の端点に一番近い端点を持つ配管
        Revit.DB.Mechanical.Duct d = _CmpGeometry.GetNearEndPntDuct(duct, ref endPnt, ary);

        ary.Remove(d);

        ret.Add(d);

        duct = d;
      }


      return ret;
    }

    /// ================================================================================
    /// <summary>配管分岐点追加</summary>
    /// 
    /// <param name="pnts"  >基準点</param>
    /// <param name="pipes" >配管</param>
    /// 
    /// <history>2014/10/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetPipeJunctionPoint(Collections.Generic.IList<Revit.DB.XYZ> pnts,
                                                                 Collections.Generic.IList<Revit.DB.Plumbing.Pipe> pipes)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      // 始点
      Revit.DB.XYZ p0 = pnts[0];

      // 基準
      foreach (Revit.DB.XYZ pnt in pnts)
      {
        ret.Add(pnt);
      }

      Revit.DB.Line line = Revit.DB.Line.CreateBound(pnts[0], pnts[pnts.Count - 1]);

      // 配管分岐点追加
      foreach (Revit.DB.Plumbing.Pipe pipe in pipes)
      {
        Revit.DB.ConnectorManager cnctMgr = pipe.ConnectorManager;

        Revit.DB.ConnectorSet cnctSet = cnctMgr.Connectors;

        foreach (Revit.DB.Connector cnct in cnctSet)
        {
          Revit.DB.ConnectorSet cs = cnct.AllRefs;

          foreach (Revit.DB.Connector c in cs)
          {
            // 継手または付属品
            if ((c.Owner.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeFitting).ToString()) ||
                 c.Owner.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeAccessory).ToString())))
            {
              // 繋がっている要素
              Collections.Generic.IList<Revit.DB.Plumbing.Pipe> cnctPipes = GetSameConnectorPipe(c.Owner);

              if (cnctPipes.Count > 1)
              {
                foreach (Revit.DB.Plumbing.Pipe p in cnctPipes)
                {
                  if (pipe.Id.ToString() == p.Id.ToString())
                  {
                    continue;
                  }

                  Revit.DB.Line l = _CmpGeometry.GetPipeLine(p);

                  if (_CmpGeometry.IsTwoLineParallelZeroDIstance_NoSlope(line, l) == false)
                  {
                    Revit.DB.XYZ cross = _CmpGeometry.TwoLineCrossPnt(line, l);

                    // 縦管
                    if (_CmpGeometry.IsVerticalSinglePipe(p))
                    {
                      double midZ = _CmpGeometry.GetZPointOnLine(line, l.GetEndPoint(0).X, l.GetEndPoint(0).Y);

                      cross = new Revit.DB.XYZ(l.GetEndPoint(0).X, l.GetEndPoint(0).Y, midZ);
                    }

                    if (cross != null)
                    {
                      //if (_CmpGeometry.IsSameDirectionOutSidePoint(pnts[0], pnts[pnts.Count - 1], cross))
                      {
                        ret.Add(cross);
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }

      // ソート
      ret = _CmpGeometry.SortNearPoints(ret, p0);

      return ret;
    }

    /// ================================================================================
    /// <summary>ダクト分岐点追加</summary>
    /// 
    /// <param name="pnts"  >基準点</param>
    /// <param name="ducts" >ダクト</param>
    /// 
    /// <history>2014/10/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetDuctJunctionPoint(Collections.Generic.IList<Revit.DB.XYZ> pnts,
                                                                 Collections.Generic.IList<Revit.DB.Mechanical.Duct> ducts)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      // 始点
      Revit.DB.XYZ p0 = pnts[0];

      // 基準
      foreach (Revit.DB.XYZ pnt in pnts)
      {
        ret.Add(pnt);
      }

      Revit.DB.Line line = Revit.DB.Line.CreateBound(pnts[0], pnts[pnts.Count - 1]);

      // ダクト端点
      Collections.Generic.IList<Revit.DB.XYZ> endPnts = new Collections.Generic.List<Revit.DB.XYZ>();


      // ダクト分岐点追加
      foreach (Revit.DB.Mechanical.Duct duct in ducts)
      {
        Revit.DB.Line dl = _CmpGeometry.GetDuctLine(duct);
        endPnts.Add(dl.GetEndPoint(0));
        endPnts.Add(dl.GetEndPoint(1));

        Revit.DB.ConnectorManager cnctMgr = duct.ConnectorManager;

        Revit.DB.ConnectorSet cnctSet = cnctMgr.Connectors;

        foreach (Revit.DB.Connector cnct in cnctSet)
        {
          Revit.DB.ConnectorSet cs = cnct.AllRefs;

          foreach (Revit.DB.Connector c in cs)
          {
            // 継手または付属品
            if ((c.Owner.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctFitting).ToString()) ||
                 c.Owner.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctAccessory).ToString())))
            {
              // 繋がっている要素
              Collections.Generic.IList<Revit.DB.Mechanical.Duct> cnctDucts = GetSameConnectorDuct(c.Owner);

              if (cnctDucts.Count > 1)
              {
                foreach (Revit.DB.Mechanical.Duct d in cnctDucts)
                {
                  if (duct.Id.ToString() == d.Id.ToString())
                  {
                    continue;
                  }

                  Revit.DB.Line l = _CmpGeometry.GetDuctLine(d);

                  if (_CmpGeometry.IsTwoLineParallelZeroDIstance_NoSlope(line, l) == false)
                  {
                    Revit.DB.XYZ cross = _CmpGeometry.TwoLineCrossPnt(line, l);

                    // 縦管
                    if (_CmpGeometry.IsVerticalSingleDuct(d))
                    {
                      double midZ = _CmpGeometry.GetZPointOnLine(line, l.GetEndPoint(0).X, l.GetEndPoint(0).Y);

                      cross = new Revit.DB.XYZ(l.GetEndPoint(0).X, l.GetEndPoint(0).Y, midZ);
                    }

                    if (cross != null)
                    {
                      //if (_CmpGeometry.IsSameDirectionOutSidePoint(pnts[0], pnts[pnts.Count - 1], cross))
                      {
                        if (_CmpGeometry.IsPointOnLine(line, cross))
                        {
                          ret.Add(cross);
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }

      // ソート
      ret = _CmpGeometry.SortNearPoints(ret, p0);

      //// 2014/10/29
      //// 端点追加
      //foreach (Revit.DB.XYZ ep in endPnts)
      //{
      //  // 始点の外
      //  if (_CmpGeometry.IsSameDirectionOutSidePoint(ret[ret.Count - 1], ret[0], ep))
      //  {
      //    ret.Insert(0, ep);
      //  }
      //  // 終点の外
      //  if (_CmpGeometry.IsSameDirectionOutSidePoint(ret[0], ret[ret.Count - 1], ep))
      //  {
      //    ret.Add(ep);
      //  }
      //}

      return ret;
    }

    /// ================================================================================
    /// <summary>スペース交点</summary>
    /// 
    /// <history>2014/09/25 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.XYZ> GetSpaceBndryCrossingLine(Collections.Generic.IList<Revit.DB.XYZ> linePnts,
                                                                      Collections.Generic.IList<Revit.DB.Mechanical.Space> spaces)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.XYZ> ret = new Collections.Generic.List<Revit.DB.XYZ>();

      // 始点終点
      Revit.DB.XYZ p0 = linePnts[0];
      Revit.DB.XYZ p1 = linePnts[linePnts.Count - 1];// linePnts[1];

      foreach (Revit.DB.XYZ p in linePnts)
      {
        ret.Add(p);
      }

      Revit.DB.Line line = Revit.DB.Line.CreateBound(p0, p1);

      // スペース境界
      Collections.Generic.IDictionary<Revit.DB.Mechanical.Space, Collections.Generic.IList<Revit.DB.Curve>> dicSpaceBndryCrv = _CmpGeometry.SpaceBndryCrv;

      foreach (Revit.DB.Mechanical.Space space in dicSpaceBndryCrv.Keys)
      {
        // スペース境界線
        Collections.Generic.IList<Revit.DB.Curve> bndryCrvs = dicSpaceBndryCrv[space];// _CmpGeometry.GetSpaceBndryCrv(space, 1);

        // スペース高さ
        double topHeight = _CmpParameters.GetSpaceTopHeight(space);
        double btmHeight = _CmpParameters.GetSpaceBottomHeight(space);

        if ((topHeight < p0.Z && topHeight < p1.Z) ||
               btmHeight > p0.Z && btmHeight > p1.Z)
        {
          continue;
        }

        // 境界基準・高さ補正
        #region 境界基準・高さ補正

        foreach (Revit.DB.Curve crv in bndryCrvs)
        {
          // 直線
          if (crv.IsCyclic == false)
          {
            Revit.DB.Line l = crv as Revit.DB.Line;
            Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(line, l, 1);

            if (cross != null)
            {
              // 高さ範囲内か
              double midZ = _CmpGeometry.GetZPointOnLine(line, cross.X, cross.Y);

              Revit.DB.XYZ midP = _CmpGeometry.GetMidPointOnLine(line, midZ);

              if (midZ <= topHeight && midZ >= btmHeight)
              {
                Revit.DB.XYZ p = new Revit.DB.XYZ(cross.X, cross.Y, midZ);
                ret.Add(p);
              }
            }
          }
          // 曲線
          else
          {
            Revit.DB.Arc arc = crv as Revit.DB.Arc;
            Collections.Generic.IList<Revit.DB.XYZ> crosses = _CmpGeometry.GetXYCrossPoint(line, arc);

            foreach (Revit.DB.XYZ cross in crosses)
            {
              // 高さ範囲ないか
              double midZ = _CmpGeometry.GetZPointOnLine(line, cross.X, cross.Y);

              Revit.DB.XYZ midP = _CmpGeometry.GetMidPointOnLine(line, midZ);

              if (midZ <= topHeight && midZ >= btmHeight)
              {
                Revit.DB.XYZ p = new Revit.DB.XYZ(cross.X, cross.Y, midZ);
                ret.Add(p);
              }
            }
          }
        }

        #endregion


        // 高さ基準・境界補正
        #region 高さ基準・境界補正

        // 配管上のスペース上端高さ
        Revit.DB.XYZ midTop = _CmpGeometry.GetMidPointOnLine(line, topHeight);

        // 交点が配管範囲内
        if (midTop != null)
        {
          // スペース内に含まれるか

          Revit.DB.LocationPoint locPnt = space.Location as Revit.DB.LocationPoint;
          Revit.DB.XYZ point = locPnt.Point;


          // Z座標と配置座標(境界内)を結んだ線分が境界と交差しなければ内部
          Revit.DB.Line l = Revit.DB.Line.CreateBound(midTop, point);

          // 交差しない = スペース内
          bool isCross = _CmpGeometry.IsOutCurves(l, bndryCrvs);

          if (isCross == false)
          {
            // 完全内部の可能性

            // 高さ範囲内
            if (p0.Z < topHeight &&
                p1.Z < topHeight &&
                p0.Z > btmHeight &&
                p1.Z > btmHeight)
            {
              Revit.DB.Line l0 = Revit.DB.Line.CreateBound(p0, point);
              Revit.DB.Line l1 = Revit.DB.Line.CreateBound(p1, point);

              bool isCrossing0 = _CmpGeometry.IsOutCurves(l0, bndryCrvs);
              bool isCrossing1 = _CmpGeometry.IsOutCurves(l1, bndryCrvs);

              // 境界と交差しない
              if (isCrossing0 == false &&
                  isCrossing1 == false)
              {
                if (_CmpGeometry.Distance(p0, p1) > _CmpParameters.LineMinLength)
                {
                  ret.Add(midTop);
                }
              }
            }

            //ret.Add(midTop);
          }
        }

        // 配管上のスペース下端高さ
        Revit.DB.XYZ midBtm = _CmpGeometry.GetMidPointOnLine(line, btmHeight);

        // 交点が配管範囲内
        if (midBtm != null)
        {
          // スペース内に含まれるか

          Revit.DB.LocationPoint locPnt = space.Location as Revit.DB.LocationPoint;
          Revit.DB.XYZ point = locPnt.Point;

          // Z座標と配置座標(境界内)を結んだ線分が境界と交差しなければ内部
          Revit.DB.Line l = Revit.DB.Line.CreateBound(midBtm, point);

          // 交差しない = スペース内
          bool isCross = _CmpGeometry.IsOutCurves(l, bndryCrvs);

          if (isCross == false)
          {
            // 完全内部の可能性

            // 高さ範囲内
            if (p0.Z < topHeight &&
                p1.Z < topHeight &&
                p0.Z > btmHeight &&
                p1.Z > btmHeight)
            {
              Revit.DB.Line l0 = Revit.DB.Line.CreateBound(p0, point);
              Revit.DB.Line l1 = Revit.DB.Line.CreateBound(p1, point);

              bool isCrossing0 = _CmpGeometry.IsOutCurves(l0, bndryCrvs);
              bool isCrossing1 = _CmpGeometry.IsOutCurves(l1, bndryCrvs);

              // 境界と交差しない
              if (isCrossing0 == false &&
                  isCrossing1 == false)
              {
                if (_CmpGeometry.Distance(p0, p1) > _CmpParameters.LineMinLength)
                {
                  ret.Add(midBtm);
                }
              }
            }

            //ret.Add(midBtm);
          }
        }

        #endregion

        //bool isCrossing = false;

        #region
        //// 境界基準・高さ補正
        //#region 境界基準・高さ補正

        //foreach (Revit.DB.Curve crv in bndryCrvs)
        //{
        //  // 直線
        //  if (crv.IsCyclic == false)
        //  {
        //    Revit.DB.Line l = crv as Revit.DB.Line;
        //    Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(line, l, 1);

        //    if (cross != null)
        //    {
        //      // 高さ範囲ないか
        //      double midZ = _CmpGeometry.GetZPointOnLine(line, cross.X, cross.Y);

        //      if (midZ <= topHeight && midZ >= btmHeight)
        //      {
        //        // 交点追加
        //        ret.Add(cross);

        //        isCrossing = true;
        //      }
        //    }
        //  }
        //  // 曲線
        //  else
        //  {
        //    Revit.DB.Arc arc = crv as Revit.DB.Arc;
        //    Collections.Generic.IList<Revit.DB.XYZ> crosses = _CmpGeometry.GetXYCrossPoint(line, arc);

        //    foreach (Revit.DB.XYZ cross in crosses)
        //    {
        //      // 高さ範囲ないか
        //      double midZ = _CmpGeometry.GetZPointOnLine(line, cross.X, cross.Y);

        //      if (midZ <= topHeight && midZ >= btmHeight)
        //      {
        //        // 交点追加
        //        ret.Add(cross);

        //        isCrossing = true;
        //      }
        //    }
        //  }
        //}

        //#endregion

        //// 高さ基準・境界補正
        //#region 高さ基準・境界補正

        //if (isCrossing == false)
        //{
        //  // 配管上の上端高さ
        //  Revit.DB.XYZ mid = _CmpGeometry.GetMidPointOnLine(line, topHeight);

        //  if (mid != null)
        //  {
        //    // スペース内に含まれるか

        //    // 交差していないので完全内部か

        //    Revit.DB.LocationPoint locPnt = space.Location as Revit.DB.LocationPoint;
        //    Revit.DB.XYZ point = locPnt.Point;

        //    // 端点(どちらかでいい)と配置座標(境界内)を結んだ線分が境界と交差しなければ内部
        //    Revit.DB.Line l = Revit.DB.Line.CreateBound(p0, point);

        //    bool isCross = _CmpGeometry.IsCrossing(l, bndryCrvs);

        //    if (isCross == false)
        //    {
        //      ret.Add(mid);
        //    }
        //  }

        //}

        //#endregion
        #endregion
      }

      // 始点側からソート
      ret = _CmpGeometry.SortNearPoints(ret, p0);

      return ret;
    }

    /// ================================================================================
    /// <summary>線分とスペースの交差端点</summary>
    /// 
    /// <param name="line"  >線分</param>
    /// 
    /// <history><p>2014/10/07 Created GSA, Inc. Ryo Kuroda</p>
    ///           <p>2015/01/20 Modified GSA, Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>> GetCrossingSpaceAndPoint(Revit.DB.Line line)
    {
      // 戻り値
      Collections.Generic.IDictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>> ret =
        new Collections.Generic.Dictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>>();

      Revit.DB.XYZ p0 = line.GetEndPoint(0);
      Revit.DB.XYZ p1 = line.GetEndPoint(1);

      // スペース境界
      Collections.Generic.IDictionary<Revit.DB.Mechanical.Space, Collections.Generic.IList<Revit.DB.Curve>> dicSpaceBndryCrv = _CmpGeometry.SpaceBndryCrv;

      foreach (Revit.DB.Mechanical.Space space in dicSpaceBndryCrv.Keys)
      {
        bool isInSpace = false;

        Collections.Generic.IList<Revit.DB.XYZ> retList = new Collections.Generic.List<Revit.DB.XYZ>();

        // 配置されていないスペース
        if (space.Location == null)
        {
          continue;
        }
        
        // スペース境界線
        Collections.Generic.IList<Revit.DB.Curve> bndryCrvs = dicSpaceBndryCrv[space];// _CmpGeometry.GetSpaceBndryCrv(space, 1);

        if (bndryCrvs.Count < 1)
        {
          continue;
        }

        // スペース高さ
        double topHeight = _CmpParameters.GetSpaceTopElev(space);
        double btmHeight = _CmpParameters.GetSpaceBtmElev(space);

        if ((topHeight < p0.Z && topHeight < p1.Z) ||
             btmHeight > p0.Z && btmHeight > p1.Z)
        {
          continue;
        }

        // 交点
        Collections.Generic.IList<Revit.DB.XYZ> crossPnts = new Collections.Generic.List<Revit.DB.XYZ>();

        // 境界基準・高さ補正
        #region 境界基準・高さ補正

        foreach (Revit.DB.Curve crv in bndryCrvs)
        {
          // 直線
          if (crv.IsCyclic == false)
          {
            Revit.DB.Line l = crv as Revit.DB.Line;
            Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(line, l, 1);

            if (cross != null)
            {
              // 高さ範囲内か
              double midZ = _CmpGeometry.GetZPointOnLine(line, cross.X, cross.Y);

              if (midZ <= topHeight && midZ >= btmHeight)
              {
                Revit.DB.XYZ p = new Revit.DB.XYZ(cross.X, cross.Y, midZ);
                crossPnts.Add(p);
              }
            }
          }
          // 曲線
          else
          {
            Revit.DB.Arc arc = crv as Revit.DB.Arc;
            Collections.Generic.IList<Revit.DB.XYZ> crosses = _CmpGeometry.GetXYCrossPoint(line, arc);

            foreach (Revit.DB.XYZ cross in crosses)
            {
              // 高さ範囲ないか
              double midZ = _CmpGeometry.GetZPointOnLine(line, cross.X, cross.Y);

              if (midZ <= topHeight && midZ >= btmHeight)
              {
                Revit.DB.XYZ p = new Revit.DB.XYZ(cross.X, cross.Y, midZ);
                crossPnts.Add(p);
              }
            }
          }
        }

        #endregion


        // 高さ基準・境界補正
        #region 高さ基準・境界補正

        // 配管上のスペース上端高さ
        Revit.DB.XYZ midTop = _CmpGeometry.GetMidPointOnLine(line, topHeight);

        // 交点が配管範囲内
        if (midTop != null)
        {
          // スペース内に含まれるか

          Revit.DB.LocationPoint locPnt = space.Location as Revit.DB.LocationPoint;
          Revit.DB.XYZ point = locPnt.Point;


          // Z座標と配置座標(境界内)を結んだ線分が境界と交差しなければ内部
          Revit.DB.Line l = Revit.DB.Line.CreateBound(midTop, point);

          // 交差しない = スペース内
          bool isCross = _CmpGeometry.IsOutCurves(l, bndryCrvs);

          if (isCross == false)
          {
            crossPnts.Add(midTop);
          }
        }

        // 配管上のスペース下端高さ
        Revit.DB.XYZ midBtm = _CmpGeometry.GetMidPointOnLine(line, btmHeight);

        // 交点が配管範囲内
        if (midBtm != null)
        {
          // スペース内に含まれるか

          Revit.DB.LocationPoint locPnt = space.Location as Revit.DB.LocationPoint;
          Revit.DB.XYZ point = locPnt.Point;

          // Z座標と配置座標(境界内)を結んだ線分が境界と交差しなければ内部
          Revit.DB.Line l = Revit.DB.Line.CreateBound(midBtm, point);

          // 交差しない = スペース内
          bool isCross = _CmpGeometry.IsOutCurves(l, bndryCrvs);

          if (isCross == false)
          {
            crossPnts.Add(midBtm);
          }
        }

        #endregion


        if (crossPnts.Count == 0)
        {
          // 完全内部の可能性
          #region
          // 高さ範囲内
          if (p0.Z < topHeight &&
              p1.Z < topHeight &&
              p0.Z > btmHeight &&
              p1.Z > btmHeight)
          {
            Revit.DB.LocationPoint locPnt = space.Location as Revit.DB.LocationPoint;
            Revit.DB.XYZ point = locPnt.Point;

            Revit.DB.Line l0 = Revit.DB.Line.CreateBound(p0, point);
            Revit.DB.Line l1 = Revit.DB.Line.CreateBound(p1, point);

            bool isCrossing0 = _CmpGeometry.IsOutCurves(l0, bndryCrvs);
            bool isCrossing1 = _CmpGeometry.IsOutCurves(l1, bndryCrvs);

            // 境界と交差しない
            if (isCrossing0 == false &&
                isCrossing1 == false)
            {
              if (_CmpGeometry.Distance(p0, p1) > _CmpParameters.LineMinLength)
              {
                if (ret.ContainsKey(space.Id))
                {
                  retList.Add(p0);
                  retList.Add(p1);
                  ret[space.Id].Add(retList);
                }
                else
                {
                  Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listList = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
                  retList.Add(p0);
                  retList.Add(p1);
                  listList.Add(retList);

                  ret.Add(space.Id, listList);
                }

                isInSpace = true;
              }
            }
          }
          #endregion
        }
        else if (crossPnts.Count == 1)
        {
          #region
          Revit.DB.XYZ pnt = crossPnts[0];

          // 始点または終点のどちら側か
          // 境界線の内部に含まれる方 = 交差しない方

          Revit.DB.LocationPoint locPnt = space.Location as Revit.DB.LocationPoint;
          Revit.DB.XYZ point = locPnt.Point;

          Revit.DB.Line l0 = Revit.DB.Line.CreateBound(p0, point);
          Revit.DB.Line l1 = Revit.DB.Line.CreateBound(p1, point);

          // 交差しない方

          // XY交差
          bool isCrossing0 = _CmpGeometry.IsOutCurves(l0, bndryCrvs);
          bool isCrossing1 = _CmpGeometry.IsOutCurves(l1, bndryCrvs);

          // 高さ範囲
          if (p0.Z < topHeight && p0.Z > btmHeight)
          {
            if (_CmpGeometry.Distance(p0, pnt) > _CmpParameters.LineMinLength)
            {
              if (isCrossing0 == false)
              {
                if (ret.ContainsKey(space.Id))
                {
                  retList.Add(p0);
                  retList.Add(pnt);
                  ret[space.Id].Add(retList);
                }
                else
                {
                  Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listList = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
                  retList.Add(p0);
                  retList.Add(pnt);
                  listList.Add(retList);

                  ret.Add(space.Id, listList);
                }
              }
            }
          }
          if (p1.Z < topHeight && p1.Z > btmHeight)
          {
            if (_CmpGeometry.Distance(p1, pnt) > _CmpParameters.LineMinLength)
            {
              if (isCrossing1 == false)
              {
                if (ret.ContainsKey(space.Id))
                {
                  retList.Add(p1);
                  retList.Add(pnt);
                  ret[space.Id].Add(retList);
                }
                else
                {
                  Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listList = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
                  retList.Add(p1);
                  retList.Add(pnt);
                  listList.Add(retList);

                  ret.Add(space.Id, listList);
                }
              }
            }
          }
          #endregion
        }
        else if (crossPnts.Count == 2)
        {
          // 貫通しているので2点間距離
          #region
          Revit.DB.XYZ pnt0 = crossPnts[0];
          Revit.DB.XYZ pnt1 = crossPnts[1];

          if (_CmpGeometry.Distance(pnt0, pnt1) > _CmpParameters.LineMinLength)
          {
            if (ret.ContainsKey(space.Id))
            {
              retList.Add(pnt0);
              retList.Add(pnt1);
              ret[space.Id].Add(retList);
            }
            else
            {
              Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listList = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
              retList.Add(pnt0);
              retList.Add(pnt1);
              listList.Add(retList);

              ret.Add(space.Id, listList);
            }
          }
          #endregion
        }
        else if (crossPnts.Count >= 2)
        {
          // 出入り
          #region
          // 始点終点追加
          crossPnts.Insert(0, p0);
          crossPnts.Add(p1);

          for (int i = 0; i < crossPnts.Count; ++i)
          {
            if (i == crossPnts.Count - 1)
            {
              break;
            }

            Revit.DB.XYZ pnt0 = crossPnts[i];
            Revit.DB.XYZ pnt1 = crossPnts[i + 1];



          }
          #endregion
        }

        if (isInSpace)
        {
          break;
        }
      }

      // スペースに含まれない範囲
      #region

      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> _PntsAryAry = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();

      foreach (Revit.DB.ElementId space in ret.Keys)
      {
        var pntAryAry = ret[space];

        foreach (var pntAry in pntAryAry)
        {
          _PntsAryAry.Add(pntAry);
        }
      }

      var notIncludePnts = _CmpGeometry.GetNotIncludeLinePoint(line, _PntsAryAry);

      if (notIncludePnts.Count > 0)
      {
        Revit.DB.ElementId eid = new Revit.DB.ElementId(-1);
        ret.Add(eid, notIncludePnts);
      }

      #endregion

      return ret;
    }

    /// ================================================================================
    /// <summary>フレキシブルダクトの含まれるスペース</summary>
    /// 
    /// <param name="flexDuct">フレキシブルダクト</param>
    /// 
    /// <history><p>2015/12/22 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2017/07/21 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>> GetCrossingSpaceAndPoint(Revit.DB.Mechanical.FlexDuct flexDuct)
    {
      // 戻り値
      Collections.Generic.IDictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>> ret =
        new Collections.Generic.Dictionary<Revit.DB.ElementId, Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>>>();

      Revit.DB.LocationCurve locCrv = flexDuct.Location as Revit.DB.LocationCurve;

      // エルミートスプライン
      Revit.DB.HermiteSpline hrmtSpline = locCrv.Curve as Revit.DB.HermiteSpline;

      Collections.Generic.IList<Revit.DB.XYZ> hsPnts = hrmtSpline.Tessellate();

      if (hsPnts.Count < 2)
      {
        return ret;
      }

      // 座標
      //Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listPnts = _CmpGeometry.DivideMeterLength(flexDuct);
      Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listPnts = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
      Collections.Generic.IList<Revit.DB.XYZ> listPnt = new Collections.Generic.List<Revit.DB.XYZ>();

      Revit.DB.Parameter parLength = flexDuct.get_Parameter(Autodesk.Revit.DB.BuiltInParameter.CURVE_ELEM_LENGTH);
      double lenght = parLength.AsDouble();

      Revit.DB.XYZ fdP0 = locCrv.Curve.GetEndPoint(0);
      Revit.DB.XYZ fdP1 = fdP0 + new Revit.DB.XYZ(lenght, 0, 0);

      listPnt.Add(fdP0);
      listPnt.Add(fdP1);
      listPnts.Add(listPnt);

      int i = 1;

      // 始点側の最小線分のみで判定
      Revit.DB.XYZ p0 = hsPnts[0];
      Revit.DB.XYZ p1 = hsPnts[i];

      Revit.DB.Line line = null;

      while (line == null)
      {
        if (_CmpGeometry.Distance(p0, p1) > _CmpParameters.LineMinLength)
        {
          line = Revit.DB.Line.CreateBound(p0, p1);
        }
        else
        {
          i += 1;

          if (i == hsPnts.Count)
          {
            return ret;
          }

          p1 = hsPnts[i];
        }
      }

      // スペース境界
      Collections.Generic.IDictionary<Revit.DB.Mechanical.Space, Collections.Generic.IList<Revit.DB.Curve>> dicSpaceBndryCrv = _CmpGeometry.SpaceBndryCrv;

      foreach (Revit.DB.Mechanical.Space space in dicSpaceBndryCrv.Keys)
      {
        Revit.DB.Location spaceLoc = space.Location;

        // 配置されていないスペース
        if (spaceLoc == null)
        {
          continue;
        }

        // スペース座標
        Revit.DB.LocationPoint spaceLocPnt = spaceLoc as Revit.DB.LocationPoint;
        Revit.DB.XYZ spacePoint = spaceLocPnt.Point;

        // スペース境界線
        Collections.Generic.IList<Revit.DB.Curve> bndryCrvs = dicSpaceBndryCrv[space];

        if (bndryCrvs.Count < 1)
        {
          continue;
        }

        // スペース高さ
        double topHeight = _CmpParameters.GetSpaceTopElev(space);
        double btmHeight = _CmpParameters.GetSpaceBtmElev(space);

        // 両端とも上面より上または下面より下
        if ((topHeight < p0.Z && topHeight < p1.Z) ||
            (btmHeight > p0.Z && btmHeight > p1.Z))
        {
          continue;
        }

        // 交点
        Collections.Generic.IList<Revit.DB.XYZ> crossPnts = new Collections.Generic.List<Revit.DB.XYZ>();

        // 境界基準・高さ絞り込み
        #region 境界基準・高さ絞り込み

        foreach (Revit.DB.Curve crv in bndryCrvs)
        {
          // 直線
          if (crv.IsCyclic == false)
          {
            Revit.DB.Line l = crv as Revit.DB.Line;
            Revit.DB.XYZ cross = _CmpGeometry.CrossPointXY(line, l, 1);

            if (cross != null)
            {
              // 高さ範囲内か
              double midZ = _CmpGeometry.GetZPointOnLine(line, cross.X, cross.Y);

              if (midZ <= topHeight && midZ >= btmHeight)
              {
                Revit.DB.XYZ p = new Revit.DB.XYZ(cross.X, cross.Y, midZ);
                crossPnts.Add(p);
              }
            }
          }
          // 曲線
          else
          {
            Revit.DB.Arc arc = crv as Revit.DB.Arc;
            Collections.Generic.IList<Revit.DB.XYZ> crosses = _CmpGeometry.GetXYCrossPoint(line, arc);

            foreach (Revit.DB.XYZ cross in crosses)
            {
              // 高さ範囲内か
              double midZ = _CmpGeometry.GetZPointOnLine(line, cross.X, cross.Y);

              if (midZ <= topHeight && midZ >= btmHeight)
              {
                Revit.DB.XYZ p = new Revit.DB.XYZ(cross.X, cross.Y, midZ);
                crossPnts.Add(p);
              }
            }
          }
        }

        #endregion

        // 高さ基準・境界絞り込み
        #region 高さ基準・境界絞り込み

        // 配管上のスペース上端高さ
        Revit.DB.XYZ midTop = _CmpGeometry.GetMidPointOnLine(line, topHeight);

        // 交点が配管範囲内
        if (midTop != null)
        {
          // スペース内に含まれるか


          // Z座標と配置座標(境界内)を結んだ線分が境界と交差しなければ内部
          Revit.DB.Line l = Revit.DB.Line.CreateBound(midTop, spacePoint);

          // 交差しない = スペース内
          bool isCross = _CmpGeometry.IsOutCurves(l, bndryCrvs);

          if (isCross == false)
          {
            crossPnts.Add(midTop);
          }
        }

        // 配管上のスペース下端高さ
        Revit.DB.XYZ midBtm = _CmpGeometry.GetMidPointOnLine(line, btmHeight);

        // 交点が配管範囲内
        if (midBtm != null)
        {
          // スペース内に含まれるか

          // Z座標と配置座標(境界内)を結んだ線分が境界と交差しなければ内部
          Revit.DB.Line l = Revit.DB.Line.CreateBound(midBtm, spacePoint);

          // 交差しない = スペース内
          bool isCross = _CmpGeometry.IsOutCurves(l, bndryCrvs);

          if (isCross == false)
          {
            crossPnts.Add(midBtm);
          }
        }

        #endregion



        if (crossPnts.Count == 0)
        {
          // 完全内部の判定

          // 高さ範囲内

          if (p0.Z < topHeight &&
              p1.Z < topHeight &&
              p0.Z > btmHeight &&
              p1.Z > btmHeight)
          {
            // 端点とスペース座標をむすぶ線分
            Revit.DB.Line l0 = Revit.DB.Line.CreateBound(p0, spacePoint);
            Revit.DB.Line l1 = Revit.DB.Line.CreateBound(p1, spacePoint);

            bool isCrossing0 = _CmpGeometry.IsOutCurves(l0, bndryCrvs);
            bool isCrossing1 = _CmpGeometry.IsOutCurves(l1, bndryCrvs);

            // 境界と交差しない
            if (isCrossing0 == false && isCrossing1 == false)
            {
              if (ret.ContainsKey(space.Id))
              {
                //Collections.Generic.IList<Revit.DB.XYZ> pnts = new Collections.Generic.List<Revit.DB.XYZ>();
                //Revit.DB.XYZ ep = hsPnts[hsPnts.Count - 1];
                //pnts.Add(p0);
                //pnts.Add(ep);

                foreach (Collections.Generic.IList<Revit.DB.XYZ> pnts in listPnts)
                {
                  ret[space.Id].Add(pnts);
                }
              }
              else
              {
                //Collections.Generic.IList<Revit.DB.XYZ> pnts = new Collections.Generic.List<Revit.DB.XYZ>();
                //Revit.DB.XYZ ep = hsPnts[hsPnts.Count - 1];
                //pnts.Add(p0);
                //pnts.Add(ep);

                //Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listPnts = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
                //listPnts.Add(pnts);

                ret.Add(space.Id, listPnts);
              }
            }
          }

        }
      }

      if (ret.Count == 0)
      {
        Revit.DB.ElementId eid = new Revit.DB.ElementId(-1);

        //Collections.Generic.IList<Revit.DB.XYZ> pnts = new Collections.Generic.List<Revit.DB.XYZ>();
        //Revit.DB.XYZ ep = hsPnts[hsPnts.Count - 1];
        //pnts.Add(p0);
        //pnts.Add(ep);

        //Collections.Generic.IList<Collections.Generic.IList<Revit.DB.XYZ>> listPnts = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.XYZ>>();
        //listPnts.Add(pnts);

        ret.Add(eid, listPnts);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>文字作成</summary>
    /// 
    /// <param name="view"      >ビュー</param>
    /// <param name="origin"    >原点</param>
    /// <param name="baseVec"   >横方向ベクトル</param>
    /// <param name="horizontal">水平基点</param>
    /// <param name="vertical"  >垂直基点</param>
    /// <param name="text"      >文字</param>
    /// 
    /// <history>2015/09/03 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.TextNote CreateTextNoteXYPosRotateSet(Revit.DB.Transaction trans,
                                                   Revit.DB.View view,
                                                   Revit.DB.XYZ origin,
                                                   Revit.DB.XYZ baseVec,
                                                   Revit.DB.HorizontalTextAlignment horizontal,
                                                   Revit.DB.VerticalTextAlignment vertical,
                                                   string text)
    {
      // 戻り値
      Revit.DB.TextNote ret = null;

      // 回転角
      Revit.DB.XYZ p0 = new Revit.DB.XYZ(0, 0, 0);
      Revit.DB.XYZ p1 = new Revit.DB.XYZ(1, 0, 0);
      Revit.DB.XYZ p2 = baseVec;

      double dotProduct = _CmpGeometry.DotProduct2D(p0,
                                                    p1,
                                                    p2);
      double crossProduct = _CmpGeometry.CrossProduct2D(p0,
                                                        p1,
                                                        p2);
      double rotate = System.Math.Atan2(crossProduct,
                                        dotProduct);

      // 作成
      ret = _CmpElements.CreateTextNoteXYPosRotateSet(trans,
                                                      view,
                                                      origin,
                                                      rotate,
                                                      horizontal,
                                                      vertical,
                                                      text);

      return ret;
    }

    /// ================================================================================
    /// <summary>CSV出力</summary>
    /// 
    /// <param name="csvName" >CSVファイル名</param>
    /// <param name="dataList">データ</param>
    /// <param name="iType"   >0:ダクト 1:配管</param>
    /// 
    /// <history><p>2017/07/04 Created CST Hideki Sudoh</p>
    ///           <p>2017/07/21 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    string OutPutData(String csvName, Collections.Generic.IList<OutPutParam> dataList, int iType)
    {
      string ret = "";

      String csvPath = System.IO.Path.Combine(_ExportFolderPath, csvName);

      String cm = ",";
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      System.Text.Encoding enc = System.Text.Encoding.GetEncoding("Shift_JIS");

      try
      {
        //書き込むファイルを開く
        System.IO.StreamWriter sw = new System.IO.StreamWriter(csvPath, false, enc);

        //ヘッダー
        String outH = "";

        Collections.Generic.IList<String> outputHeader = new Collections.Generic.List<String>();
        if (iType == 0)
        {
          outputHeader = _CmpParameters._OutPutDuctHeader;
        }
        else if (iType == 1)
        {
          outputHeader = _CmpParameters._OutPutPipeHeader;
        }

        foreach (String data in outputHeader)
        {
          outH += data;
        }
        sw.WriteLine(outH);

        //データ
        double valD;
        String valS;
        foreach (OutPutParam data in dataList)
        {
          String outS = "";

          outS += (data.SystemType);  //システムタイプ
          outS += (cm);
          outS += (data.SystemName);  //システム名
          outS += (cm);
          outS += (data.SpaceName);   //スペース名
          outS += (cm);
          outS += (data.SpaceNumber);   //スペース番号
          outS += (cm);

          // ダクト
          if (iType == 0)
          {
            //0:角 1:丸
            if (data.iShape == 0)
            {
              //幅
              valD = data.Width * Parameters.FTOMM;
              valS = Math.Round(valD, 1, MidpointRounding.AwayFromZero).ToString();
              outS += (valS);
              outS += (cm);

              //高さ
              valD = data.Height * Parameters.FTOMM;
              valS = Math.Round(valD, 1, MidpointRounding.AwayFromZero).ToString();
              outS += (valS);
              outS += (cm);

              //直径
              outS += (cm);
            }
            else if (data.iShape == 1)
            {
              //幅
              outS += (cm);

              //高さ
              outS += (cm);

              //直径
              valD = data.Diameter * Parameters.FTOMM;
              valS = Math.Round(valD, 1, MidpointRounding.AwayFromZero).ToString();
              outS += (valS);
              outS += (cm);
            }
          }
          // 配管
          else if (iType == 1)
          {
            //0:角 1:丸
            if (data.iShape == 1)
            {
              //直径
              valD = data.Diameter * Parameters.FTOMM;
              valS = Math.Round(valD, 1, MidpointRounding.AwayFromZero).ToString();
              outS += (valS);
              outS += (cm);
            }
          }

          //長さ
          valD = data.Length * Parameters.FTOMM;
          valS = Math.Round(valD, 0, MidpointRounding.AwayFromZero).ToString();
          outS += (valS);
          outS += (cm);

          // 方向
          //outS += data.iVertical.ToString();
          if (data.iVertical == 0)
          {
            outS += "";
          }
          else if (data.iVertical == 1)
          {
            outS += "Slope";
          }
          else if (data.iVertical == 2)
          {
            outS += "Riser";
          }
          else if (data.iVertical == 3)
          {
            outS += "Flexible";
          }

          sw.WriteLine(outS);
        }
        sw.Close();
      }
      catch
      {
        ret = _CmpAttribute.ResourceText("IDS_ERR_OUTPUTCSV");
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>CSV出力パラメータセット</summary>
    /// 
    /// <param name="iType" >ダクト・配管フラグ</param>
    /// <param name="space" >スペース</param>
    /// <param name="duct"  >ダクト・配管</param>
    /// <param name="listP" >端点座標</param>
    /// 
    /// <history><p>2017/07/04 Created CST Hideki Sudoh</p>
    ///           <p>2017/07/21 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public Components.OutPutParam SetOutPutParameter(int iType,
                                                     Revit.DB.Mechanical.Space space,
                                                     Revit.DB.MEPCurve duct,
                                                     Collections.Generic.IList<Revit.DB.XYZ> listP)
    {
      if (listP.Count != 2)
      {
        return null;
      }

      //出力項目
      Components.OutPutParam oparam = new Components.OutPutParam();
      oparam.iType = iType;   //0:ダクト 1:配管

      //スペース名
      if (space != null)
      {
        Revit.DB.Parameter roomName = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_NAME);
        oparam.SpaceName = roomName != null ? roomName.AsString() : "";

        Revit.DB.Parameter roomNumber = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_NUMBER);
        oparam.SpaceNumber = roomNumber != null ? roomNumber.AsString() : "";
      }

      // ダクト
      if (iType == 0)
      {
        // システムタイプ
        Revit.DB.Parameter systemType = duct.get_Parameter(Revit.DB.BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM);
        oparam.SystemType = systemType != null ? systemType.AsValueString() : "";
      }
      // 配管
      else if (iType == 1)
      {
        // システムタイプ
        Revit.DB.Parameter systemType = duct.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM);
        oparam.SystemType = systemType != null ? systemType.AsValueString() : "";
      }

      //システム名
      Revit.DB.Parameter systemName = duct.get_Parameter(Revit.DB.BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
      oparam.SystemName = systemName != null ? systemName.AsString() : "";

      Revit.DB.BuiltInParameter diaPrm = Revit.DB.BuiltInParameter.RBS_CURVE_DIAMETER_PARAM;
      Revit.DB.BuiltInParameter wPrm = Revit.DB.BuiltInParameter.RBS_CURVE_WIDTH_PARAM;
      Revit.DB.BuiltInParameter hPrm = Revit.DB.BuiltInParameter.RBS_CURVE_HEIGHT_PARAM;
      //配管
      if (iType == 1)
      {
        diaPrm = Revit.DB.BuiltInParameter.RBS_PIPE_DIAMETER_PARAM;
      }

      Revit.DB.Parameter diameter = duct.get_Parameter(diaPrm);
      if (diameter != null)  //丸
      {
        oparam.iShape = 1;
        oparam.Diameter = diameter.AsDouble();
      }
      else//角
      {
        oparam.iShape = 0;
        Revit.DB.Parameter width = duct.get_Parameter(wPrm);
        oparam.Width = width != null ? width.AsDouble() : 0;

        Revit.DB.Parameter height = duct.get_Parameter(hPrm);
        oparam.Height = height != null ? height.AsDouble() : 0;
      }

      oparam.Length = _CmpGeometry.Distance(listP[0], listP[1]);

      Revit.DB.XYZ p0 = listP[0];
      Revit.DB.XYZ p1 = listP[1];

      // XY成分の差分 1mm単位
      double distance2d = _CmpGeometry.ToHalfAdjust(_CmpGeometry.Distance2D(p0, p1) * Parameters.FTOMM, 0);

      if (distance2d >= 1)
      {
        // Z成分の差分 1mm 単位
        double distanceZ = _CmpGeometry.ToHalfAdjust(Math.Abs(p0.Z - p1.Z) * Parameters.FTOMM, 0);

        if (distanceZ < 1)
        {
          oparam.iVertical = 0;
        }
        else
        {
          oparam.iVertical = 1;
        }
      }
      else
      {
        oparam.iVertical = 2;
      }

      if (duct is Revit.DB.Mechanical.FlexDuct)
      {
        oparam.iVertical = 3;
      }

      return oparam;

    }

    #endregion

    // プロパティ
    #region Properties

    #endregion
  }
}