using System;
using System.IO ;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using MepManholeTool.Const;
using MepManholeTool.Properties;
using MepManholeTool.Utils;
using Revit       = Autodesk.Revit;

namespace MepManholeTool.Tools
{
    [Revit.Attributes.TransactionAttribute(Revit.Attributes.TransactionMode.Manual)]
    public class MepManhole : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication? application)
        {
            // 引数がnullの場合はFailedを返却。
            if (application == null) return Result.Failed;

            //Setup menu
            MepManhole.SetRexjMenu(application);

            //Load Parameter Mapping Info
            GlobalMappings.Instance.Init();
            
            // DocumentOpenedイベントを登録して、ドキュメントを開いた時にファミリをロードする
            application.ControlledApplication.DocumentOpened += OnDocumentOpened;
            
            // DocumentCreatedイベントを登録して、新規ドキュメント作成時にもファミリをロードする
            application.ControlledApplication.DocumentCreated += OnDocumentCreated;
            
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            // イベントハンドラを解除
            application.ControlledApplication.DocumentOpened -= OnDocumentOpened;
            application.ControlledApplication.DocumentCreated -= OnDocumentCreated;
            return Result.Succeeded;
        }
        
        /// <summary>
        /// ドキュメントが開かれた時に必要なファミリをロードします。
        /// </summary>
        private void OnDocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            try
            {
                Document doc = e.Document;
                if (doc == null) return;
                
                LoadFamiliesForDocument(doc);
            }
            catch (Exception ex)
            {
                // エラーが発生してもアプリケーションを停止しない
                TaskDialog.Show("Family Load Warning", $"Failed to load some families:\n{ex.Message}");
            }
        }
        
        /// <summary>
        /// 新規ドキュメントが作成された時に必要なファミリをロードします。
        /// </summary>
        private void OnDocumentCreated(object sender, DocumentCreatedEventArgs e)
        {
            try
            {
                Document doc = e.Document;
                if (doc == null) return;
                
                LoadFamiliesForDocument(doc);
            }
            catch (Exception ex)
            {
                // エラーが発生してもアプリケーションを停止しない
                TaskDialog.Show("Family Load Warning", $"Failed to load some families:\n{ex.Message}");
            }
        }
        
        /// <summary>
        /// ドキュメントに必要なファミリをロードします。
        /// </summary>
        private void LoadFamiliesForDocument(Document doc)
        {
            var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(folder)) return;
            
            LoadFamiliesForVersion(doc, Path.Combine(folder, "Resources", "rfa2026"));
        }
        
        /// <summary>
        /// 指定されたバージョンのフォルダから必要なファミリをロードします。
        /// </summary>
        private void LoadFamiliesForVersion(Document doc, string rfaFolder)
        {
            // ロードする必要があるファミリのリスト
            string[] familyNames = new[]
            {
                "RJ_2D_凡_配管付属品_桝_断面",
                "桝詳細項目タグ",
                "00_RJ_タグ_配管付属品_CL15",
                "08070_公共桝",
                "08060_ため桝_RC",
                "08050_インバート桝_SC",
                "08060_トラップ桝"
            };
            
            foreach (string familyName in familyNames)
            {
                LoadSymbolAndTag(doc, rfaFolder, familyName);
            }
        }
        
        /// <summary>
        /// ファミリをドキュメントにロードします。既にロードされている場合はスキップします。
        /// </summary>
        private static void LoadSymbolAndTag(Document doc, string folder, string familyFileName)
        {
            string familyFile = Path.Combine(folder, familyFileName + ".rfa");
            
            // ファイルが存在するか確認
            if (!File.Exists(familyFile))
            {
                return; // ファイルが存在しない場合は静かにスキップ
            }
            
            string familyName = Path.GetFileNameWithoutExtension(familyFile);
            var lstFamilyInProject = new FilteredElementCollector(doc)
                .OfClass(typeof(Family))
                .Cast<Family>()
                .Where(x => x.Name == familyName)
                .ToList();
                
            if (lstFamilyInProject.Count == 0)
            {
                using (Transaction tran = new Transaction(doc, $"Load {familyFileName}"))
                {
                    tran.Start();
                    Family family = null;
                    doc.LoadFamily(familyFile, out family);
                    tran.Commit();
                }
            }
        }
        
        private static void SetRexjMenu(UIControlledApplication application)
        {
            var assemblyPath = Assembly.GetExecutingAssembly().Location;
            
            RevitUIUtilis.CreateRibbonTab(application, MepModelLineConst.REVITリボン_タブ_タイトル);
            var panel = RevitUIUtilis.CreateRibbonPanel(application, MepModelLineConst.REVITリボン_タブ_タイトル, MepModelLineConst.REVITリボン_機能名_桝ツール);

            SplitButtonData splitButtonData = new SplitButtonData("MappingSplit", "Mapping Options");
            SplitButton splitButton = panel.AddItem(splitButtonData) as SplitButton;
            
            var helpPath = $"{Path.GetDirectoryName( assemblyPath )}\\Help\\桝ツール.pdf" ;
            // モデル線分作成ボタン プルダウン
            var push1 = new PushButtonData("MepModelLine", MepModelLineConst.REVITリボン_モデル線分作成_プルダウン名_線分, assemblyPath, typeof(MepModelLineCommand).FullName);
            push1.ToolTip = MepModelLineConst.REVITツールチップ_モデル線分;
            push1.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, helpPath));
            
            var push2 = new PushButtonData("Mapping Parameter", MepModelLineConst.REVITリボン_モデル線分作成_プルダウン名_パラメータ, assemblyPath, typeof(MepModelLineParameterCommand).FullName);
            push2.ToolTip = MepModelLineConst.REVITツールチップ_モデルパラメータマッピング;
            push2.SetContextualHelp(new ContextualHelp(ContextualHelpType.Url, helpPath));

            if ( splitButton.AddPushButton( push1 ) is PushButton pushButton1 ) {
                RevitUIUtilis.TrySetBitmapResourceToButton(pushButton1, Resources.MepModelLineOpen);
            }
            
            if ( splitButton.AddPushButton( push2 ) is PushButton pushButton2 ) {
                RevitUIUtilis.TrySetBitmapResourceToButton(pushButton2, Resources.MepModelLineParameter);
            }
        }
    }
}