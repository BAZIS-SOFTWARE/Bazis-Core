using GmshApi.Api;

namespace GmshApi
{
    /// <summary>
    /// Интерфейс IMeshField
    /// </summary>
    public interface IMeshField
    {
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        int Tag { get; }
        /// <summary>
        /// Тип поля
        /// </summary>
        MeshFieldType Type { get; }

        /// <summary>
        /// Устанавливает значение текущей опции
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        void SetOptionValue(string option, double value);
        /// <summary>
        /// Возвращает значение текущей опции
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        double GetOptionValue(string option);

        /// <summary>
        /// Установить значения опции
        /// </summary>
        /// <param name="option"></param>
        /// <param name="values"></param>
        void SetOptionValues(string option, double[] values);

        /// <summary>
        /// Возвратить значения опции
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        double[] GetOptionValues(string option);

        /// <summary>
        /// Возвратить строку опции
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        string GetOptionString(string option);

        /// <summary>
        /// Установить строку опции
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        void SetOptionString(string option, string value);

        void SetDisplayFieldType(DisplayFieldType type);
    }
}
