
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using static IronPython.Modules._ast;
using System.Linq;
using System.Globalization;

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

    /// <summary>Участие инструкции в очереди расчётов.</summary>
    public enum TaskStatus
    {
        выполнить,
        пропустить
    }

    [Serializable]
    public class GeneralParameters
    {
        public TaskKind TaskKind { get; set; }
        /// <summary>Статус инструкции в текущей очереди.</summary>
        public TaskStatus Status { get; set; } = TaskStatus.выполнить;
        /// <summary>
        /// Индекс расчётного интервала, присвоенный препроцессором начиная с нуля.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Calculation algorithm with respect to physical time.
        /// </summary>
        public CalculationMode CalculationMode { get; set; } = CalculationMode.Transient;

        /// <summary>
        /// Conditions selected by PreProc for this calculation.
        /// </summary>
        public List<ConditionReference> ActiveConditions { get; set; }
        /// <summary>
        /// Внешние физические поля.
        /// </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public List<PhysicalSet> InputSets { get; set; } = new List<PhysicalSet>();
        /// <summary>
        /// Родной набор физических полей, задающий подтвержденное начальное состояние.
        /// </summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public PhysicalSet? InitialSet { get; set; }
        public int SaveRate { get; set; }

        public int UpdMatrixIteration { get; set; } = 1;
        public int Iterations { get; set; }
        [JsonProperty(Required = Required.Always)]
        public SolverSettings SolverSettings { get; set; } = null!;
        public TimeSettings TimeSettings { get; set; } = new TimeSettings();

        /// <summary>
        /// Возвращает внешний набор физических полей.
        /// </summary>
        public PhysicalSet GetInputField(PhysicalSetName fieldSet)
        {
            var fields = (InputSets ?? new List<PhysicalSet>()).Where(field => field.Name == fieldSet).ToList();
            if (fields.Count > 1)
                throw new InvalidOperationException($"Several external sources are configured for field set {fieldSet}.");

            return fields.SingleOrDefault();
        }
        /// <summary>
        /// Возвращает переносимую основу имени файлов TSF и DB без статуса очереди.
        /// </summary>
        [JsonIgnore]
        public string FileStem
        {
            get
            {
                var start = TimeSettings.StartTime.ToString("G9", CultureInfo.InvariantCulture);
                var stop = TimeSettings.StopTime.ToString("G9", CultureInfo.InvariantCulture);
                return $"{TaskKind}_{Index}_{start}_{stop}";
            }
        }

        /// <summary>Возвращает текст инструкции для отображения в очереди.</summary>
        public override string ToString() => $"{FileStem}_{Status}";
    }
}
