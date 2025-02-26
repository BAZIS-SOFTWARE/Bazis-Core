using Geometry;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.ObjectsFinders;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Controller
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
                    e2D.Add(new Triangle(i, n));
                else
                    e2D.Add(new Quad(i, n));
            };
            Action<string, int, Node[]> filler3D = (s, i, n) =>
            {
                if (s.Contains("Tetrahedron"))
                    e3D.Add(new Tetra(i, n));
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
        public List<Node> GetNodes(int dim = -1, int tag = - 1, bool includeBoundary = true)
        {
            var data = Gmsh.Model.Mesh.GetNodes(dim, tag, includeBoundary, false);
            var tags = data.Item1;
            var coords = data.Item2;
            var nodes = new List<Node>();
            for (var i = 0; i < tags.Length; ++i)
            {
                var point = new Point3D((float)coords[i * 3], (float)coords[i * 3 + 1], (float)coords[i * 3 + 2]);
                nodes.Add(new Node((int)tags[i], point));
            }
            return nodes.OrderBy(x => x.Number).ToList();
        }


        private void GetMeshEntities(int dim, int tag, List<Node> nodes, Action<string, int, Node[]> fillDelegete)
        {
            if (nodes.Count > 0)
            {
                var data = Gmsh.Model.Mesh.GetElements(dim, tag);
                for(var i = 0; i < data.Item1.Length; ++i)
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
    }
}
