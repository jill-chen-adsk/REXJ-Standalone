namespace ADSK.JExtRAC.AutoCreateRoomView.Screen
{
    partial class FormAutoCreateRoomView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.floorRadio = new System.Windows.Forms.RadioButton();
            this.ceilingRadio = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.viewTypeButton = new System.Windows.Forms.Button();
            this.viewTypeCombo = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tagCombo = new System.Windows.Forms.ComboBox();
            this.tagTypeButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.offsetText = new System.Windows.Forms.TextBox();
            this.shapeCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.levelListView = new System.Windows.Forms.ListView();
            this.roomListView = new System.Windows.Forms.ListView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.copyRadio = new System.Windows.Forms.RadioButton();
            this.recreateRadio = new System.Windows.Forms.RadioButton();
            this.skipRadio = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.templateCombo = new System.Windows.Forms.ComboBox();
            this.templateButton = new System.Windows.Forms.Button();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 13);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please select the view category to create.";
            // 
            // floorRadio
            // 
            this.floorRadio.AutoSize = true;
            this.floorRadio.Checked = true;
            this.floorRadio.Location = new System.Drawing.Point(14, 33);
            this.floorRadio.Margin = new System.Windows.Forms.Padding(2);
            this.floorRadio.Name = "floorRadio";
            this.floorRadio.Size = new System.Drawing.Size(83, 16);
            this.floorRadio.TabIndex = 1;
            this.floorRadio.TabStop = true;
            this.floorRadio.Text = "Floor plan (per room)";
            this.floorRadio.UseVisualStyleBackColor = true;
            this.floorRadio.CheckedChanged += new System.EventHandler(this.ViewRadio_Change);
            // 
            // ceilingRadio
            // 
            this.ceilingRadio.AutoSize = true;
            this.ceilingRadio.Location = new System.Drawing.Point(14, 54);
            this.ceilingRadio.Margin = new System.Windows.Forms.Padding(2);
            this.ceilingRadio.Name = "ceilingRadio";
            this.ceilingRadio.Size = new System.Drawing.Size(83, 16);
            this.ceilingRadio.TabIndex = 2;
            this.ceilingRadio.Text = "Ceiling plan (per room)";
            this.ceilingRadio.UseVisualStyleBackColor = true;
            this.ceilingRadio.CheckedChanged += new System.EventHandler(this.ViewRadio_Change);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.viewTypeButton);
            this.groupBox3.Controls.Add(this.viewTypeCombo);
            this.groupBox3.Location = new System.Drawing.Point(12, 83);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(325, 56);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "View Type";
            // 
            // viewTypeButton
            // 
            this.viewTypeButton.Location = new System.Drawing.Point(215, 23);
            this.viewTypeButton.Margin = new System.Windows.Forms.Padding(2);
            this.viewTypeButton.Name = "viewTypeButton";
            this.viewTypeButton.Size = new System.Drawing.Size(95, 22);
            this.viewTypeButton.TabIndex = 1;
            this.viewTypeButton.Text = "Type Edit";
            this.viewTypeButton.UseVisualStyleBackColor = true;
            this.viewTypeButton.Click += new System.EventHandler(this.TypeEditButton_Click);
            // 
            // viewTypeCombo
            // 
            this.viewTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.viewTypeCombo.FormattingEnabled = true;
            this.viewTypeCombo.Location = new System.Drawing.Point(14, 24);
            this.viewTypeCombo.Margin = new System.Windows.Forms.Padding(2);
            this.viewTypeCombo.Name = "viewTypeCombo";
            this.viewTypeCombo.Size = new System.Drawing.Size(190, 20);
            this.viewTypeCombo.TabIndex = 0;
            this.viewTypeCombo.SelectedValueChanged += new System.EventHandler(this.ViewTypeCombo_Change);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tagCombo);
            this.groupBox1.Controls.Add(this.tagTypeButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 213);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(325, 56);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tag";
            // 
            // tagCombo
            // 
            this.tagCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagCombo.FormattingEnabled = true;
            this.tagCombo.Location = new System.Drawing.Point(14, 24);
            this.tagCombo.Margin = new System.Windows.Forms.Padding(2);
            this.tagCombo.Name = "tagCombo";
            this.tagCombo.Size = new System.Drawing.Size(190, 20);
            this.tagCombo.TabIndex = 0;
            this.tagCombo.TabStop = false;
            // 
            // tagTypeButton
            // 
            this.tagTypeButton.Location = new System.Drawing.Point(215, 23);
            this.tagTypeButton.Margin = new System.Windows.Forms.Padding(2);
            this.tagTypeButton.Name = "tagTypeButton";
            this.tagTypeButton.Size = new System.Drawing.Size(95, 22);
            this.tagTypeButton.TabIndex = 1;
            this.tagTypeButton.Text = "Type Edit";
            this.tagTypeButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(255, 539);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(82, 22);
            this.cancelButton.TabIndex = 12;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(150, 26);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "Offset (mm)";
            // 
            // offsetText
            // 
            this.offsetText.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.offsetText.Location = new System.Drawing.Point(223, 24);
            this.offsetText.Margin = new System.Windows.Forms.Padding(2);
            this.offsetText.Name = "offsetText";
            this.offsetText.ShortcutsEnabled = false;
            this.offsetText.Size = new System.Drawing.Size(87, 19);
            this.offsetText.TabIndex = 1;
            this.offsetText.Text = "0";
            this.offsetText.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // shapeCombo
            // 
            this.shapeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.shapeCombo.FormattingEnabled = true;
            this.shapeCombo.Items.AddRange(new object[] {
            "Rectangle",
            "Room boundary"});
            this.shapeCombo.Location = new System.Drawing.Point(44, 24);
            this.shapeCombo.Margin = new System.Windows.Forms.Padding(2);
            this.shapeCombo.Name = "shapeCombo";
            this.shapeCombo.Size = new System.Drawing.Size(94, 20);
            this.shapeCombo.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 26);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "Shape";
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(165, 539);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(82, 22);
            this.okButton.TabIndex = 11;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // applyButton
            // 
            this.applyButton.Location = new System.Drawing.Point(75, 539);
            this.applyButton.Margin = new System.Windows.Forms.Padding(2);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(82, 22);
            this.applyButton.TabIndex = 10;
            this.applyButton.Text = "Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.ApplicationButton_Click);
            // 
            // levelListView
            // 
            this.levelListView.Alignment = System.Windows.Forms.ListViewAlignment.Default;
            this.levelListView.HideSelection = false;
            this.levelListView.Location = new System.Drawing.Point(12, 348);
            this.levelListView.Margin = new System.Windows.Forms.Padding(2);
            this.levelListView.Name = "levelListView";
            this.levelListView.Size = new System.Drawing.Size(155, 105);
            this.levelListView.TabIndex = 7;
            this.levelListView.UseCompatibleStateImageBehavior = false;
            this.levelListView.View = System.Windows.Forms.View.Details;
            this.levelListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.LevelListItemsChecked_Change);
            // 
            // roomListView
            // 
            this.roomListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.roomListView.HideSelection = false;
            this.roomListView.Location = new System.Drawing.Point(182, 348);
            this.roomListView.Margin = new System.Windows.Forms.Padding(2);
            this.roomListView.Name = "roomListView";
            this.roomListView.Size = new System.Drawing.Size(155, 105);
            this.roomListView.TabIndex = 8;
            this.roomListView.UseCompatibleStateImageBehavior = false;
            this.roomListView.View = System.Windows.Forms.View.Details;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.copyRadio);
            this.groupBox4.Controls.Add(this.recreateRadio);
            this.groupBox4.Controls.Add(this.skipRadio);
            this.groupBox4.Location = new System.Drawing.Point(12, 468);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox4.Size = new System.Drawing.Size(325, 56);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "When a view with the same name exists";
            // 
            // copyRadio
            // 
            this.copyRadio.AutoSize = true;
            this.copyRadio.Location = new System.Drawing.Point(223, 25);
            this.copyRadio.Margin = new System.Windows.Forms.Padding(2);
            this.copyRadio.Name = "copyRadio";
            this.copyRadio.Size = new System.Drawing.Size(74, 16);
            this.copyRadio.TabIndex = 2;
            this.copyRadio.Text = "Copy";
            this.copyRadio.UseVisualStyleBackColor = true;
            // 
            // recreateRadio
            // 
            this.recreateRadio.AutoSize = true;
            this.recreateRadio.Location = new System.Drawing.Point(119, 25);
            this.recreateRadio.Margin = new System.Windows.Forms.Padding(2);
            this.recreateRadio.Name = "recreateRadio";
            this.recreateRadio.Size = new System.Drawing.Size(56, 16);
            this.recreateRadio.TabIndex = 0;
            this.recreateRadio.Text = "Overwrite";
            this.recreateRadio.UseVisualStyleBackColor = true;
            // 
            // skipRadio
            // 
            this.skipRadio.AutoSize = true;
            this.skipRadio.Checked = true;
            this.skipRadio.Location = new System.Drawing.Point(14, 25);
            this.skipRadio.Margin = new System.Windows.Forms.Padding(2);
            this.skipRadio.Name = "skipRadio";
            this.skipRadio.Size = new System.Drawing.Size(58, 16);
            this.skipRadio.TabIndex = 1;
            this.skipRadio.TabStop = true;
            this.skipRadio.Text = "Skip";
            this.skipRadio.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.offsetText);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.shapeCombo);
            this.groupBox2.Location = new System.Drawing.Point(12, 278);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(325, 56);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Trimming";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.templateCombo);
            this.groupBox5.Controls.Add(this.templateButton);
            this.groupBox5.Location = new System.Drawing.Point(12, 148);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox5.Size = new System.Drawing.Size(325, 56);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "View Template";
            // 
            // templateCombo
            // 
            this.templateCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.templateCombo.FormattingEnabled = true;
            this.templateCombo.Location = new System.Drawing.Point(14, 24);
            this.templateCombo.Margin = new System.Windows.Forms.Padding(2);
            this.templateCombo.Name = "templateCombo";
            this.templateCombo.Size = new System.Drawing.Size(190, 20);
            this.templateCombo.TabIndex = 0;
            this.templateCombo.TabStop = false;
            // 
            // templateButton
            // 
            this.templateButton.Location = new System.Drawing.Point(215, 23);
            this.templateButton.Margin = new System.Windows.Forms.Padding(2);
            this.templateButton.Name = "templateButton";
            this.templateButton.Size = new System.Drawing.Size(95, 22);
            this.templateButton.TabIndex = 1;
            this.templateButton.Text = "Manage Templates";
            this.templateButton.UseVisualStyleBackColor = true;
            // 
            // FormAutoCreateRoomView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 576);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.roomListView);
            this.Controls.Add(this.levelListView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ceilingRadio);
            this.Controls.Add(this.floorRadio);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAutoCreateRoomView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Room View Creation";
            this.groupBox3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton ceilingRadio;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button viewTypeButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button tagTypeButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.ListView levelListView;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button templateButton;
        public System.Windows.Forms.ComboBox templateCombo;
        public System.Windows.Forms.RadioButton floorRadio;
        public System.Windows.Forms.ComboBox viewTypeCombo;
        public System.Windows.Forms.ComboBox tagCombo;
        public System.Windows.Forms.TextBox offsetText;
        public System.Windows.Forms.ComboBox shapeCombo;
        public System.Windows.Forms.ListView roomListView;
        public System.Windows.Forms.RadioButton copyRadio;
        public System.Windows.Forms.RadioButton recreateRadio;
        public System.Windows.Forms.RadioButton skipRadio;
    }
}
