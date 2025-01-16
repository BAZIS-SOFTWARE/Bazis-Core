using System;

namespace Project.TaskParameters
{
    [Serializable]
    public class TimeSettings
    {
        public float MaxTimeStep { get; set; }
        public float MinTimeStep { get; set; }
        public float StartTime { get; set; }
        public float StopTime { get; set; }
        public float InitTimeStep { get; set; }
    }
}
