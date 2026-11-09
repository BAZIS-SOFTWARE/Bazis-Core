namespace MaterialDB
{
    /// <summary>
    /// Вид зависимости, которую описывает таблица DataItem.
    /// </summary>
    public enum ArgumentKind
    {
        /// <summary>
        /// Поле отсутствует в файле; после загрузки заменяется резолвером и не сохраняется.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// Одна строка: постоянное значение.
        /// </summary>
        Constant = 1,
        /// <summary>
        /// Одна переменная X: у свойства материала — температура, у функции — любая.
        /// </summary>
        Argument = 2,
        /// <summary>
        /// «Температура» и колонка на каждую фазу «Структуры».
        /// </summary>
        TemperaturePhase = 3,
        /// <summary>
        /// «Температура» и колонки давлений «P_&lt;МПа&gt;».
        /// </summary>
        TemperaturePressure = 4,
        /// <summary>
        /// Описательная таблица, не интерполируется.
        /// </summary>
        Descriptor = 5,
        /// <summary>
        /// Процесс металлургии.
        /// </summary>
        Process = 6
    }
}
