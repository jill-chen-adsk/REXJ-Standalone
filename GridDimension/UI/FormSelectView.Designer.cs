namespace ADSK.JExtRAC.GridDimension.UI
{
    partial class FormSelectView
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
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cklView = new System.Windows.Forms.CheckedListBox();
            this.cbkSelecAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(175, 322);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(256, 322);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cklView
            // 
            this.cklView.CheckOnClick = true;
            this.cklView.FormattingEnabled = true;
            this.cklView.Location = new System.Drawing.Point(12, 12);
            this.cklView.Name = "cklView";
            this.cklView.Size = new System.Drawing.Size(320, 289);
            this.cklView.TabIndex = 2;
            this.cklView.SelectedIndexChanged += new System.EventHandler(this.cklView_SelectedIndexChanged);
            // 
            // cbkSelecAll
            // 
            this.cbkSelecAll.AutoSize = true;
            this.cbkSelecAll.Location = new System.Drawing.Point(13, 308);
            this.cbkSelecAll.Name = "cbkSelecAll";
            this.cbkSelecAll.Size = new System.Drawing.Size(85, 17);
            this.cbkSelecAll.TabIndex = 3;
            this.cbkSelecAll.Text = "cbkSelectAll";
            this.cbkSelecAll.UseVisualStyleBackColor = true;
            this.cbkSelecAll.CheckedChanged += new System.EventHandler(this.cbkSelecAll_CheckedChanged);
            // 
            // FormSelectView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(344, 358);
            this.Controls.Add(this.cbkSelecAll);
            this.Controls.Add(this.cklView);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSelectView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormSelectView";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckedListBox cklView;
        private System.Windows.Forms.CheckBox cbkSelecAll;
    }
}