
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static IronPython.Modules._ast;

namespace Project.TaskParameters
{
    /// <summary>
    /// Calculation algorithm with respect to physical time.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum CalculationMode
    {
        /// <summary>
        /// Calculation advances through physical time.
        /// </summary>
        Transient,

        /// <summary>
        /// Calculation solves one time-independent equilibrium state.
        /// </summary>
        Stationary
    }

    [Serializable]
    public class GeneralParameters
    {
        public TaskKind TaskKind { get; set; }

        /// <summary>
        /// Calculation algorithm with respect to physical time.
        /// </summary>
        public CalculationMode CalculationMode { get; set; } = CalculationMode.Transient;

        /// <summary>
        /// Conditions selected by PreProc for this calculation.
        /// </summary>
        public List<ConditionReference> ActiveConditions { get; set; }
        public Dictionary<string, double> InitTemp { get; set; } = new Dictionary<string, double>();
        public int SaveRate { get; set; }
        public string RestartFile { get; set; } = "";

        public int UpdMatrixIteration { get; set; } = 1;
        public int Iterations { get; set; }
        public SolverSettings SolverSettings { get; set; } = new SolverSettings();
        public TimeSettings TimeSettings { get; set; } = new TimeSettings();
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{TaskKind}_{TimeSettings.StartTime}_{TimeSettings.StopTime}";
        }
    }
}
