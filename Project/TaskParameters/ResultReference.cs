using System;

namespace Project.TaskParameters
{
    /// <summary>
    /// Reference to a saved result used as the initial state.
    /// </summary>
    [Serializable]
    public sealed class ResultReference
    {
        /// <summary>Name or search pattern of the result file.</summary>
        public string FileName { get; set; } = "";

        /// <summary>Zero-based result step index; -1 selects the last saved step.</summary>
        public int Step { get; set; } = -1;
    }
}
