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
    }
}