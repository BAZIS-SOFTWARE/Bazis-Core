using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Interfaces.Tasks
{
    public interface ITimeDependentData
    {
        /// <summary>
        /// StartTime
        /// </summary>
        float StartTime { get; set; }
        /// <summary>
        /// StopTime
        /// </summary>
        float StopTime { get; set; }
    }
}
