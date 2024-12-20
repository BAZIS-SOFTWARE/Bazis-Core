using System;
using PreProc.Interfaces;

namespace PreProc
{
    [Serializable]
    public class MechanicalParameters : GeneralParameters, IChemicalDependent,IThermalDependent
    {
        public string ChemicalFile { get; set; } = "";
        public string ThermalFile { get; set; } =  "";

        public MechanicalConvergence MechanicalConvergence { get; set; } = new MechanicalConvergence();

    }
}
