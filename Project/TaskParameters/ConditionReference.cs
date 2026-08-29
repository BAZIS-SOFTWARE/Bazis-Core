using Newtonsoft.Json;
using Project.Interfaces.Tasks;
using System;

namespace Project.TaskParameters
{
    /// <summary>
    /// Reference to a condition in TaskData.
    /// </summary>
    [JsonConverter(typeof(ConditionReferenceJsonConverter))]
    public sealed class ConditionReference
    {
        /// <summary>
        /// Creates a reference to a condition in TaskData.
        /// </summary>
        /// <param name="index">Condition index in TaskData.</param>
        /// <param name="kind">Condition kind.</param>
        /// <param name="groupName">Condition group name.</param>
        public ConditionReference(int index, DataKind kind, string groupName)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс условия не может быть отрицательным.");
            if (string.IsNullOrWhiteSpace(groupName))
                throw new ArgumentException("Имя группы условия не может быть пустым.", nameof(groupName));

            Index = index;
            Kind = kind;
            GroupName = groupName.Trim();
        }

        /// <summary>
        /// Condition index in TaskData.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Condition kind used to validate the reference.
        /// </summary>
        public DataKind Kind { get; }

        /// <summary>
        /// Group name used to validate the reference.
        /// </summary>
        public string GroupName { get; }

        /// <summary>
        /// Parses a compact condition reference.
        /// </summary>
        /// <param name="value">Reference in the format '&lt;index&gt; &lt;kind&gt; &lt;group&gt;'.</param>
        /// <returns>Parsed condition reference.</returns>
        public static ConditionReference Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new JsonSerializationException("Ссылка на условие не может быть пустой.");

            var parts = value.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length != 3)
                throw new JsonSerializationException($"Некорректная ссылка на условие '{value}'. Ожидается формат '<индекс> <тип> <группа>'.");
            if (!int.TryParse(parts[0], out var index) || index < 0)
                throw new JsonSerializationException($"Некорректный индекс условия '{parts[0]}'.");
            if (!Enum.TryParse(parts[1], false, out DataKind kind) || !Enum.IsDefined(typeof(DataKind), kind))
                throw new JsonSerializationException($"Некорректный тип условия '{parts[1]}'.");

            return new ConditionReference(index, kind, parts[2]);
        }

        public override string ToString() => $"{Index} {Kind} {GroupName}";
    }

    internal sealed class ConditionReferenceJsonConverter : JsonConverter<ConditionReference>
    {
        public override ConditionReference ReadJson(JsonReader reader, Type objectType, ConditionReference existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
                throw new JsonSerializationException("Ссылка на условие должна быть записана строкой.");

            return ConditionReference.Parse((string)reader.Value);
        }

        public override void WriteJson(JsonWriter writer, ConditionReference value, JsonSerializer serializer) => writer.WriteValue(value.ToString());
    }
}
