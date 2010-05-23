using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace PongXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum GameState {
            Initialize,
            Title,
            Play,
            Result,
        }

        GameState gameState;

        enum PlayState {
            Start,
            Rally,
            Restart,
        }

        PlayState playState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        Texture2D image;
        Texture2D titleImage;
        Texture2D winImage;
        Texture2D enterImage;
        Texture2D[] numberImages = new Texture2D[10];

        Random random = new Random();

        Ball ball;
        Wall northWall;
        Wall southWall;
        Texture2D centerline;
        Paddle[] paddles = new Paddle[2];

        int screenWidth = 640;
        int screenHeight = 480;

        int score1;
        int score2;
        int winPlayer;

        bool oldEnterDown = false;
        bool enterTrigger = false;

        FPSCounter fpsCounter = new FPSCounter();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;

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
            gameState = GameState.Initialize;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            font = Content.Load<SpriteFont>("DefaultFont");
            image = Content.Load<Texture2D>("niconicochan");

            titleImage = Content.Load<Texture2D>("title");
            winImage = Content.Load<Texture2D>("win");
            enterImage = Content.Load<Texture2D>("enter");

            for (int i = 0; i < numberImages.Length; i++)
            {
                numberImages[i] = Content.Load<Texture2D>("number" + i);
            }

            ball = new Ball(Content.Load<Texture2D>("ball"));
            ball.Reset(new Vector2(320, 240), new Vector2(-1, -1), ballStartSpeed);

            Texture2D wallImage = Content.Load<Texture2D>("wall");
            northWall = new Wall(wallImage, new Vector2(0, 1), new Vector2(screenWidth / 2, 10));
            southWall = new Wall(wallImage, new Vector2(0, -1), new Vector2(screenWidth / 2, 470.0f));

            centerline = Content.Load<Texture2D>("centerline");

            Texture2D paddleImage = Content.Load<Texture2D>("paddle");

            paddles[0] = new KeyboardPaddle(paddleImage, new Vector2(1,0), new Vector2(10.0f, 240.0f));
            
            CpuPaddle cpuPaddle = new CpuPaddle(paddleImage, new Vector2(-1, 0), new Vector2(630.0f, 240.0f));
            cpuPaddle.Ball = ball;
            paddles[1] = cpuPaddle;

            gameState = GameState.Title;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            fpsCounter.Update(gameTime.ElapsedRealTime.Milliseconds);


            bool newEnterDown = Keyboard.GetState().IsKeyDown(Keys.Enter);
            enterTrigger = !oldEnterDown & newEnterDown;
            oldEnterDown = newEnterDown;

            switch (gameState)
            {
                case GameState.Initialize:
                    break;
                case GameState.Title:
                    ProcessTitle();
                    break;
                case GameState.Play:
                    ProcessPlay();
                    break;
                case GameState.Result:
                    ProcessResult();
                    break;
                default:
                    break;
            }
            base.Update(gameTime);
        }

        private int titleCounter = 0;
        private void ProcessTitle()
        {
            titleCounter++;
            if (titleCounter >= 120) titleCounter = 0;

            if (enterTrigger)
            {
                startWaitCounter = 0;
                score1 = 0;
                score2 = 0;
                foreach (Paddle paddle in paddles)
                {
                    paddle.Reset();
                }
                gameState = GameState.Play;
                playState = PlayState.Start;
            }
        }

        private int startWaitCounter;
        private void ProcessPlay() {
            switch (playState)
            {
                case PlayState.Start:
                    ProcessPlayStart();
                    break;
                case PlayState.Rally:
                    ProcessPlayRally();
                    break;
                case PlayState.Restart:
                    ProcessPlayRestart();
                    break;
                default:
                    break;
            }
        }

        private float speed = 4.0f;

        private void ProcessPlayStart() {
            if (startWaitCounter == 0) {
                ball.Reset(new Vector2(screenWidth/2, screenHeight/2), Vector2.Zero, 0);
            }
            else if (startWaitCounter > 60)
            {
                float x = (float)(random.NextDouble() - 0.5);
                float y = (float)(random.NextDouble() - 0.5);

                Vector2 startVector = new Vector2(0, -1);
                startVector.Normalize();
                ball.Vector = startVector;
                ball.Speed = speed;
                playState = PlayState.Rally;
            }
            startWaitCounter++;
        }

        private int winScore = 5;
        private void ProcessPlayRally()
        {
            for (int i = 0; i < paddles.Length; i++)
            {
                paddles[i].Update();
            }
            ball.Update();

            ProcesskCollisition();
            
            if (CheckGoal()) {
                if (score1 >= winScore)
                {
                    winPlayer = 0;
                    resultCounter = 0;
                    resultEnterCounter = 0;
                    gameState = GameState.Result;
                }
                else if (score2 >= winScore)
                {
                    winPlayer = 1;
                    resultCounter = 0;
                    resultEnterCounter = 0;
                    gameState = GameState.Result;
                }
                playState = PlayState.Restart;
                restartWaitCounter = 0;
            }
        }

        private int restartWaitCounter;
        private void ProcessPlayRestart()
        {
            if (restartWaitCounter == 0)
            {
                ball.Reset(new Vector2(screenWidth / 2, screenHeight / 2), Vector2.Zero, 0);
            }
            else if (restartWaitCounter > 30)
            {
                float x = (float)(random.NextDouble() - 0.5);
                float y = (float)(random.NextDouble() - 0.5);
                Vector2 startVector = new Vector2(x, y);
                startVector.Normalize();
                ball.Vector = startVector;
                ball.Speed = speed;

                playState = PlayState.Rally;
            }
            restartWaitCounter++;
        }

        private int resultCounter;
        private int resultEnterCounter;
        private void ProcessResult()
        {
            if (resultCounter > 180)
            {
                resultEnterCounter++;
                if (resultEnterCounter > 120) resultEnterCounter = 0;

                if (enterTrigger)
                {
                    gameState = GameState.Title;
                }
            }
            else {
                resultCounter++;
            }
        }

        private Boolean CheckGoal() {
            if (ball.Position.X < 0)
            {
                if (score2 < 9) score2++;
                ball.Reset(new Vector2(320, 240), new Vector2(-1, -1), ballStartSpeed);
                return true;
            }
            if (ball.Position.X > 640)
            {
                if (score1 < 9) score1++;
                ball.Reset(new Vector2(320, 240), new Vector2(-1, -1), ballStartSpeed);
                return true;
            }

            return false;
        }

        private float reflectRandom = 0.24f;
        private float reflectAccele = 0.4f;
        private float ballStartSpeed = 3.0f;
        private float ballLimitSpeed = 20.0f;

        private void ProcesskCollisition() {
            Wall[] walls = new Wall[] { northWall, southWall};
            for (int i = 0; i < walls.Length; i++)
            {
                BoundingBox box = walls[i].Bounding;
                BoundingSphere sphere = ball.Bounding;
                if (box.Intersects(sphere) &&
                    Vector2.Dot(ball.Vector, walls[i].Direction) < 0) {

                    Vector2 reflectVector = Vector2.Reflect(ball.Vector, walls[i].Direction);
                    reflectVector.X += ((float)random.NextDouble() - 0.5f) * 2.0f * reflectRandom;
                    reflectVector.Y += ((float)random.NextDouble() - 0.5f) * 2.0f * reflectRandom;
                    ball.Vector = reflectVector;
                    ball.Speed += reflectAccele;
                }
            }

            // ƒpƒhƒ‹‚ÌÕ“Ë”»’è
            for (int i = 0; i < paddles.Length; i++)
            {
                BoundingBox box = paddles[i].Bounding;
                BoundingSphere sphere = ball.Bounding;
                if (box.Intersects(sphere) &&
                    Vector2.Dot(ball.Vector, paddles[i].Direction) < 0) {

                    Vector2 reflectVector = Vector2.Reflect(ball.Vector, paddles[i].Direction);
                    reflectVector.X += ((float)random.NextDouble() - 0.5f) * 2.0f * reflectRandom;
                    reflectVector.Y += ((float)random.NextDouble() - 0.5f) * 2.0f * reflectRandom;
                    ball.Vector = reflectVector;
                    ball.Speed += reflectAccele;
                }
            }

            if (ball.Speed > ballLimitSpeed) ball.Speed = ballLimitSpeed;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, "FPS:" + fpsCounter.FPS, new Vector2(100, 100), Color.White);
            spriteBatch.End();

            switch (gameState)
            {
                case GameState.Initialize:
                    break;
                case GameState.Title:
                    DrawTitle();
                    break;
                case GameState.Play:
                    DrawPlay();
                    break;
                case GameState.Result:
                    DrawResult();
                    break;
                default:
                    break;
            }

            base.Draw(gameTime);
        }

        private void DrawTitle() {
            spriteBatch.Begin();
            spriteBatch.Draw(titleImage, new Vector2((screenWidth - titleImage.Width) / 2, (screenHeight / 3 - titleImage.Height/2)), Color.White);
            if (titleCounter < 100) {
                spriteBatch.Draw(enterImage, new Vector2((screenWidth - enterImage.Width) / 2, (screenHeight * 3 / 4 - enterImage.Height / 2)), Color.White);
            }
            spriteBatch.End();
        }

        private void DrawPlay() {
            spriteBatch.Begin();
            spriteBatch.Draw(numberImages[score1], new Vector2(190f, 40f), Color.White);
            spriteBatch.Draw(numberImages[score2], new Vector2(390f, 40f), Color.White);

            spriteBatch.Draw(centerline, new Vector2(310.0f, 0.0f), Color.White);

            Vector2 niconicochanPos = (new Vector2(630, 480) - new Vector2(93, 69)) / 2;
            spriteBatch.Draw(image, niconicochanPos, new Color(Color.White, 255));

            northWall.Draw(spriteBatch);
            southWall.Draw(spriteBatch);
            for (int i = 0; i < paddles.Length; i++)
            {
                paddles[i].Draw(spriteBatch);
            }
            ball.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawResult()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(numberImages[score1], new Vector2(190f, 40f), Color.White);
            spriteBatch.Draw(numberImages[score2], new Vector2(390f, 40f), Color.White);

            spriteBatch.Draw(centerline, new Vector2(310.0f, 0.0f), Color.White);

            Vector2 niconicochanPos = (new Vector2(630, 480) - new Vector2(93, 69)) / 2;
            spriteBatch.Draw(image, niconicochanPos, new Color(Color.White, 255));

            northWall.Draw(spriteBatch);
            southWall.Draw(spriteBatch);
            for (int i = 0; i < paddles.Length; i++)
            {
                paddles[i].Draw(spriteBatch);
            }

            Vector2[] winPositions = new Vector2[] {
                new Vector2((screenWidth * 1 / 4 - winImage.Width/2), (screenHeight - winImage.Height) / 2),
                new Vector2((screenWidth * 3 / 4 - winImage.Width/2), (screenHeight - winImage.Height) / 2)
            };
            spriteBatch.Draw(winImage, winPositions[winPlayer], Color.White);

            Vector2[] enterPositions = new Vector2[] {
                new Vector2((screenWidth * 1 / 4 - enterImage.Width/2), (screenHeight * 3 / 4 - enterImage.Height / 2)),
                new Vector2((screenWidth * 3 / 4 - enterImage.Width/2), (screenHeight * 3 / 4 - enterImage.Height / 2))
            };
            if (resultCounter > 180 && resultEnterCounter < 100)
            {
                spriteBatch.Draw(enterImage, enterPositions[winPlayer], Color.White);
            }

            spriteBatch.End();
        }
    }
}
