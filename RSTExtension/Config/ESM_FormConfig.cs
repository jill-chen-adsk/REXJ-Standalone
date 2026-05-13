using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using RvtExtApp = RSTExtension;
using System.Reflection;

namespace RSTExtension.Config
{
    /// ================================================================================
    /// <summary>画面 設定</summary>
    /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
    /// ================================================================================
    public partial class ESM_FormConfig : Form
    {
        // メンバ変数

        #region Memeber Variables

        /// <summary>属性</summary>
        private RvtExtApp.Components.Attribute _CmpAttribute;

        /// <summary>データテーブル - ビュー</summary>
        private RvtExtApp.Entities.Exclusion.DtView _EntDtView;

        /// <summary>データテーブル - 壁タグ</summary>
        private RvtExtApp.Entities.Exclusion.DtWallTag _EntDtWallTag;

        /// <summary>データテーブル - 柱タグ</summary>
        private RvtExtApp.Entities.Exclusion.DtColumnTag _EntDtColumnTag;

        /// <summary>データテーブル - 梁タグ</summary>
        private RvtExtApp.Entities.Exclusion.DtBeamTag _EntDtBeamTag;

        /// <summary>データテーブル - スラブタグ</summary>
        private RvtExtApp.Entities.Exclusion.DtSlabTag _EntDtSlabTag;

        /// <summary>データテーブル - 基礎タグ</summary>
        private RvtExtApp.Entities.Exclusion.DtFoundationTag _EntDtFoundationTag;

        #endregion Memeber Variables

        // コンストラクタ

        #region Constructor

        /// ================================================================================
        /// <summary>コンストラクタ</summary>
        ///
        /// <param name="cmpAttribute"      >属性</param>
        /// <param name="entDtView"         >データテーブル - ビュー</param>
        /// <param name="entDtWallTag"      >データテーブル - 壁タグ</param>
        /// <param name="entDtColumnTag"    >データテーブル - 柱タグ</param>
        /// <param name="entDtBeamTag"      >データテーブル - 梁タグ</param>
        /// <param name="entDtSlabTag"      >データテーブル - スラブタグ</param>
        /// <param name="entDtFoundationTag">データテーブル - 基礎タグ</param>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        public ESM_FormConfig(RvtExtApp.Components.Attribute cmpAttribute,
                          RvtExtApp.Entities.Exclusion.DtView entDtView,
                          RvtExtApp.Entities.Exclusion.DtWallTag entDtWallTag,
                          RvtExtApp.Entities.Exclusion.DtColumnTag entDtColumnTag,
                          RvtExtApp.Entities.Exclusion.DtBeamTag entDtBeamTag,
                          RvtExtApp.Entities.Exclusion.DtSlabTag entDtSlabTag,
                          RvtExtApp.Entities.Exclusion.DtFoundationTag entDtFoundationTag)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;
            _EntDtView = entDtView;
            _EntDtWallTag = entDtWallTag;
            _EntDtColumnTag = entDtColumnTag;
            _EntDtBeamTag = entDtBeamTag;
            _EntDtSlabTag = entDtSlabTag;
            _EntDtFoundationTag = entDtFoundationTag;

            SetText();
            SetData();
        }

        #endregion Constructor

        // メンバ関数

        #region Member Functions

        /// ================================================================================
        /// <summary>フォームの文字設定</summary>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        ///         <p>2021/10/12 Modified GSA,Inc. Shinichi Ishii<p></history>
        /// ================================================================================
        private
        void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_EXCLUSIONSPECIALMENTION");

            this.lblMark.Text = _CmpAttribute.ResourceText("IDS_TXT_MARK");
            this.lblLevel.Text = _CmpAttribute.ResourceText("IDS_TXT_LEVEL");

            this.gpbFrame.Text = _CmpAttribute.ResourceText("IDS_TXT_FRAME");
            this.lblGirder.Text = _CmpAttribute.ResourceText("IDS_TXT_GIRDER");
            this.lblBeam.Text = _CmpAttribute.ResourceText("IDS_TXT_BEAM");
            this.lblHbrace.Text = _CmpAttribute.ResourceText("IDS_TXT_HBRACE");
            this.lblVbrace.Text = _CmpAttribute.ResourceText("IDS_TXT_VBRACE");
            this.lblOther.Text = _CmpAttribute.ResourceText("IDS_TXT_OTHER");

            this.gpbWall.Text = _CmpAttribute.ResourceText("IDS_TXT_WALL");
            this.lblWallExt.Text = _CmpAttribute.ResourceText("IDS_TXT_EXTWALL");
            this.lblWallInt.Text = _CmpAttribute.ResourceText("IDS_TXT_INTWALL");

            this.gpbSlab.Text = _CmpAttribute.ResourceText("IDS_TXT_SLAB");
            this.lblSlab.Text = _CmpAttribute.ResourceText("IDS_TXT_SLAB");

            this.gpbColumn.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMN");
            this.lblColumn.Text = _CmpAttribute.ResourceText("IDS_TXT_COLUMN");

            this.gpbFoundation.Text = _CmpAttribute.ResourceText("IDS_TXT_FOUNDATION");
            this.lblFoundation.Text = _CmpAttribute.ResourceText("IDS_TXT_FOUNDATION");

            this.btnDispReset.Text = _CmpAttribute.ResourceText("IDS_TXT_DISPRESET");
            this.btnNonDisp.Text = _CmpAttribute.ResourceText("IDS_TXT_NONDISP");
            this.btnCancel.Text = _CmpAttribute.ResourceText("IDS_TXT_CANCEL");

            var iconStream = typeof(ESM_FormConfig).Assembly
                .GetManifestResourceStream("RSTExtension.Resources.Images.IDI_SUBS_ICON.ico");
            if ( iconStream != null ) this.Icon = new System.Drawing.Icon( iconStream ) ;
        }

        /// ================================================================================
        /// <summary>フォームのデータ設定</summary>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        ///         <p>2021/10/12 Modified GSA,Inc. Shinichi Ishii<p></history>
        /// ================================================================================
        private
        void SetData()
        {
            // 初期化
            string sValue = "";
            System.Collections.Generic.IList<string> dt = null;

            // ========== 既存値 壁 ==========
            dt = _EntDtView.WallData;

            // ---------- 外壁 ----------

            // 外壁 - 符号 - 値
            sValue = dt[0];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtWallExtMark.Text = sValue;
            dt[0] = sValue;

            // 外壁 - レベル - 値
            sValue = dt[1];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtWallExtLevel.Text = sValue;
            dt[1] = sValue;

            // ---------- 内壁 ----------

            // 内壁 - 符号 - 値
            sValue = dt[2];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtWallIntMark.Text = sValue;
            dt[2] = sValue;

            // 内壁 - レベル - 値
            sValue = dt[3];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtWallIntLevel.Text = sValue;
            dt[3] = sValue;

            // ========== 既存値 柱 ==========
            dt = _EntDtView.ColumnData;

            // ---------- 柱 ----------

            // 柱 - 符号 - 値
            sValue = dt[0];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtColumnMark.Text = sValue;
            dt[0] = sValue;

            // 柱 - レベル - 値
            sValue = dt[1];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtColumnLevel.Text = sValue;
            dt[1] = sValue;

            // ========== 既存値 梁 ==========
            dt = _EntDtView.BeamData;

            // ---------- 大梁 ----------

            // 大梁 - 符号 - 値
            sValue = dt[0];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtGirderMark.Text = sValue;
            dt[0] = sValue;

            // 大梁 - レベル - 値
            sValue = dt[1];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtGirderLevel.Text = sValue;
            dt[1] = sValue;

            // ---------- 小梁 ----------

            // 小梁 - 符号 - 値
            sValue = dt[2];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtBeamMark.Text = sValue;
            dt[2] = sValue;

            // 小梁 - レベル - 値
            sValue = dt[3];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtBeamLevel.Text = sValue;
            dt[3] = sValue;

            // ---------- 鉛直ブレース ----------

            // 鉛直ブレース - 符号 - 値
            sValue = dt[4];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtVbraceMark.Text = sValue;
            dt[4] = sValue;

            // 鉛直ブレース - レベル - 値
            sValue = dt[5];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtVbraceLevel.Text = sValue;
            dt[5] = sValue;

            // ---------- 水平ブレース ----------

            // 水平ブレース - 符号 - 値
            sValue = dt[6];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtHbraceMark.Text = sValue;
            dt[6] = sValue;

            // 水平ブレース - レベル - 値
            sValue = dt[7];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtHbraceLevel.Text = sValue;
            dt[7] = sValue;

            // ---------- その他 ----------

            // その他 - 符号 - 値
            sValue = dt[8];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtOtherMark.Text = sValue;
            dt[8] = sValue;

            // その他 - レベル - 値
            sValue = dt[9];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtOtherLevel.Text = sValue;
            dt[9] = sValue;

            // ========== 既存値 スラブ ==========
            dt = _EntDtView.SlabData;

            // ---------- スラブ ----------

            // スラブ - 符号 - 値
            sValue = dt[0];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtSlabMark.Text = sValue;
            dt[0] = sValue;

            // スラブ - レベル - 値
            sValue = dt[1];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtSlabLevel.Text = sValue;
            dt[1] = sValue;

            // ========== 既存値 基礎 ==========
            dt = _EntDtView.FoundationData;

            // ---------- 構造基礎 ----------

            // 構造基礎 - 符号 - 値
            sValue = dt[0];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtFoundationMark.Text = sValue;
            dt[0] = sValue;

            // 構造基礎 - レベル - 値
            sValue = dt[1];
            if (sValue == null)
            {
                sValue = "";
            }
            this.txtFoundationLevel.Text = sValue;
            dt[1] = sValue;
        }

        /// ================================================================================
        /// <summary>フォームのデータ取得</summary>
        ///
        /// <history><p>2011/11/26 Created GSA,Inc. Shinichi Ishii</p>
        ///         <p>2021/10/12 Modified GSA,Inc. Shinichi Ishii</p></history>
        /// ================================================================================
        private
        void GetData()
        {
            // 初期化
            string sValue = "";
            System.Collections.Generic.IList<string> dt = null;

            // ========== 既存値 壁 ==========
            dt = _EntDtView.WallData;

            // ---------- 外壁 ----------

            // 外壁 - 符号 - 値
            sValue = this.txtWallExtMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[0] = sValue;

            // 外壁 - レベル - 値
            sValue = this.txtWallExtLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[1] = sValue;

            // ---------- 内壁 ----------

            // 内壁 - 符号 - 値
            sValue = this.txtWallIntMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[2] = sValue;

            // 内壁 - レベル - 値
            sValue = this.txtWallIntLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[3] = sValue;

            // ========== 既存値 柱 ==========
            dt = _EntDtView.ColumnData;

            // ---------- 柱 ----------

            // 柱 - 符号 - 値
            sValue = this.txtColumnMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[0] = sValue;

            // 柱 - レベル - 値
            sValue = this.txtColumnLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[1] = sValue;

            // ========== 既存値 梁 ==========
            dt = _EntDtView.BeamData;

            // ---------- 大梁 ----------

            // 大梁 - 符号 - 値
            sValue = this.txtGirderMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[0] = sValue;

            // 大梁 - レベル - 値
            sValue = this.txtGirderLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[1] = sValue;

            // ---------- 小梁 ----------

            // 小梁 - 符号 - 値
            sValue = this.txtBeamMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[2] = sValue;

            // 小梁 - レベル - 値
            sValue = this.txtBeamLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[3] = sValue;

            // ---------- 鉛直ブレース ----------

            // 鉛直ブレース - 符号 - 値
            sValue = this.txtVbraceMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[4] = sValue;

            // 鉛直ブレース - レベル - 値
            sValue = this.txtVbraceLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[5] = sValue;

            // ---------- 水平ブレース ----------

            // 水平ブレース - 符号 - 値
            sValue = this.txtHbraceMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[6] = sValue;

            // 水平ブレース - レベル - 値
            sValue = this.txtHbraceLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[7] = sValue;

            // ---------- その他 ----------

            // その他 - 符号 - 値
            sValue = this.txtOtherMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[8] = sValue;

            // その他 - レベル - 値
            sValue = this.txtOtherLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[9] = sValue;

            // ========== 既存値 スラブ ==========
            dt = _EntDtView.SlabData;

            // ---------- スラブ ----------

            // スラブ - 符号 - 値
            sValue = this.txtSlabMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[0] = sValue;

            // スラブ - レベル - 値
            sValue = this.txtSlabLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[1] = sValue;

            // ========== 既存値 基礎 ==========
            dt = _EntDtView.FoundationData;

            // ---------- 構造基礎 ----------

            // 構造基礎 - 符号 - 値
            sValue = this.txtFoundationMark.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[0] = sValue;

            // 構造基礎 - レベル - 値
            sValue = this.txtFoundationLevel.Text;
            if (sValue == null)
            {
                sValue = "";
            }
            dt[1] = sValue;

            // 要素 - 壁
            _EntDtWallTag.GetData(_EntDtView.WallData);

            // 要素 - 柱
            _EntDtColumnTag.GetData(_EntDtView.ColumnData);

            // 要素 - 梁
            _EntDtBeamTag.GetData(_EntDtView.BeamData);

            // 要素 - スラブ
            _EntDtSlabTag.GetData(_EntDtView.SlabData);

            // 要素 - 基礎
            _EntDtFoundationTag.GetData(_EntDtView.FoundationData);
        }

        #endregion Member Functions

        // プロパティ

        // イベント

        #region Events

        /// ================================================================================
        /// <summary>Handles the Click event of the btnDispReset control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/11/26 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnDispReset_Click(object sender, EventArgs e)
        {
            // データ取得
            GetData();

            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.Close();
        }

        /// ================================================================================
        /// <summary>Handles the Click event of the btnNonDisp control</summary>
        ///
        /// <param name="sender">The source of the event.</param>
        /// <param name="e"     >The <see cref="System::EventArgs"/> instance containing the event data.</param>
        ///
        /// <history>2011/12/07 Created GSA,Inc. Shinichi Ishii</history>
        /// ================================================================================
        private
        void btnNonDisp_Click(object sender, EventArgs e)
        {
            // データ取得
            GetData();

            this.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Close();
        }

        #endregion Events
    }
}
