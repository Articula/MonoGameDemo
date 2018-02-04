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
		private int healthCount;
		private Texture2D heartTexture = TextureFactory.Instance.getTexture(Texture.Heart);
        private Texture2D halfTexture = TextureFactory.Instance.getTexture(Texture.HalfHeart);
        private Texture2D emptyTexture = TextureFactory.Instance.getTexture(Texture.EmptyHeart);
        private int offsetX = 300;
        const int heartCount = 3;

		public HealthBar(SpriteBatch batch, int health)
		{
			spriteBatch = batch;
			healthCount = health;
			Level.currentLevel.HealthChangeEvent += OnHealthChange;
		}

		public void Reset()
		{
			healthCount = 6;
		}

		public void Draw()
		{
            float health = (float)healthCount / 2;
			for (int i = 0; i < heartCount; i++)
			{
                if (health - i >= 1)
                {
                    spriteBatch.Draw(heartTexture, new Vector2(offsetX + (70 * i), 10), Color.White);
                }
                else if ((health - i) % 1 > 0)
                {
                    spriteBatch.Draw(halfTexture, new Vector2(offsetX + (70 * i), 10), Color.White);
                }
                else
                {
                    spriteBatch.Draw(emptyTexture, new Vector2(offsetX + (70 * i), 10), Color.White);
                }
			}
		}

		private void OnHealthChange(object sender, HealthChangeEventArgs args)
		{
			if (args.isPositive)
			{
				healthCount++;
			}
			else
			{
				healthCount--;
			}
		}
	}
}
