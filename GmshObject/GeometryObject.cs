using Geometry;
using Model.GmshWrapper;
using System;
using System.Collections.Generic;
using Model.Interfaces;
using System.Linq;
using System.IO;

namespace Model.GmshObject
{  
    public class GeometryObject : IGeometryApi, IDisposable
    {
        private static List<ulong> idList = new List<ulong>();
        private ulong id;
        public ulong Id 
        {
            get => id;
            set
            {
                while(idList.FindIndex(v => v == value) != -1)
                    value = GmshWrapperGeneral.GenerateRandom();
                id = value;
            }
        }
        public int CurveSteps 
        { 
            get => Convert.ToInt32(GmshWrapperGeneral.OptionGetNumber("Geometry.NumSubEdges")); 
            set => GmshWrapperGeneral.OptionSetNumber("Geometry.NumSubEdges",Convert.ToDouble(value));
        }
        public GeometryObject() 
        {
            GmshWrapperConnection.Initialize();
            Id = GmshWrapperGeneral.GenerateRandom();
            GmshWrapperModel.ModelAdd(Id.ToString());
        }

        public GeometryObject(string path)
        {
            GmshWrapperConnection.Initialize();
            Id = GmshWrapperGeneral.GenerateRandom();
            GmshWrapperModel.ModelAdd(Id.ToString());
            Load(path);
        }

        public void Load(string filePath)
        {
            SetCurrent();
            GmshWrapperIO.Merge(filePath);
            GmshWrapperIO.ModelSetFileName(filePath);
        }

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

        private int ConvertToDim(ObjKind objKind)
        {
            switch(objKind)
            {
                case ObjKind.Node: return 0;
                case ObjKind.Point: return 0;
                case ObjKind.Beam: return 1;
                case ObjKind.Curve: return 1;
                case ObjKind.Quad: return 2;
                case ObjKind.Triangle: return 2;
                case ObjKind.GeometrySurface : return 2;
                case ObjKind.ElementSurface : return 2;
                case ObjKind.Tetra: return 3;
                case ObjKind.Penta: return 3;
                case ObjKind.Hexa: return 3;
                default: throw new ArgumentException($"Unable to map element of {objKind} type");
            }
        }

        public void DeleteEntities(List<ModelObject> entities, bool recursive = false)
        {
            var dimTags = entities.Select(entity => Tuple.Create(ConvertToDim(entity.ObjKind), entity.Number))
                .ToArray();
            GmshWrapperModel.RemoveEntities(dimTags, recursive);
        }

        public void Synchronize()
        {
            GmshWrapperModel.Synchronize();
        }

        virtual public void Dispose()
        {
            idList.Remove(Id);
            GmshWrapperModel.ModelRemoveByName(Id.ToString());
        }
        protected void SetCurrent() => GmshWrapperModel.ModelSetCurrent(Id.ToString());
    }
}