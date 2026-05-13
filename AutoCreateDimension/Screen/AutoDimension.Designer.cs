namespace ADSK.JExtRAC.AutoCreateDimension.Screen
{
    partial class AutoDimension
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
            if (disposing && (components != null)) {
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.faceCoreRadio = new System.Windows.Forms.RadioButton();
            this.faceRadio = new System.Windows.Forms.RadioButton();
            this.coreRadio = new System.Windows.Forms.RadioButton();
            this.dimensionButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.lineCheck = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.faceCoreRadio);
            this.groupBox1.Controls.Add(this.faceRadio);
            this.groupBox1.Controls.Add(this.coreRadio);
            this.groupBox1.Location = new System.Drawing.Point(12, 11);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox1.Size = new System.Drawing.Size(149, 101);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "配置タイプ";
            // 
            // faceCoreRadio
            // 
            this.faceCoreRadio.AutoSize = true;
            this.faceCoreRadio.Location = new System.Drawing.Point(9, 77);
            this.faceCoreRadio.Margin = new System.Windows.Forms.Padding(2);
            this.faceCoreRadio.Name = "faceCoreRadio";
            this.faceCoreRadio.Size = new System.Drawing.Size(61, 16);
            this.faceCoreRadio.TabIndex = 2;
            this.faceCoreRadio.Text = "面 - 芯";
            this.faceCoreRadio.UseVisualStyleBackColor = true;
            // 
            // faceRadio
            // 
            this.faceRadio.AutoSize = true;
            this.faceRadio.Location = new System.Drawing.Point(9, 49);
            this.faceRadio.Margin = new System.Windows.Forms.Padding(2);
            this.faceRadio.Name = "faceRadio";
            this.faceRadio.Size = new System.Drawing.Size(61, 16);
            this.faceRadio.TabIndex = 1;
            this.faceRadio.Text = "面 - 面";
            this.faceRadio.UseVisualStyleBackColor = true;
            // 
            // coreRadio
            // 
            this.coreRadio.AutoSize = true;
            this.coreRadio.Checked = true;
            this.coreRadio.Location = new System.Drawing.Point(9, 22);
            this.coreRadio.Margin = new System.Windows.Forms.Padding(2);
            this.coreRadio.Name = "coreRadio";
            this.coreRadio.Size = new System.Drawing.Size(61, 16);
            this.coreRadio.TabIndex = 0;
            this.coreRadio.TabStop = true;
            this.coreRadio.Text = "芯 - 芯";
            this.coreRadio.UseVisualStyleBackColor = true;
            // 
            // dimensionButton
            // 
            this.dimensionButton.Location = new System.Drawing.Point(12, 154);
            this.dimensionButton.Margin = new System.Windows.Forms.Padding(2);
            this.dimensionButton.Name = "dimensionButton";
            this.dimensionButton.Size = new System.Drawing.Size(70, 22);
            this.dimensionButton.TabIndex = 2;
            this.dimensionButton.Text = "OK";
            this.dimensionButton.UseVisualStyleBackColor = true;
            this.dimensionButton.Click += new System.EventHandler(this.DimensionButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(91, 154);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(70, 22);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "キャンセル";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // lineCheck
            // 
            this.lineCheck.AutoSize = true;
            this.lineCheck.Location = new System.Drawing.Point(12, 109);
            this.lineCheck.Margin = new System.Windows.Forms.Padding(2);
            this.lineCheck.Name = "lineCheck";
            this.lineCheck.Size = new System.Drawing.Size(153, 40);
            this.lineCheck.TabIndex = 1;
            this.lineCheck.Text = "\r\n参照が取得できない場合、\r\n詳細線分を作成";
            this.lineCheck.UseVisualStyleBackColor = true;
            // 
            // AutoDimension
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(173, 186);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.dimensionButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lineCheck);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoDimension";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "オブジェクト一括寸法";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton coreRadio;
        private System.Windows.Forms.RadioButton faceRadio;
        private System.Windows.Forms.Button dimensionButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RadioButton faceCoreRadio;
        private System.Windows.Forms.CheckBox lineCheck;
    }
}