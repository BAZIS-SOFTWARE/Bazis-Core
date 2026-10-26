using Model.MeshObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules.xxsubtype;

namespace OperationalController.MeshObjsCreator.Extrude
{
    /// <summary>
    /// Стратегия экструзии 3д элементов
    /// </summary>
    public class ExtrudeBeamStrategy : IExtrusionStrategy<List<Beam>>
    {
        private Node _firstNode = null;
        private List<Beam> _result = new();
        private Vector4 _startNode = Vector4.Zero;
        private Vector4 _curCenter = Vector4.Zero;

        /// <summary>
        /// Инициализация данных, выполняется до начала перебора кривых и начала обхода каждой кривой
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="extrudeElements">Начальные элементы для экструзии</param>
        public void Initialize(CurveExtruder extruder, object extrudeElements)
        {
            _firstNode = extrudeElements as Node;
            _startNode = new Vector4((float)_firstNode._x, (float)_firstNode._y, (float)_firstNode._z, 1);
        }
        /// <summary>
        /// Препроцессор, выполняется перед началом внутреннего цикла обхода кривой
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="firstMatrix">Начальная матрица кривой (используется для формирования первого слоя)</param>
        public void PreProcess(CurveExtruder extruder, Matrix4x4 firstMatrix)
        {
            _curCenter = Vector4.Zero;
        }
        /// <summary>
        /// Добавление элементов на шаге кривой
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="correctMatrix">Корректирующая матрица (используется при переходе между двумя кривыми, чтобы сгладить переходы медлу слоями)</param>
        /// <param name="outIndex">Индекс обхода по внешнему циклу</param>
        /// <param name="inIndex">Индекс обхода по внутреннему циклу</param>
        public void AddElement(CurveExtruder extruder, Matrix4x4 correctMatrix, int outIndex, int inIndex)
        {
            if (outIndex > 0 && inIndex == 0)
                _firstNode = _result[_result.Count - 1][1];
            else
            {
                _curCenter = extruder.GetTransformedVector(_startNode, correctMatrix);
                var node = extruder.CreateNode(_curCenter);
                extruder.Nodes.Add(node.Number, node);

                if (inIndex > 0)
                {
                    var beam = extruder.CreateBeam(_firstNode, node);
                    _result.Add(beam);
                }
                _firstNode = node;
            }
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
            _startNode = rotateStatus || curveIndex == lastCurveIndex ? _curCenter
                                                                  : extruder.GetTransformedVector(_startNode, lastCorrectMatrix);
        }
        /// <summary>
        /// Получает результат Beam или Element3D
        /// </summary>
        /// <returns>Beam или Element3D</returns>
        public List<Beam> GetResult() => _result;
    }
}
