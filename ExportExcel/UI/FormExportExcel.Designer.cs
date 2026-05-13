namespace ADSK.JExtRAC.ExportExcel.UI
{
    partial class FormExportExcel
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
            if (disposing && (components != null)) {
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnNoExport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.tvExports = new System.Windows.Forms.TreeView();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnSearchParameter = new System.Windows.Forms.Button();
            this.txtSearchCategory = new System.Windows.Forms.TextBox();
            this.txtSearchParameter = new System.Windows.Forms.TextBox();
            this.btnSearchCategory = new System.Windows.Forms.Button();
            this.lblCategory = new System.Windows.Forms.Label();
            this.lblParameter = new System.Windows.Forms.Label();
            this.lblExport = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSearchOutput = new System.Windows.Forms.Button();
            this.txtSearchExport = new System.Windows.Forms.TextBox();
            this.clbCategories = new System.Windows.Forms.CheckedListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tvParameters = new System.Windows.Forms.TreeView();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnCancel.Location = new System.Drawing.Point(200, 40);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(86, 23);
            this.btnCancel.TabIndex = 44;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOK.Location = new System.Drawing.Point(108, 40);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(86, 23);
            this.btnOK.TabIndex = 43;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnLoad.Location = new System.Drawing.Point(108, 4);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(86, 23);
            this.btnLoad.TabIndex = 41;
            this.btnLoad.Text = "設定読込";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnRestore.Location = new System.Drawing.Point(16, 4);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(86, 23);
            this.btnRestore.TabIndex = 40;
            this.btnRestore.Text = "設定復元";
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnCancel);
            this.panel5.Controls.Add(this.btnOK);
            this.panel5.Controls.Add(this.btnLoad);
            this.panel5.Controls.Add(this.btnSave);
            this.panel5.Controls.Add(this.btnRestore);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(640, 481);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(289, 69);
            this.panel5.TabIndex = 37;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSave.Location = new System.Drawing.Point(200, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(86, 23);
            this.btnSave.TabIndex = 42;
            this.btnSave.Text = "設定保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDown.Location = new System.Drawing.Point(1, 243);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(39, 23);
            this.btnDown.TabIndex = 33;
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnUp.Location = new System.Drawing.Point(1, 214);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(39, 23);
            this.btnUp.TabIndex = 32;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnNoExport
            // 
            this.btnNoExport.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnNoExport.Location = new System.Drawing.Point(1, 141);
            this.btnNoExport.Name = "btnNoExport";
            this.btnNoExport.Size = new System.Drawing.Size(39, 23);
            this.btnNoExport.TabIndex = 31;
            this.btnNoExport.Text = "<";
            this.btnNoExport.UseVisualStyleBackColor = true;
            this.btnNoExport.Click += new System.EventHandler(this.btnUnExport_Click);
            // 
            // btnExport
            // 
            this.btnExport.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnExport.Location = new System.Drawing.Point(1, 112);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(39, 23);
            this.btnExport.TabIndex = 30;
            this.btnExport.Text = ">";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // tvExports
            // 
            this.tvExports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvExports.Location = new System.Drawing.Point(640, 57);
            this.tvExports.Name = "tvExports";
            this.tvExports.Size = new System.Drawing.Size(289, 418);
            this.tvExports.TabIndex = 35;
            this.tvExports.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvExports_BeforeSelect);
            this.tvExports.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvExports_AfterSelect);
            this.tvExports.DoubleClick += new System.EventHandler(this.tvExports_DoubleClick);
            this.tvExports.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvExports_MouseDown);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnDown);
            this.panel4.Controls.Add(this.btnUp);
            this.panel4.Controls.Add(this.btnNoExport);
            this.panel4.Controls.Add(this.btnExport);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(591, 57);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(43, 418);
            this.panel4.TabIndex = 36;
            // 
            // btnSearchParameter
            // 
            this.btnSearchParameter.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSearchParameter.Location = new System.Drawing.Point(230, 5);
            this.btnSearchParameter.Name = "btnSearchParameter";
            this.btnSearchParameter.Size = new System.Drawing.Size(58, 23);
            this.btnSearchParameter.TabIndex = 28;
            this.btnSearchParameter.Text = "検索";
            this.btnSearchParameter.UseVisualStyleBackColor = true;
            this.btnSearchParameter.Click += new System.EventHandler(this.btnSearchParameter_Click);
            // 
            // txtSearchCategory
            // 
            this.txtSearchCategory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchCategory.Location = new System.Drawing.Point(0, 7);
            this.txtSearchCategory.Name = "txtSearchCategory";
            this.txtSearchCategory.Size = new System.Drawing.Size(218, 20);
            this.txtSearchCategory.TabIndex = 24;
            this.txtSearchCategory.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchCategory_KeyDown);
            // 
            // txtSearchParameter
            // 
            this.txtSearchParameter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchParameter.Location = new System.Drawing.Point(0, 7);
            this.txtSearchParameter.Name = "txtSearchParameter";
            this.txtSearchParameter.Size = new System.Drawing.Size(224, 20);
            this.txtSearchParameter.TabIndex = 27;
            this.txtSearchParameter.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchParameter_KeyDown);
            // 
            // btnSearchCategory
            // 
            this.btnSearchCategory.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSearchCategory.Location = new System.Drawing.Point(227, 5);
            this.btnSearchCategory.Name = "btnSearchCategory";
            this.btnSearchCategory.Size = new System.Drawing.Size(58, 23);
            this.btnSearchCategory.TabIndex = 25;
            this.btnSearchCategory.Text = "検索";
            this.btnSearchCategory.UseVisualStyleBackColor = true;
            this.btnSearchCategory.Click += new System.EventHandler(this.btnSearchCategory_Click);
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(3, 0);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(74, 13);
            this.lblCategory.TabIndex = 20;
            this.lblCategory.Text = "カテゴリを選択";
            // 
            // lblParameter
            // 
            this.lblParameter.AutoSize = true;
            this.lblParameter.Location = new System.Drawing.Point(297, 0);
            this.lblParameter.Name = "lblParameter";
            this.lblParameter.Size = new System.Drawing.Size(61, 13);
            this.lblParameter.TabIndex = 22;
            this.lblParameter.Text = "パラメーター";
            // 
            // lblExport
            // 
            this.lblExport.AutoSize = true;
            this.lblExport.Location = new System.Drawing.Point(640, 0);
            this.lblExport.Name = "lblExport";
            this.lblExport.Size = new System.Drawing.Size(31, 13);
            this.lblExport.TabIndex = 23;
            this.lblExport.Text = "出力";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSearchCategory);
            this.panel1.Controls.Add(this.txtSearchCategory);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(288, 34);
            this.panel1.TabIndex = 25;
            // 
            // btnSearchOutput
            // 
            this.btnSearchOutput.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSearchOutput.Location = new System.Drawing.Point(231, 5);
            this.btnSearchOutput.Name = "btnSearchOutput";
            this.btnSearchOutput.Size = new System.Drawing.Size(58, 23);
            this.btnSearchOutput.TabIndex = 35;
            this.btnSearchOutput.Text = "検索";
            this.btnSearchOutput.UseVisualStyleBackColor = true;
            this.btnSearchOutput.Click += new System.EventHandler(this.btnSearchOutput_Click);
            // 
            // txtSearchExport
            // 
            this.txtSearchExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearchExport.Location = new System.Drawing.Point(0, 7);
            this.txtSearchExport.Name = "txtSearchExport";
            this.txtSearchExport.Size = new System.Drawing.Size(225, 20);
            this.txtSearchExport.TabIndex = 34;
            this.txtSearchExport.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchExport_KeyDown);
            // 
            // clbCategories
            // 
            this.clbCategories.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clbCategories.CheckOnClick = true;
            this.clbCategories.FormattingEnabled = true;
            this.clbCategories.IntegralHeight = false;
            this.clbCategories.Location = new System.Drawing.Point(3, 57);
            this.clbCategories.Name = "clbCategories";
            this.clbCategories.Size = new System.Drawing.Size(288, 418);
            this.clbCategories.Sorted = true;
            this.clbCategories.TabIndex = 30;
            this.clbCategories.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.clbCategories_ItemCheck);
            this.clbCategories.SelectedIndexChanged += new System.EventHandler(this.clbCategories_SelectedIndexChanged);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnSearchOutput);
            this.panel3.Controls.Add(this.txtSearchExport);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(640, 17);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(289, 34);
            this.panel3.TabIndex = 29;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.clbCategories, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblCategory, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblParameter, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblExport, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tvParameters, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.tvExports, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 3, 3);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 14F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(932, 553);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnSearchParameter);
            this.panel2.Controls.Add(this.txtSearchParameter);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(297, 17);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(288, 34);
            this.panel2.TabIndex = 26;
            // 
            // tvParameters
            // 
            this.tvParameters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvParameters.Location = new System.Drawing.Point(297, 57);
            this.tvParameters.Name = "tvParameters";
            this.tvParameters.Size = new System.Drawing.Size(288, 418);
            this.tvParameters.TabIndex = 31;
            this.tvParameters.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvParameters_BeforeSelect);
            this.tvParameters.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvParameters_AfterSelect);
            this.tvParameters.DoubleClick += new System.EventHandler(this.tvParameters_DoubleClick);
            this.tvParameters.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tvParameters_MouseDown);
            // 
            // FormExportExcel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(935, 564);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(951, 603);
            this.Name = "FormExportExcel";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Excelエクスポート";
            this.Load += new System.EventHandler(this.FrmExportExcel_Load);
            this.panel5.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnNoExport;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.TreeView tvExports;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnSearchParameter;
        private System.Windows.Forms.TextBox txtSearchCategory;
        private System.Windows.Forms.TextBox txtSearchParameter;
        private System.Windows.Forms.Button btnSearchCategory;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblParameter;
        private System.Windows.Forms.Label lblExport;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnSearchOutput;
        private System.Windows.Forms.TextBox txtSearchExport;
        private System.Windows.Forms.CheckedListBox clbCategories;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TreeView tvParameters;
    }
}