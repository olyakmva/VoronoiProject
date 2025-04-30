using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MapDataLib
{
    // Данные слоя карты. В списке хранятся точки на карте  
    // слой одной геометрии 
    [Serializable]
    public class MapData
    {
        public Dictionary<int, List<MapPoint>> MapObjDictionary { get; }
        public GeometryType Geometry { get; }
        public int Count => GetAllVertices().Count;
        public string ColorName { get;  set; }

        public MapData( GeometryType type)
        {
            MapObjDictionary = new Dictionary<int, List<MapPoint>>();
            Geometry = type;
            ColorName = Colors.GetNext();
        }
        
        public List<MapPoint> GetAllVertices()
        {
            var resultList = new List<MapPoint>();
            foreach (var objPair in MapObjDictionary)
            {
                resultList.AddRange(objPair.Value);
            }
            return resultList;
        }
        
        public MapData Clone()
        {
            MapData clone;
            var bf = new BinaryFormatter();
            using (Stream fs = new FileStream("temp.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                bf.Serialize(fs, this);
            }
            using (Stream fs = new FileStream("temp.bin", FileMode.Open, FileAccess.Read, FileShare.None))
            {
                clone = (MapData)bf.Deserialize(fs);
            }
            return clone;
        }
        
        public void ClearWeights()
        {
            foreach (var chain in MapObjDictionary)
            {
                foreach (var vertex in chain.Value)
                {
                    vertex.Weight = 1;
                }
            }
        }
        public List<MapObjItem> GetMapObjItems()
        {
            var items = new List<MapObjItem>();
            foreach(var obj in MapObjDictionary)
            {
                var item = new MapObjItem()
                {
                    Id = obj.Key,
                    Points = obj.Value,
                    Geometry = this.Geometry
                };
                items.Add(item);
            }
            return items;
        }


    }
}
