using System.Collections.Generic;

namespace MonoGameTiledMap
{
    public class Map
    {
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public IList<Tileset> Tilesets { get; set; }
        public IList<Layer> Layers { get; set; }

        public class Tileset
        {
            public int TileWidth { get; set; }
            public int TileHeight { get; set; }
            public int Columns { get; set; }
        }

        public class Layer
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public IList<long> Data { get; set; }
        }
    }
}