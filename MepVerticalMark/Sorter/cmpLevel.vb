Imports Autodesk
Imports Autodesk.Revit
Imports System


Public Class CmpLevel
    Implements System.Collections.Generic.IComparer(Of DB.Level)

    'xがyより小さいときはマイナスの数、大きいときはプラスの数、同じときは0を返す
    Public Function Compare(ByVal x As Autodesk.Revit.DB.Level, ByVal y As Autodesk.Revit.DB.Level) As Integer Implements System.Collections.Generic.IComparer(Of Autodesk.Revit.DB.Level).Compare
        'Nothingが最も小さいとする
        If x Is Nothing AndAlso y Is Nothing Then
            Return 0
        End If
        If x Is Nothing Then
            Return -1
        End If
        If y Is Nothing Then
            Return 1
        End If

        Dim elvX As Double = x.Elevation
        Dim elvY As Double = y.Elevation

        If elvX < elvY Then
            Return -1
        ElseIf elvX > elvY Then
            Return 1
        Else
            Return 0

        End If
    End Function
End Class

