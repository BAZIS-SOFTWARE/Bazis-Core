using MaterialDB.MaterialData.Settings;
using System;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Жидкость: тепловые и гидродинамические свойства.
    /// </summary>
    [Serializable]
    public sealed class LiquidMaterialItem : MaterialDBItem
    {
        /// <summary>
        /// Создаёт жидкость с настройками по умолчанию.
        /// </summary>
        public LiquidMaterialItem(string name) : this(name, new FluidMaterialSettings())
        {
        }

        /// <summary>
        /// Создаёт жидкость: «Общие сведения», «Опорное давление», тепловые и гидродинамические свойства.
        /// </summary>
        public LiquidMaterialItem(string name, FluidMaterialSettings settings) : base(name, settings)
        {
            AddReferencePressure(settings.ReferencePressure);
            AddProps();
        }

        internal LiquidMaterialItem()
        {
        }

        /// <inheritdoc/>
        public override MediumKind Medium => MediumKind.Liquid;

        /// <inheritdoc/>
        protected override MaterialDBItem CreateEmpty(string name)
        {
            return new LiquidMaterialItem() { Name = name };
        }

        /// <summary>
        /// Проверяет тепловые свойства и таблицу «Динамическая вязкость» в категории «Гидродинамические свойства».
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override bool CheckProps()
        {
            base.CheckProps();

            var fluidName = CategoryDic[CategoryEnum.Fluid];
            if (!CategoryData.ContainsKey(fluidName))
                throw new Exception($"Материал {Name} не содержит категорию \"{fluidName}\"!");
            if (!this[fluidName].PropertyData.ContainsKey("Динамическая вязкость"))
                throw new Exception($"Материал {Name} не содержит свойство \"Динамическая вязкость\"!");

            return true;
        }

        /// <summary>
        /// Добавляет тепловые свойства, категорию «Гидродинамические свойства» и таблицу вязкости.
        /// </summary>
        protected override void AddProps()
        {
            base.AddProps();
            AddTableProperty(CategoryEnum.Fluid, "Динамическая вязкость", ArgumentKind.Argument);
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
