using GeomObjectsLib;
using System;
using System.Collections.Generic;

namespace DiagramVoronoi
{
    public class Triangle
    {
        public Point Center { get; private set; }
        public double RadiusCircle { get; private set; }
        public Point[] Points { get; }
        public EdgeTriangle[] Edges { get; }
        internal Triangle(Point point1, Point point2, Point point3)
        {
            if (Analyser.IsOneLine(point1, point2, point3)) throw new ArgumentException("Points on one line");

            Edges = new EdgeTriangle[3];

            /*
            Данный конструктор вызывается при создании триангуляции, алгоритм которого основан на обходе внешних границ по\против часовой стрелки.
            Поэтому строго важно, чтобы стороны данного треугольника также повторяли направления по\против часовой стрелке.
            В дальнейшем при вызывании второго конструктора это условие не особо важно.
            Поэтому в данном случае
                Edges[0] = new EdgeTriangle(point1, point2);
                Edges[1] = new EdgeTriangle(point2, point3);
                Edges[2] = new EdgeTriangle(point3, point1);
            должно быть в if. НЕ ВЫТАСКИВАТЬ!!!
            */

            if (Analyser.IsCounterClockwise(point1, point2, point3) == true)
            {
                Points = new Point[3] { point1, point2, point3 };
                Edges[0] = new EdgeTriangle(point1, point2);
                Edges[1] = new EdgeTriangle(point2, point3);
                Edges[2] = new EdgeTriangle(point3, point1);
            }
            else
            {
                Points = new Point[3] { point1, point3, point2 };
                Edges[0] = new EdgeTriangle(point1, point3);
                Edges[1] = new EdgeTriangle(point3, point2);
                Edges[2] = new EdgeTriangle(point2, point1);
            }
            Edges[0].AddTriangle(this);
            Edges[1].AddTriangle(this);
            Edges[2].AddTriangle(this);

            UpdateCircumcircle(point1, point2, point3);
        }
        internal Triangle(EdgeTriangle edge1, EdgeTriangle edge2, EdgeTriangle edge3)
        {
            Edges = new EdgeTriangle[3] { edge1, edge2, edge3 };
            Edges[0].AddTriangle(this);
            Edges[1].AddTriangle(this);
            Edges[2].AddTriangle(this);

            List<Point> points = new List<Point>();
            if (!points.Contains(edge1.Point1)) points.Add(edge1.Point1);
            if (!points.Contains(edge1.Point2)) points.Add(edge1.Point2);
            if (!points.Contains(edge2.Point1)) points.Add(edge2.Point1);
            if (!points.Contains(edge2.Point2)) points.Add(edge2.Point2);
            if (!points.Contains(edge3.Point1)) points.Add(edge3.Point1);
            if (!points.Contains(edge3.Point2)) points.Add(edge3.Point2);

            if (Analyser.IsOneLine(points[0], points[1], points[2])) throw new ArgumentException("Points on one line");

            if (Analyser.IsCounterClockwise(points[0], points[1], points[2]) == true)
            {
                Points = new Point[3] { points[0], points[1], points[2] };
            }
            else
            {
                Points = new Point[3] { points[0], points[2], points[1] };
            }

            UpdateCircumcircle(points[0], points[1], points[2]);
        }
        /// <summary>
        /// Метод нахождения стороны в данном треугольнике, первая точка которого - containsPoint, вторая - следующая по часовой стрелке в треугольнике.
        /// </summary>
        public EdgeTriangle GetFirstEdgeWithPoint(Point containsPoint)
        {
            int index = 0;
            for (int i = 0; i < 3; i++)
            {
                if (Points[i].Equals(containsPoint))
                    index = i;
            }
            for (int i = 0; i < 3; i++)
            {
                if (Edges[i].Point1.Equals(Points[index]) && Edges[i].Point2.Equals(Points[(index + 1) % 3]))
                    return Edges[i];

                if (Edges[i].Point2.Equals(Points[index]) && Edges[i].Point1.Equals(Points[(index + 1) % 3]))
                    return Edges[i];
            }
            return null;
        }
        /// <summary>
        /// Метод нахождения стороны, не являющейся ни одной из notthis и включающий в себя точку containsPoint.
        /// </summary>
        public EdgeTriangle GetEdgeWithPoint(List<EdgeTriangle> notthis, Point containsPoint)
        {
            for (int i = 0; i < 3; i++)
            {
                if (notthis.Find(x => x.Equals(Edges[i])) != null) continue;
                if (Edges[i].ContainsPoint(containsPoint)) return Edges[i];
            }
            return null;
        }
        /// <summary>
        /// Метод, проверяющий находится ли точка за описанной окружностью данного треугольника.
        /// </summary>
        public bool IsPointInsideCircumcircle(Point point)
        {
            var d_squared = Math.Sqrt(Math.Pow(point.X - Center.X, 2) + Math.Pow(point.Y - Center.Y, 2));
            return d_squared < RadiusCircle;
        }
        /// <summary>
        /// Метод, вычисляющий центр и радиус описанной окружности.
        /// </summary>
        /// <exception cref="DivideByZeroException"></exception>
        private void UpdateCircumcircle(Point point1, Point point2, Point point3)
        {
            var p0 = point1;
            var p1 = point2;
            var p2 = point3;
            var dA = p0.X * p0.X + p0.Y * p0.Y;
            var dB = p1.X * p1.X + p1.Y * p1.Y;
            var dC = p2.X * p2.X + p2.Y * p2.Y;

            var aux1 = (dA * (p2.Y - p1.Y) + dB * (p0.Y - p2.Y) + dC * (p1.Y - p0.Y));
            var aux2 = -(dA * (p2.X - p1.X) + dB * (p0.X - p2.X) + dC * (p1.X - p0.X));
            var div = (2 * (p0.X * (p2.Y - p1.Y) + p1.X * (p0.Y - p2.Y) + p2.X * (p1.Y - p0.Y)));

            if (div == 0)
            {
                throw new DivideByZeroException();
            }

            var center = new Point(aux1 / div, aux2 / div);
            Center = center;
            RadiusCircle = Math.Sqrt(Math.Pow(center.X - p0.X, 2) + Math.Pow(center.Y - p0.Y, 2));
        }
    }
}
