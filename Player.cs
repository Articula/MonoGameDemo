using System;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameDemo
{
	public class Player : Actor
	{
		public int health = 3;
		public Inventory inventory;
		public bool invulnerableFlag = false;
		private const int DefaultInvulnerableTime = 3;
		private TimeSpan invulnerableTimeRemaining = new TimeSpan();
		private bool leftPressed = false;
		private bool rightPressed = false;

		public Player(Texture2D texture, Vector2 position, SpriteBatch batch)
			: base(texture, position, batch)
		{
			inventory = new Inventory();
			KeyboardMonitor.Instance.KeyPressedEvent += OnKeyPress;
			KeyboardMonitor.Instance.KeyReleasedEvent += OnKeyRelease;
			Level.currentLevel.HealthChangeEvent += OnHealthChange;
		}

		private void OnKeyPress(object sender, KeyPressedEventArgs args)
		{
			switch (args.key)
			{
				case Keys.Left:
					leftPressed = true;
					break;
				case Keys.Right:
					rightPressed = true;
					break;
				case Keys.Up:
					if (IsOnFirmGround())
					{
						movement = -Vector2.UnitY * 25;
					}
					break;
			}
		}

		private void OnKeyRelease(object sender, KeyReleasedEventArgs args)
		{
			switch (args.key)
			{
				case Keys.Left:
					leftPressed = false;
					break;
				case Keys.Right:
					rightPressed = false;
					break;
			}
		}

		private void OnHealthChange(object sender, HealthChangeEventArgs args)
		{
			if (args.isPositive)
			{
				health++;
			}
			else
			{
				health--;
				StartInvulnerability();
			}
		}

		public new void Update(GameTime gameTime)
		{
			CheckMovementFlagsAndUpdate();
			if (invulnerableFlag)
			{
				invulnerableTimeRemaining = invulnerableTimeRemaining.Subtract(gameTime.ElapsedGameTime);
				invulnerableFlag &= invulnerableTimeRemaining.CompareTo(TimeSpan.Zero) > 0;
			}
			base.Update(gameTime);
		}

		public bool IsOnFirmGround()
		{
			Rectangle onePixelLower = boundry;
			onePixelLower.Offset(0, 1);
			return !Level.currentLevel.HasRoomForRectangle(onePixelLower);
		}

		public void AddToInventory(ICollectable item)
		{
			this.inventory.add(item);
		}

		private void CheckMovementFlagsAndUpdate()
		{
			if (leftPressed) { movement += new Vector2(-1, 0); }
			if (rightPressed) { movement += new Vector2(1, 0); }
		}

		private void StartInvulnerability()
		{
			if (!invulnerableFlag)
			{
				invulnerableFlag = true;
				invulnerableTimeRemaining = TimeSpan.FromSeconds(DefaultInvulnerableTime);
			}
		}
	}
}
