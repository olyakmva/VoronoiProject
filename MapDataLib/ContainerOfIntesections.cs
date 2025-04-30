using System.Collections.Generic;
using System.Linq;

namespace MapDataLib
{
    public class ContainerOfIntersections
    {
        /// <summary>
        /// Тут будут храниться матрицы 9 пересечений
        /// </summary>
        public List<List<ModelOfNineIntersections>> ModelOfNineIntersections;
        /// <summary>
        /// Тут хранятся все объекты
        /// </summary>
        public List<MapObjItem> MapObjItemsList { get; }
        /// <summary>
        /// Количество линий
        /// </summary>
        public int CountLines = 0;
        /// <summary>
        /// Количество точек
        /// </summary>
        public int CountPoints = 0;
        /// <summary>
        /// Количество полигонов
        /// </summary>
        public int CountPolygons = 0;

        public ContainerOfIntersections()
        {
            ModelOfNineIntersections = new List<List<ModelOfNineIntersections>>();
            MapObjItemsList = new List<MapObjItem>();
        }

        public void Add(MapObjItem mapObjItem)
        {
            switch (mapObjItem.Geometry)
            {
                case GeometryType.Line:
                    CountLines++;
                    break;
                case GeometryType.Point:
                    CountPoints++;
                    break;
                case GeometryType.Polygon:
                    CountPolygons++;
                    break;
            }
            MapObjItemsList.Add(mapObjItem);
            List<ModelOfNineIntersections> list = new List<ModelOfNineIntersections>();
            if (MapObjItemsList.Count != 1)
            {
                for (int i = 0; i < MapObjItemsList.Count - 1; i++)
                {
                    ModelOfNineIntersections m = new ModelOfNineIntersections(MapObjItemsList[i], mapObjItem);
                    ModelOfNineIntersections[i].Add(m);
                }
                for (int j = 0; j < MapObjItemsList.Count; j++)
                {
                    ModelOfNineIntersections m = new ModelOfNineIntersections(mapObjItem, MapObjItemsList[j]);
                    list.Add(m);
                }
                ModelOfNineIntersections.Add(list);
            }
            else
            {
                ModelOfNineIntersections m = new ModelOfNineIntersections(mapObjItem, mapObjItem);
                list.Add(m);
                ModelOfNineIntersections.Add(list);
            }
        }
        private int FindIndex(int objId)
        {
            return MapObjItemsList.FindIndex(t => t.Id == objId);
        }

        public List<ModelOfNineIntersections> GetListsOfModels(int objId)
        {
            int index = FindIndex(objId);
            var list = ModelOfNineIntersections[index];
            return list;
        }
        public List<ModelOfNineIntersections> GetListsOfModels(int objId, MapPoint startPoint, MapPoint endPoint)
        {
            int index = 0;
            foreach( var mapobj in MapObjItemsList)
            {
                if (mapobj.Id != objId)
                {
                    index++;
                    continue;
                }
                if(mapobj.Points.Contains(startPoint) && mapobj.Points.Contains(endPoint))
                {
                    return ModelOfNineIntersections[index];
                }
                index++;
            }
            
            return null;
        }


        public List<MapPoint> GetIntersectionPoints(int objId)
        {
            var list = GetListsOfModels(objId);
            var result = new List<MapPoint>();
            foreach (var matrix in list)
            {
                if (matrix.PointIntesection != null)
                    result.Add(matrix.PointIntesection);
            }
            return result.Distinct().ToList();
        }
    }
}