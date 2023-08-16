using GmshApi.Wrapper;
using System;
using GmshApi.Api;

namespace GmshApi
{
    public class MeshField : IMeshField, IDisposable
    {
        private int tag;
        /// <summary>
        /// Идентификатор поля
        /// </summary>
        public int Tag { get => tag; }

        /// <summary>
        /// Возвращает тип поля
        /// </summary>
        public MeshFieldType Type 
        {
            get => (MeshFieldType)Enum.Parse(typeof(MeshFieldType), GmshWrapperMeshField.GetType(Tag));
        }

        /// <summary>
        /// Устанавливает значение текущей опции
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public void SetOptionValue(string option, double value) => GmshWrapperMeshField.SetNumber(Tag, option, value);
        /// <summary>
        /// Возвращает значение текущей опции
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public double GetOptionValue(string option) => GmshWrapperMeshField.GetNumber(Tag, option);
        /// <summary>
        /// Конструктор с установкой типа поля
        /// </summary>
        /// <param name="type"></param>
        public MeshField(MeshFieldType type) 
        {
            tag = GmshWrapperMeshField.Add(type.ToString(), -1);
        }
        /// <summary>
        /// Установить значения опции
        /// </summary>
        /// <param name="option"></param>
        /// <param name="values"></param>
        public void SetOptionValues(string option, double[] values) => GmshWrapperMeshField.SetNumbers(Tag, option, values);
        /// <summary>
        /// Возвратить значения опции
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public double[] GetOptionValues(string option) => GmshWrapperMeshField.GetNumbers(Tag, option);
        /// <summary>
        /// Возвратить строку опции
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public string GetOptionString(string option) => GmshWrapperMeshField.GetString(Tag, option);
        /// <summary>
        /// Установить строку опции
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public void SetOptionString(string option, string value) => GmshWrapperMeshField.SetString(Tag, option, value);
        /// <summary>
        /// Установить тип отображени
        /// </summary>
        /// <param name="type"></param>
        public void SetDisplayFieldType(DisplayFieldType type)
        {
            if (type == DisplayFieldType.BoundaryLayer)
                GmshWrapperMeshField.SetAsBoundaryLayer(Tag);
            else
                GmshWrapperMeshField.SetAsBackgroundMesh(Tag);
        }
        /// <summary>
        /// Удалить поле
        /// </summary>
        public void Dispose() => GmshWrapperMeshField.Remove(Tag);
    }
}
