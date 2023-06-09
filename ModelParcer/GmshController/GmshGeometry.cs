using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer.GmshController
{
    /// <summary>
    /// Класс с группой методов для работы с геометрией
    /// </summary>
    public static class GmshGeometry
    {
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGenerate(int dim, ref int ierr);
    }
}
