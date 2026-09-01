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
            InputFields.Add(new InputField
            {
                Field = PhysicalField.Temperature,
                Source = InputFieldSource.Constant,
                ConstantValues = new[] { 20.0 }
            });
        }
    }
}
