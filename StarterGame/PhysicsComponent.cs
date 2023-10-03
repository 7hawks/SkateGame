using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace StarterGame
{
    
    public class PhysicsComponent : IGameComponent
    {
        private bool slow;
        const int bounds = 3;
        public int jumpSpeed;
        public float speed;
        private const float maxAcceleration = 4;
        public  float maxSpeed = 9f;
        public float friction = 0.00029f;
        public const double maxFriction = 1;
        public int jumpHeight { get; set; }
        public int pushFrame;
        private const int pushInterval = 100;
        private double timer;
        public Direction collide;
        public Direction slowDownDirection;
        private double slowDown;
        public bool checkViewBox;

        public PhysicsComponent()
        {
            slowDownDirection = Direction.None;
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
        public void CollissionCheck(Direction direction, Rectangle playerRect, Rectangle block)
        {
            if (playerRect.Left < block.Right && playerRect.Right > block.Left && direction == Direction.Up && (playerRect.Top - block.Bottom) <= -60 && (playerRect.Top - block.Bottom) >= -75)
            {
                collide = Direction.Up;
                return;
            }

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

                if (direction == Direction.UpLeft && (playerRect.Top - block.Bottom) <= -40 && (playerRect.Top - block.Bottom) >= -80)
                {
                    collide = Direction.UpLeft;
                    return;
                }

                if (direction == Direction.UpRight && (playerRect.Top - block.Bottom) <= -40 && (playerRect.Top - block.Bottom) >= -80)
                {
                    collide = Direction.UpRight;
                    return;
                }
            }
            collide = Direction.None;
        }

        public void Decelerate(Player player)
        {
            if (player.acceleration > 0 && player.acceleration > .00001) { player.acceleration *= .95f; }
            if (speed > 0 && speed > .0001) { speed *= .9f; }
        }

        public void UpdateGrind(Player player, List<Platform> platforms)
        {
            if (player.state != State.Grinding)
                return;

            if (player.rect.Y >= player.startPosition + jumpHeight)
            {
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
            jumpHeight = 0;
        }

        public void Update(Player player, double elapsedTime, List<Platform> platforms, DustCloud dustCloud)
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
                case State.Jumping:
                    UpdateJump(player, elapsedTime, platforms, dustCloud);
                    CheckGrind(player, platforms);
                    if (player.input.direction == Direction.Up)   // prevents being able to super jump by pressing UP while jumping
                        Move(player, player.jumpDirection);
                    else
                        Move(player, player.jumpDirection); 
                    break;
                case State.Grounded:

                    if(player.direction == Direction.None && player.input.direction == Direction.None && speed < .5)
                    {
                        speed = 0;
                        return;
                    }

                    if (slow)
                    {
                        SlowDown(player, elapsedTime);
                    }

                    UpdateJump(player, elapsedTime, platforms, dustCloud);
                    // this block needs work in order for the LEDGE DROP to work

/*                    foreach ( Platform p in platforms) // check if player is going from one level platform to another
                    {
                        if (player.rect.Intersects(p.rect))
                        {
                            if (player.direction == Direction.Right && player.rect.Right > p.rect.Right && jumpHeight == 0) { Fall(player); }
                        } 
                    }*/

                    if (speed > 0 && player.input.direction == Direction.None)  // decelerate when not actively moving
                    {
                        Decelerate(player);
/*                        if (player.acceleration > 0 && player.acceleration > .00001) { player.acceleration *= .95f; }
                        if (speed > 0 && speed > .0001) { speed *= .9f; }*/
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
                    UpdateGrind(player, platforms);
                    UpdateJump(player, elapsedTime, platforms, dustCloud);

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
                case State.Wreck:
                    Move(player, Direction.None);
                    break;
            }
        }

        public void UpdateJump(Player player, double elapsedTime, List<Platform> platforms, DustCloud dustCloud)
        {
            
            if ((player.state == State.Grounded || player.state == State.Grinding) && (Mouse.GetState().RightButton == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.K)))
            {
                Game1.popSound.Play();
                player.state = State.Popped;
                return;
            }

            switch (player.state)
            {
                case State.Jumping:
                    Decelerate(player);
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

        //public double EaseOutQuad (double t, double b, double c, double d)
        //{
        //    t /= d;
        //    return -c * t * (t - 2) + b;
        //}

        private void Move(Player player, Direction inputDirection)
        {
            if (player.physics.collide == inputDirection)
                return;

            if (player.state != State.Jumping && player.input.direction != Direction.None && player.acceleration + .4f < maxAcceleration)
            {
                player.acceleration += .4f;
            }
            // This handles a change of direction and resistance
            if (inputDirection != player.direction && speed > 2)
            {
                if (inputDirection == Direction.Left && player.direction == Direction.Right)
                {
                    player.acceleration = 0;
                    speed -= .2f;

                    player.rect.X += (int)speed;

                    return;
                }
                else if (inputDirection == Direction.Right && player.direction == Direction.Left)
                {
                    player.acceleration = 0;
                    speed -= .2f;

                    player.rect.X -= (int)speed;

                    return;
                }
                player.direction = inputDirection;
            }

            if (CheckViewBox(player, inputDirection)) // check if player has reached the bounds of screen
                return;

            CalcSpeed(player);
                
            switch (inputDirection)
            {
                case Direction.Up:
                    player.rect.Y -= (int)speed;
                    break;

                case Direction.Right:
                    player.rect.X += (int)speed;
                    player.jumpDirection = Direction.Right;
                    break;

                case Direction.Down:
                    player.rect.Y += (int)speed; ;
                    break;
                case Direction.Left:
                    player.rect.X -= (int)speed;
                    player.jumpDirection = Direction.Left;
                    break;
                case Direction.UpLeft:
                    player.rect.Y -= (int)(speed * .7f);
                    player.rect.X -= (int)(speed * .7f);
                    player.jumpDirection = Direction.UpLeft;
                    break;
                case Direction.UpRight:
                    player.rect.Y -= (int)(speed * .7f);
                    player.rect.X += (int)(speed * .7f);
                    player.jumpDirection = Direction.UpRight;
                    break;
                case Direction.DownRight:
                    player.rect.Y += (int)(speed * .7f);
                    player.rect.X += (int)(speed * .7f);
                    player.jumpDirection = Direction.DownRight;
                    break;
                case Direction.DownLeft:
                    player.rect.Y += (int)(speed * .7f);
                    player.rect.X -= (int)(speed * .7f);
                    player.jumpDirection = Direction.DownLeft;
                    break;
            }
            player.direction = inputDirection;
        }

        // This method checks if the player has reached the bounds of the game view
        private bool CheckViewBox(Player player, Direction inputDirection)
        {
            if (inputDirection == Direction.Right || inputDirection == Direction.UpRight || inputDirection == Direction.DownRight)
            {
                if (player.rect.Right >= Game1.view.width)
                    return true;
            }

            if (inputDirection == Direction.Left || inputDirection == Direction.UpLeft || inputDirection == Direction.DownLeft)
            {
                if (player.rect.Left <= 0)
                    return true;
            }

            if (inputDirection == Direction.Down || inputDirection == Direction.DownLeft || inputDirection == Direction.DownRight)
            {
                if (player.rect.Bottom >= Game1.view.height)
                    return true;
            }

            if (inputDirection == Direction.Up || inputDirection == Direction.UpLeft || inputDirection == Direction.UpRight)
            {
                if (player.rect.Top <= 200) 
                    return true;
            }
            return false;
        }

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
                player.state = State.Grounded;
                slow = true;

                player.rect.Width = (75);
                player.rect.Height = (120);
            }
        }

        private void SlowDown(Player player, double elapsedTime)
        {
            slowDown += elapsedTime;

            if (slowDown > 400)
            {
                slowDown = 0;
                maxSpeed = 8;
                slow = false;
            }
        }

        private void Jump(Player player)
        {
            if (player.state != State.Jumping)
            {
                player.state = State.Jumping;
                jumpSpeed = -16;
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

        public void RampCheck(Player player, Rectangle ramp, PlatformType type)
        {
            if (player.rect.Intersects(ramp))
            {
                switch (type)
                {
                    case PlatformType.RampRight:
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
                                Move(player, Direction.DownRight);
                            }
                        }
                        if (player.direction == Direction.Left && (player.rect.Left + 5) < ramp.Right)
                        {
                            Decelerate(player);
                            if (friction < maxFriction)
                            {
                                friction *= 1.05f;
                            }
                            Move(player, Direction.Up);
                            Move(player, Direction.Left);
                            // Move(player, Direction.Left);
                        }

                        break;
                    case PlatformType.RampLeft:
                        break;
                }
            }
        }
    }
}
