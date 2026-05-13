using System;
using System.Linq;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using SectionListSteel.Utils;

namespace SectionListSteel.Components
{
  /// ================================================================================
  /// <summary>要素</summary>
  /// ================================================================================
  public class Elements : SectionListSteel.JExtComCompat.RvtElements
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private SectionListSteel.Components.Attribute _CmpAttribute;

    #endregion

    // コンストラクタ
    #region Constructor
    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    /// 
    /// <param name="cmpAttribute">属性</param>
    /// <param name="rvtUIDoc"    >Revit UI ドキュメント</param>
    /// 
    /// <history>2016/08/05 Created Ryo Kuroda</history>
    /// ================================================================================
    public
    Elements(SectionListSteel.Components.Attribute cmpAttribute,
             Revit.UI.UIDocument rvtUIDoc) :
      base (rvtUIDoc)
    {
      _CmpAttribute = cmpAttribute;
    }
    #endregion

    // メンバ関数
    #region Member Functions

    /// ================================================================================
    /// <summary>プロジェクト内のレベル</summary>
    /// 
    /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.List<string> ProjLevelNames()
    {
      // 戻り値
      Collections.Generic.List<string> ret = new Collections.Generic.List<string>();

      Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
      fec.OfClass(typeof(Revit.DB.Level));

      Collections.Generic.List<double> height = new Collections.Generic.List<double>();
      foreach (Revit.DB.Level l in fec)
      {
        height.Add(l.Elevation);
      }

      // 高い順ソート
      height.Sort();
      if (height[0] < height[height.Count - 1])
      {
        height.Reverse();
      }

      foreach (double d in height)
      {
        foreach (Revit.DB.Level l in fec)
        {
          if (ToHalfAdjust(d, -9) == ToHalfAdjust(l.Elevation, -9))
          {
            ret.Add(l.Name);
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>構造平面ビュータイプ取得</summary>
    /// 
    /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IEnumerable<Revit.DB.ViewFamilyType> StructuralPlanFamilyTypes()
    {
      Collections.Generic.IEnumerable<Revit.DB.ViewFamilyType> ret = null;

      try
      {
        ret = from elem in new Revit.DB.FilteredElementCollector(RvtDBDoc).OfClass(typeof(Revit.DB.ViewFamilyType))
              let type = elem as Revit.DB.ViewFamilyType
              where type.ViewFamily == Revit.DB.ViewFamily.StructuralPlan
              select type;
      }
      catch
      {
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>構造平面取得判定</summary>
    /// 
    /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsStrPlaneGet()
    {
      bool ret = false;
      Collections.Generic.IEnumerable<Revit.DB.ViewFamilyType> strPlanFamType = StructuralPlanFamilyTypes();

      if (strPlanFamType != null)
      {
        ret = true;
      }
      else if (strPlanFamType == null)
      {
        ret = false;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>すべての平面ビュー名</summary>
    /// <history>2013/05/31 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<string> AllViewPlanName()
    {
      Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

      Revit.DB.FilteredElementCollector colle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
      colle.OfClass(typeof(Revit.DB.ViewPlan));

      foreach (Revit.DB.ViewPlan vp in colle)
      {
        ret.Add(vp.Name);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>平面ビュータイプ取得</summary>
    /// 
    /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IEnumerable<Revit.DB.ViewFamilyType> ViewPlanFamilyTypes()
    {
      return from elem in new Revit.DB.FilteredElementCollector(RvtDBDoc).OfClass(typeof(Revit.DB.ViewFamilyType))
             let type = elem as Revit.DB.ViewFamilyType
             where type.ViewFamily == Revit.DB.ViewFamily.FloorPlan
             select type;
    }

    /// ================================================================================
    /// <summary>0高さレベル</summary>
    /// 
    /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Level ZeroLevel()
    {
      Revit.DB.Level ret = null;

      Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
      filterElemColle.OfCategory(Revit.DB.BuiltInCategory.OST_Levels);

      foreach (Revit.DB.Element elem in filterElemColle)
      {
        Revit.DB.Level l = elem as Revit.DB.Level;
        if (l == null)
        {
          continue;
        }

        if (ret == null)
        {
          ret = l;
        }
        else
        {
          // ゼロに近いレベル
          if (System.Math.Abs(ret.Elevation) > System.Math.Abs(l.Elevation))
          {
            ret = l;
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>作図ビュー作成</summary>
    /// 
    /// <param name="viewScale" >ビュー尺度</param>
    /// <param name="mode"      >mode = 0 柱
    ///                               = 1 間柱
    ///                               = 2 大梁
    ///                               = 3 小梁</param>
    /// 
    /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.ViewPlan SetCreateListView(int viewScale, int mode)
    {
      Revit.DB.ViewPlan vp = null;

      Revit.DB.ElementId lvlid = ZeroLevel().Id;

      // ビューの作成
      Revit.DB.Transaction trans = new Revit.DB.Transaction(RvtDBDoc);
      trans.Start("ビューの作成");

      if (StructuralPlanFamilyTypes() != null && StructuralPlanFamilyTypes().Count() > 0)
      {
        foreach (Revit.DB.ViewFamilyType vft in StructuralPlanFamilyTypes())
        {
          if (vft.Name == _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST"))
          {
            vp = Revit.DB.ViewPlan.Create(RvtDBDoc, vft.Id, lvlid);
          }
        }

        if (vp == null)
        {
          System.Collections.Generic.ICollection<Revit.DB.ElementId> copyElem = Revit.DB.ElementTransformUtils.CopyElement(RvtDBDoc, StructuralPlanFamilyTypes().First().Id, new Revit.DB.XYZ());

          Revit.DB.ViewFamilyType viewFamType = RvtDBDoc.GetElement(copyElem.First()) as Revit.DB.ViewFamilyType;
          viewFamType.Name = _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST");
          vp = Revit.DB.ViewPlan.Create(RvtDBDoc, viewFamType.Id, lvlid);
        }

        if (vp == null)
        {
          foreach (Revit.DB.ViewFamilyType vft in StructuralPlanFamilyTypes())
          {
            if (vft.Name == _CmpAttribute.ResourceText("IDS_TXT_LIST"))
            {
              vp = Revit.DB.ViewPlan.Create(RvtDBDoc, vft.Id, lvlid);
            }
          }

          if (vp == null)
          {
            vp = Revit.DB.ViewPlan.Create(RvtDBDoc, StructuralPlanFamilyTypes().First().Id, lvlid);
          }
        }
      }
      else if (ViewPlanFamilyTypes().Count() > 0)
      {
        vp = Revit.DB.ViewPlan.Create(RvtDBDoc, ViewPlanFamilyTypes().First().Id, lvlid);
      }

      if (vp == null)
      {
        trans.Commit();
        return vp;
      }

      if (viewScale > 0)
      {
        vp.Scale = viewScale;
      }

      // 名前
      // 柱
      if (mode == 0)
      {
        string vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_COLUMN");

        if (AllViewPlanName().Contains(vpName) == false)
        {
          vp.Name = vpName;
        }
        else
        {
          int nameNum = 0;

          bool isVPName = false;

          while (isVPName == false)
          {
            nameNum += 1;

            vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_COLUMN") + "(" + nameNum + ")";

            if (AllViewPlanName().Contains(vpName) == false)
            {
              vp.Name = vpName;
              isVPName = true;
            }
          }
        }
      }
      // 間柱
      else if (mode == 1)
      {
        string vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_POST");

        if (AllViewPlanName().Contains(vpName) == false)
        {
          vp.Name = vpName;
        }
        else
        {
          int nameNum = 0;

          bool isVPName = false;

          while (isVPName == false)
          {
            nameNum += 1;

            vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_POST") + "(" + nameNum + ")";

            if (AllViewPlanName().Contains(vpName) == false)
            {
              vp.Name = vpName;
              isVPName = true;
            }
          }
        }
      }
      // 大梁
      else if (mode == 2)
      {
        string vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_GIRDER");

        if (AllViewPlanName().Contains(vpName) == false)
        {
          vp.Name = vpName;
        }
        else
        {
          int nameNum = 0;

          bool isVPName = false;

          while (isVPName == false)
          {
            nameNum += 1;

            vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_GIRDER") + "(" + nameNum + ")";

            if (AllViewPlanName().Contains(vpName) == false)
            {
              vp.Name = vpName;
              isVPName = true;
            }
          }
        }
      }
      // 小梁
      else if (mode == 3)
      {
        string vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_BEAM");

        if (AllViewPlanName().Contains(vpName) == false)
        {
          vp.Name = vpName;
        }
        else
        {
          int nameNum = 0;

          bool isVPName = false;

          while (isVPName == false)
          {
            nameNum += 1;

            vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_BEAM") + "(" + nameNum + ")";

            if (AllViewPlanName().Contains(vpName) == false)
            {
              vp.Name = vpName;
              isVPName = true;
            }
          }
        }
      }
      // ブレース
      else if (mode == 4)
      {
        string vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_BRACE");

        if (AllViewPlanName().Contains(vpName) == false)
        {
          vp.Name = vpName;
        }
        else
        {
          int nameNum = 0;

          bool isVPName = false;

          while (isVPName == false)
          {
            nameNum += 1;

            vpName = _CmpAttribute.ResourceText("IDS_TXT_LISTVIEW_BRACE") + "(" + nameNum + ")";

            if (AllViewPlanName().Contains(vpName) == false)
            {
              vp.Name = vpName;
              isVPName = true;
            }
          }
        }
      }


      
      trans.Commit();

      // 作成したビューの要素
      Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc, vp.Id);
      Collections.Generic.ICollection<Revit.DB.ElementId> hideElemIds = new Collections.Generic.List<Revit.DB.ElementId>();
      foreach (Revit.DB.Element elem in filterElemColle)
      {
        if (elem.CanBeHidden(vp))
        {
          hideElemIds.Add(elem.Id);
        }
      }

      if (hideElemIds.Count > 0)
      {
        trans.Start("既存要素の非表示");
        vp.HideElements(hideElemIds);
        trans.Commit();
      }

      // アクティブビューに設定
      UiDocument.ActiveView = vp;

      if (trans.GetStatus() == Revit.DB.TransactionStatus.Started)
      {
        trans.Commit();
      }

      return vp;
    }

    /// ================================================================================
    /// <summary>ビューを閉じる</summary>
    /// 
    /// <param name="view">ビュー</param>
    /// 
    /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    string CloseView(Revit.DB.View view)
    {
      string ret = "";

      Collections.Generic.IList<Revit.UI.UIView> openViews = UiDocument.GetOpenUIViews();

      if (openViews.Count > 1)
      {
        foreach (Revit.UI.UIView uiView in openViews)
        {
          if (uiView.ViewId.Value == view.Id.Value)
          {
            uiView.Close();
          }
        }
      }
      else if (openViews.Count == 1)
      {
        foreach (Revit.UI.UIView uiView in openViews)
        {
          if (uiView.ViewId.Value == view.Id.Value)
          {
            ret = "only";
          }
        }
      }
      else
      {
        ret = "nothing";
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>ビューを削除</summary>
    /// 
    /// <param name="view">ビュー</param>
    /// 
    /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void RemoveView(Revit.DB.View view)
    {
      if (CloseView(view) == "")
      {
        RvtDBDoc.Delete(view.Id);
      }
    }

    /// ================================================================================
    /// <summary>ビューにフィット</summary>
    /// 
    /// <param name="view">ビュー</param>
    /// <param name="pos1">点1</param>
    /// <param name="pos2">点2</param>
    /// 
    /// <history>2016/09/14 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void ViewFit(Revit.DB.View view,
                 Revit.DB.XYZ pos1,
                 Revit.DB.XYZ pos2)
    {
      // 開いているビュー
      Collections.Generic.IList<Revit.UI.UIView> openViews = UiDocument.GetOpenUIViews();

      // ビューにフィット
      foreach (Revit.UI.UIView uiView in openViews)
      {
        if (uiView.ViewId.Value == view.Id.Value)
        {
          if (pos1 != null && pos2 != null)
          {
            uiView.ZoomAndCenterRectangle(pos1, pos2);
          }

          uiView.ZoomToFit();

          break;
        }
      }
    }

    /// ================================================================================
    /// <summary>名前指定文字タイプ取得</summary>
    /// 
    /// <param name="typeName">タイプ名</param>
    /// 
    /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.TextNoteType TxtNoteTypeByName(string typeName)
    {
      Revit.DB.TextNoteType ret = null;

      Collections.Generic.IList<Revit.DB.TextNoteType> txtNoteTypes = TxtNoteTypes;

      foreach (Revit.DB.TextNoteType tnt in txtNoteTypes)
      {
        if (tnt.Name == typeName)
        {
          ret = tnt;
          break;
        }
      }

      if (ret == null)
      {
        Collections.Generic.List<string> names = new Collections.Generic.List<string>();

        foreach (Revit.DB.TextNoteType tnt in txtNoteTypes)
        {
          names.Add(tnt.Name);
        }

        // 名前ソート
        names.Sort();

        foreach (Revit.DB.TextNoteType tnt in txtNoteTypes)
        {
          if (names[0] == tnt.Name)
          {
            return tnt;
          }
        }

        if (ret == null)
        {
          ret = txtNoteTypes[0];
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>名前指定詳細線分線種タイプ取得</summary>
    /// 
    /// <param name="styleName">スタイル名</param>
    /// 
    /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.GraphicsStyle GraStyleByName(string styleName)
    {
      Revit.DB.GraphicsStyle ret = null;

      Collections.Generic.IList<Revit.DB.GraphicsStyle> graStyles = DetailGraStyles;

      foreach (Revit.DB.GraphicsStyle gs in graStyles)
      {
        if (gs.Name == styleName)
        {
          ret = gs;
        }
      }

      if (ret == null && graStyles.Count > 0)
      {
        Collections.Generic.List<string> names = new Collections.Generic.List<string>();

        foreach (Revit.DB.GraphicsStyle gs in graStyles)
        {
          names.Add(gs.Name);
        }

        // 名前ソート
        names.Sort();

        foreach (Revit.DB.GraphicsStyle gs in graStyles)
        {
          if (names[0] == gs.Name)
          {
            return gs;
          }
        }

        if (ret == null)
        {
          ret = graStyles[0];
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>詳細線分作成</summary>
    /// 
    /// <param name="curve"   >カーブ</param>
    /// <param name="view"    >ビュー</param>
    /// <param name="graStyle">スタイル</param>
    /// 
    /// <history>2016/09/14 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void CreateDetailCurve(Revit.DB.Curve curve,
                           Revit.DB.View view,
                           Revit.DB.GraphicsStyle graStyle)
    {
      Revit.DB.DetailCurve dc = RvtDBDoc.Create.NewDetailCurve(view, curve);

      if (graStyle != null)
      {
        dc.LineStyle = graStyle;
      }
    }

    /// ================================================================================
    /// <summary>詳細線分線種取得</summary>
    /// 
    /// <history>2017/06/23 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.GraphicsStyle> GetDetailGraStyles()
    {
      // 戻り値
      // 線種からグラフィックスタイルを取得
      Collections.Generic.IList<Revit.DB.GraphicsStyle> graStyleColle = new Collections.Generic.List<Revit.DB.GraphicsStyle>();

      // 詳細線分が使っていない線種もあるので
      // ダミーの線分を作って詳細線分で使う線種だけを取得
      Revit.DB.XYZ siten = new Revit.DB.XYZ(0, 0, 0);
      Revit.DB.XYZ syuten = new Revit.DB.XYZ(1, 1, 0);

      Revit.DB.View actiview = base.RvtDBDoc.ActiveView;
      Revit.DB.ViewSection viewSec = actiview as Revit.DB.ViewSection;
      if (viewSec != null)
      {
        syuten = new Revit.DB.XYZ(0, 0, 1);
      }


      Revit.DB.Line line = Revit.DB.Line.CreateBound(siten, syuten);
      Revit.DB.DetailLine dl = null;

      Revit.DB.Transaction trans = new Revit.DB.Transaction(RvtDBDoc);

      // 3Dビュー
      Revit.DB.View3D view3d = actiview as Revit.DB.View3D;
      if (view3d != null)
      {
        Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
        filterElemColle.OfCategory(Revit.DB.BuiltInCategory.OST_Lines);

        Collections.Generic.IList<Revit.DB.DetailCurve> DCs = new Collections.Generic.List<Revit.DB.DetailCurve>();
        foreach (Revit.DB.Element element in filterElemColle)
        {
          Revit.DB.DetailCurve dc = element as Revit.DB.DetailCurve;
          if (dc != null)
          {
            DCs.Add(dc);
          }
        }

        // 詳細線分がプロジェクトにある場合
        if (DCs.Count() > 0)
        {
          foreach (Revit.DB.DetailCurve dc in DCs)
          {
            foreach (Revit.DB.ElementId eId in dc.GetLineStyleIds())
            {
              Revit.DB.GraphicsStyle graStyle = RvtDBDoc.GetElement(eId) as Revit.DB.GraphicsStyle;
              graStyleColle.Add(graStyle);
            }
            if (graStyleColle.Count > 0)
            {
              return graStyleColle;
            }
          }
        }
        // 詳細線分がプロジェクトにない場合
        else
        {
          // 平面ビューがあれば、
          // 平面ビューに詳細線分を作成して削除
          Revit.DB.FilteredElementCollector colle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
          colle.OfClass(typeof(Revit.DB.ViewPlan));
          Collections.Generic.IList<Revit.DB.ViewPlan> vps = new Collections.Generic.List<Revit.DB.ViewPlan>();
          foreach (Revit.DB.ViewPlan vp in colle)
          {
            if (vp.IsTemplate == false)
            {
              vps.Add(vp);
            }
          }

          if (vps.Count > 0)
          {
            foreach (Revit.DB.ViewPlan vp in vps)
            {
              line = Revit.DB.Line.CreateBound(siten, syuten);
              dl = null;

              trans.Start("詳細線分線種取得");

              try
              {
                dl = base.RvtDBDoc.Create.NewDetailCurve(vp, line) as Revit.DB.DetailLine;
                trans.Commit();
              }
              catch (Revit.Exceptions.InvalidOperationException)
              {
                trans.Commit();
                continue;
              }
              catch (Revit.Exceptions.ArgumentException)
              {
                trans.Commit();
                continue;
              }
              catch
              {
                trans.Commit();
                continue;
              }

              Collections.Generic.IList<Revit.DB.Element> elementAry = new Collections.Generic.List<Revit.DB.Element>();

              if (dl != null)
              {
                foreach (Revit.DB.ElementId eId in dl.GetLineStyleIds())
                {
                  Revit.DB.Element e = RvtDBDoc.GetElement(eId);
                  elementAry.Add(e);
                }

                // ダミーを削除
                trans.Start("ダミー削除");
                base.RvtDBDoc.Delete(dl.Id);
                trans.Commit();

                foreach (Revit.DB.Element elem in elementAry)
                {
                  Revit.DB.GraphicsStyle graStyle = elem as Revit.DB.GraphicsStyle;
                  if (graStyle != null)
                  {
                    graStyleColle.Add(graStyle);
                  }
                }

                if (trans.GetStatus() == Revit.DB.TransactionStatus.Started)
                {
                  trans.Commit();
                }

                if (graStyleColle.Count > 0)
                {
                  break;
                }
              }
              else
              {
                continue;
              }
            }

            if (graStyleColle.Count < 1)
            {
              filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
              filterElemColle.OfClass(typeof(Revit.DB.GraphicsStyle));

              if (filterElemColle.Count() > 0)
              {
                foreach (Revit.DB.GraphicsStyle gs in filterElemColle)
                {
                  try
                  {
                    if (gs.GraphicsStyleCategory.Parent != null)
                    {
                      if (gs.GraphicsStyleCategory.Parent.Id.Value.Equals((int)Revit.DB.BuiltInCategory.OST_Lines))
                      {
                        graStyleColle.Add(gs);
                      }
                    }
                  }
                  catch
                  {
                  }
                }
                return graStyleColle;
              }
              else
              {
                return graStyleColle;
              }
            }

            return graStyleColle;
          }
          else
          {
            // 平面ビューを作り、詳細線分作成
            Revit.DB.ViewPlan vp = SetCreateListView(100, 4);

            if (vp != null)
            {
              trans.Start("詳細線分線種取得");

              line = Revit.DB.Line.CreateBound(siten, syuten);

              try
              {
                dl = base.RvtDBDoc.Create.NewDetailCurve(RvtDBDoc.ActiveView, line) as Revit.DB.DetailLine;
                trans.Commit();
              }
              catch (Revit.Exceptions.InvalidOperationException)
              {
                trans.Commit();
                return graStyleColle;
              }
              catch (Revit.Exceptions.ArgumentException)
              {
                trans.Commit();
                return graStyleColle;
              }
              catch
              {
                trans.Commit();
                return graStyleColle;
              }

              Collections.Generic.IList<Revit.DB.Element> elemAry = new Collections.Generic.List<Revit.DB.Element>();

              foreach (Revit.DB.ElementId eId in dl.GetLineStyleIds())
              {
                Revit.DB.Element e = RvtDBDoc.GetElement(eId);
                elemAry.Add(e);
              }

              // ダミーを削除
              trans.Start("ダミー削除");
              base.RvtDBDoc.Delete(dl.Id);
              trans.Commit();

              foreach (Revit.DB.Element elem in elemAry)
              {
                Revit.DB.GraphicsStyle graStyle = elem as Revit.DB.GraphicsStyle;
                if (graStyle != null)
                {
                  graStyleColle.Add(graStyle);
                }
              }

              if (trans.GetStatus() == Revit.DB.TransactionStatus.Started)
              {
                trans.Commit();
              }

              trans.Start("ビュー削除");
              RemoveView(vp);
              trans.Commit();
            }
            else
            {
              filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
              filterElemColle.OfClass(typeof(Revit.DB.GraphicsStyle));

              if (filterElemColle.Count() > 0)
              {
                foreach (Revit.DB.GraphicsStyle gs in filterElemColle)
                {
                  try
                  {
                    if (gs.GraphicsStyleCategory.Parent != null)
                    {
                      if (gs.GraphicsStyleCategory.Parent.Id.Value.Equals((int)Revit.DB.BuiltInCategory.OST_Lines))
                      {
                        graStyleColle.Add(gs);
                      }
                    }
                  }
                  catch
                  {
                  }
                }
                return graStyleColle;
              }
              else
              {
                return graStyleColle;
              }
            }
          }
        }
      }
      // 3D以外
      else
      {
        Revit.DB.FilteredElementCollector filterElemColle = new Revit.DB.FilteredElementCollector(RvtDBDoc);
        filterElemColle.OfCategory(Revit.DB.BuiltInCategory.OST_Lines);

        Collections.Generic.IList<Revit.DB.DetailCurve> DCs = new Collections.Generic.List<Revit.DB.DetailCurve>();
        foreach (Revit.DB.Element element in filterElemColle)
        {
          Revit.DB.DetailCurve dc = element as Revit.DB.DetailCurve;
          if (dc != null)
          {
            DCs.Add(dc);
          }
        }

        // 詳細線分がプロジェクトにある場合
        if (DCs.Count() > 0)
        {
          foreach (Revit.DB.Element element in filterElemColle)
          {
            Revit.DB.DetailLine dLine = element as Revit.DB.DetailLine;
            if (dLine != null)
            {
              foreach (Revit.DB.ElementId eId in dLine.GetLineStyleIds())
              {
                Revit.DB.GraphicsStyle graStyle = RvtDBDoc.GetElement(eId) as Revit.DB.GraphicsStyle;
                graStyleColle.Add(graStyle);
              }
              if (graStyleColle.Count > 0)
              {
                return graStyleColle;
              }
            }
            else
            {
              Revit.DB.DetailArc dArc = element as Revit.DB.DetailArc;
              if (dArc != null)
              {
                foreach (Revit.DB.ElementId eId in dArc.GetLineStyleIds())
                {
                  Revit.DB.GraphicsStyle graStyle = RvtDBDoc.GetElement(eId) as Revit.DB.GraphicsStyle;
                  graStyleColle.Add(graStyle);
                }
                if (graStyleColle.Count > 0)
                {
                  return graStyleColle;
                }
              }
            }
          }
        }
        else
        {
          trans.Start("詳細線分線種取得");

          try
          {
            dl = base.RvtDBDoc.Create.NewDetailCurve(RvtDBDoc.ActiveView, line) as Revit.DB.DetailLine;
            trans.Commit();
          }
          catch (Revit.Exceptions.InvalidOperationException)
          {
            trans.Commit();
            return graStyleColle;
          }
          catch (Revit.Exceptions.ArgumentException)
          {
            trans.Commit();
            return graStyleColle;
          }
          catch
          {
            trans.Commit();
            return graStyleColle;
          }

          Collections.Generic.IList<Revit.DB.Element> elemAry = new Collections.Generic.List<Revit.DB.Element>();

          foreach (Revit.DB.ElementId eId in dl.GetLineStyleIds())
          {
            Revit.DB.Element e = RvtDBDoc.GetElement(eId);
            elemAry.Add(e);
          }

          // ダミーを削除
          trans.Start("ダミー削除");
          base.RvtDBDoc.Delete(dl.Id);
          trans.Commit();

          foreach (Revit.DB.Element elem in elemAry)
          {
            Revit.DB.GraphicsStyle graStyle = elem as Revit.DB.GraphicsStyle;
            if (graStyle != null)
            {
              graStyleColle.Add(graStyle);
            }
          }

          if (trans.GetStatus() == Revit.DB.TransactionStatus.Started)
          {
            trans.Commit();
          }
        }
      }

      return graStyleColle;
    }

    /// ================================================================================
    /// <summary>n位(10^digits)に四捨五入</summary>
    /// ================================================================================
    public
    double ToHalfAdjust(double value, int digits)
    {
      digits = digits * -1;

      // 10のべき乗
      double dCoef = System.Math.Pow(10, digits);

      // 指定位を1の位にする
      // 値が0より大きい場合
      // 0.5を足した値以下の最大の整数を10のべき乗で割る
      // 値が0以下の場合
      // 0.5を引いた値以上の最小の整数を10のべき乗で割る
      return value > 0 ? System.Math.Floor((value * dCoef) + 0.5) / dCoef :
                         System.Math.Ceiling((value * dCoef) - 0.5) / dCoef;

      // 例 1
      // value = 10.56, digits = 0
      // 10^digits = 10^0 = 1
      // value > 0 == true
      // Floor(10.56 * 1 + 0.5) = Floor(10.56 + 0.5) = Floor(11.06) = 11
      // return 11

      // 例 2
      // value = 0.014, digits = -2
      // digits = -2 * -1
      // 10^digits = 10^2 = 100
      // value > 0 == false
      // Ceiling(0.014 * 100 - 0.5) = Ceiling(1.4 -0.5) = Ceiling(0.9) = 1
      // 1 / 100 = 0.01
      // return 0.01
    }

    #endregion

    // プロパティ
    #region Properties
    /// ================================================================================
    /// <summary>文字タイプ</summary>
    /// <history>2016/08/29 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.TextNoteType> TxtNoteTypes
    {
      get
      {
        // 戻り値
        Collections.Generic.IList<Revit.DB.TextNoteType> rets = new Collections.Generic.List<Revit.DB.TextNoteType>();

        Revit.DB.FilteredElementCollector collector = new Revit.DB.FilteredElementCollector(RvtDBDoc);
        collector.OfClass(typeof(Revit.DB.TextNoteType));

        foreach (Revit.DB.TextNoteType tnp in collector)
        {
          rets.Add(tnp);
        }

        return rets;
      }
    }

    /// ================================================================================
    /// <summary>詳細線分タイプ</summary>
    /// <history>2016/08/29 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.GraphicsStyle> DetailGraStyles
    {
      get
      {
        // 戻り値
        // 線種からグラフィックスタイルを取得
        Collections.Generic.IList<Revit.DB.GraphicsStyle> graStyleColle = new Collections.Generic.List<Revit.DB.GraphicsStyle>();

        if (graStyleColle.Count < 1)
        {
          graStyleColle = GetDetailGraStyles();
        }
        
        return graStyleColle;
      }
    }

    /// ================================================================================
    /// <summary>鉄骨柱タイプ</summary>
    /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.FamilySymbol> SteelColumnFamSyms
    {
      get
      {
        Collections.Generic.IList<Revit.DB.FamilySymbol> ret = new Collections.Generic.List<Revit.DB.FamilySymbol>();

        Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
        fec.OfCategory(Revit.DB.BuiltInCategory.OST_StructuralColumns);
        fec.OfClass(typeof(Revit.DB.FamilySymbol));

        foreach (Revit.DB.FamilySymbol famSym in fec)
        {
          if (famSym.Family.StructuralMaterialType == Revit.DB.Structure.StructuralMaterialType.Steel)
          {
            ret.Add(famSym);
          }
        }

        return ret;
      }
    }

    /// ================================================================================
    /// <summary>鉄骨梁タイプ</summary>
    /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.FamilySymbol> SteelGirderFamSyms
    {
      get
      {
        Collections.Generic.IList<Revit.DB.FamilySymbol> ret = new Collections.Generic.List<Revit.DB.FamilySymbol>();

        Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
        fec.OfCategory(Revit.DB.BuiltInCategory.OST_StructuralFraming);
        fec.OfClass(typeof(Revit.DB.FamilySymbol));

        foreach (Revit.DB.FamilySymbol famSym in fec)
        {
          if (famSym.Family.StructuralMaterialType == Revit.DB.Structure.StructuralMaterialType.Steel)
          {
            ret.Add(famSym);
          }
        }

        return ret;
      }
    }

    #endregion
  }
}
