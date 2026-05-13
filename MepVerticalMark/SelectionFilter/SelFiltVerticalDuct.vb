Imports System
Imports Autodesk
Imports Autodesk.Revit


Public Class SelFiltVerticalDuct
    Implements Autodesk.Revit.UI.Selection.ISelectionFilter


    Public Function AllowElement(ByVal elem As Autodesk.Revit.DB.Element) As Boolean Implements Autodesk.Revit.UI.Selection.ISelectionFilter.AllowElement

        Dim dd As DB.Mechanical.Duct = TryCast(elem, DB.Mechanical.Duct)
        If IsNothing(dd) = True Then
            Return False
        End If

        Dim locPP As DB.Location = dd.Location
        If IsNothing(locPP) Then
            Return False
        End If

        Dim locCvPP As DB.LocationCurve = TryCast(locPP, DB.LocationCurve)
        If IsNothing(locCvPP) Then
            Return False
        End If

        Dim CvPP As DB.Curve = locCvPP.Curve
        '中点の方向
        Dim dirCv As DB.XYZ = CvPP.ComputeDerivatives(0.5, True).BasisX
        dirCv = dirCv.Normalize

        If dirCv.IsAlmostEqualTo(DB.XYZ.BasisZ, 0.1) Or dirCv.IsAlmostEqualTo(-DB.XYZ.BasisZ, 0.1) Then
            Return True
        Else
            Return False
        End If


    End Function

    Public Function AllowReference(ByVal reference As Autodesk.Revit.DB.Reference, ByVal position As Autodesk.Revit.DB.XYZ) As Boolean Implements Autodesk.Revit.UI.Selection.ISelectionFilter.AllowReference
        Return True
    End Function
End Class
