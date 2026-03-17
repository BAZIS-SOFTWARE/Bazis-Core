using Model.Interfaces;
using Project.Interfaces.Tasks;

namespace Project.Tasks
{
    /// <summary>
    /// HeatData.J/mm3
    /// </summary>
    public class HeatData : CondData
    {
        public HeatData(IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Нагрев;
        }
    }
}
