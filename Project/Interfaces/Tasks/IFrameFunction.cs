using Geometry;
using Model.Interfaces;
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
        /// Name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// GetIntersectedElements
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        List<T> GetIntersectedObjects<T>(Frame frame, List<T> elements) where T : IModelObject;
        /// <summary>
        /// ChechOverlappingIfMoving
        /// </summary>
        /// <param name="velocity"></param>
        /// <returns></returns>
        bool IsOverlappingSelf(float velocity);
    }
}
