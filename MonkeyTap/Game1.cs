using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace MonkeyTap
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Texture2D monkey;
        Texture2D background;
        Texture2D logo;
        SpriteFont font;
        SoundEffect hit;
        Song title;

        List<GridCell> grid = new List<GridCell>();

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
                        DisplayRectangle = new Rectangle(x, y, gridWidth, gridHeight)
                    });
                }
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.SaddleBrown);

            // TODO: Add your drawing code here

            spriteBatch.Begin();

            foreach (var square in grid)
            {
                spriteBatch.Draw(monkey, destinationRectangle: square.DisplayRectangle, color: Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
