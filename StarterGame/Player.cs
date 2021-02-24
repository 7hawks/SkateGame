using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace StarterGame
{
    public class Player
    {
        public int wreckFrame;

        public PhysicsComponent physics;
        public InputComponent input;
        public bool boxcheck;
        public float acceleration = 0;
        private bool toggle;
        public double interval = 100;
        public double timer = 0f;
        private double grindTimer = 0f;
        public int trickframe = 3;
        public Wreck wreck;
        public int coinCount;
        public float speed;
        private const int width = 20;
        public const int characterHeight = 31;
        public Direction direction;
        public float depthLayer = .1f;
        public int jumpspeed = 0;
        const int bounds = 3;
        public Rectangle player;
        //public Rectangle shadow;
        public Direction collide;
        public State state;
        public TrickState trickState;


        public int startPosition;

        public Direction jumpDirection;

        public Player(InputComponent inputComponent)
        {
            wreckFrame = 0;
            physics = new PhysicsComponent();
            input = inputComponent;
            toggle = false;
            coinCount = 0;
            trickState = TrickState.None;
            speed = 0;
            direction = Direction.Right;
            jumpDirection = Direction.Right;
            state = State.Grounded;
            
            wreck = new Wreck();
            //this.shadow = new Rectangle(0, 0, 60, 96);
            collide = Direction.None;
           
            player = new Rectangle(0, 200, 75, 120);
        }

        private void BoxCheck(Platform box)
        {
            if (player.Left - box.rect.Right < 10 && (direction == Direction.Left || direction == Direction.DownLeft || direction == Direction.UpLeft))
            {
                collide = direction;
                return;
            }
           // else
              //  collide = Direction.None;
            //boxcheck = false;
           // return false;
        }

        public void HandlePosition(double elapsedTime, SoundEffect sound, SoundEffect landingSound, SoundEffect trickSound, Song grindSong, List<Platform> platforms, SoundEffect wreckSound, DustCloud dustCloud)
        {
            input.Update(this);
            physics.Update(this, elapsedTime, sound, landingSound, trickSound, grindSong, platforms, wreckSound, dustCloud);

            if (state == State.Jumping) { return; }

            foreach(Platform p in platforms)
            {
                if (player.Intersects(p.rect))
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
                CollissionCheck(p.rect);
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

        private void CollissionCheck(Rectangle block)
        {
            if (player.Intersects(block))
            {
                if (player.Bottom > block.Top && player.Top < block.Bottom)
                {
                    if ((((player.Right - speed) - block.Left) <= bounds) && ((player.Right - speed) - block.Left) >= -bounds)
                    {
                        collide = Direction.Right;
                        return;
                    }
                    else if (((player.Left - block.Right) <= bounds) && (player.Left - block.Right) >= -bounds)
                    {
                        collide = Direction.Left;
                        return;
                    }
                }

                if ((player.Bottom - block.Top >= 8 && player.Bottom - block.Top <= 20) || player.Bottom - block.Bottom > -200 && player.Bottom - block.Bottom < 0)
                {
                    collide = Direction.Down;
                    return;
                }
                if ((player.Top - block.Bottom) <= -60 && (player.Top - block.Bottom) >= -60)
                {
                    collide = Direction.Up;
                    return;
                }
            }
            collide = Direction.None;
        }

        public void Reset()
        {
            speed = 0;
            direction = Direction.Right;
            state = State.Grounded;
        }

        public Rectangle Sprite(double elapsedTime)
        {

            switch (state)
            {
                case State.Push:
                    return new Rectangle(physics.pushFrame * 32, 126, 32, 32);
                case State.Wreck:
                    return new Rectangle(wreck.frame * 20, 95, width, characterHeight);
                case State.Popped:
                    return new Rectangle(21, 31, width, characterHeight);
                case State.Jumping:
                    return new Rectangle(0, 31, width, characterHeight);
            }

            switch (trickState)
            {
                case TrickState.Kickflip:
                    return new Rectangle(trickframe * 23, 62, width, characterHeight);
                case TrickState.Grinding:
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
                        return new Rectangle(width * (int)direction, 0, width, characterHeight);
                    }
                    return new Rectangle(width * (int)direction, 95, width, characterHeight);
            }


            if (input.direction == Direction.None)
            {
                return new Rectangle(width * (int)input.prevDirection, 0, width, characterHeight);
            }

            return new Rectangle(width * (int)direction, 0, width, characterHeight);
        }
    }
}
