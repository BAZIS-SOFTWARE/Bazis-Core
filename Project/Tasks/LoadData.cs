using Model.Interfaces;
using Project.Interfaces.Tasks;
using MaterialDB.MaterialData;
using System;
using System.Globalization;

namespace Project.Tasks
{
    public enum LoadKind
    {
        Сила,
        Давление
    }
    public class LoadData : CondData
    {
        public LoadData(IGroup group, float start, float stop) : base(group, start, stop)
        {
            Kind = DataKind.Нагрузка;
        }     


        public LoadKind LoadKind { get; set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString() + $" {LoadKind}";
        }

        public void TrySetKind(string v)
        {
            LoadKind loadKind;
            if(Enum.TryParse(v, out loadKind))
                LoadKind = loadKind;
        }
    }
}