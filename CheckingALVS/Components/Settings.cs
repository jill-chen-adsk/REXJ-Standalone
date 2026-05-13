using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ADSK.JExtRAC.CheckingALVS.Utils;

namespace ADSK.JExtRAC.CheckingALVS.Components
{
    public class Settings
    {
        private readonly UIDocument _rvtUIDoc;

        private SpatialElementBoundaryLocation _RoomAreaComputationDefault;
        private bool _RoomVolumeComputationDefault;

        public Settings(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            _RoomAreaComputationDefault = GetRoomAreaComputation();
            _RoomVolumeComputationDefault = GetRoomVolumeComputation();
        }

        public SpatialElementBoundaryLocation GetRoomAreaComputation()
        {
            AreaVolumeSettings avs = AreaVolumeSettings.GetAreaVolumeSettings(_rvtUIDoc.Document);
            return avs.GetSpatialElementBoundaryLocation(SpatialElementType.Room);
        }

        public void SetRoomAreaComputation(SpatialElementBoundaryLocation location)
        {
            AreaVolumeSettings avs = AreaVolumeSettings.GetAreaVolumeSettings(_rvtUIDoc.Document);
            using (Transaction t = new Transaction(_rvtUIDoc.Document, "Set Room Area Computation"))
            {
                t.Start();
                avs.SetSpatialElementBoundaryLocation(location, SpatialElementType.Room);
                t.Commit();
            }
        }

        public bool GetRoomVolumeComputation()
        {
            AreaVolumeSettings avs = AreaVolumeSettings.GetAreaVolumeSettings(_rvtUIDoc.Document);
            return avs.ComputeVolumes;
        }

        public void SetRoomVolumeComputation(bool computeVolumes)
        {
            AreaVolumeSettings avs = AreaVolumeSettings.GetAreaVolumeSettings(_rvtUIDoc.Document);
            using (Transaction t = new Transaction(_rvtUIDoc.Document, "Set Room Volume Computation"))
            {
                t.Start();
                avs.ComputeVolumes = computeVolumes;
                t.Commit();
            }
        }

        public void SetRoomAreaComputationDefault()
        {
            SetRoomAreaComputation(_RoomAreaComputationDefault);
        }

        public void SetRoomAreaComputationCoreCenter()
        {
            _RoomAreaComputationDefault = GetRoomAreaComputation();
            SetRoomAreaComputation(SpatialElementBoundaryLocation.CoreCenter);
        }

        public void SetRoomAreaComputationWallCenter()
        {
            _RoomAreaComputationDefault = GetRoomAreaComputation();
            SetRoomAreaComputation(SpatialElementBoundaryLocation.Center);
        }

        public void SetRoomVolumeComputationDefault()
        {
            SetRoomVolumeComputation(_RoomVolumeComputationDefault);
        }

        public void SetRoomVolumeComputationVolume()
        {
            _RoomVolumeComputationDefault = GetRoomVolumeComputation();
            SetRoomVolumeComputation(true);
        }

        public void SetRoomVolumeComputationArea()
        {
            _RoomVolumeComputationDefault = GetRoomVolumeComputation();
            SetRoomVolumeComputation(false);
        }

        public double Round(double value)
        {
            string s = UtilValue.Rounding(value, 5, 0);
            return double.TryParse(s, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out double d) ? d : 0;
        }

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(_rvtUIDoc.Document, bic);
        }

        public Category CategoryProjInfo =>
            Category.GetCategory(_rvtUIDoc.Document, BuiltInCategory.OST_ProjectInformation);

        public int GetWinDoorSymbolType(FamilySymbol familySymbol)
        {
            int ret = 0;
            string categoryDoorID = GetCategory(BuiltInCategory.OST_Doors).Id.ToString();
            string categoryWindowID = GetCategory(BuiltInCategory.OST_Windows).Id.ToString();
            string categoryID = familySymbol.Category.Id.ToString();

            if (categoryID == categoryDoorID)
                ret = 1;
            else if (categoryID == categoryWindowID)
                ret = 2;
            return ret;
        }

        public Category CategoryRoom => GetCategory(BuiltInCategory.OST_Rooms);

        public IList<Category> CategoryWinDoor
        {
            get
            {
                IList<Category> ret = new List<Category>();
                ret.Add(GetCategory(BuiltInCategory.OST_Doors));
                ret.Add(GetCategory(BuiltInCategory.OST_Windows));
                return ret;
            }
        }
    }
}
