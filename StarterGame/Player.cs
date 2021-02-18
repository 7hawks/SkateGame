using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace StarterGame
{
    public class Player
    {
        //public const double decelFactor = 
        public float acceleration = 0;
        private bool toggle;
        private const double maxFriction = 1;
        private double interval = 100;
        private double timer = 0f;
        private double grindTimer = 0f;
        public int trickframe = 3;
        private int wreckFrame = 0;
        public Wreck wreck;
        public int coinCount;
        private const float maxSpeed = 4f;
        public float friction = 0.00029f;
        
        public float speed;
        private const int width = 20;
        public const int height = 31;
        private Direction direction;
        public float depthLayer = .1f;
        int jumpspeed = 0;
        const int bounds = 3;
        private Keys prevKeyX;
        private Keys prevKeyY;
        public Rectangle player;
        //public Rectangle shadow;
        public Direction collide;
        public State state;
        public TrickState trickState;
        public int jumpHeight { get; set; }

        int startPosition;

        public Direction jumpDirection;

        public Player()
        {
            toggle = false;
            coinCount = 0;
            trickState = TrickState.None;
            speed = 0;
            direction = Direction.Right;
            jumpDirection = Direction.Right;
            state = State.Grounded;
            jumpHeight = 0;
            wreck = new Wreck();
            //this.shadow = new Rectangle(0, 0, 60, 96);
            collide = Direction.None;
           
            player = new Rectangle(0, 200, 75, 120);
        }

        public void HandlePosition(CollissionObject coll)
        {
            if (state == State.Jumping) { return; }

            if (player.Intersects(coll.block))
            {
               // CollissionCheck(coll.block);
                RampCheck(coll.block);
            }
/*            if (!player.Intersects(coll.block))
            {
                acceleration = 4;
               // friction = .00029f;
                friction = .00149f;
            }*/
            
/*            if (trickState == TrickState.Grinding || (player.Right > coll.block.Left && player.Bottom > coll.block.Top + 110))
            {
                depthLayer = .1f;
            }
            else
                depthLayer = .3f;*/

            if (speed > 0 && !(Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.D)))
            {
                speed *= .003f;
                Move(direction);

                return;
            }

            switch (Keyboard.GetState().GetPressedKeys().Length)
            {
                case 1:
                    OneButtonPress();
                    break;
                case 2:
                    TwoButtonPress();
                    break;
                case 3:
                    ThreeButtonPress();
                    break;
                default:
                    break;
            }
        }

        private void OneButtonPress()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                Move(Direction.Left);
                prevKeyX = Keys.A;
                if (acceleration + .25f < 3)
                {
                    acceleration += .2f;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Move(Direction.Up);
                prevKeyY = Keys.W;
                if (acceleration + .25f < 3)
                {
                    acceleration += .2f;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Move(Direction.Down);
                prevKeyY = Keys.S;
                if (acceleration + .25f < 3)
                {
                    acceleration += .2f;
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                Move(Direction.Right);
                prevKeyX = Keys.D;
                if (acceleration + .25f < 3)
                {
                    acceleration += .2f;
                }
                
            }
        }

        private void TwoButtonPress()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (prevKeyX == Keys.D)
                {
                    Move(Direction.Right); ;
                }
                else if (prevKeyX == Keys.A)
                {
                    Move(Direction.Left);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (prevKeyY == Keys.W)
                {
                    Move(Direction.Up);
                }
                else if (prevKeyY == Keys.S)
                {
                    Move(Direction.Down);
                }
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Move(Direction.DownRight);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                Move(Direction.DownLeft);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Move(Direction.UpLeft);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                Move(Direction.UpRight);
            }
        }

        private void ThreeButtonPress()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (prevKeyX == Keys.A)
                {
                    Move(Direction.UpLeft);
                }
                else
                    Move(Direction.UpRight);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (prevKeyY == Keys.W)
                {
                    Move(Direction.UpRight);
                }
                else
                    Move(Direction.DownRight);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (prevKeyY == Keys.W)
                {
                    Move(Direction.UpLeft);
                }
                else
                    Move(Direction.DownLeft);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (prevKeyX == Keys.A)
                {
                    Move(Direction.DownLeft);
                }
                else
                    Move(Direction.DownRight);
            }
        }

        private void RampCheck(Rectangle ramp)
        {
            if (player.Intersects(ramp))
            {
                if (direction != Direction.Left && player.Right > ramp.Left + 100 && player.Left < ramp.Right - 1)
                {
                    if (player.Right > ramp.Right)
                    {
                        Move(Direction.Right);
                        return;
                    }
                    if (direction == Direction.Right)
                    {
                        acceleration += .25f;
                       // friction += .25f;
                        Move(Direction.DownRight);
                    }
                }
                if (direction == Direction.Left && (player.Left + 5) < ramp.Right)
                {
                    //acceleration *= .85f;
                    if (friction < maxFriction)
                    {
                        friction *= 1.05f;
                    }
                    //Move(Direction.UpLeft);
                    Move(Direction.Up);
                    Move(Direction.Left);
                    Move(Direction.Left);
                }
                if (player.Bottom - ramp.Top >= 100 && player.Bottom - ramp.Top <= 110)
                {
                    collide = Direction.Down;
                }
                if ((player.Top - ramp.Bottom) <= -80 && (player.Top - ramp.Bottom) >= -90)
                {
                    collide = Direction.Up;
                }
            }
        }

        private void CollissionCheck(Rectangle block)
        {
            if (player.Intersects(block))
            {
                if (player.Bottom > block.Top && player.Top < block.Bottom)
                {
                    if ((((player.Right - speed) - block.Left) <= bounds) && ((player.Right - speed) - block.Left) >= -bounds)
                    {
                        collide = Direction.Right;
                    }
                    else if (((player.Left - block.Right) <= bounds) && (player.Left - block.Right) >= -bounds)
                    {
                        collide = Direction.Left;
                    }
                }

                if (player.Bottom - block.Top >= 8 && player.Bottom - block.Top <= 20)
                {
                    collide = Direction.Down;
                }
                if ((player.Top - block.Bottom) <= -60 && (player.Top - block.Bottom) >= -60)
                {
                    collide = Direction.Up;
                }
            }
        }

        private void CalcSpeed()
        {
            if (speed < maxSpeed)
            {
                speed += acceleration;
                //speed -= friction;
                //acceleration *= .991f;
                acceleration -= .009f;
                //speed *= .99f;
            }
        }

        private void Move(Direction inputDirection)
        {
            if (collide == inputDirection)
            {
                collide = Direction.None;
                return;
            }

            CalcSpeed();

            switch (inputDirection)
            {
                case Direction.Up:
                    if (player.Top <= 100)
                        return;
                    player.Y -= (int)speed;
                    player.Y -= (int)speed;
                    break;
                case Direction.Right:
                    if (player.Right >= 1200)
                        return;
                    player.X += (int)speed;
                    player.X += (int)speed;
                    jumpDirection = Direction.Right;
                    break;
                case Direction.Down:
                    if (player.Bottom >= 720)
                        return;
                    player.Y += (int)speed;;
                    player.Y += (int)speed;
                    break;
                case Direction.Left:
                    if (player.Left <= 0)
                        return;

                

                    player.X -= (int)speed;
                    player.X -= (int)speed;
                    jumpDirection = Direction.Left;
                    break;
                case Direction.UpLeft:
                    Move(Direction.Up);
                    Move(Direction.Left);
                    break;
                case Direction.UpRight:
                    Move(Direction.Up);
                    Move(Direction.Right);
                    break;
                case Direction.DownRight:
                    Move(Direction.Down);
                    Move(Direction.Right);
                    break;
                case Direction.DownLeft:
                    Move(Direction.Down);
                    Move(Direction.Left);
                    break;
            }
            direction = inputDirection;
        }

        private void Jump(double elapsedGameTime)
        {
            if (state != State.Jumping)
            {
                state = State.Jumping;
                jumpspeed = -16;
              
                startPosition = player.Y - jumpHeight;
            }
        }

        private void CheckTrick(double elapsedTime, Rectangle rail, Song grindSong)
        {
            if (trickState == TrickState.Kickflip)
            {
                timer += elapsedTime;

                if (timer > interval)
                {
                    trickframe++;

                    if (trickframe > 8)
                    {
                        trickframe = 3;
                    }
                    timer = 0f;
                }
                return;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                    trickState = TrickState.Kickflip;
            }

            if (player.Right > rail.Left && Mouse.GetState().LeftButton == ButtonState.Pressed && player.X - rail.Top < 60 && player.X - rail.Top > -10)
            {
                trickState = TrickState.Grinding;
                depthLayer = .1f;
                MediaPlayer.Play(grindSong);
                player.Y = rail.Y - 100;
            }
        }

        public void UpdateJump(double elapsedTime, SoundEffect sound, SoundEffect landingSound, SoundEffect trickSound, Song grindSong, Rectangle rail)
        {
            state = wreck.WreckCheck(state, elapsedTime, this);
            if(state == State.Wreck)
            {
               // acceleration -= .5f;
            }

            if (trickState == TrickState.Grinding && player.Intersects(rail))
            {
                player.Y = rail.Y - 150 + (player.X / 2);
                Move(jumpDirection);
                return;
            }

            if (state == State.Grounded)
            {
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    sound.Play();
                    state = State.Popped;
                    return;
                }
            }
            if (state == State.Popped)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    Jump(elapsedTime);
                }
            }

            if (state == State.Jumping)
            {
                CheckTrick(elapsedTime, rail, grindSong);
               
                player.Y += jumpspeed;

                jumpspeed += 1;
                switch (jumpDirection)
                {
                    case Direction.Left:
                        Move(Direction.Left);
                        break;
                    case Direction.Right:
                        Move(Direction.Right);
                        break;
                }

                if (player.Y >= startPosition + jumpHeight)
                {
                    landingSound.Play();
                    state = State.Grounded;
                    if (trickState == TrickState.Kickflip  || trickState == TrickState.Grinding)
                    {
                        MediaPlayer.Pause();
                        trickState = TrickState.None;
                       // trickSound2.Play();
                        trickSound.Play();
                    }
                    
                    return;
                }
            }
        }


        public void Reset()
        {
            speed = 0;
            direction = Direction.Right;
            state = State.Grounded;
        }

        public Rectangle Sprite(double elapsedTime)
        {
            if (state == State.Wreck)
            {
                return new Rectangle(wreck.frame * 20, 95, width, height);
            }
            if (state == State.Popped)
            {
                return new Rectangle(21, 31, width, height);
            }
            if (trickState == TrickState.Kickflip)
            {
                return new Rectangle(trickframe * 23, 62, width, height);
            }
            if (state == State.Jumping)
            {
                return new Rectangle(0, 31, width, height);
            }


            if (trickState == TrickState.Grinding)
            {
                grindTimer += elapsedTime;

                if (grindTimer > interval)
                {
                    if (!toggle)
                    {
                        toggle = true;
                    }
                    else
                        toggle = false;
                    grindTimer = 0f;
                }
                if (!toggle)
                {
                    return new Rectangle(width * (int)direction, 0, width, height);
                }
                return new Rectangle(width * (int)direction, 95, width, height);
            }
            return new Rectangle(width * (int)direction, 0, width, height);
        }
    }
}
