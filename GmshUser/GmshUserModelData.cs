using Geometry;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.GmshController;

namespace Model.GmshUser
{
    public interface GmshData
    {

    }


    /// <summary>
    /// Класс представляет данные модели
    /// </summary>
    public class GmshUserModelData : GmshData
    {
        protected List<Node> nodes;
        protected List<Curve> curves;
        protected List<Surface> surfaces;

        /// <summary>
        /// Получает идентификатор с ассоциированной моделью
        /// </summary>
        protected long Id { get; private set; }
        /// <summary>
        /// Возвращает точки как объекты ModelObject 
        /// </summary>
        public IEnumerable<Node> Nodes => nodes;
        /// <summary>
        /// Возвращает кривые как объекты ModelObject 
        /// </summary>
        public IEnumerable<Curve> Curves => curves;
        /// <summary>
        /// Возвращает поверхности как объекты ModelObject 
        /// </summary>
        public IEnumerable<Surface> Surfaces => surfaces;
        /// <summary>
        /// Экземпляр данных модели
        /// </summary>
        /// <param name="id">[In]Идентификатор модели</param>
        public GmshUserModelData(long id)
        {
            Id = id;
            var error = 0;
            GmshModel.gmshModelSetCurrent(Id.ToString(), ref error);
            nodes = new List<Node>();
            curves = new List<Curve>();
            surfaces = new List<Surface>();
        }
        /// <summary>
        /// Извлекает данные из модели
        /// </summary>
        public virtual void GetData()
        {

        }
    }
}
