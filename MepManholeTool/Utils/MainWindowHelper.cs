using Autodesk.Revit.UI;
using System;
using System.Windows;
using System.Windows.Interop;

namespace MepManholeTool.Utils
{
  public class MainWindowHelper
  {
    public static Window GetRevitMainWindow(UIApplication uiApp)
    {
      IntPtr mainWindowHandle = uiApp.MainWindowHandle;

      // Create a WindowInteropHelper and get the Window from it
      Window mainWindow = (Window)HwndSource.FromHwnd(mainWindowHandle)?.RootVisual;

      if (mainWindow == null)
      {
        // This can happen because Revit's main window is not a WPF window
        // So usually you would use the handle as the owner of your custom window
        mainWindow = new Window
        {
          WindowStyle = WindowStyle.None,
          ShowInTaskbar = false,
          Width = 0,
          Height = 0,
          Left = -10000,
          Top = -10000
        };

        new WindowInteropHelper(mainWindow).Owner = mainWindowHandle;
        mainWindow.Show();
        mainWindow.Hide();
      }

      return mainWindow;
    }
  }
}