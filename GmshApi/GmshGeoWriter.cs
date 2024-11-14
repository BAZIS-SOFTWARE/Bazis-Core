using ModelControllerInterfaces.GmshController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Xml.Linq;
using System.Runtime;
using ModelInterfaces;

namespace GmshApi
{
    /// <summary>
    /// Класс для сохранения настроек модели сеточного генератора в geo файл
    /// </summary>
    public class GmshGeoWriter
    {
        /// <summary>
        /// Ключ настроек трансфиниций кривых по которому нужно искать во внутренних структурах gmsh
        /// </summary>
        public static string TransfinitionCurveKey {  get; set; } = "transfinite";
        /// <summary>
        /// Зарезервированная часть названия файла, которая будет подставляться для формирования результирующего файла .geo
        /// </summary>
        public static string FileNamePostfix { get; set; } = "Settings";
        /// <summary>
        /// Путь до трансформированного geo файла
        /// </summary>
        public string ResultFileName { get; private set; }
        private IGmshController Controller { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="controller">[In]Контроллер gmsh</param>
        public GmshGeoWriter(IGmshController controller)
        {
            Controller = controller;
        }
        /// <summary>
        /// Записать настройки в geo
        /// </summary>
        public void WriteSettingsToGeo()
        {
            string fileName;
            Controller.ModelGetFileName(out fileName);

            var extension = Path.GetExtension(fileName);
            var modelName = Path.GetFileNameWithoutExtension(fileName);
            var currentPath = Path.GetDirectoryName(fileName);
            ResultFileName = Path.Combine(currentPath, modelName) + FileNamePostfix + ".geo";

            using (var writer = File.CreateText(ResultFileName))
            {
                writer.WriteLine($"Merge \"{modelName + extension}\";");
                var core = extension == ".geo" ? "Built-in" : "OpenCASCADE";
                writer.WriteLine($"SetFactory(\"{core}\");");
                writer.WriteLine();

                WritePointsSettings(writer);
                WriteCurvesSettings(writer);
                WriteGradientSettings(writer);
                WriteGeneratorSettings(writer);
            }
        }

        private void WritePointsSettings(StreamWriter writer)
        {
            int[] dimTags;
            Controller.ModelGetGeometryEntities(out dimTags, 0);
            var sizes = Controller.GetMeshDensity(dimTags);
            var entries = dimTags.Where((v, i) => (i & 1) == 1)
                                 .Zip(sizes, (f, s) => new { Tag = f, Key = s })
                                 .Where(v => v.Key != 0.0)
                                 .GroupBy(g => g.Key)
                                 .ToArray();

            var separator = ",";
            var numFormat = new NumberFormatInfo();

            foreach (var groups in entries)
            {
                var tags = groups.Select(v => v.Tag.ToString());
                var result = string.Join(separator, tags);
                
                writer.WriteLine($"Characteristic Length {{ { result } }} = { groups.Key.ToString(numFormat) };");
            }
            if(entries.Length > 0)
                writer.WriteLine();
        }

        private void WriteCurvesSettings(StreamWriter writer)
        {
            var dict = ParseCurvesSettings();
            foreach (var curveGroup in dict)
            {
                var key = curveGroup.Key.Split(' ');
                if (key.Length == 3 && !string.IsNullOrEmpty(key[0]) && !string.IsNullOrEmpty(key[2]))
                {
                    foreach (var curve in curveGroup.Value)
                        writer.WriteLine($"SetString[\"{TransfinitionCurveKey + " " + curve}\", \"{curveGroup.Key}\"];");
                    var tags = string.Join(",", curveGroup.Value);
                    writer.WriteLine($"Transfinite Curve {{ {tags} }} = {key[0]} Using {key[1]} {key[2]};");
                }
            }
            if(dict.Count > 0)
                writer.WriteLine();
        }

        private void WriteGradientSettings(StreamWriter writer)
        {
            int[] fieldTags, dimTags;
            Controller.ModelMeshFieldList(out fieldTags);

            if(fieldTags.Length != 0)//Есть градиентный фильтр?
            {
                Controller.ModelGetGeometryEntities(out dimTags, 0);
                var sizes = Controller.GetMeshDensity(dimTags);
                var numFormat = new NumberFormatInfo();

                var ierr = 0;
                var layerThickness = 0.0;
                Controller.MeshFieldGetNumber(1, "DistMax", ref layerThickness, ref ierr);

                var sizeMax = 0.0;
                Controller.MeshFieldGetNumber(1, "SizeMax", ref sizeMax, ref ierr);

                var power = 0.0;
                Controller.MeshFieldGetNumber(1, "Power", ref power, ref ierr);

                writer.WriteLine($"Characteristic Length {{ Point{{:}} }} = {sizes[0].ToString(numFormat)};");
                writer.WriteLine($"Field[1] = Extend;");
                writer.WriteLine($"Field[1].SurfacesList = {{ Surface{{:}} }};");
                writer.WriteLine($"Field[1].CurvesList = {{ Curve{{:}} }};");
                writer.WriteLine($"Field[1].DistMax = {layerThickness.ToString(numFormat)};");
                writer.WriteLine($"Field[1].SizeMax = {sizeMax.ToString(numFormat)};");
                writer.WriteLine($"Field[1].Power = {power.ToString(numFormat)};");
                writer.WriteLine($"Mesh.MeshSizeExtendFromBoundary = -2;");
                writer.WriteLine($"Background Field = 1;");

                writer.WriteLine();
            }
        }

        private void WriteGeneratorSettings(StreamWriter writer)
        {
            WriteGeneratorOption(writer, "Mesh.Algorithm");
            WriteGeneratorOption(writer, "Mesh.MeshSizeMin");
            WriteGeneratorOption(writer, "Mesh.MeshSizeMax");
            WriteGeneratorOption(writer, "Mesh.MeshSizeFactor");

            writer.WriteLine();
        }

        private void WriteGeneratorOption(StreamWriter writer, string option)
        {
            var ierr = 0;
            var value = 0.0;
            var numFormat = new NumberFormatInfo();

            Controller.OptionGetNumber(option, ref value, ref ierr);
            writer.WriteLine($"{option} = {value.ToString(numFormat)};");
        }

        private Dictionary<string, List<string>> ParseCurvesSettings()
        {
            string[] attribNames;
            Controller.ModelGetAttributeNames(out attribNames);

            var attribDict = new Dictionary<string, List<string>>();

            for (var i = 0; i < attribNames.Length; ++i)
            {
                if (attribNames[i].Contains(TransfinitionCurveKey))
                {
                    string[] attributes;
                    Controller.ModelGetAttribute(attribNames[i], out attributes);
                    
                    var key = string.Join(" ", attributes);
                    if (!attribDict.ContainsKey(key))
                        attribDict.Add(key, new List<string>());

                    var tag = attribNames[i].Split(' ')[1];
                    attribDict[key].Add(tag);
                }
            }
            return attribDict;
        }
    }
}
