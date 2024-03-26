using ModelControllerInterfaces;
using ModelInterfaces;
using System;

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
                    break;

                case ObjType.Фигура:
                case ObjType.Фигура2D:
                case ObjType.Фигура3D:
                    throw new Exception("Данный метод не предназначени для работы с геометрией");

                default:
                    throw new NotImplementedException("Перенумерация данного конкретного типа не реализована");
            }
        }
    }
}