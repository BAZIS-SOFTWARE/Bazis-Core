using MaterialDB.MaterialData.Settings;
using System;
using System.Data;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Газ: тепловые и газодинамические свойства.
    /// </summary>
    [Serializable]
    public sealed class GasMaterialItem : MaterialDBItem
    {
        /// <summary>
        /// Создаёт газ с настройками по умолчанию: табличные плотность и вязкость.
        /// </summary>
        public GasMaterialItem(string name) : this(name, new GasMaterialSettings())
        {
        }

        /// <summary>
        /// Создаёт газ: «Общие сведения» с моделями плотности и вязкости и «Опорным давлением»,
        /// тепловые и газодинамические свойства. Параметры моделей записываются строками в сами модели:
        /// молярная масса «M» — в «Модель плотности» при IdealGas, μ0, T0, S — в «Модель вязкости» при Sutherland.
        /// </summary>
        /// <exception cref="ArgumentException">Модель идеального газа без молярной массы или модель Сазерленда без коэффициентов.</exception>
        public GasMaterialItem(string name, GasMaterialSettings settings) : base(name, settings)
        {
            if (settings.DensityModel == DensityModel.IdealGas && settings.MolarMass == null)
                throw new ArgumentException($"Газ {name}: модель идеального газа требует молярную массу.", nameof(settings));
            if (settings.ViscosityModel == ViscosityModel.Sutherland && settings.Sutherland == null)
                throw new ArgumentException($"Газ {name}: модель вязкости Сазерленда требует коэффициенты.", nameof(settings));

            SetDensityModel(settings.DensityModel == DensityModel.IdealGas ? settings.MolarMass : null);
            SetViscosityModel(settings.ViscosityModel == ViscosityModel.Sutherland ? settings.Sutherland : null);
            AddReferencePressure(settings.ReferencePressure);

            AddProps();
        }

        internal GasMaterialItem()
        {
        }

        /// <inheritdoc/>
        public override MediumKind Medium => MediumKind.Gas;

        /// <inheritdoc/>
        protected override MaterialDBItem CreateEmpty(string name)
        {
            return new GasMaterialItem() { Name = name };
        }

        /// <summary>
        /// Проверяет тепловые свойства, молярную массу при модели идеального газа
        /// и вязкость: таблицу «Динамическая вязкость» или коэффициенты Сазерленда.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override bool CheckProps()
        {
            base.CheckProps();

            if (DensityModel == DensityModel.IdealGas && !TryGetModelParameter("Модель плотности", "M", out _))
                throw new Exception($"Материал {Name}: в «Модели плотности» нет молярной массы \"M\"!");

            if (ViscosityModel == ViscosityModel.Sutherland)
            {
                foreach (var coefficient in new[] { "μ0", "T0", "S" })
                {
                    if (!TryGetModelParameter("Модель вязкости", coefficient, out _))
                        throw new Exception($"Материал {Name}: в «Модели вязкости» нет коэффициента Сазерленда \"{coefficient}\"!");
                }
                return true;
            }

            var gasName = CategoryDic[CategoryEnum.Gas];
            if (!CategoryData.ContainsKey(gasName))
                throw new Exception($"Материал {Name} не содержит категорию \"{gasName}\"!");
            if (!this[gasName].PropertyData.ContainsKey("Динамическая вязкость"))
                throw new Exception($"Материал {Name} не содержит свойство \"Динамическая вязкость\"!");

            return true;
        }

        /// <summary>
        /// Добавляет тепловые свойства, категорию «Газодинамические свойства» и таблицу вязкости при табличной модели.
        /// </summary>
        protected override void AddProps()
        {
            base.AddProps();

            if (ViscosityModel != ViscosityModel.Table)
            {
                AddCategory(CategoryEnum.Gas);
                return;
            }

            AddTableProperty(CategoryEnum.Gas, "Динамическая вязкость", ArgumentKind.TemperaturePressure);
        }

        /// <summary>
        /// Переключает газ на табличную плотность: в «Модели плотности» строка «Модель» = Table,
        /// строка молярной массы удаляется (сама запись остаётся). В «Тепловых свойствах» создаётся
        /// пустая таблица «Плотность», если её нет; существующая сохраняется с данными.
        /// </summary>
        public void UseTableDensity()
        {
            SetDensityModel(null);
            AddTableProperty(CategoryEnum.Thermal, "Плотность", ArgumentKind.Argument);
        }

        /// <summary>
        /// Переключает газ на модель идеального газа: в «Модели плотности» строка «Модель» = IdealGas
        /// и строка «M» — молярная масса, кг/моль (заменяется). Таблица «Плотность» не удаляется:
        /// при модели идеального газа калькулятор берёт параметры из строк «Модели плотности».
        /// </summary>
        public void UseIdealGasDensity(double molarMass)
        {
            SetDensityModel(molarMass);
        }

        /// <summary>
        /// Переключает газ на табличную вязкость: в «Модели вязкости» строка «Модель» = Table,
        /// строки коэффициентов Сазерленда удаляются (сама запись остаётся). В «Газодинамических свойствах»
        /// создаётся пустая таблица «Динамическая вязкость», если её нет; существующая сохраняется с данными.
        /// </summary>
        public void UseTableViscosity()
        {
            SetViscosityModel(null);
            AddTableProperty(CategoryEnum.Gas, "Динамическая вязкость", ArgumentKind.TemperaturePressure);
        }

        /// <summary>
        /// Переключает газ на формулу Сазерленда: в «Модели вязкости» строка «Модель» = Sutherland
        /// и строки μ0, T0, S (заменяются). Таблица «Динамическая вязкость» не удаляется:
        /// при модели Сазерленда калькулятор берёт параметры из строк «Модели вязкости».
        /// </summary>
        public void UseSutherlandViscosity(SutherlandCoefficients coefficients)
        {
            SetViscosityModel(coefficients);
        }

        /// <summary>
        /// Записывает модель в «Модель плотности», меняя таблицу на месте: строка «Модель» (0 — Table, 1 — IdealGas),
        /// строки ниже первой удаляются. Если передана молярная масса, модель — IdealGas, и добавляется строка «M», кг/моль.
        /// </summary>
        private void SetDensityModel(double? molarMass)
        {
            var table = GetModelTable("Модель плотности");

            if (molarMass == null)
            {
                table.Rows[0]["Значение"] = (double)DensityModel.Table;
                return;
            }

            table.Rows[0]["Значение"] = (double)DensityModel.IdealGas;
            table.Rows.Add("M", molarMass.Value);
        }

        /// <summary>
        /// Записывает модель в «Модель вязкости», меняя таблицу на месте: строка «Модель» (0 — Table, 1 — Sutherland),
        /// строки ниже первой удаляются. Если переданы коэффициенты, модель — Sutherland, и добавляются строки μ0 (МПа·с), T0 и S (К).
        /// </summary>
        private void SetViscosityModel(SutherlandCoefficients? sutherland)
        {
            var table = GetModelTable("Модель вязкости");

            if (sutherland == null)
            {
                table.Rows[0]["Значение"] = (double)ViscosityModel.Table;
                return;
            }

            table.Rows[0]["Значение"] = (double)ViscosityModel.Sutherland;
            table.Rows.Add("μ0", sutherland.Mu0);
            table.Rows.Add("T0", sutherland.T0);
            table.Rows.Add("S", sutherland.S);
        }

        /// <summary>
        /// Таблица модели в формате «Параметр»/«Значение», в которой осталась только первая строка «Модель».
        /// Если записи нет или она в старом формате (одна колонка), запись создаётся заново со значением 0 (Table).
        /// </summary>
        private DataTable GetModelTable(string recordName)
        {
            var general = this[CategoryEnum.General].PropertyData;
            if (general.ContainsKey(recordName))
            {
                var existing = general[recordName].DataTable;
                if (existing != null && existing.Columns.Contains("Параметр") && existing.Columns.Contains("Значение") && existing.Rows.Count > 0)
                {
                    while (existing.Rows.Count > 1)
                        existing.Rows.RemoveAt(1);
                    return existing;
                }

                RemoveProperty(CategoryEnum.General, recordName);
            }

            var table = new DataTable();
            table.Columns.Add("Параметр", typeof(string));
            table.Columns.Add("Значение", typeof(double));
            table.Rows.Add("Модель", 0d);

            AddGeneralTable(recordName, table);
            return table;
        }

        /// <summary>
        /// Добавляет «Опорное давление», МПа.
        /// </summary>
        private void AddReferencePressure(double pressure)
        {
            AddGeneralValue("Опорное давление", typeof(double), pressure);
        }
    }
}
