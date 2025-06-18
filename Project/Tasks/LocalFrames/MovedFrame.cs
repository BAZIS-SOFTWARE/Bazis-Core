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
        public MovedFrame(IGroup baseLine, IGroup refLine, float _velocity)
        {
            if (baseLine.Count == 0 || refLine.Count == 0)
                throw new Exception("Линия должна собержать точки!");
            if (_velocity == 0)
                throw new Exception("Скорость не может равняться нулю!");
            velocity = _velocity;


            if (baseLine.Count != refLine.Count)
                throw new Exception($"Траектории {baseLine.Name} {refLine.Name} должны содержать одинаковое кол-во точек!");

            var res = baseLine.Intersect(refLine, new EqualityComparerByNumber<IModelObject>());
            if (res.Count() != 0)
                throw new Exception("Траектории пересекаются!");

            BaseLine = baseLine;
            RefLine = refLine;

            CalcPosition();

            //CheckStartPoints(startPoints, baseLine);
            //CheckStartPoints(startPoints, refLine);

            //SortTrajNodes(startPoints, baseLine);
            //SortTrajNodes(startPoints, refLine);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return
                $"MRF;{BaseLine.Name}|{RefLine.Name};{Velocity};{Shifting}|{Rotation}";
        }
        /// <inheritdoc/>
        public IModelObject StartPoint
        {
            get { return BaseLine.First(); }
        }
        /// <inheritdoc/>

        public IModelObject StopPoint
        {
            get { return BaseLine.Last(); }
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

        public IGroup BaseLine { get; }
        /// <inheritdoc/>

        public IGroup RefLine { get; }

        /// <inheritdoc/>

        public void SetShiftingAndRotation(string shiftingData)
        {
            var shifftingData = shiftingData.Split('|');

            if (shifftingData.Length == 4)
            {
                if (!float.TryParse(shifftingData[0], out var x))
                    throw new Exception("Смещение по X задано неверно!");
                if (!float.TryParse(shifftingData[1], out var y))
                    throw new Exception("Смещение по Y задано неверно!");
                if (!float.TryParse(shifftingData[2], out var z))
                    throw new Exception("Смещение по Z задано неверно!");
                if (!float.TryParse(shifftingData[3], out var angle))
                    throw new Exception("Угол задан неверно!");

                Shifting = new Point3D(x, y, z);
                Rotation = angle;
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
            var path = 0.0f;
            for (var i = 1; i < BaseLine.Count; i++)
                path = path + new Segment3D(BaseLine[i].CalcCentr(), BaseLine[i - 1].CalcCentr()).GetLength();

            var stopTime = path / velocity;

            return stopTime;
        }
/// <inheritdoc/>

        public override void CalcPosition()
        {
            var posit = new Point3D();
            var path = 0.0f;

            var dirY = new Point3D();
            var dirX = new Point3D();

            for (var i = 0; i < BaseLine.Count - 1; i++)
            {
                var line = new Segment3D(BaseLine[i].CalcCentr(), BaseLine[i + 1].CalcCentr());
                path = path + line.GetLength();
                dirY = BaseLine[i + 1].CalcCentr().Sub(BaseLine[i].CalcCentr());
                dirX = BaseLine[i].CalcCentr().Sub(RefLine[i].CalcCentr());
                if (path >= PathPassed)
                {
                    var delta = path - PathPassed;
                    var p = line.GetPointOnLine(line.GetLength() - delta);

                    posit = BaseLine[i].CalcCentr().Sum(p);

                    break;
                }

                posit = BaseLine[i].CalcCentr();

            }

            frame = new Frame(posit, posit.Sum(dirY), posit.Sum(dirX));

            frame.Rotate(frame.Dir_X, Rotation);
            frame.Shift(Shifting);
        }
    }
}