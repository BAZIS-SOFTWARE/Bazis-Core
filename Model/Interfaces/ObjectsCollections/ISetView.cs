using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Model.Interfaces.ObjectsCollections
{

    /// <summary>
    /// ViewMode
    /// </summary>
    public enum ViewMode
    {
        /// <summary>
        /// Point
        /// </summary>
        Point,
        /// <summary>
        /// Line
        /// </summary>
        Line,
        /// <summary>
        /// Surface
        /// </summary>
        Surface,
        /// <summary>
        /// LineSurface
        /// </summary>
        LineSurface
    }
    /// <summary>
    /// IObjectsView
    /// </summary>
    public interface ISetView
    {
        /// <summary>
        /// Color
        /// </summary>
        Color Color { get; }
        /// <summary>
        /// ViewMode
        /// </summary>
        ViewMode ViewMode { get; }
        /// <summary>
        /// SetColor
        /// </summary>
        /// <param name="color"></param>
        void SetColor(Color color);
        /// <summary>
        /// SetColor
        /// </summary>
        /// <param name="color"></param>
        /// <param name="number"></param>
        void SetColor(Color color, int number);
        /// <summary>
        /// SetBackColor
        /// </summary>
        /// <param name="number"></param>
        void SetBackColor(int number);
        /// <summary>
        /// SetBackColor
        /// </summary>
        void SetBackColor();
        /// <summary>
        /// SetViewMode
        /// </summary>
        /// <param name="viewMode"></param>
        void SetViewMode(ViewMode viewMode);
        /// <summary>
        /// GetViewState
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        bool GetViewState(int number);
    }
}
