using System;
using System.Linq;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using SectionListRC.Setting;
using System.Windows.Forms;
using System.Data;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using SectionListRC.Utils;

namespace SectionListRC.ColumnList
{
    /// ================================================================================
    /// <summary>コマンド 柱リスト</summary>
    /// ================================================================================
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdColumnList : Revit.UI.IExternalCommand
    {
        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>コマンド実行処理</summary>
        ///
        /// <param name="commandData" >Revit コマンドデータ</param>
        /// <param name="message"     >エラーメッセージ</param>
        /// <param name="elements"    >エラー要素</param>
        ///
        /// <returns>実行結果</returns>
        ///
        /// <history><p>2013/02/04 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2015/06/26 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                       ref string message,
                                       Revit.DB.ElementSet elements)
        {
            // 初期化
            Revit.UI.UIApplication rvtUiApp = commandData.Application;
            Revit.DB.Document rvtDbDoc = rvtUiApp.ActiveUIDocument.Document;
            Revit.UI.UIDocument rvtUiDoc = commandData.Application.ActiveUIDocument;
            Revit.ApplicationServices.Application rvtSrvcApp = rvtUiApp.Application;
            SectionListRC.Components.Attribute cmpAttribute = new SectionListRC.Components.Attribute();
            SectionListRC.Components.Elements cmpElements = new SectionListRC.Components.Elements(cmpAttribute, rvtUiDoc);
            SectionListRC.Components.Geometry cmpGeometry = new SectionListRC.Components.Geometry(rvtUiDoc);
            SectionListRC.Components.Parameters cmpParameters = new SectionListRC.Components.Parameters(cmpAttribute, rvtUiDoc);
            SectionListRC.Components.Settings cmpSettings = new SectionListRC.Components.Settings(rvtUiDoc);
            SectionListRC.Components.Service cmpService = new SectionListRC.Components.Service(cmpAttribute,
                                                                                                 cmpElements,
                                                                                                 cmpGeometry,
                                                                                                 cmpParameters,
                                                                                                 cmpSettings);

            // トランザクショングループ
            Revit.DB.TransactionGroup transGroup = new Revit.DB.TransactionGroup(cmpElements.RvtDBDoc);
            // スタート
            transGroup.Start(cmpAttribute.ResourceText("IDS_BTN_COLUMNLIST_NAME"));

            // 戻り値
            Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;

            Revit.DB.TextAlignFlags flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER;

            // ファミリドキュメント
            if (rvtDbDoc.IsFamilyDocument)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FAMILYDOCUMENT"));
                transGroup.Assimilate();
                return retCmd;
            }

            // プロジェクトブラウザ
            if (rvtDbDoc.ActiveView.ViewType == Revit.DB.ViewType.ProjectBrowser)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_ACTIVEPROJECTBROWSE"));
                transGroup.Assimilate();

                return retCmd;
            }

            // ワークフロー
            Revit.DB.Transaction transac = new Revit.DB.Transaction(rvtDbDoc);
            transac.Start("フロー");
            string retMsg1 = cmpService.WorkFlow(cmpAttribute.ResourceText("IDS_SHARE_PARA_COLUMN_RANGER"));
            string retMsg2 = cmpService.WorkFlow(cmpAttribute.ResourceText("IDS_SHPARAM_DEF"));

            transac.Commit();
            if (retMsg1 != null)
            {
                System.Windows.MessageBox.Show(retMsg1);
                retCmd = Revit.UI.Result.Failed;
            }
            if (retMsg2 != null)
            {
                System.Windows.MessageBox.Show(retMsg2);
                retCmd = Revit.UI.Result.Failed;
            }

            // 構造平面取得判定
            if (cmpElements.IsStrPlaneGet() == false)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTSTRPLANEVIEW"));
                transGroup.Assimilate();
                return retCmd;
            }

            // 設定ファイル名
            string settingFileName = "";
            // 設定ファイルディレクトリ
            string settingFileDirectory = "";

            string levelSortOrder = "";

            cmpService.GetString(ref settingFileName, ref settingFileDirectory, ref levelSortOrder);

            if (cmpParameters.GetSettingValue(settingFileName, settingFileDirectory).Length != 63 &&
                cmpParameters.GetSettingValue(settingFileName, settingFileDirectory).Length != 64)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETTINGFILE"));
                transGroup.Assimilate();
                return retCmd;
            }

            // 設定ファイルの値を取得
            string[] strSetAry = cmpParameters.GetSettingValue(settingFileName, settingFileDirectory);

            cmpParameters.GetSettingValue(strSetAry);

            // 予備ファイルコピー
            cmpParameters.ReserveFileCopy();

            // パラメータ名の取得
            Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> allParamName = cmpParameters.GetParamName();

            if (allParamName == null ||
                allParamName.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILE"));

                transGroup.Assimilate();
                return retCmd;
            }

            // 矩形柱パラメータ名
            Collections.Generic.IDictionary<string, string> dicKakuParamNames = null;
            // 円柱パラメータ名
            Collections.Generic.IDictionary<string, string> dicEnParamNames = null;

            // 梁パラメータ名
            Collections.Generic.IDictionary<string, string> dicHariParamNames = null;
            // 片持ち梁パラメータ名
            Collections.Generic.IDictionary<string, string> dicKatamotiParamNames = null;

            cmpParameters.GetColumnParamName(ref dicKakuParamNames,
                                             ref dicEnParamNames,
                                             ref dicHariParamNames,
                                             ref dicKatamotiParamNames);

            if (dicKakuParamNames == null ||
                dicKakuParamNames.Count == 0 ||
                dicEnParamNames == null ||
                dicEnParamNames.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILEVALUE"));
                transGroup.Assimilate();

                return retCmd;
            }

            // パラメータ名の重複判定
            string isOverlap_Kaku = cmpService.IsOverlapStrings_Kaku(dicKakuParamNames);
            string isOverlap_En = cmpService.IsOverlapStrings_En(dicEnParamNames);

            if (isOverlap_Kaku != "" || isOverlap_En != "")
            {
                string errMsg = isOverlap_Kaku;
                if (errMsg != "")
                {
                    errMsg += "\r\n";
                }
                errMsg += isOverlap_En;

                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETTINGOVERLAP") + "\r\n\r\n" + errMsg);
                transGroup.Assimilate();
                return retCmd;
            }

            cmpParameters.GetParameterValue(dicKakuParamNames,
                                            dicEnParamNames,
                                            dicHariParamNames,
                                            dicKatamotiParamNames);

            // 設定値を取得
            #region

            // 尺度
            int listViewScale = 0;
            int.TryParse(cmpParameters.ColumnListViewScale, out listViewScale);
            // 文字タイプ
            Revit.DB.TextNoteType titleTNT = cmpElements.TxtNoteTypeByName(cmpParameters.TitleFont);
            Revit.DB.TextNoteType otherTNT = cmpElements.TxtNoteTypeByName(cmpParameters.ItemFont);
            // 寸法線タイプ
            Revit.DB.DimensionType dimType = cmpElements.DimTypeByName(cmpParameters.DimensionType);
            // 線種タイプ
            Revit.DB.GraphicsStyle frameLineType = null;
            Revit.DB.GraphicsStyle bodyLineType = null;
            Revit.DB.GraphicsStyle spaceLineType = null;
            // 階表示枠表示
            int lvlFrameShow = 0;
            int.TryParse(cmpParameters.LevelFrameShow, out lvlFrameShow);
            // 階表示枠幅
            double lvlFrameWidth = 0;
            double.TryParse(cmpParameters.LevelFrameWidth, out lvlFrameWidth);
            // 項目表示枠幅
            double itemFrameWidth = 0;
            double.TryParse(cmpParameters.ItemFrameWidth, out itemFrameWidth);
            // 符号表示枠高さ
            double symbolFrameHeight = 0;
            double.TryParse(cmpParameters.SymbolFrameHeight, out symbolFrameHeight);
            // 配筋枠高さ
            double arrangementFrameHeight = 0;
            double.TryParse(cmpParameters.ArrangementFrameHeight, out arrangementFrameHeight);

            if (lvlFrameShow != 0)
            {
                // 項目表示枠幅
                double.TryParse(cmpParameters.ItemFrameWidth2, out itemFrameWidth);
                // 符号表示枠高さ
                double.TryParse(cmpParameters.SymbolFrameHeight2, out symbolFrameHeight);
                // 配筋枠高さ
                double.TryParse(cmpParameters.ArrangementFrameHeight2, out arrangementFrameHeight);
            }

            // 階表示枠タイトル
            string lvlFrameTitle = cmpParameters.LevelFrameTitle;
            // 階表示枠接尾語
            string lvlFrameEndWord = cmpParameters.LevelFrameEndWord;
            // 符号表示枠タイトル
            string symbolFrameTitle = cmpParameters.SymbolFrameTitle;

            // 左のあき
            double leftSpace = 0;
            double.TryParse(cmpParameters.ColumnLeftSpace, out leftSpace);
            // 右のあき
            double rightSpace = 0;
            double.TryParse(cmpParameters.ColumnRightSpace, out rightSpace);
            // 上のあき
            double topSpace = 0;
            double.TryParse(cmpParameters.ColumnTopSpace, out topSpace);
            // 下のあき
            double bottomSpace = 0;
            double.TryParse(cmpParameters.ColumnBottomSpace, out bottomSpace);
            // 帯筋括弧表示
            int hoopBracketShow = 0;
            int.TryParse(cmpParameters.HoopBracketShow, out hoopBracketShow);
            // 追加枠数
            int addFrameNumber = 0;
            int.TryParse(cmpParameters.ColumnAddFrameNumber, out addFrameNumber);
            // 主筋表示方法
            int rebarShow = 0;
            int.TryParse(cmpParameters.ColumnRebarShow, out rebarShow);
            // 帯筋枠タイトル
            string hoopFrameTitle = cmpParameters.HoopFrameTitle;
            // 帯筋枠区切り記号
            string hoopSpaceSymbol = cmpParameters.HoopFrameSpaceSymbol;

            // 芯鉄筋枠タイトル
            string coreRebarTitle = cmpAttribute.ResourceText("IDS_TXT_COREREBAR");

            //かぶり厚(角柱)
            double kaburi_kaku = 0;
            double.TryParse(cmpParameters.ColumnProtectThick, out kaburi_kaku);
            // かぶり厚(円柱)
            double kaburi_en = 0;
            double.TryParse(cmpParameters.CylinderProtectThick, out kaburi_en);
            // 2段筋コーナー配筋フラグ
            int secondConrnerSetFlag = 0;
            int.TryParse(cmpParameters.SecondRebarCornerSetFlag, out secondConrnerSetFlag);

            // 柱分類
            string column_Category_Kaku = cmpParameters.HashiraBunrui_Kaku;
            // 柱幅
            string dX = cmpParameters.DX_Kaku;
            // 柱成
            string dY = cmpParameters.DY_Kaku;

            // 柱頭主筋X1段太径本数
            string chutoSyukinX1HutokeiHonsu = cmpParameters.RST_ChutoSyukinX1danHutokeiHonsu_Kaku;
            // 柱頭主筋Y1段太径本数
            string chutoSyukinY1HutokeiHonsu = cmpParameters.RST_ChutoSyukinY1danHutokeiHonsu_Kaku;
            // 柱頭主筋X2段太径本数
            string chutoSyukinX2HutokeiHonsu = cmpParameters.RST_ChutoSyukinX2danHutokeiHonsu_Kaku;
            // 柱頭主筋Y2段太径本数
            string chutoSyukinY2HutokeiHonsu = cmpParameters.RST_ChutoSyukinY2danHutokeiHonsu_Kaku;
            // 柱頭主筋太径
            string chutoSyukinHutokei = cmpParameters.RST_ChutoSyukinHutokei_Kaku;

            // 柱頭主筋X1段細径本数
            string chutoSyukinX1HosokeiHonsu = cmpParameters.RST_ChutoSyukinX1danHosokeiHonsu_Kaku;
            // 柱頭主筋Y1段細径本数
            string chutoSyukinY1HosokeiHonsu = cmpParameters.RST_ChutoSyukinY1danHosokeiHonsu_Kaku;
            // 柱頭主筋X2段細径本数
            string chutoSyukinX2HosokeiHonsu = cmpParameters.RST_ChutoSyukinX2danHosokeiHonsu_Kaku;
            // 柱頭主筋Y2段細径本数
            string chutoSyukinY2HosokeiHonsu = cmpParameters.RST_ChutoSyukinY2danHosokeiHonsu_Kaku;
            // 柱頭主筋細径
            string chutoSyukinHosokei = cmpParameters.RST_ChutoSyukinHosokei_Kaku;

            // 柱脚主筋X1段太径本数
            string chukyakuSyukinX1HutokeiHonsu = cmpParameters.RST_ChukyakuSyukinX1danHutokeiHonsu_Kaku;
            // 柱脚主筋Y1段太径本数
            string chukyakuSyukinY1HutokeiHonsu = cmpParameters.RST_ChukyakuSyukinY1danHutokeiHonsu_Kaku;
            // 柱脚主筋X2段太径本数
            string chukyakuSyukinX2HutokeiHonsu = cmpParameters.RST_ChukyakuSyukinX2danHutokeiHonsu_Kaku;
            // 柱脚主筋Y2段太径本数
            string chukyakuSyukinY2HutokeiHonsu = cmpParameters.RST_ChukyakuSyukinY2danHutokeiHonsu_Kaku;
            // 柱脚主筋太径
            string chukyakuSyukinHutokei = cmpParameters.RST_ChukyakuSyukinHutokei_Kaku;

            // 柱脚主筋X1段細径本数
            string chukyakuSyukinX1HosokeiHonsu = cmpParameters.RST_ChukyakuSyukinX1danHosokeiHonsu_Kaku;
            // 柱脚主筋Y1段細径本数
            string chukyakuSyukinY1HosokeiHonsu = cmpParameters.RST_ChukyakuSyukinY1danHosokeiHonsu_Kaku;
            // 柱脚主筋X1段細径本数
            string chukyakuSyukinX2HosokeiHonsu = cmpParameters.RST_ChukyakuSyukinX2danHosokeiHonsu_Kaku;
            // 柱脚主筋Y1段細径本数
            string chukyakuSyukinY2HosokeiHonsu = cmpParameters.RST_ChukyakuSyukinY2danHosokeiHonsu_Kaku;
            // 柱脚主筋細径
            string chukyakuSyukinHosokei = cmpParameters.RST_ChukyakuSyukinHosokei_Kaku;

            // 柱頭帯筋X本数
            string chutoHoopXHonsu = cmpParameters.RST_ChutoHoopXHonsu_Kaku;
            // 柱頭帯筋Y本数
            string chutoHoopYHonsu = cmpParameters.RST_ChutoHoopYHonsu_Kaku;
            // 柱頭帯筋X径
            string chutoHoopXkei = cmpParameters.RST_ChutoHoopXKei_Kaku;
            // 柱頭帯筋ピッチ
            string chutoHoopPitch = cmpParameters.RST_ChutoHoopPitch_Kaku;

            // 柱脚帯筋X本数
            string chukyakuHoopXHonsu = cmpParameters.RST_ChukyakuHoopXHonsu_Kaku;
            // 柱脚帯筋Y本数
            string chukyakuHoopYHonsu = cmpParameters.RST_ChukyakuHoopYHonsu_Kaku;
            // 柱脚帯筋X径
            string chukyakuHoopXkei = cmpParameters.RST_ChukyakuHoopXKei_Kaku;
            // 柱脚帯筋ピッチ
            string chukyakuHoopPitch = cmpParameters.RST_ChukyakuHoopPitch_Kaku;

            // 芯鉄筋本数
            string sintekkinNumber_kaku = cmpParameters.CoreRebar_Number_Kaku;
            // 芯鉄筋径
            string sintekkinkei_kaku = cmpParameters.RST_SintekkinKei_Kaku;

            // 柱符号
            string hasiraHugo_Kaku = cmpParameters.RST_HasiraHugo_Kaku;

            // 柱分類
            string column_Category_En = cmpParameters.Column_Category_En;
            // 直径
            string tyokkei = cmpParameters.Tyokkei_En;
            // 柱頭主筋径
            string chutoSyukinkei = cmpParameters.RST_ChutoSyukinKei_En;
            // 柱脚主筋径
            string chukyakuSyukinkei = cmpParameters.RST_ChukyakuSyukinKei_En;
            // 柱頭主筋本数
            string chutoSyukinHonsu = cmpParameters.RST_ChutoSyukinHonsu_En;
            // 柱脚主筋本数
            string chukyakuSyukinHonsu = cmpParameters.RST_ChukyakuSyukinHonsu_En;
            // 芯鉄筋径
            string sintekkinkei_en = cmpParameters.RST_SintekkinKei_En;
            // 芯鉄筋本数
            string sintekkinNumber = cmpParameters.RST_SintekkinHonsu_En;
            // 芯鉄筋位置
            string sintekkinIchi = cmpParameters.RST_SintekkinIchi_En;
            // 柱頭フープX径
            string chutoHoopXkei_en = cmpParameters.RST_ChutoHoopXKei_En;
            // 柱脚フープX径
            string chukyakuHoopXkei_en = cmpParameters.RST_ChukyakuHoopXKei_En;
            // 柱頭フープピッチ
            string chutoHoopPitch_en = cmpParameters.RST_ChutoHoopPitch_En;
            // 柱脚フープピッチ
            string chukyakuHoopPitch_en = cmpParameters.RST_ChukyakuHoopPitch_En;

            // 柱符号
            string hasiraHugo_En = cmpParameters.RST_HasiraHugo_En;

            #endregion Member Functions

            Revit.DB.Transaction trans = new Revit.DB.Transaction(rvtDbDoc);
            cmpService.trans = trans;

            // 鉄筋ファミリ
            #region

            Revit.DB.Family rebarFam = null;
            bool isHaveFam = cmpElements.GetRebarFamily(ref rebarFam);

            if (isHaveFam == false)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOFAMILY"));
                transGroup.Assimilate();
                return retCmd;
            }

            // 寄せ筋記号ファミリ
            Revit.DB.Family rebar2ndFam = null;
            bool isHaveRebar2ndFam = cmpElements.GetRebarYose(ref rebar2ndFam);

            Collections.Generic.ISet<Revit.DB.ElementId> fsSet = rebar2ndFam.GetFamilySymbolIds();
            Revit.DB.FamilySymbol famSymYoseMigi = null;
            Revit.DB.FamilySymbol famSymYoseUe = null;
            Revit.DB.FamilySymbol famSymYoseHidari = null;
            Revit.DB.FamilySymbol famSymYoseSita = null;

            double markSize = kaburi_kaku / 304.8;
            string paramHaba = cmpAttribute.ResourceText("IDS_TXT_PARAMNAMEHABA");

            if (isHaveRebar2ndFam == true)
            {
                foreach (Revit.DB.ElementId id in fsSet)
                {
                    Revit.DB.FamilySymbol fs = rvtDbDoc.GetElement(id) as Revit.DB.FamilySymbol;

                    trans.Start("ファミリのアクティブ化");
                    fs.Activate();
                    trans.Commit();

                    if (fs.Name == cmpAttribute.ResourceText("IDS_TXT_YOSEMIGI"))
                    {
                        famSymYoseMigi = fs;

                        transac.Start("記号幅");
                        famSymYoseMigi.LookupParameter(paramHaba).Set(markSize);
                        transac.Commit();
                    }
                    else if (fs.Name == cmpAttribute.ResourceText("IDS_TXT_YOSEUE"))
                    {
                        famSymYoseUe = fs;

                        transac.Start("記号幅");
                        famSymYoseUe.LookupParameter(paramHaba).Set(markSize);
                        transac.Commit();
                    }
                    else if (fs.Name == cmpAttribute.ResourceText("IDS_TXT_YOSEHIDARI"))
                    {
                        famSymYoseHidari = fs;

                        transac.Start("記号幅");
                        famSymYoseHidari.LookupParameter(paramHaba).Set(markSize);
                        transac.Commit();
                    }
                    else if (fs.Name == cmpAttribute.ResourceText("IDS_TXT_YOSESITA"))
                    {
                        famSymYoseSita = fs;

                        transac.Start("記号幅");
                        famSymYoseSita.LookupParameter(paramHaba).Set(markSize);
                        transac.Commit();
                    }
                }
            }

            #endregion

            // 線種タイプ取得
            frameLineType = cmpElements.FrameLineGraStyleByName(cmpParameters.FrameLineType);
            bodyLineType = cmpElements.BodyLineGraStyleByName(cmpParameters.BodyLineType);
            spaceLineType = cmpElements.SpacerLineGraStyleByName(cmpParameters.SpacerLineType);

            // 全柱
            Collections.Generic.List<Revit.DB.FamilySymbol> allColumnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            foreach (Revit.DB.FamilySymbol famSym in cmpElements.GetRCColumnFamSymAry)
            {
                // パラメータ項目を持つ柱
                if (cmpService.IsHaveColumnParam(famSym) == true)
                {
                    allColumnAry.Add(famSym);
                }
            }

            // 柱
            Collections.Generic.IList<Revit.DB.FamilySymbol> columnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 間柱
            Collections.Generic.IList<Revit.DB.FamilySymbol> postAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            cmpElements.ColumnDivision(allColumnAry, column_Category_Kaku, column_Category_En, dX, dY, tyokkei, ref columnAry, ref postAry);

            // 対象柱なし
            if (columnAry.Count < 1 &&
                postAry.Count < 1)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTARGETCOLUMN"));
                transGroup.Assimilate();
                return retCmd;
            }

            Collections.Generic.List<Collections.Generic.List<Revit.DB.FamilySymbol>> aryAry = new Collections.Generic.List<Collections.Generic.List<Revit.DB.FamilySymbol>>();

            //Get setting value
            Revit.DB.ProjectInfo projInfo = cmpElements.ProjectInfo;

            List<string> settings = new List<string>();
            for (int i = 0; i < 9; i++)
            {
                settings.Add("");
            }
            var paraSeting = projInfo.LookupParameter(cmpAttribute.ResourceText("IDS_SHARE_PARA_COLUMN_RANGER"));
            if (paraSeting != null && paraSeting.AsString() != null)
            {
                var values = paraSeting.AsString().Split(',').ToList();
                if (values.Count == 9)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        settings[i] = values[i];
                    }
                }
            }

            bool isAll = true;
            bool isColumnType = columnAry.Count != 0 ? true : false;
            bool isPostType = postAry.Count != 0 ? true : false;
            bool byRange = false;
            double maxHeight = double.MinValue;
            double maxWidth = double.MinValue;
            int iRound = 6; 
            List<string> new_Setting = new List<string>();
            if (columnAry.Count == 0 && postAry.Count == 0)
            {
                transGroup.Assimilate();
                return retCmd;
            }
            else
            {
                //Show form
                FormColumnOption form = new FormColumnOption(cmpAttribute, false, settings, 0);
                form.IsEnabledColumnType = isColumnType;
                form.IsEnabledPostType = isPostType;
                if (form.ShowDialog() != DialogResult.OK)
                {
                    transGroup.Assimilate();
                    return retCmd;
                }

                isColumnType = form.IsColumnTypeChecked;
                isPostType = form.IsPostTypeChecked;
                isAll = form.IsExportAllChecked;

                if (form.ByRange && form.IsEnableByRange)
                {
                    byRange = form.ByRange;

                    maxHeight = Math.Round(form.MaxHeight / 304.8 * listViewScale, iRound);
                    maxWidth = Math.Round(form.MaxWidth / 304.8 * listViewScale, iRound);
                }

                new_Setting = form.GetStringSetting;
            }

            #region 複数ファミリに同じ符号があるか
            string inDifferentFamily = "";
            Collections.Generic.IList<Revit.DB.FamilySymbol> checkAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            if (isColumnType)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in columnAry)
                {
                    checkAry.Add(famSymbol);
                }
            }

            if (isPostType)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in postAry)
                {
                    checkAry.Add(famSymbol);
                }
            }

            // タイプ名重複確認
            Collections.Generic.IList<string> overlapTypeName = cmpElements.OverlapTypeName(checkAry);
            if (overlapTypeName.Count > 0)
            {
                foreach (string typeName in overlapTypeName)
                {
                    if (inDifferentFamily != "")
                    {
                        inDifferentFamily += ", ";
                    }

                    inDifferentFamily += typeName;
                }

                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_TYPENAME") + "「" + inDifferentFamily + "」" + cmpAttribute.ResourceText("IDS_ERR_INDIFFERENTFAMILY"));
            }

            #endregion

            int countAll = 0;
            if (isColumnType)
            {
                aryAry.Add(columnAry.ToList());
                countAll += columnAry.ToList().Count;
            }
            if (isPostType)
            {
                aryAry.Add(postAry.ToList());
                countAll += postAry.ToList().Count;
            }

            if (aryAry.Count == 0)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTARGETCOLUMN"));
                transGroup.Assimilate();
                return retCmd;
            }

            Collections.Generic.List<ColumnRangeItem> aryAry2 = new Collections.Generic.List<ColumnRangeItem>();
            List<string> lstEmptyVal = new List<string>();

            List<string> erlistCol = new List<string>();
            int countCol = 0;
            int countPost = 0;

            //Calculate length and width
            if (isAll && byRange)
            {
                foreach (Collections.Generic.IList<Revit.DB.FamilySymbol> cAry in aryAry)
                {
                    Collections.Generic.List<ColumnRangeItem> aryAry_range = new Collections.Generic.List<ColumnRangeItem>();

                    // 角柱
                    Collections.Generic.IList<Revit.DB.FamilySymbol> kakuColumnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    // 円柱
                    Collections.Generic.IList<Revit.DB.FamilySymbol> enColumnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    // 柱の分別
                    cmpService.ColumnDivision(cAry, ref kakuColumnAry, ref enColumnAry);

                    cmpElements.IsHaveSame(ref kakuColumnAry, hasiraHugo_Kaku);
                    cmpElements.IsHaveSame(ref enColumnAry, hasiraHugo_En);

                    // データテーブル化
                    SectionListRC.Entities.DtKakuColumn entDtKakuColumn = new SectionListRC.Entities.DtKakuColumn(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                    entDtKakuColumn.GetData(kakuColumnAry);

                    SectionListRC.Entities.DtEnColumn entDtEnColumn = new SectionListRC.Entities.DtEnColumn(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                    entDtEnColumn.GetData(enColumnAry);

                    Collections.Generic.IList<System.Data.DataTable> dataAry = new Collections.Generic.List<System.Data.DataTable>();

                    System.Data.DataTable kakuData = entDtKakuColumn.Data;
                    dataAry.Add(kakuData);
                    System.Data.DataTable enData = entDtEnColumn.Data;
                    dataAry.Add(enData);

                    // 全階 = 行数

                    Collections.Generic.IList<string> allColumnLevelAry = cmpService.LevelSortOrder_TopName(cmpService.GetAllColumnLevelAry(cAry));
                    Collections.Generic.IList<string> sorted = cmpService.LevelSortOrder_NameDESC(allColumnLevelAry);
                    // ソート済みに含まれない全柱を追加
                    foreach (string str in allColumnLevelAry)
                    {
                        if (!sorted.Contains(str))
                        {
                            sorted.Add(str);
                        }
                    }
                    allColumnLevelAry = sorted;

                    //HEIGHT////////////////////////////////////////////////////////////////////////
                    allColumnLevelAry = allColumnLevelAry.Reverse().ToList(); //From lower to higher

                    //////////////////////////////////////////////////////////////////////////
                    Dictionary<string, List<FamilySymbol>> data_type = new Dictionary<string, List<FamilySymbol>>();
                    Dictionary<string, List<FamilySymbol>> data_level = new Dictionary<string, List<FamilySymbol>>();
                    foreach (var symbol in cAry)
                    {
                        string typeName = "";
                        string levelName = "0";

                        cmpElements.GetTypeMarkLevel(symbol, ref typeName, ref levelName, cmpParameters.RST_HasiraHugo_Kaku);
                        if (typeName == "")
                            cmpElements.GetTypeMarkLevel(symbol, ref typeName, ref levelName, cmpParameters.RST_HasiraHugo_En);

                        //if (typeName == "")
                        //{
                        //    continue;
                        //}

                        //if (levelName == "0")
                        //{
                        //    countAll = countAll - 1;
                        //}

                        Revit.DB.Parameter parX = symbol.LookupParameter(cmpParameters.DX_Kaku);
                        Revit.DB.Parameter parY = symbol.LookupParameter(cmpParameters.DY_Kaku);
                        Revit.DB.Parameter parDiameter = symbol.LookupParameter(cmpParameters.Tyokkei_En);

                        if (parX != null && parY != null)
                        {
                            levelName = cmpElements.GetColumnTypeLevel(symbol, cmpParameters.RST_HasiraHugo_Kaku);
                        }

                        if (parDiameter != null)
                        {
                            levelName = cmpElements.GetColumnTypeLevel(symbol, cmpParameters.RST_HasiraHugo_En);
                        }


                        if (data_type.ContainsKey(typeName) == false)
                            data_type.Add(typeName, new List<FamilySymbol>());

                        if (data_type[typeName].Contains(symbol) == false)
                            data_type[typeName].Add(symbol);

                        if (data_level.ContainsKey(levelName) == false)
                            data_level.Add(levelName, new List<FamilySymbol>());

                        if (data_level[levelName].Contains(symbol) == false)
                            data_level[levelName].Add(symbol);
                    }

                    double h_Hugowaku = symbolFrameHeight / 304.8 * listViewScale;

                    double sumHeight = h_Hugowaku;

                    
                    //////////////////////////////////////////////////////////////////////////
                    List<List<string>> aryAry_range_by_level = new List<List<string>>();
                    List<string> range_level = new List<string>();

                    for (int j = 0; j < allColumnLevelAry.Count; ++j)
                    {
                        // 現在の階
                        string level = allColumnLevelAry[j];

                        // 同一階に異断面柱があるか
                        bool isDifference = false;
                        isDifference = cmpService.IsDifferenceTopBottomRebarInLevel(kakuData, level);
                        if (isDifference == false)
                        {
                            isDifference = cmpService.IsDifferenceTopBottomRebarInLevel(enData, level);
                        }

                        // 現在の階最大高さ
                        double levelMaxY = cmpService.ColumnHeightByLevel(kakuData, enData, level);

                        if (isDifference == true)
                            levelMaxY = levelMaxY * 2;

                        int haikinwakuNum = 2 + addFrameNumber;

                        // 現在階での芯鉄筋有無
                        bool isCoreRebar = cmpService.IsCoreRebarInLevel(kakuData, level);
                        if (isCoreRebar == false)
                        {
                            isCoreRebar = cmpService.IsCoreRebarInLevel(enData, level);
                        }

                        if (isCoreRebar)
                        {
                            haikinwakuNum += 1;
                        }
                        
                        // 階最大高さ

                        double otherH = (topSpace + bottomSpace + arrangementFrameHeight * haikinwakuNum) / 304.8 * listViewScale;

                        if (isDifference == true)
                            otherH = otherH * 2;

                        double d = levelMaxY + otherH;

                        sumHeight += Math.Round(d, iRound);

                        if (sumHeight <= maxHeight)
                        {
                            range_level.Add(level);
                        }
                        else
                        {
                            if (range_level.Count != 0)
                                aryAry_range_by_level.Add(range_level);
                            
                            sumHeight = d;
                            sumHeight += h_Hugowaku;
                            sumHeight = Math.Round(sumHeight, iRound);

                            range_level = new List<string>();

                            if (sumHeight <= maxHeight)
                            {
                                range_level.Add(level);
                            }
                        }
                    }

                    if (range_level.Count != 0)
                        aryAry_range_by_level.Add(range_level);

                    //END - HEIGHT////////////////////////////////////////////////////////////////////////

                    // 角柱符号
                    Collections.Generic.IList<string> columnHugoAry = cmpService.KakuCollumnMarkName(kakuData);

                    // 円柱符号
                    Collections.Generic.IList<string> enHugoAry = cmpService.EnCollumnMarkName(enData);

                    //WIDTH////////////////////////////////////////////////////////////////////////

                    double w_Kaihyojiwaku = lvlFrameWidth / 304.8 * listViewScale;
                    // 項目表示枠
                    double w_Komokuwaku = itemFrameWidth / 304.8 * listViewScale;

                    //行列数
                    int r = allColumnLevelAry.Count;
                    int c = columnHugoAry.Count + enHugoAry.Count; ;

                    // 階表示枠有無
                    bool kaihyoji = true;
                    if (lvlFrameShow == 0)
                    {
                        kaihyoji = true;
                    }
                    else
                    {
                        kaihyoji = false;
                    }

                    if (kaihyoji == false)
                    {
                        w_Kaihyojiwaku = 0;
                    }

                    double title_w = (w_Kaihyojiwaku/* * 2*/) + (w_Komokuwaku /** 2*/);

                    foreach (List<string> range_level2 in aryAry_range_by_level)
                    {
                        if (range_level2.Count == 0)
                            continue;

                        range_level2.Reverse();

                        var symbols = new List<FamilySymbol>();
                        foreach (string level in range_level2)
                        {
                            if (data_level.ContainsKey(level) == true)
                            {
                                symbols.AddRange(data_level[level]);
                            }
                        }

                        ColumnRangeItem range = new ColumnRangeItem(kakuData, enData, range_level2);

                        // 現在の合計枠幅
                        double sumWidth = title_w;

                        // 角柱
                        for (int i_w = 0; i_w < columnHugoAry.Count; i_w++)
                        {
                            double maxX = cmpService.ColumnWidthByMark(kakuData, columnHugoAry[i_w], true);

                            // 符号最大幅
                            var hugowakuMax = maxX + (leftSpace + rightSpace) / 304.8 * listViewScale;
                            sumWidth += Math.Round(hugowakuMax, iRound);

                            //////////////////////////////////////////////////////////////////////////
                            Collections.Generic.List<Revit.DB.FamilySymbol> types = data_type[columnHugoAry[i_w]];

                            if (data_type.ContainsKey(columnHugoAry[i_w]))
                                types = data_type[columnHugoAry[i_w]];
                            else
                                lstEmptyVal.Add(string.Empty);

                            types = (from FamilySymbol symbol in types
                                     where symbols.Contains(symbol)
                                     select symbol).ToList();

                            if (sumWidth <= maxWidth)
                            {
                                range._familySymbols.AddRange(types);
                            }
                            else
                            {
                                if (range._familySymbols.Count != 0)
                                    aryAry_range.Add(range);

                                sumWidth = hugowakuMax;

                                sumWidth += title_w;

                                sumWidth = Math.Round(sumWidth, iRound);

                                range = new ColumnRangeItem(kakuData, enData, range_level2);

                                if (sumWidth <= maxWidth)
                                    range._familySymbols.AddRange(types);
                            }

                            range._columnHugoAry.Add(columnHugoAry[i_w]);
                        }

                        // 円柱
                        for (int i_w = 0; i_w < enHugoAry.Count; i_w++)
                        {
                            double maxDia = cmpService.ColumnWidthByMark(enData, enHugoAry[i_w], false);

                            // 符号最大幅
                            var hugowakuMax = maxDia + (leftSpace + rightSpace) / 304.8 * listViewScale;
                            sumWidth += Math.Round(hugowakuMax, iRound);

                            //////////////////////////////////////////////////////////////////////////
                            Collections.Generic.List<Revit.DB.FamilySymbol> types = new List<FamilySymbol>();
                            if (data_type.ContainsKey(enHugoAry[i_w]))
                                types = data_type[enHugoAry[i_w]];
                            else
                                lstEmptyVal.Add(string.Empty);

                            types = (from FamilySymbol symbol in types
                                     where symbols.Contains(symbol)
                                     select symbol).ToList();

                            if (sumWidth <= maxWidth)
                            {
                                range._familySymbols.AddRange(types);
                            }
                            else
                            {
                                if (range._familySymbols.Count != 0)
                                    aryAry_range.Add(range);

                                sumWidth = hugowakuMax;

                                sumWidth += title_w;

                                sumWidth = Math.Round(sumWidth, iRound);

                                range = new ColumnRangeItem(kakuData, enData, range_level2);

                                if (sumWidth <= maxWidth)
                                {
                                    range._familySymbols.AddRange(types);
                                }
                            }

                            range._enHugoAry.Add(enHugoAry[i_w]);
                        }
                        if (range._familySymbols.Count != 0)
                        {
                            aryAry_range.Add(range);
                        }
                    }

                    //END WIDTH////////////////////////////////////////////////////////////////////////

                    aryAry2.AddRange(aryAry_range);
                }

                //Check count
                int added_family = 0;
                foreach (ColumnRangeItem item in aryAry2)
                {
                    var cAry = item._familySymbols;
                    added_family += cAry.Count;
                }

                //foreach (var item in lstEmptyVal)
                //    added_family += 1;


                //if (countAll > added_family)
                //{

                    foreach (ColumnRangeItem item in aryAry2)
                    {
                        foreach(FamilySymbol symbol in item._familySymbols)
                        {
                            if(columnAry.ToList().Contains(symbol))
                            {
                                countCol++;
                            }
                            if(postAry.ToList().Contains(symbol))
                            {
                                countPost++;
                            }
                        }
                    }

                    if(isColumnType && countCol < columnAry.Count)
                    {
                        erlistCol.Add(cmpAttribute.ResourceText("IDS_TXT_COLUMNLIST"));
                    }
                    if (isPostType && countPost < postAry.Count)
                    {
                        erlistCol.Add(cmpAttribute.ResourceText("IDS_TXT_POSTLIST"));
                    }
                    if(erlistCol.Count != 0)
                    {
                        string mess = string.Join("\n", erlistCol.ToArray());
                        System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERROR_SMALLSIZE") + " \n" + mess);
                        //transGroup.Assimilate();
                        ////return retCmd;

                    }
                //}
            }
            else
            {
                foreach (Collections.Generic.IList<Revit.DB.FamilySymbol> cAry in aryAry)
                {
                    if (cAry.Count < 1)
                    {
                        continue;
                    }

                    // 角柱
                    Collections.Generic.IList<Revit.DB.FamilySymbol> kakuColumnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    // 円柱
                    Collections.Generic.IList<Revit.DB.FamilySymbol> enColumnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    // 柱の分別
                    cmpService.ColumnDivision(cAry, ref kakuColumnAry, ref enColumnAry);

                    cmpElements.IsHaveSame(ref kakuColumnAry, hasiraHugo_Kaku);
                    cmpElements.IsHaveSame(ref enColumnAry, hasiraHugo_En);

                    // データテーブル化
                    SectionListRC.Entities.DtKakuColumn entDtKakuColumn = new SectionListRC.Entities.DtKakuColumn(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                    entDtKakuColumn.GetData(kakuColumnAry);

                    SectionListRC.Entities.DtEnColumn entDtEnColumn = new SectionListRC.Entities.DtEnColumn(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                    entDtEnColumn.GetData(enColumnAry);

                    Collections.Generic.IList<System.Data.DataTable> dataAry = new Collections.Generic.List<System.Data.DataTable>();

                    System.Data.DataTable kakuData = entDtKakuColumn.Data;
                    dataAry.Add(kakuData);
                    System.Data.DataTable enData = entDtEnColumn.Data;
                    dataAry.Add(enData);

                    // 角柱符号
                    Collections.Generic.IList<string> columnHugoAry = cmpService.KakuCollumnMarkName(kakuData);

                    // 円柱符号
                    Collections.Generic.IList<string> enHugoAry = cmpService.EnCollumnMarkName(enData);

                    Collections.Generic.IList<string> allColumnLevelAry = cmpService.LevelSortOrder_TopName(cmpService.GetAllColumnLevelAry(cAry));
                    Collections.Generic.IList<string> sorted = cmpService.LevelSortOrder_NameDESC(allColumnLevelAry);
                    // ソート済みに含まれない全柱を追加
                    foreach (string str in allColumnLevelAry)
                    {
                        if (!sorted.Contains(str))
                        {
                            sorted.Add(str);
                        }
                    }
                    allColumnLevelAry = sorted;

                    ColumnRangeItem range = new ColumnRangeItem(kakuData, enData, allColumnLevelAry.ToList());
                    range._familySymbols = cAry.ToList();
                    aryAry2.Add(range);

                    range._columnHugoAry = columnHugoAry.ToList();
                    range._enHugoAry = enHugoAry.ToList();
                }
            }

            //////////////////////////////////////////////////////////////////////////

            // 作図の原点
            Revit.DB.XYZ kaiHidariUe = new Revit.DB.XYZ();

            bool isCreateMultipleView = aryAry2.Count > 1 ? true : false;

            // エラーメッセージ
            string writeErr = "";

            var activeView = rvtUiDoc.ActiveView;
            Dictionary<int, List<ViewPlan>> dic_Views = new Dictionary<int, List<ViewPlan>>();
            foreach (ColumnRangeItem item in aryAry2)
            {
                var cAry = item._familySymbols;

                ///
                if(erlistCol.Count != 0)
                {
                    if (columnAry.ToList().Contains(cAry.ToList()[0]) == true)
                    {
                        if (countCol < columnAry.Count)
                            continue;
                    }
                    if (postAry.ToList().Contains(cAry.ToList()[0]) == true)
                    {
                        if (countPost < postAry.Count)
                            continue;
                    }
                }
               
                ///

                // 現在の合計枠幅
                double sumWidth = 0;

                if (cAry.Count < 1)
                {
                    continue;
                }

                // ビューを作成
                int mode = 0;
                if (columnAry.ToList().Contains(cAry.ToList()[0]) == true)
                    mode = 0;
                else
                    mode = 1;

                // 角柱
                Collections.Generic.IList<Revit.DB.FamilySymbol> kakuColumnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                // 円柱
                Collections.Generic.IList<Revit.DB.FamilySymbol> enColumnAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                // 柱の分別
                cmpService.ColumnDivision(cAry, ref kakuColumnAry, ref enColumnAry);

                cmpElements.IsHaveSame(ref kakuColumnAry, hasiraHugo_Kaku);
                cmpElements.IsHaveSame(ref enColumnAry, hasiraHugo_En);

                // データテーブル化
                SectionListRC.Entities.DtKakuColumn entDtKakuColumn = new SectionListRC.Entities.DtKakuColumn(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                entDtKakuColumn.GetData(kakuColumnAry);

                SectionListRC.Entities.DtEnColumn entDtEnColumn = new SectionListRC.Entities.DtEnColumn(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                entDtEnColumn.GetData(enColumnAry);

                Collections.Generic.IList<System.Data.DataTable> dataAry = new Collections.Generic.List<System.Data.DataTable>();

                System.Data.DataTable kakuData = item._kakuData;
                dataAry.Add(kakuData);
                System.Data.DataTable enData = item._enData;
                dataAry.Add(enData);

                // 全階 = 行数
                Collections.Generic.IList<string> allColumnLevelAry = item._levels;

                // 角柱符号
                Collections.Generic.IList<string> columnHugoAry = item._columnHugoAry;

                // 円柱符号
                Collections.Generic.IList<string> enHugoAry = item._enHugoAry;

                // 階別最大柱高さ
                Collections.Generic.IList<double> columnHeightAry = cmpService.ColumnHeightByLevel(kakuData);
                // 符号別最大柱幅
                Collections.Generic.IList<double> columnWidthAry = cmpService.ColumnWidthByMark(kakuData);
                // 階別最大円柱径
                Collections.Generic.IList<double> columnDiaLevelAry = cmpService.ColumnDiameterByLevel(enData);
                // 符号別最大円柱径
                Collections.Generic.IList<double> columnDiaMarkAry = cmpService.ColumnDiameterByMark(enData);

                //////////////////////////////////////////////////////////////////////////
                if (isAll == false)
                {
                    Collections.Generic.IList<string> sorted = cmpService.LevelSortOrder_NameDESC(allColumnLevelAry);
                    // ソート済みに含まれない全柱を追加
                    foreach (string str in allColumnLevelAry)
                    {
                        if (!sorted.Contains(str))
                        {
                            sorted.Add(str);
                        }
                    }
                    allColumnLevelAry = sorted;
                    //Show form
                    FormColumnItemList formList = new FormColumnItemList(cmpAttribute, cmpParameters, settings, mode == 0 ? EnumType.Column : EnumType.Post, columnHugoAry.ToList(),
                        enHugoAry.ToList(), kakuData, enData, allColumnLevelAry.ToList(),0);

                    if (formList.ShowDialog() != DialogResult.OK)
                    {
                        if (trans.HasStarted() == false)
                            trans.Start("Save setting");

                        //Write setting
                        if (paraSeting != null)
                        {
                            var new_Settings = string.Join(",", new_Setting.ToArray());
                            paraSeting.Set(new_Settings);
                        }

                        trans.Commit();

                        retCmd = Revit.UI.Result.Succeeded;
                        transGroup.Assimilate();
                        return retCmd;
                    }

                    if (formList._SelectedColumns.Count == 0 || formList._SelectedLevels.Count == 0)
                    {
                        transGroup.Assimilate();
                        return retCmd;
                    }

                    columnHugoAry = (from string column in columnHugoAry
                                     where formList._SelectedColumns.Contains(column) == true
                                     select column).ToList();

                    enHugoAry = (from string column in enHugoAry
                                 where formList._SelectedColumns.Contains(column) == true
                                 select column).ToList();

                    allColumnLevelAry = (from string level in allColumnLevelAry
                                         where formList._SelectedLevels.Contains(level) == true
                                         select level).ToList();

                    if (new_Setting.Count == 9 && formList.GetSettingValue.Count == 4)
                    {
                        new_Setting[new_Setting.Count - 1] = formList.GetSettingValue[formList.GetSettingValue.Count - 1];
                        new_Setting[new_Setting.Count - 2] = formList.GetSettingValue[formList.GetSettingValue.Count - 2];
                        new_Setting[new_Setting.Count - 3] = formList.GetSettingValue[formList.GetSettingValue.Count - 3];
                        new_Setting[new_Setting.Count - 4] = formList.GetSettingValue[formList.GetSettingValue.Count - 4];
                    }
                    else
                        new_Setting.AddRange(formList.GetSettingValue);
                }

                //////////////////////////////////////////////////////////////////////////

                cmpElements.SetCreateListView(listViewScale, mode);

                Revit.DB.ViewPlan vp = rvtDbDoc.ActiveView as Revit.DB.ViewPlan;

                int aryNum = mode == 0 ? (int)EnumType.Column : (int)EnumType.Post;
                if (dic_Views.ContainsKey(aryNum) == false)
                    dic_Views.Add(aryNum, new List<ViewPlan>());

                dic_Views[aryNum].Add(vp);

                // 幅
                Collections.Generic.IList<double> width_Diameters = new Collections.Generic.List<double>();
                foreach (double d in columnWidthAry)
                {
                    width_Diameters.Add(d);
                }
                foreach (double d in columnDiaMarkAry)
                {
                    width_Diameters.Add(d);
                }

                // 現在のビューの尺度
                int scale = vp.Scale;

                // 枠の大きさは印刷後の実寸なので、入力値にビューの尺度をかける。
                // 入力値(mm) ÷ 304.8(フィート化) × 尺度

                // 枠高さ
                // -Y方向に作図していくので負
                // 符号表示枠
                double h_Hugowaku = -symbolFrameHeight / 304.8 * scale;

                // 枠幅
                // X方向に作図していくので正
                // 階表示枠
                double w_Kaihyojiwaku = lvlFrameWidth / 304.8 * scale;
                // 項目表示枠
                double w_Komokuwaku = itemFrameWidth / 304.8 * scale;

                // 符号ごとの最大幅
                double w_HugowakuMax = 0;

                //行列数
                int r = allColumnLevelAry.Count;
                int c = columnHugoAry.Count + enHugoAry.Count; ;

                // 階表示枠有無
                bool kaihyoji = true;
                if (lvlFrameShow == 0)
                {
                    kaihyoji = true;
                }
                else
                {
                    kaihyoji = false;
                }

                if (kaihyoji == false)
                {
                    w_Kaihyojiwaku = 0;
                }

                sumWidth += w_Kaihyojiwaku * 2;
                sumWidth += w_Komokuwaku * 2;

                // 枠、躯体別線分
                Revit.DB.CurveArray crvAryFrame = new Revit.DB.CurveArray();
                Revit.DB.CurveArray crvAryStrct = new Revit.DB.CurveArray();

                // 寄せ筋記号位置
                Collections.Generic.IList<Revit.DB.XYZ> rebar2ndUe = new Collections.Generic.List<Revit.DB.XYZ>();
                Collections.Generic.IList<Revit.DB.XYZ> rebar2ndSita = new Collections.Generic.List<Revit.DB.XYZ>();
                Collections.Generic.IList<Revit.DB.XYZ> rebar2ndMigi = new Collections.Generic.List<Revit.DB.XYZ>();
                Collections.Generic.IList<Revit.DB.XYZ> rebar2ndHidari = new Collections.Generic.List<Revit.DB.XYZ>();

                //
                Revit.DB.XYZ koumokuHidariUe = new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y, kaiHidariUe.Z);
                Revit.DB.XYZ hugouHidariUe = new Revit.DB.XYZ(koumokuHidariUe.X + w_Komokuwaku, kaiHidariUe.Y, kaiHidariUe.Z);

                Collections.Generic.IList<Revit.DB.XYZ> pntsX = new Collections.Generic.List<Revit.DB.XYZ>();
                pntsX.Add(kaiHidariUe);

                Revit.DB.Line l = null;

                // ----- 左上の枠の交差部分 -----
                #region
                l = cmpElements.CreateBoundLine(kaiHidariUe, hugouHidariUe);
                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                l = cmpElements.CreateBoundLine(hugouHidariUe, new Revit.DB.XYZ(hugouHidariUe.X, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(hugouHidariUe.X, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z), new Revit.DB.XYZ(kaiHidariUe.X, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(kaiHidariUe.X, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z), kaiHidariUe);
                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                //        trans.Start("Text");

                if (kaihyoji == true)
                {
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y, kaiHidariUe.Z),
                                                    new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                    Revit.DB.XYZ origin = cmpGeometry.Center2Point(kaiHidariUe, new Revit.DB.XYZ(koumokuHidariUe.X, koumokuHidariUe.Y + h_Hugowaku, koumokuHidariUe.Z));// new Revit.DB.XYZ(kaiHidariUe.X + w1 / 2, kaiHidariUe.Y + h1 / 2, kaiHidariUe.Z);
                                                                                                                                                                        // 現在ビューの各方向
                    Revit.DB.XYZ baseVec = vp.RightDirection;
                    Revit.DB.XYZ upVec = vp.UpDirection;

                    // 引数 lineWidthの値が分からない
                    double lineWidth = w_Kaihyojiwaku / scale;

                    string str = lvlFrameTitle;

                    Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(vp,
                                                                             origin,
                                                                             baseVec,
                                                                             lineWidth,
                                                                             titleTNT.Id,
                                                                             str,
                                                                             rvtDbDoc);

                    trans.Start("Regenerate");
                    rvtDbDoc.Regenerate();
                    trans.Commit();

                    origin = cmpGeometry.Center2Point(koumokuHidariUe, new Revit.DB.XYZ(hugouHidariUe.X, hugouHidariUe.Y + h_Hugowaku, hugouHidariUe.Z));// new Revit.DB.XYZ(kaiHidariUe.X + w1 + w2 / 2, kaiHidariUe.Y + h1 / 2, kaiHidariUe.Z);
                    lineWidth = w_Komokuwaku / scale;
                    str = symbolFrameTitle;

                    txtNote = cmpService.CreateNewTextNote(vp,
                                                           origin,
                                                           baseVec,
                                                           lineWidth,
                                                           titleTNT.Id,
                                                           str,
                                                           rvtDbDoc);

                    trans.Start("Regenerate");
                    rvtDbDoc.Regenerate();
                    trans.Commit();
                }
                else
                {
                    // 項目表示枠に斜線
                    // 階タイトルと項目タイトル

                    l = cmpElements.CreateBoundLine(kaiHidariUe, new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                    // 三角形の中心に配置
                    Revit.DB.XYZ origin = cmpGeometry.TriangleGravity2D(kaiHidariUe, new Revit.DB.XYZ(kaiHidariUe.X, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z), new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z)); //new Revit.DB.XYZ(p0.X, p0.Y + h1, p0.Z);
                                                                                                                                                                                                                                                             // 現在ビューの各方向
                    Revit.DB.XYZ baseVec = vp.RightDirection;
                    Revit.DB.XYZ upVec = vp.UpDirection;

                    double lineWidth = w_Komokuwaku / scale;

                    string str = lvlFrameTitle;

                    Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(vp,
                                                                             origin,
                                                                             baseVec,
                                                                             lineWidth,
                                                                             titleTNT.Id,
                                                                             str,
                                                                             rvtDbDoc);

                    trans.Start("Regenerate");
                    rvtDbDoc.Regenerate();
                    trans.Commit();

                    origin = cmpGeometry.TriangleGravity2D(kaiHidariUe, new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y, kaiHidariUe.Z), new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));  //new Revit.DB.XYZ(p0.X + w2, p0.Y, p0.Z);
                    lineWidth = w_Komokuwaku / scale;
                    str = symbolFrameTitle;

                    txtNote = cmpService.CreateNewTextNote(vp,
                                                           origin,
                                                           baseVec,
                                                           lineWidth,
                                                           titleTNT.Id,
                                                           str,
                                                           rvtDbDoc);

                    trans.Start("Regenerate");
                    rvtDbDoc.Regenerate();
                    trans.Commit();
                }

                Revit.DB.XYZ px = new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku + w_Komokuwaku, kaiHidariUe.Y, kaiHidariUe.Z);
                pntsX.Add(px);
                #endregion

                // ----- 符号表示枠 -----
                #region
                double wx = 0;

                // 角柱
                for (int i_w = 0; i_w < columnHugoAry.Count; i_w++)
                {
                    double maxX = cmpService.ColumnWidthByMark(kakuData, columnHugoAry[i_w], true);

                    // 符号最大幅
                    w_HugowakuMax = maxX + (leftSpace + rightSpace) / 304.8 * scale;
                    sumWidth += w_HugowakuMax;

                    // 直前の点 + 符号最大幅
                    wx = pntsX[pntsX.Count - 1].X + w_HugowakuMax;

                    l = cmpElements.CreateBoundLine(px, new Revit.DB.XYZ(wx, px.Y, px.Z));
                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(wx, px.Y, px.Z), new Revit.DB.XYZ(wx, px.Y + h_Hugowaku, px.Z));
                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(wx, px.Y + h_Hugowaku, px.Z), new Revit.DB.XYZ(px.X, px.Y + h_Hugowaku, px.Z));
                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                    // 文字
                    string title = columnHugoAry[i_w];

                    // 複数ファミリに同一タイプ名がある場合
                    string overlapnames = "";
                    foreach (string lvlName in allColumnLevelAry)
                    {
                        string typeName = lvlName + title;
                        if (overlapTypeName.Contains(typeName))
                        {
                            if (overlapnames != "")
                            {
                                overlapnames += ", ";
                            }

                            overlapnames += typeName;
                        }
                    }

                    if (overlapnames != "")
                    {
                        title += "\r\n" + cmpAttribute.ResourceText("IDS_TXT_HUGO") + "「" + overlapnames + "」" + cmpAttribute.ResourceText("IDS_ERR_INDIFFERENTFAMILY");
                    }

                    Revit.DB.XYZ origin = cmpGeometry.Center2Point(px, new Revit.DB.XYZ(wx, px.Y + h_Hugowaku, px.Z));
                    Revit.DB.XYZ baseVec = vp.RightDirection;
                    Revit.DB.XYZ upVec = vp.UpDirection;

                    double lineWidth = w_HugowakuMax / scale;

                    Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(vp,
                                                                             origin,
                                                                             baseVec,
                                                                             lineWidth,
                                                                             titleTNT.Id,
                                                                             title,
                                                                             rvtDbDoc);

                    px = new Revit.DB.XYZ(wx, px.Y, px.Z);
                    pntsX.Add(px);
                }

                // 円柱
                for (int i_w = 0; i_w < enHugoAry.Count; i_w++)
                {
                    double maxDia = cmpService.ColumnWidthByMark(enData, enHugoAry[i_w], false);

                    // 符号最大幅
                    w_HugowakuMax = maxDia + (leftSpace + rightSpace) / 304.8 * scale;
                    sumWidth += w_HugowakuMax;
                    // 直前の点 + 符号最大幅
                    wx = pntsX[pntsX.Count - 1].X + w_HugowakuMax;

                    l = cmpElements.CreateBoundLine(px, new Revit.DB.XYZ(wx, px.Y, px.Z));
                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(wx, px.Y, px.Z), new Revit.DB.XYZ(wx, px.Y + h_Hugowaku, px.Z));
                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(wx, px.Y + h_Hugowaku, px.Z), new Revit.DB.XYZ(px.X, px.Y + h_Hugowaku, px.Z));
                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                    // 文字
                    string title = enHugoAry[i_w];

                    // 複数ファミリに同一タイプ名がある場合
                    string overlapnames = "";
                    foreach (string lvlName in allColumnLevelAry)
                    {
                        string typeName = lvlName + title;
                        if (overlapTypeName.Contains(typeName))
                        {
                            if (overlapnames != "")
                            {
                                overlapnames += ", ";
                            }

                            overlapnames += typeName;
                        }
                    }

                    if (overlapnames != "")
                    {
                        title += "\r\n" + cmpAttribute.ResourceText("IDS_TXT_TYPENAME") + "「" + overlapnames + "」" + cmpAttribute.ResourceText("IDS_ERR_INDIFFERENTFAMILY");
                    }

                    Revit.DB.XYZ origin = cmpGeometry.Center2Point(px, new Revit.DB.XYZ(wx, px.Y + h_Hugowaku, px.Z));
                    Revit.DB.XYZ baseVec = vp.RightDirection;
                    Revit.DB.XYZ upVec = vp.UpDirection;

                    double lineWidth = w_HugowakuMax / scale;

                    Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                    px = new Revit.DB.XYZ(wx, px.Y, px.Z);
                    pntsX.Add(px);
                }
                #endregion

                // ----- 階表示枠、項目表示枠 -----
                #region
                double hy = h_Hugowaku;

                // 階表示枠左上
                Revit.DB.XYZ p1 = pntsX[0];
                // 項目表示枠右上
                Revit.DB.XYZ p2 = pntsX[1];

                Collections.Generic.IList<Revit.DB.XYZ> pntsY = new Collections.Generic.List<Revit.DB.XYZ>();
                pntsY.Add(p1);
                pntsY.Add(new Revit.DB.XYZ(p1.X, p1.Y + h_Hugowaku, p1.Z));

                double h = 0;

                for (int i_h = 0; i_h < r; i_h++)
                {
                    // 現在の階名
                    string levelName = allColumnLevelAry[i_h];

                    // 現在階での芯鉄筋有無
                    bool isCoreRebar = cmpService.IsCoreRebarInLevel(kakuData, levelName);
                    if (isCoreRebar == false)
                    {
                        isCoreRebar = cmpService.IsCoreRebarInLevel(enData, levelName);
                    }

                    int haikinwakuNum = 2 + addFrameNumber;
                    if (isCoreRebar)
                    {
                        haikinwakuNum += 1;
                    }

                    // 階最大高さ
                    double maxH = cmpService.ColumnHeightByLevel(kakuData, enData, levelName);

                    // 同一階に異断面柱があるか
                    bool isDifference = false;
                    isDifference = cmpService.IsDifferenceTopBottomRebarInLevel(kakuData, levelName);
                    if (isDifference == false)
                    {
                        isDifference = cmpService.IsDifferenceTopBottomRebarInLevel(enData, levelName);
                    }

                    double otherH = (topSpace + bottomSpace + arrangementFrameHeight * haikinwakuNum) / 304.8 * scale;

                    #region 異断面柱なし
                    if (isDifference == false)
                    {
                        h = -(maxH + otherH);

                        l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p1.X, p1.Y + hy + h, p1.Z));
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                        l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X, p1.Y + hy + h, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy + h, p2.Z));
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                        l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p2.X, p2.Y + hy + h, p2.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy, p2.Z));
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        Revit.DB.XYZ origin = null;
                        string title = "";
                        Revit.DB.XYZ baseVec = null;
                        Revit.DB.XYZ upVec = null;
                        double lineWidth = 0;
                        Revit.DB.TextNote txtNote = null;

                        if (kaihyoji == true)
                        {
                            l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy + h, p1.Z));
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy + h, p1.Z));

                            // 階表示文字
                            title = allColumnLevelAry[i_h] + lvlFrameEndWord;
                            baseVec = vp.RightDirection;
                            upVec = vp.UpDirection;

                            lineWidth = w_Kaihyojiwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                            origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));

                            // 断面
                            title = "断面";
                            baseVec = vp.RightDirection;
                            upVec = vp.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);
                        }
                        else
                        {
                            origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));

                            // 階表示文字
                            title = allColumnLevelAry[i_h] + lvlFrameEndWord;
                            baseVec = vp.RightDirection;
                            upVec = vp.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);
                        }

                        // ----- 主筋 -----
                        Revit.DB.XYZ point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                        Revit.DB.XYZ point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight) / 304.8 * scale), p1.Z));
                        title = "主筋";
                        baseVec = vp.RightDirection;
                        upVec = vp.UpDirection;

                        lineWidth = w_Komokuwaku / scale;

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        int wakuNum = 1;

                        // ----- 芯鉄筋 -----
                        if (isCoreRebar)
                        {
                            point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + 1)) / 304.8 * scale), p1.Z));
                            title = coreRebarTitle;
                            baseVec = vp.RightDirection;
                            upVec = vp.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                            wakuNum += 1;
                        }

                        // ----- 帯筋 -----
                        point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                        point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + 1)) / 304.8 * scale), p1.Z));
                        title = hoopFrameTitle;
                        baseVec = vp.RightDirection;
                        upVec = vp.UpDirection;

                        lineWidth = w_Komokuwaku / scale;

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        wakuNum += 1;

                        // ----- 追加枠 -----
                        if (addFrameNumber > 0)
                        {
                            point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            for (int addFrameNum = 1; addFrameNum < addFrameNumber; ++addFrameNum)
                            {
                                point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + addFrameNum)) / 304.8 * scale), p1.Z);
                                point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + addFrameNum)) / 304.8 * scale), p1.Z);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            }
                        }

                        hy += h;

                        pntsY.Add(new Revit.DB.XYZ(p1.X, p1.Y + hy, p1.Z));
                    }
                    #endregion

                    #region 異断面柱あり
                    if (isDifference == true)
                    {
                        h = -(maxH + (topSpace + bottomSpace + arrangementFrameHeight * haikinwakuNum) / 304.8 * scale) * 2;

                        l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p1.X, p1.Y + hy + h, p1.Z));
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                        l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X, p1.Y + hy + h, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy + h, p2.Z));
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                        l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p2.X, p2.Y + hy + h, p2.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy, p2.Z));
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        Revit.DB.XYZ origin = null;
                        string title = "";
                        Revit.DB.XYZ baseVec = null;
                        Revit.DB.XYZ upVec = null;
                        double lineWidth = 0;
                        Revit.DB.TextNote txtNote = null;

                        l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy + h / 2, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy + h / 2, p2.Z));
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        if (kaihyoji == true)
                        {
                            // 階表示枠と項目表示枠の間
                            l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy + h, p1.Z));
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy + h, p1.Z));

                            // 階表示文字
                            title = allColumnLevelAry[i_h] + lvlFrameEndWord;
                            baseVec = vp.RightDirection;
                            upVec = vp.UpDirection;

                            lineWidth = w_Kaihyojiwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                            origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));

                            // 断面
                            title = "断面" + "\r\n\r\n" + "柱頭";
                            baseVec = vp.RightDirection;
                            upVec = vp.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                            origin += new Revit.DB.XYZ(0, h / 2, 0);

                            title = "断面" + "\r\n\r\n" + "柱脚";

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);
                        }
                        else
                        {
                            origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));

                            // 断面
                            title = allColumnLevelAry[i_h] + lvlFrameEndWord + "\r\n\r\n" + "柱頭";
                            baseVec = vp.RightDirection;
                            upVec = vp.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                            origin += new Revit.DB.XYZ(0, h / 2, 0);
                            title = allColumnLevelAry[i_h] + lvlFrameEndWord + "\r\n\r\n" + "柱脚";

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);
                        }

                        // ----- 主筋 -----
                        Revit.DB.XYZ point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                        Revit.DB.XYZ point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight) / 304.8 * scale), p1.Z));
                        title = "主筋";
                        baseVec = vp.RightDirection;
                        upVec = vp.UpDirection;

                        lineWidth = w_Komokuwaku / scale;

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        int wakuNum = 1;

                        point1 += new Revit.DB.XYZ(0, h / 2, 0);
                        point2 += new Revit.DB.XYZ(0, h / 2, 0);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        origin += new Revit.DB.XYZ(0, h / 2, 0);

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        // ----- 芯鉄筋 -----
                        if (isCoreRebar)
                        {
                            point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + 1)) / 304.8 * scale), p1.Z));
                            title = coreRebarTitle;
                            baseVec = vp.RightDirection;
                            upVec = vp.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                            wakuNum += 1;

                            point1 += new Revit.DB.XYZ(0, h / 2, 0);
                            point2 += new Revit.DB.XYZ(0, h / 2, 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin += new Revit.DB.XYZ(0, h / 2, 0);

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);
                        }

                        // ----- 帯筋 -----
                        point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                        point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + 1)) / 304.8 * scale), p1.Z));
                        title = hoopFrameTitle;
                        baseVec = vp.RightDirection;
                        upVec = vp.UpDirection;

                        lineWidth = w_Komokuwaku / scale;

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        wakuNum += 1;

                        point1 += new Revit.DB.XYZ(0, h / 2, 0);
                        point2 += new Revit.DB.XYZ(0, h / 2, 0);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        origin += new Revit.DB.XYZ(0, h / 2, 0);

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        // ----- 追加枠 -----
                        if (addFrameNumber > 0)
                        {
                            point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            point1 += new Revit.DB.XYZ(0, h / 2, 0);
                            point2 += new Revit.DB.XYZ(0, h / 2, 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            for (int addFrameNum = 1; addFrameNum < addFrameNumber; ++addFrameNum)
                            {
                                point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + addFrameNum)) / 304.8 * scale), p1.Z);
                                point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + addFrameNum)) / 304.8 * scale), p1.Z);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                point1 += new Revit.DB.XYZ(0, h / 2, 0);
                                point2 += new Revit.DB.XYZ(0, h / 2, 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            }
                        }

                        hy += h;

                        pntsY.Add(new Revit.DB.XYZ(p1.X, p1.Y + hy, p1.Z));
                    }
                    #endregion
                }

                //        trans.Commit();

                #endregion

                // 基点
                Revit.DB.XYZ hidariUe = new Revit.DB.XYZ(hugouHidariUe.X, hugouHidariUe.Y + h_Hugowaku, hugouHidariUe.Z);
                Revit.DB.XYZ hidariSita = hidariUe;
                Revit.DB.XYZ migiUe = hidariUe;
                Revit.DB.XYZ migiSita = hidariUe;

                // 柱以外の幅
                double otherWidth = (leftSpace + rightSpace) / 304.8 * scale;
                double otherHeight = (topSpace + bottomSpace + arrangementFrameHeight * (2 + addFrameNumber)) / 304.8 * scale;
                               
                #region 角柱
                for (int i = 0; i < columnHugoAry.Count; ++i)
                {
                    hidariSita = hidariUe;
                    migiUe = hidariUe;
                    migiSita = hidariUe;

                    // 現在の符号
                    string hugoName = columnHugoAry[i];
                    // 現在の符号最大幅
                    double hugoMaxX = cmpService.ColumnWidthByMark(kakuData, hugoName, true);

                    // 符号最大幅
                    w_HugowakuMax = hugoMaxX + (leftSpace + rightSpace) / 304.8 * scale;

                    for (int j = 0; j < allColumnLevelAry.Count; ++j)
                    {
                        // 現在の階
                        string level = allColumnLevelAry[j];
                        // 現在の階最大高さ
                        double levelMaxY = cmpService.ColumnHeightByLevel(kakuData, enData, level);

                        // 現在階での芯鉄筋の有無
                        bool isCoreRebar = cmpService.IsCoreRebarInLevel(kakuData, level);
                        if (isCoreRebar == false)
                        {
                            isCoreRebar = cmpService.IsCoreRebarInLevel(enData, level);
                        }

                        int haikinwakuNum = 2 + addFrameNumber;
                        if (isCoreRebar)
                        {
                            haikinwakuNum += 1;
                        }

                        // 同一階での異断面柱の有無
                        bool isDifference = false;
                        isDifference = cmpService.IsDifferenceTopBottomRebarInLevel(kakuData, level);
                        if (isDifference == false)
                        {
                            isDifference = cmpService.IsDifferenceTopBottomRebarInLevel(enData, level);
                        }

                        double otherH = (topSpace + bottomSpace + arrangementFrameHeight * haikinwakuNum) / 304.8 * scale;

                        //データテーブル上の番号
                        int currentNum = 0;
                        // 符号と階の組み合わせに該当するか
                        bool gaitou = false;
                        for (int k = 0; k < kakuData.Rows.Count; ++k)
                        {
                            string currenthugo = (string)kakuData.Rows[k][cmpParameters.RST_HasiraHugo_Kaku];
                            string currentlevel = (string)kakuData.Rows[k][cmpParameters.LevelFrameTitle];

                            if (currenthugo == hugoName && currentlevel == level)
                            {
                                currentNum = k;
                                gaitou = true;
                                break;
                            }
                        }
                        
                        bool X1HosoLessX1Huto = true;
                        bool Y1HosoLessY1Huto = true;                       

                        int syukinX1HutokeiHonsu = 0;
                        int syukinY1HutokeiHonsu = 0;
                        int syukinX1HosokeiHonsu = 0;
                        int syukinY1HosokeiHonsu = 0;
                                                                   

                        #region 異断面柱なし
                        if (isDifference == false)
                        {
                            hidariSita = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0);
                            migiUe = hidariUe + new Revit.DB.XYZ(hugoMaxX + otherWidth, 0, 0);
                            migiSita = new Revit.DB.XYZ(migiUe.X, hidariSita.Y, migiSita.Z);

                            // 断面枠と配筋枠を作成
                            l = cmpElements.CreateBoundLine(hidariSita, migiSita);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            l = cmpElements.CreateBoundLine(migiUe, migiSita);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            // 主筋枠
                            Revit.DB.XYZ point1 = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                            Revit.DB.XYZ point2 = migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint1 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            // 芯鉄筋枠
                            Revit.DB.XYZ centerPoint_CoreRebar = null;
                            if (isCoreRebar)
                            {
                                point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                centerPoint_CoreRebar = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);
                            }

                            // 帯筋枠
                            point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint2 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            if (addFrameNumber > 0)
                            {
                                point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                for (int addFrameNum = 1; addFrameNum < addFrameNumber; ++addFrameNum)
                                {
                                    point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    l = cmpElements.CreateBoundLine(point1, point2);
                                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                }
                            }

                            if (gaitou == false)
                            {
                                //斜線を作成
                                l = cmpElements.CreateBoundLine(migiUe, hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                hidariUe = hidariSita;
                                continue;
                            }

                            // 主筋本数、径、(芯鉄筋本数、径)、帯筋本数、径
                            #region

                            //              trans.Start("Rebar Number");


                            // X

                            syukinX1HosokeiHonsu = (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                            syukinX1HutokeiHonsu = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu];
                            syukinY1HosokeiHonsu = (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu];
                            syukinY1HutokeiHonsu = (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu];

                            if (syukinX1HosokeiHonsu >= 1)
                            {
                                if (syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu)
                                {
                                    X1HosoLessX1Huto = false;
                                }
                            }
                            // Y
                            if (syukinY1HosokeiHonsu >= 1)
                            {
                                if (syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu)
                                {
                                    Y1HosoLessY1Huto = false;
                                }
                            }

                            // 主筋太径
                            // 四隅は重複している
                            int rebarCount = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] * 2 + (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] * 2 - 4;
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                            {
                                // 2段筋の端の2本は1段筋でカウントされている
                                rebarCount += ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] - 2) * 2;
                            }
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                            {
                                rebarCount += ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] - 2) * 2;
                            }
                            if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                            {
                            }
                            else
                            {
                                secondConrnerSetFlag = 0;
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)
                                {
                                    rebarCount -= 4;
                                }
                            }
                            // 2段筋コーナー
                            if (secondConrnerSetFlag == 1 &&
                                (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4 &&
                                (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)// &&
                                                                                               //(int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] - (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 2 &&
                                                                                               //(int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] - (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 2 &&
                                                                                               //((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu]) - ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu]) >= 2 &&
                                                                                               //((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu]) - ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu]) >= 2)
                            {
                                rebarCount -= 4;
                            }

                            // 主筋細径
                            // 四隅にはこない
                            int hosoCount = (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] * 2 + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] * 2;
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] > 0)
                            {
                                // 2段筋の端にはこない
                                hosoCount += (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] * 2;
                            }
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] > 0)
                            {
                                hosoCount += (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] * 2;
                            }

                            string title = "";

                            if (rebarCount > 0)
                            {
                                title = rebarCount + "-" + (string)kakuData.Rows[currentNum][chutoSyukinHutokei];
                            }

                            if (hosoCount > 0)
                            {
                                title += " + " + hosoCount + "-" + (string)kakuData.Rows[currentNum][chutoSyukinHosokei];
                            }

                            Revit.DB.XYZ baseVec = vp.RightDirection;
                            Revit.DB.XYZ upVec = vp.UpDirection;

                            double lineWidth = w_HugowakuMax / scale;
                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                            Revit.DB.TextNote txtNote = null;

                            // 柱頭主筋エラー
                            bool isSyukinError = false;

                            #region 1段筋

                            // 太径が2本未満
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 2 || (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 2)
                            {
                                isSyukinError = true;
                            }
                            // 細径が太径本数以上
                            // X
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] >= 1)
                            {
                                //                                 if ((int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu])
                                //                                 {
                                //                                     isSyukinError = true;
                                //                                 }
                            }
                            // Y
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] >= 1)
                            {
                                //                                 if ((int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu])
                                //                                 {
                                //                                     isSyukinError = true;
                                //                                 }
                            }

                            #endregion

                            #region 2段筋
                            // X太径がある
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                            {
                                // 2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 直交方向1段筋太径4本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向1段筋太径本数より多い
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                // フラグが立っていない場合
                                if (secondConrnerSetFlag == 0)
                                {
                                    // 直交2段
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                                    {
                                        if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                        {
                                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] - 2)
                                            {
                                            }
                                            else
                                            {
                                                isSyukinError = true;
                                            }
                                        }
                                    }

                                    //if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] - 2 < (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu])
                                    //{
                                    //  isSyukinError = true;
                                    //}
                                }

                                // フラグが立っている場合
                                if (secondConrnerSetFlag == 1)
                                {
                                    // 直交方向2段筋あり
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                                    {
                                        // 2段筋本数4本以上かつ直交2段筋本数4本以上
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)
                                        {
                                        }
                                        else
                                        {
                                            // 2段筋本数4本以上
                                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4)
                                            {
                                                // 2段本数は1段本数-2以下
                                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] - 2)
                                                {
                                                }
                                                else
                                                {
                                                    isSyukinError = true;
                                                }
                                            }
                                            else
                                            {
                                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] == 3)
                                                {
                                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 5)
                                                    {
                                                        isSyukinError = true;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //// コーナー配筋するとき直交方向が4本未満
                                    //if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 3 && (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] < 4)
                                    //{
                                    //  isSyukinError = true;
                                    //}
                                }
                            }
                            // X細径がある
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] > 0)
                            {
                                // 同一方向2段筋太径本数2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向2段筋太径本以上
                                if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu])
                                    {
                                        isSyukinError = true;
                                    }
                                }

                                // 直交方向1段筋太径本数4本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }
                            }
                            // 合計
                            if (secondConrnerSetFlag == 0)
                            {
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] > 0)
                                {
                                    // 直交方向2段筋あり
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                    else
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] <
                                            (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }
                            if (secondConrnerSetFlag == 1)
                            {
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] <
                                    (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 2)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 5)
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }

                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] < 4)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }

                            // Y太径
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                            {
                                // 2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 直交方向1段筋太径4本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向1段筋太径本数より多い
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                // フラグが立っていない場合
                                if (secondConrnerSetFlag == 0)
                                {
                                    // 直交2段
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                                    {
                                        if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                        {
                                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] - 2)
                                            {
                                            }
                                            else
                                            {
                                                isSyukinError = true;
                                            }
                                        }                                         
                                    }

                                    //if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] - 2 < (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu])
                                    //{
                                    //  isSyukinError = true;
                                    //}
                                }

                                // フラグが立っている場合
                                if (secondConrnerSetFlag == 1)
                                {
                                    // 直交方向2段筋あり
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                                    {
                                        // 2段筋本数4本以上かつ直交2段筋本数4本以上
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4)
                                        {
                                        }
                                        else
                                        {
                                            // 2段筋本数4本以上
                                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)
                                            {
                                                // 2段本数は1段本数-2以下
                                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] - 2)
                                                {
                                                }
                                                else
                                                {
                                                    isSyukinError = true;
                                                }
                                            }
                                            else
                                            {
                                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] == 3)
                                                {
                                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 5)
                                                    {
                                                        isSyukinError = true;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    //// コーナー配筋するとき直交方向が4本未満
                                    //if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 3 && (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] < 4)
                                    //{
                                    //  isSyukinError = true;
                                    //}
                                }
                            }
                            // Y細径
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] > 0)
                            {
                                // 同一方向2段筋太径本数2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向2段筋太径本以上
                                if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu])
                                    {
                                        isSyukinError = true;
                                    }
                                }

                                // 直交方向1段筋太径本数4本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }
                            }
                            // 合計
                            if (secondConrnerSetFlag == 0)
                            {
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                    else
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] <
                                            (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }
                            if (secondConrnerSetFlag == 1)
                            {
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] <
                                    (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 2)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 5)
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }

                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] < 4)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }

                            #endregion

                            if (title != "" && isSyukinError == false)
                            {
                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            // 芯鉄筋
                            if (isCoreRebar)
                            {
                                int sinNum = (int)kakuData.Rows[currentNum][sintekkinNumber_kaku];

                                // 4本以上の偶数本
                                if (sinNum >= 4 && sinNum % 2 == 0)
                                {
                                    title = (int)kakuData.Rows[currentNum][sintekkinNumber_kaku] + "-" + (string)kakuData.Rows[currentNum][sintekkinkei_kaku];

                                    if ((int)kakuData.Rows[currentNum][sintekkinNumber_kaku] == 0)
                                    {
                                        title = "-";
                                    }

                                    if ((int)kakuData.Rows[currentNum][sintekkinNumber_kaku] != 0 && (string)kakuData.Rows[currentNum][sintekkinkei_kaku] != "")
                                    {
                                        txtNote = cmpService.CreateNewTextNote(vp, centerPoint_CoreRebar, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();

                                        //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                                    }
                                }
                            }

                            // 帯筋
                            if (hoopBracketShow == 0)
                            {
                                title = (int)kakuData.Rows[currentNum][chutoHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chutoHoopYHonsu] + hoopSpaceSymbol + (string)kakuData.Rows[currentNum][chutoHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chutoHoopPitch];
                            }
                            else
                            {
                                title = "[" + (int)kakuData.Rows[currentNum][chutoHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chutoHoopYHonsu] + "] " + (string)kakuData.Rows[currentNum][chutoHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chutoHoopPitch];
                            }

                            #region 帯筋 エラー判定

                            bool isHoopError = false;

                            // X方向
                            if ((int)kakuData.Rows[currentNum][chutoHoopXHonsu] < 2 || (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] < (int)kakuData.Rows[currentNum][chutoHoopXHonsu])
                            {
                                isHoopError = true;
                            }
                            // Y方向
                            if ((int)kakuData.Rows[currentNum][chutoHoopYHonsu] < 2 || (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] < (int)kakuData.Rows[currentNum][chutoHoopYHonsu])
                            {
                                isHoopError = true;
                            }
                            if ((string)kakuData.Rows[currentNum][chutoHoopXkei] == "")
                            {
                                isHoopError = true;
                            }
                            if ((double)kakuData.Rows[currentNum][chutoHoopPitch] == 0)
                            {
                                isHoopError = true;
                            }
                            #endregion

                            if (isHoopError == false)
                            {
                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            //              trans.Commit();
                            #endregion

                            // 断面を作成
                            Revit.DB.XYZ centerRectangle = cmpGeometry.Center2Point(hidariUe, migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                            centerRectangle += new Revit.DB.XYZ((leftSpace - rightSpace) / 2 / 304.8 * scale, (bottomSpace - topSpace) / 2 / 304.8 * scale, 0);
                            double dx = (double)kakuData.Rows[currentNum][cmpParameters.DX_Kaku];
                            double dy = (double)kakuData.Rows[currentNum][cmpParameters.DY_Kaku];

                            // 幅か高さが0以下
                            // 本当は鉄筋の径、本数、空き寸法などの合計より小さいときがいい
                            //if (dx <= 0 || dy <= 0)
                            //{
                            //  writeErr += level + hugoName + " : " + cmpAttribute.ResourceText("IDS_ERR_COLUMNXORY") + "\r\n";
                            //  continue;
                            //}

                            Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs = cmpGeometry.CrvRectangle(dx, dy, centerRectangle);
                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(dx, dy, centerRectangle);

                            cmpElements.AddCrvByAry(ref crvAryStrct, rectangleCrvs);

                            // 躯体は寸法線をひくため、逐一作図
                            Collections.Generic.IList<Revit.DB.DetailLine> dLines = new Collections.Generic.List<Revit.DB.DetailLine>();
                            trans.Start("Write Struct");

                            foreach (Revit.DB.Curve crv in crvAryStrct)
                            {
                                Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                dc.LineStyle = bodyLineType;
                                dLines.Add(dc as Revit.DB.DetailLine);
                            }

                            // 寸法線
                            cmpGeometry.CreateDimensionsRectangleBottomLeft(dLines, dimType, vp);

                            trans.Commit();
                            crvAryStrct.Clear();

                            // 配筋
                            string strRet = cmpService.CreateRebar_Kaku(kakuData, currentNum, centerRectangle, true, rvtUiApp);

                            if (strRet != "")
                            {
                                writeErr += level + hugoName + " : " + "\r\n" + strRet + "\r\n";
                                strRet = level + hugoName + " : " + "\r\n" + strRet;

                                lineWidth = w_HugowakuMax / scale;

                                Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe, migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));

                                //                trans.Start("Error Message");
                                txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                //txtNote.Width = w_HugowakuMax;
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //                trans.Commit();
                            }
                            else
                            {
                                // 配筋ができたら
                                // 寄せ筋記号と本数の作成

                                // X2段筋の有無判定
                                bool isX2ndRebar = cmpService.IsX2ndRebar;
                                // Y2段筋の有無判定
                                bool isY2ndRebar = cmpService.IsY2ndRebar;

                                // X寄せ筋記号直径
                                double x2ndSymbolDiameter = cmpService.X2ndSymbolDiameter;
                                // Y寄せ筋記号直径
                                double y2ndSymbolDiameter = cmpService.Y2ndSymbolDiameter;

                                Revit.DB.XYZ rectangleLeftTop = rectanglePoints[0];
                                Revit.DB.XYZ rectangleRightTop = rectanglePoints[1];
                                Revit.DB.XYZ rectangleRightBottom = rectanglePoints[2];
                                Revit.DB.XYZ rectangleLeftBottom = rectanglePoints[3];

                                // 寄せ筋記号作成
                                if (isY2ndRebar == true)
                                {
                                    if (isHaveRebar2ndFam == true)
                                    {
                                        #region ファミリ
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        rebar2ndUe.Add(pnt);
                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        rebar2ndUe.Add(pnt);
                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        rebar2ndSita.Add(pnt);
                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        rebar2ndSita.Add(pnt);
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 線分
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        Revit.DB.Curve halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 0);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);

                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 0);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);

                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 1);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);

                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 1);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);
                                        #endregion
                                    }
                                }
                                if (isX2ndRebar == true)
                                {
                                    if (isHaveRebar2ndFam == true)
                                    {
                                        #region ファミリ
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        rebar2ndHidari.Add(pnt);
                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        rebar2ndHidari.Add(pnt);
                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        rebar2ndMigi.Add(pnt);
                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        rebar2ndMigi.Add(pnt);

                                        #endregion
                                    }
                                    else
                                    {
                                        #region 線分
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        Revit.DB.Curve halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 3);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 2);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 2);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 3);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);
                                        #endregion
                                    }
                                }

                                // 本数の作成

                                double offset = vp.Scale / 304.8;
                                Revit.DB.XYZ verticVec = new Revit.DB.XYZ(0, 1, 0);
                                lineWidth = w_Komokuwaku / scale;
                                // 本数を分割表示
                                if (rebarShow == 1)
                                {
                                    #region

                                    //                  trans.Start("RebarNumberShow");

                                    // X段筋左側
                                    double diaSyukin = cmpService.SyukinDiameter;

                                    Revit.DB.XYZ txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + diaSyukin / 2, kaburi_kaku / 304.8, 0);

                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_BOTTOM;

                                    int x1Num = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                                    int x2Num = (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu];
                                    title = x1Num.ToString();

                                    if (x2Num > 0)
                                    {
                                        title += "+" + x2Num.ToString();

                                        txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                    }

                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // X段筋右側
                                    title = x1Num.ToString();
                                    txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - diaSyukin / 2, kaburi_kaku / 304.8, 0);
                                    if (x2Num > 0)
                                    {
                                        title = x2Num.ToString() + "+" + title;

                                        txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                    }
                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // Y段筋下側
                                    txtOrigin = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + diaSyukin / 2, 0);
                                    int y1Num = (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu];
                                    int y2Num = (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu];
                                    title = y1Num.ToString();

                                    if (y2Num > 0)
                                    {
                                        title += "+" + y2Num.ToString();
                                        txtOrigin = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + y2ndSymbolDiameter / 2, 0);
                                    }
                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // Y段筋上側
                                    title = y1Num.ToString();
                                    txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - diaSyukin / 2, 0);
                                    if (y2Num > 0)
                                    {
                                        title = y2Num.ToString() + "+" + title;

                                        txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, -kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, 0);
                                    }
                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    //                  trans.Commit();

                                    #endregion
                                }
                                // 本数をまとめて表示(分割表示とはXYの配置方向が逆)
                                else if (rebarShow == 2)
                                {
                                    #region

                                    //                  trans.Start("RebarNumberShow");

                                    // X段筋
                                    Revit.DB.XYZ txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleLeftBottom) + new Revit.DB.XYZ(-kaburi_kaku / 304.8, 0, 0);

                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_BOTTOM;

                                    int x1Num = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                                    int x2Num = (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu];
                                    title = x1Num.ToString();

                                    if (x2Num > 0)
                                    {
                                        title += "+" + x2Num.ToString();
                                    }

                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // Y段筋
                                    txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleRightTop) + new Revit.DB.XYZ(0, kaburi_kaku / 304.8, 0);

                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_BOTTOM;

                                    int y1Num = (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu];
                                    int y2Num = (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu];
                                    title = y1Num.ToString();

                                    if (y2Num > 0)
                                    {
                                        title += "+" + y2Num.ToString();
                                    }

                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    //                  trans.Commit();
                                    #endregion
                                }
                            }
                        }
                        #endregion

                        #region 異断面柱あり
                        if (isDifference == true)
                        {
                            hidariSita = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH) * 2, 0);
                            migiUe = hidariUe + new Revit.DB.XYZ(hugoMaxX + otherWidth, 0, 0);
                            migiSita = new Revit.DB.XYZ(migiUe.X, hidariSita.Y, migiSita.Z);

                            // 断面枠と配筋枠を作成
                            l = cmpElements.CreateBoundLine(hidariSita, migiSita);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            l = cmpElements.CreateBoundLine(migiUe, migiSita);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            l = cmpElements.CreateBoundLine(hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0), migiUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0));
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            // 主筋枠
                            Revit.DB.XYZ point1 = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                            Revit.DB.XYZ point2 = migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint1 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            Revit.DB.XYZ point3 = point1 + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0);
                            Revit.DB.XYZ point4 = point2 + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0);
                            l = cmpElements.CreateBoundLine(point3, point4);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint2 = cmpGeometry.Center2Point(point3, point4) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            // 芯鉄筋枠
                            Revit.DB.XYZ centerPoint_CoreRebar1 = null;
                            Revit.DB.XYZ centerPoint_CoreRebar2 = null;
                            if (isCoreRebar)
                            {
                                point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                centerPoint_CoreRebar1 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                                point3 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point4 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point3, point4);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                centerPoint_CoreRebar2 = cmpGeometry.Center2Point(point3, point4) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);
                            }

                            // 帯筋枠
                            point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint3 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            point3 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            point4 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point3, point4);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint4 = cmpGeometry.Center2Point(point3, point4) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            if (addFrameNumber > 0)
                            {
                                point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                point3 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point4 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point3, point4);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                for (int addFrameNum = 1; addFrameNum < addFrameNumber; ++addFrameNum)
                                {
                                    point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    l = cmpElements.CreateBoundLine(point1, point2);
                                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                    point3 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    point4 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    l = cmpElements.CreateBoundLine(point3, point4);
                                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                }
                            }

                            if (gaitou == false)
                            {
                                //斜線を作成
                                l = cmpElements.CreateBoundLine(migiUe, hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                l = cmpElements.CreateBoundLine(migiUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0),
                                                                hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0) + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0));
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                hidariUe = hidariSita;
                                continue;
                            }

                            // 鉄筋本数、径
                            #region

                            //              trans.Start("Rebar Number");

                            // 主筋 - 柱頭
                            // 主筋太径
                            // 四隅は重複している
                            
                            // X

                            syukinX1HosokeiHonsu = (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                            syukinX1HutokeiHonsu = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu];
                            syukinY1HosokeiHonsu = (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu];
                            syukinY1HutokeiHonsu = (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu];

                            if (syukinX1HosokeiHonsu >= 1)
                            {
                                if (syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu)
                                {
                                    X1HosoLessX1Huto = false;
                                }
                            }
                            // Y
                            if (syukinY1HosokeiHonsu >= 1)
                            {
                                if (syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu)
                                {
                                    Y1HosoLessY1Huto = false;
                                }
                            }


                            int rebarCount = ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu]) * 2 - 4;
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                            {
                                // 2段筋の端の2本は1段筋でカウントされている
                                rebarCount += ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] - 2) * 2;
                            }
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                            {
                                rebarCount += ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] - 2) * 2;
                            }

                            if(X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                            {
                            }
                            else
                            {
                                secondConrnerSetFlag = 0;
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)
                                {
                                    rebarCount -= 4;
                                }
                            }
                                // 2段筋コーナー
                                if (secondConrnerSetFlag == 1 &&
                                (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4 &&
                                (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)// &&
                                                                                               //(int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] - (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 2 &&
                                                                                               //(int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] - (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 2 &&
                                                                                               //((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu]) - ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu]) >= 2 &&
                                                                                               //((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu]) - ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu]) >= 2)
                            {
                                rebarCount -= 4;
                            }

                            // 主筋細径
                            // 四隅にはこない
                            int hosoCount = ((int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu]) * 2;
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] > 0)
                            {
                                // 2段筋の端にはこない
                                hosoCount += (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] * 2;
                            }
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] > 0)
                            {
                                hosoCount += (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] * 2;
                            }

                            string title = "";

                            if (rebarCount > 0)
                            {
                                title = rebarCount + "-" + (string)kakuData.Rows[currentNum][chutoSyukinHutokei];
                            }

                            if (hosoCount > 0)
                            {
                                title += " + " + hosoCount + "-" + (string)kakuData.Rows[currentNum][chutoSyukinHosokei];
                            }

                            Revit.DB.XYZ baseVec = vp.RightDirection;
                            Revit.DB.XYZ upVec = vp.UpDirection;

                            double lineWidth = w_HugowakuMax / scale;
                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                            Revit.DB.TextNote txtNote = null;
                                                        

                            // 柱頭主筋エラー
                            bool isSyukinError = false;

                            #region 1段筋

                            // 太径が2本未満
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 2 || (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 2)
                            {
                                isSyukinError = true;
                            }
                            // 細径が太径本数以上
                            // X
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] >= 1)
                            {
                                //                                 if ((int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu])
                                //                                 {
                                //                                     isSyukinError = true;
                                //                                 }
                            }
                            // Y
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] >= 1)
                            {
                                //                                 if ((int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu])
                                //                                 {
                                //                                     isSyukinError = true;
                                //                                 }
                            }

                            #endregion

                            #region 2段筋
                            // X太径がある
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                            {
                                // 2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 直交方向1段筋太径4本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向1段筋太径本数より多い
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                // フラグが立っていない場合
                                if (secondConrnerSetFlag == 0)
                                {
                                    // 直交2段
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                                    {
                                        if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                        {
                                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] - 2)
                                            {
                                            }
                                            else
                                            {
                                                isSyukinError = true;
                                            }
                                        }

                                    }
                                }

                                // フラグが立っている場合
                                if (secondConrnerSetFlag == 1)
                                {
                                    // 直交方向2段筋あり
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                                    {
                                        // 2段筋本数4本以上かつ直交2段筋本数4本以上
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)
                                        {
                                        }
                                        else
                                        {
                                            // 2段筋本数4本以上
                                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4)
                                            {
                                                // 2段本数は1段本数-2以下
                                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] - 2)
                                                {
                                                }
                                                else
                                                {
                                                    isSyukinError = true;
                                                }
                                            }
                                            else
                                            {
                                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] == 3)
                                                {
                                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 5)
                                                    {
                                                        isSyukinError = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // X細径がある
                            if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] > 0)
                            {
                                // 同一方向2段筋太径本数2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向2段筋太径本以上
                                if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu])
                                    {
                                        isSyukinError = true;
                                    }
                                }                                 

                                // 直交方向1段筋太径本数4本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }
                            }
                            // 合計
                            if (secondConrnerSetFlag == 0)
                            {
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                    else
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] <
                                            (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }
                            if (secondConrnerSetFlag == 1)
                            {
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] <
                                    (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 2)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 5)
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }

                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] < 4)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }

                            // Y太径
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] > 0)
                            {
                                // 2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 直交方向1段筋太径4本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向1段筋太径本数より多い
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                // フラグが立っていない場合
                                if (secondConrnerSetFlag == 0)
                                {
                                    // 直交2段
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                                    {
                                        if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                        {
                                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] - 2)
                                            {
                                            }
                                            else
                                            {
                                                isSyukinError = true;
                                            }
                                        }                                           
                                    }
                                }

                                // フラグが立っている場合
                                if (secondConrnerSetFlag == 1)
                                {
                                    // 直交方向2段筋あり
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                                    {
                                        // 2段筋本数4本以上かつ直交2段筋本数4本以上
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 4)
                                        {
                                        }
                                        else
                                        {
                                            // 2段筋本数4本以上
                                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)
                                            {
                                                // 2段本数は1段本数-2以下
                                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] - 2)
                                                {
                                                }
                                                else
                                                {
                                                    isSyukinError = true;
                                                }
                                            }
                                            else
                                            {
                                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] == 3)
                                                {
                                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 5)
                                                    {
                                                        isSyukinError = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // Y細径
                            if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] > 0)
                            {
                                // 同一方向2段筋太径本数2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向2段筋太径本以上
                                if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu])
                                    {
                                        isSyukinError = true;
                                    }
                                }                                

                                // 直交方向1段筋太径本数4本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }
                            }
                            // 合計
                            if (secondConrnerSetFlag == 0)
                            {
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                    else
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] <
                                            (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }
                            if (secondConrnerSetFlag == 1)
                            {
                                if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] <
                                    (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                if ((int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] >= 2)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 5)
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }

                                if ((int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] < 4)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }

                            #endregion

                            if (title != "" && isSyukinError == false)
                            {
                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            // 主筋 - 柱脚
                            // 主筋太径
                            // 四隅は重複している

                            // X

                            syukinX1HosokeiHonsu = (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu];
                            syukinX1HutokeiHonsu = (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu];
                            syukinY1HosokeiHonsu = (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu];
                            syukinY1HutokeiHonsu = (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu];

                            if (syukinX1HosokeiHonsu >= 1)
                            {
                                if (syukinX1HosokeiHonsu >= syukinX1HutokeiHonsu)
                                {
                                    X1HosoLessX1Huto = false;
                                }
                            }
                            // Y
                            if (syukinY1HosokeiHonsu >= 1)
                            {
                                if (syukinY1HosokeiHonsu >= syukinY1HutokeiHonsu)
                                {
                                    Y1HosoLessY1Huto = false;
                                }
                            }

                            rebarCount = ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu]) * 2 - 4;
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] > 0)
                            {
                                // 2段筋の端の2本は1段筋でカウントされている
                                rebarCount += ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] - 2) * 2;
                            }
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] > 0)
                            {
                                rebarCount += ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] - 2) * 2;
                            }

                            if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                            {
                            }
                            else
                            {
                                secondConrnerSetFlag = 0;
                                if((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] >= 4)
                                {
                                    rebarCount -= 4;
                                }
                            }
                            // 2段筋コーナー
                            if (secondConrnerSetFlag == 1 &&
                                (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] >= 4 &&
                                (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] >= 4)// &&
                                                                                                  //(int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] - (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] >= 2 &&
                                                                                                  //(int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] - (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] >= 2 &&
                                                                                                  //((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu]) - ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu]) >= 2 &&
                                                                                                  //((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu]) - ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu]) >= 2)
                            {
                                rebarCount -= 4;
                            }

                            // 主筋細径
                            // 四隅にはこない
                            hosoCount = ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu]) * 2;
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu] > 0)
                            {
                                // 2段筋の端にはこない
                                hosoCount += (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu] * 2;
                            }
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu] > 0)
                            {
                                hosoCount += (int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu] * 2;
                            }

                            title = "";

                            if (rebarCount > 0)
                            {
                                title = rebarCount + "-" + (string)kakuData.Rows[currentNum][chukyakuSyukinHutokei];
                            }

                            if (hosoCount > 0)
                            {
                                title += " + " + hosoCount + "-" + (string)kakuData.Rows[currentNum][chukyakuSyukinHosokei];
                            }

                            // 柱脚主筋エラー
                            isSyukinError = false;

                            #region 1段筋

                            // 太径が2本未満
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] < 2 || (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] < 2)
                            {
                                isSyukinError = true;
                            }
                            // 細径が太径本数以上
                            // X
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] >= 1)
                            {
                                //                                 if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu])
                                //                                 {
                                //                                     isSyukinError = true;
                                //                                 }
                            }
                            // Y
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu] >= 1)
                            {
                                //                                 if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu])
                                //                                 {
                                //                                     isSyukinError = true;
                                //                                 }
                            }

                            #endregion

                            #region 2段筋
                            // X太径がある
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] > 0)
                            {
                                // 2本未満
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 直交方向1段筋太径4本未満
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向1段筋太径本数より多い
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] < (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                // フラグが立っていない場合
                                if (secondConrnerSetFlag == 0)
                                {
                                    // 直交2段
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] > 0)
                                    {
                                        if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                        {
                                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] - 2)
                                            {
                                            }
                                            else
                                            {
                                                isSyukinError = true;
                                            }
                                        }                                          
                                    }
                                }

                                // フラグが立っている場合
                                if (secondConrnerSetFlag == 1)
                                {
                                    // 直交方向2段筋あり
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] > 0)
                                    {
                                        // 2段筋本数4本以上かつ直交2段筋本数4本以上
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] >= 4)
                                        {
                                        }
                                        else
                                        {
                                            // 2段筋本数4本以上
                                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] >= 4)
                                            {
                                                // 2段本数は1段本数-2以下
                                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] - 2)
                                                {
                                                }
                                                else
                                                {
                                                    isSyukinError = true;
                                                }
                                            }
                                            else
                                            {
                                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] == 3)
                                                {
                                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] < 5)
                                                    {
                                                        isSyukinError = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // X細径がある
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu] > 0)
                            {
                                // 同一方向2段筋太径本数2本未満
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向2段筋太径本以上
                                if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                {
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu])
                                    {
                                        isSyukinError = true;
                                    }
                                }                                 

                                // 直交方向1段筋太径本数4本未満
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }
                            }
                            // 合計
                            if (secondConrnerSetFlag == 0)
                            {
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] > 0)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                    else
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] <
                                            (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }
                            if (secondConrnerSetFlag == 1)
                            {
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] <
                                    (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] >= 2)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] < 5)
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }

                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] < 4)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }

                            // Y太径
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] > 0)
                            {
                                // 2本未満
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 直交方向1段筋太径4本未満
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向1段筋太径本数より多い
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] < (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                // フラグが立っていない場合
                                if (secondConrnerSetFlag == 0)
                                {
                                    // 直交2段
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] > 0)
                                    {
                                        if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                        {
                                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] - 2)
                                            {
                                            }
                                            else
                                            {
                                                isSyukinError = true;
                                            }
                                        }                                         
                                    }
                                }

                                // フラグが立っている場合
                                if (secondConrnerSetFlag == 1)
                                {
                                    // 直交方向2段筋あり
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] > 0)
                                    {
                                        // 2段筋本数4本以上かつ直交2段筋本数4本以上
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] >= 4)
                                        {
                                        }
                                        else
                                        {
                                            // 2段筋本数4本以上
                                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] >= 4)
                                            {
                                                // 2段本数は1段本数-2以下
                                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] <= (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] - 2)
                                                {
                                                }
                                                else
                                                {
                                                    isSyukinError = true;
                                                }
                                            }
                                            else
                                            {
                                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] == 3)
                                                {
                                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] < 5)
                                                    {
                                                        isSyukinError = true;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            // Y細径
                            if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu] > 0)
                            {
                                // 同一方向2段筋太径本数2本未満
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }

                                // 同一方向2段筋太径本以上
                                if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                {
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu])
                                    {
                                        isSyukinError = true;
                                    }
                                }                                   

                                // 直交方向1段筋太径本数4本未満
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] < 4)
                                {
                                    isSyukinError = true;
                                }
                            }
                            // 合計
                            if (secondConrnerSetFlag == 0)
                            {
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] > 0)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                    else
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu] <
                                            (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }
                            if (secondConrnerSetFlag == 1)
                            {
                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu] <
                                    (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu])
                                {
                                    isSyukinError = true;
                                }

                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] >= 2)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] < 5)
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }

                                if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] > 0)
                                {
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] < 4)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu] - 2 <
                                            (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                }
                            }

                            #endregion

                            if (title != "" && isSyukinError == false)
                            {
                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            // 芯鉄筋
                            if (isCoreRebar)
                            {
                                int sinNum = (int)kakuData.Rows[currentNum][sintekkinNumber_kaku];

                                if (sinNum > 0)
                                {
                                    title = (int)kakuData.Rows[currentNum][sintekkinNumber_kaku] + "-" + (string)kakuData.Rows[currentNum][sintekkinkei_kaku];

                                    if ((int)kakuData.Rows[currentNum][sintekkinNumber_kaku] == 0)
                                    {
                                        title = "-";
                                    }

                                    if ((int)kakuData.Rows[currentNum][sintekkinNumber_kaku] != 0 && (string)kakuData.Rows[currentNum][sintekkinkei_kaku] != "")
                                    {
                                        txtNote = cmpService.CreateNewTextNote(vp, centerPoint_CoreRebar1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();

                                        //txtNote.Width = w_HugowakuMax;// hugoMaxX;

                                        txtNote = cmpService.CreateNewTextNote(vp, centerPoint_CoreRebar2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();

                                        //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                                    }
                                }
                            }

                            // 帯筋 - 柱頭
                            if (hoopBracketShow == 0)
                            {
                                title = (int)kakuData.Rows[currentNum][chutoHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chutoHoopYHonsu] + hoopSpaceSymbol + (string)kakuData.Rows[currentNum][chutoHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chutoHoopPitch];
                            }
                            else
                            {
                                title = "[" + (int)kakuData.Rows[currentNum][chutoHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chutoHoopYHonsu] + "] " + (string)kakuData.Rows[currentNum][chutoHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chutoHoopPitch];
                            }

                            #region 帯筋 エラー判定

                            bool isHoopError = false;

                            // X方向
                            if ((int)kakuData.Rows[currentNum][chutoHoopXHonsu] < 2 || (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] < (int)kakuData.Rows[currentNum][chutoHoopXHonsu])
                            {
                                isHoopError = true;
                            }
                            // Y方向
                            if ((int)kakuData.Rows[currentNum][chutoHoopYHonsu] < 2 || (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] < (int)kakuData.Rows[currentNum][chutoHoopYHonsu])
                            {
                                isHoopError = true;
                            }
                            if ((string)kakuData.Rows[currentNum][chutoHoopXkei] == "")
                            {
                                isHoopError = true;
                            }
                            if ((double)kakuData.Rows[currentNum][chutoHoopPitch] == 0)
                            {
                                isHoopError = true;
                            }
                            #endregion

                            if (isHoopError == false)
                            {
                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint3, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            // 帯筋 - 柱脚
                            if (hoopBracketShow == 0)
                            {
                                title = (int)kakuData.Rows[currentNum][chukyakuHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chukyakuHoopYHonsu] + hoopSpaceSymbol + (string)kakuData.Rows[currentNum][chukyakuHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chukyakuHoopPitch];
                            }
                            else
                            {
                                title = "[" + (int)kakuData.Rows[currentNum][chukyakuHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chukyakuHoopYHonsu] + "] " + (string)kakuData.Rows[currentNum][chukyakuHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chukyakuHoopPitch];
                            }

                            #region 帯筋 エラー判定

                            isHoopError = false;

                            // X方向
                            if ((int)kakuData.Rows[currentNum][chukyakuHoopXHonsu] < 2 || (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] < (int)kakuData.Rows[currentNum][chukyakuHoopXHonsu])
                            {
                                isHoopError = true;
                            }
                            // Y方向
                            if ((int)kakuData.Rows[currentNum][chukyakuHoopYHonsu] < 2 || (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu] < (int)kakuData.Rows[currentNum][chukyakuHoopYHonsu])
                            {
                                isHoopError = true;
                            }
                            if ((string)kakuData.Rows[currentNum][chukyakuHoopXkei] == "")
                            {
                                isHoopError = true;
                            }
                            if ((double)kakuData.Rows[currentNum][chukyakuHoopPitch] == 0)
                            {
                                isHoopError = true;
                            }
                            #endregion

                            if (isHoopError == false)
                            {
                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint4, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            //              trans.Commit();
                            #endregion

                            // 断面を作成
                            #region 柱頭

                            Revit.DB.XYZ centerRectangle = cmpGeometry.Center2Point(migiUe, hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                            centerRectangle += new Revit.DB.XYZ((leftSpace - rightSpace) / 2 / 304.8 * scale, (bottomSpace - topSpace) / 2 / 304.8 * scale, 0);
                            double dx = (double)kakuData.Rows[currentNum][cmpParameters.DX_Kaku];
                            double dy = (double)kakuData.Rows[currentNum][cmpParameters.DY_Kaku];

                            //if (dx <= 0 || dy <= 0)
                            //{
                            //  writeErr += level + hugoName + " : " + cmpAttribute.ResourceText("IDS_ERR_COLUMNXORY") + "\r\n";
                            //  continue;
                            //}

                            Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs = cmpGeometry.CrvRectangle(dx, dy, centerRectangle);
                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(dx, dy, centerRectangle);

                            cmpElements.AddCrvByAry(ref crvAryStrct, rectangleCrvs);

                            // 躯体は寸法線をひくため、逐一作図
                            Collections.Generic.IList<Revit.DB.DetailLine> dLines = new Collections.Generic.List<Revit.DB.DetailLine>();
                            trans.Start("Write Struct");

                            foreach (Revit.DB.Curve crv in crvAryStrct)
                            {
                                Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                dc.LineStyle = bodyLineType;
                                dLines.Add(dc as Revit.DB.DetailLine);
                            }

                            // 寸法線
                            cmpGeometry.CreateDimensionsRectangleBottomLeft(dLines, dimType, vp);

                            trans.Commit();
                            crvAryStrct.Clear();
                            dLines.Clear();

                            // 配筋
                            string strRet = cmpService.CreateRebar_Kaku(kakuData, currentNum, centerRectangle, true, rvtUiApp);
                            if (strRet != "")
                            {
                                writeErr += level + hugoName + "(柱頭) : " + "\r\n" + strRet + "\r\n";
                                strRet = level + hugoName + "(柱頭) : " + "\r\n" + strRet;

                                lineWidth = w_HugowakuMax / scale;

                                Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(migiUe, hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));

                                //                trans.Start("Error Message");
                                txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                //txtNote.Width = w_HugowakuMax;
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //                trans.Commit();
                            }
                            else
                            {
                                // 配筋ができたら
                                // 寄せ筋記号と本数の作成

                                // X2段筋の有無判定
                                bool isX2ndRebar = cmpService.IsX2ndRebar;
                                // Y2段筋の有無判定
                                bool isY2ndRebar = cmpService.IsY2ndRebar;

                                // X寄せ筋記号直径
                                double x2ndSymbolDiameter = cmpService.X2ndSymbolDiameter;
                                // Y寄せ筋記号直径
                                double y2ndSymbolDiameter = cmpService.Y2ndSymbolDiameter;

                                Revit.DB.XYZ rectangleLeftTop = rectanglePoints[0];
                                Revit.DB.XYZ rectangleRightTop = rectanglePoints[1];
                                Revit.DB.XYZ rectangleRightBottom = rectanglePoints[2];
                                Revit.DB.XYZ rectangleLeftBottom = rectanglePoints[3];

                                // 寄せ筋記号作成
                                if (isY2ndRebar == true)
                                {
                                    if (isHaveRebar2ndFam == true)
                                    {
                                        #region ファミリ
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        rebar2ndUe.Add(pnt);
                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        rebar2ndUe.Add(pnt);
                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        rebar2ndSita.Add(pnt);
                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        rebar2ndSita.Add(pnt);
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 線分
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        Revit.DB.Curve halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 0);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);

                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 0);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);

                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 1);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);

                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 1);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);
                                        #endregion
                                    }
                                }
                                if (isX2ndRebar == true)
                                {
                                    if (isHaveRebar2ndFam == true)
                                    {
                                        #region ファミリ
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        rebar2ndHidari.Add(pnt);
                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        rebar2ndHidari.Add(pnt);
                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        rebar2ndMigi.Add(pnt);
                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        rebar2ndMigi.Add(pnt);

                                        #endregion
                                    }
                                    else
                                    {
                                        #region 線分
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        Revit.DB.Curve halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 3);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 2);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 2);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 3);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);
                                        #endregion
                                    }
                                }

                                // 本数を作成

                                double offset = vp.Scale / 304.8;
                                Revit.DB.XYZ verticVec = new Revit.DB.XYZ(0, 1, 0);
                                lineWidth = w_Komokuwaku / scale;
                                // 本数を分割表示
                                if (rebarShow == 1)
                                {
                                    #region

                                    //                  trans.Start("RebarNumberShow");

                                    // X段筋左側
                                    double diaSyukin = cmpService.SyukinDiameter;

                                    Revit.DB.XYZ txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + diaSyukin / 2, kaburi_kaku / 304.8, 0);

                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_BOTTOM;

                                    int x1Num = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                                    int x2Num = (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu];
                                    title = x1Num.ToString();

                                    if (x2Num > 0)
                                    {
                                        title += "+" + x2Num.ToString();

                                        txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                    }

                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // X段筋右側
                                    title = x1Num.ToString();
                                    txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - diaSyukin / 2, kaburi_kaku / 304.8, 0);
                                    if (x2Num > 0)
                                    {
                                        title = x2Num.ToString() + "+" + title;

                                        txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                    }
                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // Y段筋下側
                                    txtOrigin = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + diaSyukin / 2, 0);
                                    int y1Num = (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu];
                                    int y2Num = (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu];
                                    title = y1Num.ToString();

                                    if (y2Num > 0)
                                    {
                                        title += "+" + y2Num.ToString();
                                        txtOrigin = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + y2ndSymbolDiameter / 2, 0);
                                    }
                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // Y段筋上側
                                    title = y1Num.ToString();
                                    txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - diaSyukin / 2, 0);
                                    if (y2Num > 0)
                                    {
                                        title = y2Num.ToString() + "+" + title;

                                        txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, -kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, 0);
                                    }
                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    //                  trans.Commit();

                                    #endregion
                                }
                                // 本数をまとめて表示
                                else if (rebarShow == 2)
                                {
                                    #region

                                    //                  trans.Start("RebarNumberShow");

                                    // X段筋
                                    Revit.DB.XYZ txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleLeftBottom) + new Revit.DB.XYZ(-kaburi_kaku / 304.8, 0, 0);

                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_BOTTOM;

                                    int x1Num = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                                    int x2Num = (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu];
                                    title = x1Num.ToString();

                                    if (x2Num > 0)
                                    {
                                        title += "+" + x2Num.ToString();
                                    }

                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // Y段筋
                                    txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleRightTop) + new Revit.DB.XYZ(0, kaburi_kaku / 304.8, 0);

                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_BOTTOM;

                                    int y1Num = (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu];
                                    int y2Num = (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu];
                                    title = y1Num.ToString();

                                    if (y2Num > 0)
                                    {
                                        title += "+" + y2Num.ToString();
                                    }

                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    //                  trans.Commit();
                                    #endregion
                                }
                            }
                            #endregion

                            #region 柱脚

                            centerRectangle = cmpGeometry.Center2Point(migiUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0),
                                                                       hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0) + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0));
                            centerRectangle += new Revit.DB.XYZ((leftSpace - rightSpace) / 2 / 304.8 * scale, (bottomSpace - topSpace) / 2 / 304.8 * scale, 0);
                            rectangleCrvs = cmpGeometry.CrvRectangle(dx, dy, centerRectangle);
                            rectanglePoints = cmpGeometry.RectanglePoints(dx, dy, centerRectangle);

                            cmpElements.AddCrvByAry(ref crvAryStrct, rectangleCrvs);

                            trans.Start("Write Struct");

                            foreach (Revit.DB.Curve crv in crvAryStrct)
                            {
                                Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                dc.LineStyle = bodyLineType;
                                dLines.Add(dc as Revit.DB.DetailLine);
                            }

                            // 寸法線
                            cmpGeometry.CreateDimensionsRectangleBottomLeft(dLines, dimType, vp);

                            trans.Commit();
                            crvAryStrct.Clear();

                            // 配筋
                            strRet = cmpService.CreateRebar_Kaku(kakuData, currentNum, centerRectangle, false, rvtUiApp);

                            if (strRet != "")
                            {
                                writeErr += level + hugoName + "(柱脚) : " + "\r\n" + strRet + "\r\n";
                                strRet = level + hugoName + "(柱脚) : " + "\r\n" + strRet;

                                lineWidth = w_HugowakuMax / scale;

                                Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(migiUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0),
                                                                                    hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0) +
                                                                                      new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0));

                                //                trans.Start("Error Message");
                                txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                //txtNote.Width = w_HugowakuMax;
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //                trans.Commit();
                            }
                            else
                            {
                                // 配筋ができたら
                                // 寄せ筋記号と本数の作成

                                // X2段筋の有無判定
                                bool isX2ndRebar = cmpService.IsX2ndRebar;
                                // Y2段筋の有無判定
                                bool isY2ndRebar = cmpService.IsY2ndRebar;

                                // X寄せ筋記号直径
                                double x2ndSymbolDiameter = cmpService.X2ndSymbolDiameter;
                                // Y寄せ筋記号直径
                                double y2ndSymbolDiameter = cmpService.Y2ndSymbolDiameter;

                                Revit.DB.XYZ rectangleLeftTop = rectanglePoints[0];
                                Revit.DB.XYZ rectangleRightTop = rectanglePoints[1];
                                Revit.DB.XYZ rectangleRightBottom = rectanglePoints[2];
                                Revit.DB.XYZ rectangleLeftBottom = rectanglePoints[3];

                                #region 寄せ筋記号作成(旧)
                                //if (isX2ndRebar == true)
                                //{
                                //  #region
                                //  // 寄せ筋記号
                                //  // 左上
                                //  Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                //  Revit.DB.Curve halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 0);
                                //  cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                //  // 右上
                                //  pnt = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                //  halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 0);
                                //  cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                //  // 右下
                                //  pnt = rectangleRightBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                //  halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 1);
                                //  cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                //  // 左下
                                //  pnt = rectangleLeftBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                //  halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 1);
                                //  cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);
                                //  #endregion
                                //}
                                //if (isY2ndRebar == true)
                                //{
                                //  #region
                                //  // 寄せ筋記号
                                //  // 左上
                                //  Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                //  Revit.DB.Curve halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 3);
                                //  cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                //  // 右上
                                //  pnt = rectangleRightTop + new Revit.DB.XYZ(kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                //  halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 2);
                                //  cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                //  // 右下
                                //  pnt = rectangleRightBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                //  halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 2);
                                //  cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                //  // 左下
                                //  pnt = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                //  halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 3);
                                //  cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);
                                //  #endregion
                                //}
                                #endregion

                                // 寄せ筋記号作成
                                if (isY2ndRebar == true)
                                {
                                    if (isHaveRebar2ndFam == true)
                                    {
                                        #region ファミリ
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        rebar2ndUe.Add(pnt);
                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        rebar2ndUe.Add(pnt);
                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        rebar2ndSita.Add(pnt);
                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        rebar2ndSita.Add(pnt);
                                        #endregion
                                    }
                                    else
                                    {
                                        #region 線分
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        Revit.DB.Curve halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 0);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);

                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 0);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);

                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 1);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);

                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, -kaburi_kaku / 304.8, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 1);
                                        cmpElements.NotNullCurveSet(ref crvAryStrct, halfCrv);
                                        #endregion
                                    }
                                }
                                if (isX2ndRebar == true)
                                {
                                    if (isHaveRebar2ndFam == true)
                                    {
                                        #region ファミリ
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        rebar2ndHidari.Add(pnt);
                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        rebar2ndHidari.Add(pnt);
                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        rebar2ndMigi.Add(pnt);
                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        rebar2ndMigi.Add(pnt);

                                        #endregion
                                    }
                                    else
                                    {
                                        #region 線分
                                        // 左上
                                        Revit.DB.XYZ pnt = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        Revit.DB.Curve halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 3);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                        // 右上
                                        pnt = rectangleRightTop + new Revit.DB.XYZ(kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 2);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                        // 右下
                                        pnt = rectangleRightBottom + new Revit.DB.XYZ(kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 2);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);

                                        // 左下
                                        pnt = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        halfCrv = cmpGeometry.CrvHalfCircle(x2ndSymbolDiameter, pnt, 3);
                                        cmpElements.NotNullCurveSet(ref crvAryFrame, halfCrv);
                                        #endregion
                                    }
                                }

                                // 本数を作成

                                double offset = vp.Scale / 304.8;
                                Revit.DB.XYZ verticVec = new Revit.DB.XYZ(0, 1, 0);
                                lineWidth = w_Komokuwaku / scale;
                                // 本数を分割表示
                                if (rebarShow == 1)
                                {
                                    #region

                                    //                  trans.Start("RebarNumberShow");

                                    // X段筋左側
                                    double diaSyukin = cmpService.SyukinDiameter;

                                    Revit.DB.XYZ txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + diaSyukin / 2, kaburi_kaku / 304.8, 0);

                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_BOTTOM;

                                    int x1Num = (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu];
                                    int x2Num = (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu];
                                    title = x1Num.ToString();

                                    if (x2Num > 0)
                                    {
                                        title += "+" + x2Num.ToString();

                                        txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                    }

                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // X段筋右側
                                    title = x1Num.ToString();
                                    txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - diaSyukin / 2, kaburi_kaku / 304.8, 0);
                                    if (x2Num > 0)
                                    {
                                        title = x2Num.ToString() + "+" + title;

                                        txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                    }
                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // Y段筋下側
                                    txtOrigin = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8, kaburi_kaku / 304.8 + diaSyukin / 2, 0);
                                    int y1Num = (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu];
                                    int y2Num = (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu];
                                    title = y1Num.ToString();

                                    if (y2Num > 0)
                                    {
                                        title += "+" + y2Num.ToString();
                                        txtOrigin = rectangleLeftBottom + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + y2ndSymbolDiameter / 2, 0);
                                    }
                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // Y段筋上側
                                    title = y1Num.ToString();
                                    txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - diaSyukin / 2, 0);
                                    if (y2Num > 0)
                                    {
                                        title = y2Num.ToString() + "+" + title;
                                        txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, -kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, 0);
                                    }
                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    //                  trans.Commit();

                                    #endregion
                                }
                                // 本数をまとめて表示
                                else if (rebarShow == 2)
                                {
                                    #region

                                    //                  trans.Start("RebarNumberShow");

                                    // X段筋
                                    Revit.DB.XYZ txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleLeftBottom) + new Revit.DB.XYZ(-kaburi_kaku / 304.8, 0, 0);

                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_BOTTOM;

                                    int x1Num = (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu];
                                    int x2Num = (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu];
                                    title = x1Num.ToString();

                                    if (x2Num > 0)
                                    {
                                        title += "+" + x2Num.ToString();
                                    }

                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    // Y段筋
                                    txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleRightTop) + new Revit.DB.XYZ(0, kaburi_kaku / 304.8, 0);

                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_BOTTOM;

                                    int y1Num = (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu];
                                    int y2Num = (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu];
                                    title = y1Num.ToString();

                                    if (y2Num > 0)
                                    {
                                        title += "+" + y2Num.ToString();
                                    }

                                    txtNote = cmpService.CreateNewTextNote_Offset(vp, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtNote.Width = w_Komokuwaku / 2;

                                    //                  trans.Commit();
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        #endregion

                        // 左下を左上
                        hidariUe = hidariSita;
                    }

                    hugouHidariUe += new Revit.DB.XYZ(hugoMaxX + otherWidth, 0, 0);

                    hidariUe = new Revit.DB.XYZ(hugouHidariUe.X, hugouHidariUe.Y + h_Hugowaku, hugouHidariUe.Z);
                }
                #endregion

                #region 円柱
                for (int i = 0; i < enHugoAry.Count; ++i)
                {
                    hidariSita = hidariUe;
                    migiUe = hidariUe;
                    migiSita = hidariUe;

                    // 現在の符号
                    string hugoName = enHugoAry[i];
                    // 現在の符号最大幅
                    double hugoMaxX = cmpService.ColumnWidthByMark(enData, hugoName, false);

                    // 符号最大幅
                    w_HugowakuMax = hugoMaxX + (leftSpace + rightSpace) / 304.8 * scale;

                    for (int j = 0; j < allColumnLevelAry.Count; ++j)
                    {
                        // 現在の階
                        string level = allColumnLevelAry[j];

                        // 現在の階最大高さ
                        double levelMaxY = cmpService.ColumnHeightByLevel(kakuData, enData, level);

                        // 現在階での芯鉄筋の有無
                        bool isCoreRebar = cmpService.IsCoreRebarInLevel(kakuData, level);
                        if (isCoreRebar == false)
                        {
                            isCoreRebar = cmpService.IsCoreRebarInLevel(enData, level);
                        }

                        int haikinwakuNum = 2 + addFrameNumber;
                        if (isCoreRebar)
                        {
                            haikinwakuNum += 1;
                        }

                        double otherH = (topSpace + bottomSpace + arrangementFrameHeight * haikinwakuNum) / 304.8 * scale;

                        // 同一階での異断面柱の有無
                        bool isDifference = false;
                        isDifference = cmpService.IsDifferenceTopBottomRebarInLevel(kakuData, level);
                        if (isDifference == false)
                        {
                            isDifference = cmpService.IsDifferenceTopBottomRebarInLevel(enData, level);
                        }

                        //データテーブル上の番号
                        int currentNum = 0;
                        // 符号と階の組み合わせに該当するか
                        bool gaitou = false;
                        for (int k = 0; k < enData.Rows.Count; ++k)
                        {
                            string currenthugo = (string)enData.Rows[k][cmpParameters.RST_HasiraHugo_En];
                            string currentlevel = (string)enData.Rows[k][cmpParameters.LevelFrameTitle];

                            if (currenthugo == hugoName && currentlevel == level)
                            {
                                currentNum = k;
                                gaitou = true;
                                break;
                            }
                        }

                        #region 異断面柱なし
                        if (isDifference == false)
                        {
                            hidariSita = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0);
                            migiUe = hidariUe + new Revit.DB.XYZ(hugoMaxX + otherWidth, 0, 0);
                            migiSita = new Revit.DB.XYZ(migiUe.X, hidariSita.Y, migiSita.Z);

                            // 断面枠と配筋枠を作成
                            l = cmpElements.CreateBoundLine(hidariSita, migiSita);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            l = cmpElements.CreateBoundLine(migiUe, migiSita);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            // 主筋枠
                            Revit.DB.XYZ point1 = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                            Revit.DB.XYZ point2 = migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint1 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            // 芯鉄筋枠
                            Revit.DB.XYZ centerPoint_CoreRebar = null;
                            if (isCoreRebar)
                            {
                                point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                centerPoint_CoreRebar = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);
                            }

                            // 帯筋枠
                            point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint2 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            if (addFrameNumber > 0)
                            {
                                point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                for (int addFrameNum = 1; addFrameNum < addFrameNumber; ++addFrameNum)
                                {
                                    point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    l = cmpElements.CreateBoundLine(point1, point2);
                                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                }
                            }

                            if (gaitou == false)
                            {
                                //斜線を作成
                                l = cmpElements.CreateBoundLine(migiUe, hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                hidariUe = hidariSita;
                                continue;
                            }

                            // 主筋本数、径、(芯鉄筋本数、径)、帯筋本数、径
                            #region
                            //              trans.Start("Rebar Number");

                            string title = "";
                            Revit.DB.TextNote txtNote = null;
                            Revit.DB.XYZ baseVec = vp.RightDirection;
                            Revit.DB.XYZ upVec = vp.UpDirection;
                            double lineWidth = w_HugowakuMax / scale;
                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                            if ((int)enData.Rows[currentNum][chutoSyukinHonsu] > 0 && (string)enData.Rows[currentNum][chutoSyukinkei] != "")
                            {
                                title = (int)enData.Rows[currentNum][chutoSyukinHonsu] + "-" + (string)enData.Rows[currentNum][chutoSyukinkei];

                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            if (isCoreRebar)
                            {
                                int sinNum = (int)enData.Rows[currentNum][sintekkinNumber];

                                if (sinNum > 0 && (string)enData.Rows[currentNum][sintekkinkei_en] != "")
                                {
                                    title = (int)enData.Rows[currentNum][sintekkinNumber] + "-" + (string)enData.Rows[currentNum][sintekkinkei_en];
                                    txtNote = cmpService.CreateNewTextNote(vp, centerPoint_CoreRebar, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();

                                    //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                                }
                            }

                            //if (hoopBracketShow == 0)
                            //{
                            //  title = (int)enData.Rows[currentNum][chutoHoopXHonsu] + hoopSpaceSymbol + (int)enData.Rows[currentNum][chutoHoopYHonsu] + hoopSpaceSymbol + (string)enData.Rows[currentNum][chutoHoopXkei] + "@" + (double)enData.Rows[currentNum][chutoHoopPitch];
                            //}
                            //else
                            //{
                            //  title = "[" + (int)enData.Rows[currentNum][chutoHoopXHonsu] + hoopSpaceSymbol + (int)enData.Rows[currentNum][chutoHoopYHonsu] + "] " + (string)enData.Rows[currentNum][chutoHoopXkei] + "@" + (double)enData.Rows[currentNum][chutoHoopPitch];
                            //}

                            title = (string)enData.Rows[currentNum][chutoHoopXkei_en] + "@" + (double)enData.Rows[currentNum][chutoHoopPitch_en];

                            if ((string)enData.Rows[currentNum][chutoHoopXkei_en] != "" && (double)enData.Rows[currentNum][chutoHoopPitch_en] != 0)
                            {
                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            //              trans.Commit();
                            #endregion

                            // 断面を作成
                            Revit.DB.XYZ centerCircle = cmpGeometry.Center2Point(hidariUe, migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                            centerCircle += new Revit.DB.XYZ((leftSpace - rightSpace) / 2 / 304.8 * scale, (bottomSpace - topSpace) / 2 / 304.8 * scale, 0);
                            double diameter = (double)enData.Rows[currentNum][tyokkei];

                            Revit.DB.Curve circle = cmpGeometry.CrvCircle(diameter, centerCircle);

                            cmpElements.NotNullCurveSet(ref crvAryStrct, circle);

                            // 躯体は寸法線をひくため、逐一作図
                            Collections.Generic.IList<Revit.DB.DetailCurve> dCrvs = new Collections.Generic.List<Revit.DB.DetailCurve>();
                            trans.Start("Write Struct");

                            foreach (Revit.DB.Curve crv in crvAryStrct)
                            {
                                Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                dc.LineStyle = bodyLineType;

                                dCrvs.Add(dc);
                            }

                            // 寸法線
                            cmpGeometry.CreateDimensionCircleBottom(dCrvs, dimType, vp);

                            trans.Commit();
                            crvAryStrct.Clear();

                            // 配筋
                            string strRet = cmpService.CreateRebar_En(enData, currentNum, centerCircle, true, rvtUiApp);

                            if (strRet != "")
                            {
                                writeErr += level + hugoName + " : " + "\r\n" + strRet + "\r\n";
                                strRet = level + hugoName + " : " + "\r\n" + strRet;

                                lineWidth = w_HugowakuMax / scale;

                                Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe, migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));

                                //                trans.Start("Error Message");
                                txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                //txtNote.Width = w_HugowakuMax;//
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //                trans.Commit();
                            }
                        }
                        #endregion

                        #region 異断面柱あり
                        if (isDifference == true)
                        {
                            hidariSita = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH) * 2, 0);
                            migiUe = hidariUe + new Revit.DB.XYZ(hugoMaxX + otherWidth, 0, 0);
                            migiSita = new Revit.DB.XYZ(migiUe.X, hidariSita.Y, migiSita.Z);

                            // 断面枠と配筋枠を作成
                            l = cmpElements.CreateBoundLine(hidariSita, migiSita);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            l = cmpElements.CreateBoundLine(migiUe, migiSita);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            l = cmpElements.CreateBoundLine(hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0), migiUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0));
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            // 主筋枠
                            Revit.DB.XYZ point1 = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                            Revit.DB.XYZ point2 = migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint1 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            Revit.DB.XYZ point3 = point1 + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0);
                            Revit.DB.XYZ point4 = point2 + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0);
                            l = cmpElements.CreateBoundLine(point3, point4);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint2 = cmpGeometry.Center2Point(point3, point4) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            // 芯鉄筋枠
                            Revit.DB.XYZ centerPoint_CoreRebar1 = null;
                            Revit.DB.XYZ centerPoint_CoreRebar2 = null;
                            if (isCoreRebar)
                            {
                                point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                centerPoint_CoreRebar1 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                                point3 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point4 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point3, point4);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                centerPoint_CoreRebar2 = cmpGeometry.Center2Point(point3, point4) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);
                            }

                            // 帯筋枠
                            point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint3 = cmpGeometry.Center2Point(point1, point2) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            point3 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            point4 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point3, point4);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                            Revit.DB.XYZ centerPoint4 = cmpGeometry.Center2Point(point3, point4) + new Revit.DB.XYZ(0, -(arrangementFrameHeight / 2 / 304.8 * scale), 0);

                            if (addFrameNumber > 0)
                            {
                                point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                point3 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point4 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point3, point4);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                for (int addFrameNum = 1; addFrameNum < addFrameNumber; ++addFrameNum)
                                {
                                    point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    l = cmpElements.CreateBoundLine(point1, point2);
                                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                    point3 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    point4 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                    l = cmpElements.CreateBoundLine(point3, point4);
                                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);
                                }
                            }

                            if (gaitou == false)
                            {
                                //斜線を作成
                                l = cmpElements.CreateBoundLine(migiUe, hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                l = cmpElements.CreateBoundLine(migiUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0),
                                                                hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0) + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0));
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                hidariUe = hidariSita;
                                continue;
                            }

                            // 鉄筋本数、径
                            #region
                            //              trans.Start("Rebar Number");

                            string title = "";
                            Revit.DB.XYZ baseVec = vp.RightDirection;
                            Revit.DB.XYZ upVec = vp.UpDirection;
                            double lineWidth = w_HugowakuMax / scale;
                            //Revit.DB.TextAlignFlags flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;
                            Revit.DB.TextNote txtNote = null;

                            // 主筋 - 柱頭
                            if ((int)enData.Rows[currentNum][chutoSyukinHonsu] > 0 && (string)enData.Rows[currentNum][chutoSyukinkei] != "")
                            {
                                title = (int)enData.Rows[currentNum][chutoSyukinHonsu] + "-" + (string)enData.Rows[currentNum][chutoSyukinkei];

                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            // 主筋 - 柱脚
                            if ((int)enData.Rows[currentNum][chukyakuSyukinHonsu] > 0 && (string)enData.Rows[currentNum][chukyakuSyukinkei] != "")
                            {
                                title = (int)enData.Rows[currentNum][chukyakuSyukinHonsu] + "-" + (string)enData.Rows[currentNum][chukyakuSyukinkei];

                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            // 芯鉄筋
                            if (isCoreRebar)
                            {
                                int sinNum = (int)enData.Rows[currentNum][sintekkinNumber];

                                if (sinNum > 0 && (string)enData.Rows[currentNum][sintekkinkei_en] != "")
                                {
                                    title = (int)enData.Rows[currentNum][sintekkinNumber] + "-" + (string)enData.Rows[currentNum][sintekkinkei_en];
                                    txtNote = cmpService.CreateNewTextNote(vp, centerPoint_CoreRebar1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();

                                    //txtNote.Width = w_HugowakuMax;// hugoMaxX;

                                    title = (int)enData.Rows[currentNum][sintekkinNumber] + "-" + (string)enData.Rows[currentNum][sintekkinkei_en];
                                    txtNote = cmpService.CreateNewTextNote(vp, centerPoint_CoreRebar2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();

                                    //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                                }
                            }

                            // 帯筋 - 柱頭
                            //if (hoopBracketShow == 0)
                            //{
                            //  title = (int)kakuData.Rows[currentNum][chutoHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chutoHoopYHonsu] + hoopSpaceSymbol + (string)kakuData.Rows[currentNum][chutoHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chutoHoopPitch];
                            //}
                            //else
                            //{
                            //  title = "[" + (int)kakuData.Rows[currentNum][chutoHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chutoHoopYHonsu] + "] " + (string)kakuData.Rows[currentNum][chutoHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chutoHoopPitch];
                            //}

                            if ((string)enData.Rows[currentNum][chutoHoopXkei_en] != "" && (double)enData.Rows[currentNum][chutoHoopPitch_en] != 0)
                            {
                                title = (string)enData.Rows[currentNum][chutoHoopXkei_en] + "@" + (double)enData.Rows[currentNum][chutoHoopPitch_en];

                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint3, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            // 帯筋 - 柱脚
                            //if (hoopBracketShow == 0)
                            //{
                            //  title = (int)kakuData.Rows[currentNum][chukyakuHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chukyakuHoopYHonsu] + hoopSpaceSymbol + (string)kakuData.Rows[currentNum][chukyakuHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chukyakuHoopPitch];
                            //}
                            //else
                            //{
                            //  title = "[" + (int)kakuData.Rows[currentNum][chukyakuHoopXHonsu] + hoopSpaceSymbol + (int)kakuData.Rows[currentNum][chukyakuHoopYHonsu] + "] " + (string)kakuData.Rows[currentNum][chukyakuHoopXkei] + "@" + (double)kakuData.Rows[currentNum][chukyakuHoopPitch];
                            //}
                            if ((string)enData.Rows[currentNum][chukyakuHoopXkei_en] != "" && (double)enData.Rows[currentNum][chukyakuHoopPitch_en] != 0)
                            {
                                title = (string)enData.Rows[currentNum][chukyakuHoopXkei_en] + "@" + (double)enData.Rows[currentNum][chukyakuHoopPitch_en];

                                txtNote = cmpService.CreateNewTextNote(vp, centerPoint4, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //txtNote.Width = w_HugowakuMax;// hugoMaxX;
                            }

                            //              trans.Commit();
                            #endregion

                            // 断面を作成
                            Revit.DB.XYZ centerCircle = cmpGeometry.Center2Point(hidariUe, migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                            centerCircle += new Revit.DB.XYZ((leftSpace - rightSpace) / 2 / 304.8 * scale, (bottomSpace - topSpace) / 2 / 304.8 * scale, 0);
                            double diameter = (double)enData.Rows[currentNum][tyokkei];

                            Revit.DB.Curve circle = cmpGeometry.CrvCircle(diameter, centerCircle);

                            cmpElements.NotNullCurveSet(ref crvAryStrct, circle);

                            // 躯体は寸法線をひくため、逐一作図
                            Collections.Generic.IList<Revit.DB.DetailCurve> dCrvs = new Collections.Generic.List<Revit.DB.DetailCurve>();
                            trans.Start("Write Struct");

                            foreach (Revit.DB.Curve crv in crvAryStrct)
                            {
                                Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                dc.LineStyle = bodyLineType;

                                dCrvs.Add(dc);
                            }

                            // 寸法線
                            cmpGeometry.CreateDimensionCircleBottom(dCrvs, dimType, vp);

                            trans.Commit();
                            crvAryStrct.Clear();
                            dCrvs.Clear();

                            // 配筋
                            string strRet = cmpService.CreateRebar_En(enData, currentNum, centerCircle, true, rvtUiApp);

                            if (strRet != "")
                            {
                                writeErr += level + hugoName + "(柱頭) : " + "\r\n" + strRet + "\r\n";
                                strRet = level + hugoName + "(柱頭) : " + "\r\n" + strRet;

                                lineWidth = w_HugowakuMax / scale;

                                Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe, migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));

                                //                trans.Start("Error Message");
                                txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                //txtNote.Width = w_HugowakuMax;
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //                trans.Commit();
                            }

                            centerCircle = cmpGeometry.Center2Point(hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0),
                                                                    migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0) + new Revit.DB.XYZ(0, -(levelMaxY + otherHeight), 0));
                            centerCircle += new Revit.DB.XYZ((leftSpace - rightSpace) / 2 / 304.8 * scale, (bottomSpace - topSpace) / 2 / 304.8 * scale, 0);

                            circle = cmpGeometry.CrvCircle(diameter, centerCircle);
                            cmpElements.NotNullCurveSet(ref crvAryStrct, circle);

                            trans.Start("Write Struct");

                            foreach (Revit.DB.Curve crv in crvAryStrct)
                            {
                                Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                dc.LineStyle = bodyLineType;

                                dCrvs.Add(dc);
                            }

                            // 寸法線
                            cmpGeometry.CreateDimensionCircleBottom(dCrvs, dimType, vp);

                            trans.Commit();
                            crvAryStrct.Clear();
                            dCrvs.Clear();

                            // 配筋
                            strRet = cmpService.CreateRebar_En(enData, currentNum, centerCircle, false, rvtUiApp);

                            if (strRet != "")
                            {
                                writeErr += level + hugoName + "(柱脚) : " + "\r\n" + strRet + "\r\n";
                                strRet = level + hugoName + "(柱脚) : " + "\r\n" + strRet;

                                lineWidth = w_HugowakuMax / scale;

                                Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0),
                                                                      migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0) + new Revit.DB.XYZ(0, -(levelMaxY + otherHeight), 0));

                                //                trans.Start("Error Message");
                                txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                //txtNote.Width = w_HugowakuMax;
                                trans.Start("Regenerate");
                                rvtDbDoc.Regenerate();
                                trans.Commit();

                                //                trans.Commit();
                            }
                        }
                        #endregion

                        // 左下を左上
                        hidariUe = hidariSita;
                    }

                    hugouHidariUe += new Revit.DB.XYZ(hugoMaxX + otherWidth, 0, 0);

                    hidariUe = new Revit.DB.XYZ(hugouHidariUe.X, hugouHidariUe.Y + h_Hugowaku, hugouHidariUe.Z);
                }
                #endregion

                foreach (Revit.DB.Curve crv in crvAryFrame)
                {
                    if (crv != null)
                    {
                        double lengthlength = crv.Length;
                    }
                }

                trans.Start("Draw");

                // 枠作図
                foreach (Revit.DB.Curve crv in crvAryFrame)
                {
                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                    dc.LineStyle = frameLineType;
                }

                trans.Commit();

                // 寄せ筋記号
                trans.Start("寄筋");
                foreach (Revit.DB.XYZ pnt in rebar2ndUe)
                {
                    Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance(pnt, famSymYoseUe, vp);
                }

                foreach (Revit.DB.XYZ pnt in rebar2ndHidari)
                {
                    Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance(pnt, famSymYoseHidari, vp);
                }

                foreach (Revit.DB.XYZ pnt in rebar2ndSita)
                {
                    Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance(pnt, famSymYoseSita, vp);
                }

                foreach (Revit.DB.XYZ pnt in rebar2ndMigi)
                {
                    Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance(pnt, famSymYoseMigi, vp);
                }
                trans.Commit();

                kaiHidariUe = new Revit.DB.XYZ(); // += new Revit.DB.XYZ(sumWidth, 0, 0);

                // ビューにフィット
                foreach (Revit.UI.UIView uiVIew in rvtUiDoc.GetOpenUIViews())
                {
                    if (uiVIew.ViewId.Value == rvtDbDoc.ActiveView.Id.Value)
                    {
                        uiVIew.ZoomToFit();

                        break;
                    }
                }
            }

            if (trans.GetStatus() == Revit.DB.TransactionStatus.Started)
            {
                trans.Commit();
            }

            //Save setting
            trans.Start("Save setting");
            if (paraSeting != null)
            {
                var new_Settings = string.Join(",", new_Setting.ToArray());
                paraSeting.Set(new_Settings);
            }
            trans.Commit();

            //Set name
            trans.Start("Set name");

            cmpElements.SetViewPlanName(dic_Views, byRange);
            trans.Commit();

            //Active view
            rvtUiDoc.ActiveView = activeView;
            foreach (KeyValuePair<int, List<ViewPlan>> keyPair in dic_Views)
            {
                foreach (ViewPlan vp in keyPair.Value)
                {
                    rvtUiDoc.ActiveView = vp;
                }
            }

            //if (dic_Views.Count == 0)
            //{
            //    writeErr = cmpAttribute.ResourceText("IDS_ERR_NO_VIEW_LIST");
            //}

            if (writeErr != "")
            {
                System.Windows.MessageBox.Show(writeErr, cmpAttribute.ResourceText("IDS_ERR_TITLE"));
            }

            retCmd = Revit.UI.Result.Succeeded;
            transGroup.Assimilate();
            return retCmd;
        }

        #endregion
    }
}