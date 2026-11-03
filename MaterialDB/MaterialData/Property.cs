using MaterialDB.Interfaces;
using System;
using System.Data;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Property
    /// </summary>
    [Serializable]
    public class Property : IProperty
    {
        public double this[int row,int col]
        {
            get { return Convert.ToDouble(DataTable.Rows[row][col]); }
            set { DataTable.Rows[row][col] = value; }
        }


        /// <inheritdoc/>
        public string Name 
        {
            get;
            set;
        }
/// <inheritdoc/>

        public string X_unit { get; set; }
/// <inheritdoc/>


        public string Y_unit { get; set; }
/// <inheritdoc/>


        public string Units { get;set; }
/// <inheritdoc/>

        public DataTable DataTable 
        { 
            get; 
            set; 
        }
/// <inheritdoc/>
        public override string ToString()
        {
            return $"{Name},{Units}";
        }

        public Property(string name, string x_unit, string y_unit, string units, DataTable dataTable)
        {
            Name = name;
            X_unit = x_unit;
            Y_unit = y_unit;
            Units = units;
            DataTable = dataTable;
        }

        /// <inheritdoc/>
        public Property Copy(string copyName)
        {
            var newProperty = new Property(copyName, X_unit, Y_unit, Units, DataTable.Copy());

            return newProperty;
        }
/// <inheritdoc/>
        public void AddRow()
        {
            var row = DataTable.NewRow();
            DataTable.Rows.Add(row);
        }
    }
}
