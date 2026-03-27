using OperationalController.GmshController;
using Project.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController.MeshObjsCreator
{
    public class Hole
    {/*
        public Hole(IGmshController controller, int surface, int[] hole) : base(controller, surface, hole)
        {
        }

        public string GetHoleSurface()
        {
            var surfaces = new List<int>();
            var sb = new StringBuilder();

            foreach(var curve in curves)
            {
                var surf = controller.Gmsh.Model.GetAdjacencies(1, curve.Tag).Item1
                                                .Where(v => v != Surface)
                                                .First();
                surfaces.Add(surf);
            }

            foreach (var id in surfaces.OrderBy(v => v))
            {
                sb.Append(id);
                sb.Append(' ');
            }

            return sb.ToString(0, sb.Length - 1);
        }
        */
    }
}
