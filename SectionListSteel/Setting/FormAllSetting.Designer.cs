namespace SectionListSteel.Setting
{
    partial class FormAllSetting
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
            this.tabControl = new System.Windows.Forms.TabControl();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.btnEnd = new System.Windows.Forms.Button();
            this.btnOverWriteSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Location = new System.Drawing.Point(8, 9);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(967, 733);
            this.tabControl.TabIndex = 1;
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Location = new System.Drawing.Point(752, 751);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(120, 23);
            this.btnSaveAs.TabIndex = 12;
            this.btnSaveAs.Text = "btnSaveAs";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnEnd
            // 
            this.btnEnd.Location = new System.Drawing.Point(882, 751);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(90, 23);
            this.btnEnd.TabIndex = 13;
            this.btnEnd.Text = "btnEnd";
            this.btnEnd.UseVisualStyleBackColor = true;
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // btnOverWriteSave
            // 
            this.btnOverWriteSave.Location = new System.Drawing.Point(652, 751);
            this.btnOverWriteSave.Name = "btnOverWriteSave";
            this.btnOverWriteSave.Size = new System.Drawing.Size(90, 23);
            this.btnOverWriteSave.TabIndex = 11;
            this.btnOverWriteSave.Text = "btnOverWriteSave";
            this.btnOverWriteSave.UseVisualStyleBackColor = true;
            this.btnOverWriteSave.Click += new System.EventHandler(this.btnOverWriteSave_Click);
            // 
            // FormAllSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(981, 783);
            this.Controls.Add(this.btnSaveAs);
            this.Controls.Add(this.btnEnd);
            this.Controls.Add(this.btnOverWriteSave);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormAllSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FormAllSetting";
            this.Load += new System.EventHandler(this.FormAllSetting_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnEnd;
        private System.Windows.Forms.Button btnOverWriteSave;
    }
}