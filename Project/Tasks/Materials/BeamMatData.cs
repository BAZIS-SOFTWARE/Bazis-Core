using Model.Interfaces;
using PropertiesCalculator.MaterialData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.Materials
{
    internal class BeamMatData : MatData
    {
        public double Square { get; set; }
        public BeamMatData(MaterialDBItem material, IGroup group, float start, float stop) : base(material, group, start, stop)
        {

        }
    }
}
