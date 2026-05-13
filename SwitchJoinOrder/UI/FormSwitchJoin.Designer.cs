namespace ADSK.JExtRAC.SwitchJoinOrder.UI
{
    partial class FormSwitchJoin
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnRm = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.ckbGroup = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbLeft = new System.Windows.Forms.Label();
            this.lbRight = new System.Windows.Forms.Label();
            this.lbUp = new System.Windows.Forms.Label();
            this.lbPriority = new System.Windows.Forms.Label();
            this.lbDown = new System.Windows.Forms.Label();
            this.btnDetails = new System.Windows.Forms.Button();
            this.dgvRight = new System.Windows.Forms.DataGridView();
            this.nameCate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.countCate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbCount = new System.Windows.Forms.Label();
            this.dgvLeft = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeft)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRm
            // 
            this.btnRm.Location = new System.Drawing.Point(261, 126);
            this.btnRm.Name = "btnRm";
            this.btnRm.Size = new System.Drawing.Size(47, 23);
            this.btnRm.TabIndex = 9;
            this.btnRm.Text = "btnRm";
            this.btnRm.UseVisualStyleBackColor = true;
            this.btnRm.Click += new System.EventHandler(this.btnLeft_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(261, 98);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(47, 23);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "btnAdd";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnRight_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(261, 245);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(47, 23);
            this.btnUp.TabIndex = 9;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(261, 274);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(47, 23);
            this.btnDown.TabIndex = 9;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // ckbGroup
            // 
            this.ckbGroup.AutoSize = true;
            this.ckbGroup.Location = new System.Drawing.Point(311, 449);
            this.ckbGroup.Name = "ckbGroup";
            this.ckbGroup.Size = new System.Drawing.Size(73, 17);
            this.ckbGroup.TabIndex = 10;
            this.ckbGroup.Text = "ckbGroup";
            this.ckbGroup.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(371, 475);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 25);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(452, 475);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbLeft
            // 
            this.lbLeft.AutoSize = true;
            this.lbLeft.Location = new System.Drawing.Point(12, 4);
            this.lbLeft.Name = "lbLeft";
            this.lbLeft.Size = new System.Drawing.Size(33, 13);
            this.lbLeft.TabIndex = 14;
            this.lbLeft.Text = "lbLeft";
            // 
            // lbRight
            // 
            this.lbRight.AutoSize = true;
            this.lbRight.Location = new System.Drawing.Point(311, 4);
            this.lbRight.Name = "lbRight";
            this.lbRight.Size = new System.Drawing.Size(40, 13);
            this.lbRight.TabIndex = 14;
            this.lbRight.Text = "lbRight";
            // 
            // lbUp
            // 
            this.lbUp.AutoSize = true;
            this.lbUp.Location = new System.Drawing.Point(558, 78);
            this.lbUp.Name = "lbUp";
            this.lbUp.Size = new System.Drawing.Size(35, 13);
            this.lbUp.TabIndex = 15;
            this.lbUp.Text = "label1";
            // 
            // lbPriority
            // 
            this.lbPriority.AutoSize = true;
            this.lbPriority.Location = new System.Drawing.Point(558, 198);
            this.lbPriority.Name = "lbPriority";
            this.lbPriority.Size = new System.Drawing.Size(35, 13);
            this.lbPriority.TabIndex = 16;
            this.lbPriority.Text = "label2";
            // 
            // lbDown
            // 
            this.lbDown.AutoSize = true;
            this.lbDown.Location = new System.Drawing.Point(558, 354);
            this.lbDown.Name = "lbDown";
            this.lbDown.Size = new System.Drawing.Size(35, 13);
            this.lbDown.TabIndex = 17;
            this.lbDown.Text = "label3";
            // 
            // btnDetails
            // 
            this.btnDetails.Location = new System.Drawing.Point(12, 475);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new System.Drawing.Size(75, 23);
            this.btnDetails.TabIndex = 18;
            this.btnDetails.Text = "btnDetails";
            this.btnDetails.UseVisualStyleBackColor = true;
            this.btnDetails.Click += new System.EventHandler(this.btnDetails_Click);
            // 
            // dgvRight
            // 
            this.dgvRight.AllowUserToAddRows = false;
            this.dgvRight.AllowUserToDeleteRows = false;
            this.dgvRight.AllowUserToResizeColumns = false;
            this.dgvRight.AllowUserToResizeRows = false;
            this.dgvRight.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvRight.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvRight.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRight.ColumnHeadersVisible = false;
            this.dgvRight.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameCate,
            this.countCate});
            this.dgvRight.Location = new System.Drawing.Point(314, 25);
            this.dgvRight.MultiSelect = false;
            this.dgvRight.Name = "dgvRight";
            this.dgvRight.RowHeadersVisible = false;
            this.dgvRight.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvRight.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRight.Size = new System.Drawing.Size(240, 418);
            this.dgvRight.TabIndex = 20;
            this.dgvRight.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRight_CellClick);
            this.dgvRight.DoubleClick += new System.EventHandler(this.dgvRight_DoubleClick);
            // 
            // nameCate
            // 
            this.nameCate.HeaderText = "";
            this.nameCate.Name = "nameCate";
            this.nameCate.ReadOnly = true;
            this.nameCate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.nameCate.Width = 175;
            // 
            // countCate
            // 
            this.countCate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopLeft;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.countCate.DefaultCellStyle = dataGridViewCellStyle1;
            this.countCate.HeaderText = "";
            this.countCate.Name = "countCate";
            this.countCate.ReadOnly = true;
            this.countCate.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.Location = new System.Drawing.Point(490, 4);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(43, 13);
            this.lbCount.TabIndex = 21;
            this.lbCount.Text = "lbCount";
            // 
            // dgvLeft
            // 
            this.dgvLeft.AllowUserToAddRows = false;
            this.dgvLeft.AllowUserToDeleteRows = false;
            this.dgvLeft.AllowUserToResizeColumns = false;
            this.dgvLeft.AllowUserToResizeRows = false;
            this.dgvLeft.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLeft.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvLeft.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLeft.ColumnHeadersVisible = false;
            this.dgvLeft.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1});
            this.dgvLeft.Location = new System.Drawing.Point(15, 25);
            this.dgvLeft.MultiSelect = false;
            this.dgvLeft.Name = "dgvLeft";
            this.dgvLeft.RowHeadersVisible = false;
            this.dgvLeft.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvLeft.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLeft.Size = new System.Drawing.Size(240, 418);
            this.dgvLeft.TabIndex = 22;
            this.dgvLeft.DoubleClick += new System.EventHandler(this.dgvLeft_DoubleClick);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn1.HeaderText = "";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // FormSwitchJoin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 506);
            this.Controls.Add(this.dgvLeft);
            this.Controls.Add(this.lbCount);
            this.Controls.Add(this.dgvRight);
            this.Controls.Add(this.btnDetails);
            this.Controls.Add(this.lbDown);
            this.Controls.Add(this.lbPriority);
            this.Controls.Add(this.lbUp);
            this.Controls.Add(this.lbRight);
            this.Controls.Add(this.lbLeft);
            this.Controls.Add(this.ckbGroup);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnRm);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSwitchJoin";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormSwitchJoin";
            this.Load += new System.EventHandler(this.FormSwitchJoin_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLeft)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRm;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.CheckBox ckbGroup;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lbLeft;
        private System.Windows.Forms.Label lbRight;
		private System.Windows.Forms.Label lbUp;
		private System.Windows.Forms.Label lbPriority;
		private System.Windows.Forms.Label lbDown;
        private System.Windows.Forms.Button btnDetails;
        private System.Windows.Forms.DataGridView dgvRight;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.DataGridView dgvLeft;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameCate;
        private System.Windows.Forms.DataGridViewTextBoxColumn countCate;
    }
}
