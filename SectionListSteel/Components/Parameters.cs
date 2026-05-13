using System;
using System.Text;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.IO;
using Autodesk.Revit.DB ;

namespace SectionListSteel.Components
{
    /// ================================================================================
    /// <summary>パラメータ</summary>
    /// ================================================================================
    public class Parameters : SectionListSteel.JExtComCompat.RvtParameters
    {
        // メンバ変数

        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>標準共有パラメータファイル名</summary>
        private string _ShParamDefaultFileName;

        /// <summary>共有パラメータフォルダ名</summary>
        private string _ShParamFolderName;

        /// <summary>共有パラメータファイル名</summary>
        private string _ShParamFileName;

        /// <summary>共有パラメータグループ名</summary>
        private string _ShParamGroupName;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        /// <summary>レベルソート順序</summary>
        private string _LevelSortOrder;

        // 設定ファイル値

        #region 設定ファイル値

        /// <summary>柱リストビュー尺度</summary>
        private string _ViewScaleColumn;

        /// <summary>梁リストビュー尺度</summary>
        private string _ViewScaleBeam;

        /// <summary>タイトル文字</summary>
        private string _FontTitle;

        /// <summary>鉄骨サイズ文字</summary>
        private string _FontSteel;

        /// <summary>線種</summary>
        private string _LineType;

        /// <summary>タイトル表示</summary>
        private string _ShowTitle;

        /// <summary>枠幅 2タイトル</summary>
        private string _FrameWidth2Title;

        /// <summary>枠高さ 2タイトル</summary>
        private string _FrameHeight2Title;

        /// <summary>枠幅 1タイトル</summary>
        private string _FrameWidth1Title;

        /// <summary>枠高さ 1タイトル</summary>
        private string _FrameHeight1Title;

        /// <summary>階表示枠タイトル</summary>
        private string _TitleLvlFrame;

        /// <summary>符号表示枠タイトル</summary>
        private string _TitleSymbolFrame;

        /// <summary>枠タイトル</summary>
        private string _TitleFrame;

        /// <summary>階表示枠接尾語</summary>
        private string _LvlEndword;

        /// <summary>枠幅 二次部材</summary>
        private string _FrameWidthSub;

        /// <summary>枠高さ 二次部材</summary>
        private string _FrameHeightSub;

        /// <summary>枠タイトル 二次部材</summary>
        private string _TitleFrameSub;

        /// <summary>マッピングテーブル</summary>
        private string _SelectTable;

        /// <summary>マッピングテーブルの指定</summary>
        private string _PickTable;

        /// <summary>柱 鉄骨サイズ 枠幅</summary>
        private string _ColumnSteelFrameWidth;

        /// <summary>柱 鉄骨サイズ 高さ</summary>
        private string _ColumnSteelFrameHeight;

        /// <summary>柱 鋼材種表示</summary>
        private string _ColumnShowSteel;

        /// <summary>柱 充填コンクリート設計基準強度表示</summary>
        private string _ColumnShowConcrete;

        /// <summary>柱 備考枠</summary>
        private string _ColumnShowNote;

        /// <summary>柱 リスト折り返し</summary>
        private string _ColumnNewLine;

        /// <summary>柱 折り返しスパン</summary>
        private string _ColumnNewLineSpan;

        /// <summary>間柱 鉄骨サイズ 枠幅</summary>
        private string _SubColumnSteelFrameWidth;

        /// <summary>間柱 備考 枠幅</summary>
        private string _SubColumnNoteFrameWidth;

        /// <summary>間柱 枠高さ</summary>
        private string _SubColumnFrameHeight;

        /// <summary>間柱 鋼材種表示</summary>
        private string _SubColumnShowSteel;

        /// <summary>間柱 備考枠</summary>
        private string _SubColumnShowNote;

        /// <summary>梁 断面位置 枠幅</summary>
        private string _BeamSecFrameWidth;

        /// <summary>梁 鉄骨サイズ 枠幅</summary>
        private string _BeamSteelFrameWidth;

        /// <summary>梁 鉄骨サイズ 枠高さ</summary>
        private string _BeamSteelFrameHeight;

        /// <summary>梁 鋼材種表示</summary>
        private string _BeamShowSteel;

        /// <summary>梁 全断</summary>
        private string _BeamSecZendan;

        /// <summary>梁 中央</summary>
        private string _BeamSecChuoh;

        /// <summary>梁 端部</summary>
        private string _BeamSecTanbu;

        /// <summary>梁 始端</summary>
        private string _BeamSecShitan;

        /// <summary>梁 終端</summary>
        private string _BeamSecSyutan;

        /// <summary>梁 元端</summary>
        private string _BeamSecMototan;

        /// <summary>梁 先端</summary>
        private string _BeamSecSentan;

        /// <summary>梁 備考枠</summary>
        private string _BeamShowNote;

        /// <summary>梁 リスト折り返し</summary>
        private string _BeamNewLine;

        /// <summary>梁 折り返しスパン</summary>
        private string _BeamNewLineSpan;

        /// <summary>小梁 断面位置 枠幅</summary>
        private string _SubBeamSecFrameWidth;

        /// <summary>小梁 鉄骨サイズ 枠幅</summary>
        private string _SubBeamSteelFrameWidth;

        /// <summary>小梁 備考 枠幅</summary>
        private string _SubBeamNoteFrameWidth;

        /// <summary>小梁 鉄骨サイズ 枠高さ</summary>
        private string _SubBeamSteelFrameHeight;

        /// <summary>小梁 鋼材種表示</summary>
        private string _SubBeamShowSteel;

        /// <summary>小梁 備考枠</summary>
        private string _SubBeamShowNote;

        /// <summary>Brace steel frame width</summary>
        private string _BraceSteelFrameWidth;

        /// <summary>Brace note frame width</summary>
        private string _BraceNoteFrameWidth;

        /// <summary>Brace steel frame height</summary>
        private string _BraceSteelFrameHeight;

        /// <summary>Brace show steel</summary>
        private string _BraceShowSteel;

        /// <summary>Brace show note</summary>
        private string _BraceShowNote;

        #endregion 設定ファイル値

        // パラメータマッピングの名前

        #region パラメータマッピングの名前

        /// <summary>S柱 H形鋼 ウェブマテリアル</summary>
        private string _SColumnHWebMaterial;

        /// <summary>S柱 H形鋼 フランジマテリアル</summary>
        private string _SColumnHFlangeMaterial;

        /// <summary>S柱 H形鋼 柱種別</summary>
        private string _SColumnHSyubetsu;

        /// <summary>S柱 H形鋼 柱幅</summary>
        private string _SColumnHHaba;

        /// <summary>S柱 H形鋼 柱せい</summary>
        private string _SColumnHSei;

        /// <summary>S柱 H形鋼 ウェブ厚</summary>
        private string _SColumnHWebAtsu;

        /// <summary>S柱 H形鋼 フランジ厚</summary>
        private string _SColumnHFlangeAtsu;

        /// <summary>S柱 H形鋼 フィレット</summary>
        private string _SColumnHFillet;

        /// <summary>S柱 H形鋼 符号</summary>
        private string _SColumnHFugo;

        /// <summary>S柱 角形鋼管 構造マテリアル</summary>
        private string _SColumnRectMaterial;

        /// <summary>S柱 角形鋼管 柱種別</summary>
        private string _SColumnRectSyubetsu;

        /// <summary>S柱 角形鋼管 柱幅</summary>
        private string _SColumnRectHaba;

        /// <summary>S柱 角形鋼管 柱せい</summary>
        private string _SColumnRectSei;

        /// <summary>S柱 角形鋼管 板厚</summary>
        private string _SColumnRectAtsu;

        /// <summary>S柱 角形鋼管 フィレット</summary>
        private string _SColumnRectFillet;

        /// <summary>S柱 角形鋼管 符号</summary>
        private string _SColumnRectFugo;

        /// <summary>S柱 鋼管 構造マテリアル</summary>
        private string _SColumnRoundMaterial;

        /// <summary>S柱 鋼管 柱種別</summary>
        private string _SColumnRoundSyubetsu;

        /// <summary>S柱 鋼管 直径</summary>
        private string _SColumnRoundDiameter;

        /// <summary>S柱 鋼管 板厚</summary>
        private string _SColumnRoundAtsu;

        /// <summary>S柱 鋼管 符号</summary>
        private string _SColumnRoundFugo;

        /// <summary>CFT柱 角形鋼管 構造マテリアル</summary>
        private string _CFTColumnRectStrcMaterial;

        /// <summary>CFT柱 角形鋼管 コンクリートマテリアル</summary>
        private string _CFTColumnRectConcMaterial;

        /// <summary>CFT柱 角形鋼管 柱種別</summary>
        private string _CFTColumnRectSyubetsu;

        /// <summary>CFT柱 角形鋼管 柱幅</summary>
        private string _CFTColumnRectHaba;

        /// <summary>CFT柱 角形鋼管 柱せい</summary>
        private string _CFTColumnRectSei;

        /// <summary>CFT柱 角形鋼管 板厚</summary>
        private string _CFTColumnRectAtsu;

        /// <summary>CFT柱 角形鋼管 フィレット</summary>
        private string _CFTColumnRectFillet;

        /// <summary>CFT柱 角形鋼管 符号</summary>
        private string _CFTColumnRectFugo;

        /// <summary>CFT柱 鋼管 構造マテリアル</summary>
        private string _CFTColumnRoundStrcMaterial;

        /// <summary>CFT柱 鋼管 コンクリートマテリアル</summary>
        private string _CFTColumnRoundConcMaterial;

        /// <summary>CFT柱 鋼管 柱種別</summary>
        private string _CFTColumnRoundSyubetsu;

        /// <summary>CFT柱 鋼管 直径</summary>
        private string _CFTColumnRoundDiameter;

        /// <summary>CFT柱 鋼管 板厚</summary>
        private string _CFTColumnRoundAtsu;

        /// <summary>CFT柱 鋼管 符号</summary>
        private string _CFTColumnRoundFugo;

        /// <summary>CFT柱 鋼管 T2</summary>
        private string _CFTColumnRectT2;

        /// <summary>S柱山形鋼 構造マテリアル</summary>
        private string _LColumnStrcMaterial;

        /// <summary>S柱山形鋼 柱種別</summary>
        private string _LColumnSyubetsu;

        /// <summary>S柱山形鋼 柱せい</summary>
        private string _LColumnSei;

        /// <summary>S柱山形鋼 柱幅</summary>
        private string _LColumnHaba;

        /// <summary>S柱山形鋼 せい方向板厚</summary>
        private string _LColumnDirThick;

        /// <summary>S柱山形鋼 幅方向板厚</summary>
        private string _LColumnWidthThick;

        /// <summary>S柱山形鋼 符号</summary>
        private string _LColumnFugo;

        /// <summary>S柱溝形鋼 構造マテリアル</summary>
        private string _UColumnStrcMaterial;

        /// <summary>S柱溝形鋼 柱種別</summary>
        private string _UColumnSyubetsu;

        /// <summary>S柱溝形鋼 柱せい</summary>
        private string _UColumnSei;

        /// <summary>S柱溝形鋼 柱幅</summary>
        private string _UColumnHaba;

        /// <summary>S柱溝形鋼 ウェブ厚</summary>
        private string _UColumnWebAtsu;

        /// <summary>S柱溝形鋼 フランジ厚</summary>
        private string _UColumnFlangeAtsu;

        /// <summary>S柱溝形鋼 符号</summary>
        private string _UColumnFugo;

        /// <summary>S柱リップ鋼 構造マテリアル</summary>
        private string _CColumnStrcMaterial;

        /// <summary>S柱リップ鋼 柱種別</summary>
        private string _CColumnSyubetsu;

        /// <summary>S柱リップ鋼 柱せい</summary>
        private string _CColumnSei;

        /// <summary>S柱リップ鋼 柱幅</summary>
        private string _CColumnHaba;

        /// <summary>S柱リップ鋼 リップ長</summary>
        private string _CColumnLipLength;

        /// <summary>S柱リップ鋼 板厚</summary>
        private string _CColumnBoardThick;

        /// <summary>S柱リップ鋼 符号</summary>
        private string _CColumnFugo;

        /// <summary>S柱フラット板 構造マテリアル</summary>
        private string _FBColumnStrcMaterial;

        /// <summary>S柱フラット板 柱種別</summary>
        private string _FBColumnSyubetsu;

        /// <summary>S柱フラット板 幅</summary>
        private string _FBColumnWidth;

        /// <summary>S柱フラット板 板厚</summary>
        private string _FBColumnBoardThick;

        /// <summary>S柱フラット板 符号</summary>
        private string _FBColumnFugo;

        /// <summary>S柱丸棒 構造マテリアル</summary>
        private string _MColumnStrcMaterial;

        /// <summary>S柱丸棒 柱種別</summary>
        private string _MColumnSyubetsu;

        /// <summary>S柱丸棒 直径</summary>
        private string _MColumnDiameter;

        /// <summary>S柱丸棒 符号</summary>
        private string _MColumnFugo;

        /// <summary>S柱T形鋼 ウェブマテリアル</summary>
        private string _TColumnWebMat;

        /// <summary>S柱T形鋼 フランジマテリアル</summary>
        private string _TColumnFlangeMat;

        /// <summary>S柱T形鋼 柱種別</summary>
        private string _TColumnSyubetsu;

        /// <summary>S柱T形鋼 柱せい</summary>
        private string _TColumnSei;

        /// <summary>S柱T形鋼 柱幅</summary>
        private string _TColumnHaba;

        /// <summary>S柱T形鋼 ウェブ厚</summary>
        private string _TColumnWebAtsu;

        /// <summary>S柱T形鋼 フランジ厚</summary>
        private string _TColumnFlangeAtsu;

        /// <summary>S柱T形鋼 符号</summary>
        private string _TColumnFugo;

        /// <summary>S梁 始端 ウェブマテリアル</summary>
        private string _GirderWebMaterial_S;

        /// <summary>S梁 始端 フランジマテリアル</summary>
        private string _GirderFlangeMaterial_S;

        /// <summary>S梁 中央 ウェブマテリアル</summary>
        private string _GirderWebMaterial_C;

        /// <summary>S梁 中央 フランジマテリアル</summary>
        private string _GirderFlangeMaterial_C;

        /// <summary>S梁 終端 ウェブマテリアル</summary>
        private string _GirderWebMaterial_E;

        /// <summary>S梁 終端 フランジマテリアル</summary>
        private string _GirderFlangeMaterial_E;

        /// <summary>S梁 梁種別</summary>
        private string _GirderSyubetsu;

        /// <summary>S梁 始端 梁せい</summary>
        private string _GirderSei_S;

        /// <summary>S梁 始端 梁幅</summary>
        private string _GirderHaba_S;

        /// <summary>S梁 始端 ウェブ厚</summary>
        private string _GirderWebAtsu_S;

        /// <summary>S梁 始端 フランジ厚</summary>
        private string _GirderFlangeAtsu_S;

        /// <summary>S梁 始端 フィレット</summary>
        private string _GirderFillet_S;

        /// <summary>S梁 中央 梁せい</summary>
        private string _GirderSei_C;

        /// <summary>S梁 中央 梁幅</summary>
        private string _GirderHaba_C;

        /// <summary>S梁 中央 ウェブ厚</summary>
        private string _GirderWebAtsu_C;

        /// <summary>S梁 中央 フランジ厚</summary>
        private string _GirderFlangeAtsu_C;

        /// <summary>S梁 中央 フィレット</summary>
        private string _GirderFillet_C;

        /// <summary>S梁 終端 梁せい</summary>
        private string _GirderSei_E;

        /// <summary>S梁 終端 梁幅</summary>
        private string _GirderHaba_E;

        /// <summary>S梁 終端 ウェブ厚</summary>
        private string _GirderWebAtsu_E;

        /// <summary>S梁 終端 フランジ厚</summary>
        private string _GirderFlangeAtsu_E;

        /// <summary>S梁 終端 フィレット</summary>
        private string _GirderFillet_E;

        /// <summary>S梁 始端 ハンチ長さ</summary>
        private string _GirderHaunchNagasa_S;

        /// <summary>S梁 終端 ハンチ長さ</summary>
        private string _GirderHaunchNagasa_E;

        /// <summary>S梁 符号</summary>
        private string _GirderFugo;

        /// <summary>S梁 始端 BH</summary>
        private string _GirderBH_S;

        /// <summary>S梁 終端 BH</summary>
        private string _GirderBH_E;

        /// <summary>S片持ち梁 元端 ウェブマテリアル</summary>
        private string _CantiGirderWebMaterial_S;

        /// <summary>S片持ち梁 元端 フランジマテリアル</summary>
        private string _CantiGirderFlangeMaterial_S;

        /// <summary>S片持ち梁 先端 ウェブマテリアル</summary>
        private string _CantiGirderWebMaterial_E;

        /// <summary>S片持ち梁 先端 フランジマテリアル</summary>
        private string _CantiGirderFlangeMaterial_E;

        /// <summary>S片持ち梁 梁種別</summary>
        private string _CantiGirderSyubetsu;

        /// <summary>S片持ち梁 元端 梁せい</summary>
        private string _CantiGirderSei_S;

        /// <summary>S片持ち梁 元端 梁幅</summary>
        private string _CantiGirderHaba_S;

        /// <summary>S片持ち梁 元端 ウェブ厚</summary>
        private string _CantiGirderWebAtsu_S;

        /// <summary>S片持ち梁 元端 フランジ厚</summary>
        private string _CantiGirderFlangeAtsu_S;

        /// <summary>S片持ち梁 元端 フィレット</summary>
        private string _CantiGirderFillet_S;

        /// <summary>S片持ち梁 先端 梁せい</summary>
        private string _CantiGirderSei_E;

        /// <summary>S片持ち梁 先端 梁幅</summary>
        private string _CantiGirderHaba_E;

        /// <summary>S片持ち梁 先端 ウェブ厚</summary>
        private string _CantiGirderWebAtsu_E;

        /// <summary>S片持ち梁 先端 フランジ厚</summary>
        private string _CantiGirderFlangeAtsu_E;

        /// <summary>S片持ち梁 先端 フィレット</summary>
        private string _CantiGirderFillet_E;

        /// <summary>S片持ち梁 符号</summary>
        private string _CantiGirderFugo;

        /// <summary>S片持ち梁 始端 BH</summary>
        private string _CantiGirderBH_S;

        /// <summary>S片持ち梁 終端 BH</summary>
        private string _CantiGirderBH_E;

        /// <summary>Column Mark</summary>
        private string _ColumnMark;

        /// <summary>Girder Mark</summary>
        private string _GirderMark_S;

        /// <summary>Girder Mark</summary>
        private string _GirderMark_C;

        /// <summary>Girder Mark</summary>
        private string _GirderMark_E;

        /// <summary>Girder Mark</summary>
        private string _CantiGirderMark_S;

        /// <summary>Girder Mark</summary>
        private string _CantiGirderMark_E;

        /// <summary>Rect T2</summary>
        private string _SColumnRectT2;

        /// <summary>S梁・ブレース山形鋼 構造マテリアル</summary>
        private string _LGirderMaterial;

        /// <summary>S梁・ブレース山形鋼 梁種別</summary>
        private string _LGirderHashiyubetsu;

        /// <summary>S梁・ブレース山形鋼 中央 梁せい</summary>
        private string _LGirderSei_C;

        /// <summary>S梁・ブレース山形鋼 中央 梁幅</summary>
        private string _LGirderHaba_C;

        /// <summary>S梁・ブレース山形鋼 中央 せい方向板厚</summary>
        private string _LGirderDirThick_C;

        /// <summary>S梁・ブレース山形鋼 中央 幅方向板厚</summary>
        private string _LGirderWidthThick_C;

        /// <summary>S梁・ブレース山形鋼 符号</summary>
        private string _LGirderFugo;

        /// <summary> S梁・ブレース溝形鋼 構造マテリアル</summary>
        private string _UGirderMaterial;

        /// <summary> S梁・ブレース溝形鋼 梁種別</summary>
        private string _UGirderHashiyubetsu;

        /// <summary> S梁・ブレース溝形鋼 中央 梁せい</summary>
        private string _UGirderSei_C;

        /// <summary> S梁・ブレース溝形鋼 中央 梁幅</summary>
        private string _UGirderHaba_C;

        /// <summary> S梁・ブレース溝形鋼 中央 ウェブ厚</summary>
        private string _UGirderWebAtsu_C;

        /// <summary> S梁・ブレース溝形鋼 中央 フランジ厚</summary>
        private string _UGirderFlangeAtsu_C;

        /// <summary> S梁・ブレース溝形鋼 符号</summary>
        private string _UGirderFugo;

        /// <summary> S梁・ブレースリップ溝形鋼 構造マテリアル</summary>
        private string _CGirderMaterial;

        /// <summary> S梁・ブレースリップ溝形鋼 梁種別</summary>
        private string _CGirderHashiyubetsu;

        /// <summary> S梁・ブレースリップ溝形鋼 中央 梁せい</summary>
        private string _CGirderSei_C;

        /// <summary> S梁・ブレースリップ溝形鋼 中央 梁幅</summary>
        private string _CGirderHaba_C;

        /// <summary> S梁・ブレースリップ溝形鋼 中央 リップ長</summary>
        private string _CGirderLipLength_C;

        /// <summary> S梁・ブレースリップ溝形鋼 中央 リップ長</summary>
        private string _CGirderThick_C;

        /// <summary> S梁・ブレースリップ溝形鋼 符号</summary>
        private string _CGirderFugo;

        /// <summary> ブレースフラットバー</summary>
        private string _FBGirderMaterial;

        private string _FBGirderBraceType;
        private string _FBGirderWidth;
        private string _FBGirderBoardThick;
        private string _FBGirderFugo;

        /// <summary> ブレース丸鋼</summary>
        private string _MGirderMaterial;

        private string _MGirderBraceType;
        private string _MGirderDiameter;
        private string _MGirderFugo;

        /// <summary> S梁カットティー</summary>
        private string _TGirderMaterial;

        private string _TGirderBraceType;
        private string _TGirderSei;
        private string _TGirderHaba;
        private string _TGirderWebAtsu;
        private string _TGirderFlangeAtsu;
        private string _TGirderFugo;

        /// <summary> ブレース角形鋼管</summary>
        private string _RectGirderMaterial;

        private string _RectGirderBraceType;
        private string _RectGirderSei;
        private string _RectGirderHaba;
        private string _RectGirderDirThick;
        private string _RectGirderDirWidth;
        private string _RectGirderFillet;
        private string _RectGirderFugo;

        /// <summary> ブレース円形鋼管</summary>
        private string _PGirderMaterial;

        private string _PGirderBraceType;
        private string _PGirderDiameter;
        private string _PGirderItaatsu;
        private string _PGirderFugo;

        private string _Beam_NoteHeight1;
        private string _Beam_NoteHeight2;
        private string _Beam_NoteHeight3;

        private string _Beam_NoteName1;
        private string _Beam_NoteName2;
        private string _Beam_NoteName3;

        private string _Beam_SelectedNoteIndex;

        private string _Column_NoteHeight1;
        private string _Column_NoteHeight2;
        private string _Column_NoteHeight3;

        private string _Column_NoteName1;
        private string _Column_NoteName2;
        private string _Column_NoteName3;

        private string _Column_SelectedNoteIndex;

        // GIRDERMARK
        private string _GirderMark;

        #endregion パラメータマッピングの名前

        #endregion Member Variables

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
        Parameters(SectionListSteel.Components.Attribute cmpAttribute,
                    Revit.UI.UIDocument rvtUIDoc) :
          base(rvtUIDoc)
        {
            _CmpAttribute = cmpAttribute;

            // デフォルト共有パラメータ
            _ShParamDefaultFileName = null;
            Revit.DB.DefinitionFile defFile = base.GetSharedParameterFile();
            if (defFile != null)
            {
                _ShParamDefaultFileName = defFile.Filename;
            }

            // アプリケーション用共有パラメータ
            _ShParamFolderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _ShParamFileName = _CmpAttribute.ResourceText("IDS_SHPARAM_FILE");
            _ShParamGroupName = _CmpAttribute.ResourceText("IDS_SHPARAM_GROUP");

            if (_ShParamDefaultFileName == null)
            {
                _ShParamDefaultFileName = _ShParamFolderName + "\\" + _ShParamFileName;
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
        /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool SetSharedParamDefault()
        {
            bool ret = false;

            // 共有パラメータファイル設定
            Revit.DB.DefinitionFile defFile = base.SetSharedParameterFile(null, _ShParamDefaultFileName);
            if (defFile != null)
            {
                ret = true;
            }
            return ret;
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
        /// <history>2016/08/05 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool SetDefinition(Revit.DB.Element elem,
                           Collections.Generic.IList<Revit.DB.Category> categories,
                           string defName,
                           Revit.DB.ForgeTypeId paramType,
                           ForgeTypeId bltParamGroup,
                           bool visible,
                           int bindingMode)
        {
            bool ret = base.SetDefinition(elem,
                                          _ShParamFolderName,
                                          _ShParamFileName,
                                          _ShParamGroupName,
                                          categories,
                                          defName,
                                          paramType,
                                          bltParamGroup,
                                          visible,
                                          bindingMode);
            return ret;
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
        /// <history>2016/08/05 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool SetDefinition(Revit.DB.Element elem,
                           Revit.DB.Category category,
                           string defName,
                           Revit.DB.ForgeTypeId paramType,
                           Revit.DB.ForgeTypeId bltParamGroup,
                           bool visible,
                           int bindingMode)
        {
            Collections.Generic.IList<Revit.DB.Category> categories = new Collections.Generic.List<Revit.DB.Category>();
            categories.Add(category);
            return SetDefinition(elem,
                                 categories,
                                 defName,
                                 paramType,
                                 bltParamGroup,
                                 visible,
                                 bindingMode);
        }

        /// ================================================================================
        /// <summary>共有パラメータ文字列取得</summary>
        /// <history>2016/08/05 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetValueString(
            Revit.DB.Element elem,
            string defName,
            Revit.DB.ForgeTypeId paramType,
            Revit.DB.ForgeTypeId bltParamGroup,
            ref string sValue)
        {
            sValue = "";
            if (elem == null)
            {
                return;
            }
            foreach (Revit.DB.Parameter p in elem.Parameters)
            {
                if (p.Definition?.Name != defName)
                {
                    continue;
                }
                if (p.Definition.GetDataType() != paramType)
                {
                    continue;
                }
                if (bltParamGroup.TypeId != string.Empty && p.Definition.GetGroupTypeId() != bltParamGroup)
                {
                    continue;
                }
                sValue = p.AsString() ?? "";
                return;
            }
        }

        /// ================================================================================
        /// <summary>設定ファイル名取得</summary>
        ///
        /// <param name="settingFileName"     >設定ファイル名</param>
        /// <param name="settingFileDirectory">設定ファイルディレクトリ</param>
        /// <param name="levelSortOrder"      >レベルソート順</param>
        ///
        /// <history>2016/08/05 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetStrVal(ref string settingFileName,
                       ref string settingFileDirectory,
                       ref string levelSortOrder)
        {
            settingFileName = _SettingFileName;
            settingFileDirectory = _SettingFileDirectory;
            levelSortOrder = _LevelSortOrder;
        }

        /// ================================================================================
        /// <summary>設定ファイル名取得</summary>
        ///
        /// <param name="settingFileName"     >設定ファイル名</param>
        /// <param name="settingFileDirectory">設定ファイルディレクトリ</param>
        /// <param name="levelSortOrder"      >レベルソート順</param>
        ///
        /// <history>2016/08/05 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetStrVal(string settingFileName,
                       string settingFileDirectory,
                       string levelSortOrder)
        {
            _SettingFileName = settingFileName;
            _SettingFileDirectory = settingFileDirectory;
            _LevelSortOrder = levelSortOrder;
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <param name="settingFileName">設定ファイルの名前</param>
        /// <param name="settingFilePath">設定ファイルの場所</param>
        ///
        /// <history>2016/08/31 Created  GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string[] GetSettingValue(string settingFileName,
                                 string settingFilePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

            string[] ret = null;

            if (System.IO.File.Exists(settingFilePath + settingFileName))
            {
                ret = System.IO.File.ReadAllLines(settingFilePath + settingFileName, enc);
            }
            else
            {
                ret = DefaultSettingParameter;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <param name="stringAry">文字列</param>
        ///
        /// <history><p>2016/08/31 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2017/06/22 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void GetSettingValue(string[] stringAry)
        {
            // 共通
            _ViewScaleColumn = stringAry[0];
            _ViewScaleBeam = stringAry[1];
            _FontTitle = stringAry[2];
            _FontSteel = stringAry[3];
            _LineType = stringAry[4];
            _ShowTitle = stringAry[5];
            _FrameWidth2Title = stringAry[6];
            _FrameHeight2Title = stringAry[7];
            _FrameWidth1Title = stringAry[8];
            _FrameHeight1Title = stringAry[9];
            _TitleLvlFrame = stringAry[10];
            _TitleSymbolFrame = stringAry[11];
            _TitleFrame = stringAry[12];
            _LvlEndword = stringAry[13];
            _FrameWidthSub = stringAry[14];
            _FrameHeightSub = stringAry[15];
            _TitleFrameSub = stringAry[16];

            _SelectTable = stringAry[17];
            _PickTable = stringAry[18];

            // 柱
            _ColumnSteelFrameWidth = stringAry[20];
            _ColumnSteelFrameHeight = stringAry[21];
            _ColumnShowSteel = stringAry[22];
            _ColumnShowConcrete = stringAry[23];
            _ColumnShowNote = stringAry[24];
            _Column_NoteHeight1 = stringAry[25];
            _Column_NoteHeight2 = stringAry[26];
            _Column_NoteHeight3 = stringAry[27];
            _Column_SelectedNoteIndex = stringAry[28];
            _Column_NoteName1 = stringAry[29];
            _Column_NoteName2 = stringAry[30];
            _Column_NoteName3 = stringAry[31];
            _ColumnNewLine = stringAry[32];
            _ColumnNewLineSpan = stringAry[33];

            // 間柱
            _SubColumnSteelFrameWidth = stringAry[35];
            _SubColumnNoteFrameWidth = stringAry[36];
            _SubColumnFrameHeight = stringAry[37];
            _SubColumnShowSteel = stringAry[38];
            _SubColumnShowNote = stringAry[39];

            // 梁
            _BeamSecFrameWidth = stringAry[41];
            _BeamSteelFrameWidth = stringAry[42];
            _BeamSteelFrameHeight = stringAry[43];
            _BeamShowSteel = stringAry[44];
            _BeamSecZendan = stringAry[45];
            _BeamSecChuoh = stringAry[46];
            _BeamSecTanbu = stringAry[47];
            _BeamSecShitan = stringAry[48];
            _BeamSecSyutan = stringAry[49];
            _BeamSecMototan = stringAry[50];
            _BeamSecSentan = stringAry[51];
            _BeamShowNote = stringAry[52];
            _Beam_NoteHeight1 = stringAry[53];
            _Beam_NoteHeight2 = stringAry[54];
            _Beam_NoteHeight3 = stringAry[55];
            _Beam_SelectedNoteIndex = stringAry[56];
            _Beam_NoteName1 = stringAry[57];
            _Beam_NoteName2 = stringAry[58];
            _Beam_NoteName3 = stringAry[59];
            _BeamNewLine = stringAry[60];
            _BeamNewLineSpan = stringAry[61];

            // 小梁
            _SubBeamSecFrameWidth = stringAry[63];
            _SubBeamSteelFrameWidth = stringAry[64];
            _SubBeamNoteFrameWidth = stringAry[65];
            _SubBeamSteelFrameHeight = stringAry[66];
            _SubBeamShowSteel = stringAry[67];
            _SubBeamShowNote = stringAry[68];

            //Brace
            _BraceSteelFrameWidth = stringAry[70];
            _BraceNoteFrameWidth = stringAry[71];
            _BraceSteelFrameHeight = stringAry[72];
            _BraceShowSteel = stringAry[73];
            _BraceShowNote = stringAry[74];
        }

        /// ================================================================================
        /// <summary>パラメータ名取得</summary>
        ///
        /// <history>2016/08/23 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> GetParamNames()
        {
            // 戻り値
            Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> ret = new Collections.Generic.List<Collections.Generic.IDictionary<string, string>>();

            // テーブルパス
            string path = TableFilePath;

            // 存在確認
            if (!System.IO.File.Exists(path))
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_GETTABLEFILE"));

                return ret;
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

            // パラメータ値取得
            string[] strAry = null;

            try
            {
                strAry = System.IO.File.ReadAllLines(path, enc);
            }
            catch (System.IO.IOException)
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_GETTABLEVALUE"));

                return ret;
            }

            // S柱 H形鋼ヘッダ
            string headerHColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_HCOLUMN");
            // S柱 角形鋼管ヘッダ
            string headerRectColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_RECTCOLUMN");
            // S柱 鋼管ヘッダ
            string headerRoundColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_ROUNDCOLUMN");
            // CFT柱 角形鋼管ヘッダ
            string headerCFTRectColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_RECTCOLUMN_CFT");
            // CFT柱 鋼管ヘッダ
            string headerCFTRoundColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_ROUNDCOLUMN_CFT");

            //////////////////////////////////////////////////////////////////////////
            string headerLColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_LCOLUMN");
            string headerUColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_UCOLUMN");
            string headerCColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_CCOLUMN");
            string headerFBColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_FBCOLUMN");
            string headerMColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_MCOLUMN");
            string headerTColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_TCOLUMN");

            string headerLGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_LGIRDER");
            string headerUGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_UGIRDER");
            string headerCGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_CGIRDER");
            string headerFBGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_FBGIRDER");
            string headerMGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_MGIRDER");
            string headerTGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_TGIRDER");
            string headerRectGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_RECTGIRDER");
            string headerPGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_PGIRDER");

            //////////////////////////////////////////////////////////////////////////

            // S梁 ヘッダ
            string headerGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_GIRDER");
            // S片持ち梁 ヘッダ
            string headerCantiGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_CANTIGIRDER");

            // パラメータ内容 - マッピングパラメータ名 対応
            Collections.Generic.IDictionary<string, string> dic = new Collections.Generic.Dictionary<string, string>();

            bool isHColumn = false;
            bool isRectColumn = false;
            bool isRoundColumn = false;
            bool isCFTRectColumn = false;
            bool isCFTRoundColumn = false;

            //////////////////////////////////////////////////////////////////////////
            bool isLColumn = false;
            bool isUColumn = false;
            bool isCColumn = false;
            bool isFBColumn = false;
            bool isMColumn = false;
            bool isTColumn = false;

            bool isLGirder = false;
            bool isUGirder = false;
            bool isCGirder = false;
            bool isFBGirder = false;
            bool isMGirder = false;
            bool isTGirder = false;
            bool isRectGirder = false;
            bool isPGirder = false;
            //////////////////////////////////////////////////////////////////////////

            bool isGirder = false;
            bool isCanti = false;

            foreach (string str in strAry)
            {
                // 値なし
                if (string.IsNullOrEmpty(str) || str == "")
                {
                    isHColumn = false;
                    isRectColumn = false;
                    isRoundColumn = false;
                    isCFTRectColumn = false;
                    isCFTRoundColumn = false;

                    //////////////////////////////////////////////////////////////////////////
                    isLColumn = false;
                    isUColumn = false;
                    isCColumn = false;
                    isFBColumn = false;
                    isMColumn = false;
                    isTColumn = false;

                    isLGirder = false;
                    isUGirder = false;
                    isCGirder = false;
                    isFBGirder = false;
                    isMGirder = false;
                    isTGirder = false;
                    isRectGirder = false;
                    isPGirder = false;

                    //////////////////////////////////////////////////////////////////////////

                    isGirder = false;
                    isCanti = false;

                    if (dic.Count > 0)
                    {
                        ret.Add(dic);

                        dic = new Collections.Generic.Dictionary<string, string>();
                    }

                    continue;
                }

                // 分割した文字
                Collections.Generic.IList<string> strs = new Collections.Generic.List<string>();

                // 区切り文字
                string separetor = " : ";

                if (str.Contains(separetor))
                {
                    // 分割
                    strs = SectionListSteel.JExtComCompat.UtilValue.SplitString(str, separetor);
                }
                else
                {
                    // そのまま
                    strs.Add(str);
                }

                if (strs.Count == 1)
                {
                    // ヘッダ判定
                    if (strs[0] == headerHColumn)
                    {
                        isHColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerHColumn);
                    }
                    else if (strs[0] == headerRectColumn)
                    {
                        isRectColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerRectColumn);
                    }
                    else if (strs[0] == headerRoundColumn)
                    {
                        isRoundColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerRoundColumn);
                    }
                    else if (strs[0] == headerCFTRectColumn)
                    {
                        isCFTRectColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerCFTRectColumn);
                    }
                    else if (strs[0] == headerCFTRoundColumn)
                    {
                        isCFTRoundColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerCFTRoundColumn);
                    }
                    //////////////////////////////////////////////////////////////////////////
                    else if (strs[0] == headerLColumn)
                    {
                        isLColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerLColumn);
                    }
                    else if (strs[0] == headerUColumn)
                    {
                        isUColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerUColumn);
                    }
                    else if (strs[0] == headerCColumn)
                    {
                        isCColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerCColumn);
                    }
                    else if (strs[0] == headerFBColumn)
                    {
                        isFBColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerFBColumn);
                    }
                    else if (strs[0] == headerMColumn)
                    {
                        isMColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerMColumn);
                    }
                    else if (strs[0] == headerTColumn)
                    {
                        isTColumn = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerTColumn);
                    }
                    else if (strs[0] == headerLGirder)
                    {
                        isLGirder = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerLGirder);
                    }
                    else if (strs[0] == headerUGirder)
                    {
                        isUGirder = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerUGirder);
                    }
                    else if (strs[0] == headerCGirder)
                    {
                        isCGirder = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerCGirder);
                    }
                    else if (strs[0] == headerFBGirder)
                    {
                        isFBGirder = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerFBGirder);
                    }
                    else if (strs[0] == headerMGirder)
                    {
                        isMGirder = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerMGirder);
                    }
                    else if (strs[0] == headerTGirder)
                    {
                        isTGirder = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerTGirder);
                    }
                    else if (strs[0] == headerRectGirder)
                    {
                        isRectGirder = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerRectGirder);
                    }
                    else if (strs[0] == headerPGirder)
                    {
                        isPGirder = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerPGirder);
                    }
                    //////////////////////////////////////////////////////////////////////////
                    else if (strs[0] == headerGirder)
                    {
                        isGirder = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerGirder);
                    }
                    else if (strs[0] == headerCantiGirder)
                    {
                        isCanti = true;

                        dic.Add(_CmpAttribute.ResourceText("IDS_TXT_CATEGORY"), headerCantiGirder);
                    }
                }
                else if (strs.Count >= 3)
                {
                    // パラメータ取得
                    if (isHColumn || isRectColumn || isRoundColumn || isCFTRectColumn || isCFTRoundColumn ||
                        isLColumn || isUColumn || isCColumn || isFBColumn || isMColumn || isTColumn ||
                        isLGirder || isUGirder || isCGirder || isFBGirder || isMGirder || isTGirder || isRectGirder || isPGirder ||

                        isGirder || isCanti)
                    {
                        dic.Add(strs[1], strs[2]);
                    }
                }
            }

            //////////////////////////////////////////////////////////////////////////
            if (dic.Count > 0)
            {
                ret.Add(dic);

                dic = new Collections.Generic.Dictionary<string, string>();
            }
            //////////////////////////////////////////////////////////////////////////
            return ret;
        }

        /// ================================================================================
        /// <summary>パラメータ名取得</summary>
        ///
        /// <param name="dicHColumn"        >S柱 H形鋼</param>
        /// <param name="dicRectColumn"     >S柱 角形鋼管</param>
        /// <param name="dicRoundColumn"    >S柱 鋼管</param>
        /// <param name="dicCFTRectColumn"  >CFT柱 角形鋼管</param>
        /// <param name="dicCFTRoundColumn" >CFT柱 鋼管</param>
        /// <param name="dicGirder"         >S梁</param>
        /// <param name="dicCantiGirder"    >S片持ち梁</param>
        ///
        /// <history>2016/08/23 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool GetParamNames(ref Collections.Generic.IDictionary<string, string> dicHColumn,
                           ref Collections.Generic.IDictionary<string, string> dicRectColumn,
                           ref Collections.Generic.IDictionary<string, string> dicRoundColumn,
                           ref Collections.Generic.IDictionary<string, string> dicCFTRectColumn,
                           ref Collections.Generic.IDictionary<string, string> dicCFTRoundColumn,
                           ref Collections.Generic.IDictionary<string, string> dicLColumn,
                           ref Collections.Generic.IDictionary<string, string> dicUColumn,
                           ref Collections.Generic.IDictionary<string, string> dicCColumn,
                           ref Collections.Generic.IDictionary<string, string> dicFBColumn,
                           ref Collections.Generic.IDictionary<string, string> dicMColumn,
                           ref Collections.Generic.IDictionary<string, string> dicTColumn,
                           ref Collections.Generic.IDictionary<string, string> dicGirder,
                           ref Collections.Generic.IDictionary<string, string> dicCantiGirder,
                           ref Collections.Generic.IDictionary<string, string> dicLGirder,
                           ref Collections.Generic.IDictionary<string, string> dicUGirder,
                           ref Collections.Generic.IDictionary<string, string> dicCGirder,
                           ref Collections.Generic.IDictionary<string, string> dicFBGirder,
                           ref Collections.Generic.IDictionary<string, string> dicMGirder,
                           ref Collections.Generic.IDictionary<string, string> dicTGirder,
                           ref Collections.Generic.IDictionary<string, string> dicRectGirder,
                           ref Collections.Generic.IDictionary<string, string> dicPGirder
                           )
        {
            bool ret = false;

            // パラメータ名取得
            Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> allParamNames = GetParamNames();

            if (allParamNames == null || allParamNames.Count < 1)
            {
                return ret;
            }

            foreach (Collections.Generic.IDictionary<string, string> dicParamNames in allParamNames)
            {
                // S柱 H形鋼ヘッダ
                string headerHColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_HCOLUMN");
                // S柱 角形鋼管ヘッダ
                string headerRectColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_RECTCOLUMN");
                // S柱 鋼管ヘッダ
                string headerRoundColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_ROUNDCOLUMN");
                // CFT柱 角形鋼管ヘッダ
                string headerCFTRectColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_RECTCOLUMN_CFT");
                // CFT柱 鋼管ヘッダ
                string headerCFTRoundColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_ROUNDCOLUMN_CFT");

                //////////////////////////////////////////////////////////////////////////
                string headerLColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_LCOLUMN");
                string headerUColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_UCOLUMN");
                string headerCColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_CCOLUMN");
                string headerFBColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_FBCOLUMN");
                string headerMColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_MCOLUMN");
                string headerTColumn = _CmpAttribute.ResourceText("IDS_TXT_HEADER_TCOLUMN");

                string headerLGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_LGIRDER");
                string headerUGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_UGIRDER");
                string headerCGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_CGIRDER");
                string headerFBGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_FBGIRDER");
                string headerMGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_MGIRDER");
                string headerTGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_TGIRDER");
                string headerRectGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_RECTGIRDER");
                string headerPGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_PGIRDER");
                //////////////////////////////////////////////////////////////////////////

                // S梁 ヘッダ
                string headerGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_GIRDER");
                // S片持ち梁 ヘッダ
                string headerCantiGirder = _CmpAttribute.ResourceText("IDS_TXT_HEADER_CANTIGIRDER");

                string key = dicParamNames[_CmpAttribute.ResourceText("IDS_TXT_CATEGORY")];

                if (key == headerHColumn)
                {
                    dicHColumn = dicParamNames;
                }
                else if (key == headerRectColumn)
                {
                    dicRectColumn = dicParamNames;
                }
                else if (key == headerRoundColumn)
                {
                    dicRoundColumn = dicParamNames;
                }
                else if (key == headerCFTRectColumn)
                {
                    dicCFTRectColumn = dicParamNames;
                }
                else if (key == headerCFTRoundColumn)
                {
                    dicCFTRoundColumn = dicParamNames;
                }
                //////////////////////////////////////////////////////////////////////////
                else if (key == headerLColumn)
                {
                    dicLColumn = dicParamNames;
                }
                else if (key == headerUColumn)
                {
                    dicUColumn = dicParamNames;
                }
                else if (key == headerCColumn)
                {
                    dicCColumn = dicParamNames;
                }
                else if (key == headerFBColumn)
                {
                    dicFBColumn = dicParamNames;
                }
                else if (key == headerMColumn)
                {
                    dicMColumn = dicParamNames;
                }
                else if (key == headerTColumn)
                {
                    dicTColumn = dicParamNames;
                }
                //////////////////////////////////////////////////////////////////////////
                else if (key == headerLGirder)
                {
                    dicLGirder = dicParamNames;
                }
                else if (key == headerUGirder)
                {
                    dicUGirder = dicParamNames;
                }
                else if (key == headerCGirder)
                {
                    dicCGirder = dicParamNames;
                }
                else if (key == headerFBGirder)
                {
                    dicFBGirder = dicParamNames;
                }
                else if (key == headerMGirder)
                {
                    dicMGirder = dicParamNames;
                }
                else if (key == headerTGirder)
                {
                    dicTGirder = dicParamNames;
                }
                else if (key == headerRectGirder)
                {
                    dicRectGirder = dicParamNames;
                }
                else if (key == headerPGirder)
                {
                    dicPGirder = dicParamNames;
                }
                //////////////////////////////////////////////////////////////////////////
                else if (key == headerGirder)
                {
                    dicGirder = dicParamNames;
                }
                else if (key == headerCantiGirder)
                {
                    dicCantiGirder = dicParamNames;
                }
            }
            if (dicHColumn == null)
                dicHColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicRectColumn == null)
                dicRectColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicRoundColumn == null)
                dicRoundColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicCFTRectColumn == null)
                dicCFTRectColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicCFTRoundColumn == null)
                dicCFTRoundColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicLColumn == null)
                dicLColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicUColumn == null)
                dicUColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicCColumn == null)
                dicCColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicFBColumn == null)
                dicFBColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicMColumn == null)
                dicMColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicTColumn == null)
                dicTColumn = new Collections.Generic.Dictionary<string, string>();

            if (dicGirder == null)
                dicGirder = new Collections.Generic.Dictionary<string, string>();

            if (dicCantiGirder == null)
                dicCantiGirder = new Collections.Generic.Dictionary<string, string>();

            if (dicLGirder == null)
                dicLGirder = new Collections.Generic.Dictionary<string, string>();

            if (dicUGirder == null)
                dicUGirder = new Collections.Generic.Dictionary<string, string>();

            if (dicCGirder == null)
                dicCGirder = new Collections.Generic.Dictionary<string, string>();

            if (dicFBGirder == null)
                dicFBGirder = new Collections.Generic.Dictionary<string, string>();

            if (dicMGirder == null)
                dicMGirder = new Collections.Generic.Dictionary<string, string>();

            if (dicTGirder == null)
                dicTGirder = new Collections.Generic.Dictionary<string, string>();

            if (dicRectGirder == null)
                dicRectGirder = new Collections.Generic.Dictionary<string, string>();

            if (dicPGirder == null)
                dicPGirder = new Collections.Generic.Dictionary<string, string>();

            ret = true;
            return ret;
        }

        /// ================================================================================
        /// <summary>パラメータ名取得</summary>
        ///
        /// <param name="dicHColumn"        >S柱 H形鋼</param>
        /// <param name="dicRectColumn"     >S柱 角形鋼管</param>
        /// <param name="dicRoundColumn"    >S柱 鋼管</param>
        /// <param name="dicCFTRectColumn"  >CFT柱 角形鋼管</param>
        /// <param name="dicCFTRoundColumn" >CFT柱 鋼管</param>
        /// <param name="dicGirder"         >S梁</param>
        /// <param name="dicCantiGirder"    >S片持ち梁</param>
        ///
        /// <history><p>2016/08/24 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2017/06/20 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void SetParamNames(Collections.Generic.IDictionary<string, string> dicHColumn,
                           Collections.Generic.IDictionary<string, string> dicRectColumn,
                           Collections.Generic.IDictionary<string, string> dicRoundColumn,
                           Collections.Generic.IDictionary<string, string> dicCFTRectColumn,
                           Collections.Generic.IDictionary<string, string> dicCFTRoundColumn,

                            Collections.Generic.IDictionary<string, string> dicLColumn,
                            Collections.Generic.IDictionary<string, string> dicUColumn,
                            Collections.Generic.IDictionary<string, string> dicCColumn,
                            Collections.Generic.IDictionary<string, string> dicFBColumn,
                            Collections.Generic.IDictionary<string, string> dicMColumn,
                            Collections.Generic.IDictionary<string, string> dicTColumn,

                           Collections.Generic.IDictionary<string, string> dicGirder,
                           Collections.Generic.IDictionary<string, string> dicCantiGirder,

                                  Collections.Generic.IDictionary<string, string> dicLGirder,
                            Collections.Generic.IDictionary<string, string> dicUGirder,
                            Collections.Generic.IDictionary<string, string> dicCGirder,
                            Collections.Generic.IDictionary<string, string> dicFBGirder,
                            Collections.Generic.IDictionary<string, string> dicMGirder,
                            Collections.Generic.IDictionary<string, string> dicTGirder,
                            Collections.Generic.IDictionary<string, string> dicRectGirder,
                            Collections.Generic.IDictionary<string, string> dicPGirder
                           )
        {
            #region S柱 H形鋼

            _SColumnHWebMaterial = GetValue(dicHColumn, _CmpAttribute.ResourceText("IDS_TXT_WEBMATERIAL"));
            _SColumnHFlangeMaterial = GetValue(dicHColumn, _CmpAttribute.ResourceText("IDS_TXT_FLANGEMATERIAL"));
            _SColumnHSyubetsu = GetValue(dicHColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _SColumnHHaba = GetValue(dicHColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRAHABA"));
            _SColumnHSei = GetValue(dicHColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASEI"));
            _SColumnHWebAtsu = GetValue(dicHColumn, _CmpAttribute.ResourceText("IDS_TXT_WEBATSU"));
            _SColumnHFlangeAtsu = GetValue(dicHColumn, _CmpAttribute.ResourceText("IDS_TXT_FLANGEATSU"));
            _SColumnHFillet = GetValue(dicHColumn, _CmpAttribute.ResourceText("IDS_TXT_FILLET"));
            _SColumnHFugo = GetValue(dicHColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));

            #endregion S柱 H形鋼

            #region S柱 角形鋼管

            _SColumnRectMaterial = GetValue(dicRectColumn, _CmpAttribute.ResourceText("IDS_TXT_STRUCTURALMATERIAL"));
            _SColumnRectSyubetsu = GetValue(dicRectColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _SColumnRectHaba = GetValue(dicRectColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRAHABA"));
            _SColumnRectSei = GetValue(dicRectColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASEI"));
            _SColumnRectAtsu = GetValue(dicRectColumn, _CmpAttribute.ResourceText("IDS_TXT_ITAATSU"));
            _SColumnRectFillet = GetValue(dicRectColumn, _CmpAttribute.ResourceText("IDS_TXT_FILLET"));
            _SColumnRectFugo = GetValue(dicRectColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));
            _SColumnRectT2 = GetValue(dicRectColumn, _CmpAttribute.ResourceText("IDS_TXT_COLUMN_T2"));

            #endregion S柱 角形鋼管

            #region S柱 鋼管

            _SColumnRoundMaterial = GetValue(dicRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_STRUCTURALMATERIAL"));
            _SColumnRoundSyubetsu = GetValue(dicRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _SColumnRoundDiameter = GetValue(dicRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_DIAMETER"));
            _SColumnRoundAtsu = GetValue(dicRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_ITAATSU"));
            _SColumnRoundFugo = GetValue(dicRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));

            #endregion S柱 鋼管

            #region CFT柱 角形鋼管

            _CFTColumnRectStrcMaterial = GetValue(dicCFTRectColumn, _CmpAttribute.ResourceText("IDS_TXT_STRUCTURALMATERIAL"));
            _CFTColumnRectConcMaterial = GetValue(dicCFTRectColumn, _CmpAttribute.ResourceText("IDS_TXT_CONCRETEMATERIAL"));
            _CFTColumnRectSyubetsu = GetValue(dicCFTRectColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _CFTColumnRectHaba = GetValue(dicCFTRectColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRAHABA"));
            _CFTColumnRectSei = GetValue(dicCFTRectColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASEI"));
            _CFTColumnRectAtsu = GetValue(dicCFTRectColumn, _CmpAttribute.ResourceText("IDS_TXT_ITAATSU"));
            _CFTColumnRectFillet = GetValue(dicCFTRectColumn, _CmpAttribute.ResourceText("IDS_TXT_FILLET"));
            _CFTColumnRectFugo = GetValue(dicCFTRectColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));
            _CFTColumnRectT2 = GetValue(dicCFTRectColumn, _CmpAttribute.ResourceText("IDS_TXT_T2"));

            #endregion CFT柱 角形鋼管

            #region CFT柱 鋼管

            _CFTColumnRoundStrcMaterial = GetValue(dicCFTRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_STRUCTURALMATERIAL"));
            _CFTColumnRoundConcMaterial = GetValue(dicCFTRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_CONCRETEMATERIAL"));
            _CFTColumnRoundSyubetsu = GetValue(dicCFTRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _CFTColumnRoundDiameter = GetValue(dicCFTRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_DIAMETER"));
            _CFTColumnRoundAtsu = GetValue(dicCFTRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_ITAATSU"));
            _CFTColumnRoundFugo = GetValue(dicCFTRoundColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));

            #endregion CFT柱 鋼管

            #region S柱山形鋼

            _LColumnStrcMaterial = GetValue(dicLColumn, _CmpAttribute.ResourceText("IDS_TXT_STRUCTURALMATERIAL"));
            _LColumnSyubetsu = GetValue(dicLColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _LColumnSei = GetValue(dicLColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASEI"));
            _LColumnHaba = GetValue(dicLColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRAHABA"));
            _LColumnDirThick = GetValue(dicLColumn, _CmpAttribute.ResourceText("IDS_TXT_DIRECTION_THICKNESS"));
            _LColumnWidthThick = GetValue(dicLColumn, _CmpAttribute.ResourceText("IDS_TXT_WIDTH_THICKNESS"));
            _LColumnFugo = GetValue(dicLColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));

            #endregion S柱山形鋼

            #region S柱溝形鋼

            _UColumnStrcMaterial = GetValue(dicUColumn, _CmpAttribute.ResourceText("IDS_TXT_STRUCTURALMATERIAL"));
            _UColumnSyubetsu = GetValue(dicUColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _UColumnSei = GetValue(dicUColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASEI"));
            _UColumnHaba = GetValue(dicUColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRAHABA"));
            _UColumnWebAtsu = GetValue(dicUColumn, _CmpAttribute.ResourceText("IDS_TXT_WEBATSU"));
            _UColumnFlangeAtsu = GetValue(dicUColumn, _CmpAttribute.ResourceText("IDS_TXT_FLANGEATSU"));
            _UColumnFugo = GetValue(dicUColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));

            #endregion S柱溝形鋼

            #region S柱リップ鋼

            _CColumnStrcMaterial = GetValue(dicCColumn, _CmpAttribute.ResourceText("IDS_TXT_STRUCTURALMATERIAL"));
            _CColumnSyubetsu = GetValue(dicCColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _CColumnSei = GetValue(dicCColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASEI"));
            _CColumnHaba = GetValue(dicCColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRAHABA"));
            _CColumnLipLength = GetValue(dicCColumn, _CmpAttribute.ResourceText("IDS_TXT_LIP_LENGTH"));
            _CColumnBoardThick = GetValue(dicCColumn, _CmpAttribute.ResourceText("IDS_TXT_BOARD_THICKNESS"));
            _CColumnFugo = GetValue(dicCColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));

            #endregion S柱リップ鋼

            #region S柱フラット板

            _FBColumnStrcMaterial = GetValue(dicFBColumn, _CmpAttribute.ResourceText("IDS_TXT_STRUCTURALMATERIAL"));
            _FBColumnSyubetsu = GetValue(dicFBColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _FBColumnWidth = GetValue(dicFBColumn, _CmpAttribute.ResourceText("IDS_TXT_WIDTH"));
            _FBColumnBoardThick = GetValue(dicFBColumn, _CmpAttribute.ResourceText("IDS_TXT_BOARD_THICKNESS"));
            _FBColumnFugo = GetValue(dicFBColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));

            #endregion S柱フラット板

            #region S柱丸棒

            _MColumnStrcMaterial = GetValue(dicMColumn, _CmpAttribute.ResourceText("IDS_TXT_STRUCTURALMATERIAL"));
            _MColumnSyubetsu = GetValue(dicMColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _MColumnDiameter = GetValue(dicMColumn, _CmpAttribute.ResourceText("IDS_TXT_DIAMETER"));
            _MColumnFugo = GetValue(dicMColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));

            #endregion S柱丸棒

            #region S柱T形鋼

            _TColumnWebMat = GetValue(dicTColumn, _CmpAttribute.ResourceText("IDS_TXT_WEB_MATERIAL"));
            _TColumnFlangeMat = GetValue(dicTColumn, _CmpAttribute.ResourceText("IDS_TXT_FLANGE_MATERIAL"));
            _TColumnSyubetsu = GetValue(dicTColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASYUBETSU"));
            _TColumnSei = GetValue(dicTColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRASEI"));
            _TColumnHaba = GetValue(dicTColumn, _CmpAttribute.ResourceText("IDS_TXT_HASHIRAHABA"));
            _TColumnWebAtsu = GetValue(dicTColumn, _CmpAttribute.ResourceText("IDS_TXT_WEBATSU"));
            _TColumnFlangeAtsu = GetValue(dicTColumn, _CmpAttribute.ResourceText("IDS_TXT_FLANGEATSU"));
            _TColumnFugo = GetValue(dicTColumn, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));

            #endregion S柱T形鋼

            #region S梁

            _GirderWebMaterial_S = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SHITAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBMATERIAL"));
            _GirderFlangeMaterial_S = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SHITAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEMATERIAL"));
            _GirderWebMaterial_C = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_CHUOH") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBMATERIAL"));
            _GirderFlangeMaterial_C = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_CHUOH") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEMATERIAL"));
            _GirderWebMaterial_E = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SYUTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBMATERIAL"));
            _GirderFlangeMaterial_E = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SYUTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEMATERIAL"));
            _GirderSyubetsu = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_HARISYUBETSU"));
            _GirderSei_S = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SHITAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARISEI"));
            _GirderHaba_S = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SHITAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARIHABA"));
            _GirderWebAtsu_S = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SHITAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBATSU"));
            _GirderFlangeAtsu_S = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SHITAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEATSU"));
            _GirderFillet_S = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SHITAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FILLET"));
            _GirderSei_C = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_CHUOH") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARISEI"));
            _GirderHaba_C = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_CHUOH") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARIHABA"));
            _GirderWebAtsu_C = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_CHUOH") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBATSU"));
            _GirderFlangeAtsu_C = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_CHUOH") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEATSU"));
            _GirderFillet_C = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_CHUOH") + " " + _CmpAttribute.ResourceText("IDS_TXT_FILLET"));
            _GirderSei_E = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SYUTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARISEI"));
            _GirderHaba_E = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SYUTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARIHABA"));
            _GirderWebAtsu_E = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SYUTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBATSU"));
            _GirderFlangeAtsu_E = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SYUTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEATSU"));
            _GirderFillet_E = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SYUTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FILLET"));
            _GirderHaunchNagasa_S = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SHITAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HAUNCHNAGASA"));
            _GirderHaunchNagasa_E = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_SYUTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HAUNCHNAGASA"));
            _GirderFugo = GetValue(dicGirder, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));
            _GirderBH_S = _CmpAttribute.ResourceText("IDS_TXT_BH_S");
            _GirderBH_E = _CmpAttribute.ResourceText("IDS_TXT_BH_E");

            #endregion S梁

            #region S片持ち梁

            _CantiGirderWebMaterial_S = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_MOTOTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBMATERIAL"));
            _CantiGirderFlangeMaterial_S = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_MOTOTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEMATERIAL"));
            _CantiGirderWebMaterial_E = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_SENTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBMATERIAL"));
            _CantiGirderFlangeMaterial_E = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_SENTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEMATERIAL"));
            _CantiGirderSyubetsu = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_HARISYUBETSU"));
            _CantiGirderSei_S = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_MOTOTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARISEI"));
            _CantiGirderHaba_S = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_MOTOTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARIHABA"));
            _CantiGirderWebAtsu_S = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_MOTOTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBATSU"));
            _CantiGirderFlangeAtsu_S = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_MOTOTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEATSU"));
            _CantiGirderFillet_S = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_MOTOTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FILLET"));
            _CantiGirderSei_E = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_SENTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARISEI"));
            _CantiGirderHaba_E = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_SENTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_HARIHABA"));
            _CantiGirderWebAtsu_E = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_SENTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_WEBATSU"));
            _CantiGirderFlangeAtsu_E = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_SENTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FLANGEATSU"));
            _CantiGirderFillet_E = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_SENTAN") + " " + _CmpAttribute.ResourceText("IDS_TXT_FILLET"));
            _CantiGirderFugo = GetValue(dicCantiGirder, _CmpAttribute.ResourceText("IDS_TXT_FUGO"));
            _CantiGirderBH_S = _CmpAttribute.ResourceText("IDS_TXT_BH_S");
            _CantiGirderBH_E = _CmpAttribute.ResourceText("IDS_TXT_BH_E");

            _ColumnMark = _CmpAttribute.ResourceText("IDS_PARAM_MARK_COL");

            _GirderMark_S = _CmpAttribute.ResourceText("IDS_TXT_GIRDER_S");
            _GirderMark_C = _CmpAttribute.ResourceText("IDS_TXT_GIRDER_C");
            _GirderMark_E = _CmpAttribute.ResourceText("IDS_TXT_GIRDER_E");
            _CantiGirderMark_S = _CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S");
            _CantiGirderMark_E = _CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_E");

            #endregion S片持ち梁

            #region S梁・ブレース山形鋼

            _LGirderMaterial = GetValue(dicLGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_MATERIAL"));
            _LGirderHashiyubetsu = GetValue(dicLGirder, _CmpAttribute.ResourceText("IDS_TXT_HARISYUBETSU"));
            _LGirderSei_C = GetValue(dicLGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_CENTER_SEI"));
            _LGirderHaba_C = GetValue(dicLGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_CENTER_WIDTH"));
            _LGirderDirThick_C = GetValue(dicLGirder, _CmpAttribute.ResourceText("IDS_TXT_DIRECTION_THICKNESS_GIRDER"));
            _LGirderWidthThick_C = GetValue(dicLGirder, _CmpAttribute.ResourceText("IDS_TXT_WIDTH_THICKNESS_GIRDER"));
            _LGirderFugo = GetValue(dicLGirder, _CmpAttribute.ResourceText("IDS_CN_FUGO"));

            #endregion S梁・ブレース山形鋼

            #region S梁・ブレース溝形鋼

            _UGirderMaterial = GetValue(dicUGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_MATERIAL"));
            _UGirderHashiyubetsu = GetValue(dicUGirder, _CmpAttribute.ResourceText("IDS_TXT_HARISYUBETSU"));
            _UGirderSei_C = GetValue(dicUGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_CENTER_SEI"));
            _UGirderHaba_C = GetValue(dicUGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_CENTER_WIDTH"));
            _UGirderWebAtsu_C = GetValue(dicUGirder, _CmpAttribute.ResourceText("IDS_TXT_WEBATSU_C"));
            _UGirderFlangeAtsu_C = GetValue(dicUGirder, _CmpAttribute.ResourceText("IDS_TXT_FLANGE_C"));
            _UGirderFugo = GetValue(dicUGirder, _CmpAttribute.ResourceText("IDS_CN_FUGO"));

            #endregion S梁・ブレース溝形鋼

            #region S梁・ブレースリップ溝形鋼

            _CGirderMaterial = GetValue(dicCGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_MATERIAL"));
            _CGirderHashiyubetsu = GetValue(dicCGirder, _CmpAttribute.ResourceText("IDS_TXT_HARISYUBETSU"));
            _CGirderSei_C = GetValue(dicCGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_CENTER_SEI"));
            _CGirderHaba_C = GetValue(dicCGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_CENTER_WIDTH"));
            _CGirderLipLength_C = GetValue(dicCGirder, _CmpAttribute.ResourceText("IDS_TXT_LIP_LENGTH_GIRDER"));
            _CGirderThick_C = GetValue(dicCGirder, _CmpAttribute.ResourceText("IDS_TXT_THICKNESS_GIRDER"));
            _CGirderFugo = GetValue(dicCGirder, _CmpAttribute.ResourceText("IDS_CN_FUGO"));

            #endregion S梁・ブレースリップ溝形鋼

            #region ブレースフラットバー

            _FBGirderMaterial = GetValue(dicFBGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_MATERIAL"));
            _FBGirderBraceType = GetValue(dicFBGirder, _CmpAttribute.ResourceText("IDS_TXT_BRACETYPE"));
            _FBGirderWidth = GetValue(dicFBGirder, _CmpAttribute.ResourceText("IDS_TXT_WIDTH"));
            _FBGirderBoardThick = GetValue(dicFBGirder, _CmpAttribute.ResourceText("IDS_TXT_BOARD_THICKNESS"));
            _FBGirderFugo = GetValue(dicFBGirder, _CmpAttribute.ResourceText("IDS_CN_FUGO"));

            #endregion ブレースフラットバー

            #region ブレース丸鋼

            _MGirderMaterial = GetValue(dicMGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_MATERIAL"));
            _MGirderBraceType = GetValue(dicMGirder, _CmpAttribute.ResourceText("IDS_TXT_BRACETYPE"));
            _MGirderDiameter = GetValue(dicMGirder, _CmpAttribute.ResourceText("IDS_TXT_DIAMETER_GIRDER"));
            _MGirderFugo = GetValue(dicMGirder, _CmpAttribute.ResourceText("IDS_CN_FUGO"));

            #endregion ブレース丸鋼

            #region S梁カットティー

            _TGirderMaterial = GetValue(dicTGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_MATERIAL"));
            _TGirderBraceType = GetValue(dicTGirder, _CmpAttribute.ResourceText("IDS_TXT_HARISYUBETSU"));
            _TGirderSei = GetValue(dicTGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_CENTER_SEI"));
            _TGirderHaba = GetValue(dicTGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_CENTER_WIDTH"));
            _TGirderWebAtsu = GetValue(dicTGirder, _CmpAttribute.ResourceText("IDS_TXT_WEBATSU_C"));
            _TGirderFlangeAtsu = GetValue(dicTGirder, _CmpAttribute.ResourceText("IDS_TXT_FLANGE_C"));
            _TGirderFugo = GetValue(dicTGirder, _CmpAttribute.ResourceText("IDS_CN_FUGO"));

            #endregion S梁カットティー

            #region ブレース角形鋼管

            _RectGirderMaterial = GetValue(dicRectGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_MATERIAL"));
            _RectGirderBraceType = GetValue(dicRectGirder, _CmpAttribute.ResourceText("IDS_TXT_BRACETYPE"));
            _RectGirderSei = GetValue(dicRectGirder, _CmpAttribute.ResourceText("IDS_TXT_STEEL_FRAME_GIRDER"));
            _RectGirderHaba = GetValue(dicRectGirder, _CmpAttribute.ResourceText("IDS_TXT_STEEL_FRAME_WIDTH_GIRDER"));
            _RectGirderDirThick = GetValue(dicRectGirder, _CmpAttribute.ResourceText("IDS_TXT_THICKNESS_IN_DIRECT_GIRDER"));
            _RectGirderDirWidth = GetValue(dicRectGirder, _CmpAttribute.ResourceText("IDS_TXT_WIDTH_IN_DIRECT_GIRDER"));
            _RectGirderFillet = GetValue(dicRectGirder, _CmpAttribute.ResourceText("IDS_TXT_FILLET"));
            _RectGirderFugo = GetValue(dicRectGirder, _CmpAttribute.ResourceText("IDS_CN_FUGO"));

            #endregion ブレース角形鋼管

            #region ブレース円形鋼管

            _PGirderMaterial = GetValue(dicPGirder, _CmpAttribute.ResourceText("IDS_TXT_GIRDER_MATERIAL"));
            _PGirderBraceType = GetValue(dicPGirder, _CmpAttribute.ResourceText("IDS_TXT_BRACETYPE"));
            _PGirderDiameter = GetValue(dicPGirder, _CmpAttribute.ResourceText("IDS_TXT_DIAMETER_GIRDER"));
            _PGirderItaatsu = GetValue(dicPGirder, _CmpAttribute.ResourceText("IDS_TXT_ITAATSU"));
            _PGirderFugo = GetValue(dicPGirder, _CmpAttribute.ResourceText("IDS_CN_FUGO"));

            #endregion ブレース円形鋼管

            // Girder Mark
            _GirderMark = _CmpAttribute.ResourceText("IDS_TXT_GIRDER_MARK");
        }

        private string GetValue(Collections.Generic.IDictionary<string, string> dic, string key)
        {
            if (dic.ContainsKey(key))
                return dic[key];

            return string.Empty;
        }

        /// ================================================================================
        /// <summary>タイプの階取得</summary>
        ///
        /// <param name="famSym"        >ファミリタイプ</param>
        /// <param name="fugoParamName" >符号パラメータ名</param>
        ///
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GetTypeLevel(Revit.DB.FamilySymbol famSym,
                            string fugoParamName)
        {
            string ret = "0";

            Revit.DB.Parameter parFugo = famSym.LookupParameter(fugoParamName);

            if (parFugo != null)
            {
                string name = famSym.Name;

                try
                {
                    string strFugo = parFugo.AsString();

                    if (strFugo != "")
                    {
                        ret = name.Substring(0, name.LastIndexOf(strFugo));
                    }
                    else
                    {
                        ret = name;
                    }
                }
                catch
                {
                    ret = name;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>パラメータファイルパス</summary>
        ///
        /// <history><p>2016/08/19 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2017/06/19 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string ParameterFilePath()
        {
            string ret = "";
            
            var version = _CmpAttribute.ResourceText("IDS_TXT_REVITVERSION_2027");

            // フォルダ
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ") + "\\" + version+ "\\";

            // 存在確認
            if (!System.IO.Directory.Exists(folder))
            {
                return ret;
            }

            foreach (string str in System.IO.Directory.GetFiles(folder, "*.xls"))
            {
                if (str.StartsWith(folder + _CmpAttribute.ResourceText($"IDS_TXT_PARAMETERFILE_NAME_HEAD_{version}")))
                {
                    if (ret == "")
                    {
                        ret = str;
                    }
                    else
                    {
                        string sub1 = str.Substring(folder.Length + 12);
                        string sub2 = ret.Substring(folder.Length + 12);

                        sub1 = sub1.Substring(0, sub1.LastIndexOf(".xls"));
                        sub2 = sub2.Substring(0, sub2.LastIndexOf(".xls"));

                        double i1 = 0;
                        double i2 = 0;

                        double.TryParse(sub1, out i1);
                        double.TryParse(sub2, out i2);

                        if (i1 > i2)
                        {
                            ret = str;
                        }
                    }
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>予備ファイルのコピー</summary>
        ///
        /// <history><p>2016/08/19 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2017/06/19 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void ReserveFileCopy()
        {
            var version = _CmpAttribute.ResourceText("IDS_TXT_REVITVERSION_2027");
            
            // 実行フォルダ
            string reservePath = _CmpAttribute.ExecuteFolder;

            // マイドキュメント
            string myDoc = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            // SS3 Linkとの共通フォルダ
            string shareFolderPath = myDoc + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ") + "\\" + version;

            // テーブルファイル
            string tableFile = _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");


            // マッピングパラメータファイル
            string mapParamFile = _CmpAttribute.ResourceText($"IDS_TXT_PARAMETERFILE_NAME_{version}");

            // 共有パラメータファイル
            string shareFile = _CmpAttribute.ResourceText("IDS_TXT_SHAREFILE");

            // 共有パラメータファイル - オリジナル
            string shareFileOrg = _CmpAttribute.ResourceText("IDS_TXT_SHAREFILE_ORG");

            // フォルダ存在確認
            if (System.IO.Directory.Exists(shareFolderPath))
            {
                // テーブルなしまたはデフォルト
                if (!System.IO.File.Exists(_SelectTable) || _PickTable == "0")
                {
                    if (System.IO.File.Exists(reservePath + "\\" + tableFile))
                    {
                        if (!System.IO.File.Exists(shareFolderPath + "\\" + tableFile))
                        {
                            System.IO.File.Copy(reservePath + "\\" + tableFile,
                                                shareFolderPath + "\\" + tableFile);

                            _SelectTable = shareFolderPath + "\\" + tableFile;
                        }
                    }
                }

                // マッピングファイルなし
                if (!System.IO.File.Exists(shareFolderPath + "\\" + mapParamFile))
                {
                    if (System.IO.File.Exists(reservePath + "\\" + mapParamFile))
                    {
                        System.IO.File.Copy(reservePath + "\\" + mapParamFile,
                                            shareFolderPath + "\\" + mapParamFile);
                    }
                }

                // 共有パラメータファイルなし
                if (!System.IO.File.Exists(shareFolderPath + "\\" + shareFile))
                {
                    if (System.IO.File.Exists(reservePath + "\\" + shareFile))
                    {
                        System.IO.File.Copy(reservePath + "\\" + shareFile,
                                            shareFolderPath + "\\" + shareFile);
                    }
                }

                // 共有パラメータオリジナルファイルなし
                if (!System.IO.File.Exists(shareFolderPath + "\\" + shareFileOrg))
                {
                    if (System.IO.File.Exists(reservePath + "\\" + shareFileOrg))
                    {
                        System.IO.File.Copy(reservePath + "\\" + shareFileOrg,
                                            shareFolderPath + "\\" + shareFileOrg);
                    }
                }
            }
            // フォルダがない場合
            else
            {
                try
                {
                    // 「Autodesk REXJ」フォルダ作成
                    if (!System.IO.Directory.Exists(myDoc + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ")))
                    {
                        System.IO.Directory.CreateDirectory(myDoc + "\\" + _CmpAttribute.ResourceText("IDS_TXT_ADSKREXJ"));
                    }

                    // 「2018」フォルダ作成
                    System.IO.Directory.CreateDirectory(shareFolderPath);

                    // フォルダがないならファイルもない

                    // テーブルコピー
                    if (System.IO.File.Exists(reservePath + "\\" + tableFile))
                    {
                        System.IO.File.Copy(reservePath + "\\" + tableFile,
                                            shareFolderPath + "\\" + tableFile);
                    }

                    // マッピングファイルコピー
                    if (System.IO.File.Exists(reservePath + "\\" + mapParamFile))
                    {
                        System.IO.File.Copy(reservePath + "\\" + mapParamFile,
                                            shareFolderPath + "\\" + mapParamFile);
                    }

                    // 共有パラメータファイルコピー
                    if (System.IO.File.Exists(reservePath + "\\" + shareFile))
                    {
                        System.IO.File.Copy(reservePath + "\\" + shareFile,
                                            shareFolderPath + "\\" + shareFile);
                    }

                    // 共有パラメータオリジナルファイルコピー
                    if (System.IO.File.Exists(reservePath + "\\" + shareFileOrg))
                    {
                        System.IO.File.Copy(reservePath + "\\" + shareFileOrg,
                                            shareFolderPath + "\\" + shareFileOrg);
                    }
                }
                catch
                {
                }
            }

            return;
        }

        #endregion Member Functions

        // プロパティ

        #region Properties

        #region 設定値

        /// ================================================================================
        /// <summary>柱リストビュー尺度</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string ViewScaleColumn
        {
            get
            {
                return _ViewScaleColumn;
            }
        }

        /// ================================================================================
        /// <summary>梁リストビュー尺度</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string ViewScaleBeam
        {
            get
            {
                return _ViewScaleBeam;
            }
        }

        /// ================================================================================
        /// <summary>タイトル文字</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string FontTitle
        {
            get
            {
                return _FontTitle;
            }
        }

        /// ================================================================================
        /// <summary>鉄骨サイズ文字</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string FontSteel
        {
            get
            {
                return _FontSteel;
            }
        }

        /// ================================================================================
        /// <summary>線種</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string LineType
        {
            get
            {
                return _LineType;
            }
        }

        /// ================================================================================
        /// <summary>タイトル表示</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string ShowTitle
        {
            get
            {
                return _ShowTitle;
            }
        }

        /// ================================================================================
        /// <summary>枠幅 2タイトル</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string FrameWidth2Title
        {
            get
            {
                return _FrameWidth2Title;
            }
        }

        /// ================================================================================
        /// <summary>枠高さ 2タイトル</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string FrameHeight2Title
        {
            get
            {
                return _FrameHeight2Title;
            }
        }

        /// ================================================================================
        /// <summary>枠幅 1タイトル</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string FrameWidth1Title
        {
            get
            {
                return _FrameWidth1Title;
            }
        }

        /// ================================================================================
        /// <summary>枠高さ 1タイトル</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string FrameHeight1Title
        {
            get
            {
                return _FrameHeight1Title;
            }
        }

        /// ================================================================================
        /// <summary>階表示枠タイトル</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string TitleLvlFrame
        {
            get
            {
                return _TitleLvlFrame;
            }
        }

        /// ================================================================================
        /// <summary>符号表示枠タイトル</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string TitleSymbolFrame
        {
            get
            {
                return _TitleSymbolFrame;
            }
        }

        /// ================================================================================
        /// <summary>枠タイトル</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string TitleFrame
        {
            get
            {
                return _TitleFrame;
            }
        }

        /// ================================================================================
        /// <summary>階表示枠接尾語</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string LvlEndword
        {
            get
            {
                return _LvlEndword;
            }
        }

        /// ================================================================================
        /// <summary>枠幅 二次部材</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string FrameWidthSub
        {
            get
            {
                return _FrameWidthSub;
            }
        }

        /// ================================================================================
        /// <summary>枠高さ 二次部材</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string FrameHeightSub
        {
            get
            {
                return _FrameHeightSub;
            }
        }

        /// ================================================================================
        /// <summary>枠タイトル 二次部材</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string TitleFrameSub
        {
            get
            {
                return _TitleFrameSub;
            }
        }

        /// ================================================================================
        /// <summary>テーブルファイルパス</summary>
        /// <history><p>2016/08/23 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2017/06/16 Modified CST,Co.Ltd Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string TableFilePath
        {
            get
            {
                var version = _CmpAttribute.ResourceText("IDS_TXT_REVITVERSION_2027");
                
                string ret = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\Autodesk REXJ\\" + version + "\\" + _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");

                if (System.IO.File.Exists(_SelectTable))
                {
                    ret = _SelectTable;
                }

                if (_PickTable == "0")
                {
                    ret = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\Autodesk REXJ\\" + version + "\\" + _CmpAttribute.ResourceText($"IDS_TXT_TABLEFILE_{version}");
                }

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>テーブルファイル指定</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string PickTable
        {
            get
            {
                return _PickTable;
            }
        }

        /// ================================================================================
        /// <summary>柱 鉄骨サイズ 枠幅</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string ColumnSteelFrameWidth
        {
            get
            {
                return _ColumnSteelFrameWidth;
            }
        }

        /// ================================================================================
        /// <summary>柱 鉄骨サイズ 枠高さ</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string ColumnSteelFrameHeight
        {
            get
            {
                return _ColumnSteelFrameHeight;
            }
        }

        /// ================================================================================
        /// <summary>柱 鋼材種表示</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string ColumnShowSteel
        {
            get
            {
                return _ColumnShowSteel;
            }
        }

        /// ================================================================================
        /// <summary>柱 充填コンクリート設計基準強度表示</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string ColumnShowConcrete
        {
            get
            {
                return _ColumnShowConcrete;
            }
        }

        /// ================================================================================
        /// <summary>柱 備考枠</summary>
        /// <history>2016/10/28 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public string ColumnShowNote
        {
            get
            {
                return _ColumnShowNote;
            }
        }

        /// ================================================================================
        /// <summary>柱 リストの折り返し</summary>
        /// <history>2017/06/21 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public string ColumnNewLine
        {
            get
            {
                return _ColumnNewLine;
            }
        }

        /// ================================================================================
        /// <summary>柱 リストの折り返しスパン</summary>
        /// <history>2017/06/21 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public string ColumnNewLineSpan
        {
            get
            {
                return _ColumnNewLineSpan;
            }
        }

        /// ================================================================================
        /// <summary>間柱 鉄骨サイズ 枠幅</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubColumnSteelFrameWidth
        {
            get
            {
                return _SubColumnSteelFrameWidth;
            }
        }

        /// ================================================================================
        /// <summary>間柱 鉄骨サイズ 枠幅</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubColumnNoteFrameWidth
        {
            get
            {
                return _SubColumnNoteFrameWidth;
            }
        }

        /// ================================================================================
        /// <summary>間柱 鉄骨サイズ 枠高さ</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubColumnSteelFrameHeight
        {
            get
            {
                return _SubColumnFrameHeight;
            }
        }

        /// ================================================================================
        /// <summary>間柱 鋼材種表示</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubColumnShowSteel
        {
            get
            {
                return _SubColumnShowSteel;
            }
        }

        /// ================================================================================
        /// <summary>間柱 備考枠</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public string SubColumnShowNote
        {
            get
            {
                return _SubColumnShowNote;
            }
        }

        /// ================================================================================
        /// <summary>梁 断面位置 枠幅</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSecFrameWidth
        {
            get
            {
                return _BeamSecFrameWidth;
            }
        }

        /// ================================================================================
        /// <summary>梁 鉄骨サイズ 枠幅</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSteelFrameWidth
        {
            get
            {
                return _BeamSteelFrameWidth;
            }
        }

        /// ================================================================================
        /// <summary>梁 鉄骨サイズ 枠高さ</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSteelFrameHeight
        {
            get
            {
                return _BeamSteelFrameHeight;
            }
        }

        /// ================================================================================
        /// <summary>梁 鋼材種表示</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamShowSteel
        {
            get
            {
                return _BeamShowSteel;
            }
        }

        /// ================================================================================
        /// <summary>梁 全断</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSecZendan
        {
            get
            {
                return _BeamSecZendan;
            }
        }

        /// ================================================================================
        /// <summary>梁 中央</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSecChuoh
        {
            get
            {
                return _BeamSecChuoh;
            }
        }

        /// ================================================================================
        /// <summary>梁 端部</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSecTanbu
        {
            get
            {
                return _BeamSecTanbu;
            }
        }

        /// ================================================================================
        /// <summary>梁 始端</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSecShitan
        {
            get
            {
                return _BeamSecShitan;
            }
        }

        /// ================================================================================
        /// <summary>梁 終端</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSecSyutan
        {
            get
            {
                return _BeamSecSyutan;
            }
        }

        /// ================================================================================
        /// <summary>梁 元端</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSecMototan
        {
            get
            {
                return _BeamSecMototan;
            }
        }

        /// ================================================================================
        /// <summary>梁 先端</summary>
        /// <history>2016/08/31 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string BeamSecSentan
        {
            get
            {
                return _BeamSecSentan;
            }
        }

        /// ================================================================================
        /// <summary>梁 備考枠</summary>
        /// <history>2016/10/28 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public string BeamShowNote
        {
            get
            {
                return _BeamShowNote;
            }
        }

        /// ================================================================================
        /// <summary>梁 リストの折り返し</summary>
        /// <history>2017/06/21 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public string BeamNewLine
        {
            get
            {
                return _BeamNewLine;
            }
        }

        /// ================================================================================
        /// <summary>梁 リストの折り返しスパン</summary>
        /// <history>2017/06/21 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public string BeamNewLineSpan
        {
            get
            {
                return _BeamNewLineSpan;
            }
        }

        /// ================================================================================
        /// <summary>小梁 断面位置 枠幅</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubBeamSecFrameWidth
        {
            get
            {
                return _SubBeamSecFrameWidth;
            }
        }

        /// ================================================================================
        /// <summary>小梁 鉄骨サイズ 枠幅</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubBeamSteelFrameWidth
        {
            get
            {
                return _SubBeamSteelFrameWidth;
            }
        }

        /// ================================================================================
        /// <summary>小梁 備考 枠幅</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubBeamNoteFrameWidth
        {
            get
            {
                return _SubBeamNoteFrameWidth;
            }
        }

        /// ================================================================================
        /// <summary>小梁 鉄骨サイズ 枠高さ</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubBeamSteelFrameHeight
        {
            get
            {
                return _SubBeamSteelFrameHeight;
            }
        }

        /// ================================================================================
        /// <summary>小梁 鋼材種表示</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubBeamShowSteel
        {
            get
            {
                return _SubBeamShowSteel;
            }
        }

        /// ================================================================================
        /// <summary>小梁 備考枠</summary>
        /// <history>2017/06/22 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SubBeamShowNote
        {
            get
            {
                return _SubBeamShowNote;
            }
        }

        public
            string BraceSteelFrameWidth
        {
            get
            {
                return _BraceSteelFrameWidth;
            }
        }

        public
        string BraceNoteFrameWidth
        {
            get
            {
                return _BraceNoteFrameWidth;
            }
        }

        public
        string BraceSteelFrameHeight
        {
            get
            {
                return _BraceSteelFrameHeight;
            }
        }

        public
        string BraceShowSteel
        {
            get
            {
                return _BraceShowSteel;
            }
        }

        public
        string BraceShowNote
        {
            get
            {
                return _BraceShowNote;
            }
        }

        public string Beam_NoteHeight1
        {
            get
            {
                return _Beam_NoteHeight1;
            }
        }

        public string Beam_NoteHeight2
        {
            get
            {
                return _Beam_NoteHeight2;
            }
        }

        public string Beam_NoteHeight3
        {
            get
            {
                return _Beam_NoteHeight3;
            }
        }

        public string Beam_NoteName1
        {
            get
            {
                return _Beam_NoteName1;
            }
        }

        public string Beam_NoteName2
        {
            get
            {
                return _Beam_NoteName2;
            }
        }

        public string Beam_NoteName3
        {
            get
            {
                return _Beam_NoteName3;
            }
        }

        public string Beam_SelectedNoteIndex
        {
            get
            {
                return _Beam_SelectedNoteIndex;
            }
        }

        public string Column_NoteHeight1
        {
            get
            {
                return _Column_NoteHeight1;
            }
        }

        public string Column_NoteHeight2
        {
            get
            {
                return _Column_NoteHeight2;
            }
        }

        public string Column_NoteHeight3
        {
            get
            {
                return _Column_NoteHeight3;
            }
        }

        public string Column_NoteName1
        {
            get
            {
                return _Column_NoteName1;
            }
        }

        public string Column_NoteName2
        {
            get
            {
                return _Column_NoteName2;
            }
        }

        public string Column_NoteName3
        {
            get
            {
                return _Column_NoteName3;
            }
        }

        public string Column_SelectedNoteIndex
        {
            get
            {
                return _Column_SelectedNoteIndex;
            }
        }

        #endregion 設定値

        #region パラメータ名

        /// ================================================================================
        /// <summary>S柱H形鋼 ウェブマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnHWebMaterial
        {
            get
            {
                return _SColumnHWebMaterial;
            }
        }

        /// ================================================================================
        /// <summary>S柱H形鋼 フランジマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnHFlangeMaterial
        {
            get
            {
                return _SColumnHFlangeMaterial;
            }
        }

        /// ================================================================================
        /// <summary>S柱H形鋼 柱種別</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnHSyubetsu
        {
            get
            {
                return _SColumnHSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱H形鋼 柱幅</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnHHaba
        {
            get
            {
                return _SColumnHHaba;
            }
        }

        /// ================================================================================
        /// <summary>S柱H形鋼 柱せい</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnHSei
        {
            get
            {
                return _SColumnHSei;
            }
        }

        /// ================================================================================
        /// <summary>S柱H形鋼 ウェブ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnHWebAtsu
        {
            get
            {
                return _SColumnHWebAtsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱H形鋼 フランジ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnHFlangeAtsu
        {
            get
            {
                return _SColumnHFlangeAtsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱H形鋼 フィレット</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnHFillet
        {
            get
            {
                return _SColumnHFillet;
            }
        }

        /// ================================================================================
        /// <summary>S柱H形鋼 符号</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnHFugo
        {
            get
            {
                return _SColumnHFugo;
            }
        }

        /// ================================================================================
        /// <summary>S柱角形鋼管 構造マテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRectStructuralMaterial
        {
            get
            {
                return _SColumnRectMaterial;
            }
        }

        /// ================================================================================
        /// <summary>S柱角形鋼管 柱種別</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRectSyubetsu
        {
            get
            {
                return _SColumnRectSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱角形鋼管 柱幅</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRectHaba
        {
            get
            {
                return _SColumnRectHaba;
            }
        }

        /// ================================================================================
        /// <summary>S柱角形鋼管 柱せい</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRectSei
        {
            get
            {
                return _SColumnRectSei;
            }
        }

        /// ================================================================================
        /// <summary>S柱角形鋼管 板厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRectItaAtsu
        {
            get
            {
                return _SColumnRectAtsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱角形鋼管 フィレット</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRectFillet
        {
            get
            {
                return _SColumnRectFillet;
            }
        }

        /// ================================================================================
        /// <summary>S柱角形鋼管 符号</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRectFugo
        {
            get
            {
                return _SColumnRectFugo;
            }
        }

        /// ================================================================================
        /// <summary>S柱鋼管 構造マテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRoundStructuralMaterial
        {
            get
            {
                return _SColumnRoundMaterial;
            }
        }

        /// ================================================================================
        /// <summary>S柱鋼管 柱種別</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRoundSyubetsu
        {
            get
            {
                return _SColumnRoundSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱鋼管 直径</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRoundDiameter
        {
            get
            {
                return _SColumnRoundDiameter;
            }
        }

        /// ================================================================================
        /// <summary>S柱鋼管 板厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRoundItaAtsu
        {
            get
            {
                return _SColumnRoundAtsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱鋼管 符号</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string SColumnRoundFugo
        {
            get
            {
                return _SColumnRoundFugo;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱角形鋼管 構造マテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRectStructuralMaterial
        {
            get
            {
                return _CFTColumnRectStrcMaterial;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱角形鋼管 コンクリートマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRectConcreteMaterial
        {
            get
            {
                return _CFTColumnRectConcMaterial;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱角形鋼管 柱種別</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRectSyubetsu
        {
            get
            {
                return _CFTColumnRectSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱角形鋼管 柱幅</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRectHaba
        {
            get
            {
                return _CFTColumnRectHaba;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱角形鋼管 柱せい</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRectSei
        {
            get
            {
                return _CFTColumnRectSei;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱角形鋼管 板厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRectItaAtsu
        {
            get
            {
                return _CFTColumnRectAtsu;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱角形鋼管 フィレット</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRectFillet
        {
            get
            {
                return _CFTColumnRectFillet;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱角形鋼管 符号</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRectFugo
        {
            get
            {
                return _CFTColumnRectFugo;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱角形鋼管 符号</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRectT2
        {
            get
            {
                return _CFTColumnRectT2;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱鋼管 構造マテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRoundStructuralMaterial
        {
            get
            {
                return _CFTColumnRoundStrcMaterial;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱鋼管 コンクリートマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRoundConcreteMaterial
        {
            get
            {
                return _CFTColumnRoundConcMaterial;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱鋼管 柱種別</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRoundSyubetsu
        {
            get
            {
                return _CFTColumnRoundSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱鋼管 直径</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRoundDiameter
        {
            get
            {
                return _CFTColumnRoundDiameter;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱鋼管 板厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRoundItaAtsu
        {
            get
            {
                return _CFTColumnRoundAtsu;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱鋼管 符号</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CFTColumnRoundFugo
        {
            get
            {
                return _CFTColumnRoundFugo;
            }
        }

        /// ================================================================================
        /// <summary>CFT柱鋼管 T2</summary>
        /// ================================================================================
        public
        string CFTColumnRoundT2
        {
            get
            {
                return _CFTColumnRectT2;
            }
        }

        /// ================================================================================
        /// <summary>S柱山形鋼 構造マテリアル</summary>
        /// ================================================================================
        public
        string LColumnStrcMaterial
        {
            get
            {
                return _LColumnStrcMaterial;
            }
        }

        /// ================================================================================
        /// <summary>S柱山形鋼 柱種別</summary>
        /// ================================================================================
        public
        string LColumnSyubetsu
        {
            get
            {
                return _LColumnSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱山形鋼 柱せい</summary>
        /// ================================================================================
        public
        string LColumnSei
        {
            get
            {
                return _LColumnSei;
            }
        }

        /// ================================================================================
        /// <summary>S柱山形鋼 柱幅</summary>
        /// ================================================================================
        public
        string LColumnHaba
        {
            get
            {
                return _LColumnHaba;
            }
        }

        /// ================================================================================
        /// <summary>S柱山形鋼 せい方向板厚</summary>
        /// ================================================================================
        public
        string LColumnDirThick
        {
            get
            {
                return _LColumnDirThick;
            }
        }

        /// ================================================================================
        /// <summary>S柱山形鋼 幅方向板厚</summary>
        /// ================================================================================
        public
        string LColumnWidthThick
        {
            get
            {
                return _LColumnWidthThick;
            }
        }

        /// ================================================================================
        /// <summary>S柱山形鋼 符号</summary>
        /// ================================================================================
        public
        string LColumnFugo
        {
            get
            {
                return _LColumnFugo;
            }
        }

        /// ================================================================================
        /// <summary>S柱山形鋼 構造マテリアル</summary>
        /// ================================================================================
        public
        string UColumnStrcMaterial
        {
            get
            {
                return _UColumnStrcMaterial;
            }
        }

        /// ================================================================================
        /// <summary>S柱溝形鋼 柱種別</summary>
        /// ================================================================================
        public
        string UColumnSyubetsu
        {
            get
            {
                return _UColumnSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱溝形鋼 柱せい</summary>
        /// ================================================================================
        public
        string UColumnSei
        {
            get
            {
                return _UColumnSei;
            }
        }

        /// ================================================================================
        /// <summary>S柱溝形鋼 柱幅</summary>
        /// ================================================================================
        public
        string UColumnHaba
        {
            get
            {
                return _UColumnHaba;
            }
        }

        /// ================================================================================
        /// <summary>S柱溝形鋼 ウェブ厚</summary>
        /// ================================================================================
        public
        string UColumnWebAtsu
        {
            get
            {
                return _UColumnWebAtsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱溝形鋼 フランジ厚</summary>
        /// ================================================================================
        public
        string UColumnFlangeAtsu
        {
            get
            {
                return _UColumnFlangeAtsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱溝形鋼 符号</summary>
        /// ================================================================================
        public
        string UColumnFugo
        {
            get
            {
                return _UColumnFugo;
            }
        }

        /// ================================================================================
        /// <summary>S柱リップ鋼 構造マテリアル</summary>
        /// ================================================================================
        public
        string CColumnStrcMaterial
        {
            get
            {
                return _CColumnStrcMaterial;
            }
        }

        /// ================================================================================
        /// <summary>S柱リップ鋼 柱種別</summary>
        /// ================================================================================
        public
        string CColumnSyubetsu
        {
            get
            {
                return _CColumnSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱リップ鋼 柱せい</summary>
        /// ================================================================================
        public
        string CColumnSei
        {
            get
            {
                return _CColumnSei;
            }
        }

        /// ================================================================================
        /// <summary>S柱リップ鋼 柱幅</summary>
        /// ================================================================================
        public
        string CColumnHaba
        {
            get
            {
                return _CColumnHaba;
            }
        }

        /// ================================================================================
        /// <summary>S柱リップ鋼 リップ長</summary>
        /// ================================================================================
        public
        string CColumnLipLength
        {
            get
            {
                return _CColumnLipLength;
            }
        }

        /// ================================================================================
        /// <summary>S柱リップ鋼 板厚</summary>
        /// ================================================================================
        public
        string CColumnBoardThick
        {
            get
            {
                return _CColumnBoardThick;
            }
        }

        /// ================================================================================
        /// <summary>S柱リップ鋼 符号</summary>
        /// ================================================================================
        public
        string CColumnFugo
        {
            get
            {
                return _CColumnFugo;
            }
        }

        /// ================================================================================
        /// <summary>S柱フラット板 構造マテリアル</summary>
        /// ================================================================================
        public
        string FBColumnStrcMaterial
        {
            get
            {
                return _FBColumnStrcMaterial;
            }
        }

        /// ================================================================================
        /// <summary>S柱フラット板 柱種別</summary>
        /// ================================================================================
        public
        string FBColumnSyubetsu
        {
            get
            {
                return _FBColumnSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱フラット板 幅</summary>
        /// ================================================================================
        public
        string FBColumnWidth
        {
            get
            {
                return _FBColumnWidth;
            }
        }

        /// ================================================================================
        /// <summary>S柱フラット板 板厚</summary>
        /// ================================================================================
        public
        string FBColumnBoardThick
        {
            get
            {
                return _FBColumnBoardThick;
            }
        }

        /// ================================================================================
        /// <summary>S柱フラット板 符号</summary>
        /// ================================================================================
        public
        string FBColumnFugo
        {
            get
            {
                return _FBColumnFugo;
            }
        }

        /// ================================================================================
        /// <summary>S柱丸棒 構造マテリアル</summary>
        /// ================================================================================
        public
        string MColumnStrcMaterial
        {
            get
            {
                return _MColumnStrcMaterial;
            }
        }

        /// ================================================================================
        /// <summary>S柱丸棒 柱種別</summary>
        /// ================================================================================
        public
        string MColumnSyubetsu
        {
            get
            {
                return _MColumnSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱丸棒 直径</summary>
        /// ================================================================================
        public
        string MColumnDiameter
        {
            get
            {
                return _MColumnDiameter;
            }
        }

        /// ================================================================================
        /// <summary>S柱丸棒 符号</summary>
        /// ================================================================================
        public
        string MColumnFugo
        {
            get
            {
                return _MColumnFugo;
            }
        }

        /// ================================================================================
        /// <summary>S柱T形鋼 ウェブマテリアル</summary>
        /// ================================================================================
        public
        string TColumnWebMat
        {
            get
            {
                return _TColumnWebMat;
            }
        }

        /// ================================================================================
        /// <summary>S柱T形鋼 フランジマテリアル</summary>
        /// ================================================================================
        public
        string TColumnFlangeMat
        {
            get
            {
                return _TColumnFlangeMat;
            }
        }

        /// ================================================================================
        /// <summary>S柱T形鋼 柱種別</summary>
        /// ================================================================================
        public
        string TColumnSyubetsu
        {
            get
            {
                return _TColumnSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱T形鋼 柱せい</summary>
        /// ================================================================================
        public
        string TColumnSei
        {
            get
            {
                return _TColumnSei;
            }
        }

        /// ================================================================================
        /// <summary>S柱T形鋼 柱幅</summary>
        /// ================================================================================
        public
        string TColumnHaba
        {
            get
            {
                return _TColumnHaba;
            }
        }

        /// ================================================================================
        /// <summary>S柱T形鋼 ウェブ厚</summary>
        /// ================================================================================
        public
        string TColumnWebAtsu
        {
            get
            {
                return _TColumnWebAtsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱T形鋼 フランジ厚</summary>
        /// ================================================================================
        public
        string TColumnFlangeAtsu
        {
            get
            {
                return _TColumnFlangeAtsu;
            }
        }

        /// ================================================================================
        /// <summary>S柱T形鋼 符号</summary>
        /// ================================================================================
        public
        string TColumnFugo
        {
            get
            {
                return _TColumnFugo;
            }
        }

        /// ================================================================================
        /// <summary>S梁 始端 ウェブマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderWebMaterial_S
        {
            get
            {
                return _GirderWebMaterial_S;
            }
        }

        /// ================================================================================
        /// <summary>S梁 始端 フランジマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFlangeMaterial_S
        {
            get
            {
                return _GirderFlangeMaterial_S;
            }
        }

        /// ================================================================================
        /// <summary>S梁 中央 ウェブマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderWebMaterial_C
        {
            get
            {
                return _GirderWebMaterial_C;
            }
        }

        /// ================================================================================
        /// <summary>S梁 中央 フランジマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFlangeMaterial_C
        {
            get
            {
                return _GirderFlangeMaterial_C;
            }
        }

        /// ================================================================================
        /// <summary>S梁 終端 ウェブマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderWebMaterial_E
        {
            get
            {
                return _GirderWebMaterial_E;
            }
        }

        /// ================================================================================
        /// <summary>S梁 終端 フランジマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFlangeMaterial_E
        {
            get
            {
                return _GirderFlangeMaterial_E;
            }
        }

        /// ================================================================================
        /// <summary>S梁 梁種別</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderSyubetsu
        {
            get
            {
                return _GirderSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S梁 始端 梁せい</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderSei_S
        {
            get
            {
                return _GirderSei_S;
            }
        }

        /// ================================================================================
        /// <summary>S梁 始端 梁幅</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderHaba_S
        {
            get
            {
                return _GirderHaba_S;
            }
        }

        /// ================================================================================
        /// <summary>S梁 始端 ウェブ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderWebAtsu_S
        {
            get
            {
                return _GirderWebAtsu_S;
            }
        }

        /// ================================================================================
        /// <summary>S梁 始端 フランジ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFlangeAtsu_S
        {
            get
            {
                return _GirderFlangeAtsu_S;
            }
        }

        /// ================================================================================
        /// <summary>S梁 始端 フィレット</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFillet_S
        {
            get
            {
                return _GirderFillet_S;
            }
        }

        /// ================================================================================
        /// <summary>S梁 中央 梁せい</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderSei_C
        {
            get
            {
                return _GirderSei_C;
            }
        }

        /// ================================================================================
        /// <summary>S梁 中央 梁幅</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderHaba_C
        {
            get
            {
                return _GirderHaba_C;
            }
        }

        /// ================================================================================
        /// <summary>S梁 中央 ウェブ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderWebAtsu_C
        {
            get
            {
                return _GirderWebAtsu_C;
            }
        }

        /// ================================================================================
        /// <summary>S梁 中央 フランジ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFlangeAtsu_C
        {
            get
            {
                return _GirderFlangeAtsu_C;
            }
        }

        /// ================================================================================
        /// <summary>S梁 中央 フィレット</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFillet_C
        {
            get
            {
                return _GirderFillet_C;
            }
        }

        /// ================================================================================
        /// <summary>S梁 終端 梁せい</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderSei_E
        {
            get
            {
                return _GirderSei_E;
            }
        }

        /// ================================================================================
        /// <summary>S梁 終端 梁幅</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderHaba_E
        {
            get
            {
                return _GirderHaba_E;
            }
        }

        /// ================================================================================
        /// <summary>S梁 終端 ウェブ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderWebAtsu_E
        {
            get
            {
                return _GirderWebAtsu_E;
            }
        }

        /// ================================================================================
        /// <summary>S梁 終端 フランジ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFlangeAtsu_E
        {
            get
            {
                return _GirderFlangeAtsu_E;
            }
        }

        /// ================================================================================
        /// <summary>S梁 終端 フィレット</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFillet_E
        {
            get
            {
                return _GirderFillet_E;
            }
        }

        /// ================================================================================
        /// <summary>S梁 始端 ハンチ長さ</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderHaunchNagasa_S
        {
            get
            {
                return _GirderHaunchNagasa_S;
            }
        }

        /// ================================================================================
        /// <summary>S梁 終端 ハンチ長さ</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderHaunchNagasa_E
        {
            get
            {
                return _GirderHaunchNagasa_E;
            }
        }

        /// ================================================================================
        /// <summary>S梁 符号</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderFugo
        {
            get
            {
                return _GirderFugo;
            }
        }

        /// ================================================================================
        /// <summary>S梁 始端 BH</summary>
        /// <history>2017/06/20 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderBH_S
        {
            get
            {
                return _GirderBH_S;
            }
        }

        /// ================================================================================
        /// <summary>S梁 終端 BH</summary>
        /// <history>2017/06/20 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string GirderBH_E
        {
            get
            {
                return _GirderBH_E;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 元端 ウェブマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderWebMaterial_S
        {
            get
            {
                return _CantiGirderWebMaterial_S;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 元端 フランジマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderFlangeMaterial_S
        {
            get
            {
                return _CantiGirderFlangeMaterial_S;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 先端 ウェブマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderWebMaterial_E
        {
            get
            {
                return _CantiGirderWebMaterial_E;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 先端 フランジマテリアル</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderFlangeMaterial_E
        {
            get
            {
                return _CantiGirderFlangeMaterial_E;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 梁種別</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderSyubetsu
        {
            get
            {
                return _CantiGirderSyubetsu;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 元端 梁せい</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderSei_S
        {
            get
            {
                return _CantiGirderSei_S;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 元端 梁幅</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderHaba_S
        {
            get
            {
                return _CantiGirderHaba_S;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 元端 ウェブ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderWebAtsu_S
        {
            get
            {
                return _CantiGirderWebAtsu_S;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 元端 フランジ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderFlangeAtsu_S
        {
            get
            {
                return _CantiGirderFlangeAtsu_S;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 元端 フィレット</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderFillet_S
        {
            get
            {
                return _CantiGirderFillet_S;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 先端 梁せい</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderSei_E
        {
            get
            {
                return _CantiGirderSei_E;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 先端 梁幅</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderHaba_E
        {
            get
            {
                return _CantiGirderHaba_E;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 先端 ウェブ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderWebAtsu_E
        {
            get
            {
                return _CantiGirderWebAtsu_E;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 先端 フランジ厚</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderFlangeAtsu_E
        {
            get
            {
                return _CantiGirderFlangeAtsu_E;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 先端 フィレット</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderFillet_E
        {
            get
            {
                return _CantiGirderFillet_E;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 符号</summary>
        /// <history>2016/08/24 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderFugo
        {
            get
            {
                return _CantiGirderFugo;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 始端 BH</summary>
        /// <history>2017/06/20 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderBH_S
        {
            get
            {
                return _CantiGirderBH_S;
            }
        }

        /// ================================================================================
        /// <summary>S片持ち梁 終端 BH</summary>
        /// <history>2017/06/20 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        string CantiGirderBH_E
        {
            get
            {
                return _CantiGirderBH_E;
            }
        }

        /// ================================================================================
        /// <summary>Column Mark </summary>
        /// <history>2020/02/04 Created AT</history>
        /// ================================================================================
        public
        string ColumnMark
        {
            get
            {
                return _ColumnMark;
            }
        }

        /// ================================================================================
        /// <summary>Beam Mark </summary>
        /// <history>2020/02/04 Created AT</history>
        /// ================================================================================
        public
        string GirderMark_S
        {
            get
            {
                return _GirderMark_S;
            }
        }

        /// ================================================================================
        /// <summary>Beam Mark </summary>
        /// <history>2020/02/04 Created AT</history>
        /// ================================================================================
        public
        string GirderMark_C
        {
            get
            {
                return _GirderMark_C;
            }
        }

        /// ================================================================================
        /// <summary>Beam Mark </summary>
        /// <history>2020/02/04 Created AT</history>
        /// ================================================================================
        public
        string GirderMark_E
        {
            get
            {
                return _GirderMark_E;
            }
        }

        /// ================================================================================
        /// <summary>Beam Mark </summary>
        /// <history>2020/02/04 Created AT</history>
        /// ================================================================================
        public
        string CantiGirderMark_S
        {
            get
            {
                return _CantiGirderMark_S;
            }
        }

        /// ================================================================================
        /// <summary>Beam Mark </summary>
        /// <history>2020/02/04 Created AT</history>
        /// ================================================================================
        public
        string CantiGirderMark_E
        {
            get
            {
                return _CantiGirderMark_E;
            }
        }

        public
        string SColumnRectT2
        {
            get
            {
                return _SColumnRectT2;
            }
        }

        // S梁・ブレース山形鋼
        public
        string LGirderMaterial
        {
            get
            {
                return _LGirderMaterial;
            }
        }

        public
        string LGirderHashiyubetsu
        {
            get
            {
                return _LGirderHashiyubetsu;
            }
        }

        public
        string LGirderSei_C
        {
            get
            {
                return _LGirderSei_C;
            }
        }

        public
        string LGirderHaba_C
        {
            get
            {
                return _LGirderHaba_C;
            }
        }

        public
        string LGirderDirThick_C
        {
            get
            {
                return _LGirderDirThick_C;
            }
        }

        public
        string LGirderWidthThick_C
        {
            get
            {
                return _LGirderWidthThick_C;
            }
        }

        public
        string LGirderFugo
        {
            get
            {
                return _LGirderFugo;
            }
        }

        // S梁・ブレース溝形鋼
        public
        string UGirderMaterial
        {
            get
            {
                return _UGirderMaterial;
            }
        }

        public
        string UGirderHashiyubetsu
        {
            get
            {
                return _UGirderHashiyubetsu;
            }
        }

        public
        string UGirderSei_C
        {
            get
            {
                return _UGirderSei_C;
            }
        }

        public
        string UGirderHaba_C
        {
            get
            {
                return _UGirderHaba_C;
            }
        }

        public
        string UGirderWebAtsu_C
        {
            get
            {
                return _UGirderWebAtsu_C;
            }
        }

        public
        string UGirderFlangeAtsu_C
        {
            get
            {
                return _UGirderFlangeAtsu_C;
            }
        }

        public
        string UGirderFugo
        {
            get
            {
                return _UGirderFugo;
            }
        }

        // S梁・ブレースリップ溝形鋼
        public
        string CGirderMaterial
        {
            get
            {
                return _CGirderMaterial;
            }
        }

        public
        string CGirderHashiyubetsu
        {
            get
            {
                return _CGirderHashiyubetsu;
            }
        }

        public
        string CGirderSei_C
        {
            get
            {
                return _CGirderSei_C;
            }
        }

        public
        string CGirderHaba_C
        {
            get
            {
                return _CGirderHaba_C;
            }
        }

        public
        string CGirderLipLength_C
        {
            get
            {
                return _CGirderLipLength_C;
            }
        }

        public
        string CGirderThick_C
        {
            get
            {
                return _CGirderThick_C;
            }
        }

        public
        string CGirderFugo
        {
            get
            {
                return _CGirderFugo;
            }
        }

        // ブレースフラットバー
        public
        string FBGirderMaterial
        {
            get
            {
                return _FBGirderMaterial;
            }
        }

        public
        string FBGirderBraceType
        {
            get
            {
                return _FBGirderBraceType;
            }
        }

        public
        string FBGirderWidth
        {
            get
            {
                return _FBGirderWidth;
            }
        }

        public
        string FBGirderBoardThick
        {
            get
            {
                return _FBGirderBoardThick;
            }
        }

        public
        string FBGirderFugo
        {
            get
            {
                return _FBGirderFugo;
            }
        }

        // ブレース丸鋼
        public
        string MGirderMaterial
        {
            get
            {
                return _MGirderMaterial;
            }
        }

        public
        string MGirderBraceType
        {
            get
            {
                return _MGirderBraceType;
            }
        }

        public
        string MGirderDiameter
        {
            get
            {
                return _MGirderDiameter;
            }
        }

        public
        string MGirderFugo
        {
            get
            {
                return _MGirderFugo;
            }
        }

        // ブレース角形鋼管
        public
        string RectGirderMaterial
        {
            get
            {
                return _RectGirderMaterial;
            }
        }

        public
        string RectGirderBraceType
        {
            get
            {
                return _RectGirderBraceType;
            }
        }

        public
        string RectGirderSei
        {
            get
            {
                return _RectGirderSei;
            }
        }

        public
        string RectGirderHaba
        {
            get
            {
                return _RectGirderHaba;
            }
        }

        public
        string RectGirderDirThick
        {
            get
            {
                return _RectGirderDirThick;
            }
        }

        public
        string RectGirderDirWidth
        {
            get
            {
                return _RectGirderDirWidth;
            }
        }

        public
        string RectGirderFillet
        {
            get
            {
                return _RectGirderFillet;
            }
        }

        public
        string RectGirderFugo
        {
            get
            {
                return _RectGirderFugo;
            }
        }

        // ブレース円形鋼管

        public
        string PGirderMaterial
        {
            get
            {
                return _PGirderMaterial;
            }
        }

        public
        string PGirderBraceType
        {
            get
            {
                return _PGirderBraceType;
            }
        }

        public
        string PGirderDiameter
        {
            get
            {
                return _PGirderDiameter;
            }
        }

        public
        string PGirderItaatsu
        {
            get
            {
                return _PGirderItaatsu;
            }
        }

        public
        string PGirderFugo
        {
            get
            {
                return _PGirderFugo;
            }
        }

        // S梁カットティー
        public
        string TGirderMaterial
        {
            get
            {
                return _TGirderMaterial;
            }
        }

        public
        string TGirderBraceType
        {
            get
            {
                return _TGirderBraceType;
            }
        }

        public
        string TGirderSei
        {
            get
            {
                return _TGirderSei;
            }
        }

        public
        string TGirderHaba
        {
            get
            {
                return _TGirderHaba;
            }
        }

        public
        string TGirderWebAtsu
        {
            get
            {
                return _TGirderWebAtsu;
            }
        }

        public
        string TGirderFlangeAtsu
        {
            get
            {
                return _TGirderFlangeAtsu;
            }
        }

        public
        string TGirderFugo
        {
            get
            {
                return _TGirderFugo;
            }
        }

        // GIRDER MARK
        public
        string GirderMark
        {
            get
            {
                return _GirderMark;
            }
        }

        #endregion パラメータ名

        /// ================================================================================
        /// <summary>デフォルト設定パラメータ</summary>
        /// <history><p>2016/08/31 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2017/07/12 Modified CST,Co.Ltd Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string[] DefaultSettingParameter
        {
            get
            {
                string[] ret = new string[75];

                ret[0] = "30";
                ret[1] = "30";
                ret[2] = "2.5 mm Arial";
                ret[3] = "2mm Arial";
                ret[4] = "細線";
                ret[5] = "0";
                ret[6] = "15";
                ret[7] = "15";
                ret[8] = "15";
                ret[9] = "15";
                ret[10] = "階";
                ret[11] = "符号";
                ret[12] = "階";
                ret[13] = "F";
                ret[14] = "15";
                ret[15] = "12";
                ret[16] = "符号";
                ret[17] = "Default";
                ret[18] = "0";

                ret[20] = "45";
                ret[21] = "9";
                ret[22] = "0";
                ret[23] = "0";
                ret[24] = "1";
                ret[25] = "9";
                ret[26] = "9";
                ret[27] = "9";
                ret[28] = "0";
                //ret[29] = "";
                //ret[30] = "";
                //ret[31] = "";
                ret[32] = "1";
                ret[33] = "5";

                ret[35] = "45";
                ret[36] = "20";
                ret[37] = "9";
                ret[38] = "1";
                ret[39] = "1";

                ret[41] = "15";
                ret[42] = "70";
                ret[43] = "9";
                ret[44] = "1";
                ret[45] = "全断";
                ret[46] = "中央";
                ret[47] = "端部";
                ret[48] = "始端";
                ret[49] = "終端";
                ret[50] = "元端";
                ret[51] = "先端";
                ret[52] = "1";
                ret[53] = "9";
                ret[54] = "9";
                ret[55] = "9";
                ret[56] = "0";
                //ret[57] = "";
                //ret[58] = "";
                //ret[59] = "";
                ret[60] = "1";
                ret[61] = "5";

                ret[63] = "15";
                ret[64] = "60";
                ret[65] = "20";
                ret[66] = "9";
                ret[67] = "1";
                ret[68] = "1";

                ret[70] = "60";
                ret[71] = "20";
                ret[72] = "9";
                ret[73] = "1";
                ret[74] = "1";

                return ret;
            }
        }

        #endregion Properties
    }
}