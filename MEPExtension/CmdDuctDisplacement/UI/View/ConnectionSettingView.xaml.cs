using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.Logic;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;
using System.Windows;
using CmdDuctDisplacement.Resource;
using CmdDuctDisplacement.UI.Model.ControlEventManager;

namespace CmdDuctDisplacement.UI.View
{
    /// <summary>
    /// Code-behind for <see cref="ConnectionSettingView"/> (duct displacement settings dialog).
    /// </summary>
    public partial class ConnectionSettingView : Window
    {
        #region Memeber Variables
        private ConnectionSettingViewModel ductp;
        MEPOperation mep;
        private Logger log;
        #endregion

        #region Constructor
        public ConnectionSettingView(MEPOperation _mep)
        {
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
            mep = _mep;
            ductp = new ConnectionSettingViewModel(this, _mep);
            TextBoxHandle textboxhandle = new TextBoxHandle(this);

            this.DataContext = ductp;

            int min, max;
            if (!int.TryParse(ExResources.ResxString(DuctDisplacementDefine.LOG_LEVEL_MAX), out max))
            {
                max = DuctDisplacementDefine.LOG_LEVEL_MAX_DEF;
            }
            if (!int.TryParse(ExResources.ResxString(DuctDisplacementDefine.LOG_LEVEL_MIN), out min))
            {
                min = DuctDisplacementDefine.LOG_LEVEL_MIN_DEF;
            }
            this.log = new Logger(max, min, DuctDisplacementDefine.LOG_FOLDER_PATH_DEF);
        }
        #endregion

        #region Member Functions

        /// <summary>Returns the dialog view-model instance.</summary>
        public ConnectionSettingViewModel GetviewModel()
        {
            return ductp;
        }

        /// <summary>Hides the window (Revit-hosted mode pattern).</summary>
        public void CloseWindow()
        {
            this.Visibility = System.Windows.Visibility.Hidden;
        }


        #endregion
    }
}
