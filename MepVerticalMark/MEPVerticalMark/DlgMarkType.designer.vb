<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DlgMarkType
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.BtnVerticalMark0 = New System.Windows.Forms.Button()
        Me.BtnVerticalMark1 = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'BtnVerticalMark0
        '
        Me.BtnVerticalMark0.Image = ADSK.MepVerticalMark.My.Resources.Resources.VerticalMark0
        Me.BtnVerticalMark0.Location = New System.Drawing.Point(12, 12)
        Me.BtnVerticalMark0.Name = "BtnVerticalMark0"
        Me.BtnVerticalMark0.Size = New System.Drawing.Size(111, 110)
        Me.BtnVerticalMark0.TabIndex = 0
        Me.BtnVerticalMark0.UseVisualStyleBackColor = True
        '
        'BtnVerticalMark1
        '
        Me.BtnVerticalMark1.Image = ADSK.MepVerticalMark.My.Resources.Resources.VerticalMark1
        Me.BtnVerticalMark1.Location = New System.Drawing.Point(129, 12)
        Me.BtnVerticalMark1.Name = "BtnVerticalMark1"
        Me.BtnVerticalMark1.Size = New System.Drawing.Size(111, 110)
        Me.BtnVerticalMark1.TabIndex = 1
        Me.BtnVerticalMark1.UseVisualStyleBackColor = True
        '
        'DlgMarkType
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(252, 134)
        Me.Controls.Add(Me.BtnVerticalMark1)
        Me.Controls.Add(Me.BtnVerticalMark0)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "DlgMarkType"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "DlgMarkType"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents BtnVerticalMark0 As System.Windows.Forms.Button
    Friend WithEvents BtnVerticalMark1 As System.Windows.Forms.Button
End Class
