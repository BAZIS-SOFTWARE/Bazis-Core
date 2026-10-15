
using Project.Interfaces.Tasks;
using Project.TaskParameters;
using Project.Tasks;
using System.Collections.Generic;

namespace PreProc.Interfaces
{
    /// <summary>
    /// Тип подпроцесса для препроцессора по созданию пользовательских термических задач
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
        /// Формирует параметры задач в порядке числовых индексов: тепло перед механикой.
        /// Файлы TSF и TCF не записывает.
        /// </summary>
        IReadOnlyList<GeneralParameters> CalcCompDataV2(IEnumerable<ICondData> data, ProcessProperty property);

        /// <summary>
        /// Записывает TCF по строкам текущей очереди «вид относительный-путь статус».
        /// Сохраняет порядок и статусы без изменения TSF.
        /// </summary>
        string WriteCommandFile(string projectPath, IEnumerable<string> queueRows);

        /// <summary>
        /// SortCompDataByTimeAndType
        /// </summary>
        /// <param name="dataStrs"></param>
        /// <returns>
        /// Список задач по числовому индексу; при равенстве тепло перед механикой.
        /// </returns>
        List<string> SortCompDataByTimeAndType(string[] dataStrs);

        /// <summary>
        /// SetDraftParameters
        /// </summary>
        /// <typeparam name="ResultParam"></typeparam>
        /// <param name="parameter"></param>>
        void SetDraftParameters<ResultParam>(ResultParam parameter)
            where ResultParam : GeneralParameters, new();
    }
}
