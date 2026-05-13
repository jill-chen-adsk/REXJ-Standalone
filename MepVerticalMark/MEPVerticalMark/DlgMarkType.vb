Imports System.ComponentModel
Imports System.Windows.Forms

Public Class DlgMarkType

    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property MarkType As Integer = -1

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub BtnVerticalMark0_Click(sender As Object, e As EventArgs) Handles BtnVerticalMark0.Click

        MarkType = 0
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()


    End Sub

    Private Sub BtnVerticalMark1_Click(sender As Object, e As EventArgs) Handles BtnVerticalMark1.Click

        MarkType = 1
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.Close()

    End Sub
End Class
