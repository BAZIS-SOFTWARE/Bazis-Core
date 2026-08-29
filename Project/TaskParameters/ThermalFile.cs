using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Project.TaskParameters
{
    /// <summary>
    /// Selects a range of thermal results used by a dependent task.
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(ThermalFileJsonConverter))]
    public sealed class ThermalFile
    {
        /// <summary>
        /// File name or search pattern of the thermal-results database.
        /// </summary>
        public string FileName { get; set; } = "";

        /// <summary>
        /// Zero-based index of the first result to use.
        /// </summary>
        public int StartStep { get; set; }

        /// <summary>
        /// Number of results to use. Zero means all results starting from <see cref="StartStep"/>.
        /// </summary>
        public int StepsCount { get; set; }

        /// <summary>
        /// Indicates that a thermal-results file has not been configured.
        /// </summary>
        [JsonIgnore]
        public bool IsEmpty => string.IsNullOrWhiteSpace(FileName);
    }

    internal sealed class ThermalFileJsonConverter : JsonConverter<ThermalFile>
    {
        public override ThermalFile ReadJson(JsonReader reader, Type objectType, ThermalFile existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return new ThermalFile();

            if (reader.TokenType == JsonToken.String)
                return new ThermalFile { FileName = Convert.ToString(reader.Value) ?? "" };

            if (reader.TokenType != JsonToken.StartObject)
                throw new JsonSerializationException("Параметр ThermalFile должен быть строкой или объектом.");

            var result = existingValue ?? new ThermalFile();
            var data = JObject.Load(reader);
            using var objectReader = data.CreateReader();
            serializer.Populate(objectReader, result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, ThermalFile value, JsonSerializer serializer)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(ThermalFile.FileName));
            writer.WriteValue(value?.FileName ?? "");
            writer.WritePropertyName(nameof(ThermalFile.StartStep));
            writer.WriteValue(value?.StartStep ?? 0);
            writer.WritePropertyName(nameof(ThermalFile.StepsCount));
            writer.WriteValue(value?.StepsCount ?? 0);
            writer.WriteEndObject();
        }
    }
}
