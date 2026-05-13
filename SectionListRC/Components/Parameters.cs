using System ;
using System.Text.RegularExpressions ;
using Autodesk.Revit.ApplicationServices ;
using Autodesk.Revit.DB ;
using Microsoft.VisualBasic.FileIO ;
using Collections = System.Collections ;
using Revit = Autodesk.Revit ;
using System.Text;

namespace SectionListRC.Components
{
  /// ================================================================================
  /// <summary>パラメータ</summary>
  /// ================================================================================
  public partial class Parameters : SectionListRC.JExtComCompat.RvtParameters
  {
    // メンバ変数

    #region Member Variables

    /// <summary>属性</summary>
    private SectionListRC.Components.Attribute _CmpAttribute ;

    //標準共有パラメータファイル名
    private string _ShParamDefaultFileName ;

    //共有パラメータフォルダ名
    private string _ShParamFolderName ;

    //共有パラメータファイル名
    private string _ShParamFileName ;

    //共有パラメータグループ名
    private string _ShParamGroupName ;

    // 設定ファイル名
    private string _SettingFileName ;

    // 設定ファイルディレクトリ
    private string _SettingFileDirectory ;

    /// <summary>柱リストビュー尺度</summary>
    private string _ColumnListViewScale ;

    /// <summary>梁リストビュー尺度</summary>
    private string _BeamListViewScale ;

    /// <summary>タイトルフォント</summary>
    private string _TitleFont ;

    /// <summary>小項目フォント</summary>
    private string _ItemFont ;

    /// <summary>寸法線タイプ</summary>
    private string _DimensionType ;

    /// <summary>枠線種タイプ</summary>
    private string _FrameLineType ;

    /// <summary>躯体線種タイプ</summary>
    private string _BodyLineType ;

    /// <summary>幅止筋線種タイプ</summary>
    private string _SpacerLineType ;

    /// <summary>階表示枠表示</summary>
    private string _LevelFrameShow ;

    /// <summary>階表示枠幅</summary>
    private string _LevelFrameWidth ;

    /// <summary>項目表示枠幅</summary>
    private string _ItemFrameWidth ;

    /// <summary>符号表示枠高さ</summary>
    private string _SymbolFrameHeight ;

    /// <summary>配筋枠高さ</summary>
    private string _ArrangementFrameHeight ;

    /// <summary>項目表示枠幅2</summary>
    private string _ItemFrameWidth2 ;

    /// <summary>符号表示枠高さ2</summary>
    private string _SymbolFrameHeight2 ;

    /// <summary>配筋枠高さ2</summary>
    private string _ArrangementFrameHeight2 ;

    /// <summary>階表示枠タイトル</summary>
    private string _LevelFrameTitle ;

    /// <summary>階表示枠接尾語</summary>
    private string _LevelFrameEndWord ;

    /// <summary>符号表示枠タイトル</summary>
    private string _SymbolFrameTitle ;

    /// <summary>マッピングテーブル</summary>
    private string _SelectTable ;

    /// <summary>マッピングテーブルの指定</summary>
    private string _PickTable ;

    /// <summary>左のあき - 柱</summary>
    private string _ColumnLeftSpace ;

    /// <summary>右のあき - 柱</summary>
    private string _ColumnRightSpace ;

    /// <summary>上のあき - 柱</summary>
    private string _ColumnTopSpace ;

    /// <summary>下のあき - 柱</summary>
    private string _ColumnBottomSpace ;

    /// <summary>帯筋括弧表示</summary>
    private string _HoopBracketShow ;

    /// <summary>追加枠数 - 柱</summary>
    private string _ColumnAddFrameNumber ;

    /// <summary>主筋表示 - 柱</summary>
    private string _ColumnRebarShow ;

    /// <summary>帯筋枠タイトル</summary>
    private string _HoopFrameTitle ;

    /// <summary>帯筋枠区切り記号</summary>
    private string _HoopFrameSpaceSymbol ;

    /// <summary>左のあき - 梁</summary>
    private string _BeamLeftSpace ;

    /// <summary>右のあき - 梁</summary>
    private string _BeamRightSpace ;

    /// <summary>中間あきタイプ</summary>
    private string _BeamCenterSpaceType ;

    /// <summary>中間のあき</summary>
    private string _BeamCenterSpace ;

    /// <summary>上のあき - 梁</summary>
    private string _BeamTopSpace ;

    /// <summary>下のあき - 梁</summary>
    private string _BeamBottomSpace ;

    /// <summary>位置表示枠高さ</summary>
    private string _PositionFrameHeight ;

    /// <summary>肋筋括弧表示</summary>
    private string _StirrupBracketShow ;

    /// <summary>追加枠数 - 梁</summary>
    private string _BeamAddFrameNumber ;

    /// <summary>位置表示枠タイトル表示</summary>
    private string _PositionFrameTitleShow ;

    /// <summary>位置表示枠区切り線表示</summary>
    private string _PositionFrameSpaceLineShow ;

    /// <summary>全断面タイトル</summary>
    private string _AllSectionTitle ;

    /// <summary>端部タイトル</summary>
    private string _EdgeTitle ;

    /// <summary>中央部タイトル</summary>
    private string _CenterSectionTitle ;

    /// <summary>始端タイトル</summary>
    private string _ItanSectionTitle ;

    /// <summary>終端タイトル</summary>
    private string _JtanSectionTitle ;

    /// <summary>片持ち梁元端タイトル</summary>
    private string _CantileverStartTitle ;

    /// <summary>片持ち梁先端タイトル</summary>
    private string _CantileverEndTitle ;

    /// <summary>肋筋枠タイトル</summary>
    private string _StirrupFrameTitle ;

    /// <summary>肋筋枠区切り記号</summary>
    private string _StirrupFrameSpaceSymbol ;

    /// <summary>幅寸法線表示</summary>
    private string _WidthDimensionShow ;

    /// <summary>高さ寸法線表示</summary>
    private string _HeightDimensionShow ;

    /// <summary>主筋表示 - 梁</summary>
    private string _BeamRebarShow ;

    /// <summary>肋筋枠表示</summary>
    private string _StirrupFrameShow ;

    /// <summary>腹筋枠表示</summary>
    private string _WebFrameShow ;

    /// <summary>かぶり厚 - 角柱</summary>
    private string _ColumnProtectThick ;

    /// <summary>2段筋コーナー配筋フラグ</summary>
    private string _2ndRebarCornerSetFlag ;

    /// <summary>かぶり厚 - 円柱</summary>
    private string _CylinderProtectThick ;

    /// <summary>かぶり厚 - 梁</summary>
    private string _BeamProtectThick ;

    // ---------- ---------- ---------- ---------- ----------
    // パラメータマッピングの名前

    /// <summary>柱分類 - 角柱</summary>
    private string _HashiraBunrui_Kaku ;

    /// <summary>柱幅 - 角柱</summary>
    private string _HashiraHaba_Kaku ;

    /// <summary>柱成 - 角柱</summary>
    private string _HashiraSei_Kaku ;

    /// <summary>符号 - 角柱</summary>
    private string _Hugo_Kaku ;

    /// <summary>柱頭 主筋太径 - 角柱</summary>
    private string _ChutoSyukinHutokei_Kaku ;

    /// <summary>柱頭 主筋細径 - 角柱</summary>
    private string _ChutoSyukinHosokei_Kaku ;

    /// <summary>柱頭 主筋X方向1段太筋本数 - 角柱</summary>
    private string _ChutoSyukinX1danHutokinHonsu_Kaku ;

    /// <summary>柱頭 主筋X方向1段細筋本数 - 角柱</summary>
    private string _ChutoSyukinX1danHosokinHonsu_Kaku ;

    /// <summary>柱頭 主筋X方向2段太筋本数 - 角柱</summary>
    private string _ChutoSyukinX2danHutokinHonsu_Kaku ;

    /// <summary>柱頭 主筋X方向2段細筋本数 - 角柱</summary>
    private string _ChutoSyukinX2danHosokinHonsu_Kaku ;

    /// <summary>柱頭 主筋Y方向1段太筋本数 - 角柱</summary>
    private string _ChutoSyukinY1danHutokinHonsu_Kaku ;

    /// <summary>柱頭 主筋Y方向1段細筋本数 - 角柱</summary>
    private string _ChutoSyukinY1danHosokinHonsu_Kaku ;

    /// <summary>柱頭 主筋Y方向2段太筋本数 - 角柱</summary>
    private string _ChutoSyukinY2danHutokinHonsu_Kaku ;

    /// <summary>柱頭 主筋Y方向2段細筋本数 - 角柱</summary>
    private string _ChutoSyukinY2danHosokinHonsu_Kaku ;

    /// <summary>柱脚 主筋太径 - 角柱</summary>
    private string _ChukyakuSyukinHutokei_Kaku ;

    /// <summary>柱脚 主筋細径 - 角柱</summary>
    private string _ChukyakuSyukinHosokei_Kaku ;

    /// <summary>柱脚 主筋X方向1段太筋本数 - 角柱</summary>
    private string _ChukyakuSyukinX1danHutokinHonsu_Kaku ;

    /// <summary>柱脚 主筋X方向1段細筋本数 - 角柱</summary>
    private string _ChukyakuSyukinX1danHosokinHonsu_Kaku ;

    /// <summary>柱脚 主筋X方向2段太筋本数 - 角柱</summary>
    private string _ChukyakuSyukinX2danHutokinHonsu_Kaku ;

    /// <summary>柱脚 主筋X方向2段細筋本数 - 角柱</summary>
    private string _ChukyakuSyukinX2danHosokinHonsu_Kaku ;

    /// <summary>柱脚 主筋Y方向1段太筋本数 - 角柱</summary>
    private string _ChukyakuSyukinY1danHutokinHonsu_Kaku ;

    /// <summary>柱脚 主筋Y方向1段細筋本数 - 角柱</summary>
    private string _ChukyakuSyukinY1danHosokinHonsu_Kaku ;

    /// <summary>柱脚 主筋Y方向2段太筋本数 - 角柱</summary>
    private string _ChukyakuSyukinY2danHutokinHonsu_Kaku ;

    /// <summary>柱脚 主筋Y方向2段細筋本数 - 角柱</summary>
    private string _ChukyakuSyukinY2danHosokinHonsu_Kaku ;

    /// <summary>柱頭 帯筋径 - 角柱</summary>
    private string _ChutoObikinKei_Kaku ;

    /// <summary>柱頭 帯筋X方向本数 - 角柱</summary>
    private string _ChutoObikinXHonsu_Kaku ;

    /// <summary>柱頭 帯筋Y方向本数 - 角柱</summary>
    private string _ChutoObikinYHonsu_Kaku ;

    /// <summary>柱頭 帯筋ピッチ - 角柱</summary>
    private string _ChutoObikinPitch_Kaku ;

    /// <summary>柱脚 帯筋径 - 角柱</summary>
    private string _ChukyakuObikinKei_Kaku ;

    /// <summary>柱脚 帯筋X方向本数 - 角柱</summary>
    private string _ChukyakuObikinXHonsu_Kaku ;

    /// <summary>柱脚 帯筋Y方向本数 - 角柱</summary>
    private string _ChukyakuObikinYHonsu_Kaku ;

    /// <summary>柱脚 帯筋ピッチ - 角柱</summary>
    private string _ChukyakuObikinPitch_Kaku ;

    /// <summary>幅止筋ピッチ - 角柱</summary>
    private string _HabadomekinPitch_Kaku ;

    /// <summary>幅止筋径 - 角柱</summary>
    private string _HabadomekinKei_Kaku ;

    /// <summary>柱頭 幅止筋X方向本数 - 角柱</summary>
    private string _ChutoHabadomekinXHonsu_Kaku ;

    /// <summary>柱頭 幅止筋Y方向本数 - 角柱</summary>
    private string _ChutoHabadomekinYHonsu_Kaku ;

    /// <summary>柱脚 幅止筋X方向本数 - 角柱</summary>
    private string _ChukyakuHabadomekinXHonsu_Kaku ;

    /// <summary>柱脚 幅止筋Y方向本数 - 角柱</summary>
    private string _ChukyakuHabadomekinYHonsu_Kaku ;

    /// <summary>芯鉄筋径 - 角柱</summary>
    private string _SinTekkinKei_Kaku ;

    /// <summary>芯鉄筋本数 - 角柱</summary>
    private string _SinTekkinHonsu_Kaku ;

    /// <summary>躯体面から芯鉄筋X方向までの距離 - 角柱</summary>
    private string _SinTekkinXHoukouKyori_Kaku ;

    /// <summary>躯体面から芯鉄筋Y方向までの距離 - 角柱</summary>
    private string _SinTekkinYHoukouKyuori_Kaku ;

    /// <summary>コンクリート強度の低減率 - 角柱</summary>
    private string _ConcreteTeigenritsu_Kaku ;

    /// <summary>主筋種別X - 角柱</summary>
    private string _SyukinSyubetuX_Kaku ;

    /// <summary>主筋種別Y - 角柱</summary>
    private string _SyukinSyubetuY_Kaku ;

    /// <summary>芯鉄筋種別 - 角柱</summary>
    private string _SinTekkinSyubetu_Kaku ;

    /// <summary>柱頭 寄せ筋方向 - 角柱</summary>
    private string _ChutoYosekinHoukou_Kaku ;

    /// <summary>柱脚 寄せ筋方向 - 角柱</summary>
    private string _ChukyakuYosekinHoukou_Kaku ;

    /// <summary>柱種別 - 円柱</summary>
    private string _HashiraSyubetsu_En ;

    /// <summary>直径 - 円柱</summary>
    private string _Tyokkei_En ;

    /// <summary>符号 - 円柱</summary>
    private string _Hugo_En ;

    /// <summary>柱頭 主筋径 - 円柱</summary>
    private string _ChutoSyukinKei_En ;

    /// <summary>柱頭 主筋本数 - 円柱</summary>
    private string _ChutoSyukinHonsu_En ;

    /// <summary>柱脚 主筋径 - 円柱</summary>
    private string _ChukyakuSyukinKei_En ;

    /// <summary>柱脚 主筋本数 - 円柱</summary>
    private string _ChukyakuSyukinHonsu_En ;

    /// <summary>柱頭 帯筋径 - 円柱</summary>
    private string _ChutoObikinKei_En ;

    /// <summary>柱頭 帯筋ピッチ - 円柱</summary>
    private string _ChutoObikinPitch_En ;

    /// <summary>柱脚 帯筋径 - 円柱</summary>
    private string _ChukyakuObikinKei_En ;

    /// <summary>柱脚 帯筋ピッチ - 円柱</summary>
    private string _ChukyakuObikinPitch_En ;

    /// <summary>幅止筋ピッチ - 円柱</summary>
    private string _HabadomekinPitch_En ;

    /// <summary>幅止筋径 - 円柱</summary>
    private string _HabadomekinKei_En ;

    /// <summary>柱頭 幅止筋X方向本数 - 円柱</summary>
    private string _ChutoHabadomekinXHonsu_En ;

    /// <summary>柱頭 幅止筋Y方向本数 - 円柱</summary>
    private string _ChutoHabadomekinYHonsu_En ;

    /// <summary>柱脚 幅止筋X方向本数 - 円柱</summary>
    private string _ChukyakuHabadomekinXHonsu_En ;

    /// <summary>柱脚 幅止筋Y方向本数 - 円柱</summary>
    private string _ChukyakuHabadomekinYHonsu_En ;

    /// <summary>芯鉄筋径 - 円柱</summary>
    private string _SinTekkinKei_En ;

    /// <summary>芯鉄筋本数 - 円柱</summary>
    private string _SinTekkinHonsu_En ;

    /// <summary>躯体面から芯鉄筋までの距離 - 円柱</summary>
    private string _SinTekkinKyori_En ;

    /// <summary>コンクリート強度の低減率 - 円柱</summary>
    private string _ConcreteTeigenritu_En ;

    /// <summary>主筋種別 - 円柱</summary>
    private string _SyukinSyubetu_En ;

    /// <summary>芯鉄筋種別 - 円柱</summary>
    private string _SinTekkinSyubetu_En ;

    /// <summary>梁種別</summary>
    private string _HariSyubetu ;

    /// <summary>始端 梁幅</summary>
    private string _ShitanHarihaba ;

    /// <summary>中央 梁幅</summary>
    private string _ChuouHarihaba ;

    /// <summary>終端 梁幅</summary>
    private string _SyutanHarihaba ;

    /// <summary>始端 梁成</summary>
    private string _ShitanHarisei ;

    /// <summary>中央 梁成</summary>
    private string _ChuouHarisei ;

    /// <summary>終端 梁成</summary>
    private string _SyutanHarisei ;

    /// <summary>ボックスハンチ 始端</summary>
    private string _ShitanBoxHaunch ;

    /// <summary>ボックスハンチ 終端</summary>
    private string _SyutanBoxHaunch ;

    /// <summary>始端 ハンチ長さ</summary>
    private string _ShitanHaunchNagasa ;

    /// <summary>終端 ハンチ長さ</summary>
    private string _SyutanHaunchNagasa ;

    /// <summary>符号</summary>
    private string _Hugo_Hari ;

    /// <summary>始端 上主筋 太径</summary>
    private string _SitanUeSyukinHutokei ;

    /// <summary>始端 上主筋 細径</summary>
    private string _SitanUeSyukinHosokei ;

    /// <summary>始端 上主筋 1段筋太筋本数</summary>
    private string _SitanUeSyukin1danHutokeiHonsu ;

    /// <summary>始端 上主筋 1段筋細筋本数</summary>
    private string _SitanUeSyukin1danHosokeiHonsu ;

    /// <summary>始端 上主筋 2段筋太筋本数</summary>
    private string _SitanUeSyukin2danHutokeiHonsu ;

    /// <summary>始端 上主筋 2段筋細筋本数</summary>
    private string _SitanUeSyukin2danHosokeiHonsu ;

    /// <summary>始端 上主筋 3段筋太筋本数</summary>
    private string _SitanUeSyukin3danHutokeiHonsu ;

    /// <summary>始端 上主筋 3段筋細筋本数</summary>
    private string _SitanUeSyukin3danHosokeiHonsu ;

    /// <summary>始端 下主筋 太径</summary>
    private string _SitanSitaSyukinHutokei ;

    /// <summary>始端 下主筋 細径</summary>
    private string _SitanSitaSyukinHosokei ;

    /// <summary>始端 下主筋 1段筋太筋本数</summary>
    private string _SitanSitaSyukin1danHutokeiHonsu ;

    /// <summary>始端 下主筋 1段筋細筋本数</summary>
    private string _SitanSitaSyukin1danHosokeiHonsu ;

    /// <summary>始端 下主筋 2段筋太筋本数</summary>
    private string _SitanSitaSyukin2danHutokeiHonsu ;

    /// <summary>始端 下主筋 2段筋細筋本数</summary>
    private string _SitanSitaSyukin2danHosokeiHonsu ;

    /// <summary>始端 下主筋 3段筋太筋本数</summary>
    private string _SitanSitaSyukin3danHutokeiHonsu ;

    /// <summary>始端 下主筋 3段筋細筋本数</summary>
    private string _SitanSitaSyukin3danHosokeiHosnu ;

    /// <summary>中央 上主筋 太径</summary>
    private string _ChuouUeSyukinHutokei ;

    /// <summary>中央 上主筋 細径</summary>
    private string _ChuouUeSyukinHosokei ;

    /// <summary>中央 上主筋 1段筋太筋本数</summary>
    private string _ChuouUeSyukin1danHutokeiHonsu ;

    /// <summary>中央 上主筋 1段筋細筋本数</summary>
    private string _ChuouUeSyukin1danHosokeiHonsu ;

    /// <summary>中央 上主筋 2段筋太筋本数</summary>
    private string _ChuouUeSyukin2danHutokeiHonsu ;

    /// <summary>中央 上主筋 2段筋細筋本数</summary>
    private string _ChuouUeSyukin2danHosokeiHonsu ;

    /// <summary>中央 上主筋 3段筋太筋本数</summary>
    private string _ChuouUeSyukin3danHutokeiHonsu ;

    /// <summary>中央 上主筋 3段筋細筋本数</summary>
    private string _ChuouUeSyukin3danHosokeiHonsu ;

    /// <summary>中央 下主筋 太径</summary>
    private string _ChuouSitaSyukinHutokei ;

    /// <summary>中央 下主筋 細径</summary>
    private string _ChuouSitaSyukinHosokei ;

    /// <summary>中央 下主筋 1段筋太筋本数</summary>
    private string _ChuouSitaSyukin1danHutokeiHonsu ;

    /// <summary>中央 下主筋 1段筋細筋本数</summary>
    private string _ChuouSitaSyukin1danHosokeiHonsu ;

    /// <summary>中央 下主筋 2段筋太筋本数</summary>
    private string _ChuouSitaSyukin2danHutokeiHonsu ;

    /// <summary>中央 下主筋 2段筋細筋本数</summary>
    private string _ChuouSitaSyukin2danHosokeiHonsu ;

    /// <summary>中央 下主筋 3段筋太筋本数</summary>
    private string _ChuouSitaSyukin3danHutokeiHonsu ;

    /// <summary>中央 下主筋 3段筋細筋本数</summary>
    private string _ChuouSitaSyukin3danHosokeiHonsu ;

    /// <summary>終端 上主筋 太径</summary>
    private string _SyutanUeSyukinHutokei ;

    /// <summary>終端 上主筋 細径</summary>
    private string _SyutanUeSyukinHosokei ;

    /// <summary>終端 上主筋 1段筋太筋本数</summary>
    private string _SyutanUeSyukin1danHutokeiHonsu ;

    /// <summary>終端 上主筋 1段筋細筋本数</summary>
    private string _SyutanUeSyukin1danHosokeiHonsu ;

    /// <summary>終端 上主筋 2段筋太筋本数</summary>
    private string _SyutanUeSyukin2danHutokeiHonsu ;

    /// <summary>終端 上主筋 2段筋細筋本数</summary>
    private string _SyutanUeSyukin2danHosokeiHonsu ;

    /// <summary>終端 上主筋 3段筋太筋本数</summary>
    private string _SyutanUeSyukin3danHutokeiHonsu ;

    /// <summary>RST_主筋j端上3段筋細筋本数</summary>
    private string _SyutanUeSyukin3danHosokeiHonsu ;

    /// <summary>終端 下主筋 太径</summary>
    private string _SyutanSitaSyukinHutokei ;

    /// <summary>終端 下主筋 細径</summary>
    private string _SyutanSitaSyukinHosokei ;

    /// <summary>終端 下主筋 1段筋太筋本数</summary>
    private string _SyutanSitaSyukin1danHutokeiHonsu ;

    /// <summary>終端 下主筋 1段筋細筋本数</summary>
    private string _SyutanSitaSyukin1danHosokeiHonsu ;

    /// <summary>終端 下主筋 2段筋太筋本数</summary>
    private string _SyutanSitaSyukin2danHutokeiHonsu ;

    /// <summary>終端 下主筋 2段筋細筋本数</summary>
    private string _SyutanSitaSyukin2danHosokeiHonsu ;

    /// <summary>終端 下主筋 3段筋太筋本数</summary>
    private string _SyutanSitaSyukin3danHutokeiHonsu ;

    /// <summary>終端 下主筋 3段筋細筋本数</summary>
    private string _SyutanSitaSyukin3danHosokeiHosnu ;

    /// <summary>始端 肋筋径</summary>
    private string _SitanAbarakinkei ;

    /// <summary>中央 肋筋径</summary>
    private string _ChuouAbarakinkei ;

    /// <summary>終端 肋筋径</summary>
    private string _SyutanAbarakinkei ;

    /// <summary>始端 肋筋本数</summary>
    private string _SitanAbarakinHonsu ;

    /// <summary>中央 肋筋本数</summary>
    private string _ChuouAbarakinHonsu ;

    /// <summary>終端 肋筋本数</summary>
    private string _SyutanAbarakinHonsu ;

    /// <summary>始端 肋筋ピッチ</summary>
    private string _SitanAbarakinPitch ;

    /// <summary>中央 肋筋ピッチ</summary>
    private string _ChuouAbarakinPitch ;

    /// <summary>終端 肋筋ピッチ</summary>
    private string _SyutanAbarakinPitch ;

    /// <summary>始端 腹筋径</summary>
    private string _SitanHarakinkei ;

    /// <summary>中央 腹筋径</summary>
    private string _ChuouHarakinkei ;

    /// <summary>終端 腹筋径</summary>
    private string _SyutanHarakinkei ;

    /// <summary>始端 腹筋本数</summary>
    private string _SitanHarakinHonsu ;

    /// <summary>中央 腹筋本数</summary>
    private string _ChuouHarakinHonsu ;

    /// <summary>終端 腹筋本数</summary>
    private string _SyutanHarakinHonsu ;

    /// <summary>始端 幅止筋径</summary>
    private string _SitanHabadomekinkei ;

    /// <summary>中央 幅止筋径</summary>
    private string _ChuouHabadomekinkei ;

    /// <summary>終端 幅止筋径</summary>
    private string _SyutanHabadomekinkei ;

    /// <summary>始端 幅止筋本数</summary>
    private string _SitanHabadomekinHonsu ;

    /// <summary>中央 幅止筋本数</summary>
    private string _ChuouHabadomekinHonsu ;

    /// <summary>終端 幅止筋本数</summary>
    private string _SyutanHabadomekinHonsu ;

    /// <summary>始端 幅止筋ピッチ</summary>
    private string _SitanHabadomekinPitch ;

    /// <summary>中央 幅止筋ピッチ</summary>
    private string _ChuouHabadomekinPitch ;

    /// <summary>終端 幅止筋ピッチ</summary>
    private string _SyutanHabadomekinPitch ;

    /// <summary>主筋種別 - 梁</summary>
    private string _SyukinSyubetu_Hari ;

    /// <summary>梁種別 - 片持ち梁</summary>
    private string _HariSyubetu_Katamoti ;

    /// <summary>元端 梁幅</summary>
    private string _MototanHarihaba ;

    /// <summary>先端 梁幅</summary>
    private string _SentanHarihaba ;

    /// <summary>元端 梁せい</summary>
    private string _MototanHariSei ;

    /// <summary>先端 梁せい</summary>
    private string _SentanHarisei ;

    /// <summary>符号 - 片持ち梁</summary>
    private string _Hugo_Katamoti ;

    /// <summary>元端 上主筋太径</summary>
    private string _MototanUeSyukinHutokei ;

    /// <summary>元端 上主筋細径</summary>
    private string _MototanUeSyukinHosokei ;

    /// <summary>元端 上主筋 1段太筋本数</summary>
    private string _MototanUeSyukin1danHutokinHonsu ;

    /// <summary>元端 上主筋 1段細筋本数</summary>
    private string _MototanUeSyukin1danHosokinHonsu ;

    /// <summary>元端 上主筋 2段太筋本数</summary>
    private string _MototanUeSyukin2danHutokinHonsu ;

    /// <summary>元端 上主筋 2段細筋本数</summary>
    private string _MototanUeSyukin2danHosokinHonsu ;

    /// <summary>元端 上主筋 3段太筋本数</summary>
    private string _MototanUeSyukin3danHutokinHonsu ;

    /// <summary>元端 上主筋 3段細筋本数</summary>
    private string _MototanUeSyukin3danHosokinHonsu ;

    /// <summary>元端 下主筋太径</summary>
    private string _MototanSitaSyukinHutokei ;

    /// <summary>元端 下主筋細径</summary>
    private string _MototanSitaSyukinHosokei ;

    /// <summary>元端 下主筋 1段太筋本数</summary>
    private string _MototanSitaSyukin1danHutokinHonsu ;

    /// <summary>元端 下主筋 1段細筋本数</summary>
    private string _MototanSitaSyukin1danHosokinHonsu ;

    /// <summary>元端 下主筋 2段太筋本数</summary>
    private string _MototanSitaSyukin2danHutokinHonsu ;

    /// <summary>元端 下主筋 2段細筋本数</summary>
    private string _MototanSitaSyukin2danHosokinHonsu ;

    /// <summary>元端 下主筋 3段太筋本数</summary>
    private string _MototanSitaSyukin3danHutokinHonsu ;

    /// <summary>元端 下主筋 3段細筋本数</summary>
    private string _MototanSitaSyukin3danHosokinHonsu ;

    /// <summary>先端 上主筋太径</summary>
    private string _SentanUeSyukinHutokei ;

    /// <summary>先端 上主筋細径</summary>
    private string _SentanUeSyukinHosokei ;

    /// <summary>先端 上主筋 1段太筋本数</summary>
    private string _SentanUeSyukin1danHutokinHonsu ;

    /// <summary>先端 上主筋 1段細筋本数</summary>
    private string _SentanUeSyukin1danHosokinHonsu ;

    /// <summary>先端 上主筋 2段太筋本数</summary>
    private string _SentanUeSyukin2danHutokinHonsu ;

    /// <summary>先端 上主筋 2段細筋本数</summary>
    private string _SentanUeSyukin2danHosokinHonsu ;

    /// <summary>先端 上主筋 3段太筋本数</summary>
    private string _SentanUeSyukin3danHutokinHonsu ;

    /// <summary>先端 上主筋 3段細筋本数</summary>
    private string _SentanUeSyukin3danHosokinHonsu ;

    /// <summary>先端 下主筋太径</summary>
    private string _SentanSitaSyukinHutokei ;

    /// <summary>先端 下主筋細径</summary>
    private string _SentanSitaSyukinHosokei ;

    /// <summary>先端 下主筋 1段太筋本数</summary>
    private string _SentanSitaSyukin1danHutokinHonsu ;

    /// <summary>先端 下主筋 1段細筋本数</summary>
    private string _SentanSitaSyukin1danHosokinHonsu ;

    /// <summary>先端 下主筋 2段太筋本数</summary>
    private string _SentanSitaSyukin2danHutokinHonsu ;

    /// <summary>先端 下主筋 2段細筋本数</summary>
    private string _SentanSitaSyukin2danHosokinHonsu ;

    /// <summary>先端 下主筋 3段太筋本数</summary>
    private string _SentanSitaSyukin3danHutokinHonsu ;

    /// <summary>先端 下主筋 3段細筋本数</summary>
    private string _SentanSitaSyukin3danHosokinHonsu ;

    /// <summary>元端 肋筋径</summary>
    private string _MototanAbarakinkei ;

    /// <summary>先端 肋筋径</summary>
    private string _SentanAbarakinkei ;

    /// <summary>元端 肋筋本数</summary>
    private string _MototanAbarakinHonsu ;

    /// <summary>先端 肋筋本数</summary>
    private string _SentanAbarakinHonsu ;

    /// <summary>元端 肋筋ピッチ</summary>
    private string _MototanAbarakinPitch ;

    /// <summary>先端 肋筋ピッチ</summary>
    private string _SentanAbarakinPitch ;

    /// <summary>元端 腹筋径</summary>
    private string _MototanHarakinkei ;

    /// <summary>先端 腹筋径</summary>
    private string _SentanHarakinkei ;

    /// <summary>元端 腹筋本数</summary>
    private string _MototanHarakinHonsu ;

    /// <summary>先端 腹筋本数</summary>
    private string _SentanHarakinHonsu ;

    /// <summary>元端 幅止筋径</summary>
    private string _MototanHabadomekinkei ;

    /// <summary>先端 幅止筋径</summary>
    private string _SentanHabadomekinkei ;

    /// <summary>元端 幅止筋本数</summary>
    private string _MototanHabadomekinHonsu ;

    /// <summary>先端 幅止筋本数</summary>
    private string _SentanHabadomekinHonsu ;

    /// <summary>元端 幅止筋ピッチ</summary>
    private string _MototanHabadomekinPitch ;

    /// <summary>先端 幅止筋ピッチ</summary>
    private string _SentanHabadomekinPitch ;

    /// <summary>主筋種別 - 片持ち梁</summary>
    private string _SyukinSyubetu_Katamoti ;

    /// <summary>レベルソート順序</summary>
    private string _LevelSortOrder ;

    #endregion Member Variables

    // コンストラクタ

    #region Constructor

    /// ================================================================================
    /// <summary>コンストラクタ</summary>
    ///
    /// <param name="cmpAttribute"  >属性</param>
    /// <param name="rvtUIDoc">Revit UIドキュメント</p></param>
    ///
    /// <history>2013/03/29 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Parameters( SectionListRC.Components.Attribute cmpAttribute, Revit.UI.UIDocument rvtUIDoc ) : base( rvtUIDoc )
    {
      _CmpAttribute = cmpAttribute ;

      // デフォルト共有パラメータ
      _ShParamDefaultFileName = null ;
      Revit.DB.DefinitionFile defFile = base.GetSharedParameterFile() ;
      if ( defFile != null ) {
        _ShParamDefaultFileName = defFile.Filename ;
      }

      // アプリケーション用共有パラメータ
      _ShParamFolderName = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) ; //  _CmpAttribute.DataFolder;
      _ShParamFileName = _CmpAttribute.ResourceText( "IDS_SHPARAM_FILE" ) ;
      _ShParamGroupName = _CmpAttribute.ResourceText( "IDS_SHPARAM_GROUP" ) ;

      //if (_ShParamFileName == null)
      //{
      //  _ShParamFileName  = "GSAI_ExtRST_SectionList.txt";
      //  _ShParamGroupName = "GSAIExtRST";
      //}

      if ( _ShParamDefaultFileName == null ) {
        _ShParamDefaultFileName = _ShParamFolderName + "\\" + _ShParamFileName ;
      }
    }

    #endregion Constructor

    // メンバ関数

    #region Member Functions

    /// ================================================================================
    /// <summary>標準共有パラメータファイル設定</summary>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2012/04/5 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public bool SetSharedParamDefault()
    {
      bool ret = false ;

      // 共有パラメータファイル設定
      Revit.DB.DefinitionFile defFile = base.SetSharedParameterFile( null, _ShParamDefaultFileName ) ;
      if ( defFile != null ) {
        ret = true ;
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>定義設定</summary>
    ///
    /// <param name="elem"          >要素</param>
    /// <param name="categories"    >カテゴリ</param>
    /// <param name="defName"       >定義名</param>
    /// <param name="paramType"     >パラメータタイプ</param>
    /// <param name="bltParamGroup" >組込パラメータグループ</param>
    /// <param name="visible"       >可視</param>
    /// <param name="bindingMode"   ><p>結合モード</p>
    ///                                 <p>0 = インスタンス</p>
    ///                                 <p>1 = タイプ</p></param>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2013/04/05 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public bool SetDefinition( Revit.DB.Element elem, Collections.Generic.IList<Revit.DB.Category> categories, string defName, Revit.DB.ForgeTypeId paramType, ForgeTypeId bltParamGroup, bool visible, int bindingMode )
    {
      bool ret = base.SetDefinition( elem, _ShParamFolderName, _ShParamFileName, _ShParamGroupName, categories, defName, paramType, bltParamGroup, visible, bindingMode ) ;
      return ret ;
    }

    /// ================================================================================
    /// <summary>定義設定(オーバーロード)</summary>
    ///
    /// <param name="elem"          >要素</param>
    /// <param name="category"      >カテゴリ</param>
    /// <param name="defName"       >定義名</param>
    /// <param name="paramType"     >パラメータタイプ</param>
    /// <param name="bltParamGroup" >組込パラメータグループ</param>
    /// <param name="visible"       >可視</param>
    /// <param name="bindingMode"   ><p>結合モード</p>
    ///                                 <p>0 = インスタンス</p>
    ///                                 <p>1 = タイプ</p></param>
    ///
    /// <returns><p>結果</p>
    ///             <p>True  = 成功</p>
    ///             <p>False = 失敗</p></returns>
    ///
    /// <history>2013/04/05 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public bool SetDefinition( Revit.DB.Element elem, Revit.DB.Category category, string defName, Revit.DB.ForgeTypeId paramType, Revit.DB.ForgeTypeId bltParamGroup, bool visible, int bindingMode )
    {
      Collections.Generic.IList<Revit.DB.Category> categories = new Collections.Generic.List<Revit.DB.Category>() ;
      categories.Add( category ) ;
      return SetDefinition( elem, categories, defName, paramType, bltParamGroup, visible, bindingMode ) ;
    }

    /// ================================================================================
    /// <summary>共有パラメータ文字列取得</summary>
    /// <history>2013/04/05 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void GetValueString( Revit.DB.Element elem, string defName, Revit.DB.ForgeTypeId paramType, Revit.DB.ForgeTypeId bltParamGroup, ref string sValue )
    {
      sValue = "" ;
      if ( elem == null ) {
        return ;
      }
      foreach ( Revit.DB.Parameter p in elem.Parameters ) {
        if ( p.Definition?.Name != defName ) {
          continue ;
        }
        if ( p.Definition.GetDataType() != paramType ) {
          continue ;
        }
        if ( bltParamGroup.TypeId != string.Empty && p.Definition.GetGroupTypeId() != bltParamGroup ) {
          continue ;
        }
        sValue = p.AsString() ?? "" ;
        return ;
      }
    }

    /// ================================================================================
    /// <summary>設定ファイル名取得</summary>
    ///
    /// <history>2013/04/05 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void GetStrVal( ref string settingFileName, ref string settingFileDirectory, ref string levelSortOrder )
    {
      settingFileName = _SettingFileName ;
      settingFileDirectory = _SettingFileDirectory ;
      levelSortOrder = _LevelSortOrder ;
    }

    /// ================================================================================
    /// <summary>設定ファイル名取得</summary>
    ///
    /// <history>2013/04/05 Created  GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void GetStrVal( string settingFileName, string settingFileDirectory, string levelSortOrder )
    {
      _SettingFileName = settingFileName ;
      _SettingFileDirectory = settingFileDirectory ;
      _LevelSortOrder = levelSortOrder ;
    }

    /// ================================================================================
    /// <summary>設定値取得</summary>
    ///
    /// <history><p>2013/04/10 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string[] GetSettingValue()
    {
      string settingFileName = "" ;
      string settingFileDirectory = "" ;
      string levelSortOrder = "" ;

      GetStrVal( ref settingFileName, ref settingFileDirectory, ref levelSortOrder ) ;

      string full = settingFileDirectory + settingFileName ;

      if ( ! System.IO.File.Exists( full ) ) {
        full = System.IO.Path.GetDirectoryName( System.Reflection.Assembly.GetExecutingAssembly().Location ) + "\\" + "SettingFile.txt" ;
      }

      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "shift_jis" ) ;

      return System.IO.File.ReadAllLines( full, enc ) ;
    }

    /// ================================================================================
    /// <summary>設定値取得</summary>
    ///
    /// <param name="settingFileName">設定ファイルの名前</param>
    /// <param name="settingFilePath">設定ファイルの場所</param>
    ///
    /// <history><p>2013/04/12 Created  GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public string[] GetSettingValue( string settingFileName, string settingFilePath )
    {
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "shift_jis" ) ;

      string[] ret = null ;

      if ( System.IO.File.Exists( settingFilePath + settingFileName ) ) {
        ret = System.IO.File.ReadAllLines( settingFilePath + settingFileName, enc ) ;
      }
      else {
        ret = DefaultSettingParameter ; // DefaultParameter();
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>設定値取得</summary>
    ///
    /// <param name="dataAry">設定値</param>
    ///
    /// <history><p>2013/04/10 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/05/29 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public void GetSettingValue( string[] stringAry )
    {
      _ColumnListViewScale = stringAry[ 0 ] ;
      _BeamListViewScale = stringAry[ 1 ] ;
      _TitleFont = stringAry[ 2 ] ;
      _ItemFont = stringAry[ 3 ] ;
      _DimensionType = stringAry[ 4 ] ;
      _FrameLineType = stringAry[ 5 ] ;
      _BodyLineType = stringAry[ 6 ] ;
      _SpacerLineType = stringAry[ 7 ] ;
      _LevelFrameShow = stringAry[ 8 ] ;
      _LevelFrameWidth = stringAry[ 9 ] ;
      _ItemFrameWidth = stringAry[ 10 ] ;
      _SymbolFrameHeight = stringAry[ 11 ] ;
      _ArrangementFrameHeight = stringAry[ 12 ] ;
      _ItemFrameWidth2 = stringAry[ 13 ] ;
      _SymbolFrameHeight2 = stringAry[ 14 ] ;
      _ArrangementFrameHeight2 = stringAry[ 15 ] ;
      _LevelFrameTitle = stringAry[ 16 ] ;
      _LevelFrameEndWord = stringAry[ 17 ] ;
      _SymbolFrameTitle = stringAry[ 18 ] ;
      _SelectTable = stringAry[ 19 ] ;
      _PickTable = stringAry[ 20 ] ;

      _ColumnLeftSpace = stringAry[ 22 ] ;
      _ColumnRightSpace = stringAry[ 23 ] ;
      _ColumnTopSpace = stringAry[ 24 ] ;
      _ColumnBottomSpace = stringAry[ 25 ] ;
      _HoopBracketShow = stringAry[ 26 ] ;
      _ColumnAddFrameNumber = stringAry[ 27 ] ;
      _ColumnRebarShow = stringAry[ 28 ] ;
      _HoopFrameTitle = stringAry[ 29 ] ;
      _HoopFrameSpaceSymbol = stringAry[ 30 ] ;

      _BeamLeftSpace = stringAry[ 32 ] ;
      _BeamRightSpace = stringAry[ 33 ] ;
      _BeamCenterSpaceType = stringAry[ 34 ] ;
      _BeamCenterSpace = stringAry[ 35 ] ;
      _BeamTopSpace = stringAry[ 36 ] ;
      _BeamBottomSpace = stringAry[ 37 ] ;
      _PositionFrameHeight = stringAry[ 38 ] ;
      _StirrupBracketShow = stringAry[ 39 ] ;
      _BeamAddFrameNumber = stringAry[ 40 ] ;
      _PositionFrameTitleShow = stringAry[ 41 ] ;
      _PositionFrameSpaceLineShow = stringAry[ 42 ] ;
      _AllSectionTitle = stringAry[ 43 ] ;
      _EdgeTitle = stringAry[ 44 ] ;
      _CenterSectionTitle = stringAry[ 45 ] ;
      _ItanSectionTitle = stringAry[ 46 ] ;
      _JtanSectionTitle = stringAry[ 47 ] ;
      _CantileverStartTitle = stringAry[ 48 ] ;
      _CantileverEndTitle = stringAry[ 49 ] ;
      _StirrupFrameTitle = stringAry[ 50 ] ;
      _StirrupFrameSpaceSymbol = stringAry[ 51 ] ;

      _WidthDimensionShow = stringAry[ 53 ] ;
      _HeightDimensionShow = stringAry[ 54 ] ;
      _BeamRebarShow = stringAry[ 55 ] ;
      _StirrupFrameShow = stringAry[ 56 ] ;
      _WebFrameShow = stringAry[ 57 ] ;

      _ColumnProtectThick = stringAry[ 59 ] ;
      _2ndRebarCornerSetFlag = stringAry[ 60 ] ;
      _CylinderProtectThick = stringAry[ 61 ] ;
      _BeamProtectThick = stringAry[ 62 ] ;

      return ;
    }

    /// ================================================================================
    /// <summary>設定値取得</summary>
    ///
    /// <history>2014/05/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Collections.Generic.IList<string> GetSettingFileValue()
    {
      // 戻り値
      Collections.Generic.IList<string> ret = new Collections.Generic.List<string>() ;
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "Shift_JIS" ) ;

      // 設定ファイル
      string setting = "" ;

      // 設定情報ファイル取得
      string settingInfo = _CmpAttribute.ExecuteFolder + "\\" + "SettingFileInfo.txt" ;

      if ( ! System.IO.File.Exists( settingInfo ) ) {
        // 設定情報ファイル作成
        string def = "Default" ;
        System.IO.File.WriteAllText( settingInfo, def, enc ) ;
      }

      // 設定ファイル取得
      string[] strArry = System.IO.File.ReadAllLines( settingInfo, enc ) ;
      foreach ( string s in strArry ) {
        setting += s ;
      }

      if ( setting == "Default" ) {
        setting = _CmpAttribute.ExecuteFolder + "\\" + "SettingFile.txt" ;
      }

      if ( ! System.IO.File.Exists( setting ) ) {
        // 設定ファイル作成
        setting = _CmpAttribute.ExecuteFolder + "\\" + "SettingFile.txt" ;

        string defValue = "" ;
        foreach ( string s in DefaultSettingParameter ) {
          defValue += s + "\r\n" ;
        }

        System.IO.File.WriteAllText( setting, defValue, enc ) ;
      }

      // 設定値取得
      string[] settingAry = System.IO.File.ReadAllLines( setting, enc ) ;

      for ( int i = 0 ; i < settingAry.Length ; ++i ) {
        string s = settingAry[ i ] ;

        if ( s == null ) {
          s = "" ;
        }

        ret.Add( s ) ;
      }

      return ret ;
    }
    

    /// ================================================================================
    /// <summary>パラメータ名取得</summary>
    ///
    /// <returns>パラメータ内容とマッピング名</returns>
    ///
    /// <history><p>2014/05/20 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/06/13 Modified GSA,Inc. Ryo Kuroda</p>
    ///           <p>2014/07/15 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> GetParamName()
    {
      // 戻り値
      Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> ret = new Collections.Generic.List<Collections.Generic.IDictionary<string, string>>() ;

      // テーブルパス
      string path = TableFilePath ;

      // 存在確認
      if ( ! System.IO.File.Exists( path ) ) {
        return ret ;
      }
      Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
      System.Text.Encoding enc = System.Text.Encoding.GetEncoding( "shift_jis" ) ;

      // パラメータ値取得
      string[] strAry = null ;

      try {
        strAry = System.IO.File.ReadAllLines( path, enc ) ;
      }
      catch ( System.IO.IOException ) {
        System.Windows.Forms.MessageBox.Show( _CmpAttribute.ResourceText( "IDS_ERR_GETTABLEVALUE" ) ) ;

        return ret ;
      }

      // 矩形柱ヘッダ
      string rectangleColumn = _CmpAttribute.ResourceText( "IDS_TXT_HEADER_RCCOLUMN" ) ;
      // 円柱ヘッダ
      string roundColumn = _CmpAttribute.ResourceText( "IDS_TXT_HEADER_RCROUNDCOLUMN" ) ;
      // 梁ヘッダ
      string girder = _CmpAttribute.ResourceText( "IDS_TXT_HEADER_RCGIRDER" ) ;
      // 片持ち梁ヘッダ
      string cantiGirder = _CmpAttribute.ResourceText( "IDS_TXT_HEADER_RCCG" ) ;

      // パラメータ内容 - マッピングパラメータ名 対応
      Collections.Generic.IDictionary<string, string> dic = new Collections.Generic.Dictionary<string, string>() ;

      bool isColumn = false ;
      bool isRound = false ;
      bool isGirder = false ;
      bool isCanti = false ;

      foreach ( string str in strAry ) {
        // 値なし
        if ( string.IsNullOrEmpty( str ) || str == "" ) {
          isColumn = false ;
          isRound = false ;
          isGirder = false ;
          isCanti = false ;

          if ( dic.Count > 0 ) {
            ret.Add( dic ) ;

            dic = new Collections.Generic.Dictionary<string, string>() ;
          }

          continue ;
        }

        // 分割した文字
        Collections.Generic.IList<string> strs = new Collections.Generic.List<string>() ;

        // 区切り文字
        string separetor = " : " ;

        if ( str.Contains( separetor ) ) {
          // 分割
          strs = SectionListRC.JExtComCompat.UtilValue.SplitString( str, separetor ) ;
        }
        else {
          // そのまま
          strs.Add( str ) ;
        }

        if ( strs.Count == 1 ) {
          // ヘッダ判定
          if ( strs[ 0 ] == rectangleColumn ) {
            isColumn = true ;

            dic.Add( "カテゴリ", rectangleColumn ) ;
          }
          else if ( strs[ 0 ] == roundColumn ) {
            isRound = true ;

            dic.Add( "カテゴリ", roundColumn ) ;
          }
          else if ( strs[ 0 ] == girder ) {
            isGirder = true ;

            dic.Add( "カテゴリ", girder ) ;
          }
          else if ( strs[ 0 ] == cantiGirder ) {
            isCanti = true ;

            dic.Add( "カテゴリ", cantiGirder ) ;
          }
        }
        else if ( strs.Count >= 3 ) {
          // パラメータ取得
          if ( isColumn || isRound || isGirder || isCanti ) {
            dic.Add( strs[ 1 ], strs[ 2 ] ) ;
          }
        }
      }

      return ret ;
    }

    /// ================================================================================
    /// <summary>パラメータ名取得</summary>
    ///
    /// <param name="rectangleColumn" >矩形柱</param>
    /// <param name="roundColumn"     >円柱</param>
    /// <param name="girder"          >梁</param>
    /// <param name="cantiGirder"     >片持ち梁</param>
    ///
    /// <history>2014/05/29 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void GetColumnParamName( ref Collections.Generic.IDictionary<string, string> rectangleColumn, ref Collections.Generic.IDictionary<string, string> roundColumn, ref Collections.Generic.IDictionary<string, string> girder, ref Collections.Generic.IDictionary<string, string> cantiGirder )
    {
      Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> allParamName = GetParamName() ;

      foreach ( Collections.Generic.IDictionary<string, string> dic in allParamName ) {
        // 矩形柱ヘッダ
        string kakuHeader = _CmpAttribute.ResourceText( "IDS_TXT_HEADER_RCCOLUMN" ) ;
        // 円柱ヘッダ
        string enHeader = _CmpAttribute.ResourceText( "IDS_TXT_HEADER_RCROUNDCOLUMN" ) ;

        // 梁ヘッダ
        string hariHeader = _CmpAttribute.ResourceText( "IDS_TXT_HEADER_RCGIRDER" ) ;
        // 片持ち梁ヘッダ
        string katamotiHeader = _CmpAttribute.ResourceText( "IDS_TXT_HEADER_RCCG" ) ;

        // キーがカテゴリのときの値をヘッダと比較
        if ( dic[ _CmpAttribute.ResourceText( "IDS_TXT_CATEGORY" ) ] == kakuHeader ) {
          rectangleColumn = dic ;
        }

        if ( dic[ _CmpAttribute.ResourceText( "IDS_TXT_CATEGORY" ) ] == enHeader ) {
          roundColumn = dic ;
        }

        if ( dic[ _CmpAttribute.ResourceText( "IDS_TXT_CATEGORY" ) ] == hariHeader ) {
          girder = dic ;
        }

        if ( dic[ _CmpAttribute.ResourceText( "IDS_TXT_CATEGORY" ) ] == katamotiHeader ) {
          cantiGirder = dic ;
        }
      }
    }

    /// ================================================================================
    /// <summary>設定値取得</summary>
    ///
    /// <param name="rectangleColumn" >矩形柱</param>
    /// <param name="roundColumn"     >円柱</param>
    /// <param name="girder"          >梁</param>
    /// <param name="cantiGirder"     >片持ち梁</param>
    ///
    /// <history>2014/05/29 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public void GetParameterValue( Collections.Generic.IDictionary<string, string> rectangleColumn, Collections.Generic.IDictionary<string, string> roundColumn, Collections.Generic.IDictionary<string, string> girder, Collections.Generic.IDictionary<string, string> cantiGirder )
    {
      #region 矩形柱

      _HashiraBunrui_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HASHIRABUNRUI" ) ] ;
      _HashiraHaba_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HASIRAHABA" ) ] ;
      _HashiraSei_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HASIRASEI" ) ] ;
      _Hugo_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HUGO" ) ] ;
      _ChutoSyukinHutokei_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _ChutoSyukinHosokei_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _ChutoSyukinX1danHutokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChutoSyukinX1danHosokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChutoSyukinX2danHutokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChutoSyukinX2danHosokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChutoSyukinY1danHutokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChutoSyukinY1danHosokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChutoSyukinY2danHutokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChutoSyukinY2danHosokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChukyakuSyukinHutokei_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _ChukyakuSyukinHosokei_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _ChukyakuSyukinX1danHutokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChukyakuSyukinX1danHosokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChukyakuSyukinX2danHutokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChukyakuSyukinX2danHosokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChukyakuSyukinY1danHutokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChukyakuSyukinY1danHosokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChukyakuSyukinY2danHutokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChukyakuSyukinY2danHosokinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChutoObikinKei_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChutoObikinXHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChutoObikinYHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChutoObikinPitch_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _ChukyakuObikinKei_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChukyakuObikinXHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChukyakuObikinYHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChukyakuObikinPitch_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _HabadomekinKei_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _HabadomekinPitch_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _ChutoHabadomekinXHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChutoHabadomekinYHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChukyakuHabadomekinXHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChukyakuHabadomekinYHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SinTekkinKei_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SINTEKKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SinTekkinHonsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SINTEKKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SinTekkinXHoukouKyori_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SINTEKKINXKYORI" ) ] ;
      _SinTekkinYHoukouKyuori_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SINTEKKINYKYORI" ) ] ;
      _ConcreteTeigenritsu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CONCRETETEIGENRITU" ) ] ;
      _SyukinSyubetuX_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_SYUBETU" ) + "X" ] ;
      _SyukinSyubetuY_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_SYUBETU" ) + "Y" ] ;
      _SinTekkinSyubetu_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SINTEKKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_SYUBETU" ) ] ;
      _ChutoYosekinHoukou_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_YOSEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOUKOU" ) ] ;
      _ChukyakuYosekinHoukou_Kaku = rectangleColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_YOSEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOUKOU" ) ] ;

      #endregion 矩形柱

      #region 円柱

      _HashiraSyubetsu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HASIRASYUBETU" ) ] ;
      _Tyokkei_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_TYOKKEI" ) ] ;
      _Hugo_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HUGO" ) ] ;
      _ChutoSyukinKei_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChutoSyukinHonsu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChukyakuSyukinKei_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChukyakuSyukinHonsu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChutoObikinKei_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChutoObikinPitch_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _ChukyakuObikinKei_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChukyakuObikinPitch_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_OBIKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _HabadomekinPitch_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _HabadomekinKei_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChutoHabadomekinXHonsu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChutoHabadomekinYHonsu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUTO" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChukyakuHabadomekinXHonsu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_XHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChukyakuHabadomekinYHonsu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CHUKYAKU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_YHOUKOU" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SinTekkinKei_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SINTEKKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SinTekkinHonsu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SINTEKKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SinTekkinKyori_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SINTEKKINKYORI" ) ] ;
      _ConcreteTeigenritu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_CONCRETETEIGENRITU" ) ] ;
      _SyukinSyubetu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_SYUBETU" ) ] ;
      _SinTekkinSyubetu_En = roundColumn[ _CmpAttribute.ResourceText( "IDS_TXT_SINTEKKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_SYUBETU" ) ] ;

      #endregion 円柱

      #region 梁

      _HariSyubetu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_HARISYUBETU" ) ] ;
      _ShitanHarihaba = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARIHABA" ) ] ;
      _ChuouHarihaba = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARIHABA" ) ] ;
      _SyutanHarihaba = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARIHABA" ) ] ;
      _ShitanHarisei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARISEI" ) ] ;
      _ChuouHarisei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARISEI" ) ] ;
      _SyutanHarisei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARISEI" ) ] ;
      _Hugo_Hari = girder[ _CmpAttribute.ResourceText( "IDS_TXT_HUGO" ) ] ;
      _SitanUeSyukinHutokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _SitanUeSyukinHosokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _SitanUeSyukin1danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SitanUeSyukin1danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SitanUeSyukin2danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SitanUeSyukin2danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SitanUeSyukin3danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SitanUeSyukin3danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SitanSitaSyukinHutokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _SitanSitaSyukinHosokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _SitanSitaSyukin1danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SitanSitaSyukin1danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SitanSitaSyukin2danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SitanSitaSyukin2danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SitanSitaSyukin3danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SitanSitaSyukin3danHosokeiHosnu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChuouUeSyukinHutokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _ChuouUeSyukinHosokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _ChuouUeSyukin1danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChuouUeSyukin1danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChuouUeSyukin2danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChuouUeSyukin2danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChuouUeSyukin3danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChuouUeSyukin3danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChuouSitaSyukinHutokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _ChuouSitaSyukinHosokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _ChuouSitaSyukin1danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChuouSitaSyukin1danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChuouSitaSyukin2danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChuouSitaSyukin2danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _ChuouSitaSyukin3danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _ChuouSitaSyukin3danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SyutanUeSyukinHutokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _SyutanUeSyukinHosokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _SyutanUeSyukin1danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SyutanUeSyukin1danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SyutanUeSyukin2danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SyutanUeSyukin2danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SyutanUeSyukin3danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SyutanUeSyukin3danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SyutanSitaSyukinHutokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _SyutanSitaSyukinHosokei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _SyutanSitaSyukin1danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SyutanSitaSyukin1danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_1DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SyutanSitaSyukin2danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SyutanSitaSyukin2danHosokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_2DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SyutanSitaSyukin3danHutokeiHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SyutanSitaSyukin3danHosokeiHosnu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_3DANKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SitanAbarakinkei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChuouAbarakinkei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SyutanAbarakinkei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SitanAbarakinHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChuouAbarakinHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SyutanAbarakinHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SitanAbarakinPitch = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _ChuouAbarakinPitch = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _SyutanAbarakinPitch = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _SitanHarakinkei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChuouHarakinkei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SyutanHarakinkei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SitanHarakinHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChuouHarakinHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SyutanHarakinHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SitanHabadomekinkei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _ChuouHabadomekinkei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SyutanHabadomekinkei = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SitanHabadomekinHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _ChuouHabadomekinHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SyutanHabadomekinHonsu = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SitanHabadomekinPitch = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _ChuouHabadomekinPitch = girder[ _CmpAttribute.ResourceText( "IDS_TXT_CHUOU" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _SyutanHabadomekinPitch = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _ShitanBoxHaunch = girder[ _CmpAttribute.ResourceText( "IDS_TXT_BOXHAUNCH" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) ] ;
      _SyutanBoxHaunch = girder[ _CmpAttribute.ResourceText( "IDS_TXT_BOXHAUNCH" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) ] ;
      _ShitanHaunchNagasa = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SITAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HAUNCHNAGASA" ) ] ;
      _SyutanHaunchNagasa = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HAUNCHNAGASA" ) ] ;
      _SyukinSyubetu_Hari = girder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_SYUBETU" ) ] ;

      #endregion 梁

      #region 片持ち梁

      _HariSyubetu_Katamoti = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_HARISYUBETU" ) ] ;
      _MototanHarihaba = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARIHABA" ) ] ;
      _SentanHarihaba = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARIHABA" ) ] ;
      _MototanHariSei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARISEI" ) ] ;
      _SentanHarisei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARISEI" ) ] ;
      _Hugo_Katamoti = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_HUGO" ) ] ;
      _MototanUeSyukinHutokei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _MototanUeSyukinHosokei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _MototanUeSyukin1danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _MototanUeSyukin1danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _MototanUeSyukin2danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _MototanUeSyukin2danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _MototanUeSyukin3danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_3DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _MototanUeSyukin3danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_3DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _MototanSitaSyukinHutokei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _MototanSitaSyukinHosokei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _MototanSitaSyukin1danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _MototanSitaSyukin1danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _MototanSitaSyukin2danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _MototanSitaSyukin2danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _MototanSitaSyukin3danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_3DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _MototanSitaSyukin3danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_3DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SentanUeSyukinHutokei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _SentanUeSyukinHosokei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _SentanUeSyukin1danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SentanUeSyukin1danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SentanUeSyukin2danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SentanUeSyukin2danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SentanUeSyukin3danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_3DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SentanUeSyukin3danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_UESYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_3DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SentanSitaSyukinHutokei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKEI" ) ] ;
      _SentanSitaSyukinHosokei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKEI" ) ] ;
      _SentanSitaSyukin1danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SentanSitaSyukin1danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_1DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SentanSitaSyukin2danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SentanSitaSyukin2danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_2DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _SentanSitaSyukin3danHutokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_3DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HUTOKINHONSU" ) ] ;
      _SentanSitaSyukin3danHosokinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_SITASYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_3DAN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HOSOKINHONSU" ) ] ;
      _MototanAbarakinkei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SentanAbarakinkei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _MototanAbarakinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SentanAbarakinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _MototanAbarakinPitch = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _SentanAbarakinPitch = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_ABARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _MototanHarakinkei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SentanHarakinkei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _MototanHarakinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SentanHarakinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HARAKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _MototanHabadomekinkei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _SentanHabadomekinkei = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_KEI" ) ] ;
      _MototanHabadomekinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _SentanHabadomekinHonsu = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_HONSU" ) ] ;
      _MototanHabadomekinPitch = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_MOTOTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _SentanHabadomekinPitch = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SENTAN" ) + " " + _CmpAttribute.ResourceText( "IDS_TXT_HABADOMEKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_PITCH" ) ] ;
      _SyukinSyubetu_Katamoti = cantiGirder[ _CmpAttribute.ResourceText( "IDS_TXT_SYUKIN" ) + _CmpAttribute.ResourceText( "IDS_TXT_SYUBETU" ) ] ;

      #endregion 片持ち梁
    }

    /// ================================================================================
    /// <summary>10のdigits乗を四捨五入</summary>
    /// ================================================================================
    public double ToHalfAdjust( double value, int digits )
    {
      digits *= -1 ;

      double dCoef = System.Math.Pow( 10, digits ) ;

      return value > 0 ? System.Math.Floor( ( value * dCoef ) + 0.5 ) / dCoef : System.Math.Ceiling( ( value * dCoef ) - 0.5 ) / dCoef ;
    }

    /// ================================================================================
    /// <summary>予備ファイルのコピー</summary>
    ///
    /// <history><p>2014/06/16 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2017/06/19 Modified GSA,Inc. Ryo Kuroda</p></history>
    /// ================================================================================
    public void ReserveFileCopy()
    {
      // 実行フォルダ
      string reservePath = _CmpAttribute.ExecuteFolder ;

      // マイドキュメント
      string myDoc = System.Environment.GetFolderPath( System.Environment.SpecialFolder.MyDocuments ) ;


      
      var version = _CmpAttribute.ResourceText( "IDS_TXT_REVITVERSION_2027" ) ;
      
      // SS3 Linkとの共通フォルダ
      string shareFolderPath = myDoc + "\\Autodesk REXJ\\" + version + "\\" ;
      // マッピングパラメータファイル
      string mapParamFile = _CmpAttribute.ResourceText( $"IDS_TXT_PARAMETERFILE_NAME_{version}" ) ;
      // テーブルファイル
      string tableFile = _CmpAttribute.ResourceText( $"IDS_TXT_TABLEFILE_{version}" ) ;

      // 共有パラメータファイル
      string shareFile = _CmpAttribute.ResourceText( "IDS_TXT_SHAREFILE" ) ;

      // 共有パラメータファイル - オリジナル
      string shareFileOrg = _CmpAttribute.ResourceText( "IDS_TXT_SHAREFILE_ORG" ) ;

      // フォルダ存在確認
      if ( System.IO.Directory.Exists( shareFolderPath ) ) {
        // テーブルなしまたはデフォルト
        if ( ! System.IO.File.Exists( _SelectTable ) || _PickTable == "0" ) {
          if ( System.IO.File.Exists( reservePath + "\\" + tableFile ) ) {
            if ( ! System.IO.File.Exists( shareFolderPath + "\\" + tableFile ) ) {
              System.IO.File.Copy( reservePath + "\\" + tableFile, shareFolderPath + "\\" + tableFile ) ;

              _SelectTable = shareFolderPath + "\\" + tableFile ;
            }
          }
        }

        // マッピングファイルなし
        if ( ! System.IO.File.Exists( shareFolderPath + "\\" + mapParamFile ) ) {
          if ( System.IO.File.Exists( reservePath + "\\" + mapParamFile ) ) {
            System.IO.File.Copy( reservePath + "\\" + mapParamFile, shareFolderPath + "\\" + mapParamFile ) ;
          }
        }

        // 共有パラメータファイルなし
        if ( ! System.IO.File.Exists( shareFolderPath + "\\" + shareFile ) ) {
          if ( System.IO.File.Exists( reservePath + "\\" + shareFile ) ) {
            System.IO.File.Copy( reservePath + "\\" + shareFile, shareFolderPath + "\\" + shareFile ) ;
          }
        }

        // 共有パラメータオリジナルファイルなし
        if ( ! System.IO.File.Exists( shareFolderPath + "\\" + shareFileOrg ) ) {
          if ( System.IO.File.Exists( reservePath + "\\" + shareFileOrg ) ) {
            System.IO.File.Copy( reservePath + "\\" + shareFileOrg, shareFolderPath + "\\" + shareFileOrg ) ;
          }
        }
      }
      // フォルダがない場合
      else {
        try {
          // 「Autodesk REXJ」フォルダ作成
          if ( ! System.IO.Directory.Exists( myDoc + "\\Autodesk REXJ" ) ) {
            System.IO.Directory.CreateDirectory( myDoc + "\\Autodesk REXJ" ) ;
          }

          // 「2017」フォルダ作成
          System.IO.Directory.CreateDirectory( shareFolderPath ) ;

          // フォルダがないならファイルもない

          // テーブルコピー
          if ( System.IO.File.Exists( reservePath + "\\" + tableFile ) ) {
            System.IO.File.Copy( reservePath + "\\" + tableFile, shareFolderPath + "\\" + tableFile ) ;
          }

          // マッピングファイルコピー
          if ( System.IO.File.Exists( reservePath + "\\" + mapParamFile ) ) {
            System.IO.File.Copy( reservePath + "\\" + mapParamFile, shareFolderPath + "\\" + mapParamFile ) ;
          }

          // 共有パラメータファイルコピー
          if ( System.IO.File.Exists( reservePath + "\\" + shareFile ) ) {
            System.IO.File.Copy( reservePath + "\\" + shareFile, shareFolderPath + "\\" + shareFile ) ;
          }

          // 共有パラメータオリジナルファイルコピー
          if ( System.IO.File.Exists( reservePath + "\\" + shareFileOrg ) ) {
            System.IO.File.Copy( reservePath + "\\" + shareFileOrg, shareFolderPath + "\\" + shareFileOrg ) ;
          }
        }
        catch {
        }
      }

      return ;
    }

    #endregion Member Functions

    // プロパティ

    #region Properties

    /// ================================================================================
    /// <summary>テーブルファイルパス</summary>
    /// <history><p>2014/06/13 Created GSA,Inc. Ryo Kuroda</p>
    ///           <p>2017/06/16 Modified CST,Co.Ltd Ryo Kuroda</p></history>
    /// ================================================================================
    public string TableFilePath
    {
      get
      {
        var revitVersion = _CmpAttribute.ResourceText( "IDS_TXT_REVITVERSION_2027" ) ;
        var tableFile = _CmpAttribute.ResourceText( "IDS_TXT_TABLEFILE_2027" ) ;
        
        string ret = System.Environment.GetFolderPath( System.Environment.SpecialFolder.MyDocuments ) + "\\Autodesk REXJ\\" + revitVersion + "\\" + tableFile ;

        if ( System.IO.File.Exists( _SelectTable ) ) {
          ret = _SelectTable ;
        }

        if ( _PickTable == "0" ) {
          ret = System.Environment.GetFolderPath( System.Environment.SpecialFolder.MyDocuments ) + "\\Autodesk REXJ\\" + revitVersion + "\\" + tableFile ;
        }

        return ret ;
      }
    }

    /// ================================================================================
    /// <summary>設定値 - 柱リストビュー尺度</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ColumnListViewScale
    {
      get { return _ColumnListViewScale ; }
      set { _ColumnListViewScale = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 梁リストビュー尺度</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamListViewScale
    {
      get { return _BeamListViewScale ; }
      set { _BeamListViewScale = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - タイトルフォント</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string TitleFont
    {
      get { return _TitleFont ; }
      set { _TitleFont = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 小項目フォント</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ItemFont
    {
      get { return _ItemFont ; }
      set { _ItemFont = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 寸法線タイプ</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string DimensionType
    {
      get { return _DimensionType ; }
      set { _DimensionType = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 枠線種タイプ</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string FrameLineType
    {
      get { return _FrameLineType ; }
      set { _FrameLineType = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 躯体線種タイプ</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BodyLineType
    {
      get { return _BodyLineType ; }
      set { _BodyLineType = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 幅止筋線種タイプ</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SpacerLineType
    {
      get { return _SpacerLineType ; }
      set { _SpacerLineType = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 階表示枠表示</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string LevelFrameShow
    {
      get { return _LevelFrameShow ; }
      set { _LevelFrameShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 階表示枠幅</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string LevelFrameWidth
    {
      get { return _LevelFrameWidth ; }
      set { _LevelFrameWidth = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 項目表示枠幅</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ItemFrameWidth
    {
      get { return _ItemFrameWidth ; }
      set { _ItemFrameWidth = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 符号表示枠高さ</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SymbolFrameHeight
    {
      get { return _SymbolFrameHeight ; }
      set { _SymbolFrameHeight = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 配筋枠高さ</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ArrangementFrameHeight
    {
      get { return _ArrangementFrameHeight ; }
      set { _ArrangementFrameHeight = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 項目表示枠幅2</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ItemFrameWidth2
    {
      get { return _ItemFrameWidth2 ; }
      set { _ItemFrameWidth2 = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 符号表示枠高さ2</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SymbolFrameHeight2
    {
      get { return _SymbolFrameHeight2 ; }
      set { _SymbolFrameHeight2 = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 配筋枠高さ2</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ArrangementFrameHeight2
    {
      get { return _ArrangementFrameHeight2 ; }
      set { _ArrangementFrameHeight2 = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 階表示枠タイトル</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string LevelFrameTitle
    {
      get { return _LevelFrameTitle ; }
      set { _LevelFrameTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 階表示枠接尾語</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string LevelFrameEndWord
    {
      get { return _LevelFrameEndWord ; }
      set { _LevelFrameEndWord = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 符号表示枠タイトル</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SymbolFrameTitle
    {
      get { return _SymbolFrameTitle ; }
      set { _SymbolFrameTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 書き出しフォルダ</summary>
    /// <history>2014/07/09 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ExportFolder
    {
      get
      {
        if ( _SelectTable == _CmpAttribute.ResourceText( "IDS_TXT_DESKTOP" ) ) {
          return System.Environment.GetFolderPath( System.Environment.SpecialFolder.DesktopDirectory ) ;
        }
        else if ( System.IO.Directory.Exists( _SelectTable ) ) {
          return _SelectTable ;
        }
        else {
          return System.Environment.GetFolderPath( System.Environment.SpecialFolder.DesktopDirectory ) ;
        }
      }
      set { _SelectTable = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 左のあき - 柱</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ColumnLeftSpace
    {
      get { return _ColumnLeftSpace ; }
      set { _ColumnLeftSpace = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 右のあき - 柱</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ColumnRightSpace
    {
      get { return _ColumnRightSpace ; }
      set { _ColumnRightSpace = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 上のあき - 柱</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ColumnTopSpace
    {
      get { return _ColumnTopSpace ; }
      set { _ColumnTopSpace = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 下のあき - 柱</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ColumnBottomSpace
    {
      get { return _ColumnBottomSpace ; }
      set { _ColumnBottomSpace = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 帯筋括弧表示</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string HoopBracketShow
    {
      get { return _HoopBracketShow ; }
      set { _HoopBracketShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 追加枠数 - 柱</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ColumnAddFrameNumber
    {
      get { return _ColumnAddFrameNumber ; }
      set { _ColumnAddFrameNumber = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 主筋表示 - 柱</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ColumnRebarShow
    {
      get { return _ColumnRebarShow ; }
      set { _ColumnRebarShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 帯筋枠タイトル</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string HoopFrameTitle
    {
      get { return _HoopFrameTitle ; }
      set { _HoopFrameTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 帯筋枠区切り記号</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string HoopFrameSpaceSymbol
    {
      get { return _HoopFrameSpaceSymbol ; }
      set { _HoopFrameSpaceSymbol = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 左のあき - 梁</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamLeftSpace
    {
      get { return _BeamLeftSpace ; }
      set { _BeamLeftSpace = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 右のあき - 梁</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamRightSpace
    {
      get { return _BeamRightSpace ; }
      set { _BeamRightSpace = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中間あきタイプ</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamCenterSpaceType
    {
      get { return _BeamCenterSpaceType ; }
      set { _BeamCenterSpaceType = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中間のあき</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamCenterSpace
    {
      get { return _BeamCenterSpace ; }
      set { _BeamCenterSpace = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 上のあき - 梁</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamTopSpace
    {
      get { return _BeamTopSpace ; }
      set { _BeamTopSpace = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 下のあき - 梁</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamBottomSpace
    {
      get { return _BeamBottomSpace ; }
      set { _BeamBottomSpace = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 位置表示枠高さ</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string PositionFrameHeight
    {
      get { return _PositionFrameHeight ; }
      set { _PositionFrameHeight = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 肋筋括弧表示</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string StirrupBracketShow
    {
      get { return _StirrupBracketShow ; }
      set { _StirrupBracketShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 追加枠数 - 梁</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamAddFrameNumber
    {
      get { return _BeamAddFrameNumber ; }
      set { _BeamAddFrameNumber = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 位置表示枠タイトル表示</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string PositionFrameTitleShow
    {
      get { return _PositionFrameTitleShow ; }
      set { _PositionFrameTitleShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 位置表示枠区切り線表示</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string PositionFrameSpaceLineShow
    {
      get { return _PositionFrameSpaceLineShow ; }
      set { _PositionFrameSpaceLineShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 全断面タイトル</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string AllSectionTitle
    {
      get { return _AllSectionTitle ; }
      set { _AllSectionTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 端部タイトル</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string EdgeTitle
    {
      get { return _EdgeTitle ; }
      set { _EdgeTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中心部タイトル</summary>
    /// <history>2013/07/09 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string CenterSectionTitle
    {
      get { return _CenterSectionTitle ; }
      set { _CenterSectionTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 始端タイトル</summary>
    /// <history>2013/07/09 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ItanSectionTitle
    {
      get { return _ItanSectionTitle ; }
      set { _ItanSectionTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 終端タイトル</summary>
    /// <history>2013/07/09 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string JtanSectionTitle
    {
      get { return _JtanSectionTitle ; }
      set { _JtanSectionTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 片持ち梁元端タイトル</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string CantileverStartTitle
    {
      get { return _CantileverStartTitle ; }
      set { _CantileverStartTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 片持ち梁先端タイトル</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string CantileverEndTitle
    {
      get { return _CantileverEndTitle ; }
      set { _CantileverEndTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 肋筋枠タイトル</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string StirrupFrameTitle
    {
      get { return _StirrupFrameTitle ; }
      set { _StirrupFrameTitle = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 肋筋枠区切り記号</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string StirrupFrameSpaceSymbol
    {
      get { return _StirrupFrameSpaceSymbol ; }
      set { _StirrupFrameSpaceSymbol = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 幅寸法表示</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string WidthDimensionShow
    {
      get { return _WidthDimensionShow ; }
      set { _WidthDimensionShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 高さ寸法表示</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string HeightDimensionShow
    {
      get { return _HeightDimensionShow ; }
      set { _HeightDimensionShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 主筋表示 - 梁</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamRebarShow
    {
      get { return _BeamRebarShow ; }
      set { _BeamRebarShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 肋筋枠表示</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string StirrupFrameShow
    {
      get { return _StirrupFrameShow ; }
      set { _StirrupFrameShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 腹筋枠表示</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string WebFrameShow
    {
      get { return _WebFrameShow ; }
      set { _WebFrameShow = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - かぶり厚 - 角柱</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ColumnProtectThick
    {
      get { return _ColumnProtectThick ; }
      set { _ColumnProtectThick = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 2段筋コーナー配筋フラグ</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SecondRebarCornerSetFlag
    {
      get { return _2ndRebarCornerSetFlag ; }
      set { _2ndRebarCornerSetFlag = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - かぶり厚 - 円柱</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string CylinderProtectThick
    {
      get { return _CylinderProtectThick ; }
      set { _CylinderProtectThick = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - かぶり厚 - 梁</summary>
    /// <history>2013/04/11 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BeamProtectThick
    {
      get { return _BeamProtectThick ; }
      set { _BeamProtectThick = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 柱分類 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string HashiraBunrui_Kaku
    {
      get { return _HashiraBunrui_Kaku ; }
      set { _HashiraBunrui_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 柱幅 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string DX_Kaku
    {
      get { return _HashiraHaba_Kaku ; }
      set { _HashiraHaba_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 柱成 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string DY_Kaku
    {
      get { return _HashiraSei_Kaku ; }
      set { _HashiraSei_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋太径 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinHutokei_Kaku
    {
      get { return _ChutoSyukinHutokei_Kaku ; }
      set { _ChutoSyukinHutokei_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋太径 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinHutokei_Kaku
    {
      get { return _ChukyakuSyukinHutokei_Kaku ; }
      set { _ChukyakuSyukinHutokei_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋X1段太径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinX1danHutokeiHonsu_Kaku
    {
      get { return _ChutoSyukinX1danHutokinHonsu_Kaku ; }
      set { _ChutoSyukinX1danHutokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋X1段太径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku
    {
      get { return _ChukyakuSyukinX1danHutokinHonsu_Kaku ; }
      set { _ChukyakuSyukinX1danHutokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋X2段太径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinX2danHutokeiHonsu_Kaku
    {
      get { return _ChutoSyukinX2danHutokinHonsu_Kaku ; }
      set { _ChutoSyukinX2danHutokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋X2段太径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku
    {
      get { return _ChukyakuSyukinX2danHutokinHonsu_Kaku ; }
      set { _ChukyakuSyukinX2danHutokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋Y1段太径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinY1danHutokeiHonsu_Kaku
    {
      get { return _ChutoSyukinY1danHutokinHonsu_Kaku ; }
      set { _ChutoSyukinY1danHutokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋Y1段太径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku
    {
      get { return _ChukyakuSyukinY1danHutokinHonsu_Kaku ; }
      set { _ChukyakuSyukinY1danHutokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋Y2段太径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinY2danHutokeiHonsu_Kaku
    {
      get { return _ChutoSyukinY2danHutokinHonsu_Kaku ; }
      set { _ChutoSyukinY2danHutokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋Y2段太径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku
    {
      get { return _ChukyakuSyukinY2danHutokinHonsu_Kaku ; }
      set { _ChukyakuSyukinY2danHutokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋細径 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinHosokei_Kaku
    {
      get { return _ChutoSyukinHosokei_Kaku ; }
      set { _ChutoSyukinHosokei_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋細径 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinHosokei_Kaku
    {
      get { return _ChukyakuSyukinHosokei_Kaku ; }
      set { _ChukyakuSyukinHosokei_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋X1段細径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinX1danHosokeiHonsu_Kaku
    {
      get { return _ChutoSyukinX1danHosokinHonsu_Kaku ; }
      set { _ChutoSyukinX1danHosokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋X1段細径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku
    {
      get { return _ChukyakuSyukinX1danHosokinHonsu_Kaku ; }
      set { _ChukyakuSyukinX1danHosokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋X2段細径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinX2danHosokeiHonsu_Kaku
    {
      get { return _ChutoSyukinX2danHosokinHonsu_Kaku ; }
      set { _ChutoSyukinX2danHosokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋X2段細径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku
    {
      get { return _ChukyakuSyukinX2danHosokinHonsu_Kaku ; }
      set { _ChukyakuSyukinX2danHosokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋Y1段細径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinY1danHosokeiHonsu_Kaku
    {
      get { return _ChutoSyukinY1danHosokinHonsu_Kaku ; }
      set { _ChutoSyukinY1danHosokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋Y1段細径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku
    {
      get { return _ChukyakuSyukinY1danHosokinHonsu_Kaku ; }
      set { _ChukyakuSyukinY1danHosokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋Y2段細径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinY2danHosokeiHonsu_Kaku
    {
      get { return _ChutoSyukinY2danHosokinHonsu_Kaku ; }
      set { _ChutoSyukinY2danHosokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋Y2段細径本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku
    {
      get { return _ChukyakuSyukinY2danHosokinHonsu_Kaku ; }
      set { _ChukyakuSyukinY2danHosokinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_芯鉄筋径 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SintekkinKei_Kaku
    {
      get { return _SinTekkinKei_Kaku ; }
      set { _SinTekkinKei_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - CoreRebar_Number - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string CoreRebar_Number_Kaku
    {
      get { return _SinTekkinHonsu_Kaku ; }
      set { _SinTekkinHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_芯鉄筋位置X - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SintekkinIchiX_Kaku
    {
      get { return _SinTekkinXHoukouKyori_Kaku ; }
      set { _SinTekkinXHoukouKyori_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_芯鉄筋位置Y - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SintekkinIchiY_Kaku
    {
      get { return _SinTekkinYHoukouKyuori_Kaku ; }
      set { _SinTekkinYHoukouKyuori_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭フープX径 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoHoopXKei_Kaku
    {
      get { return _ChutoObikinKei_Kaku ; }
      set { _ChutoObikinKei_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚フープX径 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuHoopXKei_Kaku
    {
      get { return _ChukyakuObikinKei_Kaku ; }
      set { _ChukyakuObikinKei_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭フープX本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoHoopXHonsu_Kaku
    {
      get { return _ChutoObikinXHonsu_Kaku ; }
      set { _ChutoObikinXHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚フープX本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuHoopXHonsu_Kaku
    {
      get { return _ChukyakuObikinXHonsu_Kaku ; }
      set { _ChukyakuObikinXHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭フープY本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoHoopYHonsu_Kaku
    {
      get { return _ChutoObikinYHonsu_Kaku ; }
      set { _ChutoObikinYHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚フープY本数 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuHoopYHonsu_Kaku
    {
      get { return _ChukyakuObikinYHonsu_Kaku ; }
      set { _ChukyakuObikinYHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭フープピッチ - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoHoopPitch_Kaku
    {
      get { return _ChutoObikinPitch_Kaku ; }
      set { _ChutoObikinPitch_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚フープピッチ - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuHoopPitch_Kaku
    {
      get { return _ChukyakuObikinPitch_Kaku ; }
      set { _ChukyakuObikinPitch_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_幅止筋径 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_HabadomekinKei_Kaku
    {
      get { return _HabadomekinKei_Kaku ; }
      set { _HabadomekinKei_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - Top_Spacing_XDirectionNumber - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Top_Spacing_XDirectionNumber_Kaku
    {
      get { return _ChutoHabadomekinXHonsu_Kaku ; }
      set { _ChutoHabadomekinXHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - Bottom_Spacing_XDirectionNumber - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Bottom_Spacing_XDirectionNumber_Kaku
    {
      get { return _ChukyakuHabadomekinXHonsu_Kaku ; }
      set { _ChukyakuHabadomekinXHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - Top_Spacing_YDirectionNumber - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Top_Spacing_YDirectionNumber_Kaku
    {
      get { return _ChutoHabadomekinYHonsu_Kaku ; }
      set { _ChutoHabadomekinYHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - Bottom_Spacing_YDirectionNumber - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Bottom_Spacing_YDirectionNumber_Kaku
    {
      get { return _ChukyakuHabadomekinYHonsu_Kaku ; }
      set { _ChukyakuHabadomekinYHonsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_幅止筋ピッチ - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_HabadomekinPitch_Kaku
    {
      get { return _HabadomekinPitch_Kaku ; }
      set { _HabadomekinPitch_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱符号 - 角柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_HasiraHugo_Kaku
    {
      get { return _Hugo_Kaku ; }
      set { _Hugo_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - コンクリート強度の低減率 - 角柱</summary>
    /// <history>20014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ConcreteTeigenritu_Kaku
    {
      get { return _ConcreteTeigenritsu_Kaku ; }
      set { _ConcreteTeigenritsu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 主筋種別X - 角柱</summary>
    /// <history>2014/06/16 GSA,Inc. GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SyukinSyubetuX_Kaku
    {
      get { return _SyukinSyubetuX_Kaku ; }
      set { _SyukinSyubetuX_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 主筋種別Y - 角柱</summary>
    /// <history>2014/06/16 GSA,Inc. GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SyukinSyubetuY_Kaku
    {
      get { return _SyukinSyubetuY_Kaku ; }
      set { _SyukinSyubetuY_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 芯鉄筋種別 - 角柱</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SinTekkinSyubetu
    {
      get { return _SinTekkinSyubetu_Kaku ; }
      set { _SinTekkinSyubetu_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 柱頭 寄せ筋方向 - 角柱</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ChutoYOsekinHoukou_Kaku
    {
      get { return _ChutoYosekinHoukou_Kaku ; }
      set { _ChutoYosekinHoukou_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 柱脚 寄せ筋方向 - 角柱</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ChukyakuYOsekinHoukou_Kaku
    {
      get { return _ChukyakuYosekinHoukou_Kaku ; }
      set { _ChukyakuYosekinHoukou_Kaku = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 柱分類 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Column_Category_En
    {
      get { return _HashiraSyubetsu_En ; }
      set { _HashiraSyubetsu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 直径 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Tyokkei_En
    {
      get { return _Tyokkei_En ; }
      set { _Tyokkei_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋径 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinKei_En
    {
      get { return _ChutoSyukinKei_En ; }
      set { _ChutoSyukinKei_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋径 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinKei_En
    {
      get { return _ChukyakuSyukinKei_En ; }
      set { _ChukyakuSyukinKei_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭主筋本数 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoSyukinHonsu_En
    {
      get { return _ChutoSyukinHonsu_En ; }
      set { _ChutoSyukinHonsu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚主筋本数 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuSyukinHonsu_En
    {
      get { return _ChukyakuSyukinHonsu_En ; }
      set { _ChukyakuSyukinHonsu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_芯鉄筋径 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SintekkinKei_En
    {
      get { return _SinTekkinKei_En ; }
      set { _SinTekkinKei_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_芯鉄筋本数 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SintekkinHonsu_En
    {
      get { return _SinTekkinHonsu_En ; }
      set { _SinTekkinHonsu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_芯鉄筋位置 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SintekkinIchi_En
    {
      get { return _SinTekkinKyori_En ; }
      set { _SinTekkinKyori_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭フープX径 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoHoopXKei_En
    {
      get { return _ChutoObikinKei_En ; }
      set { _ChutoObikinKei_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚フープX径 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuHoopXKei_En
    {
      get { return _ChukyakuObikinKei_En ; }
      set { _ChukyakuObikinKei_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱頭フープピッチ - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChutoHoopPitch_En
    {
      get { return _ChutoObikinPitch_En ; }
      set { _ChutoObikinPitch_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱脚フープピッチ - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_ChukyakuHoopPitch_En
    {
      get { return _ChukyakuObikinPitch_En ; }
      set { _ChukyakuObikinPitch_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_幅止筋径 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_HabadomekinKei_En
    {
      get { return _HabadomekinKei_En ; }
      set { _HabadomekinKei_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - Top_Spacing_XDirectionNumber - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Top_Spacing_XDirectionNumber_En
    {
      get { return _ChutoHabadomekinXHonsu_En ; }
      set { _ChutoHabadomekinXHonsu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - Bottom_Spacing_XDirectionNumber - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Bottom_Spacing_XDirectionNumber_En
    {
      get { return _ChukyakuHabadomekinXHonsu_En ; }
      set { _ChukyakuHabadomekinXHonsu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - Top_Spacing_YDirectionNumber - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Top_Spacing_YDirectionNumber_En
    {
      get { return _ChutoHabadomekinYHonsu_En ; }
      set { _ChutoHabadomekinYHonsu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - Bottom_Spacing_YDirectionNumber - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Bottom_Spacing_YDirectionNumber_En
    {
      get { return _ChukyakuHabadomekinYHonsu_En ; }
      set { _ChukyakuHabadomekinYHonsu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_幅止筋ピッチ - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_HabadomekinPitch_En
    {
      get { return _HabadomekinPitch_En ; }
      set { _HabadomekinPitch_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_柱符号 - 円柱</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_HasiraHugo_En
    {
      get { return _Hugo_En ; }
      set { _Hugo_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - コンクリート強度の低減率 - 円柱</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string ConcreteTeigenritu_En
    {
      get { return _ConcreteTeigenritu_En ; }
      set { _ConcreteTeigenritu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 主筋種別 - 円柱</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SyukinSYubetu_En
    {
      get { return _SyukinSyubetu_En ; }
      set { _SyukinSyubetu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 芯鉄筋種別 - 円柱</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SinTekkinSyubetu_En
    {
      get { return _SinTekkinSyubetu_En ; }
      set { _SinTekkinSyubetu_En = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 梁分類 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string Girder_Category
    {
      get { return _HariSyubetu ; }
      set { _HariSyubetu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端梁幅 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_B
    {
      get { return _ShitanHarihaba ; }
      set { _ShitanHarihaba = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部梁幅 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_B
    {
      get { return _ChuouHarihaba ; }
      set { _ChuouHarihaba = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端梁幅 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_B
    {
      get { return _SyutanHarihaba ; }
      set { _SyutanHarihaba = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端梁成 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_D
    {
      get { return _ShitanHarisei ; }
      set { _ShitanHarisei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部梁成 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_D
    {
      get { return _ChuouHarisei ; }
      set { _ChuouHarisei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端梁成 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_D
    {
      get { return _SyutanHarisei ; }
      set { _SyutanHarisei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端上太径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanUeHutokei
    {
      get { return _SitanUeSyukinHutokei ; }
      set { _SitanUeSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央上太径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohUeHutokei
    {
      get { return _ChuouUeSyukinHutokei ; }
      set { _ChuouUeSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端上太径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanUeHutokei
    {
      get { return _SyutanUeSyukinHutokei ; }
      set { _SyutanUeSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端上1段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanUe1dankinHutokeiHonsu
    {
      get { return _SitanUeSyukin1danHutokeiHonsu ; }
      set { _SitanUeSyukin1danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央上1段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohUe1dankinHutokeiHonsu
    {
      get { return _ChuouUeSyukin1danHutokeiHonsu ; }
      set { _ChuouUeSyukin1danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端上1段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanUe1dankinHutokeiHonsu
    {
      get { return _SyutanUeSyukin1danHutokeiHonsu ; }
      set { _SyutanUeSyukin1danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端上2段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanUe2dankinHutokeiHonsu
    {
      get { return _SitanUeSyukin2danHutokeiHonsu ; }
      set { _SitanUeSyukin2danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央上2段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohUe2dankinHutokeiHonsu
    {
      get { return _ChuouUeSyukin2danHutokeiHonsu ; }
      set { _ChuouUeSyukin2danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端上2段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanUe2dankinHutokeiHonsu
    {
      get { return _SyutanUeSyukin2danHutokeiHonsu ; }
      set { _SyutanUeSyukin2danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端上3段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanUe3dankinHutokeiHonsu
    {
      get { return _SitanUeSyukin3danHutokeiHonsu ; }
      set { _SitanUeSyukin3danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央上3段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohUe3dankinHutokeiHonsu
    {
      get { return _ChuouUeSyukin3danHutokeiHonsu ; }
      set { _ChuouUeSyukin3danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端上3段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanUe3dankinHutokeiHonsu
    {
      get { return _SyutanUeSyukin3danHutokeiHonsu ; }
      set { _SyutanUeSyukin3danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端下太径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanSitaHutokei
    {
      get { return _SitanSitaSyukinHutokei ; }
      set { _SitanSitaSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央下太径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohSitaHutokei
    {
      get { return _ChuouSitaSyukinHutokei ; }
      set { _ChuouSitaSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端下太径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanSitaHutokei
    {
      get { return _SyutanSitaSyukinHutokei ; }
      set { _SyutanSitaSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端下1段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanSita1dankinHutokeiHonsu
    {
      get { return _SitanSitaSyukin1danHutokeiHonsu ; }
      set { _SitanSitaSyukin1danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央下1段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohSita1dankinHutokeiHonsu
    {
      get { return _ChuouSitaSyukin1danHutokeiHonsu ; }
      set { _ChuouSitaSyukin1danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端下1段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanSita1dankinHutokeiHonsu
    {
      get { return _SyutanSitaSyukin1danHutokeiHonsu ; }
      set { _SyutanSitaSyukin1danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端下2段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanSita2dankinHutokeiHonsu
    {
      get { return _SitanSitaSyukin2danHutokeiHonsu ; }
      set { _SitanSitaSyukin2danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央下2段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohSita2dankinHutokeiHonsu
    {
      get { return _ChuouSitaSyukin2danHutokeiHonsu ; }
      set { _ChuouSitaSyukin2danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端下2段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanSita2dankinHutokeiHonsu
    {
      get { return _SyutanSitaSyukin2danHutokeiHonsu ; }
      set { _SyutanSitaSyukin2danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端下3段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanSita3dankinHutokeiHonsu
    {
      get { return _SitanSitaSyukin3danHutokeiHonsu ; }
      set { _SitanSitaSyukin3danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央下3段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohSita3dankinHutokeiHonsu
    {
      get { return _ChuouSitaSyukin3danHutokeiHonsu ; }
      set { _ChuouSitaSyukin3danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端下3段筋太径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanSita3dankinHutokeiHonsu
    {
      get { return _SyutanSitaSyukin3danHutokeiHonsu ; }
      set { _SyutanSitaSyukin3danHutokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端上細径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanUeHosokei
    {
      get { return _SitanUeSyukinHosokei ; }
      set { _SitanUeSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央上細径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohUeHosokei
    {
      get { return _ChuouUeSyukinHosokei ; }
      set { _ChuouUeSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端上細径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanUeHosokei
    {
      get { return _SyutanUeSyukinHosokei ; }
      set { _SyutanUeSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端上1段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanUe1dankinHosokeiHonsu
    {
      get { return _SitanUeSyukin1danHosokeiHonsu ; }
      set { _SitanUeSyukin1danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央上1段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohUe1dankinHosokeiHonsu
    {
      get { return _ChuouUeSyukin1danHosokeiHonsu ; }
      set { _ChuouUeSyukin1danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端上1段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanUe1dankinHosokeiHonsu
    {
      get { return _SyutanUeSyukin1danHosokeiHonsu ; }
      set { _SyutanUeSyukin1danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端上2段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanUe2dankinHosokeiHonsu
    {
      get { return _SitanUeSyukin2danHosokeiHonsu ; }
      set { _SitanUeSyukin2danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央上2段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohUe2dankinHosokeiHonsu
    {
      get { return _ChuouUeSyukin2danHosokeiHonsu ; }
      set { _ChuouUeSyukin2danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端上2段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanUe2dankinHosokeiHonsu
    {
      get { return _SyutanUeSyukin2danHosokeiHonsu ; }
      set { _SyutanUeSyukin2danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端上3段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanUe3dankinHosokeiHonsu
    {
      get { return _SitanUeSyukin3danHosokeiHonsu ; }
      set { _SitanUeSyukin3danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央上3段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohUe3dankinHosokeiHonsu
    {
      get { return _ChuouUeSyukin3danHosokeiHonsu ; }
      set { _ChuouUeSyukin3danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端上3段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanUe3dankinHosokeiHonsu
    {
      get { return _SyutanUeSyukin3danHosokeiHonsu ; }
      set { _SyutanUeSyukin3danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端下細径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanSitaHosokei
    {
      get { return _SitanSitaSyukinHosokei ; }
      set { _SitanSitaSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央下細径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohSitaHosokei
    {
      get { return _ChuouSitaSyukinHosokei ; }
      set { _ChuouSitaSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端下細径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanSitaHosokei
    {
      get { return _SyutanSitaSyukinHosokei ; }
      set { _SyutanSitaSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端下1段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanSita1dankinHosokeiHonsu
    {
      get { return _SitanSitaSyukin1danHosokeiHonsu ; }
      set { _SitanSitaSyukin1danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央下1段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohSita1dankinHosokeiHonsu
    {
      get { return _ChuouSitaSyukin1danHosokeiHonsu ; }
      set { _ChuouSitaSyukin1danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端下1段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanSita1dankinHosokeiHonsu
    {
      get { return _SyutanSitaSyukin1danHosokeiHonsu ; }
      set { _SyutanSitaSyukin1danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端下2段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanSita2dankinHosokeiHonsu
    {
      get { return _SitanSitaSyukin2danHosokeiHonsu ; }
      set { _SitanSitaSyukin2danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央下2段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohSita2dankinHosokeiHonsu
    {
      get { return _ChuouSitaSyukin2danHosokeiHonsu ; }
      set { _ChuouSitaSyukin2danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端下2段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanSita2dankinHosokeiHonsu
    {
      get { return _SyutanSitaSyukin2danHosokeiHonsu ; }
      set { _SyutanSitaSyukin2danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋i端下3段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinItanSita3dankinHosokeiHonsu
    {
      get { return _SitanSitaSyukin3danHosokeiHosnu ; }
      set { _SitanSitaSyukin3danHosokeiHosnu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋中央下3段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinChuohSita3dankinHosokeiHonsu
    {
      get { return _ChuouSitaSyukin3danHosokeiHonsu ; }
      set { _ChuouSitaSyukin3danHosokeiHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_主筋j端下3段筋細径本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_SyukinJtanSita3dankinHosokeiHonsu
    {
      get { return _SyutanSitaSyukin3danHosokeiHosnu ; }
      set { _SyutanSitaSyukin3danHosokeiHosnu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端肋筋径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_Stirrup_Diameter
    {
      get { return _SitanAbarakinkei ; }
      set { _SitanAbarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部肋筋径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_Stirrup_Diameter
    {
      get { return _ChuouAbarakinkei ; }
      set { _ChuouAbarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端肋筋径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_Stirrup_Diameter
    {
      get { return _SyutanAbarakinkei ; }
      set { _SyutanAbarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端肋筋本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_Stirrup_Number
    {
      get { return _SitanAbarakinHonsu ; }
      set { _SitanAbarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部肋筋本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_Stirrup_Number
    {
      get { return _ChuouAbarakinHonsu ; }
      set { _ChuouAbarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端肋筋本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_Stirrup_Number
    {
      get { return _SyutanAbarakinHonsu ; }
      set { _SyutanAbarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端肋筋ピッチ - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_Stirrup_Pitch
    {
      get { return _SitanAbarakinPitch ; }
      set { _SitanAbarakinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部肋筋ピッチ - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_Stirrup_Pitch
    {
      get { return _ChuouAbarakinPitch ; }
      set { _ChuouAbarakinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端肋筋ピッチ - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_Stirrup_Pitch
    {
      get { return _SyutanAbarakinPitch ; }
      set { _SyutanAbarakinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端腹筋径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_Web_Diameter
    {
      get { return _SitanHarakinkei ; }
      set { _SitanHarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部腹筋径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_Web_Diameter
    {
      get { return _ChuouHarakinkei ; }
      set { _ChuouHarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端腹筋径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_Web_Diameter
    {
      get { return _SyutanHarakinkei ; }
      set { _SyutanHarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端腹筋本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_Web_Number
    {
      get { return _SitanHarakinHonsu ; }
      set { _SitanHarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部腹筋本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_Web_Number
    {
      get { return _ChuouHarakinHonsu ; }
      set { _ChuouHarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端腹筋本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_Web_Number
    {
      get { return _SyutanHarakinHonsu ; }
      set { _SyutanHarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端幅止筋径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_Spacing_Diameter
    {
      get { return _SitanHabadomekinkei ; }
      set { _SitanHabadomekinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部幅止筋径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_Spacing_Diameter
    {
      get { return _ChuouHabadomekinkei ; }
      set { _ChuouHabadomekinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端幅止筋径 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_Spacing_Diameter
    {
      get { return _SyutanHabadomekinkei ; }
      set { _SyutanHabadomekinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端幅止筋本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_Spacing_Number
    {
      get { return _SitanHabadomekinHonsu ; }
      set { _SitanHabadomekinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部幅止筋本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_Spacing_Number
    {
      get { return _ChuouHabadomekinHonsu ; }
      set { _ChuouHabadomekinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端幅止筋本数 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_Spacing_Number
    {
      get { return _SyutanHabadomekinHonsu ; }
      set { _SyutanHabadomekinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - i端幅止筋ピッチ - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string s_Spacing_Pitch
    {
      get { return _SitanHabadomekinPitch ; }
      set { _SitanHabadomekinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 中央部幅止筋ピッチ - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string c_Spacing_Pitch
    {
      get { return _ChuouHabadomekinPitch ; }
      set { _ChuouHabadomekinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - j端幅止筋ピッチ - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string e_Spacing_Pitch
    {
      get { return _SyutanHabadomekinPitch ; }
      set { _SyutanHabadomekinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - RST_梁符号 - 梁</summary>
    /// <history>2013/04/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string RST_HariHugo
    {
      get { return _Hugo_Hari ; }
      set { _Hugo_Hari = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - ボックスハンチ 始端 - 梁</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BoxHaunchSitan
    {
      get { return _ShitanBoxHaunch ; }
      set { _ShitanBoxHaunch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - ボックスハンチ 終端 - 梁</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string BoxHaunchSyutan
    {
      get { return _SyutanBoxHaunch ; }
      set { _SyutanBoxHaunch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 始端 ハンチ長さ - 梁</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SitanHaunchNagasa
    {
      get { return _ShitanHaunchNagasa ; }
      set { _ShitanHaunchNagasa = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 終端 ハンチ長さ - 梁</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SyutanHaunchNagasa
    {
      get { return _SyutanHaunchNagasa ; }
      set { _SyutanHaunchNagasa = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 主筋種別 - 梁</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SyukinSyubetu_Hari
    {
      get { return _SyukinSyubetu_Hari ; }
      set { _SyukinSyubetu_Hari = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 梁種別 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string HariSyubetu_Katamoti
    {
      get { return _HariSyubetu_Katamoti ; }
      set { _HariSyubetu_Katamoti = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 梁幅 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanHarihaba
    {
      get { return _MototanHarihaba ; }
      set { _MototanHarihaba = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 梁幅 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanHarihaba
    {
      get { return _SentanHarihaba ; }
      set { _SentanHarihaba = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 梁せい - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanHarisei
    {
      get { return _MototanHariSei ; }
      set { _MototanHariSei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 梁せい - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanHarisei
    {
      get { return _SentanHarisei ; }
      set { _SentanHarisei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 符号 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string HariHugo_Katamoti
    {
      get { return _Hugo_Katamoti ; }
      set { _Hugo_Katamoti = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 上主筋太径 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanUeSyukinHutokei
    {
      get { return _MototanUeSyukinHutokei ; }
      set { _MototanUeSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 上主筋細径 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanUeSyukinHosokei
    {
      get { return _MototanUeSyukinHosokei ; }
      set { _MototanUeSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 上主筋1段太筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanUeSyukin1danHutokinHonsu
    {
      get { return _MototanUeSyukin1danHutokinHonsu ; }
      set { _MototanUeSyukin1danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 上主筋1段細筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanUeSyukin1danHosokinHonsu
    {
      get { return _MototanUeSyukin1danHosokinHonsu ; }
      set { _MototanUeSyukin1danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 上主筋2段太筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanUeSyukin2danHutokinHonsu
    {
      get { return _MototanUeSyukin2danHutokinHonsu ; }
      set { _MototanUeSyukin2danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 上主筋2段細筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanUeSyukin2danHosokinHonsu
    {
      get { return _MototanUeSyukin2danHosokinHonsu ; }
      set { _MototanUeSyukin2danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 上主筋3段太筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanUeSyukin3danHutokinHonsu
    {
      get { return _MototanUeSyukin3danHutokinHonsu ; }
      set { _MototanUeSyukin3danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 上主筋3段細筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanUeSyukin3danHosokinHonsu
    {
      get { return _MototanUeSyukin3danHosokinHonsu ; }
      set { _MototanUeSyukin3danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 下主筋太径 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanSitaSyukinHutokei
    {
      get { return _MototanSitaSyukinHutokei ; }
      set { _MototanSitaSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 下主筋細径 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanSitaSyukinHosokei
    {
      get { return _MototanSitaSyukinHosokei ; }
      set { _MototanSitaSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 下主筋1段太筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanSitaSyukin1danHutokinHonsu
    {
      get { return _MototanSitaSyukin1danHutokinHonsu ; }
      set { _MototanSitaSyukin1danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 下主筋1段細筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanSitaSyukin1danHosokinHonsu
    {
      get { return _MototanSitaSyukin1danHosokinHonsu ; }
      set { _MototanSitaSyukin1danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 下主筋2段太筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanSitaSyukin2danHutokinHonsu
    {
      get { return _MototanSitaSyukin2danHutokinHonsu ; }
      set { _MototanSitaSyukin2danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 下主筋2段細筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanSitaSyukin2danHosokinHonsu
    {
      get { return _MototanSitaSyukin2danHosokinHonsu ; }
      set { _MototanSitaSyukin2danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 下主筋3段太筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanSitaSyukin3danHutokinHonsu
    {
      get { return _MototanSitaSyukin3danHutokinHonsu ; }
      set { _MototanSitaSyukin3danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 下主筋3段細筋本数 - 片持ち梁</summary>
    /// <history>2014/05/30 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanSitaSyukin3danHosokinHonsu
    {
      get { return _MototanSitaSyukin3danHosokinHonsu ; }
      set { _MototanSitaSyukin3danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 上主筋太径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanUeSyukinHutokei
    {
      get { return _SentanUeSyukinHutokei ; }
      set { _SentanUeSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 上主筋細径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanUeSyukinHosokei
    {
      get { return _SentanUeSyukinHosokei ; }
      set { _SentanUeSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 上主筋1段太筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanUeSyukin1danHutokinHonsu
    {
      get { return _SentanUeSyukin1danHutokinHonsu ; }
      set { _SentanUeSyukin1danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 上主筋1段細筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanUeSyukin1danHosokinHonsu
    {
      get { return _SentanUeSyukin1danHosokinHonsu ; }
      set { _SentanUeSyukin1danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 上主筋2段太筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanUeSyukin2danHutokinHonsu
    {
      get { return _SentanUeSyukin2danHutokinHonsu ; }
      set { _SentanUeSyukin2danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 上主筋2段細筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanUeSyukin2danHosokinHonsu
    {
      get { return _SentanUeSyukin2danHosokinHonsu ; }
      set { _SentanUeSyukin2danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 上主筋3段太筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanUeSyukin3danHutokinHonsu
    {
      get { return _SentanUeSyukin3danHutokinHonsu ; }
      set { _SentanUeSyukin3danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 上主筋3段細筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanUeSyukin3danHosokinHonsu
    {
      get { return _SentanUeSyukin3danHosokinHonsu ; }
      set { _SentanUeSyukin3danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 下主筋太径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanSitaSyukinHutokei
    {
      get { return _SentanSitaSyukinHutokei ; }
      set { _SentanSitaSyukinHutokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 下主筋細径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanSitaSyukinHosokei
    {
      get { return _SentanSitaSyukinHosokei ; }
      set { _SentanSitaSyukinHosokei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 下主筋1段太筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanSitaSyukin1danHutokinHonsu
    {
      get { return _SentanSitaSyukin1danHutokinHonsu ; }
      set { _SentanSitaSyukin1danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 下主筋1段細筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanSitaSyukin1danHosokinHonsu
    {
      get { return _SentanSitaSyukin1danHosokinHonsu ; }
      set { _SentanSitaSyukin1danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 下主筋2段太筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanSitaSyukin2danHutokinHonsu
    {
      get { return _SentanSitaSyukin2danHutokinHonsu ; }
      set { _SentanSitaSyukin2danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 下主筋2段細筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanSitaSyukin2danHosokinHonsu
    {
      get { return _SentanSitaSyukin2danHosokinHonsu ; }
      set { _SentanSitaSyukin2danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 下主筋3段太筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanSitaSyukin3danHutokinHonsu
    {
      get { return _SentanSitaSyukin3danHutokinHonsu ; }
      set { _SentanSitaSyukin3danHutokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 下主筋3段細筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanSitaSyukin3danHosokinHonsu
    {
      get { return _SentanSitaSyukin3danHosokinHonsu ; }
      set { _SentanSitaSyukin3danHosokinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 肋筋径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanAbarakinkei
    {
      get { return _MototanAbarakinkei ; }
      set { _MototanAbarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 肋筋径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanAbarakinkei
    {
      get { return _SentanAbarakinkei ; }
      set { _SentanAbarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 肋筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanAbarakinHonsu
    {
      get { return _MototanAbarakinHonsu ; }
      set { _MototanAbarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 肋筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanAbarakinHonsu
    {
      get { return _SentanAbarakinHonsu ; }
      set { _SentanAbarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 肋筋ピッチ - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanAbarakinPitch
    {
      get { return _MototanAbarakinPitch ; }
      set { _MototanAbarakinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 肋筋ピッチ - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanAbarakinPitch
    {
      get { return _SentanAbarakinPitch ; }
      set { _SentanAbarakinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 腹筋径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanHarakinkei
    {
      get { return _MototanHarakinkei ; }
      set { _MototanHarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 腹筋径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanHarakinkei
    {
      get { return _SentanHarakinkei ; }
      set { _SentanHarakinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 腹筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanHarakinHonsu
    {
      get { return _MototanHarakinHonsu ; }
      set { _MototanHarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 腹筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanHarakinHonsu
    {
      get { return _SentanHarakinHonsu ; }
      set { _SentanHarakinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 幅止筋径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanHabadomekinkei
    {
      get { return _MototanHabadomekinkei ; }
      set { _MototanHabadomekinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 幅止筋径 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanHabadomekinkei
    {
      get { return _SentanHabadomekinkei ; }
      set { _SentanHabadomekinkei = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 幅止筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanHabadomekinHonsu
    {
      get { return _MototanHabadomekinHonsu ; }
      set { _MototanHabadomekinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 幅止筋本数 - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanHabadomekinHonsu
    {
      get { return _SentanHabadomekinHonsu ; }
      set { _SentanHabadomekinHonsu = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 元端 幅止筋ピッチ - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string MototanHabadomekinPitch
    {
      get { return _MototanHabadomekinPitch ; }
      set { _MototanHabadomekinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 先端 幅止筋ピッチ - 片持ち梁</summary>
    /// <history>2014/06/02 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SentanHabadomekinPitch
    {
      get { return _SentanHabadomekinPitch ; }
      set { _SentanHabadomekinPitch = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - 主筋種別 - 片持ち梁</summary>
    /// <history>2014/06/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string SyukinSyubetu_Katamoti
    {
      get { return _SyukinSyubetu_Katamoti ; }
      set { _SyukinSyubetu_Katamoti = value ; }
    }

    /// ================================================================================
    /// <summary>設定値 - レベルソート順序</summary>
    /// <history>2013/05/24 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string LevelSortOrder
    {
      get { return _LevelSortOrder ; }
      set { _LevelSortOrder = value ; }
    }

    /// ================================================================================
    /// <summary>デフォルト鉄筋パラメータ</summary>
    /// <history>2013/05/16 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public Collections.Generic.IList<string> DefaultRebarParameter()
    {
      Collections.Generic.IList<string> ret = new Collections.Generic.List<string>() ;

      ret.Add( "Column-Category" ) ;
      ret.Add( "DX" ) ;
      ret.Add( "DY" ) ;
      ret.Add( "RST_柱頭主筋太径" ) ;
      ret.Add( "RST_柱脚主筋太径" ) ;
      ret.Add( "RST_柱頭主筋X1段太径本数" ) ;
      ret.Add( "RST_柱脚主筋X1段太径本数" ) ;
      ret.Add( "RST_柱頭主筋X2段太径本数" ) ;
      ret.Add( "RST_柱脚主筋X2段太径本数" ) ;
      ret.Add( "RST_柱頭主筋Y1段太径本数" ) ;
      ret.Add( "RST_柱脚主筋Y1段太径本数" ) ;
      ret.Add( "RST_柱頭主筋Y2段太径本数" ) ;
      ret.Add( "RST_柱脚主筋Y2段太径本数" ) ;
      ret.Add( "RST_柱頭主筋細径" ) ;
      ret.Add( "RST_柱脚主筋細径" ) ;
      ret.Add( "RST_柱頭主筋X1段細径本数" ) ;
      ret.Add( "RST_柱脚主筋X1段細径本数" ) ;
      ret.Add( "RST_柱頭主筋X2段細径本数" ) ;
      ret.Add( "RST_柱脚主筋X2段細径本数" ) ;
      ret.Add( "RST_柱頭主筋Y1段細径本数" ) ;
      ret.Add( "RST_柱脚主筋Y1段細径本数" ) ;
      ret.Add( "RST_柱頭主筋Y2段細径本数" ) ;
      ret.Add( "RST_柱脚主筋Y2段細径本数" ) ;
      ret.Add( "RST_芯鉄筋径" ) ;
      ret.Add( "CoreRebar_Number" ) ;
      ret.Add( "RST_芯鉄筋位置X" ) ;
      ret.Add( "RST_芯鉄筋位置Y" ) ;

      ret.Add( "" ) ;

      ret.Add( "RST_柱頭フープX径" ) ;
      ret.Add( "RST_柱脚フープX径" ) ;
      ret.Add( "RST_柱頭フープX本数" ) ;
      ret.Add( "RST_柱脚フープX本数" ) ;
      ret.Add( "RST_柱頭フープY本数" ) ;
      ret.Add( "RST_柱脚フープY本数" ) ;
      ret.Add( "RST_柱頭フープピッチ" ) ;
      ret.Add( "RST_柱脚フープピッチ" ) ;
      ret.Add( "RST_幅止筋径" ) ;
      ret.Add( "Top_Spacing_XDirectionNumber" ) ;
      ret.Add( "Bottom_Spacing_XDirectionNumber" ) ;
      ret.Add( "Top_Spacing_YDirectionNumber" ) ;
      ret.Add( "Bottom_Spacing_YDirectionNumber" ) ;
      ret.Add( "RST_幅止筋ピッチ" ) ;
      ret.Add( "RST_柱符号" ) ;

      ret.Add( "" ) ;

      ret.Add( "Column-Category" ) ;
      ret.Add( "直径" ) ;
      ret.Add( "RST_柱頭主筋径" ) ;
      ret.Add( "RST_柱脚主筋径" ) ;
      ret.Add( "RST_柱頭主筋本数" ) ;
      ret.Add( "RST_柱脚主筋本数" ) ;
      ret.Add( "RST_芯鉄筋径" ) ;
      ret.Add( "RST_芯鉄筋本数" ) ;
      ret.Add( "RST_芯鉄筋位置" ) ;
      ret.Add( "RST_柱頭フープX径" ) ;
      ret.Add( "RST_柱脚フープX径" ) ;
      ret.Add( "RST_柱頭フープピッチ" ) ;
      ret.Add( "RST_柱脚フープピッチ" ) ;
      ret.Add( "RST_幅止筋径" ) ;
      ret.Add( "Top_Spacing_XDirectionNumber" ) ;
      ret.Add( "Bottom_Spacing_XDirectionNumber" ) ;
      ret.Add( "Top_Spacing_YDirectionNumber" ) ;
      ret.Add( "Bottom_Spacing_YDirectionNumber" ) ;
      ret.Add( "RST_幅止筋ピッチ" ) ;
      ret.Add( "RST_柱符号" ) ;

      ret.Add( "" ) ;

      ret.Add( "Girder-Category" ) ;
      ret.Add( "s_B" ) ;
      ret.Add( "c_B" ) ;
      ret.Add( "e_B" ) ;
      ret.Add( "s_D" ) ;
      ret.Add( "c_D" ) ;
      ret.Add( "e_D" ) ;
      ret.Add( "RST_主筋i端上太径" ) ;
      ret.Add( "RST_主筋中央上太径" ) ;
      ret.Add( "RST_主筋j端上太径" ) ;
      ret.Add( "RST_主筋i端上1段筋太径本数" ) ;
      ret.Add( "RST_主筋中央上1段筋太径本数" ) ;
      ret.Add( "RST_主筋j端上1段筋太径本数" ) ;
      ret.Add( "RST_主筋i端上2段筋太径本数" ) ;
      ret.Add( "RST_主筋中央上2段筋太径本数" ) ;
      ret.Add( "RST_主筋j端上2段筋太径本数" ) ;
      ret.Add( "RST_主筋i端上3段筋太径本数" ) ;
      ret.Add( "RST_主筋中央上3段筋太径本数" ) ;
      ret.Add( "RST_主筋j端上3段筋太径本数" ) ;
      ret.Add( "RST_主筋i端下太径" ) ;
      ret.Add( "RST_主筋中央下太径" ) ;
      ret.Add( "RST_主筋j端下太径" ) ;

      ret.Add( "" ) ;

      ret.Add( "RST_主筋i端下1段筋太径本数" ) ;
      ret.Add( "RST_主筋中央下1段筋太径本数" ) ;
      ret.Add( "RST_主筋j端下1段筋太径本数" ) ;
      ret.Add( "RST_主筋i端下2段筋太径本数" ) ;
      ret.Add( "RST_主筋中央下2段筋太径本数" ) ;
      ret.Add( "RST_主筋j端下2段筋太径本数" ) ;
      ret.Add( "RST_主筋i端下3段筋太径本数" ) ;
      ret.Add( "RST_主筋中央下3段筋太径本数" ) ;
      ret.Add( "RST_主筋j端下3段筋太径本数" ) ;
      ret.Add( "RST_主筋i端上細径" ) ;
      ret.Add( "RST_主筋中央上細径" ) ;
      ret.Add( "RST_主筋j端上細径" ) ;
      ret.Add( "RST_主筋i端上1段筋細径本数" ) ;
      ret.Add( "RST_主筋中央上1段筋細径本数" ) ;
      ret.Add( "RST_主筋j端上1段筋細径本数" ) ;
      ret.Add( "RST_主筋i端上2段筋細径本数" ) ;
      ret.Add( "RST_主筋中央上2段筋細径本数" ) ;
      ret.Add( "RST_主筋j端上2段筋細径本数" ) ;
      ret.Add( "RST_主筋i端上3段筋細径本数" ) ;
      ret.Add( "RST_主筋中央上3段筋細径本数" ) ;
      ret.Add( "RST_主筋j端上3段筋細径本数" ) ;
      ret.Add( "RST_主筋i端下細径" ) ;
      ret.Add( "RST_主筋中央下細径" ) ;
      ret.Add( "RST_主筋j端下細径" ) ;
      ret.Add( "RST_主筋i端下1段筋細径本数" ) ;
      ret.Add( "RST_主筋中央下1段筋細径本数" ) ;
      ret.Add( "RST_主筋j端下1段筋細径本数" ) ;
      ret.Add( "RST_主筋i端下2段筋細径本数" ) ;
      ret.Add( "RST_主筋中央下2段筋細径本数" ) ;
      ret.Add( "RST_主筋j端下2段筋細径本数" ) ;

      ret.Add( "" ) ;

      ret.Add( "RST_主筋i端下3段筋細径本数" ) ;
      ret.Add( "RST_主筋中央下3段筋細径本数" ) ;
      ret.Add( "RST_主筋j端下3段筋細径本数" ) ;
      ret.Add( "s_Stirrup_Diameter" ) ;
      ret.Add( "c_Stirrup_Diameter" ) ;
      ret.Add( "e_Stirrup_Diameter" ) ;
      ret.Add( "s_Stirrup_Number" ) ;
      ret.Add( "c_Stirrup_Number" ) ;
      ret.Add( "e_Stirrup_Number" ) ;
      ret.Add( "s_Stirrup_Pitch" ) ;
      ret.Add( "c_Stirrup_Pitch" ) ;
      ret.Add( "e_Stirrup_Pitch" ) ;
      ret.Add( "s_Web_Diameter" ) ;
      ret.Add( "c_Web_Diameter" ) ;
      ret.Add( "e_Web_Diameter" ) ;
      ret.Add( "s_Web_Number" ) ;
      ret.Add( "c_Web_Number" ) ;
      ret.Add( "e_Web_Number" ) ;
      ret.Add( "s_Spacing_Diameter" ) ;
      ret.Add( "c_Spacing_Diameter" ) ;
      ret.Add( "e_Spacing_Diameter" ) ;
      ret.Add( "s_Spacing_Number" ) ;
      ret.Add( "c_Spacing_Number" ) ;
      ret.Add( "e_Spacing_Number" ) ;
      ret.Add( "s_Spacing_Pitch" ) ;
      ret.Add( "c_Spacing_Pitch" ) ;
      ret.Add( "e_Spacing_Pitch" ) ;
      ret.Add( "RST_梁符号" ) ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>デフォルトパラメータ</summary>
    /// <history>2013/05/31 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string[] DefaultParameter()
    {
      string[] ret = new string[ 212 ] ;

      ret[ 0 ] = "30" ;
      ret[ 1 ] = "30" ;
      ret[ 2 ] = "4.0 mm" ;
      ret[ 3 ] = "2.5 mm" ;
      ret[ 4 ] = "リスト用寸法" ;
      ret[ 5 ] = "1Dot 黒 実線" ;
      ret[ 6 ] = "2Dot 緑 実線" ;
      ret[ 7 ] = "1Dot シアン HA01" ;
      ret[ 8 ] = "0" ;
      ret[ 9 ] = "12.5" ;
      ret[ 10 ] = "12.5" ;
      ret[ 11 ] = "9" ;
      ret[ 12 ] = "4.5" ;
      ret[ 13 ] = "24" ;
      ret[ 14 ] = "12" ;
      ret[ 15 ] = "4.5" ;
      ret[ 16 ] = "階" ;
      ret[ 17 ] = "階" ;
      ret[ 18 ] = "符号" ;
      ret[ 19 ] = "" ;
      ret[ 20 ] = "11" ;
      ret[ 21 ] = "11" ;
      ret[ 22 ] = "11" ;
      ret[ 23 ] = "11" ;
      ret[ 24 ] = "0" ;
      ret[ 25 ] = "0" ;
      ret[ 26 ] = "1" ;
      ret[ 27 ] = "帯筋" ;
      ret[ 28 ] = "-" ;
      ret[ 29 ] = "" ;
      ret[ 30 ] = "11" ;
      ret[ 31 ] = "11" ;
      ret[ 32 ] = "1" ;
      ret[ 33 ] = "22" ;
      ret[ 34 ] = "11" ;
      ret[ 35 ] = "11" ;
      ret[ 36 ] = "4.5" ;
      ret[ 37 ] = "0" ;
      ret[ 38 ] = "0" ;
      ret[ 39 ] = "1" ;
      ret[ 40 ] = "0" ;
      ret[ 41 ] = "全断" ;
      ret[ 42 ] = "端部" ;
      ret[ 43 ] = "中央" ;
      ret[ 44 ] = "始端" ;
      ret[ 45 ] = "終端" ;
      ret[ 46 ] = "元端" ;
      ret[ 47 ] = "先端" ;
      ret[ 48 ] = "肋筋" ;
      ret[ 49 ] = "-" ;
      ret[ 50 ] = "" ;
      ret[ 51 ] = "2" ;
      ret[ 52 ] = "1" ;
      ret[ 53 ] = "0" ;
      ret[ 54 ] = "0" ;
      ret[ 55 ] = "0" ;
      ret[ 56 ] = "" ;
      ret[ 57 ] = "40" ;
      ret[ 58 ] = "1" ;
      ret[ 59 ] = "40" ;
      ret[ 60 ] = "40" ;
      ret[ 61 ] = "" ;
      ret[ 62 ] = "Column-Category" ;
      ret[ 63 ] = "DX" ;
      ret[ 64 ] = "DY" ;
      ret[ 65 ] = "RST_柱頭主筋太径" ;
      ret[ 66 ] = "RST_柱脚主筋太径" ;
      ret[ 67 ] = "RST_柱頭主筋X1段太径本数" ;
      ret[ 68 ] = "RST_柱脚主筋X1段太径本数" ;
      ret[ 69 ] = "RST_柱頭主筋X2段太径本数" ;
      ret[ 70 ] = "RST_柱脚主筋X2段太径本数" ;
      ret[ 71 ] = "RST_柱頭主筋Y1段太径本数" ;
      ret[ 72 ] = "RST_柱脚主筋Y1段太径本数" ;
      ret[ 73 ] = "RST_柱頭主筋Y2段太径本数" ;
      ret[ 74 ] = "RST_柱脚主筋Y2段太径本数" ;
      ret[ 75 ] = "RST_柱頭主筋細径" ;
      ret[ 76 ] = "RST_柱脚主筋細径" ;
      ret[ 77 ] = "RST_柱頭主筋X1段細径本数" ;
      ret[ 78 ] = "RST_柱脚主筋X1段細径本数" ;
      ret[ 79 ] = "RST_柱頭主筋X2段細径本数" ;
      ret[ 80 ] = "RST_柱脚主筋X2段細径本数" ;
      ret[ 81 ] = "RST_柱頭主筋Y1段細径本数" ;
      ret[ 82 ] = "RST_柱脚主筋Y1段細径本数" ;
      ret[ 83 ] = "RST_柱頭主筋Y2段細径本数" ;
      ret[ 84 ] = "RST_柱脚主筋Y2段細径本数" ;
      ret[ 85 ] = "RST_芯鉄筋径" ;
      ret[ 86 ] = "CoreRebar_Number" ;
      ret[ 87 ] = "RST_芯鉄筋位置X" ;
      ret[ 88 ] = "RST_芯鉄筋位置Y" ;
      ret[ 89 ] = "" ;
      ret[ 90 ] = "RST_柱頭フープX径" ;
      ret[ 91 ] = "RST_柱脚フープX径" ;
      ret[ 92 ] = "RST_柱頭フープX本数" ;
      ret[ 93 ] = "RST_柱脚フープX本数" ;
      ret[ 94 ] = "RST_柱頭フープY本数" ;
      ret[ 95 ] = "RST_柱脚フープY本数" ;
      ret[ 96 ] = "RST_柱頭フープピッチ" ;
      ret[ 97 ] = "RST_柱脚フープピッチ" ;
      ret[ 98 ] = "RST_幅止筋径" ;
      ret[ 99 ] = "Top_Spacing_XDirectionNumber" ;
      ret[ 100 ] = "Bottom_Spacing_XDirectionNumber" ;
      ret[ 101 ] = "Top_Spacing_YDirectionNumber" ;
      ret[ 102 ] = "Bottom_Spacing_YDirectionNumber" ;
      ret[ 103 ] = "RST_幅止筋ピッチ" ;
      ret[ 104 ] = "RST_柱符号" ;
      ret[ 105 ] = "" ;
      ret[ 106 ] = "Column-Category" ;
      ret[ 107 ] = "直径" ;
      ret[ 108 ] = "RST_柱頭主筋径" ;
      ret[ 109 ] = "RST_柱脚主筋径" ;
      ret[ 110 ] = "RST_柱頭主筋本数" ;
      ret[ 111 ] = "RST_柱脚主筋本数" ;
      ret[ 112 ] = "RST_芯鉄筋径" ;
      ret[ 113 ] = "RST_芯鉄筋本数" ;
      ret[ 114 ] = "RST_芯鉄筋位置" ;
      ret[ 115 ] = "RST_柱頭フープX径" ;
      ret[ 116 ] = "RST_柱脚フープX径" ;
      ret[ 117 ] = "RST_柱頭フープピッチ" ;
      ret[ 118 ] = "RST_柱脚フープピッチ" ;
      ret[ 119 ] = "RST_幅止筋径" ;
      ret[ 120 ] = "Top_Spacing_XDirectionNumber" ;
      ret[ 121 ] = "Bottom_Spacing_XDirectionNumber" ;
      ret[ 122 ] = "Top_Spacing_YDirectionNumber" ;
      ret[ 123 ] = "Bottom_Spacing_YDirectionNumber" ;
      ret[ 124 ] = "RST_幅止筋ピッチ" ;
      ret[ 125 ] = "RST_柱符号" ;
      ret[ 126 ] = "" ;
      ret[ 127 ] = "Girder-Category" ;
      ret[ 128 ] = "s_B" ;
      ret[ 129 ] = "c_B" ;
      ret[ 130 ] = "e_B" ;
      ret[ 131 ] = "s_D" ;
      ret[ 132 ] = "c_D" ;
      ret[ 133 ] = "e_D" ;
      ret[ 134 ] = "RST_主筋i端上太径" ;
      ret[ 135 ] = "RST_主筋中央上太径" ;
      ret[ 136 ] = "RST_主筋j端上太径" ;
      ret[ 137 ] = "RST_主筋i端上1段筋太径本数" ;
      ret[ 138 ] = "RST_主筋中央上1段筋太径本数" ;
      ret[ 139 ] = "RST_主筋j端上1段筋太径本数" ;
      ret[ 140 ] = "RST_主筋i端上2段筋太径本数" ;
      ret[ 141 ] = "RST_主筋中央上2段筋太径本数" ;
      ret[ 142 ] = "RST_主筋j端上2段筋太径本数" ;
      ret[ 143 ] = "RST_主筋i端上3段筋太径本数" ;
      ret[ 144 ] = "RST_主筋中央上3段筋太径本数" ;
      ret[ 145 ] = "RST_主筋j端上3段筋太径本数" ;
      ret[ 146 ] = "RST_主筋i端下太径" ;
      ret[ 147 ] = "RST_主筋中央下太径" ;
      ret[ 148 ] = "RST_主筋j端下太径" ;
      ret[ 149 ] = "" ;
      ret[ 150 ] = "RST_主筋i端下1段筋太径本数" ;
      ret[ 151 ] = "RST_主筋中央下1段筋太径本数" ;
      ret[ 152 ] = "RST_主筋j端下1段筋太径本数" ;
      ret[ 153 ] = "RST_主筋i端下2段筋太径本数" ;
      ret[ 154 ] = "RST_主筋中央下2段筋太径本数" ;
      ret[ 155 ] = "RST_主筋j端下2段筋太径本数" ;
      ret[ 156 ] = "RST_主筋i端下3段筋太径本数" ;
      ret[ 157 ] = "RST_主筋中央下3段筋太径本数" ;
      ret[ 158 ] = "RST_主筋j端下3段筋太径本数" ;
      ret[ 159 ] = "RST_主筋i端上細径" ;
      ret[ 160 ] = "RST_主筋中央上細径" ;
      ret[ 161 ] = "RST_主筋j端上細径" ;
      ret[ 162 ] = "RST_主筋i端上1段筋細径本数" ;
      ret[ 163 ] = "RST_主筋中央上1段筋細径本数" ;
      ret[ 164 ] = "RST_主筋j端上1段筋細径本数" ;
      ret[ 165 ] = "RST_主筋i端上2段筋細径本数" ;
      ret[ 166 ] = "RST_主筋中央上2段筋細径本数" ;
      ret[ 167 ] = "RST_主筋j端上2段筋細径本数" ;
      ret[ 168 ] = "RST_主筋i端上3段筋細径本数" ;
      ret[ 169 ] = "RST_主筋中央上3段筋細径本数" ;
      ret[ 170 ] = "RST_主筋j端上3段筋細径本数" ;
      ret[ 171 ] = "RST_主筋i端下細径" ;
      ret[ 172 ] = "RST_主筋中央下細径" ;
      ret[ 173 ] = "RST_主筋j端下細径" ;
      ret[ 174 ] = "RST_主筋i端下1段筋細径本数" ;
      ret[ 175 ] = "RST_主筋中央下1段筋細径本数" ;
      ret[ 176 ] = "RST_主筋j端下1段筋細径本数" ;
      ret[ 177 ] = "RST_主筋i端下2段筋細径本数" ;
      ret[ 178 ] = "RST_主筋中央下2段筋細径本数" ;
      ret[ 179 ] = "RST_主筋j端下2段筋細径本数" ;
      ret[ 180 ] = "" ;
      ret[ 181 ] = "RST_主筋i端下3段筋細径本数" ;
      ret[ 182 ] = "RST_主筋中央下3段筋細径本数" ;
      ret[ 183 ] = "RST_主筋j端下3段筋細径本数" ;
      ret[ 184 ] = "s_Stirrup_Diameter" ;
      ret[ 185 ] = "c_Stirrup_Diameter" ;
      ret[ 186 ] = "e_Stirrup_Diameter" ;
      ret[ 187 ] = "s_Stirrup_Number" ;
      ret[ 188 ] = "c_Stirrup_Number" ;
      ret[ 189 ] = "e_Stirrup_Number" ;
      ret[ 190 ] = "s_Stirrup_Pitch" ;
      ret[ 191 ] = "c_Stirrup_Pitch" ;
      ret[ 192 ] = "e_Stirrup_Pitch" ;
      ret[ 193 ] = "s_Web_Diameter" ;
      ret[ 194 ] = "c_Web_Diameter" ;
      ret[ 195 ] = "e_Web_Diameter" ;
      ret[ 196 ] = "s_Web_Number" ;
      ret[ 197 ] = "c_Web_Number" ;
      ret[ 198 ] = "e_Web_Number" ;
      ret[ 199 ] = "s_Spacing_Diameter" ;
      ret[ 200 ] = "c_Spacing_Diameter" ;
      ret[ 201 ] = "e_Spacing_Diameter" ;
      ret[ 202 ] = "s_Spacing_Number" ;
      ret[ 203 ] = "c_Spacing_Number" ;
      ret[ 204 ] = "e_Spacing_Number" ;
      ret[ 205 ] = "s_Spacing_Pitch" ;
      ret[ 206 ] = "c_Spacing_Pitch" ;
      ret[ 207 ] = "e_Spacing_Pitch" ;
      ret[ 208 ] = "RST_梁符号" ;
      ret[ 209 ] = "" ;
      ret[ 210 ] = "" ;
      ret[ 211 ] = "***** end *****" ;

      return ret ;
    }

    /// ================================================================================
    /// <summary>デフォルト設定パラメータ</summary>
    /// <history>2014/05/28 Created GSA,Inc. Ryo Kuroda</history>
    /// ================================================================================
    public string[] DefaultSettingParameter
    {
      get
      {
        string[] ret = new string[ 63 ] ;

        ret[ 0 ] = "30" ;
        ret[ 1 ] = "30" ;
        ret[ 2 ] = "3.0 mm" ;
        ret[ 3 ] = "2.0 mm" ;
        ret[ 4 ] = "2.5 mm" ;
        ret[ 5 ] = "太線" ;
        ret[ 6 ] = "中線" ;
        ret[ 7 ] = "細線" ;
        ret[ 8 ] = "0" ;
        ret[ 9 ] = "12.5" ;
        ret[ 10 ] = "12.5" ;
        ret[ 11 ] = "9" ;
        ret[ 12 ] = "5" ;
        ret[ 13 ] = "24" ;
        ret[ 14 ] = "12" ;
        ret[ 15 ] = "5" ;
        ret[ 16 ] = "階" ;
        ret[ 17 ] = "FL" ;
        ret[ 18 ] = "符号" ;
        ret[ 19 ] = "Default" ;
        ret[ 20 ] = "0" ;

        ret[ 22 ] = "18" ;
        ret[ 23 ] = "10" ;
        ret[ 24 ] = "10" ;
        ret[ 25 ] = "15" ;
        ret[ 26 ] = "0" ;
        ret[ 27 ] = "1" ;
        ret[ 28 ] = "1" ;
        ret[ 29 ] = "帯筋" ;
        ret[ 30 ] = "-" ;

        ret[ 31 ] = "" ;

        ret[ 32 ] = "18" ;
        ret[ 33 ] = "10" ;
        ret[ 34 ] = "1" ;
        ret[ 35 ] = "28" ;
        ret[ 36 ] = "11" ;
        ret[ 37 ] = "15" ;
        ret[ 38 ] = "4.5" ;
        ret[ 39 ] = "0" ;
        ret[ 40 ] = "0" ;
        ret[ 41 ] = "0" ;
        ret[ 42 ] = "0" ;
        ret[ 43 ] = "全断" ;
        ret[ 44 ] = "端部" ;
        ret[ 45 ] = "中央" ;
        ret[ 46 ] = "始端" ;
        ret[ 47 ] = "終端" ;
        ret[ 48 ] = "元端" ;
        ret[ 49 ] = "先端" ;
        ret[ 50 ] = "肋筋" ;
        ret[ 51 ] = "-" ;

        ret[ 52 ] = "" ;

        ret[ 53 ] = "1" ;
        ret[ 54 ] = "1" ;
        ret[ 55 ] = "1" ;
        ret[ 56 ] = "0" ;
        ret[ 57 ] = "0" ;

        ret[ 58 ] = "" ;

        ret[ 59 ] = "40" ;
        ret[ 60 ] = "0" ;
        ret[ 61 ] = "40" ;
        ret[ 62 ] = "40" ;

        return ret ;
      }
    }
    
        #endregion Properties
    }
}