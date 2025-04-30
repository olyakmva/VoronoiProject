using GeomObjectsLib;
using System;

namespace MapDataLib
{
    public class ModelOfNineIntersections
    {
        #region Fields
        /// <summary>
        /// Матрица 9 пересечений
        /// </summary>
        private bool[,] matrixofnineintersections;
        /// <summary>
        /// Преимущество строки над столбцом (сначала строка, потом столбец)
        /// </summary>
        MapObjItem MapObjItemLine;
        MapObjItem MapObjItemColumn;
        /// <summary>
        /// Линия и точка
        /// </summary>
        public bool CanBeGeneralized = true;
        /// <summary>
        /// 2 линии
        /// </summary>
        public MapPoint PointIntesection;

        #endregion

        #region Construstors
        public ModelOfNineIntersections()
        {
            matrixofnineintersections = new bool[3, 3];
        }

        public ModelOfNineIntersections(MapObjItem mapDataLine, MapObjItem mapDataColumn)
        {
            matrixofnineintersections = new bool[3, 3];
            this.MapObjItemLine = mapDataLine;
            this.MapObjItemColumn = mapDataColumn;
            bool IsObjectsSimilar = true;
            if (mapDataLine.Points.Count != mapDataColumn.Points.Count)
            {
                IsObjectsSimilar = false;
            }
            if (IsObjectsSimilar)
            {
                for (int i = 0; i < mapDataLine.Points.Count; i++)
                {
                    if (Math.Abs(mapDataLine.Points[i].X - MapObjItemColumn.Points[i].X) > double.Epsilon || Math.Abs(mapDataLine.Points[i].Y - MapObjItemColumn.Points[i].Y) > double.Epsilon)
                    {
                        IsObjectsSimilar = false;
                    }
                }
            }
            if (!IsObjectsSimilar)
            {
                CalculatingIntersections();
            }
        }
        #endregion

        #region TypesOfIntersections
        //Объекты равны
        // T*F 
        // **F
        // FF*
        public bool ObjectsAreEquals()
        {
            if (matrixofnineintersections[0, 0] == true &&
                matrixofnineintersections[0, 2] == false &&
                matrixofnineintersections[1, 2] == false &&
                matrixofnineintersections[2, 0] == false &&
                matrixofnineintersections[2, 1] == false)
                return true;
            else
            {
                return false;
            }
        }

        //Объекты не пересекаются
        // FF* 
        // FF*
        // ***
        public bool ObjectsAreDisjoint()
        {
            if (matrixofnineintersections[0, 0] == false &&
                matrixofnineintersections[0, 1] == false &&
                matrixofnineintersections[1, 0] == false &&
                matrixofnineintersections[1, 1] == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Объекты касаются
        // FT*        F**        F**   
        // ***   OR   T**   OR   *T*
        // ***        ***        ***
        public bool ObjectsAreMeets()
        {
            if (matrixofnineintersections[0, 0] == false && (
                matrixofnineintersections[0, 1] == true ||
                matrixofnineintersections[1, 0] == true ||
                matrixofnineintersections[1, 1] == true))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //Объект содержится в другом
        // T**
        // ***
        // FF*

        public bool ObjectsAreContains()
        {
            if (matrixofnineintersections[0, 0] == true &&
               matrixofnineintersections[2, 1] == false &&
               matrixofnineintersections[2, 0] == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Объект покрывает другой
        // T**        *T*        ***        ***
        // ***   OR   ***   OR   T**   OR   *T*
        // FF*        FF*        FF*        FF*
        public bool ObjectsAreCovers()
        {
            if (matrixofnineintersections[2, 0] == false &&
               matrixofnineintersections[2, 1] == false && (
               matrixofnineintersections[0, 0] == true ||
               matrixofnineintersections[0, 1] == true ||
               matrixofnineintersections[1, 0] == true ||
               matrixofnineintersections[1, 1] == true))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //объекты пересекаются
        // T**        *T*        ***        ***
        // ***   OR   ***   OR   T**   OR   *T*
        // ***        ***        ***        ***
        public bool ObjectsAreIntersects()
        {
            if (matrixofnineintersections[0, 0] == true ||
               matrixofnineintersections[0, 1] == true ||
               matrixofnineintersections[1, 0] == true ||
               matrixofnineintersections[1, 1] == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //1 объект в другом
        // T*F    
        // **F   
        // *** 
        public bool ObjectsAreWithin()
        {
            if (matrixofnineintersections[0, 0] == true &&
               matrixofnineintersections[0, 2] == false &&
               matrixofnineintersections[1, 2] == false)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //объекты пересекаются
        // T*F        *TF        **F        **F
        // **F   OR   **F   OR   T*F   OR   *TF
        // ***        ***        ***        ***
        public bool ObjectsAreCoveredBy()
        {
            if ((matrixofnineintersections[0, 0] == true
                || matrixofnineintersections[0, 1] == true
                || matrixofnineintersections[1, 0] == true
                || matrixofnineintersections[1, 1] == true)
                && (matrixofnineintersections[0, 2] == false &&
             matrixofnineintersections[1, 2] == false))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        public void CalculatingIntersections()
        {
            switch (MapObjItemLine.Geometry)
            {
                case GeometryType.Point when MapObjItemColumn.Geometry == GeometryType.Point
                || (MapObjItemColumn.Geometry == GeometryType.Line && MapObjItemColumn.Points.Count == 1 && MapObjItemLine.Geometry == GeometryType.Point)
                || (MapObjItemColumn.Geometry == GeometryType.Polygon && MapObjItemColumn.Points.Count == 1 && MapObjItemLine.Geometry == GeometryType.Point)
                || (MapObjItemLine.Geometry == GeometryType.Line && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Geometry == GeometryType.Point)
                || (MapObjItemLine.Geometry == GeometryType.Polygon && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Geometry == GeometryType.Point)
                || (MapObjItemLine.Geometry == GeometryType.Line && MapObjItemColumn.Geometry == GeometryType.Line && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Points.Count == 1)
                || (MapObjItemLine.Geometry == GeometryType.Polygon && MapObjItemColumn.Geometry == GeometryType.Polygon && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Points.Count == 1)
                || (MapObjItemLine.Geometry == GeometryType.Line && MapObjItemColumn.Geometry == GeometryType.Polygon && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Points.Count == 1)
                || (MapObjItemLine.Geometry == GeometryType.Polygon && MapObjItemColumn.Geometry == GeometryType.Line && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Points.Count == 1):
                    PointPoint();
                    break;
                case GeometryType.Point when MapObjItemColumn.Geometry == GeometryType.Line
                || (MapObjItemLine.Geometry == GeometryType.Line && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Geometry == GeometryType.Line && MapObjItemColumn.Points.Count>1)
                || (MapObjItemLine.Geometry == GeometryType.Polygon && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Geometry == GeometryType.Line && MapObjItemColumn.Points.Count>1):
                    PointLine();
                    break;
                case GeometryType.Point when MapObjItemColumn.Geometry == GeometryType.Polygon
                || (MapObjItemLine.Geometry == GeometryType.Line && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Geometry == GeometryType.Polygon && MapObjItemColumn.Points.Count > 1)
                || (MapObjItemLine.Geometry == GeometryType.Polygon && MapObjItemLine.Points.Count == 1 && MapObjItemColumn.Geometry == GeometryType.Polygon && MapObjItemColumn.Points.Count > 1):
                    PointPolygon();
                    break;
                case GeometryType.Line when MapObjItemColumn.Geometry == GeometryType.Point
                || (MapObjItemColumn.Geometry == GeometryType.Line && MapObjItemColumn.Points.Count == 1 && MapObjItemLine.Geometry == GeometryType.Line && MapObjItemLine.Points.Count > 1)
                || (MapObjItemColumn.Geometry == GeometryType.Polygon && MapObjItemColumn.Points.Count == 1 && MapObjItemLine.Geometry == GeometryType.Line && MapObjItemLine.Points.Count > 1):
                    LinePoint();
                    break;
                case GeometryType.Line when MapObjItemColumn.Geometry == GeometryType.Line && MapObjItemColumn.Points.Count > 1 && MapObjItemLine.Points.Count > 1:
                    LineLine();
                    break;
                case GeometryType.Line when MapObjItemColumn.Geometry == GeometryType.Polygon && MapObjItemColumn.Points.Count > 1 && MapObjItemLine.Points.Count > 1:
                    LinePolygon();
                    break;
                case GeometryType.Polygon when MapObjItemColumn.Geometry == GeometryType.Point && MapObjItemColumn.Points.Count > 1 && MapObjItemLine.Points.Count > 1
                || (MapObjItemColumn.Geometry == GeometryType.Line && MapObjItemColumn.Points.Count == 1 && MapObjItemLine.Geometry == GeometryType.Polygon && MapObjItemLine.Points.Count > 1)
                || (MapObjItemColumn.Geometry == GeometryType.Polygon && MapObjItemColumn.Points.Count == 1 && MapObjItemLine.Geometry == GeometryType.Polygon && MapObjItemLine.Points.Count > 1):
                    PolygonPoint();
                    break;
                case GeometryType.Polygon when MapObjItemColumn.Geometry == GeometryType.Line && MapObjItemColumn.Points.Count > 1 && MapObjItemLine.Points.Count > 1:
                    PolygonLine();
                    break;
                case GeometryType.Polygon when MapObjItemColumn.Geometry == GeometryType.Polygon && MapObjItemColumn.Points.Count > 1 && MapObjItemLine.Points.Count > 1:
                    PolygonPolygon();
                    break;
            }
        }

        //Когда происходит проверка пересечения точек, они либо совпадают, либо не пересекаются

        #region PointWithOther
        public void PointPoint()
        {
            bool PointsAreEquals = false;

            var x1 = MapObjItemLine.Points[0].X;
            var y1 = MapObjItemLine.Points[0].Y;

            Point point1 = new Point{X = x1, Y = y1};

            var x2 = MapObjItemColumn.Points[0].X;
            var y2 = MapObjItemColumn.Points[0].Y;

            Point point2 = new Point { X = x2, Y = y2};
            if (point1.DistanceToVertex(point2)<0.001)
            {
                PointsAreEquals = true;
                PointIntesection = MapObjItemLine.Points[0];
            }

            if (PointsAreEquals)
            {
                matrixofnineintersections[0, 0] = true;
            }

        }
        public void PointLine()
        {
            bool PointLiesOnLine = false;

            var x = MapObjItemLine.Points[0].X;
            var y = MapObjItemLine.Points[0].Y;
            double distance = double.MaxValue;

            bool flagonperpendicular = false;
            double angle = 0;
            for (int i = 1; i < MapObjItemColumn.Points.Count; i++)
            {
                double x1 = MapObjItemColumn.Points[i].X;
                double x2 = MapObjItemColumn.Points[i - 1].X;
                double y1 = MapObjItemColumn.Points[i].Y;
                double y2 = MapObjItemColumn.Points[i - 1].Y;

                double minx = Math.Min(x1, x2);
                double miny = Math.Min(y1, y2);

                double maxx = Math.Max(x1, x2);
                double maxy = Math.Max(y1, y2);

                if (Math.Abs(((x - x1) * (y2 - y1)) - ((y - y1) * (x2 - x1))) < double.Epsilon && x >= minx && x <= maxx && y >= miny && y <= maxy)
                {
                    PointIntesection = MapObjItemLine.Points[0];
                    PointLiesOnLine = true;
                }
                else
                {
                    Line line = new Line(new Point { X = x1, Y = y1 }, new Point { X = x2, Y = y2 });
                    var q = (new Point { X = x, Y = y });
                    var p = line.GetPerpendicularFoundationPoint(q);
                    if (line.GetDistance(q) < distance && p.X >= minx && p.X <= maxx && p.Y >= miny && p.Y <= maxy)
                    {
                        distance = line.GetDistance(q);
                        Vector vector1 = new Vector(x1 - x2, y1 - y2);
                        Vector vector2= new Vector(q.X -p.X , q.Y -p.Y);
                        angle = Vector.AngleOfVectors(vector2, vector1);
                        flagonperpendicular = true;
                    }
                }
            }

            if (PointLiesOnLine)
            {
                matrixofnineintersections[0, 0] = true;
                matrixofnineintersections[2, 0] = true;
            }
            else
            {
                var x1 = MapObjItemColumn.Points[0].X;
                var y1 = MapObjItemColumn.Points[0].Y;
                var x2 = MapObjItemColumn.Points[MapObjItemColumn.Points.Count - 1].X;
                var y2 = MapObjItemColumn.Points[MapObjItemColumn.Points.Count - 1].Y;
                Line line = new Line(new Point { X = x1, Y = y1 }, new Point { X = x2, Y = y2 });
                var q = (new Point { X = x, Y = y });
                var p = line.GetPerpendicularFoundationPoint(q);
                Vector vector1 = new Vector(x2 - x1, y2 - y1);
                Vector vector2 = new Vector(q.X - p.X, q.Y - p.Y);
                var totalangle = Vector.AngleOfVectors(vector2, vector1);
                if ((totalangle >0 && totalangle<=180 && angle >0 && angle <=180) ||(totalangle > 180 && totalangle <= 360 && angle > 180 && angle <= 360) && flagonperpendicular == true)
                {

                }
                else if (flagonperpendicular == false)
                {

                }
                else
                {
                    CanBeGeneralized = false;
                }

            }
        }
        public void PointPolygon()
        {
            bool PointLiesOnPolygon = false;

            var x = MapObjItemLine.Points[0].X;
            var y = MapObjItemLine.Points[0].Y;

            for (int i = 1; i < MapObjItemColumn.Points.Count; i++)
            {
                double x1 = MapObjItemColumn.Points[i].X;
                double x2 = MapObjItemColumn.Points[i - 1].X;
                double y1 = MapObjItemColumn.Points[i].Y;
                double y2 = MapObjItemColumn.Points[i - 1].Y;

                double minx = Math.Min(x1, x2);
                double miny = Math.Min(y1, y2);

                double maxx = Math.Max(x1, x2);
                double maxy = Math.Max(y1, y2);

                if (Math.Abs(((x - x1) * (y2 - y1)) - ((y - y1) * (x2 - x1))) < double.Epsilon && x >= minx && x <= maxx && y >= miny && y <= maxy)
                {
                    PointLiesOnPolygon = true;
                    PointIntesection = MapObjItemLine.Points[0];
                }
            }
            if (PointLiesOnPolygon == true)
            {
                matrixofnineintersections[1, 1] = true;
                matrixofnineintersections[2, 0] = true;
                return;
            }
            int j = MapObjItemColumn.Points.Count - 1;
            for (int i = 0; i < MapObjItemColumn.Points.Count; i++)
            {
                if ((MapObjItemColumn.Points[i].Y < y && MapObjItemColumn.Points[j].Y >= y || MapObjItemColumn.Points[j].Y < y && MapObjItemColumn.Points[i].Y >= y) &&
                     (MapObjItemColumn.Points[i].X + (y - MapObjItemColumn.Points[i].Y) / (MapObjItemColumn.Points[j].Y - MapObjItemColumn.Points[i].Y) * (MapObjItemColumn.Points[j].X - MapObjItemColumn.Points[i].X) < x))
                {
                    PointLiesOnPolygon = !PointLiesOnPolygon;
                }
                j = i;
            }

            if (PointLiesOnPolygon)
            {
                matrixofnineintersections[0, 0] = true;
                matrixofnineintersections[2, 0] = true;
            }
        }
        #endregion

        #region LineWithOther
        public void LinePoint()
        {
            bool LineContainsPoint = false;
            var x = MapObjItemColumn.Points[0].X;
            var y = MapObjItemColumn.Points[0].Y;
            double distance = double.MaxValue;
            bool flagonperpendicular = false;
            double angle = 0;
            for (int i = 1; i < MapObjItemLine.Points.Count; i++)
            {
                double x1 = MapObjItemLine.Points[i].X;
                double x2 = MapObjItemLine.Points[i - 1].X;
                double y1 = MapObjItemLine.Points[i].Y;
                double y2 = MapObjItemLine.Points[i - 1].Y;

                double minx = Math.Min(x1, x2);
                double miny = Math.Min(y1, y2);

                double maxx = Math.Max(x1, x2);
                double maxy = Math.Max(y1, y2);

                if (Math.Abs(((x - x1) * (y2 - y1)) - ((y - y1) * (x2 - x1))) < double.Epsilon && x >= minx && x <= maxx && y >= miny && y <= maxy)
                {
                    PointIntesection = MapObjItemColumn.Points[0];
                    LineContainsPoint = true;
                }
                else
                {
                    Line line = new Line(new Point { X = x1, Y = y1 }, new Point { X = x2, Y = y2 });
                    var q = (new Point { X = x, Y = y });
                    var p = line.GetPerpendicularFoundationPoint(q);
                    if (line.GetDistance(q) < distance && p.X >= minx && p.X <= maxx && p.Y >= miny && p.Y <= maxy)
                    {
                        distance = line.GetDistance(q);
                        Vector vector1 = new Vector(x1 - x2, y1 - y2);
                        Vector vector2 = new Vector(q.X - p.X, q.Y - p.Y);
                        angle = Vector.AngleOfVectors(vector2, vector1);
                        flagonperpendicular = true;
                    }
                }
            }

            if (LineContainsPoint)
            {
                matrixofnineintersections[0, 0] = true;
                matrixofnineintersections[0, 2] = true;
            }
            else
            {
                var x1 = MapObjItemLine.Points[0].X;
                var y1 = MapObjItemLine.Points[0].Y;
                var x2 = MapObjItemLine.Points[MapObjItemLine.Points.Count - 1].X;
                var y2 = MapObjItemLine.Points[MapObjItemLine.Points.Count - 1].Y;
                Line line = new Line(new Point { X = x1, Y = y1 }, new Point { X = x2, Y = y2 });
                var q = (new Point { X = x, Y = y });
                var p = line.GetPerpendicularFoundationPoint(q);
                Vector vector1 = new Vector(x2 - x1, y2 - y1);
                Vector vector2 = new Vector(q.X - p.X, q.Y - p.Y);
                var totalangle = Vector.AngleOfVectors(vector2, vector1);
                if ((totalangle > 0 && totalangle <= 180 && angle > 0 && angle <= 180) || (totalangle > 180 && totalangle <= 360 && angle > 180 && angle <= 360) && flagonperpendicular == true)
                {

                }
                else if (flagonperpendicular == false)
                {

                }
                else
                {
                    CanBeGeneralized = false;
                }

            }
        }
        public void LineLine()
        {
            for (int i = 1; i < MapObjItemLine.Points.Count; i++)
            {
                var a1 = MapObjItemLine.Points[i - 1].X;
                var b1 = MapObjItemLine.Points[i - 1].Y;
                var a2 = MapObjItemLine.Points[i].X;
                var b2 = MapObjItemLine.Points[i].Y;
                MapPoint mapPoint1 = new MapPoint(a1, b1, 1, 1);
                MapPoint mapPoint2 = new MapPoint(a2, b2, 1, 1);
                Line l1 = new Line(mapPoint1, mapPoint2);

                for (int j = 1; j < MapObjItemColumn.Points.Count; j++)
                {
                    double x1 = MapObjItemColumn.Points[j].X;
                    double x2 = MapObjItemColumn.Points[j - 1].X;
                    double y1 = MapObjItemColumn.Points[j].Y;
                    double y2 = MapObjItemColumn.Points[j - 1].Y;
                    MapPoint mapPoint3 = new MapPoint(x1, y1, 1, 1);
                    MapPoint mapPoint4 = new MapPoint(x2, y2, 1, 1);
                    Line l2 = new Line(mapPoint3, mapPoint4);
                    var point = l1.GetIntersectionPoint(l2);

                    var maxx1 = Math.Max(a1, a2);
                    var minx1 = Math.Min(a1, a2);
                    var maxy1 = Math.Max(b1, b2);
                    var miny1 = Math.Min(b1, b2);

                    var maxx2 = Math.Max(x1, x2);
                    var minx2 = Math.Min(x1, x2);
                    var maxy2 = Math.Max(y1, y2);
                    var miny2 = Math.Min(y1, y2);
                    if (point != null)
                    {
                        if (point.X < maxx1 && point.X < maxx2 &&
                        point.X > minx1 && point.X > minx2 &&
                        point.Y < maxy1 && point.Y < maxy2 &&
                        point.Y > miny1 && point.Y > miny2)
                        {
                            PointIntesection = new MapPoint(point.X, point.Y, 1, 1);
                            var x = point.X;
                            var y = point.Y;
                            Vector vector1 = new Vector(a1 - x, b1 - y);
                            Vector vector2 = new Vector(x1 - x, y1 - y);
                            Vector vector3 = new Vector(x2 - x, y2 - y);
                            Vector vector4 = new Vector(a2 - x, b2 - y);

                            var q1 = Vector.AngleOfVectors(vector2, vector1);
                            var q2 = Vector.AngleOfVectors(vector3, vector1);
                            var q3 = Vector.AngleOfVectors(vector4, vector1);

                            var p1 = Math.Max(q2, q1);
                            var p2 = Math.Min(q1, q2);
                            if (q3 > p2 && q3 < p1)
                            {
                                matrixofnineintersections[0, 0] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                matrixofnineintersections[2, 2] = true;
                                return;
                            }
                            else
                            {
                                matrixofnineintersections[1, 1] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                return;
                            }

                        }
                        else if (point.DistanceToVertex(MapObjItemLine.Points[i - 1]) < 0.001 || point.DistanceToVertex(MapObjItemLine.Points[i]) < 0.001 || point.DistanceToVertex(MapObjItemColumn.Points[j - 1]) < 0.001 || point.DistanceToVertex(MapObjItemColumn.Points[j]) < 0.001)
                        {
                            PointIntesection = new MapPoint(point.X, point.Y, 1, 1);
                            if (i != MapObjItemLine.Points.Count - 1)
                            {
                                var tmpx = MapObjItemLine.Points[i + 1].X;
                                var tmpy = MapObjItemLine.Points[i + 1].Y;
                                var x = point.X;
                                var y = point.Y;
                                Vector vector1 = new Vector(a1 - x, b1 - y);
                                Vector vector2 = new Vector(x1 - x, y1 - y);
                                Vector vector3 = new Vector(x2 - x, y2 - y);
                                Vector vector4 = new Vector(tmpx - x, tmpy - y);

                                var q1 = Vector.AngleOfVectors(vector2, vector1);
                                var q2 = Vector.AngleOfVectors(vector3, vector1);
                                var q3 = Vector.AngleOfVectors(vector4, vector1);

                                var p1 = Math.Max(q2, q1);
                                var p2 = Math.Min(q1, q2);
                                if (q3 > p2 && q3 < p1)
                                {
                                    matrixofnineintersections[0, 0] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    matrixofnineintersections[2, 2] = true;
                                    return;
                                }
                                else
                                {
                                    matrixofnineintersections[1, 1] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    return;
                                }
                            }
                            else if (j != MapObjItemColumn.Points.Count - 1)
                            {
                                var tmpx = MapObjItemColumn.Points[j + 1].X;
                                var tmpy = MapObjItemColumn.Points[j + 1].Y;

                                var x = point.X;
                                var y = point.Y;
                                Vector vector1 = new Vector(a1 - x, b1 - y);
                                Vector vector2 = new Vector(a2 - x, b2 - y);
                                Vector vector3 = new Vector(x2 - x, y2 - y);
                                Vector vector4 = new Vector(tmpx - x, tmpy - y);

                                var q1 = Vector.AngleOfVectors(vector2, vector1);
                                var q2 = Vector.AngleOfVectors(vector3, vector1);
                                var q3 = Vector.AngleOfVectors(vector4, vector1);

                                var p2 = Math.Min(q1, q2);
                                if ((q3 >0 && p2>0 && q3<180 && p2<180) || (q3>180 && p2>180 && q3<360 && p2<360))
                                {
                                    matrixofnineintersections[1, 1] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    return; 
                                }
                                else
                                {
                                    matrixofnineintersections[0, 0] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    matrixofnineintersections[2, 2] = true;
                                    return;
                                }
                            }
                            else
                            {
                                matrixofnineintersections[1, 1] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
        public void LinePolygon()
        {
            for (int i = 1; i < MapObjItemLine.Points.Count; i++)
            {
                var a1 = MapObjItemLine.Points[i - 1].X;
                var b1 = MapObjItemLine.Points[i - 1].Y;
                var a2 = MapObjItemLine.Points[i].X;
                var b2 = MapObjItemLine.Points[i].Y;
                MapPoint mapPoint1 = new MapPoint(a1, b1, 1, 1);
                MapPoint mapPoint2 = new MapPoint(a2, b2, 1, 1);
                Line l1 = new Line(mapPoint1, mapPoint2);

                for (int j = 1; j < MapObjItemColumn.Points.Count; j++)
                {
                    double x1 = MapObjItemColumn.Points[j].X;
                    double x2 = MapObjItemColumn.Points[j - 1].X;
                    double y1 = MapObjItemColumn.Points[j].Y;
                    double y2 = MapObjItemColumn.Points[j - 1].Y;
                    MapPoint mapPoint3 = new MapPoint(x1, y1, 1, 1);
                    MapPoint mapPoint4 = new MapPoint(x2, y2, 1, 1);
                    Line l2 = new Line(mapPoint3, mapPoint4);
                    var point = l1.GetIntersectionPoint(l2);

                    var maxx1 = Math.Max(a1, a2);
                    var minx1 = Math.Min(a1, a2);
                    var maxy1 = Math.Max(b1, b2);
                    var miny1 = Math.Min(b1, b2);

                    var maxx2 = Math.Max(x1, x2);
                    var minx2 = Math.Min(x1, x2);
                    var maxy2 = Math.Max(y1, y2);
                    var miny2 = Math.Min(y1, y2);
                    if (point != null)
                    {
                        if (point.X < maxx1 && point.X < maxx2 &&
                        point.X > minx1 && point.X > minx2 &&
                        point.Y < maxy1 && point.Y < maxy2 &&
                        point.Y > miny1 && point.Y > miny2)
                        {
                            PointIntesection = new MapPoint(point.X, point.Y, 1, 1);
                            var x = point.X;
                            var y = point.Y;
                            Vector vector1 = new Vector(a1 - x, b1 - y);
                            Vector vector2 = new Vector(x1 - x, y1 - y);
                            Vector vector3 = new Vector(x2 - x, y2 - y);
                            Vector vector4 = new Vector(a2 - x, b2 - y);

                            var q1 = Vector.AngleOfVectors(vector2, vector1);
                            var q2 = Vector.AngleOfVectors(vector3, vector1);
                            var q3 = Vector.AngleOfVectors(vector4, vector1);

                            var p1 = Math.Max(q2, q1);
                            var p2 = Math.Min(q1, q2);
                            if (q3 > p2 && q3 < p1)
                            {
                                matrixofnineintersections[0, 0] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                matrixofnineintersections[2, 2] = true;
                                return;
                            }
                            else
                            {
                                matrixofnineintersections[1, 1] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                return;
                            }

                        }
                        else if (point.DistanceToVertex(MapObjItemLine.Points[i - 1]) < 0.001 || point.DistanceToVertex(MapObjItemLine.Points[i]) < 0.001 || point.DistanceToVertex(MapObjItemColumn.Points[j - 1]) < 0.001 || point.DistanceToVertex(MapObjItemColumn.Points[j]) < 0.001)
                        {
                            PointIntesection = new MapPoint(point.X, point.Y, 1, 1);
                            if (i != MapObjItemLine.Points.Count - 1)
                            {
                                var tmpx = MapObjItemLine.Points[i + 1].X;
                                var tmpy = MapObjItemLine.Points[i + 1].Y;
                                var x = point.X;
                                var y = point.Y;
                                Vector vector1 = new Vector(a1 - x, b1 - y);
                                Vector vector2 = new Vector(x1 - x, y1 - y);
                                Vector vector3 = new Vector(x2 - x, y2 - y);
                                Vector vector4 = new Vector(tmpx - x, tmpy - y);

                                var q1 = Vector.AngleOfVectors(vector2, vector1);
                                var q2 = Vector.AngleOfVectors(vector3, vector1);
                                var q3 = Vector.AngleOfVectors(vector4, vector1);

                                var p1 = Math.Max(q2, q1);
                                var p2 = Math.Min(q1, q2);
                                if (q3 > p2 && q3 < p1)
                                {
                                    matrixofnineintersections[0, 0] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    matrixofnineintersections[2, 2] = true;
                                    return;
                                }
                                else
                                {
                                    matrixofnineintersections[1, 1] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    return;
                                }
                            }
                            else if (j != MapObjItemColumn.Points.Count - 1)
                            {
                                var tmpx = MapObjItemColumn.Points[j + 1].X;
                                var tmpy = MapObjItemColumn.Points[j + 1].Y;

                                var x = point.X;
                                var y = point.Y;
                                Vector vector1 = new Vector(a1 - x, b1 - y);
                                Vector vector2 = new Vector(a2 - x, b2 - y);
                                Vector vector3 = new Vector(x2 - x, y2 - y);
                                Vector vector4 = new Vector(tmpx - x, tmpy - y);

                                var q1 = Vector.AngleOfVectors(vector2, vector1);
                                var q2 = Vector.AngleOfVectors(vector3, vector1);
                                var q3 = Vector.AngleOfVectors(vector4, vector1);

                                var p2 = Math.Min(q1, q2);
                                if ((q3 > 0 && p2 > 0 && q3 < 180 && p2 < 180) || (q3 > 180 && p2 > 180 && q3 < 360 && p2 < 360))
                                {
                                    matrixofnineintersections[1, 1] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    return;
                                }
                                else
                                {
                                    matrixofnineintersections[0, 0] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    matrixofnineintersections[2, 2] = true;
                                    return;
                                }
                            }
                            else
                            {
                                matrixofnineintersections[1, 1] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PolygonWithOther
        public void PolygonPoint()
        {
            bool PolygonContainsPoint = false;

            var x = MapObjItemColumn.Points[0].X;
            var y = MapObjItemColumn.Points[0].Y;

            for (int i = 1; i < MapObjItemLine.Points.Count; i++)
            {
                double x1 = MapObjItemLine.Points[i].X;
                double x2 = MapObjItemLine.Points[i - 1].X;
                double y1 = MapObjItemLine.Points[i].Y;
                double y2 = MapObjItemLine.Points[i - 1].Y;

                double minx = Math.Min(x1, x2);
                double miny = Math.Min(y1, y2);

                double maxx = Math.Max(x1, x2);
                double maxy = Math.Max(y1, y2);

                if (Math.Abs(((x - x1) * (y2 - y1)) - ((y - y1) * (x2 - x1))) < double.Epsilon && x >= minx && x <= maxx && y >= miny && y <= maxy)
                {
                    PointIntesection = MapObjItemColumn.Points[0];
                    PolygonContainsPoint = true;
                }
            }
            if (PolygonContainsPoint == true)
            {
                matrixofnineintersections[1, 1] = true;
                matrixofnineintersections[0, 2] = true;
                return;
            }
            int j = MapObjItemLine.Points.Count - 1;
            for (int i = 0; i < MapObjItemLine.Points.Count; i++)
            {
                if ((MapObjItemLine.Points[i].Y < y && MapObjItemLine.Points[j].Y >= y || MapObjItemLine.Points[j].Y < y && MapObjItemLine.Points[i].Y >= y) &&
                     (MapObjItemLine.Points[i].X + (y - MapObjItemLine.Points[i].Y) / (MapObjItemLine.Points[j].Y - MapObjItemLine.Points[i].Y) * (MapObjItemLine.Points[j].X - MapObjItemLine.Points[i].X) < x))
                {
                    PolygonContainsPoint = !PolygonContainsPoint;
                }
                j = i;
            }

            if (PolygonContainsPoint)
            {
                matrixofnineintersections[0, 0] = true;
                matrixofnineintersections[0, 2] = true;
            }
        }
        public void PolygonLine()
        {
            for (int i = 1; i < MapObjItemLine.Points.Count; i++)
            {
                var a1 = MapObjItemLine.Points[i - 1].X;
                var b1 = MapObjItemLine.Points[i - 1].Y;
                var a2 = MapObjItemLine.Points[i].X;
                var b2 = MapObjItemLine.Points[i].Y;
                MapPoint mapPoint1 = new MapPoint(a1, b1, 1, 1);
                MapPoint mapPoint2 = new MapPoint(a2, b2, 1, 1);
                Line l1 = new Line(mapPoint1, mapPoint2);

                for (int j = 1; j < MapObjItemColumn.Points.Count; j++)
                {
                    double x1 = MapObjItemColumn.Points[j].X;
                    double x2 = MapObjItemColumn.Points[j - 1].X;
                    double y1 = MapObjItemColumn.Points[j].Y;
                    double y2 = MapObjItemColumn.Points[j - 1].Y;
                    MapPoint mapPoint3 = new MapPoint(x1, y1, 1, 1);
                    MapPoint mapPoint4 = new MapPoint(x2, y2, 1, 1);
                    Line l2 = new Line(mapPoint3, mapPoint4);
                    var point = l1.GetIntersectionPoint(l2);

                    var maxx1 = Math.Max(a1, a2);
                    var minx1 = Math.Min(a1, a2);
                    var maxy1 = Math.Max(b1, b2);
                    var miny1 = Math.Min(b1, b2);

                    var maxx2 = Math.Max(x1, x2);
                    var minx2 = Math.Min(x1, x2);
                    var maxy2 = Math.Max(y1, y2);
                    var miny2 = Math.Min(y1, y2);
                    if (point != null)
                    {
                        if (point.X < maxx1 && point.X < maxx2 &&
                        point.X > minx1 && point.X > minx2 &&
                        point.Y < maxy1 && point.Y < maxy2 &&
                        point.Y > miny1 && point.Y > miny2)
                        {
                            PointIntesection = new MapPoint(point.X, point.Y, 1, 1);
                            var x = point.X;
                            var y = point.Y;
                            Vector vector1 = new Vector(a1 - x, b1 - y);
                            Vector vector2 = new Vector(x1 - x, y1 - y);
                            Vector vector3 = new Vector(x2 - x, y2 - y);
                            Vector vector4 = new Vector(a2 - x, b2 - y);

                            var q1 = Vector.AngleOfVectors(vector2, vector1);
                            var q2 = Vector.AngleOfVectors(vector3, vector1);
                            var q3 = Vector.AngleOfVectors(vector4, vector1);

                            var p1 = Math.Max(q2, q1);
                            var p2 = Math.Min(q1, q2);
                            if (q3 > p2 && q3 < p1)
                            {
                                matrixofnineintersections[0, 0] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                matrixofnineintersections[2, 2] = true;
                                return;
                            }
                            else
                            {
                                matrixofnineintersections[1, 1] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                return;
                            }

                        }
                        else if (point.DistanceToVertex(MapObjItemLine.Points[i - 1]) < 0.001 || point.DistanceToVertex(MapObjItemLine.Points[i]) < 0.001 || point.DistanceToVertex(MapObjItemColumn.Points[j - 1]) < 0.001 || point.DistanceToVertex(MapObjItemColumn.Points[j]) < 0.001)
                            {
                            PointIntesection = new MapPoint(point.X, point.Y, 1, 1);
                            if (i != MapObjItemLine.Points.Count - 1)
                            {
                                var tmpx = MapObjItemLine.Points[i + 1].X;
                                var tmpy = MapObjItemLine.Points[i + 1].Y;
                                var x = point.X;
                                var y = point.Y;
                                Vector vector1 = new Vector(a1 - x, b1 - y);
                                Vector vector2 = new Vector(x1 - x, y1 - y);
                                Vector vector3 = new Vector(x2 - x, y2 - y);
                                Vector vector4 = new Vector(tmpx - x, tmpy - y);

                                var q1 = Vector.AngleOfVectors(vector2, vector1);
                                var q2 = Vector.AngleOfVectors(vector3, vector1);
                                var q3 = Vector.AngleOfVectors(vector4, vector1);

                                var p1 = Math.Max(q2, q1);
                                var p2 = Math.Min(q1, q2);
                                if (q3 > p2 && q3 < p1)
                                {
                                    matrixofnineintersections[0, 0] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    matrixofnineintersections[2, 2] = true;
                                    return;
                                }
                                else
                                {
                                    matrixofnineintersections[1, 1] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    return;
                                }
                            }
                            else if (j != MapObjItemColumn.Points.Count - 1)
                            {
                                var tmpx = MapObjItemColumn.Points[j + 1].X;
                                var tmpy = MapObjItemColumn.Points[j + 1].Y;

                                var x = point.X;
                                var y = point.Y;
                                Vector vector1 = new Vector(a1 - x, b1 - y);
                                Vector vector2 = new Vector(a2 - x, b2 - y);
                                Vector vector3 = new Vector(x2 - x, y2 - y);
                                Vector vector4 = new Vector(tmpx - x, tmpy - y);

                                var q1 = Vector.AngleOfVectors(vector2, vector1);
                                var q2 = Vector.AngleOfVectors(vector3, vector1);
                                var q3 = Vector.AngleOfVectors(vector4, vector1);

                                var p2 = Math.Min(q1, q2);
                                if ((q3 > 0 && p2 > 0 && q3 < 180 && p2 < 180) || (q3 > 180 && p2 > 180 && q3 < 360 && p2 < 360))
                                {
                                    matrixofnineintersections[1, 1] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    return;
                                }
                                else
                                {
                                    matrixofnineintersections[0, 0] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    matrixofnineintersections[2, 2] = true;
                                    return;
                                }
                            }
                            else
                            {
                                matrixofnineintersections[1, 1] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
        public void PolygonPolygon()
        {
            for (int i = 1; i < MapObjItemLine.Points.Count; i++)
            {
                var a1 = MapObjItemLine.Points[i - 1].X;
                var b1 = MapObjItemLine.Points[i - 1].Y;
                var a2 = MapObjItemLine.Points[i].X;
                var b2 = MapObjItemLine.Points[i].Y;
                MapPoint mapPoint1 = new MapPoint(a1, b1, 1, 1);
                MapPoint mapPoint2 = new MapPoint(a2, b2, 1, 1);
                Line l1 = new Line(mapPoint1, mapPoint2);

                for (int j = 1; j < MapObjItemColumn.Points.Count; j++)
                {
                    double x1 = MapObjItemColumn.Points[j].X;
                    double x2 = MapObjItemColumn.Points[j - 1].X;
                    double y1 = MapObjItemColumn.Points[j].Y;
                    double y2 = MapObjItemColumn.Points[j - 1].Y;
                    MapPoint mapPoint3 = new MapPoint(x1, y1, 1, 1);
                    MapPoint mapPoint4 = new MapPoint(x2, y2, 1, 1);
                    Line l2 = new Line(mapPoint3, mapPoint4);
                    var point = l1.GetIntersectionPoint(l2);

                    var maxx1 = Math.Max(a1, a2);
                    var minx1 = Math.Min(a1, a2);
                    var maxy1 = Math.Max(b1, b2);
                    var miny1 = Math.Min(b1, b2);

                    var maxx2 = Math.Max(x1, x2);
                    var minx2 = Math.Min(x1, x2);
                    var maxy2 = Math.Max(y1, y2);
                    var miny2 = Math.Min(y1, y2);
                    if (point != null)
                    {
                        if (point.X < maxx1 && point.X < maxx2 &&
                        point.X > minx1 && point.X > minx2 &&
                        point.Y < maxy1 && point.Y < maxy2 &&
                        point.Y > miny1 && point.Y > miny2)
                        {
                            PointIntesection = new MapPoint(point.X, point.Y, 1, 1);
                            var x = point.X;
                            var y = point.Y;
                            Vector vector1 = new Vector(a1 - x, b1 - y);
                            Vector vector2 = new Vector(x1 - x, y1 - y);
                            Vector vector3 = new Vector(x2 - x, y2 - y);
                            Vector vector4 = new Vector(a2 - x, b2 - y);

                            var q1 = Vector.AngleOfVectors(vector2, vector1);
                            var q2 = Vector.AngleOfVectors(vector3, vector1);
                            var q3 = Vector.AngleOfVectors(vector4, vector1);

                            var p1 = Math.Max(q2, q1);
                            var p2 = Math.Min(q1, q2);
                            if (q3 > p2 && q3 < p1)
                            {
                                matrixofnineintersections[0, 0] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                matrixofnineintersections[2, 2] = true;
                                return;
                            }
                            else
                            {
                                matrixofnineintersections[1, 1] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                return;
                            }

                        }
                        else if (point.DistanceToVertex(MapObjItemLine.Points[i - 1]) < 0.001 || point.DistanceToVertex(MapObjItemLine.Points[i]) < 0.001 || point.DistanceToVertex(MapObjItemColumn.Points[j - 1]) < 0.001 || point.DistanceToVertex(MapObjItemColumn.Points[j]) < 0.001)
                        {
                            PointIntesection = new MapPoint(point.X, point.Y, 1, 1);
                            if (i != MapObjItemLine.Points.Count - 1)
                            {
                                var tmpx = MapObjItemLine.Points[i + 1].X;
                                var tmpy = MapObjItemLine.Points[i + 1].Y;
                                var x = point.X;
                                var y = point.Y;
                                Vector vector1 = new Vector(a1 - x, b1 - y);
                                Vector vector2 = new Vector(x1 - x, y1 - y);
                                Vector vector3 = new Vector(x2 - x, y2 - y);
                                Vector vector4 = new Vector(tmpx - x, tmpy - y);

                                var q1 = Vector.AngleOfVectors(vector2, vector1);
                                var q2 = Vector.AngleOfVectors(vector3, vector1);
                                var q3 = Vector.AngleOfVectors(vector4, vector1);

                                var p1 = Math.Max(q2, q1);
                                var p2 = Math.Min(q1, q2);
                                if (q3 > p2 && q3 < p1)
                                {
                                    matrixofnineintersections[0, 0] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    matrixofnineintersections[2, 2] = true;
                                    return;
                                }
                                else
                                {
                                    matrixofnineintersections[1, 1] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    return;
                                }
                            }
                            else if (j != MapObjItemColumn.Points.Count - 1)
                            {
                                var tmpx = MapObjItemColumn.Points[j + 1].X;
                                var tmpy = MapObjItemColumn.Points[j + 1].Y;

                                var x = point.X;
                                var y = point.Y;
                                Vector vector1 = new Vector(a1 - x, b1 - y);
                                Vector vector2 = new Vector(a2 - x, b2 - y);
                                Vector vector3 = new Vector(x2 - x, y2 - y);
                                Vector vector4 = new Vector(tmpx - x, tmpy - y);

                                var q1 = Vector.AngleOfVectors(vector2, vector1);
                                var q2 = Vector.AngleOfVectors(vector3, vector1);
                                var q3 = Vector.AngleOfVectors(vector4, vector1);

                                var p2 = Math.Min(q1, q2);
                                if ((q3 > 0 && p2 > 0 && q3 < 180 && p2 < 180) || (q3 > 180 && p2 > 180 && q3 < 360 && p2 < 360))
                                {
                                    matrixofnineintersections[1, 1] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    return;
                                }
                                else
                                {
                                    matrixofnineintersections[0, 0] = true;
                                    matrixofnineintersections[0, 2] = true;
                                    matrixofnineintersections[2, 0] = true;
                                    matrixofnineintersections[2, 2] = true;
                                    return;
                                }
                            }
                            else
                            {
                                matrixofnineintersections[1, 1] = true;
                                matrixofnineintersections[0, 2] = true;
                                matrixofnineintersections[2, 0] = true;
                                return;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
