using System;
using Tasks.Interfaces;

namespace Tasks.TaskParameters
{
    [Serializable]
    public class MechanicalParameters : GeneralParameters, IChemicalDependent,IThermalDependent
    {
        public string ChemicalFile { get; set; } = "";
        public string ThermalFile { get; set; } =  "";

        public MechanicalConvergence MechanicalConvergence { get; set; } = new MechanicalConvergence();

    }
}
