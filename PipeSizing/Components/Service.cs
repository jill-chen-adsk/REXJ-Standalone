using System;
using Collections = System.Collections;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;


namespace PipeSizing.Components
{
  /// ================================================================================
  /// <summary>サービス</summary>
  /// ================================================================================
  class Service
  {
    // メンバ変数
    #region Memeber Variables

    /// <summary>属性</summary>
    private Attribute _CmpAttribute;
    /// <summary>要素</summary>
    private Elements _CmpElements;
    /// <summary>図形</summary>
    private Geometry _CmpGeometry;
    /// <summary>パラメータ</summary>
    private Parameters _CmpParameters;
    /// <summary>設定</summary>
    private Settings _CmpSettings;

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
    public Service(Attribute cmpAttribute,
                   Elements cmpElements,
                   Geometry cmpGeometry,
                   Parameters cmpParameters,
                   Settings cmpSettings)
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
    /// <summary>Excel確認</summary>
    /// 
    /// <history>2017/10/03 Created CST,Co.Ltd. Ryo Kuroda</history>
    /// ================================================================================
    public
    bool IsExcelInComputer()
    {
      // 戻り値
      bool ret = false;

      try
      {
        System.Type type = System.Type.GetTypeFromProgID("Excel.Application");

        // Wordの場合
        //System.Type wordType = System.Type.GetTypeFromProgID("Word.Application");

        if (type == null)
        {
          ret = false;
        }
        else if (type != null)
        {
          ret = true;
        }
      }
      catch
      {
        return ret;
      }

      return ret;
    }

    /// ================================================================================
    /// <summary>配管サイズ補正</summary>
    /// 
    /// <param name="elem">配管要素</param>
    /// 
    /// <history><p>2018/06/19 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2018/06/25 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    void PipeSizing(Element elem)
    {
      // システムタイプ
      Parameter param = elem.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM);
      string systemType = param.AsValueString();

      // 材質 特記
      string valMaterial = "";

      string materialSpecial = _CmpParameters.MaterialSpecial;
      if (string.IsNullOrEmpty(materialSpecial) == false)
      {
        Parameter paramMaterial = elem.LookupParameter(materialSpecial);
        
        if (paramMaterial != null)
        {
          if (paramMaterial.StorageType == StorageType.String)
          {
            valMaterial = paramMaterial.AsString();
          }
          else
          {
            valMaterial = paramMaterial.AsValueString();
          }
        }
      }

      // 負荷単位
      Parameter parFixturesUnit = elem.get_Parameter(BuiltInParameter.RBS_PIPE_FIXTURE_UNITS_PARAM);

      // 流量
      Parameter parFlow = elem.get_Parameter(BuiltInParameter.RBS_PIPE_FLOW_PARAM);

      Collections.Generic.IDictionary<string, string> dicMaterial = _CmpParameters.DicMaterial;

      // システムタイプ定義テーブル
      System.Data.DataTable systemTable = _CmpParameters.SystemTable;

      foreach (System.Data.DataRow row in systemTable.Rows)
      {
        // システムタイプ
        if (systemType == (string)row[_CmpAttribute.ResourceText("IDS_TXT_SYSTEMTYPE")])
        {
          string system   = (string)row[_CmpAttribute.ResourceText("IDS_TXT_SYSTEM")];
          string material = (string)row[_CmpAttribute.ResourceText("IDS_TXT_MATERIAL")];

          // 特記材質で上書き
          if (string.IsNullOrEmpty(valMaterial) == false)
          {
            if (dicMaterial.ContainsKey(valMaterial))
            {
              material = dicMaterial[valMaterial];
            }
          }

          // 系統テーブル
          Collections.Generic.IDictionary<Collections.Generic.IList<string>, System.Data.DataTable> dicTable = _CmpParameters.GetDicSystemTable(system);

          if (dicTable == null)
          {
            break;
          }

          foreach (Collections.Generic.IList<string> list in dicTable.Keys)
          {
            System.Data.DataTable table = dicTable[list];

            if (list[1] == material)
            {
              // 空調は「流量」
              if(_CmpParameters.IsFlowSystem(system))
              {
                if (parFlow != null)
                {
                  // 流量
                  double flow = parFlow.AsDouble();

                  // 単位換算［Revit内部単位 → サイズテーブル単位］
                  // 立方フィート毎秒(ft^3/s) → リットル毎分(L/min)
                  // L = 10cm * 10cm * 10cm
                  // 1ft = 304.8mm = 30.48cm = 10 cm * 3.048
                  double value = flow * 3.048 * 3.048 * 3.048 * 60;

                  // 2019/10/31 Round value
                  value = Math.Round(value, 5);

                  //// 単位変換関数
                  //// 内部単位系から指定の単位系に変換
                  //double value = Revit.DB.UnitUtils.ConvertFromInternalUnits(flow, Revit.DB.DisplayUnitType.DUT_LITERS_PER_MINUTE);
                  
                  // サイズ補正
                  _CmpParameters.PipeSizing(elem,
                                            table,
                                            value);
                }
              }
              // その他は「負荷単位」
              else
              {
                if (parFixturesUnit != null)
                {
                  // 負荷単位
                  double value = parFixturesUnit.AsDouble();

                  // サイズ補正
                  _CmpParameters.PipeSizing(elem,
                                            table,
                                            value);
                }
              }
            }
          }

          break;
        }
      }
    }

    /// ================================================================================
    /// <summary>継手、付属品サイズ補正</summary>
    /// 
    /// <param name="famIns">継手、付属品要素</param>
    /// 
    /// <history><p>2018/06/12 Created CST,Co.Ltd. Ryo Kuroda</p>
    ///           <p>2018/06/22 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
    /// ================================================================================
    public
    void FittingAccessorySizing(FamilyInstance famIns)
    {
      MEPModel mepMdl = famIns.MEPModel;

      if (mepMdl != null)
      {
        ConnectorManager mgr = mepMdl.ConnectorManager;

        if (mgr != null)
        {
          // 継手につながる配管
          Collections.Generic.IList<Pipe> sameConnectorPipe = _CmpElements.GetSameConnectorPipe(famIns,
                                                                                                                  _CmpElements.AllPipeAry);

          // 配管なし
          if (sameConnectorPipe.Count < 1)
          {
            return;
          }

          // 配管の最大直径
          double maxVal = 0;

          foreach (Pipe pipe in sameConnectorPipe)
          {
            System.Data.DataTable systemTable = _CmpParameters.SystemTable;

            // システムタイプ
            Parameter param = pipe.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM);
            string systemType = param.AsValueString();

            foreach (System.Data.DataRow row in systemTable.Rows)
            {
              if (systemType == (string)row[_CmpAttribute.ResourceText("IDS_TXT_SYSTEMTYPE")])
              {
                string system = (string)row[_CmpAttribute.ResourceText("IDS_TXT_SYSTEM")];

                // 空調は「流量」
                if(_CmpParameters.IsFlowSystem(system))
                {
                  // 流量
                  Parameter parFlow = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_FLOW_PARAM);

                  if (parFlow != null && parFlow.HasValue)
                  {
                    // 直径
                    Parameter parDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);

                    if (maxVal < parDiameter.AsDouble())
                    {
                      maxVal = parDiameter.AsDouble();
                    }
                  }
                }
                // その他は「負荷単位」
                else
                {
                  // 負荷単位
                  Parameter parFixtureUnit = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_FIXTURE_UNITS_PARAM);

                  if (parFixtureUnit != null &&
                      parFixtureUnit.HasValue == true)
                  {
                    // 直径
                    Parameter parDiameter = pipe.get_Parameter(BuiltInParameter.RBS_PIPE_DIAMETER_PARAM);

                    if (maxVal < parDiameter.AsDouble())
                    {
                      maxVal = parDiameter.AsDouble();
                    }
                  }
                }
              }
            }
          }

          // サイズ取得成功
          if (_CmpGeometry.ToHalfAdjust(maxVal, -9) > 0)
          {
            // サイズ補正
            _CmpParameters.FittingAccessorySizing(famIns,
                                                  maxVal);
          }
        }
      }
    }

    #endregion

    // プロパティ
    #region Properties

    #endregion
  }
}
