using System;
using System.Collections.Generic;
using Tasks.Interfaces;

namespace Tasks
{
    [Serializable]
    public class GeneralParameters : IGeneralParameters
    {
        public List<string> MetallurgicalProcesses { get; set; } = new List<string>();
        public float InitTemp { get; set; } = 20.0f;
        public int SaveRate { get; set; }
        public string RestartFile { get; set; } = "";
        public int Iterations { get; set; }
        public ISolverSettings SolverSettings { get; set; } = new SolverSettings();
        public ITimeSettings TimeSettings { get; set; } = new TimeSettings();


    }
}
