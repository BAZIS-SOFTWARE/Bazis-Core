using Project.TaskParameters;
using Project.Tasks.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.Functions
{
    public class HEX : Function
    {
        public HEX()
        {
            Name = FuncName.HEX.ToString();
            FunctionType = FuncType.PF;
            AddParameter(new Parameter("CONV",ParameterType.Constant,1));
            AddParameter(new Parameter("TEMPS"));
            AddParameter(new Parameter("TEMPM",ParameterType.Constant,20));
        }
/// <inheritdoc/>

        public override double CalcValue()
        {
            var conv = this["CONV"].GetValue();
            var temps = this["TEMPS"].GetValue();

            var tempm = this["TEMPM"].GetValue();

            // пока для проверки работоспособности
            return conv * temps * tempm;
        }
    }
}
