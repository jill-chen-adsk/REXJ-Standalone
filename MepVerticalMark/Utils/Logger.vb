Imports System.IO
Imports System.Reflection

' アドインDLL位置にログ出力するクラス
' Log("文字列")のような形で呼び出す。

Public Class Logger
    Private Shared ReadOnly _logFilePath As String
    Shared Sub New()
        Dim assemblyLocation As String = Assembly.GetExecutingAssembly().Location
        Dim directoryPath As String = Path.GetDirectoryName(assemblyLocation)
        _logFilePath = Path.Combine(directoryPath, "Log.txt")
        'Log("Start ------------------")
    End Sub
    
    Public Shared Sub Log(message As String)
        Dim logMessage As String = String.Format("{0}: {1}{2}", DateTime.Now, message, Environment.NewLine)
        File.AppendAllText(_logFilePath, logMessage)
    End Sub
    
End Class