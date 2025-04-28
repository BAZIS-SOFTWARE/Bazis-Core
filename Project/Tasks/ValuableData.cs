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
        public IFrameFunction FrameFunction { get ; set; }
/// <inheritdoc/>


        public override string ToString()
        {
            return $"{Kind} : {GetInfo}";
        }

        /// <inheritdoc/>

        public ILocalFrame LocalFrame { get; set; }

        /// <inheritdoc/>

        public Direction Direction { get; internal set; }

    }
}
