using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGameDemo
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class MonoGameDemo : Game
	{
		private Vector2 screenSize;
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
        private TextureFactory textureFactory;
		private Camera camera;
		private Player player;
		public Level gameLevel;
		public HealthBar healthBar;
		public InventoryScreen inventoryScreen;
		private SpriteFont _debugFont;
		private bool isMenuActive;

		public MonoGameDemo()
		{
			screenSize = new Vector2(960, 640);

			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = (int)screenSize.X;
			graphics.PreferredBackBufferHeight = (int)screenSize.Y;
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
            InitInputHandler(InputHandler.Instance);
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);
            SetUpTextureFactory();

			_debugFont = Content.Load<SpriteFont>("resources/DebugFont");
			camera = new Camera(screenSize);
			inventoryScreen = new InventoryScreen(spriteBatch, GraphicsDevice, _debugFont);
            gameLevel = new Level(camera, spriteBatch, 30, 20);
			player = new Player(TextureFactory.Instance.getTexture(Texture.Player), new Vector2(80, 80), spriteBatch);
			healthBar = new HealthBar(spriteBatch, player.health);
		}

        private void SetUpTextureFactory()
        {
            textureFactory = TextureFactory.Instance;
            textureFactory.SetTextureRenderer(this);
            textureFactory.AddTextures();
        }

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif
			if (!this.isMenuActive)
			{
				base.Update(gameTime);
				player.Update(gameTime);
				gameLevel.Update(gameTime);
				camera.Update(player.position);
				gameLevel.CheckItemCollision(player);

			}
            InputHandler.Instance.Update();
		}

        private void InitInputHandler(InputHandler inputHandler)
        {
            inputHandler.InputPressedEvent += OnInputPress;
            inputHandler.InputReleasedEvent += OnInputRelease;
        }

        private void OnInputPress(object sender, InputPressedEventArgs args)
        {
            switch (args.action)
            {
                case InputAction.ResetLevel:
                    RestartGame();
                    break;
                case InputAction.ToggleMenu:
                    HandleMenuButton();
                    break;
            }
        }

        private void OnInputRelease(object sender, InputReleasedEventArgs args)
        {

        }

        private void RestartGame()
		{
			Level.currentLevel.CreateNewLevel();
			PositionPlayer();
			player.health = 3;
			healthBar.Reset();
            player.inventory.gemCount = 0;
		}

		private void HandleMenuButton()
		{
			if (!this.isMenuActive)
			{
				OpenMenu();
			}
			else
			{
				CloseMenu();
			}
		}

		private void OpenMenu()
		{
			this.isMenuActive = true;
		}

		private void CloseMenu()
		{
			this.isMenuActive = false;
		}

		private void PositionPlayer()
		{
			player.position = Vector2.One * 80;
			player.movement = Vector2.Zero;
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, 
			                  null, null, null, null, camera.ViewMatrix);
			base.Draw(gameTime);
			gameLevel.Draw();
			player.Draw();
			spriteBatch.End();

			spriteBatch.Begin();
			//Debug Text
			WriteDebugInformation();
			healthBar.Draw();
			if (this.isMenuActive) { inventoryScreen.Draw(); }
			spriteBatch.End();
		}

		private void WriteDebugInformation()
		{
			string positionInText = string.Format("Position of Jumper: ({0:0.0}, {1:0.0})", player.position.X, player.position.Y);
			string movementInText = string.Format("Current movement: ({0:0.0}, {1:0.0})", player.movement.X, player.movement.Y);
			string isOnFirmGroundText = string.Format("On firm ground: {0}", player.IsOnFirmGround());
			string GemCountText = string.Format("Gems: {0}", player.inventory.gemCount);
			string PauseText = string.Format("Pause Active: {0}", this.isMenuActive);
			string invulnerabilityText = string.Format("Invulnerable: {0}", player.invulnerableFlag);

			spriteBatch.DrawString(_debugFont, positionInText, new Vector2(10, 0), Color.White);
			spriteBatch.DrawString(_debugFont, movementInText, new Vector2(10, 20), Color.White);
			spriteBatch.DrawString(_debugFont, isOnFirmGroundText, new Vector2(10, 40), Color.White);
			spriteBatch.DrawString(_debugFont, GemCountText, new Vector2(700, 20), Color.White);
			spriteBatch.DrawString(_debugFont, PauseText, new Vector2(700, 0), Color.White);
			spriteBatch.DrawString(_debugFont, invulnerabilityText, new Vector2(700, 40), Color.White);
		}
	}
}
