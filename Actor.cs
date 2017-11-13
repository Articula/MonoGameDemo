using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public class Actor : Sprite
	{
		public Vector2 movement { get; set; }
		public Vector2 oldPosition; //Change this back to protected if you're updating collision and not generating structs

		public Actor(Texture2D texture, Vector2 position, SpriteBatch batch) : base(texture, position, batch)
		{
            oldPosition = position;
		}

		public void Update(GameTime gameTime)
		{
			AffectWithGravity();
			SimulateFriction();
			MoveAsFarAsPossible(gameTime);
			StopMovingIfBlocked();
		}

		private void AffectWithGravity()
		{
			movement += Vector2.UnitY * .5f;
		}

		private void SimulateFriction()
		{
			movement -= movement * new Vector2(.1f, .1f); //Multiply X and Y values of movement by .1
														  //if (IsOnFirmGround()) { movement -= movement* Vector2.One * .08f; }
														  //else { movement -= movement* Vector2.One * .02f; 
		}

		void MoveAsFarAsPossible(GameTime gameTime)
		{
			oldPosition = position;

			UpdatePositionBasedOnMovement(gameTime);
			position = Level.currentLevel.WhereCanIGetTo(oldPosition, position, boundry);
		}

		private void UpdatePositionBasedOnMovement(GameTime gameTime)
		{
			//Smoothes out movement, adjust /12 for speed adjustment
			position += movement * (float)gameTime.ElapsedGameTime.TotalMilliseconds / 12;
		}

		private void StopMovingIfBlocked()
		{
			Vector2 lastMovement = position - oldPosition;
			if (lastMovement.X == 0) { movement *= Vector2.UnitY; }
			if (lastMovement.Y == 0) { movement *= Vector2.UnitX; }
		}
	}
}
