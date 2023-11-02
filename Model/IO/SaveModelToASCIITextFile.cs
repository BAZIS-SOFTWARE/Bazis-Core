using Model.GroupsData;
using ModelInterfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Model.IO
{
    public class SaveModelToASCIITextFile
    {
        public void Save(IModelData modelData, string fileName)
        {
            var nodes = modelData.ObjectData.FindMany<Node>().ToList();
            var elements = modelData.ObjectData.FindMany<Element>().ToList();
            var groups = modelData.GroupData;
            using(var writer = new StreamWriter(fileName))
            {          
                WriteHeaders(writer, nodes.Count, elements.Count);
                WriteNodes(writer, nodes);
                WriteElements(writer, elements);
                WriteGroups(writer, groups);
            }
        }

        private void WriteHeaders(StreamWriter writer, int nodesCount, int elementsCount)
        {
            writer.WriteLine($"1VSD 0 230728 151609 230728 151609");
            writer.WriteLine();
            writer.WriteLine($" 100000 3 {nodesCount} {elementsCount} 0 6 12 0 0");
            writer.WriteLine("BEGIN_INFORMATIONS");
            writer.WriteLine(" 3 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0 0 0 0 0");
            writer.WriteLine(" 0 0 0 0 0 0 0 0 0 0 6 6 36 0 0 0 0 36 0 0 0 0 12 0 0 0 0 0 3 1 0 0 0 0 0 0 2 0 0 0");
            writer.WriteLine("END_INFORMATIONS");
        }

        private void WriteNodes(StreamWriter writer, List<Node> nodes)
        {
            writer.WriteLine($"BEGIN_NODES {nodes.Count}");
            foreach(var node in nodes)
            {
                var x = node.Position._x.ToString(new CultureInfo("en-US"));
                var y = node.Position._y.ToString(new CultureInfo("en-US"));
                var z = node.Position._z.ToString(new CultureInfo("en-US"));
                var nodePosition = $"{x} {y} {z}";
                writer.WriteLine($"{node.Number} 0 0 0 0 0 {nodePosition}");
            }
            writer.WriteLine($"END_NODES");
        }

        private void WriteElements(StreamWriter writer, List<Element> elements)
        {
            writer.WriteLine($"BEGIN_ELEMENTS {elements.Count}");
            foreach (var element in elements)
            {
                var nodes = string.Join(" ", element.GetNodes().Select(x => x.Number));
                var type = ConvertObjectTypeToFormatCode(element.ObjKind);
                writer.WriteLine($"{element.Number} {type} 0 0 0 {nodes}");
            }
            writer.WriteLine($"END_ELEMENTS");
        }

        private string ConvertObjectTypeToFormatCode(ObjKind type)
        {
            switch(type)
            {
                case ObjKind.Beam: return "1002";
                case ObjKind.Triangle: return "2003";
                case ObjKind.Tetra: return "3004";
                default: throw new ArgumentException($"Unable to recognize type {type}");
            }
        }

        private void WriteGroups(StreamWriter writer, IGroupData groupData)
        {
            writer.WriteLine($"BEGIN_GROUPS {groupData.Count}");
            var index = 1;
            foreach (var group in groupData)
            {
                var elements = string.Join(" ", group.Select(x => x.Number));
                writer.WriteLine($"{index++} {group.GroupName} 2 0 " +
                    $"\"PART_ID {index}\"  \"\"  \"PART built in Visual-Environment\" {elements}");
            }
            writer.WriteLine($"END_GROUPS");
        }
    }
}