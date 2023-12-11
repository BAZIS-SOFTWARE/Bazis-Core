using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;

namespace ModelInterfaces
{
    /// <summary>
    /// INode
    /// </summary>
    public interface INode : IGeometryPoint
    {
        /// <summary>
        /// AddElement
        /// </summary>
        /// <param name="element"></param>
        void AddElement(IElement element);
        /// <summary>
        /// GetElements
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElement> GetElements();
    }
}