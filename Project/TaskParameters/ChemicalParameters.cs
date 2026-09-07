using System;

namespace Project.TaskParameters
{
    [Serializable]
    public class ChemicalParameters : GeneralParameters
    {
        public ChemicalConvergence ChemicalConvergence { get; set; } = new ChemicalConvergence();

        public ChemicalParameters()
        {
            TaskKind = Interfaces.Tasks.TaskKind.химическая;
            InputSets.Add(new PhysicalSet
            {
                Name = PhysicalSetName.Thermal,
                Source = PhysicalSetSource.Values
            });
        }
    }
}
