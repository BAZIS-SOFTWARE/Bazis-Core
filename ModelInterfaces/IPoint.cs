using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelInterfaces
{
    /// <summary>
    /// IPoint
    /// </summary>
    public interface IPoint : IModelObject
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
        /// AddLine
        /// </summary>
        /// <param name="line"></param>
        void AddLine(ILineObject line);
        /// <summary>
        /// GetLines
        /// </summary>
        /// <returns></returns>
        IEnumerable<ILineObject> GetLines();
    }
}
