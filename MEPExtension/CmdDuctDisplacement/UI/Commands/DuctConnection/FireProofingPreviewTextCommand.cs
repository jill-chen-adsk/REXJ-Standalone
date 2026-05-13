using CmdDuctDisplacement.UI.Model.InheritBase;
using CmdDuctDisplacement.UI.View;
using CmdDuctDisplacement.UI.ViewModel;
using RevitMEPAddin.Common;

namespace CmdDuctDisplacement.UI.Commands.DuctConnection
{
    /// <summary>
    /// 耐火被覆厚(梁)テキストボックスPreviewTextInputイベント関連クラス
    /// </summary>
    class FireProofingPreviewTextCommand : PreviewTextBoxInputBase
    {
        //メンバ変数
        #region Memeber Variables
        private Logger log;
        private ConnectionSettingViewModel ductp;
        private ConnectionSettingView uiwindow;
        #endregion


        //コンストラクタ
        #region Constructor
        public FireProofingPreviewTextCommand(ConnectionSettingViewModel ductPropaty, Logger _log, ConnectionSettingView _uiwindow)
        {
            ductp = ductPropaty;
            uiwindow = _uiwindow;
            log = _log;
            textbox = uiwindow.FireTextBox;
        }
        #endregion

        // メンバ関数
        #region Member Functions
        #endregion
    }
}
