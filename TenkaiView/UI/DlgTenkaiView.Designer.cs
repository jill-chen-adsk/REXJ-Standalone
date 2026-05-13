using R = ADSK.ViewExtension.TenkaiView.Resources;

namespace ADSK.ViewExtension.TenkaiView.UI
{
    partial class DlgTenkaiView
    {
        private System.ComponentModel.IContainer components = null;

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
            components = new System.ComponentModel.Container();
            TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            OK_Button = new System.Windows.Forms.Button();
            Cancel_Button = new System.Windows.Forms.Button();
            GroupBox4 = new System.Windows.Forms.GroupBox();
            numUPDScale = new System.Windows.Forms.NumericUpDown();
            Label5 = new System.Windows.Forms.Label();
            Label1 = new System.Windows.Forms.Label();
            cbxViewType = new System.Windows.Forms.ComboBox();
            GroupBox2 = new System.Windows.Forms.GroupBox();
            rbnTrimLevel = new System.Windows.Forms.RadioButton();
            rbnTrimVol = new System.Windows.Forms.RadioButton();
            GroupBox1 = new System.Windows.Forms.GroupBox();
            tbxTB = new System.Windows.Forms.TextBox();
            lblTB = new System.Windows.Forms.Label();
            tbxLR = new System.Windows.Forms.TextBox();
            lblLR = new System.Windows.Forms.Label();
            GroupBox3 = new System.Windows.Forms.GroupBox();
            Button1 = new System.Windows.Forms.Button();
            btnSelAll = new System.Windows.Forms.Button();
            chbxRooms = new System.Windows.Forms.CheckedListBox();
            GroupBox5 = new System.Windows.Forms.GroupBox();
            Label4 = new System.Windows.Forms.Label();
            cbxDimLevel = new System.Windows.Forms.ComboBox();
            Label2 = new System.Windows.Forms.Label();
            cbxDimGrid = new System.Windows.Forms.ComboBox();
            ToolTip1 = new System.Windows.Forms.ToolTip(components);
            TableLayoutPanel1.SuspendLayout();
            GroupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(numUPDScale)).BeginInit();
            GroupBox2.SuspendLayout();
            GroupBox1.SuspendLayout();
            GroupBox3.SuspendLayout();
            GroupBox5.SuspendLayout();
            SuspendLayout();
            //
            // TableLayoutPanel1
            //
            TableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            TableLayoutPanel1.ColumnCount = 2;
            TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TableLayoutPanel1.Controls.Add(OK_Button, 0, 0);
            TableLayoutPanel1.Controls.Add(Cancel_Button, 1, 0);
            TableLayoutPanel1.Location = new System.Drawing.Point(433, 311);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 1;
            TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            TableLayoutPanel1.Size = new System.Drawing.Size(146, 27);
            TableLayoutPanel1.TabIndex = 0;
            //
            // OK_Button
            //
            OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            OK_Button.Location = new System.Drawing.Point(3, 3);
            OK_Button.Name = "OK_Button";
            OK_Button.Size = new System.Drawing.Size(67, 21);
            OK_Button.TabIndex = 0;
            OK_Button.Text = "OK";
            //
            // Cancel_Button
            //
            Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Cancel_Button.Location = new System.Drawing.Point(76, 3);
            Cancel_Button.Name = "Cancel_Button";
            Cancel_Button.Size = new System.Drawing.Size(67, 21);
            Cancel_Button.TabIndex = 1;
            Cancel_Button.Text = R.Text.DLG_CANCEL;
            //
            // GroupBox4
            //
            GroupBox4.Controls.Add(numUPDScale);
            GroupBox4.Controls.Add(Label5);
            GroupBox4.Controls.Add(Label1);
            GroupBox4.Controls.Add(cbxViewType);
            GroupBox4.Location = new System.Drawing.Point(12, 12);
            GroupBox4.Name = "GroupBox4";
            GroupBox4.Size = new System.Drawing.Size(337, 73);
            GroupBox4.TabIndex = 2;
            GroupBox4.TabStop = false;
            GroupBox4.Text = R.Text.GRP_VIEWSETTINGS;
            //
            // numUPDScale
            //
            numUPDScale.Increment = new decimal(new int[] { 5, 0, 0, 0 });
            numUPDScale.Location = new System.Drawing.Point(250, 44);
            numUPDScale.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            numUPDScale.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            numUPDScale.Name = "numUPDScale";
            numUPDScale.Size = new System.Drawing.Size(77, 19);
            numUPDScale.TabIndex = 2;
            numUPDScale.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            ToolTip1.SetToolTip(numUPDScale, R.Text.TIP_SCALE);
            numUPDScale.Value = new decimal(new int[] { 100, 0, 0, 0 });
            //
            // Label5
            //
            Label5.AutoSize = true;
            Label5.Location = new System.Drawing.Point(179, 46);
            Label5.Name = "Label5";
            Label5.Size = new System.Drawing.Size(29, 12);
            Label5.TabIndex = 1;
            Label5.Text = R.Text.LBL_SCALE;
            //
            // Label1
            //
            Label1.AutoSize = true;
            Label1.Location = new System.Drawing.Point(6, 21);
            Label1.Name = "Label1";
            Label1.Size = new System.Drawing.Size(57, 12);
            Label1.TabIndex = 1;
            Label1.Text = R.Text.LBL_VIEWTYPE;
            //
            // cbxViewType
            //
            cbxViewType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxViewType.FormattingEnabled = true;
            cbxViewType.Location = new System.Drawing.Point(77, 18);
            cbxViewType.Name = "cbxViewType";
            cbxViewType.Size = new System.Drawing.Size(250, 20);
            cbxViewType.TabIndex = 0;
            ToolTip1.SetToolTip(cbxViewType, R.Text.TIP_VIEWTYPE);
            //
            // GroupBox2
            //
            GroupBox2.Controls.Add(rbnTrimLevel);
            GroupBox2.Controls.Add(rbnTrimVol);
            GroupBox2.Location = new System.Drawing.Point(14, 91);
            GroupBox2.Name = "GroupBox2";
            GroupBox2.Size = new System.Drawing.Size(335, 50);
            GroupBox2.TabIndex = 3;
            GroupBox2.TabStop = false;
            GroupBox2.Text = R.Text.GRP_TRIMBASE;
            //
            // rbnTrimLevel
            //
            rbnTrimLevel.AutoSize = true;
            rbnTrimLevel.Location = new System.Drawing.Point(225, 23);
            rbnTrimLevel.Name = "rbnTrimLevel";
            rbnTrimLevel.Size = new System.Drawing.Size(52, 16);
            rbnTrimLevel.TabIndex = 1;
            rbnTrimLevel.TabStop = true;
            rbnTrimLevel.Text = R.Text.RBN_TRIMLEVEL;
            rbnTrimLevel.UseVisualStyleBackColor = true;
            //
            // rbnTrimVol
            //
            rbnTrimVol.AutoSize = true;
            rbnTrimVol.Location = new System.Drawing.Point(76, 23);
            rbnTrimVol.Name = "rbnTrimVol";
            rbnTrimVol.Size = new System.Drawing.Size(102, 16);
            rbnTrimVol.TabIndex = 0;
            rbnTrimVol.TabStop = true;
            rbnTrimVol.Text = R.Text.RBN_TRIMVOL;
            ToolTip1.SetToolTip(rbnTrimVol, R.Text.TIP_TRIMVOL);
            rbnTrimVol.UseVisualStyleBackColor = true;
            //
            // GroupBox1
            //
            GroupBox1.Controls.Add(tbxTB);
            GroupBox1.Controls.Add(lblTB);
            GroupBox1.Controls.Add(tbxLR);
            GroupBox1.Controls.Add(lblLR);
            GroupBox1.Location = new System.Drawing.Point(12, 147);
            GroupBox1.Name = "GroupBox1";
            GroupBox1.Size = new System.Drawing.Size(335, 50);
            GroupBox1.TabIndex = 4;
            GroupBox1.TabStop = false;
            GroupBox1.Text = R.Text.GRP_CROPEXTEND;
            //
            // tbxTB
            //
            tbxTB.Location = new System.Drawing.Point(250, 24);
            tbxTB.Name = "tbxTB";
            tbxTB.Size = new System.Drawing.Size(75, 19);
            tbxTB.TabIndex = 3;
            tbxTB.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            ToolTip1.SetToolTip(tbxTB, R.Text.TIP_TOPBOTTOM);
            //
            // lblTB
            //
            lblTB.AutoSize = true;
            lblTB.Location = new System.Drawing.Point(179, 27);
            lblTB.Name = "lblTB";
            lblTB.Size = new System.Drawing.Size(55, 12);
            lblTB.TabIndex = 2;
            lblTB.Text = R.Text.LBL_TOPBOTTOM;
            //
            // tbxLR
            //
            tbxLR.Location = new System.Drawing.Point(75, 24);
            tbxLR.Name = "tbxLR";
            tbxLR.Size = new System.Drawing.Size(75, 19);
            tbxLR.TabIndex = 1;
            tbxLR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            ToolTip1.SetToolTip(tbxLR, R.Text.TIP_LEFTRIGHT);
            //
            // lblLR
            //
            lblLR.AutoSize = true;
            lblLR.Location = new System.Drawing.Point(6, 27);
            lblLR.Name = "lblLR";
            lblLR.Size = new System.Drawing.Size(55, 12);
            lblLR.TabIndex = 0;
            lblLR.Text = R.Text.LBL_LEFTRIGHT;
            //
            // GroupBox3
            //
            GroupBox3.Controls.Add(Button1);
            GroupBox3.Controls.Add(btnSelAll);
            GroupBox3.Controls.Add(chbxRooms);
            GroupBox3.Location = new System.Drawing.Point(355, 12);
            GroupBox3.Name = "GroupBox3";
            GroupBox3.Size = new System.Drawing.Size(222, 292);
            GroupBox3.TabIndex = 1;
            GroupBox3.TabStop = false;
            GroupBox3.Text = R.Text.GRP_ROOMS;
            //
            // Button1
            //
            Button1.Location = new System.Drawing.Point(87, 258);
            Button1.Name = "Button1";
            Button1.Size = new System.Drawing.Size(75, 23);
            Button1.TabIndex = 2;
            Button1.Text = R.Text.BTN_CLEARALL;
            Button1.UseVisualStyleBackColor = true;
            //
            // btnSelAll
            //
            btnSelAll.Location = new System.Drawing.Point(6, 258);
            btnSelAll.Name = "btnSelAll";
            btnSelAll.Size = new System.Drawing.Size(75, 23);
            btnSelAll.TabIndex = 1;
            btnSelAll.Text = R.Text.BTN_SELECTALL;
            btnSelAll.UseVisualStyleBackColor = true;
            //
            // chbxRooms
            //
            chbxRooms.CheckOnClick = true;
            chbxRooms.FormattingEnabled = true;
            chbxRooms.Location = new System.Drawing.Point(6, 18);
            chbxRooms.Name = "chbxRooms";
            chbxRooms.Size = new System.Drawing.Size(204, 228);
            chbxRooms.TabIndex = 0;
            ToolTip1.SetToolTip(chbxRooms, R.Text.TIP_ROOMS);
            //
            // GroupBox5
            //
            GroupBox5.Controls.Add(Label4);
            GroupBox5.Controls.Add(cbxDimLevel);
            GroupBox5.Controls.Add(Label2);
            GroupBox5.Controls.Add(cbxDimGrid);
            GroupBox5.Location = new System.Drawing.Point(12, 203);
            GroupBox5.Name = "GroupBox5";
            GroupBox5.Size = new System.Drawing.Size(337, 101);
            GroupBox5.TabIndex = 5;
            GroupBox5.TabStop = false;
            GroupBox5.Text = R.Text.GRP_DIMSTYLE;
            //
            // Label4
            //
            Label4.AutoSize = true;
            Label4.Location = new System.Drawing.Point(6, 47);
            Label4.Name = "Label4";
            Label4.Size = new System.Drawing.Size(34, 12);
            Label4.TabIndex = 2;
            Label4.Text = R.Text.LBL_DIMLEVEL;
            //
            // cbxDimLevel
            //
            cbxDimLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxDimLevel.FormattingEnabled = true;
            cbxDimLevel.Location = new System.Drawing.Point(77, 44);
            cbxDimLevel.Name = "cbxDimLevel";
            cbxDimLevel.Size = new System.Drawing.Size(250, 20);
            cbxDimLevel.TabIndex = 3;
            //
            // Label2
            //
            Label2.AutoSize = true;
            Label2.Location = new System.Drawing.Point(6, 21);
            Label2.Name = "Label2";
            Label2.Size = new System.Drawing.Size(37, 12);
            Label2.TabIndex = 0;
            Label2.Text = R.Text.LBL_DIMGRID;
            //
            // cbxDimGrid
            //
            cbxDimGrid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cbxDimGrid.FormattingEnabled = true;
            cbxDimGrid.Location = new System.Drawing.Point(77, 18);
            cbxDimGrid.Name = "cbxDimGrid";
            cbxDimGrid.Size = new System.Drawing.Size(250, 20);
            cbxDimGrid.TabIndex = 1;
            //
            // DlgTenkaiView
            //
            AcceptButton = OK_Button;
            AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            CancelButton = Cancel_Button;
            ClientSize = new System.Drawing.Size(591, 349);
            Controls.Add(GroupBox5);
            Controls.Add(GroupBox3);
            Controls.Add(GroupBox2);
            Controls.Add(GroupBox1);
            Controls.Add(GroupBox4);
            Controls.Add(TableLayoutPanel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DlgTenkaiView";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "dlgTenkaiView";
            TableLayoutPanel1.ResumeLayout(false);
            GroupBox4.ResumeLayout(false);
            GroupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(numUPDScale)).EndInit();
            GroupBox2.ResumeLayout(false);
            GroupBox2.PerformLayout();
            GroupBox1.ResumeLayout(false);
            GroupBox1.PerformLayout();
            GroupBox3.ResumeLayout(false);
            GroupBox5.ResumeLayout(false);
            GroupBox5.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        private System.Windows.Forms.Button OK_Button;
        private System.Windows.Forms.Button Cancel_Button;
        private System.Windows.Forms.GroupBox GroupBox4;
        private System.Windows.Forms.Label Label1;
        private System.Windows.Forms.ComboBox cbxViewType;
        private System.Windows.Forms.GroupBox GroupBox2;
        private System.Windows.Forms.RadioButton rbnTrimLevel;
        private System.Windows.Forms.RadioButton rbnTrimVol;
        private System.Windows.Forms.GroupBox GroupBox1;
        private System.Windows.Forms.TextBox tbxTB;
        private System.Windows.Forms.Label lblTB;
        private System.Windows.Forms.TextBox tbxLR;
        private System.Windows.Forms.Label lblLR;
        private System.Windows.Forms.GroupBox GroupBox3;
        private System.Windows.Forms.CheckedListBox chbxRooms;
        private System.Windows.Forms.Button btnSelAll;
        private System.Windows.Forms.Button Button1;
        private System.Windows.Forms.GroupBox GroupBox5;
        private System.Windows.Forms.Label Label2;
        private System.Windows.Forms.ComboBox cbxDimGrid;
        private System.Windows.Forms.Label Label4;
        private System.Windows.Forms.ComboBox cbxDimLevel;
        private System.Windows.Forms.NumericUpDown numUPDScale;
        private System.Windows.Forms.Label Label5;
        private System.Windows.Forms.ToolTip ToolTip1;
    }
}
