using System.Collections.Generic;

namespace MapDataLib
{
    public class MapObjItem
    {
        /// <summary>
        /// Id of MapObjItem
        /// </summary>
        public int Id {get; set;}
        /// <summary>
        /// List of Points MapObjItem
        /// </summary>
        public List<MapPoint> Points { get; set;}
        /// <summary>
        /// Geometry of MapObjItem (Point, Line, Polygon)
        /// </summary>
        public GeometryType Geometry { get; set; }
        /// <summary>
        /// Path of MapObjItem by squares
        /// </summary>
        public List<string> Path { get; set; } = new List<string> { "*" };
    }
}
