using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmshApi
{
    /// <summary>
    /// Класс содержит информацию о типе извлечнных элементов
    /// </summary>
    public class ElementProperties
    {
        /// <summary>Имя элемента</summary>
        public string elementName;
        /// <summary>Размерность</summary>
        public int dim;
        /// <summary>Порядок элементов (линейные, квадратичные, кубические и т.д.)</summary>
        public int order;
        /// <summary>Общее количество узлов, приходящиеся на один элемент указанного типа</summary>
        public int numNodes;
        /// <summary>Массив координат узлов элементов. Сейчас не используется. Для получения координат
        /// вызываем метод ModelMeshGetNodesByElementType</summary>
        public double[] coords;
        /// <summary>Количество узлов на все элементы </summary>
        public int coords_n;
        /// <summary>Главные узлы</summary>
        public int numPrimaryNodes;
    }
}
