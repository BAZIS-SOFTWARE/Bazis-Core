using System;

namespace PreProc
{
    [Serializable]
    public class TermalConvergence
    {
        public bool Is_Switched_Tm { get; set; } = true;
        public float Tm { get; set; } = 1500.0f;
    }
}
