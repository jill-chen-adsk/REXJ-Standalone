Imports Autodesk.Revit
Imports Autodesk.Revit.UI
Imports ADSK.MepVerticalMark.My.Resources
'Imports ADSK.MepVerticalMark.Res

<Attributes.Transaction(Attributes.TransactionMode.Manual)>
Public Class App
    Implements IExternalApplication
    Public Function OnStartup(ByVal app As UIControlledApplication) As Result Implements IExternalApplication.OnStartup
        Dim pushBtnData As PushButtonData = Nothing
        Dim assembly As Reflection.Assembly = Reflection.Assembly.GetExecutingAssembly()
        Dim assemblyPath As String = IO.Path.GetDirectoryName(assembly.Location)
        
        Dim res As Res = New Res()
        
        '' リボンタブ
        Dim ribbonPanels As IList(Of RibbonPanel) = New List(Of RibbonPanel)
        
        Dim tabName As String = IDS_BTN_TABNAME

        
        Try
            If Not (app Is Nothing) Then
                ribbonPanels = app.GetRibbonPanels(tabName)
            End If
        Catch ex As Exception
        End Try
        If (ribbonPanels.Count = 0) Then
            app.CreateRibbonTab(tabName)
        End If

        '' リボンパネル
        Dim ribbonPanel As RibbonPanel = app.CreateRibbonPanel(tabName, IDS_BTN_PANELNAME)

        '' F1ヘルプ
        Dim helpFilePath As String = IO.Path.Combine(assemblyPath, IDS_TXT_HELPFOLDER, IDS_TXT_HELP)
        Dim contentHelp As ContextualHelp = Nothing
        If (IO.File.Exists(helpFilePath)) Then
            contentHelp = New ContextualHelp(ContextualHelpType.Url, helpFilePath)
        End If

        '' 矢羽ダクト
        Dim path As String = IO.Path.Combine(assemblyPath, IDS_BTN_VERTICALDUCTMARK_ASSEMBLYNAME)
        If IO.File.Exists(path) Then
            pushBtnData = New PushButtonData(IDS_BTN_VERTICALDUCTMARK_NAME,
                                         IDS_BTN_VERTICALDUCTMARK_TEXT,
                                         path,
                                         IDS_BTN_VERTICALDUCTMARK_CLASSNAME)
            pushBtnData.Image = res.ICON_SMALL_YABANE_DUCT
'            Dim bitmapS As Drawing.Bitmap = res.ICON_SMALL_YABANE_DUCT
'            If Not IsNothing(bitmapS) Then
''                pushBtnData.Image = System. Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmapS.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions())
'            End If
            pushBtnData.LargeImage = res.ICON_LARGE_YABANE_DUCT

            '            Dim bitmapL As Drawing.Bitmap = res.ICON_LARGE_YABANE_DUCT
'            If Not IsNothing(bitmapL) Then
''                pushBtnData.LargeImage = System. Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmapL.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions())
'            End If
            pushBtnData.ToolTip = IDS_BTN_VERTICALDUCTMARK_TOOLTIP
            If Not IsNothing(contentHelp) Then
                pushBtnData.SetContextualHelp(contentHelp)
            End If
            ribbonPanel.AddItem(pushBtnData)
        End If

        '' 矢羽配管
        path = IO.Path.Combine(assemblyPath, IDS_BTN_VERTICALPIPEMARK_ASSEMBLYNAME)
        If IO.File.Exists(path) Then
            pushBtnData = New PushButtonData(IDS_BTN_VERTICALPIPEMARK_NAME,
                                         IDS_BTN_VERTICALPIPEMARK_TEXT,
                                         path,
                                         IDS_BTN_VERTICALPIPEMARK_CLASSNAME)
            pushBtnData.Image = res.ICON_SMALL_YABANE_PIPE
            '            Dim bitmapS As Drawing.Bitmap = res.ICON_SMALL_YABANE_PIPE
'            If Not IsNothing(bitmapS) Then
'
''                pushBtnData.Image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmapS.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions())
'            End If
'            Dim bitmapL As Drawing.Bitmap = res.ICON_LARGE_YABANE_PIPE
            pushBtnData.LargeImage = res.ICON_LARGE_YABANE_PIPE
'            If Not IsNothing(bitmapL) Then
'                pushBtnData.LargeImage = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmapL.GetHbitmap(), IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions())
'            End If
            pushBtnData.ToolTip = IDS_BTN_VERTICALPIPEMARK_TOOLTIP
            If Not IsNothing(contentHelp) Then
                pushBtnData.SetContextualHelp(contentHelp)
            End If
            ribbonPanel.AddItem(pushBtnData)
        End If
        Return Result.Succeeded
    End Function

    Public Function OnShutdown(ByVal app As UIControlledApplication) As Result Implements IExternalApplication.OnShutdown
        Return Result.Succeeded
    End Function
End Class
