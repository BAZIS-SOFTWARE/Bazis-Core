using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Project.TaskParameters
{
    /// <summary>
    /// Набор физических полей.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PhysicalSetName
    {
        /// <summary>Термический набор.</summary>
        Thermal,
        /// <summary>Механический набор.</summary>
        Mechanical,
        /// <summary>Химический набор.</summary>
        Chemical,
        /// <summary>Гидродинамический набор.</summary>
        Hydrodynamic
    }

    /// <summary>
    /// Физическая величина в наборе полей.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PhysicalFieldName
    {
        /// <summary>Температура.</summary>
        Temperature,
        /// <summary>Фазовый состав.</summary>
        PhaseComposition,
        /// <summary>Скорость.</summary>
        Velocity,
        /// <summary>Перемещение.</summary>
        Displacement,
        /// <summary>Давление.</summary>
        Pressure,
        /// <summary>Концентрация.</summary>
        Concentration,
        /// <summary>Напряжение.</summary>
        Stress,
        /// <summary>Деформация.</summary>
        Strain
    }

    /// <summary>
    /// Источник набора физических полей.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PhysicalSetSource
    {
        /// <summary>Значения заданы для элементных групп.</summary>
        Values,
        /// <summary>Значения читаются из базы результатов.</summary>
        ResultFile
    }

    /// <summary>
    /// Значение физической величины.
    /// </summary>
    [Serializable]
    public sealed class PhysicalField
    {
        /// <summary>Физическая величина.</summary>
        public PhysicalFieldName Name { get; set; }

        /// <summary>Компоненты физической величины.</summary>
        public double[] Components { get; set; } = Array.Empty<double>();
    }

    /// <summary>
    /// Физический набор с единым источником.
    /// </summary>
    [Serializable]
    public sealed class PhysicalSet
    {
        /// <summary>Набор физических полей.</summary>
        public PhysicalSetName FieldSet { get; set; }

        /// <summary>Источник значений набора.</summary>
        public PhysicalSetSource Source { get; set; }

        /// <summary>Значения величин по именам элементных групп MatData.</summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Dictionary<string, List<PhysicalField>> Values { get; set; } = new Dictionary<string, List<PhysicalField>>();

        /// <summary>Имя файла базы результатов или шаблон поиска.</summary>
        public string FileName { get; set; } = "";

        /// <summary>Признак отсутствия файла результатов.</summary>
        [JsonIgnore]
        public bool IsFileEmpty => string.IsNullOrWhiteSpace(FileName);
    }
}
