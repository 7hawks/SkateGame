using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace StarterGame
{
    public class Game1 : Game
    {
        private const double scale = 3.75;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private bool displayPopup;
        private string popUpText;
        private GameState state;
        private Platform platform;
        
        private CollissionObject collissionObj;
        ParticleEngine particleEngine;
        Player player1;
        NonPlayableCharacter beep;
        SoundEffect soundEffect;
        SoundEffect soundLand;
        SoundEffect trickSound;
        Song grindSong;
        Song song;
        SoundEffect[] sounds = new SoundEffect[5];
        VolumeButton volume;
        Coin[] coins = new Coin[10];
        Menu menu;
        //MediaPlayer = new MediaPlayer();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 720;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            menu = new Menu();
            state = GameState.Menu;
        }

        protected override void LoadContent()
        {
            displayPopup = false;
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //mediaPlayer = new MediaPlayer();

            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(Content.Load<Texture2D>("orangePixel"));
            textures.Add(Content.Load<Texture2D>("yellowPixel"));
       
            particleEngine = new ParticleEngine(textures, new Vector2(400, 240));
            platform = new Platform(0, 109, Utilities.Scale(71, scale), Utilities.Scale(163, scale));
            soundEffect = Content.Load<SoundEffect>("realSkatePop");
            soundLand = Content.Load<SoundEffect>("realSkateLand");
            trickSound = Content.Load<SoundEffect>("trickSound2");
            grindSong = Content.Load<Song>("grindSound");
            song = Content.Load<Song>("./audio/JanouTouryuumon");
            //MediaPlayer.Play(song);
            font = Content.Load<SpriteFont>("fonts");
            player1 = new Player();
            beep = new NonPlayableCharacter(Content.Load<Texture2D>("beepBeanSpritesheet"), 600, 200, Utilities.Scale(20, scale), Utilities.Scale(28, scale));
            collissionObj = new CollissionObject(this.Content.Load<Texture2D>("stair10"), PlatformType.Rail);
            volume = new VolumeButton();

            sounds[0] = Content.Load<SoundEffect>("snare 2");
            sounds[1] = Content.Load<SoundEffect>("Open Hi Hat");
            sounds[2] = Content.Load<SoundEffect>("./audio/Hi Hat 1");
            sounds[3] = Content.Load<SoundEffect>("Hi Hat 2");
            sounds[4] = Content.Load<SoundEffect>("Kick");

            for (int k = 0; k < 5; k++)
            {
                coins[k] = new Coin(800, (k * 100) + 200, sounds);
            }
            for (int i = 0; i < 5; i++)
            {
                coins[5 + i] = new Coin(1000, (i * 100) + 200, sounds);
            }
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (platform.rect.Intersects(player1.player))
            {
                player1.jumpHeight = 0;
            }
            if (!platform.rect.Intersects(player1.player))
            {
                player1.jumpHeight = Utilities.Scale(Platform.height, 3.75);
            }

                if (state == GameState.Menu)
            {
                if (menu.StartCheck())
                {
                    state = GameState.Game;
                }
                if (menu.ExitCheck())
                {
                    this.Exit();
                }
            }
            else if (state == GameState.Game)
            {
                beep.CheckPopup();

                particleEngine.EmitterLocation = new Vector2(player1.player.X + 50, player1.player.Y + 120);
                particleEngine.Update();

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                foreach (Coin c in coins)
                {
                    c.AnimateCoin(gameTime.ElapsedGameTime.TotalMilliseconds, player1);
                }

                player1.UpdateJump(gameTime.ElapsedGameTime.TotalMilliseconds, soundEffect, soundLand, trickSound, grindSong, collissionObj.block);
                player1.HandlePosition(collissionObj);
                beep.Animate(gameTime.ElapsedGameTime.TotalMilliseconds);
                volume.CheckToggle(song);
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(this.Content.Load<Texture2D>("vaperzBg2"), new Rectangle(0, 0, 1200, 720), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 1);
            spriteBatch.Draw(this.Content.Load<Texture2D>("lightLayer"), new Rectangle(0, 0, 1200, 720), null, Color.White * .15f, 0, new Vector2(0, 0), SpriteEffects.None, .01f);
            // spriteBatch.Draw(this.Content.Load<Texture2D>("shadow"), player1.shadow, Color.White);                            shadow code here
           
            if (state == GameState.Game)
            {
                spriteBatch.Draw(collissionObj.logo, collissionObj.block, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .25f);   // stair set
                spriteBatch.Draw(this.Content.Load<Texture2D>("./platforms/50platform"), platform.rect, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .25f);
                //fspriteBatch.Draw(collissionObj.logo, collissionObj.block, new Rectangle(player1.player.X * -1, player1.player.Y * -1, 200, 200), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .25f);
                spriteBatch.Draw(this.Content.Load<Texture2D>("volume"), volume.rect, volume.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .2f);
                foreach (Coin c in coins)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("coinSpritesheet"), c.rect, c.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .2f);
                }

                spriteBatch.Draw(this.Content.Load<Texture2D>("skaterSheetWreck"), player1.player, player1.Sprite(gameTime.ElapsedGameTime.TotalMilliseconds), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);
                spriteBatch.Draw(this.Content.Load<Texture2D>("beepBeanSpritesheet"), beep.rect, beep.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);

                spriteBatch.DrawString(font, "Coins: " + player1.coinCount, new Vector2(1050, 120), Color.Lime);
                spriteBatch.DrawString(font, "friction: " + player1.friction, new Vector2(400, 160), Color.Lime);
                spriteBatch.DrawString(font, "depth: " + player1.depthLayer, new Vector2(400, 180), Color.Lime);
                spriteBatch.DrawString(font, "acceleration: " + player1.acceleration, new Vector2(400, 200), Color.Lime);
                spriteBatch.DrawString(font, "Speed: " + player1.speed, new Vector2(400, 260), Color.Lime);
                spriteBatch.DrawString(font, "JumpHeight: " + player1.jumpHeight, new Vector2(400, 40), Color.Pink);
                spriteBatch.DrawString(font, "frame: " + player1.wreck.frame, new Vector2(100, 60), Color.Black);
                spriteBatch.DrawString(font, "X: " + player1.player.X, new Vector2(100, 80), Color.Black);
                spriteBatch.DrawString(font, "Y: " + player1.player.Y, new Vector2(100, 100), Color.Black);
                spriteBatch.DrawString(font, "jump direction: " + player1.jumpDirection, new Vector2(0, 500), Color.Black);
            //    spriteBatch.DrawString(font, "player.bottom - block.top: " + (player1.player.Bottom - collissionObj.block.Top), new Vector2(100, 120), Color.Black);
                if (beep.dialogue)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("popup"), new Rectangle(300, 500, 800, 200), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .01f);
                    spriteBatch.DrawString(font, beep.popup.message, new Vector2(300, 500), Color.Black);
                }
                //spriteBatch.DrawString(font, "player.top " + player1.player.Top + "block.top " + collissionObj.block.Top, new Vector2(100, 160), Color.Black);
               // spriteBatch.DrawString(font, "player.bottom: " + player1.player.Bottom + "block.bottom: " + collissionObj.block.Bottom, new Vector2(100, 180), Color.Black);
            }
            else if (state == GameState.Menu)
            {
                spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/titleCard"), new Rectangle(0, 0, 1200, 720), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
                spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/startButton"), menu.startButton, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
                spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/exitButton"), menu.exitButton, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
            }


            spriteBatch.End();


            if (player1.trickState == TrickState.Grinding)
            {
                particleEngine.Draw(spriteBatch);
            }
            base.Draw(gameTime);
        }
    }
}
