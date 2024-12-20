
using System;
using PreProc.Interfaces;

namespace PreProc.TaskParameters
{
    [Serializable]
    public class TermalParameters : GeneralParameters, IChemicalDependent 
    {       
        public string ChemicalFile { get; set; } = "";

        public TermalConvergence TermalConvergence { get; set; } = new TermalConvergence();

    }
}
