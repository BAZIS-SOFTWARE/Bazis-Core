using Model.MeshObjects;
using ModelControllerInterfaces;
using Model.Interfaces;
using System;
using System.Collections.Generic;

namespace ModelController.MeshObjsUtility
{
    /// <summary>
    /// ObjectsRenumber
    /// </summary>
    public class ObjectsRenumber : IObjectsRenumber
    {
        /// <inheritdoc/>
        public void Renumber(IObjectsData objectsData, ObjType objType)
        {
            var number = 0;
            switch (objType)
            {
                case ObjType.Узел:
                    {
                        foreach (var node in objectsData.NodeCollection.GetObjects())
                            node.SetNumber(number++);
                    }
                    break;
                case ObjType.Элемент:
                    {
                        foreach (var elem in objectsData.GetAllElements())
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