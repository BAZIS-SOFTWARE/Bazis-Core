using Geometry;
using Model.GeometryObjects;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using Model.Interfaces.ObjectsCollections;
using Model.MeshObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Model.ObjectsCollections
{
    public class SurfacesSet : ObjectsSet<SurfaceFigure>
    {
        public SurfacesSet(string name) : base(name)
        {
        }


        /// <inheritdoc/>

        public override void SetBackColor(int number)
        {
            var surf = objectsSet[number];

            if (surf.Volume != null)
                surf.Color = surf.Volume.Color;
            else
                surf.Color = Color;
        }
/// <inheritdoc/>

        public override void SetBackColor()
        {
            foreach (var item in objectsSet)
                SetBackColor(item.Key);
        }
    }
}
