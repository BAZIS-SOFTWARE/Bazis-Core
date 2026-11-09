using MaterialDB.MaterialData.Settings;
using System;

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
        /// Создаёт газ: «Общие сведения» с моделями плотности и вязкости, «Опорным давлением»,
        /// молярной массой и коэффициентами Сазерленда (если заданы), тепловые и газодинамические свойства.
        /// </summary>
        /// <exception cref="ArgumentException">Модель идеального газа без молярной массы или модель Сазерленда без коэффициентов.</exception>
        public GasMaterialItem(string name, GasMaterialSettings settings) : base(name, settings)
        {
            if (settings.DensityModel == DensityModel.IdealGas && settings.MolarMass == null)
                throw new ArgumentException($"Газ {name}: модель идеального газа требует молярную массу.", nameof(settings));
            if (settings.ViscosityModel == ViscosityModel.Sutherland && settings.Sutherland == null)
                throw new ArgumentException($"Газ {name}: модель вязкости Сазерленда требует коэффициенты.", nameof(settings));

            AddDensityModel(settings.DensityModel);
            AddViscosityModel(settings.ViscosityModel);
            AddReferencePressure(settings.ReferencePressure);

            if (settings.MolarMass != null)
                AddMolarMass(settings.MolarMass.Value);
            if (settings.Sutherland != null)
                AddSutherlandCoefficients(settings.Sutherland);

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
        /// Проверяет тепловые свойства и вязкость газа: таблицу «Динамическая вязкость» или коэффициенты Сазерленда.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override bool CheckProps()
        {
            base.CheckProps();

            if (ViscosityModel == ViscosityModel.Sutherland)
            {
                if (!CategoryData.ContainsKey(CategoryDic[CategoryEnum.General]) || !this[CategoryEnum.General].PropertyData.ContainsKey("Коэффициенты Сазерленда"))
                    throw new Exception($"Материал {Name} не содержит свойство \"Коэффициенты Сазерленда\"!");
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
        /// Добавляет «Модель плотности».
        /// </summary>
        private void AddDensityModel(DensityModel model)
        {
            AddGeneralValue("Модель плотности", typeof(int), (int)model);
        }

        /// <summary>
        /// Добавляет «Модель вязкости».
        /// </summary>
        private void AddViscosityModel(ViscosityModel model)
        {
            AddGeneralValue("Модель вязкости", typeof(int), (int)model);
        }

        /// <summary>
        /// Добавляет «Молярную массу», кг/моль.
        /// </summary>
        private void AddMolarMass(double molarMass)
        {
            AddGeneralValue("Молярная масса", typeof(double), molarMass);
        }

        /// <summary>
        /// Добавляет «Опорное давление», МПа.
        /// </summary>
        private void AddReferencePressure(double pressure)
        {
            AddGeneralValue("Опорное давление", typeof(double), pressure);
        }

        /// <summary>
        /// Добавляет «Коэффициенты Сазерленда»: μ0 (МПа·с), T0 и S (К).
        /// </summary>
        private void AddSutherlandCoefficients(SutherlandCoefficients coefficients)
        {
            var table = CreateCoefficientTable(typeof(double));
            table.Rows.Add("μ0", coefficients.Mu0);
            table.Rows.Add("T0", coefficients.T0);
            table.Rows.Add("S", coefficients.S);

            AddGeneralTable("Коэффициенты Сазерленда", table);
        }
    }
}
