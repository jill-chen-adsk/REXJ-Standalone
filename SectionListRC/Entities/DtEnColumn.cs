using System;
using SectionListRC.Utils ;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListRC.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - 円柱</summary>
  /// ================================================================================
  public class DtEnColumn : SectionListRC.Entities.DtBase
  {
    // メンバ変数
    #region Member Variables

      private SectionListRC.Components.Parameters _CmpParameters;

      private SectionListRC.Components.Elements _CmpElements;

      private SectionListRC.Entities.SpEnColumn _EntSpEnColumn;

      private System.Data.DataTable _Data;

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
      /// <history>2013/04/15 Created  GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public DtEnColumn(SectionListRC.Components.Attribute cmpAttribute,
                        SectionListRC.Components.Elements cmpElements,
                        SectionListRC.Components.Geometry cmpGeometry,
                        SectionListRC.Components.Parameters cmpParameters,
                        SectionListRC.Components.Settings cmpSettings) :
             base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
      {
        _CmpParameters    = cmpParameters;
        _CmpElements      = cmpElements;
        _EntSpEnColumn  = new SectionListRC.Entities.SpEnColumn(cmpAttribute, cmpParameters, cmpSettings);

        if (_EntSpEnColumn.DefSuccess == false)
        {

        }

      }
    #endregion

    // メンバ関数
    #region Member Functions

      /// ================================================================================
      /// <summary>データ書式定義</summary>
      /// 
      /// <param name="data">データテーブル</param>
      /// 
      /// <history>2013/04/15 Created GSA,Inc. Ryo Kurodai</history>
      /// ================================================================================
      private
      bool DefDataFormat(ref System.Data.DataTable data)
      {
        bool ret = false;

        // 柱符号
        data.Columns.Add(_CmpParameters.RST_HasiraHugo_En, typeof(string));

        // レベル
        data.Columns.Add(_CmpParameters.LevelFrameTitle, typeof(string));

        // 柱分類
        data.Columns.Add(_CmpParameters.Column_Category_En, typeof(string));

        // 直径
        data.Columns.Add(_CmpParameters.Tyokkei_En, typeof(double));

        // 柱頭主筋径
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinKei_En, typeof(string));

        // 柱脚主筋径
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinKei_En, typeof(string));

        // 柱頭主筋本数
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinHonsu_En, typeof(int));

        // 柱脚主筋本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinHonsu_En, typeof(int));

        // 芯鉄筋径
        data.Columns.Add(_CmpParameters.RST_SintekkinKei_En, typeof(string));

        // 芯鉄筋本数
        data.Columns.Add(_CmpParameters.RST_SintekkinHonsu_En, typeof(int));

        // 芯鉄筋位置
        data.Columns.Add(_CmpParameters.RST_SintekkinIchi_En, typeof(double));

        // 柱頭フープX径
        data.Columns.Add(_CmpParameters.RST_ChutoHoopXKei_En, typeof(string));

        // 柱脚フープX径
        data.Columns.Add(_CmpParameters.RST_ChukyakuHoopXKei_En, typeof(string));

        // 柱頭フープピッチ
        data.Columns.Add(_CmpParameters.RST_ChutoHoopPitch_En, typeof(double));

        // 柱脚フープピッチ
        data.Columns.Add(_CmpParameters.RST_ChukyakuHoopPitch_En, typeof(double));

        // 幅止筋径
        data.Columns.Add(_CmpParameters.RST_HabadomekinKei_En, typeof(string));

        // Top_Spacing_XDirectionNumber
        data.Columns.Add(_CmpParameters.Top_Spacing_XDirectionNumber_En, typeof(int));

        // Bottom_Spacing_XDirectionNumber
        data.Columns.Add(_CmpParameters.Bottom_Spacing_XDirectionNumber_En, typeof(int));

        // Top_Spacing_YDirectionNumber
        data.Columns.Add(_CmpParameters.Top_Spacing_YDirectionNumber_En, typeof(int));

        // Bottom_Spacing_YDirectionNumber
        data.Columns.Add(_CmpParameters.Bottom_Spacing_YDirectionNumber_En, typeof(int));

        // 幅止筋ピッチ
        data.Columns.Add(_CmpParameters.RST_HabadomekinPitch_En, typeof(double));

        return ret;
      }

      /// ================================================================================
      /// <summary>データ取得</summary>
      /// 
      /// <param name="famSymColumn">柱</param>
      /// 
      /// <history>2013/04/15 Created GSA,Inc. Ryo Kurodai</history>
      /// ================================================================================
      public
      System.Data.DataRow GetData(Revit.DB.FamilySymbol famSymColumn)
      {
        // 初期化
        System.Data.DataRow row = null;

        if (_Data == null)
        {
          _Data = new System.Data.DataTable();
          DefDataFormat(ref _Data);
        }


        if (famSymColumn != null)
        {
          // 要素
          Revit.DB.FamilySymbol famSymColumnType = famSymColumn;
          if (famSymColumnType == null)
          {
            return row;
          }

          _EntSpEnColumn.CurrentElem = famSymColumn;

          row = _Data.NewRow();

          string typeName = "";
          string levelName = "0";
          _CmpElements.GetTypeMarkLevel(famSymColumn, ref typeName, ref levelName, _CmpParameters.RST_HasiraHugo_Kaku);

          // 柱符号
          row[_CmpParameters.RST_HasiraHugo_En] = typeName;

          // 階
          row[_CmpParameters.LevelFrameTitle] = levelName;
          
          // 柱分類
          row[_CmpParameters.Column_Category_En] = famSymColumnType.LookupParameter(_CmpParameters.Column_Category_En).AsString();

          // 直径
          row[_CmpParameters.Tyokkei_En] = famSymColumnType.LookupParameter(_CmpParameters.Tyokkei_En).AsDouble();
          if ((double)row[_CmpParameters.Tyokkei_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.Tyokkei_En).AsInteger() != 0)
          {
            row[_CmpParameters.Tyokkei_En] = famSymColumnType.LookupParameter(_CmpParameters.Tyokkei_En).AsInteger();
          }

          // 柱頭主筋径
          row[_CmpParameters.RST_ChutoSyukinKei_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChutoSyukinKei_En).AsString();

          // 柱脚主筋径
          row[_CmpParameters.RST_ChukyakuSyukinKei_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChukyakuSyukinKei_En).AsString();

          // 柱頭主筋本数
          row[_CmpParameters.RST_ChutoSyukinHonsu_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChutoSyukinHonsu_En).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoSyukinHonsu_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.RST_ChutoSyukinHonsu_En).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoSyukinHonsu_En] = System.Math.Round(famSymColumnType.LookupParameter(_CmpParameters.RST_ChutoSyukinHonsu_En).AsDouble(), MidpointRounding.AwayFromZero);
          }

          // 柱脚主筋本数
          row[_CmpParameters.RST_ChukyakuSyukinHonsu_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChukyakuSyukinHonsu_En).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuSyukinHonsu_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.RST_ChukyakuSyukinHonsu_En).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuSyukinHonsu_En] = System.Math.Round(famSymColumnType.LookupParameter(_CmpParameters.RST_ChukyakuSyukinHonsu_En).AsDouble(), MidpointRounding.AwayFromZero);
          }

          // 芯鉄筋径
          row[_CmpParameters.RST_SintekkinKei_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_SintekkinKei_En).AsString();

          // 芯鉄筋本数
          row[_CmpParameters.RST_SintekkinHonsu_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_SintekkinHonsu_En).AsInteger();
          if ((int)row[_CmpParameters.RST_SintekkinHonsu_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.RST_SintekkinHonsu_En).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SintekkinHonsu_En] = System.Math.Round(famSymColumnType.LookupParameter(_CmpParameters.RST_SintekkinHonsu_En).AsDouble(), MidpointRounding.AwayFromZero);
          }

          // 芯鉄筋位置
          row[_CmpParameters.RST_SintekkinIchi_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_SintekkinIchi_En).AsDouble();
          if ((double)row[_CmpParameters.RST_SintekkinIchi_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.RST_SintekkinIchi_En).AsInteger() != 0)
          {
            row[_CmpParameters.RST_SintekkinIchi_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_SintekkinIchi_En).AsInteger();
          }

          // 柱頭フープX径
          row[_CmpParameters.RST_ChutoHoopXKei_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChutoHoopXKei_En).AsString();

          // 柱脚フープX径
          row[_CmpParameters.RST_ChukyakuHoopXKei_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChukyakuHoopXKei_En).AsString();

          // 柱頭フープピッチ
          row[_CmpParameters.RST_ChutoHoopPitch_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChutoHoopPitch_En).AsDoubleMm();
          if ((double)row[_CmpParameters.RST_ChutoHoopPitch_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.RST_ChutoHoopPitch_En).AsInteger() != 0)
          {
            row[_CmpParameters.RST_ChutoHoopPitch_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChutoHoopPitch_En).AsInteger();
          }
          
          // 柱脚フープピッチ
          row[_CmpParameters.RST_ChukyakuHoopPitch_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChukyakuHoopPitch_En).AsDoubleMm();
          if ((double)row[_CmpParameters.RST_ChukyakuHoopPitch_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.RST_ChukyakuHoopPitch_En).AsInteger() != 0)
          {
            row[_CmpParameters.RST_ChukyakuHoopPitch_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_ChukyakuHoopPitch_En).AsInteger();
          }

          // 幅止筋径
          row[_CmpParameters.RST_HabadomekinKei_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_HabadomekinKei_En).AsString();

          // Top_Spacing_XDirectionNumber
          row[_CmpParameters.Top_Spacing_XDirectionNumber_En] = famSymColumnType.LookupParameter(_CmpParameters.Top_Spacing_XDirectionNumber_En).AsInteger();
          if ((int)row[_CmpParameters.Top_Spacing_XDirectionNumber_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.Top_Spacing_XDirectionNumber_En).AsDouble() != 0)
          {
            row[_CmpParameters.Top_Spacing_XDirectionNumber_En] = System.Math.Round(famSymColumnType.LookupParameter(_CmpParameters.Top_Spacing_XDirectionNumber_En).AsDouble(), MidpointRounding.AwayFromZero);
          }

          // Bottom_Spacing_XDirectionNumber
          row[_CmpParameters.Bottom_Spacing_XDirectionNumber_En] = famSymColumnType.LookupParameter(_CmpParameters.Bottom_Spacing_XDirectionNumber_En).AsInteger();
          if ((int)row[_CmpParameters.Bottom_Spacing_XDirectionNumber_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.Bottom_Spacing_XDirectionNumber_En).AsDouble() != 0)
          {
            row[_CmpParameters.Bottom_Spacing_XDirectionNumber_En] = System.Math.Round(famSymColumnType.LookupParameter(_CmpParameters.Bottom_Spacing_XDirectionNumber_En).AsDouble(), MidpointRounding.AwayFromZero);
          }

          // Top_Spacing_YDirectionNumber
          row[_CmpParameters.Top_Spacing_YDirectionNumber_En] = famSymColumnType.LookupParameter(_CmpParameters.Top_Spacing_YDirectionNumber_En).AsInteger();
          if ((int)row[_CmpParameters.Top_Spacing_YDirectionNumber_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.Top_Spacing_YDirectionNumber_En).AsDouble() != 0)
          {
            row[_CmpParameters.Top_Spacing_YDirectionNumber_En] = System.Math.Round(famSymColumnType.LookupParameter(_CmpParameters.Top_Spacing_YDirectionNumber_En).AsDouble(), MidpointRounding.AwayFromZero);
          }

          // Bottom_Spacing_YDirectionNumber
          row[_CmpParameters.Bottom_Spacing_YDirectionNumber_En] = famSymColumnType.LookupParameter(_CmpParameters.Bottom_Spacing_YDirectionNumber_En).AsInteger();
          if ((int)row[_CmpParameters.Bottom_Spacing_YDirectionNumber_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.Bottom_Spacing_YDirectionNumber_En).AsDouble() != 0)
          {
            row[_CmpParameters.Bottom_Spacing_YDirectionNumber_En] = System.Math.Round(famSymColumnType.LookupParameter(_CmpParameters.Bottom_Spacing_YDirectionNumber_En).AsDouble(), MidpointRounding.AwayFromZero);
          }

          // 幅止筋ピッチ
          row[_CmpParameters.RST_HabadomekinPitch_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_HabadomekinPitch_En).AsDoubleMm();
          if ((double)row[_CmpParameters.RST_HabadomekinPitch_En] == 0 && famSymColumnType.LookupParameter(_CmpParameters.RST_HabadomekinPitch_En).AsInteger() != 0)
          {
            row[_CmpParameters.RST_HabadomekinPitch_En] = famSymColumnType.LookupParameter(_CmpParameters.RST_HabadomekinPitch_En).AsInteger();
          }
        }

        return row;
      }

      // ================================================================================
      /// <summary>データ取得</summary>
      /// 
      /// <param name="famInsColumn">柱</param>
      /// 
      /// <history>2013/04/19 Created GSA,Inc. Ryo Kurodai</history>
      /// ================================================================================
      public
      void GetData(Collections.Generic.IList<Revit.DB.FamilySymbol> columnAry)
      {
        if (_Data == null)
        {
          _Data = new System.Data.DataTable();
          DefDataFormat(ref _Data);
        }

        for (int i = 0; i < columnAry.Count; ++i)
        {
          System.Data.DataRow row = GetData(columnAry[i]);
          if (row != null)
          {
            _Data.Rows.Add(row);
          }
        }
      }

    #endregion

    // プロパティ
    #region Properties

      /// ================================================================================
      /// <summary>データ</summary>
      /// <history>2013/04/19 Created GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public System.Data.DataTable Data
      {
        get
        {
          return _Data;
        }
      }

    #endregion
  }
}
