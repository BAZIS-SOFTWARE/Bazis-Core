using Model;
using Model.Interfaces;
using ModelInterface;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ModelController.IO
{
    public class SaveModelToProjectTextFile : IModelSaver
    {
        public event Action<object, SaverEventArgs> SaveEvent;
        public void Save(ModelData model, string path)
        {
            var sw = new StreamWriter(path, false, System.Text.Encoding.Unicode) { AutoFlush = true };

            //сохранение координат узлов
            var ver = ConfigurationManager.AppSettings.Get("version");
            sw.WriteLine(ver);

            bool IsNodeExist = CheckObject(sw, model, ObjType.Узлы);
            bool Ise1DExist = CheckObject(sw, model, ObjType.Элементы1D);
            bool Ise2DExist = CheckObject(sw, model, ObjType.Элементы2D);
            bool Ise3DExist = CheckObject(sw, model, ObjType.Элементы3D);

            sw.WriteLine("");
            if (IsNodeExist)
            {
                sw.WriteLine("Узлы");
                var nObj = model.ObjectData.FindMany<Node>();
                foreach (var nodeItem in nObj)
                {
                    var x = nodeItem.Position._x;
                    var y = nodeItem.Position._y;
                    var z = nodeItem.Position._z;
                    var numbr = nodeItem.Number;
                    sw.WriteLine(string.Format(CultureInfo.InvariantCulture,"{0} {1} {2} {3}", numbr, x, y, z));
                }
                sw.WriteLine("#Узлы");
            }

            if (Ise3DExist)
            {
                sw.WriteLine("Элементы 3004");
                var e3DObj = model.ObjectData.FindMany<Element3D>().Where(x => x.ObjKind == ObjKind.Tetra).ToArray();

                foreach (var e3DItem in e3DObj)
                {
                    var nodes = e3DItem.GetNodes().ToArray();
                    var n1 = nodes[0].Number;
                    var n2 = nodes[1].Number;
                    var n3 = nodes[2].Number;
                    var n4 = nodes[3].Number;
                    sw.WriteLine("{0} {1} {2} {3} {4}", e3DItem.Number, n1, n2, n3, n4);
                }
                sw.WriteLine("#Элементы 3004");
            }
            if (Ise2DExist)
            {
                sw.WriteLine("Элементы 2003");
                var e2DObj = model.ObjectData.FindMany<Element2D>().Where(x => x.ObjKind == ObjKind.Triangle).ToArray();

                foreach (var e2DItem in e2DObj)
                {
                    var nodes = e2DItem.GetNodes().ToArray();
                    var n1 = nodes[0].Number;
                    var n2 = nodes[1].Number;
                    var n3 = nodes[2].Number;
                    sw.WriteLine("{0} {1} {2} {3}", e2DItem.Number, n1, n2, n3);
                }
                sw.WriteLine("#Элементы 2003");
            }


            sw.WriteLine("");
            sw.WriteLine("Группы узлов и элементов");
            foreach (var group in model.GroupData)
            {
                var name = group.GroupName;
                var objInd = string.Join(" ", group.ObjsNumbers);
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

