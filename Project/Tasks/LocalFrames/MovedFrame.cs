using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Globalization;
using System.Linq;
using Model.Interfaces.ObjectsComparers;
using System.Collections.Generic;

namespace Project.Tasks.FrameCreators
{
    public class MovedFrame : LocalFrame
    {
        private IGroup baseLine;

        private IGroup refLine;

        public MovedFrame(IGroup baseLine, IGroup refLine, float _velocity)
            : this()
        {
            if (_velocity == 0)
                throw new Exception("Скорость не может равняться нулю!");
            Velocity = _velocity;

            BaseLine = baseLine;
            RefLine = refLine;

            //CalcPosition();

            //CheckStartPoints(startPoints, baseLine);
            //CheckStartPoints(startPoints, refLine);

            //SortTrajNodes(startPoints, baseLine);
            //SortTrajNodes(startPoints, refLine);
        }
        /// <summary>
        /// MovedFrame
        /// </summary>
        public MovedFrame()
        {
            Name = "MRF";
            frame = new Frame();
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var bls = "*";
            var rls = "*";
            if (baseLine != null)
                bls = baseLine.Name;
            if (refLine != null)
                rls = refLine.Name;
            return
                $"{Name};{bls}|{rls};{Velocity};{Shifting}|{Rotation}";
        }
        /// <inheritdoc/>
        public IModelObject StartPoint
        {
            get { return baseLine?.First(); }
        }
        /// <inheritdoc/>

        public IModelObject StopPoint
        {
            get { return baseLine?.Last(); }
        }

        float PathPassed
        {
            get { return velocity * time; }
        }

        float time;
        /// <summary>
        /// Time
        /// </summary>       
        public float Time 
        { 
            get
            {
                return time;
            }
            set
            {
                if (value < 0)
                    throw new Exception("Время меньше 0");

                time = value;
            }
        }

        float velocity;

        /// <summary>
        /// Velosity
        /// </summary>
        public float Velocity 
        {
            get { return velocity; }
            set
            {
                if (value < 0)
                    throw new Exception("Скорость меньше 0");

                velocity = value;
            }
        }
        /// <inheritdoc/>

        public IGroup BaseLine      
        {
            get { return baseLine; } 
            set
            {
                if (value.Count == 0)
                    throw new Exception("Линия должна собержать точки!");
                baseLine = value;
            }
        }
        /// <inheritdoc/>

        public IGroup RefLine
        {
            get { return refLine; }
            set
            {
                if (value.Count == 0)
                    throw new Exception("Линия должна собержать точки!");

                if(baseLine == null)
                    throw new Exception("Сначала задайте траекторию!");

                if (baseLine.Count != value.Count)
                    throw new Exception($"Траектории {baseLine.Name} {value.Name} должны содержать одинаковое кол-во точек!");

                var res = baseLine.Intersect(value, new EqualityComparerByNumber<IModelObject>());
                if (res.Count() != 0)
                    throw new Exception("Траектории пересекаются!");

                refLine = value;
            }
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

        public float CalcMotionTime()
        {
            if (baseLine == null)
                return 0;

            var path = 0.0f;
            for (var i = 1; i < baseLine.Count; i++)
                path = path + new Segment3D(baseLine[i].CalcCentr(), baseLine[i - 1].CalcCentr()).GetLength();

            var stopTime = path / velocity;

            return stopTime;
        }
/// <inheritdoc/>

        public override void CalcPosition()
        {
            if (baseLine == null | refLine == null)
                frame = new Frame();
            else
            {
                var posit = new Point3D();


                var path = 0.0f;

                var dirY = new Point3D();
                var dirX = new Point3D();

                for (var i = 0; i < baseLine.Count - 1; i++)
                {
                    var line = new Segment3D(baseLine[i].CalcCentr(), baseLine[i + 1].CalcCentr());
                    path = path + line.GetLength();
                    dirY = baseLine[i + 1].CalcCentr().Sub(baseLine[i].CalcCentr());
                    dirX = baseLine[i].CalcCentr().Sub(refLine[i].CalcCentr());
                    if (path >= PathPassed)
                    {
                        var delta = path - PathPassed;
                        var p = line.GetPointOnLine(line.GetLength() - delta);

                        posit = baseLine[i].CalcCentr().Sum(p);

                        break;
                    }

                    posit = baseLine[i].CalcCentr();

                }
                frame = new Frame(posit, posit.Sum(dirY), posit.Sum(dirX));
            }

            frame.Rotate(frame.Dir_X, Rotation);
            frame.Shift(Shifting);
        }
    }
}