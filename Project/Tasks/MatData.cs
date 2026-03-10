using Model.Interfaces;
using Project.Interfaces.Tasks;
using MaterialDB.MaterialData;

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

        public override string ToString()
        {
            return base.ToString() + $" {Material.Name}";
        }
    }
}