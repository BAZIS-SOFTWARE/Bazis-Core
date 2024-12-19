using Geometry;
using Model.Interfaces;
using Model.Interfaces.MeshObjects;
using System.Collections.Generic;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// IMovedFrameFunction
    /// </summary>
    public interface IMovedFrame
    {
        /// <summary>
        /// GetTrajectoryInfo
        /// </summary>
        /// <returns></returns>
        string GetTrajectoryInfo();
        /// <summary>
        /// StartPoints
        /// </summary>
        IGroup StartPoints { get; set; }
        /// <summary>
        /// StopPoints
        /// </summary>
        IGroup StopPoints { get; set; }
        /// <summary>
        /// Velosity
        /// </summary>
        float Velosity { get; set; }
        /// <summary>
        /// BaseLine
        /// </summary>
        IGroup BaseLine { get; set; }
        /// <summary>
        /// RefLine
        /// </summary>
        IGroup RefLine { get; set; }
        /// <summary>
        /// Shifting
        /// </summary>
        Point3D Shifting { get; set; }
        /// <summary>
        /// Rotation
        /// </summary>
        float Rotation { get; set; }
        /// <summary>
        /// SetShiftingAndRotation
        /// </summary>
        /// <param name="shiftingData"></param>
        void SetShiftingAndRotation(string shiftingData);

        /// <summary>
        /// CalcFrame
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        Frame CalcFrame(float time);
        /// <summary>
        /// CalcMotionTime
        /// </summary>
        /// <returns></returns>
        double CalcMotionTime();
        /// <summary>
        /// CheckTrajNodes
        /// </summary>
        void CheckTrajNodes();
        /// <summary>
        /// SortTrajNodes
        /// </summary>
        void SortTrajNodes();
    }
}
