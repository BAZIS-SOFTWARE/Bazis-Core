using System;

namespace Project.TaskParameters
{
    [Serializable]
    public class MechanicalParameters : GeneralParameters
    {
        public MechanicalConvergence MechanicalConvergence { get; set; } = new MechanicalConvergence();

        public MechanicalParameters()
        {
            TaskKind = Interfaces.Tasks.TaskKind.механическая;
            CalculationMode = CalculationMode.Stationary;
            InputFields.Add(new PhysicalField
            {
                FieldSet = PhysicalFieldSet.Thermal,
                Source = PhysicalFieldSource.Values
            });
        }
    }
}
