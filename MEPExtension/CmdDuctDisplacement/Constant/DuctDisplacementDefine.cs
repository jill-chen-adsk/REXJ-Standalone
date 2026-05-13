namespace CmdDuctDisplacement.Constant
{
    public class DuctDisplacementDefine
    {
        /************************/
        /* Resource file keys */
        /************************/
        public static string EXE_ASSEMBLY_FOLDER = @"";
        public static string FAMILY_FOLDER = @"\";
        public static string RESOURCE_FILE_NAME = @"\Resources.resx";




        // Prompt keys for graphical picks
        public static string REQ_ENTER_POINT_OR_GRID = "REQ_ENTER_POINT_OR_GRID";
        public static string REQ_ENTER_MEPCURVE = "REQ_ENTER_MEPCURVE";
        public static string REQ_ENTER_POINT = "REQ_ENTER_POINT";
        public static string REQ_ENTER_POINT_CNT = "REQ_ENTER_POINT_CNT";
        public static string REQ_ENTER_TARGET = "REQ_ENTER_TARGET";
        public static string REQ_ENTER_CUT_OBJ = "REQ_ENTER_CUT_OBJ";

        // TaskDialog
        public static string MSG_WARN1 = "MSG_WARN1"; // Two MEPCurves not connected (localized via resx).
        public static string MSG_WARN2 = "MSG_WARN2"; // Wrong view family (localized via resx).
        public static string MSG_WARN3 = "MSG_WARN3"; // Fabrication endpoint rule (localized via resx).
        public static string MSG_WARN4 = "MSG_WARN4"; // Duplicate segment picks (localized via resx).
        public static string MSG_ERROR1 = "MSG_ERROR1"; // Segment geometry error (localized via resx).
        public static string MSG_ERROR2 = "MSG_ERROR2"; // Routing space error (localized via resx).
        public static string MSG_ERROR3 = "MSG_ERROR3"; // Branch tap trim error (localized via resx).
        public static string Worn_ExceptionMessege = "Worn_ExceptionMessege";

        //Title
        public static string DIALOG_TITLE_CONFIRM = "DIALOG_TITLE_CONFIRM";
        public static string DIALOG_TITLE_WARN = "DIALOG_TITLE_WARN";
        public static string Worn_Caption = "Worn_Caption";

        // Dialog-bound resource keys
        public static string LVL_FloorLevel = "LVL_FloorLevel";
        public static string LVL_Beam = "LVL_Beam"; 
            public static string LVL_DIRECT_SHAPE = "LVL_DIRECT_SHAPE";
        public static string LVL_Shape = "LVL_Shape";
        public static string LVL_AmountMovement = "LVL_AmountMovement";
        public static string LVL_IsolationWorningMessage_more = "LVL_IsolationWorningMessage_more";
        public static string LVL_IsolationWorningMessage_less = "LVL_IsolationWorningMessage_less";
        public static string LVL_AmountMovementOPT_Hidden = "LVL_AmountMovementOPT_Hidden";
        public static string LVL_AmountMovementOPT_Disp = "LVL_AmountMovementOPT_Disp";
        public static string LVL_Moving = "LVL_Moving";
        public static string LVL_OfTarget = "LVL_OfTarget";
        public static string LVL_MovingMethod = "LVL_MovingMethod";
        public static string LVL_Move = "LVL_Move";
        public static string LVL_TopSide = "LVL_TopSide";
        public static string LVL_Center = "LVL_Center";
        public static string LVL_BottomSide = "LVL_BottomSide";
        public static string LVL_EllipticalDuct = "LVL_EllipticalDuct";
        public static string LVL_RoundDuct = "LVL_RoundDuct";
        public static string LVL_PipeType = "LVL_PipeType";
        public static string LVL_SquareDuct = "LVL_SquareDuct";


        #region Logging
        public static string LOG_LEVEL_MIN = "LOG_LEVEL_MIN";
        public static string LOG_LEVEL_MAX = "LOG_LEVEL_MAX";
        public static string LOG_FOLDER_PATH_DEF = @"\Log\";
        public static int LOG_LEVEL_MIN_DEF = 1;
        public static int LOG_LEVEL_MAX_DEF = 3;
        #endregion
        #region LineStyle cut line (style name preserved for Revit)
        public static string RESKEY_CUT_LINE_COLOR_R = "CUT_LINE_COLOR_R";
        public static string RESKEY_CUT_LINE_COLOR_G = "CUT_LINE_COLOR_G";
        public static string RESKEY_CUT_LINE_COLOR_B = "CUT_LINE_COLOR_B";
        public static string RESKEY_CUT_LINE_WEIGHT = "CUT_LINE_WEIGHT";
        #endregion
        #region S-curve family resource keys (Japanese filenames preserved)
        public static string S_CURVE_ANGLE = "S_CURVE_ANGLE";
        public static string S_CURVE_OVL_FAMILY = "S_CURVE_OVL_FAMILY";
        public static string S_CURVE_OVL_FILE = "S_CURVE_OVL_FILE";
        public static string S_CURVE_OVL_TYPE = "S_CURVE_OVL_TYPE";
        public static string S_CURVE_REC_FAMILY = "S_CURVE_REC_FAMILY";
        public static string S_CURVE_REC_FILE = "S_CURVE_REC_FILE";
        public static string S_CURVE_REC_TYPE = "S_CURVE_REC_TYPE";
        public static string S_CURVE_RND_FAMILY = "S_CURVE_RND_FAMILY";
        public static string S_CURVE_RND_FILE = "S_CURVE_RND_FILE";
        public static string S_CURVE_RND_TYPE = "S_CURVE_RND_TYPE";
        public static string S_CURVE_PIPE_FAMILY = "S_CURVE_PIPE_FAMILY";
        public static string S_CURVE_PIPE_FILE = "S_CURVE_PIPE_FILE";
        public static string S_CURVE_PIPE_TYPE = "S_CURVE_PIPE_TYPE";
        #endregion

        /************************/
        /* Enumerations       */
        /************************/
        #region enum
        public enum MOVE_PTN : int
        {
            OFFSET = 0,
            UNIFIEDLVEL = 1
        }
        public enum FITTING_PTN : int
        {
            deg45 = 0,
            deg90 = 1,
            S = 2
        };

        public enum FORM_MODE : int
        {
            CANCEL = 0,
            NORMAL = 1,
            EXECUE = 2,
            SELECT = 3
        };

        /// <summary>Rounding multiplier (50 mm, 100 mm, none).</summary>
        /// 

        public enum Rounder
        {
            Multiple_50 = 0,
            Multiple_100 = 1,
            Multiple_None = 2

        }
        /// <summary>Top vs bottom obstruction placement shortcut.</summary>
        /// 

        public enum PressButton
        {
            Top = 0,
            Bottom = 1

        }

        /// <summary>Add vs subtract displacement.</summary>
        /// 

        public enum MethodOfCalculation
        {
            Add = 0,
            Sub = 1

        }

        /// <summary>Dialog/button exit routes.</summary>
        /// 
        public enum WindowReturnNum
        {
            NoSelect = 0,
            OK = 1,
            Cancel = 2,
            GraphicInstructions_General = 3,
            EndRoutine = 4,
            GraphicInstructions_Linkd = 5
        }

        /// <summary>Source route for numeric text edits.</summary>
        /// 
        public enum TextChangeRoute
        {
            //init or clear
            NoSelect = 0,
            Text = 1,
            IncreaseButton = 2,
            DecreaseButton = 3,
            RefalenceLine = 4,
            MoveDistanceCalButton = 5,
            SwitchCalMethodButton = 6,
            PointClick_1_2 = 7,
            PointClick_3 = 8,
            BottomArrangmentButton = 9,
            TopArrangementButton = 10

        }

        /// <summary>Reference-line transition graph.</summary>
        /// 
        public enum ReferenceLineChangeRoute
        {
            Keep = 0,
            ToptoCenter = 1,
            ToptoBottom = 2,
            CentertoTop = 3,
            CentertoButtom = 4,
            BottumtoTop = 5,
            BottomtoCenter = 6
        }

        /// <summary>Fitting presets from the ribbon/dialog.</summary>
        /// 
        public enum SelectElbow
        {
            FortyFive = 0,
            Ninety = 1,
            Scarve = 2
        }

        /// <summary>Two-pick versus three-pick command modes.</summary>
        /// 
        public enum Frow
        {
            None = 0,
            TwoPick = 1,
            ThreePick_GeneralModel = 2,
            ThreePick_LinkdModel = 3
        }

        /// <summary>Which graphic the user referenced (move vs obstruction).</summary>
        /// 
        public enum InstructionObj
        {
            MoveObj_1 = 0,
            MoveObj_2 = 1,
            TargetObj = 2
        }

        /// <summary>Vertical justification band.</summary>
        /// 
        public enum Line
        {
            // Center = 0, // unused
            Top = 1,
            Bottom = 2
        }

        /// <summary>S-offset family discriminator.</summary>
        public enum S_CURVE_PTN : int
        {
            RECT = 1,
            ROUND = 2,
            OVAL = 3,
            PIPE = 4
        }

        #endregion

        /************************/
        /* Misc constants      */
        /************************/
        #region 
        // Built-in/detail line style name in the model template (Japanese string preserved).
        public static string CUT_LINE_LINESTYLE_NAME = "切断線";
        // Vertical offset presets
        public const int OFFSET_POS_TOP = 0;
        public const int OFFSET_POS_MIDDLE = 1;
        public const int OFFSET_POS_BOTTOM = 2;
        public static int START_SIDE = 0;
        public static int END_SIDE = 1;
        public const int DIR_UPPER = 0;
        public const int DIR_DOWN = 1;
        // Rounding constants (mm)
        public const int num_0 = 0;
        public const int num_50 = 50;
        public const int num_100 = 100;

        // Horizontal tolerance when evaluating S-offset feasibility
        public const double S_CURVE_THRESHOLD = 0.5;

        #endregion
    }
}
