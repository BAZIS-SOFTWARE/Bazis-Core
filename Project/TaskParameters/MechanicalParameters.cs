using System;

namespace Project.TaskParameters
{
    [Serializable]
    public class MechanicalParameters : GeneralParameters, IChemicalDependent,IThermalDependent
    {
        public string ChemicalFile { get; set; } = "";
        public string ThermalFile { get; set; } =  "";

        public MechanicalConvergence MechanicalConvergence { get; set; } = new MechanicalConvergence();

    }
}
