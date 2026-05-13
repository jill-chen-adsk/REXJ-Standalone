namespace ADSK.JExtRAC.EnhancedSectionBox.Screen
{
    partial class FormSectionBoxAdjustment
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label23 = new System.Windows.Forms.Label();
            this.textOffsetTop = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.textOffsetBottom = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.textOffsetBack = new System.Windows.Forms.TextBox();
            this.textOffsetForward = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textOffsetRight = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textOffsetLeft = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.confirmButton = new System.Windows.Forms.Button();
            this.canselButton = new System.Windows.Forms.Button();
            this.errorProviderApp = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderApp)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.textOffsetTop);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.textOffsetBottom);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.textOffsetBack);
            this.groupBox2.Controls.Add(this.textOffsetForward);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.textOffsetRight);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.textOffsetLeft);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Location = new System.Drawing.Point(12, 10);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox2.Size = new System.Drawing.Size(386, 93);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "長さ調節";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(354, 25);
            this.label23.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(23, 12);
            this.label23.TabIndex = 9;
            this.label23.Text = "mm";
            // 
            // textOffsetTop
            // 
            this.textOffsetTop.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textOffsetTop.Location = new System.Drawing.Point(282, 22);
            this.textOffsetTop.Margin = new System.Windows.Forms.Padding(2);
            this.textOffsetTop.Name = "textOffsetTop";
            this.textOffsetTop.Size = new System.Drawing.Size(68, 19);
            this.textOffsetTop.TabIndex = 13;
            this.textOffsetTop.Text = "0";
            this.textOffsetTop.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textOffsetTop.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validation);
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(262, 58);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(17, 12);
            this.label19.TabIndex = 8;
            this.label19.Text = "下";
            // 
            // textOffsetBottom
            // 
            this.textOffsetBottom.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textOffsetBottom.Location = new System.Drawing.Point(282, 55);
            this.textOffsetBottom.Margin = new System.Windows.Forms.Padding(2);
            this.textOffsetBottom.Name = "textOffsetBottom";
            this.textOffsetBottom.Size = new System.Drawing.Size(68, 19);
            this.textOffsetBottom.TabIndex = 15;
            this.textOffsetBottom.Text = "0";
            this.textOffsetBottom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textOffsetBottom.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validation);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(262, 25);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(17, 12);
            this.label20.TabIndex = 6;
            this.label20.Text = "上";
            // 
            // textOffsetBack
            // 
            this.textOffsetBack.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textOffsetBack.Location = new System.Drawing.Point(154, 55);
            this.textOffsetBack.Margin = new System.Windows.Forms.Padding(2);
            this.textOffsetBack.Name = "textOffsetBack";
            this.textOffsetBack.Size = new System.Drawing.Size(68, 19);
            this.textOffsetBack.TabIndex = 14;
            this.textOffsetBack.Text = "0";
            this.textOffsetBack.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textOffsetBack.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validation);
            // 
            // textOffsetForward
            // 
            this.textOffsetForward.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textOffsetForward.Location = new System.Drawing.Point(154, 22);
            this.textOffsetForward.Margin = new System.Windows.Forms.Padding(2);
            this.textOffsetForward.Name = "textOffsetForward";
            this.textOffsetForward.Size = new System.Drawing.Size(68, 19);
            this.textOffsetForward.TabIndex = 12;
            this.textOffsetForward.Text = "0";
            this.textOffsetForward.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textOffsetForward.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validation);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(133, 58);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(17, 12);
            this.label17.TabIndex = 4;
            this.label17.Text = "後";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(225, 58);
            this.label22.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(23, 12);
            this.label22.TabIndex = 7;
            this.label22.Text = "mm";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(225, 25);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(23, 12);
            this.label21.TabIndex = 9;
            this.label21.Text = "mm";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(133, 25);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(17, 12);
            this.label18.TabIndex = 3;
            this.label18.Text = "前";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(354, 58);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(23, 12);
            this.label12.TabIndex = 8;
            this.label12.Text = "mm";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(97, 25);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(23, 12);
            this.label13.TabIndex = 5;
            this.label13.Text = "mm";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(97, 58);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(23, 12);
            this.label14.TabIndex = 3;
            this.label14.Text = "mm";
            // 
            // textOffsetRight
            // 
            this.textOffsetRight.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textOffsetRight.Location = new System.Drawing.Point(26, 55);
            this.textOffsetRight.Margin = new System.Windows.Forms.Padding(2);
            this.textOffsetRight.Name = "textOffsetRight";
            this.textOffsetRight.Size = new System.Drawing.Size(68, 19);
            this.textOffsetRight.TabIndex = 2;
            this.textOffsetRight.Text = "0";
            this.textOffsetRight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textOffsetRight.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validation);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(4, 58);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(17, 12);
            this.label15.TabIndex = 2;
            this.label15.Text = "右";
            // 
            // textOffsetLeft
            // 
            this.textOffsetLeft.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.textOffsetLeft.Location = new System.Drawing.Point(26, 22);
            this.textOffsetLeft.Margin = new System.Windows.Forms.Padding(2);
            this.textOffsetLeft.Name = "textOffsetLeft";
            this.textOffsetLeft.Size = new System.Drawing.Size(68, 19);
            this.textOffsetLeft.TabIndex = 1;
            this.textOffsetLeft.Text = "0";
            this.textOffsetLeft.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textOffsetLeft.Validating += new System.ComponentModel.CancelEventHandler(this.Text_Validation);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(4, 25);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(17, 12);
            this.label16.TabIndex = 0;
            this.label16.Text = "左";
            // 
            // confirmButton
            // 
            this.confirmButton.Location = new System.Drawing.Point(226, 113);
            this.confirmButton.Margin = new System.Windows.Forms.Padding(2);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(82, 22);
            this.confirmButton.TabIndex = 3;
            this.confirmButton.Text = "OK";
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // canselButton
            // 
            this.canselButton.Location = new System.Drawing.Point(316, 113);
            this.canselButton.Margin = new System.Windows.Forms.Padding(2);
            this.canselButton.Name = "canselButton";
            this.canselButton.Size = new System.Drawing.Size(82, 22);
            this.canselButton.TabIndex = 4;
            this.canselButton.Text = "キャンセル";
            this.canselButton.UseVisualStyleBackColor = true;
            this.canselButton.Click += new System.EventHandler(this.CanselButton_Click);
            // 
            // errorProviderApp
            // 
            this.errorProviderApp.ContainerControl = this;
            // 
            // FormSectionBoxAdjustment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 146);
            this.Controls.Add(this.canselButton);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSectionBoxAdjustment";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "選択ボックス調整";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormClosing_Event);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderApp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textOffsetRight;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textOffsetLeft;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox textOffsetBottom;
        private System.Windows.Forms.TextBox textOffsetBack;
        private System.Windows.Forms.TextBox textOffsetForward;
        private System.Windows.Forms.TextBox textOffsetTop;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button canselButton;
        private System.Windows.Forms.ErrorProvider errorProviderApp;
    }
}