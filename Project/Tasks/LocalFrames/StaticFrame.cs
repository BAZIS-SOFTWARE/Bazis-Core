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

        internal StaticFrame(Point3D a, Point3D b, Point3D c)
        {
            frame = new Frame(a,b,c);
        }

        internal StaticFrame(Frame _frame)
        {
            frame = _frame;
        }

        internal StaticFrame()
        {
            frame = new Frame();
        }

        public StaticFrame(IGroup baseGr)
        {
            if (baseGr.ObjType != ObjType.Узел)
                throw new System.Exception("Группа должна состоять из узлов");
            else if (baseGr.Count() != 3)
                throw new System.Exception("Должно быть три узла");

            BaseGroup = baseGr;
        }
/// <inheritdoc/>

        public override Frame Frame
        {
            get 
            {
                if (BaseGroup == null)
                    return frame;
                else
                {
                    var points = BaseGroup.Select(x => x.CalcCentr()).ToArray();

                    if (points.Length != 3)
                        throw new System.Exception("Должно быть три узла");

                    frame = new Frame(points[0], points[1], points[2]);
                    frame.Rotate(frame.Dir_X, Rotation);
                    frame.Shift(Shifting);
                    return frame;
                }
            }
            set
            {
                frame = value;
            }
        }


        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{BaseGroup.Name};{Shifting}|{Rotation}";
        }
    }
}