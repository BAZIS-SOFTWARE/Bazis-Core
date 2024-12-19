using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectInterfaces.Tasks
{
    /// <summary>
    /// IPowerParamsFunction
    /// </summary>
    public interface IPowerParamsFunction : IPowerFunction
    {
        /// <summary>
        /// GetParameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        float GetParameter(string parameter);
        /// <summary>
        /// SetParameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="val"></param>
        void SetParameter(string parameter, float val);
    }
}
