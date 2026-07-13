using System.Windows;
using WpfDataGrid = System.Windows.Controls.DataGrid;
using RvtExtApp = ADSK.JExtRAC.CheckingALVS;

namespace ADSK.JExtRAC.CheckingALVS.Utils
{
  internal static class EnvironmentalCheckWpfGrids
  {
    public static void ConfigureRoomGrid(
        WpfDataGrid grid,
        RvtExtApp.Components.Attribute cmpAttribute,
        RvtExtApp.Entities.DtRoom entDtRoom)
    {
      if (entDtRoom?.Data == null || entDtRoom.Data.Columns.Count == 0)
        return;

      UtilWpfGrid.PrepareGrid(grid);
      int commandKind = entDtRoom.CommandKind;
      string header;

      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameID, cmpAttribute.ResourceText("IDS_TXT_ROOMELEMENTID"),
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameID), true, TextAlignment.Left);

      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameLevelName, cmpAttribute.ResourceText("IDS_TXT_LEVEL"),
          70, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameLevelName), true, TextAlignment.Left);

      UtilWpfGrid.AddComboColumn(grid, entDtRoom.ColNameRoomKind, cmpAttribute.ResourceText("IDS_TXT_ROOMKIND"),
          180,
          UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameRoomKind) && commandKind == 0,
          entDtRoom.EntDtItems?.RoomKind?.DefaultView,
          "Name",
          "Name");

      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameGroupName, cmpAttribute.ResourceText("IDS_TXT_GROUPNAME"),
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameGroupName), true, TextAlignment.Left);

      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameCalcGroupName, cmpAttribute.ResourceText("IDS_TXT_CALCGROUP_N"),
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameCalcGroupName), true, TextAlignment.Left);

      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameRoomName, cmpAttribute.ResourceText("IDS_TXT_ROOMNAME"),
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameRoomName), true, TextAlignment.Left);

      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameRoomNo, cmpAttribute.ResourceText("IDS_TXT_ROOMNO"),
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameRoomNo), true, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameArea, cmpAttribute.ResourceText("IDS_TXT_LEGALAREA"),
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameArea), true, TextAlignment.Right);

      header = commandKind switch
      {
        1 => cmpAttribute.ResourceText("IDS_TXT_SMOKENESCOEFF_N"),
        2 => cmpAttribute.ResourceText("IDS_TXT_VENTILATIONNESCOEFF_N"),
        _ => cmpAttribute.ResourceText("IDS_TXT_LIGHTINGNESCOEFF_N")
      };
      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameNecessaryCoefficient, header,
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameNecessaryCoefficient), true, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameAverageCeilingHeight, cmpAttribute.ResourceText("IDS_TXT_AVERAGECEILINGHEIGHT_N"),
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameAverageCeilingHeight) && commandKind == 1, true, TextAlignment.Right);

      header = commandKind switch
      {
        1 => cmpAttribute.ResourceText("IDS_TXT_SMOKENESAREA_N"),
        2 => cmpAttribute.ResourceText("IDS_TXT_VENTILATIONNESAREA_N"),
        _ => cmpAttribute.ResourceText("IDS_TXT_LIGHTINGNESAREA_N")
      };
      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameNecessaryArea, header,
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameNecessaryArea), true, TextAlignment.Right);

      header = cmpAttribute.ResourceText("IDS_TXT_TOTAL") + "\n" + commandKind switch
      {
        1 => cmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEAREA_N"),
        2 => cmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEAREA_N"),
        _ => cmpAttribute.ResourceText("IDS_TXT_LIGHTINGUSABLEAREA_N")
      };
      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameTotalUsableArea, header,
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameTotalUsableArea), true, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtRoom.ColNameJudgment, cmpAttribute.ResourceText("IDS_TXT_JUDGMENT"),
          0, UtilWpfGrid.HasColumn(entDtRoom.Data, entDtRoom.ColNameJudgment), true, TextAlignment.Left);
    }

    public static void ConfigurePartsGrid(
        WpfDataGrid grid,
        RvtExtApp.Components.Attribute cmpAttribute,
        RvtExtApp.Entities.DtWinDoor entDtWinDoor)
    {
      if (entDtWinDoor?.Data == null || entDtWinDoor.Data.Columns.Count == 0)
        return;

      UtilWpfGrid.PrepareGrid(grid);
      int commandKind = entDtWinDoor.CommandKind;
      string header;

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameID, cmpAttribute.ResourceText("IDS_TXT_PARTSELEMENTID"),
          130, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameID), true, TextAlignment.Left);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameCategory, cmpAttribute.ResourceText("IDS_TXT_PARTSELEMENTTYPE"),
          150, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameCategory), true, TextAlignment.Left);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameAffiliationRoom, cmpAttribute.ResourceText("IDS_TXT_PARTSAFFILIATIONROOM"),
          210, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameAffiliationRoom), true, TextAlignment.Left);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameWidth, cmpAttribute.ResourceText("IDS_TXT_WIDTH"),
          85, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameWidth), true, TextAlignment.Left);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameHeight, cmpAttribute.ResourceText("IDS_TXT_HEIGHT"),
          85, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameHeight), true, TextAlignment.Left);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameSign, cmpAttribute.ResourceText("IDS_TXT_PARTSSIGN_N"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameSign), true, TextAlignment.Left);

      UtilWpfGrid.AddCheckColumn(grid, entDtWinDoor.ColNameVeranda, cmpAttribute.ResourceText("IDS_TXT_VERANDA"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameVeranda) && commandKind == 0, false);

      UtilWpfGrid.AddCheckColumn(grid, entDtWinDoor.ColNameRoadSide, cmpAttribute.ResourceText("IDS_TXT_ROADSIDE"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameRoadSide) && commandKind == 0, false);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameDistHorizontalMeas, cmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_MEAS"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameDistHorizontalMeas) && commandKind == 0, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameDistHorizontalCorr, cmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_CORR"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameDistHorizontalCorr) && commandKind == 0, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameHorizontalDist, cmpAttribute.ResourceText("IDS_TXT_DISTHORIZONTAL_N"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameHorizontalDist) && commandKind == 0, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameDistVerticalMeas, cmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_MEAS"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameDistVerticalMeas) && commandKind == 0, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameDistVerticalCorr, cmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_CORR"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameDistVerticalCorr) && commandKind == 0, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameVerticalDist, cmpAttribute.ResourceText("IDS_TXT_DISTVERTICAL_N"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameVerticalDist) && commandKind == 0, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameDsH, cmpAttribute.ResourceText("IDS_TXT_DSH"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameDsH) && commandKind == 0, true, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameA, cmpAttribute.ResourceText("IDS_TXT_ALPHA"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameA) && commandKind == 0, true, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameB, cmpAttribute.ResourceText("IDS_TXT_BETA"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameB) && commandKind == 0, true, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameD, cmpAttribute.ResourceText("IDS_TXT_D"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameD) && commandKind == 0, true, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameATemp, cmpAttribute.ResourceText("IDS_TXT_A_TEMP_N"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameATemp) && commandKind == 0, true, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameACorr, cmpAttribute.ResourceText("IDS_TXT_A_CORR_N"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameACorr) && commandKind == 0, true, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameSmokeWinWidth, cmpAttribute.ResourceText("IDS_TXT_SMOKEWINWIDTH_N"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameSmokeWinWidth) && commandKind == 1, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameSmokeWinHeight, cmpAttribute.ResourceText("IDS_TXT_SMOKEWINHEIGHT_N"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameSmokeWinHeight) && commandKind == 1, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameOpenCoefficient, cmpAttribute.ResourceText("IDS_TXT_OPENCOEFF"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameOpenCoefficient) && commandKind != 0, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameHeadHeight, cmpAttribute.ResourceText("IDS_TXT_UPPERMOSTSIDEHEIGHT"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameHeadHeight) && commandKind == 1, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameCeilingHeight, cmpAttribute.ResourceText("IDS_TXT_CEILINGHEIGHT"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameCeilingHeight) && commandKind == 1, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameSmokeWallLength, cmpAttribute.ResourceText("IDS_TXT_SMOKEWALLLENGTH_N"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameSmokeWallLength) && commandKind == 1, false, TextAlignment.Right);

      header = commandKind switch
      {
        1 => cmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEWIDTH_N"),
        2 => cmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEWIDTH_N"),
        _ => cmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEWIDTH_N")
      };
      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameUsableWidth, header,
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameUsableWidth) && commandKind != 1, false, TextAlignment.Right);

      header = commandKind switch
      {
        1 => cmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEHEIGHT_N"),
        2 => cmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEHEIGHT_N"),
        _ => cmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEHEIGHT_N")
      };
      bool showUsableHeight = commandKind != 1;
      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameUsableHeight, header,
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameUsableHeight) && showUsableHeight, false, TextAlignment.Right);

      header = commandKind switch
      {
        1 => cmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEHEIGHT_N"),
        2 => cmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEHEIGHT_N"),
        _ => cmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEHEIGHT_N")
      };
      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameUsableHeightSmoke, header,
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameUsableHeightSmoke) && commandKind == 1, false, TextAlignment.Right);

      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameUsableOpenArea, cmpAttribute.ResourceText("IDS_TXT_OPENINGUSABLEAREA_N"),
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameUsableOpenArea) && commandKind == 0, true, TextAlignment.Right);

      header = commandKind switch
      {
        1 => cmpAttribute.ResourceText("IDS_TXT_SMOKEUSABLEAREA_N"),
        2 => cmpAttribute.ResourceText("IDS_TXT_VENTILATIONUSABLEAREA_N"),
        _ => cmpAttribute.ResourceText("IDS_TXT_LIGHTINGUSABLEAREA_N")
      };
      UtilWpfGrid.AddTextColumn(grid, entDtWinDoor.ColNameUsableArea, header,
          0, UtilWpfGrid.HasColumn(entDtWinDoor.Data, entDtWinDoor.ColNameUsableArea), true, TextAlignment.Right);
    }
  }
}
