using Geometry;
using System.Collections.Generic;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// IHeatPowerFunction
    /// </summary>
    public interface IPowerFunction : IFrameFunction
    {
        /// <summary>
        /// CalcPower
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="elementCenter"></param>
        /// <returns></returns>
        float CalcPower(Frame frame, Point3D elementCenter);

        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void CreateParameter(string name, float value);
        /// <summary>
        /// CreateParameter
        /// </summary>
        /// <param name="name"></param>
        void CreateParameter(string name);
        /// <summary>
        /// GetParameters
        /// </summary>
        /// <returns></returns>
        IEnumerable<FunctionParameter> GetParameters();
    }
}
