using MaterialDB.MaterialData.MetallurgicalData;
using MaterialDB.MaterialData.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Материал: общие данные, тепловые свойства и контейнер категорий.
    /// Состав остальных свойств определяет среда: твёрдое тело, жидкость или газ.
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(MaterialDBItemConverter))]
    public abstract class MaterialDBItem
    {
        /// <summary>
        /// Создаёт «Общие сведения» и записывает «Модель материала».
        /// Таблицы свойств добавляет конструктор наследника через AddProps(), когда записаны настройки его среды.
        /// </summary>
        protected MaterialDBItem(string name, MaterialSettings settings)
        {
            Name = name;
            AddGeneralProps();
            AddMaterialModel(settings.MaterialModel);
        }

        /// <summary>
        /// Пустой материал для загрузки из файла и копирования.
        /// </summary>
        internal MaterialDBItem()
        {
        }

        /// <summary>
        /// Словарь категорий
        /// </summary>
        [JsonIgnore]
        public Dictionary<CategoryEnum, string> CategoryDic = new Dictionary<CategoryEnum, string>()
        {
            { CategoryEnum.General,"Общие сведения"},
            { CategoryEnum.Thermal,"Тепловые свойства"},
            { CategoryEnum.Mechanical,"Механические свойства"},
            { CategoryEnum.Metallurgical,"Металлургия"},
            { CategoryEnum.Fluid,"Гидродинамические свойства"},
            { CategoryEnum.Gas,"Газодинамические свойства"}
        };
        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public string Name { get; set; } = "";

        public CategoryData CategoryData { get; set; } = new CategoryData();

        public Category this[string key]
        {
            get { return CategoryData[key]; }
            set { CategoryData[key] = value; }
        }

        public Category this[CategoryEnum key]
        {
            get { return CategoryData[CategoryDic[key]]; }
            set { CategoryData[CategoryDic[key]] = value; }
        }

        /// <summary>
        /// Среда материала. Её определяет тип класса.
        /// </summary>
        [JsonIgnore]
        public abstract MediumKind Medium { get; }

        /// <summary>
        /// Есть ли «Структура» хотя бы с одной фазой.
        /// </summary>
        [JsonIgnore]
        public bool HasMetallurgy => GetPhases().Any();

        /// <summary>
        /// Категория вязкости, сжимаемости и расширения среды: Gas для газа, Fluid для остальных.
        /// </summary>
        internal CategoryEnum FluidCategory => Medium == MediumKind.Gas ? CategoryEnum.Gas : CategoryEnum.Fluid;

        /// <summary>
        /// «Модель плотности»; если записи нет — Table.
        /// </summary>
        internal DensityModel DensityModel => TryGetGeneralValue("Модель плотности", out var value) ? (DensityModel)(int)value : DensityModel.Table;

        /// <summary>
        /// «Модель вязкости»; если записи нет — Table.
        /// </summary>
        internal ViscosityModel ViscosityModel => TryGetGeneralValue("Модель вязкости", out var value) ? (ViscosityModel)(int)value : ViscosityModel.Table;

        /// <summary>
        /// Создаёт пустой материал той же среды с заданным именем: без категорий и свойств.
        /// </summary>
        protected abstract MaterialDBItem CreateEmpty(string name);

        /// <summary>
        /// Copy
        /// </summary>
        /// <param name="copyName"></param>
        /// <returns></returns>
        public MaterialDBItem Copy(string copyName)
        {
            var newMaterial = CreateEmpty(copyName);

            foreach (var category in CategoryData)
            {
                newMaterial.CategoryData.Add(category.Key, new Category() { Name = category.Key });
                foreach (var property in category.Value.PropertyData)
                {
                    var prop = property.Value.Copy(property.Key);
                    newMaterial[category.Key].PropertyData.Add(property.Key, prop);
                }
            }
            return newMaterial;
        }

        /// <summary>
        /// Значение однострочной записи «Общих сведений».
        /// </summary>
        internal bool TryGetGeneralValue(string propertyName, out double value)
        {
            value = 0;
            if (!CategoryData.ContainsKey(CategoryDic[CategoryEnum.General]))
                return false;

            var properties = this[CategoryEnum.General].PropertyData;
            if (!properties.ContainsKey(propertyName))
                return false;

            var table = properties[propertyName].DataTable;
            if (table == null || table.Rows.Count == 0 || table.Columns.Count == 0)
                return false;

            var cell = table.Rows[0][0];
            if (cell is DBNull || cell is string)
                return false;

            value = Convert.ToDouble(cell);
            return true;
        }

        /// <summary>
        /// Проверяет свойства материала. Базовый класс проверяет тепловые свойства:
        /// теплопроводность (или X/Y/Z для ортотропии), теплоемкость и плотность при табличной модели.
        /// Наследники дополняют проверку свойствами своей среды.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public virtual bool CheckProps()
        {
            if (!CategoryData.ContainsKey("Тепловые свойства"))
                throw new Exception($"Материал {Name} не содержит категорию \"Тепловые свойства\"!");

            foreach (var propertyName in GetDirectionalNames("Теплопроводность"))
            {
                if (!this["Тепловые свойства"].PropertyData.ContainsKey(propertyName))
                    throw new Exception($"Материал {Name} не содержит свойство \"{propertyName}\"!");
            }

            if (!this["Тепловые свойства"].PropertyData.ContainsKey("Теплоемкость"))
                throw new Exception($"Материал {Name} не содержит свойство \"Теплоемкость\"!");
            if (DensityModel == DensityModel.Table && !this["Тепловые свойства"].PropertyData.ContainsKey("Плотность"))
                throw new Exception($"Материал {Name} не содержит свойство \"Плотность\"!");

            return true;
        }

        /// <summary>
        /// Имена фаз «Структуры»; пустая последовательность для материала без металлургии.
        /// </summary>
        /// <returns></returns>

        public IEnumerable<string> GetPhases()
        {
            if (!CategoryData.ContainsKey(CategoryDic[CategoryEnum.General]) || !this[CategoryEnum.General].PropertyData.ContainsKey("Структура"))
                yield break;

            var phases = this["Общие сведения"]["Структура"].DataTable;
            if (phases == null)
                yield break;

            foreach (DataRow phaseRow in phases.Rows)
            {
                yield return phaseRow[0].ToString();
            }
        }

        /// <summary>
        /// Добавляет «Общие сведения». У жидкости и газа записывает «Агрегатное состояние» —
        /// по нему загрузчик выбирает класс материала; у твёрдого тела записи нет.
        /// </summary>
        protected void AddGeneralProps()
        {
            if (CategoryData.ContainsKey("Общие сведения"))
                return;

            CategoryData.Add("Общие сведения", new Category() { Name = "Общие сведения" });

            if (Medium != MediumKind.Solid)
                AddGeneralValue("Агрегатное состояние", typeof(int), (int)Medium);
        }

        /// <summary>
        /// Добавляет «Модель материала».
        /// </summary>
        protected void AddMaterialModel(MaterialModel model)
        {
            AddGeneralValue("Модель материала", typeof(int), (int)model);
        }

        /// <summary>
        /// Добавляет свойства материала. Базовый класс добавляет тепловые свойства.
        /// Наследники дополняют набор свойствами своей среды.
        /// Вызывается из конструктора наследника после записи настроек в «Общие сведения»:
        /// от моделей материала, плотности, вязкости и «Структуры» зависит состав таблиц.
        /// </summary>
        protected virtual void AddProps()
        {
            var kind = HasMetallurgy ? ArgumentKind.TemperaturePhase : ArgumentKind.Argument;

            foreach (var propertyName in GetDirectionalNames("Теплопроводность"))
                AddTableProperty(CategoryEnum.Thermal, propertyName, kind);

            AddTableProperty(CategoryEnum.Thermal, "Теплоемкость", kind);

            if (DensityModel == DensityModel.Table)
                AddTableProperty(CategoryEnum.Thermal, "Плотность", kind);
        }

        /// <summary>
        /// Имя скалярного свойства или три имени X/Y/Z для ортотропного материала («Модель материала» = 2).
        /// </summary>
        protected IEnumerable<string> GetDirectionalNames(string propertyName)
        {
            if (TryGetGeneralValue("Модель материала", out var model) && (int)model == (int)MaterialModel.Orthotropic)
            {
                yield return propertyName + " X";
                yield return propertyName + " Y";
                yield return propertyName + " Z";
                yield break;
            }

            yield return propertyName;
        }

        protected void AddCategory(CategoryEnum category)
        {
            var categoryName = CategoryDic[category];
            if (!CategoryData.ContainsKey(categoryName))
                CategoryData.Add(categoryName, new Category() { Name = categoryName });
        }

        protected void AddTableProperty(CategoryEnum category, string propertyName, ArgumentKind kind)
        {
            AddCategory(category);

            if (this[category].PropertyData.ContainsKey(propertyName))
                return;

            var table = CreateTable(kind);
            this[category].PropertyData.Add(propertyName, new PropertyItem(propertyName, "", "", "", table, kind));
        }

        protected void AddGeneralValue(string propertyName, Type valueType, object value)
        {
            var table = new DataTable();
            table.Columns.Add(propertyName, valueType);
            table.Rows.Add(value);

            AddGeneralTable(propertyName, table, ArgumentKind.Constant);
        }

        protected void AddGeneralTable(string propertyName, DataTable table, ArgumentKind kind = ArgumentKind.Descriptor)
        {
            if (!CategoryData.ContainsKey("Общие сведения"))
                throw new GeneralAbsentException();

            this[CategoryEnum.General].PropertyData.Add(propertyName,
new PropertyItem(propertyName, "", "", "", table, kind));
        }

        protected DataTable CreateCoefficientTable(Type valueType)
        {
            var table = new DataTable();
            table.Columns.Add("Коэффициент", typeof(string));
            table.Columns.Add("Значение", valueType);
            return table;
        }

        private DataTable CreateTable(ArgumentKind kind)
        {
            var table = new DataTable();
            table.Columns.Add("Температура", typeof(double));

            if (kind == ArgumentKind.Argument)
                table.Columns.Add("Значение", typeof(double));

            if (kind == ArgumentKind.TemperaturePhase)
            {
                foreach (var item in GetPhases())
                    table.Columns.Add(item, typeof(double));
            }

            return table;
        }
    }
}
