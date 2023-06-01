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

        public GMSHConnector(string srcPath)
        {
            GMSHServer.Init();
            var error = 0;
            Tag = GMSHServer.Tag++;
            GMSHServer.gmshModelAdd(Tag.ToString(), ref error);
            GMSHServer.gmshMerge(srcPath, ref error);
            FileName = srcPath;
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

        private string GetDstFileName()
        {
            var file = Path.GetFileNameWithoutExtension(FileName);
            var ext = Path.GetExtension(FileName);
            var folder = Path.GetDirectoryName(FileName);
            return folder + @"\" + file + ".inp";
        }
    }
}
