using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PreProc.TaskParameters
{
    [Serializable]
    public class MechanicalConvergence
    {
        public bool Is_Switched_Um { get; set; }
        public float Um { get; set; }
        public float DUm { get; set; }
        public bool Is_Physically_NonLinear { get; set; } 
        public float PlasticityCriterion { get; set; }
        public float SiStm { get; set; }
        public float MaterialPlasticityCoeff { get; set; }
    }
}
