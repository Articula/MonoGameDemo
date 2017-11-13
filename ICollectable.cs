using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public interface ICollectable
	{
		ItemId itemId { get; }
		Vector2 position { get; }
		Rectangle boundry { get; }
		bool isVisible { get; set; }

		void Draw();
	}
}
