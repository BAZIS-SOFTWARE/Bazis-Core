using System;
using Tasks.Interfaces;

namespace Tasks.TaskParameters
{
    [Serializable]
    public class ChemicalParameters : GeneralParameters, IThermalDependent
    {
        public string ThermalFile { get; set; } = "";

        public float InitConcentration { get; set; }

        public ChemicalConvergence ChemicalConvergence { get; set; } = new ChemicalConvergence();
    }
}
