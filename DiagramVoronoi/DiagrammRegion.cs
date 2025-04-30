using MapDataLib;
using System.Collections.Generic;

namespace DiagramVoronoi
{
    public class DiagrammRegion
    {
        public MultiMapPoint Center;
        public BilateralList<EdgeRegion> Borders;
        public List<int> Ids => Center.Ids;
        public DiagrammRegion(MultiMapPoint center)
        {
            Center = center;
            Borders = new BilateralList<EdgeRegion>();
        }
        public DiagrammRegion(MapPoint center, List<int> indexes)
        {
            Center = new MultiMapPoint(center);
            Center.Ids = indexes;
            Borders = new BilateralList<EdgeRegion>();
        }
    }
}
