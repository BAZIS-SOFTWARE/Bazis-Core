using Model.Interfaces;
using Project.Interfaces.Tasks;
using Project.Tasks.Functions;

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
        public IFrameFunction FrameFunction { get ; set; }
        
/// <inheritdoc/>

        public override string ToString()
        {
            return string.Format("{0} : {1}", Name, GetInfo);
        }

        /// <inheritdoc/>

        public MovedFrame MovedFrame { get; set; }

/// <inheritdoc/>

        public Direction Direction { get; internal set; }
    }
}
