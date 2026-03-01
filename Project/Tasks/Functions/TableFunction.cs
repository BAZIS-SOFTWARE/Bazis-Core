using MaterialDB.MaterialData;
using Project.Tasks.FrameCreators;
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
    public class TableFunction : Function
    {
        Property table;

        //Parameter<double> parameter;

        public TableFunction(Parameter parameter)
        {
            parameters.Add(parameter);
            //this.parameter = parameter;
        }


/// <inheritdoc/>


        public override double CalcValue()
        {
            
            return table.CalcProp((float)parameters[0].Value);
        }
    }
}
