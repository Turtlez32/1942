﻿#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;

using System.Xml;
using System.Xml.Linq;
using SimpleAnimation;
#endregion

namespace _1942
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D highScoreBack;
        Texture2D hud;
        Texture2D lives;
        public int levelCount;
        private Texture2D playerPlane;
        public static Player player = null;
        public static int playerLives = 3;
        Tile[,] tileArray = null;
        List<Enemy> enemyList = new List<Enemy>();
        Random random = new Random();
        private Texture2D splashBackground;

        public static SoundEffect explosion;
        SoundEffect select;
        SoundEffect start;

        float spawnTimer = 2;
        public static Bullet[] bullets = new Bullet[50];
        float scrollSpeed = 64.0f;
        int scrollMax = 0;

        public string firstPlaceName;
        public int firstPlaceScore;
        public string secondPlaceName;
        public int secondPlaceScore;
        public string thirdPlaceName;
        public int thirdPlaceScore;
        public Texture2D sTexture;

        public static GameState gameState = GameState.SPLASH;

        public enum GameState
        {
            MENU,
            GAME,
            SPLASH,
            HIGHSCORE,
            GAMEOVER,
        };

        enum EnemyPlane
        {
            EASY,
            MEDIUM,
            HARD,
            BOSS,
        };

        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
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
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 700;

            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            /* load all the content here for the start of the game */
            player = new Player();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            /* call functions for loading content */
            LoadPlayerContent();
            LoadBulletContent();
            LoadTileContent();
            SpawnEnemy();
            GetHighScoreData();

            lives = Content.Load<Texture2D>("HUD/lives.png");
            hud = Content.Load<Texture2D>("HUD/HUD.png");
            font = Content.Load<SpriteFont>("CopperplateGothicBold");
            splashBackground = Content.Load<Texture2D>("Menu.png");
            explosion = Content.Load<SoundEffect>("explosion.wav");
            select = Content.Load<SoundEffect>("select.wav");
            start = Content.Load<SoundEffect>("start.wav");
            highScoreBack = Content.Load<Texture2D>("Finish.png");

        }

        public void LoadTileContent()
        {
            float screenWidth = graphics.GraphicsDevice.Viewport.Width;
            float screenHeight = graphics.GraphicsDevice.Viewport.Height;

            Texture2D grass = Content.Load<Texture2D>("grass.png");

            float tileWidth = grass.Bounds.Width;
            float tileHeight = grass.Bounds.Height;

            int screenTileWidth = (int)(screenWidth / tileWidth) + 1;
            int screenTileHeight = (int)(screenHeight / tileHeight) + 2;

            tileArray = new Tile[screenTileHeight, screenTileWidth];

            for (int y = 0; y < screenTileHeight; y++)
            {
                for (int x = 0; x < screenTileWidth; x++)
                {
                    Tile t = new Tile();
                    t.Texture = grass;
                    t.position = new Vector2(x * tileWidth, y * tileHeight);
                    tileArray[y, x] = t;
                }
            }

            scrollMax = (screenTileHeight - 1) * (int)tileHeight;
        }

        public void LoadPlayerContent()
        {
            playerPlane = Content.Load<Texture2D>("playercopy.png");

            player.PlayerTexture = playerPlane;

            player.Position = new Vector2(
                graphics.GraphicsDevice.Viewport.Width / 2,
                graphics.GraphicsDevice.Viewport.Height - 150);

            player.ShootFX = Content.Load<SoundEffect>("playerPew");
        }

        public void LoadBulletContent()
        {
            Texture2D bulletTexture = Content.Load<Texture2D>("bullet.png");
            InitilizeBullets(bulletTexture);
        }

        public void InitilizeBullets(Texture2D bulletTexture)
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                bullets[i] = new Bullet();
                bullets[i].IsDead = true;
                bullets[i].Texture = bulletTexture;
            }
        }

        public void SpawnEnemy()
        {
            float xPos = random.Next(100, graphics.GraphicsDevice.Viewport.Width - 100);

            Enemy enemy = new Enemy();
            enemy.Position = new Vector2(xPos, -100);
            enemy.Texture = Content.Load<Texture2D>("enemy.png");
            enemy.IsDead = false;

            /* set the enemy health */
            if (player.Score <= 10)
            {
                enemy.health = 30;
            }

            if (player.Score > 10 && player.Score < 20)
            {
                enemy.health = 60;
            }

            enemyList.Add(enemy);
            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            SetHighScoreData();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                select.Play();
                gameState = GameState.GAME;
                start.Play();
            }

            /* initialise the delta timme variable off the game time */
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            /* setup the esc key to exit the game */
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            SpawnTimerTest(deltaTime);
            BackgroundScroll(deltaTime);
            BulletLoop(deltaTime);           

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.MENU: DrawMenu(); break;
                case GameState.SPLASH: DrawSplash(); break;
                case GameState.GAME: DrawGame(gameTime); break;
                case GameState.HIGHSCORE: DrawHighScore(); break;
                case GameState.GAMEOVER: DrawGameOver(); break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void DrawGame(GameTime gameTime)
        {
            foreach (Tile t in tileArray)
            {
                t.Draw(spriteBatch);
            }

            foreach (Bullet b in bullets)
            {
                b.Draw(spriteBatch);
            }

            if (player != null)
                player.Draw(spriteBatch);

            foreach (Enemy enemy in enemyList)
            {
                if (enemy != null)
                    enemy.Draw(spriteBatch);
            }

            spriteBatch.Draw(hud, new Vector2(0, 630), Color.White);
            /*spriteBatch.DrawString(font, "Level : " + levelCount, new Vector2(100, 650), Color.White);*/
            spriteBatch.DrawString(font, "Lives: ", new Vector2(290, 645), Color.White);

            if (playerLives == 3)
            {
                spriteBatch.Draw(lives, new Vector2(365, 650), Color.White);
                spriteBatch.Draw(lives, new Vector2(400, 650), Color.White);
                spriteBatch.Draw(lives, new Vector2(435, 650), Color.White);
            }
            else if (playerLives == 2)
            {
                spriteBatch.Draw(lives, new Vector2(400, 650), Color.White);
                spriteBatch.Draw(lives, new Vector2(435, 650), Color.White);
            }
            else if (playerLives == 1)
            {
                spriteBatch.Draw(lives, new Vector2(435, 650), Color.White);
            }

            spriteBatch.DrawString(font, "1 UP", new Vector2(20, 10), Color.White);
            spriteBatch.DrawString(font, "Score: " + Game1.player.Score, new Vector2(20, 650), Color.White);

            UpdateGame(gameTime);
        }

        public void UpdateGame(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            /* check if player == null*/
            if (player != null)
                player.Update(deltaTime);

            foreach (Enemy enemy in enemyList)
            {
                if (enemy != null && enemy.IsDead == false)
                {
                    enemy.Update(deltaTime);

                    if (enemy.Position.Y > graphics.GraphicsDevice.Viewport.Height + 100)
                    {
                        enemy.IsDead = true;
                    }
                }
            }

            foreach (Enemy enemy in enemyList)
            {
                if (enemy.IsDead == true)
                {
                    enemyList.Remove(enemy);
                    break;
                }
            }
        }

        public void DrawSplash()
        {
            spriteBatch.Draw(splashBackground, new Vector2(0, 0), Color.White);
        }

        public void DrawMenu()
        {
        }
        
        public void DrawHighScore()
        {
           
        }

        public void SpawnTimerTest(float deltaTime)
        {
            spawnTimer -= deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnEnemy();
                spawnTimer = 2;
            }
        }

        public void BackgroundScroll(float deltaTime)
        {

            for (int y = 0; y < tileArray.GetLength(0); y++)
            {
                for (int x = 0; x < tileArray.GetLength(1); x++)
                {
                    Tile t = tileArray[y, x];

                    t.position.Y += scrollSpeed * deltaTime;

                    if (t.position.Y > scrollMax)
                    {
                        t.position.Y -= scrollMax + t.Texture.Height;
                    }
                }
            }

        }

        public void BulletLoop(float deltaTime)
        {
            foreach (Bullet b in bullets)
            {
                b.Update(deltaTime);

                if (b.IsDead == false)
                {
                    if (b.isFromPlayer == true && b.Position.Y < 0)
                    {
                        b.IsDead = true;
                    }

                    if (b.isFromPlayer == false && b.Position.Y > graphics.GraphicsDevice.Viewport.Height)
                    {
                        b.IsDead = true;
                    }
                }
            }
        }

        public void DrawGameOver()
        {
            spriteBatch.Draw(highScoreBack, new Vector2(0,0), Color.White);
            spriteBatch.DrawString(font, "HighScore:", new Vector2(100, 200), Color.White);
            spriteBatch.DrawString(font, "First Place Name:  " + firstPlaceName, new Vector2(100, 230), Color.White);
            spriteBatch.DrawString(font, "First Place Score: " + firstPlaceScore, new Vector2(100, 260), Color.White);
            spriteBatch.DrawString(font, "First Place Name:  " + secondPlaceName, new Vector2(100, 290), Color.White);
            spriteBatch.DrawString(font, "First Place Score: " + secondPlaceScore, new Vector2(100, 320), Color.White);
            spriteBatch.DrawString(font, "First Place Name:  " + thirdPlaceName, new Vector2(100, 350), Color.White);
            spriteBatch.DrawString(font, "First Place Score: " + thirdPlaceScore, new Vector2(100, 380), Color.White);

            CheckHighScores();
        }

        public void GetHighScoreData()
        {
            int i = 0;
            string line;
            string[] input = new string[3];

            StreamReader file = new StreamReader("./Content/HighScore.txt");

            while ((line = file.ReadLine()) != null)
            {
                if (i == 0)
                {
                    input = line.Split(' ');
                    firstPlaceName = input[0].ToString();
                    firstPlaceScore = int.Parse(input[1].ToString());                    
                }

                if (i == 1)
                {
                    input = line.Split(' ');
                    secondPlaceName = input[0].ToString();
                    secondPlaceScore = int.Parse(input[1].ToString()); 
                }

                if (i == 2)
                {
                    input = line.Split(' ');
                    thirdPlaceName = input[0].ToString();
                    thirdPlaceScore = int.Parse(input[1].ToString()); 
                }

                i++;
            }

            file.Close();
        }

        public void SetHighScoreData()
        {
            string[] lines = new string[6];
            TextWriter tw = new StreamWriter(@"./Content/HighScore.txt");

            /* check for the players score before saving the file. */
            CheckHighScores();

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        tw.Write(firstPlaceName);
                        tw.Write(" ");
                        tw.WriteLine(firstPlaceScore);
                        break;
                    case 1: 
                        tw.Write(secondPlaceName);
                        tw.Write(" ");
                        tw.WriteLine(secondPlaceScore);
                        break;
                    case 2: 
                        tw.Write(thirdPlaceName);
                        tw.Write(" ");
                        tw.WriteLine(thirdPlaceScore);
                        break;
                }
            }

            tw.Close();


        }

        public void CheckHighScores()
        {
            if (Game1.player.Score > firstPlaceScore)
            {
                firstPlaceScore = Game1.player.Score;
            }

            if (Game1.player.Score > secondPlaceScore)
            {
                secondPlaceScore = Game1.player.Score;
            }

            if (Game1.player.Score > thirdPlaceScore)
            {
                thirdPlaceScore = Game1.player.Score;
            }
            
        }

    }
}
