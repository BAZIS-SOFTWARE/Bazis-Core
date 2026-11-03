using MaterialDB.MaterialData;
using Project.Tasks.LocalFrames;
using MaterialDB.FunctionData;
using Project.Tasks.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules._ast;

namespace Project.Tasks.Functions
{
    /// <summary>
    /// TableParameter
    /// </summary>
    public class TableParameter : Parameter
    {
        private readonly FunctionCalc functionCalc = new FunctionCalc();

        public Property Table { get; }

        public Parameter Parameter { get; set; }

        public TableParameter(Property table, string name,Parameter parameter) : base(name)
        {
            Table = table;
            Parameter = parameter;
            ParameterType = ParameterType.Constant;
            ParameterKind = ParameterKind.Table;
        }


        /// <inheritdoc/>


        public override double GetValue()
        {   
            var variable = (float)Parameter.GetValue();
            return Value = functionCalc.CalcProp(Table.DataTable, variable);
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{Name}=Table({Table.Name};{Parameter.Name})";
        }
    }
}
