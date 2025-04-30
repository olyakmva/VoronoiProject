using DiagramVoronoi;
using MapDataLib;
using System;
using System.Collections.Generic;

namespace Voronoi
{
    public class VoronoiAlgorithm
    {
        Map _map;
        Triangulation _triangulation;
        Dictionary<MapPoint, DiagrammRegion> _regions;
        public DiagrammRegion GetRegion(MapPoint point)
        {
            if (!_regions.ContainsKey(point))
                _regions.Add(point, _triangulation.GenerateRegionsVoronoiToPoint(point, _map));
            return _regions[point];
        }
        public VoronoiAlgorithm(Map inputMap)
        {
            _map = inputMap;
            _triangulation = CreatingMaxMap(inputMap);
            //_triangulation = CreatingTriangle(inputMap);
            _regions = new Dictionary<MapPoint, DiagrammRegion>();
        }
        /// <summary>
        /// Метод создания триангуляции.
        /// На вход принимает 3 точки, способные создать треугольник, 
        /// после чего в процессе добавления точек расширяет область триангуляции.
        /// </summary>
        private Triangulation CreatingTriangle(Map map)
        {
            //выбираем 3 точки для создания триангуляции
            var points = FindThirdPoint(map);
            Triangulation triangulation = new Triangulation(points[0], points[1], points[2]);

            //собираем все точки в триангуляцию
            foreach (var mapData in _map.MapLayers)
            {
                foreach (var pair in mapData.MapObjDictionary)
                {
                    foreach (var point in pair.Value)
                    {
                        triangulation.AddPoint(point);
                    }
                }
            }

            return triangulation;
        }
        /// <summary>
        /// Метод создания триангуляции.
        /// В начале создаёт максимально возможную фигуру, 
        /// после чего в процессе добавления точек видоизменяет лишь внутренние треугольники.
        /// </summary>
        private Triangulation CreatingMaxMap(Map map)
        {
            //выбираем 4 точки для создания триангуляции (края карты)
            var leftup = new MapPoint(map.Xmin, map.Ymax, 0, 0);
            var leftdown = new MapPoint(map.Xmin, map.Ymin, 0, 0);
            var rightup = new MapPoint(map.Xmax, map.Ymax, 0, 0);
            var rightdown = new MapPoint(map.Xmax, map.Ymin, 0, 0);
            Triangulation triangulation = new Triangulation(leftup, leftdown, rightdown, rightup);

            //собираем все точки в триангуляцию
            foreach (var mapData in map.MapLayers)
            {
                foreach (var pair in mapData.MapObjDictionary)
                {
                    foreach (var point in pair.Value)
                    {
                        triangulation.AddPoint(point);
                    }
                }
            }
            return triangulation;
        }
        /// <summary>
        /// Метод, позволяющий найти 3 точки, способные создать треугольник.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        private MapPoint[] FindThirdPoint(Map map)
        {
            MapPoint[] points = new MapPoint[3];
            int index = 0;
            foreach (var mapData in map.MapLayers)
            {
                foreach (var pair in mapData.MapObjDictionary)
                {
                    foreach (var point in pair.Value)
                    {
                        if (index < 2)
                        {
                            points[index] = point;
                            index++;
                        }
                        else
                        {
                            points[index] = point;
                            if (!Analyser.IsOneLine(points[0], points[1], points[2]))
                            {
                                return points;
                            }
                        }
                    }
                }
            }
            throw new ArgumentException("Impossible to find 3 points");
        }
        /// <summary>
        /// Метод упрощения
        /// </summary>
        public void Process(double parametrAngle)
        {
            List<DiagrammRegion> betweensRegion = new List<DiagrammRegion>();
            foreach (var mapData in _map.MapLayers)
            {
                betweensRegion.Clear();
                foreach (var pair in mapData.MapObjDictionary)
                {
                    if (pair.Value.Count < 3) continue;

                    int leftPoint = 0;
                    MapPoint[] points = pair.Value.ToArray();

                    for (int j = 1; j < points.Length - 1; j++)
                    {
                        betweensRegion.Add(GetRegion(points[j]));
                        bool isIntersect = IsIntersectPoint(GetRegion(points[leftPoint]), GetRegion(points[j]), GetRegion(points[j + 1]), betweensRegion, parametrAngle);
                        if (isIntersect)
                        {
                            pair.Value.Remove(points[j]);
                        }
                        else
                        {
                            leftPoint = j;
                            betweensRegion.Clear();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Метод, проверяющий возможность сокращения конкретной области curRegion.
        /// Смысл: проводится прямая через curRegion напрямую из leftRegion в rightRegion и проверяется наличие некорректных пересечений и преломлений.
        /// </summary>
        private bool IsIntersectPoint(DiagrammRegion leftRegion, DiagrammRegion curRegion, DiagrammRegion rightRegion, List<DiagrammRegion> betweenRegions, double parametrAngle)
        {
            var idsCurRegion = curRegion.Ids;

            if (idsCurRegion.Count == 1) //если не один, значит регион является связующим - удалять нельзя
            {
                if (leftRegion.Ids.Contains(idsCurRegion[0])
                    && rightRegion.Ids.Contains(idsCurRegion[0]))
                {
                    if (!Analyser.IsIntersectEdgesBetweenNotEqualsRegion(leftRegion, betweenRegions, rightRegion))
                    {
                        if (Analyser.CalculateAngleAround(leftRegion.Center, curRegion.Center, rightRegion.Center) < parametrAngle)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
