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
            InputFields.Add(new InputField
            {
                Field = PhysicalField.Temperature,
                Source = InputFieldSource.Constant,
                ConstantValues = new[] { 20.0 }
            });
        }
    }
}
