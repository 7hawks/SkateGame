using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace StarterGame
{
    public class Player : GameObject
    {
        public int points;
        public int wreckFrame;
        public int trickframe = 3; // 3rd frame is where kickflip animation starts
        public PhysicsComponent physics;
        public InputComponent input;
        public float acceleration = 0;
        public double interval = 100;
        public double timer = 0f;
        public Wreck wreck;
        public int coinCount;
        private const int width = 20;
        public const int characterHeight = 31;
        public Direction direction;
        public float depthLayer = .1f;
        //public Rectangle shadow;
        public State state;
        public TrickState trickState;
        public int startPosition;

        public Direction jumpDirection;

        public Player(InputComponent inputComponent, int x, int y, int width, int height):base(x, y, width, height)
        {
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

        public void Update(double elapsedTime, List<Platform> platforms, DustCloud dustCloud)
        {
            input.Update(this);
            physics.Update(this, elapsedTime, platforms, dustCloud);

            if (state == State.Jumping || state == State.Grinding) { return; }

            foreach(Platform p in platforms)
            {
               // if (rect.Intersects(p.rect))
               // {
                    if (p.type == PlatformType.RampRight || p.type == PlatformType.RampLeft)
                    {
                        // CollissionCheck(coll.block);
                        physics.RampCheck(this, p.rect, p.type);
                    }
                    else
                        physics.CollissionCheck(direction, rect, p.rect);
            }
        }

        /// <summary>
        /// Reset the player position to start
        /// </summary>
        public void Reset()
        {
            physics.speed = 0;
            direction = Direction.Right;
            state = State.Grounded;
        }

        public Rectangle Sprite(double elapsedTime)
        {
            switch (trickState)
            {
                case TrickState.Kickflip:
                    return new Rectangle(trickframe * 23, 62, width, characterHeight);
            }
            switch (state)
            {
                case State.Grinding:
                    return new Rectangle(width * (int)jumpDirection, 0, width, characterHeight);
                case State.Push:
                    return new Rectangle(physics.pushFrame * 32, 126, 32, 32);
                case State.Wreck:
                    return new Rectangle(wreck.frame * 20, 95, width, characterHeight);
                case State.Popped:
                    return new Rectangle(21, 31, width, characterHeight);
                case State.Jumping:
                    if (jumpDirection == Direction.Right)
                        return new Rectangle(0, 31, width, characterHeight);
                    else
                        return new Rectangle(41, 31, width, characterHeight);
            }

            if (input.direction == Direction.None)
            {
                return new Rectangle(width * (int)input.prevDirection, 0, width, characterHeight);
            }

            return new Rectangle(width * (int)direction, 0, width, characterHeight);
        }
    }
}
