using System;
using PreProc.Interfaces;

namespace PreProc.TaskParameters
{
    [Serializable]
    public class TimeSettings : ITimeSettings
    {
        public float MaxTimeStep { get; set; }
        public float MinTimeStep { get; set; }
        public float StartTime { get; set; }
        public float StopTime { get; set; }
        public float InitTimeStep { get; set; }
    }
}
