using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks.TaskParameters
{
    [Serializable]
    public class ChemicalConvergence
    {
        public bool Is_Switched_Cm { get; set; }
        public float Cm { get; set; }
    }
}
