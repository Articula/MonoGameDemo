using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public class Sprite
	{
		public Vector2 position { get; set; }
		public Texture2D texture { get; set; }
		public SpriteBatch spriteBatch { get; set; }
		public bool isVisible { get; set;}

		public Rectangle boundry
		{
			get { return new Rectangle((int)position.X, (int)position.Y,
					  texture.Width, texture.Height); }
		}

		public Sprite(Texture2D texture, Vector2 position, SpriteBatch batch)
		{
			this.texture = texture;
			this.position = position;
			this.spriteBatch = batch;
		}

		public virtual void Draw()
		{
			spriteBatch.Draw(texture, position, Color.White);
		}
	}
}
