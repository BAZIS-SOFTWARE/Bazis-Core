using Model.Interfaces;
using Project.Interfaces.Tasks;
using PropertiesCalculator.MaterialData;
using System;

namespace Project.Tasks
{
    /// <summary>
    /// ObjKind. Экспериментальное перечесление
    /// </summary>
    public enum ObjKind
    {
        Стержни,
        Пластины,
        Объемы
    }

    public class MatData : CondData
    {
        
        public MatData(MaterialDBItem material, IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Материал;
            Material = material;
        }
        /// <summary>
        /// MatName
        /// </summary>
        public MaterialDBItem Material { get; set; }
/// <inheritdoc/>

        protected override string GetInfo
        {
            get
            {
                var mfInfo = "*";
                if (FrameFunction?.LocalFrame != null)
                    mfInfo = FrameFunction.LocalFrame.ToString();
                return string.Format("{0} {1} {2} {3} {4}", Group.Name, Material.Name, StartTime, StopTime, mfInfo);
            }
        }
    }
}