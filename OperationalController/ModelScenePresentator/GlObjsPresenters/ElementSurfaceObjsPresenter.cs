using Model.Interfaces;
using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OperationalController.ModelScenePresentator.GlObjsPresenters
{
    /// <summary>
    /// Собирает грани объёмных элементов с учётом видимости соседей в представлении.
    /// </summary>
    public class ElementSurfaceObjsPresenter : SurfaceObjsPresenter<ElementSurface, Node>
    {
        private readonly IModelView modelView;

        /// <summary>
        /// Создаёт презентатор граней объёмных элементов.
        /// </summary>
        internal ElementSurfaceObjsPresenter(IReadOnlyList<ISurfaceObject<ElementSurface, Node>> elements, IReadOnlyList<Color> colors, IModelView modelView)
            : base(elements, colors, !(modelView ?? throw new ArgumentNullException(nameof(modelView))).HideInsideSurfaces)
        {
            this.modelView = modelView;
        }

        /// <summary>
        /// Отбирает внешние грани и грани, открытые скрытием соседнего элемента.
        /// </summary>
        protected override IEnumerable<ElementSurface> GetDrawnSurfaces(int index)
        {
            foreach (var surface in Objects[index].GetSurfaces())
            {
                if (drawInnerSurfaces || !surface.IsInner)
                {
                    yield return surface;
                    continue;
                }

                foreach (var element in surface.GetElements())
                {
                    if (element.Number == surface.ElementNumber)
                        continue;

                    if (!modelView.GetVisible(element.ObjType, element.Number))
                        yield return surface;

                    break;
                }
            }
        }
    }
}
