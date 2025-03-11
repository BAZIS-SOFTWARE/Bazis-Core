using Geometry;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.MeshObjects;
using PostProc.ScenePresenter.Interfaces;
using Project.Interfaces;
using Project.Results;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PostProc.ScenePresenter
{
    public class ResultsFieldsCreator : IResultsFieldsCreator
    {
        ItemRange[] itemRanges;

        /// <summary>
        /// ScaleFactor
        /// </summary>
        public int ScaleFactor { get; set; }      

        public IEnumerable<ILineObject<IPoint>> CreateLineObjects(IResult result, ObjType objsType, string resDescr, ILineObject<Node> beamElement)
        {
            var gradsCreator = new LineGradientsCreator();

            throw new NotImplementedException();
        }

        public IEnumerable<ResultFigure> CreateSurfaceObjects(Result result, ObjType objsType, string resDescr, IEnumerable<ISurfaceElement> surfaceObjects)
        {
            var gradsCreator = new SurfaceGradientsCreator();
            var surfacetObjects = new List<ResultFigure>();
            foreach (var surfaceObject in surfaceObjects)
            {
                if(surfaceObject.ViewState)
                {
                    foreach (var surface in surfaceObject.GetSurfaces())
                    {
                        if (surface.ViewState)
                        {
                            //var nodeCoords = new List<Point3D>();
                            //var nodeRes = new List<float>();
                            var resItems = new List<ResultItem>(); 

                            foreach (var node in surface.GetVertexes())
                            {
                                var nodeDispl = GetNodeDispls(resDescr, node.Number, result);
                                //nodeCoords.Add(node.Position.Sum(nodeDispl));
                                var resItem = new ResultItem();
                                resItem.Coord = node.Position.Sum(nodeDispl);

                                if (objsType == ObjType.Узел)
                                    resItem.Value = result.GetValue(node.Number, resDescr);
                                else resItem.Value = result.GetValue(surfaceObject.Number, resDescr);
                                resItems.Add(resItem);
                            }

                            IEnumerable<ResultFigure> surfaces;
                            if (resItems.Count > 3)
                            {
                                surfaces = gradsCreator.CreateGradientObjects(itemRanges, 
                                    new List<ResultItem>() { resItems[0], resItems[1], resItems[2] });
                                surfacetObjects.AddRange(surfaces);
                                surfaces = gradsCreator.CreateGradientObjects(itemRanges, 
                                    new List<ResultItem>() { resItems[0], resItems[2], resItems[3] });
                                surfacetObjects.AddRange(surfaces);
                            }
                            else
                            {
                                surfaces = gradsCreator.CreateGradientObjects(itemRanges, resItems);
                                surfacetObjects.AddRange(surfaces);
                            }    

                        }
                    }
                }
            }    
            return surfacetObjects;
        }
/// <inheritdoc/>

        public void SetScaleItems(ItemRange[] itemRanges)
        {
            if (itemRanges.Length == 0)
                throw new Exception("Последовательность не содержит диапазонов");

            this.itemRanges = itemRanges;
        }

        private Point3D GetNodeDispls(string nodeRes, int nodeNumber, IResult result)
        {
            if(result.TaskKind == TaskKind.механическая)
            {
                if (nodeRes == "X")
                {
                    var x = result.GetValue(nodeNumber, "X") * ScaleFactor;
                    return new Point3D(x, 0, 0);
                }
                else if (nodeRes == "Y")
                {
                    var y = result.GetValue(nodeNumber, "Y") * ScaleFactor;
                    return new Point3D(0, y, 0);
                }
                else if (nodeRes == "Z")
                {
                    var z = result.GetValue(nodeNumber, "Z") * ScaleFactor;
                    return new Point3D(0, 0, z);
                }
                else
                {
                    var x = result.GetValue(nodeNumber, "X") * ScaleFactor;
                    var y = result.GetValue(nodeNumber, "Y") * ScaleFactor;
                    var z = result.GetValue(nodeNumber, "Z") * ScaleFactor;
                    return new Point3D(x, y, z);
                }
            }
            else return new Point3D();
        }
    }
}
