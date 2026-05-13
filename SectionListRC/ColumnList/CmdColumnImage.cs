using System;
using System.Linq;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using SectionListRC.Setting;
using System.Windows.Forms;
using SectionListRC.Utils;
using System.Collections.Generic;


namespace SectionListRC.ColumnList
{
    /// ================================================================================
    /// <summary>コマンド 柱イメージ</summary>
    /// ================================================================================
    [Revit.Attributes.Transaction(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.Regeneration(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdColumnImage : Revit.UI.IExternalCommand
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
        /// <history><p>2018/04/02 Created CST,Co.Ltd. Ryo Kuroda</p>
        ///           <p>2018/04/04 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        Revit.UI.Result Execute(Revit.UI.ExternalCommandData commandData,
                                ref string message,
                                Revit.DB.ElementSet elements)
        {
            // 戻り値
            Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;

            // 初期化
            Revit.UI.UIApplication rvtUiApp = commandData.Application;
            Revit.UI.UIDocument rvtUiDoc = rvtUiApp.ActiveUIDocument;
            Revit.DB.Document rvtDbDoc = rvtUiDoc.Document;
            Revit.ApplicationServices.Application rvtSvcApp = rvtDbDoc.Application;

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
            transGroup.Start(cmpAttribute.ResourceText("IDS_BTN_COLUMNLISTIMAGE_NAME"));

            // ファミリドキュメント
            if (rvtDbDoc.IsFamilyDocument)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_FAMILYDOCUMENT"));

                transGroup.Assimilate();
                return retCmd;
            }

            // プロジェクトブラウザ
            if (rvtDbDoc.ActiveView.ViewType == Revit.DB.ViewType.ProjectBrowser)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_ACTIVEPROJECTBROWSE"));

                transGroup.Assimilate();
                return retCmd;
            }

            // ワークフロー
            Revit.DB.Transaction trans = new Revit.DB.Transaction(rvtDbDoc);
            trans.Start("フロー");
            string retMsg1 = cmpService.WorkFlow(cmpAttribute.ResourceText("IDS_SHARE_PARA_COLUMN_RANGER"));
            string retMsg2 = cmpService.WorkFlow(cmpAttribute.ResourceText("IDS_SHPARAM_DEF"));
            trans.Commit();
            if (retMsg1 != null)
            {
                System.Windows.Forms.MessageBox.Show(retMsg1);
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
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTSTRPLANEVIEW"));
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
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETTINGFILE"));
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

                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETTINGOVERLAP") + "\r\n\r\n" + errMsg);
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

            cmpService.trans = trans;

            // 鉄筋ファミリ
            #region

            Revit.DB.Family rebarFam = null;
            bool isHaveFam = cmpElements.GetRebarFamily(ref rebarFam);

            if (isHaveFam == false)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOFAMILY"));
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

                        trans.Start("記号幅");
                        famSymYoseMigi.LookupParameter(paramHaba).Set(markSize);
                        trans.Commit();
                    }
                    else if (fs.Name == cmpAttribute.ResourceText("IDS_TXT_YOSEUE"))
                    {
                        famSymYoseUe = fs;

                        trans.Start("記号幅");
                        famSymYoseUe.LookupParameter(paramHaba).Set(markSize);
                        trans.Commit();
                    }
                    else if (fs.Name == cmpAttribute.ResourceText("IDS_TXT_YOSEHIDARI"))
                    {
                        famSymYoseHidari = fs;

                        trans.Start("記号幅");
                        famSymYoseHidari.LookupParameter(paramHaba).Set(markSize);
                        trans.Commit();
                    }
                    else if (fs.Name == cmpAttribute.ResourceText("IDS_TXT_YOSESITA"))
                    {
                        famSymYoseSita = fs;

                        trans.Start("記号幅");
                        famSymYoseSita.LookupParameter(paramHaba).Set(markSize);
                        trans.Commit();
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
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTARGETCOLUMN"));
                transGroup.Assimilate();
                return retCmd;
            }

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

            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.FamilySymbol>> aryAry = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.FamilySymbol>>();

            bool isAll = true;
            bool isColumnType = columnAry.Count != 0 ? true : false;
            bool isPostType = postAry.Count != 0 ? true : false;
            List<string> new_Setting = new List<string>();
            if (columnAry.Count == 0 && postAry.Count == 0)
            {
                transGroup.Assimilate();
                return retCmd;
            }
            else
            {
                //Show form
                FormColumnOption form = new FormColumnOption(cmpAttribute, true, settings, 2);
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

            Collections.Generic.IList<string> columnHugoAry_filter = null;
            Collections.Generic.IList<string> enHugoAry_filter = null;
            Collections.Generic.IList<string> allColumnLevelAry_filter = null;

            if (isAll == false && (isColumnType == true || isPostType == true))
            {
                var cAry = isColumnType ? columnAry : postAry;

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
                columnHugoAry_filter = cmpService.KakuCollumnMarkName(kakuData);

                // 円柱符号
                enHugoAry_filter = cmpService.EnCollumnMarkName(enData);

                // 全階 = 行数
                allColumnLevelAry_filter = cmpService.GetAllColumnLevelAry(cAry);
                Collections.Generic.IList<string> sorted = cmpService.LevelSortOrder_NameDESC(allColumnLevelAry_filter);
                // ソート済みに含まれない全柱を追加
                foreach (string str in allColumnLevelAry_filter)
                {
                    if (!sorted.Contains(str))
                    {
                        sorted.Add(str);
                    }
                }
                allColumnLevelAry_filter = sorted;

                int mode = isColumnType == true ? 0 : 1;
                //Show form
                FormColumnItemList formList = new FormColumnItemList(cmpAttribute, cmpParameters, settings, mode == 0 ? EnumType.Column : EnumType.Post, columnHugoAry_filter.ToList(),
                    enHugoAry_filter.ToList(), kakuData, enData, allColumnLevelAry_filter.ToList(), 2);

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

                columnHugoAry_filter = (from string column in columnHugoAry_filter
                                        where formList._SelectedColumns.Contains(column) == true
                                        select column).ToList();

                enHugoAry_filter = (from string column in enHugoAry_filter
                                    where formList._SelectedColumns.Contains(column) == true
                                    select column).ToList();

                allColumnLevelAry_filter = (from string level in allColumnLevelAry_filter
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

            if (isColumnType)
            {
                foreach (Revit.DB.FamilySymbol fs in columnAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);

                    aryAry.Add(fsAry);
                }
            }
            if (isPostType)
            {
                foreach (Revit.DB.FamilySymbol fs in postAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);

                    aryAry.Add(fsAry);
                }
            }

            //////////////////////////////////////////////////

            // 作図の原点
            Revit.DB.XYZ kaiHidariUe = new Revit.DB.XYZ();

            // エラーメッセージ
            string writeErr = "";

            // ビュー作成、表示
            Revit.DB.View current = null;

            string msg = cmpElements.SetCreateListView(listViewScale, ref current);

            if (msg != "")
            {
                System.Windows.Forms.MessageBox.Show(msg);

                transGroup.RollBack();
                return retCmd;
            }

            // 作図用ビュー
            Revit.DB.ViewPlan viewWork = rvtDbDoc.ActiveView as Revit.DB.ViewPlan;

            // 現在開いているビュー
            Collections.Generic.IList<Revit.UI.UIView> currentOpenViews = rvtUiApp.ActiveUIDocument.GetOpenUIViews();
            Revit.UI.UIView uiView = null;

            foreach (Revit.UI.UIView uiv in currentOpenViews)
            {
                if (viewWork.Id.Value == uiv.ViewId.Value)
                {
                    uiView = uiv;
                }
            }

            // 書き出しフォルダ
            string exportFolderPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string exportFolder = exportFolderPath + "\\柱";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && isColumnType)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            exportFolder = exportFolderPath + "\\間柱";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && isPostType)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            // プログレスバー
            SectionListRC.JExtComCompat.ProgressBarThread thread = new SectionListRC.JExtComCompat.ProgressBarThread(false, false);
            thread.ShowDialog();

            int count = 0;

            thread.SetData(cmpAttribute.ResourceText("IDS_TXT_IMAGECREATE"), aryAry.Count, count);
            thread.Active();

            // 出力

            bool isExported = false;

            Collections.Generic.IDictionary<long, string> dicTypeIdImagePath = cmpService.DicTypeId_ImagePath;

            try
            {
                foreach (Collections.Generic.IList<Revit.DB.FamilySymbol> cAry in aryAry)
                {
                    count += 1;
                    thread.SetData(cmpAttribute.ResourceText("IDS_TXT_IMAGECREATE"), aryAry.Count, count);
                    thread.Active();

                    // 斜線
                    bool isSyasen = false;

                    // 現在の合計枠幅
                    double sumWidth = 0;

                    if (cAry.Count < 1)
                    {
                        continue;
                    }

                    // タイプ名
                    string name = cAry[0].Name;
                    // 柱分類
                    Revit.DB.Parameter bunruiParam = cAry[0].LookupParameter(cmpParameters.HashiraBunrui_Kaku);
                    if (bunruiParam == null)
                    {
                        bunruiParam = cAry[0].LookupParameter(cmpParameters.Column_Category_En);
                    }

                    string bunrui = bunruiParam.AsString();
                    if (bunrui == cmpAttribute.ResourceText("IDS_TXT_COLUMN"))
                    {
                        exportFolder = exportFolderPath + "\\柱";
                    }
                    else if (bunrui == cmpAttribute.ResourceText("IDS_TXT_POST"))
                    {
                        exportFolder = exportFolderPath + "\\間柱";
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

                    // 全階 = 行数
                    var allColumnLevelAry = cmpService.GetAllColumnLevelAry(cAry);
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

                    // 角柱符号
                    Collections.Generic.IList<string> columnHugoAry = cmpService.KakuCollumnMarkName(kakuData);

                    // 円柱符号
                    Collections.Generic.IList<string> enHugoAry = cmpService.EnCollumnMarkName(enData);

                    if (allColumnLevelAry_filter != null && columnHugoAry_filter != null && enHugoAry_filter != null)
                    {
                        columnHugoAry = (from string column in columnHugoAry_filter
                                         where columnHugoAry.Contains(column) == true
                                         select column).ToList();

                        enHugoAry = (from string column in enHugoAry_filter
                                     where enHugoAry.Contains(column) == true
                                     select column).ToList();

                        allColumnLevelAry = (from string level in allColumnLevelAry_filter
                                             where allColumnLevelAry.Contains(level) == true
                                             select level).ToList();
                    }

                    if ((columnHugoAry.Count == 0 && enHugoAry.Count == 0) || allColumnLevelAry.Count == 0)
                        continue;

                    // 階別最大柱高さ
                    Collections.Generic.IList<double> columnHeightAry = cmpService.ColumnHeightByLevel(kakuData);
                    // 符号別最大柱幅
                    Collections.Generic.IList<double> columnWidthAry = cmpService.ColumnWidthByMark(kakuData);
                    // 階別最大円柱径
                    Collections.Generic.IList<double> columnDiaLevelAry = cmpService.ColumnDiameterByLevel(enData);
                    // 符号別最大円柱径
                    Collections.Generic.IList<double> columnDiaMarkAry = cmpService.ColumnDiameterByMark(enData);

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
                    int scale = viewWork.Scale;

                    // 枠の大きさは印刷後の実寸なので、入力値にビューの尺度をかける。
                    // 入力値(mm) ÷ 304.8(フィート化) × 尺度

                    // 枠高さ
                    // -Y方向に作図していくので
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
                    Collections.Generic.IList<Revit.DB.XYZ> rebar2ndHidari = new Collections.Generic.List<Revit.DB.XYZ>();
                    Collections.Generic.IList<Revit.DB.XYZ> rebar2ndSita = new Collections.Generic.List<Revit.DB.XYZ>();
                    Collections.Generic.IList<Revit.DB.XYZ> rebar2ndMigi = new Collections.Generic.List<Revit.DB.XYZ>();

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

                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(kaiHidariUe.X, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z), kaiHidariUe + new Revit.DB.XYZ(0, 0, 0));
                    cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                    //        trans.Start("Text");

                    if (kaihyoji == true)
                    {
                        l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y, kaiHidariUe.Z),
                                                        new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                        cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                        Revit.DB.XYZ origin = cmpGeometry.Center2Point(kaiHidariUe, new Revit.DB.XYZ(koumokuHidariUe.X, koumokuHidariUe.Y + h_Hugowaku, koumokuHidariUe.Z));// new Revit.DB.XYZ(kaiHidariUe.X + w1 / 2, kaiHidariUe.Y + h1 / 2, kaiHidariUe.Z);
                                                                                                                                                                            // 現在ビューの各方向
                        Revit.DB.XYZ baseVec = viewWork.RightDirection;
                        Revit.DB.XYZ upVec = viewWork.UpDirection;

                        double lineWidth = w_Kaihyojiwaku / scale;

                        string str = lvlFrameTitle;

                        Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, str, rvtDbDoc);

                        trans.Start("Regenerate");
                        rvtDbDoc.Regenerate();
                        trans.Commit();

                        origin = cmpGeometry.Center2Point(koumokuHidariUe, new Revit.DB.XYZ(hugouHidariUe.X, hugouHidariUe.Y + h_Hugowaku, hugouHidariUe.Z));// new Revit.DB.XYZ(kaiHidariUe.X + w1 + w2 / 2, kaiHidariUe.Y + h1 / 2, kaiHidariUe.Z);
                        lineWidth = w_Komokuwaku / scale;
                        str = symbolFrameTitle;

                        txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, str, rvtDbDoc);
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
                        Revit.DB.XYZ baseVec = viewWork.RightDirection;
                        Revit.DB.XYZ upVec = viewWork.UpDirection;

                        double lineWidth = w_Komokuwaku / scale;

                        string str = lvlFrameTitle;

                        Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, str, rvtDbDoc);

                        trans.Start("Regenerate");
                        rvtDbDoc.Regenerate();
                        trans.Commit();

                        origin = cmpGeometry.TriangleGravity2D(kaiHidariUe, new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y, kaiHidariUe.Z), new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));  //new Revit.DB.XYZ(p0.X + w2, p0.Y, p0.Z);
                        lineWidth = w_Komokuwaku / scale;
                        str = symbolFrameTitle;

                        txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, str, rvtDbDoc);

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
                            title += "\r\n" + cmpAttribute.ResourceText("IDS_TXT_TYPENAME") + "「" + overlapnames + "」" + cmpAttribute.ResourceText("IDS_ERR_INDIFFERENTFAMILY");
                        }

                        Revit.DB.XYZ origin = cmpGeometry.Center2Point(px, new Revit.DB.XYZ(wx, px.Y + h_Hugowaku, px.Z));
                        Revit.DB.XYZ baseVec = viewWork.RightDirection;
                        Revit.DB.XYZ upVec = viewWork.UpDirection;

                        double lineWidth = w_HugowakuMax / scale;

                        Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

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
                        Revit.DB.XYZ baseVec = viewWork.RightDirection;
                        Revit.DB.XYZ upVec = viewWork.UpDirection;

                        double lineWidth = w_HugowakuMax / scale;

                        Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

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
                                baseVec = viewWork.RightDirection;
                                upVec = viewWork.UpDirection;

                                lineWidth = w_Kaihyojiwaku / scale;

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                                origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));

                                // 断面
                                title = "断面";
                                baseVec = viewWork.RightDirection;
                                upVec = viewWork.UpDirection;

                                lineWidth = w_Komokuwaku / scale;

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);
                            }
                            else
                            {
                                origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));

                                // 階表示文字
                                title = allColumnLevelAry[i_h] + lvlFrameEndWord;
                                baseVec = viewWork.RightDirection;
                                upVec = viewWork.UpDirection;

                                lineWidth = w_Komokuwaku / scale;

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);
                            }

                            // ----- 主筋 -----
                            Revit.DB.XYZ point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                            Revit.DB.XYZ point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight) / 304.8 * scale), p1.Z));
                            title = "主筋";
                            baseVec = viewWork.RightDirection;
                            upVec = viewWork.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

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
                                baseVec = viewWork.RightDirection;
                                upVec = viewWork.UpDirection;

                                lineWidth = w_Komokuwaku / scale;

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                wakuNum += 1;
                            }

                            // ----- 帯筋 -----
                            point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + 1)) / 304.8 * scale), p1.Z));
                            title = hoopFrameTitle;
                            baseVec = viewWork.RightDirection;
                            upVec = viewWork.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

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
                                baseVec = viewWork.RightDirection;
                                upVec = viewWork.UpDirection;

                                lineWidth = w_Kaihyojiwaku / scale;

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                                origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));

                                // 断面
                                title = "断面" + "\r\n\r\n" + "柱頭";
                                baseVec = viewWork.RightDirection;
                                upVec = viewWork.UpDirection;

                                lineWidth = w_Komokuwaku / scale;

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                                origin += new Revit.DB.XYZ(0, h / 2, 0);

                                title = "断面" + "\r\n\r\n" + "柱脚";

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);
                            }
                            else
                            {
                                origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));

                                // 断面
                                title = allColumnLevelAry[i_h] + lvlFrameEndWord + "\r\n\r\n" + "柱頭";
                                baseVec = viewWork.RightDirection;
                                upVec = viewWork.UpDirection;

                                lineWidth = w_Komokuwaku / scale;

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                                origin += new Revit.DB.XYZ(0, h / 2, 0);
                                title = allColumnLevelAry[i_h] + lvlFrameEndWord + "\r\n\r\n" + "柱脚";

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);
                            }

                            // ----- 主筋 -----
                            Revit.DB.XYZ point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                            Revit.DB.XYZ point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight) / 304.8 * scale), p1.Z));
                            title = "主筋";
                            baseVec = viewWork.RightDirection;
                            upVec = viewWork.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                            int wakuNum = 1;

                            point1 += new Revit.DB.XYZ(0, h / 2, 0);
                            point2 += new Revit.DB.XYZ(0, h / 2, 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin += new Revit.DB.XYZ(0, h / 2, 0);

                            txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                            // ----- 芯鉄筋 -----
                            if (isCoreRebar)
                            {
                                point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                                point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + 1)) / 304.8 * scale), p1.Z));
                                title = coreRebarTitle;
                                baseVec = viewWork.RightDirection;
                                upVec = viewWork.UpDirection;

                                lineWidth = w_Komokuwaku / scale;

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                wakuNum += 1;

                                point1 += new Revit.DB.XYZ(0, h / 2, 0);
                                point2 += new Revit.DB.XYZ(0, h / 2, 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                                origin += new Revit.DB.XYZ(0, h / 2, 0);

                                txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);
                            }

                            // ----- 帯筋 -----
                            point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            point2 = new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * wakuNum) / 304.8 * scale), p1.Z);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * (wakuNum + 1)) / 304.8 * scale), p1.Z));
                            title = hoopFrameTitle;
                            baseVec = viewWork.RightDirection;
                            upVec = viewWork.UpDirection;

                            lineWidth = w_Komokuwaku / scale;

                            txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                            wakuNum += 1;

                            point1 += new Revit.DB.XYZ(0, h / 2, 0);
                            point2 += new Revit.DB.XYZ(0, h / 2, 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvAryFrame, l);

                            origin += new Revit.DB.XYZ(0, h / 2, 0);

                            txtNote = cmpService.CreateNewTextNote(viewWork, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

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
                        string currentHugoName = (string)kakuData.Rows[0][cmpParameters.RST_HasiraHugo_Kaku];
                        string currentLevel = (string)kakuData.Rows[0][cmpParameters.LevelFrameTitle];

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

                                    isSyasen = true;

                                    continue;
                                }

                                // 主筋本数、径、(芯鉄筋本数、径)、帯筋本数、径
                                #region

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
                                    (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)
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

                                Revit.DB.XYZ baseVec = viewWork.RightDirection;
                                Revit.DB.XYZ upVec = viewWork.UpDirection;

                                double lineWidth = w_HugowakuMax / scale;

                                Revit.DB.TextNote txtNote = null;

                                // 柱頭主筋エラー
                                bool isSyukinError = false;

                                #region 1段筋

                                // 太径が2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 2 || (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }
                                if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                                {
                                    // 細径が太径本数以上
                                    // X
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] >= 1)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                    // Y
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] >= 1)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
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
                                            if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
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

                                    if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                                    {
                                        // 同一方向2段筋太径本以上
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
                                            if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
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

                                    if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                                    {
                                        // 同一方向2段筋太径本以上
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
                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
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
                                            txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint_CoreRebar, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();
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
                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

                                #endregion

                                // 断面を作成
                                Revit.DB.XYZ centerRectangle = cmpGeometry.Center2Point(hidariUe, migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                                centerRectangle += new Revit.DB.XYZ((leftSpace - rightSpace) / 2 / 304.8 * scale, (bottomSpace - topSpace) / 2 / 304.8 * scale, 0);
                                double dx = (double)kakuData.Rows[currentNum][cmpParameters.DX_Kaku];
                                double dy = (double)kakuData.Rows[currentNum][cmpParameters.DY_Kaku];

                                Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs = cmpGeometry.CrvRectangle(dx, dy, centerRectangle);
                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(dx, dy, centerRectangle);

                                cmpElements.AddCrvByAry(ref crvAryStrct, rectangleCrvs);

                                // 躯体は寸法線をひくため、逐一作図
                                Collections.Generic.IList<Revit.DB.DetailLine> dLines = new Collections.Generic.List<Revit.DB.DetailLine>();
                                trans.Start("Write Struct");

                                foreach (Revit.DB.Curve crv in crvAryStrct)
                                {
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(viewWork, crv);
                                    dc.LineStyle = bodyLineType;
                                    dLines.Add(dc as Revit.DB.DetailLine);
                                }

                                // 寸法線
                                cmpGeometry.CreateDimensionsRectangleBottomLeft(dLines, dimType, viewWork);

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

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
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

                                    double offset = viewWork.Scale / 304.8;
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

                                        int x1Num = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                                        int x2Num = (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu];
                                        title = x1Num.ToString();

                                        if (x2Num > 0)
                                        {
                                            title += "+" + x2Num.ToString();

                                            txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        }

                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

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
                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

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
                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

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
                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

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

                                        int x1Num = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                                        int x2Num = (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu];
                                        title = x1Num.ToString();

                                        if (x2Num > 0)
                                        {
                                            title += "+" + x2Num.ToString();
                                        }

                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtNote.Width = w_Komokuwaku / 2;

                                        // Y段筋
                                        txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleRightTop) + new Revit.DB.XYZ(0, kaburi_kaku / 304.8, 0);

                                        int y1Num = (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu];
                                        int y2Num = (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu];
                                        title = y1Num.ToString();

                                        if (y2Num > 0)
                                        {
                                            title += "+" + y2Num.ToString();
                                        }

                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

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

                                    isSyasen = true;

                                    continue;
                                }

                                // 鉄筋本数、径
                                #region

                                // 主筋 - 柱頭
                                // 主筋太径
                                // 四隅は重複している

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
                                    (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] >= 4)
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

                                Revit.DB.XYZ baseVec = viewWork.RightDirection;
                                Revit.DB.XYZ upVec = viewWork.UpDirection;

                                double lineWidth = w_HugowakuMax / scale;

                                Revit.DB.TextNote txtNote = null;

                                // 柱頭主筋エラー
                                bool isSyukinError = false;

                                #region 1段筋

                                // 太径が2本未満
                                if ((int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] < 2 || (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] < 2)
                                {
                                    isSyukinError = true;
                                }
                                if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                                {
                                    // 細径が太径本数以上
                                    // X
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] >= 1)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                    // Y
                                    if ((int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] >= 1)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
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
                                            if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
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

                                    if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                                    {
                                        // 同一方向2段筋太径本以上
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
                                            if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
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
                                    if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                                    {
                                        // 同一方向2段筋太径本以上
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
                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

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

                                // 主筋 - 柱脚
                                // 主筋太径
                                // 四隅は重複している

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
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] >= 4 && (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] >= 4)
                                    {
                                        rebarCount -= 4;
                                    }
                                }
                                // 2段筋コーナー
                                if (secondConrnerSetFlag == 1 &&
                                    (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] >= 4 &&
                                    (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] >= 4)
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

                                if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                                {
                                    // 細径が太径本数以上
                                    // X
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] >= 1)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
                                    // Y
                                    if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu] >= 1)
                                    {
                                        if ((int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu] >= (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu])
                                        {
                                            isSyukinError = true;
                                        }
                                    }
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
                                            if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
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

                                    if (X1HosoLessX1Huto = true && Y1HosoLessY1Huto == true)
                                    {
                                        // 同一方向2段筋太径本以上
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
                                            if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
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

                                    if (X1HosoLessX1Huto == true && Y1HosoLessY1Huto == true)
                                    {
                                        // 同一方向2段筋太径本以上
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
                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
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
                                            txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint_CoreRebar1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint_CoreRebar2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();
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
                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint3, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
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
                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint4, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

                                #endregion

                                // 断面を作成
                                #region 柱頭

                                Revit.DB.XYZ centerRectangle = cmpGeometry.Center2Point(migiUe, hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                                centerRectangle += new Revit.DB.XYZ((leftSpace - rightSpace) / 2 / 304.8 * scale, (bottomSpace - topSpace) / 2 / 304.8 * scale, 0);
                                double dx = (double)kakuData.Rows[currentNum][cmpParameters.DX_Kaku];
                                double dy = (double)kakuData.Rows[currentNum][cmpParameters.DY_Kaku];

                                Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs = cmpGeometry.CrvRectangle(dx, dy, centerRectangle);
                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(dx, dy, centerRectangle);

                                cmpElements.AddCrvByAry(ref crvAryStrct, rectangleCrvs);

                                // 躯体は寸法線をひくため、逐一作図
                                Collections.Generic.IList<Revit.DB.DetailLine> dLines = new Collections.Generic.List<Revit.DB.DetailLine>();
                                trans.Start("Write Struct");

                                foreach (Revit.DB.Curve crv in crvAryStrct)
                                {
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(viewWork, crv);
                                    dc.LineStyle = bodyLineType;
                                    dLines.Add(dc as Revit.DB.DetailLine);
                                }

                                // 寸法線
                                cmpGeometry.CreateDimensionsRectangleBottomLeft(dLines, dimType, viewWork);

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

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
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

                                    double offset = viewWork.Scale / 304.8;
                                    Revit.DB.XYZ verticVec = new Revit.DB.XYZ(0, 1, 0);
                                    lineWidth = w_Komokuwaku / scale;
                                    // 本数を分割表示
                                    if (rebarShow == 1)
                                    {
                                        #region

                                        // X段筋左側
                                        double diaSyukin = cmpService.SyukinDiameter;

                                        Revit.DB.XYZ txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + diaSyukin / 2, kaburi_kaku / 304.8, 0);

                                        int x1Num = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                                        int x2Num = (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu];
                                        title = x1Num.ToString();

                                        if (x2Num > 0)
                                        {
                                            title += "+" + x2Num.ToString();

                                            txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        }

                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                        // X段筋右側
                                        title = x1Num.ToString();
                                        txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - diaSyukin / 2, kaburi_kaku / 304.8, 0);
                                        if (x2Num > 0)
                                        {
                                            title = x2Num.ToString() + "+" + title;

                                            txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        }
                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

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
                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                        // Y段筋上側
                                        title = y1Num.ToString();
                                        txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - diaSyukin / 2, 0);
                                        if (y2Num > 0)
                                        {
                                            title = y2Num.ToString() + "+" + title;

                                            txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, -kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, 0);
                                        }
                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                        #endregion
                                    }
                                    // 本数をまとめて表示
                                    else if (rebarShow == 2)
                                    {
                                        #region

                                        // X段筋
                                        Revit.DB.XYZ txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleLeftBottom) + new Revit.DB.XYZ(-kaburi_kaku / 304.8, 0, 0);

                                        int x1Num = (int)kakuData.Rows[currentNum][chutoSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX1HosokeiHonsu];
                                        int x2Num = (int)kakuData.Rows[currentNum][chutoSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinX2HosokeiHonsu];
                                        title = x1Num.ToString();

                                        if (x2Num > 0)
                                        {
                                            title += "+" + x2Num.ToString();
                                        }

                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                        // Y段筋
                                        txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleRightTop) + new Revit.DB.XYZ(0, kaburi_kaku / 304.8, 0);

                                        int y1Num = (int)kakuData.Rows[currentNum][chutoSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY1HosokeiHonsu];
                                        int y2Num = (int)kakuData.Rows[currentNum][chutoSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chutoSyukinY2HosokeiHonsu];
                                        title = y1Num.ToString();

                                        if (y2Num > 0)
                                        {
                                            title += "+" + y2Num.ToString();
                                        }

                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

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
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(viewWork, crv);
                                    dc.LineStyle = bodyLineType;
                                    dLines.Add(dc as Revit.DB.DetailLine);
                                }

                                // 寸法線
                                cmpGeometry.CreateDimensionsRectangleBottomLeft(dLines, dimType, viewWork);

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

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
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

                                    double offset = viewWork.Scale / 304.8;
                                    Revit.DB.XYZ verticVec = new Revit.DB.XYZ(0, 1, 0);
                                    lineWidth = w_Komokuwaku / scale;
                                    // 本数を分割表示
                                    if (rebarShow == 1)
                                    {
                                        #region

                                        // X段筋左側
                                        double diaSyukin = cmpService.SyukinDiameter;

                                        Revit.DB.XYZ txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + diaSyukin / 2, kaburi_kaku / 304.8, 0);

                                        int x1Num = (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu];
                                        int x2Num = (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu];
                                        title = x1Num.ToString();

                                        if (x2Num > 0)
                                        {
                                            title += "+" + x2Num.ToString();

                                            txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        }

                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

                                        // X段筋右側
                                        title = x1Num.ToString();
                                        txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - diaSyukin / 2, kaburi_kaku / 304.8, 0);
                                        if (x2Num > 0)
                                        {
                                            title = x2Num.ToString() + "+" + title;

                                            txtOrigin = rectangleRightTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - x2ndSymbolDiameter / 2, kaburi_kaku / 304.8 + x2ndSymbolDiameter / 2, 0);
                                        }
                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

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
                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                        // Y段筋上側
                                        title = y1Num.ToString();
                                        txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8, -kaburi_kaku / 304.8 - diaSyukin / 2, 0);
                                        if (y2Num > 0)
                                        {
                                            title = y2Num.ToString() + "+" + title;
                                            txtOrigin = rectangleLeftTop + new Revit.DB.XYZ(-kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, -kaburi_kaku / 304.8 - y2ndSymbolDiameter / 2, 0);
                                        }
                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                        #endregion
                                    }
                                    // 本数をまとめて表示
                                    else if (rebarShow == 2)
                                    {
                                        #region

                                        // X段筋
                                        Revit.DB.XYZ txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleLeftBottom) + new Revit.DB.XYZ(-kaburi_kaku / 304.8, 0, 0);

                                        int x1Num = (int)kakuData.Rows[currentNum][chukyakuSyukinX1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX1HosokeiHonsu];
                                        int x2Num = (int)kakuData.Rows[currentNum][chukyakuSyukinX2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinX2HosokeiHonsu];
                                        title = x1Num.ToString();

                                        if (x2Num > 0)
                                        {
                                            title += "+" + x2Num.ToString();
                                        }

                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, verticVec, lineWidth, otherTNT.Id, title, rvtDbDoc, -offset);

                                        // Y段筋
                                        txtOrigin = cmpGeometry.Center2Point(rectangleLeftTop, rectangleRightTop) + new Revit.DB.XYZ(0, kaburi_kaku / 304.8, 0);

                                        int y1Num = (int)kakuData.Rows[currentNum][chukyakuSyukinY1HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY1HosokeiHonsu];
                                        int y2Num = (int)kakuData.Rows[currentNum][chukyakuSyukinY2HutokeiHonsu] + (int)kakuData.Rows[currentNum][chukyakuSyukinY2HosokeiHonsu];
                                        title = y1Num.ToString();

                                        if (y2Num > 0)
                                        {
                                            title += "+" + y2Num.ToString();
                                        }

                                        txtNote = cmpService.CreateNewTextNote_Offset(viewWork, txtOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc, offset);

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

                                    isSyasen = true;

                                    continue;
                                }

                                // 主筋本数、径、(芯鉄筋本数、径)、帯筋本数、径
                                #region
                                //              trans.Start("Rebar Number");

                                string title = "";
                                Revit.DB.TextNote txtNote = null;
                                Revit.DB.XYZ baseVec = viewWork.RightDirection;
                                Revit.DB.XYZ upVec = viewWork.UpDirection;
                                double lineWidth = w_HugowakuMax / scale;

                                if ((int)enData.Rows[currentNum][chutoSyukinHonsu] > 0 && (string)enData.Rows[currentNum][chutoSyukinkei] != "")
                                {
                                    title = (int)enData.Rows[currentNum][chutoSyukinHonsu] + "-" + (string)enData.Rows[currentNum][chutoSyukinkei];

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

                                if (isCoreRebar)
                                {
                                    int sinNum = (int)enData.Rows[currentNum][sintekkinNumber];

                                    if (sinNum > 0 && (string)enData.Rows[currentNum][sintekkinkei_en] != "")
                                    {
                                        title = (int)enData.Rows[currentNum][sintekkinNumber] + "-" + (string)enData.Rows[currentNum][sintekkinkei_en];
                                        txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint_CoreRebar, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();
                                    }
                                }

                                title = (string)enData.Rows[currentNum][chutoHoopXkei_en] + "@" + (double)enData.Rows[currentNum][chutoHoopPitch_en];

                                if ((string)enData.Rows[currentNum][chutoHoopXkei_en] != "" && (double)enData.Rows[currentNum][chutoHoopPitch_en] != 0)
                                {
                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

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
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(viewWork, crv);
                                    dc.LineStyle = bodyLineType;

                                    dCrvs.Add(dc);
                                }

                                // 寸法線
                                cmpGeometry.CreateDimensionCircleBottom(dCrvs, dimType, viewWork);

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

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
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

                                    isSyasen = true;

                                    continue;
                                }

                                // 鉄筋本数、径
                                #region

                                string title = "";
                                Revit.DB.XYZ baseVec = viewWork.RightDirection;
                                Revit.DB.XYZ upVec = viewWork.UpDirection;
                                double lineWidth = w_HugowakuMax / scale;
                                Revit.DB.TextNote txtNote = null;

                                // 主筋 - 柱頭
                                if ((int)enData.Rows[currentNum][chutoSyukinHonsu] > 0 && (string)enData.Rows[currentNum][chutoSyukinkei] != "")
                                {
                                    title = (int)enData.Rows[currentNum][chutoSyukinHonsu] + "-" + (string)enData.Rows[currentNum][chutoSyukinkei];

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

                                // 主筋 - 柱脚
                                if ((int)enData.Rows[currentNum][chukyakuSyukinHonsu] > 0 && (string)enData.Rows[currentNum][chukyakuSyukinkei] != "")
                                {
                                    title = (int)enData.Rows[currentNum][chukyakuSyukinHonsu] + "-" + (string)enData.Rows[currentNum][chukyakuSyukinkei];

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

                                // 芯鉄筋
                                if (isCoreRebar)
                                {
                                    int sinNum = (int)enData.Rows[currentNum][sintekkinNumber];

                                    if (sinNum > 0 && (string)enData.Rows[currentNum][sintekkinkei_en] != "")
                                    {
                                        title = (int)enData.Rows[currentNum][sintekkinNumber] + "-" + (string)enData.Rows[currentNum][sintekkinkei_en];
                                        txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint_CoreRebar1, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();

                                        title = (int)enData.Rows[currentNum][sintekkinNumber] + "-" + (string)enData.Rows[currentNum][sintekkinkei_en];
                                        txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint_CoreRebar2, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();
                                    }
                                }

                                // 帯筋 - 柱頭
                                if ((string)enData.Rows[currentNum][chutoHoopXkei_en] != "" && (double)enData.Rows[currentNum][chutoHoopPitch_en] != 0)
                                {
                                    title = (string)enData.Rows[currentNum][chutoHoopXkei_en] + "@" + (double)enData.Rows[currentNum][chutoHoopPitch_en];

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint3, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

                                // 帯筋 - 柱脚
                                if ((string)enData.Rows[currentNum][chukyakuHoopXkei_en] != "" && (double)enData.Rows[currentNum][chukyakuHoopPitch_en] != 0)
                                {
                                    title = (string)enData.Rows[currentNum][chukyakuHoopXkei_en] + "@" + (double)enData.Rows[currentNum][chukyakuHoopPitch_en];

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerPoint4, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

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
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(viewWork, crv);
                                    dc.LineStyle = bodyLineType;

                                    dCrvs.Add(dc);
                                }

                                // 寸法線
                                cmpGeometry.CreateDimensionCircleBottom(dCrvs, dimType, viewWork);

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

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                }

                                centerCircle = cmpGeometry.Center2Point(hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherH), 0),
                                                                        migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0) + new Revit.DB.XYZ(0, -(levelMaxY + otherHeight), 0));
                                centerCircle += new Revit.DB.XYZ((leftSpace - rightSpace) / 2 / 304.8 * scale, (bottomSpace - topSpace) / 2 / 304.8 * scale, 0);

                                circle = cmpGeometry.CrvCircle(diameter, centerCircle);
                                cmpElements.NotNullCurveSet(ref crvAryStrct, circle);

                                trans.Start("Write Struct");

                                foreach (Revit.DB.Curve crv in crvAryStrct)
                                {
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(viewWork, crv);
                                    dc.LineStyle = bodyLineType;

                                    dCrvs.Add(dc);
                                }

                                // 寸法線
                                cmpGeometry.CreateDimensionCircleBottom(dCrvs, dimType, viewWork);

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

                                    txtNote = cmpService.CreateNewTextNote(viewWork, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
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
                        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(viewWork, crv);
                        dc.LineStyle = frameLineType;
                    }

                    trans.Commit();

                    // 寄せ筋記号
                    trans.Start("寄筋");
                    foreach (Revit.DB.XYZ pnt in rebar2ndUe)
                    {
                        Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance(pnt, famSymYoseUe, viewWork);
                    }

                    foreach (Revit.DB.XYZ pnt in rebar2ndHidari)
                    {
                        Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance(pnt, famSymYoseHidari, viewWork);
                    }

                    foreach (Revit.DB.XYZ pnt in rebar2ndSita)
                    {
                        Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance(pnt, famSymYoseSita, viewWork);
                    }

                    foreach (Revit.DB.XYZ pnt in rebar2ndMigi)
                    {
                        Revit.DB.FamilyInstance famInsRebar = rvtDbDoc.Create.NewFamilyInstance(pnt, famSymYoseMigi, viewWork);
                    }
                    trans.Commit();

                    kaiHidariUe = new Revit.DB.XYZ();

                    // ビューの要素
                    Revit.DB.FilteredElementCollector filterCollector = new Revit.DB.FilteredElementCollector(rvtDbDoc, viewWork.Id);
                    filterCollector.WhereElementIsNotElementType();

                    Collections.Generic.ICollection<Revit.DB.ElementId> eIds = new Collections.Generic.List<Revit.DB.ElementId>();

                    Collections.Generic.IList<Revit.DB.DetailCurve> dcAry = new Collections.Generic.List<Revit.DB.DetailCurve>();

                    foreach (Revit.DB.Element e in filterCollector)
                    {
                        // 非表示は除外
                        if (e.IsHidden(viewWork))
                        {
                            continue;
                        }

                        // 詳細線分
                        Revit.DB.DetailCurve dc = e as Revit.DB.DetailCurve;
                        if (dc != null && dc.GeometryCurve.IsBound)
                        {
                            dcAry.Add(dc);
                        }

                        // ファミリ
                        Revit.DB.FamilyInstance famIns = e as Revit.DB.FamilyInstance;

                        // 文字
                        Revit.DB.TextNote txtNote = e as Revit.DB.TextNote;

                        if (dc != null || famIns != null || txtNote != null)
                        {
                            eIds.Add(e.Id);
                        }
                    }

                    // 断面なし
                    if (isSyasen)
                    {
                        // 作図したものを削除
                        trans.Start("Delete");
                        rvtDbDoc.Delete(eIds);
                        trans.Commit();

                        continue;
                    }

                    // 枠オフセット
                    // Revitのイメージ書き出しは範囲をフィットさせると、僅かに収まらない
                    Collections.Generic.IList<Revit.DB.ElementId> frameOffset = cmpService.FrameOffset(dcAry,
                                                                                                       frameLineType,
                                                                                                       viewWork,
                                                                                                       trans);

                    eIds.Add(frameOffset[0]);
                    eIds.Add(frameOffset[1]);

                    // リスト要素のみ表示
                    rvtUiDoc.ShowElements(eIds);

                    // 書き出し

                    // 画像形式設定
                    Revit.DB.ImageExportOptions imgExprtOpt = new Revit.DB.ImageExportOptions();

                    // ファイル名使用禁止文字の置換
                    char[] invalidChars = System.IO.Path.GetInvalidFileNameChars();
                    if (name.IndexOfAny(invalidChars) >= 0)
                    {
                        foreach (char ch in invalidChars)
                        {
                            name = name.Replace(ch, '_');
                        }
                    }

                    // Revitの画像出力機能では途中にドットがあるばあい
                    // ドットの前までしかファイル名に使われない
                    name = name.Replace('.', '_');

                    imgExprtOpt.FilePath = exportFolder + "\\" + name;

                    // 書き出しビュー名
                    imgExprtOpt.ViewName = viewWork.Name;

                    // ファイル形式
                    imgExprtOpt.HLRandWFViewsFileType = Revit.DB.ImageFileType.PNG;

                    // 書き出し範囲 - 現在の表示範囲
                    //imgExprtOpt.ExportRange = Revit.DB.ExportRange.CurrentView;

                    // ズームタイプ
                    imgExprtOpt.ZoomType = Revit.DB.ZoomFitType.FitToPage;

                    // ピクセルサイズ(最低値)
                    imgExprtOpt.PixelSize = 1000;

                    // 横方向を上記ピクセルサイズ
                    imgExprtOpt.FitDirection = Revit.DB.FitDirectionType.Horizontal;

                    // 画像出力
                    rvtDbDoc.ExportImage(imgExprtOpt);

                    // 作図したものを削除
                    trans.Start("Delete");
                    rvtDbDoc.Delete(eIds);
                    trans.Commit();

                    isExported = true;

                    var id = cAry[0].Id.Value;
                    string path = exportFolder + "\\" + name + ".png";

                    if (dicTypeIdImagePath.ContainsKey(id))
                    {
                        dicTypeIdImagePath[id] = path;
                    }
                    else
                    {
                        dicTypeIdImagePath.Add(id, path);
                    }
                }
            }
            catch
            {
                if (trans.GetStatus() != Revit.DB.TransactionStatus.Committed)
                {
                    trans.Commit();
                }

                thread.Close();

                System.Windows.Forms.MessageBox.Show("作図に失敗しました");

                transGroup.RollBack();
                return retCmd;
            }

            cmpService.DicTypeId_ImagePath = dicTypeIdImagePath;

            // イメージ設定
            msg = cmpService.SetImageAsFamily(thread);
            thread.Close();

            if (msg != "")
            {
                System.Windows.Forms.MessageBox.Show(msg);

                transGroup.RollBack();
                return retCmd;
            }

            // 書出しフォルダ削除
            msg = cmpService.DeleteExportFolder(exportFolderPath);

            if (msg != "")
            {
                System.Windows.Forms.MessageBox.Show(msg);

                transGroup.RollBack();
                return retCmd;
            }

            // 実行時のビューに戻す
            rvtUiDoc.ActiveView = current;

            //Save setting
            trans.Start("Save setting");
            if (paraSeting != null)
            {
                var new_Settings = string.Join(",", new_Setting.ToArray());
                paraSeting.Set(new_Settings);
            }
            trans.Commit();

            // 作図に使用したビューを削除
            trans.Start("Delete");
            msg = cmpElements.DeleteListVIew(viewWork);

            trans.Commit();

            if (msg != "")
            {
                System.Windows.Forms.MessageBox.Show(msg);

                transGroup.RollBack();
                return retCmd;
            }

            if (writeErr != "")
            {
                System.Windows.Forms.MessageBox.Show(writeErr, cmpAttribute.ResourceText("IDS_ERR_TITLE"));
            }

            if (isExported)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_IMAGESET_COLUMN"));
            }

            // 標準共有パラメータファイル
            trans.Start("標準共有パラメータ");
            cmpParameters.SetSharedParamDefault();
            trans.Commit();

            transGroup.Assimilate();

            retCmd = Revit.UI.Result.Succeeded;
            return retCmd;
        }

        #endregion
    }
}