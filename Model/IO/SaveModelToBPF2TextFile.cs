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

                sw.WriteLine(ver);

                foreach (var item in model.ObjectData.ObjsTypes)
                {
                    sw.WriteLine("");
                    if (item == ObjType.Узел)
                    {
                        sw.WriteLine(item);
                        var nodes = model.ObjectData.NodeCollection.GetObjects();
                        foreach (var nodeItem in nodes)
                        {
                            var x = nodeItem._x;
                            var y = nodeItem._y;
                            var z = nodeItem._z;
                            var numbr = nodeItem.Number;
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", numbr, x, y, z));
                        }
                        sw.WriteLine($"#{item}");
                    }

                    else if (item == ObjType.Элемент1D)
                    {
                        sw.WriteLine(item);
                        SaveElements(model.ObjectData.E1DCollection.Objects, sw);
                        sw.WriteLine($"#{item}");
                    }
                    else if (item == ObjType.Элемент2D)
                    {
                        sw.WriteLine(item);
                        SaveElements(model.ObjectData.E2DCollection.Objects, sw);
                        sw.WriteLine($"#{item}");
                    }
                    else if (item == ObjType.Элемент3D)
                    {
                        sw.WriteLine(item);
                        SaveElements(model.ObjectData.E3DCollection.Objects, sw);
                        sw.WriteLine($"#{item}");
                    }
                }
                sw.WriteLine("");
                sw.WriteLine("Группы узлов и элементов");
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
                sw.WriteLine("#Группы узлов и элементов");
                sw.Dispose();
            }
        }

        private void SaveElements<T>(ICollection<ObjectsSet<T>> collection, StreamWriter sw)
            where T : IElement
        {
            foreach (var elem in collection)
            {
                sw.WriteLine(elem.Name);
                foreach (var value in elem)
                    sw.WriteLine(value.Value);
                sw.WriteLine($"#{elem.Name}");
            }
        }
    }
}
