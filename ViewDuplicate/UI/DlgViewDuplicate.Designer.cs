using R = ADSK.ViewExtension.ViewDuplicate.Resources;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ADSK.ViewExtension.ViewDuplicate.UI
{
    partial class DlgViewDuplicate
    {
        private IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                    components.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void InitializeComponent()
        {
            TableLayoutPanel1 = new TableLayoutPanel();
            OK_Button = new Button();
            Cancel_Button = new Button();
            CbxDicipline = new ComboBox();
            Label1 = new Label();
            CbxViewType = new ComboBox();
            Label2 = new Label();
            CbxViewFamilyType = new ComboBox();
            Label3 = new Label();
            LbxViews = new ListBox();
            dgViews = new DataGridView();
            DgcolPSfix = new DataGridViewTextBoxColumn();
            DgcolPrefix = new DataGridViewTextBoxColumn();
            DgcolViewTemplate = new DataGridViewTextBoxColumn();
            DgcolViewFamilyType = new DataGridViewTextBoxColumn();
            BtnAdd = new Button();
            BtnDel = new Button();
            CbxDupMode = new ComboBox();
            LblDupMode = new Label();
            Label4 = new Label();
            TableLayoutPanel1.SuspendLayout();
            ((ISupportInitialize)dgViews).BeginInit();
            SuspendLayout();
            //
            // TableLayoutPanel1
            //
            TableLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            TableLayoutPanel1.ColumnCount = 2;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            TableLayoutPanel1.Controls.Add(OK_Button, 0, 0);
            TableLayoutPanel1.Controls.Add(Cancel_Button, 1, 0);
            TableLayoutPanel1.Location = new Point(572, 305);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 1;
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            TableLayoutPanel1.Size = new Size(146, 27);
            TableLayoutPanel1.TabIndex = 12;
            //
            // OK_Button
            //
            OK_Button.Anchor = AnchorStyles.None;
            OK_Button.Location = new Point(3, 3);
            OK_Button.Name = "OK_Button";
            OK_Button.Size = new Size(67, 21);
            OK_Button.TabIndex = 0;
            OK_Button.Text = "OK";
            //
            // Cancel_Button
            //
            Cancel_Button.Anchor = AnchorStyles.None;
            Cancel_Button.DialogResult = DialogResult.Cancel;
            Cancel_Button.Location = new Point(76, 3);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.Size = new Size(67, 21);
            Cancel_Button.TabIndex = 1;
            Cancel_Button.Text = R.Text.BTN_CANCEL;
            //
            // CbxDicipline
            //
            CbxDicipline.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxDicipline.FormattingEnabled = true;
            CbxDicipline.Location = new Point(96, 12);
            CbxDicipline.Name = "CbxDicipline";
            CbxDicipline.Size = new Size(154, 20);
            CbxDicipline.Sorted = true;
            CbxDicipline.TabIndex = 1;
            //
            // Label1
            //
            Label1.AutoSize = true;
            Label1.Location = new Point(12, 15);
            Label1.Name = "Label1";
            Label1.Size = new Size(53, 12);
            Label1.TabIndex = 0;
            Label1.Text = R.Text.LBL_DISCIPLINE;
            //
            // CbxViewType
            //
            CbxViewType.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxViewType.FormattingEnabled = true;
            CbxViewType.Location = new Point(96, 38);
            CbxViewType.Name = "CbxViewType";
            CbxViewType.Size = new Size(154, 20);
            CbxViewType.Sorted = true;
            CbxViewType.TabIndex = 3;
            //
            // Label2
            //
            Label2.AutoSize = true;
            Label2.Location = new Point(12, 41);
            Label2.Name = "Label2";
            Label2.Size = new Size(57, 12);
            Label2.TabIndex = 2;
            Label2.Text = R.Text.LBL_VIEW_CATEGORY;
            //
            // CbxViewFamilyType
            //
            CbxViewFamilyType.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxViewFamilyType.FormattingEnabled = true;
            CbxViewFamilyType.Location = new Point(96, 64);
            CbxViewFamilyType.Name = "CbxViewFamilyType";
            CbxViewFamilyType.Size = new Size(154, 20);
            CbxViewFamilyType.Sorted = true;
            CbxViewFamilyType.TabIndex = 5;
            //
            // Label3
            //
            Label3.AutoSize = true;
            Label3.Location = new Point(12, 67);
            Label3.Name = "Label3";
            Label3.Size = new Size(60, 12);
            Label3.TabIndex = 4;
            Label3.Text = R.Text.LBL_VIEW_TYPE_FILTER;
            //
            // LbxViews
            //
            LbxViews.FormattingEnabled = true;
            LbxViews.ItemHeight = 12;
            LbxViews.Location = new Point(14, 90);
            LbxViews.Name = "LbxViews";
            LbxViews.SelectionMode = SelectionMode.MultiExtended;
            LbxViews.Size = new Size(236, 184);
            LbxViews.Sorted = true;
            LbxViews.TabIndex = 6;
            //
            // dgViews
            //
            dgViews.AllowUserToAddRows = false;
            dgViews.AllowUserToDeleteRows = false;
            dgViews.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgViews.Columns.AddRange(new DataGridViewColumn[] { DgcolPSfix, DgcolPrefix, DgcolViewTemplate, DgcolViewFamilyType });
            dgViews.Location = new Point(256, 12);
            dgViews.Name = "dgViews";
            dgViews.RowTemplate.Height = 21;
            dgViews.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgViews.Size = new Size(464, 262);
            dgViews.TabIndex = 9;
            //
            // DgcolPSfix
            //
            DgcolPSfix.HeaderText = R.Text.DGCOL_PREFIX_SUFFIX;
            DgcolPSfix.Name = "DgcolPSfix";
            DgcolPSfix.ReadOnly = true;
            DgcolPSfix.Resizable = DataGridViewTriState.True;
            DgcolPSfix.SortMode = DataGridViewColumnSortMode.NotSortable;
            //
            // DgcolPrefix
            //
            DgcolPrefix.HeaderText = R.Text.DGCOL_ADDED_TEXT;
            DgcolPrefix.Name = "DgcolPrefix";
            DgcolPrefix.ReadOnly = false;
            DgcolPrefix.Width = 120;
            //
            // DgcolViewTemplate
            //
            DgcolViewTemplate.FillWeight = 200F;
            DgcolViewTemplate.HeaderText = R.Text.DGCOL_VIEW_TEMPLATE;
            DgcolViewTemplate.Name = "DgcolViewTemplate";
            DgcolViewTemplate.ReadOnly = true;
            DgcolViewTemplate.Resizable = DataGridViewTriState.True;
            DgcolViewTemplate.Width = 150;
            //
            // DgcolViewFamilyType
            //
            DgcolViewFamilyType.HeaderText = R.Text.DGCOL_VIEW_TYPE;
            DgcolViewFamilyType.Name = "DgcolViewFamilyType";
            DgcolViewFamilyType.ReadOnly = true;
            DgcolViewFamilyType.Resizable = DataGridViewTriState.True;
            DgcolViewFamilyType.Width = 180;
            //
            // BtnAdd
            //
            BtnAdd.Location = new Point(256, 280);
            BtnAdd.Name = "BtnAdd";
            BtnAdd.Size = new Size(75, 23);
            BtnAdd.TabIndex = 10;
            BtnAdd.Text = R.Text.BTN_ADD;
            BtnAdd.UseVisualStyleBackColor = true;
            //
            // BtnDel
            //
            BtnDel.Location = new Point(337, 280);
            BtnDel.Name = "BtnDel";
            BtnDel.Size = new Size(75, 23);
            BtnDel.TabIndex = 11;
            BtnDel.Text = R.Text.BTN_DELETE;
            BtnDel.UseVisualStyleBackColor = true;
            //
            // CbxDupMode
            //
            CbxDupMode.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxDupMode.FormattingEnabled = true;
            CbxDupMode.Location = new Point(96, 283);
            CbxDupMode.Name = "CbxDupMode";
            CbxDupMode.Size = new Size(154, 20);
            CbxDupMode.TabIndex = 8;
            //
            // LblDupMode
            //
            LblDupMode.AutoSize = true;
            LblDupMode.Location = new Point(12, 286);
            LblDupMode.Name = "LblDupMode";
            LblDupMode.Size = new Size(63, 12);
            LblDupMode.TabIndex = 7;
            LblDupMode.Text = R.Text.LBL_DUP_KIND;
            //
            // Label4
            //
            Label4.AutoSize = true;
            Label4.Location = new Point(414, 312);
            Label4.Name = "Label4";
            Label4.Size = new Size(139, 12);
            Label4.TabIndex = 13;
            Label4.Text = R.Text.LBL_CREDIT;
            Label4.Visible = false;
            //
            // DlgViewDuplicate
            //
            AcceptButton = OK_Button;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = Cancel_Button;
            ClientSize = new Size(730, 344);
            Controls.Add(Label4);
            Controls.Add(LblDupMode);
            Controls.Add(CbxDupMode);
            Controls.Add(BtnDel);
            Controls.Add(BtnAdd);
            Controls.Add(dgViews);
            Controls.Add(LbxViews);
            Controls.Add(Label3);
            Controls.Add(CbxViewFamilyType);
            Controls.Add(Label2);
            Controls.Add(CbxViewType);
            Controls.Add(Label1);
            Controls.Add(CbxDicipline);
            Controls.Add(TableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DlgViewDuplicate";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "DlgViewDuplicate";
            TableLayoutPanel1.ResumeLayout(false);
            ((ISupportInitialize)dgViews).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private TableLayoutPanel TableLayoutPanel1;
        private Button OK_Button;
        private Button Cancel_Button;
        private ComboBox CbxDicipline;
        private Label Label1;
        private ComboBox CbxViewType;
        private Label Label2;
        private ComboBox CbxViewFamilyType;
        private Label Label3;
        private ListBox LbxViews;
        private DataGridView dgViews;
        private Button BtnAdd;
        private Button BtnDel;
        private ComboBox CbxDupMode;
        private Label LblDupMode;
        private DataGridViewTextBoxColumn DgcolPSfix;
        private DataGridViewTextBoxColumn DgcolPrefix;
        private DataGridViewTextBoxColumn DgcolViewTemplate;
        private DataGridViewTextBoxColumn DgcolViewFamilyType;
        private Label Label4;
    }
}
