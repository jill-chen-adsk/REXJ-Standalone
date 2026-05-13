using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFView.WPF
{
    /// <summary>
    /// WPFView.xaml の相互作用ロジック
    /// </summary>
    public partial class WPFView : Window
    {
        public WPFView()
        {
            var vm = new WPFViewModel();

            InitializeComponent();

            DataContext = vm;

            vm._txt_WPFViewModel001 = "1000";

            if (vm.CloseWindow == null)
            {
                vm.CloseWindow = new Action(this.Close);
            }
        }
    }
}
