using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Model.Interfaces.MeshObjects;

namespace Model.IO
{
    /// <summary>
    /// SaveModelToProjectTextFile
    /// </summary>
    public class SaveModelToBPFTextFile : IModelSaver
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


                sw.WriteLine("");

                sw.WriteLine("Узлы");
                foreach (var nodeItem in model.ObjectData.NodesSet.Values)
                {
                    var x = nodeItem.Position._x;
                    var y = nodeItem.Position._y;
                    var z = nodeItem.Position._z;
                    var numbr = nodeItem.Number;
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", numbr, x, y, z));
                }
                sw.WriteLine("#Узлы");

                SaveElements(model, sw, ObjType.Элемент1D);
                SaveElements(model, sw, ObjType.Элемент2D);
                SaveElements(model, sw, ObjType.Элемент3D);

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

        private static void SaveElements(IModelData model, StreamWriter sw, ObjType objsType)
        {
            sw.WriteLine(objsType);

            var objs = model.ObjectData.GetObjects(objsType);
            foreach (var obj in objs)
            {
                var elem = (IElement)obj;
                var strAr = new List<string>();
                foreach (var node in elem.GetVertexes())
                    strAr.Add(node.Number.ToString());

                var strNodes = string.Join(" ", strAr);

                sw.WriteLine("{0} {1}", elem.Number, strNodes);
            }
            sw.WriteLine($"#{objsType}");
        }
    }
}

