using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Project.Results;
using System.Collections.Generic;
using System.Drawing;

namespace PostProc.ScenePresenter.Interfaces
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
        void SetScaleItems(ItemRange[] rangesValues);
        /// <summary>
        /// CreateSurfaceObjects
        /// </summary>
        /// <param name="result"></param>
        /// <param name="objsType"></param>
        /// <param name="resDescr"></param>
        /// <param name="surfaceObjects"></param>
        /// <returns></returns>
        IEnumerable<ResultFigure> CreateSurfaceObjects(Result result, ObjType objsType, string resDescr, IEnumerable<ISurfaceElement> surfaceObjects);
    }
}