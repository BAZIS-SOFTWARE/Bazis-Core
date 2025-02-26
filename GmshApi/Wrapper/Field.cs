using System;
using static GmshApi.GmshKernel;
using static GmshApi.Memory;

namespace GmshApi
{
    /// <inheritdoc/>
    public class Field
    {
        private delegate int gmshModelMeshFieldAddFun(string fieldType, int tag, ref int ierr);
        private gmshModelMeshFieldAddFun gmshModelMeshFieldAdd = GetFunction<gmshModelMeshFieldAddFun>("gmshModelMeshFieldAdd");
        
        private delegate void gmshModelMeshFieldRemoveFun(int tag, ref int ierr);
        private gmshModelMeshFieldRemoveFun gmshModelMeshFieldRemove = GetFunction<gmshModelMeshFieldRemoveFun>("gmshModelMeshFieldRemove");
        
        private delegate void gmshModelMeshFieldListFun(ref IntPtr tags, ref IntPtr tags_n, ref int ierr);
        private gmshModelMeshFieldListFun gmshModelMeshFieldList = GetFunction<gmshModelMeshFieldListFun>("gmshModelMeshFieldList");
        
        private delegate void gmshModelMeshFieldGetTypeFun(int tag, ref IntPtr fieldType, ref int ierr);
        private gmshModelMeshFieldGetTypeFun gmshModelMeshFieldGetType = GetFunction<gmshModelMeshFieldGetTypeFun>("gmshModelMeshFieldGetType");

        private delegate void gmshModelMeshFieldSetNumberFun(int tag, string option, double value, ref int ierr);
        private gmshModelMeshFieldSetNumberFun gmshModelMeshFieldSetNumber = GetFunction<gmshModelMeshFieldSetNumberFun>("gmshModelMeshFieldSetNumber");
        
        private delegate void gmshModelMeshFieldGetNumberFun(int tag, string option, ref double value, ref int ierr);
        private gmshModelMeshFieldGetNumberFun gmshModelMeshFieldGetNumber = GetFunction<gmshModelMeshFieldGetNumberFun>("gmshModelMeshFieldGetNumber");
        
        private delegate void gmshModelMeshFieldSetStringFun(int tag, string option, string value, ref int ierr);
        private gmshModelMeshFieldSetStringFun gmshModelMeshFieldSetString = GetFunction<gmshModelMeshFieldSetStringFun>("gmshModelMeshFieldSetString");
        
        private delegate void gmshModelMeshFieldGetStringFun(int tag, string option, ref IntPtr value, ref int ierr);
        private gmshModelMeshFieldGetStringFun gmshModelMeshFieldGetString = GetFunction<gmshModelMeshFieldGetStringFun>("gmshModelMeshFieldGetString");
        
        private delegate void gmshModelMeshFieldSetNumbersFun(int tag, string option, double[] values, IntPtr values_n, ref int ierr);
        private gmshModelMeshFieldSetNumbersFun gmshModelMeshFieldSetNumbers = GetFunction<gmshModelMeshFieldSetNumbersFun>("gmshModelMeshFieldSetNumbers");
        
        private delegate void gmshModelMeshFieldGetNumbersFun(int tag, string option, ref IntPtr values, ref IntPtr values_n, ref int ierr);
        private gmshModelMeshFieldGetNumbersFun gmshModelMeshFieldGetNumbers = GetFunction<gmshModelMeshFieldGetNumbersFun>("gmshModelMeshFieldGetNumbers");
        
        private delegate void gmshModelMeshFieldSetAsBackgroundMeshFun(int tag, ref int ierr);
        private gmshModelMeshFieldSetAsBackgroundMeshFun gmshModelMeshFieldSetAsBackgroundMesh = GetFunction<gmshModelMeshFieldSetAsBackgroundMeshFun>("gmshModelMeshFieldSetAsBackgroundMesh");
        
        private delegate void gmshModelMeshFieldSetAsBoundaryLayerFun(int tag, ref int ierr);
        private gmshModelMeshFieldSetAsBoundaryLayerFun gmshModelMeshFieldSetAsBoundaryLayer = GetFunction<gmshModelMeshFieldSetAsBoundaryLayerFun>("gmshModelMeshFieldSetAsBoundaryLayer");

        /// <inheritdoc/>
        public int Add(FieldType fieldType, int tag = -1)
        {
            var ierr = 0;
            return gmshModelMeshFieldAdd(fieldType.ToString(), tag, ref ierr);
        }
        /// <inheritdoc/>
        public void Remove(int tag)
        {
            var ierr = 0;
            gmshModelMeshFieldRemove(tag, ref ierr);
        }
        /// <inheritdoc/>
        public int[] List()
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelMeshFieldList(ref intValue, ref intValue_n, ref ierr);
            return GetObjectsData<int>(intValue, intValue_n);
        }
        /// <inheritdoc/>
        public string GetType(int tag)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelMeshFieldGetType(tag, ref intValue, ref ierr);
            return Memory.GetString(intValue);
        }
        /// <inheritdoc/>
        public void SetNumber(int tag, string option, double value)
        {
            var ierr = 0;
            gmshModelMeshFieldSetNumber(tag, option, value, ref ierr);
        }
        /// <inheritdoc/>
        public double GetNumber(int tag, string option)
        {
            var ierr = 0;
            var value = 0.0;
            gmshModelMeshFieldGetNumber(tag, option, ref value, ref ierr);
            return value;
        }
        /// <inheritdoc/>
        public void SetString(int tag, string option, string value)
        {
            var ierr = 0;
            gmshModelMeshFieldSetString(tag, option, value, ref ierr);
        }
        /// <inheritdoc/>
        public string GetString(int tag, string option)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelMeshFieldGetString(tag, option, ref intValue, ref ierr);
            return Memory.GetString(intValue);
        }
        /// <inheritdoc/>
        public void SetNumbers(int tag, string option, double[] values)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            gmshModelMeshFieldSetNumbers(tag, option, values, (IntPtr)values.Length, ref ierr);
        }
        /// <inheritdoc/>
        public double[] GetNumbers(int tag, string option)
        {
            var ierr = 0;
            var intValue = IntPtr.Zero;
            var intValue_n = IntPtr.Zero;
            gmshModelMeshFieldGetNumbers(tag, option, ref intValue, ref intValue_n, ref ierr);
            return GetObjectsData<double>(intValue, intValue_n);
        }
        /// <inheritdoc/>
        public void SetAsBoundaryLayer(int tag)
        {
            var ierr = 0;
            gmshModelMeshFieldSetAsBoundaryLayer(tag, ref ierr);
        }
        /// <inheritdoc/>
        public void SetAsBackgroundMesh(int tag)
        {
            var ierr = 0;
            gmshModelMeshFieldSetAsBackgroundMesh(tag, ref ierr);
        }
    }
}
