
using System;
using System.Collections.Generic;

namespace Project.TaskParameters
{
    [Serializable]
    public class TermalParameters : GeneralParameters
    {
        public TermalConvergence TermalConvergence { get; set; } = new TermalConvergence();

        /// <summary>
        /// Metallurgical processes evaluated by the thermal task.
        /// </summary>
        public List<string> MetallurgicalProcesses { get; set; } = new();

        public TermalParameters()
        {
            TaskKind = Interfaces.Tasks.TaskKind.термическая;
            CalculationMode = CalculationMode.Transient;
        }

    
    }
}
