using Model.Interfaces;
using Project.Interfaces.Tasks;

namespace Project.Tasks
{
    /// <summary>
    /// ValuableData
    /// </summary>
    public abstract class ValuableData : ObjectData, IValuableData
    {
        protected ValuableData(IGroup group,string info) : base(group,info)
        {
        }

        /// <inheritdoc/>
        public IFrameFunction FrameFunction { get ; internal set; }
        
/// <inheritdoc/>

        public override string ToString()
        {
            return string.Format("{0} : {1}", Name, GetInfo);
        }

        /// <inheritdoc/>

        public string TrajectoryInfo
        {
            get
            {
                if (FrameFunction != null)
                    return MovedFrame.GetTrajectoryInfo();
                else return "*";
            }

        }

        /// <inheritdoc/>

        public IMovedFrame MovedFrame { get; internal set; }

/// <inheritdoc/>

        public Direction Direction { get; internal set; }
    }
}
