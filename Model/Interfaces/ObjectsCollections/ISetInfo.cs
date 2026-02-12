using Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Model.Interfaces.ObjectsCollections
{
    /// <summary>
    /// ISetInfo
    /// </summary>
    public interface ISetInfo : ISetView
    {
        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Number
        /// </summary>
        int Number { get; set; }
        /// <summary>
        /// ObjType
        /// </summary>
        ObjType ObjType { get; }
        /// <summary>
        /// Count
        /// </summary>
        int NumberOfObjects { get; }
        /// <summary>
        /// GetObjectInfo
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        string GetObjectInfo(int number);
        /// <summary>
        /// GetObjectsInfo
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetObjectsInfo();
        /// <summary>
        /// GetCoords
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        IEnumerable<Point3D> GetCoords(int number);
        /// <summary>
        /// GetCentr
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        Point3D GetCentr(int number);
        /// <summary>
        /// GetNumbers
        /// </summary>
        /// <returns></returns>
        IEnumerable<int> GetNumbers();
    }
}
