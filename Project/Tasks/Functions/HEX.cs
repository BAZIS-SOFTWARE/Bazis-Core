using Project.Tasks.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks.Functions
{
    internal class HEX : Function
    {
        public override double CalcValue()
        {
            var conv = (double)parameters[0].Value;
            var temps = (double)parameters[1].Value;

            double tempm = 0.0;
            if (parameters[1].Value is TableFunction tf)
                tempm = tf.CalcValue();
            else
                tempm = (double)parameters[1].Value;

            // пока для проверки работоспособности
            return conv * temps * tempm;
        }
    }
}
