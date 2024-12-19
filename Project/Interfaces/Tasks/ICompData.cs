using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// TaskStatus
    /// </summary>
    public enum TaskStatus : int { выполнить, пропустить };
    /// <summary>
    /// ICompData
    /// </summary>
    public interface ICompData
    {
        /// <summary>
        /// TaskKind
        /// </summary>
        TaskKind TaskKind { get;}
        /// <summary>
        /// FileParameters
        /// </summary>
        string FileParameters { get; set; }
        /// <summary>
        /// Status
        /// </summary>
        TaskStatus Status { get; }
    }
}
