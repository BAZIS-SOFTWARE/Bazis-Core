using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.Functions
{
    public class TT : Function
    {
        /// <summary>
        /// TT
        /// </summary>
        public TT()
        {
            Name = FuncName.TT.ToString();
            FunctionType = FuncType.PF;
            AddParameter(new Parameter("TIME"));
            AddParameter(new Parameter("TEMPS", ParameterType.Constant, 20));
        }
        public override double CalcValue()
        {
            // температура тела
            return this["TEMPS"].GetValue();
        }
    }
}
