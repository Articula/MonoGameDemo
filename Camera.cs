using Microsoft.Xna.Framework;

namespace MonoGameDemo
{
	public class Camera
	{
		Vector2 position;
		Matrix viewMatrix;
		readonly float screenWidth;
		readonly float screenHeight;

		public Camera(Vector2 screenSize)
		{
			screenWidth = screenSize.X;
			screenHeight = screenSize.Y;
		}

		public Matrix ViewMatrix
		{
			get { return viewMatrix; }
		}

		public void Update(Vector2 playerPosition)
		{
			position.X = playerPosition.X - (screenWidth / 2);
			position.Y = playerPosition.Y - (screenHeight / 2);

			if (position.X < 0)
			{
				position.X = 0;
			}
			else if (position.X > screenWidth)
			{
				position.X = screenWidth;
			}

			if (position.Y < 0)
			{
				position.Y = 0;
			}
			else if (position.Y > screenHeight)
			{
				position.Y = screenHeight;
			}

			viewMatrix = Matrix.CreateTranslation(new Vector3(-position, 0));
		}
	}
}
