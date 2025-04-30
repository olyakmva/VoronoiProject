using DotSpatial.Data;

namespace MapDataLib
{
    public class ShapeFileIO
    {
        public MapData Open(string shapeFileName)
        {
           var  _inputShape = FeatureSet.Open(shapeFileName);
            return  Converter.ToMapData(_inputShape);
        }

        public void Save(string fileName, MapData mapData)
        {
            IFeatureSet fs = Converter.ToShape(mapData);
            fs.SaveAs(fileName + ".shp", true);
            
        }
    }
}
