using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADSK.JExtRAC.ExportSchedule.Utils
{
    /// <summary>
    /// This class contains information about a row data in the schedule
    /// </summary>
    internal class RowTable
    {
        // メンバ変数

        #region Member Variables

        //Cell table list of a row
        public List<CellTable> _CellTables = new List<CellTable>();

        //Index of row table
        public int _RowIndex = -1;

        //Row table is group
        public bool _Header_Footer = false;

        //Element related with row table
        public Element _Element = null;

        #endregion Member Variables

        // コンストラクタ

        #region Constructor

        public RowTable(int index)
        {
            _RowIndex = index;
        }

        #endregion Constructor
    }
}