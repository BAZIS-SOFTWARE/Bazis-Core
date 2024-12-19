using System.Collections.Generic;

namespace Tasks.Interfaces
{
    public interface IGeneralParameters
    {
        /// <summary>
        /// TermalProcesses
        /// </summary>
        List<string> MetallurgicalProcesses { get; set; }
        /// <summary>
        /// InitTemp
        /// </summary>
        float InitTemp { get; set; }
        /// <summary>
        /// InitTemp
        /// </summary>
        int SaveRate { get; set; }
        /// <summary>
        /// RestartFile
        /// </summary>
        string RestartFile { get; set; }
        /// <summary>
        /// Iterations
        /// </summary>
        int Iterations { get; set; }
        /// <summary>
        /// SolverSettings
        /// </summary>
        ISolverSettings SolverSettings { get; set; }
        /// <summary>
        /// TimeSettings
        /// </summary>
        ITimeSettings TimeSettings { get; set; }
    }
}