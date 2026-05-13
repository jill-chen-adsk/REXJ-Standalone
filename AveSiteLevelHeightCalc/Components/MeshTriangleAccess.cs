using Autodesk.Revit.DB;

namespace ADSK.JExtRAC.AveSiteLevelHeightCalc.Components
{
    /// <summary>
    /// Revit API exposes mesh triangle vertices via COM-style get_Vertex; the default indexer is not
    /// consumable from C# without dynamic (CS1546).
    /// </summary>
    internal static class MeshTriangleAccess
    {
        public static XYZ GetVertex(MeshTriangle triangle, int index)
        {
            dynamic t = triangle;
            return t.get_Vertex(index);
        }
    }
}
