using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace StarterGame
{
    public class Player : GameObject
    {
        public bool busted = false;
        public int initialX;
        public int initialY;
        public int wreckFrame;
        public int trickframe = 2; // 3rd frame is where kickflip animation starts
        public int idleFrame = 9;
        public PhysicsComponent physics;
        public InputComponent input;
        public bool boxcheck;
        public float acceleration = 0;
        public double interval = 100;
        public double timer = 0f;
        public Wreck wreck;
        public int coinCount;
        public int width = 20;
        public const int characterHeight = 31;
        public Direction direction;
        public float depthLayer = .1f;
        private State _state;
        public TrickState trickState;
        public int startPositionY;
        public Rectangle wreckSprite;

        public Direction jumpDirection;
        private AudioManager audioManager;
        public float height = 32;
        public float heightScaled = 32 * (float)3.75;

        public State state
        {
            get { return _state; }
            set { SetState(value); }
        }

        private void SetState(State value)
        {
/*          if(value == State.Wreck)
            {
                this.audioManager.PlayWreckSound();
            }*/
            _state = value;
        }

/*        private void SetState(State newState, AudioManager audioManager)
        {
            // Validation or additional logic can be added here
            // Use newState and additionalParameter as needed
            _state = newState;
        }*/


public Player(AudioManager audioManager, InputComponent inputComponent, int x, int y, int width, int height):base(x, y, width, height)
        {
            initialX = x;
            initialY = y;
            this.audioManager = audioManager;
            wreckFrame = 0;
            physics = new PhysicsComponent();
            input = inputComponent;
            coinCount = 0;
            trickState = TrickState.None;
            direction = Direction.Right;
            jumpDirection = Direction.Right;
            state = State.Grounded;
            
            wreck = new Wreck();
            //this.shadow = new Rectangle(0, 0, 60, 96);
          //  wreck = new Rectangle(player1.rect.X, player1.rect.Y, 120, 120);
        }





        public double Animate(double elapsedTime)
        {
            timer += elapsedTime;

            if (timer > 500)
            {
                idleFrame++;
                if (idleFrame > 11)
                {
                    idleFrame = 9;
                }
                timer = 0f;
            }
            return timer;
        }



        /*        private void BoxCheck(Platform box)
                {
                    if (rect.Left - box.rect.Right < 10 && (direction == Direction.Left || direction == Direction.DownLeft || direction == Direction.UpLeft))
                    {
                        collide = direction;
                        return;
                    }
                   // else
                      //  collide = Direction.None;
                    //boxcheck = false;
                   // return false;
                }*/

        public void HandlePosition(double elapsedTime, List<Platform> platforms, DustCloud dustCloud, AudioManager audioManager, HudManager hud)
        {
            input.Update(this);
            physics.Update(this, elapsedTime, platforms, dustCloud, audioManager, hud);

            if (state == State.Jumping || state == State.Grinding) { return; }

            foreach(Platform p in platforms)
            {
                if (rect.Intersects(p.rect))
                {
                    if (p.type == PlatformType.Ramp)
                    {
                        // CollissionCheck(coll.block);
                        physics.RampCheck(this, p.rect);                     
                    }
                    if (p.type == PlatformType.Box)
                    {
                        //BoxCheck(p);
                    }
                }
                physics.CollissionCheck(rect, p.rect);
            }
        }



/*        private void RampCheck(Rectangle ramp)
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
        }*/

/*        private void CollissionCheck(Rectangle block)
        {
            if (rect.Intersects(block))
            {
                if (rect.Bottom > block.Top && rect.Top < block.Bottom)
                {
                    if ((((rect.Right - physics.speed) - block.Left) <= bounds) && ((rect.Right - physics.speed) - block.Left) >= -bounds)
                    {
                        collide = Direction.Right;
                        return;
                    }
                    else if (((rect.Left - block.Right) <= bounds) && (rect.Left - block.Right) >= -bounds)
                    {
                        collide = Direction.Left;
                        return;
                    }
                }

                if ((rect.Bottom - block.Top >= 8 && rect.Bottom - block.Top <= 20) || rect.Bottom - block.Bottom > -200 && rect.Bottom - block.Bottom < 0)
                {
                    collide = Direction.Down;
                    return;
                }
                if ((rect.Top - block.Bottom) <= -60 && (rect.Top - block.Bottom) >= -60)
                {
                    collide = Direction.Up;
                    return;
                }
            }
            collide = Direction.None;
        }*/

        public void Reset()
        {
            physics.speed = 0;
            direction = Direction.Right;
            state = State.Grounded;
        }

        public void ResetHard()
        {
            physics.speed = 0;
            direction = Direction.Right;
            state = State.Grounded;
            rect.X = initialX;
            rect.Y = initialY;
            busted = false;
        }

        public Rectangle Sprite(double elapsedTime)
        {
            switch (trickState)
            {
                case TrickState.Kickflip:
                    if(direction == Direction.Right || direction == Direction.DownRight || direction == Direction.UpRight)
                    {
                        return new Rectangle(trickframe * 23, 62, width, characterHeight);
                    }
                    return new Rectangle(trickframe * 23, 158, width, characterHeight);
            }
            switch (state)
            {
                case State.Idle:
                    return new Rectangle(20 * idleFrame, 0, width, 33);
                case State.Grinding:
                    return new Rectangle(width * (int)jumpDirection, 0, width, characterHeight);
                case State.Push:
                    return new Rectangle(physics.pushFrame * 32, 126, 32, 32);
                case State.Wreck:
                   // this.width = 32;
                    return new Rectangle(wreck.frame * 32, 190, 32, 32);
                case State.Popped:
                    if (direction == Direction.Right || direction == Direction.DownRight || direction == Direction.UpRight || direction == Direction.None && input.prevDirection == Direction.Right)
                    {
                        return new Rectangle(21, 31, width, characterHeight);
                    }
                    return new Rectangle(21 *4, 31, width, characterHeight);
                case State.Jumping:
                    if (direction == Direction.Right || direction == Direction.DownRight || direction == Direction.UpRight || direction == Direction.None && input.prevDirection == Direction.Right)
                    {
                        return new Rectangle(0, 31, width, characterHeight);
                    }
                        return new Rectangle(21 * 3, 31, width, characterHeight);
            }

            if (input.direction == Direction.None)
            {
                return new Rectangle(width * (int)input.prevDirection, 0, width, characterHeight);
            }

            return new Rectangle(width * (int)direction, 0, width, characterHeight);
        }

    }
}
