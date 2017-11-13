using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public struct MovementStruct
	{
		public Vector2 movementToTry { get; private set; }
		public Vector2 furthestAvailableLocationSoFar { get; set; }
		public int numberOfStepsToBreakMovementInto { get; private set; }
		public bool isDiagonalMove { get; private set; }
		public Vector2 oneStep { get; private set; }
		public Rectangle boundingRectangle { get; set; }
		public bool isMovingUp { get; private set; }

		public MovementStruct(Vector2 originalPosition, Vector2 destination, Rectangle boundingRectangle) : this()
		{
			movementToTry = destination - originalPosition;
			furthestAvailableLocationSoFar = originalPosition;
			numberOfStepsToBreakMovementInto = (int)(movementToTry.Length() * 2) + 1;
			isDiagonalMove = movementToTry.X != 0 && movementToTry.Y != 0;
			oneStep = movementToTry / numberOfStepsToBreakMovementInto;
			isMovingUp = destination.Y < originalPosition.Y;
			this.boundingRectangle = boundingRectangle;
		}
	}
}
