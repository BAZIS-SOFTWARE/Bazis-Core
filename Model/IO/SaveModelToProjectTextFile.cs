using Model.MeshObjects;
using Model.Events;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Model.IO
{
    /// <summary>
    /// SaveModelToProjectTextFile
    /// </summary>
    public class SaveModelToProjectTextFile : IModelSaver
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

                foreach (var objsType in model.ObjectData)
                {
                    sw.WriteLine("");
                    if (objsType.Key == ObjType.Узел)
                    {
                        sw.WriteLine("Узлы");
                        foreach (var nodeItem in model.ObjectData.FindMany<Node>())
                        {
                            var x = nodeItem.Position._x;
                            var y = nodeItem.Position._y;
                            var z = nodeItem.Position._z;
                            var numbr = nodeItem.Number;
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", numbr, x, y, z));
                        }
                        sw.WriteLine("#Узлы");
                    }


                    else if (objsType.Key == ObjType.Элемент1D |
                        objsType.Key == ObjType.Элемент2D |
                        objsType.Key == ObjType.Элемент3D)
                    {
                        SaveElements(model, sw, objsType.Key);
                    }
                }
                sw.WriteLine("");
                sw.WriteLine("Группы узлов и элементов");
                foreach (var group in model.GroupData)
                {
                    var name = group.GroupName;
                    var objInd = string.Join(" ", group.Select(x => x.Number));
                    var resu = name + " " + objInd;

                    if (group.ObjType == ObjType.Узел) sw.WriteLine("Группа узлов : {0}", resu);
                    if (group.ObjType == ObjType.Элемент3D) sw.WriteLine("Группа элементов 3D : {0}", resu);
                    if (group.ObjType == ObjType.Элемент2D) sw.WriteLine("Группа элементов 2D : {0}", resu);
                }
                sw.WriteLine("#Группы узлов и элементов");
                sw.Dispose();
            }
        }

        private static void SaveElements(IModelData model, StreamWriter sw, ObjType objsType)
        {
            sw.WriteLine(objsType);
            foreach (var obj in model.ObjectData[objsType])
            {
                var elem = (Element)obj;
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

