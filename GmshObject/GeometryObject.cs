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

        public void DeleteEntity(int entityId)
        {

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

        public void DeleteEntities(int[] id, bool recursive = false)
        {
            var dimTags = id.Select(x => Tuple.Create(1, x)).ToArray();
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
