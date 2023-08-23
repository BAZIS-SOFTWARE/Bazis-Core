using Model;
using System.Collections.Generic;
using System;
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
                    value = GenerateRandom();
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
                return Convert.ToInt32(GmshWrapperGeneral.OptionGetNumber("Geometry.NumSubEdges")); 
            }
            set
            {
                GmshWrapperGeneral.OptionSetNumber("Geometry.NumSubEdges", Convert.ToDouble(value));
            }
        }
        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public GeometryObject() 
        {
            GmshWrapperConnection.Initialize();
            Id = GenerateRandom();
            GmshWrapperModel.ModelAdd(Id.ToString());
        }
        /// <summary>
        /// Конструктор с загрузкой файла геометрии
        /// </summary>
        /// <param name="path">[In]Путь до файла</param>
        public GeometryObject(string path)
        {
            GmshWrapperConnection.Initialize();
            Id = GenerateRandom();
            GmshWrapperModel.ModelAdd(Id.ToString());
            Load(path);
        }
        /// <summary>
        /// Загрузить геометрию из файла
        /// </summary>
        /// <param name="filePath">[In]Путь до файла</param>
        public void Load(string filePath)
        {
            GmshWrapperIO.Load(filePath);
        }
        /// <summary>
        /// Перегененировать сетку
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        public MeshObject Generate(int dim)
        {
            if (dim != 0)
                GmshWrapperMesh.Generate(dim);
            return new MeshObject(Id);
        }
        /// <summary>
        /// Возвращает контрольные точки
        /// </summary>
        /// <returns>узлы</returns>
        public List<ModelObject> GetControlPoints()
        {
            return GmshWrapperGeometry.GetEntities(ObjKind.Point);
        }
        /// <summary>
        /// Возвращает кривые
        /// </summary>
        /// <returns>кривые</returns>
        public List<ModelObject> GetСurves()
        {
            return GmshWrapperGeometry.GetEntities(ObjKind.Curve);
        }
        /// <summary>
        /// Возвращает линии
        /// </summary>
        /// <returns>линии</returns>
        public List<ModelObject> GetElements1D()
        {

            return GmshWrapperGeometry.GetEntities(ObjKind.Beam);
        }
        /// <summary>
        /// Удаляет сущности
        /// </summary>
        /// <param name="dimTags">[In]Массив пар dim:tag</param>
        public void DeleteEntities(int[] dimTags)
        {
            GmshWrapperGeometry.RemoveEntitiesEx(dimTags, false);
        }
        /// <summary>
        /// Возвращает максимальную размерность
        /// </summary>
        /// <returns></returns>
        public int GetMaxDimension()
        {
            return GmshWrapperGeometry.GetDimension();
        }

        /// <summary>
        /// Возвращает сущности текущей размерности
        /// </summary>
        /// <param name="dim">[In]Размерность</param>
        /// <returns></returns>
        public int[] GetEntities(int dim)
        {
            return GmshWrapperGeometry.GetEntitiesTags(dim);
        }
        /// <summary>
        /// Синхронизирует модель с GmshApi
        /// </summary>
        public void Synchronize()
        {
            GmshWrapperModel.Synchronize();
        }
        /// <summary>
        /// Добавляет поле в список
        /// </summary>
        /// <param name="field"></param>
        public void AddField(MeshField field)
        {
            if(fieldList.Contains(field))
                fieldList.Add(field);
        }
        /// <summary>
        /// Удаляет поле из списка
        /// </summary>
        /// <param name="id"></param>
        public void RemoveField(int id)
        {
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
            field?.Dispose();
            fieldList.Remove(field);
        }

        /// <summary>
        /// Удаляет модель
        /// </summary>
        virtual public void Dispose()
        {
            fieldList.ForEach(v => v.Dispose());
            idList.Remove(Id);
            GmshWrapperModel.ModelRemoveByName(Id.ToString());
        }

        private ulong GenerateRandom()
        {
            var dif = ulong.MaxValue - 1UL;
            var rand = new Random();
            var value = rand.NextDouble();
            return Convert.ToUInt64(value * dif);
        }
    }
}