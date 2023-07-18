using Geometry;
using Model.GmshWrapper;
using System;
using System.Collections.Generic;
using Model.Interfaces;
using System.Linq;

namespace Model.GmshObject
{  
    public class GeometryObject : ModelObject, IGeometryApi
    {
        public int CurveSteps 
        { 
            get => Convert.ToInt32(GmshWrapperGeneral.OptionGetNumber("Geometry.NumSubEdges")); 
            set => GmshWrapperGeneral.OptionSetNumber("Geometry.NumSubEdges",Convert.ToDouble(value));
        }
        public GeometryObject() : base(){}

        public GeometryObject(string path) : base(path){}

        public IEnumerable<Node> GetControlPoints()
        {
            SetCurrent();
            return GmshWrapperGeometry.GetEntities<Node>(0, GmshWrapperGeometry.CreateControlPoint);
        }
        public IEnumerable<Curve> GetСurves()
        {
            SetCurrent();
            return GmshWrapperGeometry.GetEntities<Curve>(1, GmshWrapperGeometry.CreateCurve);
        }
        public IEnumerable<Element1D> GetElements1D()
        {
            SetCurrent();
            return GmshWrapperGeometry.GetEntities<Element1D>(1, GmshWrapperGeometry.CreateElement1D);
        }
    }
}
