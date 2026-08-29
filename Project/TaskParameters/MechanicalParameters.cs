using System;

namespace Project.TaskParameters
{
    [Serializable]
    public class MechanicalParameters : GeneralParameters, IChemicalDependent,IThermalDependent
    {
        public string ChemicalFile { get; set; } = "";
        public double ThermalLoad { get; set; } =  20;
        public ThermalFile ThermalFile { get; set; } = new();

        // Дополнительный флаг
        public bool ThermalLoadIsFile { get; set; } = false;

        public MechanicalConvergence MechanicalConvergence { get; set; } = new MechanicalConvergence();

        public MechanicalParameters()
        {
            TaskKind = Interfaces.Tasks.TaskKind.механическая;
            CalculationMode = CalculationMode.Stationary;
        }
    }
}
