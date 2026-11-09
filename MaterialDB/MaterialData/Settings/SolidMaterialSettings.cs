using MaterialDB.MaterialData.MetallurgicalData;

namespace MaterialDB.MaterialData.Settings
{
    /// <summary>
    /// Настройки твёрдого тела.
    /// </summary>
    public class SolidMaterialSettings : MaterialSettings
    {
        /// <summary>
        /// Начальный состав «Структуры». По умолчанию одна фаза «phase» с массовой долей 1.
        /// Таблицы тепловых и механических свойств строятся с колонкой на каждую фазу.
        /// После создания состав меняют SolidMaterialItem.AddPhase и RemovePhase.
        /// </summary>
        public PhaseData Phases { get; set; } = new PhaseData(new List<Phase> { new Phase { Name = "phase", Value = 1 } });

        /// <summary>
        /// Температура солидуса TS, °C. По умолчанию 1400.
        /// </summary>
        public double SolidusTemperature { get; set; } = 1400;

        /// <summary>
        /// Температура ликвидуса TL, °C. По умолчанию 1500.
        /// </summary>
        public double LiquidusTemperature { get; set; } = 1500;

        /// <summary>
        /// «Модель упрочнения». По умолчанию 2.
        /// </summary>
        public int HardeningModel { get; set; } = 2;

        /// <summary>
        /// «Модель твердости»: 0 — выключена (по умолчанию), 1 — включена.
        /// </summary>
        public int HardnessModel { get; set; }

        /// <summary>
        /// Индексы фаз модели Куркина. Записываются в «Модель твердости», если заданы.
        /// </summary>
        public HardnessPhaseIndices? HardnessIndices { get; set; }
    }
}
