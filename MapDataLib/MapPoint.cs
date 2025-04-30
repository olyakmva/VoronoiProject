using GeomObjectsLib;
using System;

namespace MapDataLib
{
    // точка на карте. Имеет координаты, 
    //  идентификатор - к какому объекту принадлежит 
    [Serializable]
    public class MapPoint : Point, IComparable<MapPoint>
    {
        public int Id { get; set; }

        public double Weight { get; set; }

        public MapPoint()
        {
            X = 0;
            Y = 0;
            Weight = 1;
        }

        public MapPoint(double coordX, double coordY, int id, double w)
        {
            X = coordX;
            Y = coordY;
            Id = id;
            Weight = w;
        }

        public MapPoint(MapPoint mp)
        {
            this.X = mp.X;
            this.Y = mp.Y;
            this.Weight= mp.Weight;
            this.Id = mp.Id;
        }

        public override string ToString()
        {
            return $"x={X} y={Y} id={Id} Wt={Weight}";
        }
        public override bool Equals(object obj)
        {
            if (!(obj is MapPoint other)) return false;
            return Math.Abs(other.X - X) < double.Epsilon && Math.Abs(other.Y - Y) < double.Epsilon;
        }

        public override int GetHashCode()
        {
            return (int)(X * 1000000 + Y);
        }

        #region IComparable Members

        public int CompareTo(MapPoint other)
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
        #endregion
    }
}

