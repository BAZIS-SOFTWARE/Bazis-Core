using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ModelController.MeshObjsUtility
{
    public class ChangeInsideSurface
    {
        /// <inheritdoc/>

        public void HideInsideSurfaces<T,U>(IEnumerable<ISurfaceObject<T, U>> surfaceObjects)
            where T : ISurface<U> where U : IPoint
        {
            var surfDict = new Dictionary<string, List<ISurface<U>>>();

            foreach (var element in surfaceObjects)
            {
                if (element.ViewState & element.NumberOfSides > 1)
                    foreach (var surface in element.GetSurfaces())
                    {
                        var numbers = surface.GetVertexes().Select(x => x.Number).ToArray();

                        Array.Sort(numbers);
                        var surfInd = string.Join("", numbers);   //numbers[0].ToString() + numbers[1].ToString() + numbers[2].ToString();

                        if (!surfDict.ContainsKey(surfInd))
                        {
                            surface.ViewState = true;
                            surfDict.Add(surfInd, new List<ISurface<U>>() { surface });
                        }

                        else
                        {
                            surface.ViewState = false;
                            surfDict[surfInd][0].ViewState = false;
                            surfDict[surfInd].Add(surface);
                        }
                    }
            }

        }

        /// <inheritdoc/>

        public void ShowInsideSurfaces<T, U>(IEnumerable<ISurfaceObject<T, U>> surfaceObjects)
            where T : ISurface<U> where U : IPoint
        {
            foreach (var element in surfaceObjects)
            {
                if (element.ViewState)
                    foreach (var surface in element.GetSurfaces())
                        surface.ViewState = true;
            }
        }
    }
}
