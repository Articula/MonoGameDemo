using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MonoGameDemo
{
	public class Level
	{
		public Tile[,] tiles;
		public Enemy[] enemies;
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

        // TODO: Seperate into two quadtrees when there are drawable but not collidable objects
        private QuadTree<IQuadStorable> quadTree;

        public event EventHandler<HealthChangeEventArgs> HealthChangeEvent;

		public Level(SpriteBatch spriteBatch, Texture2D tileTexture, Texture2D ptTexture, Texture2D enemyTexture, Texture2D gemTexture, int columns, int rows)
		{
			this.columns = columns;
			this.rows = rows;
			this.tileTexture = tileTexture;
			this.passthroughTileTexture = ptTexture;
			this.enemyTexture = enemyTexture;
            this.gemTexture = gemTexture;
			this.spriteBatch = spriteBatch;
            this.quadTree = new QuadTree<IQuadStorable>(0, 0, 1920, 1280); //TODO: Clean Magic Numbers!!
            CreateNewLevel();
			Level.currentLevel = this;
		}

		public void CreateNewLevel()
		{
            quadTree.Clear();
            InitializeTiles();
			InitializeBorderTiles();
			GeneratePassThroughTile();
			InitializeEnemies();
			SetTopLeftTileUnblocked();
            InitializeCollectables();

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
                    quadTree.Add(tile);
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
            this.tiles[4, 3] = tile;
            quadTree.Add(tile);

            tiles[4, 4].isVisible = false;
		}

		void InitializeEnemies()
		{
			this.enemies = new Enemy[1];
            Enemy enemy = new Enemy(enemyTexture, new Vector2(240, 80), spriteBatch);
            enemies[0] = enemy;
            quadTree.Add(enemy);
        }

		void InitializeCollectables()
		{
			this.collectables = new List<ICollectable>();
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
                quadTree.Add((IQuadStorable)collectables[i]);
            }
        }

		private void SetTopLeftTileUnblocked()
		{
			tiles[1, 1].isVisible = false;
		}

		public void Draw(Camera camera)
		{
            // Get all QuadTree items in camera coordinates
            List<IQuadStorable> itemsToDraw = quadTree.GetObjects(camera.GetCameraRect());

            foreach (IQuadStorable obj in itemsToDraw)
            {
                if (obj is Sprite)
                {
                    ((Sprite)obj).Draw();
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
                    quadTree.Move((IQuadStorable)collectable);
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
			foreach (var tile in tiles)
			{
				if (tile.isVisible && tile.boundry.Intersects(rectangleToCheck))
				{
					if (tile.isPassable) 
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

		/* TODO: Update Collision Detection (QuadTrees)*/
		public void CheckItemCollision(Player player)
		{
			foreach (var collectable in this.collectables)
			{
				if (collectable.isVisible && collectable.boundry.Intersects(player.boundry))
				{
					collectable.isVisible = false; //TODO: Delete from list pls
					player.AddToInventory(collectable);
				}
			}

			foreach (var enemy in this.enemies)
			{
                if (enemy.isVisible)
                {
                    /* TODO: Consider adding 4-8 rectangles within each enemy for more accurate collision detection */
                    if (enemy.boundry.Intersects(player.boundry))
                    {
                        //Determine whether this was contact from the top (enemy kill), else take damage

                        if (!player.invulnerableFlag)
                        {
                            MovementStruct move = new MovementStruct(player.oldPosition, player.position, player.boundry);
                            if (player.boundry.Bottom == enemy.boundry.Top + 1 && !move.isMovingUp)
                            {
                                enemy.isVisible = false;
                            }
                            else
                            {
                                this.HealthChangeEvent.Invoke(this, new HealthChangeEventArgs(false, 1));
                            }
                        }
                    }
                }
			}
		}
	}
}
