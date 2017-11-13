using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public class Tile : Sprite
	{
		public bool isPassable { get; set; }

		public Tile(Texture2D texture, Vector2 position, SpriteBatch batch, bool isBlocked, bool isPassable = false)
			: base(texture, position, batch)
		{
			this.isVisible = isBlocked;
			this.isPassable = isPassable;
		}

		public override void Draw()
		{
			if (isVisible) { base.Draw(); }
		}
	}
}
