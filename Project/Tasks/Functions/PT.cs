using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.Functions
{
    public class PT : Function
    {
        /// <summary>
        /// PT
        /// </summary>
        public PT()
        {
            Name = FuncName.PT.ToString();
            FunctionType = FuncType.PF;
            AddParameter(new Parameter("TIME"));
            AddParameter(new Parameter("POW", ParameterType.Constant, 20));
        }
        public override double CalcValue()
        {
            // температура тела
            return this["POW"].GetValue();
        }
    }
}
