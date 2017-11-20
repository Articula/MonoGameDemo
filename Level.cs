using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameDemo
{
	public class Level
	{
		public Tile[,] tiles;
		public List<Enemy> enemies;
        public List<ICollectable> collectables;

        public int columns;
		public int rows;
		public Texture2D tileTexture;
		public Texture2D passthroughTileTexture;
		public Texture2D enemyTexture;
		public Texture2D gemTexture;
		public SpriteBatch spriteBatch;
		public static Level currentLevel { get; private set; }
		private Random rnd = new Random();
        private Camera camera;

        private QuadTree<IQuadStorable> tileQuadTree;
        private QuadTree<IQuadStorable> actorQuadTree;

        public event EventHandler<HealthChangeEventArgs> HealthChangeEvent;

		public Level(Camera levelCamera, SpriteBatch spriteBatch, Texture2D tileTexture, Texture2D ptTexture, Texture2D enemyTexture, Texture2D gemTexture, int columns, int rows)
		{
            camera = levelCamera;
            this.columns = columns;
			this.rows = rows;
			this.tileTexture = tileTexture;
			this.passthroughTileTexture = ptTexture;
			this.enemyTexture = enemyTexture;
            this.gemTexture = gemTexture;
			this.spriteBatch = spriteBatch;
            this.tileQuadTree = new QuadTree<IQuadStorable>(0, 0, 1920, 1280); //TODO: Clean Magic Numbers!!
            this.actorQuadTree = new QuadTree<IQuadStorable>(0, 0, 1920, 1280); //TODO: Clean Magic Numbers!!
            CreateNewLevel();
			Level.currentLevel = this;
		}

		public void CreateNewLevel()
		{
            tileQuadTree.Clear();
            actorQuadTree.Clear();
            InitializeTiles();
			InitializeBorderTiles();
			GeneratePassThroughTile();
			SetTopLeftTileUnblocked();
            InitializeLevelLists();
            InitializeEnemies();

            SetGems();
		}

		void InitializeTiles()
		{
			this.tiles = new Tile[this.columns, this.rows];

			//Generating the remaining tiles randomly for by column and row
			for (int x = 0; x < this.columns; x++)
			{
				for (int y = 0; y < rows; y++)
				{
					Vector2 tilePosition =
						new Vector2(x * tileTexture.Width, y * tileTexture.Height);
                    Tile tile = new Tile(tileTexture, tilePosition, spriteBatch, rnd.Next(5) == 0);
                    this.tiles[x, y] = tile;
                    tileQuadTree.Add(tile);
                }

			}

			//Generate block tile shelf
			for (int x = 0; x< 5; x++)
			{
				for (int y = 4; y<5; y++)
				{

					this.tiles[x, y].isVisible = true;
				}

			}
		}

		void InitializeBorderTiles()
		{
			for (int x = 0; x < this.columns; x++)
			{
				for (int y = 0; y < this.rows; y++)
				{
					if (x == 0 || x == (this.columns - 1) || y == 0 || y == (this.rows - 1))
					{
						this.tiles[x, y].isVisible = true;
					}

				}
			}
		}

		void GeneratePassThroughTile()
		{
			//Generate a passthrough Tile
			Vector2 passthroughTilePosition =
				new Vector2(4 * tileTexture.Width, 3 * tileTexture.Height);
            Tile tile = new Tile(passthroughTileTexture, passthroughTilePosition, spriteBatch, true, true);
            tileQuadTree.Remove(this.tiles[4, 3]);
            this.tiles[4, 3] = tile;
            tileQuadTree.Add(tile);

            tiles[4, 4].isVisible = false;
		}

		void InitializeEnemies()
		{
            Enemy enemy = new Enemy(enemyTexture, new Vector2(240, 80), spriteBatch);
            this.enemies.Add(enemy);
            actorQuadTree.Add(enemy);
        }

		void InitializeLevelLists()
		{
			this.collectables = new List<ICollectable>();
            this.enemies = new List<Enemy>();
		}

		//TODO: Remove this after testing
		void SetGems()
		{
            for (int i = 8; i < 19; i++)
            {
                tiles[5, i].isVisible = false;
            }
            tiles[10, 5].isVisible = false;
            tiles[7, 3].isVisible = false;
            tiles[17, 8].isVisible = false;

            Vector2 gem1Position =
                        new Vector2(5 * tileTexture.Width, 8 * tileTexture.Height);
            Vector2 gem2Position =
                       new Vector2(10 * tileTexture.Width, 5 * tileTexture.Height);
            Vector2 gem3Position =
                        new Vector2(7 * tileTexture.Width, 3 * tileTexture.Height);
            Vector2 gem4Position =
                        new Vector2(17 * tileTexture.Width, 8 * tileTexture.Height);

            this.collectables.Add(new Gem(gemTexture, gem1Position, spriteBatch));
            this.collectables.Add(new Gem(gemTexture, gem2Position, spriteBatch));
            this.collectables.Add(new Gem(gemTexture, gem3Position, spriteBatch));
            this.collectables.Add(new Gem(gemTexture, gem4Position, spriteBatch));

            for (int i = 0; i < collectables.Count; i++)
            {
                actorQuadTree.Add((IQuadStorable)collectables[i]);
            }
        }

		private void SetTopLeftTileUnblocked()
		{
			tiles[1, 1].isVisible = false;
		}

		public void Draw()
		{
            // Get all QuadTree items in camera coordinates
            List<IQuadStorable> tilesToDraw = tileQuadTree.GetObjects(camera.GetCameraRect());
            List<IQuadStorable> actorsToDraw = actorQuadTree.GetObjects(camera.GetCameraRect());

            foreach (IQuadStorable obj in tilesToDraw)
            {
                if (obj is Tile)
                {
                    ((Tile)obj).Draw();
                }
            }

            foreach (IQuadStorable obj in actorsToDraw)
            {
                if (obj is Actor)
                {
                    ((Actor)obj).Draw();
                }
            }
        }

		public void Update(GameTime gameTime)
		{
			foreach (var enemy in this.enemies)
			{
				enemy.Update(gameTime);
			}

			foreach (var collectable in this.collectables)
			{
				if (collectable.GetType() == typeof(Gem))
				{
					((Gem)collectable).Update(gameTime);

                    //TODO: Only do this is the Gem has actually moved
                    actorQuadTree.Move((IQuadStorable)collectable);
                }
			}
		}

		/* Begin collision detection code */

        //TODO: Pass in just MovementStruct
		public Vector2 WhereCanIGetTo(Vector2 originalPosition, Vector2 destination, Rectangle boundingRectangle)
		{
			MovementStruct move = new MovementStruct(originalPosition, destination, boundingRectangle);

			for (int i = 1; i <= move.numberOfStepsToBreakMovementInto; i++)
			{
				Vector2 positionToTry = originalPosition + move.oneStep * i;
				Rectangle newBoundary =
					CreateRectangleAtPosition(positionToTry, boundingRectangle.Width, boundingRectangle.Height);
				if (HasRoomForRectangle(newBoundary, move.isMovingUp)) { move.furthestAvailableLocationSoFar = positionToTry; }
				else
				{
					if (move.isDiagonalMove)
					{
						move.furthestAvailableLocationSoFar = CheckPossibleNonDiagonalMovement(move, i);
					}
					break;
				}
			}
			return move.furthestAvailableLocationSoFar; 
		}

		/* TODO: Consider adding 4-8 rectangles within each tile for more accurate collision detection */
		public bool HasRoomForRectangle(Rectangle rectangleToCheck, bool isMovingUp = false)
		{
            Rectangle checkArea = CreateCollisionCheckZone(rectangleToCheck);
            List<IQuadStorable> tilesCloseToActor = tileQuadTree.GetObjects(checkArea);
            foreach (IQuadStorable tile in tilesCloseToActor)
			{
				if (((Tile)tile).isVisible && tile.boundry.Intersects(rectangleToCheck))
				{
					if (((Tile)tile).isPassable) 
					{
						if (rectangleToCheck.Bottom == tile.boundry.Top + 1 && !isMovingUp)
						{
							return false;
						}
						else
						{
							return true;
						}
					}
					else
					{
						return false;
					}
				}
			}
			return true;
		}

        private Rectangle CreateCollisionCheckZone (Rectangle actorArea)
        {
            int x = (actorArea.X - tileTexture.Width >= 0) ? actorArea.X - tileTexture.Width : 0;
            int y = (actorArea.Y - tileTexture.Height >= 0) ? actorArea.Y - tileTexture.Height : 0;
            int width = (actorArea.Width + (tileTexture.Width*2) <= 1920) ? actorArea.Width + (tileTexture.Width*2) : 1920; //TODO: Magic Numbers...
            int height = (actorArea.Height + (tileTexture.Height*2) <= 1280) ? actorArea.Height + (tileTexture.Height*2) : 1280;
            return new Rectangle(x, y, width, height);
        }

		private Rectangle CreateRectangleAtPosition(Vector2 positionToTry, int width, int height)
		{
			return new Rectangle((int)positionToTry.X, (int)positionToTry.Y, width, height);
		}

		private Vector2 CheckPossibleNonDiagonalMovement(MovementStruct movementStruct, int i)
		{
			if (movementStruct.isDiagonalMove)
			{
				int stepsLeft = movementStruct.numberOfStepsToBreakMovementInto - (i - 1);

				Vector2 remainingHorizontalMovement = movementStruct.oneStep.X * Vector2.UnitX * stepsLeft;
					movementStruct.furthestAvailableLocationSoFar =
					WhereCanIGetTo(movementStruct.furthestAvailableLocationSoFar, movementStruct.furthestAvailableLocationSoFar + remainingHorizontalMovement, movementStruct.boundingRectangle);

				Vector2 remainingVerticalMovement = movementStruct.oneStep.Y * Vector2.UnitY * stepsLeft;
					movementStruct.furthestAvailableLocationSoFar =
					WhereCanIGetTo(movementStruct.furthestAvailableLocationSoFar, movementStruct.furthestAvailableLocationSoFar + remainingVerticalMovement, movementStruct.boundingRectangle);
			}

			return movementStruct.furthestAvailableLocationSoFar;
		}

		public void CheckItemCollision(Player player)
		{
            Rectangle checkArea = CreateCollisionCheckZone(player.boundry);
            List<IQuadStorable> itemsCloseToActor = actorQuadTree.GetObjects(checkArea);
            foreach (IQuadStorable obj in itemsCloseToActor)
			{
                if (obj is ICollectable)
                {
                    if (((Sprite)obj).isVisible && obj.boundry.Intersects(player.boundry))
                    {
                        ((Sprite)obj).isVisible = false;
                        collectables.Remove((ICollectable)obj);
                        actorQuadTree.Remove(obj);
                        player.AddToInventory((ICollectable)obj);
                    }
                }
                else if (obj.GetType() == typeof(Enemy))
                {
                    EnemyCollision((Enemy)obj, player);
                }
			}
		}

        public void EnemyCollision(Enemy enemy, Player player)
        {
            if (enemy.isVisible)
            {
                /* TODO: Consider adding 4-8 rectangles within each enemy for more accurate collision detection */
                if (enemy.boundry.Intersects(player.boundry))
                {
                    //Determine whether this was contact from the top (enemy kill), else take damage
                    MovementStruct move = new MovementStruct(player.oldPosition, player.position, player.boundry);
                    if (player.boundry.Bottom == enemy.boundry.Top + 1 && !move.isMovingUp)
                    {
                        enemy.isVisible = false;
                        enemies.Remove(enemy);
                        actorQuadTree.Remove(enemy);
                    }
                    else
                    {
                        if (!player.invulnerableFlag)
                        {
                            this.HealthChangeEvent.Invoke(this, new HealthChangeEventArgs(false, 1));
                        }
                    }
                }
            }
        }
	}
}
