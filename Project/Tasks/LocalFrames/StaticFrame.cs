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
                return $"{Name}(*,{Shifting}|{Rotation_X}|{Rotation_Y}|{Rotation_Z})";
            else
                return $"{Name}({BaseGroup.Name},{Shifting}|{Rotation_X}|{Rotation_Y}|{Rotation_Z})";
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
            // Это не лишнее.
            // Без этого обновление положения будут применяться
            // к уже измененому положению!!!
            else
                frame = new Frame();

            frame.Rotate(frame.Dir_X, Rotation_X);
            frame.Rotate(frame.Dir_Y, Rotation_Y);
            frame.Rotate(frame.Dir_Z, Rotation_Z);
            frame.Shift(Shifting);
        }
    }
}