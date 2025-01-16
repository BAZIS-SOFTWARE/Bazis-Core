
using System;
using System.Collections.Generic;

namespace Project.TaskParameters
{
    [Serializable]
    public class GeneralParameters
    {
        public List<string> TermalProcesses { get; set; } = new List<string>();
        public float InitTemp { get; set; } = 20.0f;
        public int SaveRate { get; set; }
        public string RestartFile { get; set; } = "";
        public int Iterations { get; set; }
        public SolverSettings SolverSettings { get; set; } = new SolverSettings();
        public TimeSettings TimeSettings { get; set; } = new TimeSettings();
        public List<string> MetallurgicalProcesses { get; set; } = new List<string>();
    }
}
