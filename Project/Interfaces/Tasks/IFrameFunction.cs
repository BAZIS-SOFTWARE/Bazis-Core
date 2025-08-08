using Geometry;
using Model.Interfaces;
using Project.Tasks.FrameCreators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// IFrameFunction
    /// </summary>
    public interface IFrameFunction
    {
        /// <summary>
        /// Frame
        /// </summary>
        LocalFrame LocalFrame { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// GetIntersectedElements
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        List<T> GetIntersectedObjects<T>(List<T> elements) where T : IModelObject;
        /// <summary>
        /// ChechOverlappingIfMoving
        /// </summary>
        /// <param name="velocity"></param>
        /// <returns></returns>
        bool IsOverlappingSelf(float velocity);
        /// <summary>
        /// CalcPower
        /// </summary>
        /// <param name="elementCenter"></param>
        /// <returns></returns>
        float CalcValue(Point3D elementCenter);

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
