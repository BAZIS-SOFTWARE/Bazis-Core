using Project.Interfaces;
using Project.Interfaces.Tasks;

namespace Project
{
    /// <summary>
    /// GeneralData
    /// </summary>
    public class GeneralData : IGeneralData
    {
        /// <inheritdoc/>
        public string Name { get; set; }
        /// <inheritdoc/>
        public string Path { get; set; }
        /// <inheritdoc/>
        public string Comments { get; set; }
        /// <inheritdoc/>
        public string Materials { get; set; }
        /// <inheritdoc/>
        public string Functions { get; set; }
        /// <inheritdoc/>
        public TaskType TaskType { get ; set; }
/// <inheritdoc/>

        public TaskKind TaskKind { get; set; }
        /// <inheritdoc/>
        public string ResultDB { get; set; }
    }
}