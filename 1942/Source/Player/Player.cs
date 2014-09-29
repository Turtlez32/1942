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
using Microsoft.Xna.Framework.Audio;

namespace _1942
{
    public class Player
    {
        /* set the player plane speed */
        protected float speed = 125.0f;

        /* Setup all player variables */
        public int health;
        protected Vector2 position = Vector2.Zero;
        protected Texture2D texture;
        public bool isDead = false;
        public int lives = 3;
        public int maxHealth;
        protected SoundEffect shootFX;
        protected int score = 0;

        /* set the timer between each bullet */
        protected float coolDownTimer = 0;

        /* Player() default class constructor() */
        public Player()
        {
            health = 100;
            position.X = 400;
            position.Y = 400;

        }

        public Texture2D PlayerTexture
        {
            get { return texture; }
            set { texture = value; }
        }

        public int Score
        {
            get { return score; }
            set { score += value; }
        }

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        public SoundEffect ShootFX
        {
            set { shootFX = value; }
        }

        public void InitialisePlayer()
        {
            position.X = 100;
            position.Y = 100;
        }

        public void IncreasePlayerHealthByVariable(int health)
        {
            GetPlayerHealth();

            health = health + health;
        }

        public int GetPlayerHealth()
        {
            return health;
        }

        public bool HasPlayerDied
        {
            get { return isDead; }
            set { isDead = value; }
        }

        public Rectangle Bounds
        {
            get { return new Rectangle((int)position.X, (int)position.Y, texture.Bounds.Width, texture.Bounds.Height); }
        }

        public void Update(float deltaTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.W))
                position.Y -= speed * deltaTime;

            if (Keyboard.GetState().IsKeyDown(Keys.S))
                position.Y += speed * deltaTime;

            if (Keyboard.GetState().IsKeyDown(Keys.A))
                position.X -= speed * deltaTime;

            if (Keyboard.GetState().IsKeyDown(Keys.D))
                position.X += speed * deltaTime;

            if ( coolDownTimer > 0 )
            {
                coolDownTimer -= deltaTime;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                if( coolDownTimer <= 0)
                {
                    foreach (Bullet b in Game1.bullets) 
                    {
                        if( b.IsDead == true )
                        {
                            b.IsDead = false;
                            b.Position = Position;
                            shootFX.Play();
                            b.isFromPlayer = true;
                            break;
                        }
                    }
                    coolDownTimer = 0.3f;
                }
                
            }

            if (CheckCollissions() == true)
            {
                isDead = true;
            }

        }

        public void Draw(SpriteBatch SB)
        {
            if (isDead == true)
            {
                if (lives != 0)
                {
                    position.Y = SB.GraphicsDevice.Viewport.Height / 2;
                    position.X = SB.GraphicsDevice.Viewport.Width / 2;
                    Game1.playerLives--;
                    isDead = false;
                }
                else
                {
                    Game1.gameState = Game1.GameState.GAMEOVER;
                    return;
                }
            }

            if (texture == null)
            {
                return;
            }

            SB.Draw(texture, position, Color.White);

        }

        public bool CheckCollissions()
        {
            Rectangle bounds = Bounds;
            /* check the bullets */
            foreach (Bullet b in Game1.bullets)
            {
                if (b.IsDead == false && b.isFromPlayer == false && bounds.Intersects(b.Bounds) == true)
                {
                    b.IsDead = true;
                    Game1.explosion.Play();
                    lives--;
                    return true;
                }
            }

            return false;
        }

    }
}
