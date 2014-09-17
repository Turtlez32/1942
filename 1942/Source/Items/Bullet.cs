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
    public class Bullet
    {
        /* setup bullet variables */
        protected Vector2 position = Vector2.Zero;
        protected Texture2D texture = null;
        protected float speed = 300.0f;
        protected bool isDead = false;
        protected int damage = 50;

        public bool isFromPlayer = false;

        public Bullet()
        {
        }

        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }

        public int Damage
        {
            get { return damage; }
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
            if (isDead == true)
            {
                return;
            }

            if (isFromPlayer == true)
            {
                position.Y -= speed * deltaTime;
            }
            else
            {
                position.Y += speed * deltaTime;
            }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Bounds.Width, texture.Bounds.Height); }
        }
    }
}
