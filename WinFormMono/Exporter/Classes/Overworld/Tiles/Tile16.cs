namespace ZScream_Exporter
{
    public class Tile16
    {
        public TileInfo[] Info
        {
            get;
        }
        //[0,1]
        //[2,3]

        public Tile16(TileInfo tile0, TileInfo tile1, TileInfo tile2, TileInfo tile3)
        {
            Info = new TileInfo[4];
            Info[0] = tile0;
            Info[1] = tile1;
            Info[2] = tile2;
            Info[3] = tile3;
        }
    }


}
