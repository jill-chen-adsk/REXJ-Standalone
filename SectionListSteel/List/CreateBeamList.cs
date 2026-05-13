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
    /// <summary>リスト作成 - 梁</summary>
    /// ================================================================================
    internal class CreateBeamList
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
        /// <summary>梁</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _GirderAry;

        /// <summary>片持ち梁</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _CantiGirderAry;

        /// <summary>大梁</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _Girders;

        /// <summary>小梁</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _Beams;

        /// <summary>Braces</summary>
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _Braces;

        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelLAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelUAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelCAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelFBAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelMAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelTAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelRectAry;
        private Collections.Generic.IList<Revit.DB.FamilySymbol> _SteelPAry;

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

        /// <summary>枠幅 断面位置</summary>
        private double _SecFrameWidth;

        /// <summary>枠幅 鉄骨サイズ</summary>
        private double _SteelFrameWidth;

        /// <summary>枠高さ 鉄骨サイズ</summary>
        private double _SteelFrameHeight;

        /// <summary>鋼材種表示</summary>
        private int _ShowSteel;

        /// <summary>全断</summary>
        private string _SecZendan;

        /// <summary>中央</summary>
        private string _SecChuoh;

        /// <summary>端部</summary>
        private string _SecTanbu;

        /// <summary>始端</summary>
        private string _SecShitan;

        /// <summary>終端</summary>
        private string _SecSyutan;

        /// <summary>元端</summary>
        private string _SecMototan;

        /// <summary>先端</summary>
        private string _SecSentan;

        /// <summary>備考</summary>
        private int _ShowNote;

        /// <summary>Selected note index</summary>
        private int _SelectedNoteIndex;

        /// <summary>Dictionary contains the items of note</summary>
        private Dictionary<int, string> _DicNote_Items = null;

        /// <summary>リストの折り返し</summary>
        private int _NewLine;

        /// <summary>折り返しスパン</summary>
        private int _NewLineSpan;

        /// <summary>枠幅 枠タイトル 小梁</summary>
        private double _SubTitleFrameWidth;

        /// <summary>枠高さ 枠タイトル 小梁</summary>
        private double _SubTitleFrameHeight;

        /// <summary>枠タイトル 小梁</summary>
        private string _SubTitleFrame;

        /// <summary>枠幅 断面位置 小梁</summary>
        private double _SubSecFrameWidth;

        /// <summary>枠幅 鉄骨サイズ 小梁</summary>
        private double _SubSteelFrameWidth;

        /// <summary>枠幅 備考 小梁</summary>
        private double _SubNoteFrameWidth;

        /// <summary>枠高さ 鉄骨サイズ 小梁</summary>
        private double _SubSteelFrameHeight;

        /// <summary>鋼材種表示 小梁</summary>
        private int _SubShowSteel;

        /// <summary>備考 小梁</summary>
        private int _SubShowNote;

        /// <summary>マテリアル違い</summary>
        private string _MaterialVary;

        /// <summary>Brace steel frame width</summary>
        private double _BraceSteelFrameWidth;

        /// <summary>Brace note frame width</summary>
        private double _BraceNoteFrameWidth;

        /// <summary>Brace steel frame height</summary>
        private double _BraceSteelFrameHeight;

        /// <summary>Brace show steel</summary>
        private int _BraceShowSteel;

        /// <summary>Brace show note</summary>
        private int _BraceShowNote;

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
        CreateBeamList(SectionListSteel.Components.Attribute cmpAttribute,
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
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>作成</summary>
        ///
        /// <history><p>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/06/26 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string Create(bool isGirderType, bool isBeamType, bool isBrace)
        {
            string ret = "";

            GetSettingValues();

            // 対象なし
            if (GetTarget() == false)
            {
                ret = _CmpAttribute.ResourceText("IDS_ERR_NOTARGETGIRDER");
                return ret;
            }

            // 大梁、小梁の分割
            if (GirderDivision(isGirderType, isBeamType, isBrace) == false)
            {
                ret = _CmpAttribute.ResourceText("IDS_ERR_NOTARGETGIRDER");
                return ret;
            }

            // タイプ名重複確認
            string overlapTypeName = OverlapTypeName(isGirderType, isBeamType, isBrace);
            if (overlapTypeName != "")
            {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_TYPENAME") + "「" + overlapTypeName + "」" + _CmpAttribute.ResourceText("IDS_ERR_INDIFFERENTFAMILY"),
                                                     _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
            }

            // 小梁符号名重複確認

           
            if (isGirderType && isBeamType && isBrace)
            {
                string overlapSubs = OverlapSubs(false, true, false);
                if (overlapSubs != "")
                {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_FUGONAME_SUBGIRDER") + "「" + overlapSubs + "」" + _CmpAttribute.ResourceText("IDS_ERR_INDIFFERENT_SUB"),
                                                     _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
                }
                overlapSubs = OverlapSubs(false, false, true);
                if (overlapSubs != "")
                {

                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_FUGONAME_BRACE") + "「" + overlapSubs + "」" + _CmpAttribute.ResourceText("IDS_ERR_INDIFFERENT_SUB"),
                                                     _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
                }
            }
            else
            {
                if(isBeamType)
                {
                    string overlapSubs = OverlapSubs(false, true, false);
                    if (overlapSubs != "")
                    {
                        System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_FUGONAME_SUBGIRDER") + "「" + overlapSubs + "」" + _CmpAttribute.ResourceText("IDS_ERR_INDIFFERENT_SUB"),
                                                         _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
                    }
                }
                if(isBrace)
                {
                    string overlapSubs = OverlapSubs(false, false, true);
                    if (overlapSubs != "")
                    {

                        System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_TXT_FUGONAME_BRACE") + "「" + overlapSubs + "」" + _CmpAttribute.ResourceText("IDS_ERR_INDIFFERENT_SUB"),
                                                         _CmpAttribute.ResourceText("IDS_TXT_SECTIONLIST_S"));
                    }
                }


            }
            

            // 作成 大梁リスト
            if (_Girders.Count > 0)
            {
                CreateListGirders(_Girders);
            }

            // 作成 小梁リスト
            if (_Beams.Count > 0)
            {
                CreateListSubs(_Beams);
            }
            if (_Braces.Count > 0)
            {
                CreateBraceLists(_Braces);
            }

            //// 横書き
            //if (_Beams.Count > 0)
            //{
            //  CreateListGirders(_Beams);
            //}

            return ret;
        }

        /// ================================================================================
        /// <summary>作成 - 大梁リスト</summary>
        ///
        /// <param name="girders">大梁</param>
        ///
        /// <history><p>2017/06/21 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/08/01 Modified CST,Co.Ltd Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string CreateListGirders(Collections.Generic.IList<Revit.DB.FamilySymbol> girders)
        {
            string ret = "";

            Revit.DB.Transaction transac = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);

            if (girders.Count < 1)
            {
                return ret;
            }

            // 作図ビュー
            Revit.DB.ViewPlan vp = _CmpElements.SetCreateListView(_ViewScale, 2);
            if (vp == null)
            {
                return ret;
            }

            Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelPAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            // 梁、片持ち梁の振り分け
            _CmpService.GirderDivision(girders,
                           ref girderAry,
                           ref cantiGirderAry,
                           ref steelLAry,
                           ref steelUAry,
                           ref steelCAry,
                           ref steelFBAry,
                           ref steelMAry,
                           ref steelTAry,
                           ref steelRectAry,
                           ref steelPAry
                           );

            // 枠線
            Revit.DB.Line line = null;
            Collections.Generic.IList<Revit.DB.Curve> frameLines = new Collections.Generic.List<Revit.DB.Curve>();

            // タイトル文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteTitle = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 符号文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSymbol = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 断面位置文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSecName = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 鉄骨文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSteel = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();

            // 右下端
            Revit.DB.XYZ posRightEnd = new Revit.DB.XYZ();

            // 梁開始位置
            Revit.DB.XYZ posStart = new Revit.DB.XYZ();

            // 片持ち梁開始位置
            Revit.DB.XYZ posCantiStart = new Revit.DB.XYZ();

            double maxX = 0;

            double startX = 0;
            double endX = 0;

            double endY = 0;

            //////////////////////////////////////////////////////////////////////////

            // 梁
            #region

            // データテーブル化
            SectionListSteel.Entities.DtGirder entDtGirder = new SectionListSteel.Entities.DtGirder(_CmpAttribute,
                                                                                      _CmpElements,
                                                                                      _CmpGeometry,
                                                                                      _CmpParameters,
                                                                                      _CmpSettings);
            entDtGirder.GetData(girderAry, 1);

            entDtGirder.GetData(steelLAry, 3);
            entDtGirder.GetData(steelUAry, 4);
            entDtGirder.GetData(steelCAry, 5);
            entDtGirder.GetData(steelFBAry, 6);
            entDtGirder.GetData(steelMAry, 7);
            entDtGirder.GetData(steelTAry, 8);
            entDtGirder.GetData(steelRectAry, 9);
            entDtGirder.GetData(steelPAry, 10);

            System.Data.DataTable data = entDtGirder.Data;

            // 符号順序
            Collections.Generic.IList<string> fugoOrder = _CmpService.FugoOrder(data);

            // 全階
            Collections.Generic.IList<string> allLevel = GetLevelName(girderAry, steelLAry, steelUAry, steelCAry, steelFBAry, steelMAry, steelTAry, steelRectAry, steelPAry, null);
            // 階記号ソート
            Collections.Generic.IList<string> levelOrder = _CmpService.LevelOrder(allLevel);

            // 行列数
            int rowNum = levelOrder.Count;
            int colNum = fugoOrder.Count;

            bool flagGirder = false;
            if (rowNum == 0 || colNum == 0)
            {
                goto gotoCantiGirder;
            }
            flagGirder = true;

            // 折り返しごとの符号
            Collections.Generic.IList<Collections.Generic.IList<string>> fugoAry = _CmpService.FugoAryByNewLine(fugoOrder,
                                                                                                                _NewLine,
                                                                                                                _NewLineSpan);

            foreach (Collections.Generic.IList<string> fAry in fugoAry)
            {
                colNum = fAry.Count;

                // 階記号ごとの断面数 - 梁
                Collections.Generic.IDictionary<string, int> dicGirderSecNum = DicGirderSectionNumByLevel(levelOrder, data, fAry);

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
                Revit.DB.XYZ posSymbolRT = posSymbolLT + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth), 0, 0);
                Revit.DB.XYZ posSymbolRB = posSymbolLB + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth), 0, 0);

                for (int i = 0; i < colNum; ++i)
                {
                    Revit.DB.XYZ posSymbolLT2 = posSymbolLT + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * i, 0, 0);
                    Revit.DB.XYZ posSymbolLB2 = posSymbolLB + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * i, 0, 0);
                    Revit.DB.XYZ posSymbolRT2 = posSymbolRT + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * i, 0, 0);
                    Revit.DB.XYZ posSymbolRB2 = posSymbolRB + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * i, 0, 0);

                    line = _CmpGeometry.CreateBoundLine(posSymbolLT2, posSymbolRT2);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSymbolRT2, posSymbolRB2);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSymbolLB2, posSymbolRB2);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSymbolLT2, posSymbolRB2);
                    dicTextNoteSymbol.Add(posCenter, fAry[i]);

                    posRightEnd = posSymbolRB2;

                    maxX = maxX < posRightEnd.X ? posRightEnd.X : maxX;
                }

                #endregion

                // 階枠
                #region

                Revit.DB.XYZ posLevelLT = posTitleLB;
                Revit.DB.XYZ posLevelLB = null;
                Revit.DB.XYZ posLevelRT = posTitleRB;
                Revit.DB.XYZ posLevelRB = null;

                for (int i = 0; i < rowNum; ++i)
                {
                    // 断面数
                    string kai = levelOrder[i];
                    int secNum = dicGirderSecNum[kai];

                    posLevelLB = posLevelLT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);
                    posLevelRB = posLevelRT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);

                    if (_ShowNote == 1)
                    {
                        posLevelLB += new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);
                        posLevelRB += new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);
                    }

                    line = _CmpGeometry.CreateBoundLine(posLevelLT, posLevelLB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posLevelLB, posLevelRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posLevelRT, posLevelRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    // 線
                    Revit.DB.XYZ posMidTop = (posLevelLT + posLevelRT) / 2;
                    Revit.DB.XYZ posMidBtm = (posLevelLB + posLevelRB) / 2;

                    line = _CmpGeometry.CreateBoundLine(posMidTop, posMidBtm);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    // 階記号
                    Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posLevelLT, posMidBtm);
                    dicTextNoteTitle.Add(posCenter, kai + _LvlEndword);

                    // 備考なし
                    if (_ShowNote == 0)
                    {
                        // 断面
                        posCenter = _CmpGeometry.Center2Point(posMidTop, posLevelRB);
                        dicTextNoteTitle.Add(posCenter, _CmpAttribute.ResourceText("IDS_TXT_DANMEN"));
                    }
                    // 備考あり
                    else if (_ShowNote == 1)
                    {
                        Revit.DB.XYZ posMidMid = posMidTop + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);
                        Revit.DB.XYZ posRightMid = posLevelRT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);

                        line = _CmpGeometry.CreateBoundLine(posMidMid, posRightMid);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        // 断面
                        posCenter = _CmpGeometry.Center2Point(posMidTop, posRightMid);
                        dicTextNoteTitle.Add(posCenter, _CmpAttribute.ResourceText("IDS_TXT_DANMEN"));

                        // 備考
                        posCenter = _CmpGeometry.Center2Point(posMidMid, posLevelRB);
                        dicTextNoteTitle.Add(posCenter, _CmpAttribute.ResourceText("IDS_TXT_NOTE"));
                    }

                    posLevelLT = posLevelLB;
                    posLevelRT = posLevelRB;
                }

                #endregion

                // 断面位置、鉄骨サイズ枠
                #region

                Revit.DB.XYZ posSteelFrameLT = posTitleRB;
                Revit.DB.XYZ posSteelFrameLB = null;
                Revit.DB.XYZ posSteelFrameRT = null;
                Revit.DB.XYZ posSteelFrameRB = null;

                for (int iCol = 0; iCol < colNum; ++iCol)
                {
                    posSteelFrameLT = posTitleRB + new Autodesk.Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * iCol, 0, 0);

                    for (int iRow = 0; iRow < rowNum; ++iRow)
                    {
                        // 符号
                        string fugo = fAry[iCol];
                        // 階
                        string kai = levelOrder[iRow];

                        // 断面数
                        int secNum = dicGirderSecNum[kai];

                        posSteelFrameLB = posSteelFrameLT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);
                        posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SecFrameWidth + _SteelFrameWidth, 0, 0);
                        posSteelFrameRB = posSteelFrameLT + new Revit.DB.XYZ(_SecFrameWidth + _SteelFrameWidth, -_SteelFrameHeight * secNum, 0);

                        posRightEnd = posSteelFrameRB;

                        maxX = maxX < posRightEnd.X ? posRightEnd.X : maxX;

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
                            // 断面数
                            int girderType = (int)currentRow[_CmpAttribute.ResourceText("IDS_CN_TYPE")];

                            // 断面数
                            int currentSecNum = 1;
                            if (girderType == 1)
                                currentSecNum = GirderSectionNum(currentRow);

                            // 断面位置枠
                            Revit.DB.XYZ posSecFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SecFrameWidth, 0, 0);
                            Revit.DB.XYZ posSecFrameRB = posSecFrameRT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);

                            line = _CmpGeometry.CreateBoundLine(posSecFrameRT, posSecFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            string txtNoteVal = "";

                            if (girderType == 1)
                            {
                                // 断面数別
                                if (currentSecNum == 1)
                                {
                                    #region

                                    // 断面位置
                                    Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSecFrameRB);
                                    dicTextNoteSecName.Add(posCenter, _SecZendan);

                                    // 鉄骨サイズ
                                    posCenter = _CmpGeometry.Center2Point(posSecFrameRT, posSecFrameRB);

                                    // 梁せい
                                    string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                                    // 梁幅
                                    string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                                    // ウェブ厚
                                    string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                                    // フランジ厚
                                    string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                                    // フランジマテリアル
                                    string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + flangeMat + ")";
                                    }

                                    dicTextNoteSteel.Add(posCenter, txtNoteVal);

                                    #endregion
                                }
                                else if (currentSecNum == 2)
                                {
                                    #region

                                    // 区切り線
                                    Revit.DB.XYZ posMidL = posSteelFrameLT + new Revit.DB.XYZ(0, -(_SteelFrameHeight * secNum / 2), 0);
                                    Revit.DB.XYZ posMidR = posSteelFrameRT + new Revit.DB.XYZ(0, -(_SteelFrameHeight * secNum / 2), 0);

                                    line = _CmpGeometry.CreateBoundLine(posMidL, posMidR);
                                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                                    // 交点
                                    Revit.DB.XYZ posSec = posSecFrameRT + new Revit.DB.XYZ(0, -(_SteelFrameHeight * secNum / 2), 0);

                                    // 断面位置
                                    Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSec);
                                    dicTextNoteSecName.Add(posCenter, _SecTanbu);

                                    posCenter = _CmpGeometry.Center2Point(posMidL, posSecFrameRB);
                                    dicTextNoteSecName.Add(posCenter, _SecChuoh);

                                    // 鉄骨サイズ

                                    // 梁せい - 始端
                                    string sei_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S")];
                                    // 梁幅 - 始端
                                    string haba_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S")];
                                    // ウェブ厚 - 始端
                                    string webAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S")];
                                    // フランジ厚 - 始端
                                    string flangeAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S")];
                                    // フランジマテリアル - 始端
                                    string flangeMat_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                                    // ハンチ長さ - 始端
                                    string haunchNagasa_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S")];
                                    // フィレット - 始端
                                    string fillet_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S")];

                                    // 梁せい - 中央
                                    string sei_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                                    // 梁幅 - 中央
                                    string haba_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                                    // ウェブ厚 - 中央
                                    string webAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                                    // フランジ厚 - 中央
                                    string flangeAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                                    // フランジマテリアル - 中央
                                    string flangeMat_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                                    // 端部
                                    posCenter = _CmpGeometry.Center2Point(posSecFrameRT, posSec);

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;

                                    // ハンチ付き
                                    if (fillet_s == "0")
                                    {
                                        if (haunchNagasa_s != "0" && sei_s != sei_c)
                                        {
                                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "～" + sei_c + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;
                                        }
                                    }

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + flangeMat_s + ")";
                                    }

                                    dicTextNoteSteel.Add(posCenter, txtNoteVal);

                                    // 中央
                                    posCenter = _CmpGeometry.Center2Point(posSec, posSecFrameRB);

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei_c + "x" + haba_c + "x" + webAtsu_c + "x" + flangeAtsu_c;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + flangeMat_c + ")";
                                    }

                                    dicTextNoteSteel.Add(posCenter, txtNoteVal);

                                    #endregion
                                }
                                else if (currentSecNum == 3)
                                {
                                    #region

                                    // 区切り線
                                    Revit.DB.XYZ posMidL1 = posSteelFrameLT + new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);
                                    Revit.DB.XYZ posMidR1 = posSteelFrameRT + new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);

                                    line = _CmpGeometry.CreateBoundLine(posMidL1, posMidR1);
                                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                                    Revit.DB.XYZ posMidL2 = posSteelFrameLT + new Revit.DB.XYZ(0, -_SteelFrameHeight * 2, 0);
                                    Revit.DB.XYZ posMidR2 = posSteelFrameRT + new Revit.DB.XYZ(0, -_SteelFrameHeight * 2, 0);

                                    line = _CmpGeometry.CreateBoundLine(posMidL2, posMidR2);
                                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                                    // 交点
                                    Revit.DB.XYZ posSec1 = posSecFrameRT + new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);
                                    Revit.DB.XYZ posSec2 = posSecFrameRT + new Revit.DB.XYZ(0, -_SteelFrameHeight * 2, 0);

                                    // 断面位置
                                    Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSec1);
                                    dicTextNoteSecName.Add(posCenter, _SecShitan);

                                    posCenter = _CmpGeometry.Center2Point(posMidL1, posSec2);
                                    dicTextNoteSecName.Add(posCenter, _SecChuoh);

                                    posCenter = _CmpGeometry.Center2Point(posMidL2, posSecFrameRB);
                                    dicTextNoteSecName.Add(posCenter, _SecSyutan);

                                    // 鉄骨サイズ

                                    // 梁せい - 始端
                                    string sei_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S")];
                                    // 梁幅 - 始端
                                    string haba_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S")];
                                    // ウェブ厚 - 始端
                                    string webAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S")];
                                    // フランジ厚 - 始端
                                    string flangeAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S")];
                                    // フランジマテリアル - 始端
                                    string flangeMat_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                                    // ハンチ長さ - 始端
                                    string haunchNagasa_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S")];
                                    // フィレット - 始端
                                    string fillet_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S")];

                                    // 梁せい - 中央
                                    string sei_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                                    // 梁幅 - 中央
                                    string haba_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                                    // ウェブ厚 - 中央
                                    string webAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                                    // フランジ厚 - 中央
                                    string flangeAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                                    // フランジマテリアル - 中央
                                    string flangeMat_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                                    // 梁せい - 終端
                                    string sei_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_E")];
                                    // 梁幅 - 終端
                                    string haba_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_E")];
                                    // ウェブ厚 - 終端
                                    string webAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_E")];
                                    // フランジ厚 - 終端
                                    string flangeAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_E")];
                                    // フランジマテリアル - 終端
                                    string flangeMat_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")];
                                    // ハンチ長さ - 終端
                                    string haunchNagasa_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_E")];
                                    // フィレット - 終端
                                    string fillet_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_E")];

                                    // 始端
                                    posCenter = _CmpGeometry.Center2Point(posSecFrameRT, posSec1);

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;

                                    // ハンチ付き
                                    if (fillet_s == "0")
                                    {
                                        if (haunchNagasa_s != "0" && sei_s != sei_c)
                                        {
                                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "～" + sei_c + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;
                                        }
                                    }

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + flangeMat_s + ")";
                                    }

                                    dicTextNoteSteel.Add(posCenter, txtNoteVal);

                                    // 中央
                                    posCenter = _CmpGeometry.Center2Point(posSec1, posSec2);

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei_c + "x" + haba_c + "x" + webAtsu_c + "x" + flangeAtsu_c;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + flangeMat_c + ")";
                                    }

                                    dicTextNoteSteel.Add(posCenter, txtNoteVal);

                                    // 終端
                                    posCenter = _CmpGeometry.Center2Point(posSec2, posSecFrameRB);

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E")] + sei_e + "x" + haba_e + "x" + webAtsu_e + "x" + flangeAtsu_e;

                                    // ハンチ付き
                                    if (fillet_e == "0")
                                    {
                                        if (haunchNagasa_e != "0" && sei_e != sei_c)
                                        {
                                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E")] + sei_e + "～" + sei_c + "x" + haba_e + "x" + webAtsu_e + "x" + flangeAtsu_e;
                                        }
                                    }

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + flangeMat_e + ")";
                                    }

                                    dicTextNoteSteel.Add(posCenter, txtNoteVal);

                                    #endregion
                                }

                                // マテリアル違い
                                #region

                                // ウェブマテリアル 始端
                                string webMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_S")];
                                // フランジマテリアル 始端
                                string flangeMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                                // ウェブマテリアル 中央
                                string webMat_C = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_C")];
                                // フランジマテリアル 中央
                                string flangeMat_C = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];
                                // ウェブマテリアル 終端
                                string webMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_E")];
                                // フランジマテリアル 終端
                                string flangeMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")];

                                string secName = "";

                                if (webMat_S != flangeMat_S)
                                {
                                    secName = _SecShitan;
                                }
                                if (webMat_C != flangeMat_C)
                                {
                                    if (secName == "")
                                    {
                                        secName = _SecChuoh;
                                    }
                                    else
                                    {
                                        secName += ", " + _SecChuoh;
                                    }
                                }
                                if (webMat_E != flangeMat_E)
                                {
                                    if (secName == "")
                                    {
                                        secName = _SecSyutan;
                                    }
                                    else
                                    {
                                        secName += ", " + _SecSyutan;
                                    }
                                }

                                if (secName != "")
                                {
                                    if (_MaterialVary == "")
                                    {
                                        _MaterialVary = kai + fugo + " (" + secName + ")";
                                    }
                                    else
                                    {
                                        _MaterialVary += ", " + kai + fugo + " (" + secName + ")";
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                if (girderType == 3)
                                {
                                    // 梁せい
                                    var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderSei_C")];
                                    // 梁幅
                                    var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderHaba_C")];

                                    // 構造マテリアル
                                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGIRDERMAT")];

                                    var directionThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderDirThick_C")];

                                    //Width thickness
                                    var widthThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderWidthThick_C")];

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + directionThickness + "x" + widthThickness;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + strcMat + ")";
                                    }
                                }
                                else if (girderType == 4)
                                {
                                    // 梁せい
                                    var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderSei_C")];
                                    // 梁幅
                                    var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderHaba_C")];

                                    // 構造マテリアル
                                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGIRDERMAT")];

                                    // 中央 ウェブ厚
                                    var parWebAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderWebAtsu_C")];

                                    // 中央 フランジ厚
                                    var parFlangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderFlangeAtsu_C")];

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + parWebAtsu + "x" + parFlangeAtsu;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + strcMat + ")";
                                    }
                                }
                                else if (girderType == 5)
                                {
                                    // 梁せい
                                    var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderSei_C")];
                                    // 梁幅
                                    var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderHaba_C")];

                                    // 構造マテリアル
                                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGIRDERMAT")];

                                    var parLipLength = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderLipLength_C")];

                                    // 中央 板厚
                                    var parThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderThick_C")];

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + parLipLength + "x" + parThickness;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + strcMat + ")";
                                    }
                                }
                                else if (girderType == 6)
                                {
                                    // 構造マテリアル
                                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBGIRDERMAT")];

                                    // 幅
                                    var parWidth = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_WIDTH")];

                                    // 板厚
                                    var parBoardThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_THICK")];

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + parWidth + "x" + parBoardThickness;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + strcMat + ")";
                                    }
                                }
                                else if (girderType == 7)
                                {
                                    // 構造マテリアル
                                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MGIRDERMAT")];

                                    var parDiameter = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MGIRDER_DIAMETER")];

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + parDiameter;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + strcMat + ")";
                                    }
                                }
                                else if (girderType == 8)
                                {
                                    // 梁せい
                                    var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderSei_C")];
                                    // 梁幅
                                    var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderHaba_C")];

                                    // 構造マテリアル
                                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGIRDERMAT")];

                                    // 中央 ウェブ厚
                                    var parWebAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderWebAtsu_C")];

                                    // 中央 フランジ厚
                                    var parFlangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderFlangeAtsu_C")];

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + parWebAtsu + "x" + parFlangeAtsu;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + strcMat + ")";
                                    }
                                }
                                else if (girderType == 9)
                                {
                                    // 構造マテリアル
                                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDERMAT")];

                                    var SteelFrame = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_SEI")];

                                    // 鉄骨幅
                                    var SteelWFrame = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_HABA")];

                                    // せい方向の板厚
                                    var ThicknessDirect = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRTHICK")];

                                    // 幅方向の板厚
                                    var WidthDirect = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRWIDTH")];

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + SteelFrame + "x" + SteelWFrame + "x" + ThicknessDirect + "x" + WidthDirect;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + strcMat + ")";
                                    }
                                }
                                else if (girderType == 10)
                                {
                                    // 構造マテリアル
                                    string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_PGIRDERMAT")];

                                    // 直径
                                    var parDiameter = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_PGIRDER_DIAMETER")];

                                    // 板厚
                                    var parBoardThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_PGIRDER_ITAATSU")];

                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + parDiameter + "x" + parBoardThickness;

                                    // 鋼材種を表示
                                    if (_ShowSteel == 1)
                                    {
                                        txtNoteVal += " (" + strcMat + ")";
                                    }
                                }

                                // 断面位置
                                Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSecFrameRB);
                                dicTextNoteSecName.Add(posCenter, _SecZendan);

                                // 鉄骨サイズ
                                // 全断
                                posCenter = _CmpGeometry.Center2Point(posSecFrameRT, posSecFrameRB);
                                dicTextNoteSteel.Add(posCenter, txtNoteVal);
                            }
                        }
                        else
                        {
                            // 斜線
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameLB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                        }

                        posSteelFrameLT = posSteelFrameLB;

                        if (_ShowNote == 0)
                        {
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                        }
                        if (_ShowNote == 1)
                        {
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posSteelFrameLB += new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);
                            posSteelFrameRB += new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);

                            posRightEnd = posSteelFrameRB;

                            maxX = maxX < posRightEnd.X ? posRightEnd.X : maxX;

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posSteelFrameLT = posSteelFrameLB;
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
                    startX = posStart.X;
                    endX = posRightEnd.X;
                    endY = posRightEnd.Y;
                    double sumHeight = 0;

                    NoteUtil.DrawNotes(_CmpGeometry, ref frameLines, ref dicTextNoteTitle, _ShowTitle, _DicNote_Items, _SelectedNoteIndex, startX, endX, endY, _ViewScale,
                        _FrameWidth2Title, _FrameWidth1Title, _SecFrameWidth + _SteelFrameWidth, colNum, out sumHeight);

                    posStart = new Revit.DB.XYZ(posStart.X, posStart.Y - sumHeight, posStart.Z);
                }
                //End draw note
            }

            #endregion

            // goto - 片持ち梁
            gotoCantiGirder:

            // 片持ち梁
            #region

            // データテーブル化
            entDtGirder = new SectionListSteel.Entities.DtGirder(_CmpAttribute,
                                                          _CmpElements,
                                                          _CmpGeometry,
                                                          _CmpParameters,
                                                          _CmpSettings);
            entDtGirder.GetData(cantiGirderAry, 2);
            data = entDtGirder.Data;

            // 符号順序
            fugoOrder = _CmpService.FugoOrder(data);

            // 全階
            allLevel = GetLevelName(null, null, null, null, null, null, null, null, null, cantiGirderAry);
            // 階記号ソート
            levelOrder = _CmpService.LevelOrder(allLevel);

            // 行列数
            rowNum = levelOrder.Count;
            colNum = fugoOrder.Count;

            bool flagCanti = false;
            if (rowNum == 0 || colNum == 0)
            {
                goto gotoDraw;
            }
            flagCanti = true;

            fugoAry = _CmpService.FugoAryByNewLine(fugoOrder,
                                                   _NewLine,
                                                   _NewLineSpan);

            if (flagGirder)
            {
                if (_ShowTitle == 0)
                {
                    posCantiStart = new Revit.DB.XYZ(maxX + _FrameWidth2Title, 0, 0);
                }
                else if (_ShowTitle == 1)
                {
                    posCantiStart = new Revit.DB.XYZ(maxX + _FrameWidth1Title, 0, 0);
                }
            }

            foreach (Collections.Generic.IList<string> fAry in fugoAry)
            {
                colNum = fAry.Count;

                // 階記号ごとの断面数 - 片持ち梁
                Collections.Generic.IDictionary<string, int> dicCantiSecNum = DicCantiGirderSectionNumByLevel(levelOrder, data, fAry);

                // タイトル枠
                #region

                Revit.DB.XYZ posTitleLT = posCantiStart;
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

                #endregion

                // 符号枠
                #region

                Revit.DB.XYZ posSymbolLT = posTitleRT;
                Revit.DB.XYZ posSymbolLB = posTitleRB;
                Revit.DB.XYZ posSymbolRT = posSymbolLT + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth), 0, 0);
                Revit.DB.XYZ posSymbolRB = posSymbolLB + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth), 0, 0);

                for (int i = 0; i < colNum; ++i)
                {
                    Revit.DB.XYZ posSymbolLT2 = posSymbolLT + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * i, 0, 0);
                    Revit.DB.XYZ posSymbolLB2 = posSymbolLB + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * i, 0, 0);
                    Revit.DB.XYZ posSymbolRT2 = posSymbolRT + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * i, 0, 0);
                    Revit.DB.XYZ posSymbolRB2 = posSymbolRB + new Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * i, 0, 0);

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
                Revit.DB.XYZ posLevelLB = null;
                Revit.DB.XYZ posLevelRT = posTitleRB;
                Revit.DB.XYZ posLevelRB = null;

                for (int i = 0; i < rowNum; ++i)
                {
                    // 断面数
                    string kai = levelOrder[i];
                    int secNum = dicCantiSecNum[kai];

                    posLevelLB = posLevelLT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);
                    posLevelRB = posLevelRT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);

                    if (_ShowNote == 1)
                    {
                        posLevelLB += new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);
                        posLevelRB += new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);
                    }

                    line = _CmpGeometry.CreateBoundLine(posLevelLT, posLevelLB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posLevelLB, posLevelRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posLevelRT, posLevelRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    // 線
                    Revit.DB.XYZ posMidTop = (posLevelLT + posLevelRT) / 2;
                    Revit.DB.XYZ posMidBtm = (posLevelLB + posLevelRB) / 2;

                    line = _CmpGeometry.CreateBoundLine(posMidTop, posMidBtm);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    // 階記号
                    Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posLevelLT, posMidBtm);
                    dicTextNoteTitle.Add(posCenter, kai + _LvlEndword);

                    // 備考なし
                    if (_ShowNote == 0)
                    {
                        // 断面
                        posCenter = _CmpGeometry.Center2Point(posMidTop, posLevelRB);
                        dicTextNoteTitle.Add(posCenter, _CmpAttribute.ResourceText("IDS_TXT_DANMEN"));
                    }
                    // 備考あり
                    else if (_ShowNote == 1)
                    {
                        Revit.DB.XYZ posMidMid = posMidTop + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);
                        Revit.DB.XYZ posRightMid = posLevelRT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);

                        line = _CmpGeometry.CreateBoundLine(posMidMid, posRightMid);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        // 断面
                        posCenter = _CmpGeometry.Center2Point(posMidTop, posRightMid);
                        dicTextNoteTitle.Add(posCenter, _CmpAttribute.ResourceText("IDS_TXT_DANMEN"));

                        // 備考
                        posCenter = _CmpGeometry.Center2Point(posMidMid, posLevelRB);
                        dicTextNoteTitle.Add(posCenter, _CmpAttribute.ResourceText("IDS_TXT_NOTE"));
                    }

                    posLevelLT = posLevelLB;
                    posLevelRT = posLevelRB;
                }

                #endregion

                // 断面位置、鉄骨サイズ枠
                #region

                for (int iCol = 0; iCol < colNum; ++iCol)
                {
                    Revit.DB.XYZ posSteelFrameLT = posTitleRB + new Autodesk.Revit.DB.XYZ((_SecFrameWidth + _SteelFrameWidth) * iCol, 0, 0);

                    for (int iRow = 0; iRow < rowNum; ++iRow)
                    {
                        // 符号
                        string fugo = fAry[iCol];
                        // 階
                        string kai = levelOrder[iRow];

                        // 断面数
                        int secNum = dicCantiSecNum[kai];

                        Revit.DB.XYZ posSteelFrameLB = posSteelFrameLT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);
                        Revit.DB.XYZ posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SecFrameWidth + _SteelFrameWidth, 0, 0);
                        Revit.DB.XYZ posSteelFrameRB = posSteelFrameLT + new Revit.DB.XYZ(_SecFrameWidth + _SteelFrameWidth, -_SteelFrameHeight * secNum, 0);

                        posRightEnd = posSteelFrameRB;

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
                            // 当該断面数
                            int currentSecNum = CantiGirderSectionNum(currentRow);

                            // 断面位置枠
                            Revit.DB.XYZ posSecFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SecFrameWidth, 0, 0);
                            Revit.DB.XYZ posSecFrameRB = posSecFrameRT + new Revit.DB.XYZ(0, -_SteelFrameHeight * secNum, 0);

                            line = _CmpGeometry.CreateBoundLine(posSecFrameRT, posSecFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            string txtNoteVal = "";

                            // 断面数別
                            if (currentSecNum == 1)
                            {
                                #region

                                // 断面位置
                                Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSecFrameRB);
                                dicTextNoteSecName.Add(posCenter, _SecZendan);

                                // 鉄骨サイズ
                                // 全断
                                posCenter = _CmpGeometry.Center2Point(posSecFrameRT, posSecFrameRB);

                                // 梁せい
                                string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")];
                                // 梁幅
                                string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")];
                                // ウェブ厚
                                string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")];
                                // フランジ厚
                                string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")];
                                // ウェブマテリアル
                                string webMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")];
                                // フランジマテリアル
                                string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];

                                //if (webMat != flangeMat)
                                //{
                                //    if (_MaterialVary == "")
                                //    {
                                //        _MaterialVary = kai + fugo + " : " + _SecZendan;
                                //    }
                                //    else
                                //    {
                                //        _MaterialVary += ", " + kai + fugo + " : " + _SecZendan;
                                //    }
                                //}

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")] + sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                                // 鋼材種を表示
                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + flangeMat + ")";
                                }

                                dicTextNoteSteel.Add(posCenter, txtNoteVal);

                                #endregion
                            }
                            else if (currentSecNum == 2)
                            {
                                #region

                                // 区切り線
                                Revit.DB.XYZ posMidL = posSteelFrameLT + new Revit.DB.XYZ(0, -(_SteelFrameHeight * secNum / 2), 0);
                                Revit.DB.XYZ posMidR = posSteelFrameRT + new Revit.DB.XYZ(0, -(_SteelFrameHeight * secNum / 2), 0);

                                line = _CmpGeometry.CreateBoundLine(posMidL, posMidR);
                                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                                // 交点
                                Revit.DB.XYZ posSec = posSecFrameRT + new Revit.DB.XYZ(0, -(_SteelFrameHeight * secNum / 2), 0);

                                // 断面位置
                                Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSec);
                                dicTextNoteSecName.Add(posCenter, _SecMototan);

                                posCenter = _CmpGeometry.Center2Point(posMidL, posSecFrameRB);
                                dicTextNoteSecName.Add(posCenter, _SecSentan);

                                // 鉄骨サイズ
                                // 元端
                                posCenter = _CmpGeometry.Center2Point(posSecFrameRT, posSec);

                                // 梁せい
                                string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")];
                                // 梁幅
                                string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")];
                                // ウェブ厚
                                string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")];
                                // フランジ厚
                                string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")];
                                // ウェブマテリアル
                                string webMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")];
                                // フランジマテリアル
                                string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];

                                //if (webMat != flangeMat)
                                //{
                                //    if (_MaterialVary == "")
                                //    {
                                //        _MaterialVary = kai + fugo + " : " + _SecMototan;
                                //    }
                                //    else
                                //    {
                                //        _MaterialVary += ", " + kai + fugo + " : " + _SecMototan;
                                //    }
                                //}

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")] + sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                                // 鋼材種を表示
                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + flangeMat + ")";
                                }

                                dicTextNoteSteel.Add(posCenter, txtNoteVal);

                                // 先端
                                posCenter = _CmpGeometry.Center2Point(posSec, posSecFrameRB);

                                // 梁せい
                                sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_E")];
                                // 梁幅
                                haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_E")];
                                // ウェブ厚
                                webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_E")];
                                // フランジ厚
                                flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_E")];
                                // ウェブマテリアル
                                webMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E")];
                                // フランジマテリアル
                                flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")];

                                //if (webMat != flangeMat)
                                //{
                                //    if (_MaterialVary == "")
                                //    {
                                //        _MaterialVary = kai + fugo + " : " + _SecSentan;
                                //    }
                                //    else
                                //    {
                                //        _MaterialVary += ", " + kai + fugo + " : " + _SecSentan;
                                //    }
                                //}

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_E")] + sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                                // 鋼材種を表示
                                if (_ShowSteel == 1)
                                {
                                    txtNoteVal += " (" + flangeMat + ")";
                                }

                                dicTextNoteSteel.Add(posCenter, txtNoteVal);

                                #endregion
                            }

                            // マテリアル違い
                            #region

                            // ウェブマテリアル 元端
                            string webMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")];
                            // フランジマテリアル 元端
                            string flangeMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];
                            // ウェブマテリアル 先端
                            string webMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E")];
                            // フランジマテリアル 先端
                            string flangeMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")];

                            string secName = "";

                            if (webMat_S != flangeMat_S)
                            {
                                secName = _SecMototan;
                            }
                            if (webMat_E != flangeMat_E)
                            {
                                if (secName == "")
                                {
                                    secName = _SecSentan;
                                }
                                else
                                {
                                    secName += ", " + _SecSentan;
                                }
                            }

                            if (secName != "")
                            {
                                if (_MaterialVary == "")
                                {
                                    _MaterialVary = kai + fugo + " (" + secName + ")";
                                }
                                else
                                {
                                    _MaterialVary += ", " + kai + fugo + " (" + secName + ")";
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            // 斜線
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameLB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                        }

                        posSteelFrameLT = posSteelFrameLB;

                        if (_ShowNote == 0)
                        {
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                        }
                        if (_ShowNote == 1)
                        {
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posSteelFrameLB += new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);
                            posSteelFrameRB += new Revit.DB.XYZ(0, -_SteelFrameHeight, 0);

                            posRightEnd = posSteelFrameRB;

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posSteelFrameLT = posSteelFrameLB;
                        }
                    }
                }

                #endregion

                if (_ShowTitle == 0)
                {
                    posCantiStart = new Revit.DB.XYZ(posCantiStart.X, posRightEnd.Y - _FrameHeight2Title, 0);
                }
                if (_ShowTitle == 1)
                {
                    posCantiStart = new Revit.DB.XYZ(posCantiStart.X, posRightEnd.Y - _FrameHeight1Title, 0);
                }

                //Draw note
                if (_SelectedNoteIndex != 0 && _DicNote_Items.Count != 0)
                {
                    startX = posCantiStart.X;
                    endX = posRightEnd.X;
                    endY = posRightEnd.Y;
                    double sumHeight = 0;

                    NoteUtil.DrawNotes(_CmpGeometry, ref frameLines, ref dicTextNoteTitle, _ShowTitle, _DicNote_Items, _SelectedNoteIndex,
                        startX, endX, endY, _ViewScale, _FrameWidth2Title, _FrameWidth1Title, _SecFrameWidth + _SteelFrameWidth, colNum, out sumHeight);

                    posCantiStart = new Revit.DB.XYZ(posCantiStart.X, posCantiStart.Y - sumHeight, posCantiStart.Z);
                }
                //End draw note
            }

            #endregion

            // goto - 作図
            gotoDraw:

            // 作図なし
            if (flagGirder == false && flagCanti == false)
            {
                transac.Start("Remove View");
                _CmpElements.RemoveView(vp);
                transac.Commit();

                return ret;
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
            lineWidth = (_SecFrameWidth + _SteelFrameWidth) / _ViewScale;

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

            // 断面位置枠
            lineWidth = _SecFrameWidth / _ViewScale;

            foreach (Revit.DB.XYZ pos in dicTextNoteSecName.Keys)
            {
                string value = dicTextNoteSecName[pos];

                Revit.DB.TextNote txtNote = _CmpService.CreateNewTextNote(vp,
                                                                          pos,
                                                                          baseVec,
                                                                          lineWidth,
                                                                          _TxtNoteTypeSteel.Id,
                                                                          value,
                                                                          _CmpElements.RvtDBDoc,
                                                                          transac);
            }

            // 鉄骨サイズ枠
            lineWidth = _SteelFrameWidth / _ViewScale;

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

                //double width = txtNote.Width * _ViewScale;

                //Revit.DB.XYZ move = new Revit.DB.XYZ(width / 2 - 8.16 / 304.8 + 3 / 304.8 * _ViewScale, 0, 0);

                //transac.Start("Move");

                //txtNote.Location.Move(move);

                //transac.Commit();
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
        /// <summary>作成 - 小梁リスト</summary>
        ///
        /// <param name="girders">小梁</param>
        ///
        /// <history><p>2017/06/23 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/08/01 Modified CST,Co.Ltd Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string CreateListSubs(Collections.Generic.IList<Revit.DB.FamilySymbol> subGirders)
        {
            string ret = "";

            Revit.DB.Transaction transac = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);

            if (subGirders.Count < 1)
            {
                return ret;
            }

            // 作図ビュー
            Revit.DB.ViewPlan vp = _CmpElements.SetCreateListView(_ViewScale, 3);
            if (vp == null)
            {
                return ret;
            }

            Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelPAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            // 梁、片持ち梁の振り分け
            _CmpService.GirderDivision(subGirders,
                                       ref girderAry,
                                       ref cantiGirderAry,
                                       ref steelLAry,
                                       ref steelUAry,
                                       ref steelCAry,
                                       ref steelFBAry,
                                       ref steelMAry,
                                       ref steelTAry,
                                       ref steelRectAry,
                                       ref steelPAry
                                       );

            // 枠線
            Revit.DB.Line line = null;
            Collections.Generic.IList<Revit.DB.Curve> frameLines = new Collections.Generic.List<Revit.DB.Curve>();

            // タイトル文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteTitle = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 符号文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSymbol = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 断面位置文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSecName = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 鉄骨文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSteel = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();

            // 備考
            int showNote = 1;
            if (_SubShowNote == 1)
            {
                showNote = 2;
            }

            // 右下端
            Revit.DB.XYZ posRightEnd = new Revit.DB.XYZ();

            // 梁
            #region

            // データテーブル化
            SectionListSteel.Entities.DtGirder entDtGirder = new SectionListSteel.Entities.DtGirder(_CmpAttribute,
                                                                                      _CmpElements,
                                                                                      _CmpGeometry,
                                                                                      _CmpParameters,
                                                                                      _CmpSettings);
            entDtGirder.GetData(girderAry, 1);

            entDtGirder.GetData(steelLAry, 3);
            entDtGirder.GetData(steelUAry, 4);
            entDtGirder.GetData(steelCAry, 5);
            entDtGirder.GetData(steelFBAry, 6);
            entDtGirder.GetData(steelMAry, 7);
            entDtGirder.GetData(steelTAry, 8);
            entDtGirder.GetData(steelRectAry, 9);
            entDtGirder.GetData(steelPAry, 10);

            System.Data.DataTable data = entDtGirder.Data;

            // 符号順序
            Collections.Generic.IList<string> fugoOrder = _CmpService.FugoOrder(data);

            // 行数
            int numRow = fugoOrder.Count;

            if (numRow != 0)
            {
                // タイトル枠
                #region

                Revit.DB.XYZ posLT = new Revit.DB.XYZ(0, 0, 0);
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
                posRT = posLT + new Revit.DB.XYZ(_SubSteelFrameWidth + _SubSecFrameWidth, 0, 0);
                posRB = posLB + new Revit.DB.XYZ(_SubSteelFrameWidth + _SubSecFrameWidth, 0, 0);

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

                #endregion

                Collections.Generic.IList<string> usedFugo = new Collections.Generic.List<string>();

                posLT = new Revit.DB.XYZ(0, -_SubTitleFrameHeight, 0);

                Revit.DB.XYZ posSteelFrameLT = null;
                Revit.DB.XYZ posSteelFrameLB = null;
                Revit.DB.XYZ posSteelFrameRT = null;
                Revit.DB.XYZ posSteelFrameRB = null;

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

                    // 断面数
                    int girderType = (int)currentRow[_CmpAttribute.ResourceText("IDS_CN_TYPE")];

                    // 断面数
                    int secNum = 1;
                    if (girderType == 1)
                        secNum = GirderSectionNum(currentRow);

                    posSteelFrameLT = posLT;
                    posSteelFrameLB = posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);
                    posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubTitleFrameWidth, 0, 0);
                    posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameLT, posSteelFrameLB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    posTitle = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRB);
                    dicTextNoteSymbol.Add(posTitle, fugo);

                    // 断面位置枠
                    posSteelFrameLT = posSteelFrameRT;
                    posSteelFrameLB = posSteelFrameRB;
                    posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubSecFrameWidth, 0, 0);
                    posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                    string txtNoteVal = "";

                    if (girderType == 1)
                    {
                        // 断面数別
                        if (secNum == 1)
                        {
                            #region

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 断面位置
                            Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRB);
                            dicTextNoteSecName.Add(posCenter, _SecZendan);

                            // 鉄骨サイズ枠
                            posSteelFrameLT = posSteelFrameRT;
                            posSteelFrameLB = posSteelFrameRB;
                            posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);
                            posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                            // 鉄骨サイズ
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLB);

                            // 梁せい
                            string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                            // 梁幅
                            string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                            // ウェブ厚
                            string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                            // フランジ厚
                            string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                            // フランジマテリアル
                            string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                            // 鋼材種を表示
                            if (_SubShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            #endregion
                        }
                        else if (secNum == 2)
                        {
                            #region

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 断面位置 端部
                            Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            dicTextNoteSecName.Add(posCenter, _SecTanbu);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 断面位置 中央
                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                            dicTextNoteSecName.Add(posCenter, _SecChuoh);

                            // 鉄骨サイズ枠
                            posSteelFrameLT = posSteelFrameRT;
                            posSteelFrameLB = posSteelFrameRB;
                            posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);
                            posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                            // 鉄骨サイズ
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));

                            // 梁せい
                            string sei_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S")];
                            // 梁幅
                            string haba_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S")];
                            // ウェブ厚
                            string webAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S")];
                            // フランジ厚
                            string flangeAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S")];
                            // フランジマテリアル
                            string flangeMat_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                            // ハンチ長さ - 始端
                            string haunchNagasa_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S")];
                            // フィレット - 始端
                            string fillet_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S")];

                            // 梁せい
                            string sei_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                            // 梁幅
                            string haba_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                            // ウェブ厚
                            string webAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                            // フランジ厚
                            string flangeAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                            // フランジマテリアル
                            string flangeMat_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;

                            // ハンチ付き
                            if (fillet_s == "0")
                            {
                                if (haunchNagasa_s != "0" && sei_s != sei_c)
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "～" + sei_c + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;
                                }
                            }

                            // 鋼材種を表示
                            if (_SubShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_s + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 断面位置 中央
                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameLB);

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei_c + "x" + haba_c + "x" + webAtsu_c + "x" + flangeAtsu_c;

                            // 鋼材種を表示
                            if (_SubShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_c + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            #endregion
                        }
                        else if (secNum == 3)
                        {
                            #region

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 断面位置 始端
                            Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            dicTextNoteSecName.Add(posCenter, _SecShitan);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 断面位置 中央
                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0));
                            dicTextNoteSecName.Add(posCenter, _SecChuoh);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 3, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 3, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 3, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 断面位置 終端
                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 3, 0));
                            dicTextNoteSecName.Add(posCenter, _SecSyutan);

                            // 鉄骨サイズ枠
                            posSteelFrameLT = posSteelFrameRT;
                            posSteelFrameLB = posSteelFrameRB;
                            posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);
                            posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                            // 鉄骨サイズ
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));

                            // 梁せい
                            string sei_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S")];
                            // 梁幅
                            string haba_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S")];
                            // ウェブ厚
                            string webAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S")];
                            // フランジ厚
                            string flangeAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S")];
                            // フランジマテリアル
                            string flangeMat_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                            // ハンチ長さ - 始端
                            string haunchNagasa_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S")];
                            // フィレット - 始端
                            string fillet_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S")];

                            // 梁せい
                            string sei_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                            // 梁幅
                            string haba_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                            // ウェブ厚
                            string webAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                            // フランジ厚
                            string flangeAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                            // フランジマテリアル
                            string flangeMat_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                            // 梁せい
                            string sei_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_E")];
                            // 梁幅
                            string haba_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_E")];
                            // ウェブ厚
                            string webAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_E")];
                            // フランジ厚
                            string flangeAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_E")];
                            // フランジマテリアル
                            string flangeMat_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")];
                            // ハンチ長さ - 終端
                            string haunchNagasa_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_E")];
                            // フィレット - 終端
                            string fillet_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_E")];

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;

                            // ハンチ付き
                            if (fillet_s == "0")
                            {
                                if (haunchNagasa_s != "0" && sei_s != sei_c)
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "～" + sei_c + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;
                                }
                            }

                            // 鋼材種を表示
                            if (_SubShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_s + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0));

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei_c + "x" + haba_c + "x" + webAtsu_c + "x" + flangeAtsu_c;

                            // 鋼材種を表示
                            if (_SubShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_c + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 3, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 3, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 3, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * 2, 0), posSteelFrameLB);

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E")] + sei_e + "x" + haba_e + "x" + webAtsu_e + "x" + flangeAtsu_e;

                            // ハンチ付き
                            if (fillet_e == "0")
                            {
                                if (haunchNagasa_e != "0" && sei_e != sei_c)
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E")] + sei_e + "～" + sei_c + "x" + haba_e + "x" + webAtsu_e + "x" + flangeAtsu_e;
                                }
                            }

                            // 鋼材種を表示
                            if (_SubShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_e + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            #endregion
                        }
                    }
                    else
                    {
                        // 断面数別
                        if (secNum == 1)
                        {
                            #region

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 断面位置
                            Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRB);
                            dicTextNoteSecName.Add(posCenter, _SecZendan);

                            // 鉄骨サイズ枠
                            posSteelFrameLT = posSteelFrameRT;
                            posSteelFrameLB = posSteelFrameRB;
                            posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);
                            posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                            // 鉄骨サイズ
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLB);

                            if (girderType == 3)
                            {
                                // 梁せい
                                var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderSei_C")];
                                // 梁幅
                                var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderHaba_C")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGIRDERMAT")];

                                var directionThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderDirThick_C")];

                                //Width thickness
                                var widthThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderWidthThick_C")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + directionThickness + "x" + widthThickness;

                                // 鋼材種を表示
                                if (_SubShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 4)
                            {
                                // 梁せい
                                var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderSei_C")];
                                // 梁幅
                                var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderHaba_C")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGIRDERMAT")];

                                // 中央 ウェブ厚
                                var parWebAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderWebAtsu_C")];

                                // 中央 フランジ厚
                                var parFlangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderFlangeAtsu_C")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + parWebAtsu + "x" + parFlangeAtsu;

                                // 鋼材種を表示
                                if (_SubShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 5)
                            {
                                // 梁せい
                                var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderSei_C")];
                                // 梁幅
                                var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderHaba_C")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGIRDERMAT")];

                                var parLipLength = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderLipLength_C")];

                                // 中央 板厚
                                var parThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderThick_C")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + parLipLength + "x" + parThickness;

                                // 鋼材種を表示
                                if (_SubShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 6)
                            {
                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBGIRDERMAT")];

                                // 幅
                                var parWidth = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_WIDTH")];

                                // 板厚
                                var parBoardThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_THICK")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + parWidth + "x" + parBoardThickness;

                                // 鋼材種を表示
                                if (_SubShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 7)
                            {
                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MGIRDERMAT")];

                                var parDiameter = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MGIRDER_DIAMETER")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + parDiameter;

                                // 鋼材種を表示
                                if (_SubShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 8)
                            {
                                // 梁せい
                                var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderSei_C")];
                                // 梁幅
                                var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderHaba_C")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGIRDERMAT")];

                                // 中央 ウェブ厚
                                var parWebAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderWebAtsu_C")];

                                // 中央 フランジ厚
                                var parFlangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderFlangeAtsu_C")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + parWebAtsu + "x" + parFlangeAtsu;

                                // 鋼材種を表示
                                if (_SubShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 9)
                            {
                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDERMAT")];

                                var SteelFrame = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_SEI")];

                                // 鉄骨幅
                                var SteelWFrame = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_HABA")];

                                // せい方向の板厚
                                var ThicknessDirect = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRTHICK")];

                                // 幅方向の板厚
                                var WidthDirect = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRWIDTH")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + SteelFrame + "x" + SteelWFrame + "x" + ThicknessDirect + "x" + WidthDirect;

                                // 鋼材種を表示
                                if (_SubShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 10)
                            {
                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_PGIRDERMAT")];

                                // 直径
                                var parDiameter = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_PGIRDER_DIAMETER")];

                                // 板厚
                                var parBoardThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_PGIRDER_ITAATSU")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + parDiameter + "x" + parBoardThickness;

                                // 鋼材種を表示
                                if (_SubShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            #endregion
                        }
                    }

                    // 備考枠
                    if (showNote == 2)
                    {
                        posSteelFrameLT = posSteelFrameRT;
                        posSteelFrameLB = posSteelFrameRB;
                        posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubNoteFrameWidth, 0, 0);
                        posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                    }

                    posLT = new Revit.DB.XYZ(0, posSteelFrameLB.Y, 0);
                    posRightEnd = posSteelFrameRB;

                    if (girderType == 1)
                    {
                        // マテリアル違い
                        #region

                        // ウェブマテリアル 始端
                        string webMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_S")];
                        // フランジマテリアル 始端
                        string flangeMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                        // ウェブマテリアル 中央
                        string webMat_C = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_C")];
                        // フランジマテリアル 中央
                        string flangeMat_C = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];
                        // ウェブマテリアル 終端
                        string webMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_E")];
                        // フランジマテリアル 終端
                        string flangeMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")];

                        string secName = "";

                        if (webMat_S != flangeMat_S)
                        {
                            secName = _SecShitan;
                        }
                        if (webMat_C != flangeMat_C)
                        {
                            if (secName == "")
                            {
                                secName = _SecChuoh;
                            }
                            else
                            {
                                secName += ", " + _SecChuoh;
                            }
                        }
                        if (webMat_E != flangeMat_E)
                        {
                            if (secName == "")
                            {
                                secName = _SecSyutan;
                            }
                            else
                            {
                                secName += ", " + _SecSyutan;
                            }
                        }

                        if (secName != "")
                        {
                            if (_MaterialVary == "")
                            {
                                _MaterialVary = fugo + " (" + secName + ")";
                            }
                            else
                            {
                                _MaterialVary += ", " + fugo + " (" + secName + ")";
                            }
                        }
                        #endregion
                    }
                }

                #endregion
            }

            // 片持ち梁
            #region

            // データテーブル化
            entDtGirder = new SectionListSteel.Entities.DtGirder(_CmpAttribute,
                                                          _CmpElements,
                                                          _CmpGeometry,
                                                          _CmpParameters,
                                                          _CmpSettings);
            entDtGirder.GetData(cantiGirderAry, 2);
            data = entDtGirder.Data;

            // 符号順序
            fugoOrder = _CmpService.FugoOrder(data);

            // 行数
            numRow = fugoOrder.Count;

            if (numRow != 0)
            {
                // 片持ち梁開始位置
                Revit.DB.XYZ posCantiSrart = new Revit.DB.XYZ();
                if (frameLines.Count != 0)
                {
                    posCantiSrart = new Revit.DB.XYZ(posRightEnd.X + _SubTitleFrameWidth, 0, 0);
                }

                // タイトル枠
                #region

                var posLT = posCantiSrart;
                var posLB = posCantiSrart + new Revit.DB.XYZ(0, -_SubTitleFrameHeight, 0);
                var posRT = posCantiSrart + new Revit.DB.XYZ(_SubTitleFrameWidth, 0, 0);
                var posRB = posCantiSrart + new Revit.DB.XYZ(_SubTitleFrameWidth, -_SubTitleFrameHeight, 0);

                line = _CmpGeometry.CreateBoundLine(posLT, posLB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posLB, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posLT, posRT);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posRT, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                var posTitle = _CmpGeometry.Center2Point(posLT, posRB);
                dicTextNoteTitle.Add(posTitle, _SubTitleFrame);

                // 断面枠
                posLT = posRT;
                posLB = posRB;
                posRT = posLT + new Revit.DB.XYZ(_SubSteelFrameWidth + _SubSecFrameWidth, 0, 0);
                posRB = posLB + new Revit.DB.XYZ(_SubSteelFrameWidth + _SubSecFrameWidth, 0, 0);

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

                #endregion

                var usedFugo = new Collections.Generic.List<string>();

                posLT = new Revit.DB.XYZ(posCantiSrart.X, -_SubTitleFrameHeight, 0);

                Revit.DB.XYZ posSteelFrameLT = null;
                Revit.DB.XYZ posSteelFrameLB = null;
                Revit.DB.XYZ posSteelFrameRT = null;
                Revit.DB.XYZ posSteelFrameRB = null;

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

                    // 断面数
                    int secNum = CantiGirderSectionNum(currentRow);

                    posSteelFrameLT = posLT;
                    posSteelFrameLB = posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);
                    posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubTitleFrameWidth, 0, 0);
                    posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameLT, posSteelFrameLB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    posTitle = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRB);
                    dicTextNoteSymbol.Add(posTitle, fugo);

                    // 断面位置枠
                    posSteelFrameLT = posSteelFrameRT;
                    posSteelFrameLB = posSteelFrameRB;
                    posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubSecFrameWidth, 0, 0);
                    posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                    string txtNoteVal = "";

                    // 断面数別
                    if (secNum == 1)
                    {
                        #region

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        // 断面位置
                        Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRB);
                        dicTextNoteSecName.Add(posCenter, _SecZendan);

                        // 鉄骨サイズ枠
                        posSteelFrameLT = posSteelFrameRT;
                        posSteelFrameLB = posSteelFrameRB;
                        posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);
                        posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        // 鉄骨サイズ
                        posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLB);

                        // 梁せい
                        string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")];
                        // 梁幅
                        string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")];
                        // ウェブ厚
                        string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")];
                        // フランジ厚
                        string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")];
                        // フランジマテリアル
                        string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];

                        txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")] + sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                        // 鋼材種を表示
                        if (_SubShowSteel == 1)
                        {
                            txtNoteVal += " (" + flangeMat + ")";
                        }

                        dicTextNoteSteel.Add(posCenter, txtNoteVal);

                        #endregion
                    }
                    else if (secNum == 2)
                    {
                        #region

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        // 断面位置 元端
                        Revit.DB.XYZ posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                        dicTextNoteSecName.Add(posCenter, _SecMototan);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        // 断面位置 先端
                        posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                        dicTextNoteSecName.Add(posCenter, _SecSentan);

                        // 鉄骨サイズ枠
                        posSteelFrameLT = posSteelFrameRT;
                        posSteelFrameLB = posSteelFrameRB;
                        posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubSteelFrameWidth, 0, 0);
                        posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                        // 鉄骨サイズ
                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0));

                        // 梁せい
                        string sei_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")];
                        // 梁幅
                        string haba_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")];
                        // ウェブ厚
                        string webAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")];
                        // フランジ厚
                        string flangeAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")];
                        // フランジマテリアル
                        string flangeMat_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];

                        txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")] + sei_s + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;

                        // 鋼材種を表示
                        if (_SubShowSteel == 1)
                        {
                            txtNoteVal += " (" + flangeMat_s + ")";
                        }

                        dicTextNoteSteel.Add(posCenter, txtNoteVal);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight, 0), posSteelFrameLB);

                        // 梁せい
                        string sei_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_E")];
                        // 梁幅
                        string haba_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_E")];
                        // ウェブ厚
                        string webAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_E")];
                        // フランジ厚
                        string flangeAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_E")];
                        // フランジマテリアル
                        string flangeMat_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")];

                        txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_E")] + sei_e + "x" + haba_e + "x" + webAtsu_e + "x" + flangeAtsu_e;

                        // 鋼材種を表示
                        if (_SubShowSteel == 1)
                        {
                            txtNoteVal += " (" + flangeMat_e + ")";
                        }

                        dicTextNoteSteel.Add(posCenter, txtNoteVal);

                        #endregion
                    }

                    // 備考枠
                    if (showNote == 2)
                    {
                        posSteelFrameLT = posSteelFrameRT;
                        posSteelFrameLB = posSteelFrameRB;
                        posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubNoteFrameWidth, 0, 0);
                        posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_SubSteelFrameHeight * secNum, 0);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                    }

                    posLT = new Revit.DB.XYZ(posCantiSrart.X, posSteelFrameLB.Y, 0);
                    posRightEnd = posSteelFrameRB;

                    // マテリアル違い
                    #region

                    // ウェブマテリアル 元端
                    string webMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")];
                    // フランジマテリアル 元端
                    string flangeMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];
                    // ウェブマテリアル 先端
                    string webMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E")];
                    // フランジマテリアル 先端
                    string flangeMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")];

                    string secName = "";

                    if (webMat_S != flangeMat_S)
                    {
                        secName = _SecMototan;
                    }
                    if (webMat_E != flangeMat_E)
                    {
                        if (secName == "")
                        {
                            secName = _SecSentan;
                        }
                        else
                        {
                            secName += ", " + _SecSentan;
                        }
                    }

                    if (secName != "")
                    {
                        if (_MaterialVary == "")
                        {
                            _MaterialVary = fugo + " (" + secName + ")";
                        }
                        else
                        {
                            _MaterialVary += ", " + fugo + " (" + secName + ")";
                        }
                    }
                    #endregion
                }
            }

            #endregion
            //////////////////////////////////////////////////////////////////////////

            // 作図なし
            if (frameLines.Count == 0)
            {
                transac.Start("Remove View");
                _CmpElements.RemoveView(vp);
                transac.Commit();

                return ret;
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

            Collections.Generic.IList<Revit.DB.XYZ> posAry = dicTextNoteTitle.Keys.ToList<Revit.DB.XYZ>();

            for (int i = 0; i < dicTextNoteTitle.Count; ++i)
            {
                if (i == 0)
                {
                    lineWidth = _SubTitleFrameWidth / _ViewScale;
                }
                else if (i == 1)
                {
                    lineWidth = (_SubSecFrameWidth + _SubSteelFrameWidth) / _ViewScale;
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

            // 断面位置枠
            lineWidth = _SubSecFrameWidth / _ViewScale;

            foreach (Revit.DB.XYZ pos in dicTextNoteSecName.Keys)
            {
                string value = dicTextNoteSecName[pos];

                Revit.DB.TextNote txtNote = _CmpService.CreateNewTextNote(vp,
                                                                          pos,
                                                                          baseVec,
                                                                          lineWidth,
                                                                          _TxtNoteTypeSteel.Id,
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
                                 posRightEnd);

            if (transac.GetStatus() == Revit.DB.TransactionStatus.Started)
            {
                transac.Commit();
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>Create brace list</summary>
        ///
        /// <param name="braces">brace</param>
        ///
        /// ================================================================================
        public
        string CreateBraceLists(Collections.Generic.IList<Revit.DB.FamilySymbol> braces)
        {
            string ret = "";

            Revit.DB.Transaction transac = new Revit.DB.Transaction(_CmpElements.RvtDBDoc);

            if (braces.Count < 1)
            {
                return ret;
            }

            // 作図ビュー
            Revit.DB.ViewPlan vp = _CmpElements.SetCreateListView(_ViewScale, 4);
            if (vp == null)
            {
                return ret;
            }

            Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            Collections.Generic.IList<Revit.DB.FamilySymbol> steelPAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            // 梁、片持ち梁の振り分け
            _CmpService.GirderDivision(braces,
                                       ref girderAry,
                                       ref cantiGirderAry,
                                       ref steelLAry,
                                       ref steelUAry,
                                       ref steelCAry,
                                       ref steelFBAry,
                                       ref steelMAry,
                                       ref steelTAry,
                                       ref steelRectAry,
                                       ref steelPAry
                                       );

            // 枠線
            Revit.DB.Line line = null;

            Collections.Generic.IList<Revit.DB.Curve> frameLines = new Collections.Generic.List<Revit.DB.Curve>();

            // タイトル文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteTitle = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 符号文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSymbol = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            //             // 断面位置文字
            //             Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSecName = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();
            // 鉄骨文字
            Collections.Generic.IDictionary<Revit.DB.XYZ, string> dicTextNoteSteel = new Collections.Generic.Dictionary<Revit.DB.XYZ, string>();

            // 備考
            int showNote = 1;
            if (_BraceShowNote == 1)
            {
                showNote = 2;
            }

            // 右下端
            Revit.DB.XYZ posRightEnd = new Revit.DB.XYZ();

            // Brace
            // 梁
            #region

            // データテーブル化
            SectionListSteel.Entities.DtGirder entDtGirder = new SectionListSteel.Entities.DtGirder(_CmpAttribute,
                                                                                      _CmpElements,
                                                                                      _CmpGeometry,
                                                                                      _CmpParameters,
                                                                                      _CmpSettings);
            entDtGirder.GetData(girderAry, 1);

            entDtGirder.GetData(steelLAry, 3);
            entDtGirder.GetData(steelUAry, 4);
            entDtGirder.GetData(steelCAry, 5);
            entDtGirder.GetData(steelFBAry, 6);
            entDtGirder.GetData(steelMAry, 7);
            entDtGirder.GetData(steelTAry, 8);
            entDtGirder.GetData(steelRectAry, 9);
            entDtGirder.GetData(steelPAry, 10);

            System.Data.DataTable data = entDtGirder.Data;

            // 符号順序
            Collections.Generic.IList<string> fugoOrder = _CmpService.FugoOrder(data);

            // 行数
            int numRow = fugoOrder.Count;
            if (numRow != 0)
            {
                // タイトル枠
                #region

                Revit.DB.XYZ posLT = new Revit.DB.XYZ(0, 0, 0);
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
                posRT = posLT + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);
                posRB = posLB + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);

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
                    posRT = posLT + new Revit.DB.XYZ(_BraceNoteFrameWidth, 0, 0);
                    posRB = posLB + new Revit.DB.XYZ(_BraceNoteFrameWidth, 0, 0);

                    line = _CmpGeometry.CreateBoundLine(posLT, posRT);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posLB, posRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posRT, posRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    posTitle = _CmpGeometry.Center2Point(posLT, posRB);
                    dicTextNoteTitle.Add(posTitle, _CmpAttribute.ResourceText("IDS_TXT_NOTE"));
                }

                #endregion

                Collections.Generic.IList<string> usedFugo = new Collections.Generic.List<string>();

                posLT = new Revit.DB.XYZ(0, -_SubTitleFrameHeight, 0);

                Revit.DB.XYZ posSteelFrameLT = null;
                Revit.DB.XYZ posSteelFrameLB = null;
                Revit.DB.XYZ posSteelFrameRT = null;
                Revit.DB.XYZ posSteelFrameRB = null;

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

                    // 断面数
                    int girderType = (int)currentRow[_CmpAttribute.ResourceText("IDS_CN_TYPE")];

                    // 断面数
                    int secNum = 1;
                    if (girderType == 1)
                        secNum = GirderSectionNum(currentRow);

                    posSteelFrameLT = posLT;
                    posSteelFrameLB = posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);
                    posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubTitleFrameWidth, 0, 0);
                    posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameLT, posSteelFrameLB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    posTitle = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRB);
                    dicTextNoteSymbol.Add(posTitle, fugo);

                    // 断面位置枠
                    posSteelFrameLT = posSteelFrameRT;
                    posSteelFrameLB = posSteelFrameRB;
                    posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(0, 0, 0);
                    posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                    string txtNoteVal = "";

                    if (girderType == 1)
                    {
                        // 断面数別
                        if (secNum == 1)
                        {
                            #region

                            // 鉄骨サイズ枠
                            posSteelFrameLT = posSteelFrameRT;
                            posSteelFrameLB = posSteelFrameRB;
                            posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);
                            posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                            // 鉄骨サイズ
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            var posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLB);

                            // 梁せい
                            string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                            // 梁幅
                            string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                            // ウェブ厚
                            string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                            // フランジ厚
                            string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                            // フランジマテリアル
                            string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                            // 鋼材種を表示
                            if (_BraceShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            #endregion
                        }
                        else if (secNum == 2)
                        {
                            #region

                            // 鉄骨サイズ枠
                            posSteelFrameLT = posSteelFrameRT;
                            posSteelFrameLB = posSteelFrameRB;
                            posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);
                            posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                            // 鉄骨サイズ
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            var posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0));

                            // 梁せい
                            string sei_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S")];
                            // 梁幅
                            string haba_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S")];
                            // ウェブ厚
                            string webAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S")];
                            // フランジ厚
                            string flangeAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S")];
                            // フランジマテリアル
                            string flangeMat_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                            // ハンチ長さ - 始端
                            string haunchNagasa_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S")];
                            // フィレット - 始端
                            string fillet_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S")];

                            // 梁せい
                            string sei_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                            // 梁幅
                            string haba_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                            // ウェブ厚
                            string webAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                            // フランジ厚
                            string flangeAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                            // フランジマテリアル
                            string flangeMat_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;

                            // ハンチ付き
                            if (fillet_s == "0")
                            {
                                if (haunchNagasa_s != "0" && sei_s != sei_c)
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "～" + sei_c + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;
                                }
                            }

                            // 鋼材種を表示
                            if (_BraceShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_s + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 断面位置 中央
                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0), posSteelFrameLB);

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei_c + "x" + haba_c + "x" + webAtsu_c + "x" + flangeAtsu_c;

                            // 鋼材種を表示
                            if (_BraceShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_c + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            #endregion
                        }
                        else if (secNum == 3)
                        {
                            #region

                            // 鉄骨サイズ枠
                            posSteelFrameLT = posSteelFrameRT;
                            posSteelFrameLB = posSteelFrameRB;
                            posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);
                            posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                            // 鉄骨サイズ
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            var posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0));

                            // 梁せい
                            string sei_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S")];
                            // 梁幅
                            string haba_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S")];
                            // ウェブ厚
                            string webAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S")];
                            // フランジ厚
                            string flangeAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S")];
                            // フランジマテリアル
                            string flangeMat_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                            // ハンチ長さ - 始端
                            string haunchNagasa_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S")];
                            // フィレット - 始端
                            string fillet_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S")];

                            // 梁せい
                            string sei_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                            // 梁幅
                            string haba_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                            // ウェブ厚
                            string webAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                            // フランジ厚
                            string flangeAtsu_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                            // フランジマテリアル
                            string flangeMat_c = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                            // 梁せい
                            string sei_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_E")];
                            // 梁幅
                            string haba_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_E")];
                            // ウェブ厚
                            string webAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_E")];
                            // フランジ厚
                            string flangeAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_E")];
                            // フランジマテリアル
                            string flangeMat_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")];
                            // ハンチ長さ - 終端
                            string haunchNagasa_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_E")];
                            // フィレット - 終端
                            string fillet_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_E")];

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;

                            // ハンチ付き
                            if (fillet_s == "0")
                            {
                                if (haunchNagasa_s != "0" && sei_s != sei_c)
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")] + sei_s + "～" + sei_c + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;
                                }
                            }

                            // 鋼材種を表示
                            if (_BraceShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_s + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * 2, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * 2, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * 2, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0), posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * 2, 0));

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei_c + "x" + haba_c + "x" + webAtsu_c + "x" + flangeAtsu_c;

                            // 鋼材種を表示
                            if (_BraceShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_c + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * 3, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * 3, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * 2, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * 3, 0));
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * 2, 0), posSteelFrameLB);

                            txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E")] + sei_e + "x" + haba_e + "x" + webAtsu_e + "x" + flangeAtsu_e;

                            // ハンチ付き
                            if (fillet_e == "0")
                            {
                                if (haunchNagasa_e != "0" && sei_e != sei_c)
                                {
                                    txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E")] + sei_e + "～" + sei_c + "x" + haba_e + "x" + webAtsu_e + "x" + flangeAtsu_e;
                                }
                            }

                            // 鋼材種を表示
                            if (_BraceShowSteel == 1)
                            {
                                txtNoteVal += " (" + flangeMat_e + ")";
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            #endregion
                        }
                    }
                    else
                    {
                        if (secNum == 1)
                        {
                            #region

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            // 鉄骨サイズ枠
                            posSteelFrameLT = posSteelFrameRT;
                            posSteelFrameLB = posSteelFrameRB;
                            posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);
                            posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                            // 鉄骨サイズ
                            line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                            _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                            var posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLB);

                            if (girderType == 3)
                            {
                                // 梁せい
                                var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderSei_C")];
                                // 梁幅
                                var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderHaba_C")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGIRDERMAT")];

                                var directionThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderDirThick_C")];

                                //Width thickness
                                var widthThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_LGirderWidthThick_C")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + directionThickness + "x" + widthThickness;

                                // 鋼材種を表示
                                if (_BraceShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 4)
                            {
                                // 梁せい
                                var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderSei_C")];
                                // 梁幅
                                var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderHaba_C")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGIRDERMAT")];

                                // 中央 ウェブ厚
                                var parWebAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderWebAtsu_C")];

                                // 中央 フランジ厚
                                var parFlangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_UGirderFlangeAtsu_C")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + parWebAtsu + "x" + parFlangeAtsu;

                                // 鋼材種を表示
                                if (_BraceShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 5)
                            {
                                // 梁せい
                                var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderSei_C")];
                                // 梁幅
                                var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderHaba_C")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGIRDERMAT")];

                                var parLipLength = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderLipLength_C")];

                                // 中央 板厚
                                var parThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CGirderThick_C")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + parLipLength + "x" + parThickness;

                                // 鋼材種を表示
                                if (_BraceShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 6)
                            {
                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBGIRDERMAT")];

                                // 幅
                                var parWidth = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_WIDTH")];

                                // 板厚
                                var parBoardThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_FBGIRDER_THICK")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + parWidth + "x" + parBoardThickness;

                                // 鋼材種を表示
                                if (_BraceShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 7)
                            {
                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MGIRDERMAT")];

                                var parDiameter = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_MGIRDER_DIAMETER")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + parDiameter;

                                // 鋼材種を表示
                                if (_BraceShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 8)
                            {
                                // 梁せい
                                var sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderSei_C")];
                                // 梁幅
                                var haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderHaba_C")];

                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGIRDERMAT")];

                                // 中央 ウェブ厚
                                var parWebAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderWebAtsu_C")];

                                // 中央 フランジ厚
                                var parFlangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_TGirderFlangeAtsu_C")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + sei + "x" + haba + "x" + parWebAtsu + "x" + parFlangeAtsu;

                                // 鋼材種を表示
                                if (_BraceShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 9)
                            {
                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDERMAT")];

                                var SteelFrame = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_SEI")];

                                // 鉄骨幅
                                var SteelWFrame = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_HABA")];

                                // せい方向の板厚
                                var ThicknessDirect = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRTHICK")];

                                // 幅方向の板厚
                                var WidthDirect = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_RECTGIRDER_DIRWIDTH")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + SteelFrame + "x" + SteelWFrame + "x" + ThicknessDirect + "x" + WidthDirect;

                                // 鋼材種を表示
                                if (_BraceShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }
                            else if (girderType == 10)
                            {
                                // 構造マテリアル
                                string strcMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_PGIRDERMAT")];

                                // 直径
                                var parDiameter = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_PGIRDER_DIAMETER")];

                                // 板厚
                                var parBoardThickness = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_PGIRDER_ITAATSU")];

                                txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")] + parDiameter + "x" + parBoardThickness;

                                // 鋼材種を表示
                                if (_BraceShowSteel == 1)
                                {
                                    txtNoteVal += " (" + strcMat + ")";
                                }
                            }

                            dicTextNoteSteel.Add(posCenter, txtNoteVal);

                            #endregion
                        }
                    }

                    // 備考枠
                    if (showNote == 2)
                    {
                        posSteelFrameLT = posSteelFrameRT;
                        posSteelFrameLB = posSteelFrameRB;
                        posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_BraceNoteFrameWidth, 0, 0);
                        posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                    }

                    posLT = new Revit.DB.XYZ(0, posSteelFrameLB.Y, 0);
                    posRightEnd = posSteelFrameRB;

                    if (girderType == 1)
                    {
                        // マテリアル違い
                        #region

                        // ウェブマテリアル 始端
                        string webMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_S")];
                        // フランジマテリアル 始端
                        string flangeMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                        // ウェブマテリアル 中央
                        string webMat_C = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_C")];
                        // フランジマテリアル 中央
                        string flangeMat_C = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];
                        // ウェブマテリアル 終端
                        string webMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_E")];
                        // フランジマテリアル 終端
                        string flangeMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")];

                        string secName = "";

                        if (webMat_S != flangeMat_S)
                        {
                            secName = _SecShitan;
                        }
                        if (webMat_C != flangeMat_C)
                        {
                            if (secName == "")
                            {
                                secName = _SecChuoh;
                            }
                            else
                            {
                                secName += ", " + _SecChuoh;
                            }
                        }
                        if (webMat_E != flangeMat_E)
                        {
                            if (secName == "")
                            {
                                secName = _SecSyutan;
                            }
                            else
                            {
                                secName += ", " + _SecSyutan;
                            }
                        }

                        if (secName != "")
                        {
                            if (_MaterialVary == "")
                            {
                                _MaterialVary = fugo + " (" + secName + ")";
                            }
                            else
                            {
                                _MaterialVary += ", " + fugo + " (" + secName + ")";
                            }
                        }
                        #endregion
                    }
                }

                #endregion
            }

            // 片持ち梁
            #region

            // データテーブル化
            entDtGirder = new SectionListSteel.Entities.DtGirder(_CmpAttribute,
                                                          _CmpElements,
                                                          _CmpGeometry,
                                                          _CmpParameters,
                                                          _CmpSettings);
            entDtGirder.GetData(cantiGirderAry, 2);
            data = entDtGirder.Data;

            // 符号順序
            fugoOrder = _CmpService.FugoOrder(data);

            // 行数
            numRow = fugoOrder.Count;

            if (numRow != 0)
            {
                // 片持ち梁開始位置
                Revit.DB.XYZ posCantiSrart = new Revit.DB.XYZ();
                if (frameLines.Count != 0)
                {
                    posCantiSrart = new Revit.DB.XYZ(posRightEnd.X + _SubTitleFrameWidth, 0, 0);
                }

                // タイトル枠
                #region

                var posLT = posCantiSrart;
                var posLB = posCantiSrart + new Revit.DB.XYZ(0, -_SubTitleFrameHeight, 0);
                var posRT = posCantiSrart + new Revit.DB.XYZ(_SubTitleFrameWidth, 0, 0);
                var posRB = posCantiSrart + new Revit.DB.XYZ(_SubTitleFrameWidth, -_SubTitleFrameHeight, 0);

                line = _CmpGeometry.CreateBoundLine(posLT, posLB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posLB, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posLT, posRT);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                line = _CmpGeometry.CreateBoundLine(posRT, posRB);
                _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                var posTitle = _CmpGeometry.Center2Point(posLT, posRB);
                dicTextNoteTitle.Add(posTitle, _SubTitleFrame);

                // 断面枠
                posLT = posRT;
                posLB = posRB;
                posRT = posLT + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);
                posRB = posLB + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);

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
                    posRT = posLT + new Revit.DB.XYZ(_BraceNoteFrameWidth, 0, 0);
                    posRB = posLB + new Revit.DB.XYZ(_BraceNoteFrameWidth, 0, 0);

                    line = _CmpGeometry.CreateBoundLine(posLT, posRT);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posLB, posRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posRT, posRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    posTitle = _CmpGeometry.Center2Point(posLT, posRB);
                    dicTextNoteTitle.Add(posTitle, _CmpAttribute.ResourceText("IDS_TXT_NOTE"));
                }

                #endregion

                var usedFugo = new Collections.Generic.List<string>();

                posLT = new Revit.DB.XYZ(posCantiSrart.X, -_SubTitleFrameHeight, 0);

                Revit.DB.XYZ posSteelFrameLT = null;
                Revit.DB.XYZ posSteelFrameLB = null;
                Revit.DB.XYZ posSteelFrameRT = null;
                Revit.DB.XYZ posSteelFrameRB = null;

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

                    // 断面数
                    int secNum = CantiGirderSectionNum(currentRow);

                    posSteelFrameLT = posLT;
                    posSteelFrameLB = posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);
                    posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_SubTitleFrameWidth, 0, 0);
                    posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameLT, posSteelFrameLB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                    _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                    posTitle = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameRB);
                    dicTextNoteSymbol.Add(posTitle, fugo);

                    // 断面位置枠
                    posSteelFrameLT = posSteelFrameRT;
                    posSteelFrameLB = posSteelFrameRB;
                    posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(0, 0, 0);
                    posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                    string txtNoteVal = "";

                    // 断面数別
                    if (secNum == 1)
                    {
                        #region

                        // 鉄骨サイズ枠
                        posSteelFrameLT = posSteelFrameRT;
                        posSteelFrameLB = posSteelFrameRB;
                        posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);
                        posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        // 鉄骨サイズ
                        var posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLB);

                        // 梁せい
                        string sei = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")];
                        // 梁幅
                        string haba = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")];
                        // ウェブ厚
                        string webAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")];
                        // フランジ厚
                        string flangeAtsu = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")];
                        // フランジマテリアル
                        string flangeMat = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];

                        txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")] + sei + "x" + haba + "x" + webAtsu + "x" + flangeAtsu;

                        // 鋼材種を表示
                        if (_BraceShowSteel == 1)
                        {
                            txtNoteVal += " (" + flangeMat + ")";
                        }

                        dicTextNoteSteel.Add(posCenter, txtNoteVal);

                        #endregion
                    }
                    else if (secNum == 2)
                    {
                        #region

                        // 鉄骨サイズ枠
                        posSteelFrameLT = posSteelFrameRT;
                        posSteelFrameLB = posSteelFrameRB;
                        posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_BraceSteelFrameWidth, 0, 0);
                        posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                        // 鉄骨サイズ
                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        var posCenter = _CmpGeometry.Center2Point(posSteelFrameLT, posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0));

                        // 梁せい
                        string sei_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")];
                        // 梁幅
                        string haba_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")];
                        // ウェブ厚
                        string webAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")];
                        // フランジ厚
                        string flangeAtsu_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")];
                        // フランジマテリアル
                        string flangeMat_s = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];

                        txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")] + sei_s + "x" + haba_s + "x" + webAtsu_s + "x" + flangeAtsu_s;

                        // 鋼材種を表示
                        if (_BraceShowSteel == 1)
                        {
                            txtNoteVal += " (" + flangeMat_s + ")";
                        }

                        dicTextNoteSteel.Add(posCenter, txtNoteVal);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0), posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0));
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        posCenter = _CmpGeometry.Center2Point(posSteelFrameLT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight, 0), posSteelFrameLB);

                        // 梁せい
                        string sei_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_E")];
                        // 梁幅
                        string haba_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_E")];
                        // ウェブ厚
                        string webAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_E")];
                        // フランジ厚
                        string flangeAtsu_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_E")];
                        // フランジマテリアル
                        string flangeMat_e = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")];

                        txtNoteVal = (string)currentRow[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_E")] + sei_e + "x" + haba_e + "x" + webAtsu_e + "x" + flangeAtsu_e;

                        // 鋼材種を表示
                        if (_BraceShowSteel == 1)
                        {
                            txtNoteVal += " (" + flangeMat_e + ")";
                        }

                        dicTextNoteSteel.Add(posCenter, txtNoteVal);

                        #endregion
                    }

                    // 備考枠
                    if (showNote == 2)
                    {
                        posSteelFrameLT = posSteelFrameRT;
                        posSteelFrameLB = posSteelFrameRB;
                        posSteelFrameRT = posSteelFrameLT + new Revit.DB.XYZ(_BraceNoteFrameWidth, 0, 0);
                        posSteelFrameRB = posSteelFrameRT + new Revit.DB.XYZ(0, -_BraceSteelFrameHeight * secNum, 0);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameLB, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);

                        line = _CmpGeometry.CreateBoundLine(posSteelFrameRT, posSteelFrameRB);
                        _CmpGeometry.NotNullCurveSet(ref frameLines, line);
                    }

                    posLT = new Revit.DB.XYZ(posCantiSrart.X, posSteelFrameLB.Y, 0);
                    posRightEnd = posSteelFrameRB;

                    // マテリアル違い
                    #region

                    // ウェブマテリアル 元端
                    string webMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")];
                    // フランジマテリアル 元端
                    string flangeMat_S = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];
                    // ウェブマテリアル 先端
                    string webMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E")];
                    // フランジマテリアル 先端
                    string flangeMat_E = (string)currentRow[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")];

                    string secName = "";

                    if (webMat_S != flangeMat_S)
                    {
                        secName = _SecMototan;
                    }
                    if (webMat_E != flangeMat_E)
                    {
                        if (secName == "")
                        {
                            secName = _SecSentan;
                        }
                        else
                        {
                            secName += ", " + _SecSentan;
                        }
                    }

                    if (secName != "")
                    {
                        if (_MaterialVary == "")
                        {
                            _MaterialVary = fugo + " (" + secName + ")";
                        }
                        else
                        {
                            _MaterialVary += ", " + fugo + " (" + secName + ")";
                        }
                    }
                    #endregion
                }
            }

            #endregion

            // 作図なし
            if (frameLines.Count == 0)
            {
                transac.Start("Remove View");
                _CmpElements.RemoveView(vp);
                transac.Commit();

                return ret;
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

            Collections.Generic.IList<Revit.DB.XYZ> posAry = dicTextNoteTitle.Keys.ToList<Revit.DB.XYZ>();

            for (int i = 0; i < dicTextNoteTitle.Count; ++i)
            {
                if (i == 0)
                {
                    lineWidth = _SubTitleFrameWidth / _ViewScale;
                }
                else if (i == 1)
                {
                    lineWidth = (_SubSteelFrameWidth) / _ViewScale;
                }
                else if (i == 1)
                {
                    lineWidth = _BraceNoteFrameWidth / _ViewScale;
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
                                 posRightEnd);

            if (transac.GetStatus() == Revit.DB.TransactionStatus.Started)
            {
                transac.Commit();
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>設定値取得</summary>
        ///
        /// <history><p>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/06/27 Created CST,Co.Ltd Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void GetSettingValues()
        {
            // ビュー尺度
            int.TryParse(_CmpParameters.ViewScaleBeam, out _ViewScale);

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
            double.TryParse(_CmpParameters.BeamSecFrameWidth, out _SecFrameWidth);
            double.TryParse(_CmpParameters.BeamSteelFrameWidth, out _SteelFrameWidth);
            double.TryParse(_CmpParameters.BeamSteelFrameHeight, out _SteelFrameHeight);
            double.TryParse(_CmpParameters.SubBeamSecFrameWidth, out _SubSecFrameWidth);
            double.TryParse(_CmpParameters.SubBeamSteelFrameWidth, out _SubSteelFrameWidth);
            double.TryParse(_CmpParameters.SubBeamNoteFrameWidth, out _SubNoteFrameWidth);
            double.TryParse(_CmpParameters.SubBeamSteelFrameHeight, out _SubSteelFrameHeight);

            _SecFrameWidth = _SecFrameWidth / 304.8 * _ViewScale;
            _SteelFrameWidth = _SteelFrameWidth / 304.8 * _ViewScale;
            _SteelFrameHeight = _SteelFrameHeight / 304.8 * _ViewScale;
            _SubSecFrameWidth = _SubSecFrameWidth / 304.8 * _ViewScale;
            _SubSteelFrameWidth = _SubSteelFrameWidth / 304.8 * _ViewScale;
            _SubNoteFrameWidth = _SubNoteFrameWidth / 304.8 * _ViewScale;
            _SubSteelFrameHeight = _SubSteelFrameHeight / 304.8 * _ViewScale;

            // 材質
            int.TryParse(_CmpParameters.BeamShowSteel, out _ShowSteel);
            int.TryParse(_CmpParameters.SubBeamShowSteel, out _SubShowSteel);
            int.TryParse(_CmpParameters.BraceShowSteel, out _BraceShowSteel);

            // 断面位置タイトル
            _SecZendan = _CmpParameters.BeamSecZendan;
            _SecChuoh = _CmpParameters.BeamSecChuoh;
            _SecTanbu = _CmpParameters.BeamSecTanbu;
            _SecShitan = _CmpParameters.BeamSecShitan;
            _SecSyutan = _CmpParameters.BeamSecSyutan;
            _SecMototan = _CmpParameters.BeamSecMototan;
            _SecSentan = _CmpParameters.BeamSecSentan;

            // 備考
            int.TryParse(_CmpParameters.BeamShowNote, out _ShowNote);

            _DicNote_Items = new Dictionary<int, string>();

            int.TryParse(_CmpParameters.Beam_SelectedNoteIndex, out _SelectedNoteIndex);
            _DicNote_Items.Add(1, _CmpParameters.Beam_NoteName1 + "|" + _CmpParameters.Beam_NoteHeight1);
            _DicNote_Items.Add(2, _CmpParameters.Beam_NoteName2 + "|" + _CmpParameters.Beam_NoteHeight2);
            _DicNote_Items.Add(3, _CmpParameters.Beam_NoteName3 + "|" + _CmpParameters.Beam_NoteHeight3);

            int.TryParse(_CmpParameters.SubBeamShowNote, out _SubShowNote);

            // リストの折り返し
            int.TryParse(_CmpParameters.BeamNewLine, out _NewLine);
            int.TryParse(_CmpParameters.BeamNewLineSpan, out _NewLineSpan);

            double.TryParse(_CmpParameters.BraceSteelFrameWidth, out _BraceSteelFrameWidth);
            double.TryParse(_CmpParameters.BraceNoteFrameWidth, out _BraceNoteFrameWidth);
            double.TryParse(_CmpParameters.BraceSteelFrameHeight, out _BraceSteelFrameHeight);

            _BraceSteelFrameWidth = _BraceSteelFrameWidth / 304.8 * _ViewScale;
            _BraceNoteFrameWidth = _BraceNoteFrameWidth / 304.8 * _ViewScale;
            _BraceSteelFrameHeight = _BraceSteelFrameHeight / 304.8 * _ViewScale;

            int.TryParse(_CmpParameters.BraceShowNote, out _BraceShowNote);
        }

        /// ================================================================================
        /// <summary>対象ファミリ取得</summary>
        ///
        /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool GetTarget()
        {
            bool ret = false;

            _GirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _CantiGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            _SteelLAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelUAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelCAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelFBAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelMAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelTAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelRectAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _SteelPAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            _CmpService.GirderDivision(null,
               ref _GirderAry,
               ref _CantiGirderAry,
               ref _SteelLAry,
               ref _SteelUAry,
               ref _SteelCAry,
               ref _SteelFBAry,
               ref _SteelMAry,
               ref _SteelTAry,
               ref _SteelRectAry,
               ref _SteelPAry
               );

            if (_GirderAry.Count > 0 ||
                _CantiGirderAry.Count > 0 ||
                _SteelLAry.Count > 0 ||
                _SteelUAry.Count > 0 ||
                _SteelCAry.Count > 0 ||

                _SteelFBAry.Count > 0 ||
                _SteelMAry.Count > 0 ||
                _SteelTAry.Count > 0 ||
                _SteelRectAry.Count > 0 ||
                _SteelPAry.Count > 0
                )
            {
                ret = true;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>大梁、小梁の分割</summary>
        ///
        /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool GirderDivision(bool isGirderType, bool isBeamType, bool isBrace)
        {
            // 戻り値
            bool ret = false;

            _Girders = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _Beams = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            _Braces = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            // 梁
            foreach (Revit.DB.FamilySymbol famSym in _GirderAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.GirderSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }

            //////////////////////////////////////////////////////////////////////////
            foreach (Revit.DB.FamilySymbol famSym in _SteelLAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.LGirderHashiyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelUAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.UGirderHashiyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelCAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CGirderHashiyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelFBAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.FBGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelMAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.MGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelTAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.TGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelRectAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.RectGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }
            foreach (Revit.DB.FamilySymbol famSym in _SteelPAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.PGirderBraceType);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }
            //////////////////////////////////////////////////////////////////////////

            // 片持ち梁
            foreach (Revit.DB.FamilySymbol famSym in _CantiGirderAry)
            {
                Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CantiGirderSyubetsu);

                string syubetsu = parSyubetsu.AsString();

                if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_CANTILEVER_GIRDER") && isGirderType)
                {
                    _Girders.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_CANTILEVER_BEAM") && isBeamType)
                {
                    _Beams.Add(famSym);
                }
                else if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE") && isBrace)
                {
                    _Braces.Add(famSym);
                }
            }

            if (_Girders.Count > 0 || _Beams.Count > 0 || _Braces.Count > 0)
            {
                ret = true;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>違うファミリのタイプ名重複確認</summary>
        ///
        /// <history><p>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2016/10/13 Modified CST,Co.Ltd Ryo Kuroda</p></history>
        /// ================================================================================
        public
        string OverlapTypeName(bool isGirder, bool isSubBeam, bool isBrace)
        {
            // 戻り値
            string ret = "";

            // 重複タイプ名
            Collections.Generic.IList<string> nameAry = new Collections.Generic.List<string>();

            Collections.Generic.IDictionary<long, Collections.Generic.IList<string>> dicIdName = new Collections.Generic.Dictionary<long, Collections.Generic.IList<string>>();

            //                 CheckOver(_GirderAry, dicIdName, ref nameAry);
            //
            //                 CheckOver(_CantiGirderAry, dicIdName, ref nameAry);
            //                 CheckOver(_SteelLAry, dicIdName, ref nameAry);
            //
            //                 CheckOver(_SteelUAry, dicIdName, ref nameAry);
            //
            //                 CheckOver(_SteelCAry, dicIdName, ref nameAry);
            //CheckOver(_SteelFBAry, dicIdName, ref nameAry);

            //CheckOver(_SteelMAry, dicIdName, ref nameAry);

            //CheckOver(_SteelTAry, dicIdName, ref nameAry);

            //CheckOver(_SteelRectAry, dicIdName, ref nameAry);

            //CheckOver(_SteelPAry, dicIdName, ref nameAry);

            // 梁
            if (isGirder)
            {
                CheckOver(_Girders, dicIdName, ref nameAry);
            }

            if (isSubBeam)
            {
                CheckOver(_Beams, dicIdName, ref nameAry);
            }

            if (isBrace)
            {
                CheckOver(_Braces, dicIdName, ref nameAry);
            }

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
        /// <summary>小梁符号名重複確認</summary>
        ///
        /// <history>2017/06/27 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        string OverlapSubs(bool isGirder, bool isSubBeam, bool isBrace)
        {
            string ret = "";

            Collections.Generic.IDictionary<string, Collections.Generic.IList<string>> dicSameFugo = new Collections.Generic.Dictionary<string, Collections.Generic.IList<string>>();

            if (isSubBeam)
            {
                foreach (Revit.DB.FamilySymbol famSym in _GirderAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.GirderSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _CantiGirderAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CantiGirderSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_CANTILEVER_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.CantiGirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelLAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.LGirderHashiyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelUAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.UGirderHashiyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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
                foreach (Revit.DB.FamilySymbol famSym in _SteelCAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CGirderHashiyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelFBAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.FBGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelMAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.MGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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
                foreach (Revit.DB.FamilySymbol famSym in _SteelTAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.TGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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
                foreach (Revit.DB.FamilySymbol famSym in _SteelRectAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.RectGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelPAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.PGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BEAM"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

            if (isBrace)
            {
                foreach (Revit.DB.FamilySymbol famSym in _GirderAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.GirderSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _CantiGirderAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CantiGirderSyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.CantiGirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelLAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.LGirderHashiyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelUAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.UGirderHashiyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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
                foreach (Revit.DB.FamilySymbol famSym in _SteelCAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.CGirderHashiyubetsu);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelFBAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.FBGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelMAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.MGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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
                foreach (Revit.DB.FamilySymbol famSym in _SteelTAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.TGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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
                foreach (Revit.DB.FamilySymbol famSym in _SteelRectAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.RectGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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

                foreach (Revit.DB.FamilySymbol famSym in _SteelPAry)
                {
                    // 梁種別
                    Revit.DB.Parameter parSyubetsu = famSym.LookupParameter(_CmpParameters.PGirderBraceType);

                    string syubetsu = parSyubetsu.AsString();

                    // 小梁
                    if (syubetsu == _CmpAttribute.ResourceText("IDS_TXT_BRACE"))
                    {
                        // 符号
                        Revit.DB.Parameter parFugo = famSym.LookupParameter(_CmpParameters.GirderFugo);

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
        /// <param name="girderAry"     >梁</param>
        /// <param name="cantiGirderAry">片持ち梁</param>
        ///
        /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> GetLevelName(Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> cantiGirderAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelLAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelUAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelCAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelFBAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelMAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelTAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelRectAry,
                                                       Collections.Generic.IList<Revit.DB.FamilySymbol> steelPAry)
        {
            Collections.Generic.List<string> ret = new Collections.Generic.List<string>();

            if (girderAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in girderAry)
                {
                    string fugoParamName = _CmpParameters.GirderFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }

            if (cantiGirderAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in cantiGirderAry)
                {
                    string fugoParamName = _CmpParameters.CantiGirderFugo;

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
                    string fugoParamName = _CmpParameters.GirderFugo;

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
                    string fugoParamName = _CmpParameters.GirderFugo;

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
                    string fugoParamName = _CmpParameters.GirderFugo;

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
                    string fugoParamName = _CmpParameters.GirderFugo;

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
                    string fugoParamName = _CmpParameters.GirderFugo;

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
                    string fugoParamName = _CmpParameters.GirderFugo;

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
                    string fugoParamName = _CmpParameters.GirderFugo;

                    string lvlName = _CmpParameters.GetTypeLevel(famSym, fugoParamName);

                    if (ret.Contains(lvlName) == false)
                    {
                        ret.Add(lvlName);
                    }
                }
            }

            if (steelPAry != null)
            {
                foreach (Revit.DB.FamilySymbol famSym in steelPAry)
                {
                    string fugoParamName = _CmpParameters.GirderFugo;

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

        /// ================================================================================
        /// <summary>階ごとの最大断面数 - 梁</summary>
        ///
        /// <param name="levelOrder">階記号順</param>
        /// <param name="data"      >データテーブル</param>
        /// <param name="fugoAry"   >対象符号</param>
        ///
        /// <history>2017/08/01 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IDictionary<string, int> DicGirderSectionNumByLevel(Collections.Generic.IList<string> levelOrder,
                                                                                System.Data.DataTable data,
                                                                                Collections.Generic.IList<string> fugoAry)
        {
            // 戻り値
            Collections.Generic.IDictionary<string, int> ret = new Collections.Generic.Dictionary<string, int>();

            foreach (string lvlName in levelOrder)
            {
                // 断面数
                int secNum = 1;

                foreach (System.Data.DataRow row in data.Rows)
                {
                    string fugo = (string)row[_CmpAttribute.ResourceText("IDS_CN_FUGO")];

                    if (fugoAry.Contains(fugo) == false)
                    {
                        continue;
                    }

                    string currentLvlName = (string)row[_CmpAttribute.ResourceText("IDS_CN_KAI")];

                    if (lvlName != currentLvlName)
                    {
                        continue;
                    }

                    // 断面数
                    int girderType = (int)row[_CmpAttribute.ResourceText("IDS_CN_TYPE")];
                    if (girderType != 1)
                    {
                        secNum = 1;
                        break;
                    }
                    int currentSecNum = 0;

                    // 始端
                    // せい
                    string sei_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S")];
                    // 幅
                    string haba_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S")];
                    // ウェブ厚
                    string webAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S")];
                    // フランジ厚
                    string flangeAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S")];
                    // ウェブマテリアル
                    string webMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_S")];
                    // フランジマテリアル
                    string flangeMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
                    // ハンチ長さ
                    string haunchNagasa_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S")];
                    // フィレット
                    string fillet_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S")];

                    string girderMark_S = (string)row[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")];

                    // 中央
                    // せい
                    string sei_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
                    // 幅
                    string haba_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
                    // ウェブ厚
                    string webAtsu_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
                    // フランジ厚
                    string flangeAtsu_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
                    // ウェブマテリアル
                    string webMat_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_C")];
                    // フランジマテリアル
                    string flangeMat_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

                    string girderMark_C = (string)row[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")];

                    // 終端
                    // せい
                    string sei_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_E")];
                    // 幅
                    string haba_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_E")];
                    // ウェブ厚
                    string webAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_E")];
                    // フランジ厚
                    string flangeAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_E")];
                    // ウェブマテリアル
                    string webMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_E")];
                    // フランジマテリアル
                    string flangeMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")];
                    // ハンチ長さ
                    string haunchNagasa_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_E")];
                    // フィレット
                    string fillet_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_E")];

                    string girderMark_E = (string)row[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E")];

                    // 両端が違えば3断面
                    if (sei_S != sei_E ||
                        haba_S != haba_E ||
                        webAtsu_S != webAtsu_E ||
                        flangeAtsu_S != flangeAtsu_E ||
                        webMat_S != webMat_E ||
                        flangeMat_S != flangeMat_E ||

                        //(bh_S == "0" && bh_E == "0" && haunchNagasa_S == "0" && haunchNagasa_E != "0" && sei_E != sei_C) ||
                        //(bh_S == "0" && bh_E == "0" && haunchNagasa_S != "0" && haunchNagasa_E == "0" && sei_S != sei_C) ||
                        (fillet_S == "0" && fillet_E != "0" && haunchNagasa_S != "0" && sei_S != sei_C && sei_E != sei_C) ||
                        (fillet_S != "0" && fillet_E == "0" && haunchNagasa_E != "0" && sei_S != sei_C && sei_E != sei_C) ||
                        (fillet_S == "0" && fillet_E == "0" && haunchNagasa_S != "0" && haunchNagasa_E == "0" && sei_S != sei_C && sei_E != sei_C) ||
                        (fillet_S == "0" && fillet_E == "0" && haunchNagasa_S == "0" && haunchNagasa_E != "0" && sei_S != sei_C && sei_E != sei_C) ||

                        girderMark_S != girderMark_E ||
                        (fillet_S == "0" && fillet_E != "0" && haunchNagasa_S != "0" && girderMark_S != girderMark_C && girderMark_E != girderMark_C) ||
                        (fillet_S != "0" && fillet_E == "0" && haunchNagasa_E != "0" && girderMark_S != girderMark_C && girderMark_E != girderMark_C) ||
                        (fillet_S == "0" && fillet_E == "0" && haunchNagasa_S != "0" && haunchNagasa_E == "0" && girderMark_S != girderMark_C && girderMark_E != girderMark_C) ||
                        (fillet_S == "0" && fillet_E == "0" && haunchNagasa_S == "0" && haunchNagasa_E != "0" && girderMark_S != girderMark_C && girderMark_E != girderMark_C))
                    {
                        currentSecNum = 3;
                    }

                    // 始端(または終端)と中央が違えば2断面
                    else if (sei_S != sei_C ||
                             haba_S != haba_C ||
                             webAtsu_S != webAtsu_C ||
                             flangeAtsu_S != flangeAtsu_C ||
                             webMat_S != webMat_C ||
                             flangeMat_S != flangeMat_C ||
                             girderMark_S != girderMark_C)
                    {
                        currentSecNum = 2;
                    }
                    // それ以外は1断面
                    else
                    {
                        currentSecNum = 1;
                    }

                    if (secNum < currentSecNum)
                    {
                        secNum = currentSecNum;

                        if (secNum == 3)
                        {
                            break;
                        }
                    }
                }

                ret.Add(lvlName, secNum);
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>階ごとの最大断面数 - 片持ち梁</summary>
        ///
        /// <param name="levelOrder">階記号順</param>
        /// <param name="data"      >データテーブル</param>
        /// <param name="fugoAry"   >対象符号</param>
        ///
        /// <history>2017/06/21 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IDictionary<string, int> DicCantiGirderSectionNumByLevel(Collections.Generic.IList<string> levelOrder,
                                                                                     System.Data.DataTable data,
                                                                                     Collections.Generic.IList<string> fugoAry)
        {
            // 戻り値
            Collections.Generic.IDictionary<string, int> ret = new Collections.Generic.Dictionary<string, int>();

            foreach (string lvlName in levelOrder)
            {
                // 断面数
                int secNum = 1;

                foreach (System.Data.DataRow row in data.Rows)
                {
                    string fugo = (string)row[_CmpAttribute.ResourceText("IDS_CN_FUGO")];

                    if (fugoAry.Contains(fugo) == false)
                    {
                        continue;
                    }

                    string currentLvlName = (string)row[_CmpAttribute.ResourceText("IDS_CN_KAI")];

                    if (lvlName != currentLvlName)
                    {
                        continue;
                    }

                    int currentSecNum = 0;

                    // 元端
                    // せい
                    string sei_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")];
                    // 幅
                    string haba_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")];
                    // ウェブ厚
                    string webAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")];
                    // フランジ厚
                    string flangeAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")];
                    // ウェブマテリアル
                    string webMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")];
                    // フランジマテリアル
                    string flangeMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];

                    string cantiGirderMark_S = (string)row[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")];

                    // 先端
                    // せい
                    string sei_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_E")];
                    // 幅
                    string haba_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_E")];
                    // ウェブ厚
                    string webAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_E")];
                    // フランジ厚
                    string flangeAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_E")];
                    // ウェブマテリアル
                    string webMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E")];
                    // フランジマテリアル
                    string flangeMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")];

                    string cantiGirderMark_E = (string)row[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_E")];

                    // 両端が違えば2断面
                    if (sei_S != sei_E ||
                        haba_S != haba_E ||
                        webAtsu_S != webAtsu_E ||
                        flangeAtsu_S != flangeAtsu_E ||
                        webMat_S != webMat_E ||
                        flangeMat_S != flangeMat_E ||
                        cantiGirderMark_S != cantiGirderMark_E)
                    {
                        currentSecNum = 2;
                    }
                    // それ以外は1断面
                    else
                    {
                        currentSecNum = 1;
                    }

                    if (secNum < currentSecNum)
                    {
                        secNum = currentSecNum;

                        if (secNum == 2)
                        {
                            break;
                        }
                    }
                }

                ret.Add(lvlName, secNum);
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>階ごとの最大断面数 - 片持ち梁</summary>
        ///
        /// <param name="levelOrder">階記号順</param>
        /// <param name="data"      >データテーブル</param>
        ///
        /// <history>2016/09/02 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IDictionary<string, int> DicCantiGirderSectionNumByLevel(Collections.Generic.IList<string> levelOrder,
                                                                                     System.Data.DataTable data)
        {
            // 戻り値
            Collections.Generic.IDictionary<string, int> ret = new Collections.Generic.Dictionary<string, int>();

            foreach (string lvlName in levelOrder)
            {
                // 断面数
                int secNum = 0;

                foreach (System.Data.DataRow row in data.Rows)
                {
                    string currentLvlName = (string)row[_CmpAttribute.ResourceText("IDS_CN_KAI")];

                    if (lvlName != currentLvlName)
                    {
                        continue;
                    }

                    int currentSecNum = 0;

                    // 元端
                    // せい
                    string sei_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")];
                    // 幅
                    string haba_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")];
                    // ウェブ厚
                    string webAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")];
                    // フランジ厚
                    string flangeAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")];
                    // ウェブマテリアル
                    string webMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")];
                    // フランジマテリアル
                    string flangeMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];

                    string cantiGirderMark_S = (string)row[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")];

                    // 先端
                    // せい
                    string sei_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_E")];
                    // 幅
                    string haba_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_E")];
                    // ウェブ厚
                    string webAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_E")];
                    // フランジ厚
                    string flangeAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_E")];
                    // ウェブマテリアル
                    string webMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E")];
                    // フランジマテリアル
                    string flangeMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")];

                    string cantiGirderMark_E = (string)row[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_E")];

                    // 両端が違えば2断面
                    if (sei_S != sei_E ||
                        haba_S != haba_E ||
                        webAtsu_S != webAtsu_E ||
                        flangeAtsu_S != flangeAtsu_E ||
                        webMat_S != webMat_E ||
                        flangeMat_S != flangeMat_E ||
                        cantiGirderMark_S != cantiGirderMark_E)
                    {
                        currentSecNum = 2;
                    }
                    // それ以外は1断面
                    else
                    {
                        currentSecNum = 1;
                    }

                    if (secNum < currentSecNum)
                    {
                        secNum = currentSecNum;

                        if (secNum == 2)
                        {
                            break;
                        }
                    }
                }

                ret.Add(lvlName, secNum);
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>断面数 - 梁</summary>
        ///
        /// <param name="row">梁データ</param>
        ///
        /// <history><p>2016/09/07 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2017/08/01 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        int GirderSectionNum(System.Data.DataRow row)
        {
            // 戻り値
            int ret = 0;

            // 始端
            // せい
            string sei_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_S")];
            // 幅
            string haba_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_S")];
            // ウェブ厚
            string webAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_S")];
            // フランジ厚
            string flangeAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_S")];
            // ウェブマテリアル
            string webMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_S")];
            // フランジマテリアル
            string flangeMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_S")];
            // ハンチ長さ
            string haunchNagasa_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_S")];
            // フィレット
            string fillet_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_S")];

            string girderMark_S = (string)row[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_S")];

            // 中央
            // せい
            string sei_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_C")];
            // 幅
            string haba_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_C")];
            // ウェブ厚
            string webAtsu_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_C")];
            // フランジ厚
            string flangeAtsu_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_C")];
            // ウェブマテリアル
            string webMat_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_C")];
            // フランジマテリアル
            string flangeMat_C = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_C")];

            string girderMark_C = (string)row[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_C")];

            // 終端
            // せい
            string sei_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_SEI_E")];
            // 幅
            string haba_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HABA_E")];
            // ウェブ厚
            string webAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBATSU_E")];
            // フランジ厚
            string flangeAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEATSU_E")];
            // ウェブマテリアル
            string webMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_WEBMATERIAL_E")];
            // フランジマテリアル
            string flangeMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FLANGEMATERIAL_E")];
            // ハンチ長さ
            string haunchNagasa_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_HAUNCHNAGASA_E")];
            // フィレット
            string fillet_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_GIRDER_FILLET_E")];

            string girderMark_E = (string)row[_CmpAttribute.ResourceText("IDS_TXT_GIRDER_E")];

            // 両端が違えば3断面
            if (sei_S != sei_E ||
                haba_S != haba_E ||
                webAtsu_S != webAtsu_E ||
                flangeAtsu_S != flangeAtsu_E ||
                webMat_S != webMat_E ||
                flangeMat_S != flangeMat_E ||

                //(bh_S == "0" && bh_E == "0" && haunchNagasa_S == "0" && haunchNagasa_E != "0" && sei_E != sei_C) ||
                //(bh_S == "0" && bh_E == "0" && haunchNagasa_S != "0" && haunchNagasa_E == "0" && sei_S != sei_C) ||
                (fillet_S == "0" && fillet_E != "0" && haunchNagasa_S != "0" && sei_S != sei_C && sei_E != sei_C) ||
                (fillet_S != "0" && fillet_E == "0" && haunchNagasa_E != "0" && sei_S != sei_C && sei_E != sei_C) ||
                (fillet_S == "0" && fillet_E == "0" && haunchNagasa_S != "0" && haunchNagasa_E == "0" && sei_S != sei_C && sei_E != sei_C) ||
                (fillet_S == "0" && fillet_E == "0" && haunchNagasa_S == "0" && haunchNagasa_E != "0" && sei_S != sei_C && sei_E != sei_C) ||

                        girderMark_S != girderMark_E ||
                        (fillet_S == "0" && fillet_E != "0" && haunchNagasa_S != "0" && girderMark_S != girderMark_C && girderMark_E != girderMark_C) ||
                        (fillet_S != "0" && fillet_E == "0" && haunchNagasa_E != "0" && girderMark_S != girderMark_C && girderMark_E != girderMark_C) ||
                        (fillet_S == "0" && fillet_E == "0" && haunchNagasa_S != "0" && haunchNagasa_E == "0" && girderMark_S != girderMark_C && girderMark_E != girderMark_C) ||
                        (fillet_S == "0" && fillet_E == "0" && haunchNagasa_S == "0" && haunchNagasa_E != "0" && girderMark_S != girderMark_C && girderMark_E != girderMark_C))
            {
                ret = 3;
            }
            // 始端(または終端)と中央が違えば2断面
            else if (sei_S != sei_C ||
                     haba_S != haba_C ||
                     webAtsu_S != webAtsu_C ||
                     flangeAtsu_S != flangeAtsu_C ||
                     webMat_S != webMat_C ||
                     flangeMat_S != flangeMat_C ||

                    girderMark_S != girderMark_C)
            {
                ret = 2;
            }
            // それ以外は1断面
            else
            {
                ret = 1;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>断面数 - 片持ち梁</summary>
        ///
        /// <param name="row">片持ち梁データ</param>
        ///
        /// <history>2016/09/07 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        int CantiGirderSectionNum(System.Data.DataRow row)
        {
            // 戻り値
            int ret = 0;

            // 元端
            // せい
            string sei_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_S")];
            // 幅
            string haba_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_S")];
            // ウェブ厚
            string webAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_S")];
            // フランジ厚
            string flangeAtsu_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_S")];
            // ウェブマテリアル
            string webMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_S")];
            // フランジマテリアル
            string flangeMat_S = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_S")];

            string cantiGirderMark_S = (string)row[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_S")];

            // 先端
            // せい
            string sei_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_SEI_E")];
            // 幅
            string haba_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_HABA_E")];
            // ウェブ厚
            string webAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBATSU_E")];
            // フランジ厚
            string flangeAtsu_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEATSU_E")];
            // ウェブマテリアル
            string webMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_WEBMATERIAL_E")];
            // フランジマテリアル
            string flangeMat_E = (string)row[_CmpAttribute.ResourceText("IDS_CN_CANTIGIRDER_FLANGEMATERIAL_E")];

            string cantiGirderMark_E = (string)row[_CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER_E")];

            // 両端が違えば2断面
            if (sei_S != sei_E ||
                haba_S != haba_E ||
                webAtsu_S != webAtsu_E ||
                flangeAtsu_S != flangeAtsu_E ||
                webMat_S != webMat_E ||
                flangeMat_S != flangeMat_E ||
                cantiGirderMark_S != cantiGirderMark_E)
            {
                ret = 2;
            }
            // それ以外は1断面
            else
            {
                ret = 1;
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

        #endregion
    }
}