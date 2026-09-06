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
    public enum PhysicalFieldSet
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
    public enum PhysicalQuantity
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
    public enum PhysicalFieldSource
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
    public sealed class PhysicalFieldValue
    {
        /// <summary>Физическая величина.</summary>
        public PhysicalQuantity Quantity { get; set; }

        /// <summary>Компоненты физической величины.</summary>
        public double[] Components { get; set; } = Array.Empty<double>();
    }

    /// <summary>
    /// Физический набор с единым источником.
    /// </summary>
    [Serializable]
    public sealed class PhysicalField
    {
        /// <summary>Набор физических полей.</summary>
        public PhysicalFieldSet FieldSet { get; set; }

        /// <summary>Источник значений набора.</summary>
        public PhysicalFieldSource Source { get; set; }

        /// <summary>Значения величин по именам элементных групп MatData.</summary>
        [JsonProperty(ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public Dictionary<string, List<PhysicalFieldValue>> Values { get; set; } = new Dictionary<string, List<PhysicalFieldValue>>();

        /// <summary>Имя файла базы результатов или шаблон поиска.</summary>
        public string FileName { get; set; } = "";

        /// <summary>Признак отсутствия файла результатов.</summary>
        [JsonIgnore]
        public bool IsFileEmpty => string.IsNullOrWhiteSpace(FileName);
    }
}
