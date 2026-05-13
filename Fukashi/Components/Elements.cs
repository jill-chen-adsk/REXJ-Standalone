using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;

namespace ADSK.Ext.Fukashi.Components
{
  /// ================================================================================
  /// <summary>要素</summary>
  /// ================================================================================
  public class Elements
  {
    private const double DefaultApprox0Len = 1e-6;

    public Revit.UI.UIDocument RvtUIDoc { get; }
    public Revit.DB.Document RvtDBDoc { get; }
    public double Approx0Len => DefaultApprox0Len;

    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    ///
    /// <param name="rvtUIDoc">Revit UIドキュメント</param>
    ///
    /// <history>2016/11/29 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Elements(Revit.UI.UIDocument rvtUIDoc)
    {
      RvtUIDoc = rvtUIDoc;
      RvtDBDoc = rvtUIDoc.Document;
    }

    /// ================================================================================
    /// <summary>マテリアル取得</summary>
    ///
    /// <history>2016/11/29 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Material> GetMaterials()
    {
      Collections.Generic.List<Revit.DB.Material> ret = new Collections.Generic.List<Revit.DB.Material>();

      Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
      fec.OfCategory(Autodesk.Revit.DB.BuiltInCategory.OST_Materials);

      foreach (Revit.DB.Material material in fec)
      {
        ret.Add(material);
      }

      ret.Sort(new MaterialNameComparer());

      return ret;
    }

    /// ================================================================================
    /// <summary>高さ順レベル取得</summary>
    ///
    /// <history>2016/12/05 Created  CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Level> GetElevationOrderLevels()
    {
      Collections.Generic.List<Revit.DB.Level> ret = new Collections.Generic.List<Revit.DB.Level>();

      Revit.DB.FilteredElementCollector fec = new Revit.DB.FilteredElementCollector(RvtDBDoc);
      fec.OfClass(typeof(Revit.DB.Level));

      foreach (Revit.DB.Level level in fec)
      {
        ret.Add(level);
      }

      ret.Sort(new LevelElevationComparer());

      return ret;
    }

    /// ================================================================================
    /// <summary>上レベル取得</summary>
    ///
    /// <param name="level">レベル</param>
    ///
    /// <history>2017/01/10 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IList<Revit.DB.Level> GetUpperLevels(Revit.DB.Level level)
    {
      Collections.Generic.IList<Revit.DB.Level> ret = new Collections.Generic.List<Revit.DB.Level>();

      Collections.Generic.IList<Revit.DB.Level> levels = GetElevationOrderLevels();

      foreach (Revit.DB.Level lvl in levels)
      {
        if (level.Elevation < lvl.Elevation)
        {
          ret.Add(lvl);
        }
        else if (System.Math.Abs(level.Elevation - lvl.Elevation) < Approx0Len)
        {
          ret.Add(lvl);
        }
      }

      return ret;
    }
  }

  /// ================================================================================
  /// <summary>マテリアル名の並び替え</summary>
  ///
  /// <history>2016/11/29 Created GSA,Inc. Ryo Kuroda</history>
  /// ================================================================================
  public class MaterialNameComparer : System.Collections.Generic.IComparer<Revit.DB.Material>
  {
    public int Compare(Revit.DB.Material materialA, Revit.DB.Material materialB)
    {
      int ret = 0;

      string nameA = materialA.Name;
      string nameB = materialB.Name;

      ret = string.Compare(nameA, nameB);

      return ret;
    }
  }

  /// ================================================================================
  /// <summary>レベル高さの並び替え</summary>
  ///
  /// <history>2016/12/05 Created  CST,Co.Ltd. Ryo Kuroda</history>
  /// ================================================================================
  public class LevelElevationComparer : System.Collections.Generic.IComparer<Revit.DB.Level>
  {
    public int Compare(Revit.DB.Level levelA, Revit.DB.Level levelB)
    {
      int ret = 0;

      double elevA = levelA.Elevation;
      double elevB = levelB.Elevation;

      if (elevA == elevB)
      {
        ret = string.Compare(levelA.Name, levelB.Name);
      }
      else if (elevA < elevB)
      {
        ret = -1;
      }
      else if (elevA > elevB)
      {
        ret = 1;
      }

      return ret;
    }
  }
}
