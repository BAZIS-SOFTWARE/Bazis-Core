using PreProc.Interfaces;
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
        public ComplexTaskType generalTaskType;

        /// <summary>
        /// Тип подзадачи, в рамках generalTaskType
        /// </summary>
        public ProcessType commonTaskType;
    }
}
