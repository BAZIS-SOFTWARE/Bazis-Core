using Geometry;
using Geometry.Interfaces;
using System;
using System.Collections.Generic;

namespace ModelInterfaces
{
    /// <summary>
    /// INode
    /// </summary>
    public interface INode : IModelObject
    {
        /// <summary>
        /// Position
        /// </summary>
        Point3D Position { get; }
        /// <summary>
        /// Normal
        /// </summary>
        [Obsolete(@"Prop ""Normal"" is deprecated, please don't use.")]     
        Point3D Normal { get; set; }
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