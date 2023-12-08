using Model.MeshObjects;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

                foreach (var objsType in model.ObjectData.Keys)
                {
                    sw.WriteLine("");
                    if (objsType == ObjType.Узел)
                    {
                        sw.WriteLine("Узлы");
                        foreach (var nodeItem in model.ObjectData[ObjType.Узел])
                        {
                            var x = nodeItem._x;
                            var y = nodeItem._y;
                            var z = nodeItem._z;
                            var numbr = nodeItem.Number;
                            sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2} {3}", numbr, x, y, z));
                        }
                        sw.WriteLine("#Узлы");
                    }

                    else if (objsType == ObjType.Элемент3D | objsType == ObjType.Элемент2D | objsType == ObjType.Элемент1D)
                    {
                        sw.WriteLine(objsType);
                        foreach (var obj in model.ObjectData[objsType])
                        {
                            var elem = (Element)obj;
                            sw.WriteLine(elem.ToString());
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

                    if (group.ObjType == ObjType.Узел) sw.WriteLine("Группа узлов : {0}", resu);
                    if (group.ObjType == ObjType.Элемент3D) sw.WriteLine("Группа элементов 3D : {0}", resu);
                    if (group.ObjType == ObjType.Элемент2D) sw.WriteLine("Группа элементов 2D : {0}", resu);
                }
                sw.WriteLine("#Группы узлов и элементов");
                sw.Dispose();
            }
        }
    }
}
