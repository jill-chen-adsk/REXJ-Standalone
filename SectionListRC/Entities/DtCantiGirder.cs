using System;
using SectionListRC.Utils ;
using Collections = System.Collections;
using Revit       = Autodesk.Revit;
namespace SectionListRC.Entities
{
  /// ================================================================================
  /// <summary>データテーブル - 片持ち梁</summary>
  /// ================================================================================
  public class DtCantiGirder : SectionListRC.Entities.DtBase
  {
     // メンバ変数
    #region Member Variables

      private SectionListRC.Components.Parameters _CmpParameters;

      private SectionListRC.Components.Elements _CmpElements;

      private SectionListRC.Entities.SpCantiGirder _EntSpCanti;

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
      /// <history>2014/06/13 Created  GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public DtCantiGirder(SectionListRC.Components.Attribute cmpAttribute,
                           SectionListRC.Components.Elements cmpElements,
                           SectionListRC.Components.Geometry cmpGeometry,
                           SectionListRC.Components.Parameters cmpParameters,
                           SectionListRC.Components.Settings cmpSettings) :
             base(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings)
      {
        _CmpParameters    = cmpParameters;
        _CmpElements      = cmpElements;

        _EntSpCanti  = new SectionListRC.Entities.SpCantiGirder(cmpAttribute, cmpParameters, cmpSettings);

        if (_EntSpCanti.DefSuccess == false)
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
      /// <history>2014/06/13 Created GSA,Inc. Ryo Kurodai</history>
      /// ================================================================================
      private
      void DefDataFormat(ref System.Data.DataTable data)
      {
        // 梁符号
        data.Columns.Add(_CmpParameters.HariHugo_Katamoti, typeof(string));

        // レベル
        data.Columns.Add(_CmpParameters.LevelFrameTitle, typeof(string));

        // 梁種別
        data.Columns.Add(_CmpParameters.HariSyubetu_Katamoti, typeof(string));

        // 元端梁幅
        data.Columns.Add(_CmpParameters.MototanHarihaba, typeof(double));

        // 先端梁幅
        data.Columns.Add(_CmpParameters.SentanHarihaba, typeof(double));

        // 元端梁成
        data.Columns.Add(_CmpParameters.MototanHarisei, typeof(double));

        // 先端梁成
        data.Columns.Add(_CmpParameters.SentanHarisei, typeof(double));

        // 元端上主筋太径
        data.Columns.Add(_CmpParameters.MototanUeSyukinHutokei, typeof(string));

        // 先端上主筋太径
        data.Columns.Add(_CmpParameters.SentanUeSyukinHutokei, typeof(string));

        // 元端上主筋1段太筋本数
        data.Columns.Add(_CmpParameters.MototanUeSyukin1danHutokinHonsu, typeof(int));

        // 先端上主筋1段太筋本数
        data.Columns.Add(_CmpParameters.SentanUeSyukin1danHutokinHonsu, typeof(int));

        // 元端上主筋2段太筋本数
        data.Columns.Add(_CmpParameters.MototanUeSyukin2danHutokinHonsu, typeof(int));

        // 先端上主筋2段太筋本数
        data.Columns.Add(_CmpParameters.SentanUeSyukin2danHutokinHonsu, typeof(int));

        // 元端端上主筋3段太筋本数
        data.Columns.Add(_CmpParameters.MototanUeSyukin3danHutokinHonsu, typeof(int));

        // 先端上主筋3段太筋本数
        data.Columns.Add(_CmpParameters.SentanUeSyukin3danHutokinHonsu, typeof(int));

        // 元端下主筋太径
        data.Columns.Add(_CmpParameters.MototanSitaSyukinHutokei, typeof(string));

        // 先端下主筋太径
        data.Columns.Add(_CmpParameters.SentanSitaSyukinHutokei, typeof(string));

        // 元端下主筋1段太筋本数
        data.Columns.Add(_CmpParameters.MototanSitaSyukin1danHutokinHonsu, typeof(int));

        // 先端下主筋1段太筋本数
        data.Columns.Add(_CmpParameters.SentanSitaSyukin1danHutokinHonsu, typeof(int));

        // 元端下主筋2段太筋本数
        data.Columns.Add(_CmpParameters.MototanSitaSyukin2danHutokinHonsu, typeof(int));

        // 先端下主筋2段太筋本数
        data.Columns.Add(_CmpParameters.SentanSitaSyukin2danHutokinHonsu, typeof(int));

        // 元端下主筋3段太筋本数
        data.Columns.Add(_CmpParameters.MototanSitaSyukin3danHutokinHonsu, typeof(int));

        // 先端下主筋3段太筋本数
        data.Columns.Add(_CmpParameters.SentanSitaSyukin3danHutokinHonsu, typeof(int));

        // 元端上主筋細径
        data.Columns.Add(_CmpParameters.MototanUeSyukinHosokei, typeof(string));

        // 先端上主筋細径
        data.Columns.Add(_CmpParameters.SentanUeSyukinHosokei, typeof(string));

        // 元端上主筋1段細筋本数
        data.Columns.Add(_CmpParameters.MototanUeSyukin1danHosokinHonsu, typeof(int));

        // 先端上主筋1段細筋本数
        data.Columns.Add(_CmpParameters.SentanUeSyukin1danHosokinHonsu, typeof(int));

        // 元端上主筋2段細筋本数
        data.Columns.Add(_CmpParameters.MototanUeSyukin2danHosokinHonsu, typeof(int));

        // 先端上主筋2段細筋本数
        data.Columns.Add(_CmpParameters.SentanUeSyukin2danHosokinHonsu, typeof(int));

        // 元端上主筋3段細筋本数
        data.Columns.Add(_CmpParameters.MototanUeSyukin3danHosokinHonsu, typeof(int));

        // 先端上主筋3段細筋本数
        data.Columns.Add(_CmpParameters.SentanUeSyukin3danHosokinHonsu, typeof(int));

        // 元端下主筋細径
        data.Columns.Add(_CmpParameters.MototanSitaSyukinHosokei, typeof(string));

        // 先端下主筋細径
        data.Columns.Add(_CmpParameters.SentanSitaSyukinHosokei, typeof(string));

        // 元端下主筋1段細筋本数
        data.Columns.Add(_CmpParameters.MototanSitaSyukin1danHosokinHonsu, typeof(int));

        // 先端下主筋1段細筋本数
        data.Columns.Add(_CmpParameters.SentanSitaSyukin1danHosokinHonsu, typeof(int));

        // 元端下主筋2段細筋本数
        data.Columns.Add(_CmpParameters.MototanSitaSyukin2danHosokinHonsu, typeof(int));

        // 先端下主筋2段細筋本数
        data.Columns.Add(_CmpParameters.SentanSitaSyukin2danHosokinHonsu, typeof(int));

        // 元端下主筋3段細筋本数
        data.Columns.Add(_CmpParameters.MototanSitaSyukin3danHosokinHonsu, typeof(int));

        // 先端下主筋3段細筋本数
        data.Columns.Add(_CmpParameters.SentanSitaSyukin3danHosokinHonsu, typeof(int));

        // 元端肋筋径
        data.Columns.Add(_CmpParameters.MototanAbarakinkei, typeof(string));

        // 先端肋筋径
        data.Columns.Add(_CmpParameters.SentanAbarakinkei, typeof(string));

        // 元端肋筋本数
        data.Columns.Add(_CmpParameters.MototanAbarakinHonsu, typeof(int));

        // 先端肋筋本数
        data.Columns.Add(_CmpParameters.SentanAbarakinHonsu, typeof(int));

        // 元端肋筋ピッチ
        data.Columns.Add(_CmpParameters.MototanAbarakinPitch, typeof(double));

        // 先端肋筋ピッチ
        data.Columns.Add(_CmpParameters.SentanAbarakinPitch, typeof(double));

        // 元端腹筋径
        data.Columns.Add(_CmpParameters.MototanHarakinkei, typeof(string));

        // 先端腹筋径
        data.Columns.Add(_CmpParameters.SentanHarakinkei, typeof(string));

        // 元端腹筋本数
        data.Columns.Add(_CmpParameters.MototanHarakinHonsu, typeof(int));

        // 先端腹筋本数
        data.Columns.Add(_CmpParameters.SentanHarakinHonsu, typeof(int));

        // 元端幅止筋径
        data.Columns.Add(_CmpParameters.MototanHabadomekinkei, typeof(string));

        // 先端幅止筋径
        data.Columns.Add(_CmpParameters.SentanHabadomekinkei, typeof(string));


        // 元端幅止筋本数
        data.Columns.Add(_CmpParameters.MototanHabadomekinHonsu, typeof(int));

        // 先端幅止筋本数
        data.Columns.Add(_CmpParameters.SentanHabadomekinHonsu, typeof(int));

        // 元端幅止筋ピッチ
        data.Columns.Add(_CmpParameters.MototanHabadomekinPitch, typeof(double));

        // 先端幅止筋ピッチ
        data.Columns.Add(_CmpParameters.SentanHabadomekinPitch, typeof(double));
      }

      /// ================================================================================
      /// <summary>データ取得</summary>
      /// 
      /// <param name="famInsColumn">梁</param>
      /// 
      /// <history>2014/06/13 Created GSA,Inc. Ryo Kurodai</history>
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

          _EntSpCanti.CurrentElem = famSymBeamType;

          row = _Data.NewRow();

          string typeName = "";
          string levelName = "";
          _CmpElements.GetTypeMarkLevel(famInsBeam, ref typeName, ref levelName, _CmpParameters.HariHugo_Katamoti);

          // 梁符号
          row[_CmpParameters.HariHugo_Katamoti] = typeName;

          // 階
          row[_CmpParameters.LevelFrameTitle] = levelName;

          // 梁分類
          row[_CmpParameters.HariSyubetu_Katamoti] = famSymBeamType.LookupParameter(_CmpParameters.HariSyubetu_Katamoti).AsString();

          // 元端梁幅
          row[_CmpParameters.MototanHarihaba] = famSymBeamType.LookupParameter(_CmpParameters.MototanHarihaba).AsDouble();
          if ((double)row[_CmpParameters.MototanHarihaba] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanHarihaba).AsInteger() != 0)
          {
            row[_CmpParameters.MototanHarihaba] = famSymBeamType.LookupParameter(_CmpParameters.s_B).AsInteger();
          }

          // 先端梁幅
          row[_CmpParameters.SentanHarihaba] = famSymBeamType.LookupParameter(_CmpParameters.SentanHarihaba).AsDouble();
          if ((double)row[_CmpParameters.SentanHarihaba] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanHarihaba).AsInteger() != 0)
          {
            row[_CmpParameters.SentanHarihaba] = famSymBeamType.LookupParameter(_CmpParameters.SentanHarihaba).AsInteger();
          }

          // 元端梁成
          row[_CmpParameters.MototanHarisei] = famSymBeamType.LookupParameter(_CmpParameters.MototanHarisei).AsDouble();
          if ((double)row[_CmpParameters.MototanHarisei] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanHarisei).AsInteger() != 0)
          {
            row[_CmpParameters.MototanHarisei] = famSymBeamType.LookupParameter(_CmpParameters.MototanHarisei).AsInteger();
          }

          // 先端梁成
          row[_CmpParameters.SentanHarisei] = famSymBeamType.LookupParameter(_CmpParameters.SentanHarisei).AsDouble();
          if ((double)row[_CmpParameters.SentanHarisei] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanHarisei).AsInteger() != 0)
          {
            row[_CmpParameters.SentanHarisei] = famSymBeamType.LookupParameter(_CmpParameters.SentanHarisei).AsInteger();
          }

          // 元端上主筋太径
          row[_CmpParameters.MototanUeSyukinHutokei] = famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukinHutokei).AsString();

          // 先端上主筋太径
          row[_CmpParameters.SentanUeSyukinHutokei] = famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukinHutokei).AsString();

          // 元端上主筋1段太筋本数
          row[_CmpParameters.MototanUeSyukin1danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin1danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanUeSyukin1danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin1danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanUeSyukin1danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin1danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端上主筋1段太筋本数
          row[_CmpParameters.SentanUeSyukin1danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin1danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanUeSyukin1danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin1danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanUeSyukin1danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin1danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端上主筋2段太筋本数
          row[_CmpParameters.MototanUeSyukin2danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin2danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanUeSyukin2danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin2danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanUeSyukin2danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin2danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端上主筋2段太筋本数
          row[_CmpParameters.SentanUeSyukin2danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin2danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanUeSyukin2danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin2danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanUeSyukin2danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin2danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端上主筋3段太筋本数
          row[_CmpParameters.MototanUeSyukin3danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin3danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanUeSyukin3danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin3danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanUeSyukin3danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin3danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端上主筋3段太筋本数
          row[_CmpParameters.SentanUeSyukin3danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin3danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanUeSyukin3danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin3danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanUeSyukin3danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin3danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端下主筋太径
          row[_CmpParameters.MototanSitaSyukinHutokei] = famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukinHutokei).AsString();

          // 先端下主筋太径
          row[_CmpParameters.SentanSitaSyukinHutokei] = famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukinHutokei).AsString();

          // 元端下主筋1段太筋本数
          row[_CmpParameters.MototanSitaSyukin1danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin1danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanSitaSyukin1danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin1danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanSitaSyukin1danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin1danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端下主筋1段太筋本数
          row[_CmpParameters.SentanSitaSyukin1danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin1danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanSitaSyukin1danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin1danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanSitaSyukin1danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin1danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端下主筋2段太筋本数
          row[_CmpParameters.MototanSitaSyukin2danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin2danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanSitaSyukin2danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin2danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanSitaSyukin2danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin2danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端下主筋2段太筋本数
          row[_CmpParameters.SentanSitaSyukin2danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin2danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanSitaSyukin2danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin2danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanSitaSyukin2danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin2danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端下主筋3段太筋本数
          row[_CmpParameters.MototanSitaSyukin3danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin3danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanSitaSyukin3danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin3danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanSitaSyukin3danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin3danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端下主筋3段太筋本数
          row[_CmpParameters.SentanSitaSyukin3danHutokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin3danHutokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanSitaSyukin3danHutokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin3danHutokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanSitaSyukin3danHutokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin3danHutokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端上主筋細径
          row[_CmpParameters.MototanUeSyukinHosokei] = famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukinHosokei).AsString();

          // 先端上主筋細径
          row[_CmpParameters.SentanUeSyukinHosokei] = famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukinHosokei).AsString();

          // 元端上主筋1段細筋本数
          row[_CmpParameters.MototanUeSyukin1danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin1danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanUeSyukin1danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin1danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanUeSyukin1danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin1danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端上主筋1段細筋本数
          row[_CmpParameters.SentanUeSyukin1danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin1danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanUeSyukin1danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin1danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanUeSyukin1danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin1danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端上主筋2段細筋本数
          row[_CmpParameters.MototanUeSyukin2danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin2danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanUeSyukin2danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin2danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanUeSyukin2danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin2danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端上主筋2段細筋本数
          row[_CmpParameters.SentanUeSyukin2danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin2danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanUeSyukin2danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin2danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanUeSyukin2danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin2danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端上主筋3段細筋本数
          row[_CmpParameters.MototanUeSyukin3danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin3danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanUeSyukin3danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin3danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanUeSyukin3danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanUeSyukin3danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端上主筋3段細筋本数
          row[_CmpParameters.SentanUeSyukin3danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin3danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanUeSyukin3danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin3danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanUeSyukin3danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanUeSyukin3danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端下主筋細径
          row[_CmpParameters.MototanSitaSyukinHosokei] = famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukinHosokei).AsString();

          // 先端下主筋細径
          row[_CmpParameters.SentanSitaSyukinHosokei] = famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukinHosokei).AsString();

          // 元端下主筋1段細筋本数
          row[_CmpParameters.MototanSitaSyukin1danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin1danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanSitaSyukin1danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin1danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanSitaSyukin1danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin1danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端下主筋1段細筋本数
          row[_CmpParameters.SentanSitaSyukin1danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin1danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanSitaSyukin1danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin1danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanSitaSyukin1danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin1danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端下主筋2段細筋本数
          row[_CmpParameters.MototanSitaSyukin2danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin2danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanSitaSyukin2danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin2danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanSitaSyukin2danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin2danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端下主筋2段細筋本数
          row[_CmpParameters.SentanSitaSyukin2danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin2danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanSitaSyukin2danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin2danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanSitaSyukin2danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin2danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端下主筋3段細筋本数
          row[_CmpParameters.MototanSitaSyukin3danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin3danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanSitaSyukin3danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin3danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanSitaSyukin3danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanSitaSyukin3danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端下主筋3段細筋本数
          row[_CmpParameters.SentanSitaSyukin3danHosokinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin3danHosokinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanSitaSyukin3danHosokinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin3danHosokinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanSitaSyukin3danHosokinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanSitaSyukin3danHosokinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端肋筋径
          row[_CmpParameters.MototanAbarakinkei] = famSymBeamType.LookupParameter(_CmpParameters.MototanAbarakinkei).AsString();

          // 先端肋筋径
          row[_CmpParameters.SentanAbarakinkei] = famSymBeamType.LookupParameter(_CmpParameters.SentanAbarakinkei).AsString();

          // 元端肋筋本数
          row[_CmpParameters.MototanAbarakinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanAbarakinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanAbarakinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanAbarakinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanAbarakinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanAbarakinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端肋筋本数
          row[_CmpParameters.SentanAbarakinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanAbarakinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanAbarakinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanAbarakinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanAbarakinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanAbarakinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端肋筋ピッチ
          row[_CmpParameters.MototanAbarakinPitch] = famSymBeamType.LookupParameter(_CmpParameters.MototanAbarakinPitch).AsDoubleMm();
          if ((double)row[_CmpParameters.MototanAbarakinPitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanAbarakinPitch).AsInteger() != 0)
          {
            row[_CmpParameters.MototanAbarakinPitch] = famSymBeamType.LookupParameter(_CmpParameters.MototanAbarakinPitch).AsInteger();
          }

          // 先端肋筋ピッチ
          row[_CmpParameters.SentanAbarakinPitch] = famSymBeamType.LookupParameter(_CmpParameters.SentanAbarakinPitch).AsDoubleMm();
          if ((double)row[_CmpParameters.SentanAbarakinPitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanAbarakinPitch).AsInteger() != 0)
          {
            row[_CmpParameters.SentanAbarakinPitch] = famSymBeamType.LookupParameter(_CmpParameters.SentanAbarakinPitch).AsInteger();
          }

          // 元端腹筋径
          row[_CmpParameters.MototanHarakinkei] = famSymBeamType.LookupParameter(_CmpParameters.MototanHarakinkei).AsString();

          // 先端腹筋径
          row[_CmpParameters.SentanHarakinkei] = famSymBeamType.LookupParameter(_CmpParameters.SentanHarakinkei).AsString();

          // 元端腹筋本数
          row[_CmpParameters.MototanHarakinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanHarakinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanHarakinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanHarakinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanHarakinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanHarakinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端腹筋本数
          row[_CmpParameters.SentanHarakinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanHarakinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanHarakinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanHarakinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanHarakinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanHarakinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端幅止筋径
          row[_CmpParameters.MototanHabadomekinkei] = famSymBeamType.LookupParameter(_CmpParameters.MototanHabadomekinkei).AsString();

          // 先端幅止筋径
          row[_CmpParameters.SentanHabadomekinkei] = famSymBeamType.LookupParameter(_CmpParameters.SentanHabadomekinkei).AsString();

          // 元端幅止筋本数
          row[_CmpParameters.MototanHabadomekinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.MototanHabadomekinHonsu).AsInteger();
          if ((int)row[_CmpParameters.MototanHabadomekinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanHabadomekinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.MototanHabadomekinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.MototanHabadomekinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 先端幅止筋本数
          row[_CmpParameters.SentanHabadomekinHonsu] = famSymBeamType.LookupParameter(_CmpParameters.SentanHabadomekinHonsu).AsInteger();
          if ((int)row[_CmpParameters.SentanHabadomekinHonsu] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanHabadomekinHonsu).AsDouble() != 0)
          {
            row[_CmpParameters.SentanHabadomekinHonsu] = System.Math.Round(famSymBeamType.LookupParameter(_CmpParameters.SentanHabadomekinHonsu).AsDouble(), System.MidpointRounding.AwayFromZero);
          }

          // 元端幅止筋ピッチ
          if (famSymBeamType.LookupParameter(_CmpParameters.MototanHabadomekinPitch) != null)
          {
            row[_CmpParameters.MototanHabadomekinPitch] = famSymBeamType.LookupParameter(_CmpParameters.MototanHabadomekinPitch).AsDoubleMm();
            if ((double)row[_CmpParameters.MototanHabadomekinPitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.MototanHabadomekinPitch).AsInteger() != 0)
            {
              row[_CmpParameters.MototanHabadomekinPitch] = famSymBeamType.LookupParameter(_CmpParameters.MototanHabadomekinPitch).AsInteger();
            }
          }

          // 先端幅止筋ピッチ
          if (famSymBeamType.LookupParameter(_CmpParameters.SentanHabadomekinPitch) != null)
          {
            row[_CmpParameters.SentanHabadomekinPitch] = famSymBeamType.LookupParameter(_CmpParameters.SentanHabadomekinPitch).AsDoubleMm();
            if ((double)row[_CmpParameters.SentanHabadomekinPitch] == 0 && famSymBeamType.LookupParameter(_CmpParameters.SentanHabadomekinPitch).AsInteger() != 0)
            {
              row[_CmpParameters.SentanHabadomekinPitch] = famSymBeamType.LookupParameter(_CmpParameters.SentanHabadomekinPitch).AsInteger();
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
      /// <history>2014/06/13 Created GSA,Inc. Ryo Kurodai</history>
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
      /// <history>2014/06/13 Created GSA,Inc. Ryo Kuroda</history>
      /// ================================================================================
      public
      System.Data.DataTable Data
      {
        get
        {
          return _Data;
        }
      }

      #endregion
  }
}
