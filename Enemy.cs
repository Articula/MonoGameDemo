using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGameDemo
{
	public class Enemy : Actor
	{
        bool movingLeft = true;

		public Enemy(Texture2D texture, Vector2 position, SpriteBatch batch) : base (texture, position, batch)
		{
            this.isVisible = true;
            speed = 30;
        }

        public override void Update(GameTime gameTime)
        {
            Move();
            base.Update(gameTime);

            /* If we've hit a wall, turn around! */
            Vector2 lastMovement = position - oldPosition;
            if (lastMovement.X == 0)
            {
                movingLeft = movingLeft ? false : true;
            }
        }

        private void Move()
        {
            if (movingLeft) {
                movement += new Vector2(-1, 0);
            } else {
                movement += new Vector2(1, 0);
            }
        }
    }
}
