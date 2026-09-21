using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{

    public class ColorSurfaceObjsCreator<T,U> : ISurfaceObjsVertexCreator<T,U> 
        where T : ISurface<U> where U : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject<T,U> surfObject, IModelView modelView, Color? setColor)
        {
            var color = GetColor(surfObject, modelView, setColor);
            foreach (var surface in surfObject.GetSurfaces())
            {
                for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                {
                    vertexAtributes[stride + 12 * k + 0] = color.R / 255.0f;
                    vertexAtributes[stride + 12 * k + 1] = color.G / 255.0f;
                    vertexAtributes[stride + 12 * k + 2] = color.B / 255.0f;
                    vertexAtributes[stride + 12 * k + 3] = color.A / 255.0f;

                    vertexAtributes[stride + 12 * k + 4] = color.R / 255.0f;
                    vertexAtributes[stride + 12 * k + 5] = color.G / 255.0f;
                    vertexAtributes[stride + 12 * k + 6] = color.B / 255.0f;
                    vertexAtributes[stride + 12 * k + 7] = color.A / 255.0f;

                    vertexAtributes[stride + 12 * k + 8] = color.R / 255.0f;
                    vertexAtributes[stride + 12 * k + 9] = color.G / 255.0f;
                    vertexAtributes[stride + 12 * k + 10] = color.B / 255.0f;
                    vertexAtributes[stride + 12 * k + 11] = color.A / 255.0f;

                }

                stride += (surface.NumberOfPoints - 2) * 4 * 3; // (кол-во точек - 2 ) * 4 атрибута и * 3 треугольника

            }
         }

        /// <summary>
        /// Возвращает цвет объекта без поиска набора в горячем цикле.
        /// </summary>
        private Color GetColor(ISurfaceObject<T, U> surfaceObject, IModelView modelView, Color? setColor)
        {
            if (!setColor.HasValue)
                return modelView.GetColor(surfaceObject.ObjType, surfaceObject.Number);

            var color = modelView.IsSelected(surfaceObject.ObjType, surfaceObject.Number)
                ? modelView.SelectionColor
                : setColor.Value;
            return modelView.GetDisplayColor(color);
        }
    }
}
