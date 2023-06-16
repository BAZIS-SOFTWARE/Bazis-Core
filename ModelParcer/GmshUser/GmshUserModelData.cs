using Geometry;
using Model;
using ModelParcer.GmshController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelParcer.GmshUser
{
    /// <summary>
    /// Класс представляет данные модели
    /// </summary>
    public class GmshUserModelData
    {
        protected List<Node> nodes;
        protected List<Curve> curves;
        protected List<Surface> surfaces;
        /// <summary>
        /// Получает идентификатор с ассоциированной моделью
        /// </summary>
        protected long Id { get; private set; }
        /// <summary>
        /// Экземпляр данных модели
        /// </summary>
        /// <param name="id">[In]Идентификатор модели</param>
        public GmshUserModelData(long id)
        {
            Id = id;
            var error = 0;
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
        }
        /// <summary>
        /// Получает данные о модели
        /// </summary>
        public virtual void GetData()
        {
        }
    }
}
