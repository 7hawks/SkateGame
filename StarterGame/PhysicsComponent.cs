using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarterGame
{
    
    public class PhysicsComponent : IGameComponent
    {
        const float accelFactor = .2f;
        const int bounds = 3;
        public int jumpSpeed;
        public float speed;
        private const float maxAcceleration = 4;
        public float maxSpeed = 8f;
        public  float minSpeed = -8f;
        public float friction = 0.00029f;
        public const double maxFriction = 1;
        public int jumpHeight { get; set; }
        private bool decelerate;
        public int pushFrame;
        private const int pushInterval = 100;
        private double timer;
        public Direction collide;

        public PhysicsComponent()
        {
            collide = Direction.None;
            jumpHeight = 0;
            jumpSpeed = 0;
            pushFrame = 0;
            speed = 0;
            timer = 0;
        }


        private void CalcSpeed(Player player)
        {
            if (speed + player.acceleration > maxSpeed)
            {
                speed = maxSpeed;
                return;
            }
            else
                speed += .2f;
            //  speed += player.acceleration;
        }

        /// <summary>
        ///     Iterates through the game world platforms to check if player collides
        /// </summary>
        /// <param name="block"></param>
        public void CollissionCheck(Rectangle playerRect, Rectangle block)
        {
            if (playerRect.Intersects(block))
            {
                if (playerRect.Bottom > block.Top && playerRect.Top < block.Bottom)
                {
                    if ((((playerRect.Right - speed) - block.Left) <= bounds) && ((playerRect.Right - speed) - block.Left) >= -bounds)
                    {
                        collide = Direction.Right;
                        return;
                    }
                    else if (((playerRect.Left - block.Right) <= bounds) && (playerRect.Left - block.Right) >= -bounds)
                    {
                        collide = Direction.Left;
                        return;
                    }
                }

                if ((playerRect.Bottom - block.Top >= 8 && playerRect.Bottom - block.Top <= 20) || playerRect.Bottom - block.Bottom > -200 && playerRect.Bottom - block.Bottom < 0)
                {
                    collide = Direction.Down;
                    return;
                }
                if ((playerRect.Top - block.Bottom) <= -60 && (playerRect.Top - block.Bottom) >= -60)
                {
                    collide = Direction.Up;
                    return;
                }
            }
            collide = Direction.None;
        }

        public void Decelerate(float acceleration)
        {
            if (speed - acceleration < minSpeed)
            {
                speed = minSpeed;
                return;
            }
            else
                speed -= acceleration;

        }

        public void UpdateGrind(Player player, List<Platform> platforms)
        {
            if (player.state != State.Grinding)
                return;

            if (player.rect.Y >= player.startPosition + jumpHeight)
            {
/*                foreach (Platform p in platforms)
                {
                    p.intersects = false;
                }*/
                Game1.landingSound.Play();
                player.state = State.Grounded;
                MediaPlayer.Pause();
                player.trickState = TrickState.None;
                Game1.trickSound.Play();
            }

            foreach (Platform p in platforms)
            {
                if (p.type == PlatformType.Rail && player.rect.Intersects(p.rect))
                {
                    return;
                }
            }
            player.state = State.Jumping;
        }


        public void Update(Player player, double elapsedTime, List<Platform> platforms, DustCloud dustCloud)
        {

            foreach (Platform p in platforms)
            {
                if (p.type == PlatformType.Rail) // for testing purposes
                {
                    if (player.rect.Right > p.rect.Left && player.rect.Left < p.rect.Right && player.rect.Y - p.rect.Top <= 100 && player.rect.Y - p.rect.Top >= -200) 
                    {
                        p.intersects = true;
                    }
                    else
                        p.intersects = false;
                }

            }

                player.state = player.wreck.WreckCheck(player.state, elapsedTime, player);
            switch (player.state)
            {
                case State.Jumping:
                    UpdateJump(player, elapsedTime, platforms, dustCloud);
                    CheckGrind(player, platforms);
                    if (player.input.direction == Direction.Up)   // prevents being able to super jump by pressing UP while jumping
                        Move(player, player.jumpDirection);
                    else
                        Move(player, player.input.direction); 
                    break;
                case State.Grounded:
                    UpdateJump(player, elapsedTime, platforms, dustCloud);

                    foreach ( Platform p in platforms)
                    {
                        if (player.direction == Direction.Right && player.rect.Right > p.rect.Right && jumpHeight == 0)
                        {
                            Fall(player);
                        }
                    }

                    if (speed > .0001 && player.input.direction == Direction.None && player.input.prevDirection == Direction.Right)  // decelerate when not actively moving
                    {
                        if (player.acceleration > .00001)
                        {
                            player.acceleration *= .95f;
                        }
                        
                        speed *= .9f;
                        
                    }
                    else if (speed < 0 && player.input.direction == Direction.None && player.input.prevDirection == Direction.Left)  // decelerate when not actively moving
                    {
/*                        if (player.acceleration > .00001)
                        {*/
                            player.acceleration *= .95f;
                        //}

                        if (speed < 0 && speed < .0001)
                        {
                            speed *= .9f;
                        }
                    }


                    if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        Push(player, elapsedTime);
                    }

                    if (player.input.direction == Direction.None)
                    {
                        Move(player, player.input.prevDirection);
                        return;
                    }
                    Move(player, player.input.direction);
                    break;
                case State.Grinding:
                    UpdateGrind(player, platforms);

                    Move(player, player.jumpDirection);
                    break;
                case State.Popped:
                    if (player.input.rightMouseRelease)
                    {
                        player.state = State.Grounded;
                    }
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        Jump(player);
                    }
                    break;
                case State.Push:
                    Push(player, elapsedTime);
                    if (player.input.direction == Direction.None)
                    {
                        Move(player, player.input.prevDirection);
                    }
                    else
                        Move(player, player.input.direction);
                    break;
            }
        }


        public void UpdateJump(Player player, double elapsedTime, List<Platform> platforms, DustCloud dustCloud)
        {
            switch (player.state)
            {
                case State.Grounded:
                    if (Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        Game1.popSound.Play();
                        player.state = State.Popped;
                        return;
                    }
                    break;

                case State.Jumping:
                    CheckKickFlip(player, elapsedTime, platforms);   

                    player.rect.Y += jumpSpeed;
                    jumpSpeed += 1;

                    if (player.rect.Y >= player.startPosition + jumpHeight)
                    {
                        dustCloud.Draw(player.rect.X, player.rect.Y);
                        if (player.trickState == TrickState.Kickflip && player.trickframe < 7 && player.trickframe > 4)
                        {
                            player.wreckFrame = player.trickframe;
                            Game1.wreckSound.Play();
                            player.state = State.Wreck;
                            player.trickState = TrickState.None;
                            return;
                        }

                        Game1.landingSound.Play();
                        player.state = State.Grounded;
                        if (player.trickState == TrickState.Kickflip || player.state == State.Grinding)
                        {
                            MediaPlayer.Pause();
                            player.trickState = TrickState.None;
                            Game1.trickSound.Play();
                        }
                        return;
                    }
                    break;
            }
        }


        public void Initialize()
        {
            throw new NotImplementedException();
        }



        private void Move(Player player, Direction inputDirection)
        {


            if (player.state != State.Jumping && player.input.direction != Direction.None && player.acceleration + .4f < maxAcceleration)
            {
                player.acceleration += .4f;
            }


            switch (inputDirection)
            {
                case Direction.Up:
                    if (player.rect.Top <= 100)
                        return;

/*                    if (player.input.prevDirection == Direction.Right && speed > 0)
                    {
                        speed *= -1;
                    }*/
                    if ( speed > 0)
                    {
                        speed *= -1;
                    }

                    Decelerate(accelFactor);
                    player.rect.Y += (int)speed;
                    break;
                case Direction.Left:
                    if (player.physics.collide == inputDirection)
                    {
                        return;
                    }
                    if (player.rect.Left <= 0)
                        return;


/*                    if (player.input.prevDirection != Direction.Right && speed > 0)
                    {
                        speed *= -1;
                    }*/


                    Decelerate(accelFactor);
                    if (inputDirection != player.input.prevDirection && speed > 0)
                    {
                            player.rect.X -= (int)speed;
                    }
                    else
                    {
                        player.rect.X += (int)speed;
                    }
                    
                    player.jumpDirection = Direction.Left;
                    break;
                case Direction.Right:
                    if (player.rect.Right >= Game1.view.width)
                        return;
/*
                    if (player.input.prevDirection != Direction.Left && speed < 0)
                    {
                        speed *= -1;
                    }*/

                    CalcSpeed(player);

                    player.rect.X += (int)speed;
                    player.jumpDirection = Direction.Right;
                    break;

                case Direction.Down:
                    if (player.rect.Bottom >= Game1.view.height)
                        return;
                    if (player.physics.collide == inputDirection)
                    {
                        return;
                    }

                    if (player.input.prevDirection == Direction.Left && speed < 0)
                    {
                        speed *= -1;
                    }

                    CalcSpeed(player);

                    player.rect.Y += (int)speed; 
                    break;

                case Direction.UpLeft:
                    if (player.physics.collide == inputDirection)
                    {
                        Move(player, Direction.Up);
                       // return;
                    }

                    if (speed > 0)
                    {
                        player.rect.Y -= (int)(speed * .65f);
                        player.rect.X -= (int)(speed * .65f);
                    }
                    else
                    {
                        player.rect.Y += (int)(speed * .65f);
                        player.rect.X += (int)(speed * .65f);
                    }

                    break;
                case Direction.UpRight:
                    if (speed > 0)
                    {
                        player.rect.Y -= (int)(speed * .65f);
                        player.rect.X += (int)(speed * .65f);
                    }
                    else
                    {
                        player.rect.Y += (int)(speed * .65f);
                        player.rect.X -= (int)(speed * .65f);
                    }


                    break;
                case Direction.DownRight:
                    player.rect.Y += (int)(speed * .65f);
                    player.rect.X += (int)(speed * .65f);
                    break;
/*                    player.rect.Y += (int)(speed * .65f);
                    player.rect.X += (int)(speed * .65f);
                    break;*/
                case Direction.DownLeft:

                    player.rect.Y += (int)(speed * .65f); 
                    player.rect.X += (int)(speed * .65f);
                    break;
            }
            player.direction = inputDirection;
        }



        public double EaseOutQuad (double t, double b, double c, double d)
        {
            t /= d;
            return -c * t * (t - 2) + b;
        }


        private void Push(Player player, double elapsedTime)
        {
            player.acceleration = 8;
            if (player.state != State.Push)
            {
                player.state = State.Push;
                maxSpeed = 16;
               // player.acceleration = 6;
                player.rect.Width = (Utilities.Scale(36, 3.75));
                player.rect.Height = (Utilities.Scale(36, 3.75));
            }
            timer += elapsedTime;
            
           if (timer > pushInterval)
            {
                timer = 0;
                pushFrame++;
            }
           if (pushFrame > 3) // the end frame of the push
            {
                pushFrame = 0;
                timer = 0;
                player.state = State.Grounded;
                maxSpeed = 8;
                player.state = State.Grounded;
                player.rect.Width = (75);
                player.rect.Height = (120);

/*                if (maxSpeed > 8)
                {
                    maxSpeed *= .9f;
                    return;
                }
                else
                {

                }*/

            }



        }

        private void Jump(Player player)
        {
            if (player.state != State.Jumping)
            {
                player.state = State.Jumping;
                jumpSpeed = -16;
                player.acceleration = 0;
                player.startPosition = player.rect.Y - jumpHeight;
            }
        }

        public void Fall(Player player)
        {
            if (player.state != State.Jumping)
            {
                player.state = State.Jumping;
                jumpSpeed = -4;

                player.startPosition = player.rect.Y - jumpHeight;
            }
        }


        public void CheckGrind(Player player, List<Platform> platforms)
        {
            foreach (Platform p in platforms)
            {
                if (player.state != State.Grinding && p.type == PlatformType.Rail)
                {
                    if (player.rect.Right > p.rect.Left && player.rect.Left < p.rect.Right && player.rect.Y - p.rect.Top <= 100 && player.rect.Y - p.rect.Top >= -200)
                    {

                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            player.state = State.Grinding;
                            player.depthLayer = .1f;
                            MediaPlayer.Play(Game1.grindSong);
                            player.rect.Y = p.rect.Y - 100;
                        }
                        return;
                    }
                    else
                        p.intersects = false;
                }
            }
        }


        private void CheckKickFlip(Player player, double elapsedTime, List<Platform> platforms)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                player.trickState = TrickState.Kickflip;
            }
            if (player.trickState == TrickState.Kickflip)
            {
                player.timer += elapsedTime;

                if (player.timer > player.interval)
                {
                    player.trickframe++;

                    if (player.trickframe > 8)
                    {
                        player.trickframe = 3;
                    }
                    player.timer = 0f;
                }
            }
        }

        public void RampCheck(Player player, Rectangle ramp)
        {
            if (player.rect.Intersects(ramp))
            {
                player.boxcheck = true;
                if (player.direction != Direction.Left && player.rect.Right > ramp.Left + 100 && player.rect.Left < ramp.Right - 1)
                {
                    if (player.rect.Right > ramp.Right)
                    {
                        Move(player, Direction.Right);
                        return;
                    }
                    if (player.direction == Direction.Right)
                    {
                        player.acceleration += .25f;
                        // friction += .25f;
                        Move(player, Direction.DownRight);
                    }
                }
                if (player.direction == Direction.Left && (player.rect.Left + 5) < ramp.Right)
                {
                    //acceleration *= .85f;
                    if (friction < maxFriction)
                    {
                        friction *= 1.05f;
                    }
                    //Move(Direction.UpLeft);
                    Move(player, Direction.Up);
                    Move(player, Direction.Left);
                    Move(player, Direction.Left);
                }
                if (player.rect.Bottom - ramp.Top >= 100 && player.rect.Bottom - ramp.Top <= 110)
                {
                    collide = Direction.Down;
                }
                if ((player.rect.Top - ramp.Bottom) <= -80 && (player.rect.Top - ramp.Bottom) >= -90)
                {
                    collide = Direction.Up;
                }
            }
        }
    }
}
