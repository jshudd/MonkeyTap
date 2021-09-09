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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            spriteBatch.Begin();
            spriteBatch.Draw(monkey, Vector2.Zero, null, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
