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
/// <inheritdoc/>

        public override string ToString()
        {
            return base.ToString() + $" пластины {Thickness}";
        }
    }
}
