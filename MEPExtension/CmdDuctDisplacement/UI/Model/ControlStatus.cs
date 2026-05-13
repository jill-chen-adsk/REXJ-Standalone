using Autodesk.Revit.UI;
using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.Resource;
using RevitMEPAddin.Common;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;

namespace CmdDuctDisplacement.UI.Model
{
    /// <summary>
    /// シングルトンクラス
    /// 画面制御をおこなうメンバを管理する
    /// </summary>
    class ControlStatus
    {
        private Logger log;

        //メンバ変数
        #region Memeber Variables
        //インスタンス生成
        private static ControlStatus m_Instance = new ControlStatus();

        //押下されたキーの情報
        private Key _LastKey;
        //FLテキストボックスのTextChangedEventArgs ハンドラ
        private TextCompositionEventArgs _TextCompositionEvent;
        //FLテキストボックスの値変更の呼び出し経路
        private DuctDisplacementDefine.TextChangeRoute _CallRoute;
        //画面クローズの終了経路
        private DuctDisplacementDefine.WindowReturnNum _ButtonType;
        //一回前のFL算出の図形基準ライン
        private string _ReferenceLine_Befor;
        //Revitのドキュメントに設定されている長さの浮動小数点精度
        private double _RevitProjectDecimalAccuracy;
        private List<DuctDisplacementDefine.TextChangeRoute> _textchangelist;
        #endregion

        //コンストラクタ
        #region Constructor
        private ControlStatus()
        {
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

            _textchangelist = new List<DuctDisplacementDefine.TextChangeRoute>();
        }
        #endregion

        //プロパティ
        #region Properties
        public static ControlStatus Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public Key LastKey
        {
            get { return _LastKey; }
            set
            {
                _LastKey = value;
            }
        }

        public TextCompositionEventArgs TextCompositionEvent
        {
            get { return _TextCompositionEvent; }
            set
            {
                if (value != null)
                {
                    _TextCompositionEvent = value;
                }
            }
        }

        public DuctDisplacementDefine.TextChangeRoute CallRoute
        {
            get
            {
                //log.Trace("Get CallRoute:" + _CallRoute);
                return _CallRoute;
            }
            set
            {

                _CallRoute = value;
                textchangelist = value;
                log.Info("Set CallRoute:" + _CallRoute);
            }
        }

        public string ReferenceLine_Befor
        {
            get { return _ReferenceLine_Befor; }
            set
            {
                if (value != null)
                {
                    _ReferenceLine_Befor = value;
                }
            }
        }
        public DuctDisplacementDefine.WindowReturnNum ButtonType
        {
            get { return _ButtonType; }
            set { _ButtonType = value; }
        }

        public double RevitProjectDecimalAccuracy
        {
            get { return _RevitProjectDecimalAccuracy; }
            set { _RevitProjectDecimalAccuracy = value; }
        }


        private DuctDisplacementDefine.TextChangeRoute textchangelist
        {
            set { _textchangelist.Add(value); }
        }


        #endregion

        // メンバ関数
        #region Member Functions
        /// <summary>
        /// プロパティを初期化する
        /// </summary>
        public void Init()
        {
            log.Info(MethodBase.GetCurrentMethod().Name);
            LastKey = Key.None;
            TextCompositionEvent = null;
            CallRoute = DuctDisplacementDefine.TextChangeRoute.NoSelect;
            ButtonType = DuctDisplacementDefine.WindowReturnNum.NoSelect;
            ReferenceLine_Befor = string.Empty;
            _textchangelist.Clear();
        }

        /// <summary>
        /// メンバをクリアする
        /// </summary>
        public void clear()
        {
            log.Info(MethodBase.GetCurrentMethod().Name);
            LastKey = Key.None;
            CallRoute = DuctDisplacementDefine.TextChangeRoute.NoSelect;
            ButtonType = DuctDisplacementDefine.WindowReturnNum.NoSelect;
            ReferenceLine_Befor = string.Empty;
        }

        /// <summary>
        /// プロパティをセットする
        /// </summary>
        /// <param name="uidoc"></param>
        public void SetProperty(UIDocument uidoc)
        {
            WrpDocument documentelement = new WrpDocument(uidoc, log);
            RevitProjectDecimalAccuracy = documentelement.GetAccuracyDecimalLength();
            log.Info("Decimal Point" + RevitProjectDecimalAccuracy.ToString());
        }

        /// <summary>
        /// FLテキストの変更経路を末尾から取得する
        /// </summary>
        /// <param name="i"></param>
        public DuctDisplacementDefine.TextChangeRoute GetTextChangeRoute(int i)
        {
            return _textchangelist[_textchangelist.Count - i];
        }
        #endregion
    }
}