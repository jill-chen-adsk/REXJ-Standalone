namespace ADSK.JExtRAC.EnhancedSectionBox.Screen
{
    partial class FormBoxView
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.regionLabel = new System.Windows.Forms.Label();
            this.linkCountLabel = new System.Windows.Forms.Label();
            this.mainCountLabel = new System.Windows.Forms.Label();
            this.linkSelectionButton = new System.Windows.Forms.Button();
            this.levelListBox = new System.Windows.Forms.ListBox();
            this.rangeSpecificationButton = new System.Windows.Forms.Button();
            this.mainSelectionButton = new System.Windows.Forms.Button();
            this.floorBoxRadio = new System.Windows.Forms.RadioButton();
            this.rangeSpecificationRadio = new System.Windows.Forms.RadioButton();
            this.objectCheck = new System.Windows.Forms.CheckBox();
            this.selectObjectRadio = new System.Windows.Forms.RadioButton();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lowerMargin = new System.Windows.Forms.TextBox();
            this.topMargin = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lowerEndCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.topEdgeCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.hiddenFloorCheck = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.executionButton = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.nameCombo = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.hiddenBeamCheck = new System.Windows.Forms.CheckBox();
            this.errorProviderApp = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderApp)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.regionLabel);
            this.groupBox1.Controls.Add(this.linkCountLabel);
            this.groupBox1.Controls.Add(this.mainCountLabel);
            this.groupBox1.Controls.Add(this.linkSelectionButton);
            this.groupBox1.Controls.Add(this.levelListBox);
            this.groupBox1.Controls.Add(this.rangeSpecificationButton);
            this.groupBox1.Controls.Add(this.mainSelectionButton);
            this.groupBox1.Controls.Add(this.floorBoxRadio);
            this.groupBox1.Controls.Add(this.rangeSpecificationRadio);
            this.groupBox1.Controls.Add(this.objectCheck);
            this.groupBox1.Controls.Add(this.selectObjectRadio);
            this.groupBox1.Location = new System.Drawing.Point(13, 10);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(555, 147);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "XY平面の範囲";
            // 
            // regionLabel
            // 
            this.regionLabel.AutoSize = true;
            this.regionLabel.Location = new System.Drawing.Point(335, 52);
            this.regionLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.regionLabel.Name = "regionLabel";
            this.regionLabel.Size = new System.Drawing.Size(41, 12);
            this.regionLabel.TabIndex = 8;
            this.regionLabel.Text = "未指定";
            // 
            // linkCountLabel
            // 
            this.linkCountLabel.AutoSize = true;
            this.linkCountLabel.Location = new System.Drawing.Point(165, 80);
            this.linkCountLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.linkCountLabel.Name = "linkCountLabel";
            this.linkCountLabel.Size = new System.Drawing.Size(53, 12);
            this.linkCountLabel.TabIndex = 8;
            this.linkCountLabel.Text = "選択数：0";
            // 
            // mainCountLabel
            // 
            this.mainCountLabel.AutoSize = true;
            this.mainCountLabel.Location = new System.Drawing.Point(165, 52);
            this.mainCountLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.mainCountLabel.Name = "mainCountLabel";
            this.mainCountLabel.Size = new System.Drawing.Size(53, 12);
            this.mainCountLabel.TabIndex = 7;
            this.mainCountLabel.Text = "選択数：0";
            // 
            // linkSelectionButton
            // 
            this.linkSelectionButton.Location = new System.Drawing.Point(26, 74);
            this.linkSelectionButton.Margin = new System.Windows.Forms.Padding(2);
            this.linkSelectionButton.Name = "linkSelectionButton";
            this.linkSelectionButton.Size = new System.Drawing.Size(138, 24);
            this.linkSelectionButton.TabIndex = 4;
            this.linkSelectionButton.Text = "リンクプロジェクトから選択";
            this.linkSelectionButton.UseVisualStyleBackColor = true;
            // 
            // levelListBox
            // 
            this.levelListBox.FormattingEnabled = true;
            this.levelListBox.HorizontalScrollbar = true;
            this.levelListBox.ItemHeight = 12;
            this.levelListBox.Location = new System.Drawing.Point(418, 46);
            this.levelListBox.Margin = new System.Windows.Forms.Padding(2);
            this.levelListBox.Name = "levelListBox";
            this.levelListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.levelListBox.Size = new System.Drawing.Size(126, 88);
            this.levelListBox.TabIndex = 7;
            // 
            // rangeSpecificationButton
            // 
            this.rangeSpecificationButton.Location = new System.Drawing.Point(256, 46);
            this.rangeSpecificationButton.Margin = new System.Windows.Forms.Padding(2);
            this.rangeSpecificationButton.Name = "rangeSpecificationButton";
            this.rangeSpecificationButton.Size = new System.Drawing.Size(78, 24);
            this.rangeSpecificationButton.TabIndex = 6;
            this.rangeSpecificationButton.Text = "範囲指定";
            this.rangeSpecificationButton.UseVisualStyleBackColor = true;
            // 
            // mainSelectionButton
            // 
            this.mainSelectionButton.Location = new System.Drawing.Point(26, 46);
            this.mainSelectionButton.Margin = new System.Windows.Forms.Padding(2);
            this.mainSelectionButton.Name = "mainSelectionButton";
            this.mainSelectionButton.Size = new System.Drawing.Size(138, 24);
            this.mainSelectionButton.TabIndex = 3;
            this.mainSelectionButton.Text = "メインプロジェクトから選択";
            this.mainSelectionButton.UseVisualStyleBackColor = true;
            // 
            // floorBoxRadio
            // 
            this.floorBoxRadio.AutoSize = true;
            this.floorBoxRadio.Location = new System.Drawing.Point(400, 26);
            this.floorBoxRadio.Margin = new System.Windows.Forms.Padding(2);
            this.floorBoxRadio.Name = "floorBoxRadio";
            this.floorBoxRadio.Size = new System.Drawing.Size(76, 16);
            this.floorBoxRadio.TabIndex = 2;
            this.floorBoxRadio.TabStop = true;
            this.floorBoxRadio.Text = "レベル指定";
            this.floorBoxRadio.UseVisualStyleBackColor = true;
            // 
            // rangeSpecificationRadio
            // 
            this.rangeSpecificationRadio.AutoSize = true;
            this.rangeSpecificationRadio.Location = new System.Drawing.Point(240, 26);
            this.rangeSpecificationRadio.Margin = new System.Windows.Forms.Padding(2);
            this.rangeSpecificationRadio.Name = "rangeSpecificationRadio";
            this.rangeSpecificationRadio.Size = new System.Drawing.Size(71, 16);
            this.rangeSpecificationRadio.TabIndex = 1;
            this.rangeSpecificationRadio.TabStop = true;
            this.rangeSpecificationRadio.Text = "範囲指定";
            this.rangeSpecificationRadio.UseVisualStyleBackColor = true;
            // 
            // objectCheck
            // 
            this.objectCheck.AutoSize = true;
            this.objectCheck.Location = new System.Drawing.Point(26, 94);
            this.objectCheck.Margin = new System.Windows.Forms.Padding(2);
            this.objectCheck.Name = "objectCheck";
            this.objectCheck.Size = new System.Drawing.Size(125, 40);
            this.objectCheck.TabIndex = 5;
            this.objectCheck.Text = "\r\n選択ボックスの角度を\r\nオブジェクトに合わせる\r\n";
            this.objectCheck.UseVisualStyleBackColor = true;
            // 
            // selectObjectRadio
            // 
            this.selectObjectRadio.AutoSize = true;
            this.selectObjectRadio.Location = new System.Drawing.Point(11, 26);
            this.selectObjectRadio.Margin = new System.Windows.Forms.Padding(2);
            this.selectObjectRadio.Name = "selectObjectRadio";
            this.selectObjectRadio.Size = new System.Drawing.Size(98, 16);
            this.selectObjectRadio.TabIndex = 0;
            this.selectObjectRadio.TabStop = true;
            this.selectObjectRadio.Text = "オブジェクト指定";
            this.selectObjectRadio.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.lowerMargin);
            this.groupBox2.Controls.Add(this.topMargin);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.lowerEndCombo);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.topEdgeCombo);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(13, 166);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(555, 94);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "レベル指定/オフセット";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 90);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(0, 12);
            this.label10.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(426, 62);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 12);
            this.label6.TabIndex = 9;
            this.label6.Text = "mm";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(426, 26);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "mm";
            // 
            // lowerMargin
            // 
            this.lowerMargin.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.lowerMargin.Location = new System.Drawing.Point(305, 59);
            this.lowerMargin.Margin = new System.Windows.Forms.Padding(2);
            this.lowerMargin.Name = "lowerMargin";
            this.lowerMargin.Size = new System.Drawing.Size(120, 19);
            this.lowerMargin.TabIndex = 3;
            this.lowerMargin.Text = "0";
            this.lowerMargin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.lowerMargin.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validation);
            // 
            // topMargin
            // 
            this.topMargin.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.topMargin.Location = new System.Drawing.Point(305, 23);
            this.topMargin.Margin = new System.Windows.Forms.Padding(2);
            this.topMargin.Name = "topMargin";
            this.topMargin.Size = new System.Drawing.Size(120, 19);
            this.topMargin.TabIndex = 1;
            this.topMargin.Text = "0";
            this.topMargin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.topMargin.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validation);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(254, 62);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "オフセット";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(254, 26);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "オフセット";
            // 
            // lowerEndCombo
            // 
            this.lowerEndCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.lowerEndCombo.FormattingEnabled = true;
            this.lowerEndCombo.Location = new System.Drawing.Point(74, 59);
            this.lowerEndCombo.Margin = new System.Windows.Forms.Padding(2);
            this.lowerEndCombo.Name = "lowerEndCombo";
            this.lowerEndCombo.Size = new System.Drawing.Size(145, 20);
            this.lowerEndCombo.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "下端レベル";
            // 
            // topEdgeCombo
            // 
            this.topEdgeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.topEdgeCombo.FormattingEnabled = true;
            this.topEdgeCombo.Location = new System.Drawing.Point(74, 23);
            this.topEdgeCombo.Margin = new System.Windows.Forms.Padding(2);
            this.topEdgeCombo.Name = "topEdgeCombo";
            this.topEdgeCombo.Size = new System.Drawing.Size(145, 20);
            this.topEdgeCombo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "上端レベル";
            // 
            // hiddenFloorCheck
            // 
            this.hiddenFloorCheck.AutoSize = true;
            this.hiddenFloorCheck.Location = new System.Drawing.Point(11, 26);
            this.hiddenFloorCheck.Margin = new System.Windows.Forms.Padding(2);
            this.hiddenFloorCheck.Name = "hiddenFloorCheck";
            this.hiddenFloorCheck.Size = new System.Drawing.Size(36, 16);
            this.hiddenFloorCheck.TabIndex = 0;
            this.hiddenFloorCheck.Text = "床";
            this.hiddenFloorCheck.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(9, 276);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(0, 12);
            this.label7.TabIndex = 2;
            // 
            // executionButton
            // 
            this.executionButton.Location = new System.Drawing.Point(396, 363);
            this.executionButton.Margin = new System.Windows.Forms.Padding(2);
            this.executionButton.Name = "executionButton";
            this.executionButton.Size = new System.Drawing.Size(82, 22);
            this.executionButton.TabIndex = 4;
            this.executionButton.Text = "OK";
            this.executionButton.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(486, 363);
            this.button4.Margin = new System.Windows.Forms.Padding(2);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(82, 22);
            this.button4.TabIndex = 5;
            this.button4.Text = "キャンセル";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(19, 342);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 2;
            this.label9.Text = "ビューの名前";
            // 
            // nameCombo
            // 
            this.nameCombo.FormattingEnabled = true;
            this.nameCombo.Items.AddRange(new object[] {
            "{3D}"});
            this.nameCombo.Location = new System.Drawing.Point(87, 338);
            this.nameCombo.Margin = new System.Windows.Forms.Padding(2);
            this.nameCombo.Name = "nameCombo";
            this.nameCombo.Size = new System.Drawing.Size(145, 20);
            this.nameCombo.TabIndex = 3;
            this.nameCombo.TextChanged += new System.EventHandler(this.ViewName_Changed);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.hiddenBeamCheck);
            this.groupBox3.Controls.Add(this.hiddenFloorCheck);
            this.groupBox3.Location = new System.Drawing.Point(13, 270);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox3.Size = new System.Drawing.Size(555, 55);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "上端レベルに属するオブジェクトを非表示にする　※リンクプロジェクト内のオブジェクトは対象外";
            // 
            // hiddenBeamCheck
            // 
            this.hiddenBeamCheck.AutoSize = true;
            this.hiddenBeamCheck.Location = new System.Drawing.Point(256, 26);
            this.hiddenBeamCheck.Margin = new System.Windows.Forms.Padding(2);
            this.hiddenBeamCheck.Name = "hiddenBeamCheck";
            this.hiddenBeamCheck.Size = new System.Drawing.Size(36, 16);
            this.hiddenBeamCheck.TabIndex = 1;
            this.hiddenBeamCheck.Text = "梁";
            this.hiddenBeamCheck.UseVisualStyleBackColor = true;
            // 
            // errorProviderApp
            // 
            this.errorProviderApp.ContainerControl = this;
            // 
            // FormBoxView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(581, 395);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.nameCombo);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.executionButton);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormBoxView";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "選択ボックス作成";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderApp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox levelListBox;
        private System.Windows.Forms.Button rangeSpecificationButton;
        private System.Windows.Forms.Button mainSelectionButton;
        private System.Windows.Forms.RadioButton floorBoxRadio;
        private System.Windows.Forms.RadioButton rangeSpecificationRadio;
        private System.Windows.Forms.RadioButton selectObjectRadio;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox objectCheck;
        private System.Windows.Forms.Button executionButton;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.Label regionLabel;
        public System.Windows.Forms.Label linkCountLabel;
        public System.Windows.Forms.Label mainCountLabel;
        private System.Windows.Forms.Button linkSelectionButton;
        public System.Windows.Forms.CheckBox hiddenFloorCheck;
        public System.Windows.Forms.CheckBox hiddenBeamCheck;
        public System.Windows.Forms.ComboBox nameCombo;
        public System.Windows.Forms.TextBox lowerMargin;
        public System.Windows.Forms.TextBox topMargin;
        public System.Windows.Forms.ComboBox lowerEndCombo;
        public System.Windows.Forms.ComboBox topEdgeCombo;
        private System.Windows.Forms.ErrorProvider errorProviderApp;
    }
}