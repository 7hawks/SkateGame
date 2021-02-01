using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace StarterGame
{
    class Player
    {
        int jumpspeed = 0;
        double prevFrameTime;
        private float velocityY = 3f;
        const float jumpHeight = 70f;
        double jumpTime = 0f;
        double jumpStartTime;
        private float velocity = 4;
        //private float reboundVelocity = 2;
        const int bounds = 3;
        private readonly float targetX = 128;
        public Vector2 position;
        public Vector2 scale;
        private Keys prevKeyX;
        private Keys prevKeyY;
        public Rectangle player;
        CollideDirection collide;
        State state;
        private Texture2D left;
        private Texture2D right;
        private Texture2D rightDown;
        private Texture2D leftDown;
        private Texture2D up;
        private Texture2D down;
        int startPosition;
        private float gravity = 2;
        JumpDirection jumpDirection;
       // Velocity = Vector2.Zero;
        

        public Texture2D logo { get; set; }

        public Player(Texture2D left, Texture2D right, Texture2D rightDown, Texture2D leftDown, Texture2D up, Texture2D down)
        {
            state = State.Walking;
            this.left = left;
            this.right = right;
            this.rightDown = rightDown;
            this.leftDown = leftDown;
            this.up = up;
            this.down = down;
            collide = CollideDirection.False;
            logo = right; 
            player = new Rectangle(0, 0, 60, 96);
            position = new Vector2(0, 0);
            scale = new Vector2(targetX / (float)logo.Width, targetX / (float)logo.Width);
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
            logo = left;
            player.X -= (int)velocity;
            position.X -= velocity;
        }

        private void MoveRight()
        {
            if (collide == CollideDirection.Left)
            {
                collide = CollideDirection.False;
                return;
            }
            logo = right;
            player.X += (int)velocity;
            position.X += velocity;
        }

        public void MoveUp()
        {
            if (collide == CollideDirection.Down)
            {
                collide = CollideDirection.False;
                return;
            }
            logo = up;
            player.Y -= (int)velocity;
            position.Y -= velocity;
        }

        private void MoveDown()
        {
            if (collide == CollideDirection.Up)
            {
                collide = CollideDirection.False;
                return;
            }
            logo = down;
            player.Y += (int)velocity;
            position.Y += velocity;
        }

        private void MoveNorthwest()
        {
            MoveUp();
            MoveLeft();
            prevKeyX = Keys.W;
        }

        private void MoveNortheast()
        {
            MoveUp();
            MoveRight();

            prevKeyX = Keys.D;
        }

        private void MoveSoutheast()
        {
            MoveDown();
            MoveRight();
            logo = rightDown;
        }

        private void MoveSouthwest()
        {
            MoveDown();
            MoveLeft();
            logo = leftDown;
        }


        private void Jump(double elapsedGameTime)
        {
            if (state != State.Jumping) ;
            {
                getJumpDirection();
                state = State.Jumping;
                jumpspeed = -14;
                jumpStartTime = elapsedGameTime;
                startPosition = player.Y;
                velocityY = 3f;
            }
        }

        public void UpdateJump(double elapsedGameTime, SoundEffect sound)
        {
            if (state == State.Walking)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    sound.Play();
                    Jump(elapsedGameTime);
                }
            }
            if (state == State.Jumping)
            {
                player.Y += jumpspeed;
/*                if (prevKeyX == Keys.D)
                {
                    player.X += (int)velocity;
                }
                else if (prevKeyX == Keys.A)
                {
                    player.X -= (int)velocity;
                }*/
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


                // jumpTime += elapsedGameTime;
               // prevFrameTime += elapsedGameTime;
               // float dt = (float)elapsedGameTime - (float)prevFrameTime;
                if (player.Y >= startPosition)
                {
                    state = State.Walking;
                    return;
                }
/*                if (dt > 0.15f)
                {
                    dt = 0.15f;
                }*/


               // player.Y -= (int)(velocityY * dt) ;
               // position.Y -= velocityY * dt;
               // velocityY += gravity * dt;

/*                if (startPosition - player.Y < 50)
                {
                    MoveUp();
                   // this.Velocity.Y++;
                }*/
                
/*                if (player.Y > startPosition)
                {
                    state = State.Walking;
                }*/
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
    }
}
