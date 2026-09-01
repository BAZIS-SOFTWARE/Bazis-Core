using System;
using System.Collections.Generic;

namespace Project.TaskParameters
{
    /// <summary>
    /// Initial values of a physical field grouped by model group name.
    /// </summary>
    [Serializable]
    public sealed class InitialCondition
    {
        /// <summary>Physical field initialized by this condition.</summary>
        public PhysicalField Field { get; set; }

        /// <summary>Field components indexed by names of groups from GroupData.</summary>
        public Dictionary<string, double[]> Values { get; set; } = new Dictionary<string, double[]>();
    }
}
