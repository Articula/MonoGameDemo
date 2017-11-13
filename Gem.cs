using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public class Gem : Actor, ICollectable
	{
		public ItemId itemId { get; }

		public Gem(Texture2D texture, Vector2 position, SpriteBatch batch)
			: base(texture, position, batch)
		{
			this.itemId = ItemId.Gem;
			this.isVisible = true;
		}

		public override void Draw()
		{
			if (isVisible) { base.Draw(); } 
		}
	}
}
