using Model;
using Model.Events;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Model.IO
{
    public class SaveModelToProjectTextFile : IModelSaver
    {
        public event Action<object, SaverEventArgs> SaveEvent;
        public void Save(ModelData model, string path)
        {
            var sw = new StreamWriter(path, false, System.Text.Encoding.Unicode) { AutoFlush = true };

            //сохранение координат узлов
            var ver = Assembly.GetExecutingAssembly().GetName().Version;
            var verStr = "Версия " + $"{ver.Major}.{ver.Minor}.{ver.Build}";

            sw.WriteLine(ver);

            foreach (var objsType in model.ObjectData.GetObjectTypes())
            {
                sw.WriteLine("");
                if (objsType == "Узлы")
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

                else if (objsType == "Элементы3D" | objsType == "Элементы2D" | objsType == "Элементы1D")
                {
                    sw.WriteLine(objsType);
                    foreach (var obj in model.ObjectData.FindMany(objsType))
                    {
                        var elem = (Element)obj;
                        var strAr = new List<string>();
                        foreach (var node in elem.GetNodes())
                            strAr.Add(node.Number.ToString());

                        var strNodes = string.Join(" ", strAr);

                        sw.WriteLine("{0} {1}", elem.Number, strNodes);
                    }
                    sw.WriteLine($"#{objsType}");
                }
            }           
            sw.WriteLine("");
            sw.WriteLine("Группы узлов и элементов");
            foreach (var group in model.GroupData)
            {
                var name = group.GroupName;
                var objInd = string.Join(" ", group.Select(x => x.Number));
                var resu = name + " " + objInd;

                if (group.ObjType == "Узлы") sw.WriteLine("Группа узлов : {0}", resu);
                if (group.ObjType == "Элементы3D") sw.WriteLine("Группа элементов 3D : {0}", resu);
                if (group.ObjType == "Элементы2D") sw.WriteLine("Группа элементов 2D : {0}", resu);
            }
            sw.WriteLine("#Группы узлов и элементов");
            sw.Dispose();
        }

        private bool CheckObject(StreamWriter sw, ModelData model, ObjType objType)
        {
            var objs = model.ObjectData.FindMany(objType.ToString()).ToArray();

            if (objs.Length > 0)
            {
                sw.WriteLine(objType + " : {0}", objs.Length);
                return true;
            }
            else return false;
        }
    }
}

