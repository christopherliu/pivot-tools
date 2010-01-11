using System.Web.UI.WebControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace PivotTools
{
    /// <summary>
    /// Represents a datatable that's been pivoted. 
    /// </summary>
    public class PivotedTable : DataTable
    {
        /// <summary>
        /// The DataColumns up to this one are regular, and the columns after this one are pivoted.
        /// Example:
        /// item, price, Q1sales, Q2sales, Q3sales
        /// price is the last non pivot column.
        /// You can use this to figure out where dynamic columns start.
        /// </summary>
        public int IndexOfLastNonPivotColumn
        {
            get;
            internal set; //If we want to start doing pivots outside of DatabaseUtils, create a constructor so we can only set it once.
        }

        /// <summary>
        /// Attaches the pivot columns to the end of the GridView.
        /// </summary>
        /// <param name="gvExcel"></param>
        public void AddPivotColumnsToGridView(ref global::System.Web.UI.WebControls.GridView gvExcel)
        {
            for (int col = IndexOfLastNonPivotColumn + 1; col <= Columns.Count - 1; col++)
            {
                BoundField pivotColumn = new BoundField();
                pivotColumn.DataField = Columns[col].ColumnName;
                //HTMLEncode doesn't make (201) display correctly: .Replace("(", "&#40;").Replace("-", "&#8722;");
                //Must experiment with something else later.
                pivotColumn.HeaderText = Columns[col].ColumnName;
                gvExcel.Columns.Add(pivotColumn);
            }
        }

    }
}
