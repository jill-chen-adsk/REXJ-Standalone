using CmdDuctDisplacement.UI.Model.InheritBase;
using CmdDuctDisplacement.UI.View;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// 部材間距離テキストボックスのPreviewTextInputイベント関連クラス
    /// </summary>
    class BetweenPreviewTextCommand : PreviewTextBoxInputBase
    {
        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductp;
        private ConnectionSettingView uiwindow;
        #endregion


        //コンストラクタ
        #region Constructor
        public BetweenPreviewTextCommand(ConnectionSettingViewModel ductPropaty, Logger _log, ConnectionSettingView _uiwindow)
        {
            ductp = ductPropaty;
            uiwindow = _uiwindow;
            log = _log;
            //PreviewTextBoxInputBaseクラスのtextboxにuiwindow.BetweenTextBoxをセット
            textbox = uiwindow.BetweenTextBox;
        }
        #endregion

        // メンバ関数
        #region Member Functions


        #endregion
    }
}