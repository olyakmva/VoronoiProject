using GeomObjectsLib;
using MapDataLib;
using System;
using System.Collections.Generic;

namespace DiagramVoronoi
{
    public class Triangulation
    {
        private enum Variant
        {
            MaxMap,
            Triangle
        }
        private Variant _variant;
        /// <summary>
        /// Все треугольники в триангуляции
        /// </summary>
        private List<Triangle> _triangles;
        /// <summary>
        /// Хранилище индексов каждой точки (используется при создании и вычислении регионов в графе вороного)
        /// </summary>
        private Dictionary<Point, List<int>> _memoryIds;
        /// <summary>
        /// Гриницы данной триангуляции (используется для быстрого определения того, новая точка внутри или снаружи триангуляции)
        /// </summary>
        private BilateralList<EdgeTriangle> _borders;

        //==========================================ДОЛЖЕН БЫТЬ ЗАМЕНЁН НА АДЕКВАТНЫЙ АЛГОРИТМ ПОИСКА ТРЕУГОЛЬНИКОВ=============================================  CouldDelete
        /// <summary>
        /// Хранилище треугольника со связанной с ним точкой (используется для быстрого нахождения треугольника, связанного с точкой) (можно заменить B-деревом)
        /// </summary>
        private Dictionary<Point, Triangle> _memoryPoint;

        public Triangulation(MapPoint p1, MapPoint p2, MapPoint p3)
        {
            _variant = Variant.Triangle;
            if (Analyser.IsOneLine(p1, p2, p3)) throw new ArgumentException("All points on one line");

            _triangles = new List<Triangle>();
            _memoryPoint = new Dictionary<Point, Triangle>();
            _memoryIds = new Dictionary<Point, List<int>>();
            _borders = new BilateralList<EdgeTriangle>();

            var first = new Triangle(p1, p2, p3);
            _triangles.Add(first);
            //Требуется для привязки новой точки к какому то треугольнику (с добавленияем метода поиска станет не нужным) CouldDelete
            _memoryPoint.Add(p1, first);
            _memoryPoint.Add(p2, first);
            _memoryPoint.Add(p3, first);
            foreach (var v in first.Edges)
                _borders.Add(v);

            _memoryIds.Add(p1, new List<int>() { p1.Id });
            _memoryIds.Add(p2, new List<int>() { p2.Id });
            _memoryIds.Add(p3, new List<int>() { p3.Id });
        }
        public Triangulation(MapPoint leftup, MapPoint leftdown, MapPoint rightdown, MapPoint rightup)
        {
            _variant = Variant.MaxMap;
            _triangles = new List<Triangle>();
            _memoryPoint = new Dictionary<Point, Triangle>();
            _memoryIds = new Dictionary<Point, List<int>>();
            _borders = new BilateralList<EdgeTriangle>();

            EdgeTriangle left = new EdgeTriangle(leftup, leftdown);
            EdgeTriangle down = new EdgeTriangle(leftdown, rightdown);
            EdgeTriangle diagonal = new EdgeTriangle(leftup, rightdown);
            EdgeTriangle right = new EdgeTriangle(rightdown, rightup);
            EdgeTriangle up = new EdgeTriangle(rightup, leftup);

            var first = new Triangle(left, down, diagonal);
            var second = new Triangle(diagonal, right, up);

            _triangles.Add(first);
            _triangles.Add(second);

            //Требуется для привязки новой точки к какому то треугольнику (с добавленияем метода поиска станет не нужным) CouldDelete
            _memoryPoint.Add(leftup, first);
            _memoryPoint.Add(leftdown, first);
            _memoryPoint.Add(rightdown, first);
            _memoryPoint.Add(rightup, second);

            _borders.Add(left);
            _borders.Add(down);
            _borders.Add(right);
            _borders.Add(up);

            _memoryIds.Add(leftup, new List<int>() { leftup.Id });
            _memoryIds.Add(leftdown, new List<int>() { leftdown.Id });
            _memoryIds.Add(rightdown, new List<int>() { rightdown.Id });
            _memoryIds.Add(rightup, new List<int>() { rightup.Id });
        }
        public void AddPoint(MapPoint point)
        {
            if (_memoryIds.ContainsKey(point))
            {
                if (!_memoryIds[point].Contains(point.Id))
                    _memoryIds[point].Add(point.Id);
            }
            else
            {
                _memoryIds.Add(point, new List<int>() { point.Id });

                bool IsOk = false;

                //Вариант, когда приходится проверить 2 возможных случая добавления точки (вне, на границе)
                if (_variant == Variant.Triangle)
                {
                    CheckLocationPointAndAddIfOutsideOrBorder(point, out IsOk);
                }
                //Упрощенный вариант, когда приходится проверить только 1 случай (на границе)
                else if (_variant == Variant.MaxMap)
                {
                    int indexBorder = 0;
                    foreach (var v in _borders.GetEnumerator())
                    {
                        if (Analyser.IsOneLine(point, v.Point1, v.Point2))
                        {
                            if (((v.Point1.X <= point.X && point.X <= v.Point2.X) || (v.Point1.X >= point.X && point.X >= v.Point2.X))
                                && ((v.Point1.Y <= point.Y && point.Y <= v.Point2.Y) || (v.Point1.Y >= point.Y && point.Y >= v.Point2.Y)))
                            {
                                AddPointOnBorder(v, point, indexBorder);
                                IsOk = true;
                            }
                        }
                        indexBorder++;
                    }
                }

                //Если точка не была добавлена выше, значит она находится внутри триангуляции
                if (!IsOk)
                    AddPointInside(point);
            }
        }
        /// <summary>
        /// Метод добавления точки внутрь триангуляции
        /// </summary>
        private void AddPointInside(Point point)
        {
            List<Triangle> insideTriandles = new List<Triangle>();
            Dictionary<EdgeTriangle, int> countsInsideEdges = new Dictionary<EdgeTriangle, int>();
            //Находятся все треугольники, для которых условие Делоне не выполняется (точка попадает в описанную окружность)
            //Параллельно считается количество сторон найденных треугольников

            //==========================================ДОЛЖЕН БЫТЬ ЗАМЕНЁН НА АДЕКВАТНЫЙ АЛГОРИТМ ПОИСКА ТРЕУГОЛЬНИКОВ=============================================
            foreach (var tria in _triangles)
            {
                if (tria.IsPointInsideCircumcircle(point))
                {
                    insideTriandles.Add(tria);
                    foreach (var edge in tria.Edges)
                    {
                        if (countsInsideEdges.ContainsKey(edge))
                            countsInsideEdges[edge]++;
                        else
                            countsInsideEdges.Add(edge, 1);
                    }
                }
            }
            //Удаление найденных треугольников
            foreach (var delete in insideTriandles)
                _triangles.Remove(delete);

            //Построение новых треугольников
            //Если сторона у найдённых треугольников встречалась 1 раз - с ней и добавленной точкой строится треугольник
            //Иначе сторона являлась смежной с двумя удалёнными треугольниками и тоже удаляется (забывается сборщиком мусора)
            Dictionary<Point, EdgeTriangle> newEdges = new Dictionary<Point, EdgeTriangle>();
            //Требуется для привязки новой точки к какому то треугольнику (с добавленияем метода поиска станет не нужным) CouldDelete
            Triangle newTrian = null;
            foreach (var pair in countsInsideEdges)
            {
                if (pair.Value == 1)
                {
                    var edge = pair.Key;
                    //Строятся прямые между точкой на прямой и новой точкой
                    if (!newEdges.ContainsKey(edge.Point1)) newEdges.Add(edge.Point1, new EdgeTriangle(edge.Point1, point));
                    if (!newEdges.ContainsKey(edge.Point2)) newEdges.Add(edge.Point2, new EdgeTriangle(point, edge.Point2));

                    foreach (var trian in insideTriandles)
                    {
                        edge.RemoveTriangle(trian);
                    }
                    newTrian = new Triangle(edge, newEdges[edge.Point1], newEdges[edge.Point2]);
                    _triangles.Add(newTrian);
                    //Требуется для привязки новой точки к какому то треугольнику (с добавленияем метода поиска станет не нужным) CouldDelete
                    if (insideTriandles.Contains(_memoryPoint[edge.Point1])) _memoryPoint[edge.Point1] = newTrian;
                    if (insideTriandles.Contains(_memoryPoint[edge.Point2])) _memoryPoint[edge.Point2] = newTrian;
                }
            }
            _memoryPoint[point] = newTrian;
        }
        /// <summary>
        /// Метод добавление точки снаружи триангуляции
        /// </summary>
        private void CheckLocationPointAndAddIfOutsideOrBorder(Point point, out bool IsOk)
        {
            //IsOk - указатель на случай, когда точка была добавлена в триангуляцию (точка может находится на границе триангуляции)
            IsOk = false;
            EdgeTriangle onBorderEdge = null;
            int indexBorderEdge = -1;
            List<EdgeTriangle> newEdges = new List<EdgeTriangle>();
            List<EdgeTriangle> deletedEdges = new List<EdgeTriangle>();
            Dictionary<Point, EdgeTriangle> newEdgesWithBorder = new Dictionary<Point, EdgeTriangle>();
            //Указатель на место в списке границ триангуляции, где происходит добавление новых треугольников
            int indexStart;

            //Вариант когда точка находится слева относительно начала и конца границ (снаружи, все стороны повернуты левой стороной наружу).
            //Следует идти в обе стороны, пока не будет найдёна часть границы, не видная из точки
            //isStartingIn - указатель на данный случай
            //(Исключает случай расположения точки на какой либо части границы триангуляции (так как проверка идёт относительно двух частей границ))
            bool isStartingIn = false;
            if (Analyser.IsCounterClockwise(_borders.Head.Point1, _borders.Head.Point2, point) == false
                && Analyser.IsCounterClockwise(_borders.Tail.Point1, _borders.Tail.Point2, point) == false)
            {
                isStartingIn = true;
                IsOk = true;
            }

            if (isStartingIn)
            {
                indexStart = _borders.Count;
                foreach (var v in _borders.GetBackEnumerator()) //первая часть
                {
                    if (Analyser.IsCounterClockwise(v.Point1, v.Point2, point) != false) break;
                    indexStart--;

                    AddPointOutside(newEdgesWithBorder, newEdges, deletedEdges, v, point);
                }
                foreach (var v in _borders.GetEnumerator()) //вторая часть
                {
                    if (Analyser.IsCounterClockwise(v.Point1, v.Point2, point) != false) break;

                    AddPointOutside(newEdgesWithBorder, newEdges, deletedEdges, v, point);
                }
            }
            //Требуется рассмотреть случай, когда точка распологается на одной из частей границы
            //В таком случае она расположена не слева от границы и на одной прямой с ней
            //onBorderEdge - указатель на данный случай (null - не располагается на стороне, не null - сторона найдена)
            else
            {
                indexStart = -1;
                bool isTakeStart = false;

                foreach (var v in _borders.GetEnumerator()) //единая часть
                {
                    if (Analyser.IsOneLine(point, v.Point1, v.Point2))
                    {
                        if (((v.Point1.X <= point.X && point.X <= v.Point2.X) || (v.Point1.X >= point.X && point.X >= v.Point2.X))
                            && ((v.Point1.Y <= point.Y && point.Y <= v.Point2.Y) || (v.Point1.Y >= point.Y && point.Y >= v.Point2.Y)))
                        {
                            onBorderEdge = v;
                            indexBorderEdge = indexStart + 1;
                        }
                    }
                    if (isTakeStart && Analyser.IsCounterClockwise(v.Point1, v.Point2, point) != false) break;
                    indexStart++;
                    if (Analyser.IsCounterClockwise(v.Point1, v.Point2, point) != false) continue;
                    isTakeStart = true;

                    AddPointOutside(newEdgesWithBorder, newEdges, deletedEdges, v, point);
                    IsOk = true;
                }
            }
            //Точка находится вне границ триангуляции и была успешно добавлена
            //Требуется провести необходимые преобразования в границах триангуляции
            if (IsOk)
            {
                bool isFirstNewEdge = true;
                foreach (var v in newEdges) //добавить две новые стороны в список границ триангуляции 
                {
                    if (v.CountTriangles == 1)
                    {
                        if (isFirstNewEdge)
                        {
                            _borders.Insert(indexStart, v);
                            isFirstNewEdge = false;
                        }
                        else
                        {
                            _borders.Insert(indexStart + 1, v);
                            break;
                        }
                    }
                }
                foreach (var v in deletedEdges) //удалить старые участки границы
                {
                    _borders.Remove(v);
                }
            }
            //Точка не была добавлена, т.к. находится либо внутри, либо на границе триангуляции
            else
            {
                if (onBorderEdge != null) //Проверка случая, когда точка находится на границе триангуляции
                {
                    IsOk = true;
                    AddPointOnBorder(onBorderEdge, point, indexBorderEdge);
                }
                //иначе IsOk == false и метод укажет, что точка не была добавлена
            }
        }
        /// <summary>
        /// Метод создания треугольников из уже существующего на границе и новой точки (старый треугольник делиться на две части)
        /// </summary>
        private void AddPointOnBorder(EdgeTriangle onBorderEdge, Point point, int indexBorder)
        {
            Triangle triangleInside = onBorderEdge.Triangles[0];
            EdgeTriangle edgeAroundPoint1 = null;
            EdgeTriangle edgeAroundPoint2 = null;
            foreach (var edge in triangleInside.Edges)
            {
                if (edge.Equals(onBorderEdge)) continue;
                if (edge.ContainsPoint(onBorderEdge.Point1)) edgeAroundPoint1 = edge;
                if (edge.ContainsPoint(onBorderEdge.Point2)) edgeAroundPoint2 = edge;
            }
            EdgeTriangle newEdgeAroundPoint1 = new EdgeTriangle(onBorderEdge.Point1, point);
            EdgeTriangle newEdgeAroundPoint2 = new EdgeTriangle(point, onBorderEdge.Point2);
            _triangles.Remove(triangleInside);
            edgeAroundPoint1.RemoveTriangle(triangleInside);
            edgeAroundPoint2.RemoveTriangle(triangleInside);
            onBorderEdge.RemoveTriangle(triangleInside);
            _borders.Remove(onBorderEdge);
            _borders.Insert(indexBorder, newEdgeAroundPoint1);
            _borders.Insert(indexBorder + 1, newEdgeAroundPoint2);
            Point pointOppositeBorder = edgeAroundPoint1.GetPointNotThis(onBorderEdge.Point1);
            EdgeTriangle newEdgeBetweenTriangles = new EdgeTriangle(point, pointOppositeBorder);
            Triangle newTriangle1 = new Triangle(newEdgeBetweenTriangles, edgeAroundPoint1, newEdgeAroundPoint1);
            Triangle newTriangle2 = new Triangle(newEdgeBetweenTriangles, edgeAroundPoint2, newEdgeAroundPoint2);
            _triangles.Add(newTriangle1);
            _triangles.Add(newTriangle2);
            //Требуется для привязки новой точки к какому то треугольнику (с добавленияем метода поиска станет не нужным) CouldDelete
            if (_memoryPoint[onBorderEdge.Point1].Equals(triangleInside))
                _memoryPoint[onBorderEdge.Point1] = newTriangle1;
            if (_memoryPoint[onBorderEdge.Point2].Equals(triangleInside))
                _memoryPoint[onBorderEdge.Point2] = newTriangle2;
            if (_memoryPoint[pointOppositeBorder].Equals(triangleInside))
                _memoryPoint[pointOppositeBorder] = newTriangle2;
            _memoryPoint[point] = newTriangle1;

            if (newTriangle1 != null)
                CheckDelone(edgeAroundPoint1, newTriangle1);
            if (newTriangle2 != null)
                CheckDelone(edgeAroundPoint2, newTriangle2);
        }
        /// <summary>
        /// Метод создания треугольника со стороной-границей триангуляции и точкой вне триангуляции
        /// </summary>
        private void AddPointOutside(Dictionary<Point, EdgeTriangle> newEdgesDictionary, List<EdgeTriangle> newEdges, List<EdgeTriangle> deletedEdges, EdgeTriangle edgeBorder, Point point)
        {
            EdgeTriangle newEdgeWithPoint1;
            EdgeTriangle newEdgeWithPoint2;
            if (newEdgesDictionary.ContainsKey(edgeBorder.Point1))
            {
                newEdgeWithPoint1 = newEdgesDictionary[edgeBorder.Point1];
            }
            else
            {
                newEdgeWithPoint1 = new EdgeTriangle(edgeBorder.Point1, point);
                newEdgesDictionary.Add(edgeBorder.Point1, newEdgeWithPoint1);
                newEdges.Add(newEdgeWithPoint1);
            }
            if (newEdgesDictionary.ContainsKey(edgeBorder.Point2))
            {
                newEdgeWithPoint2 = newEdgesDictionary[edgeBorder.Point2];
            }
            else
            {
                newEdgeWithPoint2 = new EdgeTriangle(point, edgeBorder.Point2);
                newEdgesDictionary.Add(edgeBorder.Point2, newEdgeWithPoint2);
                newEdges.Add(newEdgeWithPoint2);
            }

            Triangle newTriangle = new Triangle(edgeBorder, newEdgeWithPoint1, newEdgeWithPoint2);
            _triangles.Add(newTriangle);
            //Требуется для привязки новой точки к какому то треугольнику (с добавленияем метода поиска станет не нужным) CouldDelete
            if (!_memoryPoint.ContainsKey(point))
                _memoryPoint.Add(point, newTriangle);
            deletedEdges.Add(edgeBorder);

            CheckDelone(edgeBorder, newTriangle);
        }
        /// <summary>
        /// Проверка Делоне (запускается циклически)
        /// </summary>
        private void CheckDelone(EdgeTriangle edgeBetween, Triangle newTriangle)
        {
            Triangle oppositeTriangle = edgeBetween.GetTriangleNotThis(newTriangle);
            if (oppositeTriangle == null) return;

            EdgeTriangle edgeOppositeWithPoint1 = null;
            EdgeTriangle edgeOppositeWithPoint2 = null;
            EdgeTriangle edgeNewWithPoint1 = null;
            EdgeTriangle edgeNewWithPoint2 = null;
            foreach (var edge in oppositeTriangle.Edges)
            {
                if (edge.Equals(edgeBetween)) continue;
                if (edge.ContainsPoint(edgeBetween.Point1)) edgeOppositeWithPoint1 = edge;
                if (edge.ContainsPoint(edgeBetween.Point2)) edgeOppositeWithPoint2 = edge;
            }
            foreach (var edge in newTriangle.Edges)
            {
                if (edge.Equals(edgeBetween)) continue;
                if (edge.ContainsPoint(edgeBetween.Point1)) edgeNewWithPoint1 = edge;
                if (edge.ContainsPoint(edgeBetween.Point2)) edgeNewWithPoint2 = edge;
            }
            Point extremePointOpposite;
            Point extremePointNew;
            if (edgeOppositeWithPoint1.Point1.Equals(edgeBetween.Point1))
                extremePointOpposite = edgeOppositeWithPoint1.Point2;
            else
                extremePointOpposite = edgeOppositeWithPoint1.Point1;
            if (edgeNewWithPoint1.Point1.Equals(edgeBetween.Point1))
                extremePointNew = edgeNewWithPoint1.Point2;
            else
                extremePointNew = edgeNewWithPoint1.Point1;

            //проверка Делоне
            if (oppositeTriangle.IsPointInsideCircumcircle(extremePointNew) || newTriangle.IsPointInsideCircumcircle(extremePointOpposite))
            {
                edgeOppositeWithPoint1.RemoveTriangle(oppositeTriangle);
                edgeOppositeWithPoint2.RemoveTriangle(oppositeTriangle);
                edgeNewWithPoint1.RemoveTriangle(newTriangle);
                edgeNewWithPoint2.RemoveTriangle(newTriangle);
                _triangles.Remove(newTriangle);
                _triangles.Remove(oppositeTriangle);
                EdgeTriangle newEdgeBetween = new EdgeTriangle(extremePointNew, extremePointOpposite);
                Triangle newTriangleWithExtremePoint2 = new Triangle(newEdgeBetween, edgeOppositeWithPoint2, edgeNewWithPoint2);
                Triangle newTriangleWithExtremePoint1 = new Triangle(newEdgeBetween, edgeOppositeWithPoint1, edgeNewWithPoint1);
                _triangles.Add(newTriangleWithExtremePoint2);
                _triangles.Add(newTriangleWithExtremePoint1);

                //Требуется для привязки новой точки к какому то треугольнику (с добавленияем метода поиска станет не нужным) CouldDelete
                if (_memoryPoint[edgeBetween.Point1].Equals(oppositeTriangle) || _memoryPoint[edgeBetween.Point1].Equals(newTriangle))
                    _memoryPoint[edgeBetween.Point1] = newTriangleWithExtremePoint1;
                if (_memoryPoint[edgeBetween.Point2].Equals(oppositeTriangle) || _memoryPoint[edgeBetween.Point2].Equals(newTriangle))
                    _memoryPoint[edgeBetween.Point2] = newTriangleWithExtremePoint2;
                if (_memoryPoint[extremePointOpposite].Equals(oppositeTriangle))
                    _memoryPoint[extremePointOpposite] = newTriangleWithExtremePoint2;
                _memoryPoint[extremePointNew] = newTriangleWithExtremePoint1;

                if (edgeOppositeWithPoint1.CountTriangles == 2)
                {
                    CheckDelone(edgeOppositeWithPoint1, newTriangleWithExtremePoint1);
                }
                if (edgeOppositeWithPoint2.CountTriangles == 2)
                {
                    CheckDelone(edgeOppositeWithPoint2, newTriangleWithExtremePoint2);
                }
            }
        }

        /// <summary>
        /// Метод построения границы Вороного для точки
        /// </summary>
        public DiagrammRegion GenerateRegionsVoronoiToPoint(MapPoint point, Map map)
        {
            //Поиск треугольника, связанного с этой точкой
            //(с добавленияем метода поиска станет не нужным) CouldDelete
            Triangle triangle = _memoryPoint[point];
            DiagrammRegion region = new DiagrammRegion(point, _memoryIds[point]);

            //Переменная, указывающая на то, что прямая начинает проводится из-за границы map
            bool IsStartedAroundPole = false;
            if (triangle.Center.X < map.Xmin || triangle.Center.X > map.Xmax
                || triangle.Center.Y < map.Ymin || triangle.Center.Y > map.Ymax)
            {
                IsStartedAroundPole = true;
            }

            EdgeTriangle nextEdge = triangle.GetFirstEdgeWithPoint(point);

            //идём против часовой стрелки
            GoToRegion(map, triangle, nextEdge, point, region, true, IsStartedAroundPole, out bool IsCloseRegion);

            if (!IsCloseRegion) //если регион не замкнут (прямыкает к краю) идём по часовой стрелке
            {
                nextEdge = triangle.GetEdgeWithPoint(new List<EdgeTriangle>() { nextEdge }, point);

                GoToRegion(map, triangle, nextEdge, point, region, false, IsStartedAroundPole, out _);
            }

            return region;
        }
        /// <summary>
        /// Метод обхода границы региона до мометна, пока мы не сделали круг или не пришли к границе триангуляции 
        /// Возвращает IsCloseRegion - получилось ли сделать круг или нам придётся идти ещё раз, но в другом направлении
        /// </summary>
        private void GoToRegion(Map map, Triangle currentTriangle, EdgeTriangle nextEdge, MapPoint point, DiagrammRegion region, bool isClockwise, bool IsLastAroundPole, out bool IsCloseRegion)
        {
            EdgeTriangle startEdge = currentTriangle.GetFirstEdgeWithPoint(point);
            Point lastPoint = currentTriangle.Center;
            IsCloseRegion = true;
            //идём вокруг точки:
            //сторона -> смежный треугольник -> сторона -> смежный треугольник -> ...
            while (nextEdge != null)
            {
                //уткнулись в край триангуляции
                if (nextEdge.CountTriangles == 1)
                {
                    //если начинали из границ, строим луч, направленный из центра треугольника в сторону границы
                    //иначе строим отрезок от центра одного треугольника до центра другого
                    IsCloseRegion = false;
                    if (IsLastAroundPole)
                    {
                        EdgeRegion e = EdgeRegion.CreateDiaRayAround(nextEdge.Center, lastPoint, map, point, isClockwise, _memoryIds[point], _memoryIds[nextEdge.GetPointNotThis(point)]);
                        if (!(e is null)) AddEdgeInBorders(isClockwise, region, e);
                    }
                    else
                    {
                        EdgeRegion e = EdgeRegion.CreateDiaRay(lastPoint, nextEdge.Center, map, point, !isClockwise, _memoryIds[point], _memoryIds[nextEdge.GetPointNotThis(point)]);
                        if (!(e is null)) AddEdgeInBorders(isClockwise, region, e);
                    }
                    break;
                }

                currentTriangle = nextEdge.GetTriangleNotThis(currentTriangle);

                if (currentTriangle.Center.X < map.Xmin || currentTriangle.Center.X > map.Xmax
                    || currentTriangle.Center.Y < map.Ymin || currentTriangle.Center.Y > map.Ymax)
                {
                    //если центр следующего треугольника находится за границей
                    //--если мы в данный момент за границей, то прямой может не быть
                    //--иначе строим луч, направленный из центра треугольника в сторону границы
                    if (IsLastAroundPole)
                    {
                        EdgeRegion e = EdgeRegion.CreateDiaEdgeAround(lastPoint, currentTriangle.Center, map, nextEdge.Center, _memoryIds[point], _memoryIds[nextEdge.GetPointNotThis(point)]);
                        if (!(e is null)) AddEdgeInBorders(isClockwise, region, e);
                    }
                    else
                    {
                        IsLastAroundPole = true;
                        EdgeRegion e = EdgeRegion.CreateDiaRay(lastPoint, currentTriangle.Center, map, point, !isClockwise, _memoryIds[point], _memoryIds[nextEdge.GetPointNotThis(point)]);
                        if (!(e is null)) AddEdgeInBorders(isClockwise, region, e);
                    }
                }
                else
                {
                    //иначе
                    //--если мы в данный момент за границей, то строим луч из центра следующего треугольника в сторону текущего (в границу)
                    //--иначе строим отрезок из центра одного до центра другого
                    if (IsLastAroundPole)
                    {
                        IsLastAroundPole = false;
                        EdgeRegion e = EdgeRegion.CreateDiaRay(currentTriangle.Center, lastPoint, map, point, isClockwise, _memoryIds[point], _memoryIds[nextEdge.GetPointNotThis(point)]);
                        if (!(e is null)) AddEdgeInBorders(isClockwise, region, e);
                    }
                    else
                    {
                        EdgeRegion e = EdgeRegion.CreateDiaEdge(lastPoint, currentTriangle.Center, _memoryIds[point], _memoryIds[nextEdge.GetPointNotThis(point)]);
                        if (!(e is null)) AddEdgeInBorders(isClockwise, region, e);
                    }
                }

                lastPoint = currentTriangle.Center;
                nextEdge = currentTriangle.GetEdgeWithPoint(new List<EdgeTriangle>() { nextEdge, startEdge }, point);
            }
        }
        /// <summary>
        /// Метод добавления новой стороны в границу региона в зависимости от того идём мы по часовой стрелке или против
        /// </summary>
        private void AddEdgeInBorders(bool isClockwise, DiagrammRegion region, EdgeRegion edge)
        {
            if (isClockwise)
                region.Borders.Add(edge);
            else
                region.Borders.AddFirst(edge);
        }
    }
}
