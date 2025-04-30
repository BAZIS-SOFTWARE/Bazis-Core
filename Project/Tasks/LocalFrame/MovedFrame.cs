using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Globalization;
using System.Linq;
using Model.Interfaces.ObjectsComparers;
using System.Collections.Generic;

namespace Project.Tasks.LocalFrame
{
    public class MovedFrame : LocalFrame
    {
        public MovedFrame(IGroup baseLine, IGroup refLine, float velocity)
        {
            if (baseLine.Count == 0 || refLine.Count == 0)
                throw new Exception("Линия должна собержать точки!");
            if (velocity == 0)
                throw new Exception("Скорость не может равняться нулю!");
            Velosity = velocity;


            if (baseLine.Count != refLine.Count)
                throw new Exception("Траектории должны содержать одинаковое кол-во точек!");

            var res = baseLine.Intersect(refLine, new EqualityComparerByNumber<IModelObject>());
            if (res.Count() != 0)
                throw new Exception("Траектории пересекаются!");

            BaseLine = baseLine;
            RefLine = refLine;

            //CheckStartPoints(startPoints, baseLine);
            //CheckStartPoints(startPoints, refLine);

            //SortTrajNodes(startPoints, baseLine);
            //SortTrajNodes(startPoints, refLine);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0}|{1};{2};{3}|{4}",
                BaseLine.Name, RefLine.Name, Velosity, Shifting.ToString(), Rotation);
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
        /// <inheritdoc/>

        public float Velosity { get; set; }
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
                if (!float.TryParse(shifftingData[0], out float x))
                    throw new Exception("Смещение по X задано неверно!");
                if (!float.TryParse(shifftingData[1], out float y))
                    throw new Exception("Смещение по Y задано неверно!");
                if (!float.TryParse(shifftingData[2], out float z))
                    throw new Exception("Смещение по Z задано неверно!");
                if (!float.TryParse(shifftingData[3], out float angle))
                    throw new Exception("Угол задан неверно!");

                Shifting = new Point3D(x, y, z);
                Rotation = angle;
            }
        }
        /// <inheritdoc/>


        public Frame CalcFrame(float time)
        {
            var pathPassed = Velosity * time;
            var posit = new Point3D();
            var path = 0.0f;

            Point3D dirY = new Point3D();
            Point3D dirX = new Point3D();

            for (int i = 0; i < BaseLine.Count - 1; i++)
            {
                var line = new Segment3D(BaseLine[i].CalcCentr(), BaseLine[i + 1].CalcCentr());
                path = path + line.GetLength();
                dirY = BaseLine[i + 1].CalcCentr().Sub(BaseLine[i].CalcCentr());
                dirX = BaseLine[i].CalcCentr().Sub(RefLine[i].CalcCentr());
                if (path >= pathPassed)
                {
                    var delta = path - pathPassed;
                    var p = line.GetPointOnLine(line.GetLength() - delta);

                    posit = BaseLine[i].CalcCentr().Sum(p);

                    break;
                }

                posit = BaseLine[i].CalcCentr();

            }

            var frame = new Frame(posit, posit.Sum(dirY), posit.Sum(dirX));

            var sFrame = frame.Shift(Shifting);
            var axis = sFrame.Dir_Y.Sub(sFrame.Centre);
            axis = Vector.GetVectorNorm(axis);
            return sFrame.Rotate(axis, Rotation);
        }

        /// <inheritdoc/>

        public double CalcMotionTime()
        {
            var path = 0.0f;
            for (int i = 1; i < BaseLine.Count; i++)
                path = path + new Segment3D(BaseLine[i].CalcCentr(), BaseLine[i - 1].CalcCentr()).GetLength();

            var stopTime = path / Velosity;

            return stopTime;
        }

    }
}