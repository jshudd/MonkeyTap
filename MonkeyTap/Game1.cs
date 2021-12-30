using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;


namespace MonkeyTap
{
    // following line fixes MediaPlayer issues for iOS
    using MediaPlayer = Microsoft.Xna.Framework.Media.MediaPlayer;

    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Texture2D monkey;
        private Texture2D background;
        private Texture2D logo;
        private SpriteFont font;
        private SoundEffect hit;
        private Song title;
        
        List<GridCell> grid = new List<GridCell>();

        enum GameState
        {
            Start,
            Playing,
            GameOver
        }

        //Set initial game state
        GameState currentState = GameState.Start;
        Random rnd = new Random();

        //Test to display to user
        string gameOverText = "Game Over";
        string tapToStartText = "Tap to Start";
        string scoreText = "Score: {0}";

        //Timers: Calculate when events should occur in our game
        TimeSpan gameTimer = TimeSpan.FromMilliseconds(0);
        //Define how often the level difficulty increases
        TimeSpan increaseLevelTimer = TimeSpan.FromMilliseconds(0);
        //Define the delay between game ending and new game beginning
        TimeSpan tapToRestartTimer = TimeSpan.FromSeconds(2);

        //how many cells should be altered in a level
        int cellsToChange = 0;
        int maxCells = 1;
        int maxCellsToChange = 14;
        int score = 0;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.SupportedOrientations = DisplayOrientation.Portrait;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // Set window size for OpenGL; add to project instructions & explain            
            if (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width > 1300)
            {
                graphics.IsFullScreen = false;
                graphics.PreferredBackBufferWidth = 512; // width of background image
                graphics.PreferredBackBufferHeight = 1024; // height of background image
                graphics.ApplyChanges();
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            monkey = Content.Load<Texture2D>("monkey");
            background = Content.Load<Texture2D>("background");
            logo = Content.Load<Texture2D>("logo");
            font = Content.Load<SpriteFont>("font");
            hit = Content.Load<SoundEffect>("hit");
            title = Content.Load<Song>("title");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(title);

            //load display stuff
            var viewport = graphics.GraphicsDevice.Viewport;
            var padding = (viewport.Width / 100);
            var gridWidth = (viewport.Width - (padding * 5)) / 4;
            var gridHeight = gridWidth;

            for (int y = padding; y < gridHeight * 5; y+=gridHeight+padding)
            {
                for (int x = padding; x < viewport.Width - gridWidth; x += gridWidth + padding)
                {
                    grid.Add(new GridCell()
                    {
                        DisplayRectangle = new Rectangle(x, y + (int)(gridHeight / .5), gridWidth, gridHeight) // Add gridHeight/.5 to y to help center grid; cast to int
                    });
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            //For mobile, this logic will close the game when the Back button is pressed
            //Exit() is obsolete on iOS
#if !_IOS_ && !_TVOS_
            // commented due to iOS issue
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
#endif
            // TODO: Add your update logic here

            //Custom logic
            var touchState = TouchPanel.GetState();
            switch (currentState)
            {
                case GameState.Start:
                    if (touchState.Count > 0)
                    {
                        currentState = GameState.Playing;
                    }
                    break;
                case GameState.Playing:
                    PlayGame(gameTime, touchState);
                    break;
                case GameState.GameOver:
                    tapToRestartTimer -= gameTime.ElapsedGameTime;
                    if (touchState.Count > 0 && tapToRestartTimer.TotalMilliseconds < 0)
                    {
                        currentState = GameState.Start;
                        score = 0;
                        increaseLevelTimer = TimeSpan.FromMilliseconds(0);
                        gameTimer = TimeSpan.FromMilliseconds(0);
                        cellsToChange = 1;
                        maxCells = 1;
                        for (int i = 0; i < grid.Count; i++)
                        {
                            grid[i].Reset();
                        }
                    }
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.SaddleBrown);

            // TODO: Add your drawing code here

            //Calculate the center of the screen
            var center = graphics.GraphicsDevice.Viewport.Bounds.Center.ToVector2();

            //Calculate half the width of the screen
            var half = graphics.GraphicsDevice.Viewport.Width / 2;

            //Calculate aspect ratio of the MonkeyTap logo
            var aspect = (float)logo.Height / logo.Width;

            //Calculate position of logo on screen
            var rect = new Rectangle((int)center.X - (half / 2), 0, half, (int)(half * aspect));

            spriteBatch.Begin();

            //Draw the background
            spriteBatch.Draw(background, destinationRectangle: graphics.GraphicsDevice.Viewport.Bounds, color: Color.White);

            //Draw MonkeyTap logo
            spriteBatch.Draw(logo, destinationRectangle: rect, color: Color.White);

            //Draw a grid of squares
            foreach (var square in grid)
            {
                spriteBatch.Draw(monkey, destinationRectangle: square.DisplayRectangle, color: Color.Lerp(Color.TransparentBlack, square.Color, square.Transition));
            }

            // If the game is over, draw the score and game over text in the center of screen.
            if(currentState == GameState.GameOver)
            {
                //Measure the text so we can center if correctly
                var v = new Vector2(font.MeasureString(gameOverText).X / 2, 0);
                spriteBatch.DrawString(font, gameOverText, center - v, Color.OrangeRed);

                var t = string.Format(scoreText, score);

                //Measure the text so we can center it correctly
                v = new Vector2(font.MeasureString(t).X / 2, 0);

                //We can use the font.LineSpacing to draw on the line underneath the "Game Over" text
                spriteBatch.DrawString(font, t, center + new Vector2(-v.X, font.LineSpacing), Color.White);
            }

            //if the game is starting over, add "Tap to Start" text
            if(currentState == GameState.Start)
            {
                //Measure the text so we can center it correctly
                var v = new Vector2(font.MeasureString(tapToStartText).X / 2, 0);
                spriteBatch.DrawString(font, tapToStartText, center - v, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        void ProcessTouches(TouchCollection touchState)
        {
            foreach (var touch in touchState)
            {
                if (touch.State != TouchLocationState.Released)
                    continue;
                for (int i = 0; i < grid.Count; i++)
                {
                    if (grid[i].DisplayRectangle.Contains(touch.Position) && grid[i].Color == Color.White)
                    {
                        hit.Play();
                        grid[i].Reset();
                        score += 1;
                    }
                }
            }
        }

        void CheckForGameOver(GameTime gameTime)
        {
            for (int i = 0; i < grid.Count; i++)
            {
                if (grid[i].Update(gameTime))
                {
                    currentState = GameState.GameOver;
                    tapToRestartTimer = TimeSpan.FromSeconds(2);
                    break;
                }
            }
        }

        void CalculateCellsToChange(GameTime gameTime)
        {
            gameTimer += gameTime.ElapsedGameTime;
            if(gameTimer.TotalSeconds > 2)
            {
                gameTimer = TimeSpan.FromMilliseconds(0);
                cellsToChange = Math.Min(maxCells, maxCellsToChange);
            }
        }

        void IncreaseLevel(GameTime gameTime)
        {
            increaseLevelTimer += gameTime.ElapsedGameTime;
            if(increaseLevelTimer.TotalSeconds > 10)
            {
                increaseLevelTimer = TimeSpan.FromMilliseconds(0);
                maxCells++;
            }
        }

        void MakeMonkeyVisible()
        {
            if(cellsToChange > 0)
            {
                var idx = rnd.Next(grid.Count);
                if (grid[idx].Color == Color.TransparentBlack)
                {
                    grid[idx].Show();
                    cellsToChange--;
                }
            }
        }

        void PlayGame(GameTime gameTime, TouchCollection touchState)
        {
            ProcessTouches(touchState);
            CheckForGameOver(gameTime);
            CalculateCellsToChange(gameTime);
            MakeMonkeyVisible();
            IncreaseLevel(gameTime);
        }
    }
}
