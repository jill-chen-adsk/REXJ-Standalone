using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace Quantity.Components
{
  /// ================================================================================
  /// <summary>要素</summary>
  /// ================================================================================
  public class Elements
  {
    // メンバ変数
    #region Member Variables

    /// <summary>属性</summary>
    private Quantity.Components.Attribute _CmpAttribute;

    /// <summary>スペース</summary>
    private Collections.Generic.IList<Revit.DB.Mechanical.Space> _Spaces;

    /// <summary>Active UI document</summary>
    private Revit.UI.UIDocument _rvtUIDoc;
    
    #endregion

    // コンストラクタ
    #region Constructor
    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    /// 
    /// <param name="rvtUIDoc"    >Revit UIドキュメント</param>
    /// <param name="cmpAttribute">属性</param>
    /// 
    /// <history>2014/07/14 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Elements(Revit.UI.UIDocument rvtUIDoc, Quantity.Components.Attribute cmpAttribute)
    {
      _rvtUIDoc = rvtUIDoc;
      _CmpAttribute = cmpAttribute;
    }
    #endregion

    // メンバ関数
    #region

    /// ================================================================================
    /// <summary>指定コネクタを持つ配管</summary>
    /// 
    /// <param name="connector" >コネクタ</param>
    /// <param name="pipeAry"   >配管</param>
    /// 
    /// <history><p>2014/07/23 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/10/17 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Plumbing.Pipe> GetSameConnectorPipe(Revit.DB.Element connector)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Plumbing.Pipe> ret = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();

      Collections.Generic.IList<string> ids = new Collections.Generic.List<string>();

      // 継手につながっている要素(1回目)
      Collections.Generic.IList<Revit.DB.Element> cnctElemes = GetConnectorConnectElems(connector);

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
        else if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeFitting).ToString()) ||
                 elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeAccessory).ToString()))
        {
          Collections.Generic.IList<Revit.DB.Element> cncts = GetConnectorConnectElems(elem);

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
            else if (el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeFitting).ToString()) ||
                     el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeAccessory).ToString()))
            {
              Collections.Generic.IList<Revit.DB.Element> cs = GetConnectorConnectElems(el);

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

      //foreach (Revit.DB.Plumbing.Pipe pipe in pipeAry)
      //{
      //  Revit.DB.Element connector1 = null;
      //  Revit.DB.Element connector2 = null;

      //  GetConnector(pipe, ref connector1, ref connector2);

      //  if (connector1 != null && connector1.Id.IntegerValue == connector.Id.IntegerValue)
      //  {
      //    ret.Add(pipe);
      //  }
      //  else if (connector2 != null && connector2.Id.IntegerValue == connector.Id.IntegerValue)
      //  {
      //    ret.Add(pipe);
      //  }
      //}

      return ret;
    }

    /// ================================================================================
    /// <summary>指定コネクタを持つダクト</summary>
    /// 
    /// <param name="connector" >コネクタ</param>
    /// <param name="ductAry"   >配管</param>
    /// 
    /// <history><p>2014/07/23 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/10/17 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Duct> GetSameConnectorDuct(Revit.DB.Element connector)
    {
      // 戻り値
      Collections.Generic.IList<Revit.DB.Mechanical.Duct> ret = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();

      // 継手につながっている要素(1回目)
      Collections.Generic.IList<Revit.DB.Element> cnctElemes = GetConnectorConnectElems(connector);

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
        else if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctFitting).ToString()) ||
                 elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctAccessory).ToString()))
        {
          Collections.Generic.IList<Revit.DB.Element> cncts = GetConnectorConnectElems(elem);

          foreach (Revit.DB.Element el in cncts)
          {
            if (el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctCurves).ToString()))
            {
              Revit.DB.Mechanical.Duct duct = el as Revit.DB.Mechanical.Duct;

              ret.Add(duct);
            }
            // 継手につながっている要素(3回目)
            //else
            else if (el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctFitting).ToString()) ||
                     el.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctAccessory).ToString()))
            {
              Collections.Generic.IList<Revit.DB.Element> cs = GetConnectorConnectElems(el);

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
    /// <summary>継手につながっている要素</summary>
    /// 
    /// <param name="connector">継手要素、フレキ配管</param>
    /// 
    /// <history><p>2014/07/29 Created GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Element> GetConnectorConnectElems(Revit.DB.Element connector)
    {
      Collections.Generic.IList<Revit.DB.Element> ret = new Collections.Generic.List<Revit.DB.Element>();

      Revit.DB.FamilyInstance famIns = connector as Revit.DB.FamilyInstance;

      if (famIns != null)
      {
        // MEP
        Revit.DB.MEPModel mepMdl = famIns.MEPModel;

        Revit.DB.ConnectorManager cnctMgr = mepMdl.ConnectorManager;

        // 接続要素
        Revit.DB.ConnectorSet cnctSet = cnctMgr.Connectors;

        foreach (Revit.DB.Connector cnct in cnctSet)
        {
          Revit.DB.ConnectorSet cs = cnct.AllRefs;

          foreach (Revit.DB.Connector c in cs)
          {
            ret.Add(c.Owner);
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>既定の寸法線タイプ</summary>
    /// 
    /// <history>2014/09/22 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.DimensionType DefDimensionType()
    {
      // 戻り値
      Revit.DB.DimensionType ret = null;

      Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
      fec.OfClass(typeof(Revit.DB.DimensionType));

      foreach (Revit.DB.DimensionType dt in fec)
      {
        if (dt.Name == _CmpAttribute.ResourceText("IDS_TXT_DEFDIMENSIONTYPE"))
        {
          ret = dt;
        }
      }

      if (ret == null)
      {
        foreach (Revit.DB.DimensionType dt in fec)
        {
          if (dt.Name == "標準" || dt.Name == "Standard")
          {
            ret = dt;
          }
        }
      }

      return ret;
    }
    
    /// ================================================================================
    /// <summary>寸法作成</summary>
    /// 
    /// <param name="view"  >ビュー</param>
    /// <param name="line"  >基準線</param>
    /// <param name="refAry">寸法参照</param>
    /// 
    /// <history>2014/09/29 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Revit.DB.Dimension CreateDimension(Revit.DB.View view,
                                       Revit.DB.Line line,
                                       Revit.DB.ReferenceArray refAry)
    {
      Revit.DB.Dimension ret = null;

      Revit.DB.DimensionType dimType = DefDimensionType();

      if (dimType != null)
      {
        ret = RvtDBDoc.Create.NewDimension(view, line, refAry, dimType);
      }
      else
      {
        ret = RvtDBDoc.Create.NewDimension(view, line, refAry);
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>文字作成 - 位置、回転調整</summary>
    /// 
    /// <remarks>2016から文字は上基点でしか作れない。回転も上基点を中心に行われる。</remarks>
    /// 
    /// <param name="view"      >ビュー</param>
    /// <param name="origin"    >配置原点</param>
    /// <param name="angle"     >回転角</param>
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
                                                   double angle,
                                                   Revit.DB.HorizontalTextAlignment horizontal,
                                                   Revit.DB.VerticalTextAlignment vertical,
                                                   string text)
    {
      // 戻り値
      Revit.DB.TextNote ret = null;

      trans.Start("Rotate text");
      // 作成(回転なし)
      Revit.DB.ElementId typeId = RvtDBDoc.GetDefaultElementTypeId(Revit.DB.ElementTypeGroup.TextNoteType);
      if (typeId == Revit.DB.ElementId.InvalidElementId)
      {
        Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
        fec.OfClass(typeof(Revit.DB.TextNoteType));
        fec.WhereElementIsElementType();
        Revit.DB.TextNoteType fst = null;
        foreach (Revit.DB.Element e in fec)
        {
          fst = e as Revit.DB.TextNoteType;
          if (fst != null)
            break;
        }
        if (fst != null)
          typeId = fst.Id;
      }
      if (typeId != Revit.DB.ElementId.InvalidElementId)
      {
        var opts = new Revit.DB.TextNoteOptions(typeId) { HorizontalAlignment = horizontal };
        ret = Revit.DB.TextNote.Create(RvtDBDoc, view.Id, origin, text, opts);
      }
      trans.Commit();

      if (ret == null)
        return null;

      // 垂直位置調整
      // 上基点の場合はそのまま
      if (vertical != Revit.DB.VerticalTextAlignment.Top)
      {
        Revit.DB.BoundingBoxXYZ bndBox = ret.get_BoundingBox(view);

        Revit.DB.XYZ min = bndBox.Min;

        // 移動量
        double disY = 0;

        // 中央
        if (vertical == Revit.DB.VerticalTextAlignment.Middle)
        {
          // 半分移動
          disY = (origin.Y - min.Y) / 2;
        }
        // 下
        else if (vertical == Revit.DB.VerticalTextAlignment.Bottom)
        {
          // 全体移動
          disY = origin.Y - min.Y;
        }

        // 移動量
        Revit.DB.XYZ translate = new Revit.DB.XYZ(0, disY, 0);

        // 移動
        trans.Start("Move text");
        ret.Location.Move(translate);
        trans.Commit();
      }

      // 回転調整
      if (angle != 0d)
      {
        Revit.DB.XYZ z = origin + Revit.DB.XYZ.BasisZ;

        trans.Start("Adjust rotation");
        // 回転軸
        Revit.DB.Line axis = Revit.DB.Line.CreateBound(origin, z);

        // 回転
        ret.Location.Rotate(axis, angle);
        trans.Commit();
      }

      return ret;
    }
    
    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>Active UI document</summary>
    public Revit.UI.UIDocument RvtUIDoc => _rvtUIDoc;

    /// <summary>Associated database document</summary>
    public Revit.DB.Document RvtDBDoc => _rvtUIDoc.Document;

    /// <summary>Project information element</summary>
    public Revit.DB.ProjectInfo ProjectInfo => RvtDBDoc.ProjectInformation;

    /// ================================================================================
    /// <summary>選択している配管</summary>
    /// 
    /// <history>2014/07/14 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Plumbing.Pipe> SelectPipeAry
    {
      get
      {
        Collections.Generic.IList<Revit.DB.Plumbing.Pipe> ret = new Collections.Generic.List<Revit.DB.Plumbing.Pipe>();

        Collections.Generic.ICollection<Revit.DB.ElementId> selectIds = RvtUIDoc.Selection.GetElementIds();

        foreach (Revit.DB.ElementId eId in selectIds)
        {
          Revit.DB.Element elem = RvtDBDoc.GetElement(eId);

          if (elem.Category != null)
          {
            if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_PipeCurves).ToString()))
            {
              Revit.DB.Plumbing.Pipe pipe = elem as Revit.DB.Plumbing.Pipe;

              ret.Add(pipe);
            }
          }
        }

        return ret;
      }
    }

    /// ================================================================================
    /// <summary>選択しているダクト</summary>
    /// 
    /// <history>2014/07/14 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Duct> SelectDuctAry
    {
      get
      {
        Collections.Generic.IList<Revit.DB.Mechanical.Duct> ret = new Collections.Generic.List<Revit.DB.Mechanical.Duct>();

        Collections.Generic.ICollection<Revit.DB.ElementId> selectIds = RvtUIDoc.Selection.GetElementIds();

        foreach (Revit.DB.ElementId eId in selectIds)
        {
          Revit.DB.Element elem = RvtDBDoc.GetElement(eId);

          if (elem.Category != null)
          {
            if (elem.Category.Id.ToString().Equals(((int)Revit.DB.BuiltInCategory.OST_DuctCurves).ToString()))
            {
              Revit.DB.Mechanical.Duct duct = elem as Revit.DB.Mechanical.Duct;

              ret.Add(duct);
            }
          }
        }

        return ret;
      }
    }

    /// ================================================================================
    /// <summary>全スペース</summary>
    /// 
    /// <history>2014/09/08 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Space> AllSpaceAry
    {
      get
      {
        Collections.Generic.IList<Revit.DB.Mechanical.Space> ret = new Collections.Generic.List<Revit.DB.Mechanical.Space>();

        Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
        fec.OfCategory(Revit.DB.BuiltInCategory.OST_MEPSpaces);
        fec.WhereElementIsNotElementType();

        foreach (Revit.DB.Mechanical.Space space in fec)
        {
          if (space.Location == null)
          {
            continue;
          }

          ret.Add(space);
        }

        return ret;
      }
    }

    /// ================================================================================
    /// <summary>スペース</summary>
    /// 
    /// <history>2014/10/30 Created GSA, Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Mechanical.Space> Spaces
    {
      get
      {
        return _Spaces;
      }
      set
      {
        _Spaces = value;
      }
    }
    
    #endregion
  }
}
