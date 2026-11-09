using MaterialDB.Compatibility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Читает и пишет материал в прежнем формате файла (Name и CategoryData) без «$type».
    /// Класс материала выбирается по записи «Агрегатное состояние» в «Общих сведениях»;
    /// если записи нет — твёрдое тело.
    /// </summary>
    public sealed class MaterialDBItemConverter : JsonConverter
    {
        private const string MediumPath = "CategoryData['Общие сведения'].PropertyData['Агрегатное состояние'].DataTable[0]";

        /// <inheritdoc/>
        public override bool CanConvert(Type objectType)
        {
            return typeof(MaterialDBItem).IsAssignableFrom(objectType);
        }

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var material = (MaterialDBItem)value!;

            writer.WriteStartObject();
            writer.WritePropertyName(nameof(MaterialDBItem.Name));
            writer.WriteValue(material.Name);
            writer.WritePropertyName(nameof(MaterialDBItem.CategoryData));
            serializer.Serialize(writer, material.CategoryData);
            writer.WriteEndObject();
        }

        /// <inheritdoc/>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            var json = JObject.Load(reader);
            var material = CreateMaterial(ReadMedium(json));

            material.Name = (string?)json[nameof(MaterialDBItem.Name)] ?? "";
            material.CategoryData = json[nameof(MaterialDBItem.CategoryData)]?.ToObject<CategoryData>(serializer) ?? new CategoryData();

            new ArgumentKindResolver().Resolve(material);
            return material;
        }

        private MediumKind ReadMedium(JObject json)
        {
            var row = json.SelectToken(MediumPath) as JObject;
            var cell = row?.Properties().FirstOrDefault()?.Value;
            if (cell == null || cell.Type == JTokenType.Null)
                return MediumKind.Solid;

            var value = cell.Value<int>();
            if (!Enum.IsDefined(typeof(MediumKind), value))
                throw new JsonSerializationException($"Неизвестное значение записи «Агрегатное состояние»: {value}.");

            return (MediumKind)value;
        }

        private MaterialDBItem CreateMaterial(MediumKind medium)
        {
            switch (medium)
            {
                case MediumKind.Liquid:
                    return new LiquidMaterialItem();
                case MediumKind.Gas:
                    return new GasMaterialItem();
                default:
                    return new SolidMaterialItem();
            }
        }
    }
}
