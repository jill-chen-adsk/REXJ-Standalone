using System;
using System.Data;
using System.Globalization;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ADSK.JExtRAC.AreaSchedule.Components
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

        public Category GetCategory(BuiltInCategory bic)
        {
            return Category.GetCategory(_rvtUIDoc.Document, bic);
        }

        public Category CategoryArea => GetCategory(BuiltInCategory.OST_Areas);
        public Category CategoryRoom => GetCategory(BuiltInCategory.OST_Rooms);
        public Category CategoryProjInfo => GetCategory(BuiltInCategory.OST_ProjectInformation);

        public SpatialElementBoundaryLocation GetRoomAreaComputation()
        {
            AreaVolumeSettings avs = AreaVolumeSettings.GetAreaVolumeSettings(_rvtUIDoc.Document);
            return avs.GetSpatialElementBoundaryLocation(SpatialElementType.Room);
        }

        public void SetRoomAreaComputation(SpatialElementBoundaryLocation location)
        {
            AreaVolumeSettings avs = AreaVolumeSettings.GetAreaVolumeSettings(_rvtUIDoc.Document);
            if (_rvtUIDoc.Document.IsModifiable)
            {
                avs.SetSpatialElementBoundaryLocation(location, SpatialElementType.Room);
            }
            else
            {
                using (var t = new Transaction(_rvtUIDoc.Document, "Boundary location"))
                {
                    t.Start();
                    avs.SetSpatialElementBoundaryLocation(location, SpatialElementType.Room);
                    t.Commit();
                }
            }
        }

        public bool GetRoomVolumeComputation()
        {
            return AreaVolumeSettings.GetAreaVolumeSettings(_rvtUIDoc.Document).ComputeVolumes;
        }

        public void SetRoomVolumeComputation(bool computeVolumes)
        {
            AreaVolumeSettings avs = AreaVolumeSettings.GetAreaVolumeSettings(_rvtUIDoc.Document);
            if (_rvtUIDoc.Document.IsModifiable)
            {
                avs.ComputeVolumes = computeVolumes;
            }
            else
            {
                using (var t = new Transaction(_rvtUIDoc.Document, "Volume computation"))
                {
                    t.Start();
                    avs.ComputeVolumes = computeVolumes;
                    t.Commit();
                }
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

        public double Round(double value) => Math.Round(value, 2);

        public void GetPIData(int startCount, int endCount, ref DataTable table)
        {
            string strPI = Math.PI.ToString(CultureInfo.InvariantCulture);
            if (startCount < endCount)
            {
                int count = -1;
                for (int i = startCount; i <= endCount; ++i)
                {
                    count++;
                    string str = strPI.Substring(0, Math.Min(i, strPI.Length));
                    DataRow row = table.NewRow();
                    row[0] = count;
                    row[1] = str;
                    table.Rows.Add(row);
                }
            }
        }

        public void SetUnitAreaM2(int _decimalPlacesIgnored)
        {
            try
            {
                if (_rvtUIDoc.Document.IsModifiable)
                {
                    Units units = _rvtUIDoc.Document.GetUnits();
                    FormatOptions fo = units.GetFormatOptions(SpecTypeId.Area);
                    fo.SetUnitTypeId(UnitTypeId.SquareMeters);
                    units.SetFormatOptions(SpecTypeId.Area, fo);
                }
                else
                {
                    using (var t = new Transaction(_rvtUIDoc.Document, "Area units"))
                    {
                        t.Start();
                        Units units = _rvtUIDoc.Document.GetUnits();
                        FormatOptions fo = units.GetFormatOptions(SpecTypeId.Area);
                        fo.SetUnitTypeId(UnitTypeId.SquareMeters);
                        units.SetFormatOptions(SpecTypeId.Area, fo);
                        t.Commit();
                    }
                }
            }
            catch { }
        }
    }
}
