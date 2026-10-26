
using Project.Interfaces.Tasks;
using Project.TaskParameters;
using Project.Tasks;
using System.Collections.Generic;

namespace PreProc.Interfaces
{
    /// <summary>
    /// IPreProc
    /// </summary>
    public interface IPreProc : IEnumerable<GeneralParameters>
    {
        /// <summary>
        /// Формирует параметры заданных физических задач в порядке числовых индексов: тепло перед механикой.
        /// При включенной синхронизации связанные задачи получают общие шаги на каждом интервале.
        /// Файлы TSF и TCF не записывает.
        /// </summary>
        IReadOnlyList<GeneralParameters> CalcCompDataV2(IEnumerable<ICondData> data, TaskKind taskKinds, bool synchronizeTimeSteps = false);

        /// <summary>Заменяет текущую очередь готовыми параметрами.</summary>
        void Load(IEnumerable<GeneralParameters> parameters);

        /// <summary>Читает TCF проекта и связанные TSF, восстанавливая порядок и статусы очереди.</summary>
        void LoadInstructions(string projectPath);

        /// <summary>Добавляет инструкцию в конец очереди.</summary>
        void Add(GeneralParameters parameters);

        /// <summary>Удаляет инструкцию из очереди.</summary>
        bool Remove(GeneralParameters parameters);

        /// <summary>Удаляет все инструкции из текущей очереди.</summary>
        void Clear();

        /// <summary>Записывает текущие параметры в формате выбранного решателя.</summary>
        string WriteInstructions(string projectPath);

        /// <summary>
        /// Задает базовые численные параметры физической задачи.
        /// </summary>
        /// <typeparam name="ResultParam"></typeparam>
        /// <param name="parameter">Подготавливаемые параметры задачи.</param>
        void SetDraftParameters<ResultParam>(ResultParam parameter)
            where ResultParam : GeneralParameters, new();
    }
}
