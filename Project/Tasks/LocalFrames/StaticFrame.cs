using Geometry;
using Model.Interfaces;
using System.Linq;

namespace Project.Tasks.FrameCreators
{
    public class StaticFrame : LocalFrame
    {
        private IGroup baseGr;
        public IGroup BaseGroup 
        { 
            get { return baseGr; }
            set
            {
                if (value.ObjType != ObjType.Узел)
                    throw new System.Exception("Группа должна состоять из узлов");
                else if (value.Count() != 3)
                    throw new System.Exception("Должно быть три узла");

                baseGr = value;
            }
        }

        internal StaticFrame(Point3D a, Point3D b, Point3D c)
        {
            frame = new Frame(a,b,c);
        }

        internal StaticFrame(Frame _frame)
        {
            frame = _frame;
        }

        public StaticFrame()
        {
            Name = "SRF";
            frame = new Frame();
        }

        public StaticFrame(IGroup baseGr) : this()
        {
            if (baseGr.ObjType != ObjType.Узел)
                throw new System.Exception("Группа должна состоять из узлов");
            else if (baseGr.Count() != 3)
                throw new System.Exception("Должно быть три узла");

            BaseGroup = baseGr;

            CalcPosition();
        }
/// <inheritdoc/>

        public override Frame Frame
        {
            get 
            {
                return frame;
            }
        }


        /// <inheritdoc/>
        public override string ToString()
        {
            if(BaseGroup == null)
                return $"{Name};*;{Shifting}|{Rotation}";
            else
                return $"{Name};{BaseGroup.Name};{Shifting}|{Rotation}";
        }
/// <inheritdoc/>

        public override void CalcPosition()
        {
            if(BaseGroup != null)
            {
                var points = BaseGroup.Select(x => x.CalcCentr()).ToArray();

                if (points.Length != 3)
                    throw new System.Exception("Должно быть три узла");

                frame = new Frame(points[0], points[1], points[2]);
            }
            // вроде это лишнее
            //else
                //frame = new Frame();

            frame.Rotate(frame.Dir_X, Rotation);
            frame.Shift(Shifting);
        }
    }
}