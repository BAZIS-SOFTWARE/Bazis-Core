using Model.Interfaces;
using MaterialDB.MaterialData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project.Tasks.Materials
{
    public class BeamMatData : MatData
    {
        /// <summary>
        /// Diameter
        /// </summary>
        public double Diameter { get; set; } = 1;
        public BeamMatData(MaterialDBItem material, IGroup group, float start, float stop) : 
            base(material, group, start, stop)
        {

        }
/// <inheritdoc/>

        public override string ToString()
        {
            return base.ToString() + $" стержни {Diameter}";
        }
    }
}
