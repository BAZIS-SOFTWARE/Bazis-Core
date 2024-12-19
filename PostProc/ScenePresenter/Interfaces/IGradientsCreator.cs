using Geometry;
using Project.Interfaces.Results;
using System.Collections.Generic;
using System.Drawing;

namespace PostProc.ScenePresenter.Interfaces
{
    /// <summary>
    /// IGradientsCreator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IGradientsCreator<T>
    {
        /// <summary>
        /// CreateGradientObjects
        /// </summary>
        /// <param name="itemsValues"></param>
        /// <param name="itemsColors"></param>
        /// <param name="nodesCoords"></param>
        /// <param name="nodesResults"></param>
        /// <returns></returns>
        IEnumerable<T> CreateGradientObjects(ItemRange[] itemsValues, Color[] itemsColors, List<Point3D> nodesCoords, List<float> nodesResults);
    }
}
