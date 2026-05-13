using CmdDuctDisplacement.Constant;
using CmdDuctDisplacement.Properties;
using System.Collections;
using System.Linq;

namespace CmdDuctDisplacement.Resource
{
    internal class ExResources
    {
        private static System.Resources.ResourceSet resourceSet = Resources.ResourceManager.GetResourceSet(System.Globalization.CultureInfo.CurrentUICulture, true, true);

        internal static string ResxString(string key)
        {
            foreach (var entry in resourceSet.OfType<DictionaryEntry>().Select((item, i) => new { Index = i, Key = item.Key, Value = item.Value }))
            {
                if (entry.Key.ToString().Equals(key))
                {
                    return entry.Value.ToString();
                }
            }
            return null;
        }

        /// <summary>
        /// Loads a keyed resource entry for databound forms.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static string ResxStringforForm(string key)
        {
            string res = ResxString(key);
            if(res == null)
            {
                return "";
            }
            else
            {
                return res;
            }
        }

        /// <summary>
        ///   Move amount.
        /// </summary>
        public static string LVL_AmountMovement
        {
            get
            {
                return ResxStringforForm("LVL_AmountMovement");
            }
        }

        /// <summary>
        ///   Show move-distance options.
        /// </summary>
        public static string LVL_AmountMovementOPT_Disp
        {
            get
            {
                return ResxStringforForm("LVL_AmountMovementOPT_Disp");
            }
        }

        /// <summary>
        ///   Hide move-distance options.
        /// </summary>
        public static string LVL_AmountMovementOPT_Hidden
        {
            get
            {
                return ResxStringforForm("LVL_AmountMovementOPT_Hidden");
            }
        }

        /// <summary>
        ///   Beam.
        /// </summary>
        public static string LVL_Beam
        {
            get
            {
                return ResxStringforForm("LVL_Beam");
            }
        }

        /// <summary>
        ///   Bottom face.
        /// </summary>
        public static string LVL_BottomSide
        {
            get
            {
                return ResxStringforForm("LVL_BottomSide");
            }
        }

        /// <summary>
        ///   Center reference.
        /// </summary>
        public static string LVL_Center
        {
            get
            {
                return ResxStringforForm("LVL_Center");
            }
        }

        /// <summary>
        ///   Lower reference label.
        /// </summary>
        public static string LVL_Down
        {
            get
            {
                return ResxStringforForm("LVL_Down");
            }
        }

        /// <summary>
        ///   45° elbow connector.
        /// </summary>
        public static string LVL_DuctElbow45Degree
        {
            get
            {
                return ResxStringforForm("LVL_DuctElbow45Degree");
            }
        }

        /// <summary>
        ///   90° elbow connector.
        /// </summary>
        public static string LVL_DuctElbow90Degree
        {
            get
            {
                return ResxStringforForm("LVL_DuctElbow90Degree");
            }
        }

        /// <summary>
        ///   Oval duct profile label.
        /// </summary>
        public static string LVL_EllipticalDuct
        {
            get
            {
                return ResxStringforForm("LVL_EllipticalDuct");
            }
        }

        /// <summary>
        ///   Finish command shortcut label.
        /// </summary>
        public static string LVL_EndThisCommand
        {
            get
            {
                return ResxStringforForm("LVL_EndThisCommand");
            }
        }

        /// <summary>
        ///   Prompt for beam fireproofing coating thickness.
        /// </summary>
        public static string LVL_EnterTheThicknessOfFireproofCoatingOfTheBeam
        {
            get
            {
                return ResxStringforForm("LVL_EnterTheThicknessOfFireproofCoatingOfTheBeam");
            }
        }

        /// <summary>
        ///   FL 
        /// </summary>
        public static string LVL_FloorLevel
        {
            get
            {
                return ResxStringforForm("LVL_FloorLevel");
            }
        }

        /// <summary>
        ///   Off-state label for coatings.
        /// </summary>
        public static string LVL_InValid
        {
            get
            {
                return ResxStringforForm("LVL_InValid");
            }
        }

        /// <summary>
        ///   Interference banner suffix appended to offending elements.
        /// </summary>
        public static string LVL_IsolationWorningMessage_less
        {
            get
            {
                return ResxStringforForm("LVL_IsolationWorningMessage_less");
            }
        }

        /// <summary>
        ///   Message when routed spacing violates minimum clearance rule.
        /// </summary>
        public static string LVL_IsolationWorningMessage_more
        {
            get
            {
                return ResxStringforForm("LVL_IsolationWorningMessage_more");
            }
        }

        /// <summary>
        ///   Fitting-connection options group caption.
        /// </summary>
        public static string LVL_Junction
        {
            get
            {
                return ResxStringforForm("LVL_Junction");
            }
        }

        /// <summary>
        ///   Level constraint area caption.
        /// </summary>
        public static string LVL_Level
        {
            get
            {
                return ResxStringforForm("LVL_Level");
            }
        }

        /// <summary>
        ///   Move verb shortcut.
        /// </summary>
        public static string LVL_Move
        {
            get
            {
                return ResxStringforForm("LVL_Move");
            }
        }

        /// <summary>
        ///   Move-distance options group caption.
        /// </summary>
        public static string LVL_MoveOption
        {
            get
            {
                return ResxStringforForm("LVL_MoveOption");
            }
        }

        /// <summary>
        ///   Apply-move button label text.
        /// </summary>
        public static string LVL_Moving
        {
            get
            {
                return ResxStringforForm("LVL_Moving");
            }
        }

        /// <summary>
        ///   Move-strategy picker caption.
        /// </summary>
        public static string LVL_MovingMethod
        {
            get
            {
                return ResxStringforForm("LVL_MovingMethod");
            }
        }

        /// <summary>
        ///   None / no rounding qualifier.
        /// </summary>
        public static string LVL_None
        {
            get
            {
                return ResxStringforForm("LVL_None");
            }
        }

        /// <summary>
        ///   Connecting particle rendered between obstruction names and glyphs.
        /// </summary>
        public static string LVL_Of
        {
            get
            {
                return ResxStringforForm("LVL_Of");
            }
        }

        /// <summary>
        ///   Offset reference label.
        /// </summary>
        public static string LVL_Offset
        {
            get
            {
                return ResxStringforForm("LVL_Offset");
            }
        }

        /// <summary>
        ///   Prefix appended to obstruction labels highlighting mandatory clearances.
        /// </summary>
        public static string LVL_OfTarget
        {
            get
            {
                return ResxStringforForm("LVL_OfTarget");
            }
        }

        /// <summary>
        ///   Button label prompting host-model obstruction pick.
        /// </summary>
        public static string LVL_Pick_a_TargetObject
        {
            get
            {
                return ResxStringforForm("LVL_Pick_a_TargetObject");
            }
        }

        /// <summary>
        ///   Button label prompting linked-model obstruction pick.
        /// </summary>
        public static string LVL_Pick_a_TargetObject_LinkModel
        {
            get
            {
                return ResxStringforForm("LVL_Pick_a_TargetObject_LinkModel");
            }
        }


        /// <summary>
        ///   Pipe-system type label helper.
        /// </summary>
        public static string LVL_PipeType
        {
            get
            {
                return ResxStringforForm("LVL_PipeType");
            }
        }

        /// <summary>
        ///   Placement zone grouping header.
        /// </summary>
        public static string LVL_Placement
        {
            get
            {
                return ResxStringforForm("LVL_Placement");
            }
        }

        /// <summary>
        ///   Layout-rules grouping caption.
        /// </summary>
        public static string LVL_PlacementCondition
        {
            get
            {
                return ResxStringforForm("LVL_PlacementCondition");
            }
        }

        /// <summary>
        ///   Placement positional controls caption.
        /// </summary>
        public static string LVL_Position
        {
            get
            {
                return ResxStringforForm("LVL_Position");
            }
        }

        /// <summary>
        ///   Command to revisit segment selections.
        /// </summary>
        public static string LVL_ReturnToSectionSelection
        {
            get
            {
                return ResxStringforForm("LVL_ReturnToSectionSelection");
            }
        }

        /// <summary>
        ///   Round duct profile label.
        /// </summary>
        public static string LVL_RoundDuct
        {
            get
            {
                return ResxStringforForm("LVL_RoundDuct");
            }
        }

        /// <summary>
        ///   Rounding increment header.
        /// </summary>
        public static string LVL_RoundingPrecision
        {
            get
            {
                return ResxStringforForm("LVL_RoundingPrecision");
            }
        }

        /// <summary>
        ///   S-curve transitional fitting label.
        /// </summary>
        public static string LVL_SCurveShapedDuct
        {
            get
            {
                return ResxStringforForm("LVL_SCurveShapedDuct");
            }
        }

        /// <summary>
        ///   Read-only baseline finish level caption.
        /// </summary>
        public static string LVL_SelectedFL
        {
            get
            {
                return ResxStringforForm("LVL_SelectedFL");
            }
        }

        /// <summary>
        ///   Clearance-span readout caption.
        /// </summary>
        public static string LVL_Separation
        {
            get
            {
                return ResxStringforForm("LVL_Separation");
            }
        }

        /// <summary>
        ///   Generic shape discriminator label.
        /// </summary>
        public static string LVL_Shape
        {
            get
            {
                return ResxStringforForm("LVL_Shape");
            }
        }

        /// <summary>
        ///   Rectangular duct profile label.
        /// </summary>
        public static string LVL_SquareDuct
        {
            get
            {
                return ResxStringforForm("LVL_SquareDuct");
            }
        }

        /// <summary>
        ///   Minimum allowable spacing between routed MEP fabrication.
        /// </summary>
        public static string LVL_TheDistanceOfElement
        {
            get
            {
                return ResxStringforForm("LVL_TheDistanceOfElement");
            }
        }

        /// <summary>
        ///   Active beam-fireproof coating thickness caption.
        /// </summary>
        public static string LVL_ThicknessOfFireProofOfBeam
        {
            get
            {
                return ResxStringforForm("LVL_ThicknessOfFireProofOfBeam");
            }
        }

        /// <summary>
        ///   Upper reference label for vertical offsets.
        /// </summary>
        public static string LVL_TopSide
        {
            get
            {
                return ResxStringforForm("LVL_TopSide");
            }
        }

        /// <summary>
        ///   Harmonize routed levels checkbox caption.
        /// </summary>
        public static string LVL_UnifyLevels
        {
            get
            {
                return ResxStringforForm("LVL_UnifyLevels");
            }
        }

        /// <summary>
        ///   Upper bound label for obstruction bands.
        /// </summary>
        public static string LVL_Up
        {
            get
            {
                return ResxStringforForm("LVL_Up");
            }
        }

        /// <summary>
        ///   On-state label for coatings.
        /// </summary>
        public static string LVL_Valid
        {
            get
            {
                return ResxStringforForm("LVL_Valid");
            }
        }
    }
}
