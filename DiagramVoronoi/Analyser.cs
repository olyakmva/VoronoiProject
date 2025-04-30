using GeomObjectsLib;
using System;
using System.Collections.Generic;

namespace DiagramVoronoi
{
    public class Analyser
    {
        /// <summary>
        /// Метод определяет расположены ли точки 1 2 3 по часовой стрелке.
        /// </summary>
        /// <returns>True - по часовой, False - против часовой, Null - на одной прямой</returns>
        public static bool? IsCounterClockwise(Point point1, Point point2, Point point3)
        {
            var result = (point2.X - point1.X) * (point3.Y - point1.Y) -
                (point3.X - point1.X) * (point2.Y - point1.Y);
            if (result == 0) return null;
            return result > 0;
        }
        /// <summary>
        /// Метод определяет расположены ли точки 1 2 3 на одной прямой.
        /// </summary>
        public static bool IsOneLine(Point point1, Point point2, Point point3)
        {
            return IsCounterClockwise(point1, point2, point3) == null;
        }
        /// <summary>
        /// Метод определяет пересекает ли линия, проведённая из start в end, какие либо "чужие" границы, принадлежащие регионам start betweenRegions end. 
        /// Чужие границы - границы между двумя регионами с разными индексами (IsIndexRegionEquals == false).
        /// </summary>
        public static bool IsIntersectEdgesBetweenNotEqualsRegion(DiagrammRegion start, IEnumerable<DiagrammRegion> betweenRegions, DiagrammRegion end)
        {
            foreach (var edge in start.Borders.GetEnumerator())
            {
                if (edge.IsIndexRegionsEquals) continue;

                var intersect = IsEdgeIntersect(start.Center, end.Center, edge.Point1, edge.Point2);
                if (intersect) return true;
            }
            foreach (var edge in end.Borders.GetEnumerator())
            {
                if (edge.IsIndexRegionsEquals) continue;

                var intersect = IsEdgeIntersect(start.Center, end.Center, edge.Point1, edge.Point2);
                if (intersect) return true;
            }
            foreach (var region in betweenRegions)
            {
                foreach (var edge in region.Borders.GetEnumerator())
                {
                    if (edge.IsIndexRegionsEquals) continue;

                    var intersect = IsEdgeIntersect(start.Center, end.Center, edge.Point1, edge.Point2);
                    if (intersect) return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Метод определяет пересекаются ли линии, проведённые из start1 в end1 и из start2 в end2.
        /// </summary>
        public static bool IsEdgeIntersect(Point start1, Point end1, Point start2, Point end2)
        {
            if (start1.X == end1.X)
            {
                if (start2.X == end2.X)
                {
                    if (start1.X == start2.X) return true;
                    return false;
                }

                double a2 = (start2.Y - end2.Y) / (start2.X - end2.X);
                double b2 = start2.Y - a2 * start2.X;

                double y = a2 * start1.X + b2;

                if (!((start1.Y >= y && y >= end1.Y) || (start1.Y <= y && y <= end1.Y))) return false;
                if (!((start2.Y >= y && y >= end2.Y) || (start2.Y <= y && y <= end2.Y))) return false;
                return true;
            }
            else
            {
                double a1 = (start1.Y - end1.Y) / (start1.X - end1.X);
                double b1 = start1.Y - a1 * start1.X;

                if (start2.X == end2.X)
                {
                    double y = a1 * start2.X + b1;

                    if (!((start1.Y >= y && y >= end1.Y) || (start1.Y <= y && y <= end1.Y))) return false;
                    if (!((start2.Y >= y && y >= end2.Y) || (start2.Y <= y && y <= end2.Y))) return false;
                    return true;
                }

                double a2 = (start2.Y - end2.Y) / (start2.X - end2.X);
                double b2 = start2.Y - a2 * start2.X;

                double x = (b1 - b2) / (a2 - a1);

                if (!((start1.X >= x && x >= end1.X) || (start1.X <= x && x <= end1.X))) return false;
                if (!((start2.X >= x && x >= end2.X) || (start2.X <= x && x <= end2.X))) return false;
                return true;
            }
        }
        /// <summary>
        /// Метод вычисляет угол в градусах между двумя линиями, проведёнными из first в second и из second в third.
        /// </summary>
        public static double CalculateAngleAround(Point first, Point second, Point third)
        {
            double A = Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2);
            double B = Math.Pow(third.X - second.X, 2) + Math.Pow(third.Y - second.Y, 2);
            double C = Math.Pow(first.X - third.X, 2) + Math.Pow(first.Y - third.Y, 2);
            double cos = (A + B - C) / (2 * Math.Sqrt(A) * Math.Sqrt(B));
            return 180 - Math.Acos(cos) * (180 / Math.PI);
        }
    }
}
