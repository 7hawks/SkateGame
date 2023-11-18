using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace StarterGame
{
    class SecurityGuard : GameObject
    {
        private int catchThreshold = 50;
        private readonly double interval = 300;
        public int frame = 0;
        private double timer = 0f;
        public bool dialogue;
        public Popup popup;
        public Texture2D logo { get; set; }
        private int initialX;
        private int initialY;

        public enum guardState
        {
            Chasing,
            Resting,
            Boosting
        }

        public guardState CurrentState { get; private set; }

        private float moderateSpeed = 120.0f;
        private float boostSpeed = 5.0f;
        private float acceleration = 1.0f;

        private float restDuration = 2.0f; // Duration to rest in seconds
        private float boostDuration = 1.0f; // Duration to boost in seconds

        private float currentRestTime = 0.0f;
        private float currentBoostTime = 0.0f;

        public Vector2 position;
        public Vector2 velocity;

        public float transformX;
        public float transformY;
        public float elapsedTime;
    //   public Vector2 playerPosition;

        public SecurityGuard(Texture2D inputLogo, int x, int y, int width, int height) : base(x, y, width, height)
        {
            initialX = x;
            initialY = y;
            logo = inputLogo;
            this.CurrentState = guardState.Chasing;
        }


        public void Animate(double elapsedTime)
        {

            timer += elapsedTime;

            if (timer > interval)
            {
                frame++;
                if (frame > 3)
                {
                    frame = 0;
                }
                timer = 0f;
            }
        }

        public Rectangle Sprite()
        {
            return new Rectangle(frame * 32, 0, 32, 32);
        }


        public void Update(Player player, GameTime gameTime, Vector2 playerPosition)
        {
            ChasePlayer(player, playerPosition, moderateSpeed, gameTime);
            this.Animate(gameTime.ElapsedGameTime.TotalMilliseconds);
            /*            switch (CurrentState)
                        {
                            case guardState.Chasing:
                                ChasePlayer(playerPosition, moderateSpeed, gameTime);
                                break;

                            case guardState.Resting:
                                Rest(gameTime);
                                break;

                            case guardState.Boosting:
                                BoostTowardsPlayer(playerPosition, boostSpeed, gameTime);
                                break;
                        }*/

           
        }

        private void ChasePlayer(Player player, Vector2 playerPosition, float speed, GameTime gameTime)
        {
            if (player.state == State.Grinding)
                return;


            if(Vector2.Distance(playerPosition, position) < catchThreshold) // Security guard "catches" player
            {
                player.state = State.Wreck;
                player.busted = true;
                this.Reset();
                return;
            }
            
            

            elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 direction = Vector2.Normalize(playerPosition - position);
            position += direction * speed * elapsedTime;
            this.rect.X = (int)position.X;
            this.rect.Y = (int)position.Y;
        }

        private void Rest(GameTime gameTime)
        {
            // Perform logic related to resting behavior
            currentRestTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Transition to Chasing state after resting for the specified duration
            if (currentRestTime >= restDuration)
            {
                CurrentState = guardState.Chasing;
            }
        }

        private void BoostTowardsPlayer(Vector2 playerPosition, float boostSpeed, GameTime gameTime)
        {
            Vector2 direction = Vector2.Normalize(playerPosition - position);
            velocity = direction * boostSpeed;

            // Perform additional logic related to boosting behavior
            currentBoostTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Transition back to Chasing state after boosting for the specified duration
            if (currentBoostTime >= boostDuration)
            {
                CurrentState = guardState.Chasing;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the NPC using spriteBatch
            // (Assuming you have a texture or sprite for the NPC)
           // spriteBatch.Draw(/*Texture*/, position, Color.White);
        }

        public void Reset()
        {
            this.rect.X = initialX;
            this.rect.Y = initialY;
        }
    }
}
