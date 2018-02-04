using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public class Gem : Actor, ICollectable
	{
		public ItemId itemId { get; }

		public Gem(Texture2D tex, Vector2 position, SpriteBatch batch)
			: base(tex, position, batch)
		{
			itemId = ItemId.Gem;
			isVisible = true;
            texture = tex;
		}

		public override void Draw()
		{
			if (isVisible) { base.Draw(); } 
		}
	}
}
