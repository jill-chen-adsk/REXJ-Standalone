using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using ADSK.ViewExtension.TenkaiView.Resources;
using ADSK.ViewExtension.TenkaiView.Sorter;

namespace ADSK.ViewExtension.TenkaiView.Utils
{
    public static class RoomElevation
    {
        public static List<ElementId> CreateElevation3(
            ElementId roomId,
            Document document1,
            Autodesk.Revit.UI.UIDocument uiDocument1,
            ElementId tenkaiPlan,
            CreateTenkaiJoken tenkaiJoken)
        {
            Element roomElm = document1.GetElement(roomId);
            Room roomTemp = roomElm as Room;
            if (roomTemp == null)
                return null;
            if (roomTemp.Area == 0)
                return null;

            ElementId roomLevelId = roomElm.LevelId;
            Level roomLevel = document1.GetElement(roomLevelId) as Level;

            BoundingBoxXYZ roomBB = GetRoomBoundingBox(document1, roomId);
            if (roomBB == null)
                return null;

            XYZ pntMin = roomBB.Transform.OfPoint(roomBB.Min);
            XYZ pntMax = roomBB.Transform.OfPoint(roomBB.Max);

            if (tenkaiJoken.TrimBase == CreateTenkaiJoken.TrimingBase.BetweenLevel)
            {
                Level upperLevel = GetNextLevel(roomTemp);
                if (upperLevel != null)
                {
                    pntMin = new XYZ(pntMin.X, pntMin.Y, roomLevel.ProjectElevation);
                    pntMax = new XYZ(pntMax.X, pntMax.Y, upperLevel.ProjectElevation);
                }
            }

            Line bbLine = Line.CreateBound(pntMin, pntMax);
            XYZ ptCenter = bbLine.Evaluate(0.5, true);
            XYZ ptMarker = new XYZ(ptCenter.X, ptCenter.Y, pntMin.Z);

            ElevationMarker eMarker = null;
            using (Transaction trElvMakar = new Transaction(document1, Text.TRANS_ELEVATIONMARKER))
            {
                if (trElvMakar.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        eMarker = ElevationMarker.CreateElevationMarker(document1, tenkaiJoken.ViewTypeID, ptMarker, tenkaiJoken.ViewScale);
                        trElvMakar.Commit();
                    }
                    catch
                    {
                        trElvMakar.RollBack();
                    }
                }
            }
            if (eMarker == null)
                return null;

            List<string> lstVNames = ViewNames(tenkaiJoken, roomTemp);
            List<string> lstSTitle = SheetTitles(tenkaiJoken, roomTemp);

            List<ElementId> lstVSecId = new List<ElementId>();
            Parameter prmSheetName = null;

            using (Transaction trTenkai = new Transaction(document1, Text.TRANS_CREATEVIEWSECTION))
            {
                if (trTenkai.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        ViewSection secview1 = eMarker.CreateElevation(document1, tenkaiPlan, 1);
                        secview1.Name = GetSafeSectionName(document1, lstVNames[0]);
                        prmSheetName = secview1.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION);
                        prmSheetName?.Set(lstSTitle[0]);
                        lstVSecId.Add(secview1.Id);

                        ViewSection secview3 = eMarker.CreateElevation(document1, tenkaiPlan, 3);
                        secview3.Name = GetSafeSectionName(document1, lstVNames[2]);
                        prmSheetName = secview3.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION);
                        prmSheetName?.Set(lstSTitle[2]);
                        lstVSecId.Add(secview3.Id);

                        ViewSection secview2 = eMarker.CreateElevation(document1, tenkaiPlan, 2);
                        secview2.Name = GetSafeSectionName(document1, lstVNames[1]);
                        prmSheetName = secview2.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION);
                        prmSheetName?.Set(lstSTitle[1]);
                        lstVSecId.Add(secview2.Id);

                        ViewSection secview0 = eMarker.CreateElevation(document1, tenkaiPlan, 0);
                        secview0.Name = GetSafeSectionName(document1, lstVNames[3]);
                        prmSheetName = secview0.get_Parameter(BuiltInParameter.VIEW_DESCRIPTION);
                        prmSheetName?.Set(lstSTitle[3]);
                        lstVSecId.Add(secview0.Id);

                        trTenkai.Commit();
                    }
                    catch (Exception ex)
                    {
                        trTenkai.RollBack();
                        throw new Exception(string.Format(Text.ERR_CREATETENKAIVIEWFAIL, ex.Message));
                    }
                }
            }

            using (Transaction tr1 = new Transaction(document1, Text.TRANS_UPDATECROPBOX))
            {
                if (tr1.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        foreach (ElementId secViewId in lstVSecId)
                            EditSectionView(document1, secViewId, tenkaiJoken.ExtendedRightLeft, tenkaiJoken.ExtendTopBottom, pntMin.Z, pntMax.Z);
                        tr1.Commit();
                    }
                    catch (Exception ex)
                    {
                        tr1.RollBack();
                        throw new Exception(string.Format(Text.ERR_UPDATECROPBOXFAIL, ex.Message));
                    }
                }
            }

            using (Transaction tr1 = new Transaction(document1, Text.TRANS_CREATEDIMENSION))
            {
                if (tr1.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        foreach (ElementId secViewId in lstVSecId)
                            AddDimensionToView(document1, secViewId, roomId, tenkaiJoken);
                        tr1.Commit();
                    }
                    catch (Exception ex)
                    {
                        tr1.RollBack();
                        throw new Exception(string.Format(Text.ERR_CREATEDIMENSIONFAIL, ex.Message));
                    }
                }
            }

            return lstVSecId;
        }

        public static Level GetNextLevel(Room roomElm)
        {
            FilteredElementCollector lvCollector = new FilteredElementCollector(roomElm.Document);
            lvCollector.OfClass(typeof(Level));
            List<Level> lstLevel = lvCollector.Cast<Level>().ToList();
            lstLevel.Sort(new CmpLevelElement());

            for (int i = 0; i < lstLevel.Count - 1; i++)
            {
                Level lv2 = lstLevel[i];
                if (lv2.Id.Equals(roomElm.LevelId))
                    return lstLevel[i + 1];
            }
            return null;
        }

        public static string GetSafeSectionName(Document dbDoc, string newName)
        {
            FilteredElementCollector viewCollector = new FilteredElementCollector(dbDoc);
            viewCollector.OfClass(typeof(ViewSection));
            List<string> lstVname = viewCollector.Cast<ViewSection>().Select(v1 => v1.Name).ToList();
            lstVname.Sort();

            if (!lstVname.Contains(newName))
                return newName;

            int i = 0;
            string strName = newName;
            while (lstVname.Contains(strName))
            {
                i += 1;
                strName = newName + i.ToString();
            }

            return strName;
        }

        public enum ELVDIRECTION
        {
            TOP = 0,
            RIGHT = 1,
            BOTTOM = 2,
            LEFT = 3
        }

        public static bool EditSectionView(
            Document dbDoc,
            ElementId secViewId,
            double horizontalExtend,
            double verticalExtend,
            double lowZval,
            double topZval)
        {
            ViewSection secView = dbDoc.GetElement(secViewId) as ViewSection;
            BoundingBoxXYZ crpBox = secView.CropBox;
            Transform secTran = crpBox.Transform;

            XYZ ucsMax = crpBox.Max;
            XYZ ucsMin = crpBox.Min;
            XYZ wcsMax = secTran.OfPoint(ucsMax);
            XYZ wcsMin = secTran.OfPoint(ucsMin);

            wcsMax = new XYZ(wcsMax.X, wcsMax.Y, topZval);
            wcsMin = new XYZ(wcsMin.X, wcsMin.Y, lowZval);

            XYZ ucsMax2 = secTran.Inverse.OfPoint(wcsMax);
            XYZ ucsMin2 = secTran.Inverse.OfPoint(wcsMin);

            ucsMax2 = new XYZ(ucsMax2.X + horizontalExtend, ucsMax2.Y + verticalExtend, ucsMax2.Z);
            ucsMin2 = new XYZ(ucsMin2.X - horizontalExtend, ucsMin2.Y - verticalExtend, ucsMin2.Z);

            BoundingBoxXYZ newBox = new BoundingBoxXYZ
            {
                Enabled = true,
                Max = ucsMax2,
                Min = ucsMin2,
                Transform = secTran
            };

            secView.CropBox = newBox;
            return false;
        }

        public static BoundingBoxXYZ GetRoomBoundingBox(Document dbDoc, ElementId roomId)
        {
            Solid roomSolid = GetRoomVolumeSolid(dbDoc, roomId);
            if (roomSolid == null)
                return null;
            return roomSolid.GetBoundingBox();
        }

        public static Solid GetRoomVolumeSolid(Document dbDoc, ElementId roomId)
        {
            Room roomElm = dbDoc.GetElement(roomId) as Room;
            if (roomElm.Area == 0)
                return null;

            GeometryElement geoElm = roomElm.ClosedShell;

            Solid roomSolid = null;
            foreach (GeometryObject geoObj in geoElm)
            {
                roomSolid = geoObj as Solid;
                if (roomSolid != null)
                    break;
            }
            if (roomSolid == null)
                return null;
            return roomSolid;
        }

        public static List<string> SheetTitles(CreateTenkaiJoken tenkaijoken, Room room1)
        {
            Parameter prmName = room1.get_Parameter(BuiltInParameter.ROOM_NAME);
            string strName = prmName.AsString();

            Level level1 = room1.Level;
            string strLevel = level1.Name;

            string strSTitleA = string.Format(Text.TXT_SHEETTITLEFORMAT, strName, tenkaijoken.View0);
            string strSTitleB = string.Format(Text.TXT_SHEETTITLEFORMAT, strName, tenkaijoken.View3);
            string strSTitleC = string.Format(Text.TXT_SHEETTITLEFORMAT, strName, tenkaijoken.View6);
            string strSTitleD = string.Format(Text.TXT_SHEETTITLEFORMAT, strName, tenkaijoken.View9);

            return new List<string> { strSTitleA, strSTitleB, strSTitleC, strSTitleD };
        }

        public static List<string> ViewNames(CreateTenkaiJoken tenkaiJoken, Room room1)
        {
            Parameter prmName = room1.get_Parameter(BuiltInParameter.ROOM_NAME);
            Parameter prmNumb = room1.get_Parameter(BuiltInParameter.ROOM_NUMBER);
            string strName = prmName.AsString();
            string strNumb = prmNumb.AsString();

            Level level1 = room1.Level;
            string strLevel = level1.Name;

            bool bContainDirection = false;

            string viewNameA = string.Empty;
            string viewNameB = string.Empty;
            string viewNameC = string.Empty;
            string viewNameD = string.Empty;

            switch (tenkaiJoken.Name1)
            {
                case CreateTenkaiJoken.NamingRule.Direction:
                    viewNameA = tenkaiJoken.View0;
                    viewNameB = tenkaiJoken.View3;
                    viewNameC = tenkaiJoken.View6;
                    viewNameD = tenkaiJoken.View9;
                    bContainDirection = true;
                    break;
                case CreateTenkaiJoken.NamingRule.LevelName:
                    viewNameA = strLevel;
                    viewNameB = strLevel;
                    viewNameC = strLevel;
                    viewNameD = strLevel;
                    break;
                case CreateTenkaiJoken.NamingRule.RoomName:
                    viewNameA = strName;
                    viewNameB = strName;
                    viewNameC = strName;
                    viewNameD = strName;
                    break;
                case CreateTenkaiJoken.NamingRule.RoomNameAndNumber:
                    viewNameA = strNumb + Text.TXT_UNDERSCORE + strName;
                    viewNameB = strNumb + Text.TXT_UNDERSCORE + strName;
                    viewNameC = strNumb + Text.TXT_UNDERSCORE + strName;
                    viewNameD = strNumb + Text.TXT_UNDERSCORE + strName;
                    break;
            }

            switch (tenkaiJoken.Name2)
            {
                case CreateTenkaiJoken.NamingRule.Direction:
                    viewNameA = viewNameA + Text.TXT_UNDERSCORE + tenkaiJoken.View0;
                    viewNameB = viewNameB + Text.TXT_UNDERSCORE + tenkaiJoken.View3;
                    viewNameC = viewNameC + Text.TXT_UNDERSCORE + tenkaiJoken.View6;
                    viewNameD = viewNameD + Text.TXT_UNDERSCORE + tenkaiJoken.View9;
                    bContainDirection = true;
                    break;
                case CreateTenkaiJoken.NamingRule.LevelName:
                    viewNameA = viewNameA + Text.TXT_UNDERSCORE + strLevel;
                    viewNameB = viewNameB + Text.TXT_UNDERSCORE + strLevel;
                    viewNameC = viewNameC + Text.TXT_UNDERSCORE + strLevel;
                    viewNameD = viewNameD + Text.TXT_UNDERSCORE + strLevel;
                    break;
                case CreateTenkaiJoken.NamingRule.RoomName:
                    viewNameA = viewNameA + Text.TXT_UNDERSCORE + strName;
                    viewNameB = viewNameB + Text.TXT_UNDERSCORE + strName;
                    viewNameC = viewNameC + Text.TXT_UNDERSCORE + strName;
                    viewNameD = viewNameD + Text.TXT_UNDERSCORE + strName;
                    break;
                case CreateTenkaiJoken.NamingRule.RoomNameAndNumber:
                    viewNameA = viewNameA + Text.TXT_UNDERSCORE + strNumb + Text.TXT_UNDERSCORE + strName;
                    viewNameB = viewNameB + Text.TXT_UNDERSCORE + strNumb + Text.TXT_UNDERSCORE + strName;
                    viewNameC = viewNameC + Text.TXT_UNDERSCORE + strNumb + Text.TXT_UNDERSCORE + strName;
                    viewNameD = viewNameD + Text.TXT_UNDERSCORE + strNumb + Text.TXT_UNDERSCORE + strName;
                    break;
            }

            switch (tenkaiJoken.Name3)
            {
                case CreateTenkaiJoken.NamingRule.Direction:
                    viewNameA = viewNameA + Text.TXT_UNDERSCORE + tenkaiJoken.View0;
                    viewNameB = viewNameB + Text.TXT_UNDERSCORE + tenkaiJoken.View3;
                    viewNameC = viewNameC + Text.TXT_UNDERSCORE + tenkaiJoken.View6;
                    viewNameD = viewNameD + Text.TXT_UNDERSCORE + tenkaiJoken.View9;
                    bContainDirection = true;
                    break;
                case CreateTenkaiJoken.NamingRule.LevelName:
                    viewNameA = viewNameA + Text.TXT_UNDERSCORE + strLevel;
                    viewNameB = viewNameB + Text.TXT_UNDERSCORE + strLevel;
                    viewNameC = viewNameC + Text.TXT_UNDERSCORE + strLevel;
                    viewNameD = viewNameD + Text.TXT_UNDERSCORE + strLevel;
                    break;
                case CreateTenkaiJoken.NamingRule.RoomName:
                    viewNameA = viewNameA + Text.TXT_UNDERSCORE + strName;
                    viewNameB = viewNameB + Text.TXT_UNDERSCORE + strName;
                    viewNameC = viewNameC + Text.TXT_UNDERSCORE + strName;
                    viewNameD = viewNameD + Text.TXT_UNDERSCORE + strName;
                    break;
                case CreateTenkaiJoken.NamingRule.RoomNameAndNumber:
                    viewNameA = viewNameA + Text.TXT_UNDERSCORE + strNumb + Text.TXT_UNDERSCORE + strName;
                    viewNameB = viewNameB + Text.TXT_UNDERSCORE + strNumb + Text.TXT_UNDERSCORE + strName;
                    viewNameC = viewNameC + Text.TXT_UNDERSCORE + strNumb + Text.TXT_UNDERSCORE + strName;
                    viewNameD = viewNameD + Text.TXT_UNDERSCORE + strNumb + Text.TXT_UNDERSCORE + strName;
                    break;
            }

            if (!bContainDirection)
            {
                viewNameA = viewNameA + Text.TXT_DIRECTIONA;
                viewNameB = viewNameB + Text.TXT_DIRECTIONB;
                viewNameC = viewNameC + Text.TXT_DIRECTIONC;
                viewNameD = viewNameD + Text.TXT_DIRECTIOND;
            }

            return new List<string> { viewNameA, viewNameB, viewNameC, viewNameD };
        }

        public static List<ElementId> AddDimensionToView(
            Document dbDoc,
            ElementId viewId,
            ElementId roomId,
            CreateTenkaiJoken joken)
        {
            List<ElementId> lstElmIds = new List<ElementId>();
            View tenkaiView = dbDoc.GetElement(viewId) as View;

            FilteredElementCollector gdCollector = new FilteredElementCollector(dbDoc, viewId);
            gdCollector.OfClass(typeof(Grid));
            List<Grid> lstGrids = gdCollector.Cast<Grid>().ToList();

            FilteredElementCollector lvCollector = new FilteredElementCollector(dbDoc, viewId);
            lvCollector.OfClass(typeof(Level));
            List<Level> lstLevels = lvCollector.Cast<Level>().ToList();

            if (!joken.DimTypeTorishinID.Equals(ElementId.InvalidElementId) && lstGrids.Count > 1)
                CreateGridDimension(dbDoc, lstGrids, tenkaiView, joken.DimTypeTorishinID);

            if (!joken.DimLevelID.Equals(ElementId.InvalidElementId) && lstLevels.Count > 1)
            {
                List<Dimension> lstDims = CreateLevelDimension(dbDoc, lstLevels, tenkaiView, joken.DimLevelID);
                if (lstDims.Count > 0)
                {
                    foreach (Dimension dm in lstDims)
                        dm.ChangeTypeId(joken.DimLevelID);
                }
            }

            return lstElmIds;
        }

        public static List<Dimension> CreateGridDimension(
            Document dbDoc,
            List<Grid> lstGrid,
            View thisView,
            ElementId dimTypeId)
        {
            DimensionType dimType = dbDoc.GetElement(dimTypeId) as DimensionType;
            Parameter prmSnap = dimType.get_Parameter(BuiltInParameter.DIM_STYLE_DIM_LINE_SNAP_DIST);
            double dblSnap = thisView.Scale * prmSnap.AsDouble();

            ReferenceArray ra0 = dbDoc.Application.Create.NewReferenceArray();
            ReferenceArray ra1 = dbDoc.Application.Create.NewReferenceArray();
            Line ln0 = null;
            Line ln1 = null;

            for (int i = 0; i < lstGrid.Count; i++)
            {
                Grid gd = lstGrid[i];

                Options goption = new Options
                {
                    ComputeReferences = true,
                    IncludeNonVisibleObjects = true,
                    View = thisView
                };
                GeometryElement gElm = gd.get_Geometry(goption);
                Line dgLine = null;
                foreach (GeometryObject gObj in gElm)
                {
                    dgLine = gObj as Line;
                    if (dgLine != null)
                        break;
                }
                if (dgLine == null)
                    continue;

                if (i == 0)
                {
                    ra0.Append(dgLine.Reference);
                    ra1.Append(dgLine.Reference);

                    XYZ gdDire = dgLine.Direction;
                    XYZ dgOrth = gdDire.CrossProduct(thisView.ViewDirection);

                    XYZ lnp0 = dgLine.Evaluate(dblSnap, false);
                    ln0 = Line.CreateUnbound(lnp0, dgOrth);

                    XYZ lnp1 = dgLine.Evaluate(2.0 * dblSnap, false);
                    ln1 = Line.CreateUnbound(lnp1, dgOrth);
                }
                else if (i == lstGrid.Count - 1)
                {
                    ra0.Append(dgLine.Reference);
                    ra1.Append(dgLine.Reference);
                }
                else
                {
                    ra1.Append(dgLine.Reference);
                }
            }

            Dimension dim0 = null;
            Dimension dim1 = null;
            using (SubTransaction sb1 = new SubTransaction(dbDoc))
            {
                if (sb1.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        dim0 = dbDoc.Create.NewDimension(thisView, ln0, ra0);
                        if (lstGrid.Count > 2)
                        {
                            dim1 = dbDoc.Create.NewDimension(thisView, ln1, ra1);
                            dim1.ChangeTypeId(dimTypeId);
                        }
                        sb1.Commit();
                    }
                    catch
                    {
                        sb1.RollBack();
                    }
                }
            }

            dbDoc.Regenerate();

            List<Dimension> lstDims = new List<Dimension>();
            if (dim0 != null)
                lstDims.Add(dim0);
            if (dim1 != null)
                lstDims.Add(dim1);

            return lstDims;
        }

        public static List<Dimension> CreateLevelDimension(
            Document dbDoc,
            List<Level> lstLevel,
            View thisView,
            ElementId dimTypeId)
        {
            DimensionType dimType = dbDoc.GetElement(dimTypeId) as DimensionType;
            Parameter prmSnap = dimType.get_Parameter(BuiltInParameter.DIM_STYLE_DIM_LINE_SNAP_DIST);
            double dblSnap = thisView.Scale * prmSnap.AsDouble();

            ReferenceArray ra1 = dbDoc.Application.Create.NewReferenceArray();
            Line ln1 = null;

            IList<Curve> lstLeveCvs = lstLevel[0].GetCurvesInView(DatumExtentType.ViewSpecific, thisView);
            XYZ dimOrg = lstLeveCvs[0].GetEndPoint(0) + thisView.RightDirection * dblSnap * 2.0;

            ln1 = Line.CreateUnbound(dimOrg, thisView.UpDirection);

            for (int i = 0; i < lstLevel.Count; i++)
            {
                Level lv = lstLevel[i];
                Reference refp = lv.GetPlaneReference();
                ra1.Append(refp);
            }

            Dimension dim1 = null;
            using (SubTransaction sb1 = new SubTransaction(dbDoc))
            {
                if (sb1.Start() == TransactionStatus.Started)
                {
                    try
                    {
                        dim1 = dbDoc.Create.NewDimension(thisView, ln1, ra1);
                        dim1.ChangeTypeId(dimTypeId);
                        sb1.Commit();
                    }
                    catch
                    {
                        sb1.RollBack();
                    }
                }
            }

            dbDoc.Regenerate();

            List<Dimension> lstDims = new List<Dimension>();
            if (dim1 != null)
                lstDims.Add(dim1);

            return lstDims;
        }
    }
}
