Imports Autodesk
Imports Autodesk.Revit
Imports Autodesk.Revit.DB.Mechanical

Public Class CmpDuctByCoordX
    Implements System.Collections.Generic.IComparer(Of DB.Mechanical.Duct)

    Public Function Compare(x As Duct, y As Duct) As Integer Implements IComparer(Of Duct).Compare

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

        Dim retval As Integer = 0

        Try
            Dim locX As DB.LocationCurve = x.Location
            Dim locY As DB.LocationCurve = y.Location
            Dim cvX As DB.Curve = locX.Curve
            Dim cvY As DB.Curve = locY.Curve
            Dim mpX As DB.XYZ = cvX.Evaluate(0.5, True)
            Dim mpY As DB.XYZ = cvY.Evaluate(0.5, True)

            If mpX.X < mpY.X Then
                retval = -1
            ElseIf mpX.X < mpY.X Then
                retval = 1
            Else
                retval = 0
            End If

        Catch ex As Exception
            retval = 0
        End Try

        Return retval

    End Function
End Class
