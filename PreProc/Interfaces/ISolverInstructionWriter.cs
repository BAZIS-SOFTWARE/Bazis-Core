using System.Collections.Generic;
using Project.TaskParameters;

namespace PreProc.Interfaces
{
    /// <summary>Записывает параметры очереди в формате конкретного решателя.</summary>
    public interface ISolverInstructionWriter
    {
        /// <summary>Возвращает путь к основной инструкции решателя.</summary>
        string Write(string projectPath, IEnumerable<GeneralParameters> parameters);
    }
}
