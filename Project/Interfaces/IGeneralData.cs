using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Interfaces
{
    /// <summary>
    /// IGeneralData
    /// </summary>
    public interface IGeneralData
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// Path
        /// </summary>
        string Path { get; set; }
        /// <summary>
        /// Comments
        /// </summary>
        string Comments { get; set; }
    }
}
