using Geometry;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.FrameCreators
{
    /// <summary>
    /// LocalFrame
    /// </summary>
    public abstract class LocalFrame
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; internal set; }
        /// <inheritdoc/>
        public Point3D Shifting { get; set; } = new Point3D();
        /// <inheritdoc/>
        public float Rotation { get; set; }
        /// <summary>
        /// Frame
        /// </summary>
        internal Frame frame;
/// <inheritdoc/>

        public abstract Frame Frame { get;}
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

            if (shifftingData.Length == 4)
            {
                if (!float.TryParse(shifftingData[0], out var x))
                    throw new Exception("Смещение по X задано неверно!");
                if (!float.TryParse(shifftingData[1], out var y))
                    throw new Exception("Смещение по Y задано неверно!");
                if (!float.TryParse(shifftingData[2], out var z))
                    throw new Exception("Смещение по Z задано неверно!");
                if (!float.TryParse(shifftingData[3], out var angle))
                    throw new Exception("Угол задан неверно!");

                Shifting = new Point3D(x, y, z);
                Rotation = angle;
            }
        }
    }
}
