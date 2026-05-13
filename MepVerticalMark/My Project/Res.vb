Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Windows.Media
Imports System.Windows.Media.Imaging

Public Class Res
    Public Const CMD_VERTICALMARK = "Create Pipe Arrow Mark"
    Public Const CMD_VERTICAL_DUCT = "Create Duct Arrow Mark"
    Public Const IDS_BTN_PANELNAME = "Arrow Mark Tool"
    Public Const IDS_BTN_TABNAME = "REXJ Standalone"
    Public Const IDS_BTN_VERTICALDUCTMARK_ASSEMBLYNAME = "ADSK.MepVerticalMark.dll"
    Public Const IDS_BTN_VERTICALDUCTMARK_CLASSNAME = "ADSK.MepVerticalMark.CmdVerticalDuctMark"
    Public Const IDS_BTN_VERTICALDUCTMARK_NAME = "Arrow Duct"
    Public Const IDS_BTN_VERTICALDUCTMARK_TEXT = "Arrow" & vbLf & "Duct"
    Public Const IDS_BTN_VERTICALDUCTMARK_TOOLTIP = "Creates arrow marks for vertical ducts"
    Public Const IDS_BTN_VERTICALPIPEMARK_ASSEMBLYNAME = "ADSK.MepVerticalMark.dll"
    Public Const IDS_BTN_VERTICALPIPEMARK_CLASSNAME = "ADSK.MepVerticalMark.CmdVerticalPipeMark"
    Public Const IDS_BTN_VERTICALPIPEMARK_NAME = "Arrow Pipe"
    Public Const IDS_BTN_VERTICALPIPEMARK_TEXT = "Arrow" & vbLf & "Pipe"
    Public Const IDS_BTN_VERTICALPIPEMARK_TOOLTIP = "Creates arrow marks for vertical pipes"
    Public Const IDS_ERR_CREATEDETAILLINEERR = "Error creating detail line"
    Public Const IDS_ERR_FAMILYNOTLOAD = "{0} is not loaded."
    Public Const IDS_ERR_INVALIDPOINT = "Invalid point position."
    Public Const IDS_ERR_NOCURRENTLEVELDUCT = "No duct found passing through {0}+100."
    Public Const IDS_ERR_NOCURRENTLEVELPIPE = "No pipe found passing through {0}+100."
    Public Const IDS_ERR_NOLEADERLINESTYLE = "Line style ""{0}"" is not defined."
    Public Const IDS_ERR_NOTVIEWPLAN = "Please run this command from a floor plan view."
    Public Const IDS_ERR_NOUPLEVELDUCT = "No duct found passing through {0}-100."
    Public Const IDS_ERR_NOUPLEVELPIPE = "No pipe found passing through {0}-100."
    Public Const IDS_ERR_NOUPPERLEVEL = "No upper level found."
    Public Const IDS_ERR_NO_SPECIFIED_DUCT = "No duct found passing through {0}{1}."
    Public Const IDS_ERR_NO_SPECIFIED_PIPE = "No pipe found passing through {0}{1}."
    Public Const IDS_ERR_SELECTDUCT = "Please select a duct."
    Public Const IDS_ERR_SELECTPIPE = "Please select a pipe."
    Public Const IDS_INFO_OPERATIONCANCEL = "Operation was cancelled."
    Public Const IDS_STATUS_SELECTDUCT = "Select vertical duct"
    Public Const IDS_STATUS_SELECTPIPE = "Select vertical pipe"
    Public Const IDS_STATUS_SELECTPLACETAGPOINT = "Specify a point to place the tag."
    Public Const IDS_STATUS_SELECTSTARTDUCT = "Select the starting duct"
    Public Const IDS_STATUS_SELECTSTARTPIPE = "Select the starting pipe"
    Public Const IDS_TXT_COLON = ":"
    Public Const IDS_TXT_DOWNDIR = "Downward"
    Public Const IDS_TXT_ERR = "Error"
    Public Const IDS_TXT_HELP = "RME 矢羽配管・ダクト.pdf"
    Public Const IDS_TXT_HELPFOLDER = "Resources"
    Public Const IDS_TXT_INFO = "Info"
    Public Const IDS_TXT_UPDIR = "Upward"
    Public Const IDS_TXT_ZATSUHAISUI = "雑排水"
    Public Const LINESTYLE_LEADER = "*引出線 - D2/黒/実線"
    Public Const MARK_D = "D"
    Public Const MARK_HAISUI = ")"
    Public Const MARK_R = "R"
    Public Const TAG_DUCT_SIZE_100 = "タグ 矢羽 ダクト サイズ_SC 100"
    Public Const TAG_DUCT_SIZE_200 = "タグ 矢羽 ダクト サイズ_SC 200"
    Public Const TAG_DUCT_YABANE_100 = "タグ 矢羽 ダクト システム省略形_SC 100"
    Public Const TAG_DUCT_YABANE_200 = "タグ 矢羽 ダクト システム省略形_SC 200"
    Public Const TAG_PIPE_REIBAI_100 = "タグ 矢羽 配管 冷媒管符号_SC 100"
    Public Const TAG_PIPE_REIBAI_200 = "タグ 矢羽 配管 冷媒管符号_SC 200"
    Public Const TAG_PIPE_SIZE_100 = "タグ 矢羽 配管 サイズ_SC 100"
    Public Const TAG_PIPE_SIZE_200 = "タグ 矢羽 配管 サイズ_SC 200"
    Public Const TAG_PIPE_YABANE_EX_100 = "タグ 矢羽 配管 システム省略形_排水_SC 100"
    Public Const TAG_PIPE_YABANE_EX_200 = "タグ 矢羽 配管 システム省略形_排水_SC 200"
    Public Const TAG_PIPE_YABANE_SA_100 = "タグ 矢羽 配管 システム省略形_供給_SC 100"
    Public Const TAG_PIPE_YABANE_SA_200 = "タグ 矢羽 配管 システム省略形_供給_SC 200"
    Public Const TRANS_CREATELINESTYLE = "Create Line Style"
    Public Const TRANS_CREATEMARK = "Create Arrow Mark"
    Public Const TRANS_CREATEUNDERLINE = "Create Underline"
    Public Const TRANS_MOVETAG = "Move Tag"
    Public Const TYPE_COMMA = "カンマ"
    Public Const TYPE_NASHI = "00_なし"
    Public Const TYPE_OSUI = "01_汚水"
    Public Const TYPE_SHITA = "02_下"
    Public Const TYPE_STANDARD = "標準"
    Public Const TYPE_TSUKI = "00_通気"
    Public Const TYPE_UE = "01_上"
    Public Const TYPE_ZATSUHAISUI = "02_雑排水"

'    Public ReadOnly ICON_LARGE_YABANE_DUCT = EmbeddedBitmap("Resources\ICON_LARGE_YABANE_DUCT.png") 
'    Public ReadOnly ICON_LARGE_YABANE_PIPE = EmbeddedBitmap("Resources\ICON_LARGE_YABANE_PIPE.png")
'    Public ReadOnly ICON_SMALL_YABANE_DUCT = EmbeddedBitmap("Resources\ICON_SMALL_YABANE_DUCT.png")
'    Public ReadOnly ICON_SMALL_YABANE_PIPE = EmbeddedBitmap("Resources\ICON_SMALL_YABANE_PIPE.png") 
'    Public ReadOnly VerticalMark0 = EmbeddedBitmap("Resources\VerticalMark0.png")
'    Public ReadOnly VerticalMark1 = EmbeddedBitmap("Resources\VerticalMark1.png")
    
    Public ReadOnly ICON_LARGE_YABANE_DUCT = EmbeddedBitmapSource("ICON_LARGE_YABANE_DUCT.png") 
    Public ReadOnly ICON_LARGE_YABANE_PIPE = EmbeddedBitmapSource("ICON_LARGE_YABANE_PIPE.png")
    Public ReadOnly ICON_SMALL_YABANE_DUCT = EmbeddedBitmapSource("ICON_SMALL_YABANE_DUCT.png")
    Public ReadOnly ICON_SMALL_YABANE_PIPE = EmbeddedBitmapSource("ICON_SMALL_YABANE_PIPE.png") 
    Public Shared ReadOnly VerticalMark0 As Bitmap = EmbeddedBitmap("VerticalMark0.png")
    Public Shared ReadOnly VerticalMark1 As Bitmap = EmbeddedBitmap("VerticalMark1.png") 
    
    Private Shared Function EmbeddedBitmap(resourceName As String) As Bitmap
        Dim assembly As Assembly = Assembly.GetExecutingAssembly()
        
        Using stream As IO.Stream = assembly.GetManifestResourceStream("ADSK.MepVerticalMark." + resourceName)
            If stream IsNot Nothing Then
                Return  New Bitmap(stream)
            Else
                Throw New Exception("Resource not found " & resourceName)
            End If
        End Using
    End Function
    
    Private Shared Function EmbeddedBitmapSource(resourceName As String) As ImageSource
        Dim assembly As Assembly = Assembly.GetExecutingAssembly()
        
        Using stream As IO.Stream = assembly.GetManifestResourceStream("ADSK.MepVerticalMark." + resourceName)
            If stream IsNot Nothing Then
                Return  ConvertBitmapToImageSource(New Bitmap(stream) )
            Else
                Throw New Exception("Resource not found " & resourceName)
            End If
        End Using
    End Function
    
    Private Shared Function ResImage(ByVal path As String) As BitmapImage
        
        Dim location As String = System.IO.Path.GetDirectoryName(Reflection.Assembly.GetExecutingAssembly().Location)

        If path = "" Then
            Return New BitmapImage()
        End If

        Dim imagePath As String = location & "\" & path

        If Not System.IO.File.Exists(imagePath) Then
            Throw New Exception()
        End If

        Dim bmp As New BitmapImage(New Uri(imagePath, UriKind.Absolute))
        Return bmp
    End Function
    
    Private Shared Function ConvertBitmapToImageSource(bitmap As Bitmap) As ImageSource
        Using memoryStream As New MemoryStream()
            ' BitmapをMemoryStreamに保存
            bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png)
            memoryStream.Position = 0

            ' MemoryStreamをBitmapImageに読み込む
            Dim bitmapImage As New BitmapImage()
            bitmapImage.BeginInit()
            bitmapImage.StreamSource = memoryStream
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad
            bitmapImage.EndInit()

            Return bitmapImage
        End Using
    End Function
    
    Private Shared Function ConvertBitmapImageToBitmap(bitmapImage As BitmapImage) As Bitmap
        ' BitmapImageからストリームを作成
        Dim bitmap As Bitmap
        Using memoryStream As New IO.MemoryStream()
            Dim encoder As New PngBitmapEncoder()
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage))
            encoder.Save(memoryStream)
            memoryStream.Position = 0
            ' MemoryStreamからBitmapを作成
            bitmap = New Bitmap(memoryStream)
        End Using
        Return bitmap
    End Function
    
End Class
