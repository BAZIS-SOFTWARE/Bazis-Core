using Geometry.Attribute;
using ModelInterfaces;

namespace ModelControllerInterfaces
{
    /// <summary>
    /// IObjectsRenumber
    /// </summary>
    public interface IObjectsRenumber
    {
        /// <summary>
        /// IObjectsRenumber. Перенумерация указанных объектов
        /// </summary>
        /// <param name="objectsData"></param>
        /// <param name="objType"></param>
        [Warning("Не использовать для фигур и точек!")]
        void IObjectsRenumber(IObjectsData objectsData, ObjType objType);
    }
}