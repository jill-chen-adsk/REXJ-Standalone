using System;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.IO;
using Collections = System.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ClosedXML.Excel;


namespace PipeSizing.Components
{
  /// ================================================================================
  /// <summary>パラメータ</summary>
  /// ================================================================================
  public class Parameters
  {
    // メンバ変数
    #region Memeber Variables

    /// <summary>属性</summary>
    private Attribute _CmpAttribute;
    
    /// <summary>テーブル 系統</summary>
    private System.Data.DataTable _SystemTable;

    /// <summary>材質 特記</summary>
    private string _MaterialSpecial;

    /// <summary>材質マッピング</summary>
    private Collections.Generic.IDictionary<string, string> _DicMaterial;

    /// <summary>系統マッピング</summary>
    private Collections.Generic.IDictionary<string, Collections.Generic.IList<string>> _DicSystem;

    /// <summary>継手サイズ</summary>
    private string _FittingSize;

    /// <summary>直径</summary>
    private bool _IsDiameter;

    /// <summary>テーブル 空調</summary>
    private Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> _DicTable_Air;

    /// <summary>テーブル 給水</summary>
    private Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> _DicTable_Water;

    /// <summary>テーブル 給水タンク</summary>
    private Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> _DicTable_Water_Tank;

    /// <summary>テーブル 給湯</summary>
    private Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> _DicTable_HotWater;

    /// <summary>テーブル 排水</summary>
    private Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> _DicTable_Drain;

    /// <summary>系統別テーブル</summary>
    private Collections.Generic.IDictionary<string, Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable>> _DicSystemTable;

    #endregion

    // コンストラクタ
    #region Constructor
    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    /// 
    /// <param name="rvtUIDoc">Revit UIドキュメント</param>
    /// 
    /// <history>2014/07/14 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Parameters(Attribute cmpAttribute,
                      UIDocument rvtUIDoc)
    {
        _CmpAttribute = cmpAttribute;
    }

    private static string PipeSizingDataDir(string executeFolder) =>
      Path.Combine(executeFolder, "Data", "PipeSizing");

    private static string GetCellString(IXLWorksheet ws, int row, int col)
    {
      var c = ws.Cell(row, col);
      if (c.IsEmpty())
      {
        return string.Empty;
      }

      return c.GetFormattedString().Trim();
    }
    #endregion

    // メンバ関数
    #region Member Functions

    /// ================================================================================
    /// <summary>配管サイズテーブル(XML)取得</summary>
    /// 
    /// <history>2018/06/15 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool GetSizeTable()
    {
      // 戻り値
      bool ret = false;

      // 初期化
      _DicSystemTable = new Collections.Generic.Dictionary<string, Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable>>();

      string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
      string dataDir = PipeSizingDataDir(folderPath);

      // テーブル定義XML
      string tableXml = Path.Combine(dataDir, _CmpAttribute.ResourceText("IDS_TXT_DEFTABLE"));

      if (System.IO.File.Exists(tableXml))
      {
        Collections.Generic.IList<Collections.Generic.IList<string>> tableXmlList = new Collections.Generic.List<Collections.Generic.IList<string>>();
        string system = "";
        string pipeType = "";
        string mark = "";
        string fileName = "";

        XmlReader reader = XmlReader.Create(tableXml);
        
        while (reader.Read())
        {
          if (reader.NodeType == XmlNodeType.Element)
          {
            string localName = reader.LocalName;

            if (localName == "Values" ||
                localName == "Item")
            {
              continue;
            }
            else if (localName == "PipeType")
            {
              pipeType = reader.ReadString();
            }
            else if (localName == "Mark")
            {
              mark = reader.ReadString();
            }
            else if (localName == "FileName")
            {
              fileName = reader.ReadString();

              Collections.Generic.IList<string> list = new Collections.Generic.List<string>();
              list.Add(system);
              list.Add(pipeType);
              list.Add(mark);
              list.Add(fileName);

              tableXmlList.Add(list);

              // 1行以上
              ret = true;
            }
            else
            {
              // 既定のタグ名以外は系統とみなす
              
              if (_DicSystem.ContainsKey(localName))
              {
                system = _DicSystem[localName][0];
              }
              else
              {
                system = localName;
              }
            }
          }
        }

        foreach (Collections.Generic.IList<string> list in tableXmlList)
        {
          system    = list[0];
          pipeType  = list[1];
          mark      = list[2];
          fileName  = list[3];

          System.Data.DataTable table = new System.Data.DataTable();
          table.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_VALMIN"), typeof(double));
          table.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_VALMAX"), typeof(double));
          table.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_VALUE"), typeof(double));

          Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
          names.Add(pipeType);
          names.Add(mark);

          string xmlPath = Path.Combine(dataDir, fileName);

          if (System.IO.File.Exists(xmlPath))
          {
            System.Data.DataRow row = null;

            reader = new XmlTextReader(xmlPath);

            while (reader.Read())
            {
              if (reader.NodeType == XmlNodeType.Element)
              {
                switch (reader.LocalName)
                {
                  case "Item":
                    row = table.NewRow();

                    break;
                  case "ValMin":
                    row[_CmpAttribute.ResourceText("IDS_TXT_VALMIN")] = double.Parse(reader.ReadString());

                    break;
                  case "ValMax":
                    row[_CmpAttribute.ResourceText("IDS_TXT_VALMAX")] = double.Parse(reader.ReadString());

                    break;
                  case "Value":
                    row[_CmpAttribute.ResourceText("IDS_TXT_VALUE")] = double.Parse(reader.ReadString());

                    table.Rows.Add(row);

                    break;
                }
              }
            }

            // 既存系統
            if (_DicSystemTable.ContainsKey(system))
            {
              if (_DicSystemTable[system].ContainsKey(names))
              {
                // 系統、材質が同じものは除外
              }
              else
              {
                _DicSystemTable[system].Add(names, table);
              }
            }
            else
            {
              Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> dicTable = new Collections.Generic.Dictionary<Collections.Generic.IList<string>, System.Data.DataTable>();
              dicTable.Add(names, table);

              _DicSystemTable.Add(system, dicTable);
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>配管サイズテーブル(XML)取得 - 旧コード</summary>
    /// 
    /// <history><p>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2018/06/14 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    bool GetSizeTable_OLD()
    {
      // 戻り値
      bool ret = false;

      // 初期化
      _DicTable_Air         = new Collections.Generic.Dictionary<Collections.Generic.IList<string>, System.Data.DataTable>();
      _DicTable_Water       = new Collections.Generic.Dictionary<Collections.Generic.IList<string>, System.Data.DataTable>();
      _DicTable_Water_Tank  = new Collections.Generic.Dictionary<Collections.Generic.IList<string>, System.Data.DataTable>();
      _DicTable_HotWater    = new Collections.Generic.Dictionary<Collections.Generic.IList<string>, System.Data.DataTable>();
      _DicTable_Drain       = new Collections.Generic.Dictionary<Collections.Generic.IList<string>, System.Data.DataTable>();

      string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
      string dataDir = PipeSizingDataDir(folderPath);

      // テーブル定義XML
      string tableXml = Path.Combine(dataDir, _CmpAttribute.ResourceText("IDS_TXT_DEFTABLE"));

      if (System.IO.File.Exists(tableXml))
      {
        Collections.Generic.IList<Collections.Generic.IList<string>> tableXmlList = new Collections.Generic.List<Collections.Generic.IList<string>>();
        string system   = "";
        string pipeType = "";
        string mark     = "";
        string fileName = "";

        XmlTextReader reader = new XmlTextReader(tableXml);

        while (reader.Read())
        {
          if (reader.NodeType == XmlNodeType.Element)
          {
            switch (reader.LocalName)
            {
              case "Air":
                system = _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_AIR");

                break;
              case "Water":
                system = _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_WATER");

                break;
              case "Water_Tank":
                system = _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_WATERTANK");

                break;
              case "HotWater":
                system = _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_HOTWATER");

                break;
              case "Drain":
                system = _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_DRAIN");

                break;
              case "PipeType":
                pipeType = reader.ReadString();

                break;
              case "Mark":
                mark = reader.ReadString();

                break;
              case "FileName":
                fileName = reader.ReadString();

                Collections.Generic.IList<string> list = new Collections.Generic.List<string>();
                list.Add(system);
                list.Add(pipeType);
                list.Add(mark);
                list.Add(fileName);

                tableXmlList.Add(list);

                ret = true; // 1行以上

                break;
            }
          }
        }
        
        foreach (Collections.Generic.IList<string> list in tableXmlList)
        {
          system    = list[0];
          pipeType  = list[1];
          mark      = list[2];
          fileName  = list[3];

          System.Data.DataTable table = new System.Data.DataTable();
          table.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_VALMIN"), typeof(double));
          table.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_VALMAX"), typeof(double));
          table.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_VALUE"), typeof(double));

          Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
          names.Add(pipeType);
          names.Add(mark);

          string xmlPath = Path.Combine(dataDir, fileName);

          if (System.IO.File.Exists(xmlPath))
          {
            System.Data.DataRow row = null;

            reader = new XmlTextReader(xmlPath);

            while (reader.Read())
            {
              if (reader.NodeType == XmlNodeType.Element)
              {
                switch (reader.LocalName)
                {
                  case "Item":
                    row = table.NewRow();

                    break;
                  case "ValMin":
                    row[_CmpAttribute.ResourceText("IDS_TXT_VALMIN")] = double.Parse(reader.ReadString());

                    break;
                  case "ValMax":
                    row[_CmpAttribute.ResourceText("IDS_TXT_VALMAX")] = double.Parse(reader.ReadString());

                    break;
                  case "Value":
                    row[_CmpAttribute.ResourceText("IDS_TXT_VALUE")] = double.Parse(reader.ReadString());

                    table.Rows.Add(row);

                    break;
                }
              }
            }

            if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_AIR"))
            {
              _DicTable_Air.Add(names, table);
            }
            else if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_WATER"))
            {
              _DicTable_Water.Add(names, table);
            }
            else if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_WATERTANK"))
            {
              _DicTable_Water_Tank.Add(names, table);
            }
            else if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_HOTWATER"))
            {
              _DicTable_HotWater.Add(names, table);
            }
            else if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_DRAIN"))
            {
              _DicTable_Drain.Add(names, table);
            }
          }
        }
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>システムタイプ定義テーブル(Excel)取得</summary>
    /// 
    /// <history><p>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2018/09/26 Modified CST,Co.Lt.d. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    bool GetSystemTypeTable()
    {
      // 戻り値
      bool ret = false;

      // 初期化
      _SystemTable = new System.Data.DataTable();
      _SystemTable.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_SYSTEMTYPE"), typeof(string));
      _SystemTable.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_SYSTEM"), typeof(string));
      _SystemTable.Columns.Add(_CmpAttribute.ResourceText("IDS_TXT_MATERIAL"), typeof(string));

      string folderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? ".";
      string filePath = Path.Combine(PipeSizingDataDir(folderPath), _CmpAttribute.ResourceText("IDS_TXT_DEFSYSTEMFILE"));

      if (!File.Exists(filePath))
      {
        return ret;
      }

      try
      {
        using var workbook = new XLWorkbook(filePath);

        string dataSheetName = _CmpAttribute.ResourceText("IDS_TXT_SHEETNAME_DATA");
        IXLWorksheet wsData = workbook.Worksheets.FirstOrDefault(w => w.Name == dataSheetName);
        if (wsData == null)
        {
          return ret;
        }

        int usedRows = wsData.LastRowUsed()?.RowNumber() ?? 0;

        _DicMaterial = new Collections.Generic.Dictionary<string, string>();

        for (int i = 2; i <= usedRows; ++i)
        {
          string key = GetCellString(wsData, i, 5);
          string value = GetCellString(wsData, i, 6);

          if (string.IsNullOrWhiteSpace(key) ||
              string.IsNullOrWhiteSpace(value))
          {
            break;
          }

          if (_DicMaterial.ContainsKey(key) == false)
          {
            _DicMaterial.Add(key, value);
          }
        }

        _DicSystem = new Collections.Generic.Dictionary<string, Collections.Generic.IList<string>>();

        for (int i = 2; i <= usedRows; ++i)
        {
          string key = GetCellString(wsData, i, 2);
          string value1 = GetCellString(wsData, i, 1);
          string value2 = GetCellString(wsData, i, 3);

          if (string.IsNullOrWhiteSpace(key) ||
              string.IsNullOrWhiteSpace(value1) ||
              string.IsNullOrWhiteSpace(value2))
          {
            break;
          }

          if (_DicSystem.ContainsKey(key) == false)
          {
            Collections.Generic.IList<string> values = new Collections.Generic.List<string>();
            values.Add(value1);
            values.Add(value2);

            _DicSystem.Add(key, values);
          }
        }

        string sheetName = _CmpAttribute.ResourceText("IDS_TXT_SHEETNAME");
        IXLWorksheet wsSys = workbook.Worksheets.FirstOrDefault(w => w.Name == sheetName);
        if (wsSys == null)
        {
          return ret;
        }

        usedRows = wsSys.LastRowUsed()?.RowNumber() ?? 0;

        _MaterialSpecial = GetCellString(wsSys, 3, 2);

        _FittingSize = GetCellString(wsSys, 3, 5);

        string strDiamter = GetCellString(wsSys, 3, 7);
        if (strDiamter == "直径")
        {
          _IsDiameter = true;
        }
        else
        {
          _IsDiameter = false;
        }

        int maxRowNum = usedRows;

        for (int i = 6; i <= maxRowNum; ++i)
        {
          string systemType = GetCellString(wsSys, i, 1);
          string system = GetCellString(wsSys, i, 2);
          string material = GetCellString(wsSys, i, 3);

          if (string.IsNullOrWhiteSpace(systemType) ||
              string.IsNullOrWhiteSpace(system) ||
              string.IsNullOrWhiteSpace(material))
          {
            continue;
          }

          if (_DicMaterial.ContainsKey(material))
          {
            material = _DicMaterial[material];
          }
          else
          {
            material = "";
          }

          System.Data.DataRow row = _SystemTable.NewRow();

          row[_CmpAttribute.ResourceText("IDS_TXT_SYSTEMTYPE")] = systemType;
          row[_CmpAttribute.ResourceText("IDS_TXT_SYSTEM")] = system;
          row[_CmpAttribute.ResourceText("IDS_TXT_MATERIAL")] = material;

          _SystemTable.Rows.Add(row);
        }

        ret = true;
      }
      catch
      {
        return false;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>系統テーブル取得</summary>
    /// 
    /// <param name="system">システム名</param>
    /// 
    /// <history>2018/06/15 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> GetDicSystemTable(string system)
    {
      // 戻り値
      Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> ret = null;

      if (_DicSystemTable.ContainsKey(system))
      {
        ret = _DicSystemTable[system];
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>系統テーブル取得 - 旧コード</summary>
    /// 
    /// <param name="system">システム名</param>
    /// 
    /// <history>2018/06/12 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> GetDicSystemTable_OLD(string system)
    {
      // 戻り値
      Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> ret = null;

      if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_AIR"))
      {
        ret = DicTable_Air;
      }
      else if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_WATER"))
      {
        ret = DicTable_Water;
      }
      else if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_WATERTANK"))
      {
        ret = DicTable_Water_Tank;
      }
      else if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_HOTWATER"))
      {
        ret = DicTable_HotWater;
      }
      else if (system == _CmpAttribute.ResourceText("IDS_TXT_SYSTEM_DRAIN"))
      {
        ret = DicTable_Drain;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>配管サイズ変更</summary>
    /// 
    /// <param name="pipe"  >配管</param>
    /// <param name="table" >サイズテーブル</param>
    /// <param name="value" >値</param>
    /// 
    /// <history>2017/09/28 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void PipeSizing(Element pipe,
                    System.Data.DataTable table,
                    double value)
    {
      // セットするサイズ
      double setVal = 0;

      bool getVal = false;

      for (int i = 0; i < table.Rows.Count; ++i)
      {
        System.Data.DataRow row = table.Rows[i];

        // 最小値
        double min = (double)row[0];

        // 最大値
        double max = (double)row[1];

        // サイズ
        double size = (double)row[2];

        // 最小値を超え、最大値以下
        if (min < value && value <= max)
        {
          setVal = size;
          getVal = true;

          break;
        }
      }

      // テーブル範囲内
      if (getVal)
      {
        if (pipe.Category != null)
        {
          // 配管、フレキシブル配管
          if (pipe.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_PipeCurves).ToString()) ||
              pipe.Category.Id.ToString().Equals(((int)BuiltInCategory.OST_FlexPipeCurves).ToString()))
          {
            // 直径
            Parameter parDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);

            if (parDiameter != null &&
                parDiameter.IsReadOnly == false)
            {
              parDiameter.Set(setVal / 304.8);
            }
          }
        }
      }
    }

    /// ================================================================================
    /// <summary>継手、付属品サイズ変更</summary>
    /// 
    /// <param name="famIns">継手、付属品</param>
    /// <param name="value" >値</param>
    /// 
    /// <history>2018/06/13 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    void FittingAccessorySizing(FamilyInstance famIns,
                                double value)
    {
      // サイズパラメータ
      Parameter parSize = famIns.LookupParameter(FittingSize);

      if (parSize != null &&
          parSize.IsReadOnly == false)
      {
        // 直径
        if (IsDiameter)
        {
          parSize.Set(value);
        }
        // 半径
        else
        {
          parSize.Set(value / 2);
        }
      }
    }

    /// ================================================================================
    /// <summary>流量系システム判定</summary>
    /// 
    /// <param name="system">システム名</param>
    /// 
    /// <history>2018/06/15 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsFlowSystem(string system)
    {
      // 戻り値
      bool ret = false;

      // 系統マッピングテーブル
      foreach (Collections.Generic.IList<string> values in _DicSystem.Values)
      {
        if (values[0] == system &&
            values[1] == _CmpAttribute.ResourceText("IDS_TXT_FLOW"))
        {
          ret = true;
        }
      }

      return ret;
    }

    #endregion

    // プロパティ
    #region Properties

    /// ================================================================================
    /// <summary>システムタイプ定義テーブル</summary>
    /// 
    /// <history>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    System.Data.DataTable SystemTable
    {
      get
      {
        return _SystemTable;
      }
    }

    /// ================================================================================
    /// <summary>空調テーブル</summary>
    /// 
    /// <history>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> DicTable_Air
    {
      get
      {
        return _DicTable_Air;
      }
    }

    /// ================================================================================
    /// <summary>給水テーブル</summary>
    /// 
    /// <history>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> DicTable_Water
    {
      get
      {
        return _DicTable_Water;
      }
    }

    /// ================================================================================
    /// <summary>給水タンクテーブル</summary>
    /// 
    /// <history>2018/06/14 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> DicTable_Water_Tank
    {
      get
      {
        return _DicTable_Water_Tank;
      }
    }

    /// ================================================================================
    /// <summary>給湯テーブル</summary>
    /// 
    /// <history>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> DicTable_HotWater
    {
      get
      {
        return _DicTable_HotWater;
      }
    }

    /// ================================================================================
    /// <summary>排水テーブル</summary>
    /// 
    /// <history>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> DicTable_Drain
    {
      get
      {
        return _DicTable_Drain;
      }
    }

    /// ================================================================================
    /// <summary>材質 特記</summary>
    /// 
    /// <history>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    string MaterialSpecial
    {
      get
      {
        return _MaterialSpecial;
      }
    }

    /// ================================================================================
    /// <summary>材質マッピング</summary>
    /// 
    /// <history>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    Collections.Generic.IDictionary<string, string> DicMaterial
    {
      get
      {
        return _DicMaterial;
      }
    }

    /// ================================================================================
    /// <summary>継手サイズ</summary>
    /// 
    /// <history>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    string FittingSize
    {
      get
      {
        return _FittingSize;
      }
    }

    /// ================================================================================
    /// <summary>直径</summary>
    /// 
    /// <history>2017/09/27 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsDiameter
    {
      get
      {
        return _IsDiameter;
      }
    }
    
    #endregion
  }
}
