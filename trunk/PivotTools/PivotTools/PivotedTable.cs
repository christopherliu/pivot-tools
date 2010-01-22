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
    /// 
    /// Copyright 2010 MagazineRadar
    ///  
    /// Licensed under the Apache License, Version 2.0 (the "License");
    /// you may not use this file except in compliance with the License.
    /// You may obtain a copy of the License at
    /// 
    /// http://www.apache.org/licenses/LICENSE-2.0
    /// 
    /// Unless required by applicable law or agreed to in writing, software
    /// distributed under the License is distributed on an "AS IS" BASIS,
    /// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    /// See the License for the specific language governing permissions
    /// and limitations under the License. 
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
