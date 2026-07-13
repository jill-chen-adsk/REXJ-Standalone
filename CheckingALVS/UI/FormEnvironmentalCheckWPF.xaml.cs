using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using ADSK.JExtRAC.CheckingALVS.Utils;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.UI
{
  public partial class FormEnvironmentalCheckWPF : Window, IWeaveChromeWindow
  {
    readonly RvtExtApp.Components.Attribute _cmpAttribute;
    readonly RvtExtApp.Entities.DtRoom _entDtRoom;
    readonly RvtExtApp.Entities.DtWinDoor _entDtWinDoor;
    readonly RvtExtApp.Entities.DtCmd _entDtCmd;
    readonly HashSet<string> _numericPartColumns = new HashSet<string>(StringComparer.Ordinal);
    EnvironmentalCheckCloseAction _closeAction = EnvironmentalCheckCloseAction.None;
    bool _suppressRoomSelectionChanged;
    bool _suppressTreeSelectionChanged;
    bool _suppressUseDistrictChanged;
    string _nothingGroupName;

    public FormEnvironmentalCheckWPF(
        RvtExtApp.Components.Attribute cmpAttribute,
        RvtExtApp.Entities.DtRoom entDtRoom,
        RvtExtApp.Entities.DtWinDoor entDtWinDoor,
        RvtExtApp.Entities.DtCmd entDtCmd)
    {
      InitializeComponent();

      _cmpAttribute = cmpAttribute;
      _entDtRoom = entDtRoom;
      _entDtWinDoor = entDtWinDoor;
      _entDtCmd = entDtCmd;
      _nothingGroupName = _cmpAttribute.ResourceText("IDS_TXT_NOTHING");

      string title = CheckingCommandTitles.GetCommandTitle(_cmpAttribute, _entDtRoom.CommandKind);
      WeaveTheme.Apply(this, this, title, () => Finish(EnvironmentalCheckCloseAction.Cancel));

      SetText();
      ConfigureGrids();
      SetData();

      btnCancel.Click += (_, __) => TryFinish(EnvironmentalCheckCloseAction.Cancel);
      btnClose.Click += (_, __) => TryFinish(EnvironmentalCheckCloseAction.Close);
      btnOutExcel.Click += (_, __) => TryFinish(EnvironmentalCheckCloseAction.Excel);
      btnSelectParts.Click += (_, __) => TryFinish(EnvironmentalCheckCloseAction.Ignore);

      ContentRendered += (_, __) => ApplyWinDoorDrawSelection();
    }

    public EnvironmentalCheckCloseAction CloseAction => _closeAction;

    public System.Windows.Controls.Border ChromeOuterBorder => chromeOuterBorder;
    public System.Windows.Controls.Grid ChromeTitleBar => chromeTitleBar;
    public System.Windows.Controls.Border ChromeDivider => chromeDivider;
    public System.Windows.Controls.TextBlock ChromeTitleText => chromeTitleText;
    public System.Windows.Controls.Button ChromeCloseButton => chromeCloseButton;

    public void PrepareForShow()
    {
      _closeAction = EnvironmentalCheckCloseAction.None;
      SetData();
    }

    void Finish(EnvironmentalCheckCloseAction action)
    {
      _closeAction = action;
      DialogResult = action == EnvironmentalCheckCloseAction.Cancel ? (bool?)false : true;
      Close();
    }

    void SetText()
    {
      lblSelectRoomGroup.Text = _cmpAttribute.ResourceText("IDS_TXT_SELECTEDROOMGROUP");
      lblRoomGroup.Text = _cmpAttribute.ResourceText("IDS_TXT_ROOMGROUP");
      btnAddGroup.Content = _cmpAttribute.ResourceText("IDS_TXT_ADD");
      btnDelGroup.Content = _cmpAttribute.ResourceText("IDS_TXT_DEL");
      btnEditRoomGroup.Content = _cmpAttribute.ResourceText("IDS_TXT_EDIT");

      lblSelectRoom.Text = _cmpAttribute.ResourceText("IDS_TXT_SELECTEDROOM");
      lblTotalRoomGroup.Text = _cmpAttribute.ResourceText("IDS_TXT_ROOMGROUPTOTAL");
      lblGroupNecessaryArea.Text = _cmpAttribute.ResourceText("IDS_TXT_NESAREA");
      lblGroupUsableArea.Text = _cmpAttribute.ResourceText("IDS_TXT_USABLEAREA");
      lblGroupJudgment.Text = _cmpAttribute.ResourceText("IDS_TXT_JUDGMENT");

      lblSelectParts.Text = _cmpAttribute.ResourceText("IDS_TXT_SELECTEDPARTS");

      gpbLightingTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_LIGHTING") + _cmpAttribute.ResourceText("IDS_TXT_CHANGEBULK");
      gpbVerandaTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_VERANDA");
      btnVeranda.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");
      gpbRoadSideTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_ROADSIDE");
      btnRoadSide.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");
      gpbHorizontalMeasTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_MEAS");
      btnHorizontalMeas.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");
      gpbHorizontalCorrTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_CORR");
      btnHorizontalCorr.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");
      gpbVerticalMeasTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_MEAS");
      btnVerticalMeas.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");
      gpbVerticalCorrTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_CORR");
      btnVerticalCorr.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");

      gpbSmokeTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_SMOKE") + _cmpAttribute.ResourceText("IDS_TXT_CHANGEBULK");
      gpbHeadHeightTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_UPPERMOSTSIDEHEIGHT");
      btnHeadHeight.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");
      gpbCeilingHeightTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_CEILINGHEIGHT");
      btnCeilingHeight.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");
      gpbSmokeWallLengthTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_SMOKEWALLLENGTH");
      btnSmokeWallLength.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");
      gpbUsableHeightSmokeTitle.Text = _cmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEHEIGHT");
      btnUsableHeightSmoke.Content = _cmpAttribute.ResourceText("IDS_TXT_UPDATE");

      btnSelectParts.Content = _cmpAttribute.ResourceText("IDS_TXT_SELECTPARTS");
      lblUseDistrict.Text = _cmpAttribute.ResourceText("IDS_TXT_USEDISTRICT");
      chkCreateHeader.Content = _cmpAttribute.ResourceText("IDS_TXT_CREATEHEADER");
      btnOutExcel.Content = _cmpAttribute.ResourceText("IDS_TXT_OUTEXCEL");
      btnClose.Content = _cmpAttribute.ResourceText("IDS_TXT_CLOSE");
      btnCancel.Content = _cmpAttribute.ResourceText("IDS_TXT_CANCEL");
    }

    void ConfigureGrids()
    {
      EnvironmentalCheckWpfGrids.ConfigureRoomGrid(dgvSelectRoom, _cmpAttribute, _entDtRoom);
      EnvironmentalCheckWpfGrids.ConfigurePartsGrid(dgvSelectParts, _cmpAttribute, _entDtWinDoor);
      RegisterNumericPartColumns();

      // Weave.DataGrid sets IsReadOnly=True; override so Room Type combo and opening edits work.
      dgvSelectRoom.IsReadOnly = false;
      dgvSelectParts.IsReadOnly = false;
    }

    void RegisterNumericPartColumns()
    {
      _numericPartColumns.Clear();
      if (_entDtWinDoor == null)
        return;

      switch (_entDtRoom.CommandKind)
      {
        case 0:
          _numericPartColumns.Add(_entDtWinDoor.ColNameDistHorizontalMeas);
          _numericPartColumns.Add(_entDtWinDoor.ColNameDistHorizontalCorr);
          _numericPartColumns.Add(_entDtWinDoor.ColNameHorizontalDist);
          _numericPartColumns.Add(_entDtWinDoor.ColNameDistVerticalMeas);
          _numericPartColumns.Add(_entDtWinDoor.ColNameDistVerticalCorr);
          _numericPartColumns.Add(_entDtWinDoor.ColNameVerticalDist);
          _numericPartColumns.Add(_entDtWinDoor.ColNameUsableWidth);
          _numericPartColumns.Add(_entDtWinDoor.ColNameUsableHeight);
          break;

        case 1:
          _numericPartColumns.Add(_entDtWinDoor.ColNameSmokeWinWidth);
          _numericPartColumns.Add(_entDtWinDoor.ColNameSmokeWinHeight);
          _numericPartColumns.Add(_entDtWinDoor.ColNameOpenCoefficient);
          _numericPartColumns.Add(_entDtWinDoor.ColNameHeadHeight);
          _numericPartColumns.Add(_entDtWinDoor.ColNameCeilingHeight);
          _numericPartColumns.Add(_entDtWinDoor.ColNameSmokeWallLength);
          _numericPartColumns.Add(_entDtWinDoor.ColNameUsableHeightSmoke);
          break;

        case 2:
          _numericPartColumns.Add(_entDtWinDoor.ColNameOpenCoefficient);
          _numericPartColumns.Add(_entDtWinDoor.ColNameUsableWidth);
          _numericPartColumns.Add(_entDtWinDoor.ColNameUsableHeight);
          break;
      }
    }

    void SetData()
    {
      if (_entDtRoom.CommandKind == 0)
      {
        System.Data.DataTable useDistrict = _entDtRoom.EntDtItems?.UseDistrict;
        if (useDistrict != null)
        {
          _suppressUseDistrictChanged = true;
          cboUseDistrict.ItemsSource = useDistrict.DefaultView;
          cboUseDistrict.DisplayMemberPath = "Name";
          cboUseDistrict.SelectedValuePath = "Name";
          if (cboUseDistrict.Items.Count > 0)
            cboUseDistrict.SelectedIndex = _entDtCmd.CvUseDistrictOpt;
          _suppressUseDistrictChanged = false;
        }
        else
        {
          cboUseDistrict.ItemsSource = null;
        }

        lblUseDistrict.Visibility = Visibility.Visible;
        cboUseDistrict.Visibility = Visibility.Visible;
      }
      else
      {
        lblUseDistrict.Visibility = Visibility.Collapsed;
        cboUseDistrict.Visibility = Visibility.Collapsed;
      }

      chkCreateHeader.IsChecked = _entDtCmd.CvChkCreateHeader;
      chkVeranda.IsChecked = _entDtCmd.CvVeranda;
      chkRoadSide.IsChecked = _entDtCmd.CvRoadSide;
      txtHorizontalMeas.Text = _entDtCmd.CvHorizontalMeas ?? string.Empty;
      txtHorizontalCorr.Text = _entDtCmd.CvHorizontalCorr ?? string.Empty;
      txtVerticalMeas.Text = _entDtCmd.CvVerticalMeas ?? string.Empty;
      txtVerticalCorr.Text = _entDtCmd.CvVerticalCorr ?? string.Empty;
      txtHeadHeight.Text = _entDtCmd.CvHeadHeight ?? string.Empty;
      txtCeilingHeight.Text = _entDtCmd.CvCeilingHeight ?? string.Empty;
      txtSmokeWallLength.Text = _entDtCmd.CvSmokeWallLength ?? string.Empty;
      txtUsableHeightSmoke.Text = _entDtCmd.CvUsableHeightSmoke ?? string.Empty;

      PopulateRoomGroupTree();

      if (_entDtRoom.Data != null)
        dgvSelectRoom.ItemsSource = _entDtRoom.Data.DefaultView;
      else
        dgvSelectRoom.ItemsSource = null;

      if (_entDtWinDoor.Data != null)
        dgvSelectParts.ItemsSource = _entDtWinDoor.Data.DefaultView;
      else
        dgvSelectParts.ItemsSource = null;

      switch (_entDtRoom.CommandKind)
      {
        case 0:
          gpbLighting.Visibility = Visibility.Visible;
          gpbSmoke.Visibility = Visibility.Collapsed;
          break;
        case 1:
          gpbLighting.Visibility = Visibility.Collapsed;
          gpbSmoke.Visibility = Visibility.Visible;
          break;
        default:
          gpbLighting.Visibility = Visibility.Collapsed;
          gpbSmoke.Visibility = Visibility.Collapsed;
          break;
      }

      if (_entDtRoom.Data != null && _entDtRoom.Data.Rows.Count > 0)
      {
        SelectInitialRoomGroup();
        RefreshRoomCalculations();
        _suppressRoomSelectionChanged = true;
        if (dgvSelectRoom.Items.Count > 0)
          dgvSelectRoom.SelectedIndex = 0;
        _suppressRoomSelectionChanged = false;
        if (dgvSelectRoom.SelectedItem is DataRowView initialRoom)
        {
          SyncTreeSelection(
              initialRoom[_entDtRoom.ColNameID]?.ToString(),
              initialRoom[_entDtRoom.ColNameGroupName]?.ToString());
        }
        SetPartsData();
      }
      else
      {
        ClearRoomGroupTotals();
      }

      btnSelectParts.IsEnabled = _entDtWinDoor.Data != null && dgvSelectParts.Items.Count > 0;
    }

    void PopulateRoomGroupTree()
    {
      trvRoomGroup.Items.Clear();
      if (_entDtRoom.Data == null)
        return;

      var groupNames = new List<string>();
      var groupRooms = new List<List<DataRow>>();

      foreach (DataRow rowRoom in _entDtRoom.Data.Rows)
      {
        string groupName = rowRoom[_entDtRoom.ColNameGroupName].ToString();
        int index = groupNames.IndexOf(groupName);
        if (index < 0)
        {
          groupNames.Add(groupName);
          groupRooms.Add(new List<DataRow> { rowRoom });
        }
        else
        {
          groupRooms[index].Add(rowRoom);
        }
      }

      foreach (string groupName in groupNames.OrderBy(name => name, StringComparer.CurrentCulture))
      {
        var groupItem = new TreeViewItem { Header = groupName };
        int groupIndex = groupNames.IndexOf(groupName);
        foreach (DataRow rowRoom in groupRooms[groupIndex])
        {
          string roomLabel = rowRoom[_entDtRoom.ColNameRoomName] + rowRoom[_entDtRoom.ColNameRoomNo].ToString();
          var roomItem = new TreeViewItem
          {
            Header = roomLabel,
            Tag = rowRoom[_entDtRoom.ColNameID].ToString()
          };
          groupItem.Items.Add(roomItem);
        }

        trvRoomGroup.Items.Add(groupItem);
      }
    }

    void SelectInitialRoomGroup()
    {
      if (trvRoomGroup.Items.Count == 0)
        return;

      _suppressTreeSelectionChanged = true;
      if (trvRoomGroup.Items[0] is TreeViewItem firstGroup)
      {
        firstGroup.IsSelected = true;
        firstGroup.IsExpanded = true;
        ApplyRoomGroupFilter(GetGroupNameFromTreeItem(firstGroup));
      }
      _suppressTreeSelectionChanged = false;
    }

    void TrvRoomGroup_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (_suppressTreeSelectionChanged)
        return;

      if (e.NewValue is not TreeViewItem selectedItem)
        return;

      string groupName = GetGroupNameFromTreeItem(selectedItem);
      ApplyRoomGroupFilter(groupName);

      if (IsRoomTreeItem(selectedItem))
      {
        SelectRoomInGrid(selectedItem.Tag?.ToString());
      }
      else if (dgvSelectRoom.Items.Count > 0)
      {
        _suppressRoomSelectionChanged = true;
        dgvSelectRoom.SelectedIndex = 0;
        _suppressRoomSelectionChanged = false;
        SetPartsData();
      }

      RefreshRoomCalculations();
      SetTotalRoomGroup(groupName);
    }

    void DgvSelectRoom_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (_suppressRoomSelectionChanged)
        return;

      SetPartsData();
      RefreshRoomCalculations();
      btnSelectParts.IsEnabled = dgvSelectParts.Items.Count > 0;

      if (dgvSelectRoom.SelectedItem is DataRowView roomRow)
      {
        string groupName = roomRow[_entDtRoom.ColNameGroupName]?.ToString();
        SetTotalRoomGroup(groupName);
        SyncTreeSelection(roomRow[_entDtRoom.ColNameID]?.ToString(), groupName);
      }
    }

    void BtnAddGroup_Click(object sender, RoutedEventArgs e)
    {
      var form = new RvtExtApp.Components.FormGroupName(_cmpAttribute);
      if (WeaveDialogHost.ShowWinFormsDialog(form, this) != System.Windows.Forms.DialogResult.OK)
        return;

      string groupName = form.GroupName;
      if (string.IsNullOrWhiteSpace(groupName) || GroupExistsInTree(groupName))
        return;

      trvRoomGroup.Items.Add(new TreeViewItem { Header = groupName });
      SortGroupTree();

      // Match WinForms: keep the room list visible by selecting the first group after sort.
      _suppressTreeSelectionChanged = true;
      if (trvRoomGroup.Items.Count > 0 && trvRoomGroup.Items[0] is TreeViewItem firstGroup)
      {
        firstGroup.IsSelected = true;
        firstGroup.IsExpanded = true;
        firstGroup.BringIntoView();
      }
      _suppressTreeSelectionChanged = false;

      if (trvRoomGroup.SelectedItem is TreeViewItem selectedGroup)
      {
        string selectedGroupName = GetGroupNameFromTreeItem(selectedGroup);
        ApplyRoomGroupFilter(selectedGroupName);
        SetTotalRoomGroup(selectedGroupName);
      }

      trvRoomGroup.Focus();
    }

    void BtnDelGroup_Click(object sender, RoutedEventArgs e)
    {
      if (trvRoomGroup.SelectedItem is not TreeViewItem selectedItem || !IsGroupTreeItem(selectedItem))
        return;

      string groupName = selectedItem.Header?.ToString();
      if (string.IsNullOrWhiteSpace(groupName) || groupName == _nothingGroupName)
        return;

      var roomIds = selectedItem.Items
          .OfType<TreeViewItem>()
          .Select(child => child.Tag?.ToString())
          .Where(roomId => !string.IsNullOrWhiteSpace(roomId))
          .ToList();

      if (roomIds.Count > 0)
      {
        _entDtRoom.AssignRoomsToGroup(roomIds, _nothingGroupName);
        RefreshRoomGridBinding();
      }

      trvRoomGroup.Items.Remove(selectedItem);
      PopulateRoomGroupTree();
      SelectInitialRoomGroup();
      RefreshRoomCalculations();
      if (trvRoomGroup.SelectedItem is TreeViewItem selectedGroup)
        SetTotalRoomGroup(GetGroupNameFromTreeItem(selectedGroup));
      trvRoomGroup.Focus();
    }

    void BtnEditRoomGroup_Click(object sender, RoutedEventArgs e)
    {
      if (trvRoomGroup.SelectedItem is not TreeViewItem selectedItem || !IsRoomTreeItem(selectedItem))
        return;

      var groupTree = BuildWinFormsGroupTree();
      var form = new RvtExtApp.Components.FormGroup(_cmpAttribute, groupTree.Nodes);
      if (WeaveDialogHost.ShowWinFormsDialog(form, this) != System.Windows.Forms.DialogResult.OK || form.Group == null)
        return;

      string roomId = selectedItem.Tag?.ToString();
      string targetGroupName = form.Group.Text;
      if (string.IsNullOrWhiteSpace(roomId) || !_entDtRoom.AssignRoomToGroup(roomId, targetGroupName))
        return;

      _suppressRoomSelectionChanged = true;
      _suppressTreeSelectionChanged = true;

      RefreshRoomGridBinding();
      PopulateRoomGroupTree();
      SelectGroupInTree(targetGroupName, roomId);
      ApplyRoomGroupFilter(targetGroupName);
      SetTotalRoomGroup(targetGroupName);
      SelectRoomInGrid(roomId);
      RefreshRoomCalculations();
      SetPartsData();

      _suppressTreeSelectionChanged = false;
      _suppressRoomSelectionChanged = false;
      trvRoomGroup.Focus();
    }

    System.Windows.Forms.TreeView BuildWinFormsGroupTree()
    {
      var treeView = new System.Windows.Forms.TreeView();
      foreach (TreeViewItem groupItem in trvRoomGroup.Items.OfType<TreeViewItem>())
      {
        treeView.Nodes.Add(groupItem.Header?.ToString() ?? string.Empty);
      }

      return treeView;
    }

    void RefreshRoomGridBinding()
    {
      dgvSelectRoom.ItemsSource = null;
      if (_entDtRoom.Data != null)
        dgvSelectRoom.ItemsSource = _entDtRoom.Data.DefaultView;
    }

    void ApplyRoomGroupFilter(string groupName)
    {
      if (string.IsNullOrWhiteSpace(groupName) || _entDtRoom.Data == null)
        return;

      _entDtRoom.SetVisbleRowsRoomsOfRoomGroup(groupName);
    }

    void SetTotalRoomGroup(string groupName)
    {
      string strGroupNecessaryArea = "";
      string strGroupUsableArea = "";
      string strGroupJudgment = "";

      _entDtRoom.TotalRoomGroup(groupName,
          ref strGroupNecessaryArea,
          ref strGroupUsableArea,
          ref strGroupJudgment);

      txtGroupNecessaryArea.Text = strGroupNecessaryArea;
      txtGroupUsableArea.Text = strGroupUsableArea;
      txtGroupJudgment.Text = strGroupJudgment;
    }

    void ClearRoomGroupTotals()
    {
      txtGroupNecessaryArea.Text = string.Empty;
      txtGroupUsableArea.Text = string.Empty;
      txtGroupJudgment.Text = string.Empty;
    }

    void RefreshRoomCalculations()
    {
      if (_entDtRoom.Data == null || _entDtWinDoor.Data == null)
        return;

      _entDtWinDoor.SetTotalUsableArea(_entDtRoom.Data, _entDtWinDoor.Data);
      _entDtRoom.SetJudgment(_entDtRoom.Data, _entDtWinDoor.Data);
    }

    bool GroupExistsInTree(string groupName)
    {
      return trvRoomGroup.Items
          .OfType<TreeViewItem>()
          .Any(item => string.Equals(item.Header?.ToString(), groupName, StringComparison.CurrentCulture));
    }

    void SortGroupTree()
    {
      var sortedGroups = trvRoomGroup.Items
          .OfType<TreeViewItem>()
          .OrderBy(item => item.Header?.ToString(), StringComparer.CurrentCulture)
          .ToList();

      trvRoomGroup.Items.Clear();
      foreach (TreeViewItem groupItem in sortedGroups)
        trvRoomGroup.Items.Add(groupItem);
    }

    void SelectGroupInTree(string groupName, string roomId)
    {
      _suppressTreeSelectionChanged = true;
      foreach (TreeViewItem groupItem in trvRoomGroup.Items.OfType<TreeViewItem>())
      {
        if (!string.Equals(groupItem.Header?.ToString(), groupName, StringComparison.CurrentCulture))
          continue;

        groupItem.IsExpanded = true;
        if (!string.IsNullOrWhiteSpace(roomId))
        {
          foreach (TreeViewItem roomItem in groupItem.Items.OfType<TreeViewItem>())
          {
            if (roomItem.Tag?.ToString() != roomId)
              continue;

            roomItem.IsSelected = true;
            roomItem.BringIntoView();
            _suppressTreeSelectionChanged = false;
            return;
          }
        }

        groupItem.IsSelected = true;
        groupItem.BringIntoView();
        break;
      }
      _suppressTreeSelectionChanged = false;
    }

    void SyncTreeSelection(string roomId, string groupName)
    {
      if (string.IsNullOrWhiteSpace(groupName))
        return;

      _suppressTreeSelectionChanged = true;
      foreach (TreeViewItem groupItem in trvRoomGroup.Items.OfType<TreeViewItem>())
      {
        if (!string.Equals(groupItem.Header?.ToString(), groupName, StringComparison.CurrentCulture))
          continue;

        groupItem.IsExpanded = true;
        if (!string.IsNullOrWhiteSpace(roomId))
        {
          foreach (TreeViewItem roomItem in groupItem.Items.OfType<TreeViewItem>())
          {
            if (roomItem.Tag?.ToString() != roomId)
              continue;

            roomItem.IsSelected = true;
            roomItem.BringIntoView();
            _suppressTreeSelectionChanged = false;
            return;
          }
        }

        groupItem.IsSelected = true;
        break;
      }
      _suppressTreeSelectionChanged = false;
    }

    void SelectRoomInGrid(string roomId)
    {
      if (string.IsNullOrWhiteSpace(roomId))
        return;

      _suppressRoomSelectionChanged = true;
      foreach (object item in dgvSelectRoom.Items)
      {
        if (item is not DataRowView rowView)
          continue;

        if (rowView[_entDtRoom.ColNameID]?.ToString() != roomId)
          continue;

        dgvSelectRoom.SelectedItem = item;
        dgvSelectRoom.ScrollIntoView(item);
        break;
      }
      _suppressRoomSelectionChanged = false;
      SetPartsData();
      RefreshRoomCalculations();
    }

    static bool IsGroupTreeItem(TreeViewItem item) => item.Parent is not TreeViewItem;

    static bool IsRoomTreeItem(TreeViewItem item) => item.Parent is TreeViewItem;

    static string GetGroupNameFromTreeItem(TreeViewItem item)
    {
      if (item.Parent is TreeViewItem parentGroup)
        return parentGroup.Header?.ToString() ?? string.Empty;

      return item.Header?.ToString() ?? string.Empty;
    }

    void SetPartsData()
    {
      if (_entDtWinDoor.Data == null)
        return;

      if (dgvSelectRoom.SelectedItem is not DataRowView roomRow)
        return;

      if (!int.TryParse(roomRow[_entDtRoom.ColNameID]?.ToString(), out int roomId))
        return;

      _entDtWinDoor.SetVisbleWinDoor(_entDtWinDoor.Data, roomId);
      AppendExcludedPartsFilter();

      if (_entDtRoom.CommandKind == 0 && cboUseDistrict.SelectedIndex > -1)
        ApplyUseDistrictToVisibleParts(cboUseDistrict.SelectedIndex);
    }

    void ApplyUseDistrictToVisibleParts(int useDistrictOpt)
    {
      string alpha = "-";
      string beta = "-";
      string dValue = "-";
      _entDtWinDoor.GetUseDistrictValue(useDistrictOpt, ref alpha, ref beta, ref dValue);

      foreach (object item in dgvSelectParts.Items)
      {
        if (item is not DataRowView rowView)
          continue;

        rowView[_entDtWinDoor.ColNameA] = alpha;
        rowView[_entDtWinDoor.ColNameB] = beta;
        rowView[_entDtWinDoor.ColNameD] = dValue;
      }
    }

    void CboUseDistrict_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (_suppressUseDistrictChanged || _entDtRoom.CommandKind != 0)
        return;

      if (cboUseDistrict.SelectedIndex < 0 || dgvSelectRoom.Items.Count == 0)
        return;

      object selectedRoom = dgvSelectRoom.SelectedItem;
      _suppressRoomSelectionChanged = true;
      foreach (object item in dgvSelectRoom.Items)
      {
        dgvSelectRoom.SelectedItem = item;
        SetPartsData();
      }

      if (selectedRoom != null)
        dgvSelectRoom.SelectedItem = selectedRoom;
      _suppressRoomSelectionChanged = false;

      SetPartsData();
    }

    void DgvSelectRoom_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
      if (e.EditAction != DataGridEditAction.Commit)
        return;

      if (GetColumnName(dgvSelectRoom, e.Column) != _entDtRoom.ColNameRoomKind)
        return;

      string newRoomKind = null;
      if (e.EditingElement is System.Windows.Controls.ComboBox combo)
        newRoomKind = combo.SelectedValue?.ToString();

      int rowIndex = e.Row.GetIndex();
      Dispatcher.BeginInvoke(
          DispatcherPriority.ApplicationIdle,
          new Action(() => UpdateSelectRoom(rowIndex, newRoomKind)));
    }

    void DgvSelectParts_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
      if (e.EditAction != DataGridEditAction.Commit)
        return;

      string colName = GetColumnName(dgvSelectParts, e.Column);
      if (string.IsNullOrWhiteSpace(colName))
        return;

      int rowIndex = e.Row.GetIndex();
      if (colName == _entDtWinDoor.ColNameVeranda || colName == _entDtWinDoor.ColNameRoadSide)
      {
        Dispatcher.BeginInvoke(
            DispatcherPriority.ApplicationIdle,
            new Action(() => UpdateSelectParts(rowIndex, string.Empty)));
        return;
      }

      if (!_numericPartColumns.Contains(colName))
        return;

      if (e.EditingElement is System.Windows.Controls.TextBox textBox &&
          !ValidateNumericValue(textBox.Text, false, out string errorMessage))
      {
        e.Cancel = true;
        System.Windows.MessageBox.Show(this, errorMessage, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      Dispatcher.BeginInvoke(
          DispatcherPriority.ApplicationIdle,
          new Action(() => UpdateSelectParts(rowIndex, colName)));
    }

    void UpdateSelectRoom(int rowIndex, string explicitRoomKind = null)
    {
      if (rowIndex < 0 || rowIndex >= dgvSelectRoom.Items.Count)
        return;

      if (dgvSelectRoom.Items[rowIndex] is not DataRowView rowView)
        return;

      string roomId = rowView[_entDtRoom.ColNameID]?.ToString();
      if (string.IsNullOrWhiteSpace(roomId))
        return;

      // Prefer the explicitly captured kind (from the ComboBox editing element)
      // over reading from the DataRow which may still be in a proposed/stale state.
      string roomKind = explicitRoomKind ?? rowView[_entDtRoom.ColNameRoomKind]?.ToString() ?? string.Empty;
      string coeff = _entDtRoom.GetNesCoeff(roomKind);
      rowView[_entDtRoom.ColNameNecessaryCoefficient] = coeff;

      // Compute NecessaryArea directly from rowView values to avoid any DataRow
      // version ambiguity — GetNesArea(roomId) would read from _Data which may
      // not yet reflect the coefficient we just wrote.
      string area = rowView[_entDtRoom.ColNameArea]?.ToString() ?? "0";
      string nesAreaValue = _entDtRoom.GetNesArea(area, coeff);
      rowView[_entDtRoom.ColNameNecessaryArea] = nesAreaValue;

      // Recompute judgment using the values we just wrote
      string nesArea = nesAreaValue ?? "-";
      string totalUsable = rowView[_entDtRoom.ColNameTotalUsableArea]?.ToString() ?? "-";
      rowView[_entDtRoom.ColNameJudgment] = _entDtRoom.GetJudgment(nesArea, totalUsable);

      string groupName = rowView[_entDtRoom.ColNameGroupName]?.ToString();
      SetTotalRoomGroup(groupName);
    }

    void UpdateSelectParts(int rowIndex, string colName)
    {
      if (rowIndex < 0 || rowIndex >= dgvSelectParts.Items.Count)
        return;

      if (dgvSelectParts.Items[rowIndex] is not DataRowView rowView)
        return;

      string partsId = rowView[_entDtWinDoor.ColNameID]?.ToString();
      if (string.IsNullOrWhiteSpace(partsId))
        return;

      rowView[_entDtWinDoor.ColNameHorizontalDist] = _entDtWinDoor.GetDistHoriOrVert(
          rowView[_entDtWinDoor.ColNameDistHorizontalMeas]?.ToString(),
          rowView[_entDtWinDoor.ColNameDistHorizontalCorr]?.ToString());
      rowView[_entDtWinDoor.ColNameVerticalDist] = _entDtWinDoor.GetDistHoriOrVert(
          rowView[_entDtWinDoor.ColNameDistVerticalMeas]?.ToString(),
          rowView[_entDtWinDoor.ColNameDistVerticalCorr]?.ToString());
      rowView[_entDtWinDoor.ColNameDsH] = _entDtWinDoor.GetDsH(partsId);
      rowView[_entDtWinDoor.ColNameATemp] = _entDtWinDoor.GetAtempValue(partsId);
      rowView[_entDtWinDoor.ColNameACorr] = _entDtWinDoor.GetACorrValue(partsId);

      if (colName == _entDtWinDoor.ColNameHeadHeight ||
          colName == _entDtWinDoor.ColNameCeilingHeight ||
          colName == _entDtWinDoor.ColNameSmokeWinHeight ||
          colName == _entDtWinDoor.ColNameSmokeWallLength)
      {
        _entDtWinDoor.SetUsableHeightSmoke(partsId);
      }

      switch (_entDtWinDoor.CommandKind)
      {
        case 0:
          rowView[_entDtWinDoor.ColNameUsableOpenArea] = _entDtWinDoor.GetUsableOpenArea(partsId);
          rowView[_entDtWinDoor.ColNameUsableArea] = _entDtWinDoor.GetUsableArea(partsId);
          break;

        case 1:
        case 2:
          rowView[_entDtWinDoor.ColNameUsableArea] = _entDtWinDoor.GetUsableArea(partsId);
          break;
      }

      _entDtWinDoor.SetUsableArea(partsId);

      RefreshRoomCalculations();

      string roomId = rowView[_entDtWinDoor.ColNameAffiliationRoom]?.ToString();
      SetTotalRoomGroup(_entDtRoom.GetGroupName(roomId));
    }

    void BtnVeranda_Click(object sender, RoutedEventArgs e)
    {
      bool value = chkVeranda.IsChecked == true;
      ApplyBulkToSelectedParts(rowView =>
      {
        rowView[_entDtWinDoor.ColNameVeranda] = value;
        UpdateSelectParts(GetPartRowIndex(rowView), string.Empty);
      });
    }

    void BtnRoadSide_Click(object sender, RoutedEventArgs e)
    {
      bool value = chkRoadSide.IsChecked == true;
      ApplyBulkToSelectedParts(rowView =>
      {
        rowView[_entDtWinDoor.ColNameRoadSide] = value;
        UpdateSelectParts(GetPartRowIndex(rowView), string.Empty);
      });
    }

    void BtnHorizontalMeas_Click(object sender, RoutedEventArgs e) =>
        ApplyBulkNumeric(txtHorizontalMeas, _entDtWinDoor.ColNameDistHorizontalMeas);

    void BtnHorizontalCorr_Click(object sender, RoutedEventArgs e) =>
        ApplyBulkNumeric(txtHorizontalCorr, _entDtWinDoor.ColNameDistHorizontalCorr);

    void BtnVerticalMeas_Click(object sender, RoutedEventArgs e) =>
        ApplyBulkNumeric(txtVerticalMeas, _entDtWinDoor.ColNameDistVerticalMeas);

    void BtnVerticalCorr_Click(object sender, RoutedEventArgs e) =>
        ApplyBulkNumeric(txtVerticalCorr, _entDtWinDoor.ColNameDistVerticalCorr);

    void BtnHeadHeight_Click(object sender, RoutedEventArgs e) =>
        ApplyBulkNumeric(txtHeadHeight, _entDtWinDoor.ColNameHeadHeight);

    void BtnCeilingHeight_Click(object sender, RoutedEventArgs e) =>
        ApplyBulkNumeric(txtCeilingHeight, _entDtWinDoor.ColNameCeilingHeight);

    void BtnSmokeWallLength_Click(object sender, RoutedEventArgs e) =>
        ApplyBulkNumeric(txtSmokeWallLength, _entDtWinDoor.ColNameSmokeWallLength);

    void BtnUsableHeightSmoke_Click(object sender, RoutedEventArgs e) =>
        ApplyBulkNumeric(txtUsableHeightSmoke, _entDtWinDoor.ColNameUsableHeightSmoke);

    void BulkNumericTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      if (sender is not System.Windows.Controls.TextBox textBox)
        return;

      ValidateNumericValue(textBox.Text, false, out _);
    }

    void ApplyBulkNumeric(System.Windows.Controls.TextBox source, string columnName)
    {
      if (!ValidateNumericValue(source.Text, false, out string errorMessage))
      {
        System.Windows.MessageBox.Show(this, errorMessage, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      string value = source.Text;
      int updatedCount = 0;
      ApplyBulkToSelectedParts(rowView =>
      {
        rowView[columnName] = value;
        UpdateSelectParts(GetPartRowIndex(rowView), columnName);
        updatedCount++;
      });

      if (updatedCount == 0)
      {
        System.Windows.MessageBox.Show(
            this,
            "No openings are visible for the selected room.",
            Title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
      }
    }

    void ApplyBulkToSelectedParts(Action<DataRowView> apply)
    {
      var targets = new List<DataRowView>();

      if (dgvSelectParts.SelectedItems.Count > 0)
      {
        foreach (object item in dgvSelectParts.SelectedItems)
        {
          if (item is DataRowView rowView)
            targets.Add(rowView);
        }
      }
      else if (dgvSelectParts.SelectedItem is DataRowView single)
      {
        targets.Add(single);
      }
      else
      {
        foreach (object item in dgvSelectParts.Items)
        {
          if (item is DataRowView rowView)
            targets.Add(rowView);
        }
      }

      foreach (DataRowView rowView in targets)
        apply(rowView);
    }

    int GetPartRowIndex(DataRowView rowView) => dgvSelectParts.Items.IndexOf(rowView);

    static string GetColumnName(System.Windows.Controls.DataGrid grid, DataGridColumn column)
    {
      if (column == null)
        return string.Empty;

      if (column is DataGridBoundColumn boundColumn &&
          boundColumn.Binding is System.Windows.Data.Binding binding &&
          !string.IsNullOrWhiteSpace(binding.Path?.Path))
      {
        return binding.Path.Path;
      }

      if (column is DataGridComboBoxColumn comboColumn &&
          comboColumn.SelectedValueBinding is System.Windows.Data.Binding comboBinding &&
          !string.IsNullOrWhiteSpace(comboBinding.Path?.Path))
      {
        return comboBinding.Path.Path;
      }

      return column.SortMemberPath ?? string.Empty;
    }

    bool ValidateNumericValue(string value, bool allowEmpty, out string errorMessage)
    {
      errorMessage = _entDtRoom.SetErrPvdNumeric(value?.Trim() ?? string.Empty, allowEmpty);
      return string.IsNullOrEmpty(errorMessage);
    }

    bool ValidateSmokeWallLength(string value, out string errorMessage)
    {
      errorMessage = _entDtWinDoor.SetErrPvdSmokeWallLength(value?.Trim() ?? string.Empty);
      return string.IsNullOrEmpty(errorMessage);
    }

    void TryFinish(EnvironmentalCheckCloseAction action)
    {
      if (action == EnvironmentalCheckCloseAction.Cancel)
      {
        Finish(action);
        return;
      }

      if (!CheckError())
        return;

      if (!ValidateBulkTextBoxes())
        return;

      if (!CheckChangedUsableDim())
        return;

      Finish(action);
    }

    bool CheckError()
    {
      foreach (object item in dgvSelectParts.Items)
      {
        if (item is not DataRowView rowView)
          continue;

        foreach (string columnName in _numericPartColumns)
        {
          if (!rowView.Row.Table.Columns.Contains(columnName))
            continue;

          string value = rowView[columnName]?.ToString() ?? string.Empty;
          if (!ValidateNumericValue(value, false, out string errorMessage))
          {
            System.Windows.MessageBox.Show(this, errorMessage, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
          }
        }

        if (_entDtRoom.CommandKind == 1 &&
            rowView.Row.Table.Columns.Contains(_entDtWinDoor.ColNameSmokeWallLength))
        {
          string smokeWall = rowView[_entDtWinDoor.ColNameSmokeWallLength]?.ToString() ?? string.Empty;
          if (!ValidateSmokeWallLength(smokeWall, out string smokeError))
          {
            System.Windows.MessageBox.Show(this, smokeError, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
          }
        }
      }

      SaveCommandSettings();
      return true;
    }

    bool ValidateBulkTextBoxes()
    {
      var fields = new (System.Windows.Controls.TextBox TextBox, bool AllowEmpty)[]
      {
        (txtHorizontalMeas, false),
        (txtHorizontalCorr, false),
        (txtVerticalMeas, false),
        (txtVerticalCorr, false),
        (txtHeadHeight, false),
        (txtCeilingHeight, false),
        (txtSmokeWallLength, false),
        (txtUsableHeightSmoke, false)
      };

      foreach ((System.Windows.Controls.TextBox textBox, bool allowEmpty) in fields)
      {
        if (textBox.Visibility != Visibility.Visible && !IsDescendantOfVisiblePanel(textBox))
          continue;

        if (!ValidateNumericValue(textBox.Text, allowEmpty, out string errorMessage))
        {
          System.Windows.MessageBox.Show(this, errorMessage, Title, MessageBoxButton.OK, MessageBoxImage.Warning);
          textBox.Focus();
          return false;
        }
      }

      return true;
    }

    static bool IsDescendantOfVisiblePanel(System.Windows.Controls.TextBox textBox)
    {
      DependencyObject current = textBox;
      while (current != null)
      {
        if (current is UIElement { Visibility: Visibility.Collapsed })
          return false;

        current = LogicalTreeHelper.GetParent(current);
      }

      return true;
    }

    bool CheckChangedUsableDim()
    {
      DataTable changeData = _entDtWinDoor.GetPartsChangedUsableDim();
      if (changeData == null || changeData.Rows.Count == 0)
        return true;

      var form = new RvtExtApp.Components.FormChangedUsableDim(_cmpAttribute, _entDtWinDoor, changeData);
      return WeaveDialogHost.ShowWinFormsDialog(form, this) != System.Windows.Forms.DialogResult.Cancel;
    }

    void SaveCommandSettings()
    {
      if (_entDtRoom.CommandKind == 0 && cboUseDistrict.SelectedIndex > -1)
        _entDtCmd.CvUseDistrictOpt = cboUseDistrict.SelectedIndex;

      _entDtCmd.CvChkCreateHeader = chkCreateHeader.IsChecked == true;
      _entDtCmd.CvVeranda = chkVeranda.IsChecked == true;
      _entDtCmd.CvRoadSide = chkRoadSide.IsChecked == true;
      _entDtCmd.CvHorizontalMeas = txtHorizontalMeas.Text;
      _entDtCmd.CvHorizontalCorr = txtHorizontalCorr.Text;
      _entDtCmd.CvVerticalMeas = txtVerticalMeas.Text;
      _entDtCmd.CvVerticalCorr = txtVerticalCorr.Text;
      _entDtCmd.CvHeadHeight = txtHeadHeight.Text;
      _entDtCmd.CvCeilingHeight = txtCeilingHeight.Text;
      _entDtCmd.CvSmokeWallLength = txtSmokeWallLength.Text;
      _entDtCmd.CvUsableHeightSmoke = txtUsableHeightSmoke.Text;
    }

    void AppendExcludedPartsFilter()
    {
      if (_entDtWinDoor.Data == null)
        return;

      string filter = _entDtWinDoor.Data.DefaultView.RowFilter;
      string excludeClause = _entDtWinDoor.ColNameAffiliationRoom + " <> '-1'";
      if (string.IsNullOrWhiteSpace(filter))
        _entDtWinDoor.Data.DefaultView.RowFilter = excludeClause;
      else if (!filter.Contains(excludeClause))
        _entDtWinDoor.Data.DefaultView.RowFilter = "(" + filter + ") AND " + excludeClause;
    }

    void ApplyWinDoorDrawSelection()
    {
      if (_entDtWinDoor.WinDoorFromDraw == null || _entDtWinDoor.WinDoorFromDraw.Count == 0)
        return;

      dgvSelectParts.SelectedItems.Clear();
      foreach (object item in dgvSelectParts.Items)
      {
        if (item is not DataRowView rowView)
          continue;

        if (!int.TryParse(rowView[_entDtWinDoor.ColNameID]?.ToString(), out int id))
          continue;

        if (_entDtWinDoor.WinDoorFromDraw.Contains(id))
          dgvSelectParts.SelectedItems.Add(item);
      }

      if (dgvSelectParts.SelectedItem != null)
        dgvSelectParts.ScrollIntoView(dgvSelectParts.SelectedItem);
    }
  }
}
