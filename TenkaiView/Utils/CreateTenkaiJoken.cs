using Autodesk.Revit.DB;
using ADSK.ViewExtension.TenkaiView.Resources;

namespace ADSK.ViewExtension.TenkaiView.Utils
{
    public class CreateTenkaiJoken
    {
        public CreateTenkaiJoken()
        {
            m_View0 = Text.TXT_VIEWA;
            m_View3 = Text.TXT_VIEWB;
            m_View6 = Text.TXT_VIEWC;
            m_View9 = Text.TXT_VIEWD;
            m_Name1 = NamingRule.LevelName;
            m_Name2 = NamingRule.RoomName;
            m_Name3 = NamingRule.Direction;
            m_ExtendRightLeft = 0;
            m_ExtendTopBottom = 0;
            m_TrimBase = TrimingBase.BetweenLevel;
            m_ViewTypeId = ElementId.InvalidElementId;
            m_TagTenjo = ElementId.InvalidElementId;
            m_TagMawaribuchi = ElementId.InvalidElementId;
            m_TagKabe = ElementId.InvalidElementId;
            m_TagHabaki = ElementId.InvalidElementId;
            m_TagYuka = ElementId.InvalidElementId;
            m_DimCH = ElementId.InvalidElementId;
            m_DimChText = Text.TXT_PREFIX;
            m_DimLevel = ElementId.InvalidElementId;
            m_DimTorishin = ElementId.InvalidElementId;
        }

        public enum NamingRule
        {
            LevelName = 0,
            RoomName = 1,
            RoomNameAndNumber = 2,
            Direction = 3
        }

        public enum TrimingBase
        {
            RoomVolume = 0,
            BetweenLevel = 1
        }

        private string m_View0;
        public string View0
        {
            get => m_View0;
            set => m_View0 = value;
        }

        private string m_View3;
        public string View3
        {
            get => m_View3;
            set => m_View3 = value;
        }

        private string m_View6;
        public string View6
        {
            get => m_View6;
            set => m_View6 = value;
        }

        private string m_View9;
        public string View9
        {
            get => m_View9;
            set => m_View9 = value;
        }

        private NamingRule m_Name1;
        public NamingRule Name1
        {
            get => m_Name1;
            set => m_Name1 = value;
        }

        private NamingRule m_Name2;
        public NamingRule Name2
        {
            get => m_Name2;
            set => m_Name2 = value;
        }

        private NamingRule m_Name3;
        public NamingRule Name3
        {
            get => m_Name3;
            set => m_Name3 = value;
        }

        private ElementId m_ViewTypeId;
        public ElementId ViewTypeID
        {
            get => m_ViewTypeId;
            set => m_ViewTypeId = value;
        }

        private TrimingBase m_TrimBase;
        public TrimingBase TrimBase
        {
            get => m_TrimBase;
            set => m_TrimBase = value;
        }

        private double m_ExtendTopBottom;
        public double ExtendTopBottom
        {
            get => m_ExtendTopBottom;
            set => m_ExtendTopBottom = UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Millimeters);
        }

        private double m_ExtendRightLeft;
        public double ExtendedRightLeft
        {
            get => m_ExtendRightLeft;
            set => m_ExtendRightLeft = UnitUtils.ConvertToInternalUnits(value, UnitTypeId.Millimeters);
        }

        private ElementId m_TagTenjo;
        public ElementId TagTenjoID
        {
            get => m_TagTenjo;
            set => m_TagTenjo = value;
        }

        private ElementId m_TagMawaribuchi;
        public ElementId TagmawaribuchiID
        {
            get => m_TagMawaribuchi;
            set => m_TagMawaribuchi = value;
        }

        private ElementId m_TagKabe;
        public ElementId TagKabeID
        {
            get => m_TagKabe;
            set => m_TagKabe = value;
        }

        private ElementId m_TagHabaki;
        public ElementId TagHabakiID
        {
            get => m_TagHabaki;
            set => m_TagHabaki = value;
        }

        private ElementId m_TagYuka;
        public ElementId TagYukaID
        {
            get => m_TagYuka;
            set => m_TagYuka = value;
        }

        private ElementId m_DimTorishin;
        public ElementId DimTypeTorishinID
        {
            get => m_DimTorishin;
            set => m_DimTorishin = value;
        }

        private ElementId m_DimLevel;
        public ElementId DimLevelID
        {
            get => m_DimLevel;
            set => m_DimLevel = value;
        }

        private ElementId m_DimCH;
        public ElementId DimTypeCHID
        {
            get => m_DimCH;
            set => m_DimCH = value;
        }

        private string m_DimChText;
        public string DimCHText
        {
            get => m_DimChText;
            set => m_DimChText = value?.Trim() ?? string.Empty;
        }

        private int m_ViewScale;
        public int ViewScale
        {
            get => m_ViewScale;
            set => m_ViewScale = value;
        }
    }
}
