using Geometry;
using Model.Interfaces;
using Project.Tasks;
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
        /// <returns></returns>
        double CalcValue(Point3D elementCenter);
    }
}
