using Model.Interfaces;
using Project.Interfaces.Tasks;
using MaterialDB.MaterialData;
using System;
using System.Globalization;
using Project.Tasks.Functions;

namespace Project.Tasks
{
    public enum ClampKind
    {
        Жесткое,
        Упругое,
        Симметрия,
        Контакт
    }

    public class ClampData : CondData
    {
        public ClampData(IGroup group,float start,float stop) : base (group,start,stop)
        {
            Kind = DataKind.Закрепление;
        }
        /// <summary>
        /// ClampKind
        /// </summary>
        public ClampKind ClampKind { get; private set; }

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString() + $" {ClampKind}";
        }
        /// <summary>
        /// TrySetKind
        /// </summary>
        /// <param name="v"></param>
        public void TrySetKind(string v)
        {
            ClampKind clampKind;
            Enum.TryParse(v, out clampKind);
            ClampKind = clampKind;
        }
    }
}