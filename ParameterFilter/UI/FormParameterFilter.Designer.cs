using System.Windows.Forms;

namespace ADSK.JExtRAC.ParameterFilter.UI
{
    partial class FormParameterFilter
	{
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabParameterFilter = new System.Windows.Forms.TabControl();
            this.tabPageCategory = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lblCountObjCate = new System.Windows.Forms.Label();
            this.lblCountTypeCategory = new System.Windows.Forms.Label();
            this.lblCounterCategory = new System.Windows.Forms.Label();
            this.dgvCategory = new System.Windows.Forms.DataGridView();
            this.btnSelectClearCategory = new System.Windows.Forms.Button();
            this.btnSelectAllCategory = new System.Windows.Forms.Button();
            this.lblObjCounterCategory = new System.Windows.Forms.Label();
            this.tabPageFamily = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblCountObjectFamily = new System.Windows.Forms.Label();
            this.lblCountTypeFamily = new System.Windows.Forms.Label();
            this.dgvFamily = new System.Windows.Forms.DataGridView();
            this.lblObjectCounterFamily = new System.Windows.Forms.Label();
            this.lblTypeCounterFamily = new System.Windows.Forms.Label();
            this.btnSelectClearFamily = new System.Windows.Forms.Button();
            this.btnSelectAllFamily = new System.Windows.Forms.Button();
            this.tabPageFamilyType = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCountObjectFamilyType = new System.Windows.Forms.Label();
            this.lblCountTypeFamilyType = new System.Windows.Forms.Label();
            this.dgvFamilyType = new System.Windows.Forms.DataGridView();
            this.lblObjectCounterFamilyType = new System.Windows.Forms.Label();
            this.lblTypeCounterFamilyType = new System.Windows.Forms.Label();
            this.btnSelectClearFamilyType = new System.Windows.Forms.Button();
            this.btnSelectAllFamilyType = new System.Windows.Forms.Button();
            this.tabPageParameter = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnSettingParameterGroup = new System.Windows.Forms.Button();
            this.cbkSelectConnect = new System.Windows.Forms.CheckBox();
            this.lblCountObjectTypeParameter = new System.Windows.Forms.Label();
            this.lblCountTypeParameter = new System.Windows.Forms.Label();
            this.dgvParameter = new System.Windows.Forms.DataGridView();
            this.btnSelectClearParameter = new System.Windows.Forms.Button();
            this.btnSelectAllParameter = new System.Windows.Forms.Button();
            this.lblObjectCounterTypeParameter = new System.Windows.Forms.Label();
            this.lblTypeCounterTypeParameter = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnPrewview = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.dgvCategory_CbkCategory = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvCategory_Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCategory_Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvFamily_CbkFamily = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvFamily_Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvFamily_Family = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvFamily_Count = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvFamilyType_CbkFamilyType = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dataFamilyTypeA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataFamilyTypeB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvFamilyType_CountFamilyType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameter_CbkParameter = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvParameter_Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameter_FamilyType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameter_Parameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameter_Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameter_Min = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameter_To = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameter_Max = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameter_Error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvParameter_CountParameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabParameterFilter.SuspendLayout();
            this.tabPageCategory.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategory)).BeginInit();
            this.tabPageFamily.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamily)).BeginInit();
            this.tabPageFamilyType.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamilyType)).BeginInit();
            this.tabPageParameter.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameter)).BeginInit();
            this.SuspendLayout();
            // 
            // tabParameterFilter
            // 
            this.tabParameterFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabParameterFilter.Controls.Add(this.tabPageCategory);
            this.tabParameterFilter.Controls.Add(this.tabPageFamily);
            this.tabParameterFilter.Controls.Add(this.tabPageFamilyType);
            this.tabParameterFilter.Controls.Add(this.tabPageParameter);
            this.tabParameterFilter.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.tabParameterFilter.Location = new System.Drawing.Point(4, 4);
            this.tabParameterFilter.Name = "tabParameterFilter";
            this.tabParameterFilter.SelectedIndex = 0;
            this.tabParameterFilter.Size = new System.Drawing.Size(926, 351);
            this.tabParameterFilter.TabIndex = 0;
            this.tabParameterFilter.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.tabParameterFilter_Selecting);
            // 
            // tabPageCategory
            // 
            this.tabPageCategory.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPageCategory.Controls.Add(this.groupBox3);
            this.tabPageCategory.Location = new System.Drawing.Point(4, 24);
            this.tabPageCategory.Name = "tabPageCategory";
            this.tabPageCategory.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCategory.Size = new System.Drawing.Size(912, 321);
            this.tabPageCategory.TabIndex = 2;
            this.tabPageCategory.Text = "tabPageCategory";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.lblCountObjCate);
            this.groupBox3.Controls.Add(this.lblCountTypeCategory);
            this.groupBox3.Controls.Add(this.lblCounterCategory);
            this.groupBox3.Controls.Add(this.dgvCategory);
            this.groupBox3.Controls.Add(this.btnSelectClearCategory);
            this.groupBox3.Controls.Add(this.btnSelectAllCategory);
            this.groupBox3.Controls.Add(this.lblObjCounterCategory);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(906, 315);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            // 
            // lblCountObjCate
            // 
            this.lblCountObjCate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCountObjCate.AutoSize = true;
            this.lblCountObjCate.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCountObjCate.Location = new System.Drawing.Point(3, 288);
            this.lblCountObjCate.Name = "lblCountObjCate";
            this.lblCountObjCate.Size = new System.Drawing.Size(76, 13);
            this.lblCountObjCate.TabIndex = 5;
            this.lblCountObjCate.Text = "lblCountObject";
            // 
            // lblCountTypeCategory
            // 
            this.lblCountTypeCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCountTypeCategory.AutoSize = true;
            this.lblCountTypeCategory.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCountTypeCategory.Location = new System.Drawing.Point(3, 269);
            this.lblCountTypeCategory.Name = "lblCountTypeCategory";
            this.lblCountTypeCategory.Size = new System.Drawing.Size(45, 13);
            this.lblCountTypeCategory.TabIndex = 3;
            this.lblCountTypeCategory.Text = "lblCount";
            // 
            // lblCounterCategory
            // 
            this.lblCounterCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCounterCategory.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCounterCategory.Location = new System.Drawing.Point(35, 269);
            this.lblCounterCategory.Name = "lblCounterCategory";
            this.lblCounterCategory.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblCounterCategory.Size = new System.Drawing.Size(85, 12);
            this.lblCounterCategory.TabIndex = 12;
            this.lblCounterCategory.Text = "lblCounter";
            this.lblCounterCategory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dgvCategory
            // 
            this.dgvCategory.AllowUserToAddRows = false;
            this.dgvCategory.AllowUserToDeleteRows = false;
            this.dgvCategory.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvCategory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCategory.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvCategory.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvCategory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCategory.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvCategory_CbkCategory,
            this.dgvCategory_Category,
            this.dgvCategory_Count});
            this.dgvCategory.GridColor = System.Drawing.Color.Gainsboro;
            this.dgvCategory.Location = new System.Drawing.Point(6, 10);
            this.dgvCategory.MultiSelect = false;
            this.dgvCategory.Name = "dgvCategory";
            this.dgvCategory.RowHeadersVisible = false;
            this.dgvCategory.RowTemplate.Height = 19;
            this.dgvCategory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvCategory.Size = new System.Drawing.Size(894, 251);
            this.dgvCategory.TabIndex = 11;
            this.dgvCategory.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCategory_CellClick);
            // 
            // btnSelectClearCategory
            // 
            this.btnSelectClearCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectClearCategory.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnSelectClearCategory.Location = new System.Drawing.Point(810, 272);
            this.btnSelectClearCategory.Name = "btnSelectClearCategory";
            this.btnSelectClearCategory.Size = new System.Drawing.Size(75, 23);
            this.btnSelectClearCategory.TabIndex = 8;
            this.btnSelectClearCategory.Text = "btnSelectClear";
            this.btnSelectClearCategory.UseVisualStyleBackColor = true;
            this.btnSelectClearCategory.Click += new System.EventHandler(this.btnSelectClearCategory_Click);
            // 
            // btnSelectAllCategory
            // 
            this.btnSelectAllCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAllCategory.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnSelectAllCategory.Location = new System.Drawing.Point(729, 272);
            this.btnSelectAllCategory.Name = "btnSelectAllCategory";
            this.btnSelectAllCategory.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAllCategory.TabIndex = 7;
            this.btnSelectAllCategory.Text = "btnSelectAll";
            this.btnSelectAllCategory.UseVisualStyleBackColor = true;
            this.btnSelectAllCategory.Click += new System.EventHandler(this.btnSelectAllCategory_Click);
            // 
            // lblObjCounterCategory
            // 
            this.lblObjCounterCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblObjCounterCategory.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblObjCounterCategory.Location = new System.Drawing.Point(35, 288);
            this.lblObjCounterCategory.Name = "lblObjCounterCategory";
            this.lblObjCounterCategory.Size = new System.Drawing.Size(85, 12);
            this.lblObjCounterCategory.TabIndex = 6;
            this.lblObjCounterCategory.Text = "lblObjectCounter";
            this.lblObjCounterCategory.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPageFamily
            // 
            this.tabPageFamily.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPageFamily.Controls.Add(this.groupBox2);
            this.tabPageFamily.Location = new System.Drawing.Point(4, 24);
            this.tabPageFamily.Name = "tabPageFamily";
            this.tabPageFamily.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFamily.Size = new System.Drawing.Size(912, 321);
            this.tabPageFamily.TabIndex = 1;
            this.tabPageFamily.Text = "tabPageFamily";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lblCountObjectFamily);
            this.groupBox2.Controls.Add(this.lblCountTypeFamily);
            this.groupBox2.Controls.Add(this.dgvFamily);
            this.groupBox2.Controls.Add(this.lblObjectCounterFamily);
            this.groupBox2.Controls.Add(this.lblTypeCounterFamily);
            this.groupBox2.Controls.Add(this.btnSelectClearFamily);
            this.groupBox2.Controls.Add(this.btnSelectAllFamily);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(906, 315);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            // 
            // lblCountObjectFamily
            // 
            this.lblCountObjectFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCountObjectFamily.AutoSize = true;
            this.lblCountObjectFamily.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCountObjectFamily.Location = new System.Drawing.Point(3, 288);
            this.lblCountObjectFamily.Name = "lblCountObjectFamily";
            this.lblCountObjectFamily.Size = new System.Drawing.Size(76, 13);
            this.lblCountObjectFamily.TabIndex = 5;
            this.lblCountObjectFamily.Text = "lblCountObject";
            // 
            // lblCountTypeFamily
            // 
            this.lblCountTypeFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCountTypeFamily.AutoSize = true;
            this.lblCountTypeFamily.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCountTypeFamily.Location = new System.Drawing.Point(3, 269);
            this.lblCountTypeFamily.Name = "lblCountTypeFamily";
            this.lblCountTypeFamily.Size = new System.Drawing.Size(45, 13);
            this.lblCountTypeFamily.TabIndex = 3;
            this.lblCountTypeFamily.Text = "lblCount";
            // 
            // dgvFamily
            // 
            this.dgvFamily.AllowUserToAddRows = false;
            this.dgvFamily.AllowUserToDeleteRows = false;
            this.dgvFamily.AllowUserToResizeRows = false;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvFamily.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFamily.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvFamily.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvFamily.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFamily.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvFamily_CbkFamily,
            this.dgvFamily_Category,
            this.dgvFamily_Family,
            this.dgvFamily_Count});
            this.dgvFamily.GridColor = System.Drawing.Color.Gainsboro;
            this.dgvFamily.Location = new System.Drawing.Point(6, 10);
            this.dgvFamily.MultiSelect = false;
            this.dgvFamily.Name = "dgvFamily";
            this.dgvFamily.RowHeadersVisible = false;
            this.dgvFamily.RowTemplate.Height = 19;
            this.dgvFamily.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvFamily.Size = new System.Drawing.Size(894, 251);
            this.dgvFamily.TabIndex = 10;
            this.dgvFamily.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFamily_CellClick);
            // 
            // lblObjectCounterFamily
            // 
            this.lblObjectCounterFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblObjectCounterFamily.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblObjectCounterFamily.Location = new System.Drawing.Point(35, 288);
            this.lblObjectCounterFamily.Name = "lblObjectCounterFamily";
            this.lblObjectCounterFamily.Size = new System.Drawing.Size(85, 12);
            this.lblObjectCounterFamily.TabIndex = 6;
            this.lblObjectCounterFamily.Text = "lblObjectCounter";
            this.lblObjectCounterFamily.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTypeCounterFamily
            // 
            this.lblTypeCounterFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTypeCounterFamily.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblTypeCounterFamily.Location = new System.Drawing.Point(35, 269);
            this.lblTypeCounterFamily.Name = "lblTypeCounterFamily";
            this.lblTypeCounterFamily.Size = new System.Drawing.Size(85, 12);
            this.lblTypeCounterFamily.TabIndex = 4;
            this.lblTypeCounterFamily.Text = "lblCounter";
            this.lblTypeCounterFamily.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSelectClearFamily
            // 
            this.btnSelectClearFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectClearFamily.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnSelectClearFamily.Location = new System.Drawing.Point(810, 272);
            this.btnSelectClearFamily.Name = "btnSelectClearFamily";
            this.btnSelectClearFamily.Size = new System.Drawing.Size(75, 23);
            this.btnSelectClearFamily.TabIndex = 8;
            this.btnSelectClearFamily.Text = "btnSelectClear";
            this.btnSelectClearFamily.UseVisualStyleBackColor = true;
            this.btnSelectClearFamily.Click += new System.EventHandler(this.btnSelectClearFamily_Click);
            // 
            // btnSelectAllFamily
            // 
            this.btnSelectAllFamily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAllFamily.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnSelectAllFamily.Location = new System.Drawing.Point(729, 272);
            this.btnSelectAllFamily.Name = "btnSelectAllFamily";
            this.btnSelectAllFamily.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAllFamily.TabIndex = 7;
            this.btnSelectAllFamily.Text = "btnSelectAll";
            this.btnSelectAllFamily.UseVisualStyleBackColor = true;
            this.btnSelectAllFamily.Click += new System.EventHandler(this.btnSelectAllFamily_Click);
            // 
            // tabPageFamilyType
            // 
            this.tabPageFamilyType.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPageFamilyType.Controls.Add(this.groupBox1);
            this.tabPageFamilyType.Location = new System.Drawing.Point(4, 24);
            this.tabPageFamilyType.Name = "tabPageFamilyType";
            this.tabPageFamilyType.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageFamilyType.Size = new System.Drawing.Size(912, 321);
            this.tabPageFamilyType.TabIndex = 0;
            this.tabPageFamilyType.Text = "tabPageFamilyType";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lblCountObjectFamilyType);
            this.groupBox1.Controls.Add(this.lblCountTypeFamilyType);
            this.groupBox1.Controls.Add(this.dgvFamilyType);
            this.groupBox1.Controls.Add(this.lblObjectCounterFamilyType);
            this.groupBox1.Controls.Add(this.lblTypeCounterFamilyType);
            this.groupBox1.Controls.Add(this.btnSelectClearFamilyType);
            this.groupBox1.Controls.Add(this.btnSelectAllFamilyType);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(906, 315);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // lblCountObjectFamilyType
            // 
            this.lblCountObjectFamilyType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCountObjectFamilyType.AutoSize = true;
            this.lblCountObjectFamilyType.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCountObjectFamilyType.Location = new System.Drawing.Point(3, 288);
            this.lblCountObjectFamilyType.Name = "lblCountObjectFamilyType";
            this.lblCountObjectFamilyType.Size = new System.Drawing.Size(76, 13);
            this.lblCountObjectFamilyType.TabIndex = 6;
            this.lblCountObjectFamilyType.Text = "lblCountObject";
            // 
            // lblCountTypeFamilyType
            // 
            this.lblCountTypeFamilyType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCountTypeFamilyType.AutoSize = true;
            this.lblCountTypeFamilyType.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCountTypeFamilyType.Location = new System.Drawing.Point(3, 269);
            this.lblCountTypeFamilyType.Name = "lblCountTypeFamilyType";
            this.lblCountTypeFamilyType.Size = new System.Drawing.Size(45, 13);
            this.lblCountTypeFamilyType.TabIndex = 4;
            this.lblCountTypeFamilyType.Text = "lblCount";
            // 
            // dgvFamilyType
            // 
            this.dgvFamilyType.AllowUserToAddRows = false;
            this.dgvFamilyType.AllowUserToDeleteRows = false;
            this.dgvFamilyType.AllowUserToResizeRows = false;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvFamilyType.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvFamilyType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFamilyType.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvFamilyType.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvFamilyType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFamilyType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvFamilyType_CbkFamilyType,
            this.dataFamilyTypeA,
            this.dataFamilyTypeB,
            this.dgvFamilyType_CountFamilyType});
            this.dgvFamilyType.GridColor = System.Drawing.Color.Gainsboro;
            this.dgvFamilyType.Location = new System.Drawing.Point(6, 10);
            this.dgvFamilyType.MultiSelect = false;
            this.dgvFamilyType.Name = "dgvFamilyType";
            this.dgvFamilyType.RowHeadersVisible = false;
            this.dgvFamilyType.RowTemplate.Height = 19;
            this.dgvFamilyType.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvFamilyType.Size = new System.Drawing.Size(894, 251);
            this.dgvFamilyType.TabIndex = 11;
            this.dgvFamilyType.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFamilyType_CellClick);
            // 
            // lblObjectCounterFamilyType
            // 
            this.lblObjectCounterFamilyType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblObjectCounterFamilyType.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblObjectCounterFamilyType.Location = new System.Drawing.Point(35, 288);
            this.lblObjectCounterFamilyType.Name = "lblObjectCounterFamilyType";
            this.lblObjectCounterFamilyType.Size = new System.Drawing.Size(85, 12);
            this.lblObjectCounterFamilyType.TabIndex = 7;
            this.lblObjectCounterFamilyType.Text = "lblObjectCounter";
            this.lblObjectCounterFamilyType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTypeCounterFamilyType
            // 
            this.lblTypeCounterFamilyType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTypeCounterFamilyType.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblTypeCounterFamilyType.Location = new System.Drawing.Point(35, 269);
            this.lblTypeCounterFamilyType.Name = "lblTypeCounterFamilyType";
            this.lblTypeCounterFamilyType.Size = new System.Drawing.Size(85, 12);
            this.lblTypeCounterFamilyType.TabIndex = 5;
            this.lblTypeCounterFamilyType.Text = "lblCounter";
            this.lblTypeCounterFamilyType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSelectClearFamilyType
            // 
            this.btnSelectClearFamilyType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectClearFamilyType.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnSelectClearFamilyType.Location = new System.Drawing.Point(810, 272);
            this.btnSelectClearFamilyType.Name = "btnSelectClearFamilyType";
            this.btnSelectClearFamilyType.Size = new System.Drawing.Size(75, 23);
            this.btnSelectClearFamilyType.TabIndex = 9;
            this.btnSelectClearFamilyType.Text = "btnSelectClear";
            this.btnSelectClearFamilyType.UseVisualStyleBackColor = true;
            this.btnSelectClearFamilyType.Click += new System.EventHandler(this.btnSelectClearFamilyType_Click);
            // 
            // btnSelectAllFamilyType
            // 
            this.btnSelectAllFamilyType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAllFamilyType.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnSelectAllFamilyType.Location = new System.Drawing.Point(729, 272);
            this.btnSelectAllFamilyType.Name = "btnSelectAllFamilyType";
            this.btnSelectAllFamilyType.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAllFamilyType.TabIndex = 8;
            this.btnSelectAllFamilyType.Text = "btnSelectAll";
            this.btnSelectAllFamilyType.UseVisualStyleBackColor = true;
            this.btnSelectAllFamilyType.Click += new System.EventHandler(this.btnSelectAllFamilyType_Click);
            // 
            // tabPageParameter
            // 
            this.tabPageParameter.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPageParameter.Controls.Add(this.groupBox4);
            this.tabPageParameter.Location = new System.Drawing.Point(4, 24);
            this.tabPageParameter.Name = "tabPageParameter";
            this.tabPageParameter.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageParameter.Size = new System.Drawing.Size(912, 321);
            this.tabPageParameter.TabIndex = 3;
            this.tabPageParameter.Text = "tabPageParameter";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.btnSettingParameterGroup);
            this.groupBox4.Controls.Add(this.cbkSelectConnect);
            this.groupBox4.Controls.Add(this.lblCountObjectTypeParameter);
            this.groupBox4.Controls.Add(this.lblCountTypeParameter);
            this.groupBox4.Controls.Add(this.dgvParameter);
            this.groupBox4.Controls.Add(this.btnSelectClearParameter);
            this.groupBox4.Controls.Add(this.btnSelectAllParameter);
            this.groupBox4.Controls.Add(this.lblObjectCounterTypeParameter);
            this.groupBox4.Controls.Add(this.lblTypeCounterTypeParameter);
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(906, 315);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            // 
            // btnSettingParameterGroup
            // 
            this.btnSettingParameterGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSettingParameterGroup.Location = new System.Drawing.Point(312, 272);
            this.btnSettingParameterGroup.Name = "btnSettingParameterGroup";
            this.btnSettingParameterGroup.Size = new System.Drawing.Size(160, 21);
            this.btnSettingParameterGroup.TabIndex = 4;
            this.btnSettingParameterGroup.Text = "btnSettingParameterGroup";
            this.btnSettingParameterGroup.UseVisualStyleBackColor = true;
            this.btnSettingParameterGroup.Click += new System.EventHandler(this.btnSettingParameterGroup_Click);
            // 
            // cbkSelectConnect
            // 
            this.cbkSelectConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cbkSelectConnect.AutoSize = true;
            this.cbkSelectConnect.Location = new System.Drawing.Point(176, 275);
            this.cbkSelectConnect.Name = "cbkSelectConnect";
            this.cbkSelectConnect.Size = new System.Drawing.Size(124, 19);
            this.cbkSelectConnect.TabIndex = 11;
            this.cbkSelectConnect.Text = "cbkSelectConnect";
            this.cbkSelectConnect.UseVisualStyleBackColor = true;
            // 
            // lblCountObjectTypeParameter
            // 
            this.lblCountObjectTypeParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCountObjectTypeParameter.AutoSize = true;
            this.lblCountObjectTypeParameter.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCountObjectTypeParameter.Location = new System.Drawing.Point(3, 288);
            this.lblCountObjectTypeParameter.Name = "lblCountObjectTypeParameter";
            this.lblCountObjectTypeParameter.Size = new System.Drawing.Size(76, 13);
            this.lblCountObjectTypeParameter.TabIndex = 5;
            this.lblCountObjectTypeParameter.Text = "lblCountObject";
            // 
            // lblCountTypeParameter
            // 
            this.lblCountTypeParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCountTypeParameter.AutoSize = true;
            this.lblCountTypeParameter.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblCountTypeParameter.Location = new System.Drawing.Point(3, 269);
            this.lblCountTypeParameter.Name = "lblCountTypeParameter";
            this.lblCountTypeParameter.Size = new System.Drawing.Size(45, 13);
            this.lblCountTypeParameter.TabIndex = 3;
            this.lblCountTypeParameter.Text = "lblCount";
            // 
            // dgvParameter
            // 
            this.dgvParameter.AllowUserToAddRows = false;
            this.dgvParameter.AllowUserToDeleteRows = false;
            this.dgvParameter.AllowUserToResizeRows = false;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvParameter.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle12;
            this.dgvParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvParameter.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgvParameter.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvParameter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParameter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvParameter_CbkParameter,
            this.dgvParameter_Category,
            this.dgvParameter_FamilyType,
            this.dgvParameter_Parameter,
            this.dgvParameter_Value,
            this.dgvParameter_Min,
            this.dgvParameter_To,
            this.dgvParameter_Max,
            this.dgvParameter_Error,
            this.dgvParameter_CountParameter});
            this.dgvParameter.GridColor = System.Drawing.Color.Gainsboro;
            this.dgvParameter.Location = new System.Drawing.Point(6, 10);
            this.dgvParameter.MultiSelect = false;
            this.dgvParameter.Name = "dgvParameter";
            this.dgvParameter.RowHeadersVisible = false;
            this.dgvParameter.RowTemplate.Height = 19;
            this.dgvParameter.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvParameter.Size = new System.Drawing.Size(894, 251);
            this.dgvParameter.TabIndex = 10;
            this.dgvParameter.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParameter_CellClick);
            this.dgvParameter.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgvParameter_CellPainting);
            // 
            // btnSelectClearParameter
            // 
            this.btnSelectClearParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectClearParameter.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnSelectClearParameter.Location = new System.Drawing.Point(810, 272);
            this.btnSelectClearParameter.Name = "btnSelectClearParameter";
            this.btnSelectClearParameter.Size = new System.Drawing.Size(75, 23);
            this.btnSelectClearParameter.TabIndex = 8;
            this.btnSelectClearParameter.Text = "btnSelectClear";
            this.btnSelectClearParameter.UseVisualStyleBackColor = true;
            this.btnSelectClearParameter.Click += new System.EventHandler(this.btnSelectClearParameter_Click);
            // 
            // btnSelectAllParameter
            // 
            this.btnSelectAllParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectAllParameter.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnSelectAllParameter.Location = new System.Drawing.Point(729, 272);
            this.btnSelectAllParameter.Name = "btnSelectAllParameter";
            this.btnSelectAllParameter.Size = new System.Drawing.Size(75, 23);
            this.btnSelectAllParameter.TabIndex = 7;
            this.btnSelectAllParameter.Text = "btnSelectAll";
            this.btnSelectAllParameter.UseVisualStyleBackColor = true;
            this.btnSelectAllParameter.Click += new System.EventHandler(this.btnSelectAllParameter_Click);
            // 
            // lblObjectCounterTypeParameter
            // 
            this.lblObjectCounterTypeParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblObjectCounterTypeParameter.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblObjectCounterTypeParameter.Location = new System.Drawing.Point(35, 288);
            this.lblObjectCounterTypeParameter.Name = "lblObjectCounterTypeParameter";
            this.lblObjectCounterTypeParameter.Size = new System.Drawing.Size(85, 12);
            this.lblObjectCounterTypeParameter.TabIndex = 6;
            this.lblObjectCounterTypeParameter.Text = "lblObjectCounter";
            this.lblObjectCounterTypeParameter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTypeCounterTypeParameter
            // 
            this.lblTypeCounterTypeParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTypeCounterTypeParameter.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblTypeCounterTypeParameter.Location = new System.Drawing.Point(35, 269);
            this.lblTypeCounterTypeParameter.Name = "lblTypeCounterTypeParameter";
            this.lblTypeCounterTypeParameter.Size = new System.Drawing.Size(85, 12);
            this.lblTypeCounterTypeParameter.TabIndex = 4;
            this.lblTypeCounterTypeParameter.Text = "lblCounter";
            this.lblTypeCounterTypeParameter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(740, 362);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(659, 362);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnPrewview
            // 
            this.btnPrewview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrewview.Location = new System.Drawing.Point(821, 362);
            this.btnPrewview.Name = "btnPrewview";
            this.btnPrewview.Size = new System.Drawing.Size(75, 23);
            this.btnPrewview.TabIndex = 1;
            this.btnPrewview.Text = "btnPreview";
            this.btnPrewview.UseVisualStyleBackColor = true;
            this.btnPrewview.Click += new System.EventHandler(this.btnPrewview_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Location = new System.Drawing.Point(572, 362);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 1;
            this.btnNext.Text = "btnNext";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevious.Location = new System.Drawing.Point(484, 362);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 23);
            this.btnPrevious.TabIndex = 1;
            this.btnPrevious.Text = "btnPrevious";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // dgvCategory_CbkCategory
            // 
            this.dgvCategory_CbkCategory.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dgvCategory_CbkCategory.HeaderText = "cbk";
            this.dgvCategory_CbkCategory.MinimumWidth = 35;
            this.dgvCategory_CbkCategory.Name = "dgvCategory_CbkCategory";
            this.dgvCategory_CbkCategory.ReadOnly = true;
            this.dgvCategory_CbkCategory.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCategory_CbkCategory.Width = 35;
            // 
            // dgvCategory_Category
            // 
            this.dgvCategory_Category.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvCategory_Category.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCategory_Category.HeaderText = "category";
            this.dgvCategory_Category.Name = "dgvCategory_Category";
            this.dgvCategory_Category.ReadOnly = true;
            this.dgvCategory_Category.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCategory_Category.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvCategory_Count
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvCategory_Count.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCategory_Count.HeaderText = "count";
            this.dgvCategory_Count.Name = "dgvCategory_Count";
            this.dgvCategory_Count.ReadOnly = true;
            this.dgvCategory_Count.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCategory_Count.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvCategory_Count.Width = 55;
            // 
            // dgvFamily_CbkFamily
            // 
            this.dgvFamily_CbkFamily.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dgvFamily_CbkFamily.HeaderText = "cbk";
            this.dgvFamily_CbkFamily.MinimumWidth = 35;
            this.dgvFamily_CbkFamily.Name = "dgvFamily_CbkFamily";
            this.dgvFamily_CbkFamily.ReadOnly = true;
            this.dgvFamily_CbkFamily.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFamily_CbkFamily.Width = 35;
            // 
            // dgvFamily_Category
            // 
            this.dgvFamily_Category.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvFamily_Category.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvFamily_Category.HeaderText = "category";
            this.dgvFamily_Category.Name = "dgvFamily_Category";
            this.dgvFamily_Category.ReadOnly = true;
            this.dgvFamily_Category.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFamily_Category.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvFamily_Family
            // 
            this.dgvFamily_Family.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvFamily_Family.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvFamily_Family.HeaderText = "family";
            this.dgvFamily_Family.Name = "dgvFamily_Family";
            this.dgvFamily_Family.ReadOnly = true;
            this.dgvFamily_Family.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFamily_Family.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvFamily_Count
            // 
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvFamily_Count.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvFamily_Count.HeaderText = "count";
            this.dgvFamily_Count.Name = "dgvFamily_Count";
            this.dgvFamily_Count.ReadOnly = true;
            this.dgvFamily_Count.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFamily_Count.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvFamily_Count.Width = 55;
            // 
            // dgvFamilyType_CbkFamilyType
            // 
            this.dgvFamilyType_CbkFamilyType.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dgvFamilyType_CbkFamilyType.HeaderText = "cbk";
            this.dgvFamilyType_CbkFamilyType.MinimumWidth = 35;
            this.dgvFamilyType_CbkFamilyType.Name = "dgvFamilyType_CbkFamilyType";
            this.dgvFamilyType_CbkFamilyType.ReadOnly = true;
            this.dgvFamilyType_CbkFamilyType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFamilyType_CbkFamilyType.Width = 35;
            // 
            // dataFamilyTypeA
            // 
            this.dataFamilyTypeA.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dataFamilyTypeA.DefaultCellStyle = dataGridViewCellStyle9;
            this.dataFamilyTypeA.HeaderText = "category";
            this.dataFamilyTypeA.Name = "dataFamilyTypeA";
            this.dataFamilyTypeA.ReadOnly = true;
            this.dataFamilyTypeA.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataFamilyTypeA.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataFamilyTypeB
            // 
            this.dataFamilyTypeB.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dataFamilyTypeB.DefaultCellStyle = dataGridViewCellStyle10;
            this.dataFamilyTypeB.HeaderText = "familyType";
            this.dataFamilyTypeB.Name = "dataFamilyTypeB";
            this.dataFamilyTypeB.ReadOnly = true;
            this.dataFamilyTypeB.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataFamilyTypeB.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvFamilyType_CountFamilyType
            // 
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvFamilyType_CountFamilyType.DefaultCellStyle = dataGridViewCellStyle11;
            this.dgvFamilyType_CountFamilyType.HeaderText = "count";
            this.dgvFamilyType_CountFamilyType.Name = "dgvFamilyType_CountFamilyType";
            this.dgvFamilyType_CountFamilyType.ReadOnly = true;
            this.dgvFamilyType_CountFamilyType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvFamilyType_CountFamilyType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvFamilyType_CountFamilyType.Width = 55;
            // 
            // dgvParameter_CbkParameter
            // 
            this.dgvParameter_CbkParameter.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.dgvParameter_CbkParameter.HeaderText = "cbk";
            this.dgvParameter_CbkParameter.MinimumWidth = 35;
            this.dgvParameter_CbkParameter.Name = "dgvParameter_CbkParameter";
            this.dgvParameter_CbkParameter.ReadOnly = true;
            this.dgvParameter_CbkParameter.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParameter_CbkParameter.Width = 35;
            // 
            // dgvParameter_Category
            // 
            this.dgvParameter_Category.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle13.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvParameter_Category.DefaultCellStyle = dataGridViewCellStyle13;
            this.dgvParameter_Category.HeaderText = "Category";
            this.dgvParameter_Category.Name = "dgvParameter_Category";
            this.dgvParameter_Category.ReadOnly = true;
            this.dgvParameter_Category.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParameter_Category.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvParameter_FamilyType
            // 
            this.dgvParameter_FamilyType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle14.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvParameter_FamilyType.DefaultCellStyle = dataGridViewCellStyle14;
            this.dgvParameter_FamilyType.HeaderText = "FamilyType";
            this.dgvParameter_FamilyType.Name = "dgvParameter_FamilyType";
            this.dgvParameter_FamilyType.ReadOnly = true;
            this.dgvParameter_FamilyType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParameter_FamilyType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvParameter_Parameter
            // 
            this.dgvParameter_Parameter.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle15.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvParameter_Parameter.DefaultCellStyle = dataGridViewCellStyle15;
            this.dgvParameter_Parameter.HeaderText = "Parameter";
            this.dgvParameter_Parameter.Name = "dgvParameter_Parameter";
            this.dgvParameter_Parameter.ReadOnly = true;
            this.dgvParameter_Parameter.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParameter_Parameter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvParameter_Value
            // 
            dataGridViewCellStyle16.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvParameter_Value.DefaultCellStyle = dataGridViewCellStyle16;
            this.dgvParameter_Value.HeaderText = "Value";
            this.dgvParameter_Value.Name = "dgvParameter_Value";
            this.dgvParameter_Value.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParameter_Value.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvParameter_Min
            // 
            this.dgvParameter_Min.HeaderText = "Min";
            this.dgvParameter_Min.Name = "dgvParameter_Min";
            this.dgvParameter_Min.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvParameter_To
            // 
            this.dgvParameter_To.HeaderText = "~";
            this.dgvParameter_To.Name = "dgvParameter_To";
            this.dgvParameter_To.ReadOnly = true;
            this.dgvParameter_To.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvParameter_To.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvParameter_To.Width = 30;
            // 
            // dgvParameter_Max
            // 
            this.dgvParameter_Max.HeaderText = "Max";
            this.dgvParameter_Max.Name = "dgvParameter_Max";
            this.dgvParameter_Max.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvParameter_Error
            // 
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgvParameter_Error.DefaultCellStyle = dataGridViewCellStyle17;
            this.dgvParameter_Error.HeaderText = "";
            this.dgvParameter_Error.Name = "dgvParameter_Error";
            this.dgvParameter_Error.ReadOnly = true;
            this.dgvParameter_Error.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvParameter_Error.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvParameter_Error.Width = 20;
            // 
            // dgvParameter_CountParameter
            // 
            dataGridViewCellStyle18.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopRight;
            dataGridViewCellStyle18.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.dgvParameter_CountParameter.DefaultCellStyle = dataGridViewCellStyle18;
            this.dgvParameter_CountParameter.HeaderText = "count";
            this.dgvParameter_CountParameter.Name = "dgvParameter_CountParameter";
            this.dgvParameter_CountParameter.ReadOnly = true;
            this.dgvParameter_CountParameter.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvParameter_CountParameter.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dgvParameter_CountParameter.Width = 55;
            // 
            // FormParameterFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 396);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabParameterFilter);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnPrevious);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.btnPrewview);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MinimumSize = new System.Drawing.Size(950, 435);
            this.Name = "FormParameterFilter";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "    ";
            this.Load += new System.EventHandler(this.FormLevelFilter_Load);
            this.tabParameterFilter.ResumeLayout(false);
            this.tabPageCategory.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCategory)).EndInit();
            this.tabPageFamily.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamily)).EndInit();
            this.tabPageFamilyType.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamilyType)).EndInit();
            this.tabPageParameter.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParameter)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private TabControl tabParameterFilter;
        private TabPage tabPageCategory;
        private Button btnCancel;
        private Button btnOK;
        private Button btnPrewview;
        private TabPage tabPageFamily;
        private GroupBox groupBox2;
        private Label lblObjectCounterFamily;
        private Label lblCountObjectFamily;
        private Label lblTypeCounterFamily;
        private Label lblCountTypeFamily;
        private Button btnSelectClearFamily;
        private Button btnSelectAllFamily;
        private TabPage tabPageFamilyType;
        private GroupBox groupBox1;
        private Label lblObjectCounterFamilyType;
        private Label lblCountObjectFamilyType;
        private Label lblTypeCounterFamilyType;
        private Label lblCountTypeFamilyType;
        private Button btnSelectClearFamilyType;
        private Button btnSelectAllFamilyType;
        private TabPage tabPageParameter;
        private GroupBox groupBox4;
        private Button btnSelectClearParameter;
        private Button btnSelectAllParameter;
        private Label lblObjectCounterTypeParameter;
        private Label lblCountObjectTypeParameter;
        private Label lblTypeCounterTypeParameter;
        private Label lblCountTypeParameter;
        private GroupBox groupBox3;
        private Button btnSelectClearCategory;
        private Button btnSelectAllCategory;
        private Label lblObjCounterCategory;
        private Label lblCountObjCate;
        private Label lblCountTypeCategory;
        public DataGridView dgvFamily;
        public DataGridView dgvFamilyType;
        public DataGridView dgvParameter;
        public DataGridView dgvCategory;
        private Label lblCounterCategory;
        private Button btnNext;
        private Button btnPrevious;
        private CheckBox cbkSelectConnect;
        private Button btnSettingParameterGroup;
        private DataGridViewCheckBoxColumn dgvCategory_CbkCategory;
        private DataGridViewTextBoxColumn dgvCategory_Category;
        private DataGridViewTextBoxColumn dgvCategory_Count;
        private DataGridViewCheckBoxColumn dgvFamily_CbkFamily;
        private DataGridViewTextBoxColumn dgvFamily_Category;
        private DataGridViewTextBoxColumn dgvFamily_Family;
        private DataGridViewTextBoxColumn dgvFamily_Count;
        private DataGridViewCheckBoxColumn dgvFamilyType_CbkFamilyType;
        private DataGridViewTextBoxColumn dataFamilyTypeA;
        private DataGridViewTextBoxColumn dataFamilyTypeB;
        private DataGridViewTextBoxColumn dgvFamilyType_CountFamilyType;
        private DataGridViewCheckBoxColumn dgvParameter_CbkParameter;
        private DataGridViewTextBoxColumn dgvParameter_Category;
        private DataGridViewTextBoxColumn dgvParameter_FamilyType;
        private DataGridViewTextBoxColumn dgvParameter_Parameter;
        private DataGridViewTextBoxColumn dgvParameter_Value;
        private DataGridViewTextBoxColumn dgvParameter_Min;
        private DataGridViewTextBoxColumn dgvParameter_To;
        private DataGridViewTextBoxColumn dgvParameter_Max;
        private DataGridViewTextBoxColumn dgvParameter_Error;
        private DataGridViewTextBoxColumn dgvParameter_CountParameter;
    }
}