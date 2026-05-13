using System;
using SectionListRC.Utils ;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListRC.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - 梁</summary>
  /// ================================================================================
  public class DtBeam : SectionListRC.Entities.DtBase
  {
    // メンバ変数
    #region Member Variables

      private SectionListRC.Components.Parameters _CmpParameters;

      private SectionListRC.Components.Elements _CmpElements;

      private SectionListRC.Entities.SpBeam _EntSpBeam;

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
      /// <history>2013/04/25 Created  GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public DtBeam(SectionListRC.Components.Attribute cmpAttribute,
                    SectionListRC.Components.Elements cmpElements,
                    SectionListRC.Components.Geometry cmpGeometry,
                    SectionListRC.Components.Parameters cmpParameters,
                    SectionListRC.Components.Settings cmpSettings) :
             base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
      {
        _CmpParameters    = cmpParameters;
        _CmpElements      = cmpElements;
        _EntSpBeam  = new SectionListRC.Entities.SpBeam(cmpAttribute, cmpParameters, cmpSettings);

        if (_EntSpBeam.DefSuccess == false)
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
      /// <history>2013/04/25 Created GSA,Inc. Ryo Kurodai</history>
      /// ================================================================================
      private
      void DefDataFormat(ref System.Data.DataTable data)
      {
        // 梁符号
        data.Columns.Add(_CmpParameters.RST_HariHugo, typeof(string));

        // レベル
        data.Columns.Add(_CmpParameters.LevelFrameTitle, typeof(string));

        // 梁分類
        data.Columns.Add(_CmpParameters.Girder_Category, typeof(string));

        // i端梁幅
        data.Columns.Add(_CmpParameters.s_B, typeof(double));

        // 中央部梁幅
        data.Columns.Add(_CmpParameters.c_B, typeof(double));

        // j端梁幅
        data.Columns.Add(_CmpParameters.e_B, typeof(double));

        // i端梁成
        data.Columns.Add(_CmpParameters.s_D, typeof(double));

        // 中央部梁成
        data.Columns.Add(_CmpParameters.c_D, typeof(double));

        // j端梁成
        data.Columns.Add(_CmpParameters.e_D, typeof(double));

        // i端上主筋太径
        data.Columns.Add(_CmpParameters.RST_SyukinItanUeHutokei, typeof(string));

        // 中央上主筋太径
        data.Columns.Add(_CmpParameters.RST_SyukinChuohUeHutokei, typeof(string));

        // j端上主筋太径
        data.Columns.Add(_CmpParameters.RST_SyukinJtanUeHutokei, typeof(string));

        // i端上主筋1段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu, typeof(int));

        // 中央上主筋1段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu, typeof(int));

        // j端上主筋1段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu, typeof(int));

        // i端上主筋2段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu, typeof(int));

        // 中央上主筋2段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu, typeof(int));

        // j端上主筋2段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu, typeof(int));

        // i端上主筋3段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu, typeof(int));

        // 中央上主筋3段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu, typeof(int));

        // j端上主筋3段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu, typeof(int));

        // i端下主筋太径
        data.Columns.Add(_CmpParameters.RST_SyukinItanSitaHutokei, typeof(string));

        // 中央下主筋太径
        data.Columns.Add(_CmpParameters.RST_SyukinChuohSitaHutokei, typeof(string));

        // j端下主筋太径
        data.Columns.Add(_CmpParameters.RST_SyukinJtanSitaHutokei, typeof(string));

        // i端下主筋1段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu, typeof(int));

        // 中央下主筋1段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu, typeof(int));

        // j端下主筋1段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu, typeof(int));

        // i端下主筋2段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu, typeof(int));

        // 中央下主筋2段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu, typeof(int));

        // j端下主筋2段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu, typeof(int));

        // i端下主筋3段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu, typeof(int));

        // 中央下主筋3段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu, typeof(int));

        // j端下主筋3段太筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu, typeof(int));

        // i端上主筋細径
        data.Columns.Add(_CmpParameters.RST_SyukinItanUeHosokei, typeof(string));

        // 中央上主筋細径
        data.Columns.Add(_CmpParameters.RST_SyukinChuohUeHosokei, typeof(string));

        // j端上主筋細径
        data.Columns.Add(_CmpParameters.RST_SyukinJtanUeHosokei, typeof(string));

        // i端上主筋1段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu, typeof(int));

        // 中央上主筋1段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu, typeof(int));

        // j端上主筋1段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu, typeof(int));

        // i端上主筋2段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu, typeof(int));

        // 中央上主筋2段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu, typeof(int));

        // j端上主筋2段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu, typeof(int));

        // i端上主筋3段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu, typeof(int));

        // 中央上主筋3段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu, typeof(int));

        // j端上主筋3段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu, typeof(int));

        // i端下主筋細径
        data.Columns.Add(_CmpParameters.RST_SyukinItanSitaHosokei, typeof(string));

        // 中央下主筋細径
        data.Columns.Add(_CmpParameters.RST_SyukinChuohSitaHosokei, typeof(string));

        // j端下主筋細径
        data.Columns.Add(_CmpParameters.RST_SyukinJtanSitaHosokei, typeof(string));

        // i端下主筋1段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu, typeof(int));

        // 中央下主筋1段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu, typeof(int));

        // j端下主筋1段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu, typeof(int));

        // i端下主筋2段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu, typeof(int));

        // 中央下主筋2段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu, typeof(int));

        // j端下主筋2段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu, typeof(int));

        // i端下主筋3段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu, typeof(int));

        // 中央下主筋3段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu, typeof(int));

        // j端下主筋3段細筋本数
        data.Columns.Add(_CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu, typeof(int));

        // i端肋筋径
        data.Columns.Add(_CmpParameters.s_Stirrup_Diameter, typeof(string));

        // 中央部肋筋径
        data.Columns.Add(_CmpParameters.c_Stirrup_Diameter, typeof(string));

        // j端肋筋径
        data.Columns.Add(_CmpParameters.e_Stirrup_Diameter, typeof(string));

        // i端肋筋本数
        data.Columns.Add(_CmpParameters.s_Stirrup_Number, typeof(int));

        // 中央部肋筋本数
        data.Columns.Add(_CmpParameters.c_Stirrup_Number, typeof(int));

        // j端肋筋本数
        data.Columns.Add(_CmpParameters.e_Stirrup_Number, typeof(int));

        // i端肋筋ピッチ
        data.Columns.Add(_CmpParameters.s_Stirrup_Pitch, typeof(double));

        // 中央部肋筋ピッチ
        data.Columns.Add(_CmpParameters.c_Stirrup_Pitch, typeof(double));

        // j端肋筋ピッチ
        data.Columns.Add(_CmpParameters.e_Stirrup_Pitch, typeof(double));

        // i端腹筋径
        data.Columns.Add(_CmpParameters.s_Web_Diameter, typeof(string));

        // 中央部腹筋径
        data.Columns.Add(_CmpParameters.c_Web_Diameter, typeof(string));

        // j端腹筋径
        data.Columns.Add(_CmpParameters.e_Web_Diameter, typeof(string));

        // i端腹筋本数
        data.Columns.Add(_CmpParameters.s_Web_Number, typeof(int));

        // 中央部腹筋本数
        data.Columns.Add(_CmpParameters.c_Web_Number, typeof(int));

        // j端腹筋本数
        data.Columns.Add(_CmpParameters.e_Web_Number, typeof(int));

        // i端幅止筋径
        data.Columns.Add(_CmpParameters.s_Spacing_Diameter, typeof(string));

        // 中央部幅止筋径
        data.Columns.Add(_CmpParameters.c_Spacing_Diameter, typeof(string));

        // j端幅止筋径
        data.Columns.Add(_CmpParameters.e_Spacing_Diameter, typeof(string));


        // i端幅止筋本数
        data.Columns.Add(_CmpParameters.s_Spacing_Number, typeof(int));

        // 中央部幅止筋本数
        data.Columns.Add(_CmpParameters.c_Spacing_Number, typeof(int));

        // j端幅止筋本数
        data.Columns.Add(_CmpParameters.e_Spacing_Number, typeof(int));

        // i端幅止筋ピッチ
        data.Columns.Add(_CmpParameters.s_Spacing_Pitch, typeof(double));

        // 中央部幅止筋ピッチ
        data.Columns.Add(_CmpParameters.c_Spacing_Pitch, typeof(double));

        // j端幅止筋ピッチ
        data.Columns.Add(_CmpParameters.e_Spacing_Pitch, typeof(double));
      }

      /// ================================================================================
      /// <summary>データ取得</summary>
      /// 
      /// <param name="famInsColumn">梁</param>
      /// 
      /// <history>2013/04/25 Created GSA,Inc. Ryo Kurodai</history>
      /// ================================================================================
      public
      System.Data.DataRow GetData(Revit.DB.FamilySymbol famInsBeam)
      {
        // 初期化
        System.Data.DataRow row = null;

        if (_Data == null)
        {
          _Data = new System.Data.DataTable();
          DefDataFormat(ref _Data);
        }

        if (famInsBeam != null)
        {
          Revit.DB.FamilySymbol famSymBeamType = famInsBeam;
          if (famSymBeamType == null)
          {
            return row;
          }
          
          _EntSpBeam.CurrentElem = famSymBeamType;

          row = _Data.NewRow();

          string typeName = "";
          string levelName = "";
          _CmpElements.GetTypeMarkLevel(famInsBeam, ref typeName, ref levelName, _CmpParameters.RST_HariHugo);

          // 梁符号
          row[_CmpParameters.RST_HariHugo] = typeName;

          // 階
          row[_CmpParameters.LevelFrameTitle] = levelName;

          // 梁分類
          row[_CmpParameters.Girder_Category] = famSymBeamType.LookupParameter(_CmpParameters.Girder_Category).AsString();

          // i端梁幅
          row[_CmpParameters.s_B] = famSymBeamType.LookupParameter(_CmpParameters.s_B).AsDouble();
          if ((double)row[_CmpParameters.s_B] == 0 && famSymBeamType.LookupParameter(_CmpParameters.s_B).AsInteger() != 0)
          {
            row[_CmpParameters.s_B] = famSymBeamType.LookupParameter(_CmpParameters.s_B).AsInteger();
          }

          // 中央部梁幅
          row[_CmpParameters.c_B] = famSymBeamType.LookupParameter(_CmpParameters.c_B).AsDouble();
          if ((double)row[_CmpParameters.c_B] == 0 && famSymBeamType.LookupParameter(_CmpParameters.c_B).AsInteger() != 0)
          {
            row[_CmpParameters.c_B] = famSymBeamType.LookupParameter(_CmpParameters.c_B).AsInteger();
          }

          // j端梁幅
          row[_CmpParameters.e_B] = famSymBeamType.LookupParameter(_CmpParameters.e_B).AsDouble();
          if ((double)row[_CmpParameters.e_B] == 0 && famSymBeamType.LookupParameter(_CmpParameters.e_B).AsInteger() != 0)
          {
            row[_CmpParameters.e_B] = famSymBeamType.LookupParameter(_CmpParameters.e_B).AsInteger();
          }

          // i端梁成
          row[_CmpParameters.s_D] = famSymBeamType.LookupParameter(_CmpParameters.s_D).AsDouble();
          if ((double)row[_CmpParameters.s_D] == 0 && famSymBeamType.LookupParameter(_CmpParameters.s_D).AsInteger() != 0)
          {
            row[_CmpParameters.s_D] = famSymBeamType.LookupParameter(_CmpParameters.s_D).AsInteger();
          }

          // 中央部梁成
          row[_CmpParameters.c_D] = famSymBeamType.LookupParameter(_CmpParameters.c_D).AsDouble();
          if ((double)row[_CmpParameters.c_D] == 0 && famSymBeamType.LookupParameter(_CmpParameters.c_D).AsInteger() != 0)
          {
            row[_CmpParameters.c_D] = famSymBeamType.LookupParameter(_CmpParameters.c_D).AsInteger();
          }

          // j端梁成
          row[_CmpParameters.e_D] = famSymBeamType.LookupParameter(_CmpParameters.e_D).AsDouble();
          if ((double)row[_CmpParameters.e_D] == 0 && famSymBeamType.LookupParameter(_CmpParameters.e_D).AsInteger() != 0)
          {
            row[_CmpParameters.e_D] = famSymBeamType.LookupParameter(_CmpParameters.e_D).AsInteger();
          }

          // i端上主筋太径
          row[_CmpParameters.RST_SyukinItanUeHutokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUeHutokei).AsString();

          // 中央上主筋太径
          row[_CmpParameters.RST_SyukinChuohUeHutokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUeHutokei).AsString();

          // j端上主筋太径
          row[_CmpParameters.RST_SyukinJtanUeHutokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUeHutokei).AsString();

          // i端上主筋1段太筋本数
          row[_CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央上主筋1段太筋本数
          row[_CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端上主筋1段太筋本数
          row[_CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端上主筋2段太筋本数
          row[_CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央上主筋2段太筋本数
          row[_CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端上主筋2段太筋本数
          row[_CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端上主筋3段太筋本数
          row[_CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央上主筋3段太筋本数
          row[_CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端上主筋3段太筋本数
          row[_CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端下主筋太径
          row[_CmpParameters.RST_SyukinItanSitaHutokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSitaHutokei).AsString();

          // 中央下主筋太径
          row[_CmpParameters.RST_SyukinChuohSitaHutokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSitaHutokei).AsString();

          // j端下主筋太径
          row[_CmpParameters.RST_SyukinJtanSitaHutokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSitaHutokei).AsString();

          // i端下主筋1段太筋本数
          row[_CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央下主筋1段太筋本数
          row[_CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端下主筋1段太筋本数
          row[_CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端下主筋2段太筋本数
          row[_CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央下主筋2段太筋本数
          row[_CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端下主筋2段太筋本数
          row[_CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端下主筋3段太筋本数
          row[_CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央下主筋3段太筋本数
          row[_CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端下主筋3段太筋本数
          row[_CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端上主筋細径
          row[_CmpParameters.RST_SyukinItanUeHosokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUeHosokei).AsString();

          // 中央上主筋細径
          row[_CmpParameters.RST_SyukinChuohUeHosokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUeHosokei).AsString();

          // j端上主筋細径
          row[_CmpParameters.RST_SyukinJtanUeHosokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUeHosokei).AsString();

          // i端上主筋1段細筋本数
          row[_CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央上主筋1段細筋本数
          row[_CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端上主筋1段細筋本数
          row[_CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端上主筋2段細筋本数
          row[_CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央上主筋2段細筋本数
          row[_CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端上主筋2段細筋本数
          row[_CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端上主筋3段細筋本数
          row[_CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央上主筋3段細筋本数
          row[_CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端上主筋3段細筋本数
          row[_CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端下主筋細径
          row[_CmpParameters.RST_SyukinItanSitaHosokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSitaHosokei).AsString();

          // 中央下主筋細径
          row[_CmpParameters.RST_SyukinChuohSitaHosokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSitaHosokei).AsString();

          // j端下主筋細径
          row[_CmpParameters.RST_SyukinJtanSitaHosokei] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSitaHosokei).AsString();

          // i端下主筋1段細筋本数
          row[_CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央下主筋1段細筋本数
          row[_CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端下主筋1段細筋本数
          row[_CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端下主筋2段細筋本数
          row[_CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央下主筋2段細筋本数
          row[_CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端下主筋2段細筋本数
          row[_CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端下主筋3段細筋本数
          row[_CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央下主筋3段細筋本数
          row[_CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端下主筋3段細筋本数
          row[_CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu] = famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu).AsInteger();
          if ((int)row[_CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端肋筋径
          row[_CmpParameters.s_Stirrup_Diameter] = famSymBeamType.LookupParameter(_CmpParameters.s_Stirrup_Diameter).AsString();

          // 中央部肋筋径
          row[_CmpParameters.c_Stirrup_Diameter] = famSymBeamType.LookupParameter(_CmpParameters.c_Stirrup_Diameter).AsString();

          // j端肋筋径
          row[_CmpParameters.e_Stirrup_Diameter] = famSymBeamType.LookupParameter(_CmpParameters.e_Stirrup_Diameter).AsString();

          // i端肋筋本数
          row[_CmpParameters.s_Stirrup_Number] = famSymBeamType.LookupParameter(_CmpParameters.s_Stirrup_Number).AsInteger();
          if ((int)row[_CmpParameters.s_Stirrup_Number] == 0 && famSymBeamType.LookupParameter(_CmpParameters.s_Stirrup_Number).AsDouble() != 0)
          {
            row[_CmpParameters.s_Stirrup_Number] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.s_Stirrup_Number).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央部肋筋本数
          row[_CmpParameters.c_Stirrup_Number] = famSymBeamType.LookupParameter(_CmpParameters.c_Stirrup_Number).AsInteger();
          if ((int)row[_CmpParameters.c_Stirrup_Number] == 0 && famSymBeamType.LookupParameter(_CmpParameters.c_Stirrup_Number).AsDouble() != 0)
          {
            row[_CmpParameters.c_Stirrup_Number] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.c_Stirrup_Number).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端肋筋本数
          row[_CmpParameters.e_Stirrup_Number] = famSymBeamType.LookupParameter(_CmpParameters.e_Stirrup_Number).AsInteger();
          if ((int)row[_CmpParameters.e_Stirrup_Number] == 0 && famSymBeamType.LookupParameter(_CmpParameters.e_Stirrup_Number).AsDouble() != 0)
          {
            row[_CmpParameters.e_Stirrup_Number] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.e_Stirrup_Number).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端肋筋ピッチ
          row[_CmpParameters.s_Stirrup_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.s_Stirrup_Pitch).AsDoubleMm();
          if ((double)row[_CmpParameters.s_Stirrup_Pitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.s_Stirrup_Pitch).AsInteger() != 0)
          {
            row[_CmpParameters.s_Stirrup_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.s_Stirrup_Pitch).AsInteger();
          }

          // 中央部肋筋ピッチ
          row[_CmpParameters.c_Stirrup_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.c_Stirrup_Pitch).AsDoubleMm();
          if ((double)row[_CmpParameters.c_Stirrup_Pitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.c_Stirrup_Pitch).AsInteger() != 0)
          {
            row[_CmpParameters.c_Stirrup_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.c_Stirrup_Pitch).AsInteger();
          }

          // j端肋筋ピッチ
          row[_CmpParameters.e_Stirrup_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.e_Stirrup_Pitch).AsDoubleMm();
          if ((double)row[_CmpParameters.e_Stirrup_Pitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.e_Stirrup_Pitch).AsInteger() != 0)
          {
            row[_CmpParameters.e_Stirrup_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.e_Stirrup_Pitch).AsInteger();
          }

          // i端腹筋径
          row[_CmpParameters.s_Web_Diameter] = famSymBeamType.LookupParameter(_CmpParameters.s_Web_Diameter).AsString();

          // 中央部腹筋径
          row[_CmpParameters.c_Web_Diameter] = famSymBeamType.LookupParameter(_CmpParameters.c_Web_Diameter).AsString();

          // j端腹筋径
          row[_CmpParameters.e_Web_Diameter] = famSymBeamType.LookupParameter(_CmpParameters.e_Web_Diameter).AsString();

          // i端腹筋本数
          row[_CmpParameters.s_Web_Number] = famSymBeamType.LookupParameter(_CmpParameters.s_Web_Number).AsInteger();
          if ((int)row[_CmpParameters.s_Web_Number] == 0 && famSymBeamType.LookupParameter(_CmpParameters.s_Web_Number).AsDouble() != 0)
          {
            row[_CmpParameters.s_Web_Number] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.s_Web_Number).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央部腹筋本数
          row[_CmpParameters.c_Web_Number] = famSymBeamType.LookupParameter(_CmpParameters.c_Web_Number).AsInteger();
          if ((int)row[_CmpParameters.c_Web_Number] == 0 && famSymBeamType.LookupParameter(_CmpParameters.c_Web_Number).AsDouble() != 0)
          {
            row[_CmpParameters.c_Web_Number] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.c_Web_Number).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端腹筋本数
          row[_CmpParameters.e_Web_Number] = famSymBeamType.LookupParameter(_CmpParameters.e_Web_Number).AsInteger();
          if ((int)row[_CmpParameters.e_Web_Number] == 0 && famSymBeamType.LookupParameter(_CmpParameters.e_Web_Number).AsDouble() != 0)
          {
            row[_CmpParameters.e_Web_Number] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.e_Web_Number).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端幅止筋径
          row[_CmpParameters.s_Spacing_Diameter] = famSymBeamType.LookupParameter(_CmpParameters.s_Spacing_Diameter).AsString();

          // 中央部幅止筋径
          row[_CmpParameters.c_Spacing_Diameter] = famSymBeamType.LookupParameter(_CmpParameters.c_Spacing_Diameter).AsString();

          // j端幅止筋径
          row[_CmpParameters.e_Spacing_Diameter] = famSymBeamType.LookupParameter(_CmpParameters.e_Spacing_Diameter).AsString();

          // i端幅止筋本数
          row[_CmpParameters.s_Spacing_Number] = famSymBeamType.LookupParameter(_CmpParameters.s_Spacing_Number).AsInteger();
          if ((int)row[_CmpParameters.s_Spacing_Number] == 0 && famSymBeamType.LookupParameter(_CmpParameters.s_Spacing_Number).AsDouble() != 0)
          {
            row[_CmpParameters.s_Spacing_Number] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.s_Spacing_Number).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 中央部幅止筋本数
          row[_CmpParameters.c_Spacing_Number] = famSymBeamType.LookupParameter(_CmpParameters.c_Spacing_Number).AsInteger();
          if ((int)row[_CmpParameters.c_Spacing_Number] == 0 && famSymBeamType.LookupParameter(_CmpParameters.c_Spacing_Number).AsDouble() != 0)
          {
            row[_CmpParameters.c_Spacing_Number] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.c_Spacing_Number).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // j端幅止筋本数
          row[_CmpParameters.e_Spacing_Number] = famSymBeamType.LookupParameter(_CmpParameters.e_Spacing_Number).AsInteger();
          if ((int)row[_CmpParameters.e_Spacing_Number] == 0 && famSymBeamType.LookupParameter(_CmpParameters.e_Spacing_Number).AsDouble() != 0)
          {
            row[_CmpParameters.e_Spacing_Number] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.e_Spacing_Number).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // i端幅止筋ピッチ
          if (famSymBeamType.LookupParameter(_CmpParameters.s_Spacing_Pitch) != null)
          {
            row[_CmpParameters.s_Spacing_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.s_Spacing_Pitch).AsDoubleMm();
            if ((double)row[_CmpParameters.s_Spacing_Pitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.s_Spacing_Pitch).AsInteger() != 0)
            {
              row[_CmpParameters.s_Spacing_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.s_Spacing_Pitch).AsInteger();
            }
          }

          // 中央部幅止筋ピッチ
          if (famSymBeamType.LookupParameter(_CmpParameters.c_Spacing_Pitch) != null)
          {
            row[_CmpParameters.c_Spacing_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.c_Spacing_Pitch).AsDoubleMm();
            if ((double)row[_CmpParameters.c_Spacing_Pitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.c_Spacing_Pitch).AsInteger() != 0)
            {
              row[_CmpParameters.c_Spacing_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.c_Spacing_Pitch).AsInteger();
            }
          }

          // j端幅止筋ピッチ
          if (famSymBeamType.LookupParameter(_CmpParameters.e_Spacing_Pitch) != null)
          {
            row[_CmpParameters.e_Spacing_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.e_Spacing_Pitch).AsDoubleMm();
            if ((double)row[_CmpParameters.e_Spacing_Pitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.e_Spacing_Pitch).AsInteger() != 0)
            {
              row[_CmpParameters.e_Spacing_Pitch] = famSymBeamType.LookupParameter(_CmpParameters.e_Spacing_Pitch).AsInteger();
            }
          }
        }

        return row;
      }

      /// ================================================================================
      /// <summary>データ取得</summary>
      /// 
      /// <param name="famInsColumn">梁</param>
      /// 
      /// <history>2013/04/25 Created GSA,Inc. Ryo Kurodai</history>
      /// ================================================================================
      public
      void GetData(Collections.Generic.IList<Revit.DB.FamilySymbol> beamAry)
      {
        if (_Data == null)
        {
          _Data = new System.Data.DataTable();
          DefDataFormat(ref _Data);
        }

        for (int i = 0; i < beamAry.Count; ++i)
        {
          System.Data.DataRow row = GetData(beamAry[i]);
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
      /// <history>2013/04/25 Created GSA,Inc. Ryo Kuroda</history>
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
