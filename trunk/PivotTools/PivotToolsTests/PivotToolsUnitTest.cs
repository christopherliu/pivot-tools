using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Xml;
using PivotTools;

namespace PivotToolsTests
{
    /// <summary>
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
    [TestClass]
    public class PivotToolsUnitTest
    {
        public PivotToolsUnitTest()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        //This table is just like the one on the website.
        private static DataTable GenerateStubDataTable()
        {
            DataTable myDataTable = new DataTable();
            DataColumn myDataColumn;

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Product";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.String");
            myDataColumn.ColumnName = "Region";
            myDataTable.Columns.Add(myDataColumn);

            myDataColumn = new DataColumn();
            myDataColumn.DataType = Type.GetType("System.Int32");
            myDataColumn.ColumnName = "Sales";
            myDataTable.Columns.Add(myDataColumn);

            DataRow myDataRow = myDataTable.NewRow();
            myDataRow["Product"] = "Cheese";
            myDataRow["Region"] = "U.S.";
            myDataRow["Sales"] = 1000;
            myDataTable.Rows.Add(myDataRow);

            myDataRow = myDataTable.NewRow();
            myDataRow["Product"] = "Eggs";
            myDataRow["Region"] = "U.S.";
            myDataRow["Sales"] = 1100;
            myDataTable.Rows.Add(myDataRow);

            myDataRow = myDataTable.NewRow();
            myDataRow["Product"] = "Eggs";
            myDataRow["Region"] = "Canada";
            myDataRow["Sales"] = 1000;
            myDataTable.Rows.Add(myDataRow);

            return myDataTable;
        }

        /// <summary>
        /// Asserts that we can pivot a very basic data table.
        /// </summary>
        [TestMethod]
        public void TestBasicDataTablePivot()
        {
            DataTable stub = GenerateStubDataTable();
            PivotedTable x = PivotTools.PivotTools.Pivot(stub, "Product", "Region", "Sales");
            Assert.IsTrue(x.Columns.Contains("U.S."));
            Assert.IsTrue(x.Columns.Contains("Canada"));

            //Make sure zero is filled in properly
            Assert.AreEqual(x.Select("Product = 'Cheese'").Single()["Canada"], 0);
            //Make sure regular data pivots as well.
            Assert.AreEqual(x.Select("Product = 'Cheese'").Single()["U.S."], 1000);
        }
    }
}
