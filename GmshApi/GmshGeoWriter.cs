using ModelControllerInterfaces.GmshController;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Xml.Linq;

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
            ResultFileName = Path.Combine(currentPath, modelName) + "Settings.geo";

            using (var writer = File.CreateText(ResultFileName))
            {
                writer.WriteLine($"Merge \"{modelName + extension}\";");
                var core = extension == ".geo" ? "Built-in" : "OpenCASCADE";
                writer.WriteLine($"SetFactory(\"{core}\");");

                WritePointsSettings(writer);
                WriteCurvesSettings(writer);
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
                                 .GroupBy(g => g.Key);

            var separator = ",";
            var numFormat = new NumberFormatInfo();

            foreach (var groups in entries)
            {
                var tags = groups.Select(v => v.Tag.ToString());
                var result = string.Join(separator, tags);
                
                writer.WriteLine($"Characteristic Length {{ { result } }} = { groups.Key.ToString(numFormat) };");
            }
        }

        private void WriteCurvesSettings(StreamWriter writer)
        {
            var dict = ParseCurvesSettings();
            foreach (var curveGroup in dict)
            {
                var key = curveGroup.Key.Split(' ');
                if (key.Length == 3 && !string.IsNullOrEmpty(key[0]) && !string.IsNullOrEmpty(key[2]))
                {
                    var tags = string.Join(",", curveGroup.Value);
                    writer.WriteLine($"Transfinite Curve {{ {tags} }} = {key[0]} Using {key[1]} {key[2]};");
                }
            }
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
