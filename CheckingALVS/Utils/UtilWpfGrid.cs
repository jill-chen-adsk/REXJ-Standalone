using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WpfDataGrid = System.Windows.Controls.DataGrid;
using WpfDataGridCheckBoxColumn = System.Windows.Controls.DataGridCheckBoxColumn;
using WpfDataGridColumn = System.Windows.Controls.DataGridColumn;
using WpfDataGridComboBoxColumn = System.Windows.Controls.DataGridComboBoxColumn;
using WpfDataGridLength = System.Windows.Controls.DataGridLength;
using WpfDataGridTextColumn = System.Windows.Controls.DataGridTextColumn;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
  public static class UtilWpfGrid
  {
    private const double HeaderCharWidth = 9.0;
    private const double HeaderHorizontalPadding = 40;
    private const double NumericColumnMinWidth = 80;

    /// <summary>
    /// Estimates a column width that fits the header text; optional minimum preserves data-cell space.
    /// </summary>
    public static double ResolveColumnWidth(string header, double minimumWidth = 0, bool numericColumn = false)
    {
      double floor = numericColumn ? NumericColumnMinWidth : 0;
      double headerWidth = 60;
      if (!string.IsNullOrEmpty(header))
      {
        var lines = header.Replace("\r\n", "\n").Split('\n');
        int maxChars = lines.Max(line => line.Length);
        headerWidth = maxChars * HeaderCharWidth + HeaderHorizontalPadding;
      }

      double resolved = Math.Max(headerWidth, floor);
      if (minimumWidth > 0)
        resolved = Math.Max(resolved, minimumWidth);

      return Math.Ceiling(resolved);
    }

    private static void ApplyFixedColumnWidth(WpfDataGridColumn column, double width)
    {
      column.Width = new WpfDataGridLength(width);
      column.MinWidth = width;
    }

    public static void PrepareGrid(WpfDataGrid grid)
    {
      grid.Columns.Clear();
      grid.AutoGenerateColumns = false;
      grid.CanUserResizeColumns = true;
      ScrollViewer.SetHorizontalScrollBarVisibility(grid, ScrollBarVisibility.Auto);
      ScrollViewer.SetVerticalScrollBarVisibility(grid, ScrollBarVisibility.Auto);
    }

    public static void AddTextColumn(
        WpfDataGrid grid,
        string bindingPath,
        string header,
        double width,
        bool visible,
        bool readOnly,
        TextAlignment alignment)
    {
      if (!visible)
        return;

      var binding = new System.Windows.Data.Binding(bindingPath);
      if (readOnly)
        binding.Mode = System.Windows.Data.BindingMode.OneWay;

      bool numericColumn = alignment == TextAlignment.Right;
      double resolvedWidth = ResolveColumnWidth(header, width, numericColumn);

      var column = new WpfDataGridTextColumn
      {
        Header = header?.Replace("\n", Environment.NewLine),
        Binding = binding,
        IsReadOnly = readOnly
      };

      ApplyFixedColumnWidth(column, resolvedWidth);

      if (grid.TryFindResource("Weave.DataGridTextBlock") is Style elementStyle)
        column.ElementStyle = elementStyle;

      if (!readOnly && grid.TryFindResource("Weave.DataGridTextBox") is Style editingStyle)
        column.EditingElementStyle = editingStyle;

      grid.Columns.Add(column);
    }

    public static void AddCheckColumn(
        WpfDataGrid grid,
        string bindingPath,
        string header,
        double width,
        bool visible,
        bool readOnly)
    {
      if (!visible)
        return;

      var binding = new System.Windows.Data.Binding(bindingPath);
      if (readOnly)
        binding.Mode = System.Windows.Data.BindingMode.OneWay;

      double resolvedWidth = ResolveColumnWidth(header, width > 0 ? width : 70);
      var column = new WpfDataGridCheckBoxColumn
      {
        Header = header,
        Binding = binding,
        IsReadOnly = readOnly
      };
      ApplyFixedColumnWidth(column, resolvedWidth);

      if (grid.TryFindResource("Weave.DataGridCheckBox") is Style elementStyle)
        column.ElementStyle = elementStyle;

      grid.Columns.Add(column);
    }

    public static void AddComboColumn(
        WpfDataGrid grid,
        string bindingPath,
        string header,
        double width,
        bool visible,
        IEnumerable itemsSource,
        string displayMemberPath,
        string selectedValuePath)
    {
      if (!visible)
        return;

      double resolvedWidth = ResolveColumnWidth(header, width > 0 ? width : 220);
      var column = new WpfDataGridComboBoxColumn
      {
        Header = header,
        ItemsSource = itemsSource,
        DisplayMemberPath = displayMemberPath,
        SelectedValuePath = selectedValuePath,
        IsReadOnly = false,
        SelectedValueBinding = new System.Windows.Data.Binding(bindingPath)
        {
          UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        }
      };
      ApplyFixedColumnWidth(column, resolvedWidth);

      if (grid.TryFindResource("Weave.DataGridComboBox") is Style comboStyle)
      {
        column.ElementStyle = comboStyle;
        column.EditingElementStyle = comboStyle;
      }

      grid.Columns.Add(column);
    }

    public static bool HasColumn(System.Data.DataTable table, string columnName)
    {
      return table != null && table.Columns.Contains(columnName);
    }
  }
}
