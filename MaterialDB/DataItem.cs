using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Data;

namespace MaterialDB
{
    /// <summary>
    /// Общие табличные данные свойства материала или функции.
    /// </summary>
    [Serializable]
    public abstract class DataItem
    {
        /// <summary>
        /// Создаёт объект с заданной таблицей данных без указания вида зависимости.
        /// </summary>
        protected DataItem(string name, string x_unit, string y_unit, string units, DataTable dataTable)
            : this(name, x_unit, y_unit, units, dataTable, ArgumentKind.Undefined)
        {
        }

        /// <summary>
        /// Создаёт объект с заданной таблицей данных и видом зависимости.
        /// </summary>
        protected DataItem(string name, string x_unit, string y_unit, string units, DataTable dataTable, ArgumentKind argumentKind)
        {
            Name = name;
            X_unit = x_unit;
            Y_unit = y_unit;
            Units = units;
            DataTable = dataTable;
            ArgumentKind = argumentKind;
        }

        /// <summary>
        /// Копирует общие данные и таблицу исходного объекта.
        /// </summary>
        protected DataItem(string copyName, DataItem source)
        {
            Name = copyName;
            X_unit = source.X_unit;
            Y_unit = source.Y_unit;
            Units = source.Units;
            ArgumentKind = source.ArgumentKind;
            DataTable = source.DataTable.Copy();
        }

        public string Name { get; set; }
        public string X_unit { get; set; }
        public string Y_unit { get; set; }
        public string Units { get; set; }

        /// <summary>
        /// Вид зависимости таблицы. Undefined в файл не записывается.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public ArgumentKind ArgumentKind { get; set; }

        public DataTable DataTable { get; set; }

        public double this[int row, int col]
        {
            get { return Convert.ToDouble(DataTable.Rows[row][col]); }
            set { DataTable.Rows[row][col] = value; }
        }

        /// <summary>
        /// Создаёт независимую копию табличных данных с новым именем.
        /// </summary>
        public abstract DataItem Copy(string copyName);

        /// <summary>
        /// Добавляет пустую строку в таблицу данных.
        /// </summary>
        public void AddRow()
        {
            var row = DataTable.NewRow();
            DataTable.Rows.Add(row);
        }

        /// <summary>
        /// Возвращает имя и единицы измерения.
        /// </summary>
        public override string ToString()
        {
            return $"{Name},{Units}";
        }
    }
}
