using PreProc.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace PreProc
{
    /// <summary>
    /// 
    /// </summary>
    public struct ProcessProperty
    {
        /// <summary>
        /// Общий тип задачи
        /// </summary>
        public TaskKind TaskKind;

        /// <summary>
        /// Тип подзадачи, в рамках generalTaskType
        /// </summary>
        public ProcessType CommonTaskType;
    }
}
