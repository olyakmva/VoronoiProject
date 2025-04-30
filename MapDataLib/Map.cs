using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using GeomObjectsLib;

namespace MapDataLib
{
    [Serializable]
    public class Map:IEnumerable<MapData>
    {
        public List<MapData> MapLayers;

        public double Xmin, Xmax, Ymin, Ymax;
        public Map()
        {
            MapLayers = new List<MapData>();
            Xmin = double.MaxValue;
            Ymin = Xmin;
            Colors.Init();
        }
        
        public Map(List<MapData> mapLayers, double xmin, double ymin, double xmax, double ymax)
        {
            MapLayers = mapLayers;
            Xmin = xmin;
            Ymin = ymin;
            Xmax = xmax;
            Ymax = ymax;
            Colors.Init();
        }
        public List<MapObjItem> GetMapObjItems()
        {
            var list = new List<MapObjItem>();
            foreach(var item in MapLayers)
            {
                list.AddRange(item.GetMapObjItems());
            }
            return list;
        }

        public Square GetCoverage()
        {
            double size = Math.Max(Xmax - Xmin, Ymax - Ymin);
            return new Square()
            {
                LowLeft = new Point() { X = Xmin, Y = Ymin },
                UpLeft = new Point() { X = Xmin, Y = Ymin + size },
                UpRight = new Point() { X = Xmin + size, Y = Ymin + size },
                LowRight = new Point() { X = Xmin + size, Y = Ymin }
            };
        }

        void ComputeMinMaxValues(MapData mapData)
        {
            foreach (var pair in mapData.MapObjDictionary)
            {
                var vlist = pair.Value;
                var xmin = vlist.Min(point=> point.X);
                var xmax = vlist.Max(point => point.X);
                Xmin = Math.Min(xmin, Xmin);
                Xmax = Math.Max(xmax, Xmax);
                Ymax = Math.Max(Ymax, vlist.Max(point => point.Y));
                Ymin=  Math.Min(Ymin, vlist.Min(point => point.Y));
            }
        }

        public void Add(MapData mapData)
        {
            MapLayers.Add(mapData);
            ComputeMinMaxValues(mapData);
        }
        public Map Clone()
        {
            Map clone;
            var bf = new BinaryFormatter();
            using (Stream fs = new FileStream("temp.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bf.Serialize(fs, this);
            }
            using (Stream fs = new FileStream("temp.bin", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                clone = (Map)bf.Deserialize(fs);
            }
            return clone;
        }
        public MapData GetObjById(int id)
        {
            return MapLayers.Find(x => x.MapObjDictionary.ContainsKey(id));
        }

        public IEnumerator<MapData> GetEnumerator()
        {
            return MapLayers.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
