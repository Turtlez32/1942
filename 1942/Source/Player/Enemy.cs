using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _1942
{
    public class Enemy
    {
        protected Vector2 position = Vector2.Zero;
        protected Texture2D texture = null;
        protected float speed = 200.0f;
        public int health;
        float shootTimer = 0;

        protected bool isDead = true;

        protected Bullet[] enemyBullet = new Bullet[50];

        public Enemy()
        {
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public Vector2 Position
        {
            get {return position;}
            set { position = value; }
        }

        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        public void Draw(SpriteBatch SB)
        {
            /* draw player bullets before player */

            if (IsDead == true)
            {
                return;
            }

            if (texture == null)
            {
                return;
            }

            SB.Draw(texture, position, Color.White);

        }

        public void Update(float deltaTime)
        {
            if( shootTimer > 0)
            {
                shootTimer -= deltaTime;
            }

            if (Game1.player.Position.X < position.X + texture.Width && Game1.player.Position.X > position.X)
            {
                if (shootTimer <= 0)
                {
                    foreach (Bullet b in Game1.bullets)
                    {
                        if (b.IsDead == true)
                        {
                            b.IsDead = false;
                            b.Position = Position;
                            b.isFromPlayer = false;
                            break;
                        }
                    }

                    shootTimer = 0.5f;
                }
            }
           
            if (IsDead == true)
            {
                Game1.player.Score = 200;
                return;
            }

            position.Y += speed * deltaTime;
            
            if (CheckCollissions() == true)
            {
                IsDead = true;
            }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Bounds.Width, texture.Bounds.Height); }
        }

        public bool CheckCollissions()
        {
            Rectangle bounds = Bounds;
            /* check the bullets */
            foreach (Bullet b in Game1.bullets)
            {
                if (health <= 0)
                {
                    if (b.IsDead == false && b.isFromPlayer == true && bounds.Intersects(b.Bounds) == true)
                    {
                        b.IsDead = true;
                        Game1.player.Score = 200;
                        return true;
                    }
                } 
                else
                { 
                    health -= b.Damage;
                }

            }

            return false;
        }
    }
}
