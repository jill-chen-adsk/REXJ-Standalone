using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ADSK.JExtRAC.SwitchJoinOrder.UI
{
    internal static class PriorityListActions
    {
        public static bool IsItemDoubleClick(ListView list, MouseButtonEventArgs e)
        {
            return e.OriginalSource is DependencyObject source
                && ItemsControl.ContainerFromElement(list, source) is ListViewItem;
        }

        // Moves every selected row one step (direction -1 = up, +1 = down) and keeps the selection.
        // Rows already packed against the edge stay put; swapModel(a, b) mirrors each swap in the data model.
        public static bool MoveSelected(
            ListView list,
            ObservableCollection<PriorityListRow> rows,
            int direction,
            Action<int, int> swapModel)
        {
            var selected = list.SelectedItems.Cast<PriorityListRow>().ToList();
            var found = selected.Select(rows.IndexOf).Where(i => i >= 0);
            var indexes = (direction < 0 ? found.OrderBy(i => i) : found.OrderByDescending(i => i)).ToList();
            if (indexes.Count == 0)
                return false;

            int edge = direction < 0 ? 0 : rows.Count - 1;
            bool moved = false;
            foreach (int idx in indexes)
            {
                if (idx == edge)
                {
                    edge -= direction;
                    continue;
                }

                swapModel(idx, idx + direction);
                rows.Move(idx, idx + direction);
                moved = true;
            }

            list.SelectedItems.Clear();
            foreach (var row in selected)
                list.SelectedItems.Add(row);
            list.ScrollIntoView(direction < 0 ? selected.OrderBy(rows.IndexOf).First() : selected.OrderBy(rows.IndexOf).Last());
            return moved;
        }
    }
}
