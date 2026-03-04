using Model.Interfaces;
using Project.Interfaces.Tasks;

namespace Project.Tasks
{
    /// <summary>
    /// MediaType
    /// </summary>
    public enum MediaType
    {
        /// <summary>
        /// HeatFlux
        /// </summary>
        HeatFlux,
        /// <summary>
        /// ConstantTemp
        /// </summary>
        ConstantTemp
    }
    public class MediaData : CondData
    {
        /// <summary>
        /// MediaProcess
        /// </summary>
        //public string MediaProcess { get; internal set; } 

        public MediaData(IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Среда;
        }

        /// <summary>
        /// MediaType
        /// </summary>
        public MediaType MediaType { get; set; }
/// <inheritdoc/>

        public override string ToString()
        {
            return base.ToString() + $" {MediaType}";
        }


    }
}