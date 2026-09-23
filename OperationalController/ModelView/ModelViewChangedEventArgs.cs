using Model.Interfaces;
using Model.Interfaces.ObjectsCollections;
using System;
using System.Collections.Generic;

namespace OperationalController
{
    /// <summary>
    /// Описывает изменение состояния представления: что изменилось и у каких наборов.
    /// </summary>
    public class ModelViewChangedEventArgs : EventArgs
    {
        readonly ModelViewChange common;
        readonly Dictionary<ObjType, ModelViewChange> byType;
        readonly Dictionary<ISetInfo, ModelViewChange> bySet;

        /// <summary>
        /// Создаёт аргументы события, копируя переданные изменения.
        /// </summary>
        public ModelViewChangedEventArgs(ModelViewChange common, IReadOnlyDictionary<ObjType, ModelViewChange> byType, IReadOnlyDictionary<ISetInfo, ModelViewChange> bySet)
        {
            this.common = common;
            this.byType = new Dictionary<ObjType, ModelViewChange>(byType);
            this.bySet = new Dictionary<ISetInfo, ModelViewChange>(bySet, ReferenceEqualityComparer.Instance);
        }

        /// <summary>Возвращает все изменения, затрагивающие заданный набор.</summary>
        public ModelViewChange GetChanges(ISetInfo setInfo)
        {
            byType.TryGetValue(setInfo.ObjType, out var typeChanges);
            bySet.TryGetValue(setInfo, out var setChanges);
            return common | typeChanges | setChanges;
        }

        /// <summary>Есть ли у набора хотя бы одно из указанных изменений.</summary>
        public bool HasAny(ISetInfo setInfo, ModelViewChange changes)
        {
            var setChanges = GetChanges(setInfo);
            return (setChanges & changes) != ModelViewChange.None;
        }
    }
}
