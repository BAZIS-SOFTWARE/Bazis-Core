
using System;

namespace Project.TaskParameters
{
    [Serializable]
    public class TermalParameters : GeneralParameters, IChemicalDependent 
    {       
        public string ChemicalFile { get; set; } = "";

        public TermalConvergence TermalConvergence { get; set; } = new TermalConvergence();

    }
}
