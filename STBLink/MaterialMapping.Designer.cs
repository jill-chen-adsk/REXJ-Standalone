namespace STBLink
{
    partial class MaterialMapping
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
            this.OK = new System.Windows.Forms.Button() ;
            this.Cancel = new System.Windows.Forms.Button() ;
            this.groupBox1 = new System.Windows.Forms.GroupBox() ;
            this.label3 = new System.Windows.Forms.Label() ;
            this.textBox2 = new System.Windows.Forms.TextBox() ;
            this.textBox1 = new System.Windows.Forms.TextBox() ;
            this.label2 = new System.Windows.Forms.Label() ;
            this.label1 = new System.Windows.Forms.Label() ;
            this.groupBox2 = new System.Windows.Forms.GroupBox() ;
            this.DGV = new System.Windows.Forms.DataGridView() ;
            this.matename = new System.Windows.Forms.DataGridViewTextBoxColumn() ;
            this.Combbox = new System.Windows.Forms.DataGridViewComboBoxColumn() ;
            this.linkLabel1 = new System.Windows.Forms.LinkLabel() ;
            this.linkLabel2 = new System.Windows.Forms.LinkLabel() ;
            this.Back = new System.Windows.Forms.Button() ;
            this.groupBox1.SuspendLayout() ;
            this.groupBox2.SuspendLayout() ;
            ( (System.ComponentModel.ISupportInitialize)( this.DGV ) ).BeginInit() ;
            this.SuspendLayout() ;
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point( 411, 378 ) ;
            this.OK.Name = "OK" ;
            this.OK.Size = new System.Drawing.Size( 75, 23 ) ;
            this.OK.TabIndex = 5 ;
            this.OK.Text = "Next" ;
            this.OK.UseVisualStyleBackColor = true ;
            this.OK.Click += new System.EventHandler( this.OK_Click ) ;
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel ;
            this.Cancel.Location = new System.Drawing.Point( 492, 378 ) ;
            this.Cancel.Name = "Cancel" ;
            this.Cancel.Size = new System.Drawing.Size( 75, 23 ) ;
            this.Cancel.TabIndex = 6 ;
            this.Cancel.Text = "Cancel" ;
            this.Cancel.UseVisualStyleBackColor = true ;
            this.Cancel.Click += new System.EventHandler( this.Cancel_Click ) ;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add( this.label3 ) ;
            this.groupBox1.Controls.Add( this.textBox2 ) ;
            this.groupBox1.Controls.Add( this.textBox1 ) ;
            this.groupBox1.Controls.Add( this.label2 ) ;
            this.groupBox1.Controls.Add( this.label1 ) ;
            this.groupBox1.Location = new System.Drawing.Point( 12, 12 ) ;
            this.groupBox1.Name = "groupBox1" ;
            this.groupBox1.Size = new System.Drawing.Size( 253, 140 ) ;
            this.groupBox1.TabIndex = 0 ;
            this.groupBox1.TabStop = false ;
            this.groupBox1.Text = "Concrete" ;
            // 
            // label3
            // 
            this.label3.AutoSize = true ;
            this.label3.Location = new System.Drawing.Point( 6, 94 ) ;
            this.label3.Name = "label3" ;
            this.label3.Size = new System.Drawing.Size( 204, 24 ) ;
            this.label3.TabIndex = 4 ;
            this.label3.Text = "## is replaced by the concrete strength.\r\n    Example: Concrete - CONCR Fc24" ;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point( 71, 58 ) ;
            this.textBox2.Name = "textBox2" ;
            this.textBox2.Size = new System.Drawing.Size( 176, 19 ) ;
            this.textBox2.TabIndex = 3 ;
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler( this.textBox1_KeyPress ) ;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point( 71, 21 ) ;
            this.textBox1.Name = "textBox1" ;
            this.textBox1.Size = new System.Drawing.Size( 176, 19 ) ;
            this.textBox1.TabIndex = 1 ;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler( this.textBox1_KeyPress ) ;
            // 
            // label2
            // 
            this.label2.AutoSize = true ;
            this.label2.Location = new System.Drawing.Point( 6, 65 ) ;
            this.label2.Name = "label2" ;
            this.label2.Size = new System.Drawing.Size( 27, 12 ) ;
            this.label2.TabIndex = 2 ;
            this.label2.Text = "CFT" ;
            // 
            // label1
            // 
            this.label1.AutoSize = true ;
            this.label1.Location = new System.Drawing.Point( 6, 28 ) ;
            this.label1.Name = "label1" ;
            this.label1.Size = new System.Drawing.Size( 50, 12 ) ;
            this.label1.TabIndex = 0 ;
            this.label1.Text = "RC/SRC" ;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.DGV ) ;
            this.groupBox2.Location = new System.Drawing.Point( 271, 12 ) ;
            this.groupBox2.Name = "groupBox2" ;
            this.groupBox2.Size = new System.Drawing.Size( 296, 345 ) ;
            this.groupBox2.TabIndex = 1 ;
            this.groupBox2.TabStop = false ;
            this.groupBox2.Text = "Steel" ;
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false ;
            this.DGV.AllowUserToDeleteRows = false ;
            this.DGV.AllowUserToResizeColumns = false ;
            this.DGV.AllowUserToResizeRows = false ;
            this.DGV.BackgroundColor = System.Drawing.SystemColors.Control ;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize ;
            this.DGV.ColumnHeadersVisible = false ;
            this.DGV.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] { this.matename, this.Combbox } ) ;
            this.DGV.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter ;
            this.DGV.Location = new System.Drawing.Point( 6, 18 ) ;
            this.DGV.Name = "DGV" ;
            this.DGV.RowHeadersVisible = false ;
            this.DGV.RowTemplate.Height = 21 ;
            this.DGV.Size = new System.Drawing.Size( 284, 321 ) ;
            this.DGV.TabIndex = 0 ;
            this.DGV.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler( this.DGV_CellValidating ) ;
            this.DGV.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler( this.DGV_EditingControlShowing ) ;
            // 
            // matename
            // 
            this.matename.HeaderText = "STB Material Name" ;
            this.matename.Name = "matename" ;
            this.matename.ReadOnly = true ;
            this.matename.Resizable = System.Windows.Forms.DataGridViewTriState.False ;
            this.matename.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable ;
            // 
            // Combbox
            // 
            this.Combbox.HeaderText = "Revit Material" ;
            this.Combbox.Name = "Combbox" ;
            this.Combbox.Resizable = System.Windows.Forms.DataGridViewTriState.False ;
            this.Combbox.Width = 180 ;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true ;
            this.linkLabel1.Location = new System.Drawing.Point( 10, 383 ) ;
            this.linkLabel1.Name = "linkLabel1" ;
            this.linkLabel1.Size = new System.Drawing.Size( 34, 12 ) ;
            this.linkLabel1.TabIndex = 2 ;
            this.linkLabel1.TabStop = true ;
            this.linkLabel1.Text = "Help" ;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.linkLabel1_LinkClicked ) ;
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true ;
            this.linkLabel2.Location = new System.Drawing.Point( 50, 383 ) ;
            this.linkLabel2.Name = "linkLabel2" ;
            this.linkLabel2.Size = new System.Drawing.Size( 74, 12 ) ;
            this.linkLabel2.TabIndex = 3 ;
            this.linkLabel2.TabStop = true ;
            this.linkLabel2.Text = "About" ;
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.linkLabel2_LinkClicked ) ;
            // 
            // Back
            // 
            this.Back.Location = new System.Drawing.Point( 330, 378 ) ;
            this.Back.Name = "Back" ;
            this.Back.Size = new System.Drawing.Size( 75, 23 ) ;
            this.Back.TabIndex = 4 ;
            this.Back.Text = "Back" ;
            this.Back.UseVisualStyleBackColor = true ;
            this.Back.Click += new System.EventHandler( this.Back_Click ) ;
            // 
            // MaterialMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F ) ;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font ;
            this.ClientSize = new System.Drawing.Size( 579, 406 ) ;
            this.Controls.Add( this.Back ) ;
            this.Controls.Add( this.linkLabel2 ) ;
            this.Controls.Add( this.linkLabel1 ) ;
            this.Controls.Add( this.groupBox2 ) ;
            this.Controls.Add( this.groupBox1 ) ;
            this.Controls.Add( this.Cancel ) ;
            this.Controls.Add( this.OK ) ;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle ;
            this.KeyPreview = true ;
            this.MaximizeBox = false ;
            this.MinimizeBox = false ;
            this.Name = "MaterialMapping" ;
            this.ShowIcon = false ;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent ;
            this.Text = "Material Mapping" ;
            this.Load += new System.EventHandler( this.MaterialMapping_Load ) ;
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler( this.MaterialMapping_HelpRequested ) ;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler( this.MaterialMapping_KeyPress ) ;
            this.groupBox1.ResumeLayout( false ) ;
            this.groupBox1.PerformLayout() ;
            this.groupBox2.ResumeLayout( false ) ;
            ( (System.ComponentModel.ISupportInitialize)( this.DGV ) ).EndInit() ;
            this.ResumeLayout( false ) ;
            this.PerformLayout() ;
        }

        #endregion

        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView DGV;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.DataGridViewTextBoxColumn matename;
        private System.Windows.Forms.DataGridViewComboBoxColumn Combbox;
        private System.Windows.Forms.Button Back;
    }
}