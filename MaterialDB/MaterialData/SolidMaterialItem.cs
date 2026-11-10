using MaterialDB.MaterialData.MetallurgicalData;
using MaterialDB.MaterialData.Settings;
using System;
using System.Data;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Твёрдое тело: тепловые, механические свойства и металлургия.
    /// </summary>
    [Serializable]
    public sealed class SolidMaterialItem : MaterialDBItem
    {
        /// <summary>
        /// Создаёт изотропное твёрдое тело с настройками по умолчанию: одна фаза «phase»,
        /// кристаллизация 1400–1500, «Модель упрочнения» 2, «Модель твердости» выключена.
        /// </summary>
        public SolidMaterialItem(string name) : this(name, new SolidMaterialSettings())
        {
        }

        /// <summary>
        /// Создаёт твёрдое тело. В «Общие сведения» записываются «Структура», «Кристаллизация»,
        /// «Модель упрочнения» и «Модель твердости»; затем создаются тепловые и механические свойства
        /// (по колонке на фазу) и пустая категория «Металлургия». Фазовые переходы задаются отдельно.
        /// </summary>
        /// <exception cref="ArgumentException">Нет фаз; TS не меньше TL; «Модель твердости» = 1 без индексов фаз.</exception>
        public SolidMaterialItem(string name, SolidMaterialSettings settings) : base(name, settings)
        {
            CheckSettings(name, settings);

            AddPhaseData(settings.Phases);
            AddCrystallization(settings.SolidusTemperature, settings.LiquidusTemperature);
            AddHardeningModel(settings.HardeningModel);
            AddHardnessModel(settings.HardnessModel, settings.HardnessIndices);

            AddProps();
            AddCategory(CategoryEnum.Metallurgical);
        }

        internal SolidMaterialItem()
        {
        }

        /// <inheritdoc/>
        public override MediumKind Medium => MediumKind.Solid;

        /// <inheritdoc/>
        protected override MaterialDBItem CreateEmpty(string name)
        {
            return new SolidMaterialItem() { Name = name };
        }

        /// <summary>
        /// Меняет модель материала и состав направленных свойств. При переходе к ортотропии
        /// копирует скалярные таблицы независимо для X/Y/Z; при возврате берёт таблицы X.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Неизвестная модель материала.</exception>
        /// <exception cref="InvalidOperationException">Отсутствует исходная таблица свойства.</exception>
        public void SetMaterialModel(MaterialModel model)
        {
            if (model != MaterialModel.Isotropic && model != MaterialModel.Orthotropic)
                throw new ArgumentOutOfRangeException(nameof(model), model, "Unknown material model.");

            var general = this[CategoryEnum.General].PropertyData;
            var hasModel = TryGetGeneralValue("Модель материала", out var value);
            if (general.ContainsKey("Модель материала") && !hasModel)
                throw new InvalidOperationException("The material model value is invalid.");

            var current = hasModel ? (MaterialModel)(int)value : MaterialModel.Isotropic;
            if ((hasModel && value != (int)current) || (current != MaterialModel.Isotropic && current != MaterialModel.Orthotropic))
                throw new InvalidOperationException("The current material model is unknown.");
            if (current == model)
                return;

            var conductivity = CreateModelProperties(CategoryEnum.Thermal, "Теплопроводность", model);
            var youngModule = CreateModelProperties(CategoryEnum.Mechanical, "Модуль Юнга", model);
            var thermalExpansion = CreateModelProperties(CategoryEnum.Mechanical, "ТКЛР", model);

            ReplaceModelProperties(CategoryEnum.Thermal, "Теплопроводность", conductivity);
            ReplaceModelProperties(CategoryEnum.Mechanical, "Модуль Юнга", youngModule);
            ReplaceModelProperties(CategoryEnum.Mechanical, "ТКЛР", thermalExpansion);

            if (general.ContainsKey("Модель материала"))
            {
                var table = general["Модель материала"].DataTable;
                var column = table.Columns.Contains("Значение") ? "Значение" : table.Columns[0].ColumnName;
                table.Rows[0][column] = (int)model;
            }
            else
                AddMaterialModel(model);
        }

        /// <summary>
        /// Создаёт независимые копии таблицы активного свойства для новой модели.
        /// </summary>
        private PropertyItem[] CreateModelProperties(CategoryEnum category, string propertyName, MaterialModel model)
        {
            var sourceName = model == MaterialModel.Orthotropic ? propertyName : propertyName + " X";
            var properties = this[category].PropertyData;
            if (!properties.ContainsKey(sourceName) || properties[sourceName].DataTable == null)
                throw new InvalidOperationException($"The source property '{sourceName}' is missing.");

            var source = properties[sourceName];
            if (model == MaterialModel.Isotropic)
                return new[] { source.Copy(propertyName) };

            return new[]
            {
                source.Copy(propertyName + " X"),
                source.Copy(propertyName + " Y"),
                source.Copy(propertyName + " Z")
            };
        }

        /// <summary>
        /// Заменяет скалярное и направленные свойства подготовленными таблицами новой модели.
        /// </summary>
        private void ReplaceModelProperties(CategoryEnum category, string propertyName, PropertyItem[] replacements)
        {
            var properties = this[category].PropertyData;
            properties.Remove(propertyName);
            properties.Remove(propertyName + " X");
            properties.Remove(propertyName + " Y");
            properties.Remove(propertyName + " Z");

            foreach (var replacement in replacements)
                properties.Add(replacement.Name, replacement);
        }

        /// <summary>
        /// Проверяет тепловые и механические свойства: пределы текучести и прочности,
        /// коэффициент упрочнения, модуль Юнга и ТКЛР (или X/Y/Z для ортотропии).
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override bool CheckProps()
        {
            base.CheckProps();

            if (!CategoryData.ContainsKey("Механические свойства"))
                throw new Exception($"Материал {Name} не содержит категорию \"Механические свойства\"!");

            if (!this["Механические свойства"].PropertyData.ContainsKey("Предел текучести"))
                throw new Exception($"Материал {Name} не содержит свойство \"Предел текучести\"!");
            if (!this["Механические свойства"].PropertyData.ContainsKey("Предел прочности"))
                throw new Exception($"Материал {Name} не содержит свойство \"Предел прочности\"!");
            if (!this["Механические свойства"].PropertyData.ContainsKey("Коэффициент упрочнения"))
                throw new Exception($"Материал {Name} не содержит свойство \"Коэффициент упрочнения\"!");

            foreach (var propertyName in GetDirectionalNames("Модуль Юнга"))
            {
                if (!this["Механические свойства"].PropertyData.ContainsKey(propertyName))
                    throw new Exception($"Материал {Name} не содержит свойство \"{propertyName}\"!");
            }

            foreach (var propertyName in GetDirectionalNames("ТКЛР"))
            {
                if (!this["Механические свойства"].PropertyData.ContainsKey(propertyName))
                    throw new Exception($"Материал {Name} не содержит свойство \"{propertyName}\"!");
            }

            return true;
        }

        /// <summary>
        /// CheckPhaseData
        /// </summary>
        /// <exception cref="Exception"></exception>
        public bool CheckPhaseData()
        {
            if (!CategoryData.ContainsKey("Общие сведения"))
                throw new Exception($"Материал {Name} не содержит категорию \"Общие сведения\"!");

            if (!this["Общие сведения"].PropertyData.ContainsKey("Структура"))
                throw new Exception($"Материал {Name} не содержит свойство \"Структура\"!");

            var phaseTable = this["Общие сведения"]["Структура"].DataTable;

            if (phaseTable == null)
                throw new Exception("Ошибка загрузки данных структуры материала! \nПроверьте раздел \"Структура\"");

            if (phaseTable.Rows.Count == 0)
                throw new Exception("Ошибка загрузки данных структуры материала! \nПроверьте колличество фаз!");
            return true;
        }

        /// <summary>
        /// Проверяет настройки, без которых твёрдое тело несогласовано.
        /// </summary>
        private void CheckSettings(string name, SolidMaterialSettings settings)
        {
            if (settings.Phases.Count == 0)
                throw new ArgumentException($"Материал {name}: «Структура» должна содержать хотя бы одну фазу.", nameof(settings));
            if (settings.SolidusTemperature >= settings.LiquidusTemperature)
                throw new ArgumentException($"Материал {name}: температура солидуса должна быть меньше температуры ликвидуса.", nameof(settings));
            if (settings.HardnessModel == 1 && settings.HardnessIndices == null)
                throw new ArgumentException($"Материал {name}: включённая модель твердости требует индексы фаз.", nameof(settings));
        }

        /// <summary>
        /// Добавляет фазу в конец «Структуры» и колонку этой фазы, заполненную нулями,
        /// в каждую таблицу свойств с раскладкой по фазам (TemperaturePhase). Сумму долей проверяет валидатор (V10).
        /// </summary>
        /// <exception cref="ArgumentException">Фаза с таким именем уже есть.</exception>
        /// <exception cref="InvalidOperationException">У материала нет «Структуры» (старый файл без металлургии).</exception>
        public void AddPhase(string phaseName, double massFraction)
        {
            if (!this[CategoryEnum.General].PropertyData.ContainsKey("Структура"))
                throw new InvalidOperationException($"Материал {Name} не содержит «Структуру».");
            if (GetPhases().Contains(phaseName))
                throw new ArgumentException($"Материал {Name} уже содержит фазу \"{phaseName}\".", nameof(phaseName));

            this[CategoryEnum.General]["Структура"].DataTable.Rows.Add(phaseName, massFraction);

            foreach (var table in GetPhaseTables())
            {
                table.Columns.Add(phaseName, typeof(double));
                foreach (DataRow row in table.Rows)
                    row[phaseName] = 0d;
            }
        }

        /// <summary>
        /// Удаляет фазу и всё, что с ней связано: строку «Структуры», её колонки в таблицах свойств,
        /// процессы «Металлургии», где она исходная или конечная («Нагрев F-A»).
        /// Индекс этой фазы в «Модели твердости» становится −1, индексы следующих фаз уменьшаются на 1.
        /// </summary>
        /// <exception cref="ArgumentException">Фазы нет в «Структуре».</exception>
        /// <exception cref="InvalidOperationException">Фаза последняя.</exception>
        public void RemovePhase(string phaseName)
        {
            var phases = GetPhases().ToList();
            var phaseIndex = phases.IndexOf(phaseName);

            if (phaseIndex < 0)
                throw new ArgumentException($"Материал {Name} не содержит фазу \"{phaseName}\".", nameof(phaseName));
            if (phases.Count == 1)
                throw new InvalidOperationException($"Материал {Name}: нельзя удалить последнюю фазу \"{phaseName}\".");

            this[CategoryEnum.General]["Структура"].DataTable.Rows.RemoveAt(phaseIndex);

            foreach (var table in GetPhaseTables())
            {
                if (table.Columns.Contains(phaseName))
                    table.Columns.Remove(phaseName);
            }

            RemovePhaseProcesses(phaseName);
            ShiftHardnessIndices(phaseIndex);
        }

        /// <summary>
        /// Таблицы свойств с раскладкой по фазам во всех категориях.
        /// </summary>
        private IEnumerable<DataTable> GetPhaseTables()
        {
            foreach (var category in CategoryData.Values)
            {
                foreach (var property in category.PropertyData.Values)
                {
                    if (property.ArgumentKind == ArgumentKind.TemperaturePhase && property.DataTable != null)
                        yield return property.DataTable;
                }
            }
        }

        /// <summary>
        /// Удаляет процессы «Металлургии» вида «Вид Исходная-Конечная», где фаза исходная или конечная.
        /// </summary>
        private void RemovePhaseProcesses(string phaseName)
        {
            if (!CategoryData.ContainsKey(CategoryDic[CategoryEnum.Metallurgical]))
                return;

            var removed = new List<string>();

            foreach (var processName in this[CategoryEnum.Metallurgical].PropertyData.Keys)
            {
                var spaceIndex = processName.IndexOf(' ');
                var transitionPhases = processName.Substring(spaceIndex + 1).Split('-');
                if (transitionPhases.Contains(phaseName))
                    removed.Add(processName);
            }

            foreach (var processName in removed)
                RemoveProperty(CategoryEnum.Metallurgical, processName);
        }

        /// <summary>
        /// Пересчитывает индексы фаз в «Модели твердости» после удаления строки «Структуры» с номером removedIndex.
        /// </summary>
        private void ShiftHardnessIndices(int removedIndex)
        {
            var general = this[CategoryEnum.General].PropertyData;
            if (!general.ContainsKey("Модель твердости"))
                return;

            var table = general["Модель твердости"].DataTable;
            if (table == null || table.Rows.Count == 0)
                return;

            var row = table.Rows[0];
            foreach (DataColumn column in table.Columns)
            {
                if (column.Ordinal == 0 || row[column] is DBNull)
                    continue;

                var index = Convert.ToInt32(row[column]);
                if (index == removedIndex)
                    row[column] = -1;
                else if (index > removedIndex)
                    row[column] = index - 1;
            }
        }

        /// <summary>
        /// Добавляет «Структуру»: фазы и их массовые доли.
        /// </summary>
        private void AddPhaseData(PhaseData phases)
        {
            if (!CategoryData.ContainsKey("Общие сведения"))
                throw new GeneralAbsentException();

            var table = new DataTable();
            table.Columns.Add("Фаза", typeof(string));
            table.Columns.Add("Масс.доли", typeof(double));

            foreach (var item in phases)
            {
                var row = table.NewRow();
                row[0] = item.Name;
                row[1] = item.Value;
                table.Rows.Add(row);
            }

            AddGeneralTable("Структура", table);
        }

        /// <summary>
        /// Добавляет «Кристаллизацию»: строки TS и TL.
        /// </summary>
        private void AddCrystallization(double ts, double tl)
        {
            if (!CategoryData.ContainsKey("Общие сведения"))
                throw new GeneralAbsentException();

            var table = new DataTable();
            table.Columns.Add("Точка", typeof(string));
            table.Columns.Add("Температура", typeof(double));

            var row = table.NewRow();
            row[0] = "TS";
            row[1] = ts;
            table.Rows.Add(row);

            row = table.NewRow();
            row[0] = "TL";
            row[1] = tl;
            table.Rows.Add(row);

            AddGeneralTable("Кристаллизация", table);
        }

        /// <summary>
        /// Добавляет «Модель упрочнения».
        /// </summary>
        private void AddHardeningModel(int mode)
        {
            AddGeneralValue("Модель упрочнения", typeof(int), mode);
        }

        /// <summary>
        /// Добавляет «Модель твердости»: 0 — выключена, 1 — включена. Индексы фаз модели Куркина
        /// хранятся в той же записи: колонки FerriteIndex … AusteniteIndex в единственной строке.
        /// </summary>
        private void AddHardnessModel(int mode, HardnessPhaseIndices? indices)
        {
            if (indices == null)
            {
                AddGeneralValue("Модель твердости", typeof(int), mode);
                return;
            }

            var table = new DataTable();
            table.Columns.Add("Модель твердости", typeof(int));
            table.Columns.Add(nameof(HardnessPhaseIndices.FerriteIndex), typeof(int));
            table.Columns.Add(nameof(HardnessPhaseIndices.PerliteIndex), typeof(int));
            table.Columns.Add(nameof(HardnessPhaseIndices.BeiniteIndex), typeof(int));
            table.Columns.Add(nameof(HardnessPhaseIndices.MartensiteIndex), typeof(int));
            table.Columns.Add(nameof(HardnessPhaseIndices.AusteniteIndex), typeof(int));
            table.Rows.Add(mode, indices.FerriteIndex, indices.PerliteIndex, indices.BeiniteIndex, indices.MartensiteIndex, indices.AusteniteIndex);

            AddGeneralTable("Модель твердости", table);
        }

        /// <summary>
        /// Добавляет тепловые и механические свойства. При металлургии таблицы строятся по фазам,
        /// поэтому конструктор записывает «Структуру» до вызова.
        /// </summary>
        protected override void AddProps()
        {
            base.AddProps();

            var kind = HasMetallurgy ? ArgumentKind.TemperaturePhase : ArgumentKind.Argument;

            AddTableProperty(CategoryEnum.Mechanical, "Предел текучести", kind);
            AddTableProperty(CategoryEnum.Mechanical, "Предел прочности", kind);
            AddTableProperty(CategoryEnum.Mechanical, "Коэффициент упрочнения", kind);

            foreach (var propertyName in GetDirectionalNames("Модуль Юнга"))
                AddTableProperty(CategoryEnum.Mechanical, propertyName, kind);

            foreach (var propertyName in GetDirectionalNames("ТКЛР"))
                AddTableProperty(CategoryEnum.Mechanical, propertyName, kind);
        }
    }
}
