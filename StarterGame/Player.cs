using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace StarterGame
{
    class Player
    {
        private const int width = 20;
        private const int height = 31;
        private Direction direction;

        int jumpspeed = 0;
        double prevFrameTime;
        private float velocityY = 3f;
        const float jumpHeight = 70f;
        double jumpTime = 0f;
        double jumpStartTime;
        private float velocity = 4;
       
        const int bounds = 3;
        private readonly float targetX = 128;
        public Vector2 position;
        public Vector2 scale;
        private Keys prevKeyX;
        private Keys prevKeyY;
        public Rectangle player;
        public Rectangle shadow;
        CollideDirection collide;
        State state;

        private Texture2D olliePop;
        private Texture2D ollieAir;

        int startPosition;

        JumpDirection jumpDirection;

        public Texture2D logo { get; set; }

        public Player(Texture2D pop, Texture2D air)
        {
            direction = Direction.Right;
            state = State.Walking;

            this.olliePop = pop;
            this.ollieAir = air;

            this.shadow = new Rectangle(0, 0, 60, 96);
            collide = CollideDirection.False;
           // logo = right; 
            player = new Rectangle(0, 0, 60, 96);
            position = new Vector2(0, 0);
          //  scale = new Vector2(targetX / (float)logo.Width, targetX / (float)logo.Width);
        }


        public void HandlePosition(Keys[] inputKeys, CollissionObject coll)
        {
            CollissionCheck(coll.block);

            switch (inputKeys.Length)
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
                
                MoveLeft();
      
                prevKeyX = Keys.A;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W))
            {

                MoveUp();
                prevKeyY = Keys.W;

            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                MoveDown();
                prevKeyY = Keys.S;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                MoveRight();
                prevKeyX = Keys.D;
            }
        }

        private void TwoButtonPress()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (prevKeyX == Keys.D)
                {
                    MoveRight(); ;
                }
                else if (prevKeyX == Keys.A)
                {
                    MoveLeft();
                }
           
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (prevKeyY == Keys.W)
                {
                    MoveUp();
                }
                else if (prevKeyY == Keys.S)
                {
                    MoveDown();
                }
                
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                MoveSoutheast();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                MoveSouthwest();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                MoveNorthwest();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                MoveNortheast();
            }
        }

        private void ThreeButtonPress()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (prevKeyX == Keys.A)
                {
                    MoveNorthwest();
                }
                else
                    MoveNortheast();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.W) && Keyboard.GetState().IsKeyDown(Keys.D) && Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (prevKeyY == Keys.W)
                {
                    MoveNortheast();
                }
                else
                    MoveSoutheast();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (prevKeyY == Keys.W)
                {
                    MoveNorthwest();
                }
                else
                    MoveSouthwest();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.A) && Keyboard.GetState().IsKeyDown(Keys.S) && Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (prevKeyX == Keys.A)
                {
                    MoveSouthwest();
                }
                else
                    MoveSoutheast();
            }

        }

        private void CollissionCheck(Rectangle block)
        {

            if (player.Top < block.Bottom && player.Bottom > block.Top)
            {
                if (((player.Right - block.Left) <= bounds) && (player.Right - block.Left) >= -bounds)
                {
                    collide = CollideDirection.Left;
                }
                else if (((player.Left - block.Right) <= bounds) && (player.Left - block.Right) >= -bounds)
                {
                    collide = CollideDirection.Right;
                }
            }
            else if (player.Right > block.Left && player.Left < block.Right)
            {
                if ((player.Bottom - block.Top) <= bounds && (player.Bottom - block.Top) >= -bounds)
                {
                    collide = CollideDirection.Up;
                }
                else if ((player.Top - block.Bottom) <= bounds && (player.Top - block.Bottom) >= - bounds)
                {
                    collide = CollideDirection.Down;
                }
            }
        }


        private void MoveLeft()
        {
            if (collide == CollideDirection.Right)
            {
                collide = CollideDirection.False;
                return;
            }
           // logo = left;
            player.X -= (int)velocity;
            shadow.X -= (int)velocity;
            position.X -= velocity;
            direction = Direction.Left;
        }

        private void MoveRight()
        {
            if (collide == CollideDirection.Left)
            {
                collide = CollideDirection.False;
                return;
            }
           // logo = right;
            player.X += (int)velocity;
            shadow.X += (int)velocity;
            position.X += velocity;
            direction = Direction.Right;
        }

        public void MoveUp()
        {
            if (collide == CollideDirection.Down)
            {
                collide = CollideDirection.False;
                return;
            }
            //logo = up;
            player.Y -= (int)velocity;
            if (state != State.Jumping)
            {
                shadow.Y -= (int)velocity;
            }
            
            position.Y -= velocity;
            direction = Direction.Up;
        }

        private void MoveDown()
        {
            if (collide == CollideDirection.Up)
            {
                collide = CollideDirection.False;
                return;
            }
           // logo = down;
            player.Y += (int)velocity;
            if (state != State.Jumping)
            {
                shadow.Y += (int)velocity;
            }
                
            position.Y += velocity;
            direction = Direction.Down;
        }

        private void MoveNorthwest()
        {
            MoveUp();
            MoveLeft();
            //logo = upLeft;
            prevKeyX = Keys.W;
            direction = Direction.UpLeft;
        }

        private void MoveNortheast()
        {
            MoveUp();
            MoveRight();
            //logo = upRight;
            prevKeyX = Keys.D;
            direction = Direction.UpRight;
        }

        private void MoveSoutheast()
        {
            MoveDown();
            MoveRight();
            //logo = rightDown;
            direction = Direction.DownRight;
        }

        private void MoveSouthwest()
        {
            MoveDown();
            MoveLeft();
            //logo = leftDown;
            direction = Direction.DownLeft;
        }


        private void Jump(double elapsedGameTime)
        {
            if (state != State.Jumping)
            {
                getJumpDirection();
                state = State.Jumping;
                jumpspeed = -14;
                jumpStartTime = elapsedGameTime;
                startPosition = player.Y;
                velocityY = 3f;
            }
        }

        public void UpdateJump(double elapsedGameTime, SoundEffect sound, SoundEffect soundLand)
        {
            if (state == State.Walking)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    sound.Play();
                    logo = olliePop;
                    Jump(elapsedGameTime);
                }
            }
            if (state == State.Jumping)
            {
                if (jumpspeed < -5)
                {
                    logo = olliePop;
                }
                else logo = ollieAir;
               
                player.Y += jumpspeed;

                jumpspeed += 1;
                switch (jumpDirection)
                {
                    case JumpDirection.Left:
                        MoveLeft();
                        break;
                    case JumpDirection.Right:
                        MoveRight();
                        break;
                }

                if (player.Y >= startPosition)
                {
                    soundLand.Play();
                   // logo = right;
                    state = State.Walking;
                    return;
                }

            }
        }
        public void getJumpDirection()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                jumpDirection = JumpDirection.Left;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                jumpDirection = JumpDirection.Right;
            }
            else
                jumpDirection = JumpDirection.None;
        }

        public Rectangle Sprite()
        {
            return new Rectangle(width * (int)direction, 0, width, height);
        }
    }
}
