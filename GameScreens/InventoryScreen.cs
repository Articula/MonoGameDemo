using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public class InventoryScreen
	{
		private SpriteBatch spriteBatch;
		private Texture2D pixel;
		private SpriteFont font;

		public InventoryScreen(SpriteBatch batch, GraphicsDevice graphicsDevice, SpriteFont font)
		{
			spriteBatch = batch;
			this.font = font;
			//TODO: Make the following a util or rectangle generating class
			//Create a filled in rectangle

			// Make a 1x1 texture named pixel
			Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);

			// Create a 1D array of color data to fill the pixel texture with.  
			Color[] colorData = {
				Color.White
			};

			// Set the texture data with our color information  
			pixel.SetData<Color>(colorData);

			this.pixel = pixel;
		}

		public void Draw()
		{
			// Draw a fancy purple rectangle.  
			spriteBatch.Draw(pixel, new Rectangle(330, 170, 300, 300), Color.Purple);
			spriteBatch.DrawString(this.font, "Pause", new Vector2(460, 315), Color.White);
		}
	}
}
