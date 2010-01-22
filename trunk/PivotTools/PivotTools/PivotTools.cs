using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace PivotTools
{
    public static class PivotTools
    {
        /// <summary>
        /// See documentation for the version of this function accepting an
        /// IDataReader.
        /// </summary>
        /// <param name="dataValues">Any DataTable. It should be sorted
        /// before the pivot takes place.</param>
        /// <param name="keyColumn">Column in the DataTable which
        /// serves to identify each row. Your DataTable's recordset should be
        /// primarily grouped and sorted by this column.</param>
        /// <param name="pivotNameColumn">Column in the DataTable that contains
        /// the values you'd like to transform from rows into columns.</param>
        /// <param name="pivotValueColumn">Column that in the DataReader that
        /// contains the values to pivot into the appropriate columns.</param>
        /// <param name="indexOfLastNonPivotColumn">Returns the index of the
        /// last non-pivot column. All columns after it are pivoted.</param>
        /// <returns></returns>
        public static PivotedTable Pivot(DataTable dataValues, string keyColumn,
            string pivotNameColumn, string pivotValueColumn)
        {
            return Pivot(dataValues.CreateDataReader(), keyColumn, pivotNameColumn, pivotValueColumn);
        }

        /// <summary>
        /// Pivots results from a IDataReader into a new table. The resulting
        /// datatype is a PivotedTable with the pivoted columns at the end of
        /// the table.
        /// Adapted from the code at 
        /// http://weblogs.sqlteam.com/jeffs/articles/5091.aspx
        /// Performance: Should generally be faster than the SQL equivalent,
        /// but this is not always guaranteed. More discussion:
        /// http://weblogs.sqlteam.com/jeffs/jeffs/archive/2005/05/12/5127.aspx
        /// </summary>
        /// <param name="dataValues">Any open DataReader object, ready
        /// to be transformed and pivoted into a DataTable. It should be sorted
        /// before the pivot takes place.</param>
        /// <param name="keyColumn">Column in the DataReader which
        /// serves to identify each row. Your DataReader's recordset should be
        /// primarily grouped and sorted by this column.</param>
        /// <param name="pivotNameColumn">Column in the DataReader that contains
        /// the values you'd like to transform from rows into columns.</param>
        /// <param name="pivotValueColumn">Column that in the DataReader that
        /// contains the values to pivot into the appropriate columns.</param>
        /// <param name="indexOfLastNonPivotColumn">Returns the index of the
        /// last non-pivot column. All columns after it are pivoted.</param>
        /// <returns></returns>
        public static PivotedTable Pivot(IDataReader dataValues, string keyColumn,
            string pivotNameColumn, string pivotValueColumn)
        {
            PivotedTable tableResults = new PivotedTable();
            int pValIndex, pNameIndex;
            string sColumnName;
            bool isFirstRow = true;
            tableResults.IndexOfLastNonPivotColumn = dataValues.FieldCount - 3;

            // Add non-pivot columns to the data table:
            pValIndex = dataValues.GetOrdinal(pivotValueColumn);
            pNameIndex = dataValues.GetOrdinal(pivotNameColumn);
            for (int colIndex = 0; colIndex <= dataValues.FieldCount - 1; colIndex++)
                if (colIndex != pValIndex && colIndex != pNameIndex)
                    tableResults.Columns.Add(dataValues.GetName(colIndex), dataValues.GetFieldType(colIndex));

            // Now, fill up the table with the data:
            DataRow r = null;
            string LastKey = "//dummy//";
            while (dataValues.Read())
            {
                // see if we need to start a new row
                if (dataValues[keyColumn].ToString() != LastKey)
                {
                    // if this isn't the very first row, we need to add the last one to the table
                    if (!isFirstRow)
                        tableResults.Rows.Add(r);
                    r = tableResults.NewRow();
                    isFirstRow = false;

                    // Add all non-pivot column values to the new row:
                    for (int i = 0; i <= tableResults.IndexOfLastNonPivotColumn; i++)
                        r[i] = dataValues[tableResults.Columns[i].ColumnName];
                    LastKey = dataValues[keyColumn].ToString();
                }

                // assign the pivot values to the proper column; add new columns if needed:
                sColumnName = dataValues[pNameIndex].ToString();
                if (sColumnName != "")
                {
                    if (!tableResults.Columns.Contains(sColumnName))
                    {
                        DataColumn c = tableResults.Columns.Add(sColumnName, dataValues.GetFieldType(pValIndex));
                        // set the index so that it is sorted properly:
                        int newOrdinal = c.Ordinal;
                        for (int i = newOrdinal - 1; i >= dataValues.FieldCount - 2; i--)
                            if (c.ColumnName.CompareTo(tableResults.Columns[i].ColumnName) < 0)
                                newOrdinal = i;
                        c.SetOrdinal(newOrdinal);
                    }

                    r[sColumnName] = dataValues[pValIndex];
                }
            }

            // add that final row to the datatable:
            if (r != null)
                tableResults.Rows.Add(r);

            // Add in zeroes
            for (int row = 0; row < tableResults.Rows.Count; row++)
                for (int col = tableResults.IndexOfLastNonPivotColumn - 1; col < tableResults.Columns.Count; col++)
                    if (tableResults.Rows[row][col].ToString() == "")
                        tableResults.Rows[row][col] = 0;

            dataValues.NextResult();
            return tableResults;
        }
    }
}
