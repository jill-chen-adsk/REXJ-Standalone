namespace ADSK.JExtRAC.PrintRegion.UI
{
    partial class PrintFrm
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
            this.lblPrintName = new System.Windows.Forms.Label();
            this.cbPrintName = new System.Windows.Forms.ComboBox();
            this.btPropetives = new System.Windows.Forms.Button();
            this.lblScale = new System.Windows.Forms.Label();
            this.cbScale = new System.Windows.Forms.ComboBox();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.errPvd = new System.Windows.Forms.ErrorProvider(this.components);
            this.btPreview = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).BeginInit();
            this.SuspendLayout();
            // 
            // lblPrintName
            // 
            this.lblPrintName.AutoSize = true;
            this.lblPrintName.Location = new System.Drawing.Point(16, 16);
            this.lblPrintName.Name = "lblPrintName";
            this.lblPrintName.Size = new System.Drawing.Size(66, 13);
            this.lblPrintName.TabIndex = 0;
            this.lblPrintName.Text = "lblPrintName";
            // 
            // cbPrintName
            // 
            this.cbPrintName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPrintName.FormattingEnabled = true;
            this.cbPrintName.Location = new System.Drawing.Point(24, 32);
            this.cbPrintName.Name = "cbPrintName";
            this.cbPrintName.Size = new System.Drawing.Size(264, 21);
            this.cbPrintName.Sorted = true;
            this.cbPrintName.TabIndex = 1;
            this.cbPrintName.SelectedIndexChanged += new System.EventHandler(this.cbPrintName_SelectedIndexChanged);
            // 
            // btPropetives
            // 
            this.btPropetives.Location = new System.Drawing.Point(336, 32);
            this.btPropetives.Name = "btPropetives";
            this.btPropetives.Size = new System.Drawing.Size(104, 23);
            this.btPropetives.TabIndex = 2;
            this.btPropetives.Text = "btPropetives";
            this.btPropetives.UseVisualStyleBackColor = true;
            this.btPropetives.Click += new System.EventHandler(this.btPropetives_Click);
            // 
            // lblScale
            // 
            this.lblScale.AutoSize = true;
            this.lblScale.Location = new System.Drawing.Point(16, 72);
            this.lblScale.Name = "lblScale";
            this.lblScale.Size = new System.Drawing.Size(44, 13);
            this.lblScale.TabIndex = 0;
            this.lblScale.Text = "lblScale";
            // 
            // cbScale
            // 
            this.cbScale.FormattingEnabled = true;
            this.cbScale.Location = new System.Drawing.Point(24, 88);
            this.cbScale.Name = "cbScale";
            this.cbScale.Size = new System.Drawing.Size(264, 21);
            this.cbScale.TabIndex = 1;
            this.cbScale.SelectedValueChanged += new System.EventHandler(this.cbScale_SelectedValueChanged);
            this.cbScale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbScale_KeyPress);
            // 
            // btOK
            // 
            this.btOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btOK.Location = new System.Drawing.Point(224, 136);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(104, 23);
            this.btOK.TabIndex = 2;
            this.btOK.Text = "btOK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.btOK_Click);
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(336, 136);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(104, 23);
            this.btCancel.TabIndex = 2;
            this.btCancel.Text = "btCancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
            // 
            // errPvd
            // 
            this.errPvd.ContainerControl = this;
            // 
            // btPreview
            // 
            this.btPreview.Location = new System.Drawing.Point(112, 136);
            this.btPreview.Name = "btPreview";
            this.btPreview.Size = new System.Drawing.Size(104, 23);
            this.btPreview.TabIndex = 2;
            this.btPreview.Text = "btPreview";
            this.btPreview.UseVisualStyleBackColor = true;
            this.btPreview.Click += new System.EventHandler(this.btPreview_Click);
            // 
            // PrintFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 177);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btPreview);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.btPropetives);
            this.Controls.Add(this.cbScale);
            this.Controls.Add(this.lblScale);
            this.Controls.Add(this.cbPrintName);
            this.Controls.Add(this.lblPrintName);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(470, 216);
            this.Name = "PrintFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PrintFrm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PrintFrm_FormClosed);
            this.Load += new System.EventHandler(this.PrintFrm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errPvd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblPrintName;
        private System.Windows.Forms.ComboBox cbPrintName;
        private System.Windows.Forms.Button btPropetives;
        private System.Windows.Forms.Label lblScale;
        private System.Windows.Forms.ComboBox cbScale;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.ErrorProvider errPvd;
        private System.Windows.Forms.Button btPreview;
    }
}