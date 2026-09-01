
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static IronPython.Modules._ast;
using System.Linq;

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
        /// <summary>
        /// Physical fields supplied as constants or imported from previously completed tasks.
        /// </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<InputField> InputFields { get; set; } = new List<InputField>();
        /// <summary>
        /// Initial state defined by group values or a result of the same physics.
        /// </summary>
        public InitialState InitialState { get; set; } = new InitialState();
        public int SaveRate { get; set; }

        public int UpdMatrixIteration { get; set; } = 1;
        public int Iterations { get; set; }
        [JsonProperty(Required = Required.Always)]
        public SolverSettings SolverSettings { get; set; } = null!;
        public TimeSettings TimeSettings { get; set; } = new TimeSettings();

        /// <summary>Returns the configured source of a physical field.</summary>
        public InputField GetInputField(PhysicalField field)
        {
            var fields = (InputFields ?? new List<InputField>()).Where(input => input.Field == field).ToList();
            if (fields.Count > 1)
                throw new InvalidOperationException($"Для поля {field} указано несколько источников.");

            return fields.SingleOrDefault();
        }

        /// <summary>Returns initial values configured for a physical field.</summary>
        public InitialCondition GetInitialCondition(PhysicalField field)
        {
            var initialState = InitialState ?? throw new InvalidOperationException("Не задан источник начального состояния.");
            if (initialState.Source == InitialStateSource.Result)
            {
                ValidateResultInitialState(initialState);
                return null;
            }
            if (initialState.Result != null)
                throw new InvalidOperationException("Для начального состояния по значениям не должна быть задана ссылка на результат.");

            var conditions = (initialState.Conditions ?? new List<InitialCondition>()).Where(condition => condition.Field == field).ToList();
            if (conditions.Count > 1)
                throw new InvalidOperationException($"Для поля {field} указано несколько наборов начальных условий.");

            return conditions.SingleOrDefault();
        }

        /// <summary>Returns the result used as the initial state.</summary>
        public ResultReference GetInitialResult()
        {
            var initialState = InitialState ?? throw new InvalidOperationException("Не задан источник начального состояния.");
            if (initialState.Source == InitialStateSource.Values)
            {
                if (initialState.Result != null)
                    throw new InvalidOperationException("Для начального состояния по значениям не должна быть задана ссылка на результат.");

                return null;
            }

            ValidateResultInitialState(initialState);
            return initialState.Result;
        }

        private static void ValidateResultInitialState(InitialState initialState)
        {
            if (initialState.Conditions != null && initialState.Conditions.Count > 0)
                throw new InvalidOperationException("Для начального состояния из результата не должны быть заданы значения по группам.");
            if (initialState.Result == null)
                throw new InvalidOperationException("Не задан результат для начального состояния.");
            if (string.IsNullOrWhiteSpace(initialState.Result.FileName))
                throw new InvalidOperationException("Не задан файл результата для начального состояния.");
            if (initialState.Result.Step < -1)
                throw new InvalidOperationException("Номер шага начального результата должен быть не меньше -1.");
        }
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{TaskKind}_{TimeSettings.StartTime}_{TimeSettings.StopTime}";
        }
    }
}
