using System;
using System.Windows;
using Autodesk.Revit.UI;
using MepManholeTool.ViewModel;

namespace MepManholeTool.Views
{
    public partial class ParameterMappingView : Window
    {
        private readonly ParameterMappingViewModel _viewModel;

        public ParameterMappingView(UIDocument uiDocument)
        {
            InitializeComponent();
            _viewModel = new ParameterMappingViewModel(uiDocument);
            DataContext = _viewModel;
            var vm = this.DataContext as ParameterMappingViewModel;
            
            // Set window property
            if (vm != null && vm.Window == null)
                vm.Window = this;
            
            if (vm != null && vm.OKAction == null)
                vm.OKAction = new Action(this.OK);
            if (vm != null && vm.CancelAction == null)
                vm.CancelAction = new Action(this.Cancel);
        }

        private void OK() => this.DialogResult = true;
        private void Cancel() => this.DialogResult = false;
    }
}