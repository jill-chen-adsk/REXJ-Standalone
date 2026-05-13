using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.ExportSchedule.Utils
{
    /// <summary>
    /// This class contains information about a cell data in the row table
    /// </summary>
    public class CellTable
    {
        // メンバ変数

        #region Member Variables

        //Text of cell
        public string Text = null;

        //Row index
        public int Row = -1;

        //Column index
        public int Col = -1;

        //Merged cell
        public TableMergedCell MergeCell = null;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        public CellTable(string text, int row, int col)
        {
            Text = text;
            Row = row;
            Col = col;
        }

        #endregion Constructor
    }
}