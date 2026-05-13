namespace STBLink
{
    partial class LevelMapping
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle() ;
            this.DGV = new System.Windows.Forms.DataGridView() ;
            this.STBStory = new System.Windows.Forms.DataGridViewTextBoxColumn() ;
            this.Revit = new System.Windows.Forms.DataGridViewComboBoxColumn() ;
            this.Revit2 = new System.Windows.Forms.DataGridViewTextBoxColumn() ;
            this.btOK = new System.Windows.Forms.Button() ;
            this.Cancel = new System.Windows.Forms.Button() ;
            this.label1 = new System.Windows.Forms.Label() ;
            this.groupBox1 = new System.Windows.Forms.GroupBox() ;
            this.groupBox2 = new System.Windows.Forms.GroupBox() ;
            this.Numoffset_Y2 = new System.Windows.Forms.NumericUpDown() ;
            this.Numoffset_Y1 = new System.Windows.Forms.NumericUpDown() ;
            this.Numoffset_X2 = new System.Windows.Forms.NumericUpDown() ;
            this.Numoffset_X1 = new System.Windows.Forms.NumericUpDown() ;
            this.cmbRevit_Yaxis = new System.Windows.Forms.ComboBox() ;
            this.cmbRevit_Xaxis = new System.Windows.Forms.ComboBox() ;
            this.cmbSTB_Xaxis = new System.Windows.Forms.ComboBox() ;
            this.cmbSTB_Yaxis = new System.Windows.Forms.ComboBox() ;
            this.labmm2 = new System.Windows.Forms.Label() ;
            this.label6 = new System.Windows.Forms.Label() ;
            this.labmm1 = new System.Windows.Forms.Label() ;
            this.labY = new System.Windows.Forms.Label() ;
            this.labX = new System.Windows.Forms.Label() ;
            this.label5 = new System.Windows.Forms.Label() ;
            this.label4 = new System.Windows.Forms.Label() ;
            this.label3 = new System.Windows.Forms.Label() ;
            this.label2 = new System.Windows.Forms.Label() ;
            this.radb2 = new System.Windows.Forms.RadioButton() ;
            this.radb1 = new System.Windows.Forms.RadioButton() ;
            this.linkLabel1 = new System.Windows.Forms.LinkLabel() ;
            this.linkLabel2 = new System.Windows.Forms.LinkLabel() ;
            this.btBack = new System.Windows.Forms.Button() ;
            ( (System.ComponentModel.ISupportInitialize)( this.DGV ) ).BeginInit() ;
            this.groupBox1.SuspendLayout() ;
            this.groupBox2.SuspendLayout() ;
            ( (System.ComponentModel.ISupportInitialize)( this.Numoffset_Y2 ) ).BeginInit() ;
            ( (System.ComponentModel.ISupportInitialize)( this.Numoffset_Y1 ) ).BeginInit() ;
            ( (System.ComponentModel.ISupportInitialize)( this.Numoffset_X2 ) ).BeginInit() ;
            ( (System.ComponentModel.ISupportInitialize)( this.Numoffset_X1 ) ).BeginInit() ;
            this.SuspendLayout() ;
            // 
            // DGV
            // 
            this.DGV.AllowUserToAddRows = false ;
            this.DGV.AllowUserToDeleteRows = false ;
            this.DGV.AllowUserToResizeRows = false ;
            this.DGV.Anchor = System.Windows.Forms.AnchorStyles.None ;
            this.DGV.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells ;
            this.DGV.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize ;
            this.DGV.Columns.AddRange( new System.Windows.Forms.DataGridViewColumn[] { this.STBStory, this.Revit, this.Revit2 } ) ;
            this.DGV.Location = new System.Drawing.Point( 12, 29 ) ;
            this.DGV.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.DGV.Name = "DGV" ;
            this.DGV.RowHeadersVisible = false ;
            this.DGV.RowTemplate.Height = 21 ;
            this.DGV.Size = new System.Drawing.Size( 514, 204 ) ;
            this.DGV.TabIndex = 1 ;
            this.DGV.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler( this.DGV_CellEnter ) ;
            this.DGV.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler( this.DGV_CellPainting ) ;
            this.DGV.CellValidated += new System.Windows.Forms.DataGridViewCellEventHandler( this.DGV_CellValidated ) ;
            this.DGV.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler( this.DGV_CellValidating ) ;
            this.DGV.SelectionChanged += new System.EventHandler( this.DGV_SelectionChanged ) ;
            // 
            // STBStory
            // 
            this.STBStory.HeaderText = "ST-Bridge" ;
            this.STBStory.Name = "STBStory" ;
            this.STBStory.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable ;
            this.STBStory.Width = 110 ;
            // 
            // Revit
            // 
            this.Revit.HeaderText = "Revit" ;
            this.Revit.MaxDropDownItems = 99 ;
            this.Revit.Name = "Revit" ;
            this.Revit.Resizable = System.Windows.Forms.DataGridViewTriState.False ;
            this.Revit.Width = 284 ;
            // 
            // Revit2
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight ;
            dataGridViewCellStyle1.NullValue = "0" ;
            this.Revit2.DefaultCellStyle = dataGridViewCellStyle1 ;
            this.Revit2.HeaderText = "Revit" ;
            this.Revit2.MaxInputLength = 8 ;
            this.Revit2.Name = "Revit2" ;
            this.Revit2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable ;
            // 
            // btOK
            // 
            this.btOK.Anchor = System.Windows.Forms.AnchorStyles.None ;
            this.btOK.Location = new System.Drawing.Point( 374, 463 ) ;
            this.btOK.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.btOK.Name = "btOK" ;
            this.btOK.Size = new System.Drawing.Size( 74, 23 ) ;
            this.btOK.TabIndex = 6 ;
            this.btOK.Text = "Next" ;
            this.btOK.UseVisualStyleBackColor = true ;
            this.btOK.Click += new System.EventHandler( this.BtOK_Click ) ;
            // 
            // Cancel
            // 
            this.Cancel.Anchor = System.Windows.Forms.AnchorStyles.None ;
            this.Cancel.Location = new System.Drawing.Point( 455, 463 ) ;
            this.Cancel.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.Cancel.Name = "Cancel" ;
            this.Cancel.Size = new System.Drawing.Size( 74, 23 ) ;
            this.Cancel.TabIndex = 7 ;
            this.Cancel.Text = "Cancel" ;
            this.Cancel.UseVisualStyleBackColor = true ;
            this.Cancel.Click += new System.EventHandler( this.Cancel_Click ) ;
            // 
            // label1
            // 
            this.label1.AutoSize = true ;
            this.label1.Font = new System.Drawing.Font( "MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ( (byte)( 128 ) ) ) ;
            this.label1.Location = new System.Drawing.Point( 10, 11 ) ;
            this.label1.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.label1.Name = "label1" ;
            this.label1.Size = new System.Drawing.Size( 270, 12 ) ;
            this.label1.TabIndex = 0 ;
            this.label1.Text = "Map each ST-Bridge story to a Revit level." ;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.None ;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink ;
            this.groupBox1.Controls.Add( this.groupBox2 ) ;
            this.groupBox1.Controls.Add( this.radb2 ) ;
            this.groupBox1.Controls.Add( this.radb1 ) ;
            this.groupBox1.Font = new System.Drawing.Font( "MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ( (byte)( 128 ) ) ) ;
            this.groupBox1.Location = new System.Drawing.Point( 12, 243 ) ;
            this.groupBox1.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.groupBox1.Name = "groupBox1" ;
            this.groupBox1.Padding = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.groupBox1.Size = new System.Drawing.Size( 514, 179 ) ;
            this.groupBox1.TabIndex = 2 ;
            this.groupBox1.TabStop = false ;
            this.groupBox1.Text = "Reference position" ;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add( this.Numoffset_Y2 ) ;
            this.groupBox2.Controls.Add( this.Numoffset_Y1 ) ;
            this.groupBox2.Controls.Add( this.Numoffset_X2 ) ;
            this.groupBox2.Controls.Add( this.Numoffset_X1 ) ;
            this.groupBox2.Controls.Add( this.cmbRevit_Yaxis ) ;
            this.groupBox2.Controls.Add( this.cmbRevit_Xaxis ) ;
            this.groupBox2.Controls.Add( this.cmbSTB_Xaxis ) ;
            this.groupBox2.Controls.Add( this.cmbSTB_Yaxis ) ;
            this.groupBox2.Controls.Add( this.labmm2 ) ;
            this.groupBox2.Controls.Add( this.label6 ) ;
            this.groupBox2.Controls.Add( this.labmm1 ) ;
            this.groupBox2.Controls.Add( this.labY ) ;
            this.groupBox2.Controls.Add( this.labX ) ;
            this.groupBox2.Controls.Add( this.label5 ) ;
            this.groupBox2.Controls.Add( this.label4 ) ;
            this.groupBox2.Controls.Add( this.label3 ) ;
            this.groupBox2.Controls.Add( this.label2 ) ;
            this.groupBox2.Location = new System.Drawing.Point( 6, 72 ) ;
            this.groupBox2.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.groupBox2.Name = "groupBox2" ;
            this.groupBox2.Padding = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.groupBox2.Size = new System.Drawing.Size( 502, 92 ) ;
            this.groupBox2.TabIndex = 0 ;
            this.groupBox2.TabStop = false ;
            // 
            // Numoffset_Y2
            // 
            this.Numoffset_Y2.DecimalPlaces = 1 ;
            this.Numoffset_Y2.Location = new System.Drawing.Point( 270, 64 ) ;
            this.Numoffset_Y2.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.Numoffset_Y2.Maximum = new decimal( new int[] { 1000000, 0, 0, 0 } ) ;
            this.Numoffset_Y2.Minimum = new decimal( new int[] { 1000000, 0, 0, -2147483648 } ) ;
            this.Numoffset_Y2.Name = "Numoffset_Y2" ;
            this.Numoffset_Y2.Size = new System.Drawing.Size( 50, 19 ) ;
            this.Numoffset_Y2.TabIndex = 9 ;
            this.Numoffset_Y2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right ;
            // 
            // Numoffset_Y1
            // 
            this.Numoffset_Y1.DecimalPlaces = 1 ;
            this.Numoffset_Y1.Location = new System.Drawing.Point( 362, 69 ) ;
            this.Numoffset_Y1.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.Numoffset_Y1.Maximum = new decimal( new int[] { 1000000, 0, 0, 0 } ) ;
            this.Numoffset_Y1.Minimum = new decimal( new int[] { 1000000, 0, 0, -2147483648 } ) ;
            this.Numoffset_Y1.Name = "Numoffset_Y1" ;
            this.Numoffset_Y1.Size = new System.Drawing.Size( 50, 19 ) ;
            this.Numoffset_Y1.TabIndex = 13 ;
            this.Numoffset_Y1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right ;
            this.Numoffset_Y1.Enter += new System.EventHandler( this.Numoffset_Y_Enter ) ;
            // 
            // Numoffset_X2
            // 
            this.Numoffset_X2.DecimalPlaces = 1 ;
            this.Numoffset_X2.Location = new System.Drawing.Point( 134, 67 ) ;
            this.Numoffset_X2.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.Numoffset_X2.Maximum = new decimal( new int[] { 1000000, 0, 0, 0 } ) ;
            this.Numoffset_X2.Minimum = new decimal( new int[] { 1000000, 0, 0, -2147483648 } ) ;
            this.Numoffset_X2.Name = "Numoffset_X2" ;
            this.Numoffset_X2.Size = new System.Drawing.Size( 50, 19 ) ;
            this.Numoffset_X2.TabIndex = 5 ;
            this.Numoffset_X2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right ;
            // 
            // Numoffset_X1
            // 
            this.Numoffset_X1.DecimalPlaces = 1 ;
            this.Numoffset_X1.Location = new System.Drawing.Point( 362, 29 ) ;
            this.Numoffset_X1.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.Numoffset_X1.Maximum = new decimal( new int[] { 1000000, 0, 0, 0 } ) ;
            this.Numoffset_X1.Minimum = new decimal( new int[] { 1000000, 0, 0, -2147483648 } ) ;
            this.Numoffset_X1.Name = "Numoffset_X1" ;
            this.Numoffset_X1.Size = new System.Drawing.Size( 50, 19 ) ;
            this.Numoffset_X1.TabIndex = 12 ;
            this.Numoffset_X1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right ;
            this.Numoffset_X1.Enter += new System.EventHandler( this.Numoffset_X_Enter ) ;
            // 
            // cmbRevit_Yaxis
            // 
            this.cmbRevit_Yaxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList ;
            this.cmbRevit_Yaxis.FormattingEnabled = true ;
            this.cmbRevit_Yaxis.Location = new System.Drawing.Point( 204, 60 ) ;
            this.cmbRevit_Yaxis.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.cmbRevit_Yaxis.Name = "cmbRevit_Yaxis" ;
            this.cmbRevit_Yaxis.Size = new System.Drawing.Size( 40, 20 ) ;
            this.cmbRevit_Yaxis.TabIndex = 7 ;
            // 
            // cmbRevit_Xaxis
            // 
            this.cmbRevit_Xaxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList ;
            this.cmbRevit_Xaxis.FormattingEnabled = true ;
            this.cmbRevit_Xaxis.Location = new System.Drawing.Point( 204, 31 ) ;
            this.cmbRevit_Xaxis.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.cmbRevit_Xaxis.Name = "cmbRevit_Xaxis" ;
            this.cmbRevit_Xaxis.Size = new System.Drawing.Size( 40, 20 ) ;
            this.cmbRevit_Xaxis.TabIndex = 6 ;
            // 
            // cmbSTB_Xaxis
            // 
            this.cmbSTB_Xaxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList ;
            this.cmbSTB_Xaxis.FormattingEnabled = true ;
            this.cmbSTB_Xaxis.Location = new System.Drawing.Point( 62, 31 ) ;
            this.cmbSTB_Xaxis.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.cmbSTB_Xaxis.Name = "cmbSTB_Xaxis" ;
            this.cmbSTB_Xaxis.Size = new System.Drawing.Size( 40, 20 ) ;
            this.cmbSTB_Xaxis.TabIndex = 2 ;
            // 
            // cmbSTB_Yaxis
            // 
            this.cmbSTB_Yaxis.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList ;
            this.cmbSTB_Yaxis.FormattingEnabled = true ;
            this.cmbSTB_Yaxis.Location = new System.Drawing.Point( 62, 60 ) ;
            this.cmbSTB_Yaxis.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.cmbSTB_Yaxis.Name = "cmbSTB_Yaxis" ;
            this.cmbSTB_Yaxis.Size = new System.Drawing.Size( 40, 20 ) ;
            this.cmbSTB_Yaxis.TabIndex = 3 ;
            // 
            // labmm2
            // 
            this.labmm2.AutoSize = true ;
            this.labmm2.Location = new System.Drawing.Point( 418, 68 ) ;
            this.labmm2.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.labmm2.Name = "labmm2" ;
            this.labmm2.Size = new System.Drawing.Size( 17, 12 ) ;
            this.labmm2.TabIndex = 15 ;
            this.labmm2.Text = "mm" ;
            this.labmm2.TextAlign = System.Drawing.ContentAlignment.BottomLeft ;
            // 
            // label6
            // 
            this.label6.AutoSize = true ;
            this.label6.Location = new System.Drawing.Point( 442, 52 ) ;
            this.label6.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.label6.Name = "label6" ;
            this.label6.Size = new System.Drawing.Size( 35, 12 ) ;
            this.label6.TabIndex = 16 ;
            this.label6.Text = "label6" ;
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft ;
            // 
            // labmm1
            // 
            this.labmm1.AutoSize = true ;
            this.labmm1.Location = new System.Drawing.Point( 418, 31 ) ;
            this.labmm1.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.labmm1.Name = "labmm1" ;
            this.labmm1.Size = new System.Drawing.Size( 17, 12 ) ;
            this.labmm1.TabIndex = 14 ;
            this.labmm1.Text = "mm" ;
            this.labmm1.TextAlign = System.Drawing.ContentAlignment.BottomLeft ;
            // 
            // labY
            // 
            this.labY.AutoSize = true ;
            this.labY.Location = new System.Drawing.Point( 344, 71 ) ;
            this.labY.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.labY.Name = "labY" ;
            this.labY.Size = new System.Drawing.Size( 12, 12 ) ;
            this.labY.TabIndex = 11 ;
            this.labY.Text = "Y" ;
            this.labY.TextAlign = System.Drawing.ContentAlignment.BottomLeft ;
            // 
            // labX
            // 
            this.labX.AutoSize = true ;
            this.labX.Location = new System.Drawing.Point( 344, 35 ) ;
            this.labX.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.labX.Name = "labX" ;
            this.labX.Size = new System.Drawing.Size( 12, 12 ) ;
            this.labX.TabIndex = 10 ;
            this.labX.Text = "X" ;
            this.labX.TextAlign = System.Drawing.ContentAlignment.BottomLeft ;
            // 
            // label5
            // 
            this.label5.AutoSize = true ;
            this.label5.Location = new System.Drawing.Point( 253, 52 ) ;
            this.label5.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.label5.Name = "label5" ;
            this.label5.Size = new System.Drawing.Size( 35, 12 ) ;
            this.label5.TabIndex = 8 ;
            this.label5.Text = "label5" ;
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft ;
            // 
            // label4
            // 
            this.label4.AutoSize = true ;
            this.label4.Location = new System.Drawing.Point( 110, 52 ) ;
            this.label4.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.label4.Name = "label4" ;
            this.label4.Size = new System.Drawing.Size( 35, 12 ) ;
            this.label4.TabIndex = 4 ;
            this.label4.Text = "label4" ;
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft ;
            // 
            // label3
            // 
            this.label3.AutoSize = true ;
            this.label3.Location = new System.Drawing.Point( 6, 52 ) ;
            this.label3.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.label3.Name = "label3" ;
            this.label3.Size = new System.Drawing.Size( 35, 12 ) ;
            this.label3.TabIndex = 1 ;
            this.label3.Text = "label3" ;
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft ;
            // 
            // label2
            // 
            this.label2.AutoSize = true ;
            this.label2.Location = new System.Drawing.Point( 6, 15 ) ;
            this.label2.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.label2.Name = "label2" ;
            this.label2.Size = new System.Drawing.Size( 35, 12 ) ;
            this.label2.TabIndex = 0 ;
            this.label2.Text = "label2" ;
            // 
            // radb2
            // 
            this.radb2.AutoSize = true ;
            this.radb2.Checked = true ;
            this.radb2.Location = new System.Drawing.Point( 28, 51 ) ;
            this.radb2.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.radb2.Name = "radb2" ;
            this.radb2.Size = new System.Drawing.Size( 308, 16 ) ;
            this.radb2.TabIndex = 1 ;
            this.radb2.TabStop = true ;
            this.radb2.Text = "Offset specification. Use this option when grids are unavailable." ;
            this.radb2.UseVisualStyleBackColor = true ;
            this.radb2.CheckedChanged += new System.EventHandler( this.Radb2_CheckedChanged ) ;
            // 
            // radb1
            // 
            this.radb1.AutoSize = true ;
            this.radb1.Location = new System.Drawing.Point( 28, 28 ) ;
            this.radb1.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.radb1.Name = "radb1" ;
            this.radb1.Size = new System.Drawing.Size( 90, 16 ) ;
            this.radb1.TabIndex = 0 ;
            this.radb1.Text = "Mapping by grid axes" ;
            this.radb1.UseVisualStyleBackColor = true ;
            this.radb1.CheckedChanged += new System.EventHandler( this.Radb1_CheckedChanged ) ;
            // 
            // linkLabel1
            // 
            this.linkLabel1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) ) ;
            this.linkLabel1.AutoSize = true ;
            this.linkLabel1.Location = new System.Drawing.Point( 10, 468 ) ;
            this.linkLabel1.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.linkLabel1.Name = "linkLabel1" ;
            this.linkLabel1.Size = new System.Drawing.Size( 34, 12 ) ;
            this.linkLabel1.TabIndex = 3 ;
            this.linkLabel1.TabStop = true ;
            this.linkLabel1.Text = "Help" ;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.LinkLabel1_LinkClicked ) ;
            // 
            // linkLabel2
            // 
            this.linkLabel2.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) ) ;
            this.linkLabel2.AutoSize = true ;
            this.linkLabel2.Location = new System.Drawing.Point( 49, 468 ) ;
            this.linkLabel2.Margin = new System.Windows.Forms.Padding( 2, 0, 2, 0 ) ;
            this.linkLabel2.Name = "linkLabel2" ;
            this.linkLabel2.Size = new System.Drawing.Size( 74, 12 ) ;
            this.linkLabel2.TabIndex = 4 ;
            this.linkLabel2.TabStop = true ;
            this.linkLabel2.Text = "About" ;
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler( this.LinkLabel2_LinkClicked ) ;
            // 
            // btBack
            // 
            this.btBack.Anchor = System.Windows.Forms.AnchorStyles.None ;
            this.btBack.Location = new System.Drawing.Point( 293, 463 ) ;
            this.btBack.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.btBack.Name = "btBack" ;
            this.btBack.Size = new System.Drawing.Size( 74, 23 ) ;
            this.btBack.TabIndex = 5 ;
            this.btBack.Text = "Back" ;
            this.btBack.UseVisualStyleBackColor = true ;
            this.btBack.Click += new System.EventHandler( this.Button1_Click ) ;
            // 
            // LevelMapping
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 12F ) ;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font ;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange ;
            this.ClientSize = new System.Drawing.Size( 542, 489 ) ;
            this.Controls.Add( this.btBack ) ;
            this.Controls.Add( this.linkLabel2 ) ;
            this.Controls.Add( this.linkLabel1 ) ;
            this.Controls.Add( this.groupBox1 ) ;
            this.Controls.Add( this.label1 ) ;
            this.Controls.Add( this.Cancel ) ;
            this.Controls.Add( this.btOK ) ;
            this.Controls.Add( this.DGV ) ;
            this.Font = new System.Drawing.Font( "MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ( (byte)( 128 ) ) ) ;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle ;
            this.KeyPreview = true ;
            this.Margin = new System.Windows.Forms.Padding( 2, 3, 2, 3 ) ;
            this.MaximizeBox = false ;
            this.MinimizeBox = false ;
            this.Name = "LevelMapping" ;
            this.ShowIcon = false ;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide ;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent ;
            this.Text = "Level Mapping" ;
            this.Load += new System.EventHandler( this.LevelMapping_Load ) ;
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler( this.Help_Requested ) ;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler( this.LevelMapping_KeyDown ) ;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler( this.LevelMapping_KeyPress ) ;
            ( (System.ComponentModel.ISupportInitialize)( this.DGV ) ).EndInit() ;
            this.groupBox1.ResumeLayout( false ) ;
            this.groupBox1.PerformLayout() ;
            this.groupBox2.ResumeLayout( false ) ;
            this.groupBox2.PerformLayout() ;
            ( (System.ComponentModel.ISupportInitialize)( this.Numoffset_Y2 ) ).EndInit() ;
            ( (System.ComponentModel.ISupportInitialize)( this.Numoffset_Y1 ) ).EndInit() ;
            ( (System.ComponentModel.ISupportInitialize)( this.Numoffset_X2 ) ).EndInit() ;
            ( (System.ComponentModel.ISupportInitialize)( this.Numoffset_X1 ) ).EndInit() ;
            this.ResumeLayout( false ) ;
            this.PerformLayout() ;
        }

        #endregion

        private System.Windows.Forms.DataGridView DGV ;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Label label1 ;
        private System.Windows.Forms.GroupBox groupBox1 ;
        private System.Windows.Forms.RadioButton radb2 ;
        private System.Windows.Forms.RadioButton radb1 ;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown Numoffset_Y1;
        private System.Windows.Forms.NumericUpDown Numoffset_X1;
        private System.Windows.Forms.ComboBox cmbRevit_Yaxis;
        private System.Windows.Forms.ComboBox cmbRevit_Xaxis;
        private System.Windows.Forms.ComboBox cmbSTB_Xaxis;
        private System.Windows.Forms.ComboBox cmbSTB_Yaxis;
        private System.Windows.Forms.Label labmm2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labmm1;
        private System.Windows.Forms.Label labY;
        private System.Windows.Forms.Label labX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.NumericUpDown Numoffset_X2;
        private System.Windows.Forms.NumericUpDown Numoffset_Y2;
        private System.Windows.Forms.DataGridViewTextBoxColumn STBStory ;
        private System.Windows.Forms.DataGridViewComboBoxColumn Revit ;
        private System.Windows.Forms.DataGridViewTextBoxColumn Revit2 ;
        private System.Windows.Forms.Button btBack;
    }
}