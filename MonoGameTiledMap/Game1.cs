using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace MonoGameTiledMap
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Map _mapa;
        Map.Tileset _tileset;
        Texture2D _tilesetTexture;
        Stream _platformerGameMapStream;

        public Game1(Stream platformerGameMapStream)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;

            _platformerGameMapStream = platformerGameMapStream;
        }

        protected override void Initialize()
        {
            // Lê todo o arquivo JSON
            var conteudoDoJson = string.Empty;
            using (var streamReader = new StreamReader(_platformerGameMapStream))
                conteudoDoJson = streamReader.ReadToEnd();

            // Converte o conteúdo JSON em um objeto Map e recupera o primeiro Tileset do mapa
            _mapa = JsonConvert.DeserializeObject<Map>(conteudoDoJson);
            _tileset = _mapa.Tilesets.First();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _tilesetTexture = Content.Load<Texture2D>("PlatformerGameTileset");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            foreach (var layer in _mapa.Layers)
            {
                //for (int indiceLinhaDoLayer = 0; indiceLinhaDoLayer < layer.Height; indiceLinhaDoLayer++) //R-D
                var skp = 0;
                for (int indiceLinhaDoLayer = layer.Height; indiceLinhaDoLayer > 0; indiceLinhaDoLayer--) //R-U
                {
                    var dadosLinhaDoLayer = layer.Data.SkipLast(skp * layer.Width).TakeLast(layer.Width).ToList();
                    skp++;

                    for (int indiceColunaDoLayer = 0; indiceColunaDoLayer < layer.Width; indiceColunaDoLayer++)
                    {
                        var dadoDaColuna = dadosLinhaDoLayer.ElementAt(indiceColunaDoLayer);

                        if (dadoDaColuna == 0)
                            continue;

                        var tileId = dadoDaColuna - 1;

                        // Posição do tile no layer do mapa
                        var x = indiceColunaDoLayer * _mapa.TileWidth;
                        var y = indiceLinhaDoLayer * _mapa.TileHeight;

                        // Obtém o indice do tile no tileset
                        var tilesetRowIndex = (int)(tileId / _tileset.Columns);
                        var tilesetColIndex = (int)(tileId - (tilesetRowIndex * _tileset.Columns));

                        // Posição do tile no tileset
                        var tileSrcPosX = _tileset.TileWidth * tilesetColIndex;
                        var tileSrcPosY = _tileset.TileHeight * tilesetRowIndex;

                        // Desenha o tile
                        //public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color);
                        spriteBatch.Draw(
                            _tilesetTexture,            // Textura contendo todos os tiles (Tileset).
                            new Vector2(x, y),          // Posição que o tile deve ser desenhado na tela.
                            new Rectangle(
                                tileSrcPosX,
                                tileSrcPosY,
                                _tileset.TileWidth,
                                _tileset.TileHeight),   // Identifica qual tile do tileset deve ser desenhado.
                            Color.White);
                    }
                }

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}