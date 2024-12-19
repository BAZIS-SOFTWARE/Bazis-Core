using Model.Interfaces;

namespace Project.Interfaces.Tasks
{
    /// <summary>
    /// IGroupData
    /// </summary>
    public interface IGroupDependent
    {
        /// <summary>
        /// IGroup
        /// </summary>
        IGroup Group { get; set; }
    }
}
