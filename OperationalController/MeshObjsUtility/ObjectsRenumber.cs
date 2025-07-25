using Model.MeshObjects;
using OperationalController;
using Model.Interfaces;
using System;
using System.Collections.Generic;

namespace OperationalController.MeshObjsUtility
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
                        foreach (var node in objectsData.NodesSet.Values)
                            node.SetNumber(number++);
                    }
                    break;
                //case ObjType.Элемент:
                //    {
                //        foreach (var elem in objectsData.GetAllElements())
                //            elem.SetNumber(number++);
                //    }
                //    break;


                case ObjType.Поверхность:
                case ObjType.Точка:
                    throw new Exception("Данный метод не предназначени для работы с геометрией");

                default:
                    throw new NotImplementedException("Перенумерация данного конкретного типа не реализована");
            }
        }
    }
}