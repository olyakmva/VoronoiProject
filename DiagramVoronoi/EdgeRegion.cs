using GeomObjectsLib;
using MapDataLib;
using System.Collections.Generic;

namespace DiagramVoronoi
{
    /// <summary>
    /// Класс, описывающий сторону DiagrammRegion
    /// </summary>
    public class EdgeRegion
    {
        public bool IsIndexRegionsEquals { get; }
        public Point Point1 { get; }
        public Point Point2 { get; }
        /// <summary>
        /// Конструктор луча, выходящего из точки vertex и упирающегося в границу map. Прямая, дополняющая луч, проходит через точку point. 
        /// Переменная bidirectional указывает, является ли луч "двунаправленным".
        /// Двунаправленный луч - отрезок от одной из границ map до другой его границы.
        /// Если луч отдонаправленный, направление луча вычисляется при помощи дополнительной точки center и булевского значения isClockwise, указывающее взаимное расположение точек center, vertex, point.
        /// </summary>
        private EdgeRegion(Point vertex, Point point, Map map, Point center, bool isClockwise, bool bidirectional, bool isIndexRegionsEquals)
        {
            List<Point> points;
            if (point.X == vertex.X)
            {
                points = FindBordersPointPerpend(vertex.X, vertex.Y, map);
            }
            else
            {
                double a = (point.Y - vertex.Y) / (point.X - vertex.X);
                double b = vertex.Y - a * vertex.X;
                points = FindBordersPoint(a, b, map);
            }

            points.Add(vertex);
            points.Sort();
            int indexPoint = points.IndexOf(vertex);

            if (!bidirectional)
            {
                Point1 = vertex;
                if (Analyser.IsCounterClockwise(center, vertex, points[indexPoint + 1]) == isClockwise) Point2 = points[indexPoint + 1];
                else Point2 = points[indexPoint - 1];
            }
            else
            {
                Point1 = points[indexPoint - 1];
                Point2 = points[indexPoint + 1];
            }
            IsIndexRegionsEquals = isIndexRegionsEquals;
        }
        /// <summary>
        /// Конструктор отрезка, проходящего через точку center и упирающегося в границу map.
        /// Прямая, дополняющая отрезок, проходит через точки vertex1 и vertex2.
        /// </summary>
        private EdgeRegion(Point vertex1, Point vertex2, Map map, Point center, bool isIndexRegionsEquals)
        {
            List<Point> points;
            if (vertex1.X == vertex2.X)
            {
                points = FindBordersPointPerpend(vertex1.X, vertex1.Y, map);
            }
            else
            {
                double a = (vertex1.Y - vertex2.Y) / (vertex1.X - vertex2.X);
                double b = vertex2.Y - a * vertex2.X;
                points = FindBordersPoint(a, b, map);
            }

            points.Add(center);
            points.Sort();
            int indexPoint = points.IndexOf(center);

            Point1 = points[indexPoint + 1];
            Point2 = points[indexPoint - 1];
            IsIndexRegionsEquals = isIndexRegionsEquals;
        }
        /// <summary>
        /// Конструктор отрезка, выходящего из point1 в point2.
        /// </summary>
        private EdgeRegion(Point point1, Point point2, bool isIndexRegionsEquals)
        {
            Point1 = point1;
            Point2 = point2;
            IsIndexRegionsEquals = isIndexRegionsEquals;
        }
        /// <summary>
        /// Метод создания отрезка.
        /// </summary>
        public static EdgeRegion CreateDiaEdge(Point point1, Point point2, List<int> indexRegion1, List<int> indexRegion2)
        {
            foreach (var index1 in indexRegion1)
                foreach (var index2 in indexRegion2)
                    if (index1 == index2)
                        return new EdgeRegion(point1, point2, true);
            return new EdgeRegion(point1, point2, false);
        }
        /// <summary>
        /// Метод создания луча.
        /// </summary>
        public static EdgeRegion CreateDiaRay(Point vertex, Point point, Map map, Point center, bool isClockwise, List<int> indexRegion1, List<int> indexRegion2)
        {
            foreach (var index1 in indexRegion1)
                foreach (var index2 in indexRegion2)
                    if (index1 == index2)
                        return new EdgeRegion(vertex, point, map, center, isClockwise, false, true);
            return new EdgeRegion(vertex, point, map, center, isClockwise, false, false);
        }
        /// <summary>
        /// Метод создания луча (как однонаправленного, так и двунаправленного).
        /// </summary>
        public static EdgeRegion CreateDiaRayAround(Point vertex, Point point, Map map, Point center, bool isClockwise, List<int> indexRegion1, List<int> indexRegion2)
        {
            if (Analyser.IsCounterClockwise(center, vertex, point) == isClockwise)
            {
                foreach (var index1 in indexRegion1)
                    foreach (var index2 in indexRegion2)
                        if (index1 == index2)
                            return new EdgeRegion(vertex, point, map, center, isClockwise, true, true);
                return new EdgeRegion(vertex, point, map, center, isClockwise, true, false);
            }
            return null;
        }
        /// <summary>
        /// Метод создания отрезка, о котором мы знаем то, что он проходит за границами map.
        /// </summary>
        /// <returns>Null возвращается в том случае, если прямая полностью проходит на границами map.</returns>
        public static EdgeRegion CreateDiaEdgeAround(Point vertex1, Point vertex2, Map map, Point center, List<int> indexRegion1, List<int> indexRegion2)
        {
            if (vertex1.X < map.Xmin && vertex2.X < map.Xmin) return null;
            if (vertex1.X > map.Xmax && vertex2.X > map.Xmax) return null;
            if (vertex1.Y < map.Ymin && vertex2.Y < map.Ymin) return null;
            if (vertex1.Y > map.Ymax && vertex2.Y > map.Ymax) return null;

            foreach (var index1 in indexRegion1)
                foreach (var index2 in indexRegion2)
                    if (index1 == index2)
                        return new EdgeRegion(vertex1, vertex2, map, center, true);
            return new EdgeRegion(vertex1, vertex2, map, center, false);
        }
        /// <summary>
        /// Метод нахождения точек пересечения с границами map.
        /// </summary>
        private List<Point> FindBordersPoint(double a, double b, Map map)
        {
            if (a == 0)
            {
                return new List<Point>()
                {
                    new Point(map.Xmin, b),
                    new Point(map.Xmax, b)
                };
            }
            return new List<Point>()
            {
                new Point(map.Xmin, a * map.Xmin + b),
                new Point((map.Ymin - b) / a, map.Ymin),
                new Point((map.Ymax - b) / a, map.Ymax),
                new Point(map.Xmax, a * map.Xmax + b)
            };
        }
        /// <summary>
        /// Метод нахождения точек пересечения с границами map, когда прямая перпендикулярна они x (a - бесконечность).
        /// </summary>
        private List<Point> FindBordersPointPerpend(double x, double y, Map map)
        {
            return new List<Point>()
            {
                new Point(x, map.Ymin),
                new Point(x, map.Ymax)
            };

        }
    }
}
