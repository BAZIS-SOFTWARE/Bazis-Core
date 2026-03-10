using Project.TaskParameters;
using Project.Tasks.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.Functions
{
    public class DHEX : Function
    {
        /// <summary>
        /// DHEX
        /// </summary>
        public DHEX()
        {
            Name = FuncName.DHEX.ToString();
            FunctionType = FuncType.PF;
            AddParameter(new Parameter("TIME")); // неизвестная, вычисляется во время расчета
            AddParameter(new Parameter("TEMPS")); // неизвестная, вычисляется во время расчета
            AddParameter(new Parameter("CONV",ParameterType.Constant,0.000025)); // w/mm^2 * temp
            AddParameter(new Parameter("TEMPM",ParameterType.Constant,20));
            AddParameter(new Parameter("STEF", ParameterType.Constant, 5.67*1e-14)); // w / mm^2 * temp^4
            AddParameter(new Parameter("BLACK", ParameterType.Constant, 0.6)); // приведеная степень черноты 1 / (1/e1 + 1/e2 -1)
        }
/// <inheritdoc/>

        public override double CalcValue()
        {
            // излуч = BLACK * STEF * (TEMPS^4 - TEMPM^4)

            var conv = this["CONV"].GetValue();
            var temps = this["TEMPS"].GetValue();
            var tempm = this["TEMPM"].GetValue();
            var black = this["STEF"].GetValue();
            var stef = this["BLACK"].GetValue();

            // нужно проверить работоспособность
            return conv + black * stef *(Math.Pow(temps,4) - Math.Pow(tempm, 4));
        }
    }
}
