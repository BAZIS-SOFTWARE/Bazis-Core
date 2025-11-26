using Model.Interfaces;
using MaterialDB.MaterialData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.Materials
{
    public class PlateMatData : MatData
    {
        /// <summary>
        /// Thickness
        /// </summary>
        public double Thickness { get; set; } = 1;
        public PlateMatData(MaterialDBItem material, IGroup group, float start, float stop) : 
            base(material, group, start, stop)
        {
        }

        protected override string GetInfo
        {
            get
            {
                var mfInfo = "*";
                if (FrameFunction?.LocalFrame != null)
                    mfInfo = FrameFunction.LocalFrame.ToString();
                return $"{Group.Name} пластины {Thickness} {Material.Name} {StartTime} {StopTime} {mfInfo}";
            }
        }
    }
}
