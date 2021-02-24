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
        private const float maxAcceleration = 4;
        public  float maxSpeed = 8f;
        public float friction = 0.00029f;
        public const double maxFriction = 1;
        public int jumpHeight { get; set; }
        private bool decelerate;
        public int pushFrame;
        private const int pushInterval = 100;
        private double timer;

        public PhysicsComponent()
        {
            jumpHeight = 0;
            pushFrame = 0;
            timer = 0;
        }

        public void Update(Player player, double elapsedTime, SoundEffect sound, SoundEffect landingSound, SoundEffect trickSound, Song grindSong, List<Platform> platforms, SoundEffect wreckSound, DustCloud dustCloud)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Space) || player.state == State.Push)
            {
                Push(player, elapsedTime);
            }

            UpdateJump(player, elapsedTime, sound, landingSound, trickSound, grindSong, platforms, wreckSound, dustCloud);

            if (player.state != State.Jumping && player.speed > 0 && player.input.direction == Direction.None)  // decelerate when not actively moving
            {
/*                if (player.acceleration < .01)
                {
                    player.acceleration = 0;
                    return;
                }
                if (player.speed < .01)
                {
                    player.speed = 0;
                    return;
                }*/

                if (player.acceleration > 0 && player.acceleration > .00001)
                {
                    player.acceleration *= .95f;
                }
                
                if (player.speed > 0 && player.speed > .0001)
                {
                    player.speed *= .9f;
                }

            }
            if (player.input.direction == Direction.None && player.acceleration > .01) 
            {
                Move(player, player.input.prevDirection);
                return;
            }
            else
                Move(player, player.input.direction);
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
            if (inputDirection != player.direction)
            {
                if (inputDirection == Direction.Left && player.direction == Direction.Right && player.speed > 0)
                {
                    player.speed -= .2f;

                    player.player.X += (int)player.speed;
                    player.player.X += (int)player.speed;
                    return;
                }
                else if (inputDirection == Direction.Right && player.direction == Direction.Left && player.speed > 0)
                {

                    Decelerate();

                    player.speed -= .2f;

                    player.player.X -= (int)player.speed;
                    player.player.X -= (int)player.speed;
                    return;
                }
                player.direction = inputDirection;
            }

            CalcSpeed(player);

            switch (inputDirection)
            {
                case Direction.Up:
                    if (player.player.Top <= 100)
                        return;

                    player.player.Y -= (int)player.speed;
                    break;

                case Direction.Right:
                    if (player.player.Right >= 1200)
                        return;

                    player.player.X += (int)player.speed;
                    player.jumpDirection = Direction.Right;
                    break;

                case Direction.Down:
                    if (player.player.Bottom >= 720)
                        return;
                    if (player.collide == inputDirection)
                    {
                        return;
                    }

                    player.player.Y += (int)player.speed; ;
                    break;
                case Direction.Left:
                    if (player.collide == inputDirection)
                    {
                        return;
                    }

                    if (player.player.Left <= 0)
                        return;

                    player.player.X -= (int)player.speed;
                    player.jumpDirection = Direction.Left;
                    break;
                case Direction.UpLeft:
                    if (player.collide == inputDirection)
                    {
                        Move(player, Direction.Up);
                        return;
                    }
                    player.player.Y -= (int)(player.speed * .65f);
                    player.player.X -= (int)(player.speed * .65f);
                  //  Move(player, Direction.Up);
                   // Move(player, Direction.Left);
                    break;
                case Direction.UpRight:
                    player.player.Y -= (int)(player.speed * .65f);
                    player.player.X += (int)(player.speed * .65f);
                    //Move(player, Direction.Up);
                   // Move(player, Direction.Right);
                    break;
                case Direction.DownRight:
                    player.player.Y += (int)(player.speed * .65f);
                    player.player.X += (int)(player.speed * .65f);
                   // Move(player, Direction.Down);
                   // Move(player, Direction.Right);
                    break;
                case Direction.DownLeft:

                    player.player.Y += (int)(player.speed * .65f); 
                    player.player.X -= (int)(player.speed * .65f);
                    // Move(player, Direction.Down);
                    // Move(player, Direction.Left);
                    break;
            }
            player.direction = inputDirection;
        }

        private void CalcSpeed(Player player)
        {
            if (player.speed + player.acceleration > maxSpeed)
            {
                player.speed = maxSpeed;
                return;
            }
            else
                player.speed += player.acceleration;
        }

        public double EaseOutQuad (double t, double b, double c, double d)
        {
            t /= d;
            return -c * t * (t - 2) + b;
        }

        public void UpdateJump(Player player, double elapsedTime, SoundEffect sound, SoundEffect landingSound, SoundEffect trickSound, Song grindSong, List<Platform> platforms, SoundEffect wreckSound, DustCloud dustCloud)
        {
            player.state = player.wreck.WreckCheck(player.state, elapsedTime, player);

            foreach (Platform p in platforms)
            {
                if (p.type == PlatformType.Rail && player.trickState == TrickState.Grinding && player.player.Intersects(p.rect))
                {
                    player.player.Y = p.rect.Y - 150 + (player.player.X / 2);
                    Move(player, player.jumpDirection);
                    return;
                }
                if (player.state == State.Grounded)
                {
                    if (player.direction == Direction.Right && player.player.Right > p.rect.Right && jumpHeight == 0)
                    {
                        Fall(player);
                    }
                }
            }

            if (player.state == State.Grounded)
            {
                if (Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    sound.Play();
                    player.state = State.Popped;
                    return;
                }
            }
            if (player.state == State.Popped)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    Jump(player);
                }
            }

            if (player.state == State.Jumping)
            {
                CheckTrick(player, elapsedTime, platforms, grindSong);      // need for grinding

                player.player.Y += player.jumpspeed;
                player.jumpspeed += 1;

                if (player.player.Y >= player.startPosition + jumpHeight)
                {
                    dustCloud.Draw(player.player.X, player.player.Y);
                    if (player.trickState == TrickState.Kickflip && player.trickframe < 7 && player.trickframe > 4)
                    {
                        player.wreckFrame = player.trickframe;
                        wreckSound.Play();
                        player.state = State.Wreck;
                        player.trickState = TrickState.None;
                        return;
                    }

                    landingSound.Play();
                    player.state = State.Grounded;
                    if (player.trickState == TrickState.Kickflip || player.trickState == TrickState.Grinding)
                    {
                        Microsoft.Xna.Framework.Media.MediaPlayer.Pause();
                        player.trickState = TrickState.None;
                        trickSound.Play();
                    }
                    return;
                }
            }
        }

        private void Push(Player player, double elapsedTime)
        {
            player.acceleration = 8;
            if (player.state != State.Push)
            {
                player.state = State.Push;
                maxSpeed = 16;
               // player.acceleration = 6;
                player.player.Width = (Utilities.Scale(36, 3.75));
                player.player.Height = (Utilities.Scale(36, 3.75));
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
                player.player.Width = (75);
                player.player.Height = (120);
            }
        }

        private void Jump(Player player)
        {
            if (player.state != State.Jumping)
            {
                player.state = State.Jumping;
                player.jumpspeed = -16;
                player.acceleration = 0;
                player.startPosition = player.player.Y - jumpHeight;
            }
        }

        public void Fall(Player player)
        {
            if (player.state != State.Jumping)
            {
                player.state = State.Jumping;
                player.jumpspeed = -4;

                player.startPosition = player.player.Y - jumpHeight;
            }
        }

        private void CheckTrick(Player player, double elapsedTime, List<Platform> platforms, Song grindSong)
        {
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
                return;
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                player.trickState = TrickState.Kickflip;
            }

            foreach (Platform p in platforms)
            {
                if (p.type == PlatformType.Rail)
                {
                    if (player.player.Right > p.rect.Left && Mouse.GetState().LeftButton == ButtonState.Pressed && player.player.X - p.rect.Top < 60 && player.player.X - p.rect.Top > -10)
                    {
                        player.trickState = TrickState.Grinding;
                        player.depthLayer = .1f;
                        MediaPlayer.Play(grindSong);
                        player.player.Y = p.rect.Y - 100;
                    }
                }
            }

        }

        public void RampCheck(Player player, Rectangle ramp)
        {
            if (player.player.Intersects(ramp))
            {
                player.boxcheck = true;
                if (player.direction != Direction.Left && player.player.Right > ramp.Left + 100 && player.player.Left < ramp.Right - 1)
                {
                    if (player.player.Right > ramp.Right)
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
                if (player.direction == Direction.Left && (player.player.Left + 5) < ramp.Right)
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
                if (player.player.Bottom - ramp.Top >= 100 && player.player.Bottom - ramp.Top <= 110)
                {
                    player.collide = Direction.Down;
                }
                if ((player.player.Top - ramp.Bottom) <= -80 && (player.player.Top - ramp.Bottom) >= -90)
                {
                    player.collide = Direction.Up;
                }
            }
        }
    }
}
