using Geometry;
using Project.Interfaces.Tasks;
using System.Collections.Generic;

namespace Project.Tasks.Functions
{
    /// <summary>
    /// IHeatPowerFunction
    /// </summary>
    public abstract class PowerFunction
    {
        /// <summary>
        /// CalcPower
        /// </summary>
        /// <param name="elementCenter"></param>
        /// <returns></returns>
        public abstract float CalcPower(Point3D elementCenter);

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public abstract void CreateParameter(string name, float value);
        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <param name="name"></param>
        public abstract void CreateParameter(string name);
        /// <summary>
        /// GetParameters
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<FunctionParameter> GetParameters();
    }
}
