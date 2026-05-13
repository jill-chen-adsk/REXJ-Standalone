using System;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
using SectionListRC.Utils;

namespace SectionListRC.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - 柱</summary>
  /// ================================================================================
  public class DtKakuColumn : SectionListRC.Entities.DtBase
  {
    // メンバ変数
    #region Member Variables

      private SectionListRC.Components.Parameters _CmpParameters;

      private SectionListRC.Components.Elements _CmpElements;

      private SectionListRC.Entities.SpKakuColumn _EntSpKakuColumn;

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
      public DtKakuColumn(SectionListRC.Components.Attribute cmpAttribute,
                          SectionListRC.Components.Elements cmpElements,
                          SectionListRC.Components.Geometry cmpGeometry,
                          SectionListRC.Components.Parameters cmpParameters,
                          SectionListRC.Components.Settings cmpSettings) :
             base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
      {
        _CmpParameters    = cmpParameters;
        _CmpElements      = cmpElements;
        _EntSpKakuColumn  = new SectionListRC.Entities.SpKakuColumn(cmpAttribute, cmpParameters, cmpSettings);

        if (_EntSpKakuColumn.DefSuccess == false)
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
      /// <history>2013/04/15 Created GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      private
      void DefDataFormat(ref System.Data.DataTable data)
      {
        // 柱符号
        data.Columns.Add(_CmpParameters.RST_HasiraHugo_Kaku, typeof(string));

        // レベル
        data.Columns.Add(_CmpParameters.LevelFrameTitle, typeof(string));

        // 柱分類
        data.Columns.Add(_CmpParameters.HashiraBunrui_Kaku, typeof(string));

        // 柱幅
        data.Columns.Add(_CmpParameters.DX_Kaku, typeof(double));

        // 柱成
        data.Columns.Add(_CmpParameters.DY_Kaku, typeof(double));

        // 柱頭主筋太径
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinHutokei_Kaku, typeof(string));

        // 柱脚主筋太径
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinHutokei_Kaku, typeof(string));

        // 柱頭主筋X1段太径本数
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku, typeof(int));

        // 柱脚主筋X1段太径本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku, typeof(int));

        // 柱頭主筋X2段太径本数
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku, typeof(int));

        // 柱脚主筋X2段太径本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku, typeof(int));

        // 柱頭主筋Y1段太径本数
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku, typeof(int));

        // 柱脚主筋Y1段太径本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku, typeof(int));

        // 柱頭主筋Y2段太径本数
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku, typeof(int));

        // 柱脚主筋Y2段太径本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku, typeof(int));

        // 柱頭主筋細径
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinHosokei_Kaku, typeof(string));

        // 柱脚主筋細径
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinHosokei_Kaku, typeof(string));

        // 柱頭主筋X1段細径本数
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku, typeof(int));

        // 柱脚主筋X1段細径本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku, typeof(int));

        // 柱頭主筋X2段細径本数
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku, typeof(int));

        // 柱脚主筋X2段細径本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku, typeof(int));

        // 柱頭主筋Y1段細径本数
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku, typeof(int));

        // 柱脚主筋Y1段細径本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku, typeof(int));

        // 柱頭主筋Y2段細径本数
        data.Columns.Add(_CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku, typeof(int));

        // 柱脚主筋Y2段細径本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku, typeof(int));

        // 芯鉄筋径
        data.Columns.Add(_CmpParameters.RST_SintekkinKei_Kaku, typeof(string));

        // 芯鉄筋本数
        data.Columns.Add(_CmpParameters.CoreRebar_Number_Kaku, typeof(int));

        // 芯鉄筋位置X
        data.Columns.Add(_CmpParameters.RST_SintekkinIchiX_Kaku, typeof(double));

        // 芯鉄筋位置Y
        data.Columns.Add(_CmpParameters.RST_SintekkinIchiY_Kaku, typeof(double));

        // 柱頭フープX径
        data.Columns.Add(_CmpParameters.RST_ChutoHoopXKei_Kaku, typeof(string));

        // 柱脚フープX径
        data.Columns.Add(_CmpParameters.RST_ChukyakuHoopXKei_Kaku, typeof(string));

        // 柱頭フープX本数
        data.Columns.Add(_CmpParameters.RST_ChutoHoopXHonsu_Kaku, typeof(int));

        // 柱脚フープX本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuHoopXHonsu_Kaku, typeof(int));

        // 柱頭フープY本数
        data.Columns.Add(_CmpParameters.RST_ChutoHoopYHonsu_Kaku, typeof(int));

        // 柱脚フープY本数
        data.Columns.Add(_CmpParameters.RST_ChukyakuHoopYHonsu_Kaku, typeof(int));

        // 柱頭フープピッチ
        data.Columns.Add(_CmpParameters.RST_ChutoHoopPitch_Kaku, typeof(double));

        // 柱脚フープピッチ
        data.Columns.Add(_CmpParameters.RST_ChukyakuHoopPitch_Kaku, typeof(double));

        // 幅止筋径
        data.Columns.Add(_CmpParameters.RST_HabadomekinKei_Kaku, typeof(string));

        // Top_Spacing_XDirecyionNumber
        data.Columns.Add(_CmpParameters.Top_Spacing_XDirectionNumber_Kaku, typeof(int));

        // Bottom_Spacing_XDirecyionNumber
        data.Columns.Add(_CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku, typeof(int));

        // Top_Spacing_YDirecyionNumber
        data.Columns.Add(_CmpParameters.Top_Spacing_YDirectionNumber_Kaku, typeof(int));

        // Bottom_Spacing_YDirecyionNumber
        data.Columns.Add(_CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku, typeof(int));

        // 幅止筋ピッチ
        data.Columns.Add(_CmpParameters.RST_HabadomekinPitch_Kaku, typeof(double));
      }

      /// ================================================================================
      /// <summary>データ取得</summary>
      /// 
      /// <param name="famSymColumn">柱</param>
      /// 
      /// <history>2013/04/15 Created GSA,Inc. Ryo Kuroda</history>
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
          _EntSpKakuColumn.CurrentElem = famSymColumn;

          row = _Data.NewRow();

          string typeName = "";
          string levelName = "0";
          _CmpElements.GetTypeMarkLevel(famSymColumn, ref typeName, ref levelName, _CmpParameters.RST_HasiraHugo_Kaku);

          if (typeName == "")
          {
            return null;
          }

          //柱符号
          row[_CmpParameters.RST_HasiraHugo_Kaku] = typeName;

          // 階
          row[_CmpParameters.LevelFrameTitle] = levelName;

          // 柱分類
          row[_CmpParameters.HashiraBunrui_Kaku] = famSymColumn.LookupParameter(_CmpParameters.HashiraBunrui_Kaku).AsString();

          // 柱幅
          row[_CmpParameters.DX_Kaku] = famSymColumn.LookupParameter(_CmpParameters.DX_Kaku).AsDouble();
          if ((double)row[_CmpParameters.DX_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.DX_Kaku).AsInteger() != 0)
          {
            row[_CmpParameters.DX_Kaku] = famSymColumn.LookupParameter(_CmpParameters.DX_Kaku).AsInteger();
          }

          // 柱成
          row[_CmpParameters.DY_Kaku] = famSymColumn.LookupParameter(_CmpParameters.DY_Kaku).AsDouble();
          if ((double)row[_CmpParameters.DY_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.DY_Kaku).AsInteger() != 0)
          {
            row[_CmpParameters.DY_Kaku] = famSymColumn.LookupParameter(_CmpParameters.DY_Kaku).AsInteger();
          }

          // 柱頭主筋太径
          row[_CmpParameters.RST_ChutoSyukinHutokei_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinHutokei_Kaku).AsString();

          // 柱脚主筋太径
          row[_CmpParameters.RST_ChukyakuSyukinHutokei_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinHutokei_Kaku).AsString();

          // 柱頭主筋X1段太径本数
          row[_CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku).AsDouble(), MidpointRounding.AwayFromZero);
          }

          // 柱脚主筋X1段太径本数
          row[_CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱頭主筋X2段太径本数
          row[_CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱脚主筋X2段太径本数
          row[_CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱頭主筋Y1段太径本数
          row[_CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱脚主筋Y1段太径本数
          row[_CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱頭主筋Y2段太径本数
          row[_CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱脚主筋Y2段太径本数
          row[_CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱頭主筋細径
          row[_CmpParameters.RST_ChutoSyukinHosokei_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinHosokei_Kaku).AsString();

          // 柱脚主筋細径
          row[_CmpParameters.RST_ChukyakuSyukinHosokei_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinHosokei_Kaku).AsString();

          // 柱頭主筋X1段細径本数
          row[_CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱脚主筋X1段細径本数
          row[_CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱頭主筋X2段細径本数
          row[_CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱脚主筋X2段細径本数
          row[_CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱頭主筋Y1段細径本数
          row[_CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱脚主筋Y1段細径本数
          row[_CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱頭主筋Y2段細径本数
          row[_CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱脚主筋Y2段細径本数
          row[_CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 芯鉄筋径
          row[_CmpParameters.RST_SintekkinKei_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_SintekkinKei_Kaku).AsString();

          // 芯鉄筋本数
          row[_CmpParameters.CoreRebar_Number_Kaku] = famSymColumn.LookupParameter(_CmpParameters.CoreRebar_Number_Kaku).AsInteger();
          if ((int)row[_CmpParameters.CoreRebar_Number_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.CoreRebar_Number_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.CoreRebar_Number_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.CoreRebar_Number_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 芯鉄筋位置X
          row[_CmpParameters.RST_SintekkinIchiX_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_SintekkinIchiX_Kaku).AsDouble();
          if ((double)row[_CmpParameters.RST_SintekkinIchiX_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_SintekkinIchiX_Kaku).AsInteger() != 0)
          {
            row[_CmpParameters.RST_SintekkinIchiX_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_SintekkinIchiX_Kaku).AsInteger();
          }

          // 芯鉄筋位置Y
          row[_CmpParameters.RST_SintekkinIchiY_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_SintekkinIchiY_Kaku).AsDouble();
          if ((double)row[_CmpParameters.RST_SintekkinIchiY_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_SintekkinIchiY_Kaku).AsInteger() != 0)
          {
            row[_CmpParameters.RST_SintekkinIchiY_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_SintekkinIchiY_Kaku).AsInteger();
          }

          // 柱頭フープX径
          row[_CmpParameters.RST_ChutoHoopXKei_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopXKei_Kaku).AsString();

          // 柱脚フープX径
          row[_CmpParameters.RST_ChukyakuHoopXKei_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopXKei_Kaku).AsString();

          // 柱頭フープX本数
          row[_CmpParameters.RST_ChutoHoopXHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopXHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoHoopXHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopXHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoHoopXHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopXHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱脚フープX本数
          row[_CmpParameters.RST_ChukyakuHoopXHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopXHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuHoopXHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopXHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuHoopXHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopXHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱頭フープY本数
          row[_CmpParameters.RST_ChutoHoopYHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopYHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChutoHoopYHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopYHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChutoHoopYHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopYHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱脚フープY本数
          row[_CmpParameters.RST_ChukyakuHoopYHonsu_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopYHonsu_Kaku).AsInteger();
          if ((int)row[_CmpParameters.RST_ChukyakuHoopYHonsu_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopYHonsu_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.RST_ChukyakuHoopYHonsu_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopYHonsu_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 柱頭フープピッチ
          row[_CmpParameters.RST_ChutoHoopPitch_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopPitch_Kaku).AsDoubleMm();
          if ((double)row[_CmpParameters.RST_ChutoHoopPitch_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopPitch_Kaku).AsInteger() != 0)
          {
            row[_CmpParameters.RST_ChutoHoopPitch_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChutoHoopPitch_Kaku).AsInteger();
          }

          // 柱脚フープピッチ
          row[_CmpParameters.RST_ChukyakuHoopPitch_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopPitch_Kaku).AsDoubleMm();
          if ((double)row[_CmpParameters.RST_ChukyakuHoopPitch_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopPitch_Kaku).AsInteger() != 0)
          {
            row[_CmpParameters.RST_ChukyakuHoopPitch_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_ChukyakuHoopPitch_Kaku).AsInteger();
          }

          // 幅止筋径
          row[_CmpParameters.RST_HabadomekinKei_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_HabadomekinKei_Kaku).AsString();

          // Top_Spacing_XDirectionNumber
          row[_CmpParameters.Top_Spacing_XDirectionNumber_Kaku] = famSymColumn.LookupParameter(_CmpParameters.Top_Spacing_XDirectionNumber_Kaku).AsInteger();
          if ((int)row[_CmpParameters.Top_Spacing_XDirectionNumber_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.Top_Spacing_XDirectionNumber_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.Top_Spacing_XDirectionNumber_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.Top_Spacing_XDirectionNumber_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // Bottom_Spacing_XDirectionNumber
          row[_CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku] = famSymColumn.LookupParameter(_CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku).AsInteger();
          if ((int)row[_CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.Bottom_Spacing_XDirectionNumber_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // Top_Spacing_YDirectionNumber
          row[_CmpParameters.Top_Spacing_YDirectionNumber_Kaku] = famSymColumn.LookupParameter(_CmpParameters.Top_Spacing_YDirectionNumber_Kaku).AsInteger();
          if ((int)row[_CmpParameters.Top_Spacing_YDirectionNumber_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.Top_Spacing_YDirectionNumber_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.Top_Spacing_YDirectionNumber_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.Top_Spacing_YDirectionNumber_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // Bottom_Spacing_XDirectionNumber
          row[_CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku] = famSymColumn.LookupParameter(_CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku).AsInteger();
          if ((int)row[_CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku).AsDouble() != 0)
          {
            row[_CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku] = System.Math.Round(famSymColumn.LookupParameter(_CmpParameters.Bottom_Spacing_YDirectionNumber_Kaku).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 幅止筋ピッチ
          row[_CmpParameters.RST_HabadomekinPitch_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_HabadomekinPitch_Kaku).AsDoubleMm();
          if ((double)row[_CmpParameters.RST_HabadomekinPitch_Kaku] == 0 && famSymColumn.LookupParameter(_CmpParameters.RST_HabadomekinPitch_Kaku).AsInteger() != 0)
          {
            row[_CmpParameters.RST_HabadomekinPitch_Kaku] = famSymColumn.LookupParameter(_CmpParameters.RST_HabadomekinPitch_Kaku).AsInteger();
          }
        }

        return row;
      }

      /// ================================================================================
      /// <summary>データ取得</summary>
      /// 
      /// <param name="famInsColumn">柱</param>
      /// 
      /// <history>2013/04/17 Created GSA,Inc. Ryo Kuroda</history>
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
      /// <history>2013/04/17 Created GSA,Inc. Ryo Kuroda</history>
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
