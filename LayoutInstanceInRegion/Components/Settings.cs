using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.LayoutInstanceInRegion.Components
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
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        public Category GetCategory(BuiltInCategory builtInCategory)
        {
            return Category.GetCategory(_rvtUIDoc.Document, builtInCategory);
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
