using GeomObjectsLib;
using MapDataLib;
using System;
using System.Collections.Generic;

namespace DiagramVoronoi
{
    /// <summary>
    /// Класс списан с класса MapPoint с одним изменением: данный класс может быть нескольких индексов (являтся смежной точкой разных слоёв карты).
    /// </summary>
    public class MultiMapPoint : Point, IComparable<MultiMapPoint>
    {
        public List<int> Ids { get; set; }

        public MultiMapPoint()
        {
            X = 0;
            Y = 0;
        }

        public MultiMapPoint(double coordX, double coordY, int id)
        {
            X = coordX;
            Y = coordY;
            Ids = new List<int>() { id };
        }
        public MultiMapPoint(MapPoint mp)
        {
            this.X = mp.X;
            this.Y = mp.Y;
            Ids = new List<int>() { mp.Id };
        }

        public override string ToString()
        {
            string ids = "";
            foreach (var v in Ids) ids += v + " ";
            return $"x={X} y={Y} ids={ids}";
        }
        public override bool Equals(object obj)
        {
            if (!(obj is MultiMapPoint other)) return false;
            return Math.Abs(other.X - X) < double.Epsilon && Math.Abs(other.Y - Y) < double.Epsilon;
        }

        public override int GetHashCode()
        {
            return (int)(X * 1000000 + Y);
        }

        public int CompareTo(MultiMapPoint other)
        {
            if (Math.Abs(other.X - X) < double.Epsilon && Math.Abs(other.Y - Y) < double.Epsilon)
            {
                return 0;
            }
            if (Math.Abs(other.X - X) < double.Epsilon)
            {
                if (other.Y < Y)
                    return 1;
                return -1;
            }
            if (other.X < X)
            {
                return 1;
            }
            return -1;
        }
    }
}

