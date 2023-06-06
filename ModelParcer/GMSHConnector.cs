using Model;
using Model.Interfaces;
using PrFunctionLib;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ModelParcer
{
    public static class GMSHServer
    {
        private static bool isInit = false;
        private static int globalTags = 1;
        /// <summary>
        /// Return current Tag index
        /// </summary>
        public static ref int Tag { get { return ref globalTags; } }
        public static ref bool IsInit { get { return ref isInit; } }

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshInitialize(int argc, ref String argv, int readConfigFile,
                                                  int run, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void gmshFinalize(ref int error);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOpen(String src, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshMerge(String src, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshWrite(String src, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int gmshClear(ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGenerate(int dim, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshRecombine(ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshRefine(ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOptionSetNumber(String src, double value, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOptionGetNumber(String src, ref double value, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshOptionSetString(String name, String value, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelAdd(String src, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelSetCurrent(String src, ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelRemove(ref int ierr);

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelGetEntities(ref IntPtr dimTags, ref UIntPtr dimTags_n, 
                                                       int dim, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshFree(IntPtr p);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGetElements(ref IntPtr elementTypes, ref IntPtr elementTypes_n,
                                                           ref IntPtr elementTags, ref IntPtr elementTags_n,
                                                           ref IntPtr elementTags_nn, ref IntPtr nodeTags,
                                                           ref IntPtr nodeTags_n, ref IntPtr nodeTags_nn,
                                                           int dim, int tag, ref int ierr);
        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGetNodes(ref IntPtr nodeTags, ref IntPtr nodeTags_n,
                                    ref IntPtr coord, ref IntPtr coord_n,
                                    ref IntPtr parametricCoord, ref IntPtr parametricCoord_n,
                                    int dim, int tag, int includeBoundary, int returnParametricCoord,
                                    ref int ierr);

        /*gmshModelMeshGetNodes(size_t** nodeTags, size_t* nodeTags_n,
                                    double** coord, size_t* coord_n,
                                    double** parametricCoord, size_t* parametricCoord_n,
                                    const int dim,
                                    const int tag,
                                    const int includeBoundary,
                                    const int returnParametricCoord,
                                    int* ierr);*/

        [DllImport(".\\gmsh-4.11.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void gmshModelMeshGetElementProperties(int elementType, ref IntPtr elementName,
                                                                    ref int dim, ref int order, ref int numNodes,
                                                                    ref IntPtr localeNodeCoord,
                                                                    ref IntPtr localeNodeCoord_n, ref int numPrimaryNodes,
                                                                    ref int ierr);

        /*void gmshModelMeshGetElementProperties(const int elementType,
                                                char** elementName,
                                                int* dim,
                                                int* order,
                                                int* numNodes,
                                                double** localNodeCoord, size_t * localNodeCoord_n,
                                                int* numPrimaryNodes,
                                                int* ierr);*/


        public static void Init()
        {
            if (!IsInit)
            {
                int error = 0;
                var str = String.Empty;
                gmshInitialize(0, ref str, 0, 0, ref error);
                isInit = true;
            }
        }
        public static void Finalize()
        {
            if (IsInit)
            {
                IsInit = false;
                var error = 0;
                globalTags = 1;
                gmshClear(ref error);
                gmshFinalize(ref error);
            }
        }
    }



    public class GMSHConnector : IDisposable
    {
        /// <summary>
        /// Get/Set unical id
        /// </summary>
        public int Tag { get; private set; }
        /// <summary>
        /// Get/Set the deletion flag for the destination file
        /// </summary>
        public bool DeleteDstFile { get; set; }
        /// <summary>
        /// Get/Set FileName associated with current mesh
        /// </summary>
        public string FileName{ get; private set; }

        /// <summary>
        /// Get/Set mesh density (before Generate)
        /// </summary>
        public string MeshDensity 
        {
            get 
            {
                var error = 0;
                GMSHServer.gmshModelSetCurrent(Tag.ToString(), ref error);
                double min = 0, max = 0;
                GMSHServer.gmshOptionGetNumber("Mesh.MeshSizeMin", ref min, ref error);
                GMSHServer.gmshOptionGetNumber("Mesh.MeshSizeMax", ref max, ref error);
                return min.ToString(CultureInfo.InvariantCulture) + " " 
                     + max.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                var error = 0;
                GMSHServer.gmshModelSetCurrent(Tag.ToString(), ref error);
                var values = value.Split(' ');
                var min = Double.Parse(values[0], CultureInfo.InvariantCulture);
                var max = Double.Parse(values[1], CultureInfo.InvariantCulture);
                GMSHServer.gmshOptionSetNumber("Mesh.MeshSizeMin", min, ref error);
                GMSHServer.gmshOptionSetNumber("Mesh.MeshSizeMax", max, ref error);
            }
        }
        public GMSHConnector(string srcPath)
        {
            GMSHServer.Init();
            var error = 0;
            Tag = GMSHServer.Tag++;
            GMSHServer.gmshModelAdd(Tag.ToString(), ref error);
            GMSHServer.gmshMerge(srcPath, ref error);
            FileName = srcPath;
        }

        /// Get ModelData (works only after Generate)
        public ModelData GetModelData()
        {
            var dstFile = GetDstFileName();
            var error = 0;
            GMSHServer.gmshModelSetCurrent(Tag.ToString(), ref error);
            GMSHServer.gmshWrite(dstFile, ref error);
            var data = new LoadModelFromGMSHTextFile();
            data.LoadEvent += (ar1, ar2) => { };
            var modelData = data.Load(dstFile);
            return modelData;
        }

        public void GetEntitiesTest()
        {
            var error = 0;
            IntPtr buffer = IntPtr.Zero;
            UIntPtr dimTags_n = UIntPtr.Zero;//Количество пар dimTags
            GMSHServer.gmshModelGetEntities(ref buffer, ref dimTags_n, -1, ref error);
            var count = dimTags_n.ToUInt32();
            var tagsArray = new int[count];
            Marshal.Copy(buffer, tagsArray, 0, (int)count);
            GMSHServer.gmshFree(buffer);
        }

        public void GetElements(out List<Node> nodes, out List<Element1D> e1d,
                                out List<Element2D> e2d, out List<Element3D> e3d)
        {
            var error = 0;
            SetDefaultLists(out nodes, out e1d, out e2d, out e3d);
            IntPtr eTypes, eTypes_n, eTags, eTags_n,eTags_nn, nodeTags, nodeTags_n, nodeTags_nn;
            eTypes = eTypes_n = eTags = eTags_n = eTags_nn = nodeTags = nodeTags_n = nodeTags_nn = IntPtr.Zero;
            GMSHServer.gmshModelMeshGetElements(ref eTypes, ref eTypes_n,
                                                ref eTags, ref eTags_n, ref eTags_nn,
                                                ref nodeTags, ref nodeTags_n, ref nodeTags_nn, -1, -1, ref error);
            long[][] elements;
            long[][] points;
            int[] eMeta;
            GetTypesData(out eMeta, eTypes, eTypes_n);
            GetElementData(out elements, eTags, eTags_n, eTags_nn);
            GetElementData(out points, nodeTags, nodeTags_n, nodeTags_nn);

            for(var i = 0; i < eMeta.Length; ++i)
            {
                var elName = IntPtr.Zero;
                var localeNodeCoord = IntPtr.Zero;
                var localeNodeCoord_n = IntPtr.Zero;
                int dim, order, numNodes, primaryNodes;
                dim = order = numNodes = primaryNodes = 0;
                GMSHServer.gmshModelMeshGetElementProperties(eMeta[i], ref elName, ref dim, ref order, ref numNodes,
                                                             ref localeNodeCoord, ref localeNodeCoord_n,
                                                             ref primaryNodes, ref error);
                //Marshal.

            }
            







            /*GMSHServer.gmshModelMeshGetNodes(ref nodeTags, ref nodeTags_n, ref eTags, ref eTags_n,
                                             ref eTypes, ref eTypes_n, -1, -1, 0, 0, ref error);*/

            //Сделаем возврат по GetNodesByElementType;


            /*Координаты не верны
            var nodesTags = nodeTags_n.ToInt32();
            var pts = new long[nodesTags];
            Marshal.Copy(nodeTags, pts, 0, nodesTags);
            GMSHServer.gmshFree(nodeTags);

            var coordsCount = eTags_n.ToInt32();
            var coords = new double[coordsCount];
            Marshal.Copy(nodeTags, coords, 0, coordsCount);
            GMSHServer.gmshFree(eTags);
            GMSHServer.gmshFree(eTypes);*/
        }

        /// <summary>
        /// Create ModelData from script file
        /// </summary>
        /// <param name="srcPath"></param>
        /// <param name="objTypeCreate"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        /// 
        public void Generate(string objType)
        {
            if (GMSHServer.IsInit)
            {
                var dim = new Dictionary<string, int>() { { "Узлы", 0 }, { "Линии", 1 }, { "Элементы1D", 1 },
                                                          { "Элементы2D", 2 }, { "Элементы3D", 3 } };
                var error = 0;
                var dstFile = GetDstFileName();
                GMSHServer.gmshModelSetCurrent(Tag.ToString(), ref error);
                GMSHServer.gmshOptionSetNumber("Mesh.SaveAll", 1, ref error);
                GMSHServer.gmshOptionSetNumber("Mesh.SaveGroupsOfNodes", -1111, ref error);
                GMSHServer.gmshModelMeshGenerate(dim[objType], ref error);
            }
        }

        /// <summary>
        /// Quadrangulate geometry (works only after generate method)
        /// </summary>
        public void Quadrangulate()
        {
            var error = 0;
            GMSHServer.gmshModelSetCurrent(Tag.ToString(), ref error);
            GMSHServer.gmshModelMeshRecombine(ref error);
        }

        /// <summary>
        /// Tesselate (works only after generate method)
        /// </summary>
        public void Tesselate()
        {
            var error = 0;
            GMSHServer.gmshModelSetCurrent(Tag.ToString(), ref error);
            GMSHServer.gmshModelMeshRefine(ref error);
        }

        /// <summary>
        /// Remove model from gmsh API
        /// </summary>
        public void Dispose()
        {
            if(GMSHServer.IsInit)
            {
                var error = 0;
                if (DeleteDstFile)
                    File.Delete(GetDstFileName());
                GMSHServer.gmshModelSetCurrent(Tag.ToString(), ref error);
                GMSHServer.gmshModelRemove(ref error);
            }
        }

        private void SetDefaultLists(out List<Node> nodes, out List<Element1D> e1D, out List<Element2D> e2D,
                                     out List<Element3D> e3D)
        {
            nodes = null;
            e1D = null;
            e2D = null;
            e3D = null;
        }

        private void GetTypesData(out int[] data, IntPtr eTypes, IntPtr eCount, int sizeFactor = 1)
        {
            var count = eCount.ToInt32();
            data = new int[count * sizeFactor];
            Marshal.Copy(eTypes, data, 0, count * sizeFactor);
            GMSHServer.gmshFree(eTypes);
        }

        private void GetElementData(out long[][] data, IntPtr tagsData, IntPtr lengthPerDim, IntPtr dimCount)
        {
            var dCount = dimCount.ToInt32();
            data = new long[dCount][];
            var tagsDataPtr = new IntPtr[dCount];
            var lengthData = new IntPtr[dCount];
            Marshal.Copy(tagsData, tagsDataPtr, 0, dCount);
            Marshal.Copy(lengthPerDim, lengthData, 0, dCount);
            GMSHServer.gmshFree(lengthPerDim);
            for (var i = 0; i < dCount; ++i)
            {
                var tags = lengthData[i].ToInt32();
                data[i] = new long[tags];
                Marshal.Copy(tagsDataPtr[i], data[i], 0, tags);
                GMSHServer.gmshFree(tagsDataPtr[i]);
            }
            GMSHServer.gmshFree(tagsData);
        }

        private string GetDstFileName()
        {
            var file = Path.GetFileNameWithoutExtension(FileName);
            var ext = Path.GetExtension(FileName);
            var folder = Path.GetDirectoryName(FileName);
            return folder + @"\" + file + ".inp";
        }
    }
}
