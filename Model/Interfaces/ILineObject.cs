using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Interfaces
{
    public interface ILineObject<T> : IModelObject,IVertexObject<T> where T : IPoint
    {
        /// <summary>
        /// CalcLength
        /// </summary>
        /// <returns></returns>
        double CalcLength();
    }
}
