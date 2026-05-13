using System;
using System.Linq;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using SectionListRC.Setting;
using System.Windows.Forms;
using SectionListRC.Utils;
using System.Collections.Generic;

namespace SectionListRC.BeamList
{
    /// ================================================================================
    /// <summary>コマンド 個別梁リスト</summary>
    /// ================================================================================
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdEachOneBeamList : Revit.UI.IExternalCommand
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
        /// <history>2014/06/13 Created GSA,Inc. Ryo Kuroda</history>
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
            transGroup.Start(cmpAttribute.ResourceText("IDS_BTN_EACHONEBEAMLIST_NAME"));

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
            string retMsg1 = cmpService.WorkFlow(cmpAttribute.ResourceText("IDS_SHARE_PARA_BEAM_RANGER"));
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

            //string isOverlap = cmpService.IsOverlapStrings_Hari(strSetAry);
            //if (isOverlap != "")
            //{
            //  System.Windows.MessageBox.Show(SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_SETTINGOVERLAP") + "\r\n\r\n" + isOverlap);
            //  return retCmd;
            //}

            cmpParameters.GetSettingValue(strSetAry);

            // 予備ファイルコピー
            cmpParameters.ReserveFileCopy();

            // パラメータ名取得
            Collections.Generic.IList<Collections.Generic.IDictionary<string, string>> allParamName = cmpParameters.GetParamName();

            if (allParamName == null ||
                allParamName.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILE"));

                transGroup.Assimilate();
                return retCmd;
            }

            // 矩形柱パラメータ名
            Collections.Generic.IDictionary<string, string> dicKakuParamName = null;
            // 円柱パラメータ名
            Collections.Generic.IDictionary<string, string> dicEnParamName = null;

            // 梁パラメータ名
            Collections.Generic.IDictionary<string, string> dicHariParamName = null;
            // 片持ち梁パラメータ名
            Collections.Generic.IDictionary<string, string> dicKatamotiParamName = null;

            cmpParameters.GetColumnParamName(ref dicKakuParamName,
                                             ref dicEnParamName,
                                             ref dicHariParamName,
                                             ref dicKatamotiParamName);

            if (dicHariParamName == null ||
                dicHariParamName.Count == 0 ||
                dicKatamotiParamName == null ||
                dicKatamotiParamName.Count == 0)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOPARAMETERFILEVALUE"));

                transGroup.Assimilate();
                return retCmd;
            }

            // パラメータの重複判定
            string isOverlapHari = cmpService.IsOverlapStrings(dicHariParamName);
            string isOverlapKatamoti = cmpService.IsOverlapStrings(dicKatamotiParamName);

            if (isOverlapHari != "" || isOverlapKatamoti != "")
            {
                string errMsg = isOverlapHari;
                if (errMsg != "")
                {
                    errMsg += "\r\n";
                }
                errMsg += isOverlapKatamoti;

                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_SETTINGOVERLAP") + "\r\n\r\n" + errMsg);
                transGroup.Assimilate();
                return retCmd;
            }

            cmpParameters.GetParameterValue(dicKakuParamName,
                                            dicEnParamName,
                                            dicHariParamName,
                                            dicKatamotiParamName);

            // データ取得
            #region

            // 尺度
            int viewScale = 0;
            int.TryParse(cmpParameters.BeamListViewScale, out viewScale);
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

            // 位置表示枠高さ
            double positionFrameHeight = 0;
            double.TryParse(cmpParameters.PositionFrameHeight, out positionFrameHeight);

            // 階表示枠タイトル
            string lvlFrameTitle = cmpParameters.LevelFrameTitle;
            // 階表示枠接尾語
            string lvlFrameEndWord = cmpParameters.LevelFrameEndWord;
            // 符号表示枠タイトル
            string symbolFrameTitle = cmpParameters.SymbolFrameTitle;

            // 左のあき
            double leftSpace = 0;
            double.TryParse(cmpParameters.BeamLeftSpace, out leftSpace);
            // 右のあき
            double rightSpace = 0;
            double.TryParse(cmpParameters.BeamRightSpace, out rightSpace);
            // 中間のあきタイプ
            int centerSpaceType = 0;
            int.TryParse(cmpParameters.BeamCenterSpaceType, out centerSpaceType);
            // 中間のあき
            double centerSpace = 0;
            double.TryParse(cmpParameters.BeamCenterSpace, out centerSpace);
            // 上のあき
            double topSpace = 0;
            double.TryParse(cmpParameters.BeamTopSpace, out topSpace);
            // 下のあき
            double bottomSpace = 0;
            double.TryParse(cmpParameters.BeamBottomSpace, out bottomSpace);

            // 肋筋括弧表示
            int stirrupBracketShow = 0;
            int.TryParse(cmpParameters.StirrupBracketShow, out stirrupBracketShow);
            // 追加枠数
            int addFrameNumber = 0;
            int.TryParse(cmpParameters.BeamAddFrameNumber, out addFrameNumber);

            // 位置表示枠タイトル表示
            int positionFrameTitleShow = 0;
            int.TryParse(cmpParameters.PositionFrameTitleShow, out positionFrameTitleShow);

            // 位置表示枠区切り線表示
            int positionSpaceLineShow = 0;
            int.TryParse(cmpParameters.PositionFrameSpaceLineShow, out positionSpaceLineShow);

            // 位置表示枠全断タイトル
            string allSectionTitle = cmpParameters.AllSectionTitle;
            // 位置表示枠端部タイトル
            string edgeTitle = cmpParameters.EdgeTitle;
            // 位置表示枠中心部タイトル
            string centerSectionTitle = cmpParameters.CenterSectionTitle;
            // 位置表示枠始端タイトル
            string itanSectionTitle = cmpParameters.ItanSectionTitle;
            // 位置表示枠終端タイトル
            string jtanSectionTitle = cmpParameters.JtanSectionTitle;
            // 位置表示枠片持ち梁元端タイトル
            string cantiLeverStartTitle = cmpParameters.CantileverStartTitle;
            // 位置表示枠片持ち梁先端タイトル
            string cantiLeverEndTitle = cmpParameters.CantileverEndTitle;

            // 肋筋枠タイトル
            string stirrupFrameTitle = cmpParameters.StirrupFrameTitle;
            // 肋筋区切り記号
            string stirrupSpaceSymbol = cmpParameters.StirrupFrameSpaceSymbol;

            // 幅寸法線表示(すべて、左基準、中央基準)
            int widthDimShow = 0;
            int.TryParse(cmpParameters.WidthDimensionShow, out widthDimShow);
            // 高さ寸法線表示(すべて、省略)
            int heightDimShow = 0;
            int.TryParse(cmpParameters.HeightDimensionShow, out heightDimShow);

            // 主筋表示方法
            int rebarShow = 0;
            int.TryParse(cmpParameters.BeamRebarShow, out rebarShow);
            // 肋筋枠表示方法
            int stirrupFrameShow = 0;
            int.TryParse(cmpParameters.StirrupFrameShow, out stirrupFrameShow);
            // 腹筋枠表示方法
            int webFrameShow = 0;
            int.TryParse(cmpParameters.WebFrameShow, out webFrameShow);

            // i端幅
            string s_B = cmpParameters.s_B;
            // 中央幅
            string c_B = cmpParameters.c_B;
            // j端幅
            string e_B = cmpParameters.e_B;
            // i端成
            string s_D = cmpParameters.s_D;
            // 中央成
            string c_D = cmpParameters.c_D;
            // j端成
            string e_D = cmpParameters.e_D;

            // 主筋i端上太径
            string syukinItanUeHutokei = cmpParameters.RST_SyukinItanUeHutokei;
            // 主筋中央上太径
            string syukinChuohUeHutokei = cmpParameters.RST_SyukinChuohUeHutokei;
            // 主筋j端上太径
            string syukinJtanUeHutokei = cmpParameters.RST_SyukinJtanUeHutokei;
            // 主筋i端上1段筋太径本数
            string syukinItanUe1danHutokeiHonsu = cmpParameters.RST_SyukinItanUe1dankinHutokeiHonsu;
            // 主筋中央上1段筋太径本数
            string syukinChuohUe1danHutokeiHonsu = cmpParameters.RST_SyukinChuohUe1dankinHutokeiHonsu;
            // 主筋j端上1段筋太径本数
            string syukinJtanUe1danHutokeiHonsu = cmpParameters.RST_SyukinJtanUe1dankinHutokeiHonsu;
            // 主筋i端上2段筋太径本数
            string syukinItanUe2danHutokeiHonsu = cmpParameters.RST_SyukinItanUe2dankinHutokeiHonsu;
            // 主筋中央上2段筋太径本数
            string syukinChuohUe2danHutokeiHonsu = cmpParameters.RST_SyukinChuohUe2dankinHutokeiHonsu;
            // 主筋j端上2段筋太径本数
            string syukinJtanUe2danHutokeiHonsu = cmpParameters.RST_SyukinJtanUe2dankinHutokeiHonsu;
            // 主筋i端上3段筋太径本数
            string syukinItanUe3danHutokeiHonsu = cmpParameters.RST_SyukinItanUe3dankinHutokeiHonsu;
            // 主筋中央上3段筋太径本数
            string syukinChuohUe3danHutokeiHonsu = cmpParameters.RST_SyukinChuohUe3dankinHutokeiHonsu;
            // 主筋j端上3段筋太径本数
            string syukinJtanUe3danHutokeiHonsu = cmpParameters.RST_SyukinJtanUe3dankinHutokeiHonsu;

            // 主筋i端上細径
            string syukinItanUeHosokei = cmpParameters.RST_SyukinItanUeHosokei;
            // 主筋中央上細径
            string syukinChuohUeHosokei = cmpParameters.RST_SyukinChuohUeHosokei;
            // 主筋j端上細径
            string syukinJtanUeHosokei = cmpParameters.RST_SyukinJtanUeHosokei;
            // 主筋i端上1段筋太径本数
            string syukinItanUe1danHosokeiHonsu = cmpParameters.RST_SyukinItanUe1dankinHosokeiHonsu;
            // 主筋中央上1段筋太径本数
            string syukinChuohUe1danHosokeiHonsu = cmpParameters.RST_SyukinChuohUe1dankinHosokeiHonsu;
            // 主筋j端上1段筋太径本数
            string syukinJtanUe1danHosokeiHonsu = cmpParameters.RST_SyukinJtanUe1dankinHosokeiHonsu;
            // 主筋i端上2段筋太径本数
            string syukinItanUe2danHosokeiHonsu = cmpParameters.RST_SyukinItanUe2dankinHosokeiHonsu;
            // 主筋中央上2段筋太径本数
            string syukinChuohUe2danHosokeiHonsu = cmpParameters.RST_SyukinChuohUe2dankinHosokeiHonsu;
            // 主筋j端上2段筋太径本数
            string syukinJtanUe2danHosokeiHonsu = cmpParameters.RST_SyukinJtanUe2dankinHosokeiHonsu;
            // 主筋i端上3段筋太径本数
            string syukinItanUe3danHosokeiHonsu = cmpParameters.RST_SyukinItanUe3dankinHosokeiHonsu;
            // 主筋中央上3段筋太径本数
            string syukinChuohUe3danHosokeiHonsu = cmpParameters.RST_SyukinChuohUe3dankinHosokeiHonsu;
            // 主筋j端上3段筋太径本数
            string syukinJtanUe3danHosokeiHonsu = cmpParameters.RST_SyukinJtanUe3dankinHosokeiHonsu;

            // 主筋i端下太径
            string syukinItanSitaHutokei = cmpParameters.RST_SyukinItanSitaHutokei;
            // 主筋中央下太径
            string syukinChuohSitaHutokei = cmpParameters.RST_SyukinChuohSitaHutokei;
            // 主筋j端下太径
            string syukinJtanSitaHutokei = cmpParameters.RST_SyukinJtanSitaHutokei;
            // 主筋i端下1段筋太径本数
            string syukinItanSita1danHutokeiHonsu = cmpParameters.RST_SyukinItanSita1dankinHutokeiHonsu;
            // 主筋中央下1段筋太径本数
            string syukinChuohSita1danHutokeiHonsu = cmpParameters.RST_SyukinChuohSita1dankinHutokeiHonsu;
            // 主筋j端下1段筋太径本数
            string syukinJtanSita1danHutokeiHonsu = cmpParameters.RST_SyukinJtanSita1dankinHutokeiHonsu;
            // 主筋i端下2段筋太径本数
            string syukinItanSita2danHutokeiHonsu = cmpParameters.RST_SyukinItanSita2dankinHutokeiHonsu;
            // 主筋中央下2段筋太径本数
            string syukinChuohSita2danHutokeiHonsu = cmpParameters.RST_SyukinChuohSita2dankinHutokeiHonsu;
            // 主筋j端下2段筋太径本数
            string syukinJtanSita2danHutokeiHonsu = cmpParameters.RST_SyukinJtanSita2dankinHutokeiHonsu;
            // 主筋i端下3段筋太径本数
            string syukinItanSita3danHutokeiHonsu = cmpParameters.RST_SyukinItanSita3dankinHutokeiHonsu;
            // 主筋中央下3段筋太径本数
            string syukinChuohSita3danHutokeiHonsu = cmpParameters.RST_SyukinChuohSita3dankinHutokeiHonsu;
            // 主筋j端下3段筋太径本数
            string syukinJtanSita3danHutokeiHonsu = cmpParameters.RST_SyukinJtanSita3dankinHutokeiHonsu;

            // 主筋i端下細径
            string syukinItanSitaHosokei = cmpParameters.RST_SyukinItanSitaHosokei;
            // 主筋中央下細径
            string syukinChuohSitaHosokei = cmpParameters.RST_SyukinChuohSitaHosokei;
            // 主筋j端下細径
            string syukinJtanSitaHosokei = cmpParameters.RST_SyukinJtanSitaHosokei;
            // 主筋i端下1段筋太径本数
            string syukinItanSita1danHosokeiHonsu = cmpParameters.RST_SyukinItanSita1dankinHosokeiHonsu;
            // 主筋中央下1段筋太径本数
            string syukinChuohSita1danHosokeiHonsu = cmpParameters.RST_SyukinChuohSita1dankinHosokeiHonsu;
            // 主筋j端下1段筋太径本数
            string syukinJtanSita1danHosokeiHonsu = cmpParameters.RST_SyukinJtanSita1dankinHosokeiHonsu;
            // 主筋i端下2段筋太径本数
            string syukinItanSita2danHosokeiHonsu = cmpParameters.RST_SyukinItanSita2dankinHosokeiHonsu;
            // 主筋中央下2段筋太径本数
            string syukinChuohSita2danHosokeiHonsu = cmpParameters.RST_SyukinChuohSita2dankinHosokeiHonsu;
            // 主筋j端下2段筋太径本数
            string syukinJtanSita2danHosokeiHonsu = cmpParameters.RST_SyukinJtanSita2dankinHosokeiHonsu;
            // 主筋i端下3段筋太径本数
            string syukinItanSita3danHosokeiHonsu = cmpParameters.RST_SyukinItanSita3dankinHosokeiHonsu;
            // 主筋中央下3段筋太径本数
            string syukinChuohSita3danHosokeiHonsu = cmpParameters.RST_SyukinChuohSita3dankinHosokeiHonsu;
            // 主筋j端下3段筋太径本数
            string syukinJtanSita3danHosokeiHonsu = cmpParameters.RST_SyukinJtanSita3dankinHosokeiHonsu;

            // 肋筋i端径
            string s_Stirrup_Diameter = cmpParameters.s_Stirrup_Diameter;
            // 肋筋中央径
            string c_Stirrup_Diameter = cmpParameters.c_Stirrup_Diameter;
            // 肋筋j端径
            string e_Stirrup_Diameter = cmpParameters.e_Stirrup_Diameter;
            // 肋筋i端本数
            string s_Stirrup_Number = cmpParameters.s_Stirrup_Number;
            // 肋筋中央本数
            string c_Stirrup_Number = cmpParameters.c_Stirrup_Number;
            // 肋筋j端本数
            string e_Stirrup_Number = cmpParameters.e_Stirrup_Number;
            // 肋筋i端ピッチ
            string s_Stirrup_Pitch = cmpParameters.s_Stirrup_Pitch;
            // 肋筋中央ピッチ
            string c_Stirrup_Pitch = cmpParameters.c_Stirrup_Pitch;
            // 肋筋j端ピッチ
            string e_Stirrup_Pitch = cmpParameters.e_Stirrup_Pitch;

            // 腹筋i端径
            string s_Web_Diameter = cmpParameters.s_Web_Diameter;
            // 腹筋中央径
            string c_Web_Diameter = cmpParameters.c_Web_Diameter;
            // 腹筋j端径
            string e_Web_Diameter = cmpParameters.e_Web_Diameter;
            // 腹筋i端本数
            string s_Web_Number = cmpParameters.s_Web_Number;
            // 腹筋中央本数
            string c_Web_Number = cmpParameters.c_Web_Number;
            // 腹筋j端本数
            string e_Web_Number = cmpParameters.e_Web_Number;

            // 幅止筋i端径
            string s_Spacing_Diameter = cmpParameters.s_Spacing_Diameter;
            // 幅止筋中央径
            string c_Spacing_Diameter = cmpParameters.c_Spacing_Diameter;
            // 幅止筋j端径
            string e_Spacing_Diameter = cmpParameters.e_Spacing_Diameter;
            // 幅止筋i端本数
            string s_Spacing_Number = cmpParameters.s_Spacing_Number;
            // 幅止筋中央本数
            string c_Spacing_Number = cmpParameters.c_Spacing_Number;
            // 幅止筋j端本数
            string e_Spacing_Number = cmpParameters.e_Spacing_Number;
            // 幅止筋i端ピッチ
            string s_Spacing_Pitch = cmpParameters.s_Spacing_Pitch;
            // 幅止筋中央ピッチ
            string c_Spacing_Pitch = cmpParameters.c_Spacing_Pitch;
            // 幅止筋j端ピッチ
            string e_Spacing_Pitch = cmpParameters.e_Spacing_Pitch;

            // 梁符号
            string hariHugo = cmpParameters.RST_HariHugo;

            string harihugo_katamoti = cmpParameters.HariHugo_Katamoti;

            // かぶり厚
            double kaburi = 0;
            double.TryParse(cmpParameters.BeamProtectThick, out kaburi);

            #endregion Member Functions

            // 鉄筋ファミリ
            Revit.DB.Family rebarFam = null;
            bool isHaveFam = cmpElements.GetRebarFamily(ref rebarFam);

            // 線種タイプ取得
            frameLineType = cmpElements.FrameLineGraStyleByName(cmpParameters.FrameLineType);
            bodyLineType = cmpElements.BodyLineGraStyleByName(cmpParameters.BodyLineType);
            spaceLineType = cmpElements.SpacerLineGraStyleByName(cmpParameters.SpacerLineType);

            Revit.DB.Transaction trans = new Revit.DB.Transaction(rvtDbDoc);
            cmpService.trans = trans;

            // 全梁
            Collections.Generic.IList<Collections.Generic.IList<Revit.DB.FamilySymbol>> aryAry = new Collections.Generic.List<Collections.Generic.IList<Revit.DB.FamilySymbol>>();

            Collections.Generic.IList<Revit.DB.FamilySymbol> allGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            foreach (Revit.DB.FamilySymbol fs in cmpElements.GetRCBeamFamSymAry)
            {
                // 規定パラメータの所持判定
                if (cmpService.IsHaveGirderParam(fs) == true)
                {
                    allGirderAry.Add(fs);
                }
            }

            // 大梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> girderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>(); // cmpElements.GetGirderFamInsAry(cmpParameters.Girder_Category);
                                                                                                                                // 小梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> beamAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 片持ち大梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 片持ち小梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiBeamAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 基礎大梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> foundationGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 基礎小梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> foundationBeamAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 基礎片持ち大梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiFoundationGirderAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
            // 基礎片持ち小梁
            Collections.Generic.IList<Revit.DB.FamilySymbol> cantiFoundationBeamAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            cmpElements.GirderDivision(allGirderAry,
                                       cmpParameters.Girder_Category,
                                       cmpParameters.HariSyubetu_Katamoti,
                                       ref girderAry,
                                       ref beamAry,
                                       ref cantiGirderAry,
                                       ref cantiBeamAry,
                                       ref foundationGirderAry,
                                       ref foundationBeamAry,
                                       ref cantiFoundationGirderAry,
                                       ref cantiFoundationBeamAry);

            // 対象梁なし
            if (girderAry.Count < 1 &&
                beamAry.Count < 1 &&
                cantiGirderAry.Count < 1 &&
                cantiBeamAry.Count < 1 &&
                foundationGirderAry.Count < 1 &&
                foundationBeamAry.Count < 1 &&
                cantiFoundationGirderAry.Count < 1 &&
                cantiFoundationBeamAry.Count < 1)
            {
                System.Windows.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_NOTARGETGIRDER"));
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
            var paraSeting = projInfo.LookupParameter(cmpAttribute.ResourceText("IDS_SHARE_PARA_BEAM_RANGER"));
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

            //Show form
            FormBeamOption form = new FormBeamOption(cmpAttribute, true, settings, 1);
            form.IsEnabledGirderType = girderAry.Count != 0 ? true : false;
            form.IsEnabledCantiGirderType = cantiGirderAry.Count != 0 ? true : false;
            form.IsEnabledBeamType = beamAry.Count != 0 ? true : false;
            form.IsEnabledCantiBeamType = cantiBeamAry.Count != 0 ? true : false;
            form.IsEnabledFoundationGirdeType = foundationGirderAry.Count != 0 ? true : false;
            form.IsEnabledCantiFoundationGirderType = cantiFoundationGirderAry.Count != 0 ? true : false;
            form.IsEnabledFoundationBeamType = foundationBeamAry.Count != 0 ? true : false;
            form.IsEnabledCantiFoundationBeamType = cantiFoundationBeamAry.Count != 0 ? true : false;

            if (form.ShowDialog() != DialogResult.OK)
            {
                transGroup.Assimilate();
                return retCmd;
            }

            var new_Setting = form.GetStringSetting;

            Collections.Generic.IList<string> enHugoAry_filter = null;
            Collections.Generic.IList<string> allColumnLevelAry_filter = null;

            int aryNum = 0;
            if (form.IsExportAllChecked == false)
            {
                Collections.Generic.IList<Revit.DB.FamilySymbol> bAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();
                EnumType enumType = EnumType.Invalid;

                if (form.IsGirderTypeChecked)
                {
                    enumType = EnumType.Girder;
                    bAry = girderAry;
                }
                if (form.IsBeamTypeChecked)
                {
                    enumType = EnumType.Beam;
                    bAry = beamAry;
                }

                if (form.IsCantiGirderTypeChecked)
                {
                    enumType = EnumType.CantiGirder;
                    bAry = cantiGirderAry;
                }

                if (form.IsCantiBeamTypeChecked)
                {
                    enumType = EnumType.CantiBeam;
                    bAry = cantiBeamAry;
                }

                if (form.IsFoundationGirderTypeChecked)
                {
                    enumType = EnumType.FoundationGirder;
                    bAry = foundationGirderAry;
                }

                if (form.IsFoundationBeamTypeChecked)
                {
                    enumType = EnumType.FoundationBeam;
                    bAry = foundationBeamAry;
                }

                if (form.IsCantiFoundationGirderTypeChecked)
                {
                    enumType = EnumType.CantiFoundationGirder;
                    bAry = cantiFoundationGirderAry;
                }

                if (form.IsCantiFoundationBeamTypeChecked)
                {
                    enumType = EnumType.CantiFoundationBeam;
                    bAry = cantiFoundationBeamAry;
                }

                if (bAry.Count < 1)
                {
                    transGroup.Assimilate();
                    return retCmd;
                }

                aryNum = (int)enumType;

                // データテーブル
                System.Data.DataTable data = null;

                Collections.Generic.IList<Revit.DB.FamilySymbol> beams = bAry;

                if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                {
                    cmpElements.IsHaveSame(ref beams, cmpParameters.RST_HariHugo);

                    SectionListRC.Entities.DtBeam entDtBeam = new SectionListRC.Entities.DtBeam(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                    entDtBeam.GetData(beams);

                    data = entDtBeam.Data;
                }
                else
                {
                    cmpElements.IsHaveSame(ref beams, cmpParameters.HariHugo_Katamoti);

                    SectionListRC.Entities.DtCantiGirder entDtCanti = new SectionListRC.Entities.DtCantiGirder(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                    entDtCanti.GetData(beams);

                    data = entDtCanti.Data;
                }

                // 全階
                Collections.Generic.IList<string> allLevelAry = new Collections.Generic.List<string>();

                if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                {
                    allLevelAry = cmpService.LevelSortOrder_TopName(cmpService.GetAllBeamLevelAry(beams));
                }
                else
                {
                    allLevelAry = cmpService.LevelSortOrder_TopName(cmpService.GetAllBeamLevelAry_Canti(beams));
                }

                Collections.Generic.IList<string> sorted = cmpService.LevelSortOrder_NameDESC(allLevelAry);

                // ソート済みに含まれない全梁を追加
                foreach (string str in allLevelAry)
                {
                    if (!sorted.Contains(str))
                    {
                        sorted.Add(str);
                    }
                }
                allColumnLevelAry_filter = sorted;

                // 全梁符号
                Collections.Generic.IList<string> allHugoAry = new Collections.Generic.List<string>();

                if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                {
                    enHugoAry_filter = cmpService.GetAllBeamHugoAry(beams);
                }
                else
                {
                    enHugoAry_filter = cmpService.GetAllBeamHugoAry_Canti(beams);
                }

                //Show form
                var formList = new FormBeamItemList(cmpAttribute, cmpParameters, settings, enumType, enHugoAry_filter.ToList(), data, allColumnLevelAry_filter.ToList(), 1);
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

            #region 複数ファミリに同じ符号があるか
            string inDifferentFamily = "";
            Collections.Generic.IList<Revit.DB.FamilySymbol> checkAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

            if (form.IsGirderTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in girderAry)
                {
                    checkAry.Add(famSymbol);
                }
            }
            if (form.IsBeamTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in beamAry)
                {
                    checkAry.Add(famSymbol);
                }
            }

            if (form.IsCantiGirderTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in cantiGirderAry)
                {
                    checkAry.Add(famSymbol);
                }
            }

            if (form.IsCantiBeamTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in cantiBeamAry)
                {
                    checkAry.Add(famSymbol);
                }
            }

            if (form.IsFoundationGirderTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in foundationGirderAry)
                {
                    checkAry.Add(famSymbol);
                }
            }

            if (form.IsFoundationBeamTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in foundationBeamAry)
                {
                    checkAry.Add(famSymbol);
                }
            }

            if (form.IsCantiFoundationGirderTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in cantiFoundationGirderAry)
                {
                    checkAry.Add(famSymbol);
                }
            }

            if (form.IsCantiFoundationBeamTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol famSymbol in cantiFoundationBeamAry)
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

            #endregion 複数ファミリに同じ符号があるか

            if (form.IsGirderTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol fs in girderAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);
                    aryAry.Add(fsAry);
                }
            }

            if (form.IsCantiGirderTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol fs in cantiGirderAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);
                    aryAry.Add(fsAry);
                }
            }

            if (form.IsBeamTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol fs in beamAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);
                    aryAry.Add(fsAry);
                }
            }

            if (form.IsCantiBeamTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol fs in cantiBeamAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);
                    aryAry.Add(fsAry);
                }
            }

            if (form.IsFoundationGirderTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol fs in foundationGirderAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);
                    aryAry.Add(fsAry);
                }
            }

            if (form.IsCantiFoundationGirderTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol fs in cantiFoundationGirderAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);
                    aryAry.Add(fsAry);
                }
            }

            if (form.IsFoundationBeamTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol fs in foundationBeamAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);
                    aryAry.Add(fsAry);
                }
            }

            if (form.IsCantiFoundationBeamTypeChecked)
            {
                foreach (Revit.DB.FamilySymbol fs in cantiFoundationBeamAry)
                {
                    Collections.Generic.IList<Revit.DB.FamilySymbol> fsAry = new Collections.Generic.List<Revit.DB.FamilySymbol>();

                    fsAry.Add(fs);
                    aryAry.Add(fsAry);
                }
            }

            // 書き出しフォルダ
            string exportFolderPath = "";// cmpParameters.ExportFolder;

            // フォルダ選択
            System.Windows.Forms.FolderBrowserDialog folderDlg = new System.Windows.Forms.FolderBrowserDialog();

            if (folderDlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                exportFolderPath = folderDlg.SelectedPath;
            }
            else
            {
                transGroup.Assimilate();
                return retCmd;
            }

            // エラーメッセージ
            string writeErr = "";

            //////////////////////////////////////////////////

            // ビュー作成、表示
            Revit.DB.View current = rvtDbDoc.ActiveView;
            cmpElements.SetCreateListView(viewScale);

            // 作図用ビュー
            Revit.DB.ViewPlan vp = rvtDbDoc.ActiveView as Revit.DB.ViewPlan;

            // 現在開いているビュー
            Collections.Generic.IList<Revit.UI.UIView> currentOpenView = rvtUiApp.ActiveUIDocument.GetOpenUIViews();
            Revit.UI.UIView uiView = null;

            foreach (Revit.UI.UIView uiv in currentOpenView)
            {
                if (vp.Id.Value == uiv.ViewId.Value)
                {
                    uiView = uiv;
                }
            }

            //// プロジェクト名
            //string projName = rvtDbDoc.PathName;
            //if (projName.Length > ".rvt".Length)
            //{
            //  projName = projName.Substring(0, projName.LastIndexOf(".rvt"));
            //  projName = projName.Substring(projName.LastIndexOf("\\") + 1);
            //}
            //else
            //{
            //  System.Windows.Forms.MessageBox.Show("プロジェクトに名前がついていないため、\r\n現在時刻の名前を付けたフォルダに書き出します。");

            //  // 現在時刻
            //  System.DateTime time = System.DateTime.Now;
            //  projName = time.ToString("yyyyMMddHHmmss");
            //}

            string exportFolder = exportFolderPath + "\\大梁";
            //string exportFolder = exportFolderPath + "\\" + projName + "\\大梁";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && form.IsGirderTypeChecked)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            exportFolder = exportFolderPath + "\\小梁";
            //exportFolder = exportFolderPath + "\\" + projName + "\\小梁";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && form.IsBeamTypeChecked)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            exportFolder = exportFolderPath + "\\片持ち大梁";
            //exportFolder = exportFolderPath + "\\" + projName + "\\片持ち大梁";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && form.IsCantiGirderTypeChecked)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            exportFolder = exportFolderPath + "\\片持ち小梁";
            //exportFolder = exportFolderPath + "\\" + projName + "\\片持ち小梁";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && form.IsCantiBeamTypeChecked)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            exportFolder = exportFolderPath + "\\基礎大梁";
            //exportFolder = exportFolderPath + "\\" + projName + "\\基礎大梁";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && form.IsFoundationGirderTypeChecked)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            exportFolder = exportFolderPath + "\\基礎小梁";
            //exportFolder = exportFolderPath + "\\" + projName + "\\基礎小梁";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && form.IsFoundationBeamTypeChecked)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            exportFolder = exportFolderPath + "\\片持ち基礎大梁";
            //exportFolder = exportFolderPath + "\\" + projName + "\\片持ち基礎大梁";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && form.IsCantiFoundationGirderTypeChecked)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            exportFolder = exportFolderPath + "\\片持ち基礎小梁";
            //exportFolder = exportFolderPath + "\\" + projName + "\\片持ち基礎小梁";

            // 存在確認
            if (System.IO.Directory.Exists(exportFolder) == false && form.IsCantiFoundationBeamTypeChecked)
            {
                System.IO.Directory.CreateDirectory(exportFolder);
            }

            //////////////////////////////////////////////////

            // 書き出し有無
            bool isExported = false;

            // foreachの回数
            // aryNum == 2,4,6,8 が片持ち
            aryNum = 0;

            foreach (Collections.Generic.IList<Revit.DB.FamilySymbol> bAry in aryAry)
            {
                // 作図の原点
                Revit.DB.XYZ kaiHidariUe = new Revit.DB.XYZ();

                // 斜線
                bool isSyasen = false;

                // 現在の合計枠幅
                double sumWidth = 0;

                if (bAry.Count < 1)
                {
                    continue;
                }

                // タイプ名
                string name = bAry[0].Name;
                // 梁種別
                Revit.DB.Parameter bunruiParam = bAry[0].LookupParameter(cmpParameters.Girder_Category);
                if (bunruiParam == null)
                {
                    bunruiParam = bAry[0].LookupParameter(cmpParameters.HariSyubetu_Katamoti);
                }

                string bunrui = bunruiParam.AsString();
                if (bunrui == cmpAttribute.ResourceText("IDS_TXT_GIRDER"))
                {
                    exportFolder = exportFolderPath + "\\大梁";
                    //exportFolder = exportFolderPath + "\\" + projName + "\\大梁";

                    aryNum = 1;
                }
                else if (bunrui == cmpAttribute.ResourceText("IDS_TXT_BEAM"))
                {
                    exportFolder = exportFolderPath + "\\小梁";
                    //exportFolder = exportFolderPath + "\\" + projName + "\\小梁";

                    aryNum = 1;
                }
                else if (bunrui == cmpAttribute.ResourceText("IDS_TXT_CANTILEVER_GIRDER"))
                {
                    exportFolder = exportFolderPath + "\\片持ち大梁";
                    //exportFolder = exportFolderPath + "\\" + projName + "\\片持ち大梁";

                    aryNum = 2;
                }
                else if (bunrui == cmpAttribute.ResourceText("IDS_TXT_CANTILEVER_BEAM"))
                {
                    exportFolder = exportFolderPath + "\\片持ち小梁";
                    //exportFolder = exportFolderPath + "\\" + projName + "\\片持ち小梁";

                    aryNum = 2;
                }
                else if (bunrui == cmpAttribute.ResourceText("IDS_TXT_FOUNDATION_GIRDER"))
                {
                    exportFolder = exportFolderPath + "\\基礎大梁";
                    //exportFolder = exportFolderPath + "\\" + projName + "\\基礎大梁";

                    aryNum = 1;
                }
                else if (bunrui == cmpAttribute.ResourceText("IDS_TXT_FOUNDATION_BEAM"))
                {
                    exportFolder = exportFolderPath + "\\基礎小梁";
                    //exportFolder = exportFolderPath + "\\" + projName + "\\基礎小梁";

                    aryNum = 1;
                }
                else if (bunrui == cmpAttribute.ResourceText("IDS_TXT_CANTILEVER_FOUNDATION_GIRDER"))
                {
                    exportFolder = exportFolderPath + "\\片持ち基礎大梁";
                    //exportFolder = exportFolderPath + "\\" + projName + "\\片持ち基礎大梁";

                    aryNum = 2;
                }
                else if (bunrui == cmpAttribute.ResourceText("IDS_TXT_CANTILEVER_FOUNDATION_BEAM"))
                {
                    exportFolder = exportFolderPath + "\\片持ち基礎小梁";
                    //exportFolder = exportFolderPath + "\\" + projName + "\\片持ち基礎小梁";

                    aryNum = 2;
                }

                // ビュー尺度
                int scale = vp.Scale;

                // データテーブル
                System.Data.DataTable data = null;

                Collections.Generic.IList<Revit.DB.FamilySymbol> beams = bAry;

                if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                {
                    cmpElements.IsHaveSame(ref beams, cmpParameters.RST_HariHugo);

                    SectionListRC.Entities.DtBeam entDtBeam = new SectionListRC.Entities.DtBeam(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                    entDtBeam.GetData(beams);

                    data = entDtBeam.Data;
                }
                else
                {
                    cmpElements.IsHaveSame(ref beams, cmpParameters.HariHugo_Katamoti);

                    SectionListRC.Entities.DtCantiGirder entDtCanti = new SectionListRC.Entities.DtCantiGirder(cmpAttribute, cmpElements, cmpGeometry, cmpParameters, cmpSettings);
                    entDtCanti.GetData(beams);

                    data = entDtCanti.Data;
                }

                // 全階
                Collections.Generic.IList<string> allLevelAry = new Collections.Generic.List<string>();

                if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                {
                    allLevelAry = cmpService.GetAllBeamLevelAry(beams);
                }
                else
                {
                    allLevelAry = cmpService.GetAllBeamLevelAry_Canti(beams);
                }

                Collections.Generic.IList<string> sorted = cmpService.LevelSortOrder_NameDESC(allLevelAry);

                // ソート済みに含まれない全梁を追加
                foreach (string str in allLevelAry)
                {
                    if (!sorted.Contains(str))
                    {
                        sorted.Add(str);
                    }
                }
                allLevelAry = sorted;

                // 全梁符号
                Collections.Generic.IList<string> allHugoAry = new Collections.Generic.List<string>();

                if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                {
                    allHugoAry = cmpService.GetAllBeamHugoAry(beams);
                }
                else
                {
                    allHugoAry = cmpService.GetAllBeamHugoAry_Canti(beams);
                }

                if (allColumnLevelAry_filter != null && enHugoAry_filter != null)
                {
                    allHugoAry = (from string column in enHugoAry_filter
                                  where allHugoAry.Contains(column) == true
                                  select column).ToList();

                    allLevelAry = (from string level in allColumnLevelAry_filter
                                   where allLevelAry.Contains(level) == true
                                   select level).ToList();
                }

                if (allHugoAry.Count == 0 || allLevelAry.Count == 0)
                    continue;

                // 階別最大高さ
                Collections.Generic.IList<double> beamHeightAry = new Collections.Generic.List<double>();

                if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                {
                    beamHeightAry = cmpService.BeamHeightByLevel(data);
                }
                else
                {
                    beamHeightAry = cmpService.BeamHeightByLevel_Canti(data);
                }

                // 符号別最大幅
                Collections.Generic.IList<double> beamWidthAry = new Collections.Generic.List<double>();

                if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                {
                    beamWidthAry = cmpService.BeamWidthByMark(data);
                }
                else
                {
                    beamWidthAry = cmpService.BeamWidthByMark_Canti(data);
                }

                // 符号表示枠高さ
                double h_Hugowaku = -symbolFrameHeight / 304.8 * scale;

                // 位置表示枠高さ
                double h_Ichiwaku = -positionFrameHeight / 304.8 * scale;

                // 階表示枠幅
                double w_Kaihyojiwaku = lvlFrameWidth / 304.8 * scale;

                // 項目表示枠
                double w_Komokuwaku = itemFrameWidth / 304.8 * scale;

                // 符号ごとの最大幅
                double w_HugowakuMax = 0;

                // 行列数
                int r = allLevelAry.Count;
                int c = allHugoAry.Count;

                // 階表示枠有無
                bool kaihyoji = true;
                if (lvlFrameShow != 0)
                {
                    kaihyoji = false;
                }

                if (kaihyoji == false)
                {
                    w_Kaihyojiwaku = 0;
                }

                sumWidth += w_Kaihyojiwaku * 2;
                sumWidth += w_Komokuwaku * 2;

                // 枠、躯体線分
                Revit.DB.CurveArray crvFrameAry = new Revit.DB.CurveArray();
                Revit.DB.CurveArray crvStrctAry = new Revit.DB.CurveArray();

                //
                Revit.DB.XYZ koumokuHidariUe = new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y, kaiHidariUe.Z);
                Revit.DB.XYZ hugoHidariUe = new Revit.DB.XYZ(koumokuHidariUe.X + w_Komokuwaku, kaiHidariUe.Y, kaiHidariUe.Z);

                // 点
                Collections.Generic.IList<Revit.DB.XYZ> pntsX = new Collections.Generic.List<Revit.DB.XYZ>();
                pntsX.Add(kaiHidariUe);

                // 線
                Revit.DB.Line l = null;

                // ----- 左上の交差部分 -----
                #region

                l = cmpElements.CreateBoundLine(kaiHidariUe, hugoHidariUe);
                cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                l = cmpElements.CreateBoundLine(hugoHidariUe, new Revit.DB.XYZ(hugoHidariUe.X, kaiHidariUe.Y + h_Hugowaku + h_Ichiwaku, kaiHidariUe.Z));
                cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(hugoHidariUe.X, kaiHidariUe.Y + h_Hugowaku + h_Ichiwaku, kaiHidariUe.Z),
                                                new Revit.DB.XYZ(kaiHidariUe.X, kaiHidariUe.Y + h_Hugowaku + h_Ichiwaku, kaiHidariUe.Z));
                cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(kaiHidariUe.X, kaiHidariUe.Y + h_Hugowaku + h_Ichiwaku, kaiHidariUe.Z), kaiHidariUe + new Revit.DB.XYZ(0, 0.01, 0));
                cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                //        trans.Start("タイトル枠");

                if (kaihyoji == true)
                {
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y, kaiHidariUe.Z),
                                                    new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y + h_Hugowaku + h_Ichiwaku, kaiHidariUe.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z),
                                                    new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    // 階表示
                    Revit.DB.XYZ origin = cmpGeometry.Center2Point(kaiHidariUe,
                                                                   new Revit.DB.XYZ(koumokuHidariUe.X, koumokuHidariUe.Y + h_Hugowaku + h_Ichiwaku, koumokuHidariUe.Z));
                    Revit.DB.XYZ baseVec = vp.RightDirection;
                    Revit.DB.XYZ upVec = vp.UpDirection;

                    double lineWidth = w_Kaihyojiwaku / scale;

                    string str = lvlFrameTitle;

                    Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, str, rvtDbDoc);

                    trans.Start("Regenerate");
                    rvtDbDoc.Regenerate();
                    trans.Commit();

                    // 項目表示
                    origin = cmpGeometry.Center2Point(koumokuHidariUe, new Revit.DB.XYZ(hugoHidariUe.X, hugoHidariUe.Y + h_Hugowaku, hugoHidariUe.Z));
                    lineWidth = w_Komokuwaku / scale;
                    str = symbolFrameTitle;

                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, str, rvtDbDoc);

                    trans.Start("Regenerate");
                    rvtDbDoc.Regenerate();
                    trans.Commit();

                    // 位置表示
                    origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(koumokuHidariUe.X, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z),
                                                      new Revit.DB.XYZ(hugoHidariUe.X, kaiHidariUe.Y + h_Hugowaku + h_Ichiwaku, kaiHidariUe.Z));
                    lineWidth = w_Komokuwaku / scale;
                    str = "位置";

                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, str, rvtDbDoc);

                    trans.Start("Regenerate");
                    rvtDbDoc.Regenerate();
                    trans.Commit();
                }
                else
                {
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z),
                                                    new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    // 項目表示枠に斜線
                    // 階タイトルと項目タイトル

                    l = cmpElements.CreateBoundLine(kaiHidariUe, new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    // 三角形の中心に配置
                    Revit.DB.XYZ origin = cmpGeometry.TriangleGravity2D(kaiHidariUe,
                                                                        new Revit.DB.XYZ(kaiHidariUe.X, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z),
                                                                        new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                    // 現在ビューの各方向
                    Revit.DB.XYZ baseVec = vp.RightDirection;
                    Revit.DB.XYZ upVec = vp.UpDirection;

                    double lineWidth = w_Komokuwaku / scale;

                    string str = lvlFrameTitle;

                    Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, str, rvtDbDoc);

                    trans.Start("Regenerate");
                    rvtDbDoc.Regenerate();
                    trans.Commit();

                    // 項目表示
                    origin = cmpGeometry.TriangleGravity2D(kaiHidariUe,
                                                           new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y, kaiHidariUe.Z),
                                                           new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z));
                    lineWidth = w_Komokuwaku / scale;
                    str = symbolFrameTitle;

                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, str, rvtDbDoc);

                    rvtDbDoc.Regenerate();

                    // 位置表示
                    origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(kaiHidariUe.X, kaiHidariUe.Y + h_Hugowaku, kaiHidariUe.Z),
                                                      new Revit.DB.XYZ(kaiHidariUe.X + w_Komokuwaku, kaiHidariUe.Y + h_Hugowaku + h_Ichiwaku, kaiHidariUe.Z));
                    lineWidth = w_Komokuwaku / scale;
                    str = "位置";

                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, str, rvtDbDoc);

                    trans.Start("Regenerate");
                    rvtDbDoc.Regenerate();
                    trans.Commit();
                }

                //        trans.Commit();

                Revit.DB.XYZ px = new Revit.DB.XYZ(kaiHidariUe.X + w_Kaihyojiwaku + w_Komokuwaku, kaiHidariUe.Y, kaiHidariUe.Z);
                pntsX.Add(px);

                #endregion

                // ----- 符号表示枠 -----
                #region
                double wx = 0;

                //        trans.Start("符号枠");

                for (int i = 0; i < allHugoAry.Count; ++i)
                {
                    // 梁幅(合計)
                    double maxW = 0;

                    if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                    {
                        maxW = cmpService.BeamWidthByMark(data, allHugoAry[i]);
                    }
                    else
                    {
                        maxW = cmpService.BeamWidthByMark_Canti(data, allHugoAry[i]);
                    }

                    // 断面数
                    int secNum = 0;
                    int typeNum = 0;

                    Collections.Generic.IList<double> beamSecWidthAry = null;

                    if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                    {
                        beamSecWidthAry = cmpService.BeamSecWidthAry(data, allHugoAry[i], ref typeNum);
                    }
                    // 片持ちの場合
                    if (aryNum == 2 || aryNum == 4 || aryNum == 6 || aryNum == 8)
                    {
                        beamSecWidthAry = cmpService.BeamSecWidthAry_Canti(data, allHugoAry[i], ref typeNum);

                        if (typeNum == 2)
                        {
                            // 中央部がある
                            writeErr += "片持ち梁「" + allHugoAry[i] + "」は中央部に値が存在します。" + "\r\n";
                        }
                    }

                    // 2014/01/15
                    // 断面形状が取得できなかった場合
                    if (beamSecWidthAry.Count == 0)
                    {
                        if (typeNum == 0)
                        {
                            beamSecWidthAry.Add(0);
                        }
                        else if (typeNum == 1)
                        {
                            beamSecWidthAry.Add(0);
                            beamSecWidthAry.Add(0);
                        }
                        else
                        {
                            beamSecWidthAry.Add(0);
                            beamSecWidthAry.Add(0);
                            beamSecWidthAry.Add(0);
                        }
                    }

                    if (typeNum == 0)
                    {
                        secNum = 1;
                    }
                    else if (typeNum == 1)
                    {
                        secNum = 2;
                    }
                    else
                    {
                        secNum = 3;
                    }

                    // 枠最大幅
                    w_HugowakuMax = maxW + (leftSpace + rightSpace + centerSpace * (secNum - 1)) / 304.8 * scale;
                    sumWidth += w_HugowakuMax;

                    // 断面ごとの幅
                    double w_itanWaku = 0;
                    double w_chuohWaku = 0;
                    double w_jtanWaku = 0;

                    if (beamSecWidthAry.Count == 1)
                    {
                        w_itanWaku = beamSecWidthAry[0] + (leftSpace + rightSpace) / 304.8 * scale;
                    }
                    else if (beamSecWidthAry.Count == 2)
                    {
                        w_itanWaku = beamSecWidthAry[0] + (leftSpace + centerSpace / 2) / 304.8 * scale;
                        w_chuohWaku = beamSecWidthAry[1] + (centerSpace / 2 + rightSpace) / 304.8 * scale;
                    }
                    else if (beamSecWidthAry.Count == 3)
                    {
                        w_itanWaku = beamSecWidthAry[0] + (leftSpace + centerSpace / 2) / 304.8 * scale;
                        w_chuohWaku = beamSecWidthAry[1] + centerSpace / 304.8 * scale;
                        w_jtanWaku = beamSecWidthAry[2] + (centerSpace / 2 + rightSpace) / 304.8 * scale;
                    }

                    // 直前の点 + 符号最大幅
                    wx = pntsX[pntsX.Count - 1].X + w_HugowakuMax;

                    // 符号表示枠
                    l = cmpElements.CreateBoundLine(px, new Revit.DB.XYZ(wx, px.Y, px.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(wx, px.Y, px.Z), new Revit.DB.XYZ(wx, px.Y + h_Hugowaku + h_Ichiwaku, px.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(wx, px.Y + h_Hugowaku + h_Ichiwaku, px.Z), new Revit.DB.XYZ(px.X, px.Y + h_Hugowaku + h_Ichiwaku, px.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(px.X, px.Y + h_Hugowaku, px.Z), new Revit.DB.XYZ(wx, px.Y + h_Hugowaku, px.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    // 符号表示枠タイトル
                    string title = allHugoAry[i];

                    // 複数ファミリに同一タイプ名がある場合
                    string overlapnames = "";
                    foreach (string lvlName in allLevelAry)
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
                    //Revit.DB.TextAlignFlags flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                    Revit.DB.TextNote txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                    //if (titleTNT != null)
                    //{
                    //  txtNote.ChangeTypeId(titleTNT.Id);
                    //}

                    //txtnote.width = w_HugowakuMax;// txtNote.LineWidth;

                    // 位置表示枠
                    Revit.DB.XYZ basePoint = new Revit.DB.XYZ(px.X, px.Y + h_Hugowaku, px.Z);

                    Collections.Generic.IList<Revit.DB.XYZ> titlePoints = cmpService.PositionFrameTitlePoints(basePoint,
                                                                                                              h_Ichiwaku,
                                                                                                              leftSpace / 304.8 * scale,
                                                                                                              centerSpace / 304.8 * scale,
                                                                                                              rightSpace / 304.8 * scale,
                                                                                                              beamSecWidthAry);

                    // 区切り線
                    Collections.Generic.IList<Revit.DB.Line> spaceLineAry = new Collections.Generic.List<Revit.DB.Line>();
                    // 区切り線表示
                    if (positionSpaceLineShow == 0)
                    {
                        spaceLineAry = cmpService.PositionFrameSpaceLines(basePoint, h_Ichiwaku, leftSpace / 304.8 * scale, centerSpace / 304.8 * scale, beamSecWidthAry);
                        foreach (Revit.DB.Line spaceline in spaceLineAry)
                        {
                            cmpElements.NotNullCurveSet(ref crvFrameAry, spaceline);
                        }
                    }

                    #region 断面数別タイトル
                    if (secNum == 1)
                    {
                        title = allSectionTitle;
                        origin = titlePoints[0];
                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        if (otherTNT != null)
                        {
                            txtNote.ChangeTypeId(otherTNT.Id);
                        }
                        //txtnote.width = w_itanWaku;// w_HugowakuMax;// txtNote.LineWidth;
                    }
                    else if (secNum == 2)
                    {
                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;

                        if (aryNum != 2 && aryNum != 4 && aryNum != 6 && aryNum != 8)
                        {
                            title = edgeTitle;
                            origin = titlePoints[0];

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                            if (otherTNT != null)
                            {
                                txtNote.ChangeTypeId(otherTNT.Id);
                            }
                            //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// txtNote.LineWidth;

                            lineWidth = w_chuohWaku / scale;

                            title = centerSectionTitle;
                            origin = titlePoints[1];

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                            if (otherTNT != null)
                            {
                                txtNote.ChangeTypeId(otherTNT.Id);
                            }
                            //txtnote.width = w_chuohWaku;// w_HugowakuMax / 2;// txtNote.LineWidth;
                        }
                        // 片持ち
                        else
                        {
                            // 元端
                            title = cantiLeverStartTitle;
                            origin = titlePoints[0];

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                            if (otherTNT != null)
                            {
                                txtNote.ChangeTypeId(otherTNT.Id);
                            }
                            //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// txtNote.LineWidth;

                            lineWidth = w_chuohWaku / scale;

                            // 先端
                            title = cantiLeverEndTitle;
                            origin = titlePoints[1];

                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                            if (otherTNT != null)
                            {
                                txtNote.ChangeTypeId(otherTNT.Id);
                            }
                            //txtnote.width = w_chuohWaku;// w_HugowakuMax / 2;// txtNote.LineWidth;
                        }
                    }
                    else if (secNum == 3)
                    {
                        // 始端
                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 3;

                        title = itanSectionTitle;
                        if (aryNum == 2 || aryNum == 4 || aryNum == 6 || aryNum == 8)
                        {
                            title = cantiLeverStartTitle;
                        }

                        origin = titlePoints[0];

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        if (otherTNT != null)
                        {
                            txtNote.ChangeTypeId(otherTNT.Id);
                        }
                        //txtnote.width = w_itanWaku;// w_HugowakuMax / 3;// txtNote.LineWidth;

                        // 中央
                        lineWidth = w_chuohWaku / scale;

                        title = centerSectionTitle;

                        origin = titlePoints[1];

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        if (otherTNT != null)
                        {
                            txtNote.ChangeTypeId(otherTNT.Id);
                        }
                        //txtnote.width = w_chuohWaku;// w_HugowakuMax / 3;// txtNote.LineWidth;

                        // 終端
                        lineWidth = w_jtanWaku / scale;

                        title = jtanSectionTitle;
                        if (aryNum == 2 || aryNum == 4 || aryNum == 6 || aryNum == 8)
                        {
                            title = cantiLeverEndTitle;
                        }

                        origin = titlePoints[2];

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        if (otherTNT != null)
                        {
                            txtNote.ChangeTypeId(otherTNT.Id);
                        }
                        //txtnote.width = w_jtanWaku;// w_HugowakuMax / 3;// txtNote.LineWidth;
                    }
                    #endregion

                    px = new Revit.DB.XYZ(wx, px.Y, px.Z);
                    pntsX.Add(px);
                }

                //        trans.Commit();

                #endregion

                // ----- 階表示枠、項目表示枠 -----
                #region

                double hugo_Ichiwaku = h_Hugowaku + h_Ichiwaku;

                // 階表示枠左上
                Revit.DB.XYZ p1 = pntsX[0];
                // 項目表示枠右上
                Revit.DB.XYZ p2 = pntsX[1];

                Collections.Generic.IList<Revit.DB.XYZ> pntsY = new Collections.Generic.List<Revit.DB.XYZ>();
                pntsY.Add(p1);
                pntsY.Add(new Revit.DB.XYZ(p1.X, p1.Y + h_Hugowaku, p1.Z));

                double h = 0;

                //        trans.Start("表示枠");

                for (int i_h = 0; i_h < r; ++i_h)
                {
                    h = 0;

                    // 現在の階名
                    string levelName = allLevelAry[i_h];

                    // 最大高さ
                    double maxH = 0;

                    if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                    {
                        maxH = cmpService.BeamHeightByLevel(data, levelName);
                    }
                    else
                    {
                        maxH = cmpService.BeamHeightByLevel_Canti(data, levelName);
                    }

                    double otherH = (topSpace + bottomSpace + arrangementFrameHeight * (4 + addFrameNumber)) / 304.8 * scale;
                    if (webFrameShow == 2)
                    {
                        otherH = (topSpace + bottomSpace + arrangementFrameHeight * (3 + addFrameNumber)) / 304.8 * scale;
                    }

                    Revit.DB.XYZ origin = null;
                    string title = "";
                    Revit.DB.XYZ baseVec = vp.RightDirection;
                    Revit.DB.XYZ upVec = vp.UpDirection;
                    double lineWidth = w_Kaihyojiwaku / scale;
                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;
                    Revit.DB.TextNote txtNote = null;

                    // 最上階以外
                    if (i_h != 0)
                    {
                        if (positionFrameTitleShow == 0)
                        {
                            l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X, p1.Y + hugo_Ichiwaku, p1.Z), new Revit.DB.XYZ(p1.X, p1.Y + hugo_Ichiwaku + h_Ichiwaku, p1.Z));
                            cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                            l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku + h_Ichiwaku, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hugo_Ichiwaku + h_Ichiwaku, p2.Z));
                            cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                            l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p2.X, p2.Y + hugo_Ichiwaku + h_Ichiwaku, p2.Z), new Revit.DB.XYZ(p2.X, p2.Y + hugo_Ichiwaku, p2.Z));
                            cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                            if (kaihyoji == true)
                            {
                                //l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku, p1.Z), new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku + h_Ichiwaku, p1.Z));
                                //cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                                //origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X, p1.Y + hugo_Ichiwaku, p1.Z), new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku + h_Ichiwaku, p1.Z));
                                //title = lvlFrameEndWord;

                                //txtNote = cmpService.CreateNewTextNote(vp,
                                //                                 origin,
                                //                                 baseVec,
                                //                                 upVec,
                                //                                 lineWidth,
                                //                                 flags,
                                //                                 title);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                // //txtnote.width = txtNote.LineWidth;

                                origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku, p1.Z), new Revit.DB.XYZ(p2.X, p1.Y + hugo_Ichiwaku + h_Ichiwaku, p1.Z));
                                title = "位置";

                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                //txtnote.width = w_Kaihyojiwaku;// txtNote.LineWidth;
                            }
                            else
                            {
                                origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku, p1.Z), new Revit.DB.XYZ(p2.X, p1.Y + hugo_Ichiwaku + h_Ichiwaku, p1.Z));
                                title = "位置";
                                lineWidth = w_Komokuwaku / scale;

                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                //if (otherTNT != null)
                                //{
                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                //}
                                //txtnote.width = w_Komokuwaku;// txtNote.LineWidth;
                            }

                            hugo_Ichiwaku += h_Ichiwaku;
                        }
                    }

                    h += -(maxH + otherH);

                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X, p1.Y + hugo_Ichiwaku, p1.Z), new Revit.DB.XYZ(p1.X, p1.Y + hugo_Ichiwaku + h, p1.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X, p1.Y + hugo_Ichiwaku + h, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hugo_Ichiwaku + h, p2.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                    l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p2.X, p2.Y + hugo_Ichiwaku + h, p2.Z), new Revit.DB.XYZ(p2.X, p2.Y + hugo_Ichiwaku, p2.Z));
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    if (kaihyoji == true)
                    {
                        l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku, p1.Z), new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku + h, p1.Z));
                        cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                        // 階表示文字
                        origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X, p1.Y + hugo_Ichiwaku, p1.Z), new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku + h, p1.Z));
                        title = allLevelAry[i_h] + lvlFrameEndWord;
                        baseVec = vp.RightDirection;
                        upVec = vp.UpDirection;

                        lineWidth = w_Kaihyojiwaku / scale;
                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                        //if (titleTNT != null)
                        //{
                        //  txtNote.ChangeTypeId(titleTNT.Id);
                        //}
                        //txtnote.width = w_Kaihyojiwaku;// txtNote.LineWidth;

                        // 断面
                        origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku, p1.Z),
                                                          new Revit.DB.XYZ(p2.X, p2.Y + hugo_Ichiwaku - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));
                        title = "断面";
                        lineWidth = w_Komokuwaku / scale;

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                        //if (titleTNT != null)
                        //{
                        //  txtNote.ChangeTypeId(titleTNT.Id);
                        //}
                        //txtnote.width = w_Komokuwaku;// txtNote.LineWidth;
                    }
                    else
                    {
                        origin = cmpGeometry.Center2Point(new Revit.DB.XYZ(p1.X, p1.Y + hugo_Ichiwaku, p1.Z), new Revit.DB.XYZ(p2.X, p2.Y + hugo_Ichiwaku - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p2.Z));

                        // 階表示文字
                        title = allLevelAry[i_h] + lvlFrameEndWord;
                        lineWidth = w_Komokuwaku / scale;

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, titleTNT.Id, title, rvtDbDoc);

                        //if (titleTNT != null)
                        //{
                        //  txtNote.ChangeTypeId(titleTNT.Id);
                        //}
                        //txtnote.width = w_Komokuwaku;// txtNote.LineWidth;
                    }

                    // ----- 上端筋 -----
                    Revit.DB.XYZ point1 = new Revit.DB.XYZ(p1.X + w_Kaihyojiwaku, p1.Y + hugo_Ichiwaku - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                    Revit.DB.XYZ point2 = new Revit.DB.XYZ(p2.X, p1.Y + hugo_Ichiwaku - (maxH + (topSpace + bottomSpace) / 304.8 * scale), p1.Z);
                    l = cmpElements.CreateBoundLine(point1, point2);
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    origin = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hugo_Ichiwaku - (maxH + (topSpace + bottomSpace + arrangementFrameHeight) / 304.8 * scale), p1.Z));
                    title = "上端筋";
                    lineWidth = w_Komokuwaku / scale;

                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                    //if (otherTNT != null)
                    //{
                    //  txtNote.ChangeTypeId(otherTNT.Id);
                    //}
                    //txtnote.width = w_Komokuwaku;// txtNote.LineWidth;

                    // ----- 下端筋 -----
                    point1 += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0);
                    point2 += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0);
                    l = cmpElements.CreateBoundLine(point1, point2);
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    origin += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0); // = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * 2) / 304.8 * scale), p1.Z));
                    title = "下端筋";

                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                    //if (otherTNT != null)
                    //{
                    //  txtNote.ChangeTypeId(otherTNT.Id);
                    //}
                    //txtnote.width = w_Komokuwaku;// txtNote.LineWidth;

                    // ----- 肋筋 -----
                    point1 += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0);
                    point2 += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0);
                    l = cmpElements.CreateBoundLine(point1, point2);
                    cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                    origin += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0); // = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * 2) / 304.8 * scale), p1.Z));
                    title = stirrupFrameTitle;

                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                    //if (otherTNT != null)
                    //{
                    //  txtNote.ChangeTypeId(otherTNT.Id);
                    //}
                    //txtnote.width = w_Komokuwaku;// txtNote.LineWidth;

                    // ----- 腹筋 -----
                    if (webFrameShow != 2)
                    {
                        point1 += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0);
                        point2 += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                        origin += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0); // = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(p2.X, p1.Y + hy - (maxH + (topSpace + bottomSpace + arrangementFrameHeight * 2) / 304.8 * scale), p1.Z));
                        title = "腹筋";

                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                        //if (otherTNT != null)
                        //{
                        //  txtNote.ChangeTypeId(otherTNT.Id);
                        //}
                        //txtnote.width = w_Komokuwaku;// txtNote.LineWidth;
                    }

                    // ----- 追加枠 -----
                    if (addFrameNumber > 0)
                    {
                        for (int addFrameNum = 0; addFrameNum < addFrameNumber; ++addFrameNum)
                        {
                            point1 += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0);
                            point2 += new Revit.DB.XYZ(0, -arrangementFrameHeight / 304.8 * scale, 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                        }
                    }

                    hugo_Ichiwaku += h;

                    pntsX.Add(new Revit.DB.XYZ(p1.X, p1.Y + hugo_Ichiwaku, p1.Z));
                }

                //        trans.Commit();

                #endregion

                // ----- 断面 -----
                #region

                // 基点
                Revit.DB.XYZ hidariUe = new Revit.DB.XYZ(hugoHidariUe.X, hugoHidariUe.Y + h_Hugowaku + h_Ichiwaku, hugoHidariUe.Z);

                for (int i = 0; i < allHugoAry.Count; ++i)
                {
                    Revit.DB.XYZ hidariSita = hidariUe;
                    Revit.DB.XYZ migiUe = hidariUe;
                    Revit.DB.XYZ migiSita = hidariUe;

                    // 現在の符号
                    string hugoName = allHugoAry[i];
                    // 現在の符号最大幅
                    double hugoMaxX = 0;

                    if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                    {
                        hugoMaxX = cmpService.BeamWidthByMark(data, hugoName);
                    }
                    else
                    {
                        hugoMaxX = cmpService.BeamWidthByMark_Canti(data, hugoName);
                    }

                    // 梁断面タイプ(0 = 全断、1 = 端部・中央、2 = 始端・中央・終端)
                    int secTypeNum = 0;

                    // 現在の符号の最大幅
                    Collections.Generic.IList<double> beamSecWidthAryByMark = new Collections.Generic.List<double>();

                    if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                    {
                        beamSecWidthAryByMark = cmpService.BeamSecWidthAry(data, hugoName, ref secTypeNum);
                    }
                    else
                    {
                        beamSecWidthAryByMark = cmpService.BeamSecWidthAry_Canti(data, hugoName, ref secTypeNum);
                    }

                    // 2014/01/15
                    if (beamSecWidthAryByMark.Count == 0)
                    {
                        if (secTypeNum == 0)
                        {
                            beamSecWidthAryByMark.Add(0);
                        }
                        else if (secTypeNum == 1)
                        {
                            beamSecWidthAryByMark.Add(0);
                            beamSecWidthAryByMark.Add(0);
                        }
                        else
                        {
                            beamSecWidthAryByMark.Add(0);
                            beamSecWidthAryByMark.Add(0);
                            beamSecWidthAryByMark.Add(0);
                        }
                    }

                    // 断面ごとの幅
                    double w_itanWaku = 0;
                    double w_chuohWaku = 0;
                    double w_jtanWaku = 0;

                    if (beamSecWidthAryByMark.Count == 1)
                    {
                        w_itanWaku = beamSecWidthAryByMark[0] + (leftSpace + rightSpace) / 304.8 * scale;
                    }
                    else if (beamSecWidthAryByMark.Count == 2)
                    {
                        w_itanWaku = beamSecWidthAryByMark[0] + (leftSpace + centerSpace / 2) / 304.8 * scale;
                        w_chuohWaku = beamSecWidthAryByMark[1] + (centerSpace / 2 + rightSpace) / 304.8 * scale;
                    }
                    else if (beamSecWidthAryByMark.Count == 3)
                    {
                        w_itanWaku = beamSecWidthAryByMark[0] + (leftSpace + centerSpace / 2) / 304.8 * scale;
                        w_chuohWaku = beamSecWidthAryByMark[1] + centerSpace / 304.8 * scale;
                        w_jtanWaku = beamSecWidthAryByMark[2] + (centerSpace / 2 + rightSpace) / 304.8 * scale;
                    }

                    // 梁以外の高さ
                    double otherHeight = (topSpace + bottomSpace + arrangementFrameHeight * (4 + addFrameNumber)) / 304.8 * scale;
                    if (webFrameShow == 2)
                    {
                        otherHeight = (topSpace + bottomSpace + arrangementFrameHeight * (3 + addFrameNumber)) / 304.8 * scale;
                    }
                    // 梁以外の幅
                    double otherWidth = (leftSpace + rightSpace + centerSpace * (beamSecWidthAryByMark.Count - 1)) / 304.8 * scale;

                    w_HugowakuMax = 0;
                    foreach (double widthd in beamSecWidthAryByMark)
                    {
                        w_HugowakuMax += widthd;
                    }
                    w_HugowakuMax += otherWidth;

                    for (int j = 0; j < allLevelAry.Count; ++j)
                    {
                        // 現在の階
                        string level = allLevelAry[j];
                        // 現在の階最大高さ
                        double levelMaxY = 0;

                        if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                        {
                            levelMaxY = cmpService.BeamHeightByLevel(data, level);
                        }
                        else
                        {
                            levelMaxY = cmpService.BeamHeightByLevel_Canti(data, level);
                        }

                        // データテーブル上の番号
                        int currentNum = 0;
                        // 符号と階の組み合わせに該当するか
                        bool gaito = false;
                        for (int k = 0; k < data.Rows.Count; ++k)
                        {
                            string currentHugo = (string)data.Rows[k][cmpParameters.RST_HariHugo];
                            string currentLevel = (string)data.Rows[k][cmpParameters.LevelFrameTitle];

                            if (currentHugo == hugoName && currentLevel == level)
                            {
                                currentNum = k;
                                gaito = true;
                                break;
                            }
                        }

                        string title = "";
                        Revit.DB.XYZ origin = new Revit.DB.XYZ();
                        Revit.DB.XYZ baseVec = vp.RightDirection;
                        Revit.DB.XYZ upVec = vp.UpDirection;
                        double lineWidth = hugoMaxX / scale;
                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;
                        Revit.DB.TextNote txtNote = null;

                        // 最上階以外
                        if (j != 0)
                        {
                            // 位置表示枠をすべての階に表示
                            if (positionFrameTitleShow == 0)
                            {
                                l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(hidariUe.X, hidariUe.Y + h_Ichiwaku, hidariUe.Z), new Revit.DB.XYZ(hidariUe.X + hugoMaxX + otherWidth, hidariUe.Y + h_Ichiwaku, hidariUe.Z));
                                cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                                l = cmpElements.CreateBoundLine(new Revit.DB.XYZ(hidariUe.X + hugoMaxX + otherWidth, hidariUe.Y, hidariUe.Z), new Revit.DB.XYZ(hidariUe.X + hugoMaxX + otherWidth, hidariUe.Y + h_Ichiwaku, hidariUe.Z));
                                cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                                // 位置表示枠
                                Revit.DB.XYZ basePoint = hidariUe;

                                Collections.Generic.IList<Revit.DB.XYZ> titlePoints = cmpService.PositionFrameTitlePoints(basePoint,
                                                                                                                          h_Ichiwaku,
                                                                                                                          leftSpace / 304.8 * scale,
                                                                                                                          centerSpace / 304.8 * scale,
                                                                                                                          rightSpace / 304.8 * scale,
                                                                                                                          beamSecWidthAryByMark);

                                // 区切り線
                                Collections.Generic.IList<Revit.DB.Line> spaceLineAry = new Collections.Generic.List<Revit.DB.Line>();
                                // 区切り線表示
                                if (positionSpaceLineShow == 0)
                                {
                                    spaceLineAry = cmpService.PositionFrameSpaceLines(basePoint,
                                                                                      h_Ichiwaku,
                                                                                      leftSpace / 304.8 * scale,
                                                                                      centerSpace / 304.8 * scale,
                                                                                      beamSecWidthAryByMark);
                                    foreach (Revit.DB.Line spaceline in spaceLineAry)
                                    {
                                        cmpElements.NotNullCurveSet(ref crvFrameAry, spaceline);
                                    }
                                }

                                #region 断面数別タイトル
                                //                trans.Start("断面数別タイトル");
                                if (titlePoints.Count == 1)
                                {
                                    title = allSectionTitle;
                                    origin = titlePoints[0];
                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;

                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtnote.width = w_itanWaku;// w_HugowakuMax;// txtNote.LineWidth;
                                }
                                else if (titlePoints.Count == 2)
                                {
                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;

                                    if (aryNum != 2 && aryNum != 4 && aryNum != 6 && aryNum != 8)
                                    {
                                        title = edgeTitle;
                                        origin = titlePoints[0];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// txtNote.LineWidth;

                                        lineWidth = w_chuohWaku / scale;
                                        title = centerSectionTitle;
                                        origin = titlePoints[1];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_chuohWaku;// w_HugowakuMax / 2;// txtNote.LineWidth;
                                    }
                                    // 片持ち
                                    else
                                    {
                                        // 元端
                                        title = cantiLeverStartTitle;
                                        origin = titlePoints[0];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// txtNote.LineWidth;

                                        // 先端
                                        lineWidth = w_chuohWaku / scale;
                                        title = cantiLeverEndTitle;
                                        origin = titlePoints[1];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_chuohWaku;// w_HugowakuMax / 2;// txtNote.LineWidth;
                                    }
                                }
                                else if (titlePoints.Count == 3)
                                {
                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 3;

                                    // 始端
                                    title = itanSectionTitle;
                                    if (aryNum == 2 || aryNum == 4 || aryNum == 6 || aryNum == 8)
                                    {
                                        title = cantiLeverStartTitle;
                                    }

                                    origin = titlePoints[0];

                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtnote.width = w_itanWaku;// w_HugowakuMax / 3;// txtNote.LineWidth;

                                    // 中央
                                    lineWidth = w_chuohWaku / scale;
                                    title = centerSectionTitle;

                                    origin = titlePoints[1];

                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtnote.width = w_chuohWaku;// w_HugowakuMax / 3;// txtNote.LineWidth;

                                    // 終端
                                    lineWidth = w_jtanWaku / scale;
                                    title = jtanSectionTitle;
                                    if (aryNum == 2 || aryNum == 4 || aryNum == 6 || aryNum == 8)
                                    {
                                        title = cantiLeverEndTitle;
                                    }

                                    origin = titlePoints[2];

                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtnote.width = w_jtanWaku;// w_HugowakuMax / 3;// txtNote.LineWidth;
                                }
                                //                trans.Commit();
                                #endregion

                                hidariUe += new Revit.DB.XYZ(0, -(positionFrameHeight / 304.8 * scale), 0);
                            }
                        }

                        hidariSita = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + otherHeight), 0);
                        migiUe = hidariUe + new Revit.DB.XYZ(hugoMaxX + otherWidth, 0, 0);
                        migiSita = new Revit.DB.XYZ(migiUe.X, hidariSita.Y, migiSita.Z);

                        // 断面枠と配筋枠を作成
                        l = cmpElements.CreateBoundLine(hidariSita, migiSita);
                        cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                        l = cmpElements.CreateBoundLine(migiUe, migiSita);
                        cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                        // 上端筋枠
                        Revit.DB.XYZ point1 = hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                        Revit.DB.XYZ point2 = migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                        Collections.Generic.IList<Revit.DB.XYZ> uwabakinTitlePoints = cmpService.PositionFrameTitlePoints(point1,
                                                                                                                          arrangementFrameHeight / 304.8 * scale,
                                                                                                                          leftSpace / 304.8 * scale,
                                                                                                                          centerSpace / 304.8 * scale,
                                                                                                                          rightSpace / 304.8 * scale,
                                                                                                                          beamSecWidthAryByMark);
                        if (positionSpaceLineShow == 0)
                        {
                            Collections.Generic.IList<Revit.DB.Line> uwabaLines = cmpService.PositionFrameSpaceLines(point1,
                                                                                                                     arrangementFrameHeight / 304.8 * scale,
                                                                                                                     leftSpace / 304.8 * scale,
                                                                                                                     centerSpace / 304.8 * scale,
                                                                                                                     beamSecWidthAryByMark);
                            foreach (Revit.DB.Line stirrupl in uwabaLines)
                            {
                                crvFrameAry.Append(stirrupl);
                            }
                        }

                        // 下端筋枠
                        point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                        point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                        Collections.Generic.IList<Revit.DB.XYZ> sitabakinTitlePoints = cmpService.PositionFrameTitlePoints(point1,
                                                                                                                           arrangementFrameHeight / 304.8 * scale,
                                                                                                                           leftSpace / 304.8 * scale,
                                                                                                                           centerSpace / 304.8 * scale,
                                                                                                                           rightSpace / 304.8 * scale,
                                                                                                                           beamSecWidthAryByMark);
                        if (positionSpaceLineShow == 0)
                        {
                            Collections.Generic.IList<Revit.DB.Line> sitabaLines = cmpService.PositionFrameSpaceLines(point1,
                                                                                                                      arrangementFrameHeight / 304.8 * scale,
                                                                                                                      leftSpace / 304.8 * scale,
                                                                                                                      centerSpace / 304.8 * scale,
                                                                                                                      beamSecWidthAryByMark);
                            foreach (Revit.DB.Line stirrupl in sitabaLines)
                            {
                                crvFrameAry.Append(stirrupl);
                            }
                        }

                        // 肋筋枠
                        point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                        point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                        l = cmpElements.CreateBoundLine(point1, point2);
                        cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                        Revit.DB.XYZ centerStirrupFrame = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(point2.X, point2.Y - (arrangementFrameHeight / 304.8 * scale), point2.Z));

                        Collections.Generic.IList<Revit.DB.XYZ> stirrupTitlePoints = cmpService.PositionFrameTitlePoints(point1,
                                                                                                                         arrangementFrameHeight / 304.8 * scale,
                                                                                                                         leftSpace / 304.8 * scale,
                                                                                                                         centerSpace / 304.8 * scale,
                                                                                                                         rightSpace / 304.8 * scale,
                                                                                                                         beamSecWidthAryByMark);

                        // 肋筋を断面ごとに表示
                        if (stirrupFrameShow == 1 || cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == false)
                        {
                            if (positionSpaceLineShow == 0)
                            {
                                Collections.Generic.IList<Revit.DB.Line> stirrupFrameSpaceLines = cmpService.PositionFrameSpaceLines(point1,
                                                                                                                                     arrangementFrameHeight / 304.8 * scale,
                                                                                                                                     leftSpace / 304.8 * scale,
                                                                                                                                     centerSpace / 304.8 * scale,
                                                                                                                                     beamSecWidthAryByMark);
                                foreach (Revit.DB.Line stirrupl in stirrupFrameSpaceLines)
                                {
                                    crvFrameAry.Append(stirrupl);
                                }
                            }
                        }

                        // 腹筋枠
                        Collections.Generic.IList<Revit.DB.XYZ> harakinTitlePoints = new Collections.Generic.List<Revit.DB.XYZ>();
                        Revit.DB.XYZ centerWebFrame = new Revit.DB.XYZ();

                        if (webFrameShow != 2)
                        {
                            point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                            centerWebFrame = cmpGeometry.Center2Point(point1, new Revit.DB.XYZ(point2.X, point2.Y - (arrangementFrameHeight / 304.8 * scale), point2.Z));

                            harakinTitlePoints = cmpService.PositionFrameTitlePoints(point1,
                                                                                     arrangementFrameHeight / 304.8 * scale,
                                                                                     leftSpace / 304.8 * scale,
                                                                                     centerSpace / 304.8 * scale,
                                                                                     rightSpace / 304.8 * scale,
                                                                                     beamSecWidthAryByMark);

                            // 腹筋を断面ごとに表示
                            if (webFrameShow == 1 || cmpService.IsSameWebBySection(data.Rows[currentNum]) == false)
                            {
                                if (positionSpaceLineShow == 0)
                                {
                                    Collections.Generic.IList<Revit.DB.Line> stirrupFrameSpaceLines = cmpService.PositionFrameSpaceLines(point1,
                                                                                                                                         arrangementFrameHeight / 304.8 * scale,
                                                                                                                                         leftSpace / 304.8 * scale,
                                                                                                                                         centerSpace / 304.8 * scale,
                                                                                                                                         beamSecWidthAryByMark);
                                    foreach (Revit.DB.Line stirrupl in stirrupFrameSpaceLines)
                                    {
                                        crvFrameAry.Append(stirrupl);
                                    }
                                }
                            }
                        }

                        // 追加枠
                        if (addFrameNumber > 0)
                        {
                            point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                            l = cmpElements.CreateBoundLine(point1, point2);
                            cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                            for (int addFrameNum = 1; addFrameNum < addFrameNumber; ++addFrameNum)
                            {
                                point1 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                point2 += new Revit.DB.XYZ(0, -(arrangementFrameHeight / 304.8 * scale), 0);
                                l = cmpElements.CreateBoundLine(point1, point2);
                                cmpElements.NotNullCurveSet(ref crvFrameAry, l);
                            }
                        }

                        // 符号と階の組み合わせに該当しない場合
                        if (gaito == false)
                        {
                            // 斜線を作成
                            l = cmpElements.CreateBoundLine(migiUe, hidariUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));
                            cmpElements.NotNullCurveSet(ref crvFrameAry, l);

                            hidariUe = hidariSita;

                            isSyasen = true;

                            continue;
                        }

                        int rebarCount = 0;
                        int hosoCount = 0;

                        // エラーステップ
                        int isSyukinUeSet = 0;
                        int isSyukinSitaSet = 0;
                        bool isStirrupSet = true;
                        bool isWebSet = true;

                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                        // 片持ち?
                        bool isCanti = false;
                        if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                        {
                            isCanti = false;
                        }
                        else
                        {
                            isCanti = true;
                        }

                        // 断面を作成
                        if (secTypeNum == 0)
                        {
                            if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                            {
                                #region 全断

                                Revit.DB.XYZ centerRectangle = hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] / 2, -(levelMaxY / 2 + topSpace / 304.8 * scale), 0);//cmpGeometry.Center2Point(hidariUe, migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));

                                double haba1 = (double)data.Rows[currentNum][s_B];
                                double takasa1 = (double)data.Rows[currentNum][s_D];

                                if (haba1 <= 0 || takasa1 <= 0)
                                {
                                    //writeErr += level + hugoName + "(" + allSectionTitle + ") : " + SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_GIRDERXORY") + "\r\n";
                                    //continue;
                                }

                                double sabun = levelMaxY - takasa1;
                                centerRectangle += new Revit.DB.XYZ(0, sabun / 2, 0);

                                Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs = cmpGeometry.CrvRectangle(haba1, takasa1, centerRectangle);

                                cmpElements.AddCrvByAry(ref crvStrctAry, rectangleCrvs);

                                Collections.Generic.IList<Revit.DB.DetailLine> dLines = new Collections.Generic.List<Revit.DB.DetailLine>();

                                trans.Start("躯体作成");
                                foreach (Revit.DB.Curve crv in rectangleCrvs)
                                {
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                    dc.LineStyle = bodyLineType;
                                    dLines.Add(dc as Revit.DB.DetailLine);
                                }

                                // 寸法線
                                cmpGeometry.CreateDimensionsRectangleBottomLeft(dLines, dimType, vp);

                                trans.Commit();
                                crvStrctAry.Clear();

                                // 配筋
                                string strRet = cmpService.CreateRebar_Beam(data, currentNum, centerRectangle, 0, rvtUiApp, ref isSyukinUeSet, ref isSyukinSitaSet, ref isStirrupSet, ref isWebSet, isCanti);

                                if (strRet != "")
                                {
                                    writeErr += level + hugoName + "(" + allSectionTitle + ") : " + "\r\n" + strRet + "\r\n";
                                    strRet = level + hugoName + "(" + allSectionTitle + ") : " + "\r\n" + strRet;

                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe,
                                                                                        hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + rightSpace / 304.8 * scale,
                                                                                                                    -(levelMaxY + topSpace / 304.8 * scale + bottomSpace / 304.8 * scale), 0));

                                    //                  trans.Start("Error Message");
                                    txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtnote.width = w_itanWaku;// w_HugowakuMax;
                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                    //                  trans.Commit();
                                }

                                #region 主筋本数、径、肋筋本数、径、腹筋本数、径

                                if (isSyukinUeSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    //上端筋
                                    if (isCanti == false)
                                    {
                                        rebarCount = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinItanUe2danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinItanUe3danHutokeiHonsu];

                                        hosoCount = (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinItanUe2danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinItanUe3danHosokeiHonsu];

                                        title = "";
                                        if (rebarCount > 1 && "" != (string)data.Rows[currentNum][syukinItanUeHutokei])
                                        {
                                            title = rebarCount + "-" + (string)data.Rows[currentNum][syukinItanUeHutokei];

                                            if (hosoCount > 0 && (string)data.Rows[currentNum][syukinItanUeHosokei] != "")
                                            {
                                                title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinItanUeHosokei];
                                            }
                                        }
                                    }
                                    else
                                    {
                                        rebarCount = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinItanUe2danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinItanUe3danHutokeiHonsu];

                                        hosoCount = (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinItanUe2danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinItanUe3danHosokeiHonsu];
                                    }

                                    origin = uwabakinTitlePoints[0];
                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    if (title != "")
                                    {
                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax;
                                    }

                                    //                  trans.Commit();
                                }

                                if (isSyukinSitaSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 下端筋
                                    rebarCount = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinItanSita2danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinItanSita3danHutokeiHonsu];

                                    hosoCount = (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinItanSita2danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinItanSita3danHosokeiHonsu];

                                    title = "";
                                    if (rebarCount > 1 && (string)data.Rows[currentNum][syukinItanSitaHutokei] != "")
                                    {
                                        title = rebarCount + "-" + (string)data.Rows[currentNum][syukinItanSitaHutokei];

                                        if (hosoCount > 0 && (string)data.Rows[currentNum][syukinItanSitaHosokei] != "")
                                        {
                                            title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinItanSitaHosokei];
                                        }
                                    }

                                    origin = sitabakinTitlePoints[0];
                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    if (title != "")
                                    {
                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax;// hugoMaxX;
                                    }

                                    //                  trans.Commit();
                                }

                                if (strRet.Contains("IDS_ERR_HUTO_FAMILY_NOTSET") == false &&
                                    strRet.Contains("IDS_ERR_HOSO_FAMILY_NOTSET") == false &&
                                    rectangleCrvs.Count == 4)
                                {
                                    #region 断面枠の本数表示

                                    if (rebarShow == 1)
                                    {
                                        //                    trans.Start("RebarNumberShow");

                                        int ue1dan = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu];
                                        int sita1dan = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu];

                                        int length = 0;

                                        if (ue1dan > sita1dan)
                                        {
                                            length = ue1dan.ToString().Length;
                                        }
                                        else
                                        {
                                            length = sita1dan.ToString().Length;
                                        }

                                        double txtMaxW = 0;
                                        Collections.Generic.IList<Revit.DB.TextNote> tntAry = new Collections.Generic.List<Revit.DB.TextNote>();

                                        if (isSyukinUeSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle);

                                            Revit.DB.XYZ txtNoteOrigin = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarTop.Y, rectanglePoints[1].Z);

                                            //int ue1dan = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu];
                                            int ue2dan = (int)data.Rows[currentNum][syukinItanUe2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe2danHosokeiHonsu];
                                            int ue3dan = (int)data.Rows[currentNum][syukinItanUe3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe3danHosokeiHonsu];

                                            title = ue1dan.ToString();
                                            if (ue2dan > 0 && isSyukinUeSet > 1)
                                            {
                                                title += "\r\n" + ue2dan.ToString();
                                                if (ue3dan > 0 && isSyukinUeSet > 2)
                                                {
                                                    title += "\r\n" + ue3dan.ToString();
                                                }
                                            }

                                            //int length = ue1dan.ToString().Length;
                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;// w_Komokuwaku / scale;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, txtNoteOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtNote.Width;
                                            tntAry.Add(txtNote);

                                            //txtNote.Location.Move(new Revit.DB.XYZ(txtNote.Width / 2, 0, 0));

                                            //txtNote.LookupParameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                        }

                                        if (isSyukinSitaSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle);

                                            Revit.DB.XYZ innerBottom = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarBtm.Y, rectanglePoints[1].Z);

                                            //int sita1dan = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu];
                                            int sita2dan = (int)data.Rows[currentNum][syukinItanSita2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita2danHosokeiHonsu];
                                            int sita3dan = (int)data.Rows[currentNum][syukinItanSita3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita3danHosokeiHonsu];

                                            title = sita1dan.ToString();
                                            if (sita2dan > 0 && isSyukinSitaSet > 1)
                                            {
                                                title = sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                if (sita3dan > 0 && isSyukinSitaSet > 2)
                                                {
                                                    title = sita3dan.ToString() + "\r\n" + sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                }
                                            }

                                            //int length = sita1dan.ToString().Length;
                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;// w_Komokuwaku / scale;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerBottom, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtMaxW > txtNote.Width ? txtMaxW : txtNote.Width;
                                            tntAry.Add(txtNote);

                                            //txtNote.Location.Move(new Revit.DB.XYZ(txtNote.Width / 2, 0, 0));

                                            //txtNote.LookupParameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                        }

                                        //foreach (Revit.DB.TextNote tnt in tntAry)
                                        //{
                                        //  tnt.Width = txtMaxW;
                                        //  tnt.Location.Move(new Revit.DB.XYZ(tnt.Width / 2, 0, 0));
                                        //  tnt.get_Parameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                        //}

                                        //                    trans.Commit();
                                    }

                                    #endregion
                                }

                                if (isStirrupSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 肋筋
                                    title = "";

                                    if ((int)data.Rows[currentNum][s_Stirrup_Number] > 1 && (string)data.Rows[currentNum][s_Stirrup_Diameter] != "" && (double)data.Rows[currentNum][s_Stirrup_Pitch] > 0)
                                    {
                                        if (stirrupBracketShow == 0)
                                        {
                                            title = (int)data.Rows[currentNum][s_Stirrup_Number] + stirrupSpaceSymbol + (string)data.Rows[currentNum][s_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][s_Stirrup_Pitch];
                                        }
                                        else
                                        {
                                            title = "[" + (int)data.Rows[currentNum][s_Stirrup_Number] + "] " + (string)data.Rows[currentNum][s_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][s_Stirrup_Pitch];
                                        }
                                    }

                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;
                                    origin = stirrupTitlePoints[0];

                                    if (title != "")
                                    {
                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax;// hugoMaxX;
                                    }

                                    //                  trans.Commit();
                                }

                                if (isWebSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 腹筋
                                    if (webFrameShow != 2)
                                    {
                                        title = "";

                                        if ((int)data.Rows[currentNum][s_Web_Number] > 0 && (string)data.Rows[currentNum][s_Web_Diameter] != "")
                                        {
                                            title = (int)data.Rows[currentNum][s_Web_Number] + "-" + (string)data.Rows[currentNum][s_Web_Diameter];

                                            if ((int)data.Rows[currentNum][s_Web_Number] == 0)
                                            {
                                                title = "-";
                                            }

                                            lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;
                                            origin = harakinTitlePoints[0];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_itanWaku;// w_HugowakuMax;// hugoMaxX;
                                        }
                                    }

                                    //                  trans.Commit();
                                }

                                #endregion

                                #endregion
                            }
                            else
                            {
                                // 片持ち梁
                                #region 全断

                                Revit.DB.XYZ centerRectangle = hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] / 2, -(levelMaxY / 2 + topSpace / 304.8 * scale), 0);//cmpGeometry.Center2Point(hidariUe, migiUe + new Revit.DB.XYZ(0, -(levelMaxY + (topSpace + bottomSpace) / 304.8 * scale), 0));

                                double haba1 = (double)data.Rows[currentNum][cmpParameters.MototanHarihaba];
                                double takasa1 = (double)data.Rows[currentNum][cmpParameters.MototanHarisei];

                                if (haba1 <= 0 || takasa1 <= 0)
                                {
                                    //writeErr += level + hugoName + "(" + allSectionTitle + ") : " + SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_GIRDERXORY") + "\r\n";
                                    //continue;
                                }

                                double sabun = levelMaxY - takasa1;
                                centerRectangle += new Revit.DB.XYZ(0, sabun / 2, 0);

                                Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs = cmpGeometry.CrvRectangle(haba1, takasa1, centerRectangle);

                                cmpElements.AddCrvByAry(ref crvStrctAry, rectangleCrvs);

                                Collections.Generic.IList<Revit.DB.DetailLine> dLines = new Collections.Generic.List<Revit.DB.DetailLine>();

                                trans.Start("躯体作成");
                                foreach (Revit.DB.Curve crv in rectangleCrvs)
                                {
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                    dc.LineStyle = bodyLineType;
                                    dLines.Add(dc as Revit.DB.DetailLine);
                                }

                                // 寸法線
                                cmpGeometry.CreateDimensionsRectangleBottomLeft(dLines, dimType, vp);

                                trans.Commit();
                                crvStrctAry.Clear();

                                // 配筋
                                string strRet = cmpService.CreateRebar_Beam(data, currentNum, centerRectangle, 0, rvtUiApp, ref isSyukinUeSet, ref isSyukinSitaSet, ref isStirrupSet, ref isWebSet, isCanti);

                                if (strRet != "")
                                {
                                    writeErr += level + hugoName + "(" + allSectionTitle + ") : " + "\r\n" + strRet + "\r\n";
                                    strRet = level + hugoName + "(" + allSectionTitle + ") : " + "\r\n" + strRet;

                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe,
                                                                                        hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + rightSpace / 304.8 * scale,
                                                                                                                    -(levelMaxY + topSpace / 304.8 * scale + bottomSpace / 304.8 * scale), 0));

                                    //                  trans.Start("Error Message");
                                    txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    if (otherTNT != null)
                                    {
                                        txtNote.ChangeTypeId(otherTNT.Id);
                                    }
                                    //txtnote.width = w_itanWaku;// w_HugowakuMax;
                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();
                                    //                  trans.Commit();
                                }

                                #region 主筋本数、径、肋筋本数、径、腹筋本数、径

                                if (isSyukinUeSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    //上端筋
                                    rebarCount = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin1danHutokinHonsu] +
                                                   (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin2danHutokinHonsu] +
                                                   (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin3danHutokinHonsu];

                                    hosoCount = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin1danHosokinHonsu] +
                                                (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin2danHosokinHonsu] +
                                                (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin3danHosokinHonsu];

                                    title = "";
                                    if (rebarCount > 1 && "" != (string)data.Rows[currentNum][cmpParameters.MototanUeSyukinHutokei])
                                    {
                                        title = rebarCount + "-" + (string)data.Rows[currentNum][cmpParameters.MototanUeSyukinHutokei];

                                        if (hosoCount > 0 && (string)data.Rows[currentNum][cmpParameters.MototanUeSyukinHosokei] != "")
                                        {
                                            title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][cmpParameters.MototanUeSyukinHosokei];
                                        }
                                    }

                                    origin = uwabakinTitlePoints[0];
                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    if (title != "")
                                    {
                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax;
                                    }

                                    //                  trans.Commit();
                                }

                                if (isSyukinSitaSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 下端筋
                                    rebarCount = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin1danHutokinHonsu] +
                                                 (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin2danHutokinHonsu] +
                                                 (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin3danHutokinHonsu];

                                    hosoCount = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin1danHosokinHonsu] +
                                                (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin2danHosokinHonsu] +
                                                (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin3danHosokinHonsu];

                                    title = "";
                                    if (rebarCount > 1 && (string)data.Rows[currentNum][cmpParameters.MototanSitaSyukinHutokei] != "")
                                    {
                                        title = rebarCount + "-" + (string)data.Rows[currentNum][cmpParameters.MototanSitaSyukinHutokei];

                                        if (hosoCount > 0 && (string)data.Rows[currentNum][cmpParameters.MototanSitaSyukinHosokei] != "")
                                        {
                                            title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][cmpParameters.MototanSitaSyukinHosokei];
                                        }
                                    }

                                    origin = sitabakinTitlePoints[0];
                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    if (title != "")
                                    {
                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax;// hugoMaxX;
                                    }

                                    //                  trans.Commit();
                                }

                                if (strRet.Contains("IDS_ERR_HUTO_FAMILY_NOTSET") == false &&
                                    strRet.Contains("IDS_ERR_HOSO_FAMILY_NOTSET") == false &&
                                    rectangleCrvs.Count == 4)
                                {
                                    #region 断面枠の本数表示

                                    if (rebarShow == 1)
                                    {
                                        //                    trans.Start("RebarNumberShow");

                                        int ue1dan = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin1danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin1danHosokinHonsu];
                                        int sita1dan = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin1danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin1danHosokinHonsu];

                                        int length = 0;

                                        if (ue1dan > sita1dan)
                                        {
                                            length = ue1dan.ToString().Length;
                                        }
                                        else
                                        {
                                            length = sita1dan.ToString().Length;
                                        }

                                        double txtMaxW = 0;
                                        Collections.Generic.IList<Revit.DB.TextNote> tntAry = new Collections.Generic.List<Revit.DB.TextNote>();

                                        if (isSyukinUeSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle);

                                            Revit.DB.XYZ txtNoteOrigin = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarTop.Y, rectanglePoints[1].Z);

                                            //int ue1dan = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu];
                                            int ue2dan = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin2danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin2danHosokinHonsu];
                                            int ue3dan = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin3danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin3danHosokinHonsu];

                                            title = ue1dan.ToString();
                                            if (ue2dan > 0 && isSyukinUeSet > 1)
                                            {
                                                title += "\r\n" + ue2dan.ToString();
                                                if (ue3dan > 0 && isSyukinUeSet > 2)
                                                {
                                                    title += "\r\n" + ue3dan.ToString();
                                                }
                                            }

                                            //int length = ue1dan.ToString().Length;
                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;// w_Komokuwaku / scale;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, txtNoteOrigin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            if (otherTNT != null)
                                            {
                                                txtNote.ChangeTypeId(otherTNT.Id);
                                            }

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtNote.Width;
                                            tntAry.Add(txtNote);

                                            //txtNote.Location.Move(new Revit.DB.XYZ(txtNote.Width / 2, 0, 0));

                                            //txtNote.LookupParameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                        }

                                        if (isSyukinSitaSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle);

                                            Revit.DB.XYZ innerBottom = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarBtm.Y, rectanglePoints[1].Z);

                                            //int sita1dan = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu];
                                            int sita2dan = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin2danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin2danHosokinHonsu];
                                            int sita3dan = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin3danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin3danHosokinHonsu];

                                            title = sita1dan.ToString();
                                            if (sita2dan > 0 && isSyukinSitaSet > 1)
                                            {
                                                title = sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                if (sita3dan > 0 && isSyukinSitaSet > 2)
                                                {
                                                    title = sita3dan.ToString() + "\r\n" + sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                }
                                            }

                                            //int length = sita1dan.ToString().Length;
                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;// w_Komokuwaku / scale;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerBottom, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            if (otherTNT != null)
                                            {
                                                txtNote.ChangeTypeId(otherTNT.Id);
                                            }

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtMaxW > txtNote.Width ? txtMaxW : txtNote.Width;
                                            tntAry.Add(txtNote);

                                            //txtNote.Location.Move(new Revit.DB.XYZ(txtNote.Width / 2, 0, 0));

                                            //txtNote.LookupParameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                        }

                                        //foreach (Revit.DB.TextNote tnt in tntAry)
                                        //{
                                        //  tnt.Width = txtMaxW;
                                        //  tnt.Location.Move(new Revit.DB.XYZ(tnt.Width / 2, 0, 0));
                                        //  tnt.get_Parameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                        //}

                                        //                    trans.Commit();
                                    }

                                    #endregion
                                }

                                if (isStirrupSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 肋筋
                                    title = "";

                                    if ((int)data.Rows[currentNum][cmpParameters.MototanAbarakinHonsu] > 1 && (string)data.Rows[currentNum][cmpParameters.MototanAbarakinkei] != "" && (double)data.Rows[currentNum][cmpParameters.MototanAbarakinPitch] > 0)
                                    {
                                        if (stirrupBracketShow == 0)
                                        {
                                            title = (int)data.Rows[currentNum][cmpParameters.MototanAbarakinHonsu] + stirrupSpaceSymbol + (string)data.Rows[currentNum][cmpParameters.MototanAbarakinkei] + "@" + (double)data.Rows[currentNum][cmpParameters.MototanAbarakinPitch];
                                        }
                                        else
                                        {
                                            title = "[" + (int)data.Rows[currentNum][cmpParameters.MototanAbarakinHonsu] + "] " + (string)data.Rows[currentNum][cmpParameters.MototanAbarakinkei] + "@" + (double)data.Rows[currentNum][cmpParameters.MototanAbarakinPitch];
                                        }
                                    }

                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;
                                    origin = stirrupTitlePoints[0];

                                    if (title != "")
                                    {
                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax;// hugoMaxX;
                                    }

                                    //                  trans.Commit();
                                }

                                if (isWebSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 腹筋
                                    if (webFrameShow != 2)
                                    {
                                        title = "";

                                        if ((int)data.Rows[currentNum][cmpParameters.MototanHarakinHonsu] > 0 && (string)data.Rows[currentNum][cmpParameters.MototanHarakinkei] != "")
                                        {
                                            title = (int)data.Rows[currentNum][cmpParameters.MototanHarakinHonsu] + "-" + (string)data.Rows[currentNum][cmpParameters.MototanHarakinkei];

                                            if ((int)data.Rows[currentNum][cmpParameters.MototanHarakinHonsu] == 0)
                                            {
                                                title = "-";
                                            }

                                            lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;
                                            origin = harakinTitlePoints[0];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_itanWaku;// w_HugowakuMax;// hugoMaxX;
                                        }
                                    }

                                    //                  trans.Commit();
                                }

                                #endregion

                                #endregion
                            }
                        }
                        else if (secTypeNum == 1)
                        {
                            // 一般梁
                            if (aryNum == 1 || aryNum == 3 || aryNum == 5 || aryNum == 7)
                            {
                                #region 始端、中央

                                #region 始端部

                                Revit.DB.XYZ centerRectangle1 = hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] / 2,
                                                                                            -(levelMaxY / 2 + topSpace / 304.8 * scale),
                                                                                            0);

                                double haba1 = (double)data.Rows[currentNum][s_B];
                                double takasa1 = (double)data.Rows[currentNum][s_D];

                                double sabun = levelMaxY - takasa1;
                                centerRectangle1 += new Revit.DB.XYZ(0, sabun / 2, 0);

                                string str = (aryNum == 2 || aryNum == 4 || aryNum == 6 || aryNum == 8) ? cantiLeverStartTitle : edgeTitle;

                                //if (haba1 <= 0 || takasa1 <= 0)
                                //{
                                //  // 3478でなければedgeTitle
                                //  // 3478であればcanti
                                //  //writeErr += level + hugoName + "(" + str + ") : " + SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_GIRDERXORY") + "\r\n";
                                //  // continue;
                                //}
                                //else
                                {
                                    Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs1 = cmpGeometry.CrvRectangle(haba1, takasa1, centerRectangle1);
                                    Collections.Generic.IList<Revit.DB.DetailLine> dLines1 = new Collections.Generic.List<Revit.DB.DetailLine>();

                                    trans.Start("躯体作成");

                                    foreach (Revit.DB.Curve crv in rectangleCrvs1)
                                    {
                                        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                        dc.LineStyle = bodyLineType;
                                        dLines1.Add(dc as Revit.DB.DetailLine);
                                    }

                                    // 幅寸法
                                    // すべて表示
                                    if (widthDimShow == 0)
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines1, dimType);
                                    }
                                    // 左を基準
                                    else if (widthDimShow == 1)
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines1, dimType);
                                    }
                                    // 中央を基準
                                    else if (widthDimShow == 2)
                                    {
                                        if (cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[0], -9) != cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[1], -9))
                                        {
                                            cmpGeometry.CreateDimensionRectBottom(dLines1, dimType);
                                        }
                                    }

                                    // 高さ寸法
                                    // すべて表示
                                    if (heightDimShow == 0)
                                    {
                                        cmpGeometry.CreateDimensionRectLeft(dLines1, dimType);
                                    }
                                    // 左を基準
                                    else if (heightDimShow == 1)
                                    {
                                        cmpGeometry.CreateDimensionRectLeft(dLines1, dimType);
                                    }

                                    trans.Commit();

                                    // 配筋
                                    string strRet = cmpService.CreateRebar_Beam(data, currentNum, centerRectangle1, 0, rvtUiApp, ref isSyukinUeSet, ref isSyukinSitaSet, ref isStirrupSet, ref isWebSet, isCanti);

                                    if (strRet != "")
                                    {
                                        writeErr += level + hugoName + "(" + str + ") : " + "\r\n" + strRet + "\r\n";
                                        strRet = level + hugoName + "(" + str + ") : " + "\r\n" + strRet;

                                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe,
                                                                                            hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 2 / 304.8 * scale,
                                                                                                                        -(levelMaxY + topSpace / 304.8 * scale + bottomSpace / 304.8 * scale),
                                                                                                                        0));

                                        //                    trans.Start("Error Message");
                                        txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;
                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();
                                        //                    trans.Commit();
                                    }

                                    // 始端のテキスト
                                    #region 主筋本数、径、肋筋本数、径、腹筋本数、径

                                    if (isSyukinUeSet > 2)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 上端筋始端
                                        rebarCount = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinItanUe2danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinItanUe3danHutokeiHonsu];

                                        hosoCount = (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinItanUe2danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinItanUe3danHosokeiHonsu];

                                        title = "";
                                        if (rebarCount > 1 && (string)data.Rows[currentNum][syukinItanUeHutokei] != "")
                                        {
                                            title = rebarCount + "-" + (string)data.Rows[currentNum][syukinItanUeHutokei];
                                            if (hosoCount > 0 && (string)data.Rows[currentNum][syukinItanUeHosokei] != "")
                                            {
                                                title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinItanUeHosokei];
                                            }

                                            origin = uwabakinTitlePoints[0];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (isSyukinSitaSet > 2)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 下端筋始端
                                        rebarCount = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinItanSita2danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinItanSita3danHutokeiHonsu];

                                        hosoCount = (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinItanSita2danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinItanSita3danHosokeiHonsu];

                                        title = "";

                                        if (rebarCount > 1 && (string)data.Rows[currentNum][syukinItanSitaHutokei] != "")
                                        {
                                            title = rebarCount + "-" + (string)data.Rows[currentNum][syukinItanSitaHutokei];
                                            if (hosoCount > 0 && (string)data.Rows[currentNum][syukinItanSitaHosokei] != "")
                                            {
                                                title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinItanSitaHosokei];
                                            }

                                            origin = sitabakinTitlePoints[0];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (strRet.Contains("IDS_ERR_HUTO_FAMILY_NOTSET") == false &&
                                        strRet.Contains("IDS_ERR_HOSO_FAMILY_NOTSET") == false &&
                                        rectangleCrvs1.Count == 4)
                                    {
                                        #region 断面枠の本数表示

                                        if (rebarShow == 1)
                                        {
                                            //                      trans.Start("RebarNumberShow");

                                            int ue1dan = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu];
                                            int sita1dan = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu];

                                            int length = 0;

                                            if (ue1dan > sita1dan)
                                            {
                                                length = ue1dan.ToString().Length;
                                            }
                                            else
                                            {
                                                length = sita1dan.ToString().Length;
                                            }

                                            double txtMaxW = 0;
                                            Collections.Generic.IList<Revit.DB.TextNote> tntAry = new Collections.Generic.List<Revit.DB.TextNote>();

                                            if (isSyukinUeSet > 0)
                                            {
                                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle1);

                                                Revit.DB.XYZ innerTop = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarTop.Y, rectanglePoints[1].Z);

                                                //int ue1dan = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu];
                                                int ue2dan = (int)data.Rows[currentNum][syukinItanUe2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe2danHosokeiHonsu];
                                                int ue3dan = (int)data.Rows[currentNum][syukinItanUe3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe3danHosokeiHonsu];

                                                title = ue1dan.ToString();
                                                if (ue2dan > 0 && isSyukinUeSet > 1)
                                                {
                                                    title += "\r\n" + ue2dan.ToString();
                                                    if (ue3dan > 0 && isSyukinUeSet > 2)
                                                    {
                                                        title += "\r\n" + ue3dan.ToString();
                                                    }
                                                }

                                                double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                                double a = 0.000057930;
                                                double b = 0.000164979;

                                                lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerTop, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}

                                                trans.Start("Regenerate");
                                                rvtDbDoc.Regenerate();
                                                trans.Commit();

                                                //txtNote.Text = title;

                                                txtMaxW = txtNote.Width;
                                                tntAry.Add(txtNote);
                                            }

                                            if (isSyukinSitaSet > 0)
                                            {
                                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle1);
                                                Revit.DB.XYZ innerBottom = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarBtm.Y, rectanglePoints[1].Z);

                                                //int sita1dan = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu];
                                                int sita2dan = (int)data.Rows[currentNum][syukinItanSita2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita2danHosokeiHonsu];
                                                int sita3dan = (int)data.Rows[currentNum][syukinItanSita3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita3danHosokeiHonsu];

                                                title = sita1dan.ToString();
                                                if (sita2dan > 0 && isSyukinSitaSet > 1)
                                                {
                                                    title = sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                    if (sita3dan > 0 && isSyukinSitaSet > 2)
                                                    {
                                                        title = sita3dan.ToString() + "\r\n" + sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                    }
                                                }

                                                double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                                double a = 0.000057930;
                                                double b = 0.000164979;

                                                lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerBottom, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}

                                                trans.Start("Regenerate");
                                                rvtDbDoc.Regenerate();
                                                trans.Commit();

                                                //txtNote.Text = title;

                                                txtMaxW = txtMaxW > txtNote.Width ? txtMaxW : txtNote.Width;
                                                tntAry.Add(txtNote);
                                            }

                                            //foreach (Revit.DB.TextNote tnt in tntAry)
                                            //{
                                            //  tnt.Width = txtMaxW;
                                            //  tnt.Location.Move(new Revit.DB.XYZ(tnt.Width / 2, 0, 0));
                                            //  tnt.get_Parameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                            //}

                                            //                      trans.Commit();
                                        }

                                        #endregion
                                    }

                                    if (isStirrupSet)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        // 肋筋
                                        // まとめて表示
                                        if (stirrupFrameShow == 0 && cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == true)
                                        {
                                        }
                                        // 断面別に表示
                                        else if (stirrupFrameShow == 1 || cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == false)
                                        {
                                            lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            // 肋筋始端
                                            if ((int)data.Rows[currentNum][s_Stirrup_Number] > 1 && (string)data.Rows[currentNum][s_Stirrup_Diameter] != "" && (double)data.Rows[currentNum][s_Stirrup_Pitch] > 0)
                                            {
                                                if (stirrupBracketShow == 0)
                                                {
                                                    title = (int)data.Rows[currentNum][s_Stirrup_Number] + stirrupSpaceSymbol + (string)data.Rows[currentNum][s_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][s_Stirrup_Pitch];
                                                }
                                                else
                                                {
                                                    title = "[" + (int)data.Rows[currentNum][s_Stirrup_Number] + "] " + (string)data.Rows[currentNum][s_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][s_Stirrup_Pitch];
                                                }

                                                origin = stirrupTitlePoints[0];
                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                            }
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (isWebSet)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        // 腹筋
                                        if (webFrameShow != 2)
                                        {
                                            // まとめて表示
                                            if (webFrameShow == 0 && cmpService.IsSameWebBySection(data.Rows[currentNum]) == true)
                                            {
                                            }
                                            else if (webFrameShow == 1 || cmpService.IsSameWebBySection(data.Rows[currentNum]) == false)
                                            {
                                                lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                // 腹筋始端
                                                if ((int)data.Rows[currentNum][s_Web_Number] > 1 && (string)data.Rows[currentNum][s_Web_Diameter] != "")
                                                {
                                                    title = (int)data.Rows[currentNum][s_Web_Number] + "-" + (string)data.Rows[currentNum][s_Web_Diameter];

                                                    if ((int)data.Rows[currentNum][s_Web_Number] == 0)
                                                    {
                                                        title = "-";
                                                    }

                                                    origin = harakinTitlePoints[0];

                                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                    //if (otherTNT != null)
                                                    //{
                                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                                    //}
                                                    //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                                }
                                            }
                                        }

                                        //                    trans.Commit();
                                    }

                                    #endregion
                                }

                                #endregion

                                #region 全断部（中央）

                                Revit.DB.XYZ centerRectangle2 = hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[1] / 2,
                                                                                            -(levelMaxY / 2 + topSpace / 304.8 * scale),
                                                                                            0);

                                double haba2 = (double)data.Rows[currentNum][c_B];
                                double takasa2 = (double)data.Rows[currentNum][c_D];

                                sabun = levelMaxY - takasa2;
                                centerRectangle2 += new Revit.DB.XYZ(0, sabun / 2, 0);

                                //if (haba2 <= 0 || takasa2 <= 0)
                                //{
                                //  //writeErr += level + hugoName + "(" + centerSectionTitle + ") : " + SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_GIRDERXORY") + "\r\n";
                                //  //continue;
                                //}
                                //else
                                {
                                    Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs2 = cmpGeometry.CrvRectangle(haba2, takasa2, centerRectangle2);
                                    Collections.Generic.IList<Revit.DB.DetailLine> dLines2 = new Collections.Generic.List<Revit.DB.DetailLine>();

                                    trans.Start("躯体作成");

                                    foreach (Revit.DB.Curve crv in rectangleCrvs2)
                                    {
                                        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                        dc.LineStyle = bodyLineType;
                                        dLines2.Add(dc as Revit.DB.DetailLine);
                                    }

                                    // 幅寸法
                                    // すべて表示
                                    if (widthDimShow == 0)
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines2, dimType);
                                    }
                                    // 左を基準
                                    else if (widthDimShow == 1)
                                    {
                                        if (cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[0], -9) != cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[1], -9))
                                        {
                                            cmpGeometry.CreateDimensionRectBottom(dLines2, dimType);
                                        }
                                    }
                                    // 中央を基準
                                    else if (widthDimShow == 2)
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines2, dimType);
                                    }

                                    // 高さ寸法
                                    // すべて表示
                                    if (heightDimShow == 0)
                                    {
                                        cmpGeometry.CreateDimensionRectLeft(dLines2, dimType);
                                    }
                                    // 左を基準
                                    else if (heightDimShow == 1)
                                    {
                                        if (cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][s_D], -9) != cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][c_D], -9))
                                        {
                                            cmpGeometry.CreateDimensionRectLeft(dLines2, dimType);
                                        }
                                    }

                                    trans.Commit();

                                    // 配筋
                                    string strRet = cmpService.CreateRebar_Beam(data, currentNum, centerRectangle2, 1, rvtUiApp, ref isSyukinUeSet, ref isSyukinSitaSet, ref isStirrupSet, ref isWebSet, isCanti);

                                    if (strRet != "")
                                    {
                                        writeErr += level + hugoName + "(" + centerSectionTitle + ") : " + "\r\n" + strRet + "\r\n";
                                        strRet = level + hugoName + "(" + centerSectionTitle + ") : " + "\r\n" + strRet;

                                        lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 2 / 304.8 * scale, 0, 0),
                                                                                            hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[1] + rightSpace / 304.8 * scale,
                                                                                                                        -(levelMaxY + topSpace / 304.8 * scale + bottomSpace / 304.8 * scale),
                                                                                                                        0));

                                        //                    trans.Start("Error Message");
                                        txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_chuohWaku;// w_HugowakuMax / 2;
                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();
                                        //                    trans.Commit();
                                    }

                                    // 中央のテキスト
                                    #region 主筋本数、径、肋筋本数、径、腹筋本数、径

                                    if (isSyukinUeSet > 2)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 上端筋中央
                                        rebarCount = (int)data.Rows[currentNum][syukinChuohUe1danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinChuohUe2danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinChuohUe3danHutokeiHonsu];

                                        hosoCount = (int)data.Rows[currentNum][syukinChuohUe1danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinChuohUe2danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinChuohUe3danHosokeiHonsu];

                                        title = "";

                                        if (rebarCount > 1 && (string)data.Rows[currentNum][syukinChuohUeHutokei] != "")
                                        {
                                            title = rebarCount + "-" + (string)data.Rows[currentNum][syukinChuohUeHutokei];
                                            if (hosoCount > 0 && (string)data.Rows[currentNum][syukinChuohUeHosokei] != "")
                                            {
                                                title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinChuohUeHosokei];
                                            }

                                            origin = uwabakinTitlePoints[1];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_chuohWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (isSyukinSitaSet > 2)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 下端筋中央
                                        rebarCount = (int)data.Rows[currentNum][syukinChuohSita1danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinChuohSita2danHutokeiHonsu] +
                                                     (int)data.Rows[currentNum][syukinChuohSita3danHutokeiHonsu];

                                        hosoCount = (int)data.Rows[currentNum][syukinChuohSita1danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinChuohSita2danHosokeiHonsu] +
                                                    (int)data.Rows[currentNum][syukinChuohSita3danHosokeiHonsu];

                                        title = "";

                                        if (rebarCount > 1 && (string)data.Rows[currentNum][syukinChuohSitaHutokei] != "")
                                        {
                                            title = rebarCount + "-" + (string)data.Rows[currentNum][syukinChuohSitaHutokei];
                                            if (hosoCount > 0 && (string)data.Rows[currentNum][syukinChuohSitaHosokei] != "")
                                            {
                                                title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinChuohSitaHosokei];
                                            }

                                            origin = sitabakinTitlePoints[1];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_chuohWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (strRet.Contains("IDS_ERR_HUTO_FAMILY_NOTSET") == false &&
                                        strRet.Contains("IDS_ERR_HOSO_FAMILY_NOTSET") == false &&
                                        rectangleCrvs2.Count == 4)
                                    {
                                        #region 断面枠の本数表示

                                        if (rebarShow == 1)
                                        {
                                            //                      trans.Start("RebarNumberShow");

                                            int ue1dan = (int)data.Rows[currentNum][syukinChuohUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohUe1danHosokeiHonsu];
                                            int sita1dan = (int)data.Rows[currentNum][syukinChuohSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohSita1danHosokeiHonsu];

                                            int length = 0;

                                            if (ue1dan > sita1dan)
                                            {
                                                length = ue1dan.ToString().Length;
                                            }
                                            else
                                            {
                                                length = sita1dan.ToString().Length;
                                            }

                                            double txtMaxW = 0;
                                            Collections.Generic.IList<Revit.DB.TextNote> tntAry = new Collections.Generic.List<Revit.DB.TextNote>();

                                            if (isSyukinUeSet > 0)
                                            {
                                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba2, takasa2, centerRectangle2);

                                                Revit.DB.XYZ innerTop = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarTop.Y, rectanglePoints[1].Z);

                                                //int ue1dan = (int)data.Rows[currentNum][syukinChuohUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohUe1danHosokeiHonsu];
                                                int ue2dan = (int)data.Rows[currentNum][syukinChuohUe2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohUe2danHosokeiHonsu];
                                                int ue3dan = (int)data.Rows[currentNum][syukinChuohUe3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohUe3danHosokeiHonsu];

                                                title = ue1dan.ToString();
                                                if (ue2dan > 0 && isSyukinUeSet > 1)
                                                {
                                                    title += "\r\n" + ue2dan.ToString();
                                                    if (ue3dan > 0 && isSyukinUeSet > 2)
                                                    {
                                                        title += "\r\n" + ue3dan.ToString();
                                                    }
                                                }

                                                double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                                double a = 0.000057930;
                                                double b = 0.000164979;

                                                lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerTop, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}

                                                trans.Start("Regenerate");
                                                rvtDbDoc.Regenerate();
                                                trans.Commit();

                                                //txtNote.Text = title;

                                                txtMaxW = txtMaxW > txtNote.Width ? txtMaxW : txtNote.Width;
                                                tntAry.Add(txtNote);
                                            }

                                            if (isSyukinSitaSet > 0)
                                            {
                                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba2, takasa2, centerRectangle2);

                                                Revit.DB.XYZ innerBottom = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarBtm.Y, rectanglePoints[1].Z);

                                                //int sita1dan = (int)data.Rows[currentNum][syukinChuohSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohSita1danHosokeiHonsu];
                                                int sita2dan = (int)data.Rows[currentNum][syukinChuohSita2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohSita2danHosokeiHonsu];
                                                int sita3dan = (int)data.Rows[currentNum][syukinChuohSita3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohSita3danHosokeiHonsu];

                                                title = sita1dan.ToString();
                                                if (sita2dan > 0 && isSyukinSitaSet > 1)
                                                {
                                                    title = sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                    if (sita3dan > 0 && isSyukinSitaSet > 2)
                                                    {
                                                        title = sita3dan.ToString() + "\r\n" + sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                    }
                                                }

                                                double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                                double a = 0.000057930;
                                                double b = 0.000164979;

                                                lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerBottom, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}

                                                trans.Start("Regenerate");
                                                rvtDbDoc.Regenerate();
                                                trans.Commit();

                                                //txtNote.Text = title;

                                                txtMaxW = txtMaxW > txtNote.Width ? txtMaxW : txtNote.Width;
                                                tntAry.Add(txtNote);
                                            }

                                            //foreach (Revit.DB.TextNote tnt in tntAry)
                                            //{
                                            //  tnt.Width = txtMaxW;
                                            //  tnt.Location.Move(new Revit.DB.XYZ(tnt.Width / 2, 0, 0));
                                            //  tnt.get_Parameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                            //}

                                            //                      trans.Commit();
                                        }

                                        #endregion
                                    }

                                    if (isStirrupSet)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        // 肋筋
                                        // まとめて表示
                                        if (stirrupFrameShow == 0 && cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == true)
                                        {
                                            lineWidth = w_HugowakuMax / scale;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            // 肋筋中央
                                            if ((int)data.Rows[currentNum][c_Stirrup_Number] > 1 && (string)data.Rows[currentNum][c_Stirrup_Diameter] != "" && (double)data.Rows[currentNum][c_Stirrup_Pitch] > 0)
                                            {
                                                if (stirrupBracketShow == 0)
                                                {
                                                    title = (int)data.Rows[currentNum][c_Stirrup_Number] + stirrupSpaceSymbol + (string)data.Rows[currentNum][c_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][c_Stirrup_Pitch];
                                                }
                                                else
                                                {
                                                    title = "[" + (int)data.Rows[currentNum][c_Stirrup_Number] + "] " + (string)data.Rows[currentNum][c_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][c_Stirrup_Pitch];
                                                }

                                                origin = centerStirrupFrame;
                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_HugowakuMax;// hugoMaxX;
                                            }
                                        }
                                        // 断面別に表示
                                        else if (stirrupFrameShow == 1 || cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == false)
                                        {
                                            lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            // 肋筋中央
                                            if ((int)data.Rows[currentNum][c_Stirrup_Number] > 1 && (string)data.Rows[currentNum][c_Stirrup_Diameter] != "" && (double)data.Rows[currentNum][c_Stirrup_Pitch] > 0)
                                            {
                                                if (stirrupBracketShow == 0)
                                                {
                                                    title = (int)data.Rows[currentNum][c_Stirrup_Number] + stirrupSpaceSymbol + (string)data.Rows[currentNum][c_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][c_Stirrup_Pitch];
                                                }
                                                else
                                                {
                                                    title = "[" + (int)data.Rows[currentNum][c_Stirrup_Number] + "] " + (string)data.Rows[currentNum][c_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][c_Stirrup_Pitch];
                                                }

                                                origin = stirrupTitlePoints[1];
                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_chuohWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                            }
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (isWebSet)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        // 腹筋
                                        if (webFrameShow != 2)
                                        {
                                            // まとめて表示
                                            if (webFrameShow == 0 && cmpService.IsSameWebBySection(data.Rows[currentNum]) == true)
                                            {
                                                lineWidth = w_HugowakuMax / scale;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                // 腹筋中央
                                                if ((int)data.Rows[currentNum][c_Web_Number] > 1 && (string)data.Rows[currentNum][c_Web_Diameter] != "")
                                                {
                                                    title = (int)data.Rows[currentNum][c_Web_Number] + "-" + (string)data.Rows[currentNum][c_Web_Diameter];

                                                    if ((int)data.Rows[currentNum][c_Web_Number] == 0)
                                                    {
                                                        title = "-";
                                                    }

                                                    origin = centerWebFrame;

                                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                    //if (otherTNT != null)
                                                    //{
                                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                                    //}
                                                    //txtnote.width = w_HugowakuMax;// hugoMaxX;
                                                }
                                            }
                                            else if (webFrameShow == 1 || cmpService.IsSameWebBySection(data.Rows[currentNum]) == false)
                                            {
                                                lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                // 腹筋中央
                                                if ((int)data.Rows[currentNum][c_Web_Number] > 1 && (string)data.Rows[currentNum][c_Web_Diameter] != "")
                                                {
                                                    title = (int)data.Rows[currentNum][c_Web_Number] + "-" + (string)data.Rows[currentNum][c_Web_Diameter];

                                                    if ((int)data.Rows[currentNum][c_Web_Number] == 0)
                                                    {
                                                        title = "-";
                                                    }

                                                    origin = harakinTitlePoints[1];

                                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                    //if (otherTNT != null)
                                                    //{
                                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                                    //}
                                                    //txtnote.width = w_chuohWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                                }
                                            }
                                        }

                                        //                    trans.Commit();
                                    }

                                    #endregion
                                }

                                #endregion

                                #endregion
                            }
                            // 片持ち梁
                            else if (aryNum == 2 || aryNum == 4 || aryNum == 6 || aryNum == 8)
                            {
                                #region 元端、先端

                                #region 元端部

                                Revit.DB.XYZ centerRectangle1 = hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] / 2,
                                                                                            -(levelMaxY / 2 + topSpace / 304.8 * scale),
                                                                                            0);

                                double haba1 = (double)data.Rows[currentNum][cmpParameters.MototanHarihaba];
                                double takasa1 = (double)data.Rows[currentNum][cmpParameters.MototanHarisei];

                                double sabun = levelMaxY - takasa1;
                                centerRectangle1 += new Revit.DB.XYZ(0, sabun / 2, 0);

                                string str = cantiLeverStartTitle;

                                //if (haba1 <= 0 || takasa1 <= 0)
                                //{
                                //  // 3478でなければedgeTitle
                                //  // 3478であればcanti
                                //  //writeErr += level + hugoName + "(" + str + ") : " + SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_GIRDERXORY") + "\r\n";
                                //  // continue;
                                //}
                                //else
                                {
                                    Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs1 = cmpGeometry.CrvRectangle(haba1, takasa1, centerRectangle1);
                                    Collections.Generic.IList<Revit.DB.DetailLine> dLines1 = new Collections.Generic.List<Revit.DB.DetailLine>();

                                    trans.Start("躯体作成");

                                    foreach (Revit.DB.Curve crv in rectangleCrvs1)
                                    {
                                        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                        dc.LineStyle = bodyLineType;
                                        dLines1.Add(dc as Revit.DB.DetailLine);
                                    }

                                    // 幅寸法
                                    // すべて表示
                                    if (widthDimShow == 0)
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines1, dimType);
                                    }
                                    // 左を基準
                                    else if (widthDimShow == 1)
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines1, dimType);
                                    }
                                    // 中央を基準
                                    else if (widthDimShow == 2)
                                    {
                                        if (haba1 != (double)data.Rows[currentNum][cmpParameters.SentanHarihaba])
                                        {
                                            cmpGeometry.CreateDimensionRectBottom(dLines1, dimType);
                                        }
                                    }

                                    // 高さ寸法
                                    // すべて表示
                                    if (heightDimShow == 0)
                                    {
                                        cmpGeometry.CreateDimensionRectLeft(dLines1, dimType);
                                    }
                                    // 左を基準
                                    else if (heightDimShow == 1)
                                    {
                                        cmpGeometry.CreateDimensionRectLeft(dLines1, dimType);
                                    }

                                    trans.Commit();

                                    // 配筋
                                    string strRet = cmpService.CreateRebar_Beam(data, currentNum, centerRectangle1, 0, rvtUiApp, ref isSyukinUeSet, ref isSyukinSitaSet, ref isStirrupSet, ref isWebSet, isCanti);

                                    if (strRet != "")
                                    {
                                        writeErr += level + hugoName + "(" + str + ") : " + "\r\n" + strRet + "\r\n";
                                        strRet = level + hugoName + "(" + str + ") : " + "\r\n" + strRet;

                                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe,
                                                                                            hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 2 / 304.8 * scale,
                                                                                                                        -(levelMaxY + topSpace / 304.8 * scale + bottomSpace / 304.8 * scale),
                                                                                                                        0));

                                        //                    trans.Start("Error Message");
                                        txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax;
                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();

                                        //                    trans.Commit();
                                    }

                                    // 元端のテキスト
                                    #region 主筋本数、径、肋筋本数、径、腹筋本数、径

                                    if (isSyukinUeSet > 2)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 上端筋元端
                                        rebarCount = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin1danHutokinHonsu] +
                                                     (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin2danHutokinHonsu] +
                                                     (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin3danHutokinHonsu];

                                        hosoCount = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin1danHosokinHonsu] +
                                                    (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin2danHosokinHonsu] +
                                                    (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin3danHosokinHonsu];

                                        if (rebarCount > 1 && (string)data.Rows[currentNum][cmpParameters.MototanUeSyukinHutokei] != "")
                                        {
                                            title = rebarCount + "-" + (string)data.Rows[currentNum][cmpParameters.MototanUeSyukinHutokei];
                                            if (hosoCount > 0 && (string)data.Rows[currentNum][cmpParameters.MototanUeSyukinHosokei] != "")
                                            {
                                                title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][cmpParameters.MototanUeSyukinHosokei];
                                            }

                                            origin = uwabakinTitlePoints[0];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (isSyukinSitaSet > 2)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 下端筋元端
                                        rebarCount = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin1danHutokinHonsu] +
                                                     (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin2danHutokinHonsu] +
                                                     (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin3danHutokinHonsu];

                                        hosoCount = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin1danHosokinHonsu] +
                                                    (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin2danHosokinHonsu] +
                                                    (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin3danHosokinHonsu];

                                        if (rebarCount > 1 && (string)data.Rows[currentNum][cmpParameters.MototanSitaSyukinHutokei] != "")
                                        {
                                            title = rebarCount + "-" + (string)data.Rows[currentNum][cmpParameters.MototanSitaSyukinHutokei];
                                            if (hosoCount > 0 && (string)data.Rows[currentNum][cmpParameters.MototanSitaSyukinHosokei] != "")
                                            {
                                                title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][cmpParameters.MototanSitaSyukinHosokei];
                                            }

                                            origin = sitabakinTitlePoints[0];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (strRet.Contains("IDS_ERR_HUTO_FAMILY_NOTSET") == false &&
                                        strRet.Contains("IDS_ERR_HOSO_FAMILY_NOTSET") == false &&
                                        rectangleCrvs1.Count == 4)
                                    {
                                        #region 断面枠の本数表示

                                        if (rebarShow == 1)
                                        {
                                            //                      trans.Start("RebarNumberShow");

                                            int ue1dan = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin1danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin1danHosokinHonsu];
                                            int sita1dan = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin1danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin1danHosokinHonsu];

                                            int length = 0;

                                            if (ue1dan > sita1dan)
                                            {
                                                length = ue1dan.ToString().Length;
                                            }
                                            else
                                            {
                                                length = sita1dan.ToString().Length;
                                            }

                                            double txtMaxW = 0;
                                            Collections.Generic.IList<Revit.DB.TextNote> tntAry = new Collections.Generic.List<Revit.DB.TextNote>();

                                            if (isSyukinUeSet > 0)
                                            {
                                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle1);

                                                Revit.DB.XYZ innerTop = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarTop.Y, rectanglePoints[1].Z);

                                                //int ue1dan = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu];
                                                int ue2dan = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin2danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin2danHosokinHonsu];
                                                int ue3dan = (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin3danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanUeSyukin3danHosokinHonsu];

                                                title = ue1dan.ToString();
                                                if (ue2dan > 0 && isSyukinUeSet > 1)
                                                {
                                                    title += "\r\n" + ue2dan.ToString();
                                                    if (ue3dan > 0 && isSyukinUeSet > 2)
                                                    {
                                                        title += "\r\n" + ue3dan.ToString();
                                                    }
                                                }

                                                double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                                double a = 0.000057930;
                                                double b = 0.000164979;

                                                lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerTop, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}

                                                trans.Start("Regenerate");
                                                rvtDbDoc.Regenerate();
                                                trans.Commit();

                                                //txtNote.Text = title;

                                                txtMaxW = txtMaxW > txtNote.Width ? txtMaxW : txtNote.Width;
                                                tntAry.Add(txtNote);
                                            }

                                            if (isSyukinSitaSet > 0)
                                            {
                                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle1);

                                                Revit.DB.XYZ innerBottom = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarBtm.Y, rectanglePoints[1].Z);

                                                //int sita1dan = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu];
                                                int sita2dan = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin2danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin2danHosokinHonsu];
                                                int sita3dan = (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin3danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.MototanSitaSyukin3danHosokinHonsu];

                                                title = sita1dan.ToString();
                                                if (sita2dan > 0 && isSyukinSitaSet > 1)
                                                {
                                                    title = sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                    if (sita3dan > 0 && isSyukinSitaSet > 2)
                                                    {
                                                        title = sita3dan.ToString() + "\r\n" + sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                    }
                                                }

                                                double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                                double a = 0.000057930;
                                                double b = 0.000164979;

                                                lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerBottom, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}

                                                trans.Start("Regenerate");
                                                rvtDbDoc.Regenerate();
                                                trans.Commit();

                                                //txtNote.Text = title;

                                                txtMaxW = txtMaxW > txtNote.Width ? txtMaxW : txtNote.Width;
                                                tntAry.Add(txtNote);
                                            }

                                            //foreach (Revit.DB.TextNote tnt in tntAry)
                                            //{
                                            //  tnt.Width = txtMaxW;
                                            //  tnt.Location.Move(new Revit.DB.XYZ(tnt.Width / 2, 0, 0));
                                            //  tnt.get_Parameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                            //}

                                            //                      trans.Commit();
                                        }

                                        #endregion
                                    }

                                    if (isStirrupSet)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        // 肋筋
                                        // まとめて表示
                                        if (stirrupFrameShow == 0 && cmpService.IsSameStirrupBySection_Canti(data.Rows[currentNum]) == true)
                                        {
                                        }
                                        // 断面別に表示
                                        else if (stirrupFrameShow == 1 || cmpService.IsSameStirrupBySection_Canti(data.Rows[currentNum]) == false)
                                        {
                                            lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            // 肋筋元端
                                            if ((int)data.Rows[currentNum][cmpParameters.MototanAbarakinHonsu] > 1 && (string)data.Rows[currentNum][cmpParameters.MototanAbarakinkei] != "" && (double)data.Rows[currentNum][cmpParameters.MototanAbarakinPitch] > 0)
                                            {
                                                if (stirrupBracketShow == 0)
                                                {
                                                    title = (int)data.Rows[currentNum][cmpParameters.MototanAbarakinHonsu] + stirrupSpaceSymbol + (string)data.Rows[currentNum][cmpParameters.MototanAbarakinkei] + "@" + (double)data.Rows[currentNum][cmpParameters.MototanAbarakinPitch];
                                                }
                                                else
                                                {
                                                    title = "[" + (int)data.Rows[currentNum][cmpParameters.MototanAbarakinHonsu] + "] " + (string)data.Rows[currentNum][cmpParameters.MototanAbarakinkei] + "@" + (double)data.Rows[currentNum][cmpParameters.MototanAbarakinPitch];
                                                }

                                                origin = stirrupTitlePoints[0];
                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                            }
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (isWebSet)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        // 腹筋
                                        if (webFrameShow != 2)
                                        {
                                            // まとめて表示
                                            if (webFrameShow == 0 && cmpService.IsSameWebBySection_Canti(data.Rows[currentNum]) == true)
                                            {
                                            }
                                            else if (webFrameShow == 1 || cmpService.IsSameWebBySection_Canti(data.Rows[currentNum]) == false)
                                            {
                                                lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 2;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                // 腹筋元端
                                                if ((int)data.Rows[currentNum][cmpParameters.MototanHarakinHonsu] > 1 && (string)data.Rows[currentNum][cmpParameters.MototanHarakinkei] != "")
                                                {
                                                    title = (int)data.Rows[currentNum][cmpParameters.MototanHarakinHonsu] + "-" + (string)data.Rows[currentNum][cmpParameters.MototanHarakinkei];

                                                    if ((int)data.Rows[currentNum][cmpParameters.MototanHarakinHonsu] == 0)
                                                    {
                                                        title = "-";
                                                    }

                                                    origin = harakinTitlePoints[0];

                                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                    //if (otherTNT != null)
                                                    //{
                                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                                    //}
                                                    //txtnote.width = w_itanWaku;// w_HugowakuMax / 2;// hugoMaxX;
                                                }
                                            }
                                        }

                                        //                    trans.Commit();
                                    }

                                    #endregion
                                }
                                #endregion

                                #region 先端部
                                Revit.DB.XYZ centerRectangle2 = hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[1] / 2,
                                                                                            -(levelMaxY / 2 + topSpace / 304.8 * scale),
                                                                                            0);

                                double haba2 = (double)data.Rows[currentNum][cmpParameters.SentanHarihaba];
                                double takasa2 = (double)data.Rows[currentNum][cmpParameters.SentanHarisei];

                                sabun = levelMaxY - takasa2;
                                centerRectangle2 += new Revit.DB.XYZ(0, sabun / 2, 0);

                                //if (haba2 <= 0 || takasa2 <= 0)
                                //{
                                //  //writeErr += level + hugoName + "(" + cantiLeverEndTitle + ") : " + SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_GIRDERXORY") + "\r\n";
                                //  //continue;
                                //}
                                //else
                                {
                                    Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs2 = cmpGeometry.CrvRectangle(haba2, takasa2, centerRectangle2);
                                    Collections.Generic.IList<Revit.DB.DetailLine> dLines2 = new Collections.Generic.List<Revit.DB.DetailLine>();

                                    trans.Start("躯体作成");

                                    foreach (Revit.DB.Curve crv in rectangleCrvs2)
                                    {
                                        Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                        dc.LineStyle = bodyLineType;
                                        dLines2.Add(dc as Revit.DB.DetailLine);
                                    }

                                    // 幅寸法
                                    // すべて表示
                                    if (widthDimShow == 0)
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines2, dimType);
                                    }
                                    // 左を基準
                                    else if (widthDimShow == 1)
                                    {
                                        if (haba1 != haba2)
                                        {
                                            cmpGeometry.CreateDimensionRectBottom(dLines2, dimType);
                                        }
                                    }
                                    // 中央を基準
                                    else if (widthDimShow == 2)
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines2, dimType);
                                    }

                                    // 高さ寸法
                                    // すべて表示
                                    if (heightDimShow == 0)
                                    {
                                        cmpGeometry.CreateDimensionRectLeft(dLines2, dimType);
                                    }
                                    // 左を基準
                                    else if (heightDimShow == 1)
                                    {
                                        if (cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][cmpParameters.MototanHarisei], -9) != cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][cmpParameters.SentanHarisei], -9))
                                        {
                                            cmpGeometry.CreateDimensionRectLeft(dLines2, dimType);
                                        }
                                    }

                                    trans.Commit();

                                    // 配筋
                                    string strRet = cmpService.CreateRebar_Beam(data, currentNum, centerRectangle2, 2, rvtUiApp, ref isSyukinUeSet, ref isSyukinSitaSet, ref isStirrupSet, ref isWebSet, isCanti);

                                    if (strRet != "")
                                    {
                                        writeErr += level + hugoName + "(" + cantiLeverEndTitle + ") : " + "\r\n" + strRet + "\r\n";
                                        strRet = level + hugoName + "(" + cantiLeverEndTitle + ") : " + "\r\n" + strRet;

                                        lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 2 / 304.8 * scale, 0, 0),
                                                                                            hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[1] + rightSpace / 304.8 * scale,
                                                                                                                        -(levelMaxY + topSpace / 304.8 * scale + bottomSpace / 304.8 * scale),
                                                                                                                        0));

                                        //                    trans.Start("Error Message");
                                        txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_chuohWaku;
                                        trans.Start("Regenerate");
                                        rvtDbDoc.Regenerate();
                                        trans.Commit();

                                        //                    trans.Commit();
                                    }

                                    // 先端のテキスト
                                    #region 主筋本数、径、肋筋本数、径、腹筋本数、径

                                    if (isSyukinUeSet > 2)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 上端筋先端
                                        rebarCount = (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin1danHutokinHonsu] +
                                                     (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin2danHutokinHonsu] +
                                                     (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin3danHutokinHonsu];

                                        hosoCount = (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin1danHosokinHonsu] +
                                                    (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin2danHosokinHonsu] +
                                                    (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin3danHosokinHonsu];

                                        if (rebarCount > 1 && (string)data.Rows[currentNum][cmpParameters.SentanUeSyukinHutokei] != "")
                                        {
                                            title = rebarCount + "-" + (string)data.Rows[currentNum][cmpParameters.SentanUeSyukinHutokei];
                                            if (hosoCount > 0 && (string)data.Rows[currentNum][cmpParameters.SentanUeSyukinHosokei] != "")
                                            {
                                                title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][cmpParameters.SentanUeSyukinHosokei];
                                            }

                                            origin = uwabakinTitlePoints[1];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_chuohWaku;
                                        }
                                        //                    trans.Commit();
                                    }

                                    if (isSyukinSitaSet > 2)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 下端筋先端
                                        rebarCount = (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin1danHutokinHonsu] +
                                                     (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin2danHutokinHonsu] +
                                                     (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin3danHutokinHonsu];

                                        hosoCount = (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin1danHosokinHonsu] +
                                                    (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin2danHosokinHonsu] +
                                                    (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin3danHosokinHonsu];

                                        if (rebarCount > 1 && (string)data.Rows[currentNum][cmpParameters.SentanSitaSyukinHutokei] != "")
                                        {
                                            title = rebarCount + "-" + (string)data.Rows[currentNum][cmpParameters.SentanSitaSyukinHutokei];
                                            if (hosoCount > 0 && (string)data.Rows[currentNum][cmpParameters.SentanSitaSyukinHosokei] != "")
                                            {
                                                title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][cmpParameters.SentanSitaSyukinHosokei];
                                            }

                                            origin = sitabakinTitlePoints[1];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_chuohWaku;
                                        }
                                        //                    trans.Commit();
                                    }

                                    if (strRet.Contains("IDS_ERR_HUTO_FAMILY_NOTSET") == false &&
                                        strRet.Contains("IDS_ERR_HOSO_FAMILY_NOTSET") == false &&
                                        rectangleCrvs2.Count == 4)
                                    {
                                        #region 断面枠の本数表示

                                        if (rebarShow == 1)
                                        {
                                            //                      trans.Start("RebarNumberShow");

                                            int ue1dan = (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin1danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin1danHosokinHonsu];
                                            int sita1dan = (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin1danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin1danHosokinHonsu];

                                            int length = 0;

                                            if (ue1dan > sita1dan)
                                            {
                                                length = ue1dan.ToString().Length;
                                            }
                                            else
                                            {
                                                length = sita1dan.ToString().Length;
                                            }

                                            double txtMaxW = 0;
                                            Collections.Generic.IList<Revit.DB.TextNote> tntAry = new Collections.Generic.List<Revit.DB.TextNote>();

                                            if (isSyukinUeSet > 0)
                                            {
                                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba2, takasa2, centerRectangle2);

                                                Revit.DB.XYZ innerTop = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarTop.Y, rectanglePoints[1].Z);

                                                //int ue1dan = (int)data.Rows[currentNum][syukinJtanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanUe1danHosokeiHonsu];
                                                int ue2dan = (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin2danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin2danHosokinHonsu];
                                                int ue3dan = (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin3danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.SentanUeSyukin3danHosokinHonsu];

                                                double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                                double a = 0.000057930;
                                                double b = 0.000164979;

                                                lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                                title = ue1dan.ToString();
                                                if (ue2dan > 0 && isSyukinUeSet > 1)
                                                {
                                                    title += "\r\n" + ue2dan.ToString();
                                                    if (ue3dan > 0 && isSyukinUeSet > 2)
                                                    {
                                                        title += "\r\n" + ue3dan.ToString();
                                                    }
                                                }

                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerTop, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                if (otherTNT != null)
                                                {
                                                    txtNote.ChangeTypeId(otherTNT.Id);
                                                }

                                                trans.Start("Regenerate");
                                                rvtDbDoc.Regenerate();
                                                trans.Commit();

                                                //txtNote.Text = title;

                                                txtMaxW = txtMaxW > txtNote.Width ? txtMaxW : txtNote.Width;
                                                tntAry.Add(txtNote);
                                            }

                                            if (isSyukinSitaSet > 0)
                                            {
                                                Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba2, takasa2, centerRectangle2);

                                                Revit.DB.XYZ innerBottom = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarBtm.Y, rectanglePoints[1].Z);

                                                //int sita1dan = (int)data.Rows[currentNum][syukinJtanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanSita1danHosokeiHonsu];
                                                int sita2dan = (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin2danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin2danHosokinHonsu];
                                                int sita3dan = (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin3danHutokinHonsu] + (int)data.Rows[currentNum][cmpParameters.SentanSitaSyukin3danHosokinHonsu];

                                                title = sita1dan.ToString();
                                                if (sita2dan > 0 && isSyukinSitaSet > 1)
                                                {
                                                    title = sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                    if (sita3dan > 0 && isSyukinSitaSet > 2)
                                                    {
                                                        title = sita3dan.ToString() + "\r\n" + sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                    }
                                                }

                                                double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                                double a = 0.000057930;
                                                double b = 0.000164979;

                                                lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerBottom, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                if (otherTNT != null)
                                                {
                                                    txtNote.ChangeTypeId(otherTNT.Id);
                                                }

                                                trans.Start("Regenerate");
                                                rvtDbDoc.Regenerate();
                                                trans.Commit();

                                                //txtNote.Text = title;

                                                txtMaxW = txtMaxW > txtNote.Width ? txtMaxW : txtNote.Width;
                                                tntAry.Add(txtNote);
                                            }

                                            //foreach (Revit.DB.TextNote tnt in tntAry)
                                            //{
                                            //  tnt.Width = txtMaxW;
                                            //  tnt.Location.Move(new Revit.DB.XYZ(tnt.Width / 2, 0, 0));
                                            //  tnt.get_Parameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                            //}

                                            //                      trans.Commit();
                                        }

                                        #endregion
                                    }

                                    if (isStirrupSet)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        // 肋筋
                                        // まとめて表示
                                        if (stirrupFrameShow == 0 && cmpService.IsSameStirrupBySection_Canti(data.Rows[currentNum]) == true)
                                        {
                                            lineWidth = w_HugowakuMax / scale;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            // 肋筋先端
                                            if ((int)data.Rows[currentNum][cmpParameters.SentanAbarakinHonsu] > 1 && (string)data.Rows[currentNum][cmpParameters.SentanAbarakinkei] != "" && (double)data.Rows[currentNum][cmpParameters.SentanAbarakinPitch] > 0)
                                            {
                                                if (stirrupBracketShow == 0)
                                                {
                                                    title = (int)data.Rows[currentNum][cmpParameters.SentanAbarakinHonsu] + stirrupSpaceSymbol + (string)data.Rows[currentNum][cmpParameters.SentanAbarakinkei] + "@" + (double)data.Rows[currentNum][cmpParameters.SentanAbarakinPitch];
                                                }
                                                else
                                                {
                                                    title = "[" + (int)data.Rows[currentNum][cmpParameters.SentanAbarakinHonsu] + "] " + (string)data.Rows[currentNum][cmpParameters.SentanAbarakinkei] + "@" + (double)data.Rows[currentNum][cmpParameters.SentanAbarakinPitch];
                                                }

                                                origin = centerStirrupFrame;
                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_HugowakuMax;
                                            }
                                        }
                                        // 断面別に表示
                                        else if (stirrupFrameShow == 1 || cmpService.IsSameStirrupBySection_Canti(data.Rows[currentNum]) == false)
                                        {
                                            lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            // 肋筋先端
                                            if ((int)data.Rows[currentNum][cmpParameters.SentanAbarakinHonsu] > 1 && (string)data.Rows[currentNum][cmpParameters.SentanAbarakinkei] != "" && (double)data.Rows[currentNum][cmpParameters.SentanAbarakinPitch] > 0)
                                            {
                                                if (stirrupBracketShow == 0)
                                                {
                                                    title = (int)data.Rows[currentNum][cmpParameters.SentanAbarakinHonsu] + stirrupSpaceSymbol + (string)data.Rows[currentNum][cmpParameters.SentanAbarakinkei] + "@" + (double)data.Rows[currentNum][cmpParameters.SentanAbarakinPitch];
                                                }
                                                else
                                                {
                                                    title = "[" + (int)data.Rows[currentNum][cmpParameters.SentanAbarakinHonsu] + "] " + (string)data.Rows[currentNum][cmpParameters.SentanAbarakinkei] + "@" + (double)data.Rows[currentNum][cmpParameters.SentanAbarakinPitch];
                                                }

                                                origin = stirrupTitlePoints[1];
                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_chuohWaku;
                                            }
                                        }

                                        //                    trans.Commit();
                                    }

                                    if (isWebSet)
                                    {
                                        //                    trans.Start("鉄筋本数径");

                                        // 腹筋
                                        if (webFrameShow != 2)
                                        {
                                            // まとめて表示
                                            if (webFrameShow == 0 && cmpService.IsSameWebBySection_Canti(data.Rows[currentNum]) == true)
                                            {
                                                lineWidth = w_HugowakuMax / scale;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                // 肋筋先端
                                                if ((int)data.Rows[currentNum][cmpParameters.SentanHarakinHonsu] > 1 && (string)data.Rows[currentNum][cmpParameters.SentanHarakinkei] != "")
                                                {
                                                    title = (int)data.Rows[currentNum][cmpParameters.SentanHarakinHonsu] + "-" + (string)data.Rows[currentNum][cmpParameters.SentanHarakinkei];

                                                    if ((int)data.Rows[currentNum][cmpParameters.SentanHarakinHonsu] == 0)
                                                    {
                                                        title = "-";
                                                    }

                                                    origin = centerWebFrame;

                                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                    //if (otherTNT != null)
                                                    //{
                                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                                    //}
                                                    //txtnote.width = w_HugowakuMax;
                                                }
                                            }
                                            else if (webFrameShow == 1 || cmpService.IsSameWebBySection_Canti(data.Rows[currentNum]) == false)
                                            {
                                                lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 2;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                // 腹筋先端
                                                if ((int)data.Rows[currentNum][cmpParameters.SentanHarakinHonsu] > 1 && (string)data.Rows[currentNum][cmpParameters.SentanHarakinkei] != "")
                                                {
                                                    title = (int)data.Rows[currentNum][cmpParameters.SentanHarakinHonsu] + "-" + (string)data.Rows[currentNum][cmpParameters.SentanHarakinkei];

                                                    if ((int)data.Rows[currentNum][cmpParameters.SentanHarakinHonsu] == 0)
                                                    {
                                                        title = "-";
                                                    }

                                                    origin = harakinTitlePoints[1];

                                                    txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                    //if (otherTNT != null)
                                                    //{
                                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                                    //}
                                                    //txtnote.width = w_chuohWaku;
                                                }
                                            }
                                        }

                                        //                    trans.Commit();
                                    }

                                    #endregion
                                }
                                #endregion

                                #endregion
                            }
                        }
                        else if (secTypeNum == 2)
                        {
                            #region 始端、中央、終端

                            #region 始端
                            Revit.DB.XYZ centerRectangle1 = hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] / 2,
                                                                                        -(levelMaxY / 2 + topSpace / 304.8 * scale),
                                                                                        0);

                            double haba1 = (double)data.Rows[currentNum][s_B];
                            double takasa1 = (double)data.Rows[currentNum][s_D];

                            double sabun = levelMaxY - takasa1;
                            centerRectangle1 += new Revit.DB.XYZ(0, sabun / 2, 0);

                            string str = (aryNum == 2 || aryNum == 4 || aryNum == 6 || aryNum == 8) ? cantiLeverStartTitle : itanSectionTitle;

                            //if (haba1 <= 0 || takasa1 <= 0)
                            //{
                            //  // 3478でなければedgeTitle
                            //  // 3478であればcanti
                            //  //writeErr += level + hugoName + "(" + str + ") : " + SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_GIRDERXORY") + "\r\n";
                            //  // continue;
                            //}
                            //else
                            {
                                Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs1 = cmpGeometry.CrvRectangle(haba1, takasa1, centerRectangle1);
                                Collections.Generic.IList<Revit.DB.DetailLine> dLines1 = new Collections.Generic.List<Revit.DB.DetailLine>();

                                trans.Start("躯体作成");

                                foreach (Revit.DB.Curve crv in rectangleCrvs1)
                                {
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                    dc.LineStyle = bodyLineType;
                                    dLines1.Add(dc as Revit.DB.DetailLine);
                                }

                                // 幅寸法
                                // すべて表示
                                if (widthDimShow == 0)
                                {
                                    cmpGeometry.CreateDimensionRectBottom(dLines1, dimType);
                                }
                                // 左を基準
                                else if (widthDimShow == 1)
                                {
                                    cmpGeometry.CreateDimensionRectBottom(dLines1, dimType);
                                }
                                // 中央を基準
                                else if (widthDimShow == 2)
                                {
                                    if (cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[0], -9) != cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[1], -9))
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines1, dimType);
                                    }
                                }

                                // 高さ寸法
                                // すべて表示
                                if (heightDimShow == 0)
                                {
                                    cmpGeometry.CreateDimensionRectLeft(dLines1, dimType);
                                }
                                // 左を基準
                                else if (heightDimShow == 1)
                                {
                                    cmpGeometry.CreateDimensionRectLeft(dLines1, dimType);
                                }

                                trans.Commit();

                                // 配筋
                                string strRet = cmpService.CreateRebar_Beam(data, currentNum, centerRectangle1, 0, rvtUiApp, ref isSyukinUeSet, ref isSyukinSitaSet, ref isStirrupSet, ref isWebSet, isCanti);

                                if (strRet != "")
                                {
                                    writeErr += level + hugoName + "(" + str + ") : " + "\r\n" + strRet + "\r\n";
                                    strRet = level + hugoName + "(" + str + ") : " + "\r\n" + strRet;

                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 3;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe,
                                                                                        hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 2 / 304.8 * scale,
                                                                                                                    -(levelMaxY + topSpace / 304.8 * scale + bottomSpace / 304.8 * scale),
                                                                                                                    0));

                                    //                  trans.Start("Error Message");
                                    txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtnote.width = w_itanWaku;// w_HugowakuMax / 3;
                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();

                                    //                  trans.Commit();
                                }

                                // 始端のテキスト
                                #region 主筋本数、径、肋筋本数、径、腹筋本数、径

                                if (isSyukinUeSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 3;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    // 上端筋始端
                                    rebarCount = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinItanUe2danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinItanUe3danHutokeiHonsu];

                                    hosoCount = (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinItanUe2danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinItanUe3danHosokeiHonsu];

                                    if (rebarCount > 1 && (string)data.Rows[currentNum][syukinItanUeHutokei] != "")
                                    {
                                        title = rebarCount + "-" + (string)data.Rows[currentNum][syukinItanUeHutokei];
                                        if (hosoCount > 0 && (string)data.Rows[currentNum][syukinItanUeHosokei] != "")
                                        {
                                            title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinItanUeHosokei];
                                        }

                                        origin = uwabakinTitlePoints[0];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                    }
                                    //                  trans.Commit();
                                }

                                if (isSyukinSitaSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 3;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    // 下端筋始端
                                    rebarCount = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinItanSita2danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinItanSita3danHutokeiHonsu];

                                    hosoCount = (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinItanSita2danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinItanSita3danHosokeiHonsu];

                                    if (rebarCount > 1 && (string)data.Rows[currentNum][syukinItanSitaHutokei] != "")
                                    {
                                        title = rebarCount + "-" + (string)data.Rows[currentNum][syukinItanSitaHutokei];
                                        if (hosoCount > 0 && (string)data.Rows[currentNum][syukinItanSitaHosokei] != "")
                                        {
                                            title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinItanSitaHosokei];
                                        }

                                        origin = sitabakinTitlePoints[0];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_itanWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                    }

                                    //                  trans.Commit();
                                }

                                if (strRet.Contains("IDS_ERR_HUTO_FAMILY_NOTSET") == false &&
                                    strRet.Contains("IDS_ERR_HOSO_FAMILY_NOTSET") == false &&
                                    rectangleCrvs1.Count == 4)
                                {
                                    #region 断面枠の本数表示

                                    if (rebarShow == 1)
                                    {
                                        //                    trans.Start("RebarNumberShow");

                                        int ue1dan = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu];
                                        int sita1dan = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu];

                                        int length = 0;

                                        if (ue1dan > sita1dan)
                                        {
                                            length = ue1dan.ToString().Length;
                                        }
                                        else
                                        {
                                            length = sita1dan.ToString().Length;
                                        }

                                        double txtMaxW = 0;
                                        Collections.Generic.IList<Revit.DB.TextNote> tntAry = new Collections.Generic.List<Revit.DB.TextNote>();

                                        if (isSyukinUeSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle1);

                                            Revit.DB.XYZ innerTop = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarTop.Y, rectanglePoints[1].Z);

                                            //int ue1dan = (int)data.Rows[currentNum][syukinItanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe1danHosokeiHonsu];
                                            int ue2dan = (int)data.Rows[currentNum][syukinItanUe2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe2danHosokeiHonsu];
                                            int ue3dan = (int)data.Rows[currentNum][syukinItanUe3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanUe3danHosokeiHonsu];

                                            title = ue1dan.ToString();
                                            if (ue2dan > 0 && isSyukinUeSet > 1)
                                            {
                                                title += "\r\n" + ue2dan.ToString();
                                                if (ue3dan > 0 && isSyukinUeSet > 2)
                                                {
                                                    title += "\r\n" + ue3dan.ToString();
                                                }
                                            }

                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerTop, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtNote.Width;
                                            tntAry.Add(txtNote);
                                        }

                                        if (isSyukinSitaSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba1, takasa1, centerRectangle1);

                                            Revit.DB.XYZ innerBottom = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarBtm.Y, rectanglePoints[1].Z);

                                            //int sita1dan = (int)data.Rows[currentNum][syukinItanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita1danHosokeiHonsu];
                                            int sita2dan = (int)data.Rows[currentNum][syukinItanSita2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita2danHosokeiHonsu];
                                            int sita3dan = (int)data.Rows[currentNum][syukinItanSita3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinItanSita3danHosokeiHonsu];

                                            title = sita1dan.ToString();
                                            if (sita2dan > 0 && isSyukinSitaSet > 1)
                                            {
                                                title = sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                if (sita3dan > 0 && isSyukinSitaSet > 2)
                                                {
                                                    title = sita3dan.ToString() + "\r\n" + sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                }
                                            }

                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerBottom, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtNote.Width;
                                            tntAry.Add(txtNote);
                                        }

                                        //foreach (Revit.DB.TextNote tnt in tntAry)
                                        //{
                                        //  tnt.Width = txtMaxW;
                                        //  tnt.Location.Move(new Revit.DB.XYZ(tnt.Width / 2, 0, 0));
                                        //  tnt.get_Parameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                        //}

                                        //                    trans.Commit();
                                    }

                                    #endregion
                                }

                                if (isStirrupSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 肋筋
                                    // まとめて表示
                                    if (stirrupFrameShow == 0 && cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == true)
                                    {
                                    }
                                    // 断面別に表示
                                    else if (stirrupFrameShow == 1 || cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == false)
                                    {
                                        lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 3;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 肋筋始端
                                        if ((int)data.Rows[currentNum][s_Stirrup_Number] > 1 && (string)data.Rows[currentNum][s_Stirrup_Diameter] != "" && (double)data.Rows[currentNum][s_Stirrup_Pitch] > 0)
                                        {
                                            if (stirrupBracketShow == 0)
                                            {
                                                title = (int)data.Rows[currentNum][s_Stirrup_Number] + stirrupSpaceSymbol + (string)data.Rows[currentNum][s_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][s_Stirrup_Pitch];
                                            }
                                            else
                                            {
                                                title = "[" + (int)data.Rows[currentNum][s_Stirrup_Number] + "] " + (string)data.Rows[currentNum][s_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][s_Stirrup_Pitch];
                                            }

                                            origin = stirrupTitlePoints[0];

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_itanWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                        }
                                    }

                                    //                  trans.Commit();
                                }

                                if (isWebSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 腹筋
                                    if (webFrameShow != 2)
                                    {
                                        // まとめて表示
                                        if (webFrameShow == 0 && cmpService.IsSameWebBySection(data.Rows[currentNum]) == true)
                                        {
                                        }
                                        else if (webFrameShow == 1 || cmpService.IsSameWebBySection(data.Rows[currentNum]) == false)
                                        {
                                            lineWidth = w_itanWaku / scale;// w_HugowakuMax / scale / 3;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            // 腹筋始端
                                            if ((int)data.Rows[currentNum][s_Web_Number] > 1 && (string)data.Rows[currentNum][s_Web_Diameter] != "")
                                            {
                                                title = (int)data.Rows[currentNum][s_Web_Number] + "-" + (string)data.Rows[currentNum][s_Web_Diameter];

                                                if ((int)data.Rows[currentNum][s_Web_Number] == 0)
                                                {
                                                    title = "-";
                                                }

                                                origin = harakinTitlePoints[0];

                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_itanWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                            }
                                        }
                                    }

                                    //                  trans.Commit();
                                }

                                #endregion
                            }
                            #endregion

                            #region 中央部
                            Revit.DB.XYZ centerRectangle2 = hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[1] / 2,
                                                                                        -(levelMaxY / 2 + topSpace / 304.8 * scale),
                                                                                        0);

                            double haba2 = (double)data.Rows[currentNum][c_B];
                            double takasa2 = (double)data.Rows[currentNum][c_D];

                            sabun = levelMaxY - takasa2;
                            centerRectangle2 += new Revit.DB.XYZ(0, sabun / 2, 0);

                            //if (haba2 <= 0 || takasa2 <= 0)
                            //{
                            //  // 3478でなければedgeTitle
                            //  // 3478であればcanti
                            //  //writeErr += level + hugoName + "(" + centerSectionTitle + ") : " + SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_GIRDERXORY") + "\r\n";
                            //  // continue;
                            //}
                            //else
                            {
                                Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs2 = cmpGeometry.CrvRectangle(haba2, takasa2, centerRectangle2);
                                Collections.Generic.IList<Revit.DB.DetailLine> dLines2 = new Collections.Generic.List<Revit.DB.DetailLine>();

                                trans.Start("躯体作成");

                                foreach (Revit.DB.Curve crv in rectangleCrvs2)
                                {
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                    dc.LineStyle = bodyLineType;
                                    dLines2.Add(dc as Revit.DB.DetailLine);
                                }

                                // 幅寸法
                                // すべて表示
                                if (widthDimShow == 0)
                                {
                                    cmpGeometry.CreateDimensionRectBottom(dLines2, dimType);
                                }
                                // 左を基準
                                else if (widthDimShow == 1)
                                {
                                    if (cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[0], -9) != cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[1], -9) ||
                                        cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[1], -9) != cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[2], -9))
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines2, dimType);
                                    }
                                }
                                // 中央を基準
                                else if (widthDimShow == 2)
                                {
                                    cmpGeometry.CreateDimensionRectBottom(dLines2, dimType);
                                }

                                // 高さ寸法
                                // すべて表示
                                if (heightDimShow == 0)
                                {
                                    cmpGeometry.CreateDimensionRectLeft(dLines2, dimType);
                                }
                                // 左を基準
                                else if (heightDimShow == 1)
                                {
                                    if (cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][s_D], -9) != cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][c_D], -9) ||
                                        cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][e_D], -9) != cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][c_D], -9))
                                    {
                                        cmpGeometry.CreateDimensionRectLeft(dLines2, dimType);
                                    }
                                }

                                trans.Commit();

                                // 配筋
                                string strRet = cmpService.CreateRebar_Beam(data, currentNum, centerRectangle2, 1, rvtUiApp, ref isSyukinUeSet, ref isSyukinSitaSet, ref isStirrupSet, ref isWebSet, isCanti);

                                if (strRet != "")
                                {
                                    writeErr += level + hugoName + "(" + centerSectionTitle + ") : " + "\r\n" + strRet + "\r\n";
                                    strRet = level + hugoName + "(" + centerSectionTitle + ") : " + "\r\n" + strRet;

                                    lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 3;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 2 / 304.8 * scale, 0, 0),
                                                                                        hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[1] + centerSpace / 2 / 304.8 * scale,
                                                                                                                    -(levelMaxY + topSpace / 304.8 * scale + bottomSpace / 304.8 * scale),
                                                                                                                    0));

                                    //                  trans.Start("Error Message");
                                    txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtnote.width = w_chuohWaku;// w_HugowakuMax / 3;
                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();

                                    //                  trans.Commit();
                                }

                                // 中央のテキスト
                                #region 主筋本数、径、肋筋本数、径、腹筋本数、径

                                if (isSyukinUeSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 3;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    // 上端筋中央
                                    rebarCount = (int)data.Rows[currentNum][syukinChuohUe1danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinChuohUe2danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinChuohUe3danHutokeiHonsu];

                                    hosoCount = (int)data.Rows[currentNum][syukinChuohUe1danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinChuohUe2danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinChuohUe3danHosokeiHonsu];

                                    if (rebarCount > 1 && (string)data.Rows[currentNum][syukinChuohUeHutokei] != "")
                                    {
                                        title = rebarCount + "-" + (string)data.Rows[currentNum][syukinChuohUeHutokei];
                                        if (hosoCount > 0 && (string)data.Rows[currentNum][syukinChuohUeHosokei] != "")
                                        {
                                            title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinChuohUeHosokei];
                                        }

                                        origin = uwabakinTitlePoints[1];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_chuohWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                    }

                                    //                  trans.Commit();
                                }

                                if (isSyukinSitaSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 3;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    // 下端筋中央
                                    rebarCount = (int)data.Rows[currentNum][syukinChuohSita1danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinChuohSita2danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinChuohSita3danHutokeiHonsu];

                                    hosoCount = (int)data.Rows[currentNum][syukinChuohSita1danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinChuohSita2danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinChuohSita3danHosokeiHonsu];

                                    if (rebarCount > 1 && (string)data.Rows[currentNum][syukinChuohSitaHutokei] != "")
                                    {
                                        title = rebarCount + "-" + (string)data.Rows[currentNum][syukinChuohSitaHutokei];
                                        if (hosoCount > 0 && (string)data.Rows[currentNum][syukinChuohSitaHosokei] != "")
                                        {
                                            title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinChuohSitaHosokei];
                                        }

                                        origin = sitabakinTitlePoints[1];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_chuohWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                    }

                                    //                  trans.Commit();
                                }

                                if (strRet.Contains("IDS_ERR_HUTO_FAMILY_NOTSET") == false &&
                                    strRet.Contains("IDS_ERR_HOSO_FAMILY_NOTSET") == false &&
                                    rectangleCrvs2.Count == 4)
                                {
                                    #region 断面枠の本数表示

                                    if (rebarShow == 1)
                                    {
                                        //                    trans.Start("RebarNumberShow");

                                        int ue1dan = (int)data.Rows[currentNum][syukinChuohUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohUe1danHosokeiHonsu];
                                        int sita1dan = (int)data.Rows[currentNum][syukinChuohSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohSita1danHosokeiHonsu];

                                        int length = 0;

                                        if (ue1dan > sita1dan)
                                        {
                                            length = ue1dan.ToString().Length;
                                        }
                                        else
                                        {
                                            length = sita1dan.ToString().Length;
                                        }

                                        double txtMaxW = 0;
                                        Collections.Generic.IList<Revit.DB.TextNote> tntAry = new Collections.Generic.List<Revit.DB.TextNote>();

                                        if (isSyukinUeSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba2, takasa2, centerRectangle2);

                                            Revit.DB.XYZ innerTop = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarTop.Y, rectanglePoints[1].Z);

                                            //int ue1dan = (int)data.Rows[currentNum][syukinChuohUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohUe1danHosokeiHonsu];
                                            int ue2dan = (int)data.Rows[currentNum][syukinChuohUe2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohUe2danHosokeiHonsu];
                                            int ue3dan = (int)data.Rows[currentNum][syukinChuohUe3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohUe3danHosokeiHonsu];

                                            title = ue1dan.ToString();
                                            if (ue2dan > 0 && isSyukinUeSet > 1)
                                            {
                                                title += "\r\n" + ue2dan.ToString();
                                                if (ue3dan > 0 && isSyukinUeSet > 2)
                                                {
                                                    title += "\r\n" + ue3dan.ToString();
                                                }
                                            }

                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerTop, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtNote.Width;
                                            tntAry.Add(txtNote);
                                        }

                                        if (isSyukinSitaSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba2, takasa2, centerRectangle2);

                                            Revit.DB.XYZ innerBottom = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarBtm.Y, rectanglePoints[1].Z);

                                            //int sita1dan = (int)data.Rows[currentNum][syukinChuohSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohSita1danHosokeiHonsu];
                                            int sita2dan = (int)data.Rows[currentNum][syukinChuohSita2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohSita2danHosokeiHonsu];
                                            int sita3dan = (int)data.Rows[currentNum][syukinChuohSita3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinChuohSita3danHosokeiHonsu];

                                            title = sita1dan.ToString();
                                            if (sita2dan > 0 && isSyukinSitaSet > 1)
                                            {
                                                title = sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                if (sita3dan > 0 && isSyukinSitaSet > 2)
                                                {
                                                    title = sita3dan.ToString() + "\r\n" + sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                }
                                            }

                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerBottom, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            if (otherTNT != null)
                                            {
                                                txtNote.ChangeTypeId(otherTNT.Id);
                                            }

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtNote.Width;
                                            tntAry.Add(txtNote);
                                        }

                                        //foreach (Revit.DB.TextNote tnt in tntAry)
                                        //{
                                        //  tnt.Width = txtMaxW;
                                        //  tnt.Location.Move(new Revit.DB.XYZ(tnt.Width / 2, 0, 0));
                                        //  tnt.get_Parameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                        //}

                                        //                    trans.Commit();
                                    }

                                    #endregion
                                }

                                if (isStirrupSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 肋筋
                                    // まとめて表示
                                    if (stirrupFrameShow == 0 && cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == true)
                                    {
                                        // 肋筋中央
                                        if ((int)data.Rows[currentNum][c_Stirrup_Number] > 1 && (string)data.Rows[currentNum][c_Stirrup_Diameter] != "" && (double)data.Rows[currentNum][c_Stirrup_Pitch] > 0)
                                        {
                                            if (stirrupBracketShow == 0)
                                            {
                                                title = (int)data.Rows[currentNum][c_Stirrup_Number] + stirrupSpaceSymbol + (string)data.Rows[currentNum][c_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][c_Stirrup_Pitch];
                                            }
                                            else
                                            {
                                                title = "[" + (int)data.Rows[currentNum][c_Stirrup_Number] + "] " + (string)data.Rows[currentNum][c_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][c_Stirrup_Pitch];
                                            }

                                            origin = centerStirrupFrame;
                                            lineWidth = w_HugowakuMax / scale;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_HugowakuMax;// hugoMaxX;
                                        }
                                    }
                                    // 断面別に表示
                                    else if (stirrupFrameShow == 1 || cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == false)
                                    {
                                        lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 3;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 肋筋中央
                                        if ((int)data.Rows[currentNum][c_Stirrup_Number] > 1 && (string)data.Rows[currentNum][c_Stirrup_Diameter] != "" && (double)data.Rows[currentNum][c_Stirrup_Pitch] > 0)
                                        {
                                            if (stirrupBracketShow == 0)
                                            {
                                                title = (int)data.Rows[currentNum][c_Stirrup_Number] + stirrupSpaceSymbol + (string)data.Rows[currentNum][c_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][c_Stirrup_Pitch];
                                            }
                                            else
                                            {
                                                title = "[" + (int)data.Rows[currentNum][c_Stirrup_Number] + "] " + (string)data.Rows[currentNum][c_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][c_Stirrup_Pitch];
                                            }

                                            origin = stirrupTitlePoints[1];
                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_chuohWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                        }
                                    }

                                    //                  trans.Commit();
                                }

                                if (isWebSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 腹筋
                                    if (webFrameShow != 2)
                                    {
                                        // まとめて表示
                                        if (webFrameShow == 0 && cmpService.IsSameWebBySection(data.Rows[currentNum]) == true)
                                        {
                                            // 腹筋中央
                                            if ((int)data.Rows[currentNum][c_Web_Number] > 1 && (string)data.Rows[currentNum][c_Web_Diameter] != "")
                                            {
                                                title = (int)data.Rows[currentNum][c_Web_Number] + "-" + (string)data.Rows[currentNum][c_Web_Diameter];

                                                if ((int)data.Rows[currentNum][c_Web_Number] == 0)
                                                {
                                                    title = "-";
                                                }

                                                origin = centerWebFrame;
                                                lineWidth = w_HugowakuMax / scale;
                                                flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_HugowakuMax;// hugoMaxX;
                                            }
                                        }
                                        else if (webFrameShow == 1 || cmpService.IsSameWebBySection(data.Rows[currentNum]) == false)
                                        {
                                            lineWidth = w_chuohWaku / scale;// w_HugowakuMax / scale / 3;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            // 腹筋中央
                                            if ((int)data.Rows[currentNum][c_Web_Number] > 1 && (string)data.Rows[currentNum][c_Web_Diameter] != "")
                                            {
                                                title = (int)data.Rows[currentNum][c_Web_Number] + "-" + (string)data.Rows[currentNum][c_Web_Diameter];

                                                if ((int)data.Rows[currentNum][c_Web_Number] == 0)
                                                {
                                                    title = "-";
                                                }

                                                origin = harakinTitlePoints[1];

                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_chuohWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                            }
                                        }
                                    }

                                    //                  trans.Commit();
                                }

                                #endregion
                            }
                            #endregion

                            #region 終端
                            Revit.DB.XYZ centerRectangle3 = hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[1] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[2] / 2,
                                                                                        -(levelMaxY / 2 + topSpace / 304.8 * scale),
                                                                                        0);

                            double haba3 = (double)data.Rows[currentNum][e_B];
                            double takasa3 = (double)data.Rows[currentNum][e_D];

                            sabun = levelMaxY - takasa3;
                            centerRectangle3 += new Revit.DB.XYZ(0, sabun / 2, 0);

                            str = (aryNum == 2 || aryNum == 4 || aryNum == 6 || aryNum == 8) ? cantiLeverEndTitle : jtanSectionTitle;

                            //if (haba1 <= 0 || takasa1 <= 0)
                            //{
                            //  // 3478でなければedgeTitle
                            //  // 3478であればcanti
                            //  //writeErr += level + hugoName + "(" + str + ") : " + SectionListRC.JExtComCompat.UtilAttrib.ResourceText("IDS_ERR_GIRDERXORY") + "\r\n";
                            //  // continue;
                            //}
                            //else
                            {
                                Collections.Generic.IList<Revit.DB.Curve> rectangleCrvs3 = cmpGeometry.CrvRectangle(haba3, takasa3, centerRectangle3);
                                Collections.Generic.IList<Revit.DB.DetailLine> dLines3 = new Collections.Generic.List<Revit.DB.DetailLine>();

                                trans.Start("躯体作成");

                                foreach (Revit.DB.Curve crv in rectangleCrvs3)
                                {
                                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                                    dc.LineStyle = bodyLineType;
                                    dLines3.Add(dc as Revit.DB.DetailLine);
                                }

                                // 幅寸法
                                // すべて表示
                                if (widthDimShow == 0)
                                {
                                    cmpGeometry.CreateDimensionRectBottom(dLines3, dimType);
                                }
                                // 左を基準
                                else if (widthDimShow == 1)
                                {
                                    if (cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[0], -9) != cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[2], -9) ||
                                        cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[1], -9) != cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[2], -9))
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines3, dimType);
                                    }
                                }
                                // 中央を基準
                                else if (widthDimShow == 2)
                                {
                                    if (cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[1], -9) != cmpGeometry.ToHalfAdjust(beamSecWidthAryByMark[2], -9))
                                    {
                                        cmpGeometry.CreateDimensionRectBottom(dLines3, dimType);
                                    }
                                }

                                // 高さ寸法
                                // すべて表示
                                if (heightDimShow == 0)
                                {
                                    cmpGeometry.CreateDimensionRectLeft(dLines3, dimType);
                                }
                                // 左を基準
                                else if (heightDimShow == 1)
                                {
                                    if (cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][s_D], -9) != cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][e_D], -9) ||
                                        cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][c_D], -9) != cmpGeometry.ToHalfAdjust((double)data.Rows[currentNum][e_D], -9))
                                    {
                                        cmpGeometry.CreateDimensionRectLeft(dLines3, dimType);
                                    }
                                }

                                trans.Commit();

                                // 配筋
                                string strRet = cmpService.CreateRebar_Beam(data, currentNum, centerRectangle3, 2, rvtUiApp, ref isSyukinUeSet, ref isSyukinSitaSet, ref isStirrupSet, ref isWebSet, isCanti);

                                if (strRet != "")
                                {
                                    writeErr += level + hugoName + "(" + str + ") : " + "\r\n" + strRet + "\r\n";
                                    strRet = level + hugoName + "(" + str + ") : " + "\r\n" + strRet;

                                    lineWidth = w_jtanWaku / scale;// w_HugowakuMax / scale / 3;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    Revit.DB.XYZ centerFrame = cmpGeometry.Center2Point(hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[1] + centerSpace / 2 / 304.8 * scale, 0, 0),
                                                                                        hidariUe + new Revit.DB.XYZ(leftSpace / 304.8 * scale + beamSecWidthAryByMark[0] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[1] + centerSpace / 304.8 * scale + beamSecWidthAryByMark[2] + rightSpace / 304.8 * scale,
                                                                                                                    -(levelMaxY + topSpace / 304.8 * scale + bottomSpace / 304.8 * scale),
                                                                                                                    0));

                                    //                  trans.Start("Error Message");
                                    txtNote = cmpService.CreateNewTextNote(vp, centerFrame, baseVec, lineWidth, otherTNT.Id, strRet, rvtDbDoc);

                                    //if (otherTNT != null)
                                    //{
                                    //  txtNote.ChangeTypeId(otherTNT.Id);
                                    //}
                                    //txtnote.width = w_jtanWaku;// w_HugowakuMax / 3;
                                    trans.Start("Regenerate");
                                    rvtDbDoc.Regenerate();
                                    trans.Commit();

                                    //                  trans.Commit();
                                }

                                // 終端のテキスト
                                #region 主筋本数、径、肋筋本数、径、腹筋本数、径

                                if (isSyukinUeSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    lineWidth = w_jtanWaku / scale;// w_HugowakuMax / scale / 3;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    // 上端筋終端
                                    rebarCount = (int)data.Rows[currentNum][syukinJtanUe1danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinJtanUe2danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinJtanUe3danHutokeiHonsu];

                                    hosoCount = (int)data.Rows[currentNum][syukinJtanUe1danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinJtanUe2danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinJtanUe3danHosokeiHonsu];

                                    if (rebarCount > 1 && (string)data.Rows[currentNum][syukinJtanUeHutokei] != "")
                                    {
                                        title = rebarCount + "-" + (string)data.Rows[currentNum][syukinJtanUeHutokei];
                                        if (hosoCount > 0 && (string)data.Rows[currentNum][syukinJtanUeHosokei] != "")
                                        {
                                            title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinJtanUeHosokei];
                                        }

                                        origin = uwabakinTitlePoints[2];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_jtanWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                    }

                                    //                  trans.Commit();
                                }

                                if (isSyukinSitaSet > 2)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    lineWidth = w_jtanWaku / scale;// w_HugowakuMax / scale / 3;
                                    flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                    // 下端筋終端
                                    rebarCount = (int)data.Rows[currentNum][syukinJtanSita1danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinJtanSita2danHutokeiHonsu] +
                                                 (int)data.Rows[currentNum][syukinJtanSita3danHutokeiHonsu];

                                    hosoCount = (int)data.Rows[currentNum][syukinJtanSita1danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinJtanSita2danHosokeiHonsu] +
                                                (int)data.Rows[currentNum][syukinJtanSita3danHosokeiHonsu];

                                    if (rebarCount > 1 && (string)data.Rows[currentNum][syukinJtanSitaHutokei] != "")
                                    {
                                        title = rebarCount + "-" + (string)data.Rows[currentNum][syukinJtanSitaHutokei];
                                        if (hosoCount > 0 && (string)data.Rows[currentNum][syukinJtanSitaHosokei] != "")
                                        {
                                            title += " + " + hosoCount + "-" + (string)data.Rows[currentNum][syukinJtanSitaHosokei];
                                        }

                                        origin = sitabakinTitlePoints[2];

                                        txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                        //if (otherTNT != null)
                                        //{
                                        //  txtNote.ChangeTypeId(otherTNT.Id);
                                        //}
                                        //txtnote.width = w_jtanWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                    }

                                    //                  trans.Commit();
                                }

                                if (strRet.Contains("IDS_ERR_HUTO_FAMILY_NOTSET") == false &&
                                    strRet.Contains("IDS_ERR_HOSO_FAMILY_NOTSET") == false &&
                                    rectangleCrvs3.Count == 4)
                                {
                                    #region 断面枠の本数表示

                                    if (rebarShow == 1)
                                    {
                                        //                    trans.Start("RebarNumberShow");

                                        int ue1dan = (int)data.Rows[currentNum][syukinJtanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanUe1danHosokeiHonsu];
                                        int sita1dan = (int)data.Rows[currentNum][syukinJtanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanSita1danHosokeiHonsu];

                                        int length = 0;

                                        if (ue1dan > sita1dan)
                                        {
                                            length = ue1dan.ToString().Length;
                                        }
                                        else
                                        {
                                            length = sita1dan.ToString().Length;
                                        }

                                        double txtMaxW = 0;
                                        Collections.Generic.IList<Revit.DB.TextNote> tntAry = new Collections.Generic.List<Revit.DB.TextNote>();

                                        if (isSyukinUeSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba3, takasa3, centerRectangle3);

                                            Revit.DB.XYZ innerTop = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarTop.Y, rectanglePoints[1].Z);

                                            //int ue1dan = (int)data.Rows[currentNum][syukinJtanUe1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanUe1danHosokeiHonsu];
                                            int ue2dan = (int)data.Rows[currentNum][syukinJtanUe2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanUe2danHosokeiHonsu];
                                            int ue3dan = (int)data.Rows[currentNum][syukinJtanUe3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanUe3danHosokeiHonsu];

                                            title = ue1dan.ToString();
                                            if (ue2dan > 0 && isSyukinUeSet > 1)
                                            {
                                                title += "\r\n" + ue2dan.ToString();
                                                if (ue3dan > 0 && isSyukinUeSet > 2)
                                                {
                                                    title += "\r\n" + ue3dan.ToString();
                                                }
                                            }

                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerTop, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtNote.Width;
                                            tntAry.Add(txtNote);
                                        }

                                        if (isSyukinSitaSet > 0)
                                        {
                                            Collections.Generic.IList<Revit.DB.XYZ> rectanglePoints = cmpGeometry.RectanglePoints(haba3, takasa3, centerRectangle3);

                                            Revit.DB.XYZ innerBottom = new Revit.DB.XYZ(rectanglePoints[1].X, cmpService.RebarBtm.Y, rectanglePoints[1].Z);

                                            //int sita1dan = (int)data.Rows[currentNum][syukinJtanSita1danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanSita1danHosokeiHonsu];
                                            int sita2dan = (int)data.Rows[currentNum][syukinJtanSita2danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanSita2danHosokeiHonsu];
                                            int sita3dan = (int)data.Rows[currentNum][syukinJtanSita3danHutokeiHonsu] + (int)data.Rows[currentNum][syukinJtanSita3danHosokeiHonsu];

                                            title = sita1dan.ToString();
                                            if (sita2dan > 0 && isSyukinSitaSet > 1)
                                            {
                                                title = sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                if (sita3dan > 0 && isSyukinSitaSet > 2)
                                                {
                                                    title = sita3dan.ToString() + "\r\n" + sita2dan.ToString() + "\r\n" + sita1dan.ToString();
                                                }
                                            }

                                            double txtSize = otherTNT.get_Parameter(Revit.DB.BuiltInParameter.TEXT_SIZE).AsDouble();

                                            double a = 0.000057930;
                                            double b = 0.000164979;

                                            lineWidth = (a * scale + b) * 304.8 * txtSize * length;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            txtNote = cmpService.CreateNewTextNote_RebarSyukinOffset(vp, innerBottom, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}

                                            trans.Start("Regenerate");
                                            rvtDbDoc.Regenerate();
                                            trans.Commit();

                                            //txtNote.Text = title;

                                            txtMaxW = txtNote.Width;
                                            tntAry.Add(txtNote);
                                        }

                                        //foreach (Revit.DB.TextNote tnt in tntAry)
                                        //{
                                        //  tnt.Width = txtMaxW;
                                        //  tnt.Location.Move(new Revit.DB.XYZ(tnt.Width / 2, 0, 0));
                                        //  tnt.get_Parameter(Revit.DB.BuiltInParameter.TEXT_ALIGN_HORZ).Set(256);
                                        //}

                                        //                    trans.Commit();
                                    }

                                    #endregion
                                }

                                if (isStirrupSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 肋筋
                                    // まとめて表示
                                    if (stirrupFrameShow == 0 && cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == true)
                                    {
                                    }
                                    // 断面別に表示
                                    else if (stirrupFrameShow == 1 || cmpService.IsSameStirrupBySection(data.Rows[currentNum]) == false)
                                    {
                                        lineWidth = w_jtanWaku / scale;// w_HugowakuMax / scale / 3;
                                        flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                        // 肋筋終端
                                        if ((int)data.Rows[currentNum][e_Stirrup_Number] > 1 && (string)data.Rows[currentNum][e_Stirrup_Diameter] != "" && (double)data.Rows[currentNum][e_Stirrup_Pitch] > 0)
                                        {
                                            if (stirrupBracketShow == 0)
                                            {
                                                title = (int)data.Rows[currentNum][e_Stirrup_Number] + stirrupSpaceSymbol + (string)data.Rows[currentNum][e_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][e_Stirrup_Pitch];
                                            }
                                            else
                                            {
                                                title = "[" + (int)data.Rows[currentNum][e_Stirrup_Number] + "] " + (string)data.Rows[currentNum][e_Stirrup_Diameter] + "@" + (double)data.Rows[currentNum][e_Stirrup_Pitch];
                                            }

                                            origin = stirrupTitlePoints[2];
                                            txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                            //if (otherTNT != null)
                                            //{
                                            //  txtNote.ChangeTypeId(otherTNT.Id);
                                            //}
                                            //txtnote.width = w_jtanWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                        }
                                    }

                                    //                  trans.Commit();
                                }

                                if (isWebSet)
                                {
                                    //                  trans.Start("鉄筋本数径");

                                    // 腹筋
                                    if (webFrameShow != 2)
                                    {
                                        // まとめて表示
                                        if (webFrameShow == 0 && cmpService.IsSameWebBySection(data.Rows[currentNum]) == true)
                                        {
                                        }
                                        else if (webFrameShow == 1 || cmpService.IsSameWebBySection(data.Rows[currentNum]) == false)
                                        {
                                            lineWidth = w_jtanWaku / scale;// w_HugowakuMax / scale / 3;
                                            flags = Revit.DB.TextAlignFlags.TEF_ALIGN_CENTER | Revit.DB.TextAlignFlags.TEF_ALIGN_MIDDLE;

                                            // 腹筋終端
                                            if ((int)data.Rows[currentNum][e_Web_Number] > 1 && (string)data.Rows[currentNum][e_Web_Diameter] != "")
                                            {
                                                title = (int)data.Rows[currentNum][e_Web_Number] + "-" + (string)data.Rows[currentNum][e_Web_Diameter];

                                                if ((int)data.Rows[currentNum][e_Web_Number] == 0)
                                                {
                                                    title = "-";
                                                }

                                                origin = harakinTitlePoints[2];

                                                txtNote = cmpService.CreateNewTextNote(vp, origin, baseVec, lineWidth, otherTNT.Id, title, rvtDbDoc);

                                                //if (otherTNT != null)
                                                //{
                                                //  txtNote.ChangeTypeId(otherTNT.Id);
                                                //}
                                                //txtnote.width = w_jtanWaku;// w_HugowakuMax / 3;// hugoMaxX;
                                            }
                                        }
                                    }

                                    //                  trans.Commit();
                                }

                                #endregion
                            }
                            #endregion

                            #endregion
                        }

                        // 左下を左上に
                        hidariUe = hidariSita;
                    }

                    hugoHidariUe += new Revit.DB.XYZ(hugoMaxX + otherWidth, 0, 0);

                    hidariUe = new Revit.DB.XYZ(hugoHidariUe.X, hugoHidariUe.Y + h_Hugowaku + h_Ichiwaku, hugoHidariUe.Z);
                }

                #endregion

                trans.Start("Draw");

                foreach (Revit.DB.Curve crv in crvFrameAry)
                {
                    Revit.DB.DetailCurve dc = rvtDbDoc.Create.NewDetailCurve(vp, crv);
                    dc.LineStyle = frameLineType;
                }

                trans.Commit();

                kaiHidariUe += new Revit.DB.XYZ(sumWidth, 0, 0);

                // ビューの要素
                Revit.DB.FilteredElementCollector filterCollector = new Revit.DB.FilteredElementCollector(rvtDbDoc, vp.Id);
                filterCollector.WhereElementIsNotElementType();

                Collections.Generic.ICollection<Revit.DB.ElementId> eIds = new Collections.Generic.List<Revit.DB.ElementId>();

                foreach (Revit.DB.Element e in filterCollector)
                {
                    // 非表示は除外
                    if (e.IsHidden(vp))
                    {
                        continue;
                    }

                    // 詳細線分
                    Revit.DB.DetailCurve dc = e as Revit.DB.DetailCurve;
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

                rvtUiDoc.ShowElements(eIds);

                trans.Start("Show");
                rvtDbDoc.Regenerate();
                trans.Commit();

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

                imgExprtOpt.FilePath = exportFolder + "\\" + name;

                // 書き出しビュー名
                imgExprtOpt.ViewName = vp.Name;

                // ファイル形式
                imgExprtOpt.HLRandWFViewsFileType = Revit.DB.ImageFileType.PNG;

                // ズームタイプ
                imgExprtOpt.ZoomType = Revit.DB.ZoomFitType.FitToPage;

                // サイズ
                imgExprtOpt.PixelSize = 1000;

                // 画像出力
                rvtDbDoc.ExportImage(imgExprtOpt);

                // 作図したものを削除
                trans.Start("Delete");
                rvtDbDoc.Delete(eIds);
                trans.Commit();

                isExported = true;
            }

            // 実行時のビューにもどす
            rvtUiDoc.ActiveView = current;

            if (current.Id != vp.Id)
            {
                // 作図に使ったビューを削除
                trans.Start("Delete");
                rvtDbDoc.Delete(vp.Id);
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

            if (trans.GetStatus() == Revit.DB.TransactionStatus.Started)
            {
                trans.Commit();
            }

            if (writeErr != "")
            {
                System.Windows.MessageBox.Show(writeErr, cmpAttribute.ResourceText("IDS_ERR_TITLE"));
            }

            if (isExported)
            {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_TXT_IMAGEEXPORTED"));
            }

            retCmd = Revit.UI.Result.Succeeded;
            transGroup.Assimilate();
            return retCmd;
        }

        #endregion
    }
}