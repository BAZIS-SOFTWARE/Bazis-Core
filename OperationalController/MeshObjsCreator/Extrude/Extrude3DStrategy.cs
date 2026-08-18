using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController.MeshObjsCreator.Extrude
{
    /// <summary>
    /// Стратегия экструзии 3д элементов
    /// </summary>
    public class Extrude3DStrategy : IExtrusionStrategy<List<List<Element3D>>>
    {
        private IEnumerable<Element2D> _first;
        private List<Element2D> _current;
        private List<List<Element3D>> _result = new();
        /// <summary>
        /// Флаг трансфинитной экструзии
        /// </summary>
        public bool Transfinite { get; set; }
        /// <summary>
        /// Инициализация данных, выполняется до начала перебора кривых и начала обхода каждой кривой
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="extrudeElements">Начальные элементы для экструзии</param>
        public void Initialize(CurveExtruder extruder, object extrudeElements)
        {
            _first = extruder.SortFirstLayer(extrudeElements as IEnumerable<Element2D>);
        }
        /// <summary>
        /// Препроцессор, выполняется перед началом внутреннего цикла обхода кривой
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="firstMatrix">Начальная матрица кривой (используется для формирования первого слоя)</param>
        public void PreProcess(CurveExtruder extruder, Matrix4x4 firstMatrix)
        {
            _current = extruder.CreateNextLayer(_first, firstMatrix);
        }
        /// <summary>
        /// Добавление элементов на шаге кривой
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="correctMatrix">Корректирующая матрица (используется при переходе между двумя кривыми, чтобы сгладить переходы медлу слоями)</param>
        /// <param name="index">Индекс обхода по внутреннему циклу</param>
        public void AddElement(CurveExtruder extruder, Matrix4x4 correctMatrix, int index)
        {
            var nextLayer = extruder.CreateNextLayer(_first, correctMatrix);
            _result.Add(extruder.CreateLayerTetrahedrons(_current, nextLayer));
            _current = nextLayer;
        }
        /// <summary>
        /// Постпроцессинг, вызывается как корректировка на переходах между кривыми
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="rotateStatus">Статус поворота (используется при достижении порогового угла)</param>
        /// <param name="curveIndex">Индекс кривой</param>
        /// <param name="lastCurveIndex">Индекс последней кривой</param>
        /// <param name="lastCorrectMatrix">Матрица на последнем слое умноженная на корректирующую</param>
        public void PostProcess(CurveExtruder extruder, bool rotateStatus, int curveIndex, int lastCurveIndex, Matrix4x4 lastCorrectMatrix)
        {
            _first = rotateStatus || curveIndex == lastCurveIndex ? _current
                                                                  : extruder.CreateNextLayer(_first, lastCorrectMatrix);
        }
        /// <summary>
        /// Возвращает набор наборов элементов 3д
        /// </summary>
        /// <returns>Набор наборов элементов 3д</returns>
        public List<List<Element3D>> GetResult() => _result;
    }
}
