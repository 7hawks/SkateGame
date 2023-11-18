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
        public static (int width, int height) view = (1920, 1080);
        private const double scale = 3.75;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private SpriteFont font;
        private GameState state;
        private Platform platform;
        private Platform rail;
        private List<Platform> platforms;
        public DustCloud dustCloud;

        private int playerStartX = 0;
        private int playerStartY = 500;
        private int guardInitialX = 180;
        private int guardInitialY = 700;
        public Banana bananaPeel;

        ParticleEngine particleEngine;
        Player player1;
        NonPlayableCharacter beep;
        SecurityGuard guard;
        Boombox boombox;
        public static SoundEffect trickSound;
        public static SoundEffect wreckSound;
        public Rectangle wreck;
        SoundEffect[] sounds = new SoundEffect[5];
        VolumeButton volume;
        Coin[] coins = new Coin[10];
        Menu menu;
        public AudioManager audioManager;
        public Viewport viewport;
        public static int worldWidth = 1600;
        public static int worldHeight = 1280;
        Vector2 cameraPosition = Vector2.Zero;  // Initial camera position
        float cameraLerpFactor = 0.04f;  // Adjust this value for the desired smoothing effect
        float followRatio = 2.0f / 3.0f;  // Two-thirds of the viewport width is behind the player
        float leadRatio = 1.0f / 3.0f;  // One-third of the viewport width is in front of the player

        private Vector2 cursorPos;
        MouseCursor customCursor;
        public HudManager hudManager;
        CollissionObject[] bananas;
        public bool isPaused = false;

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

            // Access the GraphicsDevice
            GraphicsDevice graphicsDevice = GraphicsDevice;

            // Create and configure the Viewport
            Viewport viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = graphicsDevice.PresentationParameters.BackBufferWidth,
                Height = graphicsDevice.PresentationParameters.BackBufferHeight,
                MinDepth = 0,
                MaxDepth = 1
            };

            // Set the Viewport on the GraphicsDevice
            graphicsDevice.Viewport = viewport;

            menu = new Menu(GraphicsDevice, view);
            state = GameState.Menu;
            audioManager = new AudioManager(Content);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            List<Texture2D> textures = new List<Texture2D>();
            textures.Add(Content.Load<Texture2D>("orangePixel"));
            textures.Add(Content.Load<Texture2D>("yellowPixel"));

            hudManager = new HudManager(Content.Load<Texture2D>("kickflipText-export"));

            particleEngine = new ParticleEngine(textures, new Vector2(0, playerStartX));
            platform = new Platform(this.Content.Load<Texture2D>("./platforms/50platform"), 0, 109, Utilities.Scale(71, scale), Utilities.Scale(163, scale), PlatformType.Box);
            rail = new Platform(this.Content.Load<Texture2D>("./platforms/railLonger"), 400, 900, Utilities.Scale(128, scale), Utilities.Scale(16, scale), PlatformType.Rail);
            platforms = new List<Platform>();
          //  platforms.Add(platform);
            platforms.Add(rail);
            trickSound = Content.Load<SoundEffect>("trickSound2");
       
            font = Content.Load<SpriteFont>("fonts");
            player1 = new Player(audioManager, new InputComponent(), playerStartX, playerStartY, 75, 120);
            wreck = new Rectangle(player1.rect.X, player1.rect.Y, 120, 120);
            beep = new NonPlayableCharacter(Content.Load<Texture2D>("beepBeanSpritesheet"), 180, 520, Utilities.Scale(20, scale), Utilities.Scale(28, scale));
            guard = new SecurityGuard(Content.Load<Texture2D>("securityGuard-Sheet"), guardInitialX, guardInitialY, Utilities.Scale(28, scale), Utilities.Scale(32, scale));
            boombox = new Boombox(Content.Load<Texture2D>("boomboxSheet"), 740, 347, Utilities.Scale(16, scale), Utilities.Scale(32, scale));
            dustCloud = new DustCloud();
            bananaPeel = new Banana(Content.Load<Texture2D>("bananaPeel"), 500, 700, Utilities.Scale(16, scale), Utilities.Scale(16, scale));

         //   collissionObj = new CollissionObject(this.Content.Load<Texture2D>("stair10"), PlatformType.Rail);
            volume = new VolumeButton();

            sounds[0] = Content.Load<SoundEffect>("snare 2");
            sounds[1] = Content.Load<SoundEffect>("Open Hi Hat");
            sounds[2] = Content.Load<SoundEffect>("./audio/Hi Hat 1");
            sounds[3] = Content.Load<SoundEffect>("Hi Hat 2");
            sounds[4] = Content.Load<SoundEffect>("Kick");

            for (int k = 0; k < 5; k++)
            {
                coins[k] = new Coin((k * 100) + 450, 800, sounds);
            }
            for (int i = 0; i < 5; i++)
            {
                coins[5 + i] = new Coin(1000, (i * 100) + 580, sounds);
            }
        }
        
        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            cursorPos = new Vector2(mouseState.X, mouseState.Y);
            // track the wreck rectangle to the player rectangle
            wreck.X = player1.rect.X;
            wreck.Y = player1.rect.Y;


            if (platform.rect.Intersects(player1.rect))
            {
                player1.physics.jumpHeight = 0;
            }
            if (!platform.rect.Intersects(player1.rect))
            {
                player1.physics.jumpHeight = Utilities.Scale(Platform.height, 3.75);
            }

            switch (state)
            {
                case GameState.Menu:
                    if (menu.startButton.ButtonCheck())
                    {
                        state = GameState.Game;
                        audioManager.PlayBackgroundMusic();
                    }
                    if (menu.ExitCheck())
                    {
                        Exit();
                    }
                    break;
                case GameState.Game:
                    if (!isPaused)
                    {
                        viewport.X = MathHelper.Clamp(viewport.X, 0, worldWidth - viewport.Width);
                        viewport.Y = MathHelper.Clamp(viewport.Y, 0, worldHeight - viewport.Height);

                        float cameraLeadX = 500;

                        if (player1.direction == Direction.Left)
                        {
                            cameraLeadX = 1000;
                        }

                        Vector2 targetCameraPosition = new Vector2(player1.rect.X - cameraLeadX, player1.rect.Y - 500);

                        cameraPosition = Vector2.Lerp(cameraPosition, targetCameraPosition, cameraLerpFactor);

                        beep.CheckPopup();

                        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                            Exit();

                        foreach (Coin c in coins)
                        {
                            c.AnimateCoin(gameTime.ElapsedGameTime.TotalMilliseconds, player1);
                        }

                        player1.HandlePosition(gameTime.ElapsedGameTime.TotalMilliseconds, platforms, dustCloud, audioManager, hudManager);
                        guard.Update(player1, gameTime, new Vector2(player1.rect.X, player1.rect.Y));
                        particleEngine.Update(player1);
                        beep.Animate(gameTime.ElapsedGameTime.TotalMilliseconds);
                        boombox.Animate(gameTime.ElapsedGameTime.TotalMilliseconds);
                        volume.UpdatePosition((int)cameraPosition.X, (int)cameraPosition.Y);
                        volume.CheckToggle(audioManager);

                        hudManager.Update((int)cameraPosition.X, (int)cameraPosition.Y, gameTime.ElapsedGameTime.TotalMilliseconds);
                    }


                        // Handle input to toggle pause state
                    if (Keyboard.GetState().IsKeyDown(Keys.P))
                    {
                        isPaused = !isPaused;
                    }
                    


                    break;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.PointClamp, transformMatrix: Matrix.CreateTranslation(-cameraPosition.X, -cameraPosition.Y, 0));

            double spriteWidth = this.Content.Load<Texture2D>("backgroundNoSky").Width * scale;
            double spriteHeight = this.Content.Load<Texture2D>("backgroundNoSky").Height * scale;

            // Print any relevant data for testing here:
            TestData test = new TestData();
            test.PrintTestData(spriteBatch, font, player1, volume, particleEngine, cameraPosition, menu, viewport, guard);

            spriteBatch.Draw(this.Content.Load<Texture2D>("backgroundNoSky"), new Rectangle(-1000, 0, (int)spriteWidth, (int)spriteHeight), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 1);


            if (state == GameState.Game)
            {
                foreach (Platform p in platforms) { 
                    if (p.intersects)
                    {
                        spriteBatch.Draw(p.sprite, p.rect, null, Color.LawnGreen, 0, new Vector2(0, 0), SpriteEffects.None, .25f);
                        break;
                    }
                    else
                        spriteBatch.Draw(p.sprite, p.rect, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .25f); 
                }

                spriteBatch.Draw(this.Content.Load<Texture2D>("volume"), volume.rect, volume.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .2f);
                foreach (Coin c in coins)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("coinSpritesheet"), c.rect, c.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .2f);
                }
                if (player1.state == State.Wreck)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("skaterSheet4"), wreck, player1.Sprite(gameTime.ElapsedGameTime.TotalMilliseconds), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);
                }
                else
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("skaterSheet4"), player1.rect, player1.Sprite(gameTime.ElapsedGameTime.TotalMilliseconds), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);
                }
                //spriteBatch.Draw(this.Content.Load<Texture2D>("skaterSheet4"), player1.rect, player1.Sprite(gameTime.ElapsedGameTime.TotalMilliseconds), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);
                spriteBatch.Draw(this.Content.Load<Texture2D>("beepBeanSpritesheet"), beep.rect, beep.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);
                spriteBatch.Draw(this.Content.Load<Texture2D>("boomboxSheet"), boombox.rect, boombox.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);
                spriteBatch.Draw(this.Content.Load<Texture2D>("securityGuard-Sheet"), guard.rect, guard.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);
                spriteBatch.Draw(this.Content.Load<Texture2D>("bananaPeel"), bananaPeel.rect, bananaPeel.Sprite(), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, player1.depthLayer);

                if (beep.dialogue)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("popup"), new Rectangle(300, 500, 800, 200), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .01f);
                    spriteBatch.DrawString(font, beep.popup.message, new Vector2(300, 500), Color.Black);
                }

                if (dustCloud.active)
                {
                    spriteBatch.Draw(this.Content.Load<Texture2D>("./decorations/dustCloudSheet"), dustCloud.rect, dustCloud.Update(gameTime.ElapsedGameTime.TotalMilliseconds), Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .01f);
                }
                hudManager.Draw(spriteBatch);
              //  particleEngine.Draw(spriteBatch, player1);
                if (player1.state == State.Grinding)
                {
                    particleEngine.Draw(spriteBatch, player1);
                }
            }
            else if (state == GameState.Menu)
            {
                var spriteWidth2 = this.Content.Load<Texture2D>("./startMenu/titleCard").Width;  
                var spriteHeight2 = this.Content.Load<Texture2D>("./startMenu/titleCard").Height;

                var screenCenterX = viewport.Width / 2;
                var screenCenterY = viewport.Height / 2;

                var spritePosition = new Vector2(screenCenterX - (spriteWidth2 / 2), screenCenterY - (spriteHeight2 / 2));
                
                spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/titleCard"), new Rectangle(0, 0, viewport.Width, viewport.Height), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
                
                spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/startButton"), menu.startButton.button, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
                spriteBatch.Draw(this.Content.Load<Texture2D>("./startMenu/exitButton"), menu.exitButton, null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, .1f);
            }
            spriteBatch.End();

/*            spriteBatch.Begin();
            if (player1.state == State.Grinding)
            {
                particleEngine.Draw(spriteBatch, player1);
            }
            spriteBatch.End();*/

            base.Draw(gameTime);
        }
    }
}
