using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace StarterGame
{
    
    public class PhysicsComponent : IGameComponent
    {
        const int bounds = 3;
        public int speedStrength = -18;
        public float speed;
        private const float maxAcceleration = 4;
        public  float maxSpeed = 9f;
        public float friction = 0.00029f;
        private float gravity = 0.8f;
        public const double maxFriction = 1;
        public int jumpHeight { get; set; }
        public int pushFrame;
        private const int pushInterval = 100;
        public double timer;
        public Direction collide;
        public Direction slowDownDirection;
        public int frame = 0;
        public Vector2 playerVelocity;
        public float groundY = 0f;


        public PhysicsComponent()
        {
          //  audio = new AudioManager();
            slowDownDirection = Direction.None;
            collide = Direction.None;
            jumpHeight = 0;
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

        public void UpdateGrind(Player player, List<Platform> platforms, AudioManager audio)
        {
            if (player.state != State.Grinding)
                return;
       
            if (player.rect.Y >= groundY)
            {
                /*                foreach (Platform p in platforms)
                                {
                                    p.intersects = false;
                                }*/
                audio.PlayLandingSound();
                player.state = State.Grounded;
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

        public void Update(Player player, double elapsedTime, List<Platform> platforms, DustCloud dustCloud, AudioManager audio, HudManager hud)
        {
            foreach (Platform p in platforms)
            {
                if (p.type == PlatformType.Rail) // this bool is used for the RAIL color to know when the rail is grindable
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
                case State.Idle:
                    if (player.input.direction != Direction.None)
                    {
                        player.state = State.Grounded;
                        timer = 0f;
                        return;
                    }
                    timer = player.Animate(elapsedTime);
                    UpdateJump(player, elapsedTime, platforms, dustCloud, audio, hud);
                    return;
                case State.Jumping:
                    UpdateJump(player, elapsedTime, platforms, dustCloud, audio, hud);
                    CheckGrind(player, platforms, audio);
                    if (player.input.direction == Direction.Up)   // prevents being able to super jump by pressing UP while jumping
                        Move(player, player.jumpDirection);
                    else
                        Move(player, player.jumpDirection); 
                    break;
                case State.Grounded:
                    if (timer > 1000 && player.direction == Direction.None)
                    {
                        player.state = State.Idle;
                        return;
                    }

                    player.physics.timer += elapsedTime;
                    if(player.direction == Direction.None && player.input.direction == Direction.None && speed < .5)
                    {
                        speed = 0;
                        return;
                    }

                    UpdateJump(player, elapsedTime, platforms, dustCloud, audio, hud);

                    
                    foreach ( Platform p in platforms)
                    {
                        if (player.direction == Direction.Right && player.rect.Right > p.rect.Right && jumpHeight == 0) { Fall(player); }
                    }

                    if (speed > 0 && player.input.direction == Direction.None)  // decelerate when not actively moving
                    {
                        if (player.acceleration > 0 && player.acceleration > .00001) { player.acceleration *= .95f; }

                        if (speed > 0 && speed > .0001) { speed *= .9f; }
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.Space)) { Push(player, elapsedTime); }


                    if (player.input.direction == Direction.None && player.acceleration > .01)
                    {
                        Move(player, player.input.prevDirection);
                        return;
                    }
                    Move(player, player.input.direction);
                    break;
                case State.Grinding:
                    UpdateGrind(player, platforms, audio);

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
                    if (player.input.direction == Direction.None) { Move(player, player.input.prevDirection); }
                    else
                        Move(player, player.input.direction);
                    break;
                case State.SlowDown:

                    break;
            }
        }



        private void Jump(Player player)
        {
            if (player.state != State.Jumping)
            {
                player.state = State.Jumping;
                groundY = player.rect.Y;
                playerVelocity.Y = speedStrength;
              //  player.acceleration = 0;
              
            }
        }

        private void UpdateJump(Player player, double elapsedTime, List<Platform> platforms, DustCloud dustCloud, AudioManager audio, HudManager hud)
        {
            if ((player.state == State.Grounded || player.state == State.Grinding || player.state == State.Idle) && (Mouse.GetState().RightButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.K)))
            {

                audio.PlayPopSound();


                player.state = State.Popped;
                return;
            }

            switch (player.state)
            {
                case State.Jumping:
                    CheckKickFlip(player, elapsedTime, platforms);

                    playerVelocity.Y += gravity;
                    player.rect.Y += (int)playerVelocity.Y;

                    if (player.rect.Y >= groundY)
                    {
                        dustCloud.Draw(player.rect.X, player.rect.Y);
                        if (player.trickState == TrickState.Kickflip && player.trickframe < 7 && player.trickframe > 4)
                        {
                            player.wreckFrame = player.trickframe;
                            audio.PlayWreckSound();
                            player.state = State.Wreck;
                            player.trickState = TrickState.None;
                            return;
                        }
                        audio.PlayLandingSound();
                        
                        player.rect.Y = (int)groundY; // Snap player to ground
                        playerVelocity.Y = 0;
                        player.state = State.Grounded;
                        if (player.trickState == TrickState.Kickflip || player.state == State.Grinding)
                        {
                          //  MediaPlayer.Pause();
                            player.trickState = TrickState.None;
                            Game1.trickSound.Play();
                            hud.SetState(true);
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

        private void Move(Player player, Direction inputDirection)
        {
            if (player.state != State.Jumping && player.input.direction != Direction.None && player.acceleration + .4f < maxAcceleration)
            {
                player.acceleration += .4f;
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
                    if (player.rect.Right >= Game1.worldWidth)
                        return;

                    player.rect.X += (int)speed;
                    player.jumpDirection = Direction.Right;
                    break;

                case Direction.Down:
                 //   if (player.rect.Bottom >= Game1.view.height)
                 //       return;
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

                    player.rect.X -= (int)speed;
                    player.jumpDirection = Direction.Left;
                    break;
                case Direction.UpLeft:
                    if (player.rect.Top <= 100)
                        return;
                    player.jumpDirection = Direction.Left;
                    if (player.physics.collide == inputDirection)
                    {
                       // Move(player, Direction.Up);
                        return;
                    }
                    player.rect.Y -= (int)(speed * .7f);
                    player.rect.X -= (int)(speed * .7f);
                    break;
                case Direction.UpRight:
                    if (player.rect.Top <= 100)
                        return;
                    player.jumpDirection = Direction.Right;
                    player.rect.Y -= (int)(speed * .7f);
                    player.rect.X += (int)(speed * .7f);
                    break;
                case Direction.DownRight:
                    player.jumpDirection = Direction.Right;
                    player.rect.Y += (int)(speed * .7f);
                    player.rect.X += (int)(speed * .7f);
                    break;
                case Direction.DownLeft:
                    player.jumpDirection = Direction.Left;
                    player.rect.Y += (int)(speed * .7f);
                    player.rect.X -= (int)(speed * .7f);
                    break;
            }
            player.direction = inputDirection;
        }

        // updates pushFrame for animation
        private void Push(Player player, double elapsedTime)
        {
            player.acceleration = 8;
            if (player.state != State.Push)
            {
                player.state = State.Push;
                maxSpeed = 16;
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
                maxSpeed = 8;
                player.state = State.Grounded;
                player.rect.Width = (75);
                player.rect.Height = (120);

            }
        }


        public void Fall(Player player)
        {
            if (player.state != State.Jumping)
            {
                player.state = State.Jumping;
                speedStrength = -4;

                player.startPositionY = player.rect.Y - jumpHeight;
            }
        }


        public void CheckGrind(Player player, List<Platform> platforms, AudioManager audioManager)
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
                            audioManager.PlayGrindSound();
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
                        player.trickframe = 2;
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
