using System;
using System.IO;
using System.Xml;
using Collections = System.Collections;
using Revit = Autodesk.Revit;
namespace Quantity.Components
{
    /// ================================================================================
    /// <summary>�p�����[�^ - �o��</summary>
    /// ================================================================================
    public class OutPutParam
    {
        /// <summary>0:�_�N�g 1:�z��</summary>
        public int iType;

        /// <summary>�X�y�[�X��</summary>
        public String SpaceName;

        /// <summary>�X�y�[�X�ԍ�</summary>
        public String SpaceNumber;

        /// <summary>�V�X�e���^�C�v</summary>
        public String SystemType;

        /// <summary>�V�X�e����</summary>
        public String SystemName;

        /// <summary>0:�p 1:��</summary>
        public int iShape;

        /// <summary>��(�p�̏ꍇ)</summary>
        public double Width;

        /// <summary>����(�p�̏ꍇ)</summary>
        public double Height;

        /// <summary>���a(�ۂ̏ꍇ)</summary>
        public double Diameter;

        /// <summary>����</summary>
        public double Length;

        /// <summary>0:���� 1:�G��</summary>
        public int iVertical;

        public OutPutParam()
        {
            iType = -1;
            SpaceName = "";
            SpaceNumber = "";
            SystemType = "";
            SystemName = "";
            iShape = -1;
            Width = 0;
            Height = 0;
            Diameter = 0;
            Length = 0;
            iVertical = -1;
        }
    }

    /// ================================================================================
    /// <summary>�p�����[�^</summary>
    /// ================================================================================
    public class Parameters
    {
        // �����o�ϐ�
        #region Member Variables

        /// <summary>feet��mm �ϊ��萔</summary>
        public const double FTOMM = 304.8;

        /// <summary>Active UI document</summary>
        private readonly Revit.UI.UIDocument _rvtUIDoc;

        /// <summary>Associated database document.</summary>
        public Revit.DB.Document RvtDBDoc => _rvtUIDoc.Document;

        /// <summary>����</summary>
        private Quantity.Components.Attribute _CmpAttribute;

        /// <summary>�W�����L�p�����[�^�t�@�C����</summary>
        private string _ShParamDefaultFileName;

        /// <summary>���L�p�����[�^�t�H���_��</summary>
        private string _ShParamFolderName;

        /// <summary>���L�p�����[�^�t�@�C����</summary>
        private string _ShParamFileName;

        /// <summary>���L�p�����[�^�O���[�v��</summary>
        private string _ShParamGroupName;

        /// <summary>�r���[�͈�</summary>
        private Revit.DB.PlanViewRange _ViewRange;

        /// <summary>�_�N�g���X�g</summary>
        public Collections.Generic.IList<String> _OutPutDuctHeader = new String[] { "�V�X�e���^�C�v,", "�V�X�e����,", "�X�y�[�X��,", "�X�y�[�X�ԍ�,", "��,", "����,", "���a,", "����,", "���l" };

        public Collections.Generic.IList<OutPutParam> _OutPutDuctList;

        /// <summary>�z�ǃ��X�g</summary>
        public Collections.Generic.IList<String> _OutPutPipeHeader = new String[] { "�V�X�e���^�C�v,", "�V�X�e����,", "�X�y�[�X��,", "�X�y�[�X�ԍ�,", "���a,", "����,", "���l" };

        public Collections.Generic.IList<OutPutParam> _OutPutPipeList;

        /// <summary>�X�y�[�X��[����</summary>
        private Collections.Generic.IDictionary<Revit.DB.Mechanical.Space, double> _SpaceTopElev;

        /// <summary>�X�y�[�X���[����</summary>
        private Collections.Generic.IDictionary<Revit.DB.Mechanical.Space, double> _SpaceBtmElev;

        /// <summary>�A���_�[���CID - ��</summary>
        private Revit.DB.ElementId _UnderLayID_Bottom;

        /// <summary>�A���_�[���CID - ��</summary>
        private Revit.DB.ElementId _UnderLayID_Top;

        #endregion Member Variables

        // �R���X�g���N�^
        #region Constructor

        /// ================================================================================
        /// <summary>�R���X�g���N�^</summary>
        ///
        /// <param name="rvtUIDoc"    >Revit UI�h�L�������g</param>
        /// <param name="cmpAttribute">����</param>
        ///
        /// <history><p>2014/07/14 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2017/07/18 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public Parameters(Revit.UI.UIDocument rvtUIDoc, Quantity.Components.Attribute cmpAttribute)
        {
            _rvtUIDoc = rvtUIDoc;
            _CmpAttribute = cmpAttribute;

            // �f�t�H���g���L�p�����[�^
            _ShParamDefaultFileName = null;
            Revit.DB.DefinitionFile defFile = TryOpenSharedParameterFile();
            if (defFile != null)
            {
                _ShParamDefaultFileName = defFile.Filename;
            }

            // �A�v���P�[�V�����p���L�p�����[�^
            _ShParamFolderName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _ShParamFileName = _CmpAttribute.ResourceText("IDS_SHPARAM_FILE");
            _ShParamGroupName = _CmpAttribute.ResourceText("IDS_SHPARAM_GROUP");

            if (_ShParamDefaultFileName == null)
            {
                _ShParamDefaultFileName = _ShParamFolderName + "\\" + _ShParamFileName;
            }
        }

        #endregion Constructor

        // �����o�֐�
        #region

        /// ================================================================================
        /// <summary>�W�����L�p�����[�^�t�@�C���ݒ�</summary>
        ///
        /// <returns><p>����</p>
        ///             <p>True  = ����</p>
        ///             <p>False = ���s</p></returns>
        ///
        /// <history>2017/07/18 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool SetSharedParamDefault()
        {
            bool ret = false;

            // ���L�p�����[�^�t�@�C���ݒ�
            Revit.DB.DefinitionFile defFile = TrySetSharedParameterFile(_ShParamDefaultFileName);
            if (defFile != null)
            {
                ret = true;
            }
            return ret;
        }

        public Revit.DB.DefinitionFile SetSharedParameterFile(string folderName, string fileName)
        {
            string path = System.IO.Path.Combine(folderName ?? string.Empty, fileName ?? string.Empty);
            return TrySetSharedParameterFile(path);
        }

        /// ================================================================================
        /// <summary>��`�ݒ�</summary>
        ///
        /// <param name="elem"          >�v�f</param>
        /// <param name="categories"    >�J�e�S��</param>
        /// <param name="defName"       >��`��</param>
        /// <param name="paramType"     >�p�����[�^�^�C�v</param>
        /// <param name="bltParamGroup" >�g���p�����[�^�O���[�v</param>
        /// <param name="visible"       >��</param>
        /// <param name="bindingMode"   ><p>�������[�h</p>
        ///                                 <p>0 = �C���X�^���X</p>
        ///                                 <p>1 = �^�C�v</p></param>
        ///
        /// <returns><p>����</p>
        ///             <p>True  = ����</p>
        ///             <p>False = ���s</p></returns>
        ///
        /// <history>2017/07/18 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool SetDefinition(Revit.DB.Element elem,
                           Collections.Generic.IList<Revit.DB.Category> categories,
                           string defName,
                           Revit.DB.ForgeTypeId paramType,
                           Revit.DB.ForgeTypeId bltParamGroup,
                           bool visible,
                           int bindingMode)
        {
            return TryInsertSharedParameter(elem,
                                          _ShParamFolderName,
                                          _ShParamFileName,
                                          _ShParamGroupName,
                                          categories,
                                          defName,
                                          paramType,
                                          bltParamGroup,
                                          visible,
                                          bindingMode);
        }

        /// ================================================================================
        /// <summary>��`�ݒ�(�I�[�o�[���[�h)</summary>
        ///
        /// <param name="elem"          >�v�f</param>
        /// <param name="category"      >�J�e�S��</param>
        /// <param name="defName"       >��`��</param>
        /// <param name="paramType"     >�p�����[�^�^�C�v</param>
        /// <param name="bltParamGroup" >�g���p�����[�^�O���[�v</param>
        /// <param name="visible"       >��</param>
        /// <param name="bindingMode"   ><p>�������[�h</p>
        ///                                 <p>0 = �C���X�^���X</p>
        ///                                 <p>1 = �^�C�v</p></param>
        ///
        /// <returns><p>����</p>
        ///             <p>True  = ����</p>
        ///             <p>False = ���s</p></returns>
        ///
        /// <history>2017/07/18 Created CST,Co.Ltd. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool SetDefinition(Revit.DB.Element elem,
                           Revit.DB.Category category,
                           string defName,
                           Revit.DB.ForgeTypeId paramType,
                           Revit.DB.ForgeTypeId bltParamGroup,
                           bool visible,
                           int bindingMode)
        {
            Collections.Generic.IList<Revit.DB.Category> categories = new Collections.Generic.List<Revit.DB.Category>();
            categories.Add(category);
            return SetDefinition(elem,
                                 categories,
                                 defName,
                                 paramType,
                                 bltParamGroup,
                                 visible,
                                 bindingMode);
        }

        /// ================================================================================
        /// <summary>�r���[�͈͎擾</summary>
        ///
        /// <param name="viewPlan">���ʃr���[</param>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetViewPlanRange(Revit.DB.ViewPlan viewPlan)
        {
            _ViewRange = viewPlan.GetViewRange();
        }

        /// ================================================================================
        /// <summary>�r���[�͈̓��x���擾</summary>
        ///
        /// <param name="planViewPlane">�r���[�͈̖͂�</param>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.Level GetViewRangeLevel(Revit.DB.PlanViewPlane planViewPlane)
        {
            Revit.DB.Level ret = null;

            Revit.DB.ElementId lvlId = _ViewRange.GetLevelId(planViewPlane);

            Revit.DB.Element elem = RvtDBDoc.GetElement(lvlId);

            Revit.DB.Level lvl = elem as Revit.DB.Level;

            ret = lvl;

            return ret;
        }

        /// ================================================================================
        /// <summary>�X�y�[�X�㉺�����擾</summary>
        ///
        /// <param name="spaces">�X�y�[�X</param>
        ///
        /// <history>2015/01/29 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetSpaceElev(Collections.Generic.IList<Revit.DB.Mechanical.Space> spaces)
        {
            _SpaceTopElev = new Collections.Generic.Dictionary<Revit.DB.Mechanical.Space, double>();
            _SpaceBtmElev = new Collections.Generic.Dictionary<Revit.DB.Mechanical.Space, double>();

            foreach (Revit.DB.Mechanical.Space space in spaces)
            {
                double top = GetSpaceTopHeight(space);
                double btm = GetSpaceBottomHeight(space);

                _SpaceTopElev.Add(space, top);
                _SpaceBtmElev.Add(space, btm);
            }
        }

        /// ================================================================================
        /// <summary>�X�y�[�X��[����</summary>
        ///
        /// <param name="space">�X�y�[�X</param>
        ///
        /// <history><p>2014/09/25 Created GSA, Inc. Ryo Kuroda</p>
        ///           <p>2015/01/20 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        double GetSpaceTopHeight(Revit.DB.Mechanical.Space space)
        {
            // �߂�l
            double ret = 0;

            // ��[���x��
            Revit.DB.ElementId topLvlId = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_UPPER_LEVEL).AsElementId();
            Revit.DB.Level topLvl = space.Document.GetElement(topLvlId) as Revit.DB.Level;
            double topOffset = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_UPPER_OFFSET).AsDouble();

            if (topLvl != null)
            {
                ret = topLvl.Elevation + topOffset;
            }
            else
            {
                // ���[���x��
                Revit.DB.ElementId btmLvlId = space.LevelId;
                Revit.DB.Level btmLvl = space.Document.GetElement(btmLvlId) as Revit.DB.Level;

                ret = btmLvl.Elevation + topOffset;
            }

            // �v���i���łȂ��ꍇ�A���[�𐳂Ƃ��ĕ␳
            Revit.DB.Parameter parSpaceType = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_SPACE_TYPE_PARAM);

            if (parSpaceType.AsInteger() != 104)
            {
                // �̐ρ��ʐ�
                Revit.DB.Parameter parVolume = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_VOLUME);
                Revit.DB.Parameter parArea = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_AREA);

                double volume = parVolume.AsDouble();
                double area = parArea.AsDouble();

                double height = volume / area;

                double btm = GetSpaceBottomHeight(space);

                double top = btm + height;

                if (top < ret)
                {
                    ret = top;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>�X�y�[�X���[����</summary>
        ///
        /// <param name="space">�X�y�[�X</param>
        ///
        /// <history><p>2014/09/25 Created GSA, Inc. Ryo Kuroda</p>
        ///           <p>2015/01/20 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        double GetSpaceBottomHeight(Revit.DB.Mechanical.Space space)
        {
            // �߂�l
            double ret = 0;

            // ���[���x��
            Revit.DB.ElementId btmLvlId = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_LEVEL_ID).AsElementId();
            Revit.DB.Level btmLvl = space.Document.GetElement(btmLvlId) as Revit.DB.Level;
            double btmOffset = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_LOWER_OFFSET).AsDouble();

            if (btmLvl != null)
            {
                ret = btmLvl.Elevation + btmOffset;
            }
            else
            {
                btmLvlId = space.LevelId;
                btmLvl = space.Document.GetElement(btmLvlId) as Revit.DB.Level;

                ret = btmLvl.Elevation + btmOffset;
            }

            // �v���i���̏ꍇ�A��[�𐳂Ƃ��ĕ␳
            Revit.DB.Parameter parSpaceType = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_SPACE_TYPE_PARAM);

            if (parSpaceType.AsInteger() == 104)
            {
                double top = 0;

                // ��[���x��
                Revit.DB.ElementId topLvlId = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_UPPER_LEVEL).AsElementId();
                Revit.DB.Level topLvl = space.Document.GetElement(topLvlId) as Revit.DB.Level;
                double topOffset = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_UPPER_OFFSET).AsDouble();

                if (topLvl != null)
                {
                    top = topLvl.Elevation + topOffset;
                }
                else
                {
                    top = btmLvl.Elevation + topOffset;
                }

                // �̐ρ��ʐ�
                Revit.DB.Parameter parVolume = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_VOLUME);
                Revit.DB.Parameter parArea = space.get_Parameter(Revit.DB.BuiltInParameter.ROOM_AREA);

                double volume = parVolume.AsDouble();
                double area = parArea.AsDouble();

                double height = volume / area;

                double btm = top - height;

                if (ret < btm)
                {
                    ret = btm;
                }
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>�X�y�[�X��[����</summary>
        ///
        /// <param name="space">�X�y�[�X</param>
        ///
        /// <history><p>2015/01/29 Created GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        double GetSpaceTopElev(Revit.DB.Mechanical.Space space)
        {
            double ret = 0;

            if (_SpaceTopElev.ContainsKey(space))
            {
                ret = _SpaceTopElev[space];
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>�X�y�[�X���[����</summary>
        ///
        /// <param name="space">�X�y�[�X</param>
        ///
        /// <history><p>2015/01/29 Created GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        double GetSpaceBtmElev(Revit.DB.Mechanical.Space space)
        {
            double ret = 0;

            if (_SpaceBtmElev.ContainsKey(space))
            {
                ret = _SpaceBtmElev[space];
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>�A���_�[���C��\��</summary>
        ///
        /// <param name="view">�r���[</param>
        ///
        /// <history><p>2015/12/03 Created GSA, Inc. Ryo Kuroda</p>
        ///           <p>2017/07/18 Modified CST,Co.Ltd. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        void HideUnderLay(Revit.DB.View view)
        {
            try
            {
                // ���ʃA���_�[���CID
                Revit.DB.Parameter parUnderLayIdBtm = view.get_Parameter(Revit.DB.BuiltInParameter.VIEW_UNDERLAY_BOTTOM_ID);

                if (parUnderLayIdBtm != null && parUnderLayIdBtm.IsReadOnly == false)
                {
                    Revit.DB.ElementId underlayId = parUnderLayIdBtm.AsElementId();

                    if (underlayId != null)
                    {
                        _UnderLayID_Bottom = underlayId;

                        Revit.DB.ElementId eId = new Revit.DB.ElementId(-1);

                        parUnderLayIdBtm.Set(eId);
                    }
                }

                // ��ʃA���_�[���CID
                Revit.DB.Parameter parUnderLayIdTop = view.get_Parameter(Revit.DB.BuiltInParameter.VIEW_UNDERLAY_TOP_ID);

                if (parUnderLayIdTop != null && parUnderLayIdTop.IsReadOnly == false)
                {
                    Revit.DB.ElementId underlayId = parUnderLayIdTop.AsElementId();

                    if (underlayId != null)
                    {
                        _UnderLayID_Top = underlayId;

                        Revit.DB.ElementId eId = new Revit.DB.ElementId(-1);

                        parUnderLayIdTop.Set(eId);
                    }
                }
            }
            catch
            {
            }
        }

        /// ================================================================================
        /// <summary>�A���_�[���C�\��</summary>
        ///
        /// <param name="view">�r���[</param>
        ///
        /// <hisotry><p>2015/12/03 Created GSA, Inc. Ryo Kuroda</p>
        ///           <p>2017/07/18 Modified CST,Co.Ltd. Ryo Kuroda</p></hisotry>
        /// ================================================================================
        public
        void UnHideUnderLay(Revit.DB.View view)
        {
            try
            {
                // ���ʃA���_�[���CID
                Revit.DB.Parameter parUnderLayIdBtm = view.get_Parameter(Revit.DB.BuiltInParameter.VIEW_UNDERLAY_BOTTOM_ID);

                if (parUnderLayIdBtm != null && parUnderLayIdBtm.IsReadOnly == false)
                {
                    if (_UnderLayID_Bottom != null)
                    {
                        parUnderLayIdBtm.Set(_UnderLayID_Bottom);
                    }
                }

                // ��ʃA���_�[���CID
                Revit.DB.Parameter parUnderLayIdTop = view.get_Parameter(Revit.DB.BuiltInParameter.VIEW_UNDERLAY_TOP_ID);

                if (parUnderLayIdTop != null && parUnderLayIdTop.IsReadOnly == false)
                {
                    if (_UnderLayID_Top != null)
                    {
                        parUnderLayIdTop.Set(_UnderLayID_Top);
                    }
                }
            }
            catch
            {
            }
        }

        /// ================================================================================
        /// <summary>�_�N�g�T�C�Y�擾</summary>
        ///
        /// <param name="size">�T�C�Y</param>
        /// <param name="size1">�T�C�Y1</param>
        /// <param name="size2">�T�C�Y2</param>
        ///
        /// <history>2015/12/25 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        void GetDuctSize(string size,
                         ref string size1,
                         ref string size2)
        {
            // �p�^
            if (size.Contains("x"))
            {
                size1 = size.Substring(0, size.IndexOf("x"));
                size2 = size.Substring(size.LastIndexOf("x") + 1);

                if (size1.Contains(" "))
                {
                    size1 = size1.Substring(0, size1.IndexOf(" "));
                }
                if (size2.Contains(" "))
                {
                    size2 = size2.Substring(0, size2.IndexOf(" "));
                }
            }
            // �ی^
            else if (size.Contains("o"))
            {
                size1 = size.Substring(0, size.IndexOf("o"));

                if (size1.Contains(" "))
                {
                    size1 = size1.Substring(0, size1.IndexOf(" "));
                }
            }
            // �I�[�o���^
            else if (size.Contains("/"))
            {
                size1 = size.Substring(0, size.IndexOf("/"));
                size2 = size.Substring(size.LastIndexOf("/") + 1);

                if (size1.Contains(" "))
                {
                    size1 = size1.Substring(0, size1.IndexOf(" "));
                }
                if (size2.Contains(" "))
                {
                    size2 = size2.Substring(0, size2.IndexOf(" "));
                }
            }
            // �l�̂�
            else
            {
                size1 = size;
            }
        }

        /// ================================================================================
        /// <summary>�z�ǂ̎d�l��r</summary>
        ///
        /// <param name="pipe1">��z��</param>
        /// <param name="pipe2">��r�z��</param>
        ///
        /// <history><p>2015/12/02 Created GSA,Inc. Ryo Kuroda</p>
        ///           <p>2015/12/18 Modified GSA,Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        bool ComparePipeShiyo(Revit.DB.Plumbing.Pipe pipe1,
                              Revit.DB.Plumbing.Pipe pipe2)
        {
            // �߂�l
            bool ret = false;

            // �z�ǃV�X�e���^�C�v���ƃC���X�^���X�̒l���r
            // (�����^�C�v�Ȃ�^�C�v�p�����[�^�̒l�͓���)

            try
            {
                // �V�X�e���^�C�v��
                string systemTypeName1 = pipe1.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString();
                string systemTypeName2 = pipe2.get_Parameter(Revit.DB.BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM).AsValueString();

                if (systemTypeName1 != systemTypeName2)
                {
                    return ret;
                }

                // �ψ�
                Revit.DB.Parameter parTaiatsu1 = pipe1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_TAIATSU_TOKKI"));
                Revit.DB.Parameter parTaiatsu2 = pipe2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_TAIATSU_TOKKI"));

                if (parTaiatsu1.AsString() != parTaiatsu2.AsString())
                {
                    return ret;
                }

                // �n��
                Revit.DB.Parameter parKeito1 = pipe1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KEITO_TOKKI"));
                Revit.DB.Parameter parKeito2 = pipe2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KEITO_TOKKI"));

                if (parKeito1.AsString() != parKeito2.AsString())
                {
                    return ret;
                }

                // �h��
                Revit.DB.Parameter parToso1 = pipe1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_TOSO_TOKKI"));
                Revit.DB.Parameter parToso2 = pipe2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_TOSO_TOKKI"));

                if (parToso1.AsString() != parToso2.AsString())
                {
                    return ret;
                }

                ret = true;
            }
            catch
            {
                return ret;
            }

            return ret;
        }

        /// ================================================================================
        /// <summary>�_�N�g�̎d�l��r</summary>
        ///
        /// <param name="duct1">��_�N�g</param>
        /// <param name="duct2">��r�_�N�g</param>
        ///
        /// <history>2015/12/18 Created GSA,Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        bool CompareDuctShiyo(Revit.DB.Mechanical.Duct duct1,
                              Revit.DB.Mechanical.Duct duct2)
        {
            // �߂�l
            bool ret = false;

            // �z�ǃV�X�e���^�C�v���ƃC���X�^���X�̒l���r
            // (�����^�C�v�Ȃ�^�C�v�p�����[�^�̒l�͓���)

            try
            {
                // �V�X�e���^�C�v��
                string systemTypeName1 = duct1.get_Parameter(Revit.DB.BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM).AsValueString();
                string systemTypeName2 = duct2.get_Parameter(Revit.DB.BuiltInParameter.RBS_DUCT_SYSTEM_TYPE_PARAM).AsValueString();

                if (systemTypeName1 != systemTypeName2)
                {
                    return ret;
                }

                // �H�@
                Revit.DB.Parameter parKoho1 = duct1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KOHO_TOKKI"));
                Revit.DB.Parameter parKoho2 = duct2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KOHO_TOKKI"));

                if (parKoho1.AsString() != parKoho2.AsString())
                {
                    return ret;
                }

                // ����
                Revit.DB.Parameter parAtsuryoku1 = duct1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_ATSURYOKU_TOKKI"));
                Revit.DB.Parameter parAtsuryoku2 = duct2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_ATSURYOKU_TOKKI"));

                if (parAtsuryoku1.AsString() != parAtsuryoku2.AsString())
                {
                    return ret;
                }

                // �n��
                Revit.DB.Parameter parKeito1 = duct1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KEITO_TOKKI"));
                Revit.DB.Parameter parKeito2 = duct2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_KEITO_TOKKI"));

                if (parKeito1.AsString() != parKeito2.AsString())
                {
                    return ret;
                }

                // �h��
                Revit.DB.Parameter parToso1 = duct1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_TOSO_TOKKI"));
                Revit.DB.Parameter parToso2 = duct2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_TOSO_TOKKI"));

                if (parToso1.AsString() != parToso2.AsString())
                {
                    return ret;
                }

                // ���(���͊�)
                Revit.DB.Parameter parSyubetsu1 = duct1.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_SYUBETSUENVIRONMENT_TOKKI"));
                Revit.DB.Parameter parSyubetsu2 = duct2.LookupParameter(_CmpAttribute.ResourceText("IDS_TXT_SYUBETSUENVIRONMENT_TOKKI"));

                if (parSyubetsu1 != null && parSyubetsu2 != null &&
                    parSyubetsu1.AsString() != parSyubetsu2.AsString())
                {
                    return ret;
                }

                ret = true;
            }
            catch
            {
                return ret;
            }

            return ret;
        }

        Revit.DB.DefinitionFile TryOpenSharedParameterFile()
        {
            try
            {
                return _rvtUIDoc.Application.Application.OpenSharedParameterFile();
            }
            catch
            {
                return null;
            }
        }

        Revit.DB.DefinitionFile TrySetSharedParameterFile(string filePath)
        {
            try
            {
                var app = _rvtUIDoc.Application.Application;
                app.SharedParametersFilename = filePath;
                return app.OpenSharedParameterFile();
            }
            catch
            {
                return null;
            }
        }

        bool TryInsertSharedParameter(Revit.DB.Element unusedElem,
                                       string folderName,
                                       string fileName,
                                       string groupName,
                                       Collections.Generic.IList<Revit.DB.Category> categories,
                                       string defName,
                                       Revit.DB.ForgeTypeId paramType,
                                       Revit.DB.ForgeTypeId bltParamGroup,
                                       bool visible,
                                       int bindingMode)
        {
            try
            {
                var app = _rvtUIDoc.Application.Application;
                string filePath = System.IO.Path.Combine(folderName, fileName);

                if (!System.IO.File.Exists(filePath))
                    using (System.IO.File.Create(filePath)) { }

                string origFile = app.SharedParametersFilename;
                app.SharedParametersFilename = filePath;
                Revit.DB.DefinitionFile defFile = app.OpenSharedParameterFile();
                if (defFile == null)
                    return false;

                Revit.DB.DefinitionGroup group = defFile.Groups.get_Item(groupName);
                if (group == null)
                    group = defFile.Groups.Create(groupName);

                Revit.DB.ExternalDefinition def = group.Definitions.get_Item(defName) as Revit.DB.ExternalDefinition;
                if (def == null)
                {
                    var opts = new Revit.DB.ExternalDefinitionCreationOptions(defName, paramType);
                    opts.Visible = visible;
                    def = group.Definitions.Create(opts) as Revit.DB.ExternalDefinition;
                }

                if (def != null)
                {
                    var catSet = new Revit.DB.CategorySet();
                    foreach (var cat in categories)
                    {
                        if (cat != null)
                            catSet.Insert(cat);
                    }

                    Revit.DB.BindingMap map = RvtDBDoc.ParameterBindings;
                    if (map.get_Item(def) == null)
                    {
                        Revit.DB.ElementBinding binding;
                        if (bindingMode == 1)
                            binding = app.Create.NewTypeBinding(catSet);
                        else
                            binding = app.Create.NewInstanceBinding(catSet);

                        map.Insert(def, binding, bltParamGroup);
                    }
                }

                try { app.SharedParametersFilename = origFile; } catch { }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Shared-parameter value read for project info.</summary>
        public void GetValueString(Revit.DB.Element elem,
                                   string paramName,
                                   Revit.DB.ForgeTypeId paramType,
                                   Revit.DB.ForgeTypeId paramGroup,
                                   ref string value)
        {
            value = string.Empty;
            if (elem == null)
                return;
            Revit.DB.Parameter param = elem.LookupParameter(paramName);
            if (param != null)
                value = param.AsString() ?? param.AsValueString() ?? string.Empty;
        }

        /// <summary>Shared-parameter value write.</summary>
        public bool SetValue(Revit.DB.Element elem,
                              string paramName,
                              Revit.DB.ForgeTypeId paramType,
                              Revit.DB.ForgeTypeId paramGroup,
                              string value)
        {
            if (elem == null)
                return false;
            Revit.DB.Parameter param = elem.LookupParameter(paramName);
            if (param == null || param.IsReadOnly)
                return false;
            param.Set(value);
            return true;
        }

        #endregion

        // �v���p�e�B
        #region Properties

        /// ================================================================================
        /// <summary>�r���[�͈͍��� - ���</summary>
        ///
        /// <history>2014/09/24 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        double ViewRangeTopElevation
        {
            get
            {
                double ret = 0;

                if (ViewRangeTopLevel != null)
                {
                    ret = ViewRangeTopLevel.Elevation + ViewRangeTopOffset;
                }
                else
                {
                    // 1km
                    ret = 1000000 / 304.8;
                }

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>�r���[�͈͍��� - ����</summary>
        ///
        /// <history><p>2014/09/24 Created GSA, Inc. Ryo Kuroda</p>
        ///           <p>2015/06/25 Modified GSA, Inc. Ryo Kuroda</p></history>
        /// ================================================================================
        public
        double ViewRangeBottomElevation
        {
            get
            {
                double ret = 0;

                if (ViewRangeBottomLevel != null)
                {
                    ret = ViewRangeBottomLevel.Elevation + ViewRangeBottomOffset;
                }
                else
                {
                    // -1km
                    ret = -1000000 / 304.8;
                }

                return ret;
            }
        }

        /// ================================================================================
        /// <summary>�r���[�͈̓��x�� - ���</summary>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.Level ViewRangeTopLevel
        {
            get
            {
                return GetViewRangeLevel(Revit.DB.PlanViewPlane.TopClipPlane);
            }
        }

        // ================================================================================
        /// <summary>�r���[�͈̓��x�� - �f��</summary>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.Level ViewRangeCutLevel
        {
            get
            {
                return GetViewRangeLevel(Revit.DB.PlanViewPlane.CutPlane);
            }
        }

        // ================================================================================
        /// <summary>�r���[�͈̓��x�� - ����</summary>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.Level ViewRangeBottomLevel
        {
            get
            {
                return GetViewRangeLevel(Revit.DB.PlanViewPlane.BottomClipPlane);
            }
        }

        // ================================================================================
        /// <summary>�r���[�͈̓��x�� - ���s</summary>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        Revit.DB.Level ViewRangeViewDepthLevel
        {
            get
            {
                return GetViewRangeLevel(Revit.DB.PlanViewPlane.ViewDepthPlane);
            }
        }

        /// ================================================================================
        /// <summary>�r���[�͈̓I�t�Z�b�g�l - ���</summary>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        double ViewRangeTopOffset
        {
            get
            {
                return _ViewRange.GetOffset(Revit.DB.PlanViewPlane.TopClipPlane);
            }
        }

        /// ================================================================================
        /// <summary>�r���[�͈̓I�t�Z�b�g�l - �f��</summary>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        double ViewRangeCutOffset
        {
            get
            {
                return _ViewRange.GetOffset(Revit.DB.PlanViewPlane.CutPlane);
            }
        }

        /// ================================================================================
        /// <summary>�r���[�͈̓I�t�Z�b�g�l - ����</summary>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        double ViewRangeBottomOffset
        {
            get
            {
                return _ViewRange.GetOffset(Revit.DB.PlanViewPlane.BottomClipPlane);
            }
        }

        /// ================================================================================
        /// <summary>�r���[�͈̓I�t�Z�b�g�l - ���s</summary>
        ///
        /// <history>2014/09/17 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        double ViewRangeDepthOffset
        {
            get
            {
                return _ViewRange.GetOffset(Revit.DB.PlanViewPlane.ViewDepthPlane);
            }
        }

        /// ================================================================================
        /// <summary>�����ŏ�����</summary>
        ///
        /// <history>2014/11/04 Created GSA, Inc. Ryo Kuroda</history>
        /// ================================================================================
        public
        double LineMinLength
        {
            get
            {
                return _rvtUIDoc.Application.Application.ShortCurveTolerance;
            }
        }

        #endregion
    }
}