using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace STBLink
{
  public static class RevitCompatibilityUtils
  {
#if (REVIT2021 || REVIT2022 || REVIT2023)
    public static long Value( this Autodesk.Revit.DB.ElementId elementId ) => elementId.IntegerValue ;
    public static SlabShapeEditor SlabShapeEditor( this Autodesk.Revit.DB.Floor floor ) => floor.SlabShapeEditor ;
#else
    public static long Value( this Autodesk.Revit.DB.ElementId elementId ) => elementId.Value ;
    public static SlabShapeEditor SlabShapeEditor(this Autodesk.Revit.DB.Floor floor) => floor.GetSlabShapeEditor();

#endif

#if (REVIT2021 || REVIT2022 || REVIT2023 || REVIT2024)
    public static SlabShapeVertex AddPoint( this Autodesk.Revit.DB.SlabShapeEditor editor, XYZ point)
    {
        return editor.DrawPoint(point);
    }
#else
    public static SlabShapeVertex AddPoint( this Autodesk.Revit.DB.SlabShapeEditor editor, XYZ point)
    {
        return editor.AddPoint(point);
    }

#endif
  }
}