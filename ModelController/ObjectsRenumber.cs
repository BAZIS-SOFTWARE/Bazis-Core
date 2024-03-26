using Model.MeshObjects;
using ModelControllerInterfaces;
using ModelInterfaces;
using System;
using System.Collections.Generic;

namespace ModelController
{
    /// <summary>
    /// ObjectsRenumber
    /// </summary>
    public class ObjectsRenumber : IObjectsRenumber
    {
        /// <inheritdoc/>
        public void IObjectsRenumber(IObjectsData objectsData, ObjType objType)
        {
            switch(objType)
            {
                case ObjType.Элемент:
                    {
                        var elements = new List<IModelObject>();
                        elements.AddRange(objectsData.GetAllElements());

                        var number = 1;
                        foreach (var elem in elements)
                            elem.SetNumber(number++);
                    }
                    break;

                case ObjType.Фигура:
                case ObjType.Фигура2D:
                case ObjType.Фигура3D:
                case ObjType.Точка:
                    throw new Exception("Данный метод не предназначени для работы с геометрией");

                default:
                    throw new NotImplementedException("Перенумерация данного конкретного типа не реализована");
            }
        }
    }
}