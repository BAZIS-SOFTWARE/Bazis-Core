using ModelInterfaces;
using ModelController.ModelScenePresentator.Interfaces;

namespace ModelController.ModelScenePresentator.GlObjsVertexCreators
{
    public class NormalsSurfaceObjsCreator : ISurfaceObjsVertexCreator
    {
        public void Create(ref int stride, float[] vertexAtributes, ISurfaceObject surfaceElement)
        {
            if (surfaceElement.ViewState)
            {
                //var centr = surfaceElement.CalcCentralPoint();
                foreach (var surface in surfaceElement.GetSurface())
                {
                    if (surface.ViewState)
                    {
                        var normal = surface.CalcNormal();
                        //var plane = new Plane(surface[0].Position, normal);
                        //var proj = centr.GetPointProectionOnPlane(plane);
                        //normal = proj.Sub(centr);
                        for (int k = 0; k < surface.NumberOfPoints - 2; k++)
                        {
                            vertexAtributes[stride + 9 * k + 0] = normal._x;
                            vertexAtributes[stride + 9 * k + 1] = normal._y;
                            vertexAtributes[stride + 9 * k + 2] = normal._z;

                            vertexAtributes[stride + 9 * k + 3] = normal._x;
                            vertexAtributes[stride + 9 * k + 4] = normal._y;
                            vertexAtributes[stride + 9 * k + 5] = normal._z;

                            vertexAtributes[stride + 9 * k + 6] = normal._x;
                            vertexAtributes[stride + 9 * k + 7] = normal._y;
                            vertexAtributes[stride + 9 * k + 8] = normal._z;
                        }

                        stride += (surface.NumberOfPoints - 2) * 3 * 3; // (кол-во точек - 2 ) * 3 атрибута и * 3 треугольника

                    }
                }
            }
        }
    }
}