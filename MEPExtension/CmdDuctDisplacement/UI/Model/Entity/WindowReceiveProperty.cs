using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.Resource;
using RevitMEPAddin.Common;

namespace CmdDuctDisplacement.UI.Model.Entity
{

    /// <summary>
    /// ユーザー入力のオプションを保持するシングルトンクラス
    /// Revitのが立ち上がっている間は、このクラスのメンバは破棄されない
    /// </summary>
    class WindowReceiveProperty
    {
        //メンバ変数
        #region Memeber Variables
        private static WindowReceiveProperty m_Instance = new WindowReceiveProperty();

        private Logger log;

        //フィールド定義
        //メイン画面
        //配置条件
        private bool _OffsetRadioButton;
        private bool _UnityRadioButton;
        //接合部
        private bool _FortyFiveElbowRadioButton;
        private bool _NinetyElbowRadioButton;
        private bool _ScarveElbowRadioButton;

        //移動距離オプション
        //丸め精度
        private bool _FiftyButton;
        private bool _OneHandredButton;
        private bool _NothingButton;
        //耐火被覆厚
        private bool _ValidButton;
        private bool _InValidButton;
        //最低部材間距離
        private double _BetweenObjValue;
        //耐火被覆の厚み
        private double _FireProofingValue;

        //基準位置
        private string _DuctReferenceLine;

        //移動FL
        private double _FlValue;
        #endregion

        //コンストラクタ
        #region Constructor
        private WindowReceiveProperty()
        {
            this.DuctOptionPropertyInit();
            this.DuctConnectionSettingPropertyInit();

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

        //プロパティ
        #region Properties
        public static WindowReceiveProperty Instance
        {
            get
            {
                return m_Instance;
            }
        }

        public bool OffsetRadioButton
        {
            get { return _OffsetRadioButton; }
            set { _OffsetRadioButton = value; }
        }

        public bool UnityRadioButton
        {
            get { return _UnityRadioButton; }
            set { _UnityRadioButton = value; }
        }

        public bool FortyFiveElbowRadioButton
        {
            get { return _FortyFiveElbowRadioButton; }
            set { _FortyFiveElbowRadioButton = value; }
        }

        public bool NinetyElbowRadioButton
        {
            get { return _NinetyElbowRadioButton; }
            set { _NinetyElbowRadioButton = value; }
        }

        public bool ScarveElbowRadioButton
        {
            get { return _ScarveElbowRadioButton; }
            set { _ScarveElbowRadioButton = value; }
        }


        public bool FiftyButton
        {
            get { return _FiftyButton; }
            set { _FiftyButton = value; }
        }

        public bool OneHandredButton
        {
            get { return _OneHandredButton; }
            set { _OneHandredButton = value; }
        }

        public bool NothingButton
        {
            get { return _NothingButton; }
            set { _NothingButton = value; }
        }

        public bool ValidButton
        {
            get { return _ValidButton; }
            set { _ValidButton = value; }
        }
        public bool InValidButton
        {
            get { return _InValidButton; }
            set { _InValidButton = value; }
        }
        public double BetweenObjValue
        {
            get { return _BetweenObjValue; }
            set { _BetweenObjValue = value; }
        }
        public double FireProofingValue
        {

            get { return _FireProofingValue; }
            set { _FireProofingValue = value; }
        }


        public string DuctReferenceLine
        {

            get { return _DuctReferenceLine; }
            set { _DuctReferenceLine = value; }
        }


        public double FlValue
        {

            get { return _FlValue; }
            set { _FlValue = value; }
        }
        #endregion

        // メンバ関数
        #region Member Functions
        /// <summary>
        /// 接合部と移動方法のプロパティを初期化する
        /// </summary>
        public void DuctConnectionSettingPropertyInit()
        {
            OffsetRadioButton = true;
            UnityRadioButton = false;
            FortyFiveElbowRadioButton = true;
            NinetyElbowRadioButton = false;
            ScarveElbowRadioButton = false;
        }

        /// <summary>
        /// オプション関連を初期化する
        /// </summary>
        public void DuctOptionPropertyInit()
        {
            BetweenObjValue = 100;
            FireProofingValue = 45;
            FiftyButton = true;
            OneHandredButton = false;
            NothingButton = false;
            ValidButton = false;
            InValidButton = true;
        }

        /// <summary>
        /// エルボの種類を内部処理用に切り替える
        /// </summary>
        /// <returns>定数 エルボ部材</returns>
        public int ElbowType()
        {
            int rc;

            if ((FortyFiveElbowRadioButton == true) &&
                (NinetyElbowRadioButton == false) &&
                (ScarveElbowRadioButton == false))
            {
                rc = (int)DuctDisplacementDefine.FITTING_PTN.deg45;
            }

            else if ((FortyFiveElbowRadioButton == false) &&
                    (NinetyElbowRadioButton == true) &&
                    (ScarveElbowRadioButton == false))
            {
                rc = (int)DuctDisplacementDefine.FITTING_PTN.deg90;
            }

            else if ((FortyFiveElbowRadioButton == false) &&
                (NinetyElbowRadioButton == false) &&
                (ScarveElbowRadioButton == true))
            {
                rc = (int)DuctDisplacementDefine.FITTING_PTN.S;
            }

            else
            {
                //error
                log.Error("Check ElbowRadioButton Out");
                rc = (int)DuctDisplacementDefine.FITTING_PTN.deg45;
            }

            return rc;
        }

        /// <summary>
        /// 丸め精度を内部ロジック用に切り替える
        /// </summary>
        /// <returns>定数 丸め精度</returns>
        public int RoundType()
        {
            int rc;

            if ((FiftyButton == true) &&
                (OneHandredButton == false) &&
                (NothingButton == false))
            {
                rc = DuctDisplacementDefine.num_50;
            }

            else if ((FiftyButton == false) &&
                    (OneHandredButton == true) &&
                    (NothingButton == false))
            {
                rc = DuctDisplacementDefine.num_100;
            }

            else if ((FiftyButton == false) &&
                (OneHandredButton == false) &&
                (NothingButton == true))
            {
                rc = DuctDisplacementDefine.num_0;
            }

            else
            {
                //error
                log.Error("Check RoundType Out");
                rc = DuctDisplacementDefine.num_50;
            }

            return rc;
        }

        /// <summary>
        /// 基準ラインを内部ロジック用に切り替える
        /// </summary>
        /// <returns>定数 基準ライン</returns>
        public int ReferenceLineType()
        {
            int rc;

            if (this.DuctReferenceLine == ExResources.ResxString(DuctDisplacementDefine.LVL_Center))
            {
                rc = DuctDisplacementDefine.OFFSET_POS_MIDDLE;
            }

            else if (this.DuctReferenceLine == ExResources.ResxString(DuctDisplacementDefine.LVL_BottomSide))
            {
                rc = DuctDisplacementDefine.OFFSET_POS_BOTTOM;
            }

            else if (this.DuctReferenceLine == ExResources.ResxString(DuctDisplacementDefine.LVL_TopSide))
            {
                rc = DuctDisplacementDefine.OFFSET_POS_TOP;
            }

            else
            {
                //error
                log.Error("Check ReferenceLineType Out");
                rc = DuctDisplacementDefine.OFFSET_POS_MIDDLE;
            }

            return rc;
        }

        /// <summary>
        /// 移動方法を内部ロジック用に切り替える
        /// </summary>
        /// <returns>定数 移動方法</returns>
        public int MovingMethodType()
        {
            int rc;

            if ((OffsetRadioButton == true) &&
                (UnityRadioButton == false))
            {
                rc = (int)DuctDisplacementDefine.MOVE_PTN.OFFSET;
            }

            else if ((OffsetRadioButton == false) &&
                    (UnityRadioButton == true))
            {
                rc = (int)DuctDisplacementDefine.MOVE_PTN.UNIFIEDLVEL;
            }
            else
            {
                //error
                log.Error("Check ReferenceLineType Out");
                rc = (int)DuctDisplacementDefine.MOVE_PTN.OFFSET;
            }

            return rc;
        }


        /// <summary>
        /// 耐火被覆厚が有効/無効 判定
        /// </summary>
        /// <returns>ture  :有効
        ///       　false  :無効</returns>
        public bool FireProofingType()
        {
            bool rc;

            if ((ValidButton == true) &&
                (InValidButton == false))
            {
                rc = true;
            }

            else if ((ValidButton == false) &&
                    (InValidButton == true))
            {
                rc = false;
            }
            else
            {
                //error
                log.Error("Check ReferenceLineType Out");
                rc = true;
            }

            return rc;
        }
        #endregion
    }
}

