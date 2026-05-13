using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
using System.Reflection;
namespace SectionListSteel.Setting
{
    public partial class FormAllSetting : Form
    {
        /// <summary>属性</summary>
        private SectionListSteel.Components.Attribute _CmpAttribute;

        private List<Form> _FormList = new List<Form>();

        public SectionListSteel.Setting.FormCommonSetting _FormCommonSetting = null;
        public SectionListSteel.Setting.FormColumnSetting _FormColumnSetting = null;
        public SectionListSteel.Setting.FormSubItemSetting_Post _FormSubItemPost = null;
        public SectionListSteel.Setting.FormBeamSetting _FormBeamSetting = null;
        public SectionListSteel.Setting.FormBeamSetting_Sub _FormSubItemBeam = null;
        public SectionListSteel.Setting.FormBraceSetting _FormBrace = null;

        private int _Pre_TabIndex = 0;

        public FormAllSetting(SectionListSteel.Components.Elements cmpElements, SectionListSteel.Components.Attribute cmpAttribute,
                                 string settingFileName, string settingFileDirectory)
        {
            InitializeComponent();

            _CmpAttribute = cmpAttribute;

            // 文字タイプ
            Collections.Generic.IList<Revit.DB.TextNoteType> txtNoteTypeAry = cmpElements.TxtNoteTypes;
            // 線種タイプ
            Collections.Generic.IList<Revit.DB.GraphicsStyle> graStyleAry = cmpElements.DetailGraStyles;

            _FormCommonSetting = new SectionListSteel.Setting.FormCommonSetting(this, cmpAttribute, settingFileName, settingFileDirectory, txtNoteTypeAry, graStyleAry);
            _FormColumnSetting = new SectionListSteel.Setting.FormColumnSetting(this, cmpAttribute, settingFileName, settingFileDirectory);
            _FormSubItemPost = new SectionListSteel.Setting.FormSubItemSetting_Post(this, cmpAttribute, settingFileName, settingFileDirectory);
            _FormBeamSetting = new SectionListSteel.Setting.FormBeamSetting(this, cmpAttribute, settingFileName, settingFileDirectory);
            _FormSubItemBeam = new SectionListSteel.Setting.FormBeamSetting_Sub(this, cmpAttribute, settingFileName, settingFileDirectory);
            _FormBrace = new SectionListSteel.Setting.FormBraceSetting(this, cmpAttribute, settingFileName, settingFileDirectory);

            _FormList.Add(_FormCommonSetting);
            _FormList.Add(_FormColumnSetting);
            _FormList.Add(_FormSubItemPost);
            _FormList.Add(_FormBeamSetting);
            _FormList.Add(_FormSubItemBeam);
            _FormList.Add(_FormBrace);
        }

        public void BtnEnabledChange(bool enableBool)
        {
            this.btnOverWriteSave.Enabled = enableBool;
            this.btnSaveAs.Enabled = enableBool;
        }

        private void SetText()
        {
            this.Text = _CmpAttribute.ResourceText("IDS_TXT_ALLSETTING");

            this.btnOverWriteSave.Text = _CmpAttribute.ResourceText("IDS_TXT_OVERWRITESAVE");
            this.btnSaveAs.Text = _CmpAttribute.ResourceText("IDS_TXT_SAVEAS");
            this.btnEnd.Text = _CmpAttribute.ResourceText("IDS_TXT_END");
        }

        private void FormAllSetting_Load(object sender, EventArgs e)
        {
            SetText();

            if (tabControl.TabPages.Count == 0)
            {
                AddFormToTabControl();
            }
            else
            {
                _FormCommonSetting.ShowData();
                _FormColumnSetting.ShowData();
                _FormSubItemPost.ShowData();
                _FormBeamSetting.ShowData();
                _FormSubItemBeam.ShowData();
                _FormBrace.ShowLoad();
            }

            //Add event
            tabControl.Selecting += TabControl_Selecting;
            tabControl.SelectedIndexChanged += TabControl_SelectedIndexChanged;
        }

        private void TabControl_Selecting(object sender, CancelEventArgs e)
        {
            if (_Pre_TabIndex == 0 && _FormCommonSetting.AllInputJudge == false)
                e.Cancel = true;
            else if (_Pre_TabIndex == 1 && _FormColumnSetting.AllInputJudge == false)
                e.Cancel = true;
            else if (_Pre_TabIndex == 2 && _FormSubItemPost.AllInputJudge == false)
                e.Cancel = true;
            else if (_Pre_TabIndex == 3 && _FormBeamSetting.AllInputJudge == false)
                e.Cancel = true;
            else if (_Pre_TabIndex == 4 && _FormSubItemBeam.AllInputJudge == false)
                e.Cancel = true;
            else if (_Pre_TabIndex == 5 && _FormBrace.AllInputJudge == false)
                e.Cancel = true;
        }

        private void TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            _Pre_TabIndex = tabControl.SelectedIndex;
        }

        public void AddFormToTabControl()
        {
            tabControl.TabPages.Clear();
            foreach (var item in _FormList)
            {
                item.FormBorderStyle = FormBorderStyle.None;
                item.BackColor = System.Drawing.SystemColors.ButtonHighlight;

                item.TopLevel = false;

                item.Visible = true;

                TabPage tab = new TabPage(item.Text);

                item.Parent = tab;

                tabControl.TabPages.Add(tab);

                item.Location = new Point(0, 0);

                tab.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            }
        }

        /// ================================================================================
        /// <summary>上書き保存</summary>
        /// ================================================================================
        private void btnOverWriteSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _FormCommonSetting.SettingResult = 6;

            this.Close();
        }

        /// ================================================================================
        /// <summary>名前を付けて保存</summary>
        /// ================================================================================
        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Yes;
            _FormCommonSetting.SettingResult = 7;

            this.Close();
        }

        /// ================================================================================
        /// <summary>終了</summary>
        /// ================================================================================
        private void btnEnd_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;

            this.Close();
        }
    }
}