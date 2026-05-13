namespace ADSK.JExtRAC.AreaSchedule.LegalArea
{
  partial class FormWarningRooms
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.dgvRooms = new System.Windows.Forms.DataGridView();
      this.RoomName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.RoomNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.RevitArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.LegalArea = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.dgvRooms)).BeginInit();
      this.SuspendLayout();
      // 
      // btnOK
      // 
      this.btnOK.Location = new System.Drawing.Point(265, 269);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(80, 23);
      this.btnOK.TabIndex = 0;
      this.btnOK.Text = "btnOK";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(351, 269);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(80, 23);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "btnCancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // dgvRooms
      // 
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dgvRooms.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dgvRooms.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvRooms.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RoomName,
            this.RoomNumber,
            this.RevitArea,
            this.LegalArea});
      this.dgvRooms.Location = new System.Drawing.Point(12, 12);
      this.dgvRooms.Name = "dgvRooms";
      this.dgvRooms.ReadOnly = true;
      this.dgvRooms.RowHeadersVisible = false;
      this.dgvRooms.RowTemplate.Height = 21;
      this.dgvRooms.Size = new System.Drawing.Size(419, 251);
      this.dgvRooms.TabIndex = 2;
      // 
      // RoomName
      // 
      this.RoomName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      this.RoomName.DefaultCellStyle = dataGridViewCellStyle2;
      this.RoomName.HeaderText = "RoomName";
      this.RoomName.Name = "RoomName";
      this.RoomName.ReadOnly = true;
      // 
      // RoomNumber
      // 
      this.RoomNumber.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      this.RoomNumber.DefaultCellStyle = dataGridViewCellStyle3;
      this.RoomNumber.HeaderText = "RoomNumber";
      this.RoomNumber.Name = "RoomNumber";
      this.RoomNumber.ReadOnly = true;
      // 
      // RevitArea
      // 
      this.RevitArea.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      this.RevitArea.DefaultCellStyle = dataGridViewCellStyle4;
      this.RevitArea.HeaderText = "RevitArea";
      this.RevitArea.Name = "RevitArea";
      this.RevitArea.ReadOnly = true;
      // 
      // LegalArea
      // 
      this.LegalArea.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
      this.LegalArea.DefaultCellStyle = dataGridViewCellStyle5;
      this.LegalArea.HeaderText = "LegalArea";
      this.LegalArea.Name = "LegalArea";
      this.LegalArea.ReadOnly = true;
      // 
      // FormWarningRooms
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(446, 299);
      this.Controls.Add(this.dgvRooms);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOK);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormWarningRooms";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "FormWarningRooms";
      this.Load += new System.EventHandler(this.FormWarningRooms_Load);
      ((System.ComponentModel.ISupportInitialize)(this.dgvRooms)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.DataGridView dgvRooms;
    private System.Windows.Forms.DataGridViewTextBoxColumn RoomName;
    private System.Windows.Forms.DataGridViewTextBoxColumn RoomNumber;
    private System.Windows.Forms.DataGridViewTextBoxColumn RevitArea;
    private System.Windows.Forms.DataGridViewTextBoxColumn LegalArea;
  }
}