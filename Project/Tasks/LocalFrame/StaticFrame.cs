using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Project.Tasks.LocalFrame
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

        public Frame CalcFrame()
        {
            var points = BaseGroup.Select(x => x.CalcCentr()).ToArray();

            if (points.Length != 3)
                throw new System.Exception("Должно быть три узла");

            return new Frame(points[0], points[2], points[1]);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{BaseGroup.Name};{Shifting}|{Rotation}";
        }
    }
}