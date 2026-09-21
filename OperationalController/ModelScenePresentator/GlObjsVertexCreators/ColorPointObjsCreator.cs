using Model.Interfaces;
using OperationalController.ModelScenePresentator.Interfaces;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsVertexCreators
{
    public class ColorPointObjsCreator : IPointObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, IModelObject pointObject, IModelView modelView, Color? setColor)
        {
                var color = GetColor(pointObject, modelView, setColor);

                vertexAtributes[stride + 0] = color.R / 255.0f;
                vertexAtributes[stride + 1] = color.G / 255.0f;
                vertexAtributes[stride + 2] = color.B / 255.0f;
                vertexAtributes[stride + 3] = color.A / 255.0f;

                stride += 4;
        }

        /// <summary>
        /// Возвращает цвет объекта без поиска набора в горячем цикле.
        /// </summary>
        private Color GetColor(IModelObject pointObject, IModelView modelView, Color? setColor)
        {
            if (!setColor.HasValue)
                return modelView.GetColor(pointObject.ObjType, pointObject.Number);

            var color = modelView.IsSelected(pointObject.ObjType, pointObject.Number)
                ? modelView.SelectionColor
                : setColor.Value;
            return modelView.GetDisplayColor(color);
        }
    }
}
