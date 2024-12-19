using ModelInterfaces;
using ModelInterfaces.MeshObjects;
using System.Collections.Generic;
using System.Drawing;

namespace ProjectInterfaces.Results
{
    public interface IResultsFieldsCreator
    {
        /// <summary>
        /// ScaleFactor
        /// </summary>
        int ScaleFactor { get; set; }
        /// <summary>
        /// SetScaleItems
        /// </summary>
        /// <param name="rangesColors"></param>
        /// <param name="rangesValues"></param>
        void SetScaleItems(Color[] rangesColors, ItemRange[] rangesValues);
        /// <summary>
        /// CreateSurfaceObjects
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objsType"></param>
        /// <param name="resDescr"></param>
        /// <param name="surfaceObjects"></param>
        /// <returns></returns>
        IEnumerable<IResultFigure2D> CreateSurfaceObjects(IResult result, ObjType objsType, string resDescr, IEnumerable<ISurfaceElement> surfaceObjects);
    }
}