using System;
using Tasks.Interfaces;

namespace Tasks
{
    [Serializable]
    public class TermalParameters : GeneralParameters, IChemicalDependent 
    {       
        public string ChemicalFile { get; set; } = "";

        public TermalConvergence TermalConvergence { get; set; } = new TermalConvergence();       
    }
}
