using System;
using System.Linq;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Collections.Generic;
using SectionListSteel.Utils;
using Autodesk.Revit.DB;

namespace SectionListSteel.List
{
    /// ================================================================================
    /// <summary>リスト作成 - 柱</summary>
    /// ================================================================================
    internal class CreateColumnList
    {
        // メンバ変数
        #region Member Variables

        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        /// <summary>要素</summary>
        private SectionListSteel.Components.Elements _CmpElements;

        /// <summary>図形</summary>
        private SectionListSteel.Components.Geometry _CmpGeometry;

        /// <summary>パラメータ</summary>
        private SectionListSteel.Components.Parameters _CmpParameters;

        /// <summary>設定</summary>
        private SectionListSteel.Components.Settings _CmpSettings;

        /// <summary>サービス</summary>
        private SectionListSteel.Components.Service _CmpService;

        // 対象ファミリ
        /// <summary>鉄骨 H形鋼</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelHAry;

        /// <summary>鉄骨 角形鋼管</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelRectAry;

        /// <summary>鉄骨 鋼管</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelRoundAry;

        /// <summary>CFT 角形鋼管</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _CFTRectAry;

        /// <summary>CFT 鋼管</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _CFTRoundAry;

        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelLAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelUAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelCAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelFBAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelMAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelTAry;

        /// <summary>柱</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _Columns;

        /// <summary>間柱</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _Posts;

        // 設定値
        /// <summary>ビュー尺度</summary>
        private int _ViewScale;

        /// <summary>文字タイプ タイトル</summary>
        private Revit.DB.TextNoteType _TxtNoteTypeTitle;

        /// <summary>文字タイプ 鉄骨</summary>
        private Revit.DB.TextNoteType _TxtNoteTypeSteel;

        /// <summary>線種</summary>
        private Revit.DB.GraphicsStyle _GraStyleLine;

        /// <summary>タイトル表示 (0 = 2タイトル, 1 = 1タイトル)</summary>
        private int _ShowTitle;

        /// <summary>枠幅 2タイトル</summary>
        private double _FrameWidth2Title;

        /// <summary>枠高さ 2タイトル</summary>
        private double _FrameHeight2Title;

        /// <summary>枠幅 1タイトル</summary>
        private double _FrameWidth1Title;

        /// <summary>枠高さ 1タイトル</summary>
        private double _FrameHeight1Title;

        /// <summary>階表示枠タイトル</summary>
        private string _TitleLvlFrame;

        /// <summary>項目表示枠タイトル</summary>
        private string _TitleSymbolFrame;

        /// <summary>枠タイトル</summary>
        private string _TitleFrame;

        /// <summary>階表示枠接尾語</summary>
        private string _LvlEndword;

        /// <summary>枠幅 鉄骨サイズ</summary>
        private double _SteelFrameWidth;

        /// <summary>枠高さ 鉄骨サイズ</summary>
        private double _SteelFrameHeight;

        /// <summary>鋼材種表示</summary>
        private int _ShowSteel;

        /// <summary>充填コンクリートの設計基準強度表示</summary>
        private int _ShowConcrete;

        /// <summary>備考枠</summary>
        private int _ShowNote;

        /// <summary>Selected note index</summary>
        private int _SelectedNoteIndex;

        /// <summary>Dictionary contains the items of note</summary>
        private Dictionary<int, string> _DicNote_Items = null;

        /// <summary>リストの折り返し</summary>
        private int _NewLine;

        /// <summary>折り返しスパン</summary>
        private int _NewLineSpan;

        /// <summary>枠幅 枠タイトル 間柱</summary>
        private double _SubTitleFrameWidth;

        /// <summary>枠高さ 枠タイトル 間柱</summary>
        private double _SubTitleFrameHeight;

        /// <summary>枠タイトル 間柱</summary>
        private string _SubTitleFrame;

        /// <summary>枠幅 鉄骨サイズ 間柱</summary>
        private double _SubSteelFrameWidth;

        /// <summary>枠幅 備考 間柱</summary>
        private double _SubNoteFrameWidth;

        /// <summary>枠高さ 鉄骨サイズ 間柱</summary>
        private double _SubSteelFrameHeight;

        /// <summary>鋼材種表示 間柱</summary>
        private int _SubShowSteel;

        /// <summary>備考枠 間柱</summary>
        private int _SubShowNote;

        /// <summary>マテリアル違い</summary>
        private string _MaterialVary;

        private string _MaterialVaryT;

        #endregion Member Variables

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
        /// <param name="cmpService"    >サービス</param>
        ///
        /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        CreateColumnList(SectionListSteel.Components.Attribute cmpAttribute,
                         SectionListSteel.Components.Elements cmpElements,
                         SectionListSteel.Components.Geometry cmpGeometry,
                         SectionListSteel.Components.Parameters cmpParameters,
                         SectionListSteel.Components.Settings cmpSettings,
                         SectionListSteel.Components.Service cmpService)
        {
            // 初期化
            _CmpAttribute = cmpAttribute;
            _CmpElements = cmpElements;
            _CmpGeometry = cmpGeometry;
            _CmpParameters = cmpParameters;
            _CmpSettings = cmpSettings;
            _CmpService = cmpService;

            _MaterialVary = "";
            _MaterialVaryT = "";
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>作成</summary>
        ///
        /// <history><p>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/06/26 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string Create(bool isColumnType, bool isPostType)
        {
            string ret = "";

            // 設定値取得
            GetSettingValues();

            if (GetTarget(isColumnType, isPostType) == false)
            {
                ret = _CmpAttribute.ResourceText("IDS_ERR_NOTARGETCOLUMN");
                return ret;
            }

            // 柱、間柱の分割
            if (ColumnDivision(isColumnType, isPostType) == false)
            {
                ret = _CmpAttribute.ResourceText("IDS_ERR_NOTARGETCOLUMN");
                return ret;
            }

            // タイプ名重複確認
            string overlapTypeName = OverlapTypeName(isColumnType, isPostType);
            if (overlapTypeName != "")
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_TYPENAME") + "「" + overlapTypeName + "」" + _CmpAttribute.ResourceText("IDS_ERR_INDIFFERENTFAMILY"),
                                                     _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
            }

            // 間柱符号名重複確認
            string overlapSubs = OverlapSubs(isPostType);
            if (overlapSubs != "")
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_FUGONAME_SUBCOLUMN") + "「" + overlapSubs + "」" + _CmpAttribute.ResourceText("IDS_ERR_INDIFFERENT_SUB"),
                                                     _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
            }

            // 作成 柱リスト
            if (_Columns.Count > 0)
            {
                CreateListColumns(_Columns);
            }

            // 作成 間柱リスト
            if (_Posts.Count > 0)
            {
                CreateListSubs(_Posts);
            }
            //// 横書き
            //if (_Posts.Count > 0)
            //{
            //  CreateListColumns(_Posts);
            //}

            return ret;
        }

        /// ================================================================================
        /// <summary>作成 - 柱リスト</summary>
        ///
        /// <param name="columns">柱</param>
        ///
        /// <history><p>2017/06/21 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/07/31 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string CreateListColumns(Collections.Generic.IList<Revit.DB.FamilySymbol> columns)
        {
            string ret = "";

            // トランザクション
            Revit.DB.Transaction transac = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);

            // 作図ビュー
            Revit.DB.ViewPlan vp = _CmpElements.SetCreateListView(_ViewScale, 0);
            if (vp == null)
            {
                return ret;
            }

            Collections.Generic.IList<Revit.DB.FamilySymbol> steelHAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelRoundAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cftRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cftRoundAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            _CmpService.ColumnDivision(columns,
                                       ref steelHAry,
                                       ref steelRectAry,
                                       ref steelRoundAry,
                                       ref cftRectAry,
                                       ref cftRoundAry,
                                       ref steelLAry,
                                       ref steelUAry,
                                       ref steelCAry,
                                       ref steelFBAry,
                                       ref steelMAry,
                                       ref steelTAry);

            // SとCFTを分けずにデータテーブル化
            // 符号と階が同一の場合、先のものだけが登録される
            SectionListSteel.Entities.DtColumn entDtColumn = new SectionListSteel.Entities.DtColumn(_CmpAttribute,
                                                                                      _CmpElements,
                                                                                      _CmpGeometry,
                                                                                      _CmpParameters,
                                                                                      _CmpSettings);

            entDtColumn.GetData(steelHAry, 1);
            entDtColumn.GetData(steelRectAry, 2);
            entDtColumn.GetData(steelRoundAry, 3);
            entDtColumn.GetData(cftRectAry, 4);
            entDtColumn.GetData(cftRoundAry, 5);

            //////////////////////////////////////////////////////////////////////////
            entDtColumn.GetData(steelLAry, 6);
            entDtColumn.GetData(steelUAry, 7);
            entDtColumn.GetData(steelCAry, 8);
            entDtColumn.GetData(steelFBAry, 9);
            entDtColumn.GetData(steelMAry, 10);
            entDtColumn.GetData(steelTAry, 11);
            //////////////////////////////////////////////////////////////////////////

            System.Data.DataTable data = entDtColumn.Data;

            // 符号順序
            Collections.Generic.IList<string> fugoOrder = _CmpService.FugoOrder(data);

            // 全階
            Collections.Generic.IList<string> allLevel = GetLevelName(steelHAry, steelRectAry, steelRoundAry, cftRectAry, cftRoundAry, steelLAry, steelUAry, steelCAry, steelFBAry, steelMAry, steelTAry);
            // 階記号ソート
            Collections.Generic.IList<string> levelOrder = _CmpService.LevelOrder(allLevel);

            // 行列数
            int rowNum = levelOrder.Count;
            int colNum = fugoOrder.Count;

            if (rowNum == 0 || colNum == 0)
            {
                transac.Start("Remove View");
                _CmpElements.RemoveView(vp);
                transac.Commit();

                return ret;
            }

            // 折り返しごとの符号
            Collections.Generic.IList<Collections.Generic.IList<string>> fugoAry = _CmpService.FugoAryByNewLine(fugoOrder,
                                                                                                                _NewLine,
                                                                                                                _NewLineSpan);

            // 右下端
            Revit.DB.XYZ posRightEnd = new Revit.DB.XYZ();

            // 枠線
            Revit.DB.Line line = null;
            Collections.Generic.IList<Revit.DB.Curve> frameLines = new Collections.Generic.List<Revit.DB.Curve>();

            // タイトル文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteTitle = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 符号文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSymbol = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 鉄骨文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSteel = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();

            // 備考
            int showNote = 1;
            if (_ShowNote == 1)
            {
                showNote = 2;
            }

            // 柱開始位置
            Revit.DB.XYZ posStart = new Revit.DB.XYZ();

            foreach (Collections.Generic.IList<string> fAry in fugoAry)
            {
                colNum = fAry.Count;

                // タイトル枠
                #region

                Revit.DB.XYZ posTitleLT = posStart;
                Revit.DB.XYZ posTitleLB = null;
                Revit.DB.XYZ posTitleRT = null;
                Revit.DB.XYZ posTitleRB = null;

                // 2タイトル
                if (_ShowTitle == 0)
                {
                    posTitleLB = posTitleLT + new Revit.DB.XYZ(0, -_FrameHeight2Title, 0);
                    posTitleRT = posTitleLT + new Revit.DB.XYZ(_FrameWidth2Title, 0, 0);
                    posTitleRB = posTitleLT + new Revit.DB.XYZ(_FrameWidth2Title, -_FrameHeight2Title, 0);

                    line = _CmpGeometry.CreateBoundLine(posTitleLT, posTitleLB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posTitleLB, posTitleRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posTitleLT, posTitleRT);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posTitleRT, posTitleRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posTitleLT, posTitleRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    Revit.DB.XYZ posLvlTitle = _CmpGeometry.TriangleGravity2D(posTitleLT, posTitleLB, posTitleRB);
                    dicTextNoteTitle.Add(posLvlTitle, _TitleLvlFrame);

                    Revit.DB.XYZ posSymbolTitle = _CmpGeometry.TriangleGravity2D(posTitleLT, posTitleRT, posTitleRB);
                    dicTextNoteTitle.Add(posSymbolTitle, _TitleSymbolFrame);
                }
                // 1タイトル
                else if (_ShowTitle == 1)
                {
                    posTitleLB = posTitleLT + new Revit.DB.XYZ(0, -_FrameHeight1Title, 0);
                    posTitleRT = posTitleLT + new Revit.DB.XYZ(_FrameWidth1Title, 0, 0);
                    posTitleRB = posTitleLT + new Revit.DB.XYZ(_FrameWidth1Title, -_FrameHeight1Title, 0);

                    line = _CmpGeometry.CreateBoundLine(posTitleLT, posTitleLB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posTitleLB, posTitleRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posTitleLT, posTitleRT);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posTitleRT, posTitleRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    Revit.DB.XYZ posTitle = _CmpGeometry.Center2Point(posTitleLT, posTitleRB);
                    dicTextNoteTitle.Add(posTitle, _TitleFrame);
                }

                #endregion Member Functions

                // 符号枠
                #region

                Revit.DB.XYZ posSymbolLT = posTitleRT;
                Revit.DB.XYZ posSymbolLB = posTitleRB;
                Revit.DB.XYZ posSymbolRT = posSymbolLT + new Revit.DB.XYZ(_SteelFrameWidth, 0, 0);
                Revit.DB.XYZ posSymbolRB = posSymbolLB + new Revit.DB.XYZ(_SteelFrameWidth, 0, 0);

                for (int i = 0; i < colNum; ++i)
                {
                    Revit.DB.XYZ posSymbolLT2 = posSymbolLT + new Revit.DB.XYZ(_SteelFrameWidth * i, 0, 0);
                    Revit.DB.XYZ posSymbolLB2 = posSymbolLB + new Revit.DB.XYZ(_SteelFrameWidth * i, 0, 0);
                    Revit.DB.XYZ posSymbolRT2 = posSymbolRT + new Revit.DB.XYZ(_SteelFrameWidth * i, 0, 0);
                    Revit.DB.XYZ posSymbolRB2 = posSymbolRB + new Revit.DB.XYZ(_SteelFrameWidth * i, 0, 0);

                    line = _CmpGeometry.CreateBoundLine(posSymbolLT2, posSymbolRT2);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSymbolRT2, posSymbolRB2);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSymbolLB2, posSymbolRB2);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSymbolLT2, posSymbolRB2);
                    dicTextNoteSymbol.Add(posCenter, fAry[i]);
                }

                #endregion

                // 階枠
                #region

                Revit.DB.XYZ posLevelLT = posTitleLB;
                Revit.DB.XYZ posLevelLB = posTitleLB - new Revit.DB.XYZ(0, _SteelFrameHeight * showNote, 0);
                Revit.DB.XYZ posLevelRT = posTitleRB;
                Revit.DB.XYZ posLevelRB = posTitleRB - new Revit.DB.XYZ(0, _SteelFrameHeight * showNote, 0);

                for (int i = 0; i < rowNum; ++i)
                {
                    Revit.DB.XYZ posLevelLT2 = posLevelLT - new Revit.DB.XYZ(0, _SteelFrameHeight * i * showNote, 0);
                    Revit.DB.XYZ posLevelLB2 = posLevelLB - new Revit.DB.XYZ(0, _SteelFrameHeight * i * showNote, 0);
                    Revit.DB.XYZ posLevelRT2 = posLevelRT - new Revit.DB.XYZ(0, _SteelFrameHeight * i * showNote, 0);
                    Revit.DB.XYZ posLevelRB2 = posLevelRB - new Revit.DB.XYZ(0, _SteelFrameHeight * i * showNote, 0);

                    line = _CmpGeometry.CreateBoundLine(posLevelLT2, posLevelLB2);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posLevelLB2, posLevelRB2);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posLevelRT2, posLevelRB2);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    // 中央
                    Revit.DB.XYZ posMidTop = (posLevelLT2 + posLevelRT2) / 2;
                    Revit.DB.XYZ posMidBtm = (posLevelLB2 + posLevelRB2) / 2;

                    line = _CmpGeometry.CreateBoundLine(posMidTop, posMidBtm);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    // 階記号
                    Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posLevelLT2, posMidBtm);
                    dicTextNoteTitle.Add(posCenter, levelOrder[i] + _LvlEndword);

                    // 備考なし
                    if (showNote == 1)
                    {
                        // 断面
                        posCenter = _CmpGeometry.Center2Point(posMidTop, posLevelRB2);
                        dicTextNoteTitle.Add(posCenter, _CmpAttribute.ResourceText("IDS_TXT_DANMEN"));
                    }
                    // 備考あり
                    else if (showNote == 2)
                    {
                        Revit.DB.XYZ posMidMid = (posMidTop + posMidBtm) / 2;
                        Revit.DB.XYZ posRightMid = (posLevelRT2 + posLevelRB2) / 2;

                        line = _CmpGeometry.CreateBoundLine(posMidMid, posRightMid);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        // 断面
                        posCenter = _CmpGeometry.Center2Point(posMidTop, posRightMid);
                        dicTextNoteTitle.Add(posCenter, _CmpAttribute.ResourceText("IDS_TXT_DANMEN"));

                        // 備考
                        posCenter = _CmpGeometry.Center2Point(posMidMid, posLevelRB2);
                        dicTextNoteTitle.Add(posCenter, _CmpAttribute.ResourceText("IDS_TXT_NOTE"));
                    }
                }

                #endregion

                // 鉄骨サイズ
                #region

                for (int iCol = 0; iCol < colNum; ++iCol)
                {
                    for (int iRow = 0; iRow < rowNum; ++iRow)
                    {
                        Revit.DB.XYZ posSteelFrameLT = posTitleRB + new Revit.DB.XYZ(_SteelFrameWidth * iCol, -_SteelFrameHeight * iRow * showNote, 0);
                        Revit.DB.XYZ posSteelFrameLB = posSteelFrameLT + new Revit.DB.XYZ(0, -_SteelFrameHeight * showNote, 0);
                        Revit.DB.XYZ posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SteelFrameWidth, 0, 0);
                        Revit.DB.XYZ posSteelFrameRB = posSteelFrameLT + new Revit.DB.XYZ(_SteelFrameWidth, -_SteelFrameHeight * showNote, 0);

                        posRightEnd = posSteelFrameRB;

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        Revit.DB.XYZ posLeftMid = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLB);
                        Revit.DB.XYZ posRightMid = _CmpGeometry.Center2Point(posSteelFrameRT, posSteelFrameRB);

                        // 備考
                        if (showNote == 2)
                        {
                            line = _CmpGeometry.CreateBoundLine(posLeftMid, posRightMid);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                        }

                        // 符号
                        string fugo = fAry[iCol];
                        // 階
                        string kai = levelOrder[iRow];

                        System.Data.DataRow currentRow = null;

                        foreach (System.Data.DataRow row in data.Rows)
                        {
                            string rFugo = (string)row[_CmpAttribute.ResourceText("IDS_CN_FUGO")];
                            string rKai = (string)row[_CmpAttribute.ResourceText("IDS_CN_KAI")];

                            // 符号と階が一致
                            if (fugo == rFugo && kai == rKai)
                            {
                                currentRow = row;
                                break;
                            }
                        }

                        // 当該あり
                        if (currentRow != null)
                        {
                            Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLB);

                            if (showNote == 2)
                            {
                                posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posLeftMid);
                            }

                            // 柱種類
                            int columnType = (int)currentRow[_CmpAttribute.ResourceText("IDS_CN_TYPE")];

                            string txtNoteVal = "";

                            // 1 = 鉄骨 H形鋼
                            if (columnType == 1)
                            {
                                // 柱せい
                                string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_SEI")];

                                // 柱幅
                                string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_HABA")];

                                // ウェブ厚
                                string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_WEBATSU")];

                                // フランジ厚
                                string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_FLANGEATSU")];

                                // ウェブマテリアル
                                string webMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_WEBMATERIAL")];

                                // フランジマテリアル
                                string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_FLANGEMATERIAL")];

                                if (webMat != flangeMat)
                                {
                                    if (_MaterialVary == "")
                                    {
                                        _MaterialVary = kai + fugo;
                                    }
                                    else
                                    {
                                        _MaterialVary += ", " + kai + fugo;
                                    }
                                }

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                                    sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                                // 鋼材種を表示
                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + flangeMat + ")";
                                }
                            }
                            // 2 = 鉄骨 角形鋼管
                            else if (columnType == 2)
                            {
                                // 柱せい
                                string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_SEI")];

                                // 柱幅
                                string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_HABA")];

                                // 板厚
                                string itaAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_ITAATSU")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_STRUCTURALMATERIAL")];

                                // フィレット
                                string fillet = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_FILLET")];

                                //T2
                                string t2 = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_COLUMN_T2")];

                                if (fillet == "0")
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + sei + "x" + haba + "x" + itaAtsu + "x" + t2;
                                }
                                else
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + sei + "x" + haba + "x" + itaAtsu;
                                }

                                // 鋼材種を表示
                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            // 3 = 鉄骨 鋼管
                            else if (columnType == 3)
                            {
                                // 直径
                                string tyokkei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_TYOKKEI")];

                                // 板厚
                                string itaAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_ITAATSU")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_STRUCTURALMATERIAL")];

                                txtNoteVal = /*_CmpAttribute.ResourceText("IDS_TXT_MARK_ROUND")*/ (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + tyokkei + "x" + itaAtsu;

                                // 鋼材種を表示
                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            // 4 = CFT 角形鋼管
                            else if (columnType == 4)
                            {
                                // 柱せい
                                string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_SEI")];

                                // 柱幅
                                string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_HABA")];

                                // 板厚
                                string itaAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_ITAATSU")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_STRUCTURALMATERIAL")];

                                // コンクリートマテリアル
                                string concMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_CONCRETEMATERIAL")];

                                // フィレット
                                string fillet = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_FILLET")];

                                //T2
                                string t2 = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_T2")];

                                if (fillet == "0")
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + sei + "x" + haba + "x" + itaAtsu + "x" + t2;
                                }
                                else
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + sei + "x" + haba + "x" + itaAtsu;
                                }

                                // 充填コンクリートの設計基準強度を表示
                                if (_ShowConcrete == 1)
                                {
                                    txtNoteVal += " [" + concMat + "]";
                                }

                                // 鋼材種を表示
                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            // 5 = CFT 鋼管
                            else if (columnType == 5)
                            {
                                // 直径
                                string tyokkei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_TYOKKEI")];

                                // 板厚
                                string itaAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_ITAATSU")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_STRUCTURALMATERIAL")];

                                // コンクリートマテリアル
                                string concMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_CONCRETEMATERIAL")];

                                txtNoteVal = /*_CmpAttribute.ResourceText("IDS_TXT_MARK_ROUND_FILL")*/(string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + tyokkei + "x" + itaAtsu;

                                // 充填コンクリートの設計基準強度を表示
                                if (_ShowConcrete == 1)
                                {
                                    txtNoteVal += " [" + concMat + "]";
                                }

                                // 鋼材種を表示
                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            ////////
                            else if (columnType == 6)
                            {
                                // 柱せい
                                string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_SEI")];

                                // 柱幅
                                string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_HABA")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_MATERIAL")];

                                string directionThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_DIRTHICK")];

                                string widthThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_WTHICK")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                                    sei + "x" + haba + "x" + directionThickness + "x" + widthThickness;

                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (columnType == 7)
                            {
                                // 柱せい
                                string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_SEI")];

                                // 柱幅
                                string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_HABA")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_MATERIAL")];

                                // ウェブ厚
                                string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_WEBATSU")];

                                // フランジ厚
                                string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_FLANGEATSU")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                                    sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (columnType == 8)
                            {
                                // 柱せい
                                string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_SEI")];

                                // 柱幅
                                string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_HABA")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_MATERIAL")];

                                string directionThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_LIPLENGTH")];

                                string thickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_BOARDTHICK")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                                    sei + "x" + haba + "x" + directionThickness + "x" + thickness;

                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (columnType == 9)
                            {
                                string width = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_WIDTH")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_MATERIAL")];

                                string thickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_BOARDTHICK")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + width + "x" + thickness;

                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (columnType == 10)
                            {
                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_MATERIAL")];

                                string diameter = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_DIAMETER")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + diameter;

                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (columnType == 11)
                            {
                                // 柱せい
                                string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_SEI")];

                                // 柱幅
                                string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_HABA")];

                                // ウェブ厚
                                string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_WEBATSU")];

                                // フランジ厚
                                string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_FLANGEATSU")];

                                // ウェブマテリアル
                                string webMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_WEBMAT")];

                                // フランジマテリアル
                                string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_FLANGEMAT")];

                                if (webMat != flangeMat)
                                {
                                    if (_MaterialVaryT == "")
                                    {
                                        _MaterialVaryT = kai + fugo;
                                    }
                                    else
                                    {
                                        _MaterialVaryT += ", " + kai + fugo;
                                    }
                                }

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                                    sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                                // 鋼材種を表示
                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + flangeMat + ")";
                                }
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);
                        }
                        // 当該なし
                        else
                        {
                            // 斜線
                            if (showNote == 1)
                            {
                                line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameLB);
                                _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                            }
                            else if (showNote == 2)
                            {
                                line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posLeftMid);
                                _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                            }
                        }
                    }
                }

                #endregion

                if (_ShowTitle == 0)
                {
                    posStart = new Revit.DB.XYZ(0, posRightEnd.Y - _FrameHeight2Title, 0);
                }
                if (_ShowTitle == 1)
                {
                    posStart = new Revit.DB.XYZ(0, posRightEnd.Y - _FrameHeight1Title, 0);
                }

                //Draw note
                if (_SelectedNoteIndex != 0 && _DicNote_Items.Count != 0)
                {
                    var startX = posStart.X;
                    var endX = posRightEnd.X;
                    var endY = posRightEnd.Y;
                    double sumHeight = 0;

                    NoteUtil.DrawNotes(_CmpGeometry, ref frameLines, ref dicTextNoteTitle, _ShowTitle, _DicNote_Items, _SelectedNoteIndex,
                        startX, endX, endY, _ViewScale, _FrameWidth2Title, _FrameWidth1Title, _SteelFrameWidth, colNum, out sumHeight);

                    posStart = new Revit.DB.XYZ(posStart.X, posStart.Y - sumHeight, posStart.Z);
                }

                //End draw note
            }

            transac.Start("Draw Frame");

            // 枠作図
            foreach (Revit.DB.Curve crv in frameLines)
            {
                _CmpElements.CreateDetailCurve(crv,
                                               vp,
                                               _GraStyleLine);
            }

            transac.Commit();

            // 文字作成
            #region 文字作成

            Revit.DB.XYZ baseVec = vp.RightDirection;
            Revit.DB.XYZ upVec = vp.UpDirection;

            double lineWidth = 0;

            // タイトル枠、階枠
            if (_ShowTitle == 0)
            {
                lineWidth = _FrameWidth2Title / _ViewScale;
            }
            else if (_ShowTitle == 1)
            {
                lineWidth = _FrameWidth1Title / _ViewScale;
            }

            foreach (Revit.DB.XYZ pos in dicTextNoteTitle.Keys)
            {
                string value = dicTextNoteTitle[pos];

                Revit.DB.TextNote txtNote = _CmpService.CreateNewTextNote(vp,
                                                                          pos,
                                                                          baseVec,
                                                                          lineWidth,
                                                                          _TxtNoteTypeTitle.Id,
                                                                          value,
                                                                          _CmpElements.RvtDBDoc,
                                                                          transac);
            }

            // 符号枠
            if (_ShowTitle == 0)
            {
                lineWidth = _SteelFrameWidth / _ViewScale;
            }
            else if (_ShowTitle == 1)
            {
                lineWidth = _SteelFrameWidth / _ViewScale;
            }

            foreach (Revit.DB.XYZ pos in dicTextNoteSymbol.Keys)
            {
                string value = dicTextNoteSymbol[pos];

                Revit.DB.TextNote txtNote = _CmpService.CreateNewTextNote(vp,
                                                                          pos,
                                                                          baseVec,
                                                                          lineWidth,
                                                                          _TxtNoteTypeTitle.Id,
                                                                          value,
                                                                          _CmpElements.RvtDBDoc,
                                                                          transac);
            }

            // 鉄骨サイズ枠
            foreach (Revit.DB.XYZ pos in dicTextNoteSteel.Keys)
            {
                string value = dicTextNoteSteel[pos];

                Revit.DB.TextNote txtNote = _CmpService.CreateNewTextNote(vp,
                                                                          pos,
                                                                          baseVec,
                                                                          lineWidth,
                                                                          _TxtNoteTypeSteel.Id,
                                                                          value,
                                                                          _CmpElements.RvtDBDoc,
                                                                          transac);

                _CmpService.MoveTextNote(txtNote, _ViewScale, vp, transac);
            }
            #endregion

            // ビューにフィット
            _CmpElements.ViewFit(vp,
                                 new Revit.DB.XYZ(),
                                 posRightEnd);

            if (transac.GetStatus() == Revit.DB.TransactionStatus.Started)
            {
                transac.Commit();
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>作成 - 間柱リスト</summary>
        ///
        /// <param name="subColumns">間柱</param>
        ///
        /// <history><p>2017/06/23 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/07/31 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string CreateListSubs(Collections.Generic.IList<Revit.DB.FamilySymbol> subColumns)
        {
            string ret = "";

            // トランザクション
            Revit.DB.Transaction transac = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);

            // 作図ビュー
            Revit.DB.ViewPlan vp = _CmpElements.SetCreateListView(_ViewScale, 1);
            if (vp == null)
            {
                return ret;
            }

            Collections.Generic.IList<Revit.DB.FamilySymbol> steelHAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelRoundAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cftRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cftRoundAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            _CmpService.ColumnDivision(subColumns,
                                       ref steelHAry,
                                       ref steelRectAry,
                                       ref steelRoundAry,
                                       ref cftRectAry,
                                       ref cftRoundAry,
                                       ref steelLAry,
                                       ref steelUAry,
                                       ref steelCAry,
                                       ref steelFBAry,
                                       ref steelMAry,
                                       ref steelTAry);

            // SとCFTを分けずにデータテーブル化
            // 符号と階が同一の場合、先のものだけが登録される
            SectionListSteel.Entities.DtColumn entDtColumn = new SectionListSteel.Entities.DtColumn(_CmpAttribute,
                                                                                      _CmpElements,
                                                                                      _CmpGeometry,
                                                                                      _CmpParameters,
                                                                                      _CmpSettings);

            entDtColumn.GetData(steelHAry, 1);
            entDtColumn.GetData(steelRectAry, 2);
            entDtColumn.GetData(steelRoundAry, 3);
            entDtColumn.GetData(cftRectAry, 4);
            entDtColumn.GetData(cftRoundAry, 5);

            //////////////////////////////////////////////////////////////////////////
            entDtColumn.GetData(steelLAry, 6);
            entDtColumn.GetData(steelUAry, 7);
            entDtColumn.GetData(steelCAry, 8);
            entDtColumn.GetData(steelFBAry, 9);
            entDtColumn.GetData(steelMAry, 10);
            entDtColumn.GetData(steelTAry, 11);
            //////////////////////////////////////////////////////////////////////////

            System.Data.DataTable data = entDtColumn.Data;

            // 符号順序
            Collections.Generic.IList<string> fugoOrder = _CmpService.FugoOrder(data);

            // 行数
            int numRow = fugoOrder.Count;

            if (numRow == 0)
            {
                transac.Start("Remove View");
                _CmpElements.RemoveView(vp);
                transac.Commit();

                return ret;
            }

            // 枠線
            Revit.DB.Line line = null;
            Collections.Generic.IList<Revit.DB.Curve> frameLines = new Collections.Generic.List<Revit.DB.Curve>();

            // タイトル文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteTitle = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 符号文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSymbol = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 鉄骨文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSteel = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();

            // 備考
            int showNote = 1;
            if (_SubShowNote == 1)
            {
                showNote = 2;
            }

            // タイトル枠
            Revit.DB.XYZ posLT = new Revit.DB.XYZ();
            Revit.DB.XYZ posLB = new Revit.DB.XYZ(0, -_SubTitleFrameHeight, 0);
            Revit.DB.XYZ posRT = new Revit.DB.XYZ(_SubTitleFrameWidth, 0, 0);
            Revit.DB.XYZ posRB = new Revit.DB.XYZ(_SubTitleFrameWidth, -_SubTitleFrameHeight, 0);

            line = _CmpGeometry.CreateBoundLine(posLT, posLB);
            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

            line = _CmpGeometry.CreateBoundLine(posLB, posRB);
            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

            line = _CmpGeometry.CreateBoundLine(posLT, posRT);
            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

            line = _CmpGeometry.CreateBoundLine(posRT, posRB);
            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

            Revit.DB.XYZ posTitle = _CmpGeometry.Center2Point(posLT, posRB);
            dicTextNoteTitle.Add(posTitle, _SubTitleFrame);

            // 断面枠
            posLT = posRT;
            posLB = posRB;
            posRT = posLT + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);
            posRB = posLB + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);

            line = _CmpGeometry.CreateBoundLine(posLT, posRT);
            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

            line = _CmpGeometry.CreateBoundLine(posLB, posRB);
            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

            line = _CmpGeometry.CreateBoundLine(posRT, posRB);
            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

            posTitle = _CmpGeometry.Center2Point(posLT, posRB);
            dicTextNoteTitle.Add(posTitle, _CmpAttribute.ResourceText("IDS_TXT_ELEMENTSECTION"));

            // 備考枠
            if (showNote == 2)
            {
                posLT = posRT;
                posLB = posRB;
                posRT = posLT + new Revit.DB.XYZ(_SubNoteFrameWidth, 0, 0);
                posRB = posLB + new Revit.DB.XYZ(_SubNoteFrameWidth, 0, 0);

                line = _CmpGeometry.CreateBoundLine(posLT, posRT);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posLB, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posRT, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                posTitle = _CmpGeometry.Center2Point(posLT, posRB);
                dicTextNoteTitle.Add(posTitle, _CmpAttribute.ResourceText("IDS_TXT_NOTE"));
            }

            Collections.Generic.IList<string> usedFugo = new Collections.Generic.List<string>();

            int rowNum = 0;

            // 符号ごとの断面
            for (int i = 0; i < fugoOrder.Count; ++i)
            {
                string fugo = fugoOrder[i];

                if (usedFugo.Contains(fugo))
                {
                    continue;
                }

                usedFugo.Add(fugo);

                System.Data.DataRow currentRow = null;

                foreach (System.Data.DataRow row in data.Rows)
                {
                    string rFugo = (string)row[_CmpAttribute.ResourceText("IDS_CN_FUGO")];

                    // 符号が一致
                    if (fugo == rFugo)
                    {
                        currentRow = row;
                        break;
                    }
                }

                if (currentRow == null)
                {
                    continue;
                }

                // 柱種類
                int columnType = (int)currentRow[_CmpAttribute.ResourceText("IDS_CN_TYPE")];

                string txtNoteSteel = "";

                #region 鉄骨サイズ

                // 1 = 鉄骨 H形鋼
                if (columnType == 1)
                {
                    // 柱せい
                    string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_SEI")];

                    // 柱幅
                    string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_HABA")];

                    // ウェブ厚
                    string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_WEBATSU")];

                    // フランジ厚
                    string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_FLANGEATSU")];

                    // ウェブマテリアル
                    string webMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_WEBMATERIAL")];

                    // フランジマテリアル
                    string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELH_FLANGEMATERIAL")];

                    if (webMat != flangeMat)
                    {
                        if (_MaterialVary == "")
                        {
                            _MaterialVary = fugo;
                        }
                        else
                        {
                            _MaterialVary += ", " + fugo;
                        }
                    }

                    txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                        sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                    // 鋼材種を表示
                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + flangeMat + ")";
                    }
                }
                // 2 = 鉄骨 角形鋼管
                else if (columnType == 2)
                {
                    // 柱せい
                    string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_SEI")];

                    // 柱幅
                    string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_HABA")];

                    // 板厚
                    string itaAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_ITAATSU")];

                    // 構造マテリアル
                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_STRUCTURALMATERIAL")];

                    // フィレット
                    string fillet = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELRECT_FILLET")];

                    //T2
                    string t2 = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_COLUMN_T2")];

                    if (fillet == "0")
                    {
                        txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + sei + "x" + haba + "x" + itaAtsu + "x" + t2;
                    }
                    else
                    {
                        txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + sei + "x" + haba + "x" + itaAtsu;
                    }

                    // 鋼材種を表示
                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + strcMat + ")";
                    }
                }
                // 3 = 鉄骨 鋼管
                else if (columnType == 3)
                {
                    // 直径
                    string tyokkei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_TYOKKEI")];

                    // 板厚
                    string itaAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_ITAATSU")];

                    // 構造マテリアル
                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_STEELROUND_STRUCTURALMATERIAL")];

                    txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + tyokkei + "x" + itaAtsu;

                    // 鋼材種を表示
                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + strcMat + ")";
                    }
                }
                // 4 = CFT 角形鋼管
                else if (columnType == 4)
                {
                    // 柱せい
                    string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_SEI")];

                    // 柱幅
                    string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_HABA")];

                    // 板厚
                    string itaAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_ITAATSU")];

                    // 構造マテリアル
                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_STRUCTURALMATERIAL")];

                    // コンクリートマテリアル
                    string concMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_CONCRETEMATERIAL")];

                    // フィレット
                    string fillet = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTRECT_FILLET")];

                    //T2
                    string t2 = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_T2")];

                    if (fillet == "0")
                    {
                        txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + sei + "x" + haba + "x" + itaAtsu + "x" + t2 /*+ " [" + concMat + "]"*/;
                    }
                    else
                    {
                        txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + sei + "x" + haba + "x" + itaAtsu /*+ " [" + concMat + "]"*/;
                    }

                    //// 充填コンクリートの設計基準強度を表示
                    //if (_ShowConcrete == 1)
                    //{
                    //  txtNoteSteel += " [" + concMat + "]";
                    //}

                    // 鋼材種を表示
                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + strcMat + ")";
                    }
                }
                // 5 = CFT 鋼管
                else if (columnType == 5)
                {
                    // 直径
                    string tyokkei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_TYOKKEI")];

                    // 板厚
                    string itaAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_ITAATSU")];

                    // 構造マテリアル
                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_STRUCTURALMATERIAL")];

                    // コンクリートマテリアル
                    string concMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CFTROUND_CONCRETEMATERIAL")];

                    txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + tyokkei + "x" + itaAtsu /*+ " [" + concMat + "]"*/;

                    //// 充填コンクリートの設計基準強度を表示
                    //if (_ShowConcrete == 1)
                    //{
                    //  txtNoteVal += " [" + concMat + "]";
                    //}

                    // 鋼材種を表示
                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + strcMat + ")";
                    }
                }

                //////////////////////////////////////////////////////////////////////////
                else if (columnType == 6)
                {
                    // 柱せい
                    string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_SEI")];

                    // 柱幅
                    string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_HABA")];

                    // 構造マテリアル
                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_MATERIAL")];

                    string directionThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_DIRTHICK")];

                    string widthThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LCOLUMN_WTHICK")];

                    txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                        sei + "x" + haba + "x" + directionThickness + "x" + widthThickness;

                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + strcMat + ")";
                    }
                }
                else if (columnType == 7)
                {
                    // 柱せい
                    string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_SEI")];

                    // 柱幅
                    string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_HABA")];

                    // 構造マテリアル
                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_MATERIAL")];

                    // ウェブ厚
                    string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_WEBATSU")];

                    // フランジ厚
                    string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UCOLUMN_FLANGEATSU")];

                    txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                        sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + strcMat + ")";
                    }
                }
                else if (columnType == 8)
                {
                    // 柱せい
                    string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_SEI")];

                    // 柱幅
                    string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_HABA")];

                    // 構造マテリアル
                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_MATERIAL")];

                    string directionThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_LIPLENGTH")];

                    string thickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CCOLUMN_BOARDTHICK")];

                    txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                        sei + "x" + haba + "x" + directionThickness + "x" + thickness;

                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + strcMat + ")";
                    }
                }
                else if (columnType == 9)
                {
                    string width = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_WIDTH")];

                    // 構造マテリアル
                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_MATERIAL")];

                    string thickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBCOLUMN_BOARDTHICK")];

                    txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + width + "x" + thickness;

                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + strcMat + ")";
                    }
                }
                else if (columnType == 10)
                {
                    // 構造マテリアル
                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_MATERIAL")];

                    string diameter = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MCOLUMN_DIAMETER")];

                    txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] + diameter;

                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + strcMat + ")";
                    }
                }
                else if (columnType == 11)
                {
                    // 柱せい
                    string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_SEI")];

                    // 柱幅
                    string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_HABA")];

                    // ウェブ厚
                    string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_WEBATSU")];

                    // フランジ厚
                    string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_FLANGEATSU")];

                    // ウェブマテリアル
                    string webMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_WEBMAT")];

                    // フランジマテリアル
                    string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TCOLUMN_FLANGEMAT")];

                    if (webMat != flangeMat)
                    {
                        if (_MaterialVaryT == "")
                        {
                            _MaterialVaryT = fugo;
                        }
                        else
                        {
                            _MaterialVaryT += ", " + fugo;
                        }
                    }

                    txtNoteSteel = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_MARK_H")] +
                        sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                    // 鋼材種を表示
                    if (_SubShowSteel == 1)
                    {
                        txtNoteSteel += " (" + flangeMat + ")";
                    }
                }

                //////////////////////////////////////////////////////////////////////////

                #endregion

                // 符号
                posLT = new Revit.DB.XYZ(0, -_SubTitleFrameHeight - _SubSteelFrameHeight * rowNum, 0);
                posLB = posLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0);
                posRT = posLT + new Revit.DB.XYZ(_SubTitleFrameWidth, 0, 0);
                posRB = posLB + new Revit.DB.XYZ(_SubTitleFrameWidth, 0, 0);

                line = _CmpGeometry.CreateBoundLine(posLT, posLB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posLB, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posRT, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                posTitle = _CmpGeometry.Center2Point(posLT, posRB);
                dicTextNoteSymbol.Add(posTitle, fugo);

                // 断面
                posLT = posRT;
                posLB = posRB;
                posRT = posLT + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);
                posRB = posLB + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);

                line = _CmpGeometry.CreateBoundLine(posLB, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posRT, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                posTitle = _CmpGeometry.Center2Point(posLT, posLB);
                dicTextNoteSteel.Add(posTitle, txtNoteSteel);

                // 備考
                if (showNote == 2)
                {
                    posLT = posRT;
                    posLB = posRB;
                    posRT = posLT + new Revit.DB.XYZ(_SubNoteFrameWidth, 0, 0);
                    posRB = posLB + new Revit.DB.XYZ(_SubNoteFrameWidth, 0, 0);

                    line = _CmpGeometry.CreateBoundLine(posLB, posRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posRT, posRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                }

                rowNum += 1;
            }

            transac.Start("Draw Frame");

            // 枠作図
            foreach (Revit.DB.Curve crv in frameLines)
            {
                _CmpElements.CreateDetailCurve(crv,
                                               vp,
                                               _GraStyleLine);
            }

            transac.Commit();

            // 文字作成
            #region 文字作成

            Revit.DB.XYZ baseVec = vp.RightDirection;
            Revit.DB.XYZ upVec = vp.UpDirection;

            // タイトル枠
            double lineWidth = 0;

            Collections.Generic.IList<Revit.DB.XYZ> posAry = dicTextNoteTitle.Keys.ToList<Revit.DB.XYZ>();

            for (int i = 0; i < dicTextNoteTitle.Count; ++i)
            {
                if (i == 0)
                {
                    lineWidth = _SubTitleFrameWidth / _ViewScale;
                }
                else if (i == 1)
                {
                    lineWidth = _SubSteelFrameWidth / _ViewScale;
                }
                else if (i == 1)
                {
                    lineWidth = _SubNoteFrameWidth / _ViewScale;
                }

                Revit.DB.XYZ pos = posAry[i];

                string value = dicTextNoteTitle[pos];

                Revit.DB.TextNote txtNote = _CmpService.CreateNewTextNote(vp,
                                                                          pos,
                                                                          baseVec,
                                                                          lineWidth,
                                                                          _TxtNoteTypeTitle.Id,
                                                                          value,
                                                                          _CmpElements.RvtDBDoc,
                                                                          transac);
            }

            // 符号枠
            lineWidth = _SubTitleFrameWidth / _ViewScale;

            foreach (Revit.DB.XYZ pos in dicTextNoteSymbol.Keys)
            {
                string value = dicTextNoteSymbol[pos];

                Revit.DB.TextNote txtNote = _CmpService.CreateNewTextNote(vp,
                                                                          pos,
                                                                          baseVec,
                                                                          lineWidth,
                                                                          _TxtNoteTypeTitle.Id,
                                                                          value,
                                                                          _CmpElements.RvtDBDoc,
                                                                          transac);
            }

            // 鉄骨サイズ枠
            lineWidth = _SubSteelFrameWidth / _ViewScale;

            foreach (Revit.DB.XYZ pos in dicTextNoteSteel.Keys)
            {
                string value = dicTextNoteSteel[pos];

                Revit.DB.TextNote txtNote = _CmpService.CreateNewTextNote(vp,
                                                                          pos,
                                                                          baseVec,
                                                                          lineWidth,
                                                                          _TxtNoteTypeSteel.Id,
                                                                          value,
                                                                          _CmpElements.RvtDBDoc,
                                                                          transac);

                _CmpService.MoveTextNote(txtNote, _ViewScale, vp, transac);
            }
            #endregion

            // ビューにフィット
            _CmpElements.ViewFit(vp,
                                 new Revit.DB.XYZ(),
                                 posRB);

            if (transac.GetStatus() == Revit.DB.TransactionStatus.Started)
            {
                transac.Commit();
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <history><p>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/06/22 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void GetSettingValues()
        {
            // ビュー尺度
            int.TryParse(_CmpParameters.ViewScaleColumn, out _ViewScale);

            // 文字タイプ
            _TxtNoteTypeTitle = _CmpElements.TxtNoteTypeByName(_CmpParameters.FontTitle);
            _TxtNoteTypeSteel = _CmpElements.TxtNoteTypeByName(_CmpParameters.FontSteel);

            // 線種
            _GraStyleLine = _CmpElements.GraStyleByName(_CmpParameters.LineType);

            // タイトル表示
            int.TryParse(_CmpParameters.ShowTitle, out _ShowTitle);

            // 枠サイズ
            double.TryParse(_CmpParameters.FrameWidth2Title, out _FrameWidth2Title);
            double.TryParse(_CmpParameters.FrameHeight2Title, out _FrameHeight2Title);
            double.TryParse(_CmpParameters.FrameWidth1Title, out _FrameWidth1Title);
            double.TryParse(_CmpParameters.FrameHeight1Title, out _FrameHeight1Title);
            double.TryParse(_CmpParameters.FrameWidthSub, out _SubTitleFrameWidth);
            double.TryParse(_CmpParameters.FrameHeightSub, out _SubTitleFrameHeight);

            _FrameWidth2Title = _FrameWidth2Title / 304.8 * _ViewScale;
            _FrameHeight2Title = _FrameHeight2Title / 304.8 * _ViewScale;
            _FrameWidth1Title = _FrameWidth1Title / 304.8 * _ViewScale;
            _FrameHeight1Title = _FrameHeight1Title / 304.8 * _ViewScale;
            _SubTitleFrameWidth = _SubTitleFrameWidth / 304.8 * _ViewScale;
            _SubTitleFrameHeight = _SubTitleFrameHeight / 304.8 * _ViewScale;

            // タイトル
            _TitleLvlFrame = _CmpParameters.TitleLvlFrame;
            _TitleSymbolFrame = _CmpParameters.TitleSymbolFrame;
            _TitleFrame = _CmpParameters.TitleFrame;
            _LvlEndword = _CmpParameters.LvlEndword;
            _SubTitleFrame = _CmpParameters.TitleFrameSub;

            // 枠サイズ 鉄骨サイズ
            double.TryParse(_CmpParameters.ColumnSteelFrameWidth, out _SteelFrameWidth);
            double.TryParse(_CmpParameters.ColumnSteelFrameHeight, out _SteelFrameHeight);

            double.TryParse(_CmpParameters.SubColumnSteelFrameWidth, out _SubSteelFrameWidth);
            double.TryParse(_CmpParameters.SubColumnNoteFrameWidth, out _SubNoteFrameWidth);
            double.TryParse(_CmpParameters.SubColumnSteelFrameHeight, out _SubSteelFrameHeight);

            _SteelFrameWidth = _SteelFrameWidth / 304.8 * _ViewScale;
            _SteelFrameHeight = _SteelFrameHeight / 304.8 * _ViewScale;
            _SubSteelFrameWidth = _SubSteelFrameWidth / 304.8 * _ViewScale;
            _SubNoteFrameWidth = _SubNoteFrameWidth / 304.8 * _ViewScale;
            _SubSteelFrameHeight = _SubSteelFrameHeight / 304.8 * _ViewScale;

            // 材質
            int.TryParse(_CmpParameters.ColumnShowSteel, out _ShowSteel);
            int.TryParse(_CmpParameters.ColumnShowConcrete, out _ShowConcrete);
            int.TryParse(_CmpParameters.SubColumnShowSteel, out _SubShowSteel);

            // 備考
            int.TryParse(_CmpParameters.ColumnShowNote, out _ShowNote);

            _DicNote_Items = new Dictionary<int, string>();

            int.TryParse(_CmpParameters.Column_SelectedNoteIndex, out _SelectedNoteIndex);
            _DicNote_Items.Add(1, _CmpParameters.Column_NoteName1 + "|" + _CmpParameters.Column_NoteHeight1);
            _DicNote_Items.Add(2, _CmpParameters.Column_NoteName2 + "|" + _CmpParameters.Column_NoteHeight2);
            _DicNote_Items.Add(3, _CmpParameters.Column_NoteName3 + "|" + _CmpParameters.Column_NoteHeight3);

            int.TryParse(_CmpParameters.SubColumnShowNote, out _SubShowNote);

            // リストの折り返し
            int.TryParse(_CmpParameters.ColumnNewLine, out _NewLine);
            int.TryParse(_CmpParameters.ColumnNewLineSpan, out _NewLineSpan);
        }

        /// ================================================================================
        /// <summary>対象ファミリ取得</summary>
        ///
        /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool GetTarget(bool isColumnType, bool isPostType)
        {
            // 戻り値
            bool ret = false;

            _SteelHAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelRoundAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _CFTRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _CFTRoundAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            _SteelLAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelUAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelCAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelFBAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelMAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelTAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> a = null;
            if (isColumnType)
                a = _Columns;
            else if (isPostType)
                a = _Posts;

            _CmpService.ColumnDivision(a,
                                       ref _SteelHAry,
                                       ref _SteelRectAry,
                                       ref _SteelRoundAry,
                                       ref _CFTRectAry,
                                       ref _CFTRoundAry,
                                       ref _SteelLAry,
                                       ref _SteelUAry,
                                       ref _SteelCAry,
                                       ref _SteelFBAry,
                                       ref _SteelMAry,
                                       ref _SteelTAry);

            if (_SteelHAry.Count > 0 ||
                _SteelRectAry.Count > 0 ||
                _SteelRoundAry.Count > 0 ||
                _CFTRectAry.Count > 0 ||
                _CFTRoundAry.Count > 0 ||
                _SteelLAry.Count > 0 ||
                _SteelUAry.Count > 0 ||
                _SteelCAry.Count > 0 ||
                _SteelFBAry.Count > 0 ||
                _SteelMAry.Count > 0 ||
                _SteelTAry.Count > 0

                )
            {
                ret = true;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>柱、間柱の分割</summary>
        ///
        /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool ColumnDivision(bool isColumnType, bool isPostType)
        {
            // 戻り値
            bool ret = false;

            _Columns = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _Posts = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            // 鉄骨 H形鋼
            foreach (Revit.DB.FamilySymbol famSym in _SteelHAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnHSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }

            // 鉄骨 角形鋼管
            foreach (Revit.DB.FamilySymbol famSym in _SteelRectAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnRectSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }

            // 鉄骨 鋼管
            foreach (Revit.DB.FamilySymbol famSym in _SteelRoundAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnRoundSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }

            // CFT 角形鋼管
            foreach (Revit.DB.FamilySymbol famSym in _CFTRectAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CFTColumnRectSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }

            // CFT 鋼管
            foreach (Revit.DB.FamilySymbol famSym in _CFTRoundAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CFTColumnRoundSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            foreach (Revit.DB.FamilySymbol famSym in _SteelLAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.LColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelUAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.UColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelCAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelFBAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.FBColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelMAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.MColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelTAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.TColumnSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_COLUMN") && isColumnType)
                {
                    _Columns.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST") && isPostType)
                {
                    _Posts.Add(famSym);
                }
            }
            //////////////////////////////////////////////////////////////////////////

            if (_Columns.Count > 0 || _Posts.Count > 0)
            {
                ret = true;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>違うファミリのタイプ名重複確認</summary>
        ///
        /// <history><p>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2016/10/13 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string OverlapTypeName(bool isColumnType, bool isPostType)
        {
            // 戻り値
            string ret = "";

            // 重複タイプ名
            Collections.Generic.IList<string> nameAry = new Collections.Generic.List<string>();

            Collections.Generic.IDictionary<long, Collections.Generic.IList<string>> dicIdName = new Collections.Generic.Dictionary<long, Collections.Generic.IList<string>>();

            if (isColumnType)
            {
                CheckOver(_Columns, dicIdName, ref nameAry);
            }
            if (isPostType)
            {
                CheckOver(_Posts, dicIdName, ref nameAry);
            }

            //if (isColumnType)
            //{
            //    // 鉄骨 H形鋼
            //    foreach (Revit.DB.FamilySymbol famSym in _SteelHAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }

            //    // 鉄骨 角形鋼管
            //    foreach (Revit.DB.FamilySymbol famSym in _SteelRectAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }

            //    // 鉄骨 鋼管
            //    foreach (Revit.DB.FamilySymbol famSym in _SteelRoundAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }

            //    // CFT 角形鋼管
            //    foreach (Revit.DB.FamilySymbol famSym in _CFTRectAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }

            //    // CFT 鋼管
            //    foreach (Revit.DB.FamilySymbol famSym in _CFTRoundAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }

            //    //////////////////////////////////////////////////////////////////////////
            //    foreach (Revit.DB.FamilySymbol famSym in _SteelLAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }

            //    foreach (Revit.DB.FamilySymbol famSym in _SteelUAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }
            //    foreach (Revit.DB.FamilySymbol famSym in _SteelCAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }
            //    foreach (Revit.DB.FamilySymbol famSym in _SteelFBAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }
            //    foreach (Revit.DB.FamilySymbol famSym in _SteelMAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }
            //    foreach (Revit.DB.FamilySymbol famSym in _SteelTAry)
            //    {
            //        string name = famSym.Name;

            //        Revit.DB.Family fam = famSym.Family;

            //        foreach (int id in dicIdName.Keys)
            //        {
            //            // 違うファミリ
            //            if (fam.Id.IntegerValue != id)
            //            {
            //                Collections.Generic.IList<string> value = dicIdName[id];

            //                // 同じタイプ名
            //                if (value.Contains(name))
            //                {
            //                    if (nameAry.Contains(name) == false)
            //                    {
            //                        nameAry.Add(name);
            //                    }
            //                }
            //            }
            //        }

            //        if (dicIdName.ContainsKey(fam.Id.IntegerValue))
            //        {
            //            if (dicIdName[fam.Id.IntegerValue].Contains(name) == false)
            //            {
            //                dicIdName[fam.Id.IntegerValue].Add(name);
            //            }
            //        }
            //        else
            //        {
            //            Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
            //            value.Add(name);

            //            dicIdName.Add(fam.Id.IntegerValue, value);
            //        }
            //    }
            //}
            //////////////////////////////////////////////////////////////////////////

            // 戻り値
            foreach (string f in nameAry)
            {
                if (ret == "")
                {
                    ret = f;
                }
                else
                {
                    ret += ", " + f;
                }
            }

            return ret;
        }

        private void CheckOver(IList<FamilySymbol> symbols,
            Collections.Generic.IDictionary<long, Collections.Generic.IList<string>> dicIdName,
            ref Collections.Generic.IList<string> nameAry)
        {
            foreach (Revit.DB.FamilySymbol famSym in symbols)
            {
                string name = famSym.Name;

                Revit.DB.Family fam = famSym.Family;

                foreach (long id in dicIdName.Keys)
                {
                    // 違うファミリ
                    if (fam.Id.Value != id)
                    {
                        Collections.Generic.IList<string> value = dicIdName[id];

                        // 同じタイプ名
                        if (value.Contains(name))
                        {
                            if (nameAry.Contains(name) == false)
                            {
                                nameAry.Add(name);
                            }
                        }
                    }
                }

                if (dicIdName.ContainsKey(fam.Id.Value))
                {
                    if (dicIdName[fam.Id.Value].Contains(name) == false)
                    {
                        dicIdName[fam.Id.Value].Add(name);
                    }
                }
                else
                {
                    Collections.Generic.IList<string> value = new Collections.Generic.List<string>();
                    value.Add(name);

                    dicIdName.Add(fam.Id.Value, value);
                }
            }
        }

        /// ================================================================================
        /// <summary>間柱符号名重複確認</summary>
        ///
        /// <history>2017/06/27 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string OverlapSubs(bool isPostType)
        {
            string ret = "";

            Collections.Generic.IDictionary<string, Collections.Generic.IList<string>> dicSameFugo = new Collections.Generic.Dictionary<string, Collections.Generic.IList<string>>();

            if (isPostType)
            {
                // 鉄骨 H形鋼
                foreach (Revit.DB.FamilySymbol famSym in _SteelHAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnHSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.SColumnHFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // 鉄骨 角形鋼管
                foreach (Revit.DB.FamilySymbol famSym in _SteelRectAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnRectSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.SColumnRectFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // 鉄骨 鋼管
                foreach (Revit.DB.FamilySymbol famSym in _SteelRoundAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.SColumnRoundSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.SColumnRoundFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // CFT 角形鋼管
                foreach (Revit.DB.FamilySymbol famSym in _CFTRectAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CFTColumnRectSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.CFTColumnRectFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // CFT 鋼管
                foreach (Revit.DB.FamilySymbol famSym in _CFTRoundAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CFTColumnRoundSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.CFTColumnRoundFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // S柱山形鋼
                foreach (Revit.DB.FamilySymbol famSym in _SteelLAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.LColumnSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.LColumnFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // S柱溝形鋼
                foreach (Revit.DB.FamilySymbol famSym in _SteelUAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.UColumnSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.UColumnFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // S柱リップ鋼
                foreach (Revit.DB.FamilySymbol famSym in _SteelCAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CColumnSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.CColumnFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // S柱リップ鋼
                foreach (Revit.DB.FamilySymbol famSym in _SteelFBAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.FBColumnSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.FBColumnFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // S柱丸棒
                foreach (Revit.DB.FamilySymbol famSym in _SteelMAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.MColumnSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.MColumnFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }

                // S柱T形鋼
                foreach (Revit.DB.FamilySymbol famSym in _SteelTAry)
                {
                    // 柱種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.TColumnSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 間柱
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.TColumnFugo);

                        string fugo = parFugo.AsString();

                        if (dicSameFugo.ContainsKey(fugo))
                        {
                            dicSameFugo[fugo].Add(famSym.Name);
                        }
                        else
                        {
                            Collections.Generic.IList<string> names = new Collections.Generic.List<string>();
                            names.Add(famSym.Name);

                            dicSameFugo.Add(fugo, names);
                        }
                    }
                }
            }

            foreach (string key in dicSameFugo.Keys)
            {
                Collections.Generic.IList<string> names = dicSameFugo[key];

                if (names.Count > 1)
                {
                    if (ret == "")
                    {
                        ret = key;
                    }
                    else
                    {
                        ret += ", " + key;
                    }
                }
            }

            return ret;
        }        

        /// ================================================================================
        /// <summary>レベル名取得</summary>
        ///
        /// <param name="steelHAry"     >鉄骨 H形鋼</param>
        /// <param name="steelRectAry"  >鉄骨 角形鋼管</param>
        /// <param name="steelRoundAry" >鉄骨 鋼管</param>
        /// <param name="cftRectAry"    >CFT 角形鋼管</param>
        /// <param name="cftRoundAry"   >CFT 鋼管</param>
        ///
        /// <history>2016/09/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> GetLevelName(Collections.Generic.IList<Revit.DB.FamilySymbol> steelHAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelRoundAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> cftRectAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> cftRoundAry,


                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry
                                                       )
        {

            Collections.Generic.List<string> ret = new Collections.Generic.List<string>();

            if (steelHAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelHAry)
                {
                    string fugoParamName = _CmpParameters.SColumnHFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }

            if (steelRectAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelRectAry)
                {
                    string fugoParamName = _CmpParameters.SColumnRectFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }

            if (steelRoundAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelRoundAry)
                {
                    string fugoParamName = _CmpParameters.SColumnRoundFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }

            if (cftRectAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in cftRectAry)
                {
                    string fugoParamName = _CmpParameters.CFTColumnRectFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }

            if (cftRoundAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in cftRoundAry)
                {
                    string fugoParamName = _CmpParameters.CFTColumnRoundFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }
            if (steelLAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelLAry)
                {
                    string fugoParamName = _CmpParameters.LColumnFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }
            if (steelUAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelUAry)
                {
                    string fugoParamName = _CmpParameters.UColumnFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }
            if (steelCAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelCAry)
                {
                    string fugoParamName = _CmpParameters.CColumnFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }
            if (steelFBAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelFBAry)
                {
                    string fugoParamName = _CmpParameters.FBColumnFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }
            if (steelMAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelMAry)
                {
                    string fugoParamName = _CmpParameters.MColumnFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }
            if (steelTAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelTAry)
                {
                    string fugoParamName = _CmpParameters.TColumnFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }


            // ソート
            ret.Sort();

            if (ret.Count > 1)
            {
                if (string.Compare(ret[0], ret[ret.Count - 1], false) < 0)
                {
                    ret.Reverse();
                }
            }

            return ret;
        }

        #endregion

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>マテリアル違い</summary>
        ///
        /// <history>2016/09/14 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        String MaterialVary
        {
            get
            {
                return _MaterialVary;
            }
        }

        public
        String MaterialVaryT
        {
            get
            {
                return _MaterialVaryT;
            }
        }

        #endregion
    }
}