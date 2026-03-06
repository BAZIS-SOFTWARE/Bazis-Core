using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.Functions
{
    public class FHEX : Function
    {
        /// <summary>
        /// FHEX
        /// </summary>
        public FHEX()
        {
            Name = FuncName.FHEX.ToString();
            FunctionType = FuncType.PF;
            AddParameter(new Parameter("TEMPM", ParameterType.Constant, 20));
            AddParameter(new Parameter("TEMPS"));
            AddParameter(new Parameter("HEX", ParameterType.Constant, 0.000025)); // w/mm^2 * temp
        }
        /// <inheritdoc/>
        public override double CalcValue()
        {
            // суммарный коэф.теплоодачи
            return this["HEX"].GetValue();
        }
    }
}
