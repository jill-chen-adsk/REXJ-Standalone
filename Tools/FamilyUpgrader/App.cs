using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

namespace REXJ.FamilyUpgrader;

/// <summary>
/// Upgrades the REXJ bundled families to the running Revit version on Revit's first idle moment,
/// then disables its own add-in manifest so it only runs once.
/// </summary>
public class App : IExternalApplication
{
    private const string ManifestName = "REXJ.FamilyUpgrader.addin";
    private const string SolutionFileName = "REXJ-Standalone.sln";
    private static readonly Regex BackupFilePattern = new(@"\.\d{4}\.rfa$", RegexOptions.IgnoreCase);

    private bool _running;

    public Result OnStartup(UIControlledApplication application)
    {
        // Idling rather than ApplicationInitialized: Revit 2027 can load a new manifest mid-session,
        // after ApplicationInitialized has already fired.
        application.Idling += OnIdling;
        application.ControlledApplication.FailuresProcessing += OnFailuresProcessing;
        application.DialogBoxShowing += OnDialogBoxShowing;
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        application.Idling -= OnIdling;
        application.ControlledApplication.FailuresProcessing -= OnFailuresProcessing;
        application.DialogBoxShowing -= OnDialogBoxShowing;
        return Result.Succeeded;
    }

    private void OnIdling(object? sender, IdlingEventArgs e)
    {
        if (sender is not UIApplication uiApp) return;

        uiApp.Idling -= OnIdling;
        RunUpgrade(uiApp.Application);
    }

    private void RunUpgrade(Application app)
    {
        var assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var log = new StringBuilder();
        int upgraded = 0, skipped = 0, failed = 0;

        _running = true;
        try
        {
            var targetVersion = app.VersionNumber;
            var repoRoot = FindRepoRoot(assemblyDir);
            var tempDir = Path.Combine(Path.GetTempPath(), "REXJFamilyUpgrade");
            Directory.CreateDirectory(tempDir);

            log.AppendLine($"REXJ family upgrade to Revit {targetVersion} - {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            log.AppendLine($"Root: {repoRoot}");

            foreach (var folder in ReadFolders(Path.Combine(assemblyDir, "folders.txt"), repoRoot))
            {
                if (!Directory.Exists(folder))
                {
                    log.AppendLine($"MISSING FOLDER  {folder}");
                    continue;
                }

                foreach (var path in Directory.GetFiles(folder, "*.rfa", SearchOption.TopDirectoryOnly))
                {
                    if (BackupFilePattern.IsMatch(path)) continue;

                    try
                    {
                        var format = BasicFileInfo.Extract(path).Format;
                        if (format == targetVersion)
                        {
                            skipped++;
                            log.AppendLine($"SKIP ({format})   {path}");
                            continue;
                        }

                        UpgradeFamily(app, path, tempDir);
                        upgraded++;
                        log.AppendLine($"UPGRADED ({format} -> {targetVersion})   {path}");
                    }
                    catch (Exception ex)
                    {
                        failed++;
                        log.AppendLine($"FAILED   {path}   {ex.Message}");
                    }
                }
            }

            DisableManifest(targetVersion, log);
        }
        catch (Exception ex)
        {
            failed++;
            log.AppendLine($"ABORTED   {ex}");
        }
        finally
        {
            _running = false;
        }

        log.AppendLine($"Upgraded: {upgraded}, already current: {skipped}, failed: {failed}");
        var logPath = Path.Combine(assemblyDir, "FamilyUpgrader.log");
        File.WriteAllText(logPath, log.ToString(), Encoding.UTF8);

        TaskDialog.Show("REXJ Family Upgrader",
            $"Upgraded: {upgraded}\nAlready current: {skipped}\nFailed: {failed}\n\nLog: {logPath}");
    }

    private static void UpgradeFamily(Application app, string path, string tempDir)
    {
        // Saving to a temp copy with the same file name keeps the family name unchanged.
        var tempPath = Path.Combine(tempDir, Path.GetFileName(path));
        var doc = app.OpenDocumentFile(path);
        try
        {
            if (!doc.IsFamilyDocument)
                throw new InvalidOperationException("Not a family document.");

            doc.SaveAs(tempPath, new SaveAsOptions { OverwriteExistingFile = true, Compact = true, MaximumBackups = 1 });
        }
        finally
        {
            doc.Close(false);
        }

        // Another add-in's DocumentOpened handler can load extra families into the document before it is saved.
        var originalSize = new FileInfo(path).Length;
        var upgradedSize = new FileInfo(tempPath).Length;
        if (upgradedSize > originalSize * 1.5 + 512 * 1024)
        {
            File.Delete(tempPath);
            throw new InvalidOperationException(
                $"Upgraded file grew from {originalSize / 1024} KB to {upgradedSize / 1024} KB; original kept. " +
                "Check for add-ins that load families on DocumentOpened.");
        }

        File.Copy(tempPath, path, overwrite: true);
        File.Delete(tempPath);
    }

    private static IEnumerable<string> ReadFolders(string configPath, string repoRoot)
    {
        foreach (var raw in File.ReadAllLines(configPath, Encoding.UTF8))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            yield return Path.GetFullPath(Path.IsPathRooted(line) ? line : Path.Combine(repoRoot, line));
        }
    }

    private static string FindRepoRoot(string startDir)
    {
        for (var dir = new DirectoryInfo(startDir); dir != null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, SolutionFileName)))
                return dir.FullName;
        }
        throw new DirectoryNotFoundException($"{SolutionFileName} not found above {startDir}");
    }

    private static void DisableManifest(string revitVersion, StringBuilder log)
    {
        var manifest = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Autodesk", "Revit", "Addins", revitVersion, ManifestName);
        if (!File.Exists(manifest)) return;

        var disabled = manifest + ".done";
        File.Delete(disabled);
        File.Move(manifest, disabled);
        log.AppendLine($"Manifest disabled: {disabled}");
    }

    private void OnFailuresProcessing(object? sender, FailuresProcessingEventArgs e)
    {
        if (!_running) return;

        var accessor = e.GetFailuresAccessor();
        foreach (var failure in accessor.GetFailureMessages())
        {
            if (failure.GetSeverity() == FailureSeverity.Warning)
                accessor.DeleteWarning(failure);
        }
    }

    private void OnDialogBoxShowing(object? sender, DialogBoxShowingEventArgs e)
    {
        if (_running) e.OverrideResult((int)TaskDialogResult.Ok);
    }
}
