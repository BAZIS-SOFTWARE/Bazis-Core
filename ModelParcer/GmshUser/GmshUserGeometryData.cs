using Model;
using ModelParcer.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer.GmshUser
{
    public class GmshUserGeometryData : GmshUserModelData
    {
        /// <summary>
        /// Экземляр данных о геометрии заданной модели
        /// </summary>
        /// <param name="modelId"></param>
        public GmshUserGeometryData(long modelId) : base(modelId) => GetElements();
        /// <summary>
        /// Извлекает данные геометрии из модели
        /// </summary>
        protected override void GetElements()
        {
            var error = 0;
            IntPtr dimTags = IntPtr.Zero;
            IntPtr dimTags_n = IntPtr.Zero;
            GmshGeometry.gmshModelOccGetEntities(ref dimTags, ref dimTags_n, -1, ref error);
            int[] data;
            GmshUserMemory.GetTypesData(dimTags,out data, dimTags_n);

        }
    }
}
