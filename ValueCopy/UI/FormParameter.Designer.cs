namespace ADSK.JExtRAC.ValueCopy.UI
{
    partial class FormParameter
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
            this.dgvPropetives = new System.Windows.Forms.DataGridView();
            this.btApply = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.dgvCbkGroup = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvCbkParameter = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dgvPropetivesName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvPropetivesValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPropetives)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPropetives
            // 
            this.dgvPropetives.AllowUserToAddRows = false;
            this.dgvPropetives.AllowUserToDeleteRows = false;
            this.dgvPropetives.AllowUserToResizeColumns = false;
            this.dgvPropetives.AllowUserToResizeRows = false;
            this.dgvPropetives.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPropetives.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPropetives.ColumnHeadersVisible = false;
            this.dgvPropetives.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvCbkGroup,
            this.dgvCbkParameter,
            this.dgvPropetivesName,
            this.dgvPropetivesValue});
            this.dgvPropetives.Location = new System.Drawing.Point(8, 7);
            this.dgvPropetives.Name = "dgvPropetives";
            this.dgvPropetives.RowHeadersVisible = false;
            this.dgvPropetives.Size = new System.Drawing.Size(455, 465);
            this.dgvPropetives.TabIndex = 0;
            this.dgvPropetives.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPropetives_CellClick);
            this.dgvPropetives.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPropetives_CellDoubleClick);
            this.dgvPropetives.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPropetives_CellEndEdit);
            this.dgvPropetives.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvPropetives_CellValueChanged);
            this.dgvPropetives.CurrentCellDirtyStateChanged += new System.EventHandler(this.dgvPropetives_CurrentCellDirtyStateChanged);
            // 
            // btApply
            // 
            this.btApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btApply.Location = new System.Drawing.Point(295, 480);
            this.btApply.Name = "btApply";
            this.btApply.Size = new System.Drawing.Size(75, 21);
            this.btApply.TabIndex = 1;
            this.btApply.Text = "btApply";
            this.btApply.UseVisualStyleBackColor = true;
            this.btApply.Click += new System.EventHandler(this.btApply_Click);
            // 
            // btCancel
            // 
            this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(383, 480);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 21);
            this.btCancel.TabIndex = 1;
            this.btCancel.Text = "btCancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // dgvCbkGroup
            // 
            this.dgvCbkGroup.HeaderText = "dgvCbkGroup";
            this.dgvCbkGroup.MinimumWidth = 35;
            this.dgvCbkGroup.Name = "dgvCbkGroup";
            this.dgvCbkGroup.ReadOnly = true;
            this.dgvCbkGroup.ThreeState = true;
            this.dgvCbkGroup.Width = 35;
            // 
            // dgvCbkParameter
            // 
            this.dgvCbkParameter.HeaderText = "dgvCbkParameter";
            this.dgvCbkParameter.MinimumWidth = 35;
            this.dgvCbkParameter.Name = "dgvCbkParameter";
            this.dgvCbkParameter.ReadOnly = true;
            this.dgvCbkParameter.Width = 35;
            // 
            // dgvPropetivesName
            // 
            this.dgvPropetivesName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvPropetivesName.HeaderText = "dgvPropetivesName";
            this.dgvPropetivesName.Name = "dgvPropetivesName";
            this.dgvPropetivesName.ReadOnly = true;
            this.dgvPropetivesName.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvPropetivesName.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dgvPropetivesValue
            // 
            this.dgvPropetivesValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvPropetivesValue.HeaderText = "dgvPropetivesValue";
            this.dgvPropetivesValue.Name = "dgvPropetivesValue";
            this.dgvPropetivesValue.ReadOnly = true;
            // 
            // FormParameter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 510);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btApply);
            this.Controls.Add(this.dgvPropetives);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormParameter";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormParameter";
            this.Load += new System.EventHandler(this.FormParameter_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPropetives)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvPropetives;
        private System.Windows.Forms.Button btApply;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvCbkGroup;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dgvCbkParameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvPropetivesName;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvPropetivesValue;
    }
}