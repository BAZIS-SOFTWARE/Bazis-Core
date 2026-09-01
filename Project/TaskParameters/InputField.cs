using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Project.TaskParameters
{
    /// <summary>
    /// Physical quantity imported from another task result.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PhysicalField
    {
        /// <summary>Temperature scalar field.</summary>
        Temperature,
        /// <summary>Velocity vector field.</summary>
        Velocity,
        /// <summary>Concentration field with one or more components.</summary>
        Concentration,
        /// <summary>phase composition field with one or more phases.</summary>
        PhaseComposition
    }

    /// <summary>
    /// Source used to obtain an input physical field.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InputFieldSource
    {
        /// <summary>The field has the same value throughout the calculation domain.</summary>
        Constant,
        /// <summary>The field is read from a result database.</summary>
        ResultFile
    }

    /// <summary>
    /// Describes an input physical field stored in a result database.
    /// </summary>
    [Serializable]
    public sealed class InputField
    {
        /// <summary>Physical quantity read by the task.</summary>
        public PhysicalField Field { get; set; }
        /// <summary>Source used to obtain the field values.</summary>
        public InputFieldSource Source { get; set; }
        /// <summary>Components of a field that is constant throughout the calculation domain.</summary>
        public double[] ConstantValues { get; set; } = Array.Empty<double>();
        /// <summary>Result database file name or search pattern.</summary>
        public string FileName { get; set; } = "";
        /// <summary>Zero-based index of the first available result.</summary>
        public int StartStep { get; set; }
        /// <summary>Number of available results. Zero selects all results after <see cref="StartStep"/>.</summary>
        public int StepsCount { get; set; }

        /// <summary>Indicates that a result database has not been configured.</summary>
        [JsonIgnore]
        public bool IsFileEmpty => string.IsNullOrWhiteSpace(FileName);
    }
}
