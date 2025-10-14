using Model.Interfaces;
using PropertiesCalculator.MaterialData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.Materials
{
    public class PlateMatData : MatData
    {
        public double Thickness { get; set; }
        public PlateMatData(MaterialDBItem material, IGroup group, float start, float stop) : base(material, group, start, stop)
        {
        }
    }
}
