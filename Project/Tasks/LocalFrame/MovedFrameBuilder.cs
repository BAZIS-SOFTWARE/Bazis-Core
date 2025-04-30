using Model.Interfaces;
using Model.Interfaces.ObjectsComparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project.Tasks.LocalFrame
{
    public class MovedFrameBuilder
    {
        MovedFrame arcHeatSource;

        public MovedFrame Build(IGroup startPoints, IGroup baseLine, IGroup refLine, float velocity)
        {
            CheckStartPoints(startPoints, baseLine);
            CheckStartPoints(startPoints, refLine);

            SortTrajNodes(startPoints, baseLine);
            SortTrajNodes(startPoints, refLine);

            return new MovedFrame(baseLine, refLine, velocity);
        }

        /// <inheritdoc/>

        public void CheckStartPoints(IGroup startPoints, IGroup line)
        {
            var resIntersec = startPoints.Intersect(line, new EqualityComparerByNumber<IModelObject>());

            if (resIntersec.Count() == 0)
                throw new Exception(string.Format("Точки {0} не принадлежат линии {1}!", startPoints.Name, line.Name));
            if (resIntersec.Count() == 2)
                throw new Exception(string.Format("Обе точки группы {0} принадлежат линии {1}!", startPoints.Name, line.Name));
        }
        /// <inheritdoc/>

        public void SortTrajNodes(IGroup startPoints, IGroup traj)
        {
            var ptsInds = startPoints.Select(x => traj.IndexOf(x)).Where(ind => ind > -1);

            traj.Swap(0, ptsInds.First());
            traj.SortByDistance();
        }

        public static implicit operator MovedFrame(MovedFrameBuilder builder)
        {
            return builder.arcHeatSource;
        }
    }
}
