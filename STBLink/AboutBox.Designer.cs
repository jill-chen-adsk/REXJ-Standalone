namespace STBLink
{
    partial class AboutBox
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
            this.label2 = new System.Windows.Forms.Label() ;
            this.label3 = new System.Windows.Forms.Label() ;
            this.SuspendLayout() ;
            // 
            // label1
            // 
            this.label1.AutoSize = true ;
            this.label1.Location = new System.Drawing.Point( 100, 29 ) ;
            this.label1.Name = "label1" ;
            this.label1.Size = new System.Drawing.Size( 35, 12 ) ;
            this.label1.TabIndex = 0 ;
            this.label1.Text = "label1" ;
            // 
            // label2
            // 
            this.label2.AutoEllipsis = true ;
            this.label2.AutoSize = true ;
            this.label2.Location = new System.Drawing.Point( 20, 70 ) ;
            this.label2.Name = "label2" ;
            this.label2.Size = new System.Drawing.Size( 35, 12 ) ;
            this.label2.TabIndex = 1 ;
            this.label2.Text = "label2" ;
            // 
            // label3
            // 
            this.label3.AutoEllipsis = true ;
            this.label3.AutoSize = true ;
            this.label3.Location = new System.Drawing.Point( 22, 217 ) ;
            this.label3.Name = "label3" ;
            this.label3.Size = new System.Drawing.Size( 35, 12 ) ;
            this.label3.TabIndex = 2 ;
            this.label3.Text = "label3" ;
            // 
            // AboutBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F ) ;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font ;
            this.BackColor = System.Drawing.SystemColors.HighlightText ;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None ;
            this.ClientSize = new System.Drawing.Size( 552, 345 ) ;
            this.Controls.Add( this.label3 ) ;
            this.Controls.Add( this.label2 ) ;
            this.Controls.Add( this.label1 ) ;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog ;
            this.ImeMode = System.Windows.Forms.ImeMode.On ;
            this.KeyPreview = true ;
            this.MaximizeBox = false ;
            this.MinimizeBox = false ;
            this.Name = "AboutBox" ;
            this.ShowIcon = false ;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent ;
            this.Text = "ST-Bridge Link for Revit Structure" ;
            this.Load += new System.EventHandler( this.AboutBox_Load ) ;
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler( this.AboutBox_HelpRequested ) ;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler( this.AboutBox_KeyPress ) ;
            this.ResumeLayout( false ) ;
            this.PerformLayout() ;
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}