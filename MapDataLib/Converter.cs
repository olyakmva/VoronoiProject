using System;
using System.Collections.Generic;
using DotSpatial.Data;
using DotSpatial.Topology;

namespace MapDataLib
{
    public static class Converter
    {
        public static int LineIdRatio=0;
        public static int PolygoneIdRatio =0;

        public static MapData ToMapData(IFeatureSet fSet)
        {
            
            var list = fSet.Features;
            if (list.Count == 0)
                return null;
            int idRatio = 0;
            GeometryType type= GeometryType.Unspecified;
            
            switch (list[0].BasicGeometry.FeatureType)
            {
                case FeatureType.Line: type = GeometryType.Line;
                    idRatio+=LineIdRatio;
                    LineIdRatio+=list.Count;
                    break;
                case FeatureType.Point: type = GeometryType.Point;
                    idRatio = 10000;
                    break;
                case FeatureType.Polygon: type = GeometryType.Polygon;
                    idRatio = 100000 +PolygoneIdRatio;
                    PolygoneIdRatio+=list.Count;
                    break;
                case FeatureType.MultiPoint: type = GeometryType.MultiPoint;
                    idRatio = 10000;
                    break;
                case FeatureType.Unspecified:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var map = new MapData(type);
            foreach (var item in list)
            {
                var xy = item.Coordinates;
                var points = new List<MapPoint>();
                foreach (var t in xy)
                {
                    var p = new MapPoint(t.X, t.Y, item.Fid+idRatio, 1.0);

                    points.Add(p);
                }
                map.MapObjDictionary.Add(item.Fid+idRatio, points);
            }
            return map;
        }

        public static IFeatureSet ToShape(MapData map)
        {
            
            FeatureType featureType = FeatureType.Unspecified;
            switch (map.Geometry)
            {
                case GeometryType.Line: featureType = FeatureType.Line;
                    break;
                case GeometryType.Point: featureType = FeatureType.Point;
                    break;
                case GeometryType.Polygon: featureType = FeatureType.Polygon;
                    break;
                case GeometryType.MultiPoint: featureType = FeatureType.MultiPoint;
                    break;
            }
            FeatureSet fs = new FeatureSet(featureType);
            foreach (var pairList in map.MapObjDictionary)
            {
                //if (pairList.Value.Count < 3 && featureType != FeatureType.Point)
                //    continue;
                Coordinate[] coord = new Coordinate[pairList.Value.Count];
                for (int i = 0; i < pairList.Value.Count; i++)
                {
                    coord[i] = new Coordinate(pairList.Value[i].X, pairList.Value[i].Y);
                }
                var f = new Feature(featureType, coord);
                fs.Features.Add(f);
            }
            return fs;
        }
    }
}
