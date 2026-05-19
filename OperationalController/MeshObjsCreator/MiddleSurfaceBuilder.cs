using Model.GeometryObjects;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;
using static IronPython.Modules._ast;

namespace OperationalController.MeshObjsCreator
{
    /// <summary>
    /// Класс построения серединных поверхностей
    /// </summary>
    public class MiddleSurfaceBuilder
    {
        private int pointsPerCurve = 16;
        private IGmshController controller;

        /// <summary>
        /// Конструктор серединных поверхностей
        /// </summary>
        /// <param name="controller">Контроллер gmsh</param>
        public MiddleSurfaceBuilder(IGmshController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Строит серединные поверхности между парами поверхностей из surfaces
        /// </summary>
        /// <param name="surfaces">Парные перечисления поверхностей, между которыми нужно построить поверхности</param>
        public void Build(IEnumerable<int> surfaces)
        {
            var surfs = surfaces.ToList();
            if ((surfs.Count & 1) == 1)
                throw new ArgumentException("Число поверхностей должно быть четным");

            var averageLoops = new List<List<CurveLoop>>();
            for (var i = 0; i < surfs.Count; i += 2)
            {
                var master = GetCurveLoops(surfs[i]);
                var slave = GetCurveLoops(surfs[i + 1]);

                var data = MakeSimilar(master, slave);
                master = data.Item1;
                slave = data.Item2;

                for (var j = 0; j < master.Count; ++j)
                    AverageLoops(master[j], slave[j]);
                //master[0].AverageSurfaces(slave[0]);

                averageLoops.Add(master);
            }
            BuildGeometry(averageLoops);
        }

        private List<CurveLoop> GetCurveLoops(int surface)
        {
            var loops = controller.Gmsh.Model.Occ.GetCurveLoops(surface).Item2;
            var loopsList = new List<CurveLoop>();

            for (var i = 0; i < loops.Length; ++i)
            {
                var loop = new CurveLoop(controller, surface, i, pointsPerCurve);
                loopsList.Add(loop);
            }

            return loopsList;
        }

        private (List<CurveLoop>, List<CurveLoop>) MakeSimilar(List<CurveLoop> master, List<CurveLoop> slave)
        {
            var masterDict = MakeHoleKeys(master);
            var slaveDict = MakeHoleKeys(slave);

            slave[0].MakeSimilar(master[0]);

            var masterList = new List<CurveLoop>() { master[0] };
            var slaveList = new List<CurveLoop>() { slave[0] };

            foreach (var kvp in masterDict)
            {
                if (slaveDict.ContainsKey(kvp.Key))
                {
                    var slaveIndex = slaveDict[kvp.Key];

                    var m = master[kvp.Value];
                    var s = slave[slaveIndex];

                    s.MakeSimilar(m);

                    masterList.Add(m);
                    slaveList.Add(s);
                }
            }
            return (masterList, slaveList);
        }

        private Dictionary<string, int> MakeHoleKeys(List<CurveLoop> loops)
        {
            var dict = new Dictionary<string, int>();

            for (var i = 1; i < loops.Count; ++i)
            {
                var surfList = new List<int>();
                var surface = loops[i].Surface;
                var current = loops[i].Loop.First;
                while (current != null)
                {
                    var surf = controller.Gmsh.Model.GetAdjacencies(1, Math.Abs(current.Value.Tag))
                                                    .Item1
                                                    .Where(v => v != surface)
                                                    .First();
                    surfList.Add(surf);
                    current = current.Next;
                }
                var surfKeys = surfList.OrderBy(v => v).Select(v => v.ToString());
                var key = string.Join(" ", surfKeys);
                dict.Add(key, i);
            }

            return dict;
        }

        /// <summary>
        /// Усредняет координаты двух контуров и записывает эти координаты в master
        /// </summary>
        /// <param name="master">Master контур</param>
        /// <param name="slave">Slave контур</param>
        private void AverageLoops(CurveLoop master, CurveLoop slave)
        {
            var mIter = master.Loop.First;
            var sIter = slave.Loop.First;

            while (mIter != null)
            {
                var mCurve = mIter.Value;
                var sCurve = sIter.Value;

                var mParams = mCurve.GetParametrization(controller, pointsPerCurve);
                var sParams = sCurve.GetParametrization(controller, pointsPerCurve);

                var mAbsTag = Math.Abs(mCurve.Tag);
                var sAbsTag = Math.Abs(sCurve.Tag);

                var mSlaveSurf = controller.Gmsh.Model.GetAdjacencies(1, mAbsTag).Item1
                                                                                 .Where(v => v != master.Surface)
                                                                                 .ToList();
                var sSlaveSurf = controller.Gmsh.Model.GetAdjacencies(1, sAbsTag).Item1
                                                                                 .Where(v => v != slave.Surface)
                                                                                 .ToList();

                var is3dCase = mSlaveSurf.Count == 0 || sSlaveSurf.Count == 0 || mSlaveSurf[0] != sSlaveSurf[0];

                var average = is3dCase ? GetAverageCurveCoordinates(mAbsTag, mParams, sAbsTag, sParams)
                                       : GetAverageCurveCoordinates(mAbsTag, mParams, sAbsTag, sParams, mSlaveSurf[0]);

                mCurve.SetAverageCoords(average);

                mIter = mIter.Next;
                sIter = sIter.Next;
            }
        }

        private void ProjectPointsToSurface(double[] average, int surface)
        {
            var projPoint = new double[3];
            for (int i = 0; i < average.Length; i += 3)
            {
                for (var j = 0; j < 3; ++j)
                    projPoint[j] = average[i + j];

                var coords = controller.Gmsh.Model.GetClosestPoint(2, surface, projPoint).Item1;
                for (var j = 0; j < 3; ++j)
                    average[i + j] = coords[j];
            }
        }

        private bool IsCloseParametrization(double[] curveParams)
        {
            var preLast = curveParams[curveParams.Length - 2];
            var last = curveParams[curveParams.Length - 1];
            return Math.Abs(curveParams[0] - preLast) < 1e-4 && Math.Abs(curveParams[1] - last) < 1e-4;
        }

        private double[] GetAverageCurveCoordinates(int mCurve, double[] mBounds, int sCurve, double[] sBounds, int surface)
        {
            var mReparams = controller.Gmsh.Model.ReparametrizeOnSurface(1, mCurve, mBounds, surface);
            var sReparams = controller.Gmsh.Model.ReparametrizeOnSurface(1, sCurve, sBounds, surface);

            if (mReparams.Length != sReparams.Length)
                throw new Exception($"Массивы репараметризированных кривых на поверхность {surface} имеют разную длинну");

            if (IsCloseParametrization(mReparams) && IsCloseParametrization(sReparams))
            {
                var coords = GetAverageCurveCoordinates(mCurve, mBounds, sCurve, sBounds);
                ProjectPointsToSurface(coords, surface);
                return coords;
            }

            var averageUv = new double[mReparams.Length];
            for (var i = 0; i < mReparams.Length; i += 2)
            {
                averageUv[i] = (mReparams[i] + sReparams[i]) * 0.5;
                averageUv[i + 1] = (mReparams[i + 1] + sReparams[i + 1]) * 0.5;
            }

            var average = controller.Gmsh.Model.GetValue(2, surface, averageUv);

            return average;
        }

        private double[] GetAverageCurveCoordinates(int mCurve, double[] mBounds, int sCurve, double[] sBounds)
        {
            var mCoords = controller.Gmsh.Model.GetValue(1, mCurve, mBounds);
            var sCoords = controller.Gmsh.Model.GetValue(1, sCurve, sBounds);

            if (mCoords.Length != sCoords.Length)
                throw new Exception($"Массивы координат кривых {mCurve} и {sCurve} имеют разную длинну");

            var average = new double[mCoords.Length];
            for (var i = 0; i < mCoords.Length; ++i)
                average[i] = (mCoords[i] + sCoords[i]) * 0.5;

            return average;
        }

        private void BuildGeometry(List<List<CurveLoop>> averageData)
        {
            var boundPointSet = new Dictionary<int, int>();
            var curveSet = new Dictionary<int, int>();
            var initCurveSet = new Dictionary<int, CurveLoop.CurveLoopNode>();

            var lastStride = (pointsPerCurve - 1) * 3;
            var surfWires = new (int, int[])[averageData.Count];

            controller.Gmsh.Model.Add("Middle surface");

            for (var i = 0; i < averageData.Count; ++i)
            {
                var loops = averageData[i];
                var wires = new int[loops.Count];

                for (var j = 0; j < loops.Count; ++j)
                {
                    var loop = loops[j].Loop;
                    var current = loop.First;
                    var curveArray = new int[loop.Count];
                    var index = 0;

                    while (current != null)
                    {
                        var pointArray = new int[pointsPerCurve];
                        var absCurve = Math.Abs(current.Value.Tag);

                        if (curveSet.ContainsKey(absCurve))
                        {
                            var oldCurve = initCurveSet[absCurve];
                            curveArray[index] = oldCurve.Tag == current.Value.Tag ? curveSet[absCurve]
                                                                                  : -curveSet[absCurve];
                            ++index;
                            current = current.Next;
                            continue;
                        }

                        pointArray[0] = CreateGeometryPoint(boundPointSet, current.Value.Begin, current.Value.AverageCoords, 0);
                        AddGeometryPoints(pointArray, lastStride, current.Value.AverageCoords);
                        pointArray[pointsPerCurve - 1] = CreateGeometryPoint(boundPointSet, current.Value.End, current.Value.AverageCoords, lastStride);

                        curveArray[index] = CreateCurve(pointArray);
                        curveSet.Add(absCurve, curveArray[index]);
                        initCurveSet.Add(absCurve, current.Value);

                        RemoveInsidePoints(pointArray);

                        ++index;
                        current = current.Next;
                    }
                    AddWire(curveArray, wires, j);
                }
                var surface = CreateSurface(wires/*, loops[0].AverageCoords*/);
                surfWires[i] = (surface, wires);
            }
            TrimSurfaces(surfWires);
            controller.Gmsh.Model.Occ.Synchronize();
        }

        private int CreateSurface(int[] wires/*, double[] gridCoords*/)
        {
            /*var grid = new List<int>();
            if(gridCoords != null)
                for(var i = 0; i < gridCoords.Length; i+=3)
                    grid.Add(controller.Gmsh.Model.Occ.AddPoint(gridCoords[i], gridCoords[i + 1], gridCoords[i + 2]));

            int[] points = gridCoords != null ? grid.ToArray() : null;*/
            var surface = controller.Gmsh.Model.Occ.AddSurfaceFilling(wires[0]/*, -1, points*/);
            if (surface == 0)
                throw new Exception(controller.Gmsh.Logger.GetLastError());

            //RemoveInsidePoints(points, 0, 0, 0);

            return surface;
        }

        private int CreateCurve(int[] pointArray)
        {
            var newCurve = controller.Gmsh.Model.Occ.AddSpline(pointArray);

            if (newCurve == 0)
                throw new Exception(controller.Gmsh.Logger.GetLastError());

            return newCurve;
        }

        private void AddGeometryPoints(int[] pointArray, int lastStride, double[] coords)
        {
            var index = 1;
            for (var i = 3; i < lastStride; i += 3)
            {
                var newPoint = controller.Gmsh.Model.Occ.AddPoint(coords[i], coords[i + 1], coords[i + 2], 0);
                if (newPoint == 0)
                    throw new Exception(controller.Gmsh.Logger.GetLastError());
                pointArray[index++] = newPoint;
            }
        }

        private int CreateGeometryPoint(Dictionary<int, int> boundPointSet, int point, double[] coords, int stride)
        {
            if (boundPointSet.ContainsKey(point))
                return boundPointSet[point];

            var newPoint = controller.Gmsh.Model.Occ.AddPoint(coords[stride], coords[stride + 1], coords[stride + 2]);
            if (newPoint == 0)
                throw new Exception(controller.Gmsh.Logger.GetLastError());
            boundPointSet.Add(point, newPoint);

            return newPoint;
        }

        private void RemoveInsidePoints(int[] pointArray, int ignorePoints = 2, int beginIgnore = 1, int endIgnore = 1)
        {
            var dimTags = new int[(pointArray.Length - ignorePoints) * 2];
            for (var i = beginIgnore; i < pointArray.Length - endIgnore; ++i)
            {
                var stride = (i - 1) << 1;
                dimTags[stride] = 0;
                dimTags[stride + 1] = pointArray[i];
            }
            controller.Gmsh.Model.Occ.Remove(dimTags);
        }

        private void AddWire(int[] curveArray, int[] wires, int index)
        {
            var wire = controller.Gmsh.Model.Occ.AddWire(curveArray);
            if (wire == 0)
                throw new Exception(controller.Gmsh.Logger.GetLastError());

            wires[index] = wire;
        }

        private void TrimSurfaces((int, int[])[] surfaces)
        {
            foreach (var surface in surfaces)
            {
                if (surface.Item2.Length > 1)
                {
                    var newSurface = controller.Gmsh.Model.Occ.AddTrimmedSurface(surface.Item1, surface.Item2, true);
                    controller.Gmsh.Model.Occ.Remove([2, surface.Item1], true);
                }
            }
        }

        private MiddleSurfaceBuilder()
        {
        }
    }
}
