Imports System
Imports Autodesk
Imports Autodesk.Revit
Imports Autodesk.Revit.DB
Imports Autodesk.Revit.UI.Selection

Public Class SelFiltNominated
    Implements Autodesk.Revit.UI.Selection.ISelectionFilter

    Private m_NominatedId As List(Of ElementId)

    Public Sub New(nominatedId As List(Of ElementId))
        m_NominatedId = nominatedId
    End Sub

    Public Function AllowElement(elem As Element) As Boolean Implements ISelectionFilter.AllowElement

        Dim ret As Boolean = False
        Try
            ret = m_NominatedId.Contains(elem.Id)
        Catch ex As Exception
            ret = False
        End Try

        Return ret

    End Function

    Public Function AllowReference(reference As Reference, position As XYZ) As Boolean Implements ISelectionFilter.AllowReference
        Return True
    End Function
End Class
