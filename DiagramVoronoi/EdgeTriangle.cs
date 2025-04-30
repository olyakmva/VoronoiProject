using GeomObjectsLib;
using System;

namespace DiagramVoronoi
{
    public class EdgeTriangle
    {
        public Triangle[] Triangles { get; private set; }
        public int CountTriangles { get; private set; }
        public Point Point1 { get; }
        public Point Point2 { get; }
        public Point Center => new Point((Point1.X + Point2.X) / 2, (Point1.Y + Point2.Y) / 2);
        public EdgeTriangle(Point point1, Point point2)
        {
            Point1 = point1;
            Point2 = point2;
            Triangles = new Triangle[2];
            CountTriangles = 0;
        }
        public bool ContainsPoint(Point point)
        {
            return (Point1.Equals(point) || Point2.Equals(point));
        }
        public bool Equals(EdgeTriangle other)
        {
            if (other is null) return false;
            return Point1.Equals(other.Point1) && Point2.Equals(other.Point2) ||
                Point1.Equals(other.Point2) && Point2.Equals(other.Point1);
        }
        /// <summary>
        /// Метод нахождения треугольника, смежного с данной стороной, не являющийся notthis.
        /// </summary>
        public Triangle GetTriangleNotThis(Triangle notthis)
        {
            if (Triangles[0].Equals(notthis))
                return Triangles[1];
            else
                return Triangles[0];
        }
        /// <summary>
        /// Метод нахождения коненчой точки данной прямой, не являющийся notthis.
        /// </summary>
        public Point GetPointNotThis(Point notthis)
        {
            if (Point1.Equals(notthis)) return Point2;
            else return Point1;
        }
        public void AddTriangle(Triangle triangle)
        {
            if (CountTriangles == 2)
            {
                throw new IndexOutOfRangeException();
            }
            Triangles[CountTriangles] = triangle;
            CountTriangles++;
        }
        public void RemoveTriangle(Triangle remove)
        {
            if (Triangles[0] != null && Triangles[0].Equals(remove))
            {
                Triangles[0] = Triangles[1];
                Triangles[1] = null;
                CountTriangles--;
            }
            else if (Triangles[1] != null && Triangles[1].Equals(remove))
            {
                Triangles[1] = null;
                CountTriangles--;
            }
            //Если треугольник не является смежным с этой стороной - метод ничего не делает (Не вызывает ошибку).
        }
    }
}
