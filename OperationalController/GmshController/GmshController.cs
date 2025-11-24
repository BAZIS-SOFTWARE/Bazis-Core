using System;
using System.Collections.Generic;
using System.Linq;
using Geometry;
using Model.Interfaces;
using Model.MeshObjects;
using Model.GeometryObjects;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsFinders;
using GmshApi;
using System.Drawing;
using Model.ObjectsCollections;
using Model.Interfaces.ObjectsCollections;
using System.Reflection;
using Model.Utilities;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Xml.Linq;
using MathNet.Numerics;
//using static IronPython.Modules._ast;

namespace OperationalController.GmshController
{
    /// <summary>
    /// Контроллер GmshApi
    /// </summary>
    public class GmshController : IGmshController
    {
        /// <inheritdoc/>
        public Gmsh Gmsh { get; private set; }

        /// <inheritdoc/>
        public void Load(string dllToLoad)
        {
            GmshKernel.DLL = GmshKernel.LoadLibrary(dllToLoad);
            Gmsh = new Gmsh();
            Gmsh.Initialize();
        }
        /// <inheritdoc/>
        public void Unload()
        {
            Gmsh.finalize();
        }
        /// <inheritdoc/>
        public List<Node> GetNodes(int dim = -1, int tag = -1, bool includeBoundary = true)
        {
            var data = Gmsh.Model.Mesh.GetNodes(dim, tag, includeBoundary, false);
            var tags = data.Item1;
            var coords = data.Item2;
            var nodes = new Node[tags.Length];

            //if (tags.Length == 0)
            //    throw new Exception("Геометрическая модель не содержит узлов сетки");

            for (var i = 0; i < tags.Length; ++i)
            {
                var point = new Point3D((float)coords[i * 3], (float)coords[i * 3 + 1], (float)coords[i * 3 + 2]);
                nodes[i] = new Node((int)tags[i], point);
            }

            return nodes.OrderBy(x => x.Number).
                Distinct(new DefaultModelObjsComparer<Node>()).ToList();
        }
        /// <inheritdoc/>
        public List<GeometryPoint> CreateControlPoints()
        {
            var nodes = new List<GeometryPoint>();
            var dimTags = Gmsh.Model.GetEntities(0);
            for (var i = 1; i < dimTags.Length; i += 2)
            {
                var coords = Gmsh.Model.GetValue(0, dimTags[i], new double[] { });
                var point = new Point3D((float)coords[0], (float)coords[1], (float)coords[2]);
                nodes.Add(new GeometryPoint(dimTags[i], point));
            }
            return nodes;
        }
        /// <inheritdoc/>
        public List<Curve> CreateLines()
        {
            var lines = new List<Curve>();
            var curves = Gmsh.Model.GetEntities(1);
            for (var i = 1; i < curves.Length; i += 2)
            {
                // кол-во отрезков по-умолчанию равно 40
                var steps = Gmsh.Model.GetType(curves[i - 1], curves[i]).Contains("Line") ? 1 : (int)Gmsh.Option.GetNumber("Geometry.NumSubEdges");
                var tInc = 1d / steps;
                var curInc = 0.0;
                var curveParams = Gmsh.Model.GetParametrizationBounds(curves[i - 1], curves[i]);
                var dir = curveParams.Item2[0] - curveParams.Item1[0];
                var t = new double[1];
                var points = new List<GeometryPoint>();
                GeometryPoint last = null;
                for (var j = 0; j <= steps; ++j, curInc += tInc)
                {
                    t[0] = curveParams.Item1[0] + curInc * dir;
                    var coords = Gmsh.Model.GetValue(curves[i - 1], curves[i], t);
                    var current = new GeometryPoint(j, new Point3D((float)coords[0], (float)coords[1], (float)coords[2]));
                    if (j != 0)
                    {
                        // тут дублирование, так как это требуется для визуализации
                        points.Add(last);
                        points.Add(current);
                    }
                    last = current;
                }


                // Интегрируем gmshController в projectController. 
                // функцию Gmsh.Model.GetAdjacencies будем использовать явно для
                // улучшения синхронизации
                //var adjData = Gmsh.Model.GetAdjacencies(1, curves[i]);
                var curve = new Curve(curves[i], points.ToArray());
                //curve.PointsNumbers = adjData.Item2;
                lines.Add(curve);
            }
            return lines;
        }
        /// <inheritdoc/>
        public List<IElement> GetMeshEntities(int dim, int tag, List<Node> nodes)
        {
            var list = new List<IElement>();
            if (nodes.Count > 0)
            {
                var data = Gmsh.Model.Mesh.GetElements(dim, tag);
                for (var i = 0; i < data.Item1.Length; ++i)
                {
                    var properties = Gmsh.Model.Mesh.GetElementProperties(data.Item1[i]);
                    var nodesCount = properties.numNodes;
                    var elements = data.Item2[i];

                    for (var j = 0; j < elements.Length; ++j)
                    {
                        var nodesPerElems = new Node[nodesCount];
                        for (int k = 0; k < nodesCount; ++k)
                        {
                            var tagStride = j * nodesCount + k;
                            nodesPerElems[k] = ByNumberFinder.Find(nodes, (int)data.Item3[i][tagStride]);
                            if (nodesPerElems[k] == null)
                                throw new Exception($"Элемент с номером {data.Item3[i][tagStride]} не найден");
                        }
                        list.Add(CreateModelObject(ref properties.elementName, (int)elements[j], nodesPerElems));
                    }
                }
            }
            return list;
        }

        public IEnumerable<ObjectsSet<T>> GetMeshSets<T>(List<Node>nodes) 
            where T : class,IElement
        {
            var dim = 1;
            if (typeof(Element2D) == typeof(T))
                dim = 2;
            else if (typeof(Element3D) == typeof(T))
                dim = 3;

            var dimTags = Gmsh.Model.GetEntities(dim);
            //var list = new List<ObjectsSet<T>>();
            var rnd = new Random();

            for (var i = 0; i < dimTags.Length; i += 2)
            {
                var tag = dimTags[i + 1];
                var set = new ObjectsSet<T>($"e{dim}D_{tag}");
               
                var elements = GetMeshEntities(dim, tag, nodes);

                foreach (var item in elements)
                {
                    if(item is T obj)
                        set.Add(item.Number, obj);
                }
               
                var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                set.SetColor(color);

                if (set.Count != 0)
                    yield return set;
            }
                
            //return list;
        }
        /// <inheritdoc/>
        public (List<Node>, List<ILineObject<Node>>, List<Element2D>, List<Element3D>) GetMeshObjects()
        {
            var nodes = GetNodes(-1, -1, false);
            var e1D = new List<ILineObject<Node>>();
            var e2D = new List<Element2D>();
            var e3D = new List<Element3D>();
            Action<string, int, Node[]> filler1D = (s, i, n) =>
            {
                e1D.Add(new Beam(i, n));
            };
            Action<string, int, Node[]> filler2D = (s, i, n) =>
            {
                if (s.Contains("Triangle"))
                    e2D.Add(new Triangle_c(i, n));
                else
                    e2D.Add(new Quad(i, n));
            };
            Action<string, int, Node[]> filler3D = (s, i, n) =>
            {
                if (s.Contains("Tetrahedron"))
                    e3D.Add(new Tetra_c(i, n));
                else if (s.Contains("Hexahedron"))
                    e3D.Add(new Hexa(i, n));
                else
                    e3D.Add(new Penta(i, n));
            };
            GetMeshEntities(1, -1, nodes, filler1D);
            GetMeshEntities(2, -1, nodes, filler2D);
            GetMeshEntities(3, -1, nodes, filler3D);

            return (nodes, e1D, e2D, e3D);
        }

        /// <inheritdoc/>
        public double CalcCurveLength(int curveTag) => Gmsh.Model.Occ.GetMass(1, curveTag);

        /// <inheritdoc/>
        public int GetPointsCountByElementSize(int curveTag, float elemSize)
        {
            if (elemSize < 0 || Math.Abs(elemSize) < 1E-4)
                return 0;
            var length = CalcCurveLength(curveTag);
            return (int)Math.Ceiling(length / elemSize) + 1;
        }
        /// <inheritdoc/>
        public void DeleteMeshElements(IntPtr[] elementTags)
        {
            Gmsh.Model.Mesh.SetVisibility(elementTags, 0);
            Gmsh.Plugin.SetNumber(PluginName.Invisible, InvisibleOptions.DeleteElements.ToString(), 1);
            Gmsh.Plugin.Run(PluginName.Invisible);
        }

        /// <inheritdoc/>
        public void CreateGeometry(double surfAngle = Math.PI / 2, double curveAngle = Math.PI)
        {
            Gmsh.Model.Mesh.ClassifySurfaces(surfAngle, true, true);
            var dimTags = Gmsh.Model.GetEntities(-1);
            Gmsh.Model.Mesh.CreateGeometry(dimTags);
        }

        /// <inheritdoc/>
        public Tuple<IObjectsSet<SurfaceFigure>,VolumeData> CreateSurfaceData()
        {
            var volumes = new VolumeData();
            var rnd = new Random();

            var surfaceFigures = CreateSurfaceFigures();
            var allSurfaces = new ObjectsSet<SurfaceFigure>("FreeSurfaces");
            surfaceFigures.ForEach(x => allSurfaces.Add(x.Number, x));
            allSurfaces.SetColor(Color.FromArgb(255, 125, 0));

            if (Gmsh.Model.GetDimension() > 2)
            {
                //surfaceFigures = surfaceFigures.OrderBy(v => v.Number).ToList();
                var dimTags = Gmsh.Model.GetEntities(3);

                for (var i = 0; i < dimTags.Length; i += 2)
                {
                    var color = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));

                    var dim = dimTags[i];
                    var tag = dimTags[i + 1];
                    var data = Gmsh.Model.GetAdjacencies(dim, tag);
                    var upward = data.Item1;
                    var downward = data.Item2;
                    var name = Gmsh.Model.GetEntityName(dim, tag);

                    // сделаем имя для импорта уникальным во избежания коллизий
                    // 03.10.25 возврат к неуникальным именам. В сборке 
                    //name = $@"SurfaceSet_{tag}";

                    if (name == "" | name == null)
                        name = tag.ToString();
                    else
                        name = name.Replace("Shapes/", "");

                    var volSurfaces = new SurfaceFigure[downward.Length];
                    //set.Number = tag;
                    for (var j = 0; j < downward.Length; ++j)
                    {
                        var numb = downward[j];
                        if (!allSurfaces.ContainsKey(numb))
                            throw new Exception($"Поверхность с номером {numb} не найдена");
                        else
                            volSurfaces[j] = allSurfaces[numb];
                    }

                    var volume = new VolumeFigure(tag, volSurfaces);
                    volume.Name = name;
                    volume.SetColor(color);
                    volumes.Add(volume);
                }
            }
            return Tuple.Create<IObjectsSet<SurfaceFigure>, VolumeData>(allSurfaces, volumes);
        }

        /// <inheritdoc/>
        public List<SurfaceFigure> CreateSurfaceFigures()
        {
            var points = GetNodes(2, -1, false).Select(v => new GeometryPoint(v.Number, v.Position)).ToList();

            var surfaceFigures = new List<SurfaceFigure>();
            if (points.Count > 0)
            {
                var dimTags = Gmsh.Model.GetEntities(2);

                for (var i = 1; i < dimTags.Length; i += 2)
                {
                    var surfaceFigure = CreateSurfaceFigure(points, dimTags[i]);
                    surfaceFigures.Add(surfaceFigure);
                }
            }
            return surfaceFigures;
        }
        /// <inheritdoc/>
        public SurfaceFigure CreateSurfaceFigure(List<GeometryPoint> points, int surfaceTag)
        {
            var data = Gmsh.Model.Mesh.GetElements(2, surfaceTag);
            var nodeTags = data.Item3;

            var geometrySurfaces = new List<GeometrySurface>();
            //N - число точек поверхности, сложность O(N*logN) т.к выполняется бинарный поиск, можно улучшить до O(N);
            for (var i = 0; i < data.Item1.Length; ++i)
            {
                var properties = Gmsh.Model.Mesh.GetElementProperties(data.Item1[i]);
                var nodesCount = properties.numNodes;
                var elements = data.Item2[i];

                for (var j = 0; j < elements.Length; ++j)
                {
                    var nodesPerElems = new GeometryPoint[nodesCount];
                    for (int k = 0; k < properties.numNodes; ++k)
                    {
                        var tagStride = j * nodesCount + k;
                        nodesPerElems[k] = ByNumberFinder.Find(points, (int)nodeTags[i][tagStride]);
                        if (nodesPerElems[k] == null)
                            throw new Exception($"Элемент с номером {nodeTags[i][tagStride]} не найден");
                    }
                    geometrySurfaces.Add(new GeometrySurface((int)elements[j], nodesPerElems));
                }
            }
            var sFigure = new SurfaceFigure(surfaceTag, geometrySurfaces.ToArray());
            //var adjData = Gmsh.Model.GetAdjacencies(2, surfaceTag);
            //sFigure.CurveNumbers = adjData.Item2; // номера кривых;
            return sFigure;
        }
        /// <inheritdoc/>
        public bool RemoveGeometryObjects(int[] dimTags, bool delChild = false)
        {
            for (var i = 1; i < dimTags.Length; i += 2)
            {
                if (!RemoveAdjointEntities(dimTags[i - 1], dimTags[i]))
                    return false;
                if (!RemoveGeometryObject(new int[] { dimTags[i - 1], dimTags[i] }, delChild))
                    return false;
            }
            return true;
        }

        private IElement CreateModelObject(ref string elementName,
                                                  long elementId, Node[] nodes)
        {
            if (elementName.Contains("Point"))
                return null;
            else if (elementName.Contains("Line"))
                return new Beam((int)elementId, nodes);
            else if (elementName.Contains("Triangle"))
                return new Triangle_c((int)elementId, nodes);
            else if (elementName.Contains("Quadrilateral"))
                return new Quad((int)elementId, nodes);
            else if (elementName.Contains("Tetrahedron"))
                return new Tetra_c((int)elementId, nodes);
            else if (elementName.Contains("Hexahedron"))
                return new Hexa((int)elementId, nodes);
            else
                return new Penta((int)elementId, nodes);
        }

        private void GetMeshEntities(int dim, int tag, List<Node> nodes, Action<string, int, Node[]> fillDelegete)
        {
            if (nodes.Count > 0)
            {
                var data = Gmsh.Model.Mesh.GetElements(dim, tag);
                for (var i = 0; i < data.Item1.Length; ++i)
                {
                    var properties = Gmsh.Model.Mesh.GetElementProperties(data.Item1[i]);
                    var nodesCount = properties.numNodes;
                    var elements = data.Item2[i];

                    for (var j = 0; j < elements.Length; ++j)
                    {
                        var nodesPerElems = new Node[nodesCount];
                        for (int k = 0; k < nodesCount; ++k)
                        {
                            var tagStride = j * nodesCount + k;
                            nodesPerElems[k] = ByNumberFinder.Find(nodes, (int)data.Item3[i][tagStride]);
                            if (nodesPerElems[k] == null)
                                throw new Exception($"Элемент с номером {data.Item3[i][tagStride]} не найден");
                        }
                        fillDelegete(properties.elementName, (int)elements[j], nodesPerElems);
                    }
                }
            }
        }

        private bool RemoveGeometryObject(int[] dimTags, bool recursive)
        {
            Gmsh.Model.RemoveEntities(dimTags, recursive);
            var error = Gmsh.Logger.GetLastError();
            return string.IsNullOrEmpty(error);
        }

        private bool RemoveAdjointEntities(int dim, int tag)
        {
            var data = Gmsh.Model.GetAdjacencies(dim, tag);
            for (var i = 0; i < data.Item1.Length; ++i)
            {
                if (!RemoveAdjointEntities(dim + 1, data.Item1[i]))
                    return false;
            }
            if (!RemoveGeometryObject(new int[] { dim, tag }, false))
                return false;
            return true;
        }
/// <inheritdoc/>

        public double CalcVolume(int volTag)
        {
            // Реализовать метод вычисления объема по функции
            // заданной параметрически (из коробки)
            return 0;
        }
/// <inheritdoc/>

        public List<VolumeFigure> CreateVolumeFigures()
        {
            var volumeFigures = new List<VolumeFigure>();
            if (Gmsh.Model.GetDimension() > 2)
            {
                var surfaceFigures = CreateSurfaceFigures();
                if (surfaceFigures.Count > 0)
                {
                    surfaceFigures = surfaceFigures.OrderBy(v => v.Number).ToList();
                    var dimTags = Gmsh.Model.GetEntities(3);

                    for (var i = 0; i < dimTags.Length; i += 2)
                    {
                        var dim = dimTags[i];
                        var tag = dimTags[i + 1];
                        var data = Gmsh.Model.GetAdjacencies(dim, tag);
                        var upward = data.Item1;
                        var downward = data.Item2;

                        var surfacesOfVolume = new SurfaceFigure[downward.Length];
                        for (var j = 0; j < downward.Length; ++j)
                        {
                            var surfaceFigure = ByNumberFinder.Find(surfaceFigures, downward[j]);
                            if (surfaceFigure == null)
                                throw new Exception($"Поверхность с номером {surfaceFigures} не найдена");
                            else
                                surfacesOfVolume[j] = surfaceFigure;
                        }
                        volumeFigures.Add(new VolumeFigure(tag, surfacesOfVolume));
                    }
                }
            }
            return volumeFigures;
        }
        /// <inheritdoc/>
        public string[] GetTransfiniteVolume(int tag)
        {
            var attrs = Gmsh.Model.GetAttribute($"transfinite vol {tag}");
            return attrs;
        }
        /// <inheritdoc/>

        public void SetTransfiniteVolume(int tag, int[] cornerTags = null)
        {
            Gmsh.Model.SetAttribute($"transfinite vol {tag}",
                new string[] { "регулярная" });

            Gmsh.Model.Mesh.SetTransfiniteVolume(tag, cornerTags);
        }

        /// <inheritdoc/>

        public void SetGradientVolume(int tag, double power, double distMax,double surfSize,double coreSize)
        {
            var curvesTags = new List<double>();
            var surfTags = Gmsh.Model.GetAdjacencies(3, tag).Item2;

            foreach (var item in surfTags)
            {
                foreach (var crvNumb in Gmsh.Model.GetAdjacencies(2, item).Item2)
                {
                    var pointsTags = Gmsh.Model.GetAdjacencies(1, crvNumb).Item2;
                    foreach (var pointNumb in pointsTags)
                    {
                        SetSize(pointNumb, surfSize);
                    }
                    curvesTags.Add(crvNumb);
                }
                
            }
            var curveNumbers = curvesTags.Distinct().ToArray();
            var surfNumbers = surfTags.Select(x => (double)x).ToArray();

            var attributes = new string[] 
            { 
                "градиентная", 
                power.ToString(),
                distMax.ToString(),
                surfSize.ToString(),
                coreSize.ToString()
            };
            Gmsh.Model.
                SetAttribute($"transfinite vol {tag}", attributes);

            Gmsh.Model.Mesh.Field.SetNumbers(tag, ExtendOptions.CurvesList.ToString(), curveNumbers);
            Gmsh.Model.Mesh.Field.SetNumbers(tag, ExtendOptions.SurfacesList.ToString(), surfNumbers);
            Gmsh.Model.Mesh.Field.SetNumber(tag, ExtendOptions.Power.ToString(), power);
            Gmsh.Model.Mesh.Field.SetNumber(tag, ExtendOptions.DistMax.ToString(), distMax);
            Gmsh.Model.Mesh.Field.SetNumber(tag, ExtendOptions.SizeMax.ToString(), coreSize);
            Gmsh.Model.Mesh.Field.SetAsBackgroundMesh(tag);
            Gmsh.Option.SetNumber("Mesh.MeshSizeExtendFromBoundary", -2);
        }
        /// <inheritdoc/>
        public void SetTransfiniteSurface(int tag, Arrangement arrangement = Arrangement.Left, int[] cornerTags = null)
        {
            if (cornerTags == null)
                throw new Exception("Укажите угловые узлы");

            if (cornerTags.Length != 4)
                Gmsh.Model.Mesh.SetTransfiniteSurface(tag, arrangement, cornerTags);
            else
                Gmsh.Model.Mesh.SetTransfiniteSurface(tag, arrangement);


            var pointsStr = string.Join(",",cornerTags);
            var attributes = new string[] { pointsStr, Arrangement.Left.ToString() };
            
            Gmsh.Model.
                SetAttribute($"transfinite surface {tag}", attributes);              
        }
        /// <inheritdoc/>
        public void SetRecombineSurface(int tag)
        {
            Gmsh.Model.Mesh.SetRecombine(2, tag);
            Gmsh.Model.
    SetAttribute($"recombine surface {tag}", new string[] { "quad" });
        }

        /// <inheritdoc/>

        public bool GetRecombineSurface(int tag)
        {
            var attributes = Gmsh.Model.GetAttribute($"recombine surface {tag}");

            return attributes.Length != 0;
        }


        /// <inheritdoc/>

        public string[] GetTransfiniteSurface(int tag)
        {
            return Gmsh.Model.GetAttribute($"transfinite surface {tag}");
        }



        /// <inheritdoc/>
        public void SetTransfiniteCurve(int tag, int numNodes, MeshType meshType = MeshType.Progression, double coef = 1)
        {
            var attributes = new string[] 
            { 
                numNodes.ToString(), 
                MeshType.Progression.ToString(), 
                coef.ToString() 
            };

            Gmsh.Model.SetAttribute($"transfinite curve {tag}", attributes);
            Gmsh.Model.Mesh.SetTransfiniteCurve(tag, numNodes, meshType, coef);
        }

        /// <inheritdoc/>

        public string[] GetTransfiniteCurve(int tag)
        {
            var attributes = Gmsh.Model.GetAttribute($"transfinite curve {tag}");

            if (attributes.Length == 0)
                attributes = new string[] { "0", MeshType.Progression.ToString(), "1" };
            return attributes;
        }
        /// <inheritdoc/>
        public double GetSize(int tag)
        {
            var dimTags = new int[] { 0, tag };
            return Gmsh.Model.Mesh.GetSizes(dimTags)[0];
        }
        /// <inheritdoc/>
        public void SetSize(int tag, double meshDize)
        {
            var dimTags = new int[] { 0, tag };
            Gmsh.Model.Mesh.SetSize(dimTags, meshDize);
        }
/// <inheritdoc/>

        public int[] GetSurfaceNodes(int tag)
        {
            var adjCurvs = Gmsh.Model.GetAdjacencies(2, tag);
            var pointsNumbs = new List<int>();
            foreach (var item in adjCurvs.Item2)
            {
                var adjNodes = Gmsh.Model.GetAdjacencies(1, item);
                foreach (var pointNumb in adjNodes.Item2)
                {
                    pointsNumbs.Add(pointNumb);
                }
            }
            return pointsNumbs.Distinct().ToArray(); // номера узлов;
        }
    }
}
