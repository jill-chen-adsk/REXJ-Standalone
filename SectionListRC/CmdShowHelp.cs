using System;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace SectionListRC
{
    /// ================================================================================
    /// <summary>コマンド ヘルプ</summary>
    /// ================================================================================
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    [Revit.Attributes.RegenerationAttribute(Revit.Attributes.RegenerationOption.Manual)]
    public class CmdShowHelp : Revit.UI.IExternalCommand
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
        /// <history>2014/02/04 Created GSA,Inc. Ryo Kuroda</history>
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

            // 戻り値
            Revit.UI.Result retCmd = Revit.UI.Result.Cancelled;

            // 操作マニュアルパス
            // 実行フォルダ
            string filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" +
                              "RC_SectionList_Manual.pdf";

            if (System.IO.File.Exists(filePath)) {
                System.Diagnostics.Process.Start(filePath);

                retCmd = Revit.UI.Result.Succeeded;
            }
            else {
                System.Windows.Forms.MessageBox.Show(cmpAttribute.ResourceText("IDS_ERR_HELPFILE"));

                retCmd = Revit.UI.Result.Failed;
            }

            return retCmd;
        }

        #endregion Member Functions
    }
}