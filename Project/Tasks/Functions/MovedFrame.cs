using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Globalization;
using System.Linq;
using Model.Interfaces.ObjectsComparers;

namespace Project.Tasks.Functions
{
    public class MovedFrame : IMovedFrame
    {
        public MovedFrame(float velocity)
        {
            if (velocity == 0)
                throw new Exception("Скорость не может равняться нулю!");
            Velosity = velocity;
        }

        /// <inheritdoc/>
        public string GetTrajectoryInfo()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0}|{1};{2};{3};{4};{5}|{6}", 
                BaseLine.Name, RefLine.Name, Velosity,
                StartPoints.Name, StopPoints.Name, Shifting.ToString(),Rotation);
        }
        /// <inheritdoc/>
        public IGroup StartPoints { get; set; }
/// <inheritdoc/>

        public IGroup StopPoints { get;  set; }
/// <inheritdoc/>

        public float Velosity { get; set; }
/// <inheritdoc/>

        public IGroup BaseLine { get; set; }
/// <inheritdoc/>

        public IGroup RefLine { get;  set; }
/// <inheritdoc/>

        public Point3D Shifting { get;  set; } = new Point3D();
/// <inheritdoc/>

        public float Rotation { get;  set; }
/// <inheritdoc/>

        public string Name { get;  set; }

/// <inheritdoc/>

        public void SetShiftingAndRotation(string shiftingData)
        {
            var shifftingData = shiftingData.Split('|');

            if(shifftingData.Length == 4)
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
            var axis = sFrame.Y.Sub(sFrame.Centre);
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
/// <inheritdoc/>

        public void CheckTrajNodes()
        {
            //var nodes = nodesGr.ObjsNumbers.Select(x => (Node)project.Model.ObjectData.Find(x)).ToList();
            if (StartPoints.Count != 2)
                throw new Exception("Должно быть только две точки старта!");

            if (StopPoints.Count != 2)
                throw new Exception("Должно быть только две точки конца!");

            if (BaseLine.Count == 0 || RefLine.Count == 0)
                throw new Exception("Линия должна собержать точки!");

            var resIntersec = StartPoints.Intersect(BaseLine, new EqualityComparerByNumber<IModelObject>());

            if (resIntersec.Count() == 0)
                throw new Exception(string.Format("Точки {0} не принадлежат линии {1}!", StartPoints.Name, BaseLine.Name));
            if (resIntersec.Count() == 2)
                throw new Exception(string.Format("Обе точки группы {0} принадлежат линии {1}!", StartPoints.Name, BaseLine.Name));
        }
/// <inheritdoc/>

        public void SortTrajNodes()
        {
            var stBaseNodeInd = StartPoints.Select(x => BaseLine.IndexOf(x)).Where(ind => ind > -1);
            var stRefNodeInd = StartPoints.Select(x => RefLine.IndexOf(x)).Where(ind => ind > -1);

            BaseLine.Swap(0, stBaseNodeInd.First());
            RefLine.Swap(0, stRefNodeInd.First());

            BaseLine.SortByDistance();
            RefLine.SortByDistance();
        }
    }
}