using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace ADSK.JExtRAC.AutoLayoutTag.Entities
{
    /// ================================================================================
    /// <summary>Custom Datagridview</summary>
    /// ================================================================================
    public class DataGridViewDisableCheckboxCell : DataGridViewCheckBoxCell
    {
        // Member variable

        #region Member Variables

        /// <summary>enable cell</summary>
        private bool enabledValue;

        #endregion Member Variables

        // Constructor

        #region Constructor

        /// ================================================================================
        /// <summary>Constructor</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public DataGridViewDisableCheckboxCell()
        {
            this.enabledValue = true;
        }

        #endregion Constructor

        // Properties

        #region Properties

        /// ================================================================================
        /// <summary>Category</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        public bool Enabled
        {
            get
            {
                return enabledValue;
            }
            set
            {
                enabledValue = value;
            }
        }

        #endregion Properties

        //Member function

        #region Member function

        /// ================================================================================
        /// <summary>Clone</summary>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        // Override the Clone method so that the Enabled property is copied.
        public override object Clone()
        {
            DataGridViewDisableCheckboxCell cell =
                (DataGridViewDisableCheckboxCell)base.Clone();
            cell.Enabled = this.Enabled;
            return cell;
        }

        // ================================================================================
        /// <summary>Paint cell</summary>
        ///
        /// <param name="graphics">graphics</param>
        /// <param name="clipBounds">Rectangle</param>
        /// <param name="cellBounds">Rectangle</param>
        /// <param name="rowIndex">rowIndex</param>
        /// <param name="elementState">elementState</param>
        /// <param name="value">value</param>
        /// <param name="formattedValue">formattedValue</param>
        /// <param name="errorText">errorText</param>
        /// <param name="cellStyle">cellStyle</param>
        /// <param name="advancedBorderStyle">advancedBorderStyle</param>
        /// <param name="paintParts">paintParts</param>
        ///
        /// <history>2021/11/29 Created Applied Technology</history>
        /// ================================================================================
        protected override void Paint(Graphics graphics,
                                      System.Drawing.Rectangle clipBounds,
                                      System.Drawing.Rectangle cellBounds,
                                      int rowIndex,
                                      DataGridViewElementStates elementState,
                                      object value,
                                      object formattedValue,
                                      string errorText,
                                      DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle,
                                      DataGridViewPaintParts paintParts)
        {
            // The button cell is disabled, so paint the border,
            // background, and disabled button for the cell.
            if (!this.enabledValue)
            {
                // Draw the cell background, if specified.
                if ((paintParts & DataGridViewPaintParts.Background) == DataGridViewPaintParts.Background)
                {
                    SolidBrush cellBackground = new SolidBrush(cellStyle.BackColor);
                    graphics.FillRectangle(cellBackground, cellBounds);
                    cellBackground.Dispose();
                }

                // Draw the cell borders, if specified.
                if ((paintParts & DataGridViewPaintParts.Border) == DataGridViewPaintParts.Border)
                    PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);

                // Calculate the area in which to draw the button.
                System.Drawing.Rectangle buttonArea = cellBounds;
                System.Drawing.Rectangle buttonAdjustment = this.BorderWidths(advancedBorderStyle);
                buttonArea.X += buttonAdjustment.X;
                buttonArea.Y += buttonAdjustment.Y;
                buttonArea.Height -= buttonAdjustment.Height;
                buttonArea.Width -= buttonAdjustment.Width;
                Point point = new Point(cellBounds.X + cellBounds.Width / 2 - 9, cellBounds.Y + cellBounds.Height / 2 - 9);
                // Draw the disabled button.
                CheckState checkState = CheckState.Unchecked;
                CheckBoxState state = checkState == CheckState.Checked ? CheckBoxState.CheckedDisabled : CheckBoxState.UncheckedDisabled;
                Size size = CheckBoxRenderer.GetGlyphSize(graphics, state);

                Point center = new Point(cellBounds.X, cellBounds.Y);

                center.X += (cellBounds.Width - size.Width) / 2;

                center.Y += (cellBounds.Height - size.Height) / 2;

                CheckBoxRenderer.DrawCheckBox(graphics, center, System.Windows.Forms.VisualStyles.CheckBoxState.UncheckedDisabled);
            }
            else
            {
                // The button cell is enabled, so let the base class
                // handle the painting.
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }

        #endregion Member function
    }
}