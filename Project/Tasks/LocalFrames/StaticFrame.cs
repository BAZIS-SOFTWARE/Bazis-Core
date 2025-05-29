using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Project.Tasks.FrameCreators
{
    public class StaticFrame : LocalFrame
    {
        public IGroup BaseGroup { get; }

        public StaticFrame(IGroup baseGr)
        {
            if (baseGr.ObjType != ObjType.Узел)
                throw new System.Exception("Группа должна состоять из узлов");
            else if (baseGr.Count() != 3)
                throw new System.Exception("Должно быть три узла");

            BaseGroup = baseGr;
        }
/// <inheritdoc/>

        public override Frame Build()
        {
            var points = BaseGroup.Select(x => x.CalcCentr()).ToArray();

            if (points.Length != 3)
                throw new System.Exception("Должно быть три узла");

            return new Frame(points[0], points[1],points[2]);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{BaseGroup.Name};{Shifting}|{Rotation}";
        }
    }
}