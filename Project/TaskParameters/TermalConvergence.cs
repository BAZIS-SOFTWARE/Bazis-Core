using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.TaskParameters
{
    [Serializable]
    public class TermalConvergence
    {
        public bool Is_Switched_Tm { get; set; } = true;
        public float Tm { get; set; } = 1500.0f;
    }
}
