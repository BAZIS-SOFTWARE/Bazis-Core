
using System;

namespace Project.TaskParameters
{
    [Serializable]
    public class TermalParameters : GeneralParameters, IChemicalDependent 
    {             
        public string ChemicalFile { get; set; } = "";

        public ConvectionParameters ConvectionParameters { get; set; } = new();

        public TermalConvergence TermalConvergence { get; set; } = new TermalConvergence();

        public TermalParameters()
        {
            TaskKind = Interfaces.Tasks.TaskKind.термическая;
        }

    
    }
}
