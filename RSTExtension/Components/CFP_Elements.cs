using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using JExtComCompat;

namespace RSTExtension.Components
{
    public class CFP_Elements : RvtElements
    {
        public CFP_Elements(UIDocument uidoc) : base(uidoc)
        {
        }

        public Level? GetElementLevel(int elemId)
        {
            Element? elem = GetElementDoc(elemId);
            return elem as Level;
        }

        public ViewPlan? ActiveViewAreaPlan => Document.ActiveView as ViewPlan;

        public IList<Level> Levels
        {
            get
            {
                var sysTypes = new List<System.Type> { typeof(Level) };
                IList<Element> elems = GetElementsDoc(null, sysTypes, null, null, null);
                IList<Level> elemLevels = CastElements<Level>(elems);
                var ret = new List<Level>();
                if (elemLevels == null)
                {
                    return ret;
                }

                foreach (Level elemLevel in elemLevels)
                {
                    if (ret.Count == 0)
                    {
                        ret.Add(elemLevel);
                    }
                    else
                    {
                        double elevation = elemLevel.Elevation;
                        int cntRet = ret.Count;
                        int index = -1;
                        for (int j = 0; j < cntRet; ++j)
                        {
                            if (ret[j].Elevation > elevation)
                            {
                                index = j;
                                break;
                            }
                        }
                        if (index > -1)
                        {
                            ret.Insert(index, elemLevel);
                        }
                        else
                        {
                            ret.Add(elemLevel);
                        }
                    }
                }

                return ret;
            }
        }
    }
}
