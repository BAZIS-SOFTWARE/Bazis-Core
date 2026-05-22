
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using static IronPython.Modules._ast;

namespace Project.TaskParameters
{
    [Serializable]
    public class GeneralParameters
    {
        public TaskKind TaskKind { get; set; }
        public List<string> TermalProcesses { get; set; } = new List<string>();
        public float InitTemp { get; set; } = 20.0f;
        public int SaveRate { get; set; }
        public string RestartFile { get; set; } = "";

        public int UpdMatrixIteration { get; set; } = 1;
        public int Iterations { get; set; }
        public SolverSettings SolverSettings { get; set; } = new SolverSettings();
        public TimeSettings TimeSettings { get; set; } = new TimeSettings();
        public List<string> MetallurgicalProcesses { get; set; } = new List<string>();
/// <inheritdoc/>

        public override string ToString()
        {
            return $"{TaskKind}_{TimeSettings.StartTime}_{TimeSettings.StopTime}";
        }
    }
}
