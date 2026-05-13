using R = ADSK.ViewExtension.ViewDuplicate.Resources;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace ADSK.ViewExtension.ViewDuplicate.UI
{
    partial class DlgViewDuplicateItem
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
            RbnPrefix = new RadioButton();
            RbnSuffix = new RadioButton();
            LblViewFamilyType = new Label();
            LblViewTemplate = new Label();
            CbxViewFamilyType = new ComboBox();
            CbxViewTemplate = new ComboBox();
            TbxAddFor = new TextBox();
            gpAddFor = new GroupBox();
            TableLayoutPanel1.SuspendLayout();
            gpAddFor.SuspendLayout();
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
            TableLayoutPanel1.Location = new Point(106, 202);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 1;
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            TableLayoutPanel1.Size = new Size(146, 27);
            TableLayoutPanel1.TabIndex = 0;
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
            // RbnPrefix
            //
            RbnPrefix.AutoSize = true;
            RbnPrefix.Location = new Point(26, 22);
            RbnPrefix.Name = "RbnPrefix";
            RbnPrefix.Size = new Size(47, 16);
            RbnPrefix.TabIndex = 1;
            RbnPrefix.TabStop = true;
            RbnPrefix.Text = R.Text.TXT_PREFIX;
            RbnPrefix.UseVisualStyleBackColor = true;
            //
            // RbnSuffix
            //
            RbnSuffix.AutoSize = true;
            RbnSuffix.Location = new Point(145, 22);
            RbnSuffix.Name = "RbnSuffix";
            RbnSuffix.Size = new Size(47, 16);
            RbnSuffix.TabIndex = 2;
            RbnSuffix.TabStop = true;
            RbnSuffix.Text = R.Text.TXT_SUFFIX;
            RbnSuffix.UseVisualStyleBackColor = true;
            //
            // LblViewFamilyType
            //
            LblViewFamilyType.AutoSize = true;
            LblViewFamilyType.Location = new Point(9, 12);
            LblViewFamilyType.Name = "LblViewFamilyType";
            LblViewFamilyType.Size = new Size(86, 12);
            LblViewFamilyType.TabIndex = 3;
            LblViewFamilyType.Text = R.Text.LBL_ITEM_VIEW_TYPE;
            //
            // LblViewTemplate
            //
            LblViewTemplate.AutoSize = true;
            LblViewTemplate.Location = new Point(10, 60);
            LblViewTemplate.Name = "LblViewTemplate";
            LblViewTemplate.Size = new Size(85, 12);
            LblViewTemplate.TabIndex = 4;
            LblViewTemplate.Text = R.Text.LBL_ITEM_VIEW_TEMPLATE;
            //
            // CbxViewFamilyType
            //
            CbxViewFamilyType.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxViewFamilyType.FormattingEnabled = true;
            CbxViewFamilyType.Location = new Point(12, 27);
            CbxViewFamilyType.Name = "CbxViewFamilyType";
            CbxViewFamilyType.Size = new Size(236, 20);
            CbxViewFamilyType.Sorted = true;
            CbxViewFamilyType.TabIndex = 5;
            //
            // CbxViewTemplate
            //
            CbxViewTemplate.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxViewTemplate.FormattingEnabled = true;
            CbxViewTemplate.Location = new Point(12, 75);
            CbxViewTemplate.Name = "CbxViewTemplate";
            CbxViewTemplate.Size = new Size(236, 20);
            CbxViewTemplate.Sorted = true;
            CbxViewTemplate.TabIndex = 6;
            //
            // TbxAddFor
            //
            TbxAddFor.Location = new Point(6, 44);
            TbxAddFor.Name = "TbxAddFor";
            TbxAddFor.Size = new Size(225, 19);
            TbxAddFor.TabIndex = 8;
            //
            // gpAddFor
            //
            gpAddFor.Controls.Add(TbxAddFor);
            gpAddFor.Controls.Add(RbnPrefix);
            gpAddFor.Controls.Add(RbnSuffix);
            gpAddFor.Location = new Point(12, 110);
            gpAddFor.Name = "gpAddFor";
            gpAddFor.Size = new Size(237, 76);
            gpAddFor.TabIndex = 10;
            gpAddFor.TabStop = false;
            gpAddFor.Text = R.Text.GP_ADD_STRING;
            //
            // DlgViewDuplicateItem
            //
            AcceptButton = OK_Button;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            CancelButton = Cancel_Button;
            ClientSize = new Size(264, 240);
            Controls.Add(gpAddFor);
            Controls.Add(CbxViewTemplate);
            Controls.Add(CbxViewFamilyType);
            Controls.Add(LblViewTemplate);
            Controls.Add(LblViewFamilyType);
            Controls.Add(TableLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DlgViewDuplicateItem";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "DlgViewDuplicateItem";
            TableLayoutPanel1.ResumeLayout(false);
            gpAddFor.ResumeLayout(false);
            gpAddFor.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private TableLayoutPanel TableLayoutPanel1;
        private Button OK_Button;
        private Button Cancel_Button;
        private RadioButton RbnPrefix;
        private RadioButton RbnSuffix;
        private Label LblViewFamilyType;
        private Label LblViewTemplate;
        private ComboBox CbxViewFamilyType;
        private ComboBox CbxViewTemplate;
        private TextBox TbxAddFor;
        private GroupBox gpAddFor;
    }
}
