using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.Functions
{
    internal class BT : Function
    {
        /// <summary>
        /// BT
        /// </summary>
        public BT()
        {
            Name = FuncName.BT.ToString();
            FunctionType = FuncType.PF;
            AddParameter(new Parameter("TIME"));
            AddParameter(new Parameter("TEMPM", ParameterType.Constant, 20));
        }
        public override double CalcValue()
        {
            // температура тела
            return this["TEMPM"].GetValue();
        }
    }
}
