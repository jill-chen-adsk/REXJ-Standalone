Imports System.Windows.Forms

Imports System
Imports System.Drawing
Imports System.Collections
Imports System.ComponentModel
Imports Autodesk.Revit.DB

Public Class LevelSelectionForm 
    Inherits System.Windows.Forms.Form
    Private _comboBoxLevels As ComboBox
    Private WithEvents buttonOK As Button
    Private WithEvents thisLevelButton As Button
    Private WithEvents upLevelButton As Button
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)>
    Public Property SelectedLevel As String
    
    Public Sub New (thisLevel As Level, upLevel As Level)
        InitializeComponent()
'        _comboBoxLevels.Items.Add(thisLevel.Name)
'        _comboBoxLevels.Items.Add(upLevel.Name)

        thisLevelButton.Text = thisLevel.Name
        upLevelButton.Text = upLevel.Name

        
        
    End Sub
    
    Private Sub InitializeComponent()
'        _comboBoxLevels = New ComboBox()
        buttonOK = New Button()
        thisLevelButton = New Button()
        upLevelButton = New Button()
        SuspendLayout()
        
'        'comboBoxLevels
'        _comboBoxLevels.DropDownStyle = ComboBoxStyle.DropDownList
'        _comboBoxLevels.FormattingEnabled = True
'        _comboBoxLevels.Location = New Drawing.Point(12, 12)
'        _comboBoxLevels.Name = "_comboBoxLevels"
'        _comboBoxLevels.Size = New Size(200, 21)
'        _comboBoxLevels.TabIndex = 0
        
'        'buttonOK
'        buttonOK.DialogResult = DialogResult.OK
'        buttonOK.Location = New Drawing.Point(137, 39)
'        buttonOK.Name = "buttonOK"
'        buttonOK.Size = New Size(75, 23)
'        buttonOK.TabIndex = 1
'        buttonOK.Text = "OK"
'        buttonOK.UseVisualStyleBackColor = True
        
        'thisLevelButton
        thisLevelButton.DialogResult = DialogResult.Yes
        thisLevelButton.Location = New Drawing.Point(12, 12)
        thisLevelButton.Name = "thisLevelButton"
        thisLevelButton.Size = New Size(200, 23)
        thisLevelButton.TabIndex = 1
        thisLevelButton.UseVisualStyleBackColor = True

        'upLevelButton
        upLevelButton.DialogResult = DialogResult.No
        upLevelButton.Location = New Drawing.Point(12, 39)
        upLevelButton.Name = "upLevelButton"
        upLevelButton.Size = New Size(200, 23)
        upLevelButton.TabIndex = 1
        upLevelButton.UseVisualStyleBackColor = True
        
        'LevelSelectionForm
        AcceptButton = thisLevelButton
        CancelButton = upLevelButton
        AutoScaleDimensions = New SizeF(6.0!, 13.0!)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(224, 74)
'        Controls.Add(buttonOK)
'        Controls.Add(_comboBoxLevels)
        Controls.Add(thisLevelButton)
        Controls.Add(upLevelButton)
        FormBorderStyle = FormBorderStyle.FixedDialog
        MaximizeBox = False
        MinimizeBox = False
        Name = "LevelSelectionForm"
        ShowInTaskbar = False
        StartPosition = FormStartPosition.CenterParent
        Text = "Select Level"
        
        ResumeLayout(False)
    End Sub
    Private Sub buttonOK_Click(sender As Object, e As EventArgs) Handles buttonOK.Click
        SelectedLevel = _comboBoxLevels.SelectedItem
    End Sub
End Class