using System;
using GmshApi.Api;
using GmshApi.GmshController;

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
            get => (MeshFieldType)Enum.Parse(typeof(MeshFieldType), ControllerMeshField.GetType(Tag));
        }

        /// <summary>
        /// Устанавливает значение текущей опции
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public void SetOptionValue(string option, double value) => ControllerMeshField.SetNumber(Tag, option, value);
        /// <summary>
        /// Возвращает значение текущей опции
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public double GetOptionValue(string option) => ControllerMeshField.GetNumber(Tag, option);
        /// <summary>
        /// Конструктор с установкой типа поля
        /// </summary>
        /// <param name="type"></param>
        public MeshField(MeshFieldType type) 
        {
            tag = ControllerMeshField.Add(type.ToString(), -1);
        }
        /// <summary>
        /// Установить значения опции
        /// </summary>
        /// <param name="option"></param>
        /// <param name="values"></param>
        public void SetOptionValues(string option, double[] values) => ControllerMeshField.SetNumbers(Tag, option, values);
        /// <summary>
        /// Возвратить значения опции
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public double[] GetOptionValues(string option) => ControllerMeshField.GetNumbers(Tag, option);
        /// <summary>
        /// Возвратить строку опции
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public string GetOptionString(string option) => ControllerMeshField.GetString(Tag, option);
        /// <summary>
        /// Установить строку опции
        /// </summary>
        /// <param name="option"></param>
        /// <param name="value"></param>
        public void SetOptionString(string option, string value) => ControllerMeshField.SetString(Tag, option, value);
        /// <summary>
        /// Установить тип отображени
        /// </summary>
        /// <param name="type"></param>
        public void SetDisplayFieldType(DisplayFieldType type)
        {
            if (type == DisplayFieldType.BoundaryLayer)
                ControllerMeshField.SetAsBoundaryLayer(Tag);
            else
                ControllerMeshField.SetAsBackgroundMesh(Tag);
        }
        /// <summary>
        /// Удалить поле
        /// </summary>
        public void Dispose() => ControllerMeshField.Remove(Tag);
    }
}
