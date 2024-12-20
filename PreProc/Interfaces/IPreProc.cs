
using Project.Interfaces.Tasks;
using System.Collections.Generic;

namespace PreProc.Interfaces
{
    /// <summary>
    /// Тип процесса для препроцессора по созданию пользовательских задач
    /// </summary>
    public enum ProcessType
    {
        /// <summary>
        /// Сварка (нагрев, охлаждение)
        /// </summary>
        Welding,
        /// <summary>
        /// Общий нагрев (нагрев)
        /// </summary>
        Heating,
        /// <summary>
        /// Закалка (охлаждение)
        /// </summary>
        Quenching,
        /// <summary>
        /// Отпуск, отжиг, старение (выдержка)
        /// </summary>
        Tempering
    }
    /// <summary>
    /// IPreProc
    /// </summary>
    public interface IPreProc
    {
        /// <summary>
        /// Препроцессор по обработке данных пользователя в команды в формате .tsf-файлов и CompData.
        /// Актуален для процессов указанных в  "ProcessType".
        /// Для сварки:
        /// - Реализована поддержка многопроходной сварки с последовательно идущими проходами и с интервалами между ними
        /// - Среда сварки должна быть задана
        /// - Нагрев от источника сварки должен быть задан
        /// - Закрепления должны быть заданы
        /// - Среда и нагрев не должны выходить за время действия материала
        /// - Создается всегда четное кол-во tsf файлов-инструкций. На каждый временной интервал по паре тепловой и механической задачи соответственно.
        /// Для ТО:
        /// - Требования теже, что и в сварке кроме требований нагрева
        /// - Общая тепловая и механическая задачи также поддерживаются
        /// </summary>
        /// <param name="data">список входных данных ValuableData</param>
        /// <param name="processType">тип задачи</param>>
        /// <param name="path">путь папки сохранения расчетных файлов данных</param>>
        /// <returns>список расчитанных данных CompData</returns>
        void CalcCompDataV1(List<IValuableData> data, ProcessType processType, string path);
        /// <summary>
        /// SortCompDataByTimeAndType
        /// </summary>
        /// <param name="dataStrs"></param>
        /// <returns>
        /// Отсортированный по названиям и по времени действия список задач
        /// </returns>
        List<string> SortCompDataByTimeAndType(string[] dataStrs);
        /// <summary>
        /// SetDraftParameters
        /// </summary>
        /// <typeparam name="ResultParam"></typeparam>
        /// <param name="parameter"></param>
        /// <param name="processType">тип задачи</param>>
        void SetDraftParameters<ResultParam>(ResultParam parameter, ProcessType processType)
            where ResultParam : GeneralParameters, new();
    }
}
