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
        const int bounds = 3;
        public int jumpSpeed;
        public float speed;
        private const float maxAcceleration = 4;
        public  float maxSpeed = 8f;
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

        public void UpdateGrind(Player player, List<Platform> platforms)
        {
            if (player.state != State.Grinding)
                return;

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

            /*            switch (player.state)
                        {
                            case State.Jumping:
                                UpdateJump(player, elapsedTime, platforms, dustCloud);
                                break;
                            case State.Grounded:
                                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                                {
                                    Push(player, elapsedTime);
                                }
                                break;
                            case State.Grinding:
                                UpdateGrind(player, platforms);
                                break;
                            case State.Push:
                                Push(player, elapsedTime);
                                break;


                        }*/


            if (Keyboard.GetState().IsKeyDown(Keys.Space) || player.state == State.Push)
            {
                Push(player, elapsedTime);
            }

            if (player.state == State.Grinding)
            {
                UpdateGrind(player, platforms);
            }

            UpdateJump(player, elapsedTime, platforms, dustCloud);

         

            if (player.state == State.Grounded && speed > 0 && player.input.direction == Direction.None)  // decelerate when not actively moving
            {

                if (player.acceleration > 0 && player.acceleration > .00001)
                {
                    player.acceleration *= .95f;
                }
                
                if (speed > 0 && speed > .0001)
                {
                    speed *= .9f;
                }
            }

            if (player.input.direction == Direction.None && player.acceleration > .01) 
            {
                Move(player, player.input.prevDirection);
                return;
            }
            else if (player.state == State.Grinding)   // restrict movement to left and right while grinding
            {
                Move(player, player.jumpDirection);
            }
            else if (player.state == State.Jumping && player.input.direction == Direction.Up)   // prevents being able to super jump by pressing UP while jumping
                Move(player, player.jumpDirection);
            else
                Move(player, player.input.direction);
        }


        public void UpdateJump(Player player, double elapsedTime, List<Platform> platforms, DustCloud dustCloud)
        {
            player.state = player.wreck.WreckCheck(player.state, elapsedTime, player);

            /*            if (player.trickState == TrickState.Grinding)
                        {
                            Move(player, player.jumpDirection);
                            return;
                        }*/

            foreach (Platform p in platforms)
            {
                if (p.type == PlatformType.Rail && player.state == State.Grinding && player.rect.Intersects(p.rect))
                {
                    // player.rect.Y = p.rect.Y - 150 + (player.rect.X / 2);    // angle rail logic
                    //wwww player.rect.Y = p.rect.Y - 150;
                    Move(player, player.jumpDirection);
                    return;
                }
                if (player.state == State.Grounded)
                {
                    if (player.direction == Direction.Right && player.rect.Right > p.rect.Right && jumpHeight == 0)
                    {
                        Fall(player);
                    }
                }
            }

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
                case State.Popped:
                    if (Mouse.GetState().LeftButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Pressed)
                    {
                        Jump(player);
                    }
                    break;
                case State.Grinding:
                    if (player.rect.Y >= player.startPosition + jumpHeight)
                    {
                        foreach (Platform p in platforms)
                        {
                            p.intersects = false;
                        }
                        Game1.landingSound.Play();
                        player.state = State.Grounded;
                        MediaPlayer.Pause();
                        player.trickState = TrickState.None;
                        Game1.trickSound.Play();
                    }
                    return;

                case State.Jumping:
                    CheckTrick(player, elapsedTime, platforms);      // need for grinding

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

            /*            if (player.state == State.Grinding)
                        {
                            if (player.rect.Y >= player.startPosition + jumpHeight)
                            {
                                Game1.landingSound.Play();
                                player.state = State.Grounded;
                                MediaPlayer.Pause();
                                player.trickState = TrickState.None;
                                Game1.trickSound.Play();

                            }
                            return;
                        }*/

            /*            if (player.state == State.Jumping )
                        {
                            CheckTrick(player, elapsedTime, platforms);      // need for grinding

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
                        }*/
        }


        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Decelerate()
        {
            if (!decelerate)
            {
                decelerate = true;
            }
        }

        private void Move(Player player, Direction inputDirection)
        {
            if (player.state == State.Wreck)
                return;

            if (player.state == State.Popped && player.input.rightMouseRelease)
            {
                player.state = State.Grounded;
            }

            if (player.state != State.Jumping && player.input.direction != Direction.None && player.acceleration + .4f < maxAcceleration)
            {
                player.acceleration += .4f;
            }


            /*            if (collide == inputDirection)
                        {
                            collide = Direction.None;
                            return;
                        }*/
            if (inputDirection != player.direction && speed > 2)
            {
                if (inputDirection == Direction.Left && player.direction == Direction.Right)
                {
                    speed -= .2f;

                    player.rect.X += (int)speed;
                    player.rect.X += (int)speed;

                    return;
                }
                else if (inputDirection == Direction.Right && player.direction == Direction.Left)
                {

                    Decelerate();

                    speed -= .2f;

                    player.rect.X -= (int)speed;
                    player.rect.X -= (int)speed;

                    return;
                }
                player.direction = inputDirection;
            }

            CalcSpeed(player);

            switch (inputDirection)
            {
                case Direction.Up:
                    if (player.rect.Top <= 100)
                        return;

                    player.rect.Y -= (int)speed;
                    break;

                case Direction.Right:
                    if (player.rect.Right >= Game1.view.width)
                        return;

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

                    player.rect.Y += (int)speed; ;
                    break;
                case Direction.Left:
                    if (player.physics.collide == inputDirection)
                    {
                        return;
                    }

                    if (player.rect.Left <= 0)
                        return;

                    player.rect.X -= (int)speed;
                    player.jumpDirection = Direction.Left;
                    break;
                case Direction.UpLeft:
                    if (player.physics.collide == inputDirection)
                    {
                        Move(player, Direction.Up);
                        return;
                    }
                    player.rect.Y -= (int)(speed * .65f);
                    player.rect.X -= (int)(speed * .65f);
                  //  Move(player, Direction.Up);
                   // Move(player, Direction.Left);
                    break;
                case Direction.UpRight:
                    player.rect.Y -= (int)(speed * .65f);
                    player.rect.X += (int)(speed * .65f);
                    //Move(player, Direction.Up);
                   // Move(player, Direction.Right);
                    break;
                case Direction.DownRight:
                    player.rect.Y += (int)(speed * .65f);
                    player.rect.X += (int)(speed * .65f);
                   // Move(player, Direction.Down);
                   // Move(player, Direction.Right);
                    break;
                case Direction.DownLeft:

                    player.rect.Y += (int)(speed * .65f); 
                    player.rect.X -= (int)(speed * .65f);
                    // Move(player, Direction.Down);
                    // Move(player, Direction.Left);
                    break;
            }
            player.direction = inputDirection;
        }

        private void CalcSpeed(Player player)
        {
            if (speed + player.acceleration > maxSpeed)
            {
                speed = maxSpeed;
                return;
            }
            else
                speed += player.acceleration;
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
                player.rect.Width = (75);
                player.rect.Height = (120);
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

        private void CheckTrick(Player player, double elapsedTime, List<Platform> platforms)
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


            foreach (Platform p in platforms)    // grinding logic
            {
                if (player.state != State.Grinding && p.type == PlatformType.Rail)
                {
                   // if (player.rect.Right > p.rect.Left + 50 && player.rect.Left < p.rect.Right - 50 && player.rect.Y - p.rect.Top <= 100 && player.rect.Y - p.rect.Top >= -10)
                   if (player.rect.Right > p.rect.Left)
                    {
                        p.intersects = true;
                        if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                        {
                            player.state = State.Grinding;
                            player.depthLayer = .1f;
                            MediaPlayer.Play(Game1.grindSong);
                            player.rect.Y = p.rect.Y - 100;
                            p.intersects = false;
                        }
                        return;
                    }
                    else
                        p.intersects = false;
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
