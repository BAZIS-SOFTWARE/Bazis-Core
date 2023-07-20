using Geometry;
using Model.GmshWrapper;
using System;
using System.Collections.Generic;
using Model.Interfaces;
using System.Linq;
using System.IO;

namespace Model.GmshObject
{  
    /// <summary>
    /// Класс для построения геометрических сущностей
    /// </summary>
    public class GeometryObject : IGeometryApi, IDisposable
    {
        private static List<ulong> idList = new List<ulong>();
        private ulong id;
        /// <summary>
        /// Возращает\задает уникальный идентификатор
        /// </summary>
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
        /// <summary>
        /// Устанавливает количество точек для представления кривой
        /// </summary>
        public int CurveSteps 
        { 
            get 
            {
                SetCurrent();
                return Convert.ToInt32(GmshWrapperGeneral.OptionGetNumber("Geometry.NumSubEdges")); 
            }
            set
            {
                SetCurrent();
                GmshWrapperGeneral.OptionSetNumber("Geometry.NumSubEdges", Convert.ToDouble(value));
            }
        }
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public GeometryObject() 
        {
            GmshWrapperConnection.Initialize();
            Id = GmshWrapperGeneral.GenerateRandom();
            GmshWrapperModel.ModelAdd(Id.ToString());
        }
        /// <summary>
        /// Конструктор с загрузкой файла геометрии
        /// </summary>
        /// <param name="path">[In]Путь до файла</param>
        public GeometryObject(string path)
        {
            GmshWrapperConnection.Initialize();
            Id = GmshWrapperGeneral.GenerateRandom();
            GmshWrapperModel.ModelAdd(Id.ToString());
            Load(path);
        }
        /// <summary>
        /// Загрузить геометрию из файла
        /// </summary>
        /// <param name="filePath">[In]Путь до файла</param>
        public void Load(string filePath)
        {
            SetCurrent();
            GmshWrapperIO.Merge(filePath);
            GmshWrapperIO.ModelSetFileName(filePath);
        }
        /// <summary>
        /// Перегененировать сетку
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        public void Generate(int dim)
        {
            SetCurrent();
            GmshWrapperMesh.Generate(dim);
        }
        /// <summary>
        /// Удалить геометрическую сущность
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <param name="entityId">[Out]Идентификатор</param>
        public void DeleteEntity(int dim, int entityId)
        {
            SetCurrent();
            GmshWrapperGeometry.RemoveEntities(new int[] { dim, entityId });
        }
        /// <summary>
        /// Возвращает контрольные точки
        /// </summary>
        /// <returns>узлы</returns>
        public IEnumerable<Node> GetControlPoints()
        {
            SetCurrent();
            return GmshWrapperGeometry.GetEntities<Node>(0, GmshWrapperGeometry.CreateControlPoint);
        }
        /// <summary>
        /// Возвращает кривые
        /// </summary>
        /// <returns>кривые</returns>
        public IEnumerable<Curve> GetСurves()
        {
            SetCurrent();
            return GmshWrapperGeometry.GetEntities<Curve>(1, GmshWrapperGeometry.CreateCurve);
        }
        /// <summary>
        /// Возвращает линии
        /// </summary>
        /// <returns>линии</returns>
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

        /// <summary>
        /// Удаляет модель
        /// </summary>
        virtual public void Dispose()
        {
            idList.Remove(Id);
            GmshWrapperModel.ModelRemoveByName(Id.ToString());
        }
        /// <summary>
        /// Устанавливает модель текущей
        /// </summary>
        protected void SetCurrent() => GmshWrapperModel.ModelSetCurrent(Id.ToString());
    }
}