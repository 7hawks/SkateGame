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
        public static (int width, int height) view = (1480, 920);
        private const double scale = 3.75;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private GameState state;
        private Platform platform;
        private Platform rail;
        private Platform ramp;
        private List<Platform> platforms;
        public DustCloud dustCloud;
        ParticleEngine particleEngine;
        TrailEngine ghostTrail;
        Player player1;
        NonPlayableCharacter beep;
        public static SoundEffect popSound;
        public static SoundEffect landingSound;
        public static SoundEffect trickSound;
        public static SoundEffect wreckSound;
        public static Song grindSong;
        Song song;
        SoundEffect[] sounds = new SoundEffect[5];
        VolumeButton volume;
        Coin[] coins = new Coin[10];
        Menu menu;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = view.width;
            graphics.PreferredBackBufferHeight = view.height;
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(Content.Load<Texture2D>("orangePixel"));
            textures.Add(Content.Load<Texture2D>("yellowPixel"));

            List<Texture2D> ghostTextures = new List<Texture2D>();
            ghostTextures.Add(Content.Load<Texture2D>("ollieAir"));

            particleEngine = new ParticleEngine(textures, new Vector2(400, 240));
            ghostTrail = new TrailEngine(ghostTextures, new Vector2(400, 240));
            platform = new Platform(this.Content.Load<Texture2D>("./platforms/50platform"), 0, 109, Utilities.Scale(71, scale), Utilities.Scale(163, scale), PlatformType.Box);
            rail = new Platform(this.Content.Load<Texture2D>("./platforms/railLonger"), 400, 500, Utilities.Scale(128, scale), Utilities.Scale(16, scale), PlatformType.Rail);
            ramp = new Platform(this.Content.Load<Texture2D>("./platforms/rampRight"), 0, 500, Utilities.Scale(95, scale), Utilities.Scale(128, scale), PlatformType.RampRight);
            platforms = new List<Platform>();
         
            platforms.Add(rail);
           // platforms.Add(ramp);

            popSound = Content.Load<SoundEffect>("realSkatePop");
            landingSound = Content.Load<SoundEffect>("realSkateLand");
            trickSound = Content.Load<SoundEffect>("trickSound2");
            wreckSound = Content.Load<SoundEffect>("./audio/wreckSound");
            grindSong = Content.Load<Song>("grindSound");
            song = Content.Load<Song>("./audio/JanouTouryuumon");
            
            font = Content.Load<SpriteFont>("fonts");
            player1 = new Player(new InputComponent(), 100, 400, 75, 120);
            beep = new NonPlayableCharacter(Content.Load<Texture2D>("beepBeanSpritesheet"), 700, 300, Utilities.Scale(20, scale), Utilities.Scale(28, scale));
            dustCloud = new DustCloud();

            volume = new VolumeButton();

            sounds[0] = Content.Load<SoundEffect>("snare 2");
            sounds[1] = Content.Load<SoundEffect>("Open Hi Hat");
            sounds[2] = Content.Load<SoundEffect>("./audio/Hi Hat 1");
            sounds[3] = Content.Load<SoundEffect>("Hi Hat 2");
            sounds[4] = Content.Load<SoundEffect>("Kick");

            for (int k = 0; k < 5; k++)
            {
                coins[k] = new Coin(900, (k * 100) + 400, sounds);
            }
            for (int i = 0; i < 5; i++)
            {
                coins[5 + i] = new Coin(1100, (i * 100) + 400, sounds);
            }
        }
        
        protected override void Update(GameTime gameTime)
        {
            if (platform.rect.Intersects(player1.rect))
            {
                player1.physics.jumpHeight = 0;
            }
/*            if (!platform.rect.Intersects(player1.rect))
            {
                player1.physics.jumpHeight = Utilities.Scale(Platform.height, 3.75);
            }*/

            switch (state)
            {
                case GameState.Menu:
                    if (menu.StartCheck())
                    {
                        state = GameState.Game;
                    }
                    if (menu.ExitCheck())
                    {
                        Exit();
                    }
                    break;
                case GameState.Game:
                    beep.CheckPopup();

                    particleEngine.EmitterLocation = new Vector2(player1.rect.X + 50, player1.rect.Y + 120);
                    particleEngine.Update();

                    ghostTrail.EmitterLocation = new Vector2(player1.rect.X + 50, player1.rect.Y + 120);
                    ghostTrail.Update();

                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                        Exit();

                    foreach (Coin c in coins)
                    {
                        c.AnimateCoin(gameTime.ElapsedGameTime.TotalMilliseconds, player1);
                    }

                    player1.Update(gameTime.ElapsedGameTime.TotalMilliseconds, platforms, dustCloud);
                    beep.Animate(gameTime.ElapsedGameTime.TotalMilliseconds);
                    volume.CheckToggle(song);
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(this.Content.Load<Texture2D>("vaperzLarge"), new Rectangle(0, 0, 1480, 920), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 1);
          //  spriteBatch.Draw(this.Content.Load<Texture2D>("lightLayer"), new Rectangle(0, 0, 1200, 720), null, Color.White * .15f, 0, new Vector2(0, 0), SpriteEffects.None, .01f);
            // spriteBatch.Draw(this.Content.Load<Texture2D>("shadow"), player1.shadow, Color.White);                            shadow code here
           
            if (state == GameState.Game)
            {

                // i'm hiding platforms for testing
                foreach (Platform p in platforms)
                {
                    if (p.type == PlatformType.Rail && p.intersects)
                    {
                        spriteBatch.Draw(p.sprite, p.rect, null, Color.LawnGreen, 0, new Vector2(0, 0), SpriteEffects.None, .25f);
                    }
                    else
                        spriteBatch.Draw(p.sprite, p.rect, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .25f);
                }

                spriteBatch.Draw(this.Content.Load<Texture2D>("volume"), volume.rect, volume.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .2f);
                foreach (Coin c in coins)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("coinSpritesheet"), c.rect, c.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .2f);
                }

                spriteBatch.Draw(this.Content.Load<Texture2D>("skaterSheetPush"), player1.rect, player1.Sprite(gameTime.ElapsedGameTime.TotalMilliseconds), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);
                spriteBatch.Draw(this.Content.Load<Texture2D>("beepBeanSpritesheet"), beep.rect, beep.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);


                if (player1.state == State.Push)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("skaterSheetPush"), new Rectangle(player1.rect.X - 30, player1.rect.Y, Utilities.Scale(36, 3.75), Utilities.Scale(36, 3.75)), player1.Sprite(gameTime.ElapsedGameTime.TotalMilliseconds), Color.Snow * .7f, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);
                    // ghostTrail.Draw(spriteBatch);
                }

                spriteBatch.DrawString(font, "State: " + player1.state, new Vector2(0, 0), Color.Black);
                spriteBatch.DrawString(font, "trickState: " + player1.trickState, new Vector2(0, 40), Color.Black);
                spriteBatch.DrawString(font, "wreckFrame: " + player1.wreckFrame, new Vector2(0, 80), Color.Black);
                spriteBatch.DrawString(font, "player Y: " + player1.rect.Y, new Vector2(0, 110), Color.Black);
                spriteBatch.DrawString(font, "Coins: " + player1.coinCount, new Vector2(1050, 120), Color.Lime);
                spriteBatch.DrawString(font, "Points: " + player1.points, new Vector2(1050, 160), Color.Lime);
                spriteBatch.DrawString(font, "friction: " + player1.physics.friction, new Vector2(400, 160), Color.Lime);
                spriteBatch.DrawString(font, "depth: " + player1.depthLayer, new Vector2(400, 180), Color.Lime);
                spriteBatch.DrawString(font, "acceleration: " + player1.acceleration, new Vector2(400, 200), Color.Lime);
                spriteBatch.DrawString(font, "Speed: " + player1.physics.speed, new Vector2(400, 260), Color.Lime);
                spriteBatch.DrawString(font, "player top - block bottom: " + (player1.rect.Top - rail.rect.Bottom), new Vector2(0, 140), Color.Black);
                spriteBatch.DrawString(font, "jump Direction: " + player1.jumpDirection, new Vector2(0, 300), Color.Black);
                spriteBatch.DrawString(font, "prevDirection: " + player1.input.prevDirection, new Vector2(0, 340), Color.Black);
                spriteBatch.DrawString(font, "inputDirection: " + player1.input.direction, new Vector2(0, 400), Color.Black);
                spriteBatch.DrawString(font, "dustCloud active: " + dustCloud.active, new Vector2(0, 440), Color.Black);
                spriteBatch.DrawString(font, "direction: " + player1.direction, new Vector2(0, 500), Color.Black);
                spriteBatch.DrawString(font, "collide: " + player1.physics.collide, new Vector2(0, 530), Color.Black);
                spriteBatch.DrawString(font, "checkViewBox: " + player1.physics.checkViewBox, new Vector2(0, 560), Color.Black);

                if (beep.dialogue)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("popup"), new Rectangle(300, 500, 800, 200), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .01f);
                    spriteBatch.DrawString(font, beep.popup.message, new Vector2(300, 500), Color.Black);
                }

                if (dustCloud.active)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("./decorations/dustCloudSheet"), dustCloud.rect, dustCloud.Update(gameTime.ElapsedGameTime.TotalMilliseconds), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .01f);
                }
            }
            else if (state == GameState.Menu)
            {
                spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/titleCard"), new Rectangle(0, 0, 1200, 720), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
                spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/startButton"), menu.startButton, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
                spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/exitButton"), menu.exitButton, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
            }
            spriteBatch.End();

            if (player1.state == State.Grinding)
            {
                particleEngine.Draw(spriteBatch);
            }

            base.Draw(gameTime);
        }
    }
}
