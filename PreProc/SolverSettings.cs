using System;
using Tasks.Interfaces;

namespace Tasks
{
    [Serializable]
    public class SolverSettings : ISolverSettings
    {   
        public int MaxIter { get; set; }
        public string Solver { get; set; } = "Gauss_direct";

        public float MaxRelaxation { get; set; } = 1.5f;
        public float Relaxation { get; set; }

        public float Precision { get; set; }

        public string Priority { get; set; } = "Наивысший";

    }
}