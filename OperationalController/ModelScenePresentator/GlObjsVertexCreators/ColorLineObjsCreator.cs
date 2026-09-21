using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class ColorLineObjsCreator<T> : ILineObjsVertexCreator<T>
        where T : IPoint
    {
        public void Create(ref int stride, float[] vertexAtributes, IVertexObject<T> nodeObject, IModelView modelView, Color? setColor)
        {
            var color = GetColor(nodeObject, modelView, setColor);

            for (int k = 0; k < nodeObject.NumberOfPoints; k++)
            {
                vertexAtributes[stride + 4 * k + 0] = color.R / 255.0f;
                vertexAtributes[stride + 4 * k + 1] = color.G / 255.0f;
                vertexAtributes[stride + 4 * k + 2] = color.B / 255.0f;
                vertexAtributes[stride + 4 * k + 3] = color.A / 255.0f;
            }
            stride += nodeObject.NumberOfPoints * 4;
        }

        /// <summary>
        /// Возвращает цвет объекта без поиска набора в горячем цикле.
        /// </summary>
        private Color GetColor(IVertexObject<T> nodeObject, IModelView modelView, Color? setColor)
        {
            if (!setColor.HasValue)
                return modelView.GetColor(nodeObject.ObjType, nodeObject.Number);

            var color = modelView.IsSelected(nodeObject.ObjType, nodeObject.Number)
                ? modelView.SelectionColor
                : setColor.Value;
            return modelView.GetDisplayColor(color);
        }
    }
}
