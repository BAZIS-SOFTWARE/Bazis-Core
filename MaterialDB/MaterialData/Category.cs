using MaterialDB.Interfaces;
using Newtonsoft.Json;

namespace MaterialDB.MaterialData
{
    /// <summary>
    /// Category
    /// </summary>
    [Serializable]
    public class Category : ICategory
    {
        /// <summary>
        /// Словарь свойств
        /// </summary>
        [JsonIgnore]
        public Dictionary<PropertyEnum, string> PropertyDic = new Dictionary<PropertyEnum, string>()
        {
            { PropertyEnum.Capacity,"Теплоемкость"},
            { PropertyEnum.Density,"Плотность"},
            { PropertyEnum.HardeningFactor,"Коэффициент упрочнения"},
            { PropertyEnum.PoissonRatio,"Коэффициент Пуассона"},
            { PropertyEnum.Structure,"Структура"},
            { PropertyEnum.TensileStrength,"Предел прочности"},
            { PropertyEnum.ThermalConductivity,"Теплопроводность"},
            { PropertyEnum.ThermalExpansion,"ТКЛР"},
            { PropertyEnum.YieldStrength,"Предел текучести"},
            { PropertyEnum.YoungModule,"Модуль Юнга"},
            { PropertyEnum.HardeningModel,"Модель упрочнения"},
            { PropertyEnum.MaterialModel,"Модель материала"},
            { PropertyEnum.MediumKind,"Агрегатное состояние"},
            { PropertyEnum.DensityModel,"Модель плотности"},
            { PropertyEnum.MolarMass,"Молярная масса"},
            { PropertyEnum.ReferencePressure,"Опорное давление"},
            { PropertyEnum.ViscosityModel,"Модель вязкости"},
            { PropertyEnum.SutherlandCoefficients,"Коэффициенты Сазерленда"},
            { PropertyEnum.DynamicViscosity,"Динамическая вязкость"},
            { PropertyEnum.Compressibility,"Сжимаемость"},
            { PropertyEnum.VolumeExpansion,"Коэффициент объёмного расширения"},
            { PropertyEnum.ThermalConductivityX,"Теплопроводность X"},
            { PropertyEnum.ThermalConductivityY,"Теплопроводность Y"},
            { PropertyEnum.ThermalConductivityZ,"Теплопроводность Z"},
            { PropertyEnum.YoungModuleX,"Модуль Юнга X"},
            { PropertyEnum.YoungModuleY,"Модуль Юнга Y"},
            { PropertyEnum.YoungModuleZ,"Модуль Юнга Z"},
            { PropertyEnum.ThermalExpansionX,"ТКЛР X"},
            { PropertyEnum.ThermalExpansionY,"ТКЛР Y"},
            { PropertyEnum.ThermalExpansionZ,"ТКЛР Z"},
            { PropertyEnum.HardnessModel,"Модель твердости"}
        };
        /// <inheritdoc/>
        public string Name { get; set; }
/// <inheritdoc/>

        public PropertyData PropertyData { get; set; } = new PropertyData();

        public PropertyItem this[string key]
        { 
            get { return PropertyData[key]; }
            set { PropertyData[key] = value; }
        }
        public PropertyItem this[PropertyEnum key]
        {
            get { return PropertyData[PropertyDic[key]]; }
            set { PropertyData[PropertyDic[key]] = value; }
        }
    }
}
