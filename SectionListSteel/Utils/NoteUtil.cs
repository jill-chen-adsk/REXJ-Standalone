using SectionListSteel.Components;
using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SectionListSteel.Utils
{
    public class NoteUtil
    {
        public static void DrawNotes(Geometry cmpGeometry, ref IList<Curve> frameLines, ref IDictionary<XYZ, string> dicTextNoteTitle, int showTitle,
            Dictionary<int, string> dic_items, int selectedIndex, double startX, double endX, double endY, int viewScale, double frameWidth2Title,
            double frameWidth1Title, double frameWidth, int colNum, out double sumHeight)
        {
            sumHeight = 0;
            if (selectedIndex != 0 && dic_items.Count != 0)
            {
                for (int i = 0; i < selectedIndex; i++)
                {
                    int indexNote = i + 1;

                    if (dic_items.ContainsKey(indexNote) == false)
                        continue;

                    var item = dic_items[indexNote];

                    var splits = item.Split('|');

                    string name = splits[0];

                    double height = 0;
                    double.TryParse(splits[1], out height);
                    height = height / 304.8 * viewScale;

                    sumHeight += height;

                    double start_x = startX;
                    double start_y = endY;

                    double end_x = endX;
                    double end_y = endY;

                    var posLeftT = new XYZ(start_x, start_y, 0);
                    var posLeftB = new XYZ(start_x, start_y - height, 0);

                    var line = cmpGeometry.CreateBoundLine(posLeftT, posLeftB);
                    cmpGeometry.NotNullCurveSet(ref frameLines, line);

                    var posRightB = new XYZ(end_x, end_y - height, 0);
                    var posRightT = new XYZ(end_x, end_y, 0);

                    line = cmpGeometry.CreateBoundLine(posRightT, posRightB);
                    cmpGeometry.NotNullCurveSet(ref frameLines, line);

                    line = cmpGeometry.CreateBoundLine(posLeftB, posRightB);
                    cmpGeometry.NotNullCurveSet(ref frameLines, line);

                    XYZ posTitleT = null;
                    XYZ posTitleB = null;
                    if (showTitle == 0)
                    {
                        posTitleT = new XYZ(start_x + frameWidth2Title, start_y, 0);
                        posTitleB = new XYZ(posTitleT.X, start_y - height, 0);
                    }
                    else if (showTitle == 1)
                    {
                        posTitleT = new XYZ(start_x + frameWidth1Title, start_y, 0);
                        posTitleB = new XYZ(posTitleT.X, start_y - height, 0);
                    }

                    line = cmpGeometry.CreateBoundLine(posTitleT, posTitleB);
                    cmpGeometry.NotNullCurveSet(ref frameLines, line);

                    if (name != null)
                    {
                        XYZ posCenter = cmpGeometry.Center2Point(posLeftT, posTitleB);
                        dicTextNoteTitle.Add(posCenter, name);
                    }

                    for (int j = 0; j < colNum; j++)
                    {
                        posTitleT = new XYZ(posTitleT.X + frameWidth, posTitleT.Y, 0);
                        posTitleB = new XYZ(posTitleT.X, posTitleT.Y - height, 0);

                        line = cmpGeometry.CreateBoundLine(posTitleT, posTitleB);
                        cmpGeometry.NotNullCurveSet(ref frameLines, line);
                    }

                    startX = posLeftB.X;
                    endX = posRightB.X;
                    endY = posRightB.Y;
                }
            }
        }
    }
}