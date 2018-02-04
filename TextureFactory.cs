using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoGameDemo
{
    class TextureFactory
    {
        private static TextureFactory instance;
        private Dictionary<Texture, Texture2D> textures;
        private Game textureRenderer;

        private TextureFactory()
        {
            textures = new Dictionary<Texture, Texture2D>();
        }

        public static TextureFactory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TextureFactory();
                }

                return instance;
            }
        }

        public void SetTextureRenderer(Game game)
        {
            textureRenderer = game;
        }

        public void AddTextures()
        {
            textures.Add(Texture.Gem, textureRenderer.Content.Load<Texture2D>("resources/gem"));
            textures.Add(Texture.Tile, textureRenderer.Content.Load<Texture2D>("resources/tile"));
            textures.Add(Texture.PassThroughTile, textureRenderer.Content.Load<Texture2D>("resources/passthroughTile"));
            textures.Add(Texture.Player, textureRenderer.Content.Load<Texture2D>("resources/player"));
            textures.Add(Texture.Baddie, textureRenderer.Content.Load<Texture2D>("resources/baddie"));
            textures.Add(Texture.Heart, textureRenderer.Content.Load<Texture2D>("resources/heart"));
            textures.Add(Texture.HalfHeart, textureRenderer.Content.Load<Texture2D>("resources/halfheart"));
            textures.Add(Texture.EmptyHeart, textureRenderer.Content.Load<Texture2D>("resources/emptyheart"));
        }

        public Texture2D getTexture(Texture key)
        {
            return textures[key];
        }
    }
}
