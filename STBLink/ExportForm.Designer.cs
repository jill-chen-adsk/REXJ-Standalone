namespace STBLink
{
    partial class ExportForm
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
            this.label1 = new System.Windows.Forms.Label() ;
            this.groupBox1 = new System.Windows.Forms.GroupBox() ;
            this.radioButton3 = new System.Windows.Forms.RadioButton() ;
            this.radioButton2 = new System.Windows.Forms.RadioButton() ;
            this.button1 = new System.Windows.Forms.Button() ;
            this.button2 = new System.Windows.Forms.Button() ;
            this.groupBox1.SuspendLayout() ;
            this.SuspendLayout() ;
            // 
            // label1
            // 
            this.label1.AutoSize = true ;
            this.label1.Location = new System.Drawing.Point( 6, 15 ) ;
            this.label1.Name = "label1" ;
            this.label1.Size = new System.Drawing.Size( 291, 24 ) ;
            this.label1.TabIndex = 0 ;
            this.label1.Text = "The model contains members whose pile length was not set during import.\r\nChoose how to write pile length to the exported file." ;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left ) | System.Windows.Forms.AnchorStyles.Right ) ) ) ;
            this.groupBox1.Controls.Add( this.radioButton3 ) ;
            this.groupBox1.Controls.Add( this.radioButton2 ) ;
            this.groupBox1.Controls.Add( this.label1 ) ;
            this.groupBox1.Location = new System.Drawing.Point( 12, 12 ) ;
            this.groupBox1.Name = "groupBox1" ;
            this.groupBox1.Size = new System.Drawing.Size( 365, 115 ) ;
            this.groupBox1.TabIndex = 0 ;
            this.groupBox1.TabStop = false ;
            this.groupBox1.Text = "Pile length" ;
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true ;
            this.radioButton3.Location = new System.Drawing.Point( 8, 80 ) ;
            this.radioButton3.Name = "radioButton3" ;
            this.radioButton3.Size = new System.Drawing.Size( 109, 16 ) ;
            this.radioButton3.TabIndex = 3 ;
            this.radioButton3.Text = "Do not export pile length" ;
            this.radioButton3.UseVisualStyleBackColor = true ;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true ;
            this.radioButton2.Checked = true ;
            this.radioButton2.Location = new System.Drawing.Point( 8, 58 ) ;
            this.radioButton2.Name = "radioButton2" ;
            this.radioButton2.Size = new System.Drawing.Size( 152, 16 ) ;
            this.radioButton2.TabIndex = 2 ;
            this.radioButton2.TabStop = true ;
            this.radioButton2.Text = "Export using entered pile length" ;
            this.radioButton2.UseVisualStyleBackColor = true ;
            // 
            // button1
            // 
            this.button1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) ) ;
            this.button1.Location = new System.Drawing.Point( 221, 142 ) ;
            this.button1.Name = "button1" ;
            this.button1.Size = new System.Drawing.Size( 75, 23 ) ;
            this.button1.TabIndex = 1 ;
            this.button1.Text = "OK" ;
            this.button1.UseVisualStyleBackColor = true ;
            this.button1.Click += new System.EventHandler( this.Button1_Click ) ;
            // 
            // button2
            // 
            this.button2.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) ) ;
            this.button2.Location = new System.Drawing.Point( 302, 142 ) ;
            this.button2.Name = "button2" ;
            this.button2.Size = new System.Drawing.Size( 75, 23 ) ;
            this.button2.TabIndex = 2 ;
            this.button2.Text = "Cancel" ;
            this.button2.UseVisualStyleBackColor = true ;
            this.button2.Click += new System.EventHandler( this.Button2_Click ) ;
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F ) ;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font ;
            this.ClientSize = new System.Drawing.Size( 389, 177 ) ;
            this.Controls.Add( this.button2 ) ;
            this.Controls.Add( this.button1 ) ;
            this.Controls.Add( this.groupBox1 ) ;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle ;
            this.MaximizeBox = false ;
            this.MinimizeBox = false ;
            this.Name = "ExportForm" ;
            this.ShowIcon = false ;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent ;
            this.Text = "ExportForm" ;
            this.Load += new System.EventHandler( this.ExportForm_Load ) ;
            this.groupBox1.ResumeLayout( false ) ;
            this.groupBox1.PerformLayout() ;
            this.ResumeLayout( false ) ;
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}