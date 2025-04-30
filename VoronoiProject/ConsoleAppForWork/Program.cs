using MapDataLib;
using System;
using System.IO;
using Voronoi;


namespace ConsoleAppForWork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string applicationPath = Environment.CurrentDirectory;
            // СЮДА ПУТЬ к данным
            var dataPath = Path.Combine(applicationPath, "Data");
            DirectoryInfo dir = new DirectoryInfo(dataPath);
            var dataFiles = dir.GetFiles("*.shp");
            string outFolder = @"Output";
            if (!Directory.Exists(outFolder))
            {
                Directory.CreateDirectory(outFolder);
            }
            var saveFilePath = Path.Combine(applicationPath, "Output");

            string outTableFolder = @"Tables";
            var savePath = Path.Combine(applicationPath, outTableFolder);
            if (!Directory.Exists(outTableFolder))
            {
                Directory.CreateDirectory(outTableFolder);
            }

            var outTableName = savePath + "\\results.txt";

            Map inputMap = new Map();
            foreach (var file in dataFiles)
            {
                var shapeFile = new ShapeFileIO();
                var mapObj = shapeFile.Open(file.FullName);
                mapObj.ColorName = file.Name;
                if (mapObj != null)
                {
                    inputMap.Add(mapObj);
                }
                Console.WriteLine(file.Name);
            }
            var map = inputMap.Clone();
            var voronoiTriangulation = new VoronoiAlgorithm(map);
            double algmParamtrAngle = 45;
            voronoiTriangulation.Process(algmParamtrAngle);

        }
    }
}
