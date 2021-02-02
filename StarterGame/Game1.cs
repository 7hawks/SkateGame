using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace StarterGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont font;
        
        private CollissionObject collissionObj;
        Player player1;
        SoundEffect soundEffect;
        SoundEffect soundLand;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            soundEffect = Content.Load<SoundEffect>("skatePop2");
            soundLand = Content.Load<SoundEffect>("808clap");

            font = Content.Load<SpriteFont>("fonts");
            player1 = new Player(this.Content.Load<Texture2D>("olliePop"), this.Content.Load<Texture2D>("ollieAir"));
            collissionObj = new CollissionObject(this.Content.Load<Texture2D>("smileyBlue"));

        }
        
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player1.UpdateJump(gameTime.ElapsedGameTime.TotalSeconds, soundEffect, soundLand);
            player1.HandlePosition(Keyboard.GetState().GetPressedKeys(), collissionObj);

            //Point point = new Point(Mouse.GetState().X);
            if (collissionObj.block.Contains(new Point(Mouse.GetState().X)) && collissionObj.block.Contains(new Point(Mouse.GetState().Y)))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    collissionObj.logo = this.Content.Load<Texture2D>("pray");
                }
            }
            else
                collissionObj.logo = this.Content.Load<Texture2D>("smileyBlue");

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            
            _spriteBatch.Draw(this.Content.Load<Texture2D>("pavement"), new Rectangle(0, 0, 800, 480), Color.White);
            _spriteBatch.Draw(this.Content.Load<Texture2D>("shadow"), player1.shadow, Color.White);
            //_spriteBatch.Draw(player1.logo, player1.player, Color.White);
            _spriteBatch.Draw(this.Content.Load<Texture2D>("skaterSheet"), player1.player, player1.Sprite(), Color.White);
            _spriteBatch.Draw(collissionObj.logo, collissionObj.block, Color.White);
            _spriteBatch.DrawString(font, "X: " + player1.player.X, new Vector2(100, 80), Color.Black);
            _spriteBatch.DrawString(font, "Y: " + player1.player.Y, new Vector2(100, 100), Color.Black);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
