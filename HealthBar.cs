using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameDemo
{
	public class HealthBar
	{
		private SpriteBatch spriteBatch;
		private int heartCount;
		private Texture2D texture;
		private int offsetX = 300;


		public HealthBar(SpriteBatch batch, int health, Texture2D heartTexture)
		{
			spriteBatch = batch;
			heartCount = health;
			texture = heartTexture;
			Level.currentLevel.HealthChangeEvent += OnHealthChange;
		}

		public void Reset()
		{
			heartCount = 3;
		}

		public void Draw()
		{
			for (int i = 1; i <= heartCount; i++)
			{
				spriteBatch.Draw(texture, new Vector2(offsetX + (70*i), 10), Color.White);
			}
		}

		private void OnHealthChange(object sender, HealthChangeEventArgs args)
		{
			if (args.isPositive)
			{
				heartCount++;
			}
			else
			{
				heartCount--;
			}
		}
	}
}
