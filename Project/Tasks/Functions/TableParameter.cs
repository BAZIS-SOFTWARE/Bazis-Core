#pragma warning disable CS0618 // FunctionItem — публичный тип API Project до перехода на DataItem в Project 7.0.0 / MaterialDB 2.0.0
using MaterialDB.FunctionData;

namespace Project.Tasks.Functions
{
    /// <summary>
    /// TableParameter
    /// </summary>
    public class TableParameter : Parameter
    {
        private readonly LinearTable linearTable;

        public FunctionItem Table { get; }

        public Parameter Parameter { get; set; }

        /// <summary>
        /// Таблица переводится в массивы один раз; правки функции учитываются при повторном разборе задачи.
        /// </summary>
        public TableParameter(FunctionItem table, string name,Parameter parameter) : base(name)
        {
            Table = table;
            Parameter = parameter;
            ParameterType = ParameterType.Constant;
            ParameterKind = ParameterKind.Table;
            linearTable = new LinearTable(table);
        }


        /// <inheritdoc/>


        public override double GetValue()
        {
            var variable = Parameter.GetValue();
            return Value = linearTable.Interpolate(variable);
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{Name}=Table({Table.Name};{Parameter.Name})";
        }
    }
}
