using Model;
using System.Collections.Generic;
using System;
using GmshApi.Api;
using Model.Interfaces;

namespace GmshApi
{  
    /// <summary>
    /// Класс для построения геометрических сущностей
    /// </summary>
    public class GeometryObject : IGeometryApi, IDisposable
    {
        private static List<ulong> idList = new List<ulong>();
        private List<MeshField> fieldList = new List<MeshField>();
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
            GmshWrapperIO.Load(filePath);
            GmshWrapperIO.SetCurrentModelFileName(filePath);
        }
        /// <summary>
        /// Перегененировать сетку
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        public MeshObject Generate(int dim)
        {
            SetCurrent();
            if(dim != 0)
                GmshWrapperMesh.Generate(dim);
            return new MeshObject(Id);
        }
        /// <summary>
        /// Возвращает контрольные точки
        /// </summary>
        /// <returns>узлы</returns>
        public List<ModelObject> GetControlPoints()
        {
            SetCurrent();
            return GmshWrapperGeometry.GetEntities(ObjKind.Point);
        }
        /// <summary>
        /// Возвращает кривые
        /// </summary>
        /// <returns>кривые</returns>
        public List<ModelObject> GetСurves()
        {
            SetCurrent();
            return GmshWrapperGeometry.GetEntities(ObjKind.Curve);
        }
        /// <summary>
        /// Возвращает линии
        /// </summary>
        /// <returns>линии</returns>
        public List<ModelObject> GetElements1D()
        {
            SetCurrent();
            return GmshWrapperGeometry.GetEntities(ObjKind.Beam);
        }
        /// <summary>
        /// Удаляет сущности
        /// </summary>
        /// <param name="dimTags">[In]Массив пар dim:tag</param>
        public void DeleteEntities(int[] dimTags)
        {
            SetCurrent();
            GmshWrapperGeometry.RemoveEntitiesEx(dimTags, false);
        }
        /// <summary>
        /// Возвращает максимальную размерность
        /// </summary>
        /// <returns></returns>
        public int GetMaxDimension()
        {
            SetCurrent();
            return GmshWrapperGeometry.GetDimension();
        }

        /// <summary>
        /// Возвращает сущности текущей размерности
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <returns></returns>
        public int[] GetEntities(int dim)
        {
            SetCurrent();
            return GmshWrapperGeometry.GetEntitiesTags(dim);
        }
        /// <summary>
        /// Синхронизирует модель с GmshApi
        /// </summary>
        public void Synchronize()
        {
            SetCurrent();
            GmshWrapperModel.Synchronize();
        }
        /// <summary>
        /// Добавляет поле в список
        /// </summary>
        /// <param name="field"></param>
        public void AddField(MeshField field)
        {
            SetCurrent();
            if(fieldList.Contains(field))
                fieldList.Add(field);
        }
        /// <summary>
        /// Удаляет поле из списка
        /// </summary>
        /// <param name="id"></param>
        public void RemoveField(int id)
        {
            SetCurrent();
            var field = fieldList.Find(v => v.Tag == id);
            field?.Dispose();
            fieldList.Remove(field);
        }
        /// <summary>
        /// Удаляет поле из списка
        /// </summary>
        /// <param name="field"></param>
        public void RemoveField(MeshField field)
        {
            SetCurrent();
            field?.Dispose();
            fieldList.Remove(field);
        }

        /// <summary>
        /// Удаляет модель
        /// </summary>
        virtual public void Dispose()
        {
            SetCurrent();
            fieldList.ForEach(v => v.Dispose());
            idList.Remove(Id);
            GmshWrapperModel.ModelRemoveByName(Id.ToString());
        }
        /// <summary>
        /// Устанавливает модель текущей
        /// </summary>
        protected void SetCurrent() => GmshWrapperModel.ModelSetCurrent(Id.ToString());
    }
}