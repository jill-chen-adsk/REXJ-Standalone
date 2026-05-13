using System;
using System.Text;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
using UTILS = SectionListRC.Utils;

namespace SectionListRC.Setting
{
    public partial class FormParamMapping : Form
    {
        // メンバ変数
        #region Member Variables

        /// <summary>属性</summary>
        private SectionListRC.Components.Attribute _CmpAttribute;

        /// <summary>設定ファイル名</summary>
        private string _SettingFileName;

        /// <summary>設定ファイルディレクトリ</summary>
        private string _SettingFileDirectory;

        /// <summary>パラメータ</summary>
        private SectionListRC.Components.Parameters _CmpParameter;

        /// <summary>矩形柱パラメータ</summary>
        private Collections.Generic.IDictionary<string, string> _DicRectangleColumn;

        /// <summary>円柱パラメータ</summary>
        private Collections.Generic.IDictionary<string, string> _DicRoundColumn;

        /// <summary>梁パラメータ</summary>
        private Collections.Generic.IDictionary<string, string> _DicGirder;

        /// <summary>片持ち梁パラメータ</summary>
        private Collections.Generic.IDictionary<string, string> _DicCantiGirder;

        #endregion Member Variables

        // コンストラクタ
        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute">属性</param>
        ///
        /// <history>2013/06/11 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        FormParamMapping(SectionListRC.Components.Attribute cmpAttribute, string settingFileName, string settingFileDirectory, SectionListRC.Components.Parameters cmpParameter)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            InitializeComponent();
            _CmpAttribute = cmpAttribute;
            _SettingFileName = settingFileName;
            _SettingFileDirectory = settingFileDirectory;

            _CmpParameter = cmpParameter;

            SetPArameter();

            //SetData();
            //SetCurrentSettingFileValue();
        }

        #endregion Constructor

        // メンバ関数
        #region Member Functions

        /// ================================================================================
        /// <summary>パラメータ設定</summary>
        ///
        /// <history>2014/06/02 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        void SetPArameter()
        {
            //
            _CmpParameter.GetColumnParamName(ref _DicRectangleColumn,
                                             ref _DicRoundColumn,
                                             ref _DicGirder,
                                             ref _DicCantiGirder);

            // 矩形柱
            Collections.Generic.ICollection<string> keys = _DicRectangleColumn.Keys;

            foreach (string k in keys) {
                if (k == _CmpAttribute.ResourceText("IDS_TXT_CATEGORY")) {
                    continue;
                }

                this.dgvRectangleColumn.Rows.Add(k, _DicRectangleColumn[k]);
            }

            // 円柱
            keys = _DicRoundColumn.Keys;

            foreach (string k in keys) {
                if (k == _CmpAttribute.ResourceText("IDS_TXT_CATEGORY")) {
                    continue;
                }

                this.dgvCircleColumn.Rows.Add(k, _DicRoundColumn[k]);
            }

            // 梁
            keys = _DicGirder.Keys;

            foreach (string k in keys) {
                if (k == _CmpAttribute.ResourceText("IDS_TXT_CATEGORY")) {
                    continue;
                }

                this.dgvGirder.Rows.Add(k, _DicGirder[k]);
            }

            // 片持ち梁
            keys = _DicCantiGirder.Keys;

            foreach (string k in keys) {
                if (k == _CmpAttribute.ResourceText("IDS_TXT_CATEGORY")) {
                    continue;
                }

                this.dgvCantiGirder.Rows.Add(k, _DicCantiGirder[k]);
            }
        }

        /// ================================================================================
        /// <summary>データ設定</summary>
        ///
        /// <history>2013/06/11 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        void SetData()
        {
            // データグリッドのヘッダ
            this.dgvRectangleColumn.Columns[0].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMCONTENTS");
            this.dgvRectangleColumn.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CURRENTMAPPINGNAME");

            this.dgvCircleColumn.Columns[0].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMCONTENTS");
            this.dgvCircleColumn.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CURRENTMAPPINGNAME");

            this.dgvGirder.Columns[0].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMCONTENTS");
            this.dgvGirder.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CURRENTMAPPINGNAME");

            this.dgvCantiGirder.Columns[0].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMCONTENTS");
            this.dgvCantiGirder.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CURRENTMAPPINGNAME");

            // 矩形柱
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_COLUMNCATEGORY"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_DX"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_DY"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINHUTOKEI"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINHUTOKEI"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINX1HUTOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINX1HUTOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINX2HUTOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINX2HUTOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINY1HUTOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINY1HUTOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINY2HUTOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINY2HUTOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINHOSOKEI"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINHOSOKEI"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINX1HOSOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINX1HOSOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINX2HOSOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINX2HOSOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINY1HOSOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINY1HOSOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINY2HOSOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINY2HOSOKEIHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_SINTEKKINKEI"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_SINTEKKINHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_SINTEKKINXICHI"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_SINTEKKINYICHI"), "");

            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHOOPXKEI"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHOOPXKEI"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHOOPXHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHOOPXHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHOOPYHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHOOPYHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHOOPPITCH"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHOOPPITCH"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_HABADOMEKINKEI"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHABADOMEKINXHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHABADOMEKINXHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHABADOMEKINYHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHABADOMEKINYHONSU"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_HABADOMEKINPITCH"), "");
            this.dgvRectangleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_HASIRAHUGO"), "");

            this.dgvRectangleColumn.Columns[0].ReadOnly = true;

            // 円柱
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_COLUMNCATEGORY"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_TYOKKEI"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINKEI"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINKEI"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOSYUKINHONSU"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUSYUKINHONSU"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_SINTEKKINKEI"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_SINTEKKINHONSU"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_SINTEKKINICHI"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHOOPXKEI"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHOOPXKEI"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHOOPPITCH"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHOOPPITCH"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_HABADOMEKINKEI"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHABADOMEKINXHONSU"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHABADOMEKINXHONSU"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUTOHABADOMEKINYHONSU"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUKYAKUHABADOMEKINYHONSU"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_HABADOMEKINPITCH"), "");
            this.dgvCircleColumn.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_HASIRAHUGO"), "");

            this.dgvCircleColumn.Columns[0].ReadOnly = true;

            // 梁
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_GIRDERCATEGORY"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANHABA"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHHABA"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANHABA"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANHASEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINUEHUTOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINUEHUTOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINUEHUTOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINUE1HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINUE1HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINUE1HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINUE2HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINUE2HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINUE2HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINUE3HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINUE3HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINUE3HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINSITAHUTOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINSITAHUTOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINSITAHUTOKEI"), "");

            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINSITA1HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINSITA1HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINSITA1HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINSITA2HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINSITA2HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINSITA2HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINSITA3HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINSITA3HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINSITA3HUTOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINUEHOSOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINUEHOSOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINUEHOSOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINUE1HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINUE1HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINUE1HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINUE2HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINUE2HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINUE2HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINUE3HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINUE3HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINUE3HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINSITAHOSOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINSITAHOSOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINSITAHOSOKEI"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINSITA1HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINSITA1HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINSITA1HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINSITA2HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINSITA2HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINSITA2HOSOKEIHONSU"), "");

            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSYUKINSITA3HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSYUKINSITA3HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSYUKINSITA3HOSOKEIHONSU"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSTIRRUPDIAMETER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSTIRRUPDIAMETER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSTIRRUPDIAMETER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSTIRRUPNUMBER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSTIRRUPNUMBER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSTIRRUPNUMBER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSTIRRUPPITCH"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSTIRRUPPITCH"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSTIRRUPPITCH"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANWEBDIAMETER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHWEBDIAMETER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANWEBDIAMETER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANWEBNUMBER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHWEBNUMBER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANWEBNUMBER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSPACINGDIAMETER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSPACINGDIAMETER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSPACINGDIAMETER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSPACINGNUMBER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSPACINGNUMBER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSPACINGNUMBER"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_ITANSPACINGPITCH"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_CHUOHSPACINGPITCH"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_CONTENTS_JTANSPACINGPITCH"), "");
            this.dgvGirder.Rows.Add(_CmpAttribute.ResourceText("IDS_TXT_HARIHUGO"), "");

            this.dgvGirder.Columns[0].ReadOnly = true;
        }

        /// ================================================================================
        /// <summary>設定ファイル値のセット</summary>
        ///
        /// <history>2013/06/11 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        void SetCurrentSettingFileValue()
        {
            string fullName = _SettingFileDirectory + _SettingFileName;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            System.Text.Encoding enc = System.Text.Encoding.GetEncoding("shift_jis");

            if (System.IO.File.Exists(fullName)) {
                string[] strs = System.IO.File.ReadAllLines(fullName, enc);

                if (strs.Length == 212) {
                    int rectangleRowNum = 0;
                    int circleRowNum = 0;
                    int girderRowNum = 0;

                    for (int i = 62; i < 209; ++i) {
                        if (i == 89 || i == 105 || i == 126 || i == 149 || i == 180) {
                            continue;
                        }

                        string str = strs[i];

                        if (i < 105) {
                            System.Windows.Forms.DataGridViewCell dgvCell = this.dgvRectangleColumn[1, rectangleRowNum];
                            dgvCell.Value = strs[i];

                            rectangleRowNum += 1;
                        }
                        else if (i < 126) {
                            System.Windows.Forms.DataGridViewCell dgvCell = this.dgvCircleColumn[1, circleRowNum];
                            dgvCell.Value = strs[i];

                            circleRowNum += 1;
                        }
                        else {
                            System.Windows.Forms.DataGridViewCell dgvCell = this.dgvGirder[1, girderRowNum];
                            dgvCell.Value = strs[i];

                            girderRowNum += 1;
                        }
                    }
                }
            }
        }

        /// ================================================================================
        /// <summary>文字設定</summary>
        ///
        /// <history>2013/06/11 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_PARAMMAP") ;

            // タブ
            this.tabRectangleColumn.Text = _CmpAttribute.ResourceText("IDS_TXT_RECTANGLECOLUMN");
            this.tabCircleColumn.Text = _CmpAttribute.ResourceText("IDS_TXT_CIRCLECOLUMN");
            this.tabGirder.Text = _CmpAttribute.ResourceText("IDS_TXT_GIRDER_TXT");
            this.tabCantiGirder.Text = _CmpAttribute.ResourceText("IDS_TXT_CANTIGIRDER");

            // データグリッドのヘッダ
            this.dgvRectangleColumn.Columns[0].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMCONTENTS");
            this.dgvRectangleColumn.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CURRENTMAPPINGNAME");
            this.dgvCircleColumn.Columns[0].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMCONTENTS");
            this.dgvCircleColumn.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CURRENTMAPPINGNAME");
            this.dgvGirder.Columns[0].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMCONTENTS");
            this.dgvGirder.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CURRENTMAPPINGNAME");
            this.dgvCantiGirder.Columns[0].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_PARAMCONTENTS");
            this.dgvCantiGirder.Columns[1].HeaderText = _CmpAttribute.ResourceText("IDS_TXT_CURRENTMAPPINGNAME");

            // ヘルプ
            this.btnHelpRectangle.Text = "";
            this.btnHelpCircle.Text = "";
            this.btnHelpGirder.Text = "";
            this.btnHelpCantiGirder.Text = "";

            // コピーペースト
            this.btnRectCopy.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLCOPY");
            this.btnRectPaste.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLPASTE");
            this.btnCircleCopy.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLCOPY");
            this.btnCirclePaste.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLPASTE");
            this.btnGirderCopy.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLCOPY");
            this.btnGirderPaste.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLPASTE");
            this.btnCantiGirderCopy.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLCOPY");
            this.btnCantiGirderPaste.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLPASTE");

            this.btnOK.Text = _CmpAttribute.ResourceText("IDS_TXT_OK");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");
        }

        #endregion Member Functions

        // プロパティ
        #region Properties

        /// ================================================================================
        /// <summary>指定パラメータ名</summary>
        ///
        /// <history>2013/06/11 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IList<string> ParameterMappingName
        {
            get
            {
                Collections.Generic.IList<string> ret = new Collections.Generic.List<string>();

                for (int i = 0; i < this.dgvRectangleColumn.Rows.Count; ++i) {
                    ret.Add((string)this.dgvRectangleColumn[1, i].Value);

                    if (i == 26) {
                        ret.Add("");
                    }
                }
                ret.Add("");

                for (int i = 0; i < this.dgvCircleColumn.Rows.Count; ++i) {
                    ret.Add((string)this.dgvCircleColumn[1, i].Value);
                }
                ret.Add("");

                for (int i = 0; i < this.dgvGirder.Rows.Count; ++i) {
                    ret.Add((string)this.dgvGirder[1, i].Value);

                    if (i == 21) {
                        ret.Add("");
                    }
                    if (i == 51) {
                        ret.Add("");
                    }
                }

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>変更パラメータ名 -矩形柱</summary>
        ///
        /// <history>2014/06/02 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IDictionary<string, string> ChangeRectangleColumnParameters
        {
            get
            {
                return _DicRectangleColumn;
            }
        }

        /// ================================================================================
        /// <summary>変更パラメータ名 -円柱</summary>
        ///
        /// <history>2014/06/02 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IDictionary<string, string> ChangeRoundColumnParameters
        {
            get
            {
                return _DicRoundColumn;
            }
        }

        /// ================================================================================
        /// <summary>変更パラメータ名 -梁</summary>
        ///
        /// <history>2014/06/02 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IDictionary<string, string> ChangeGirderParameters
        {
            get
            {
                return _DicGirder;
            }
        }

        /// ================================================================================
        /// <summary>変更パラメータ名 -片持ち梁</summary>
        ///
        /// <history>2014/06/02 Created GSA,Inc Ryo Kuroda</history>
        /// ================================================================================
        public
        Collections.Generic.IDictionary<string, string> ChangeCantiGirderParameters
        {
            get
            {
                return _DicCantiGirder;
            }
        }

        #endregion Properties

        // イベント
        #region Events

        // ロード
        private void FormParamMapping_Load(object sender, EventArgs e)
        {
            SetText();

            this.dgvRectangleColumn.Select();
        }

        protected override void WndProc(ref Message m)
        {
            // Form のドラッグ移動 処理
            base.WndProc(ref m);
            if ((m.Msg == 0x84) && (m.Result == (IntPtr)1)) m.Result = (IntPtr)2;
        }

        // タブ切り替え
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabControl1.SelectedTab == this.tabRectangleColumn) {
                this.dgvRectangleColumn.Select();
            }
            else if (this.tabControl1.SelectedTab == this.tabCircleColumn) {
                this.dgvCircleColumn.Select();
            }
            else if (this.tabControl1.SelectedTab == this.tabGirder) {
                this.dgvGirder.Select();
            }
            else if (this.tabControl1.SelectedTab == this.tabCantiGirder) {
                this.dgvCantiGirder.Select();
            }
        }

        #region ヘルプボタン

        // 矩形柱
        private void btnHelpRectangle_Click(object sender, EventArgs e)
        {
            SectionListRC.Setting.FormHintView formhint = new SectionListRC.Setting.FormHintView(_CmpAttribute, 4, this);
            formhint.ShowDialog();
        }

        // 円形柱
        private void btnHelpCircle_Click(object sender, EventArgs e)
        {
            SectionListRC.Setting.FormHintView formhint = new SectionListRC.Setting.FormHintView(_CmpAttribute, 5, this);
            formhint.ShowDialog();
        }

        // 梁
        private void btnHelpGirder_Click(object sender, EventArgs e)
        {
            SectionListRC.Setting.FormHintView formhint = new SectionListRC.Setting.FormHintView(_CmpAttribute, 6, this);
            formhint.ShowDialog();
        }

        // 片持ち梁
        private void btnHelpCantiGirder_Click(object sender, EventArgs e)
        {
            SectionListRC.Setting.FormHintView formhint = new SectionListRC.Setting.FormHintView(_CmpAttribute, 7, this);
            formhint.ShowDialog();
        }

        #endregion ヘルプボタン

        #region 全コピーボタン

        // 矩形柱
        private void btnRectCopy_Click(object sender, EventArgs e)
        {
            string strClip = "";

            for (int i = 0; i < 42; ++i) {
                strClip += this.dgvRectangleColumn[1, i].Value.ToString() + "\r\n";
            }

            Clipboard.SetDataObject(strClip);
        }

        // 円形柱
        private void btnCircleCopy_Click(object sender, EventArgs e)
        {
            string strClip = "";

            for (int i = 0; i < 20; ++i) {
                strClip += this.dgvCircleColumn[1, i].Value.ToString() + "\r\n";
            }

            Clipboard.SetDataObject(strClip);
        }

        // 梁
        private void btnGirderCopy_Click(object sender, EventArgs e)
        {
            string strClip = "";

            for (int i = 0; i < 80; ++i) {
                strClip += this.dgvGirder[1, i].Value.ToString() + "\r\n";
            }

            Clipboard.SetDataObject(strClip);
        }

        // 片持ち梁
        private void btnCantiGirderCopy_Click(object sender, EventArgs e)
        {
            string strClip = "";

            for (int i = 0; i < 80; ++i) {
                strClip += this.dgvCantiGirder[1, i].Value.ToString() + "\r\n";
            }

            Clipboard.SetDataObject(strClip);
        }

        #endregion 全コピーボタン

        #region 全ペーストボタン

        // 矩形柱
        private void btnRectPaste_Click(object sender, EventArgs e)
        {
            //クリップボードの内容を取得
            string pasteText = Clipboard.GetText();

            if (string.IsNullOrEmpty(pasteText)) {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NOTEXTTPASTE"));
                return;
            }

            // 改行記号を統一　
            pasteText = pasteText.Replace("\r\n", "\n");
            pasteText = pasteText.Replace('\r', '\n');

            // 末尾の記号を削除
            pasteText = pasteText.TrimEnd(new char[] { '\n' });

            // 行ごとに分ける
            string[] lines = pasteText.Split('\n');

            if (lines.Length != 42) {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_WRONGPASTEVALUE"));
                return;
            }

            int insertRowIndex = 0;

            foreach (string line in lines) {
                //タブで分割
                //string[] vals = line.Split('\t');

                this.dgvRectangleColumn[1, insertRowIndex].Value = line;

                //DataGridViewRow row = this.dgvRectangleColumn.Rows[insertRowIndex];
                ////ヘッダーを設定
                //row.HeaderCell.Value = vals[0];
                ////各セルの値を設定
                //for (int i = 0; i < 1; i++)
                //{
                //  row.Cells[1].Value = vals[0];
                //}

                //次の行へ
                insertRowIndex++;
            }
        }

        // 円形柱
        private void btnCirclePaste_Click(object sender, EventArgs e)
        {
            //クリップボードの内容を取得して、行で分ける
            string pasteText = Clipboard.GetText();
            if (string.IsNullOrEmpty(pasteText)) {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NOTEXTTPASTE"));
                return;
            }

            pasteText = pasteText.Replace("\r\n", "\n");
            pasteText = pasteText.Replace('\r', '\n');
            pasteText = pasteText.TrimEnd(new char[] { '\n' });
            string[] lines = pasteText.Split('\n');

            if (lines.Length != 20) {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_WRONGPASTEVALUE"));
                return;
            }

            int insertRowIndex = 0;

            foreach (string line in lines) {
                this.dgvCircleColumn[1, insertRowIndex].Value = line;

                insertRowIndex++;
            }
        }

        // 梁
        private void btnGirderPaste_Click(object sender, EventArgs e)
        {
            //クリップボードの内容を取得して、行で分ける
            string pasteText = Clipboard.GetText();
            if (string.IsNullOrEmpty(pasteText)) {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NOTEXTTPASTE"));
                return;
            }

            pasteText = pasteText.Replace("\r\n", "\n");
            pasteText = pasteText.Replace('\r', '\n');
            pasteText = pasteText.TrimEnd(new char[] { '\n' });
            string[] lines = pasteText.Split('\n');

            if (lines.Length != 80) {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_WRONGPASTEVALUE"));
                return;
            }

            int insertRowIndex = 0;

            foreach (string line in lines) {
                this.dgvGirder[1, insertRowIndex].Value = line;

                insertRowIndex++;
            }
        }

        // 片持ち梁
        private void btnCantiGirderPaste_Click(object sender, EventArgs e)
        {
            //クリップボードの内容を取得して、行で分ける
            string pasteText = Clipboard.GetText();
            if (string.IsNullOrEmpty(pasteText)) {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NOTEXTTPASTE"));
                return;
            }

            pasteText = pasteText.Replace("\r\n", "\n");
            pasteText = pasteText.Replace('\r', '\n');
            pasteText = pasteText.TrimEnd(new char[] { '\n' });
            string[] lines = pasteText.Split('\n');

            if (lines.Length != 54) {
                System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_WRONGPASTEVALUE"));
                return;
            }

            int insertRowIndex = 0;

            foreach (string line in lines) {
                this.dgvCantiGirder[1, insertRowIndex].Value = line;

                insertRowIndex++;
            }
        }

        #endregion 全ペーストボタン

        #region キー押下(Ctrl + V)

        // 矩形柱
        private void dgvRectangleColumn_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + V(貼り付け)
            if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.V) {
                // クリップボードの内容を取得
                string pasteText = Clipboard.GetText();

                if (string.IsNullOrEmpty(pasteText)) {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NOTEXTTPASTE"));
                    return;
                }

                // 行ごとに分割
                pasteText = pasteText.Replace("\r\n", "\n");
                pasteText = pasteText.Replace('\r', '\n');
                pasteText = pasteText.TrimEnd(new char[] { '\n' });
                string[] lines = pasteText.Split('\n');

                // 現在の行番号(ヘッダーは除く)(複数セル選択時は最終選択セル)
                int currentRowNum = this.dgvRectangleColumn.CurrentCellAddress.Y;

                // 選択しているセルのうち一番上のセル
                if (this.dgvRectangleColumn.SelectedCells.Count > 1) {
                    for (int i = 0; i < this.dgvRectangleColumn.SelectedCells.Count; ++i) {
                        if (currentRowNum > this.dgvRectangleColumn.SelectedCells[i].RowIndex) {
                            currentRowNum = this.dgvRectangleColumn.SelectedCells[i].RowIndex;
                        }
                    }
                }

                int insertRowIndex = 0;

                foreach (string line in lines) {
                    if (this.dgvRectangleColumn.Rows.Count == insertRowIndex + currentRowNum) {
                        break;
                    }

                    this.dgvRectangleColumn[1, insertRowIndex + currentRowNum].Value = line;

                    insertRowIndex++;
                }
            }
        }

        // 円形柱
        private void dgvCircleColumn_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + V
            if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.V) {
                // クリップボードの内容を取得
                string pasteText = Clipboard.GetText();

                if (string.IsNullOrEmpty(pasteText)) {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NOTEXTTPASTE"));
                    return;
                }

                // 行ごとに分割
                pasteText = pasteText.Replace("\r\n", "\n");
                pasteText = pasteText.Replace('\r', '\n');
                pasteText = pasteText.TrimEnd(new char[] { '\n' });
                string[] lines = pasteText.Split('\n');

                // 現在の行番号(ヘッダーは除く)(複数セル選択時は最終選択セル)
                int currentRowNum = this.dgvCircleColumn.CurrentCellAddress.Y;

                if (this.dgvCircleColumn.SelectedCells.Count > 1) {
                    for (int i = 0; i < this.dgvCircleColumn.SelectedCells.Count; ++i) {
                        if (currentRowNum > this.dgvCircleColumn.SelectedCells[i].RowIndex) {
                            currentRowNum = this.dgvCircleColumn.SelectedCells[i].RowIndex;
                        }
                    }
                }

                int insertRowIndex = 0;

                foreach (string line in lines) {
                    if (this.dgvCircleColumn.Rows.Count == insertRowIndex + currentRowNum) {
                        break;
                    }

                    this.dgvCircleColumn[1, insertRowIndex + currentRowNum].Value = line;

                    insertRowIndex++;
                }
            }
        }

        // 梁
        private void dgvGirder_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + V
            if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.V) {
                // クリップボードの内容を取得
                string pasteText = Clipboard.GetText();

                if (string.IsNullOrEmpty(pasteText)) {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NOTEXTTPASTE"));
                    return;
                }

                // 行ごとに分割
                pasteText = pasteText.Replace("\r\n", "\n");
                pasteText = pasteText.Replace('\r', '\n');
                pasteText = pasteText.TrimEnd(new char[] { '\n' });
                string[] lines = pasteText.Split('\n');

                // 現在の行番号(ヘッダーは除く)(複数セル選択時は最終選択セル)
                int currentRowNum = this.dgvGirder.CurrentCellAddress.Y;

                if (this.dgvGirder.SelectedCells.Count > 1) {
                    for (int i = 0; i < this.dgvGirder.SelectedCells.Count; ++i) {
                        if (currentRowNum > this.dgvGirder.SelectedCells[i].RowIndex) {
                            currentRowNum = this.dgvGirder.SelectedCells[i].RowIndex;
                        }
                    }
                }

                int insertRowIndex = 0;

                foreach (string line in lines) {
                    if (this.dgvGirder.Rows.Count == insertRowIndex + currentRowNum) {
                        break;
                    }

                    this.dgvGirder[1, insertRowIndex + currentRowNum].Value = line;

                    insertRowIndex++;
                }
            }
        }

        // 片持ち梁
        private void dgvCantiGirder_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + V
            if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.V) {
                // クリップボードの内容を取得
                string pasteText = Clipboard.GetText();

                if (string.IsNullOrEmpty(pasteText)) {
                    System.Windows.Forms.MessageBox.Show(_CmpAttribute.ResourceText("IDS_ERR_NOTEXTTPASTE"));
                    return;
                }

                // 行ごとに分割
                pasteText = pasteText.Replace("\r\n", "\n");
                pasteText = pasteText.Replace('\r', '\n');
                pasteText = pasteText.TrimEnd(new char[] { '\n' });
                string[] lines = pasteText.Split('\n');

                // 現在の行番号(ヘッダーは除く)(複数セル選択時は最終選択セル)
                int currentRowNum = this.dgvCantiGirder.CurrentCellAddress.Y;

                if (this.dgvCantiGirder.SelectedCells.Count > 1) {
                    for (int i = 0; i < this.dgvCantiGirder.SelectedCells.Count; ++i) {
                        if (currentRowNum > this.dgvCantiGirder.SelectedCells[i].RowIndex) {
                            currentRowNum = this.dgvCantiGirder.SelectedCells[i].RowIndex;
                        }
                    }
                }

                int insertRowIndex = 0;

                foreach (string line in lines) {
                    if (this.dgvCantiGirder.Rows.Count == insertRowIndex + currentRowNum) {
                        break;
                    }

                    this.dgvCantiGirder[1, insertRowIndex + currentRowNum].Value = line;

                    insertRowIndex++;
                }
            }
        }

        #endregion キー押下(Ctrl + V)

        // OKボタン
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;

            // 柱
            for (int i = 0; i < this.dgvRectangleColumn.Rows.Count; ++i) {
                try {
                    _DicRectangleColumn[this.dgvRectangleColumn[0, i].Value.ToString()] = this.dgvRectangleColumn[1, i].Value.ToString();
                }
                catch {
                    continue;
                }
            }

            // 円柱
            for (int i = 0; i < this.dgvCircleColumn.Rows.Count; ++i) {
                try {
                    _DicRoundColumn[this.dgvCircleColumn[0, i].Value.ToString()] = this.dgvCircleColumn[1, i].Value.ToString();
                }
                catch {
                    continue;
                }
            }

            // 梁
            for (int i = 0; i < this.dgvGirder.Rows.Count; ++i) {
                try {
                    _DicGirder[this.dgvGirder[0, i].Value.ToString()] = this.dgvGirder[1, i].Value.ToString();
                }
                catch {
                    continue;
                }
            }

            // 片持ち梁
            for (int i = 0; i < this.dgvCantiGirder.Rows.Count; ++i) {
                try {
                    _DicCantiGirder[this.dgvCantiGirder[0, i].Value.ToString()] = this.dgvCantiGirder[1, i].Value.ToString();
                }
                catch {
                    continue;
                }
            }

            this.Close();
        }

        // キャンセルボタン
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            this.Close();
        }

        #endregion Events
    }
}