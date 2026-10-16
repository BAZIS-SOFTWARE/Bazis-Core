
using Project.Interfaces.Tasks;
using Project.TaskParameters;
using Project.Tasks;
using System.Collections.Generic;

namespace PreProc.Interfaces
{
    /// <summary>
    /// IPreProc
    /// </summary>
    public interface IPreProc
    {
        /// <summary>
        /// Формирует параметры заданных физических задач в порядке числовых индексов: тепло перед механикой.
        /// При включенной синхронизации связанные задачи получают общие шаги на каждом интервале.
        /// Файлы TSF и TCF не записывает.
        /// </summary>
        IReadOnlyList<GeneralParameters> CalcCompDataV2(IEnumerable<ICondData> data, TaskKind taskKinds, bool synchronizeTimeSteps = false);

        /// <summary>
        /// Записывает TCF по строкам текущей очереди «вид относительный-путь статус».
        /// Сохраняет порядок и статусы без изменения TSF.
        /// </summary>
        string WriteCommandFile(string projectPath, IEnumerable<string> queueRows);

        /// <summary>
        /// Задает базовые численные параметры физической задачи.
        /// </summary>
        /// <typeparam name="ResultParam"></typeparam>
        /// <param name="parameter">Подготавливаемые параметры задачи.</param>
        void SetDraftParameters<ResultParam>(ResultParam parameter)
            where ResultParam : GeneralParameters, new();
    }
}
