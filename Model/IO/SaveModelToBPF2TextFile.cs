using Model.MeshObjects;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Model.Interfaces.ObjectsCollections;
using Model.Interfaces.MeshObjects;
using Model.ObjectsCollections;

namespace Model.IO
{
    public class SaveModelToBPF2TextFile : IModelSaver
    {
        /// <inheritdoc/>
        public event Action<object, ISaverEventArgs> SaveEvent;
        /// <inheritdoc/>
        public void Save(IModelData model, string path)
        {
            using (var sw = new StreamWriter(path, false, System.Text.Encoding.Unicode))
            {
                sw.AutoFlush = true;
                //сохранение координат узлов
                var ver = Assembly.GetExecutingAssembly().GetName().Version;
                var verStr = "Версия " + $"{ver.Major}.{ver.Minor}.{ver.Build}";

                sw.WriteLine(verStr);

                sw.WriteLine("");
                SaveNodes(model, sw);
                SaveElements(model.ObjectData.E1DCollection.Objects, sw);
                SaveElements(model.ObjectData.E2DCollection.Objects, sw);
                SaveElements(model.ObjectData.E3DCollection.Objects, sw);
                sw.WriteLine("");

                sw.WriteLine("Группы");
                foreach (var group in model.GroupData)
                {
                    var name = group.Name;
                    var objInd = string.Join(" ", group.Select(x => x.Number));
                    var resu = name + " " + objInd;

                    if (group.ObjType == ObjType.Узел) sw.WriteLine("Группа узлов : {0}", resu);
                    if (group.ObjType == ObjType.Элемент3D) sw.WriteLine("Группа элементов 3D : {0}", resu);
                    if (group.ObjType == ObjType.Элемент2D) sw.WriteLine("Группа элементов 2D : {0}", resu);
                    if (group.ObjType == ObjType.Элемент1D) sw.WriteLine("Группа элементов 1D : {0}", resu);
                }
                sw.WriteLine("#Группы");
                sw.WriteLine("");
                sw.Dispose();
            }
        }
        private void SaveNodes(IModelData model, StreamWriter sw)
        {
            sw.WriteLine(ObjType.Узел);
            var nodes = model.ObjectData.NodesSet.Values;
            foreach (var nodeItem in nodes)
            {
                var x = nodeItem._x;
                var y = nodeItem._y;
                var z = nodeItem._z;
                var numbr = nodeItem.Number;
                sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", numbr, x, y, z));
            }
            sw.WriteLine($"#{ObjType.Узел}");
        }

        private void SaveElements<T>(ICollection<ObjectsSet<T>> collection, StreamWriter sw)
            where T : IElement
        {
            foreach (var set in collection)
            {
                sw.WriteLine($"{set.ObjType} {set.Name}");
                foreach (var value in set)
                {
                    var verts = value.Value.GetVertexes();
                    var str = string.Join(" ", verts.Select(x => x.Number.ToString()));
                    sw.WriteLine($"{value.Value.Number} {value.Value.Level} : {str}");
                }
                sw.WriteLine($"#{set.ObjType}");
            }
        }
    }
}
