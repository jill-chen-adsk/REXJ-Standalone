using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using ADSK.JExtRAC.ParameterFilter.Components;
using ADSK.JExtRAC.ParameterFilter.Entities;
using ADSK.JExtRAC.ParameterFilter.Utils;
using Autodesk.Revit.DB;
using Revit = Autodesk.Revit;
using RvtExtApp = ADSK.JExtRAC.ParameterFilter;

namespace ADSK.JExtRAC.ParameterFilter.UI
{
    public sealed class ParameterFilterTabState
    {
        public int SelectedTypes { get; set; }
        public int SelectedObjects { get; set; }
        public bool SelectAllEnabled { get; set; }
        public bool ClearEnabled { get; set; }
    }

    public sealed class ParameterFilterEngine
    {
        private readonly Revit.UI.UIDocument _rvtUIDoc;
        private readonly RvtExtApp.Components.Attribute _cmpAttribute;
        private readonly RvtExtApp.Components.Elements _cmpElements;
        public List<ObjectElement> ObjectElements { get; }
        private List<ObjectSelectGroup> _lstObjectSelectedGroup;
        private int _currentIndexTab;

        public IntPtr OwnerHandle { get; set; }
        public bool SelectConnectChecked { get; set; }

        public ObservableCollection<FilterRowItem> CategoryItems { get; } = new();
        public ObservableCollection<FilterRowItem> FamilyItems { get; } = new();
        public ObservableCollection<FilterRowItem> FamilyTypeItems { get; } = new();
        public ObservableCollection<ParameterFilterRow> ParameterRows { get; } = new();

        public int CurrentTabIndex => _currentIndexTab;
        public bool CanGoPrevious => _currentIndexTab > 0;
        public bool CanGoNext => _currentIndexTab < 3;

        public event EventHandler TabChanged;
        public event EventHandler StateChanged;

        public ParameterFilterEngine(
            Revit.UI.UIDocument rvtUIDoc,
            RvtExtApp.Components.Attribute cmpAttribute,
            RvtExtApp.Components.Elements cmpElements,
            List<ObjectElement> objectElements)
        {
            _rvtUIDoc = rvtUIDoc;
            _cmpAttribute = cmpAttribute;
            _cmpElements = cmpElements;
            ObjectElements = objectElements;
            _lstObjectSelectedGroup = new List<ObjectSelectGroup>();
            SetData();
        }

        public void CompleteInitialLoad()
        {
            var orderedCategories = CategoryItems.OrderBy(x => x.Name).ToList();
            CategoryItems.Clear();
            foreach (var item in orderedCategories)
                CategoryItems.Add(item);

            foreach (var item in CategoryItems)
                item.IsChecked = true;

            var catState = GetCategoryTabState();
            if (CategoryItems.Count < 1)
            {
                catState.SelectAllEnabled = false;
                catState.ClearEnabled = false;
            }
            else
            {
                catState.SelectAllEnabled = false;
                catState.ClearEnabled = true;
            }

            RaiseStateChanged();
        }

        public void GoNext()
        {
            if (_currentIndexTab >= 3)
                return;

            _currentIndexTab += 1;

            if (_currentIndexTab == 1)
                FilterCategory_Family();
            else if (_currentIndexTab == 2)
                FilterCategory_Family_Type();
            else if (_currentIndexTab == 3)
                FilterCategory_Family_Type_Parameter();

            RaiseTabChanged();
            RaiseStateChanged();
        }

        public void GoPrevious()
        {
            if (_currentIndexTab <= 0)
                return;

            _currentIndexTab -= 1;
            RaiseTabChanged();
            RaiseStateChanged();
        }

        public void SetTabIndex(int index)
        {
            if (index == _currentIndexTab)
                return;
            // Wizard blocks direct tab change
        }

        public bool UpdateSelection()
        {
            try
            {
                switch (_currentIndexTab)
                {
                    case 0:
                        SelectElementByItems(CategoryItems);
                        return true;
                    case 1:
                        SelectElementByItems(FamilyItems);
                        return true;
                    case 2:
                        SelectElementByItems(FamilyTypeItems);
                        return true;
                    case 3:
                        return SelectElementParameter();
                    default:
                        return true;
                }
            }
            catch (Exception)
            {
                ShowWeaveMessage(_cmpAttribute.ResourceText("IDS_ERR_COMMAND"), _cmpAttribute.ResourceText("IDS_ERR_ERROR"));
                return false;
            }
        }

        public void SelectAllCategory()
        {
            foreach (var row in CategoryItems)
                row.IsChecked = true;
            RefreshCategoryTabState();
        }

        public void ClearCategory()
        {
            if (CategoryItems.All(r => !r.IsChecked))
                return;
            foreach (var row in CategoryItems)
                row.IsChecked = false;
            RefreshCategoryTabState();
        }

        public void OnCategoryCheckChanged() => RefreshCategoryTabState();

        public ParameterFilterTabState GetCategoryTabState()
        {
            var checkedRows = CategoryItems.Where(r => r.IsChecked).ToList();
            var state = new ParameterFilterTabState
            {
                SelectedTypes = checkedRows.Count,
                SelectedObjects = checkedRows.Sum(r => r.Count)
            };
            if (checkedRows.Count > 0 && checkedRows.Count != CategoryItems.Count)
            {
                state.SelectAllEnabled = true;
                state.ClearEnabled = true;
            }
            else if (checkedRows.Count == 0)
            {
                state.SelectAllEnabled = true;
                state.ClearEnabled = false;
            }
            else
            {
                state.SelectAllEnabled = false;
                state.ClearEnabled = true;
            }

            return state;
        }

        public void RefreshCategoryTabState() => RaiseStateChanged();

        public void SelectAllFamily()
        {
            foreach (var row in FamilyItems)
                row.IsChecked = true;
            RefreshFamilyTabState();
        }

        public void ClearFamily()
        {
            if (FamilyItems.All(r => !r.IsChecked))
                return;
            foreach (var row in FamilyItems)
                row.IsChecked = false;
            RefreshFamilyTabState();
        }

        public void OnFamilyCheckChanged() => RefreshFamilyTabState();

        public ParameterFilterTabState GetFamilyTabState()
        {
            var checkedRows = FamilyItems.Where(r => r.IsChecked).ToList();
            var state = new ParameterFilterTabState
            {
                SelectedTypes = checkedRows.Count,
                SelectedObjects = checkedRows.Sum(r => r.Count)
            };
            if (checkedRows.Count > 0 && checkedRows.Count != FamilyItems.Count)
            {
                state.SelectAllEnabled = true;
                state.ClearEnabled = true;
            }
            else if (checkedRows.Count == 0)
            {
                state.SelectAllEnabled = true;
                state.ClearEnabled = false;
            }
            else
            {
                state.SelectAllEnabled = false;
                state.ClearEnabled = true;
            }

            return state;
        }

        public void RefreshFamilyTabState() => RaiseStateChanged();

        public void SelectAllFamilyType()
        {
            foreach (var row in FamilyTypeItems)
                row.IsChecked = true;
            RefreshFamilyTypeTabState();
        }

        public void ClearFamilyType()
        {
            if (FamilyTypeItems.All(r => !r.IsChecked))
                return;
            foreach (var row in FamilyTypeItems)
                row.IsChecked = false;
            RefreshFamilyTypeTabState();
        }

        public void OnFamilyTypeCheckChanged() => RefreshFamilyTypeTabState();

        public ParameterFilterTabState GetFamilyTypeTabState()
        {
            var checkedRows = FamilyTypeItems.Where(r => r.IsChecked).ToList();
            var state = new ParameterFilterTabState
            {
                SelectedTypes = checkedRows.Count,
                SelectedObjects = checkedRows.Sum(r => r.Count)
            };
            if (checkedRows.Count > 0 && checkedRows.Count != FamilyTypeItems.Count)
            {
                state.SelectAllEnabled = true;
                state.ClearEnabled = true;
            }
            else if (checkedRows.Count == 0)
            {
                state.SelectAllEnabled = true;
                state.ClearEnabled = false;
            }
            else
            {
                state.SelectAllEnabled = false;
                state.ClearEnabled = true;
            }

            return state;
        }

        public void RefreshFamilyTypeTabState() => RaiseStateChanged();

        public void SelectAllParameter()
        {
            foreach (var row in ParameterRows)
                row.IsChecked = true;
            RefreshParameterTabState();
        }

        public void ClearParameter()
        {
            if (ParameterRows.Where(r => r.IsVisible).All(r => !r.IsChecked))
                return;
            foreach (var row in ParameterRows)
                row.IsChecked = false;
            RefreshParameterTabState();
        }

        public void OnParameterCheckChanged() => RefreshParameterTabState();

        public ParameterFilterTabState GetParameterTabState()
        {
            var state = new ParameterFilterTabState();
            SetCountParameter(out int numberRow, out int totalCount);
            state.SelectedTypes = numberRow;
            state.SelectedObjects = totalCount;

            var checkedRows = ParameterRows.Where(r => r.IsVisible && r.IsChecked).ToList();
            var totalRow = ParameterRows.Where(r => r.IsVisible).ToList();

            if (checkedRows.Count > 0 && checkedRows.Count != totalRow.Count)
            {
                state.SelectAllEnabled = true;
                state.ClearEnabled = true;
            }
            else if (checkedRows.Count == 0 && totalRow.Count == 0)
            {
                state.SelectAllEnabled = false;
                state.ClearEnabled = false;
            }
            else if (checkedRows.Count == 0)
            {
                state.SelectAllEnabled = true;
                state.ClearEnabled = false;
            }
            else
            {
                state.SelectAllEnabled = false;
                state.ClearEnabled = true;
            }

            return state;
        }

        public void RefreshParameterTabState() => RaiseStateChanged();

        public void OpenParameterGroupDialog(Window ownerWindow = null)
        {
            if (_lstObjectSelectedGroup.Count == 0)
                _lstObjectSelectedGroup = _cmpElements.GetAllGroupTypeElement_(ObjectElements);

            var dlg = new FormParameterGroupWPF(_cmpAttribute, _lstObjectSelectedGroup);
            if (WeaveDialogHost.ShowDialog(dlg, ownerWindow, OwnerHandle) != true)
                return;

            var lstStrSelectedGroup = dlg.LstGroupAllProject.Where(x => x.IsSelected).Select(x => x.GroupTypeId).ToList();
            SetSelectedGroupParameter(lstStrSelectedGroup);
            FilterCategory_Family_Type_Parameter();
            RaiseStateChanged();
        }

        public void FilterCategory_Family()
        {
            FamilyItems.Clear();

            foreach (var row in CategoryItems)
            {
                if (!row.IsChecked || row.Tag == null)
                    continue;

                var groupByFamily = row.Tag.GroupBy(x => x.FamilyNameElement);
                foreach (var objectFamily in groupByFamily)
                {
                    if (string.IsNullOrEmpty(objectFamily.Key))
                        continue;

                    FamilyItems.Add(new FilterRowItem
                    {
                        IsChecked = true,
                        SubName = row.Name,
                        Name = objectFamily.Key,
                        Count = objectFamily.Count(),
                        Tag = objectFamily.ToList()
                    });
                }
            }

            var famState = GetFamilyTabState();
            if (FamilyItems.Count < 1)
            {
                famState.SelectAllEnabled = false;
                famState.ClearEnabled = false;
            }
            else
            {
                famState.SelectAllEnabled = false;
                famState.ClearEnabled = true;
            }

            RaiseStateChanged();
        }

        public void FilterCategory_Family_Type()
        {
            FamilyTypeItems.Clear();

            foreach (var row in FamilyItems)
            {
                if (!row.IsChecked || row.Tag == null)
                    continue;

                var groupByType = row.Tag.GroupBy(x => x.TypeNameElement);
                foreach (var objectFamilyType in groupByType)
                {
                    if (string.IsNullOrEmpty(objectFamilyType.Key))
                        continue;

                    FamilyTypeItems.Add(new FilterRowItem
                    {
                        IsChecked = true,
                        SubName = row.SubName,
                        Name = objectFamilyType.Key,
                        Count = objectFamilyType.Count(),
                        Tag = objectFamilyType.ToList()
                    });
                }
            }

            RaiseStateChanged();
        }

        public void FilterCategory_Family_Type_Parameter()
        {
            ParameterRows.Clear();

            if (_lstObjectSelectedGroup.Count == 0)
                _lstObjectSelectedGroup = _cmpElements.GetAllGroupTypeElement_(ObjectElements);

            foreach (var row in FamilyTypeItems)
            {
                if (!row.IsChecked || row.Tag == null)
                    continue;

                var lstObjElement = row.Tag
                    .OrderBy(x => x.CategoriesElement)
                    .ThenBy(x => x.FamilyNameElement)
                    .ThenBy(x => x.TypeNameElement)
                    .ToList();

                foreach (var ele in lstObjElement)
                    ele.ObjectLengths = ele.ObjectLengths.OrderBy(y => y.NameParameterLength).ToList();

                var query = lstObjElement.SelectMany(x => x.ObjectLengths, (ele, objPara) => new { ele.ElementCurrent, objPara });
                var groupByName = query.GroupBy(x => x.objPara.NameParameterLength);
                string guidSameElement = Guid.NewGuid().ToString();
                var lstIndexRowAdded = new List<int>();

                foreach (var objElement in groupByName)
                {
                    int indexAdded = -1;
                    try
                    {
                        if (objElement.Count() == 0)
                        {
                            var paraRow = new ParameterFilterRow
                            {
                                IsChecked = true,
                                Category = row.SubName,
                                FamilyType = row.Name,
                                ParameterName = string.Empty,
                                CountDisplay = lstObjElement.Count.ToString(),
                                TypeKey = guidSameElement,
                                LengthParameters = lstObjElement.SelectMany(x => x.ObjectLengths).ToList()
                            };
                            ParameterRows.Add(paraRow);
                            indexAdded = ParameterRows.Count - 1;
                            break;
                        }

                        var groupValue = objElement.GroupBy(x => x.objPara.LengthVal);

                        if (groupValue.Count() == 1)
                        {
                            int count = objElement.Select(x => x.ElementCurrent.Id).GroupBy(x => x).Select(y => y.First()).Count();
                            var lstObjPara = new List<ObjectLengthParameter>();
                            foreach (var item in objElement)
                            {
                                if (lstObjPara.Any(x => x.ElementCurrent.Id == item.objPara.ElementCurrent.Id) == false)
                                    lstObjPara.Add(item.objPara);
                            }

                            var singleLengthMm = groupValue.First().Key;
                            var paraRow = new ParameterFilterRow
                            {
                                IsChecked = true,
                                Category = row.SubName,
                                FamilyType = row.Name,
                                ParameterName = objElement.FirstOrDefault().objPara.NameParameterLength,
                                Value = FormatLengthMillimeters(singleLengthMm),
                                CountDisplay = count.ToString(),
                                TypeKey = guidSameElement,
                                LengthParameters = lstObjPara
                            };
                            ParameterRows.Add(paraRow);
                            indexAdded = ParameterRows.Count - 1;
                            continue;
                        }

                        var minVal = groupValue.Min(x => x.Key);
                        var maxVal = groupValue.Max(x => x.Key);
                        var lstObjParaMax = objElement.Select(x => x.objPara).ToList();
                        var paraRowRange = new ParameterFilterRow
                        {
                            IsChecked = true,
                            Category = row.SubName,
                            FamilyType = row.Name,
                            ParameterName = objElement.FirstOrDefault().objPara.NameParameterLength,
                            Min = FormatLengthMillimeters(minVal),
                            Max = FormatLengthMillimeters(maxVal),
                            CountDisplay = objElement.Count().ToString(),
                            TypeKey = guidSameElement,
                            LengthParameters = lstObjParaMax,
                            MinLengthTag = minVal,
                            MaxLengthTag = maxVal
                        };
                        ParameterRows.Add(paraRowRange);
                        indexAdded = ParameterRows.Count - 1;
                    }
                    catch
                    {
                        // ignored — same as original
                    }
                    finally
                    {
                        if (indexAdded >= 0)
                        {
                            var addedRow = ParameterRows[indexAdded];
                            addedRow.IsVisible = objElement.FirstOrDefault().objPara.ObjectGroupVal.IsSelected;

                            if (objElement.FirstOrDefault().objPara.ObjectGroupVal.GroupTypeId == new ForgeTypeId(string.Empty))
                                addedRow.IsVisible = false;

                            foreach (var objElementData in objElement)
                            {
                                if (_lstObjectSelectedGroup.Any(x => x.GroupTypeId == objElementData.objPara.ObjectGroupVal.GroupTypeId))
                                    continue;

                                addedRow.IsVisible = false;
                            }

                            lstIndexRowAdded.Add(indexAdded);
                        }
                    }
                }

                bool isVisibleFirst = false;
                foreach (int indexRowAdd in lstIndexRowAdded)
                {
                    var addRow = ParameterRows[indexRowAdd];
                    if (!addRow.IsVisible)
                        continue;

                    if (!isVisibleFirst)
                    {
                        int count = groupByName.FirstOrDefault().Select(x => x.ElementCurrent.Id).GroupBy(x => x).Select(y => y.First()).Count();
                        addRow.CountDisplay = count.ToString();
                    }
                    else
                        addRow.CountDisplay = string.Empty;

                    isVisibleFirst = true;
                }
            }

            SetGroupNull(_lstObjectSelectedGroup);
            RefreshParameterTabState();
        }

        void SetData()
        {
            var lstCategory = ObjectElements.GroupBy(x => x.CategoriesElement).ToList();
            foreach (var objCategory in lstCategory)
            {
                CategoryItems.Add(new FilterRowItem
                {
                    IsChecked = false,
                    Name = objCategory.Key,
                    Count = objCategory.Count(),
                    Tag = objCategory.ToList()
                });
            }
        }

        void SelectElementByItems(ObservableCollection<FilterRowItem> items)
        {
            var lstElementSelect = new List<ElementId>();
            foreach (var row in items)
            {
                if (!row.IsChecked || row.Tag == null)
                    continue;
                lstElementSelect.AddRange(row.Tag.Select(x => x.ElementCurrent.Id));
            }

            _rvtUIDoc.Selection.SetElementIds(lstElementSelect);
            _rvtUIDoc.RefreshActiveView();
        }

        bool SelectElementParameter()
        {
            if (!ValidateValueUserInput())
            {
                var errorRow = ParameterRows.FirstOrDefault(r => r.IsVisible && !string.IsNullOrWhiteSpace(r.Error));
                if (errorRow != null)
                    ShowWeaveMessage(errorRow.Error, _cmpAttribute.ResourceText("IDS_ERR_ERROR"));
                return false;
            }

            var lstElementSelect = new List<ObjectLengthParameter>();
            var lstSelected = new List<ElementId>();
            var dicSameTypeSelect = new Dictionary<string, List<ObjectLengthParameter>>();

            var progressBarThread = new ProgressBarThread(false, true);
            try
            {
                if (OwnerHandle != IntPtr.Zero)
                    progressBarThread.SetOwner(OwnerHandle);
                progressBarThread.SetData(_cmpAttribute.ResourceText("IDS_TXT_PROGESSBAR"), 0);
                progressBarThread.ShowDialog();

                int dgvCurrentCountVisible = ParameterRows.Count(r => r.IsVisible);
                progressBarThread.SetData(dgvCurrentCountVisible, 0);

                int count = 0;
                for (int i = 0; i < ParameterRows.Count; i++)
                {
                    var row = ParameterRows[i];
                    row.CountTag = 0;
                    count++;

                    if (!row.IsVisible || !row.IsChecked || row.LengthParameters == null)
                        continue;

                    object prValueDgv = row.Value;
                    object prMinDgv = row.Min;
                    object prMaxDgv = row.Max;

                    foreach (var objElement in row.LengthParameters)
                    {
                        objElement.prValueDgv = prValueDgv;
                        objElement.prMinDgv = prMinDgv;
                        objElement.prMaxDgv = prMaxDgv;
                    }

                    if (SelectConnectChecked)
                    {
                        var lstElementNeedSelect = _cmpElements.GetSelectElementConnect(row.LengthParameters);
                        UnionSelectWithSameType(row, ref dicSameTypeSelect, lstElementNeedSelect);
                        row.CountTag = lstElementNeedSelect.Count;
                    }
                    else
                    {
                        var lstObjElementNeedSelect = _cmpElements.FilterParameterByUserInput(row.LengthParameters, prValueDgv, prMinDgv, prMaxDgv);
                        if (lstObjElementNeedSelect == null)
                            return false;

                        UnionSelectWithSameType(row, ref dicSameTypeSelect, lstObjElementNeedSelect);
                        row.CountTag = lstObjElementNeedSelect.Count;
                    }

                    progressBarThread.SetData(count);
                }

                UpdateCount(dicSameTypeSelect);

                foreach (var keyPair in dicSameTypeSelect)
                    lstElementSelect.AddRange(keyPair.Value);

                lstSelected.AddRange(lstElementSelect.Select(x => x.ElementCurrent.Id).ToList());
                _rvtUIDoc.Selection.SetElementIds(lstSelected);
                _rvtUIDoc.RefreshActiveView();

                RefreshParameterTabState();
                return true;
            }
            finally
            {
                progressBarThread.Close();
            }
        }

        void UpdateCount(Dictionary<string, List<ObjectLengthParameter>> dicData)
        {
            var dicDataRowHasValue = GetDicDataParameterHasValue(out _);

            foreach (var keypair in dicDataRowHasValue)
            {
                var lstValue = new List<string>();
                foreach (int index in keypair.Key)
                {
                    var row = ParameterRows[index];
                    if (string.IsNullOrEmpty(row.TypeKey))
                        continue;
                    lstValue.Add(row.TypeKey);
                }

                lstValue = lstValue.GroupBy(x => x).Select(y => y.First()).ToList();
                string keyFind = lstValue.FirstOrDefault();
                if (keyFind == null)
                    continue;

                if (!dicData.ContainsKey(keyFind))
                    continue;

                for (int i = 0; i < keypair.Key.Count; i++)
                {
                    if (i == 0)
                        ParameterRows[keypair.Key[i]].CountDisplay = dicData[keyFind].Count.ToString();
                    else
                        ParameterRows[keypair.Key[i]].CountDisplay = string.Empty;
                }
            }
        }

        bool UnionSelectWithSameType(ParameterFilterRow currentRow, ref Dictionary<string, List<ObjectLengthParameter>> dicData, List<ObjectLengthParameter> listCurrent)
        {
            if (string.IsNullOrEmpty(currentRow.TypeKey))
                return false;

            string typeKeyData = currentRow.TypeKey;

            if (!dicData.ContainsKey(typeKeyData))
            {
                dicData.Add(typeKeyData, listCurrent);
                return true;
            }

            var lstObjectOld = dicData[typeKeyData];
            var retVal = new List<ObjectLengthParameter>();
            foreach (var objCurrentElement in listCurrent)
            {
                if (lstObjectOld.Any(x => x.ElementCurrent.Id == objCurrentElement.ElementCurrent.Id))
                    retVal.Add(objCurrentElement);
            }

            dicData[typeKeyData] = retVal;
            return true;
        }

        Dictionary<List<int>, int> GetDicDataParameterHasValue(out int numberRow)
        {
            var dicdataRow = new Dictionary<List<int>, int>();
            List<int> lstIndexPreview = new List<int>();
            numberRow = 0;

            for (int i = 0; i < ParameterRows.Count; i++)
            {
                var row = ParameterRows[i];
                if (!row.IsVisible)
                    continue;

                if (!string.IsNullOrEmpty(row.CountDisplay))
                {
                    var lstIndex = new List<int>();
                    lstIndexPreview = lstIndex;
                    dicdataRow.Add(lstIndex, int.Parse(row.CountDisplay));
                }

                if (row.IsChecked)
                    numberRow++;

                lstIndexPreview.Add(i);
            }

            return dicdataRow;
        }

        void SetCountParameter(out int numberRow, out int totalCount)
        {
            var dicdataRow = GetDicDataParameterHasValue(out numberRow);
            totalCount = 0;

            foreach (var itemData in dicdataRow)
            {
                int indexRowHasValue = itemData.Key.FirstOrDefault();

                foreach (var indexRow in itemData.Key)
                {
                    var row = ParameterRows[indexRow];
                    if (!row.IsVisible)
                        continue;

                    if (row.IsChecked)
                    {
                        string valueNum = ParameterRows[indexRowHasValue].CountDisplay;
                        if (!string.IsNullOrEmpty(valueNum))
                            totalCount += int.Parse(valueNum);
                        break;
                    }
                }
            }
        }

        bool ValidateValueUserInput()
        {
            bool isOk = true;

            foreach (var row in ParameterRows)
            {
                string errorMess = string.Empty;
                row.Error = string.Empty;

                if (!row.IsVisible)
                    continue;

                var prValueDgv = row.Value;
                var prMinDgv = row.Min;
                var prMaxDgv = row.Max;

                if (CheckWhiteSpace(prValueDgv))
                    prValueDgv = string.Empty;
                if (CheckWhiteSpace(prMinDgv))
                    prMinDgv = string.Empty;
                if (CheckWhiteSpace(prMaxDgv))
                    prMaxDgv = string.Empty;

                bool isInputValue = prValueDgv != null && !string.IsNullOrEmpty(prValueDgv.ToString());
                bool isInputMin = prMinDgv != null && !string.IsNullOrEmpty(prMinDgv.ToString());
                bool isInputMax = prMaxDgv != null && !string.IsNullOrEmpty(prMaxDgv.ToString());

                if (!isInputValue && !isInputMin && !isInputMax)
                    continue;

                errorMess += ValidateSingleValue(prValueDgv, out int valuePr, out bool isErrorValue);
                errorMess += ValidateSingleValue(prMinDgv, out int minPr, out bool isErrorMin);
                errorMess += ValidateSingleValue(prMaxDgv, out int maxPr, out bool isErrorMax);

                if (isInputValue && isInputMin && isInputMax)
                {
                    if (!isErrorValue && !isErrorMin && !isErrorMax)
                    {
                        errorMess += CheckValueAndMin(valuePr, minPr);
                        errorMess += CheckValueAndMax(valuePr, maxPr);
                        errorMess += CheckMinAndMax(minPr, maxPr);
                    }

                    if (isErrorValue && !isErrorMin && !isErrorMax)
                        errorMess += CheckMinAndMax(minPr, maxPr);
                    else if (!isErrorValue && !isErrorMin && isErrorMax)
                        errorMess += CheckValueAndMin(valuePr, minPr);
                    else if (!isErrorValue && isErrorMin && !isErrorMax)
                        errorMess += CheckValueAndMax(valuePr, maxPr);
                }
                else if (isInputValue && isInputMin && !isInputMax)
                {
                    if (!isErrorValue && !isErrorMin)
                        errorMess += CheckValueAndMin(valuePr, minPr);
                }
                else if (isInputValue && !isInputMin && isInputMax)
                {
                    if (!isErrorValue && !isErrorMax)
                        errorMess += CheckValueAndMax(valuePr, maxPr);
                }
                else if (!isInputValue && isInputMin && isInputMax)
                {
                    if (!isErrorMin && !isErrorMax)
                        errorMess += CheckMinAndMax(minPr, maxPr);
                }
                else
                {
                    if (isInputValue && isInputMin && !isInputMax)
                        errorMess += CheckValueAndMin(valuePr, minPr);
                    if (isInputValue && !isInputMin && isInputMax)
                        errorMess += CheckValueAndMax(valuePr, maxPr);
                    if (!isInputValue && isInputMin && isInputMax)
                        errorMess += CheckMinAndMax(minPr, maxPr);
                }

                if (!string.IsNullOrEmpty(errorMess))
                {
                    isOk = false;
                    row.Error = errorMess.Trim();
                }
            }

            return isOk;
        }

        string CheckValueAndMin(int valuePr, int minPr) =>
            valuePr < minPr ? _cmpAttribute.ResourceText("IDS_ERR_LESSMIN") + "\n" : string.Empty;

        string CheckValueAndMax(int valuePr, int maxPr) =>
            valuePr > maxPr ? _cmpAttribute.ResourceText("IDS_ERR_GREATERMAX") + "\n" : string.Empty;

        string CheckMinAndMax(int minPr, int maxPr) =>
            minPr > maxPr ? _cmpAttribute.ResourceText("IDS_ERR_MINMAXVALUE") + "\n" : string.Empty;

        static bool CheckWhiteSpace(object objStr)
        {
            if (objStr == null)
                return false;
            string strVal = objStr.ToString();
            return strVal.Length > 0 && strVal.Trim().Length == 0;
        }

        string ValidateSingleValue(object valueObj, out int nVal, out bool isHasError)
        {
            string errorMess = string.Empty;
            nVal = 0;
            isHasError = true;

            if (valueObj == null || string.IsNullOrEmpty(valueObj.ToString()))
                return errorMess;

            string valueData = valueObj.ToString();

            if (!int.TryParse(valueData, out nVal))
                errorMess += _cmpAttribute.ResourceText("IDS_ERR_NUMBERONLY") + "\n";

            if (nVal < 0)
                errorMess = errorMess + _cmpAttribute.ResourceText("IDS_ERR_GREATER0") + "\n";

            if (string.IsNullOrEmpty(errorMess))
                isHasError = false;

            return errorMess;
        }

        void SetGroupNull(List<ObjectSelectGroup> lstGroupAllProject)
        {
            var lstGroupSelectedInProject = lstGroupAllProject.Where(x => x.IsSelected).ToList();
            var lstAllObjectLength = new List<ObjectLengthParameter>();

            foreach (var row in ParameterRows)
            {
                if (!row.IsChecked || row.LengthParameters == null)
                    continue;
                lstAllObjectLength.AddRange(row.LengthParameters);
            }

            var objLengthElement = lstAllObjectLength.GroupBy(x => x.ElementCurrent.Id).ToList();
            var lstElementNeedAddNull = new List<Element>();

            foreach (var objElement in objLengthElement)
            {
                if (objElement.Count() == 0)
                    continue;

                bool isFound = false;
                foreach (var objItem in objElement)
                {
                    if (objItem.ObjectGroupVal.GroupTypeId == new ForgeTypeId(string.Empty))
                    {
                        isFound = false;
                        break;
                    }

                    if (lstGroupSelectedInProject.Any(x => x.GroupTypeId == objItem.ObjectGroupVal.GroupTypeId))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                    lstElementNeedAddNull.Add(objElement.FirstOrDefault().ElementCurrent);
            }

            var lstObjectElement = _cmpElements.GetDataElement(lstElementNeedAddNull);
            var groupElement = lstObjectElement.GroupBy(x => (x.CategoriesElement, x.TypeNameElement)).ToList();

            foreach (var objectElement in groupElement)
            {
                var lstTemp = new List<ObjectLengthParameter>();
                foreach (var objCurrent in objectElement)
                {
                    objCurrent.GetLengthAndGroupParamter();
                    lstTemp.Add(objCurrent.ObjectLengths.FirstOrDefault());
                }

                ParameterRows.Add(new ParameterFilterRow
                {
                    IsChecked = true,
                    Category = objectElement.Key.CategoriesElement,
                    FamilyType = objectElement.Key.TypeNameElement,
                    ParameterName = string.Empty,
                    CountDisplay = objectElement.Count().ToString(),
                    ValueReadOnly = true,
                    MinReadOnly = true,
                    MaxReadOnly = true,
                    TypeKey = Guid.NewGuid().ToString(),
                    LengthParameters = lstTemp
                });
            }
        }

        static string FormatLengthMillimeters(double lengthMm)
        {
            if (double.IsNaN(lengthMm) || lengthMm == double.MinValue)
                return string.Empty;

            return ((int)Math.Round(lengthMm, MidpointRounding.AwayFromZero)).ToString();
        }

        void SetSelectedGroupParameter(List<ForgeTypeId> lstSelectedGroup)
        {
            foreach (var objElement in ObjectElements)
            {
                foreach (var objLength in objElement.ObjectLengths)
                {
                    objLength.ObjectGroupVal.IsSelected = lstSelectedGroup.Any(x => x == objLength.ObjectGroupVal.GroupTypeId);
                }
            }
        }

        void ShowWeaveMessage(string message, string title)
        {
            IntPtr owner = OwnerHandle != IntPtr.Zero
                ? OwnerHandle
                : _rvtUIDoc.Application.MainWindowHandle;

            if (owner != IntPtr.Zero)
            {
                WeaveDialogHost.ShowMessage(owner, message, title, _cmpAttribute.ResourceText("IDS_TXT_OK"));
                return;
            }

            MessageBox.Show(message, title, MessageBoxButton.OK);
        }

        void RaiseTabChanged() => TabChanged?.Invoke(this, EventArgs.Empty);
        void RaiseStateChanged() => StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
