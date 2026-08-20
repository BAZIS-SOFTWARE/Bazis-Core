using System;

namespace Project.TaskParameters
{
    [Serializable]
    public class ChemicalParameters : GeneralParameters, IThermalDependent
    {
        public double ThermalLoad { get; set; } = 20;
        public string ThermalFile { get; set; } = "";
        public float InitConcentration { get; set; }

        public ChemicalConvergence ChemicalConvergence { get; set; } = new ChemicalConvergence();
        public bool ThermalLoadIsFile { get; set; } = false;

        public ChemicalParameters()
        {
            TaskKind = Interfaces.Tasks.TaskKind.химическая;
        }
    }
}
