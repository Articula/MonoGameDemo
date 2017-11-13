using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public class Enemy : Actor
	{
		public Enemy(Texture2D texture, Vector2 position, SpriteBatch batch) : base (texture, position, batch)
		{
            isVisible = true;
		}
	}
}
