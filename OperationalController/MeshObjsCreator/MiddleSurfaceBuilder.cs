using GmshApi;
using Model.GeometryObjects;
using Model.Interfaces;
using OperationalController.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Удалить дубликаты
        /// </summary>
        public bool RemoveDuplicates { get; set; }
        /// <summary>
        /// Использовать раздельную модель для извлечения серединных поврехностей
        /// </summary>
        public bool UseSeparateModel { get; set; }

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

                var mAbsTag = Math.Abs(mCurve.Tag);
                var sAbsTag = Math.Abs(sCurve.Tag);

                var mSlaveSurf = controller.Gmsh.Model.GetAdjacencies(1, mAbsTag).Item1
                                                                                 .Where(v => v != master.Surface)
                                                                                 .ToList();
                var sSlaveSurf = controller.Gmsh.Model.GetAdjacencies(1, sAbsTag).Item1
                                                                                 .Where(v => v != slave.Surface)
                                                                                 .ToList();

                var is3dCase = mSlaveSurf.Count == 0 || sSlaveSurf.Count == 0 || mSlaveSurf[0] != sSlaveSurf[0];

                var average = is3dCase ? GetAverageCurveCoordinates(mCurve, sCurve)
                                       : GetAverageCurveCoordinates(mCurve, sCurve, mSlaveSurf[0]);

                mCurve.SetAverageCoords(average);

                mIter = mIter.Next;
                sIter = sIter.Next;
            }
        }

        private bool IsCloseParametrization(double[] curveParams)
        {
            var preLast = curveParams[curveParams.Length - 2];
            var last = curveParams[curveParams.Length - 1];
            return Math.Abs(curveParams[0] - preLast) < 1e-4 && Math.Abs(curveParams[1] - last) < 1e-4;
        }

        private double[] GetAverageCurveCoordinates(CurveLoop.CurveLoopNode mCurve, CurveLoop.CurveLoopNode sCurve, int surface)
        {
            var mAbsCurve = Math.Abs(mCurve.Tag);
            var sAbsCurve = Math.Abs(sCurve.Tag);

            var mParams = mCurve.GetParametrization(controller, pointsPerCurve);
            var sParams = sCurve.GetParametrization(controller, pointsPerCurve);

            mParams = controller.Gmsh.Model.ReparametrizeOnSurface(1, mAbsCurve, mParams, surface);
            sParams = controller.Gmsh.Model.ReparametrizeOnSurface(1, sAbsCurve, sParams, surface);

            if (IsCloseParametrization(mParams) && IsCloseParametrization(sParams))
            {
                var crds = GetAverageCurveCoordinates(mCurve, sCurve);
                ProjectPointsToSurface(crds, surface);
                return crds;
            }

            var mAdjCurve = GetAdjacentCurve(mAbsCurve, sAbsCurve, mCurve.Begin, sCurve.Begin, surface);
            var sAdjCurve = GetAdjacentCurve(mAbsCurve, sAbsCurve, mCurve.End, sCurve.End, surface);

            var mAdjUv = GetAdjacentCurveUV(mAdjCurve, surface);
            var sAdjUv = GetAdjacentCurveUV(sAdjCurve, surface);

            var offset = GetUVOffset(mCurve, sCurve, mAdjUv, sAdjUv, surface);

            var average = new double[mParams.Length];
            for (int i = 0; i < average.Length; i += 2)
            {
                average[i] = (mParams[i] + sParams[i]) * 0.5 + offset[0];
                average[i + 1] = (mParams[i + 1] + sParams[i + 1]) * 0.5 + offset[1];
            }

            if (mAdjUv != null)
            {
                average[0] = mAdjUv[0];
                average[1] = mAdjUv[1];
            }
            if (sAdjUv != null)
            {
                var last = average.Length - 2;
                average[last] = sAdjUv[0];
                average[last + 1] = sAdjUv[1];
            }
            var coords = controller.Gmsh.Model.GetValue(2, surface, average);

            return coords;
        }

        private double[] GetUVOffset(CurveLoop.CurveLoopNode mCurve, CurveLoop.CurveLoopNode sCurve, double[] mAdjUv, double[] sAdjUv, int surface)
        {
            if (mAdjUv == null && sAdjUv == null)
                return new double[2];

            var mBegin = controller.Gmsh.Model.ReparametrizeOnSurface(0, mCurve.Begin, [], surface);
            var mEnd = controller.Gmsh.Model.ReparametrizeOnSurface(0, mCurve.End, [], surface);

            var sBegin = controller.Gmsh.Model.ReparametrizeOnSurface(0, sCurve.Begin, [], surface);
            var sEnd = controller.Gmsh.Model.ReparametrizeOnSurface(0, sCurve.End, [], surface);

            var mUv = new double[] { (mBegin[0] + sBegin[0]) * 0.5, (mBegin[1] + sBegin[1]) * 0.5 };
            var sUv = new double[] { (mEnd[0] + sEnd[0]) * 0.5, (mEnd[1] + sEnd[1]) * 0.5 };

            var offset = new double[2];

            if (mAdjUv != null && sAdjUv != null)
            {
                var scaleU = 1.0;
                var scaleV = 1.0;

                var difU = sUv[0] - mUv[0];
                var difV = sUv[1] - mUv[1];

                if (Math.Abs(difU) > 1e-4)
                {
                    scaleU = (sAdjUv[0] - mAdjUv[0]) / difU;
                    offset[0] = mAdjUv[0] - mUv[0] * scaleU;
                }
                if (Math.Abs(difV) > 1e-4)
                {
                    scaleV = (sAdjUv[1] - mAdjUv[1]) / difV;
                    offset[1] = mAdjUv[1] - mUv[1] * scaleV;
                }

                return offset;
            }

            if (mAdjUv != null)
            {
                offset[0] = mAdjUv[0] - mUv[0];
                offset[1] = mAdjUv[1] - mUv[1];

                return offset;
            }

            offset[0] = sAdjUv[0] - sUv[0];
            offset[1] = sAdjUv[1] - sUv[1];

            return offset;
        }

        private double[] GetAdjacentCurveUV(int mAdjCurve, int surface)
        {
            if (mAdjCurve != 0)
            {
                var bounds = controller.Gmsh.Model.GetParametrizationBounds(1, mAdjCurve);
                var center = (bounds.Item2[0] + bounds.Item1[0]) * 0.5;

                var uv = controller.Gmsh.Model.ReparametrizeOnSurface(1, mAdjCurve, [center], surface);
                return uv;
            }
            return null;
        }

        /// <summary>
        /// Возвращает общую смежную кривую у mCurve и sCurve если она существует или возвращает 0, если ее нет
        /// </summary>
        /// <param name="mCurve">Кривая основной петли</param>
        /// <param name="sCurve">Кривая подчиненной петли</param>
        /// <param name="mPoint">Начальная точка основной петли</param>
        /// <param name="sPoint">Начальная точка подчиненной петли</param>
        /// <param name="surface">Общая поверхность для mCurve и sCurve</param>
        /// <returns>Идентификатор общей кривой или 0, если кривая не найдена</returns>
        private int GetAdjacentCurve(int mCurve, int sCurve, int mPoint, int sPoint, int surface)
        {
            var set = new HashSet<int>() { mPoint, sPoint };
            var curves = controller.Gmsh.Model.GetAdjacencies(2, surface).Item2.Where(v => v != mCurve && v != sCurve).ToArray();

            for (var i = 0; i < curves.Length; ++i)
            {
                var points = controller.Gmsh.Model.GetAdjacencies(1, curves[i]).Item2.ToArray();
                if (set.Contains(points[0]) && set.Contains(points[1]))
                    return curves[i];
            }
            return 0;
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

        private double[] GetAverageCurveCoordinates(CurveLoop.CurveLoopNode mCurve, CurveLoop.CurveLoopNode sCurve)
        {
            var mBounds = mCurve.GetParametrization(controller, pointsPerCurve);
            var sBounds = sCurve.GetParametrization(controller, pointsPerCurve);

            var mAbsTag = Math.Abs(mCurve.Tag);
            var sAbsTag = Math.Abs(sCurve.Tag);

            var mCoords = controller.Gmsh.Model.GetValue(1, mAbsTag, mBounds);
            var sCoords = controller.Gmsh.Model.GetValue(1, sAbsTag, sBounds);

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

            if(UseSeparateModel)
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
                var surface = CreateSurface(wires);
                surfWires[i] = (surface, wires);
            }
            TrimSurfaces(surfWires);
            controller.Gmsh.Model.Occ.Synchronize();

            if (RemoveDuplicates)
            {
                var lines = controller.Gmsh.Model.GetEntities(1);
                var surfs = controller.Gmsh.Model.GetEntities(2);
                controller.Gmsh.Model.Occ.Fragment(surfs, lines);

                controller.Gmsh.Model.Occ.Synchronize();
            }
        }

        private int CreateSurface(int[] wires)
        {
            var surface = controller.Gmsh.Model.Occ.AddSurfaceFilling(wires[0]);
            if (surface == 0)
                throw new Exception(controller.Gmsh.Logger.GetLastError());

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
