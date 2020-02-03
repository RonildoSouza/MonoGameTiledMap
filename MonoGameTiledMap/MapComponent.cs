using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace MonoGameTiledMap
{
    public class MapComponent : DrawableGameComponent
    {
        private readonly StreamReader _tiledMapStreamReader;
        private readonly string _tilesetName;
        private SpriteBatch _spriteBatch;
        private Map _map;
        private Texture2D _tilesetTexture;

        public MapComponent(Game game, Stream tiledMapStream, string tilesetName) : base(game)
        {
            _tiledMapStreamReader = new StreamReader(tiledMapStream);
            _tilesetName = tilesetName;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            _spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            // Lê todo o arquivo JSON
            var conteudoDoJson = _tiledMapStreamReader.ReadToEnd();

            // Converte o conteúdo JSON em um objeto Map e recupera o primeiro Tileset do mapa
            _map = JsonConvert.DeserializeObject<Map>(conteudoDoJson);

            // Carrega o tileset para um objeto do tipo Texture2D
            _tilesetTexture = Game.Content.Load<Texture2D>(_tilesetName);

        }

        public override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();

            foreach (var layer in _map.Layers)
            {
                for (int indiceLinhaDoLayer = 0; indiceLinhaDoLayer < layer.Height; indiceLinhaDoLayer++)
                {
                    var dadosLinhaDoLayer = layer.Data.Skip(indiceLinhaDoLayer * layer.Width)
                                                      .Take(layer.Width);

                    for (int indiceColunaDoLayer = 0; indiceColunaDoLayer < layer.Width; indiceColunaDoLayer++)
                    {
                        var dadoDaColuna = dadosLinhaDoLayer.ElementAt(indiceColunaDoLayer);

                        if (dadoDaColuna == 0)
                            continue;

                        var tileId = dadoDaColuna - 1;

                        // Posição do tile no layer do mapa
                        var x = indiceColunaDoLayer * _map.TileWidth;
                        var y = indiceLinhaDoLayer * _map.TileHeight;

                        // Identifica qual tile do tileset deve ser desenhado.
                        var tile = ObtemTileDoTileset(tileId, _map.Tilesets.First());

                        // Desenha o tile
                        //public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color);
                        _spriteBatch.Draw(
                            _tilesetTexture,    // Textura contendo todos os tiles (Tileset).
                            new Vector2(x, y),  // Posição que o tile deve ser desenhado na tela.
                            tile,
                            Color.White);
                    }
                }
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Rectangle ObtemTileDoTileset(long tileId, Map.Tileset tileset)
        {
            // Obtém o indice do tile no tileset
            var indiceLinhaDoTileset = (int)(tileId / tileset.Columns);
            var indiceColunaDoTileset = (int)(tileId - (indiceLinhaDoTileset * tileset.Columns));

            // Posição do tile no tileset
            var tilePosicaoX = tileset.TileWidth * indiceColunaDoTileset;
            var tilePosicaoY = tileset.TileHeight * indiceLinhaDoTileset;

            return new Rectangle(tilePosicaoX, tilePosicaoY, tileset.TileWidth, tileset.TileHeight);
        }
    }
}