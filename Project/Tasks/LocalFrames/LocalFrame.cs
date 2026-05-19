using Geometry;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.LocalFrames
{
    public enum FrameType
    {
        MovedFrame,
        StaticFrame
    }
    /// <summary>
    /// LocalFrame
    /// </summary>
    public abstract class LocalFrame
    {
        /// <summary>
        /// FrameType
        /// </summary>
        public FrameType FrameType { get; internal set; }
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; internal set; }
        /// <inheritdoc/>
        public Point3D Shifting { get; set; } = new Point3D();
        /// <summary>
        /// Rotation_X
        /// </summary>
        public float Rotation_X { get; set; }
        /// <summary>
        /// Rotation_Y
        /// </summary>
        public float Rotation_Y { get; set; }
        /// <summary>
        /// Rotation_Z
        /// </summary>
        public float Rotation_Z { get; set; }
        /// <summary>
        /// Frame
        /// </summary>
        internal Frame frame;
        /// <inheritdoc/>

        public abstract Frame Frame { get; }
        /// <summary>
        /// CalcPosition
        /// </summary>
        public abstract void CalcPosition();

        /// <summary>
        /// SetShiftingAndRotation
        /// </summary>
        /// <param name="shiftingData"></param>
        /// <exception cref="Exception"></exception>

        public void SetShiftingAndRotation(string shiftingData)
        {
            var shifftingData = shiftingData.Split('|');

            if (shifftingData.Length != 6)
                throw new Exception("Проверьте вектор смещения и углы поворота");

            //{
            if (!float.TryParse(shifftingData[0], out var x))
                throw new Exception("Смещение по X задано неверно!");
            if (!float.TryParse(shifftingData[1], out var y))
                throw new Exception("Смещение по Y задано неверно!");
            if (!float.TryParse(shifftingData[2], out var z))
                throw new Exception("Смещение по Z задано неверно!");
            if (!float.TryParse(shifftingData[3], out var angle_x))
                throw new Exception("Угол задан неверно!");
            if (!float.TryParse(shifftingData[4], out var angle_y))
                throw new Exception("Угол задан неверно!");
            if (!float.TryParse(shifftingData[5], out var angle_z))
                throw new Exception("Угол задан неверно!");

            // TODO дописать парсинг для углов y и z

            Shifting = new Point3D(x, y, z);
            Rotation_X = angle_x;
            Rotation_Y = angle_y;
            Rotation_Z = angle_z;
            //}
        }

        /// <summary>
        /// Получение локальной точки в локальных координатах для текущего Frame
        /// </summary>
        /// <param name="point">Точка в локальной системе координат</param>
        /// <returns>Координаты точки в локальной системе координат относительно центра Frame</returns>
        public Point3D GetLocalCoordsInLocalFrame(Point3D point) => frame.GetCoordsInFrame(point);

        /// <summary>
        /// Получение глобальной точки в локальных координатах для текущего Frame
        /// </summary>
        /// <param name="point">Точка в глобальной системе координат</param>
        /// <returns>Координаты точки в локальной системе координат относительно центра Frame</returns>
        public Point3D GetGlobalCoordsInLocalFrame(Point3D point)
        {
            var tempFrame = new Frame();

            tempFrame.Rotate(frame.Dir_X, Rotation_X);
            tempFrame.Rotate(frame.Dir_Y, Rotation_Y);
            tempFrame.Rotate(frame.Dir_Z, Rotation_Z);
            tempFrame.Shift(Shifting);

            return tempFrame.GetCoordsInFrame(point);
        }
    }
}
