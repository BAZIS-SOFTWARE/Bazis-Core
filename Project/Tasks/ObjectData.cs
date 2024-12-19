using Geometry;
using Model.Interfaces;
using Project.Interfaces.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Tasks
{
    /// <summary>
    /// ObjectData
    /// </summary>
    public abstract class ObjectData : Data, IGroupDependent, ITimeDependentData
    {
        protected ObjectData(IGroup group, string info) : base(info)
        {
            if (group == null)
                throw new Exception($"Группа {group} пустая в строке : {info}");
            Group = group;
        }

        //public string GroupName { get; internal set; }
        /// <inheritdoc/>

        public float StartTime { get; set; }
/// <inheritdoc/>

        public float StopTime { get; set; }
/// <inheritdoc/>

        public IGroup Group { get; set; }
    }
}
