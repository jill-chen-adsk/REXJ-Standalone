using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AutoCreateRoomView.Components
{
    public class Settings
    {
        private readonly UIDocument _rvtUIDoc;
        private SpatialElementBoundaryLocation _roomAreaComputationDefault;
        private bool _roomVolumeComputationDefault;

        public Settings(UIDocument rvtUIDoc)
        {
            _rvtUIDoc = rvtUIDoc;
            _roomAreaComputationDefault = GetRoomAreaComputation();
            _roomVolumeComputationDefault = GetRoomVolumeComputation();
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
            SetRoomAreaComputation(_roomAreaComputationDefault);
        }

        public void SetRoomAreaComputationCoreCenter()
        {
            _roomAreaComputationDefault = GetRoomAreaComputation();
            SetRoomAreaComputation(SpatialElementBoundaryLocation.CoreCenter);
        }

        public void SetRoomAreaComputationWallCenter()
        {
            _roomAreaComputationDefault = GetRoomAreaComputation();
            SetRoomAreaComputation(SpatialElementBoundaryLocation.Center);
        }

        public void SetRoomVolumeComputationDefault()
        {
            SetRoomVolumeComputation(_roomVolumeComputationDefault);
        }

        public void SetRoomVolumeComputationVolume()
        {
            _roomVolumeComputationDefault = GetRoomVolumeComputation();
            SetRoomVolumeComputation(true);
        }

        public void SetRoomVolumeComputationArea()
        {
            _roomVolumeComputationDefault = GetRoomVolumeComputation();
            SetRoomVolumeComputation(false);
        }

        public double Round(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        public Category GetCategory(BuiltInCategory builtInCategory)
        {
            return Category.GetCategory(_rvtUIDoc.Document, builtInCategory);
        }

        public int GetWinDoorSymbolType(FamilySymbol familySymbol)
        {
            int ret = 0;
            var categoryDoorID = GetCategory(BuiltInCategory.OST_Doors).Id.ToString();
            var categoryWindowID = GetCategory(BuiltInCategory.OST_Windows).Id.ToString();
            var categoryID = familySymbol.Category.Id.ToString();
            if (categoryID == categoryDoorID) ret = 1;
            else if (categoryID == categoryWindowID) ret = 2;
            return ret;
        }

        public Category CategoryRoom => GetCategory(BuiltInCategory.OST_Rooms);

        public System.Collections.Generic.IList<Category> CategoryWinDoor
        {
            get
            {
                var ret = new System.Collections.Generic.List<Category>();
                ret.Add(GetCategory(BuiltInCategory.OST_Doors));
                ret.Add(GetCategory(BuiltInCategory.OST_Windows));
                return ret;
            }
        }
    }
}
