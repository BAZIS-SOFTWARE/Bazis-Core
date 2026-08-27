using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OperationalController.MeshObjsCreator.Extrude
{
    /// <summary>
    /// Интерфейс выбора стратегии экструзии
    /// </summary>
    /// <typeparam name="TResult">Набор элементов типа Beam или Element3D</typeparam>
    public interface IExtrusionStrategy<TResult>
    {
        /// <summary>
        /// Инициализация данных, выполняется до начала перебора кривых и начала обхода каждой кривой
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="extrudeElements">Начальные элементы для экструзии</param>
        void Initialize(CurveExtruder extruder, object extrudeElements);
        /// <summary>
        /// Препроцессор, выполняется перед началом внутреннего цикла обхода кривой
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="firstMatrix">Начальная матрица кривой (используется для формирования первого слоя)</param>
        void PreProcess(CurveExtruder extruder, Matrix4x4 firstMatrix);
        /// <summary>
        /// Добавление элементов на шаге кривой
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="correctMatrix">Корректирующая матрица (используется при переходе между двумя кривыми, чтобы сгладить переходы медлу слоями)</param>
        /// <param name="outIndex">Индекс обхода по внешнему циклу</param>
        /// <param name="inIndex">Индекс обхода по внутреннему циклу</param>
        void AddElement(CurveExtruder extruder, Matrix4x4 correctMatrix, int outIndex, int inIndex);
        /// <summary>
        /// Постпроцессинг, вызывается как корректировка на переходах между кривыми
        /// </summary>
        /// <param name="extruder">Класс экструзии</param>
        /// <param name="rotateStatus">Статус поворота (используется при достижении порогового угла)</param>
        /// <param name="curveIndex">Индекс кривой</param>
        /// <param name="lastCurveIndex">Индекс последней кривой</param>
        /// <param name="lastCorrectMatrix">Матрица на последнем слое</param>
        void PostProcess(CurveExtruder extruder, bool rotateStatus, int curveIndex, int lastCurveIndex, Matrix4x4 lastCorrectMatrix);
        /// <summary>
        /// Возвращает результат набор Beam элементов или набор наборов Element3D
        /// </summary>
        /// <returns>набор Beam элементов или набор наборов Element3D</returns>
        TResult GetResult();
    }
}
